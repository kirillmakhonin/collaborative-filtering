using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollaborativeAgent
{
    public class TestVariant
    {
        public TestVariant(BaseState state)
        {
            List<Tuple<BaseEntities.Mark, int>> rates = new List<Tuple<BaseEntities.Mark,int>>();
            List<Tuple<BaseEntities.Mark, int>> items = new List<Tuple<BaseEntities.Mark,int>>();
            List<Tuple<BaseEntities.Mark, int>> users = new List<Tuple<BaseEntities.Mark,int>>();
            List<Tuple<BaseEntities.Mark, BaseEntities.Student>> uploadedBy = new List<Tuple<BaseEntities.Mark,BaseEntities.Student>>();
            List<Tuple<BaseEntities.Student, int>> hasVariant = new List<Tuple<BaseEntities.Student, int>>();

            BaseEntities.Student me = new BaseEntities.Student() { uri = "student/me" };

            hasVariant.Add(new Tuple<BaseEntities.Student, int>(me, 1));

            #region "Generated. Not edit manual"
            rates.Add(new Tuple<BaseEntities.Mark, int>(new BaseEntities.Mark() { uri = "mark/m1_5_3" }, 3));
            items.Add(new Tuple<BaseEntities.Mark, int>(new BaseEntities.Mark() { uri = "mark/m1_5_3" }, 5));
            users.Add(new Tuple<BaseEntities.Mark, int>(new BaseEntities.Mark() { uri = "mark/m1_5_3" }, 1));
            uploadedBy.Add(new Tuple<BaseEntities.Mark, BaseEntities.Student>(new BaseEntities.Mark() { uri = "mark/m1_5_3" }, me));

            rates.Add(new Tuple<BaseEntities.Mark, int>(new BaseEntities.Mark() { uri = "mark/m1_6_2" }, 2));
            items.Add(new Tuple<BaseEntities.Mark, int>(new BaseEntities.Mark() { uri = "mark/m1_6_2" }, 6));
            users.Add(new Tuple<BaseEntities.Mark, int>(new BaseEntities.Mark() { uri = "mark/m1_6_2" }, 1));
            uploadedBy.Add(new Tuple<BaseEntities.Mark, BaseEntities.Student>(new BaseEntities.Mark() { uri = "mark/m1_6_2" }, me));

            rates.Add(new Tuple<BaseEntities.Mark, int>(new BaseEntities.Mark() { uri = "mark/m1_8_4" }, 4));
            items.Add(new Tuple<BaseEntities.Mark, int>(new BaseEntities.Mark() { uri = "mark/m1_8_4" }, 8));
            users.Add(new Tuple<BaseEntities.Mark, int>(new BaseEntities.Mark() { uri = "mark/m1_8_4" }, 1));
            uploadedBy.Add(new Tuple<BaseEntities.Mark, BaseEntities.Student>(new BaseEntities.Mark() { uri = "mark/m1_8_4" }, me));

            rates.Add(new Tuple<BaseEntities.Mark, int>(new BaseEntities.Mark() { uri = "mark/m1_3_5" }, 5));
            items.Add(new Tuple<BaseEntities.Mark, int>(new BaseEntities.Mark() { uri = "mark/m1_3_5" }, 3));
            users.Add(new Tuple<BaseEntities.Mark, int>(new BaseEntities.Mark() { uri = "mark/m1_3_5" }, 1));
            uploadedBy.Add(new Tuple<BaseEntities.Mark, BaseEntities.Student>(new BaseEntities.Mark() { uri = "mark/m1_3_5" }, me));

            rates.Add(new Tuple<BaseEntities.Mark, int>(new BaseEntities.Mark() { uri = "mark/m2_4_4" }, 4));
            items.Add(new Tuple<BaseEntities.Mark, int>(new BaseEntities.Mark() { uri = "mark/m2_4_4" }, 4));
            users.Add(new Tuple<BaseEntities.Mark, int>(new BaseEntities.Mark() { uri = "mark/m2_4_4" }, 2));
            uploadedBy.Add(new Tuple<BaseEntities.Mark, BaseEntities.Student>(new BaseEntities.Mark() { uri = "mark/m2_4_4" }, me));

            rates.Add(new Tuple<BaseEntities.Mark, int>(new BaseEntities.Mark() { uri = "mark/m2_7_3" }, 3));
            items.Add(new Tuple<BaseEntities.Mark, int>(new BaseEntities.Mark() { uri = "mark/m2_7_3" }, 7));
            users.Add(new Tuple<BaseEntities.Mark, int>(new BaseEntities.Mark() { uri = "mark/m2_7_3" }, 2));
            uploadedBy.Add(new Tuple<BaseEntities.Mark, BaseEntities.Student>(new BaseEntities.Mark() { uri = "mark/m2_7_3" }, me));

            rates.Add(new Tuple<BaseEntities.Mark, int>(new BaseEntities.Mark() { uri = "mark/m2_9_3" }, 3));
            items.Add(new Tuple<BaseEntities.Mark, int>(new BaseEntities.Mark() { uri = "mark/m2_9_3" }, 9));
            users.Add(new Tuple<BaseEntities.Mark, int>(new BaseEntities.Mark() { uri = "mark/m2_9_3" }, 2));
            uploadedBy.Add(new Tuple<BaseEntities.Mark, BaseEntities.Student>(new BaseEntities.Mark() { uri = "mark/m2_9_3" }, me));

            rates.Add(new Tuple<BaseEntities.Mark, int>(new BaseEntities.Mark() { uri = "mark/m2_3_4" }, 4));
            items.Add(new Tuple<BaseEntities.Mark, int>(new BaseEntities.Mark() { uri = "mark/m2_3_4" }, 3));
            users.Add(new Tuple<BaseEntities.Mark, int>(new BaseEntities.Mark() { uri = "mark/m2_3_4" }, 2));
            uploadedBy.Add(new Tuple<BaseEntities.Mark, BaseEntities.Student>(new BaseEntities.Mark() { uri = "mark/m2_3_4" }, me));

            rates.Add(new Tuple<BaseEntities.Mark, int>(new BaseEntities.Mark() { uri = "mark/m2_10_3" }, 3));
            items.Add(new Tuple<BaseEntities.Mark, int>(new BaseEntities.Mark() { uri = "mark/m2_10_3" }, 10));
            users.Add(new Tuple<BaseEntities.Mark, int>(new BaseEntities.Mark() { uri = "mark/m2_10_3" }, 2));
            uploadedBy.Add(new Tuple<BaseEntities.Mark, BaseEntities.Student>(new BaseEntities.Mark() { uri = "mark/m2_10_3" }, me));

            rates.Add(new Tuple<BaseEntities.Mark, int>(new BaseEntities.Mark() { uri = "mark/m3_1_4" }, 4));
            items.Add(new Tuple<BaseEntities.Mark, int>(new BaseEntities.Mark() { uri = "mark/m3_1_4" }, 1));
            users.Add(new Tuple<BaseEntities.Mark, int>(new BaseEntities.Mark() { uri = "mark/m3_1_4" }, 3));
            uploadedBy.Add(new Tuple<BaseEntities.Mark, BaseEntities.Student>(new BaseEntities.Mark() { uri = "mark/m3_1_4" }, me));

            rates.Add(new Tuple<BaseEntities.Mark, int>(new BaseEntities.Mark() { uri = "mark/m3_2_5" }, 5));
            items.Add(new Tuple<BaseEntities.Mark, int>(new BaseEntities.Mark() { uri = "mark/m3_2_5" }, 2));
            users.Add(new Tuple<BaseEntities.Mark, int>(new BaseEntities.Mark() { uri = "mark/m3_2_5" }, 3));
            uploadedBy.Add(new Tuple<BaseEntities.Mark, BaseEntities.Student>(new BaseEntities.Mark() { uri = "mark/m3_2_5" }, me));

            rates.Add(new Tuple<BaseEntities.Mark, int>(new BaseEntities.Mark() { uri = "mark/m3_6_3" }, 3));
            items.Add(new Tuple<BaseEntities.Mark, int>(new BaseEntities.Mark() { uri = "mark/m3_6_3" }, 6));
            users.Add(new Tuple<BaseEntities.Mark, int>(new BaseEntities.Mark() { uri = "mark/m3_6_3" }, 3));
            uploadedBy.Add(new Tuple<BaseEntities.Mark, BaseEntities.Student>(new BaseEntities.Mark() { uri = "mark/m3_6_3" }, me));

            rates.Add(new Tuple<BaseEntities.Mark, int>(new BaseEntities.Mark() { uri = "mark/m3_8_5" }, 5));
            items.Add(new Tuple<BaseEntities.Mark, int>(new BaseEntities.Mark() { uri = "mark/m3_8_5" }, 8));
            users.Add(new Tuple<BaseEntities.Mark, int>(new BaseEntities.Mark() { uri = "mark/m3_8_5" }, 3));
            uploadedBy.Add(new Tuple<BaseEntities.Mark, BaseEntities.Student>(new BaseEntities.Mark() { uri = "mark/m3_8_5" }, me));

            rates.Add(new Tuple<BaseEntities.Mark, int>(new BaseEntities.Mark() { uri = "mark/m3_10_4" }, 4));
            items.Add(new Tuple<BaseEntities.Mark, int>(new BaseEntities.Mark() { uri = "mark/m3_10_4" }, 10));
            users.Add(new Tuple<BaseEntities.Mark, int>(new BaseEntities.Mark() { uri = "mark/m3_10_4" }, 3));
            uploadedBy.Add(new Tuple<BaseEntities.Mark, BaseEntities.Student>(new BaseEntities.Mark() { uri = "mark/m3_10_4" }, me));

            rates.Add(new Tuple<BaseEntities.Mark, int>(new BaseEntities.Mark() { uri = "mark/m4_1_3" }, 3));
            items.Add(new Tuple<BaseEntities.Mark, int>(new BaseEntities.Mark() { uri = "mark/m4_1_3" }, 1));
            users.Add(new Tuple<BaseEntities.Mark, int>(new BaseEntities.Mark() { uri = "mark/m4_1_3" }, 4));
            uploadedBy.Add(new Tuple<BaseEntities.Mark, BaseEntities.Student>(new BaseEntities.Mark() { uri = "mark/m4_1_3" }, me));

            rates.Add(new Tuple<BaseEntities.Mark, int>(new BaseEntities.Mark() { uri = "mark/m4_7_4" }, 4));
            items.Add(new Tuple<BaseEntities.Mark, int>(new BaseEntities.Mark() { uri = "mark/m4_7_4" }, 7));
            users.Add(new Tuple<BaseEntities.Mark, int>(new BaseEntities.Mark() { uri = "mark/m4_7_4" }, 4));
            uploadedBy.Add(new Tuple<BaseEntities.Mark, BaseEntities.Student>(new BaseEntities.Mark() { uri = "mark/m4_7_4" }, me));

            rates.Add(new Tuple<BaseEntities.Mark, int>(new BaseEntities.Mark() { uri = "mark/m4_9_4" }, 4));
            items.Add(new Tuple<BaseEntities.Mark, int>(new BaseEntities.Mark() { uri = "mark/m4_9_4" }, 9));
            users.Add(new Tuple<BaseEntities.Mark, int>(new BaseEntities.Mark() { uri = "mark/m4_9_4" }, 4));
            uploadedBy.Add(new Tuple<BaseEntities.Mark, BaseEntities.Student>(new BaseEntities.Mark() { uri = "mark/m4_9_4" }, me));

            rates.Add(new Tuple<BaseEntities.Mark, int>(new BaseEntities.Mark() { uri = "mark/m4_3_5" }, 5));
            items.Add(new Tuple<BaseEntities.Mark, int>(new BaseEntities.Mark() { uri = "mark/m4_3_5" }, 3));
            users.Add(new Tuple<BaseEntities.Mark, int>(new BaseEntities.Mark() { uri = "mark/m4_3_5" }, 4));
            uploadedBy.Add(new Tuple<BaseEntities.Mark, BaseEntities.Student>(new BaseEntities.Mark() { uri = "mark/m4_3_5" }, me));

            rates.Add(new Tuple<BaseEntities.Mark, int>(new BaseEntities.Mark() { uri = "mark/m5_2_4" }, 4));
            items.Add(new Tuple<BaseEntities.Mark, int>(new BaseEntities.Mark() { uri = "mark/m5_2_4" }, 2));
            users.Add(new Tuple<BaseEntities.Mark, int>(new BaseEntities.Mark() { uri = "mark/m5_2_4" }, 5));
            uploadedBy.Add(new Tuple<BaseEntities.Mark, BaseEntities.Student>(new BaseEntities.Mark() { uri = "mark/m5_2_4" }, me));

            rates.Add(new Tuple<BaseEntities.Mark, int>(new BaseEntities.Mark() { uri = "mark/m5_7_4" }, 4));
            items.Add(new Tuple<BaseEntities.Mark, int>(new BaseEntities.Mark() { uri = "mark/m5_7_4" }, 7));
            users.Add(new Tuple<BaseEntities.Mark, int>(new BaseEntities.Mark() { uri = "mark/m5_7_4" }, 5));
            uploadedBy.Add(new Tuple<BaseEntities.Mark, BaseEntities.Student>(new BaseEntities.Mark() { uri = "mark/m5_7_4" }, me));

            rates.Add(new Tuple<BaseEntities.Mark, int>(new BaseEntities.Mark() { uri = "mark/m5_6_3" }, 3));
            items.Add(new Tuple<BaseEntities.Mark, int>(new BaseEntities.Mark() { uri = "mark/m5_6_3" }, 6));
            users.Add(new Tuple<BaseEntities.Mark, int>(new BaseEntities.Mark() { uri = "mark/m5_6_3" }, 5));
            uploadedBy.Add(new Tuple<BaseEntities.Mark, BaseEntities.Student>(new BaseEntities.Mark() { uri = "mark/m5_6_3" }, me));

            rates.Add(new Tuple<BaseEntities.Mark, int>(new BaseEntities.Mark() { uri = "mark/m5_8_3" }, 3));
            items.Add(new Tuple<BaseEntities.Mark, int>(new BaseEntities.Mark() { uri = "mark/m5_8_3" }, 8));
            users.Add(new Tuple<BaseEntities.Mark, int>(new BaseEntities.Mark() { uri = "mark/m5_8_3" }, 5));
            uploadedBy.Add(new Tuple<BaseEntities.Mark, BaseEntities.Student>(new BaseEntities.Mark() { uri = "mark/m5_8_3" }, me));

            rates.Add(new Tuple<BaseEntities.Mark, int>(new BaseEntities.Mark() { uri = "mark/m6_9_3" }, 3));
            items.Add(new Tuple<BaseEntities.Mark, int>(new BaseEntities.Mark() { uri = "mark/m6_9_3" }, 9));
            users.Add(new Tuple<BaseEntities.Mark, int>(new BaseEntities.Mark() { uri = "mark/m6_9_3" }, 6));
            uploadedBy.Add(new Tuple<BaseEntities.Mark, BaseEntities.Student>(new BaseEntities.Mark() { uri = "mark/m6_9_3" }, me));

            rates.Add(new Tuple<BaseEntities.Mark, int>(new BaseEntities.Mark() { uri = "mark/m6_6_3" }, 3));
            items.Add(new Tuple<BaseEntities.Mark, int>(new BaseEntities.Mark() { uri = "mark/m6_6_3" }, 6));
            users.Add(new Tuple<BaseEntities.Mark, int>(new BaseEntities.Mark() { uri = "mark/m6_6_3" }, 6));
            uploadedBy.Add(new Tuple<BaseEntities.Mark, BaseEntities.Student>(new BaseEntities.Mark() { uri = "mark/m6_6_3" }, me));

            rates.Add(new Tuple<BaseEntities.Mark, int>(new BaseEntities.Mark() { uri = "mark/m6_3_6" }, 6));
            items.Add(new Tuple<BaseEntities.Mark, int>(new BaseEntities.Mark() { uri = "mark/m6_3_6" }, 3));
            users.Add(new Tuple<BaseEntities.Mark, int>(new BaseEntities.Mark() { uri = "mark/m6_3_6" }, 6));
            uploadedBy.Add(new Tuple<BaseEntities.Mark, BaseEntities.Student>(new BaseEntities.Mark() { uri = "mark/m6_3_6" }, me));

            rates.Add(new Tuple<BaseEntities.Mark, int>(new BaseEntities.Mark() { uri = "mark/m7_1_3" }, 3));
            items.Add(new Tuple<BaseEntities.Mark, int>(new BaseEntities.Mark() { uri = "mark/m7_1_3" }, 1));
            users.Add(new Tuple<BaseEntities.Mark, int>(new BaseEntities.Mark() { uri = "mark/m7_1_3" }, 7));
            uploadedBy.Add(new Tuple<BaseEntities.Mark, BaseEntities.Student>(new BaseEntities.Mark() { uri = "mark/m7_1_3" }, me));

            rates.Add(new Tuple<BaseEntities.Mark, int>(new BaseEntities.Mark() { uri = "mark/m7_2_4" }, 4));
            items.Add(new Tuple<BaseEntities.Mark, int>(new BaseEntities.Mark() { uri = "mark/m7_2_4" }, 2));
            users.Add(new Tuple<BaseEntities.Mark, int>(new BaseEntities.Mark() { uri = "mark/m7_2_4" }, 7));
            uploadedBy.Add(new Tuple<BaseEntities.Mark, BaseEntities.Student>(new BaseEntities.Mark() { uri = "mark/m7_2_4" }, me));

            rates.Add(new Tuple<BaseEntities.Mark, int>(new BaseEntities.Mark() { uri = "mark/m7_4_4" }, 4));
            items.Add(new Tuple<BaseEntities.Mark, int>(new BaseEntities.Mark() { uri = "mark/m7_4_4" }, 4));
            users.Add(new Tuple<BaseEntities.Mark, int>(new BaseEntities.Mark() { uri = "mark/m7_4_4" }, 7));
            uploadedBy.Add(new Tuple<BaseEntities.Mark, BaseEntities.Student>(new BaseEntities.Mark() { uri = "mark/m7_4_4" }, me));

            rates.Add(new Tuple<BaseEntities.Mark, int>(new BaseEntities.Mark() { uri = "mark/m7_7_4" }, 4));
            items.Add(new Tuple<BaseEntities.Mark, int>(new BaseEntities.Mark() { uri = "mark/m7_7_4" }, 7));
            users.Add(new Tuple<BaseEntities.Mark, int>(new BaseEntities.Mark() { uri = "mark/m7_7_4" }, 7));
            uploadedBy.Add(new Tuple<BaseEntities.Mark, BaseEntities.Student>(new BaseEntities.Mark() { uri = "mark/m7_7_4" }, me));

            rates.Add(new Tuple<BaseEntities.Mark, int>(new BaseEntities.Mark() { uri = "mark/m7_5_4" }, 4));
            items.Add(new Tuple<BaseEntities.Mark, int>(new BaseEntities.Mark() { uri = "mark/m7_5_4" }, 5));
            users.Add(new Tuple<BaseEntities.Mark, int>(new BaseEntities.Mark() { uri = "mark/m7_5_4" }, 7));
            uploadedBy.Add(new Tuple<BaseEntities.Mark, BaseEntities.Student>(new BaseEntities.Mark() { uri = "mark/m7_5_4" }, me));

            rates.Add(new Tuple<BaseEntities.Mark, int>(new BaseEntities.Mark() { uri = "mark/m7_8_4" }, 4));
            items.Add(new Tuple<BaseEntities.Mark, int>(new BaseEntities.Mark() { uri = "mark/m7_8_4" }, 8));
            users.Add(new Tuple<BaseEntities.Mark, int>(new BaseEntities.Mark() { uri = "mark/m7_8_4" }, 7));
            uploadedBy.Add(new Tuple<BaseEntities.Mark, BaseEntities.Student>(new BaseEntities.Mark() { uri = "mark/m7_8_4" }, me));

            rates.Add(new Tuple<BaseEntities.Mark, int>(new BaseEntities.Mark() { uri = "mark/m7_10_3" }, 3));
            items.Add(new Tuple<BaseEntities.Mark, int>(new BaseEntities.Mark() { uri = "mark/m7_10_3" }, 10));
            users.Add(new Tuple<BaseEntities.Mark, int>(new BaseEntities.Mark() { uri = "mark/m7_10_3" }, 7));
            uploadedBy.Add(new Tuple<BaseEntities.Mark, BaseEntities.Student>(new BaseEntities.Mark() { uri = "mark/m7_10_3" }, me));

            rates.Add(new Tuple<BaseEntities.Mark, int>(new BaseEntities.Mark() { uri = "mark/m8_5_5" }, 5));
            items.Add(new Tuple<BaseEntities.Mark, int>(new BaseEntities.Mark() { uri = "mark/m8_5_5" }, 5));
            users.Add(new Tuple<BaseEntities.Mark, int>(new BaseEntities.Mark() { uri = "mark/m8_5_5" }, 8));
            uploadedBy.Add(new Tuple<BaseEntities.Mark, BaseEntities.Student>(new BaseEntities.Mark() { uri = "mark/m8_5_5" }, me));

            rates.Add(new Tuple<BaseEntities.Mark, int>(new BaseEntities.Mark() { uri = "mark/m8_6_2" }, 2));
            items.Add(new Tuple<BaseEntities.Mark, int>(new BaseEntities.Mark() { uri = "mark/m8_6_2" }, 6));
            users.Add(new Tuple<BaseEntities.Mark, int>(new BaseEntities.Mark() { uri = "mark/m8_6_2" }, 8));
            uploadedBy.Add(new Tuple<BaseEntities.Mark, BaseEntities.Student>(new BaseEntities.Mark() { uri = "mark/m8_6_2" }, me));

            rates.Add(new Tuple<BaseEntities.Mark, int>(new BaseEntities.Mark() { uri = "mark/m8_3_5" }, 5));
            items.Add(new Tuple<BaseEntities.Mark, int>(new BaseEntities.Mark() { uri = "mark/m8_3_5" }, 3));
            users.Add(new Tuple<BaseEntities.Mark, int>(new BaseEntities.Mark() { uri = "mark/m8_3_5" }, 8));
            uploadedBy.Add(new Tuple<BaseEntities.Mark, BaseEntities.Student>(new BaseEntities.Mark() { uri = "mark/m8_3_5" }, me));

            rates.Add(new Tuple<BaseEntities.Mark, int>(new BaseEntities.Mark() { uri = "mark/m9_1_2" }, 2));
            items.Add(new Tuple<BaseEntities.Mark, int>(new BaseEntities.Mark() { uri = "mark/m9_1_2" }, 1));
            users.Add(new Tuple<BaseEntities.Mark, int>(new BaseEntities.Mark() { uri = "mark/m9_1_2" }, 9));
            uploadedBy.Add(new Tuple<BaseEntities.Mark, BaseEntities.Student>(new BaseEntities.Mark() { uri = "mark/m9_1_2" }, me));

            rates.Add(new Tuple<BaseEntities.Mark, int>(new BaseEntities.Mark() { uri = "mark/m9_6_3" }, 3));
            items.Add(new Tuple<BaseEntities.Mark, int>(new BaseEntities.Mark() { uri = "mark/m9_6_3" }, 6));
            users.Add(new Tuple<BaseEntities.Mark, int>(new BaseEntities.Mark() { uri = "mark/m9_6_3" }, 9));
            uploadedBy.Add(new Tuple<BaseEntities.Mark, BaseEntities.Student>(new BaseEntities.Mark() { uri = "mark/m9_6_3" }, me));

            rates.Add(new Tuple<BaseEntities.Mark, int>(new BaseEntities.Mark() { uri = "mark/m9_3_5" }, 5));
            items.Add(new Tuple<BaseEntities.Mark, int>(new BaseEntities.Mark() { uri = "mark/m9_3_5" }, 3));
            users.Add(new Tuple<BaseEntities.Mark, int>(new BaseEntities.Mark() { uri = "mark/m9_3_5" }, 9));
            uploadedBy.Add(new Tuple<BaseEntities.Mark, BaseEntities.Student>(new BaseEntities.Mark() { uri = "mark/m9_3_5" }, me));

            rates.Add(new Tuple<BaseEntities.Mark, int>(new BaseEntities.Mark() { uri = "mark/m10_2_3" }, 3));
            items.Add(new Tuple<BaseEntities.Mark, int>(new BaseEntities.Mark() { uri = "mark/m10_2_3" }, 2));
            users.Add(new Tuple<BaseEntities.Mark, int>(new BaseEntities.Mark() { uri = "mark/m10_2_3" }, 10));
            uploadedBy.Add(new Tuple<BaseEntities.Mark, BaseEntities.Student>(new BaseEntities.Mark() { uri = "mark/m10_2_3" }, me));

            rates.Add(new Tuple<BaseEntities.Mark, int>(new BaseEntities.Mark() { uri = "mark/m10_4_4" }, 4));
            items.Add(new Tuple<BaseEntities.Mark, int>(new BaseEntities.Mark() { uri = "mark/m10_4_4" }, 4));
            users.Add(new Tuple<BaseEntities.Mark, int>(new BaseEntities.Mark() { uri = "mark/m10_4_4" }, 10));
            uploadedBy.Add(new Tuple<BaseEntities.Mark, BaseEntities.Student>(new BaseEntities.Mark() { uri = "mark/m10_4_4" }, me));

            rates.Add(new Tuple<BaseEntities.Mark, int>(new BaseEntities.Mark() { uri = "mark/m10_7_3" }, 3));
            items.Add(new Tuple<BaseEntities.Mark, int>(new BaseEntities.Mark() { uri = "mark/m10_7_3" }, 7));
            users.Add(new Tuple<BaseEntities.Mark, int>(new BaseEntities.Mark() { uri = "mark/m10_7_3" }, 10));
            uploadedBy.Add(new Tuple<BaseEntities.Mark, BaseEntities.Student>(new BaseEntities.Mark() { uri = "mark/m10_7_3" }, me));

            rates.Add(new Tuple<BaseEntities.Mark, int>(new BaseEntities.Mark() { uri = "mark/m10_9_2" }, 2));
            items.Add(new Tuple<BaseEntities.Mark, int>(new BaseEntities.Mark() { uri = "mark/m10_9_2" }, 9));
            users.Add(new Tuple<BaseEntities.Mark, int>(new BaseEntities.Mark() { uri = "mark/m10_9_2" }, 10));
            uploadedBy.Add(new Tuple<BaseEntities.Mark, BaseEntities.Student>(new BaseEntities.Mark() { uri = "mark/m10_9_2" }, me));

            rates.Add(new Tuple<BaseEntities.Mark, int>(new BaseEntities.Mark() { uri = "mark/m10_5_3" }, 3));
            items.Add(new Tuple<BaseEntities.Mark, int>(new BaseEntities.Mark() { uri = "mark/m10_5_3" }, 5));
            users.Add(new Tuple<BaseEntities.Mark, int>(new BaseEntities.Mark() { uri = "mark/m10_5_3" }, 10));
            uploadedBy.Add(new Tuple<BaseEntities.Mark, BaseEntities.Student>(new BaseEntities.Mark() { uri = "mark/m10_5_3" }, me));

            rates.Add(new Tuple<BaseEntities.Mark, int>(new BaseEntities.Mark() { uri = "mark/m10_8_4" }, 4));
            items.Add(new Tuple<BaseEntities.Mark, int>(new BaseEntities.Mark() { uri = "mark/m10_8_4" }, 8));
            users.Add(new Tuple<BaseEntities.Mark, int>(new BaseEntities.Mark() { uri = "mark/m10_8_4" }, 10));
            uploadedBy.Add(new Tuple<BaseEntities.Mark, BaseEntities.Student>(new BaseEntities.Mark() { uri = "mark/m10_8_4" }, me));

            rates.Add(new Tuple<BaseEntities.Mark, int>(new BaseEntities.Mark() { uri = "mark/m10_10_3" }, 3));
            items.Add(new Tuple<BaseEntities.Mark, int>(new BaseEntities.Mark() { uri = "mark/m10_10_3" }, 10));
            users.Add(new Tuple<BaseEntities.Mark, int>(new BaseEntities.Mark() { uri = "mark/m10_10_3" }, 10));
            uploadedBy.Add(new Tuple<BaseEntities.Mark, BaseEntities.Student>(new BaseEntities.Mark() { uri = "mark/m10_10_3" }, me));


            #endregion

            state.Clear();
            state.SetState(rates, items, users, uploadedBy, hasVariant);
            state.AnalyzeTriplets();

            try {
                IsCorrect = state.IsCorrect.Where(tuple => tuple.Item1 == me).Select(t => t.Item2).Single();
            }
            catch (Exception exc){
                IsCorrect = false;
            }

        }


        public bool IsCorrect
        { get; private set; }
    }
}
