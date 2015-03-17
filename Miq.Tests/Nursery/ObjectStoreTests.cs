using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

namespace Miq.Tests.Nursery
{
	static class Extensions
	{
		public static IEnumerable<T> TakeAllButLast<T>(this IEnumerable<T> source)
		{
			var it = source.GetEnumerator();
			bool hasRemainingItems = false;
			bool isFirst = true;
			T item = default(T);

			do
			{
				hasRemainingItems = it.MoveNext();
				if (hasRemainingItems)
				{
					if (!isFirst) yield return item;
					item = it.Current;
					isFirst = false;
				}
			} while (hasRemainingItems);
		}

		public static string AllText(this Stream stream)
		{
			stream.Flush();
			stream.Position = 0;

			string result;
			using (var reader = new StreamReader(stream))
			{
				result = reader.ReadToEnd();
			}
			return result;
		}

		public static byte[] AllBytes(this Stream stream)
		{
			stream.Flush();
			stream.Position = 0;

			var bytes = new byte[stream.Length];
			stream.Read(bytes, 0, (int)stream.Length);
			return bytes;
		}

		public static IEnumerable<TreeNode> AllTreeNodes(this Stream stream)
		{
			stream.Flush();
			stream.Position = 0;

			using (var treeText = new StreamReader(stream))
			{
				string line;
				while ((line = treeText.ReadLine()) != null)
				{
					yield return TreeNode.Parse(line);
				}
			}
		}
	}

	enum TreeNodeType
	{
		File, Tree
	}

	class Tree : List<TreeNode>
	{
		public Tree()
		{ }

		public Tree(Stream stream)
		{
			AddRange(stream.AllTreeNodes());
		}

		public string Write(ObjectStore store)
		{
			WriteSubtrees(store);
			using (Stream treeFile = CreateTreeFile())
			{
				return store.Store(treeFile);
			}
		}

		private Stream CreateTreeFile()
		{
			Stream stream = new MemoryStream();
			TextWriter writer = new StreamWriter(stream);
			foreach (var item in this)
			{
				writer.WriteLine(item);
			}
			writer.Flush();
			stream.Position = 0;
			return stream;
		}

		private void WriteSubtrees(ObjectStore store)
		{
			foreach (TreeNode item in this.Where(i => i.Type == TreeNodeType.Tree))
			{
				item.Sha1 = item.Write(store);
			}
		}

		public void Add(string sha1, TreeNodeType type, string name)
		{
			string[] allPaths = SeparatePaths(name);
			if (allPaths.Length == 1)
			{
				Add(new TreeNode() { Sha1 = sha1, Name = allPaths[0], Type = type });
			}
			else
			{
				if (!this.Any(n => n.Name == allPaths[0]))
				{
					this.Add(new TreeNode() { Name = allPaths[0], Type = TreeNodeType.Tree, Tree = new Tree() });
				}

				TreeNode node = this.First(n => n.Name == allPaths[0]);
				node.Tree.Add(sha1, type, allPaths[1]);
			}
		}

		private static string[] SeparatePaths(string name)
		{
			var pathSeparators = new char[] { Path.DirectorySeparatorChar };
			return name.Split(pathSeparators, 2);
		}
	}

	class TreeNode
	{
		public static TreeNode Parse(string line)
		{
			var parts = line.Split(new char[] { ' ' }, 3);
			var node = new TreeNode()
			{
				Sha1 = parts[0],
				Type = (TreeNodeType)Enum.Parse(typeof(TreeNodeType), parts[1]),
				Name = parts[2]
			};
			return node;
		}

		public string Sha1 { get; set; }
		public TreeNodeType Type { get; set; }
		public string Name { get; set; }
		public Tree Tree { get; set; }

		public string Write(ObjectStore store)
		{
			if (Type != TreeNodeType.Tree)
			{
				throw new InvalidOperationException();
			}

			return Tree.Write(store);
		}

		public override string ToString()
		{
			return string.Format("{0} {1} {2}", Sha1, Type, Name);
		}
	}

