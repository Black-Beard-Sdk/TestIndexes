using Bb.Filters.Tree;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;

namespace SortFilterUnitTest
{
    [TestClass]
    public class TreeUnitTest1
    {


        [TestMethod]
        public void TestOk()
        {         

            // Specifie l'ordre des clefs trie
            Tree<Model> tree = new Tree<Model>(
                c => c.Ope5,
                c => c.Car2,
                c => c.Qs3,
                c => c.Pu1,
                c => c.Pre4
                );

            tree.Sort(Datas.Matrix);  // on construit 
            tree.Reduce();      // et optimise les données

            // Recupère un délégué en specifiant les correspondances
            Predicate<object> filter = tree.GetEvaluator<Model1>(
                (model, expected) => model.Dgh5 == expected.Ope5,
                (model, expected) => model.Re2 == expected.Car2,
                (model, expected) => model.Df3 == expected.Qs3,
                (model, expected) => model.az1 == expected.Pu1
                );


            Assert.AreEqual(filter(new Model1() { Dgh5 = 2, Re2 = null, Df3 = null, az1 = 23 }), true);

        }


        [TestMethod]
        public void TestKo()
        {
          
            // Specifie l'ordre des clefs trie
            Tree<Model> tree = new Tree<Model>(
                c => c.Ope5,
                c => c.Car2,
                c => c.Qs3,
                c => c.Pu1,
                c => c.Pre4
                );

            tree.Sort(Datas.Matrix);  // on construit 
            tree.Reduce();      // et optimise les données

            // Recupère un délégué en specifiant les correspondances
            Predicate<object> filter = tree.GetEvaluator<Model1>(
                (model, expected) => model.Dgh5 == expected.Ope5,
                (model, expected) => model.Re2 == expected.Car2,
                (model, expected) => model.Df3 == expected.Qs3,
                (model, expected) => model.az1 == expected.Pu1
                );

            Assert.AreEqual(filter(new Model1() { Dgh5 = 1, Re2 = 2, }), false);

        }

        [TestMethod]
        public void TestPerf()
        {          

            // Specifie l'ordre des clefs trie
            Tree<Model> tree = new Tree<Model>(
                c => c.Ope5,
                c => c.Car2,
                c => c.Qs3,
                c => c.Pu1,
                c => c.Pre4
                );

            tree.Sort(Datas.Matrix);  // on construit 
            tree.Reduce();      // et optimise les données

            // Recupère un délégué en specifiant les correspondances
            Predicate<object> filter = tree.GetEvaluator<Model1>(
                (model, expected) => model.Dgh5 == expected.Ope5,
                (model, expected) => model.Re2 == expected.Car2,
                (model, expected) => model.Df3 == expected.Qs3,
                (model, expected) => model.az1 == expected.Pu1
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
                0.051013
                0.000185
                0,0010638
            }

             */

        }


    }

}
