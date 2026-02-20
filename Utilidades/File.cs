using ICSharpCode.SharpZipLib.Zip;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.RegularExpressions;

namespace Utilidades
{
    public class File
    {
        public static int ContarPaginasArquivoPDF(byte[] file)
        {
            int pageCount;
            MemoryStream stream = new MemoryStream(file);
            using (StreamReader reader = new StreamReader(stream))
            {
                string pdfText = reader.ReadToEnd();
                Regex regx = new Regex(@"/Type\s*/Page[^s]");
                MatchCollection matches = regx.Matches(pdfText);
                pageCount = matches.Count;
            }

            return pageCount;
        }

        public static MemoryStream GerarArquivoCompactado(Dictionary<string, string> conteudoCompactar)
        {
            Dictionary<string, byte[]> conteudoCompactarByte = new Dictionary<string, byte[]>();

            foreach (var conteudo in conteudoCompactar)
                conteudoCompactarByte.Add(conteudo.Key, System.Text.Encoding.Default.GetBytes(conteudo.Value));

            return GerarArquivoCompactado(conteudoCompactarByte);
        }

        public static MemoryStream GerarArquivoCompactado(Dictionary<string, byte[]> conteudoCompactar)
        {
            if ((conteudoCompactar == null) || (conteudoCompactar.Count == 0))
                return null;

            MemoryStream arquivoCompactado = new MemoryStream();
            ZipOutputStream compactadorArquivo = new ZipOutputStream(arquivoCompactado) { IsStreamOwner = false };

            compactadorArquivo.SetLevel(9);

            foreach (var conteudo in conteudoCompactar)
            {
                compactadorArquivo.PutNextEntry(new ZipEntry(conteudo.Key) { DateTime = System.DateTime.Now });
                compactadorArquivo.Write(conteudo.Value, 0, conteudo.Value.Length);
                compactadorArquivo.CloseEntry();
            }

            compactadorArquivo.Close();

            arquivoCompactado.Position = 0;

            return arquivoCompactado;
        }

        public static string GetValidFilename(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return fileName;

            char[] invalids = System.IO.Path.GetInvalidFileNameChars();

            foreach (char c in invalids)
            {
                fileName = fileName.Replace(c, '_');
            }

            return fileName;
        }

        public static byte[] GerarGZIP(byte[] originalByte)
        {
            string tempFile = Utilidades.IO.FileStorageService.Storage.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            string tempGZ = Utilidades.IO.FileStorageService.Storage.Combine(Path.GetTempPath(), Path.GetRandomFileName());

            Utilidades.IO.FileStorageService.Storage.WriteAllBytes(tempFile, originalByte);

            byte[] b;
            using (System.IO.Stream f = Utilidades.IO.FileStorageService.Storage.OpenRead(tempFile))
            {
                b = new byte[f.Length];
                f.Read(b, 0, (int)f.Length);
            }

            using (System.IO.Stream f2 = Utilidades.IO.FileStorageService.Storage.OpenWrite(tempGZ))
            using (GZipStream gz = new GZipStream(f2, CompressionMode.Compress, false))
            {
                gz.Write(b, 0, b.Length);
            }

            // Retornar os bytes
            return Utilidades.IO.FileStorageService.Storage.ReadAllBytes(tempGZ);
        }

        public static DataTable ConvertCSVtoDataTable(string strFilePath)
        {
            DataTable dt = new DataTable();
            using (StreamReader sr = new StreamReader(Utilidades.IO.FileStorageService.Storage.OpenRead(strFilePath)))
            {
                string line = sr.ReadLine();

                List<char> delimiters = new List<char> { ',', ';', '|' };

                Dictionary<char, int> counts = delimiters.ToDictionary(key => key, value => 0);

                foreach (char c in delimiters)
                    counts[c] = line.Count(t => t == c);

                char splitter = (from obj in counts orderby obj.Value descending select obj.Key).FirstOrDefault();

                string[] headers = line.Split(splitter);

                for (var i = 0; i < headers.Count(); i++)
                {
                    string header = headers[i];

                    if (dt.Columns.Contains(header))
                        header += "-" + Guid.NewGuid().ToString();

                    dt.Columns.Add(header);
                }

                while (!sr.EndOfStream)
                {
                    string[] rows = sr.ReadLine().Split(splitter);

                    if (rows.Length > 1)
                    {
                        DataRow dr = dt.NewRow();

                        for (int i = 0; i < headers.Length; i++)
                            dr[i] = rows[i].Trim().Replace("\"", "");

                        dt.Rows.Add(dr);
                    }
                }

            }

            return dt;
        }

