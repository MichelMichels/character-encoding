using System.Text;

namespace MichelMichels.CharacterEncoding
{
    public class EncodingConverter : IEncodingConverter
    {
        public byte[] Convert(byte[] bytes, Encoding inputEncoding, Encoding outputEncoding)
        {
            var inputText = inputEncoding.GetString(bytes);            
            
            return Convert(inputText, outputEncoding);
        }

        public byte[] Convert(string text, Encoding outputEncoding)
        {
            return outputEncoding.GetBytes(text);
        }     
    }
}
