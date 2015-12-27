using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollaborativeAgent
{
    public class BaseState
    {
        
        /*
         * Загруженны ли данные из smart-m3 
         */
        public bool IsLoaded
        { get; private set; }

        public List<Tuple<BaseEntities.Mark, int>> HasRates
        { get; private set; }

        public List<Tuple<BaseEntities.Mark, int>> HasItems
        { get; private set; }

        public List<Tuple<BaseEntities.Mark, int>> HasUsers
        { get; private set; }

        public List<Tuple<BaseEntities.Mark, BaseEntities.Student>> UploadedByStudent
        { get; private set; }

        public List<Tuple<BaseEntities.Student, int>> HasVariant
        { get; private set; }

        public List<Tuple<BaseEntities.Student, bool>> IsCorrect
        { get; private set; }

        public List<StudentSpace> StudentSpaces
        { get; private set; }

        public BaseState()
        {
            IsLoaded = false;
            HasRates = new List<Tuple<BaseEntities.Mark, int>>();
            HasItems = new List<Tuple<BaseEntities.Mark, int>>();
            HasUsers = new List<Tuple<BaseEntities.Mark, int>>();
            UploadedByStudent = new List<Tuple<BaseEntities.Mark, BaseEntities.Student>>();
            HasVariant = new List<Tuple<BaseEntities.Student, int>>();
            IsCorrect = new List<Tuple<BaseEntities.Student, bool>>();
            StudentSpaces = new List<StudentSpace>();
        }

        public void Clear()
        {
            HasRates = new List<Tuple<BaseEntities.Mark, int>>();
            HasItems = new List<Tuple<BaseEntities.Mark, int>>();
            HasUsers = new List<Tuple<BaseEntities.Mark, int>>();
            UploadedByStudent = new List<Tuple<BaseEntities.Mark, BaseEntities.Student>>();
            HasVariant = new List<Tuple<BaseEntities.Student, int>>();
            IsCorrect = new List<Tuple<BaseEntities.Student, bool>>();
            StudentSpaces = new List<StudentSpace>();
        }


        public void SetState(List<Tuple<BaseEntities.Mark, int>> rates,
            List<Tuple<BaseEntities.Mark, int>> items,
            List<Tuple<BaseEntities.Mark, int>> users,
            List<Tuple<BaseEntities.Mark, BaseEntities.Student>> uploadedBy,
            List<Tuple<BaseEntities.Student, int>> hasVariant)
        {
            HasRates = rates;
            HasItems = items;
            HasUsers = users;
            UploadedByStudent = uploadedBy;
            HasVariant = hasVariant;
        }

        public void LoadFromSIB()
        {
            throw new NotImplementedException("Loading from smart-m3 is not supported");
        }

        public void AnalyzeTriplets()
        {
            /*
             * user, item, rate
             */
            Dictionary<BaseEntities.Mark, Tuple<int, int, int>> markInfo = new Dictionary<BaseEntities.Mark, Tuple<int, int, int>>();

            foreach (var user in HasUsers)
            {
                if (!markInfo.ContainsKey(user.Item1))
                    markInfo[user.Item1] = new Tuple<int, int, int>(user.Item2, -1, -1);
                markInfo[user.Item1] = new Tuple<int, int, int>(user.Item2, markInfo[user.Item1].Item2, markInfo[user.Item1].Item3);
            }

            foreach (var item in HasItems)
            {
                if (!markInfo.ContainsKey(item.Item1))
                    markInfo[item.Item1] = new Tuple<int, int, int>(-1, item.Item2, -1);
                markInfo[item.Item1] = new Tuple<int, int, int>(markInfo[item.Item1].Item1, item.Item2, markInfo[item.Item1].Item3);
            }

            foreach (var rate in HasRates)
            {
                if (!markInfo.ContainsKey(rate.Item1))
                    markInfo[rate.Item1] = new Tuple<int,int,int>(-1, -1, rate.Item2);
                markInfo[rate.Item1] = new Tuple<int, int, int>(markInfo[rate.Item1].Item1, markInfo[rate.Item1].Item2, rate.Item2);
            }

            foreach (var hasVariantTriplet in HasVariant)
            {
                BaseEntities.Student student = hasVariantTriplet.Item1;
                int variantId = hasVariantTriplet.Item2;
                // Load marks which uploaded by student
                List<BaseEntities.Mark> studentVariants = UploadedByStudent.Where(ubs => ubs.Item2 == student).Select(ubs => ubs.Item1).ToList();
                var studentMarks = markInfo.Where(mi => studentVariants.Contains(mi.Key) && mi.Value.Item1 >= 0 && mi.Value.Item2 >= 0 && mi.Value.Item3 >= 0);

                if (studentVariants.Count() == 0 || studentMarks.Count() == 0)
                    continue;

                StudentSpace space = new StudentSpace(student, variantId);
                
                foreach (var mark in studentMarks)
                    space.AddMark(mark.Value.Item1, mark.Value.Item2, mark.Value.Item3);

                StudentSpaces.Add(space);                    

            }

            IsCorrect.Clear();
            foreach (var space in StudentSpaces)
            {
                space.Analyze();
                IsCorrect.Add(new Tuple<BaseEntities.Student, bool>(space.Student, space.IsCorrect));
                
            }

        }
    }
}
