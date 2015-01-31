using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Text;

namespace Miq.Tests.Nursery.MobiDeDRM
{
    public class MobiReader
    {
        public MobiReader(string path)
        {
            _FileContents = System.IO.File.ReadAllBytes(path);
        }

        public SectionTable Sections
        {
            get
            {
                if (_Sections == null)
                {
                    var numberOfSections = BitConverter.ToUInt16(_FileContents.Skip(76).Take(2).Reverse().ToArray(), 0);

                    _Sections = new SectionTable();
                    for (int i = 0; i < numberOfSections; i++)
                    {
                        _Sections.Add(new Section(_FileContents.Skip(78).Skip(i * 8).Take(8), _FileContents));
                    }

                    for (int i = 0; i < numberOfSections; i++)
                    {
                        var section = _Sections.ElementAt(i);
                        var nextSection = _Sections.ElementAtOrDefault(i + i);

                        var end = nextSection != null ? nextSection.Offset : (uint)_FileContents.Length;
                        section.Length = end - section.Offset;
                    }
                }
                return _Sections;
            }
        }

        public string Magic { get { return System.Text.Encoding.ASCII.GetString(_FileContents.Skip(0x3c).Take(8).ToArray()); } }
        public EncryptionType EncryptionType
        {
            get
            {
                var code = BitConverter.ToUInt16(Sections[0].Bytes.Skip(12).Take(2).Reverse().ToArray(), 0);
                EncryptionType type;
                switch (code)
                {
                    case 0: type = Nursery.MobiDeDRM.EncryptionType.NoEncryption; break;
                    case 1: type = Nursery.MobiDeDRM.EncryptionType.MobiPocketEncryption; break;
                    case 2: type = Nursery.MobiDeDRM.EncryptionType.AmazonEncryption; break;
                    default: type = Nursery.MobiDeDRM.EncryptionType.Unknown; break;
                }
                return type;
            }
        }

        public String StringMetaData(uint metadataCode)
        {
            if (_Metadata == null)
            {
                var firstSection = Sections[0];
                var exthOffset = firstSection.Offset + 16 + firstSection.MobiLength;
                var endOffset = Sections[1].Offset;

                _Metadata = new List<KeyValuePair<UInt32, IEnumerable<Byte>>>();
                if ((firstSection.ExthSectionFlags & 0x40) != 0 && endOffset - exthOffset >= 12)
                {
                    IEnumerable<Byte> exth = _FileContents.Skip((int)exthOffset);

                    var nitems = BitConverter.ToUInt32(exth.Skip(8).Take(4).Reverse().ToArray(), 0);
                    int pos = 12;
                    for (int i = 0; i < nitems; i++)
                    {
                        var type = BitConverter.ToUInt32(exth.Skip(pos).Take(4).Reverse().ToArray(), 0);
                        var size = BitConverter.ToUInt32(exth.Skip(pos + 4).Take(4).Reverse().ToArray(), 0);
                        var content = exth.Skip(pos + 8).Take((int)size - 8);
                        pos += (int)size;
                        _Metadata.Add(new KeyValuePair<UInt32, IEnumerable<Byte>>(type, content));
                    }
                }
            }

            var metadataContents = _Metadata.FirstOrDefault(kvp => kvp.Key == metadataCode);
            if (metadataContents.Key != metadataCode)
            {
                return null;
            }

            return System.Text.Encoding.UTF8.GetString(metadataContents.Value.ToArray());
        }

        byte[] _FileContents;
        SectionTable _Sections;
        List<KeyValuePair<UInt32, IEnumerable<Byte>>> _Metadata;
    }

    public class Section
    {
        public Section(IEnumerable<Byte> data, byte[] fileContents)
        {
            Offset = BitConverter.ToUInt32(data.Take(4).Reverse().ToArray(), 0);
            Flags = data.ElementAt(4);
            Value = data.ElementAt(5) << 16 | data.ElementAt(6) << 8 | data.ElementAt(7);

            RawContents = fileContents;
        }

        public IEnumerable<byte> Bytes { get { return RawContents.Skip((int)Offset); } }

        public UInt32 Offset { get; set; }
        public Byte Flags { get; set; }
        public Int32 Value { get; set; }

        public UInt16 NumberOfRecords { get { return UInt16At(8); } }
        public UInt16 CompressionCode { get { return UInt16At(0); } }
        public UInt32 MobiLength { get { return UInt32At(0x14); } }
        public UInt32 CodePage { get { return UInt32At(0x1c); } }
        public UInt32 Version { get { return UInt32At(0x68); } }
        public UInt16 ExtraDataFlags
        {
            get
            {
                UInt16 flags = 0;

                if (MobiLength >= 0xe4 && Version >= 5)
                {
                    flags = UInt16At(0xF2);
                }

                if (CompressionCode != 17480)
                {
                    flags &= 0xfffe;
                }

                return flags;
            }
        }

