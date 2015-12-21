using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollaborativeFiltering
{
    interface IAnalyzer
    {
        double GetAverageItemRate(int itemId);
        double GetAverageUserRate(int userId);
        List<int> GetIntersectedItems(int userA, int userB);
    }
}