	class ObjectStore
	{
		public ObjectStore()
		{
			var hitchhiker = DriveInfo.GetDrives().Where(drive => drive.IsReady).Single(drive => drive.VolumeLabel == "Hitchhiker");
			BasePath = Path.Combine(hitchhiker.Name, "Stuff", "KindleCompare", "ObjectStore");
		}

		public void AddHead(string sha1, string name)
		{
			// name must not contain new lines
			var headsPath = Path.Combine(BasePath, "HEADS");
			string[] allHeads = File.Exists(headsPath) ? File.ReadAllLines(headsPath) : new string[0];
			string newHead = string.Format("{0} {1}", sha1, name);
			if (allHeads.Contains(newHead))
			{
				return;
			}
			List<string> cleanHeads = allHeads.Where(l => !l.Contains(name)).ToList();
			cleanHeads.Add(newHead);
			File.WriteAllLines(headsPath, cleanHeads, Encoding.UTF8);
		}

		public string Store(Stream stream)
		{
			string sha1 = CalcSha1(stream);
			CreateDirectory(sha1);
			string objectFilePath = GetStorePath(sha1);
			if (!File.Exists(objectFilePath))
			{
				Compressfile(stream, objectFilePath);
			}
			return sha1;
		}

		public string Store(string path)
		{
			using (Stream stream = GetStream(path))
			{
				return Store(stream);
			}
		}

		public Stream Retrieve(string sha1)
		{
			string objectFilePath = GetStorePath(sha1);
			Stream stream = GetStream(objectFilePath);
			Stream output = new MemoryStream();
			using (var decompressStream = new GZipStream(stream, CompressionMode.Decompress))
			{
				decompressStream.CopyTo(output);
			}
			return output;
		}

		// ZZZ tree needs to know about the store, and the store needs to know about Tree :(
		public void StoreDirectory(string path, string headName)
		{
			Tree root = new Tree();
			foreach (var file in Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories))
			{
				string sha1 = Store(file);
				root.Add(sha1, TreeNodeType.File, file);
			}
			var rootsha1 = root.Write(this);
			AddHead(rootsha1, headName);
		}

		// ZZZ cleanup
		public IEnumerable<KeyValuePair<string, Stream>> RetrieveDirectory(string headName)
		{
			var trees = new Queue<Tuple<string, TreeNode>>();
			var tree = ResolveTree(GetHead(headName));
			TreeNode rootNode = tree.Single();
			var rootNodePath = rootNode.Name[rootNode.Name.Length - 1] == ':' ? rootNode.Name + '\\' : rootNode.Name;
			trees.Enqueue(new Tuple<string, TreeNode>(rootNodePath, rootNode));

			while (trees.Count != 0)
			{
				Tuple<string, TreeNode> t = trees.Dequeue();
				var path = t.Item1;
				var node = t.Item2;

				foreach (var item in node.Tree)
				{
					switch (item.Type)
					{
						case TreeNodeType.File:
							yield return new KeyValuePair<string, Stream>(Path.Combine(path, item.Name), Retrieve(item.Sha1));
							break;
						case TreeNodeType.Tree:
							trees.Enqueue(new Tuple<string, TreeNode>(Path.Combine(path, item.Name), item));
							break;
						default:
							throw new InvalidOperationException("something went wrong");
					}
				}
			}
		}

		private Tree ResolveTree(string sha1)
		{
			var tree = new Tree(Retrieve(sha1));
			foreach (var subtree in tree.Where(n => n.Type == TreeNodeType.Tree))
			{
				subtree.Tree = ResolveTree(subtree.Sha1);
			}
			return tree;
		}

		private string GetHead(string headName)
		{
			var head = File.ReadAllLines(Path.Combine(BasePath, "HEADS")).First(l => l.Contains(headName));
			var sha1 = head.Split(' ').First();
			return sha1;
		}

