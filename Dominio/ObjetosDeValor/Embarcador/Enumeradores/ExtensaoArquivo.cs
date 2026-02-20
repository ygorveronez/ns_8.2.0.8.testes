using System.IO;

namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum ExtensaoArquivo
    {
        NaoEncontrado = 0,
        GIF = 1,
        TIF = 2,
        JPG = 3,
        PNG = 4,
        PDF = 5,
        DOC = 6,
        DOCX = 7,
        XLS = 8,
        XLSX = 9,
        ZIP = 10,
        RAR = 11,
        JSON = 12
    }

    public static class ExtensaoArquivoHelper
    {
        public static bool IsArquivoBinario(this ExtensaoArquivo extensaoArquivo)
        {
            switch (extensaoArquivo)
            {
                case ExtensaoArquivo.PDF:
                case ExtensaoArquivo.XLS:
                case ExtensaoArquivo.XLSX:
                    return true;

                default: return false;
            }
        }

        public static ExtensaoArquivo ObterExtensaoArquivo(string nomeArquivo)
        {
            if (string.IsNullOrWhiteSpace(nomeArquivo))
                return ExtensaoArquivo.NaoEncontrado;

            string extensao = Path.GetExtension(nomeArquivo).Replace(".", "").ToLower();

            switch (extensao)
            {
                case "gif": return ExtensaoArquivo.GIF;
                case "tiff": return ExtensaoArquivo.TIF;
                case "tif": return ExtensaoArquivo.TIF;
                case "jpeg": return ExtensaoArquivo.JPG;
                case "jpg": return ExtensaoArquivo.JPG;
                case "png": return ExtensaoArquivo.PNG;
                case "pdf": return ExtensaoArquivo.PDF;
                case "doc": return ExtensaoArquivo.DOC;
                case "docx": return ExtensaoArquivo.DOCX;
                case "xls": return ExtensaoArquivo.XLS;
                case "xlsx": return ExtensaoArquivo.XLSX;
                case "zip": return ExtensaoArquivo.ZIP;
                case "rar": return ExtensaoArquivo.RAR;
                case "json": return ExtensaoArquivo.JSON;
                default: return ExtensaoArquivo.NaoEncontrado;
            }
        }
    }
}
