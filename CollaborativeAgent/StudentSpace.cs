using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollaborativeAgent
{
    public class StudentSpace
    {
        /*
         *  Student marks as a tuple like [user, item, rate]
         *  Need to check with etalon
         */ 
        public List<Tuple<int, int, int>> StudentMarks
        { get; private set; }

        /*
         * Is processed by user 
         */
        public bool IsRated
        { get; private set; }

        /*
         * Is user answer is correct
         */
        public bool IsCorrect
        { get; private set; }

        /*
         * Variant int
         */
        public int VariantID
        { get; private set; }

        /*
         * Student info
         */
        public BaseEntities.Student Student
        { get; private set; }


        public StudentSpace(BaseEntities.Student student, int variantId)
        {
            IsRated = false;
            IsCorrect = false;
            VariantID = variantId;
            Student = student;
            StudentMarks = new List<Tuple<int, int, int>>();
        }

        public void Analyze()
        {
            try
            {
                IsCorrect = Program.CheckVariantResults(VariantID, StudentMarks);
                IsRated = true;
            }
            catch (Exception exceeption)
            {
                IsCorrect = false;
                IsRated = false;
            }
        }

        public void AddMark(int user, int item, int rate)
        {
            StudentMarks.Add(new Tuple<int, int, int>(user, item, rate));
        }

        private void TryAnalyze()
        {

        }



    }
}
