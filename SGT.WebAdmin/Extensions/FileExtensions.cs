namespace SGT.WebAdmin.Extensions
{
    public static class FileExtensions
    {
        public static string ConvertArquivoRetorno(byte[] arquivo, string extensao)
        {
            if (arquivo != null)
            {
                return "application/octet-stream";
            }
            else
            {
                return "";
            }
        }
    }
}
