using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollaborativeFiltering
{
    public abstract class BaseAnalyzer
    {
        public enum FilteringType { UserBased = 0, ItemBased = 1 }

        abstract public List<int> GetRecommendedItems(int userId, FilteringType filter);

        abstract public List<int> GetUsersList();
        abstract public List<int> GetItemsList();


        protected Dictionary<Tuple<int, int>, double> pc_users_ = new Dictionary<Tuple<int, int>, double>();
        protected Dictionary<Tuple<int, int>, double> pc_items_ = new Dictionary<Tuple<int, int>, double>();
        protected Dictionary<Tuple<int, int>, double> ac_items_ = new Dictionary<Tuple<int, int>, double>();

        public Dictionary<Tuple<int, int>, double> PC_users
        {
            get { return pc_users_; }
        }

        public Dictionary<Tuple<int, int>, double> PC_items
        {
            get { return pc_items_; }
        }

        public Dictionary<Tuple<int, int>, double> AC_items
        {
            get { return ac_items_; }
        }

        public static Tuple<int, int> KeyOf(int a, int b, bool simmetricKey = true)
        {
            if (a == b)
                return null;

            if (a > b && simmetricKey)
                return Tuple.Create(b, a);

            return Tuple.Create(a, b);
        }

        public Dictionary<int, List<int>> GetRecommendationsDictionary(FilteringType filter)
        {
            var ret = new Dictionary<int, List<int>>();

            foreach (int user in GetUsersList())
                ret[user] = GetRecommendedItems(user, filter);

            return ret;
        }
    }
}
