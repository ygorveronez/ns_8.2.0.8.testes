using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Excel;

namespace Servicos
{
    public static class Arquivo
    {

        #region Métodos privados

        private static string SalvarArquivo(Stream arquivo, string caminhoSalvar, string extensao)
        {
            string nomeArquivo = Guid.NewGuid().ToString();

            caminhoSalvar = Utilidades.IO.FileStorageService.Storage.Combine(caminhoSalvar, string.Concat(nomeArquivo, extensao));

            using (Stream fileStream = Utilidades.IO.FileStorageService.Storage.OpenWrite(caminhoSalvar))
                arquivo.CopyTo(fileStream);

            arquivo.Close();
            arquivo.Dispose();

            return nomeArquivo;
        }

        #endregion Métodos privados

        #region Métodos públicos      

        /// <summary>
        /// Determines a text file's encoding by analyzing its byte order mark (BOM).
        /// Defaults to ASCII when detection of the text file's endianness fails.
        /// </summary>
        /// <param name="file">The text file stream to analyze.</param>
        /// <returns>The detected encoding.</returns>
        public static Encoding GetEncoding(Stream file)
        {
            // Read the BOM
            var bom = new byte[4];

            file.Read(bom, 0, 4);

            file.Position = 0;

            // Analyze the BOM
            if (bom[0] == 0x2b && bom[1] == 0x2f && bom[2] == 0x76)
                return Encoding.UTF7;

            if (bom[0] == 0xef && bom[1] == 0xbb && bom[2] == 0xbf)
                return Encoding.UTF8;

            if (bom[0] == 0xff && bom[1] == 0xfe)
                return Encoding.Unicode; //UTF-16LE

            if (bom[0] == 0xfe && bom[1] == 0xff)
                return Encoding.BigEndianUnicode; //UTF-16BE

            if (bom[0] == 0 && bom[1] == 0 && bom[2] == 0xfe && bom[3] == 0xff)
                return Encoding.UTF32;

            return null;
        }

        public static Encoding GetEncoding(string filename)
        {
            // Read the BOM
            var bom = new byte[4];
            using (Stream file = Utilidades.IO.FileStorageService.Storage.OpenRead(filename))
            {
                file.Read(bom, 0, 4);
            }

            // Analyze the BOM
            if (bom[0] == 0x2b && bom[1] == 0x2f && bom[2] == 0x76)
                return Encoding.UTF7;

            if (bom[0] == 0xef && bom[1] == 0xbb && bom[2] == 0xbf)
                return Encoding.UTF8;

            if (bom[0] == 0xff && bom[1] == 0xfe)
                return Encoding.Unicode; //UTF-16LE

            if (bom[0] == 0xfe && bom[1] == 0xff)
                return Encoding.BigEndianUnicode; //UTF-16BE

            if (bom[0] == 0 && bom[1] == 0 && bom[2] == 0xfe && bom[3] == 0xff)
                return Encoding.UTF32;

            return Encoding.Default;
        }

        /// <summary>
        /// Determines a text file's encoding by analyzing its byte order mark (BOM).
        /// Defaults to ASCII when detection of the text file's endianness fails.
        /// </summary>
        /// <param name="file">The text file stream to analyze.</param>
        /// <returns>The detected encoding.</returns>
        public static Encoding GetEncodingOrDefault(Stream file)
        {
            // Read the BOM
            var bom = new byte[4];

            file.Read(bom, 0, 4);

            file.Position = 0;

            // Analyze the BOM
            if (bom[0] == 0x2b && bom[1] == 0x2f && bom[2] == 0x76)
                return Encoding.UTF7;

            if (bom[0] == 0xef && bom[1] == 0xbb && bom[2] == 0xbf)
                return Encoding.UTF8;

            if (bom[0] == 0xff && bom[1] == 0xfe)
                return Encoding.Unicode; //UTF-16LE

            if (bom[0] == 0xfe && bom[1] == 0xff)
                return Encoding.BigEndianUnicode; //UTF-16BE

            if (bom[0] == 0 && bom[1] == 0 && bom[2] == 0xfe && bom[3] == 0xff)
                return Encoding.UTF32;

            return Encoding.Default;
        }

        public static void Mover(string pathFileFrom, string pathFileTo)
        {
            string path = Path.GetDirectoryName(pathFileTo);

            if (Utilidades.IO.FileStorageService.Storage.Exists(pathFileTo))
                pathFileTo = Utilidades.IO.FileStorageService.Storage.Combine(path, string.Concat(Path.GetFileNameWithoutExtension(pathFileTo), "_", Path.GetRandomFileName(), Path.GetExtension(pathFileTo)));

            Utilidades.IO.FileStorageService.Storage.Move(pathFileFrom, pathFileTo);
        }

        public static List<List<string>> ObterArquivoExcel(Stream stream, bool isArquivoXLSX)
        {
            IExcelDataReader excelReader = isArquivoXLSX ? ExcelReaderFactory.CreateOpenXmlReader(stream) : ExcelReaderFactory.CreateBinaryReader(stream);
            var linhas = new List<List<string>>();

            while (excelReader.Read())
            {
                int numeroDeCelulas = excelReader.FieldCount;
                List<string> celulas = new List<string>();

                for (var j = 0; j < numeroDeCelulas; j++)
                {
                    string cell = excelReader.GetString(j);
                    celulas.Add(!string.IsNullOrWhiteSpace(cell) ? cell : "");
                }

                linhas.Add(celulas);
            }

            excelReader.Close();

            return linhas;
        }

        public static List<List<string>> ObterArquivoCSV(Stream stream, char separador = ';')
        {
            StreamReader csvreader = new StreamReader(stream);

            var linhas = new List<List<String>>();
            int j = 0;

            while (!csvreader.EndOfStream)
            {
                var line = csvreader.ReadLine();
                var values = line.Split(separador);
                List<String> celulas = new List<string>();

                for (j = 0; j < values.Length; j++)
                {
                    celulas.Add(values[j]);
                }

                linhas.Add(celulas);
            }

            csvreader.Close();

            return linhas;
        }

        public static string RemoveTroublesomeCharacters(string inString)
        {
            if (inString == null) return null;

            StringBuilder newString = new StringBuilder();
            char ch;

            for (int i = 0; i < inString.Length; i++)
            {

                ch = inString[i];
                // remove any characters outside the valid UTF-8 range as well as all control characters
                // except tabs and new lines
                if ((ch < 0x00FD && ch > 0x001F) || ch == '\t' || ch == '\n' || ch == '\r')
                {
                    newString.Append(ch);
                }
            }
            return newString.ToString();

        }

        public static string SalvarArquivoPDF(Stream arquivo, string caminhoSalvar)
        {
            return SalvarArquivo(arquivo, caminhoSalvar, ".pdf");
        }

        public static string SalvarArquivoJPG(Stream arquivo, string caminhoSalvar)
        {
            return SalvarArquivo(arquivo, caminhoSalvar, ".jpg");
        }

        #endregion

    }
}