        private UInt16 UInt16At(int dataOfffset)
        {
            return BitConverter.ToUInt16(RawContents.Skip((int)Offset).Skip(dataOfffset).Take(2).Reverse().ToArray(), 0);
        }

        private UInt32 UInt32At(int dataOfffset)
        {
            return BitConverter.ToUInt32(RawContents.Skip((int)Offset).Skip(dataOfffset).Take(4).Reverse().ToArray(), 0);
        }

        private byte[] RawContents;

        public UInt32 ExthSectionFlags { get { return UInt32At(0x80); } }


        public uint Length { get; set; }
    }

    public class SectionTable : List<Section>
    {
    }

    public enum EncryptionType
    {
        Unknown = -1,
        NoEncryption,
        MobiPocketEncryption,
        AmazonEncryption
    }

    [TestClass]
    public class MobiDeDRM
    {
        [TestMethod]
        public void NotEncryptedBook()
        {
            MobiReader grimmStories = new MobiReader("C:\\Users\\Miguel\\Documents\\My Kindle Content\\B000JML1QG_EBOK.azw");

            Assert.AreEqual("BOOKMOBI", grimmStories.Magic);
            Assert.AreEqual(76, grimmStories.Sections.Count);
            Assert.AreEqual(EncryptionType.NoEncryption, grimmStories.EncryptionType, "Grimm's Fairy Stories is not encrypted");

            var firstSection = grimmStories.Sections[0];

            Assert.AreEqual(66, firstSection.NumberOfRecords);
            Assert.AreEqual(2, firstSection.CompressionCode);
            Assert.AreEqual(232u, firstSection.MobiLength);
            Assert.AreEqual(65001u, firstSection.CodePage);
            Assert.AreEqual(6u, firstSection.Version);
            Assert.AreEqual(2, firstSection.ExtraDataFlags);
            Assert.AreEqual(80u, firstSection.ExthSectionFlags);

            Assert.AreEqual("Grimm's Fairy Stories", grimmStories.StringMetaData(503));
        }

        [TestMethod]
        public void EncryptedBook()
        {
            MobiReader book = new MobiReader("C:\\Users\\Miguel\\Documents\\My Kindle Content\\B000FBFNII_EBOK.azw");
            Assert.AreEqual("A Short History of Nearly Everything", book.StringMetaData(503));
            Assert.AreEqual(EncryptionType.AmazonEncryption, book.EncryptionType);
        }
    }

    public interface IRead
    {
        void Read(BinaryReader binaryReader);
    }

    public class FixedString : IRead
    {
        public int Size { get; set; }
        public String Value { get { return _Value; } set { _Value = value; } }

        public override string ToString()
        {
            return _Value;
        }

        public void Read(BinaryReader reader)
        {
            byte[] bytes = reader.ReadBytes(Size);
            int firstNullIndex = Array.IndexOf(bytes, (byte)0);
            if (firstNullIndex == -1)
            {
                firstNullIndex = Size;
            }
            Value = Encoding.ASCII.GetString(bytes, 0, firstNullIndex);
        }

        private String _Value;
    }

    public class PalmDBName : FixedString
    {
        public PalmDBName()
            : base()
        {
            Size = 32;
        }
    }

    public class PalmDbIdent : FixedString
    {
        public PalmDbIdent()
            : base()
        {
            Size = 8;
        }
    }

    public class SectionHeaderItem : IRead
    {
        public UInt32 Offset { get; set; }
        public byte Flags { get; set; }
        public UInt32 Unknown { get; set; }

        public void Read(BinaryReader reader)
        {
            Offset = GenericReader.ReadUInt32(reader);
            Flags = GenericReader.ReadByte(reader);
            byte[] x = GenericReader.ReadByteArray(3, reader);
            Unknown = (uint)(x[0] << 16 | x[1] << 8 | x[2]);
        }
    }

    public class AzwFile : IRead
    {
        public AzwFile()
        {
            Name = new PalmDBName();
            Ident = new PalmDbIdent();
            DocHeader = new PalmDocHeader();
        }

        public PalmDBName Name { get; set; }
        public Byte[] Unknown1 { get; set; }
        public PalmDbIdent Ident { get; set; }
        public Byte[] Unknown2 { get; set; }
        public UInt16 SectionCount { get; set; }
        public SectionHeaderItem[] SectionHeaders { get; set; }
        public Byte[] Unknown3 { get; set; }

