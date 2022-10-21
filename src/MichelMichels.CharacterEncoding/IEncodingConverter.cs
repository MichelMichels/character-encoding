using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MichelMichels.CharacterEncoding
{
    public interface IEncodingConverter
    {
        /// <summary>
        /// Converts the byte array with a specified input encoding to a byte array with the same characters
        /// encoded with the output encoding.
        /// </summary>
        /// <param name="bytes">Input byte array</param>
        /// <param name="inputEncoding">Character encoding of the input.</param>
        /// <param name="outputEncoding">Character encoding of the output.</param>
        /// <returns>A byte array encoded with the outputEncoding.</returns>
        byte[] Convert(byte[] bytes, Encoding inputEncoding, Encoding outputEncoding);

       
        /// <summary>
        /// Converts a UTF-16 string to a byte array encoded with the specified output encoding.
        /// </summary>
        /// <param name="input">UTF-16 string</param>
        /// <param name="outputEncoding">Character encoding of the output.</param>
        /// <returns></returns>
        byte[] Convert(string input, Encoding outputEncoding);
    }
}
