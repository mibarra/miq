using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Miq.Tests.Nursery
{
    // Book Storage:
    //      strip a book to minimal components: word separator, paragraph separator. Marked title, marked chapter headings
    //      formating of a stripped book into text, mobi, epub, html, etc...
    //      implement sequitur
    //          compress a string
    //          decompress a string
    //          compress a single file
    //          decompress a single file
    //          compress a full book collection
    //     Concept on book, author, title. Duplicated books... etc...
    //     Concept of lexic?

    [Serializable]
    public class Tree<ValueType>
    {
        public IReadOnlyCollection<ValueType> Values
        { get { return values.AsReadOnly(); } }

        public bool HasValues
        { get { return values.Count > 0; } }

        public bool HasChildren
        { get { return Children.Keys.Count > 0; } }

        public void AddValue(ValueType value)
        {
            values.Add(value);
        }

        public void RemoveValues(Predicate<ValueType> predicate)
        {
            values.RemoveAll(predicate);
            foreach (var child in Children.Values)
            {
                child.RemoveValues(predicate);
            }
        }

        public Tree<ValueType> AddChild(char key)
        {
            var child = new Tree<ValueType>();
            Children.Add(Char.ToLower(key), child);
            return child;
        }

        public Tree<ValueType> GetChild(char key)
        {
            return Children[Char.ToLower(key)];
        }

        public bool HasChild(char key)
        {
            return Children.ContainsKey(Char.ToLower(key));
        }

        public void Compact()
        {
            foreach (var kvp in Children)
            {
                kvp.Value.Compact();
            }

            var keysToRemove = Children.Where(kvp => !kvp.Value.HasValues && !kvp.Value.HasChildren).Select(kvp => kvp.Key).ToList();
            foreach (var key in keysToRemove)
            {
                Children.Remove(key);
            }
        }

        List<ValueType> values = new List<ValueType>();
        public Dictionary<char, Tree<ValueType>> Children = new Dictionary<char, Tree<ValueType>>();
    }

    [Serializable]
    public class Trie<KeyType>
    {
        public void Add(string key, KeyType position)
        {
            var node = Extend(Tree, key);
            node.AddValue(position);
        }

        public void Add(string text, Func<int, KeyType> keySelector)
        {
            var matches = Regex.Matches(text, @"\b(?<word>\w+)");
            foreach (Match match in matches)
            {
                var m = match.Groups["word"];
                Add(m.Value, keySelector(m.Index));
            }
        }

        public void Add(TextReader reader, Func<int, KeyType> keySelector)
        {
            Add(reader.ReadToEnd(), keySelector);
        }

        public void Remove(Predicate<KeyType> predicate)
        {
            Tree.RemoveValues(predicate);
            Tree.Compact();
        }

        public IEnumerable<KeyType> Get(string key)
        {
            var node = Find(Tree, key);
            return node == null ? Enumerable.Empty<KeyType>() : node.Values;
        }

        public Trie()
        {
            Tree = new Tree<KeyType>();
        }

        private Tree<KeyType> Extend(Tree<KeyType> startingAt, string key)
        {
            if (key.Length == 0)
                return startingAt;

            var first = key.First();
            var rest = string.Join("", key.Skip(1));

            if (!startingAt.HasChild(first))
                startingAt.AddChild(first);

            return Extend(startingAt.GetChild(first), rest);
        }

        private Tree<KeyType> Find(Tree<KeyType> startingAt, string key)
        {
            try
            {
                return key.Length == 0 || startingAt == null
                    ? startingAt
                    : Find(startingAt.GetChild(key.First()), string.Join("", key.Skip(1)));
            }
            catch (KeyNotFoundException)
            {
                return null;
            }
        }

        Tree<KeyType> Tree;
    }

    [Serializable]
    public struct BookPosition
    {
        public BookPosition(Uri book, int position)
        {
            Book = book;
            Position = position;
        }

        public Uri Book;
        public int Position;
    }

    [Serializable]
    public class BookCollectionIndex : Trie<BookPosition>
    {
        public void Add(Uri uri, StringReader reader)
        {
            Add(reader, p => new BookPosition(uri, p));
        }

        public void Remove(Uri book)
        {
            Remove(bookPosition => bookPosition.Book == book);
        }
    }

    public class BookCollectionIndexStorage
    {
        public void Write(Stream stream, BookCollectionIndex books)
        {
            IFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, books);
        }

        public BookCollectionIndex Read(Stream stream)
        {
            IFormatter formatter = new BinaryFormatter();
            var books = (BookCollectionIndex)formatter.Deserialize(stream);
            return books;
        }
    }

    [TestClass]
    public class TrieTests
    {
        [TestMethod]
        public void TrieStoresPositions()
        {
            var sut = new Trie<int>();
            sut.Add("abracadabra", 42);
            Assert.AreEqual(42, sut.Get("abracadabra").First());
        }

        [TestMethod]
        public void TrieStoresSeveralWords()
        {
            var sut = new Trie<int>();
            sut.Add("abracadabra", 42);
            sut.Add("abra", 1);
            Assert.AreEqual(42, sut.Get("abracadabra").First());
            Assert.AreEqual(1, sut.Get("abra").First());
        }

        [TestMethod]
        public void Get_WithWordNotFound_ReturnsEmptyResult()
        {
            var sut = new Trie<int>();
            sut.Add("abraca", 42);
            Assert.IsTrue(sut.Get("abracadabra").Count() == 0);
        }

        [TestMethod]
        public void Get_SearchIsCaseInsensitive()
        {
            var sut = new Trie<int>();
            sut.Add("ABRACADABRA", 42);
            Assert.AreEqual(42, sut.Get("abracadabra").First());
        }

        [TestMethod]
        public void Add_OneKeyCanBeAddedMoreThanOnce()
        {
            var sut = new Trie<int>();
            sut.Add("foo", 42);
            sut.Add("foo", 420);
            Assert.AreEqual(2, sut.Get("foo").Count());
        }

        [TestMethod]
        public void FullTextSearch()
        {
            var sut = new Trie<int>();
            sut.Add(@"A psychologist and best-selling author gives us a myth-busting response to the self-help movement, with tips and tricks to improve your life that come straight from the scientific community.

Richard Wiseman has been troubled by the realization that the self-help industry often promotes exercises that destroy motivation, damage relationships, and reduce creativity: the opposite of everything it promises. Now, in 59 Seconds, he fights back, bringing together the diverse scientific advice that can help you change your life in under a minute, and guides you toward becoming more decisive, more imaginative, more engaged, and altogether more happy.", k => k);
            var actual = sut.Get("and");
            Assert.AreEqual(5, actual.Count());
            Assert.AreEqual(626, actual.Last());
        }

        [TestMethod]
        public void CanIndexFromATextReader()
        {
            var sut = new Trie<int>();
            TextReader reader = new StringReader("abra abraca abracadabra");
            sut.Add(reader: reader, keySelector: k => k);
            Assert.AreEqual(12, sut.Get("abracadabra").First());
        }

        [TestMethod]
        public void IndexAndSearchBookCollection()
        {
            var firstBook = new Uri("file://C:/firstFile");
            var secondBook = new Uri("file://C:/secondFile");
            IEnumerable<BookPosition> expected = new List<BookPosition>() {
                new BookPosition(secondBook, 37),
                new BookPosition(firstBook, 31),
                new BookPosition(secondBook, 10)
            };
            var sut = new Trie<BookPosition>();

            sut.Add(new StringReader("this text comes from the first book"), p => new BookPosition(firstBook, p));
            sut.Add(new StringReader("this text book comes from the second book"), p => new BookPosition(secondBook, p));

            CollectionAssert.AreEquivalent(expected.ToArray(), sut.Get("book").ToArray());
        }
    }

    [TestClass]
    public class TreeTests
    {
        [TestMethod]
        public void TreeCanBeInstantiated()
        {
            var sut = new Tree<int>();
            Assert.IsFalse(sut.HasValues);
        }

        [TestMethod]
        public void ValueCanBeAddedToATree()
        {
            var sut = new Tree<int>();
            sut.AddValue(42);
            Assert.IsTrue(sut.HasValues);
        }

        [TestMethod]
        public void ValuesCanBeRecoveredFromATree()
        {
            var sut = new Tree<int>();
            sut.AddValue(42);
            Assert.AreEqual(42, sut.Values.First());
        }

        [TestMethod]
        public void ChildrenCanBeAddedToATree()
        {
            var sut = new Tree<int>();
            var subTree = sut.AddChild('A');
            Assert.IsInstanceOfType(subTree, typeof(Tree<int>));
        }

        [TestMethod]
        public void ChildrenCanBeRecoveredByKey()
        {
            var sut = new Tree<int>();
            sut.AddChild('A');
            Tree<int> actual = sut.GetChild(key: 'A');
            Assert.IsInstanceOfType(actual, typeof(Tree<int>));
        }

        [TestMethod]
        public void RecoveredChildMustBeEqualToReturnedByAddChild()
        {
            var sut = new Tree<int>();

            var returned = sut.AddChild('A');
            var recovered = sut.GetChild('A');

            Assert.AreSame(returned, recovered);
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void GetChild_Throws_WhenKeyNotFound()
        {
            var sut = new Tree<int>();
            sut.GetChild('A');
        }

        [TestMethod]
        public void Tree_CanHaveMoreThanOneChild()
        {
            var sut = new Tree<int>();
            sut.AddChild('A');
            sut.AddChild('B');

            Assert.IsInstanceOfType(sut.GetChild('B'), typeof(Tree<int>));
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void TreeCanBeCompacted()
        {
            var sut = new Tree<int>();

            sut.AddChild('A');
            sut.Compact();

            sut.GetChild('A');
        }

        [TestMethod]
        public void Compact_NodesWithValues_StayInTheTreeAfterCompacting()
        {
            var sut = new Tree<int>();

            var a = sut.AddChild('A');
            var b = a.AddChild('B');
            b.AddValue(42);
            sut.Compact();

            Assert.IsNotNull(sut.GetChild('A'));
            Assert.IsNotNull(a.GetChild('B'));
        }

        // TODO what if they call addchild(null)?
        // TODO what if they call getchild(null)?
    }

    [TestClass]
    public class BookCollectionIndexTests
    {
        [TestMethod]
        public void IndexAndSearchWithABookCollectionIndex()
        {
            var sut = new BookCollectionIndex();
            var firstBook = new Uri("file://C:/firstFile");
            var secondBook = new Uri("file://C:/secondFile");

            sut.Add(firstBook, new StringReader("this text comes from the first book"));
            sut.Add(secondBook, new StringReader("this text book comes from the second book"));

            IEnumerable<BookPosition> expected = new List<BookPosition>() {
                new BookPosition(secondBook, 37),
                new BookPosition(firstBook, 31),
                new BookPosition(secondBook, 10)
            };
            CollectionAssert.AreEquivalent(expected.ToArray(), sut.Get("book").ToArray());
        }

        [TestMethod]
        public void BookCanBeRemovedFromTheIndex()
        {
            var sut = new BookCollectionIndex();
            var firstBook = new Uri("file://C:/firstFile");
            var secondBook = new Uri("file://C:/secondFile");

            sut.Add(firstBook, new StringReader("this text comes from the first book"));
            sut.Add(secondBook, new StringReader("this text book comes from the second book"));
            sut.Remove(secondBook);

            IEnumerable<BookPosition> expected = new List<BookPosition>() {
                new BookPosition(firstBook, 31),
            };
            CollectionAssert.AreEquivalent(expected.ToArray(), sut.Get("book").ToArray());
        }
    }

    [TestClass]
    public class BookCollectionIndexStorageTests
    {
        [TestMethod]
        public void BookCollectionIndexStorage_CanPersistIndexes()
        {
            var books = new BookCollectionIndex();
            var firstBook = new Uri("file://C:/firstFile");
            var secondBook = new Uri("file://C:/secondFile");
            books.Add(firstBook, new StringReader("this text comes from the first book"));
            books.Add(secondBook, new StringReader("this text book comes from the second book"));
            var storage = new BookCollectionIndexStorage();
            var stream = new MemoryStream();

            storage.Write(stream, books);
            stream.Position = 0;
            var readBooks = storage.Read(stream);

            IEnumerable<BookPosition> expected = new List<BookPosition>() {
                new BookPosition(secondBook, 37),
                new BookPosition(firstBook, 31),
                new BookPosition(secondBook, 10)
            };
            CollectionAssert.AreEquivalent(expected.ToArray(), readBooks.Get("book").ToArray());
        }
    }
}
