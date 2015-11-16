using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollaborativeFiltering
{
    public class Analyzer : IAnalyzer
    {
        private List<Mark> _marks;

        public Analyzer(List<Mark> marks = null)
        {
            _marks = marks ?? new List<Mark>();
        }

        public void AddMark(Mark mark){
            _marks.Add(mark);
        }

        public double? GetAverageItemRating(int itemId){
            return null;
        }

        public double? GetAverageUserRating(int userId){
            return null;
        }

        public List<int> GetIntersectedItems(int userA, int userB){
           Dictionary<int, bool> idsByUserA = _marks.
                Where(m => m.User == userA).ToDictionary(m => m.Item, m => false);

            foreach (int item in _marks.Where(m => m.User == userB).Select(m => m.Item))
                if (idsByUserA.ContainsKey(item))
                    idsByUserA[item] = true;

            return idsByUserA.Where(id => id.Value).Select(p => p.Key).ToList();
        }

    }
}