        // this is really part of the first section
        public PalmDocHeader DocHeader { get; set; }

        public void Read(BinaryReader reader)
        {
            Name.Read(reader);                                    //  0,32
            Unknown1 = GenericReader.ReadByteArray(28, reader);   // 32,28
            Ident.Read(reader);                                   // 60, 8
            if(Ident.Value != "BOOKMOBI")
            {
                return;
            }
            Unknown2 = GenericReader.ReadByteArray(8, reader);    // 68, 8
            SectionCount = GenericReader.ReadUInt16(reader);      // 76, 2
            SectionHeaders = new SectionHeaderItem[SectionCount]; // 78
            for (int i = 0; i < SectionCount; i++)
            {
                SectionHeaders[i] = new SectionHeaderItem();
                SectionHeaders[i].Read(reader);
            }
            Unknown3 = GenericReader.ReadByteArray(2, reader);

            // This is part of section 0
            DocHeader.Read(reader);                              // 0, 16
            //  16, 4 doc type
            //  20, 4 length
            //  24, 4 type
            //  28, 4 code page
            //  32, 4 unique id
            //  36, 4 version

            //  84, 4 title offset
            //  88, 4 title end
            //  92, 4 language code

            // 104, 4 mobi version
            // 108, 4 first image index

            // 112, 4 huff ofset  iff compression type == DH
            // 116, 4 huff number iff compression type == DH

            // 128, 4 exth flag

            // 192, 4 fdst idx
            // 196, 4 fdst cnt

            // 242, 2 extra data flags; only when mobi length > 0xe4 and mobi version >= 5
            // 244, 4 ncx idx 
            // 248, 4 div idx
            // 252, 4 skel idx
            // 256, 4 dat p idx
            // 260, 4 oth idx

            // EXTH exists (at 16 + mobi length + section 0 offset, [to the end of the section]) iff:
            //   exth flag & 0x40
        }
    }

    public class PalmDocHeader : IRead
    {
        public UInt16 CompressionType { get; private set; }
        public UInt16 Unused { get; private set; }
        public UInt32 Length { get; private set; }
        public UInt16 RecordCount { get; private set; }
        public UInt16 RecordSize { get; private set; }
        public UInt16 EncryptionType { get; private set; }
        public UInt16 Unknown { get; private set; }

        public void Read(BinaryReader reader)
        {
            CompressionType = GenericReader.ReadUInt16(reader);
            Unused = GenericReader.ReadUInt16(reader);
            Length = GenericReader.ReadUInt32(reader);
            RecordCount = GenericReader.ReadUInt16(reader);
            RecordSize = GenericReader.ReadUInt16(reader);
            EncryptionType = GenericReader.ReadUInt16(reader);
            Unknown = GenericReader.ReadUInt16(reader);
        }
    }

    static class GenericReader
    {
        public static UInt16 ReadUInt16(BinaryReader binaryReader)
        {
            byte[] ReadBuffer = new byte[2];
            binaryReader.Read(ReadBuffer, 0, 2);
            Array.Reverse(ReadBuffer);
            var value = BitConverter.ToUInt16(ReadBuffer, 0);
            return value;
        }

        public static UInt32 ReadUInt32(BinaryReader binaryReader)
        {
            byte[] ReadBuffer = new byte[4];
            binaryReader.Read(ReadBuffer, 0, 4);
            Array.Reverse(ReadBuffer);
            var value = BitConverter.ToUInt32(ReadBuffer, 0);
            return value;
        }

        public static byte[] ReadByteArray(int length, BinaryReader reader)
        {
            byte[] value = reader.ReadBytes(length);
            return value;
        }

        public static byte ReadByte(BinaryReader reader)
        {
            byte value = reader.ReadByte();
            return value;
        }
    }

    [TestClass]
    public class PalmDocTests
    {
        [TestMethod]
        public void foo()
        {
            foreach (var path in Directory.EnumerateFiles(@"C:\Users\Miguel\Documents\My Kindle Content", "*.azw"))
            {
                var file = new BinaryReader(File.OpenRead(path));
                AzwFile doc = new AzwFile();
                doc.Read(file);
                if (doc.Ident.Value != "BOOKMOBI")
                {
                    continue;
                }

                Debug.WriteLine("{0,20}\t{1,8}\t{2,8}",
                    Path.GetFileName(path), doc.SectionHeaders[1].Offset - doc.SectionHeaders[0].Offset, doc.DocHeader.Length);
            }
        }
    }

}
