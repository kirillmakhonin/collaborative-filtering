using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollaborativeFiltering
{
    public class Analyzer : BaseAnalyzer, IAnalyzer
    {
        private List<Mark> _marks;
        

        public Analyzer(List<Mark> marks = null)
        {
            _marks = marks ?? new List<Mark>();
        }

        public void AddMark(Mark mark){
            _marks.Add(mark);
        }

        public long CountOfMarks
        {
            get { return _marks.Count; }
        }

        public List<int> GetIntersectedItems(int userA, int userB){
           Dictionary<int, bool> idsByUserA = _marks.
                Where(m => m.User == userA).ToDictionary(m => m.Item, m => false);

            foreach (int item in _marks.Where(m => m.User == userB).Select(m => m.Item))
                if (idsByUserA.ContainsKey(item))
                    idsByUserA[item] = true;

            return idsByUserA.Where(id => id.Value).Select(p => p.Key).ToList();
        }

        public delegate Dictionary<int, Tuple<int, int>> GetIntersectedDelegate(int a, int b);


        public Dictionary<int, Tuple<int, int>> GetIntersectedItemsWithRates(int userA, int userB)
        {
            Dictionary<int, Tuple<int, int>> idsByUserA = _marks.
                 Where(m => m.User == userA).ToDictionary(m => m.Item, m => new Tuple<int, int>(m.Rate, 0));

            foreach (Mark item in _marks.Where(m => m.User == userB))
                if (idsByUserA.ContainsKey(item.Item))
                    idsByUserA[item.Item] = Tuple.Create(idsByUserA[item.Item].Item1, (int)item.Rate);

            return idsByUserA.Where(it => it.Value.Item2 != 0).ToDictionary(m => m.Key, m => m.Value);
        }

        public Dictionary<int, Tuple<int, int>> GetIntersectedUsersWithRates(int itemA, int itemB)
        {
            Dictionary<int, Tuple<int, int>> idsByItemA = _marks.
                 Where(m => m.Item == itemA).ToDictionary(m => m.User, m => new Tuple<int, int>(m.Rate, 0));

            foreach (Mark item in _marks.Where(m => m.Item == itemB))
                if (idsByItemA.ContainsKey(item.User))
                    idsByItemA[item.User] = Tuple.Create(idsByItemA[item.User].Item1, (int)item.Rate);

            return idsByItemA.Where(it => it.Value.Item2 != 0).ToDictionary(m => m.Key, m => m.Value);
        }

        public List<int> GetIntersectedUsers(int itemA, int itemB)
        {
            Dictionary<int, bool> idsByitemA = _marks.
                 Where(m => m.Item == itemA).ToDictionary(m => m.User, m => false);

            foreach (int user in _marks.Where(m => m.Item == itemB).Select(m => m.User))
                if (idsByitemA.ContainsKey(user))
                    idsByitemA[user] = true;

            return idsByitemA.Where(id => id.Value).Select(p => p.Key).ToList();
        }

        public delegate double GetAverage(int id);

        public double GetAverageItemRate(int id)
        {
            var rates = _marks.Where(m => m.Item == id && m.Rate > 0);
            if (rates.Count() == 0)
                return 0.0;
            return rates.Average(m => m.Rate);
        }

        public double GetAverageUserRate(int id)
        {
            var rates = _marks.Where(m => m.User == id && m.Rate > 0);
            if (rates.Count() == 0)
                return 0.0;
            return rates.Average(m => m.Rate);
        }

        public List<int> UsersThatHaveMarkForItem(int itemId, int excludeUserId = -1){
            var ret = _marks.Where(m => m.Item == itemId && m.User != excludeUserId);
            if (ret.Count() == 0)
                return new List<int>();

            return ret.Select(m => m.User).Distinct().ToList();
        }


        public override List<int> GetRecommendedItems(int userId, BaseAnalyzer.FilteringType filter)
        {
            throw new NotImplementedException();
        }

        public override List<int> GetUsersList()
        {
            return _marks.Select(m => m.User).Distinct().ToList();
        }

        public override List<int> GetItemsList()
        {
            return _marks.Select(m => m.Item).Distinct().ToList();
        }

        public void ACPCAnalyzer(Dictionary<Tuple<int, int>, double> holder, 
            int a, 
            int b, 
            GetIntersectedDelegate intersectedDel,
            GetAverage averageDel,
            bool oneAverage = false,
            bool simmetricKey = true)
        {

                var key = KeyOf(a, b, simmetricKey);
                if (key == null || holder.ContainsKey(key))
                    return;

                var intersected = intersectedDel(a, b);

                if (intersected.Count == 0)
                {
                    holder[key] = -1;
                    return;
                }

                double avA = averageDel(a), avB = averageDel(b);
                if (oneAverage)
                    avB = avA;

                double result =
                 intersected.Sum(it => (it.Value.Item1 - avA) * (it.Value.Item2 - avB)) /
                     Math.Sqrt(
                        intersected.Sum(it => Math.Pow(it.Value.Item1 - avA, 2)) *
                        intersected.Sum(it => Math.Pow(it.Value.Item2 - avB, 2))
                     );

                if (double.IsNaN(result)) result = 1;

                holder[key] = result;

        }

        private Dictionary<int, Dictionary<int, int>> _rateTable;

        private void FillRateDictionary()
        {
            _rateTable = new Dictionary<int, Dictionary<int, int>>();

            var items = GetItemsList();

            foreach (var user in GetUsersList())
            {
                _rateTable[user] = new Dictionary<int, int>();
                foreach (var item in items)
                    _rateTable[user][item] = 0;
            }

            foreach (var mark in _marks)
                _rateTable[mark.User][mark.Item] = -mark.Rate;


        }

        public void InitCoefficients()
        {
            var usersList = GetUsersList();
            var itemsList = GetItemsList();

            #region "Coeffs: PC(u)"            
            foreach (int a in usersList)
                foreach (int b in usersList)
                    ACPCAnalyzer(pc_users_, a, b, GetIntersectedItemsWithRates, GetAverageUserRate);
            #endregion

            #region "Coeffs: PC(i)"
            foreach (int a in itemsList)
                foreach (int b in itemsList)
                    ACPCAnalyzer(pc_items_, a, b, GetIntersectedUsersWithRates, GetAverageItemRate);
            #endregion


            #region "Coeffs: PC(i)"
            foreach (int a in itemsList)
                foreach (int b in itemsList)
                    ACPCAnalyzer(ac_items_, a, b, GetIntersectedUsersWithRates, GetAverageUserRate, true, false);
            #endregion


            FillRateDictionary();

        }


        private List<int> GetMaxRelevantListOfUsers(int user)
        {
            return pc_users_.Where(pc => (pc.Key.Item1 == user || pc.Key.Item2 == user) && pc.Value >= 0.5)
                .Select(pc => pc.Key.Item1 == user ? pc.Key.Item2 : pc.Key.Item1).ToList();
        }

        private List<int> GetMaxRelevantListOfItems(int item)
        {
            return pc_items_.Where(pc => (pc.Key.Item1 == item || pc.Key.Item2 == item) && pc.Value >= 0.5)
                .Select(pc => pc.Key.Item1 == item ? pc.Key.Item2 : pc.Key.Item1).ToList();
        }

        private List<int> GetUsersThatRateItem(int item, int excluded = -1)
        {
            return _marks.Where(m => m.Item == item && m.User != excluded).Select(m => m.User).ToList();
        }

        private int GetUserMark(int user, int item)
        {
            var mark = _marks.FirstOrDefault(m => m.User == user && m.Item == item);
            return mark != null ? mark.Rate : 0;
        }

        /*
         * return 
         *  user => { item: rate , item: rate,... }
         */
        public Dictionary<int, Dictionary<int, int>> GetMarks(FilteringType filter)
        {
            var rate = _rateTable;
            var items = GetItemsList();


            if (filter == FilteringType.UserBased)
            {
                foreach (var user in rate.Keys)
                {
                    var averageUserRate = GetAverageUserRate(user);
                    List<int> maxCorellatedUsers = GetMaxRelevantListOfUsers(user);

                    foreach (var item in items)
                    {
                        // Оценка уже стоит?
                        if (rate[user][item] < 0)
                            continue;

                        var usersThatHasMark = GetUsersThatRateItem(item, user);

                        var usersForProcessing = maxCorellatedUsers.Intersect(usersThatHasMark);

                        if (!usersForProcessing.Any())
                            continue;

                        var rateValue = averageUserRate +
                                   usersForProcessing.Sum(
                                       with =>
                                           pc_items_[KeyOf(user, with)] * (GetUserMark(user, item) - GetAverageUserRate(with)))
                                           / usersForProcessing.Sum(
                                       with =>
                                           pc_items_[KeyOf(user, with)]);

                        rate[user][item] = (int)Math.Round(rateValue);
                    }
                }
            }

            return rate;
        }
    }
}
