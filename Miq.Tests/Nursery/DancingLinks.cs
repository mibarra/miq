using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Miq.Tests.Nursery
{

    [TestClass]
    public class DancingLinks
    {
        class ExpandableList<T>
        {
        }

        class Node
        {
            public Node left;
            public Node right;
            public Node up;
            public Node down;
            public Node column;
            public int size;
            public string name;

            public Node(string name)
            {
                this.name = name;
            }

            public void search(int k)
            {
                // if right = this 
                //     print current solution
                //     return

                // choose column c
                // cover c

                // foreach r: d[c], d[d[c]], ... while r != c
                //     O[k]  = r
                //     foreach j: r[r], r[r[r]], ... while j != r
                //          cover C[j]
                //     search(k+1)
                //     r = O[k]
                //     c = c[r]
                //     foreach j: l[r], l[l[r]], ... while j != r
                //          uncover c[j]
                
                // uncover c
            }
        }

        public class ExpandibleList<T> : IList<T>
        {
            List<T> List;

            public ExpandibleList()
            {
                List = new List<T>();
            }

            public int IndexOf(T item)
            {
                return List.IndexOf(item); 
            }

            public void Insert(int index, T item)
            {
                MaybeExpand(index);
                List.Insert(index, item);
            }

            public void RemoveAt(int index)
            {
                List.RemoveAt(index);
            }

            public T this[int index]
            {
                get
                {
                    return List[index];
                }
                set
                {
                    MaybeExpand(index);
                    List[index] = value;
                }
            }

            public void Add(T item)
            {
                List.Add(item);
            }

            public void Clear()
            {
                List.Clear();
            }

            public bool Contains(T item)
            {
                return List.Contains(item);
                throw new NotImplementedException();
            }

            public void CopyTo(T[] array, int arrayIndex)
            {
                List.CopyTo(array, arrayIndex);
            }

            public int Count
            {
                get { return List.Count; }
            }

            public bool IsReadOnly
            {
                get { return false; }
            }

            public bool Remove(T item)
            {
                return List.Remove(item);
            }

            public IEnumerator<T> GetEnumerator()
            {
                return List.GetEnumerator();
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return ((System.Collections.IEnumerable)List).GetEnumerator();
            }

            private void MaybeExpand(int index)
            {
                int missing = List.Count - 1 - index;
                while (missing > 0)
                {
                    List.Add(default(T));
                    missing--;
                }
            }
        }


        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void IndexingOutOfBoundsAddsNewItems()
        {
            List<Node> a = new List<Node>();
            a[5] = new Node("X");
            Assert.AreEqual(6, a.Count);
        }


// XXX make this pass
//        [TestMethod]
//        public void TestNodeToString()
//        {
//            Node root = SampleDancingLinks();
//            string expected = @"
//h  a  b  c  d  e  f  g
//         c1    e1 f1
//   a2       d2       g2
//      b3 c3       f3
//   a4       d4
//      b5             g5
//            d6 e6    g6
//";
//            string actual = root.ToString();
//            Assert.AreEqual(expected, actual);
//        }

        //// XXX make this pass
        //[TestMethod]
        //public void TestDancingLinksCreation()
        //{
        //    bool[,] matix = {
        //        {false, false, true, false, true, true, false},
        //        {true, false, false, true, false, false, true},
        //        {false, true, true, false, false, true, false},
        //        {true, false, false, true, false, false, false},
        //        {false, true, false, false, false, false, true},
        //        {false, false, false, true, true, false, true}
        //    };
        //    Node dancingLinks = new Node(matrix);

        //    Assert.AreEqual(SampleDancingLinks, dancingLinks);
        //}

        private static Node SampleDancingLinks()
        {
            Node root = new Node("h");

            Node A = new Node("A"); A.size = 2;
            Node B = new Node("B"); B.size = 2;
            Node C = new Node("C"); C.size = 2;
            Node D = new Node("D"); D.size = 3;
            Node E = new Node("E"); E.size = 2;
            Node F = new Node("F"); F.size = 2;
            Node G = new Node("G"); G.size = 3;

            Node C1 = new Node("C1"); C1.column = C;
            Node E1 = new Node("E1"); E1.column = E;
            Node F1 = new Node("F1"); F1.column = F;
            Node A2 = new Node("A2"); A2.column = A;
            Node D2 = new Node("D2"); D2.column = D;
            Node G2 = new Node("G2"); G2.column = G;
            Node B3 = new Node("B3"); B3.column = B;
            Node C3 = new Node("C3"); C3.column = C;
            Node F3 = new Node("F3"); F3.column = F;
            Node A4 = new Node("A4"); A4.column = A;
            Node D4 = new Node("D4"); D4.column = D;
            Node B5 = new Node("B5"); B5.column = B;
            Node G5 = new Node("G5"); G5.column = G;
            Node D6 = new Node("D6"); D6.column = D;
            Node E6 = new Node("E6"); E6.column = E;
            Node G6 = new Node("G6"); G6.column = G;

            root.right = A; root.left = G;

            A.right = B; A.left = root; A.down = A2; A.up = A4; A.column = A;
            B.right = C; B.left = A; B.down = B3; B.up = B5; B.column = B;
            C.right = D; C.left = B; C.down = C1; C.up = C3; C.column = C;
            D.right = E; D.left = C; D.down = D2; D.up = D6; D.column = D;
            E.right = F; E.left = D; E.down = E1; E.up = E6; E.column = E;
            F.right = G; F.left = E; F.down = F1; F.up = F3; F.column = F;
            G.right = root; G.left = F; G.down = G2; G.up = G6; G.column = G;

            C1.right = E1; C1.left = F1; C1.down = C3; C1.up = C;
            E1.right = F1; E1.left = C1; E1.down = E6; E1.up = E;
            F1.right = C1; F1.left = E1; F1.down = F3; F1.up = F;
            A2.right = D2; A2.left = G2; A2.down = A4; A2.up = A;
            D2.right = G2; D2.left = A2; D2.down = D4; D2.up = D;
            G2.right = A2; G2.left = D2; G2.down = G5; G2.up = G;
            B3.right = C3; B3.left = F3; B3.down = B5; B3.up = B;
            C3.right = F3; C3.left = B3; C3.down = C; C3.up = C1;
            F3.right = B3; F3.left = C3; F3.down = F; F3.up = F1;
            A4.right = D4; A4.left = D4; A4.down = A; A4.up = A2;
            D4.right = A4; D4.left = A4; D4.down = D6; D4.up = D2;
            B5.right = G5; B5.left = G5; B5.down = B; B5.up = B3;
            G5.right = B5; G5.left = B5; G5.down = G6; G5.up = G2;
            D6.right = E6; D6.left = G6; D6.down = D; D6.up = D4;
            E6.right = G6; E6.left = D6; E6.down = E; E6.up = E1;
            G6.right = D6; G6.left = E6; G6.down = G; G6.up = G5;
            return root;
        }
    }
}