		Stream GetStream(string filename)
		{
			Stream stream;
			try
			{
				stream = System.IO.File.OpenRead(filename);
			}
			catch (PathTooLongException)
			{
				string formattedName = @"\\?\" + filename;
				SafeFileHandle handle = NativeMethods.CreateFile(formattedName, (int)0x10000000, FileShare.None, null, FileMode.Open, (int)0x00000080, IntPtr.Zero);
				if (handle.IsInvalid)
				{
					throw new System.ComponentModel.Win32Exception();
				}
				stream = new FileStream(handle, FileAccess.Read);
			}
			return stream;
		}

		string CalcSha1(Stream stream)
		{
			SHA1 sha = SHA1.Create();
			stream.Position = 0;
			byte[] result = sha.ComputeHash(stream);
			stream.Position = 0;
			string sha1 = ByteArrayToHexViaLookup32(result);
			return sha1;
		}

		string BasePath;

		void CreateDirectory(string sha1)
		{
			string directory = Path.Combine(BasePath, sha1.Substring(0, 2));
			Directory.CreateDirectory(directory);
		}

		void Compressfile(Stream stream, string objectFilePath)
		{
			Stream compressed = new FileStream(objectFilePath, FileMode.CreateNew);
			using (Stream compression = new GZipStream(compressed, CompressionMode.Compress))
			{
				stream.CopyTo(compression);
			}
		}

		string GetStorePath(string sha1)
		{
			string objectFilePath = Path.Combine(BasePath,
										sha1.Substring(0, 2),
										sha1.Substring(2));
			return objectFilePath;
		}

		readonly uint[] _lookup32 = CreateLookup32();

		static uint[] CreateLookup32()
		{
			var result = new uint[256];
			for (int i = 0; i < 256; i++)
			{
				string s = i.ToString("X2");
				result[i] = ((uint)s[0]) + ((uint)s[1] << 16);
			}
			return result;
		}

		string ByteArrayToHexViaLookup32(byte[] bytes)
		{
			var lookup32 = _lookup32;
			var result = new char[bytes.Length * 2];
			for (int i = 0; i < bytes.Length; i++)
			{
				var val = lookup32[bytes[i]];
				result[2 * i] = (char)val;
				result[2 * i + 1] = (char)(val >> 16);
			}
			return new string(result);
		}
	}

	static class NativeMethods
	{
		[StructLayout(LayoutKind.Sequential)]
		public class SECURITY_ATTRIBUTES
		{
			public int nLength;
			public IntPtr pSecurityDescriptor;
			public int bInheritHandle;
		}

		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern SafeFileHandle CreateFile(string lpFileName, int dwDesiredAccess, FileShare dwShareMode, SECURITY_ATTRIBUTES securityAttrs, FileMode dwCreationDisposition, int dwFlagsAndAttributes, IntPtr hTemplateFile);

	}

	[TestClass]
	public class ObjectStoreTests
	{
		[TestMethod]
		public void ObjectsAreStoredAndRetrieved()
		{
			var input = new MemoryStream(Encoding.ASCII.GetBytes("Hello, World!"));

			var store = new ObjectStore();
			var token = store.Store(input);
			var output = store.Retrieve(token).AllText();

			Assert.AreEqual("Hello, World!", output);
		}

		[TestMethod]
		[TestCategory("Integration")]
		public void FullFoldersCanBeStoredAndRetrieved()
		{
			var path = @"H:\Stuff\sqlite\sqlite-src-3080600\src";

			var store = new ObjectStore();
			store.StoreDirectory(path, "sqlite sources");

			var originalFiles = Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories).OrderBy(p => p);
			var storedFiles = store.RetrieveDirectory("sqlite sources").OrderBy(i => i.Key);
			var storedFilesEnum = storedFiles.GetEnumerator();

			foreach (var fname in originalFiles)
			{
				storedFilesEnum.MoveNext();
				var storedFile = storedFilesEnum.Current;

				Assert.AreEqual(fname, storedFile.Key);

				var originalContent = File.ReadAllBytes(fname);
				var storedContent = storedFile.Value.AllBytes();
				CollectionAssert.AreEqual(originalContent, storedContent);
			}
		}

		// ZZZ add code to find diferences between two heads.
	}
}
