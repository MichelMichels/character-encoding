using Microsoft.VisualStudio.TestTools.UnitTesting;
using MichelMichels.CharacterEncoding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MichelMichels.CharacterEncoding.Tests
{
    [TestClass()]
    public class EncodingConverterTests
    {
        [TestMethod()]
        public void ConvertStringTest()
        {
            // Arrange
            var converter = new EncodingConverter();

            // Act
            var bytes = converter.ConvertString("hello", Encoding.ASCII);

            // Assert
            var expectedValue = new byte[] { 0x68, 0x65, 0x6c, 0x6c, 0x6f };
            Assert.IsTrue(ByteArrayCompare(bytes, expectedValue));
        }

        [TestMethod]
        public void ConvertBytesTest()
        {
            // Arrange
            var converter = new EncodingConverter();

            // Act
            var input = Encoding.Unicode.GetBytes("hello");
            var bytes = converter.ConvertBytes(input, Encoding.Unicode, Encoding.ASCII);

            // Assert
            var expectedValue = new byte[] { 0x68, 0x65, 0x6c, 0x6c, 0x6f };
            Assert.IsTrue(ByteArrayCompare(bytes, expectedValue));
        }

        static bool ByteArrayCompare(ReadOnlySpan<byte> a1, ReadOnlySpan<byte> a2)
        {
            return a1.SequenceEqual(a2);
        }
    }
}