namespace SortFilterUnitTest
{

    public class Datas
    {

        static Datas()
        {

            Datas.Matrix = new Model[]
            {
                new Model() { Ope5 = 1, Car2 = 2     , Qs3 = 1,    Pu1 = null, Pre4 = null },
                new Model() { Ope5 = 1, Car2 = 2     , Qs3 = null, Pu1 = 10,   Pre4 = null },
                new Model() { Ope5 = 1, Car2 = 2     , Qs3 = null, Pu1 = 11,   Pre4 = null },
                new Model() { Ope5 = 1, Car2 = 3     , Qs3 = null, Pu1 = null, Pre4 = null },
                new Model() { Ope5 = 1, Car2 = 16    , Qs3 = null, Pu1 = null, Pre4 = null },
                new Model() { Ope5 = 1, Car2 = 39    , Qs3 = null, Pu1 = null, Pre4 = null },
                new Model() { Ope5 = 1, Car2 = 42    , Qs3 = null, Pu1 = null, Pre4 = null },
                new Model() { Ope5 = 2, Car2 = null  , Qs3 = null, Pu1 = 23,   Pre4 = null },
                new Model() { Ope5 = 3, Car2 = null  , Qs3 = null, Pu1 = null, Pre4 = null },
            };


        }

        public static Model[] Matrix { get; }

    }
}
