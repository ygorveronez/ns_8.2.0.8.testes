using System.Linq;

namespace Utilidades
{
    public class Encoding
    {
        public static System.Text.Encoding ObterEncoding(string encodingName)
        {
            if (!string.IsNullOrWhiteSpace(encodingName))
            {
                System.Text.EncodingInfo encodingInfo = System.Text.Encoding.GetEncodings().FirstOrDefault(info => info.Name == encodingName);

                if (encodingInfo != null)
                    return encodingInfo.GetEncoding();
            }

            return null;
        }
    }
}
