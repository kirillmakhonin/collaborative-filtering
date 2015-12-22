using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollaborativeFilteringConsole
{
    class Program
    {
        static void CheckArguments(string[] args)
        {
            if (args.Length < 2)
                throw new Exception(String.Format("Call this program with 2 arguments: <dataset_file> and <mode: user or item> and optionaly [<output_file>]"));

            if (!File.Exists(args[0]))
                throw new Exception(String.Format("File {0} can't be found", args[0]));

            string mode = args[1].ToLower();
            if (mode != "user" && mode != "item")
                throw new Exception(String.Format("Format can be only 'user' or 'item'"));

            try
            {
                File.OpenRead(args[0]);
            }
            catch (Exception exception)
            {
                throw new Exception(String.Format("Cannot open file {0}, error : {1}", args[0], exception.Message));
            }

        }

        static void PushMarksToAnalyzer(string fileName, ref CollaborativeFiltering.Analyzer analyzer)
        {
            using (TextReader reader = File.OpenText(fileName))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    int user, item, mark;
                    var chunks = line.Split(',');
                    if (
                        chunks.Length != 3 ||
                        !Int32.TryParse(chunks[0], out user) ||
                        !Int32.TryParse(chunks[1], out item) ||
                        !Int32.TryParse(chunks[2], out mark)
                    )
                        continue;

                    analyzer.AddMark(new CollaborativeFiltering.Mark(item, user, (short)mark));
                }
                    

            }

        }

        static CollaborativeFiltering.BaseAnalyzer.FilteringType GetFilter(string arg)
        {
            if (arg == "item")
                return CollaborativeFiltering.BaseAnalyzer.FilteringType.ItemBased;
            if (arg == "user")
                return CollaborativeFiltering.BaseAnalyzer.FilteringType.UserBased;

            throw new Exception("Undefined filter " + arg);
        }

        static void OutputFile(string outputFile, Dictionary<int, Dictionary<int, int>> table)
        {
            List<string> csvStringsToOut = new List<string>();
            foreach (var user in table.Keys)
                foreach (var item in table[user].Keys)
                    if (table[user][item] > 0)
                        csvStringsToOut.Add(String.Format("{0},{1},{2}", user, item, table[user][item]));

            if (csvStringsToOut.Any())
            {
                File.WriteAllText(outputFile, string.Join("\n", csvStringsToOut));
                Console.WriteLine("Saved to file: {0}. Count of predicted marks: {1}", outputFile, csvStringsToOut.Count);
            }
        }

        static void PrintAverages(CollaborativeFiltering.Analyzer analyzer)
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("User averages:");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("|{0,5}|{1,8}|", "user", "av. rate");
            Console.ForegroundColor = ConsoleColor.White;
            foreach (var user in analyzer.GetUsersList())
                Console.WriteLine("|{0,5}|   {1:F3}|", user, analyzer.GetAverageUserRate(user));

            Console.WriteLine("");

            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("Item averages:");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("|{0,5}|{1,8}|", "item", "av. rate");
            Console.ForegroundColor = ConsoleColor.White;
            foreach (var item in analyzer.GetItemsList())
                Console.WriteLine("|{0,5}|   {1:F3}|", item, analyzer.GetAverageItemRate(item));

        }

        static void PrintCoefficientTable(string name, Dictionary<Tuple<int, int>, double> data, List<int> columns, List<int> rows)
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("Table: {0}", name);

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("      ");
            foreach (var col in columns)
                Console.Write("{0,5}|", col);
            Console.WriteLine();

            foreach (var row in rows)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("{0,5}|", row);
                Console.ForegroundColor = ConsoleColor.White;

                foreach (var col in columns)
                {
                    var key = CollaborativeFiltering.BaseAnalyzer.KeyOf(row, col);
                    if (key == null || !data.ContainsKey(key))
                    {
                        Console.Write("     ");

                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.Write("|");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    else
                    {
                        Console.Write("{0}", data[key].ToString(" 0.00;-0.00; 0.00"));

                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.Write("|");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    
                }

                Console.WriteLine(" ");
            }
        }

        static void PrintTable(Dictionary<int, Dictionary<int, int>> table, List<int> users, List<int> items)
        {

            var sortedUsers = new List<int>(users);
            sortedUsers.Sort();

            var sortedItems = new List<int>(items);
            sortedItems.Sort();

            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("RESULT MARKS (white source, red predicted)");

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("   ");
            foreach (var item in sortedItems)
                Console.Write("{0,2}|", item);
            Console.WriteLine();


            foreach (var user in sortedUsers)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("{0,2}|", user);
                Console.ForegroundColor = ConsoleColor.White;

                foreach (var item in sortedItems)
                {
                    var rate = table[user][item];

                    Console.ForegroundColor = rate > 0 ? ConsoleColor.Red : ConsoleColor.White;
                    
                    if (rate == 0)
                        Console.Write("  ");
                    else if (rate > 0)
                        Console.Write(" {0}", rate);
                    else
                        Console.Write(" {0}", -rate);

                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write("|");
                    Console.ForegroundColor = ConsoleColor.White;
                    
                }

                Console.WriteLine(" ");
            }
        }
        
        static int Main(string[] args)
        {
            try {
                CheckArguments(args);

                var analyzer = new CollaborativeFiltering.Analyzer();

                PushMarksToAnalyzer(args[0], ref analyzer);

                analyzer.InitCoefficients();

                var resultTable = analyzer.GetMarks(GetFilter(args[1]));

                PrintAverages(analyzer);
                PrintCoefficientTable("PC (i,j)", analyzer.PC_items, analyzer.GetItemsList(), analyzer.GetItemsList());
                PrintCoefficientTable("PC (u,v)", analyzer.PC_users, analyzer.GetUsersList(), analyzer.GetUsersList());
                PrintTable(resultTable, analyzer.GetUsersList(), analyzer.GetItemsList());

                if (args.Length >= 3)
                    OutputFile(args[2], resultTable);
            }
            catch (Exception exc){
                Console.Error.WriteLine(exc.Message);
                return -1;
            }

            return 0;

        }
    }
}
