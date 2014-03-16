using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miq.Tests.Nursery
{
    [TestClass]
    public class LSystemsTests
    {
        class Production
        {
            public Production(char predecessor, string successor)
            {
                Predecessor = predecessor;
                Successor = successor;
            }

            public char Predecessor { get; private set; }
            public string Successor { get; private set; }

            internal bool CompliesWithAlphabet(string alphabet)
            {
                return alphabet.Contains(Predecessor) && Successor.All(c => alphabet.Contains(c));
            }
        }

        class ProductionsCollection
        {
            public ProductionsCollection(List<Production> productions)
            {
                VerifyArguments(productions);
                this.productions = new List<Production>(productions);
            }

            public string SuccessorFor(char predecessor)
            {
                var production = productions.SingleOrDefault(p => p.Predecessor == predecessor);
                return production != null ? production.Successor
                                         : predecessor.ToString();
            }

            public bool CompliesWithAlphabet(string alphabet)
            {
                return productions.All(p => p.CompliesWithAlphabet(alphabet));
            }

            private List<Production> productions;
            private static void VerifyArguments(List<Production> productions)
            {
                if (productions == null)
                {
                    throw new ArgumentNullException("productions");
                }
                if (productions.Count == 0)
                {
                    throw new ArgumentException("productions");
                }
            }
        }

        class OLSystem
        {
            public string Alphabet { get; private set; }
            public string Axiom { get; private set; }
            public ProductionsCollection Productions { get; private set; }

            public OLSystem(string alphabet, string axiom, ProductionsCollection productions)
            {
                VerifyArguments(alphabet, axiom, productions);

                Alphabet = alphabet;
                Axiom = axiom;
                Productions = productions;
            }

            public string Derive(uint derivation)
            {
                if (derivation > 10)
                {
                    throw new ArgumentException("derivation", "OLSystem cannot handle a big number (> 10) of derivations yet");
                }
                return Derive(Axiom, derivation);
            }

            private string Derive(string stringToDerive, uint derivation)
            {
                return derivation == 0? stringToDerive
                                      : Derive(DeriveOnce(stringToDerive), derivation - 1); 
            }

            private string DeriveOnce(string stringToDerive)
            {
                return String.Join("", stringToDerive.Select(c => Productions.SuccessorFor(c)));
            }

            private static void VerifyArguments(string alphabet, string axiom, ProductionsCollection productions)
            {
                if (alphabet == null)
                {
                    throw new ArgumentNullException("alphabet");
                }
                if (alphabet == string.Empty)
                {
                    throw new ArgumentException("alphabet");
                }
                if (axiom == null)
                {
                    throw new ArgumentNullException("axiom");
                }
                if (axiom == string.Empty)
                {
                    throw new ArgumentException("axiom");
                }
                if (!axiom.All(c => alphabet.Contains(c)))
                {
                    throw new ArgumentException("axiom", "axiom must consist of chars from the alphabet only");
                }
                if (!productions.CompliesWithAlphabet(alphabet))
                {
                    throw new ArgumentException("productions", "productions must consist of chars from the alphabet only");
                }
            }
        }

        [TestMethod]
        public void ProductionCanBeInstantiated()
        {
            var sut = new Production('a', "ab");

            Assert.AreEqual('a', sut.Predecessor);
            Assert.AreEqual("ab", sut.Successor);
        }

        [TestMethod]
        public void ProductionsCollectionReturnsSuccessors()
        {
            var productions = new List<Production>() {
                new Production('a', "ab"),
                new Production('b', "a")
            };

            var sut = new ProductionsCollection(productions);

            Assert.AreEqual("ab", sut.SuccessorFor('a'));
            Assert.AreEqual("a", sut.SuccessorFor('b'));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ProductionsMustNotBeNull()
        {
            new ProductionsCollection(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ProductionsMustNotBeEmpty()
        {
            var productions = new List<Production>();
            new ProductionsCollection(productions);
        }

        [TestMethod]
        public void ProductionsCollection_ImplicitDefaulProduction()
        {
            var productions = new List<Production>() {
                new Production('a', "ab"),
                new Production('b', "a")
            };

            var sut = new ProductionsCollection(productions);

            Assert.AreEqual("x", sut.SuccessorFor('x'));
        }

        [TestMethod]
        public void OLSystemCanBeInstantiated()
        {
            var productions = new List<Production>() {
                new Production('a', "ab"),
                new Production('b', "a")
            };

            var sut = new OLSystem("ab", "b", new ProductionsCollection(productions));

            Assert.AreEqual("ab", sut.Alphabet);
            Assert.AreEqual("b", sut.Axiom);
            Assert.AreEqual("ab", sut.Productions.SuccessorFor('a'));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AlphabetMustNotBeNull()
        {
            var productions = new List<Production>() {
                new Production('a', "ab"),
                new Production('b', "a")
            };

            new OLSystem(null, "b", new ProductionsCollection(productions));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AlphabetMustNotBeEmpty()
        {
            var productions = new List<Production>() {
                new Production('a', "ab"),
                new Production('b', "a")
            };

            new OLSystem(string.Empty, "b", new ProductionsCollection(productions));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AxiomMustNotBeNull()
        {
            var productions = new List<Production>() {
                new Production('a', "ab"),
                new Production('b', "a")
            };

            new OLSystem("ab", null, new ProductionsCollection(productions));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AxiomMustNotBeEmpty()
        {
            var productions = new List<Production>() {
                new Production('a', "ab"),
                new Production('b', "a")
            };

            new OLSystem("ab", string.Empty, new ProductionsCollection(productions));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AxiomMustConsistOfLettersFromTheAlphabet()
        {
            var productions = new List<Production>() {
                new Production('a', "ab"),
                new Production('b', "a")
            };

            new OLSystem("ab", "x", new ProductionsCollection(productions));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ProductionsMustConsistOfLettersFromTheAlphabet()
        {
            var productions = new ProductionsCollection(
                new List<Production>()
                {
                    new Production('x', "ab")
                });

            new OLSystem("ab", "b", productions);
        }

        [TestMethod]
        public void Production_CompliesWithAlphabet_ReturnsFalseForBadPredecessor()
        {
            var sut = new Production('x', "ab");
            Assert.IsFalse(sut.CompliesWithAlphabet("ab"));
        }

        [TestMethod]
        public void Production_CompliesWithAlphabet_ReturnsFalseForBadSuccessor()
        {
            var sut = new Production('a', "ax");
            Assert.IsFalse(sut.CompliesWithAlphabet("ab"));
        }

        [TestMethod]
        public void OLSystem_CanDerive()
        {
            var productions = new List<Production>() {
                new Production('a', "ab"),
                new Production('b', "a")
            };

            var sut = new OLSystem("ab", "b", new ProductionsCollection(productions));

            Assert.AreEqual("b", sut.Derive(0));
            Assert.AreEqual("a", sut.Derive(1));
            Assert.AreEqual("ab", sut.Derive(2));
            Assert.AreEqual("aba", sut.Derive(3));
            Assert.AreEqual("abaab", sut.Derive(4));
            Assert.AreEqual("abaababa", sut.Derive(5));
        }

        // An OL-system is deterministic (noted DOL-system) 
        // iff all productions have just one successor
    }
}
