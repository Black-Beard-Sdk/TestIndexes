using Bb.Filters.Indexes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;

namespace SortFilterUnitTest
{
    [TestClass]
    public class IndexUnitTest1
    {

        [TestMethod]
        public void TestOk()
        {

            // Specifie l'ordre des clefs trie
            Index<Model> tree = new Index<Model>(null,
                c => c.Ope5,
                c => c.Car2,
                c => c.Qs3,
                c => c.Pu1,
                c => c.Pre4
                );

            tree.Sort(Datas.Matrix);  // on construit 

            var filter = tree.GetEvaluator<Model1>(
                c => c.Dgh5,
                c => c.Re2,
                c => c.Df3,
                c => c.az1,
                c => c.Klm4
                );


            Assert.AreEqual(filter(new Model1() { Dgh5 = 2, Re2 = null, Df3 = null, az1 = 23 }), true);

        }

        [TestMethod]
        public void TestKo()
        {          

            // Specifie l'ordre des clefs trie
            Index<Model> tree = new Index<Model>(null,
                c => c.Ope5,
                c => c.Car2,
                c => c.Qs3,
                c => c.Pu1,
                c => c.Pre4
                );

            tree.Sort(Datas.Matrix);  // on construit 

            var filter = tree.GetEvaluator<Model1>(
                c => c.Dgh5,
                c => c.Re2,
                c => c.Df3,
                c => c.az1,
                c => c.Klm4
                );

            Assert.AreEqual(filter(new Model1() { Dgh5 = 1, Re2 = 2, }), false);

        }

         [TestMethod]
        public void TestPerf()
        {
          
            // Specifie l'ordre des clefs trie
            Index<Model> tree = new Index<Model>(null,
                c => c.Ope5,
                c => c.Car2,
                c => c.Qs3,
                c => c.Pu1,
                c => c.Pre4
                );

            tree.Sort(Datas.Matrix);  // on construit 

            var filter = tree.GetEvaluator<Model1>(
                c => c.Dgh5,
                c => c.Re2,
                c => c.Df3,
                c => c.az1,
                c => c.Klm4
                );

            var m = new Model1() { Dgh5 = 1, Re2 = 2, };
            Stopwatch st = new Stopwatch();

            st.Start();
            Assert.AreEqual(filter(m), false);
            st.Stop();

            Debug.WriteLine(st.Elapsed.TotalMilliseconds);           

            var count = 1000;
            st.Restart();
            for (int i = 0; i < count; i++)
            {
                filter(m);
            }

            st.Stop();
            Debug.WriteLine(st.Elapsed.TotalMilliseconds / count);


            /*
                3,4856
                0,0030199
            */

        }

    }

}
