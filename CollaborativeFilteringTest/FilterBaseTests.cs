using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CollaborativeFilteringTest
{
    [TestClass]
    public class FilterBaseTests
    {
        
        private CollaborativeFiltering.Analyzer _analyzer;

        public FilterBaseTests()
        {
            _analyzer = new CollaborativeFiltering.Analyzer();
            _analyzer.AddMark(new CollaborativeFiltering.Mark(1, 2, 4));
            _analyzer.AddMark(new CollaborativeFiltering.Mark(1, 3, 5));
            _analyzer.AddMark(new CollaborativeFiltering.Mark(2, 2, 4));
            _analyzer.AddMark(new CollaborativeFiltering.Mark(8, 4, 4));
            _analyzer.AddMark(new CollaborativeFiltering.Mark(3, 10, 4));
        }
        
        [TestMethod]
        public void TestAddingUsersThatHaveMarkForItemWithExclude()
        {
            var resultList = _analyzer.UsersThatHaveMarkForItem(1, 2);

            var needList = new List<int>();
            needList.Add(3);

            CollectionAssert.AreEqual(needList, resultList);
        }

        [TestMethod]
        public void TestAddingUsersThatHaveMarkForItemWithoutExclude()
        {
            var resultList = _analyzer.UsersThatHaveMarkForItem(1);

            var needList = new List<int>();
            needList.Add(2);
            needList.Add(3);

            CollectionAssert.AreEqual(needList, resultList);
        }


        [TestMethod]
        public void TestAdding()
        {
            Assert.AreEqual(5, _analyzer.CountOfMarks); 
        }

        [TestMethod]
        public void TestAverageItemRate()
        {
            Assert.AreEqual(4.5, _analyzer.GetAverageItemRate(1));
        }

        [TestMethod]
        public void TestAverageUserRate()
        {
            Assert.AreEqual(4, _analyzer.GetAverageUserRate(2));
        }

        [TestMethod]
        public void UndefinedAverage()
        {
            _analyzer.GetAverageUserRate(20);
        }


        [TestMethod]
        public void GetUsersAndItemsList()
        {
            _analyzer.GetUsersList();
            _analyzer.GetItemsList();
        }

        [TestMethod]
        public void TestIntersection()
        {
            var intersectedIds  = _analyzer.GetIntersectedItems(2, 3);

            var needList = new List<int>();
            needList.Add(1);
           
            CollectionAssert.AreEqual(needList, intersectedIds);
        }

        [TestMethod]
        public void TestIntersectionWithRates()
        {
            var intersection = _analyzer.GetIntersectedItemsWithRates(2, 3);

            Assert.AreEqual(1, intersection.Count);
            Assert.AreEqual(true, intersection.ContainsKey(1));
            Assert.AreEqual(Tuple.Create<int, int>(4, 5), intersection[1]);
        }
    }
}
