using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;
using System.Threading;

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
}
