using System;
using CollaborativeFiltering;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace CollaborativeFilteringTest
{
    [TestClass]
    public class BigLabTest
    {
        private CollaborativeFiltering.Analyzer _analyzer;

        public BigLabTest()
        {
            _analyzer = new CollaborativeFiltering.Analyzer();

            _analyzer.AddMark(new CollaborativeFiltering.Mark(1, 1, 2));
            _analyzer.AddMark(new CollaborativeFiltering.Mark(2, 1, 4));
            _analyzer.AddMark(new CollaborativeFiltering.Mark(4, 1, 5));
            _analyzer.AddMark(new CollaborativeFiltering.Mark(7, 1, 3));
            _analyzer.AddMark(new CollaborativeFiltering.Mark(9, 1, 2));
            _analyzer.AddMark(new CollaborativeFiltering.Mark(1, 2, 3));
            _analyzer.AddMark(new CollaborativeFiltering.Mark(2, 2, 4));
            _analyzer.AddMark(new CollaborativeFiltering.Mark(5, 2, 5));
            _analyzer.AddMark(new CollaborativeFiltering.Mark(6, 2, 1));
            _analyzer.AddMark(new CollaborativeFiltering.Mark(8, 2, 4));
            _analyzer.AddMark(new CollaborativeFiltering.Mark(3, 3, 5));
            _analyzer.AddMark(new CollaborativeFiltering.Mark(4, 3, 5));
            _analyzer.AddMark(new CollaborativeFiltering.Mark(5, 3, 5));
            _analyzer.AddMark(new CollaborativeFiltering.Mark(7, 3, 4));
            _analyzer.AddMark(new CollaborativeFiltering.Mark(9, 3, 4));
            _analyzer.AddMark(new CollaborativeFiltering.Mark(2, 4, 4));
            _analyzer.AddMark(new CollaborativeFiltering.Mark(4, 4, 4));
            _analyzer.AddMark(new CollaborativeFiltering.Mark(5, 4, 5));
            _analyzer.AddMark(new CollaborativeFiltering.Mark(6, 4, 4));
            _analyzer.AddMark(new CollaborativeFiltering.Mark(8, 4, 4));
            _analyzer.AddMark(new CollaborativeFiltering.Mark(10, 4, 4));
            _analyzer.AddMark(new CollaborativeFiltering.Mark(1, 5, 2));
            _analyzer.AddMark(new CollaborativeFiltering.Mark(3, 5, 5));
            _analyzer.AddMark(new CollaborativeFiltering.Mark(4, 5, 5));
            _analyzer.AddMark(new CollaborativeFiltering.Mark(5, 5, 2));
            _analyzer.AddMark(new CollaborativeFiltering.Mark(9, 5, 3));
            _analyzer.AddMark(new CollaborativeFiltering.Mark(1, 6, 2));
            _analyzer.AddMark(new CollaborativeFiltering.Mark(2, 6, 4));
            _analyzer.AddMark(new CollaborativeFiltering.Mark(4, 6, 5));
            _analyzer.AddMark(new CollaborativeFiltering.Mark(5, 6, 4));
            _analyzer.AddMark(new CollaborativeFiltering.Mark(7, 6, 5));
            _analyzer.AddMark(new CollaborativeFiltering.Mark(8, 6, 4));
            _analyzer.AddMark(new CollaborativeFiltering.Mark(3, 7, 4));
            _analyzer.AddMark(new CollaborativeFiltering.Mark(6, 7, 3));
            _analyzer.AddMark(new CollaborativeFiltering.Mark(9, 7, 4));
            _analyzer.AddMark(new CollaborativeFiltering.Mark(1, 8, 3));
            _analyzer.AddMark(new CollaborativeFiltering.Mark(2, 8, 3));
            _analyzer.AddMark(new CollaborativeFiltering.Mark(4, 8, 3));
            _analyzer.AddMark(new CollaborativeFiltering.Mark(8, 8, 5));
            _analyzer.AddMark(new CollaborativeFiltering.Mark(10, 8, 3));
            _analyzer.AddMark(new CollaborativeFiltering.Mark(2, 9, 4));
            _analyzer.AddMark(new CollaborativeFiltering.Mark(4, 9, 4));
            _analyzer.AddMark(new CollaborativeFiltering.Mark(5, 9, 2));
            _analyzer.AddMark(new CollaborativeFiltering.Mark(7, 9, 4));
            _analyzer.AddMark(new CollaborativeFiltering.Mark(9, 9, 5));
            _analyzer.AddMark(new CollaborativeFiltering.Mark(1, 10, 2));
            _analyzer.AddMark(new CollaborativeFiltering.Mark(3, 10, 5));
            _analyzer.AddMark(new CollaborativeFiltering.Mark(6, 10, 2));


            _analyzer.InitCoefficients();
        }

        [TestMethod]
        public void TestPCu()
        {
            Assert.AreEqual(0.9230769, _analyzer.PC_users[BaseAnalyzer.KeyOf(1, 2)], 0.00001);
        }


        [TestMethod]
        public void TestPCi()
        {
            Assert.AreEqual(-0.5976, _analyzer.PC_items[BaseAnalyzer.KeyOf(1, 2)], 0.001);
        }

        [TestMethod]
        public void TestRateMatrixUserBased()
        {
            var matrix = _analyzer.GetMarks(BaseAnalyzer.FilteringType.UserBased);
            Assert.AreEqual(3, matrix[1][5]);
        }

        [TestMethod]
        public void TestRateMatrixItemBased()
        {
            var matrix = _analyzer.GetMarks(BaseAnalyzer.FilteringType.ItemBased);
            Assert.AreEqual(4, matrix[1][5]);
        }

        [TestMethod]
        public void TestGetMaxRelevantListOfUsers()
        {
            var resultList = _analyzer.GetMaxRelevantListOfUsers(1);

            var needList = new List<int>();
            needList.Add(2);
            needList.Add(3);
            needList.Add(5);
            needList.Add(6);
            needList.Add(10);

            CollectionAssert.AreEqual(needList, resultList);
        }

        [TestMethod]
        public void TestGetUserMark()
        {
            Assert.AreEqual(4, _analyzer.GetUserMark(1, 2));
        }
    }
}
