using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollaborativeFiltering
{
    public class Mark
    {
        private Int32 _item;
        private Int32 _user;
        private Int16 _rate;

        public Int32 Item { get { return _item;  }}
        public Int32 User { get { return _user; } }
        public Int16 Rate { get { return _rate; } }


        public Mark(Int32 item, Int32 user, Int16 rate)
        {
            _item = item;
            _user = user;
            _rate = rate;
        }

        public Mark(string dataline)
        {
            string[] words = dataline.Split(',');
            if (words.Length < 3) return;

            _user = int.Parse(words[0]);
            _item = int.Parse(words[1]);
            _rate = Math.Min(Int16.Parse(words[2]), (Int16)5);
        }

        public override string ToString()
        {
            return string.Format("item = {0}, user = {1}, rate = {2}", Item, User, Rate);
        }
    }
}
