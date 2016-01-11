using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using KPICore;

namespace CollaborativeAgent
{
    public class Program : iKPIC_subscribeHandler
    {
        public static Dictionary<int, List<BaseEntities.AnswerLine>> VariantInformation
        { get; private set; }

        private static BaseState GlobalState = new BaseState();


        static public bool CheckVariantResults(int variantId, List<Tuple<int, int, int>> userItemRates)
        {
            if (!VariantInformation.ContainsKey(variantId))
                return false;

            List<BaseEntities.AnswerLine> answers = new List<BaseEntities.AnswerLine>();
            foreach (var i in userItemRates)
                answers.Add(new BaseEntities.AnswerLine() { user = i.Item1, item = i.Item2, rate = i.Item3 });

            List<BaseEntities.AnswerLine> needAnswers = VariantInformation[variantId];

            bool newVariant = answers.Any(uir => !needAnswers.Contains(uir));
            bool lostVariant = needAnswers.Any(uir => !answers.Contains(uir));

            return !newVariant && !lostVariant;
        }

        static void ParseIndexLine(string sourceLine, string[] csvLine)
        {
            if (csvLine.Length != 2)
                throw new Exception(String.Format("String {0} not valid 2-part csv string, splitted by ','", sourceLine));

            int variantNo;

            if (!int.TryParse(csvLine[0], out variantNo))
                throw new Exception(String.Format("Could not parse {0} as integer (variant) on string {1}", csvLine[0], sourceLine));

            if (variantNo <= 0)
                throw new Exception(String.Format("Variant cannot be less or equal 0 on line {0}", sourceLine));

            if (VariantInformation.ContainsKey(variantNo))
                throw new Exception(String.Format("Variant {0} already exists", variantNo));

            string answerFile = csvLine[1];

            if (!File.Exists(answerFile))
                throw new Exception(String.Format("Answer file {0} on line {1} not exists", answerFile, sourceLine));


            VariantInformation[variantNo] = new List<BaseEntities.AnswerLine>();

            try
            {
                string[] answers = File.ReadAllLines(answerFile);

                foreach (string answer in answers)
                {
                    int user = 0, item = 0, rate = 0;
                    string[] lineParts = answer.Split(',');
                    if (lineParts.Length != 3 || 
                        !int.TryParse(lineParts[0], out user) ||
                        !int.TryParse(lineParts[1], out item) ||
                        !int.TryParse(lineParts[2], out rate)
                        )
                        Console.Error.WriteLine("WARNING: ignoring line {0}. Cannt parse", answer);
                    else
                        VariantInformation[variantNo].Add(new BaseEntities.AnswerLine { item = item, rate = rate, user = user });
                }

            }
            catch (Exception exception)
            {
                throw new Exception(String.Format("Cannot open answer file {0}, error : {1}", answerFile, exception.Message));
            }

            if (VariantInformation[variantNo].Count == 0)
                Console.Error.WriteLine("WARNING: variant {0} not contains any answers", variantNo);

        }

        static void CheckArguments(string[] args)
        {
            if (args.Length < 1)
                throw new Exception(String.Format("Call this program with 1 argument: <index_file>"));

            
            if (!File.Exists(args[0]))
                throw new Exception(String.Format("Index file {0} can't be found", args[0]));

            try
            {
                string[] lines = File.ReadAllLines(args[0]);
                if (lines.Length < 1)
                    throw new Exception("Index file doesnt contains any lines");

                foreach (string line in lines)
                    ParseIndexLine(line, line.Split(','));

            }
            catch (Exception exception)
            {
                throw new Exception(String.Format("Cannot open index file {0}, error : {1}", args[0], exception.Message));
            }
            
            if (VariantInformation.Count == 0)
                throw new Exception("Cannot start without any variants");

        }

        static void LoadTestData()
        {
            Console.WriteLine("Loading test data...");

            TestVariant variant = new TestVariant(GlobalState);

            Console.WriteLine("IsCorrect = {0}", variant.IsCorrect);

        }

        public Program()
        {

        }

        KPICore.KPICore core;
        BaseState state;

        public void ClearSpace()
        {
            core.remove("any", "any", "any", "uri");
            core.remove("any", "any", "any", "literal");
        }
        public void Start(string[] args)
        {
            VariantInformation = new Dictionary<int, List<BaseEntities.AnswerLine>>();

            Console.Write("HOST>");
            string host = Console.ReadLine();
            Console.Write("PORT>");
            string portString = Console.ReadLine();
            int port;
            Console.Write("SMART SPACE NAME>");
            string smartSpaceName = Console.ReadLine();

            if (!int.TryParse(portString, out port))
            {
                Console.Error.WriteLine("Incorrect port number");
                return;
            }


            core = new KPICore.KPICore(host, port, smartSpaceName);
            state = new BaseState();

            if (!core.join())
            {
                Console.Error.WriteLine("Cannot join smart space. Host: {0}, port: {1}, smart-space-name: {2}", host, port, smartSpaceName);
                return;
            }


            ClearSpace();

            string subscribeKey = core.subscribeRDF("any", "hasVariant", "any", "literal", this);

            try
            {
                CheckArguments(args);
            }
            catch (Exception exc)
            {
                Console.Error.WriteLine("Cannot start agent, error: {0}", exc.Message);
            }            

            ConsoleKey exitKey = ConsoleKey.Escape;
            Console.WriteLine("Agent started. Press {0} for exit", exitKey);
            ConsoleKeyInfo keyinfo;
            do
            {
                keyinfo = Console.ReadKey();

                if (keyinfo.Key == ConsoleKey.T)
                    LoadTestData();
                
            }
            while (keyinfo.Key != ConsoleKey.Escape);

            core.leave();

        }



        static void Main(string[] args)
        {
            Program p = new Program();
            p.Start(args);
        }

        public void kpic_SIBEventHandler(System.Collections.ArrayList newResults, System.Collections.ArrayList obsoleteResults, string subID)
        {
            state.LoadFromSIB(core);
        }

        public void kpic_SIBEventHandlerSPARQL(SPARQLResults newResults, SPARQLResults obsoleteResults, string subID)
        {
            state.LoadFromSIB(core);
        }
    }
}
