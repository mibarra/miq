using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Security.Cryptography;
using System.IO;
using System.Text;
using System.IO.Compression;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;

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
	}

	enum TreeNodeType
	{
		File, Tree
	}

	class Tree : Dictionary<string, TreeNode>
	{
		public string Write(ObjectStore store)
		{
			string sha1;
			foreach (var item in this.Values.Where(i => i.Type == TreeNodeType.Tree))
			{
				var sha1item = item.Tree.Write(store);
				item.Sha1 = sha1item;
			}

			using (Stream stream = new MemoryStream())
			{
				TextWriter writer = new StreamWriter(stream);
				foreach (var item in this.Values)
				{
					writer.WriteLine("{0} {1} {2}", item.Sha1, item.Type, item.Name);
				}

				stream.Position = 0;
				sha1 = store.StoreFromStream(stream);
			}

			return sha1;
		}

		public void Add(string sha1, TreeNodeType type, string name)
		{
			var seps = new char[] { Path.DirectorySeparatorChar };
			var x = name.Split(seps, 2);
			if (x.Length == 1)
			{
				this.Add(x[0], new TreeNode() { Sha1 = sha1, Name = x[0], Type = type });
			}
			else
			{
				// tree exists?, if not, create
				if (!this.ContainsKey(x[0]))
				{
					var newTree = new TreeNode() { Name = x[0], Type = TreeNodeType.Tree };
					this.Add(x[0], newTree);
					newTree.Tree = new Tree();

				}
				this[x[0]].Tree.Add(sha1, type, x[1]);
			}
		}
	}

	class TreeNode
	{
		public string Sha1 { get; set; }
		public TreeNodeType Type { get; set; }
		public string Name { get; set; }
		public Tree Tree { get; set; }
	}

	class ObjectStore
	{

		internal string StoreFromStream(Stream stream)
		{
			string sha1 = CalcSha1(stream);
			CreateDirectory(sha1);
			string objectFilePath = GetStorePath(sha1);

			try
			{
				Compressfile(stream, objectFilePath);
			}
			catch (IOException) // Assuming exception is always: file already exists
			{
			}

			return sha1;
		}

		public string StoreFromFile(string filename)
		{
			using (Stream stream = GetStream(filename))
			{
				return StoreFromStream(stream);
			}
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

		void CreateDirectory(string sha1)
		{
			string directory = Path.Combine(
										@"H:\Stuff\KindleCompare\ObjectStore",
										sha1.Substring(0, 2));
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
			string objectFilePath = Path.Combine(
										@"H:\Stuff\KindleCompare\ObjectStore",
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
		public void TestMethod1()
		{
			string path = @"E:\system";

			ObjectStore store = new ObjectStore();
			Tree root = new Tree();
			foreach (var file in Directory.EnumerateFiles(path, "*.*", SearchOption.AllDirectories))
			{
				string sha1 = store.StoreFromFile(file);
				Debug.WriteLine("{0} {1}", sha1, file);
				root.Add(sha1, TreeNodeType.File, file);
			}
			var rootsha1 = root.Write(store);
		}
	}
}
