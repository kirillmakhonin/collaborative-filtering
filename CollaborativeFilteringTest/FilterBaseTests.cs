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
            
        }


        [TestMethod]
        public void TestAdding()
        {
            _analyzer.AddMark(new CollaborativeFiltering.Mark(1, 2, 4));
            _analyzer.AddMark(new CollaborativeFiltering.Mark(1, 3, 4));
            _analyzer.AddMark(new CollaborativeFiltering.Mark(2, 2, 4));
            _analyzer.AddMark(new CollaborativeFiltering.Mark(1, 4, 4));
            _analyzer.AddMark(new CollaborativeFiltering.Mark(3, 10, 4));
        }


        [TestMethod]
        public void TestIntersection()
        {
            var intersectedIds  = _analyzer.GetIntersectedItems(1, 2);

            var needList = new List<int>();
            needList.Add(2);

            Console.WriteLine(intersectedIds);

            Assert.AreEqual(intersectedIds, needList);
        }
    }
}
