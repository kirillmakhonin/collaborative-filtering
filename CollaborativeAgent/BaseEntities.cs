using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollaborativeAgent
{
    public class BaseEntities
    {
        public struct Mark
        {
            public string uri;

            public static bool operator ==(Mark c1, Mark c2)
            {
                return c1.Equals(c2);
            }

            public static bool operator !=(Mark c1, Mark c2)
            {
                return !c1.Equals(c2);
            }

        }

        public struct Student
        {
            public string uri;

            public static bool operator ==(Student c1, Student c2)
            {
                return c1.Equals(c2);
            }

            public static bool operator !=(Student c1, Student c2)
            {
                return !c1.Equals(c2);
            }
        }

        public struct AnswerLine
        {
            public int user;
            public int item;
            public int rate;

            public static bool operator ==(AnswerLine c1, AnswerLine c2)
            {
                return c1.Equals(c2);
            }

            public static bool operator !=(AnswerLine c1, AnswerLine c2)
            {
                return !c1.Equals(c2);
            }
        }
    }
}
