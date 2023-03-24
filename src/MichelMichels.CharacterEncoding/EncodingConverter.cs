using System.Text;

namespace MichelMichels.CharacterEncoding
{
    public class EncodingConverter : IEncodingConverter
    {
        public byte[] ConvertBytes(byte[] bytes, Encoding inputEncoding, Encoding outputEncoding)
        {
            var inputText = inputEncoding.GetString(bytes);            
            
            return ConvertString(inputText, outputEncoding);
        }

        public byte[] ConvertString(string text, Encoding outputEncoding)
        {
            return outputEncoding.GetBytes(text);
        }     
    }
}