        public static DataTable ConvertXSLXtoDataTable(string strFilePath, string connString)
        {
            using (OleDbConnection oledbConn = new OleDbConnection(connString))
            {
                DataTable dt = new DataTable();

                oledbConn.Open();

                DataTable dtSchema = oledbConn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });
                string sheet1 = dtSchema.Rows[0].Field<string>("TABLE_NAME");

                using (OleDbCommand cmd = new OleDbCommand("SELECT * FROM [" + sheet1 + "]", oledbConn)) // SQL-INJECTION-SAFE
                {
                    OleDbDataAdapter oleda = new OleDbDataAdapter();
                    oleda.SelectCommand = cmd;
                    DataSet ds = new DataSet();
                    oleda.Fill(ds);

                    dt = ds.Tables[0];
                }

                oledbConn.Close();

                return dt;
            }
        }

        public static void SalvarTextoEmArquivo(string arquivo, byte[] texto)
        {
            Utilidades.IO.FileStorageService.Storage.WriteAllBytes(arquivo, texto);
        }

        public static byte[] LerArquivo(string arquivo)
        {
            return Utilidades.IO.FileStorageService.Storage.ReadAllBytes(arquivo);
        }

        public static List<string> ListarArquivosDiretorio(string path)
        {
            return Utilidades.IO.FileStorageService.Storage.GetFiles(path).ToList();

        }

        public static void MoverArquivoParaDiretorio(string arquivoOrigem, string diretorioDestino)
        {
            var fileName = Path.GetFileName(arquivoOrigem);
            var destino = Utilidades.IO.FileStorageService.Storage.Combine(diretorioDestino, fileName);

            Utilidades.IO.FileStorageService.Storage.Move(arquivoOrigem, destino);
        }

        public static void RemoverArquivos(string diretorio)
        {
            IEnumerable<string> arquivos = Utilidades.IO.FileStorageService.Storage.GetFiles(diretorio);

            foreach (string arquivo in arquivos)
            {
                Utilidades.IO.FileStorageService.Storage.Delete(arquivo);
            }
        }
        public static void RemoverArquivo(string diretorio)
        {
            if (!Utilidades.IO.FileStorageService.Storage.Exists(diretorio))
                return;

            Utilidades.IO.FileStorageService.Storage.Delete(diretorio);
        }

        public static byte[] ReadToEnd(System.IO.Stream stream)
        {
            long originalPosition = 0;

            if (stream.CanSeek)
            {
                originalPosition = stream.Position;
                stream.Position = 0;
            }

            try
            {
                byte[] readBuffer = new byte[4096];

                int totalBytesRead = 0;
                int bytesRead;

                while ((bytesRead = stream.Read(readBuffer, totalBytesRead, readBuffer.Length - totalBytesRead)) > 0)
                {
                    totalBytesRead += bytesRead;

                    if (totalBytesRead == readBuffer.Length)
                    {
                        int nextByte = stream.ReadByte();
                        if (nextByte != -1)
                        {
                            byte[] temp = new byte[readBuffer.Length * 2];
                            Buffer.BlockCopy(readBuffer, 0, temp, 0, readBuffer.Length);
                            Buffer.SetByte(temp, totalBytesRead, (byte)nextByte);
                            readBuffer = temp;
                            totalBytesRead++;
                        }
                    }
                }

                byte[] buffer = readBuffer;
                if (readBuffer.Length != totalBytesRead)
                {
                    buffer = new byte[totalBytesRead];
                    Buffer.BlockCopy(readBuffer, 0, buffer, 0, totalBytesRead);
                }
                return buffer;
            }
            finally
            {
                if (stream.CanSeek)
                {
                    stream.Position = originalPosition;
                }
            }
        }

        public static byte[] MergeFiles(List<byte[]> sourceFiles)
        {
            try
            {
                return CombinarArquivosNovoMetodo(sourceFiles);
            }
            catch
            {
                return CombinarArquivosAntigoMetodo(sourceFiles);
            }
        }

        public static void CreateAppSettingsJsonIfNotExist(string diretorio)
        {
            string jsonFilePath = Utilidades.IO.FileStorageService.LocalStorage.Combine(diretorio, "appsettings.json");

            if (!Utilidades.IO.FileStorageService.LocalStorage.Exists(jsonFilePath))
            {
                var appSettings = ConfigurationManager.AppSettings;

                var jsonObject = new JObject();
                var appSettingsObject = new JObject();

                var allowedKeys = new[] { "Host", "DefaultCulture", "VersaoAplicativoMobile", "TempoSessao", "ValidarSenhaAutomatica", "UtilizaAppTrizy", "UtilizarURLCliente", "UtilizarHttps", "KillSessionAfterRequest", "CaminhoArquivos" };
                foreach (var key in appSettings.AllKeys)
                {
                    if (Array.Exists(allowedKeys, allowedKey => allowedKey.Equals(key, StringComparison.OrdinalIgnoreCase)))
                    {
                        appSettingsObject[key] = appSettings[key];
                    }
                }

                // Adiciona as chaves da seção ThreadProcessamentoDocumentoTransportePendentes
                var threadSettingsSection = ConfigurationManager.GetSection("ThreadProcessamentoDocumentoTransportePendentes") as System.Collections.Specialized.NameValueCollection;
                if (threadSettingsSection != null)
                {
                    foreach (var key in threadSettingsSection.AllKeys)
                    {
                        appSettingsObject[key] = threadSettingsSection[key];
                    }
                }
                jsonObject["AppSettings"] = appSettingsObject;

                // Adiciona as chaves da seção ThreadArquivosXmlNotaFiscaisPendentes
                var appSettingsObjectThreadArquivosXmlNotaFiscaisPendentes = new JObject();
                var threadSettingsSection2 = ConfigurationManager.GetSection("ThreadArquivosXmlNotaFiscaisPendentes") as System.Collections.Specialized.NameValueCollection;
                if (threadSettingsSection2 != null)
                {
                    foreach (var key in threadSettingsSection2.AllKeys)
                    {
                        appSettingsObjectThreadArquivosXmlNotaFiscaisPendentes[key] = threadSettingsSection2[key];
                    }
                    jsonObject["ThreadArquivosXmlNotaFiscaisPendentes"] = appSettingsObjectThreadArquivosXmlNotaFiscaisPendentes;
                }

                // Adiciona as chaves da seção ThreadArquivosXmlNotaFiscaisLiberacao
                var appSettingsObjectThreadArquivosXmlNotaFiscaisLiberacao = new JObject();
                var threadSettingsSection3 = ConfigurationManager.GetSection("ThreadArquivosXmlNotaFiscaisLiberacao") as System.Collections.Specialized.NameValueCollection;
                if (threadSettingsSection3 != null)
                {
                    foreach (var key in threadSettingsSection3.AllKeys)
                    {
                        appSettingsObjectThreadArquivosXmlNotaFiscaisLiberacao[key] = threadSettingsSection3[key];
                    }
                    jsonObject["ThreadArquivosXmlNotaFiscaisLiberacao"] = appSettingsObjectThreadArquivosXmlNotaFiscaisLiberacao;
                }

                // Adiciona as chaves da seção ThreadCargasPendentes
                var appSettingsObjectThreadCargasPendentes = new JObject();
                var threadSettingsSection4 = ConfigurationManager.GetSection("ThreadCargasPendentes") as System.Collections.Specialized.NameValueCollection;
                if (threadSettingsSection4 != null)
                {
                    foreach (var key in threadSettingsSection4.AllKeys)
                    {
                        appSettingsObjectThreadCargasPendentes[key] = threadSettingsSection4[key];
                    }
                    jsonObject["ThreadCargasPendentes"] = appSettingsObjectThreadCargasPendentes;
                }


                Utilidades.IO.FileStorageService.LocalStorage.WriteAllText(jsonFilePath, jsonObject.ToString());
            }
        }

        public static void CreateAppSettingsADIfNotExist(string diretorio)
        {
            string jsonFilePath = Utilidades.IO.FileStorageService.LocalStorage.Combine(diretorio, "appsettingsad.json");

            if (!Utilidades.IO.FileStorageService.LocalStorage.Exists(jsonFilePath))
            {
                var appSettings = System.Configuration.ConfigurationManager.AppSettings;

                var jsonObject = new JObject();
                var appSettingsObject = new JObject();

                var allowedKeys = new[] { "FormsAuthentication", "AzureAdAuthentication", "AzureAdDisplay", "ida:ClientId", "ida:ADFSDiscoveryDoc", "ida:ClientSecret", "ida:Tenant", "ida:AADInstance", "ida:PostLogoutRedirectUri", "AzureAdValidateUserGroup", "AzureAdUserGroupId", "LoginAD", "ServidorAD", "saml2:Authentication", "saml2:Display", "saml2:ClientId", "saml2:EndPoint", "saml2:Domain", "saml2:PathCertificate", "okta:Authentication", "okta:Display", "okta:EntityId", "okta:Certificado", "okta:MetadataLocation", "okta:ReturnUrl", "okta:SingleSignOnServiceUrl", "okta:IssuerUrl", "okta:SessionTime", "okta:HabilitaPortalTransportador", "okta:IssuerUrlPortalTransportador", "okta:ReturnUrlPortalTransportador" };
                foreach (var key in appSettings.AllKeys)
                {
                    if (Array.Exists(allowedKeys, allowedKey => allowedKey.Equals(key, StringComparison.OrdinalIgnoreCase)))
                    {
                        appSettingsObject[key] = appSettings[key];
                    }
                }

                jsonObject["AppSettings"] = appSettingsObject;

                Utilidades.IO.FileStorageService.LocalStorage.WriteAllText(jsonFilePath, jsonObject.ToString());
            }
        }

        #region Métodos Privados

        private static byte[] CombinarArquivosNovoMetodo(List<byte[]> sourceFiles)
        {
            using var ms = new MemoryStream();
            PdfSharpCore.Pdf.PdfDocument outputDocument = new();

            foreach (byte[] sourceFile in sourceFiles)
            {
                AdicionarPDFAoDocumento(sourceFile, outputDocument);
            }

            outputDocument.Save(ms);
            return ms.ToArray();
        }

        private static void AdicionarPDFAoDocumento(byte[] sourceFile, PdfSharpCore.Pdf.PdfDocument outputDocument)
        {
            if (sourceFile == null) return;

            using var tempStream = new MemoryStream(sourceFile);
            using var inputDocument = PdfSharpCore.Pdf.IO.PdfReader.Open(
                tempStream,
                PdfSharpCore.Pdf.IO.PdfDocumentOpenMode.Import);

            ImportarTodasAsPaginas(inputDocument, outputDocument);
        }

        private static void ImportarTodasAsPaginas(PdfSharpCore.Pdf.PdfDocument inputDocument, PdfSharpCore.Pdf.PdfDocument outputDocument)
        {
            for (int i = 0; i < inputDocument.PageCount; i++)
            {
                outputDocument.AddPage(inputDocument.Pages[i]);
            }
        }

        private static byte[] CombinarArquivosAntigoMetodo(List<byte[]> sourceFiles)
        {
            iTextSharp.text.Document document = new();
            using (MemoryStream ms = new MemoryStream())
            {
                iTextSharp.text.pdf.PdfCopy copy = new iTextSharp.text.pdf.PdfCopy(document, ms);
                document.Open();
                int documentPageCounter = 0;

                // Percorre todos os Documentos
                for (int fileCounter = 0; fileCounter < sourceFiles.Count; fileCounter++)
                {
                    // Cria leitor de PDF
                    iTextSharp.text.pdf.PdfReader reader = new iTextSharp.text.pdf.PdfReader(sourceFiles[fileCounter]);
                    int numberOfPages = reader.NumberOfPages;

                    // Percorre todas as páginas do Documento
                    for (int currentPageIndex = 1; currentPageIndex <= numberOfPages; currentPageIndex++)
                    {
                        documentPageCounter++;
                        iTextSharp.text.pdf.PdfImportedPage importedPage = copy.GetImportedPage(reader, currentPageIndex);
                        iTextSharp.text.pdf.PdfCopy.PageStamp pageStamp = copy.CreatePageStamp(importedPage);

                        pageStamp.AlterContents();

                        copy.AddPage(importedPage);
                    }

                    copy.FreeReader(reader);
                    reader.Close();
                }

                document.Close();
                return ms.GetBuffer();
            }
        }

        #endregion
    }
}