using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CollaborativeFilteringTest
{
    [TestClass]
    public class LabTest
    {
        private CollaborativeFiltering.Analyzer _analyzer;

        public LabTest()
        {
            _analyzer = new CollaborativeFiltering.Analyzer();
            
            // i 1
            _analyzer.AddMark(new CollaborativeFiltering.Mark(1, 1, 2));
            _analyzer.AddMark(new CollaborativeFiltering.Mark(1, 2, 3));
            _analyzer.AddMark(new CollaborativeFiltering.Mark(1, 5, 2));
            _analyzer.AddMark(new CollaborativeFiltering.Mark(1, 6, 2));

            // i 2
            _analyzer.AddMark(new CollaborativeFiltering.Mark(2, 1, 4));
            _analyzer.AddMark(new CollaborativeFiltering.Mark(2, 2, 4));
            _analyzer.AddMark(new CollaborativeFiltering.Mark(2, 4, 4));
            _analyzer.AddMark(new CollaborativeFiltering.Mark(2, 6, 4));

            // i 3
            _analyzer.AddMark(new CollaborativeFiltering.Mark(3, 3, 5));
            _analyzer.AddMark(new CollaborativeFiltering.Mark(3, 5, 5));

            // i 4
            _analyzer.AddMark(new CollaborativeFiltering.Mark(4, 1, 5));
            _analyzer.AddMark(new CollaborativeFiltering.Mark(4, 3, 5));
            _analyzer.AddMark(new CollaborativeFiltering.Mark(4, 4, 4));
            _analyzer.AddMark(new CollaborativeFiltering.Mark(4, 5, 5));
            _analyzer.AddMark(new CollaborativeFiltering.Mark(4, 6, 5));

            // i 5
            _analyzer.AddMark(new CollaborativeFiltering.Mark(5, 2, 5));
            _analyzer.AddMark(new CollaborativeFiltering.Mark(5, 3, 5));
            _analyzer.AddMark(new CollaborativeFiltering.Mark(5, 4, 5));
            _analyzer.AddMark(new CollaborativeFiltering.Mark(5, 5, 2));
            _analyzer.AddMark(new CollaborativeFiltering.Mark(5, 6, 4));

            // i 6
            _analyzer.AddMark(new CollaborativeFiltering.Mark(6, 2, 1));
            _analyzer.AddMark(new CollaborativeFiltering.Mark(6, 4, 4));

            _analyzer.InitCoefficients();
        }

        [TestMethod]
        public void TestPCu()
        {
            var pc_u = _analyzer.PC_users;
            var pc_i = _analyzer.PC_items;
        }
    }
}
