using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DatasetSplitter
{
    class Program
    {

        public class DatasetItem
        {
            public int user;
            public int item;
            public int mark;

            public bool origin;

            public DatasetItem(string baseLine)
            {
                string[] words = baseLine.Split(',');
                if (words.Length < 3) return;

                user = int.Parse(words[0]);
                item = int.Parse(words[1]);
                mark = Math.Min(int.Parse(words[2]), 5);

                origin = false;
            }

            public string ToString(int removeOffset = 0)
            {
                return String.Format("{0},{1},{2}", user - removeOffset, item - removeOffset, mark);
            }

            public override string ToString()
            {
                return this.ToString();
            }

            public bool IdsBetween(int a, int b)
            {
                return user >= a && user < b && item >= a && item < b && mark > 0;
            }
            
        }


        private string _baseSaveFile;
        private List<DatasetItem> _items;
        private int _idsInDataset;

        public int Count
        {
            get { return _items.Count; }
        }

        Program(string datasetFile, int idsInDataset)
        {
            _baseSaveFile = datasetFile.EndsWith(".csv") ? datasetFile.Substring(0, datasetFile.Length - 3) : datasetFile;
            _idsInDataset = idsInDataset;
            _items = new List<DatasetItem>();

            if (!File.Exists(datasetFile) || idsInDataset < 1)
                return;

            using (TextReader reader = File.OpenText(datasetFile))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                    _items.Add(new DatasetItem(line));
                
            }



        }


        public void FilterMarks()
        {
            Dictionary<int, List<int>> userMarks = new Dictionary<int, List<int>>();
            Dictionary<int, List<int>> itemMarks = new Dictionary<int, List<int>>();
            

            foreach (DatasetItem item in _items)
            {
                if (!userMarks.ContainsKey(item.user))
                    userMarks[item.user] = new List<int>();
                userMarks[item.user].Add(item.mark);

                if (!itemMarks.ContainsKey(item.item))
                    itemMarks[item.item] = new List<int>();
                itemMarks[item.item].Add(item.mark);
            }


            List<int> itemsForRemoving = (from itemMark in itemMarks where !(itemMark.Value.Count > 0 && itemMark.Value.Distinct().Count() > 1) select itemMark.Key).ToList();
            List<int> usersForRemoving = (from userMark in userMarks where !(userMark.Value.Count > 0 && userMark.Value.Distinct().Count() > 1) select userMark.Key).ToList();

            _items.RemoveAll(dsi => itemsForRemoving.Contains(dsi.item) || usersForRemoving.Contains(dsi.user));
        }

        public bool Save()
        {
            int maxId = _items.Max(m => Math.Max(m.user, m.item));
            int countOfChunks = (int)Math.Ceiling(maxId*1.0/_idsInDataset);


            for (int i = 0; i < countOfChunks; i++)
            {
                var filteredItems = _items.Where(item => item.IdsBetween(i*_idsInDataset, (i + 1)*_idsInDataset))
                    .Select(m => m.ToString(i*_idsInDataset));

                if (filteredItems.Any())
                    File.WriteAllText(_baseSaveFile + i.ToString() + ".csv", string.Join("\n", filteredItems));
            }
                

            return true;
        }


        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Call this program with 2 arguments: <dataset_file> and <ids_in_datasets>");
                return;
            }

            Program p = new Program(args[0], Int32.Parse(args[1]) + 1);

            Console.WriteLine("Before filtering: {0}", p.Count);
            p.FilterMarks();
            Console.WriteLine("After filtering: {0}", p.Count);
            
            p.Save();

            Console.WriteLine("Okay");

        }
    }
}
