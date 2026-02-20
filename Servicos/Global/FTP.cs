using FluentFTP;
using Renci.SshNet;
using Renci.SshNet.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;

namespace Servicos
{
    public class FTP
    {
        public static string SenhaSFTP { get; private set; }
        #region Métodos Públicos

        //private string SenhaSFTP { get; set; }

        public static bool TestarConexao(string host, string porta, string diretorio, string usuario, string senha, bool passivo, bool ssl, out string erro, bool utilizaSFTP = false, string certificado = "")
        {
            //LogFTP("TestarConexao", host, porta, diretorio, usuario, senha, passivo, utilizaSFTP);

            Dominio.Enumeradores.ConexaoFTP conexaoFTP = ResolveTypeConnection(porta, utilizaSFTP);

            switch (conexaoFTP)
            {
                case Dominio.Enumeradores.ConexaoFTP.FTP:

                    //System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                    RemoteCertificateValidationCallback orgCallback = ServicePointManager.ServerCertificateValidationCallback;
                    try
                    {
                        ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(OnValidateCertificate);
                        ServicePointManager.Expect100Continue = true;

                        bool OnValidateCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
                        {
                            return true;
                        }

                        string url = MontarURL(host, porta, diretorio);

                        FtpWebRequest ftp = (FtpWebRequest)FtpWebRequest.Create(url);
                        ftp.Method = WebRequestMethods.Ftp.ListDirectory;
                        ftp.Credentials = new NetworkCredential(usuario, senha);
                        ftp.UsePassive = passivo;
                        ftp.EnableSsl = ssl;

                        try
                        {
                            WebResponse response = ftp.GetResponse();
                            response.Dispose();
                            erro = string.Empty;

                            return true;
                        }
                        catch (Exception ex)
                        {
                            erro = "Falha na conexão com " + url + " - " + ex.Message;
                            return false;
                        }
                    }
                    finally
                    {
                        ServicePointManager.ServerCertificateValidationCallback = orgCallback;
                    }

                case Dominio.Enumeradores.ConexaoFTP.SFPT:
                    try
                    {
                        using (SftpClient sFTP = ConnectSFTP(host, porta, usuario, senha, certificado))
                        {
                            sFTP.Connect();
                            sFTP.Disconnect();
                        }

                        erro = string.Empty;
                        return true;
                    }
                    catch (Exception ex)
                    {
                        erro = "Falha na conexão com " + host + " - " + ex.Message;
                        return false;
                    }


                case Dominio.Enumeradores.ConexaoFTP.FTPS:
                    FtpClient client = ConnectFTPS(host, diretorio, porta, usuario, senha);

                    try
                    {
                        client.Connect();
                        client.Disconnect();

                        erro = string.Empty;
                        return true;
                    }
                    catch (Exception ex)
                    {
                        erro = "Falha na conexão com " + host + " - " + ex.Message;
                        return false;
                    }

                default:
                    erro = string.Empty;
                    return false;
            }

        }

        public static bool EnviarArquivo(System.IO.MemoryStream file, string fileName, string host, string porta, string diretorio, string usuario, string senha, bool passivo, bool ssl, out string erro, bool utilizaSFTP = false, bool criarComNomeTemporario = false, string certificado = "")
        {
            string outrasInfo = "arquivo: " + fileName;
            LogFTP("EnviarArquivo", host, porta, diretorio, usuario, senha, passivo, utilizaSFTP, outrasInfo);

            string tempName = Guid.NewGuid().ToString().Replace("-", "") + ".temp";

            Dominio.Enumeradores.ConexaoFTP conexaoFTP = ResolveTypeConnection(porta, utilizaSFTP);

            switch (conexaoFTP)
            {
                case Dominio.Enumeradores.ConexaoFTP.FTP:

                    RemoteCertificateValidationCallback orgCallback = ServicePointManager.ServerCertificateValidationCallback;
                    try
                    {
                        ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(OnValidateCertificate);
                        ServicePointManager.Expect100Continue = true;

                        bool OnValidateCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
                        {
                            return true;
                        }

                        // Monta URL de conexao juntamente com o diretorio

                        string path = MontarURL(host, porta, diretorio);
                        // Aponta o local do arquivo
                        string url = path;
                        if (!criarComNomeTemporario)
                            url += fileName;
                        else
                            url += tempName;

                        try
                        {
                            // Inicia procedimentos de autenticacao
                            FtpWebRequest ftpRequest = (FtpWebRequest)FtpWebRequest.Create(url);

                            ftpRequest.Method = WebRequestMethods.Ftp.UploadFile;
                            ftpRequest.Credentials = new NetworkCredential(usuario, senha);
                            ftpRequest.UsePassive = passivo;
                            ftpRequest.UseBinary = true;
                            ftpRequest.EnableSsl = ssl;

                            // Envio do arquivo

                            byte[] buffer = file.ToArray();

                            LogFTP("Solicitou Conexão", host, porta, diretorio, usuario, senha, passivo, utilizaSFTP, outrasInfo);
                            using (System.IO.Stream requestStream = ftpRequest.GetRequestStream())
                            {
                                LogFTP("Iniciou Envio", host, porta, diretorio, usuario, senha, passivo, utilizaSFTP, outrasInfo);
                                requestStream.Write(buffer, 0, buffer.Length);
                                LogFTP("Finalizou Envio", host, porta, diretorio, usuario, senha, passivo, utilizaSFTP, outrasInfo);
                                requestStream.Flush();
                                requestStream.Close();
                                LogFTP("Fechou Conexão", host, porta, diretorio, usuario, senha, passivo, utilizaSFTP, outrasInfo);
                            }

                            if (criarComNomeTemporario)
                            {
                                LogFTP("Iniciou Renomear", host, porta, diretorio, usuario, senha, passivo, utilizaSFTP, outrasInfo);
                                FtpWebRequest ftpRename = (FtpWebRequest)FtpWebRequest.Create(url);

                                ftpRename.Method = WebRequestMethods.Ftp.Rename;
                                ftpRename.Credentials = new NetworkCredential(usuario, senha);
                                ftpRename.UsePassive = passivo;
                                ftpRename.UseBinary = true;
                                ftpRename.EnableSsl = ssl;
                                ftpRename.RenameTo = diretorio + "/" + fileName;
                                FtpWebResponse ftpResponse = (FtpWebResponse)ftpRename.GetResponse();
                                ftpResponse.Close();
                                ftpResponse = null;
                                LogFTP("Finalizou Renomear", host, porta, diretorio, usuario, senha, passivo, utilizaSFTP, outrasInfo);
                            }

                            erro = string.Empty;
                            return true;
                        }
                        catch (Exception ex)
                        {
                            erro = "Falha ao enviar o arquivo ao FTP '" + url + "': " + ex.Message;
                            return false;
                        }
                    }
                    finally
                    {
                        ServicePointManager.ServerCertificateValidationCallback = orgCallback;
                    }

                case Dominio.Enumeradores.ConexaoFTP.SFPT:
                    try
                    {
                        // Inicia procedimentos de autenticacao
                        using (SftpClient sFTP = ConnectSFTP(host, porta, usuario, senha, certificado))
                        {
                            sFTP.Connect();

                            if (diretorio == null)
                                diretorio = string.Empty;

                            // Valida diretorios
                            if (!diretorio.StartsWith("/"))
                                diretorio = "/" + diretorio;

                            if (diretorio.EndsWith("/"))
                                diretorio = diretorio.Remove(diretorio.Length - 1, 1);

                            sFTP.ChangeDirectory(diretorio);

                            // Envio do arquivo
                            if (!criarComNomeTemporario)
                                sFTP.UploadFile(file, fileName);
                            else
                            {
                                sFTP.UploadFile(file, tempName);
                                sFTP.RenameFile(tempName, fileName);
                            }

                            sFTP.Disconnect();
                        }

                        erro = string.Empty;

                        return true;
                    }
                    catch (Exception ex)
                    {
                        Servicos.Log.TratarErro(ex);

                        erro = "Falha ao enviar o arquivo " + fileName + " para o sFTP '" + host + "': " + ex.Message;

                        return false;
                    }


                case Dominio.Enumeradores.ConexaoFTP.FTPS:
                    FtpClient client = ConnectFTPS(host, diretorio, porta, usuario, senha);

                    try
                    {
                        byte[] buffer = file.ToArray();

                        if (!diretorio.EndsWith("/"))
                            diretorio = diretorio + "/";

                        string path = diretorio;

                        if (!criarComNomeTemporario)
                            path += fileName;
                        else
                            path += tempName;

                        using (System.IO.Stream requestStream = client.OpenWrite(path))
                        {
                            requestStream.Write(buffer, 0, buffer.Length);

                            //Atenção! esse rename não foi testado não tinha um exemplo deste tipo de FTP em homologação para validar (Rodrigo)
                            if (criarComNomeTemporario)
                                client.Rename(path, diretorio + "/" + fileName);

                            requestStream.Flush();
                            requestStream.Close();
                        }

                        client.Disconnect();

                        erro = string.Empty;
                        return true;
                    }
                    catch (Exception ex)
                    {
                        erro = "Falha na conexão com " + host + " - " + ex.Message;
                        return false;
                    }

                default:
                    erro = string.Empty;
                    return false;
            }
        }

        public static void MoverArquivo(string host, string porta, string diretorio, string usuario, string senha, bool passivo, bool ssl, string nomeArquivoOriginal, string novoApontamento, out string erro, bool utilizaSFTP = false, string certificado = "")
        {
            //string outrasInfo = "nomeArquivoOriginal: " + nomeArquivoOriginal + "; novoApontamento: " + novoApontamento;
            //LogFTP("MoverArquivo", host, porta, diretorio, usuario, senha, passivo, utilizaSFTP, outrasInfo);

            Dominio.Enumeradores.ConexaoFTP conexaoFTP = ResolveTypeConnection(porta, utilizaSFTP);
            erro = string.Empty;

            // Novo nome precisa ter a pasta a partir da raiz -> /pasta/para/novoarquivo.ext
            if (!novoApontamento.StartsWith("/"))
                novoApontamento = "/" + novoApontamento;

            switch (conexaoFTP)
            {
                case Dominio.Enumeradores.ConexaoFTP.FTP:

                    RemoteCertificateValidationCallback orgCallback = ServicePointManager.ServerCertificateValidationCallback;
                    try
                    {
                        ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(OnValidateCertificate);
                        ServicePointManager.Expect100Continue = true;

                        bool OnValidateCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
                        {
                            return true;
                        }
                        /* Exemplo de chamada:
                        * apontamentoOriginal -> diretorio + nomeArquivoOriginal ex: (dir/file.txt) -> Apenas o nome do arquivo
                        * novoApontamento -> new-dir/file.txt -> Passar o novo diretorio e o nome do arquivo
                        */
                        string apontamentoOriginal = MontarURL(host, porta, diretorio) + nomeArquivoOriginal;

                        try
                        {
                            // Inicia procedimento de autenticacao
                            FtpWebRequest ftpRequest = (FtpWebRequest)FtpWebRequest.Create(apontamentoOriginal);

                            ftpRequest.Method = WebRequestMethods.Ftp.Rename;
                            ftpRequest.Credentials = new NetworkCredential(usuario, senha);
                            ftpRequest.UsePassive = passivo;
                            ftpRequest.UseBinary = true;
                            ftpRequest.RenameTo = novoApontamento;
                            ftpRequest.EnableSsl = ssl;

                            // Executa chamada para renomear
                            using (var response = (FtpWebResponse)ftpRequest.GetResponse())
                                response.Close();
                        }
                        catch (Exception ex)
                        {
                            erro = "Falha no mover arquivo (" + apontamentoOriginal + ") FTP: " + ex.Message;
                        }
                        break;
                    }
                    finally
                    {
                        ServicePointManager.ServerCertificateValidationCallback = orgCallback;
                    }

                case Dominio.Enumeradores.ConexaoFTP.SFPT:
                    using (SftpClient sFTP = ConnectSFTP(host, porta, usuario, senha, certificado))
                    {
                        sFTP.Connect();

                        // Confere diretorios
                        if (!diretorio.EndsWith("/"))
                            diretorio = diretorio + "/";

                        // Executa acao
                        sFTP.RenameFile(diretorio + nomeArquivoOriginal, novoApontamento);

                        // Desconecta
                        sFTP.Disconnect();
                    }
                    break;

                case Dominio.Enumeradores.ConexaoFTP.FTPS:
                    using (FtpClient client = ConnectFTPS(host, diretorio, porta, usuario, senha))
                    {
                        client.Connect();

                        // Confere diretorios
                        if (!diretorio.EndsWith("/"))
                            diretorio = diretorio + "/";

                        // Executa acao
                        client.Rename(diretorio + nomeArquivoOriginal, novoApontamento);

                        // Desconecta
                        client.Disconnect();
                    }
                    break;

                default:
                    erro = string.Empty;
                    break;
            }

        }

        public static void DeletarArquivo(string host, string porta, string diretorio, string usuario, string senha, bool passivo, bool ssl, string arquivo, out string erro, bool utilizaSFTP = false, string certificado = "")
        {
            //string outrasInfo = "arquivo: " + arquivo;
            //LogFTP("DeletarArquivo", host, porta, diretorio, usuario, senha, passivo, utilizaSFTP, outrasInfo);

            Dominio.Enumeradores.ConexaoFTP conexaoFTP = ResolveTypeConnection(porta, utilizaSFTP);

            switch (conexaoFTP)
            {
                case Dominio.Enumeradores.ConexaoFTP.FTP:
                    RemoteCertificateValidationCallback orgCallback = ServicePointManager.ServerCertificateValidationCallback;
                    try
                    {
                        ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(OnValidateCertificate);
                        ServicePointManager.Expect100Continue = true;

                        bool OnValidateCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
                        {
                            return true;
                        }

                        // arquivo -> nome do arquivo dentro do diretorio a excluir
                        string diretorioAbsolutoDoArquivo = MontarURL(host, porta, diretorio) + arquivo;

                        try
                        {
                            // Inicia procedimentos de autenticacao
                            FtpWebRequest ftpRequest = (FtpWebRequest)FtpWebRequest.Create(diretorioAbsolutoDoArquivo);

                            ftpRequest.Method = WebRequestMethods.Ftp.DeleteFile;
                            ftpRequest.Credentials = new NetworkCredential(usuario, senha);
                            ftpRequest.UsePassive = passivo;
                            ftpRequest.UseBinary = true;
                            ftpRequest.EnableSsl = ssl;
                            // Executa chamada para excluir
                            var response = (FtpWebResponse)ftpRequest.GetResponse();
                            response.Close();
                            erro = string.Empty;
                        }
                        catch (Exception ex)
                        {
                            erro = "Falha no deletar arquivo (" + diretorioAbsolutoDoArquivo + ") FTP: " + ex.Message;
                        }
                        break;
                    }
                    finally
                    {
                        ServicePointManager.ServerCertificateValidationCallback = orgCallback;
                    }
                case Dominio.Enumeradores.ConexaoFTP.SFPT:
                    using (SftpClient sFTP = ConnectSFTP(host, porta, usuario, senha, certificado))
                    {
                        sFTP.Connect();

                        // Confere diretorio
                        if (!diretorio.EndsWith("/"))
                            diretorio = diretorio + "/";

                        // Executa acao
                        sFTP.DeleteFile(diretorio + arquivo);

                        // Desconecta
                        sFTP.Disconnect();
                        erro = string.Empty;
                    }
                    break;

                case Dominio.Enumeradores.ConexaoFTP.FTPS:
                    using (FtpClient client = ConnectFTPS(host, diretorio, porta, usuario, senha))
                    {
                        // Confere diretorios
                        if (!diretorio.EndsWith("/"))
                            diretorio = diretorio + "/";

                        // Executa acao
                        client.DeleteFile(diretorio + arquivo);

                        // Desconecta
                        client.Disconnect();
                        erro = string.Empty;
                    }
                    break;

                default:
                    erro = string.Empty;
                    break;
            }
        }

        public static System.IO.Stream DownloadArquivo(string host, string porta, string diretorio, string usuario, string senha, bool passivo, bool ssl, string arquivo, out string erro, bool utilizaSFTP = false, string certificado = "")
        {
            Dominio.Enumeradores.ConexaoFTP conexaoFTP = ResolveTypeConnection(porta, utilizaSFTP);

            switch (conexaoFTP)
            {
                case Dominio.Enumeradores.ConexaoFTP.FTP:
                    RemoteCertificateValidationCallback orgCallback = ServicePointManager.ServerCertificateValidationCallback;
                    try
                    {
                        ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(OnValidateCertificate);
                        ServicePointManager.Expect100Continue = true;

                        bool OnValidateCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
                        {
                            return true;
                        }

                        string url = MontarURL(host, porta, diretorio) + arquivo;

                        try
                        {
                            // Inicia procedimentos de autenticacao
                            FtpWebRequest ftpRequest = (FtpWebRequest)FtpWebRequest.Create(url);

                            ftpRequest.Method = WebRequestMethods.Ftp.DownloadFile;
                            ftpRequest.Credentials = new NetworkCredential(usuario, senha);
                            ftpRequest.UsePassive = passivo;
                            ftpRequest.UseBinary = true;
                            ftpRequest.EnableSsl = ssl;

                            // Executa download
                            using (var response = (FtpWebResponse)ftpRequest.GetResponse())
                            {
                                System.IO.MemoryStream stream = new MemoryStream();

                                response.GetResponseStream().CopyTo(stream);

                                stream.Seek(0, SeekOrigin.Begin);

                                erro = string.Empty;
                                return stream;
                            }
                        }
                        catch (Exception ex)
                        {
                            erro = "Falha no download do arquivo " + arquivo + " do FTP " + url + ": " + ex.Message;
                            return null;
                        }
                    }
                    finally
                    {
                        ServicePointManager.ServerCertificateValidationCallback = orgCallback;
                    }
                case Dominio.Enumeradores.ConexaoFTP.SFPT:
                    using (SftpClient sFTP = ConnectSFTP(host, porta, usuario, senha, certificado))
                    {
                        sFTP.Connect();

                        if (!diretorio.EndsWith("/"))
                            diretorio = diretorio + "/";

                        MemoryStream stream = new MemoryStream();

                        sFTP.DownloadFile(diretorio + arquivo, stream);
                        sFTP.Disconnect();
                        stream.Seek(0, SeekOrigin.Begin);

                        erro = string.Empty;
                        return stream;
                    }

                case Dominio.Enumeradores.ConexaoFTP.FTPS:
                    using (FtpClient client = ConnectFTPS(host, diretorio, porta, usuario, senha))
                    {
                        // Confere diretorios
                        if (!diretorio.StartsWith("/")) diretorio = "/" + diretorio;
                        if (!diretorio.EndsWith("/")) diretorio = diretorio + "/";

                        System.IO.MemoryStream ftpStream = new MemoryStream();
                        client.OpenRead(diretorio + arquivo).CopyTo(ftpStream);

                        ftpStream.Seek(0, SeekOrigin.Begin);

                        client.Disconnect();

                        erro = string.Empty;
                        return ftpStream;
                    }
                default:
                    erro = string.Empty;
                    return null;
            }
        }

        public static List<string> DownloadArquivosPasta(string host, string porta, string diretorio, string usuario, string senha, bool passivo, bool ssl, string diretorioDownload, out string erro, bool utilizaSFTP = false, bool moverImportados = false, string pastaImportados = "", bool deletaImportados = false, bool renomearArquivos = true, bool usarISO8859 = false, bool usarUTF8 = false, List<string> prefixos = null, bool nomeArquivoComEspaco = false, string certificado = "")
        {
            //string outrasInfo = "moverImportados: " + (moverImportados ? "true" : "false") + "; deletaImportados: " + (deletaImportados ? "true" : "false") + "; pastaImportados: " + pastaImportados + "; ";
            //LogFTP("DownloadArquivosPasta", host, porta, diretorio, usuario, senha, passivo, utilizaSFTP, outrasInfo);

            Dominio.Enumeradores.ConexaoFTP conexaoFTP = ResolveTypeConnection(porta, utilizaSFTP);
            erro = string.Empty;

            // Lista de diretorios baixados
            List<string> arquivosBaixados = new List<string>();

            switch (conexaoFTP)
            {
                case Dominio.Enumeradores.ConexaoFTP.FTP:
                    RemoteCertificateValidationCallback orgCallback = ServicePointManager.ServerCertificateValidationCallback;
                    try
                    {
                        ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(OnValidateCertificate);
                        ServicePointManager.Expect100Continue = true;

                        bool OnValidateCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
                        {
                            return true;
                        }

                        string url = MontarURL(host, porta, diretorio);

                        // Inicia procedimentos de autenticacao
                        FtpWebRequest ftpRequest = (FtpWebRequest)FtpWebRequest.Create(url);

                        // Chama metodo de listagem
                        ftpRequest.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
                        ftpRequest.Credentials = new NetworkCredential(usuario, senha);
                        ftpRequest.UsePassive = passivo;
                        ftpRequest.UseBinary = true;
                        ftpRequest.EnableSsl = ssl;

                        StreamReader readerListagem = null;
                        try
                        {
                            //Percorrer arquivos do diretorio
                            using (var response = (FtpWebResponse)ftpRequest.GetResponse())
                            {
                                using (var stream = response.GetResponseStream())
                                {
                                    if (usarISO8859)
                                        readerListagem = new StreamReader(stream, Encoding.GetEncoding("iso-8859-1"));
                                    else if (usarUTF8)
                                        readerListagem = new StreamReader(stream, Encoding.UTF8);
                                    else
                                        readerListagem = new StreamReader(stream, true);

                                    while (!readerListagem.EndOfStream)
                                    {
                                        string arquivoDetalhes = readerListagem.ReadLine();

                                        // Verifica se e arquivo ou um diretorio
                                        if (!arquivoDetalhes.Contains("<DIR>"))
                                        {

                                            // Extrai apenas o nome do arquivo
                                            Regex regex = new Regex(@"\d{5} ");
                                            string[] detalhes = arquivoDetalhes.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                                            string arquivo = detalhes[detalhes.Length - 1]; //arquivoDetalhes.Replace(detalhes[0], "").Replace(detalhes[1], "").Replace(detalhes[2], "").Trim();

                                            if (nomeArquivoComEspaco)
                                                arquivo = regex.Split(arquivoDetalhes)[1];

                                            if (ValidarPrefixos(prefixos, arquivo))
                                            {
                                                // Gera novo nome unico
                                                string nomeGuid = Guid.NewGuid().ToString().Replace("-", "");
                                                string extencaoArquivo = System.IO.Path.GetExtension(arquivo).ToLower();

                                                string nomeArquivo = nomeGuid + extencaoArquivo;

                                                if (!renomearArquivos)
                                                    nomeArquivo = arquivo;

                                                //Download arquivo
                                                string arquivoBaixado = DownloadArquivo(host, porta, diretorio, usuario, senha, passivo, ssl, arquivo, diretorioDownload, nomeArquivo, out erro, utilizaSFTP, usarISO8859, usarUTF8, certificado);

                                                if (!string.IsNullOrEmpty(arquivoBaixado))
                                                {
                                                    arquivosBaixados.Add(arquivoBaixado);

                                                    // Move arquivos depois de importados
                                                    if (moverImportados)
                                                    {
                                                        if (!pastaImportados.EndsWith("/"))
                                                            pastaImportados = pastaImportados + "/";

                                                        if (!pastaImportados.StartsWith("/"))
                                                            pastaImportados = "/" + pastaImportados;

                                                        //Mover pasta importados
                                                        string novoApontamento = pastaImportados + arquivo;

                                                        MoverArquivo(host, porta, diretorio, usuario, senha, passivo, ssl, arquivo, novoApontamento, out erro);
                                                    }

                                                    if (deletaImportados)
                                                    {
                                                        DeletarArquivo(host, porta, diretorio, usuario, senha, passivo, ssl, arquivo, out erro, utilizaSFTP);
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    readerListagem.Dispose();
                                }

                                response.Close();
                            }
                        }
                        catch (Exception ex)
                        {
                            if (readerListagem != null)
                                readerListagem.Dispose();

                            Servicos.Log.TratarErro("Falha no Downlod arquivo FTP: " + ex.Message);
                            erro = "Falha no Downlod arquivo FTP: " + ex.Message;
                            //return null;
                        }

                        //erro = string.Empty;
                        return arquivosBaixados;
                    }
                    catch (Exception ex)
                    {
                        erro = "Falha na conexão FTP: " + ex.Message;
                        Servicos.Log.TratarErro(erro);
                        return arquivosBaixados;
                    }
                    finally
                    {
                        ServicePointManager.ServerCertificateValidationCallback = orgCallback;
                    }
                case Dominio.Enumeradores.ConexaoFTP.SFPT:
                    try
                    {
                        // Conecta
                        using (SftpClient sFTP = ConnectSFTP(host, porta, usuario, senha, certificado))
                        {
                            sFTP.Connect();

                            // Lista itens da pasta
                            var diretoriosFTP = sFTP.ListDirectory(diretorio);

                            foreach (var diretorioFTP in diretoriosFTP)
                            {
                                if (!diretorioFTP.Name.StartsWith(".") && !diretorioFTP.IsDirectory)
                                {
                                    try
                                    {
                                        string nomeGuid = Guid.NewGuid().ToString().Replace("-", "");
                                        string extencaoArquivo = System.IO.Path.GetExtension(diretorioFTP.FullName).ToLower();
                                        string nomeArquivo = nomeGuid + extencaoArquivo;

                                        if (ValidarPrefixos(prefixos, diretorioFTP.Name))
                                        {
                                            if (!renomearArquivos)
                                                nomeArquivo = diretorioFTP.Name;

                                            // Download do arquivo
                                            string arquivoBaixado = DownloadArquivo(host, porta, diretorio, usuario, senha, passivo, ssl, diretorioFTP.Name, diretorioDownload, nomeArquivo, out erro, true, false, false, certificado);
                                            arquivosBaixados.Add(arquivoBaixado);

                                            // Move arquivos depois de importados
                                            if (moverImportados)
                                            {
                                                if (!diretorio.StartsWith("/"))
                                                    diretorio = "/" + diretorio;

                                                if (!diretorio.EndsWith("/"))
                                                    diretorio = diretorio + "/";

                                                if (!pastaImportados.EndsWith("/"))
                                                    pastaImportados = pastaImportados + "/";

                                                string novoApontamento = diretorio + pastaImportados + diretorioFTP.Name;

                                                MoverArquivo(host, porta, diretorio, usuario, senha, passivo, ssl, diretorioFTP.Name, novoApontamento, out erro, utilizaSFTP);
                                            }

                                            if (deletaImportados)
                                            {
                                                DeletarArquivo(host, porta, diretorio, usuario, senha, passivo, ssl, diretorioFTP.Name, out erro, utilizaSFTP);
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        erro = "Falha no Downlod arquivo SFTP: " + ex.Message;
                                    }
                                }
                            }

                            // Fecha conexao
                            sFTP.Disconnect();
                        }

                        erro = string.Empty;
                        return arquivosBaixados;
                    }
                    catch (Exception ex)
                    {
                        erro = "Falha na conexão sFTP: " + ex.Message;

                        return arquivosBaixados;
                    }

                case Dominio.Enumeradores.ConexaoFTP.FTPS:
                    try
                    {
                        // Conecta
                        using (FtpClient client = ConnectFTPS(host, diretorio, porta, usuario, senha))
                        {
                            foreach (var item in client.GetListing(client.GetWorkingDirectory(), FtpListOption.Modify | FtpListOption.Size))
                            {
                                if (!item.Name.StartsWith(".") && item.Type == FtpFileSystemObjectType.File)
                                {
                                    try
                                    {
                                        string nomeGuid = Guid.NewGuid().ToString().Replace("-", "");
                                        string extencaoArquivo = System.IO.Path.GetExtension(item.FullName).ToLower();
                                        string nomeArquivo = nomeGuid + extencaoArquivo;

                                        if (ValidarPrefixos(prefixos, item.Name))
                                        {
                                            if (!renomearArquivos)
                                                nomeArquivo = item.Name;

                                            // Download do arquivo
                                            string arquivoBaixado = DownloadArquivo(host, porta, diretorio, usuario, senha, passivo, ssl, item.Name, diretorioDownload, nomeArquivo, out erro, true, false, false, certificado);
                                            arquivosBaixados.Add(arquivoBaixado);

                                            // Move arquivos depois de importados
                                            if (moverImportados)
                                            {
                                                if (!diretorio.StartsWith("/"))
                                                    diretorio = "/" + diretorio;

                                                if (!diretorio.EndsWith("/"))
                                                    diretorio = diretorio + "/";

                                                if (!pastaImportados.EndsWith("/"))
                                                    pastaImportados = pastaImportados + "/";

                                                string novoApontamento = diretorio + pastaImportados + item.Name;

                                                MoverArquivo(host, porta, diretorio, usuario, senha, passivo, ssl, item.Name, novoApontamento, out erro, utilizaSFTP);
                                            }

                                            if (deletaImportados)
                                            {
                                                DeletarArquivo(host, porta, diretorio, usuario, senha, passivo, ssl, item.Name, out erro, utilizaSFTP);
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        erro = "Falha no Downlod arquivo SFTP: " + ex.Message;
                                    }
                                }
                            }

                            // Fecha conexao
                            client.Disconnect();
                        }

                        erro = string.Empty;
                        return arquivosBaixados;
                    }
                    catch (Exception ex)
                    {
                        erro = "Falha na conexão sFTP: " + ex.Message;

                        return arquivosBaixados;
                    }

                default:
                    erro = string.Empty;
                    return arquivosBaixados;
            }
        }

        public static List<string> ObterListagemArquivos(string host, string porta, string diretorio, string usuario, string senha, bool passivo, bool ssl, out string erro, bool utilizaSFTP = false, bool usarISO8859 = false, List<string> prefixos = null, string certificado = "")
        {
            Dominio.Enumeradores.ConexaoFTP conexaoFTP = ResolveTypeConnection(porta, utilizaSFTP);
            erro = string.Empty;

            // Lista de diretorios baixados
            List<string> arquivosDisponiveis = new List<string>();

            switch (conexaoFTP)
            {
                case Dominio.Enumeradores.ConexaoFTP.FTP:
                    RemoteCertificateValidationCallback orgCallback = ServicePointManager.ServerCertificateValidationCallback;
                    try
                    {

                        ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(OnValidateCertificate);
                        ServicePointManager.Expect100Continue = true;

                        bool OnValidateCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
                        {
                            return true;
                        }

                        string url = MontarURL(host, porta, diretorio);

                        // Inicia procedimentos de autenticacao
                        FtpWebRequest ftpRequest = (FtpWebRequest)FtpWebRequest.Create(url);

                        // Chama metodo de listagem
                        ftpRequest.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
                        ftpRequest.Credentials = new NetworkCredential(usuario, senha);
                        ftpRequest.UsePassive = passivo;
                        ftpRequest.UseBinary = true;
                        ftpRequest.EnableSsl = ssl;

                        StreamReader readerListagem = null;
                        try
                        {
                            //Percorrer arquivos do diretorio
                            using (var response = (FtpWebResponse)ftpRequest.GetResponse())
                            {
                                using (var stream = response.GetResponseStream())
                                {
                                    if (usarISO8859)
                                        readerListagem = new StreamReader(stream, Encoding.GetEncoding("iso-8859-1"));
                                    else
                                        readerListagem = new StreamReader(stream, true);

                                    while (!readerListagem.EndOfStream)
                                    {
                                        string arquivoDetalhes = readerListagem.ReadLine();

                                        // Verifica se e arquivo ou um diretorio
                                        if (!arquivoDetalhes.Contains("<DIR>"))
                                        {
                                            // Extrai apenas o nome do arquivo
                                            string[] detalhes = arquivoDetalhes.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

                                            string arquivo = detalhes[detalhes.Length - 1];
                                            if (ValidarPrefixos(prefixos, arquivo))
                                                arquivosDisponiveis.Add(arquivo);
                                        }
                                    }

                                    readerListagem.Dispose();
                                }

                                response.Close();
                            }
                        }
                        catch (Exception ex)
                        {
                            if (readerListagem != null)
                                readerListagem.Dispose();

                            erro = "Falha na leitura dos arquivos disponíveis no FTP: " + ex.Message;
                        }

                        return arquivosDisponiveis;
                    }
                    catch (Exception ex)
                    {
                        erro = "Falha na conexão com o FTP: " + ex.Message;
                        return arquivosDisponiveis;
                    }
                    finally
                    {
                        ServicePointManager.ServerCertificateValidationCallback = orgCallback;
                    }
                case Dominio.Enumeradores.ConexaoFTP.SFPT:

                    try
                    {
                        // Conecta
                        using (SftpClient sFTP = ConnectSFTP(host, porta, usuario, senha, certificado))
                        {
                            sFTP.Connect();

                            // Lista itens da pasta
                            var diretoriosFTP = sFTP.ListDirectory(diretorio);

                            foreach (var diretorioFTP in diretoriosFTP)
                            {
                                if (!diretorioFTP.Name.StartsWith(".") && !diretorioFTP.IsDirectory)
                                {
                                    if (ValidarPrefixos(prefixos, diretorioFTP.Name))
                                        arquivosDisponiveis.Add(diretorioFTP.Name);
                                }

                            }

                            // Fecha conexao
                            sFTP.Disconnect();
                        }

                        erro = string.Empty;
                        return arquivosDisponiveis;
                    }
                    catch (Exception ex)
                    {
                        erro = "Falha na conexão com o sFTP: " + ex.Message;

                        return arquivosDisponiveis;
                    }

                case Dominio.Enumeradores.ConexaoFTP.FTPS:
                    try
                    {
                        // Conecta
                        using (FtpClient client = ConnectFTPS(host, diretorio, porta, usuario, senha))
                        {
                            string diretorioBase = client.GetWorkingDirectory() + diretorio;
                            foreach (var item in client.GetListing(diretorioBase, FtpListOption.Modify | FtpListOption.Size))
                            {
                                if (!item.Name.StartsWith(".") && item.Type == FtpFileSystemObjectType.File)
                                {
                                    if (ValidarPrefixos(prefixos, item.Name))
                                        arquivosDisponiveis.Add(item.Name);
                                }

                            }

                            // Fecha conexao
                            client.Disconnect();
                        }

                        erro = string.Empty;
                        return arquivosDisponiveis;
                    }
                    catch (Exception ex)
                    {
                        erro = "Falha na conexão sFTP: " + ex.Message;

                        return arquivosDisponiveis;
                    }

                default:
                    erro = string.Empty;
                    return arquivosDisponiveis;
            }
        }

        #endregion

        #region Métodos Privados

        private static string DownloadArquivo(string host, string porta, string diretorio, string usuario, string senha, bool passivo, bool ssl, string arquivo, string diretorioDownload, string nomeArquivo, out string erro, bool utilizaSFTP = false, bool usarISO8859 = false, bool usarUTF8 = false, string certificado = "")
        {
            //string outrasInfo = "diretorioDownload: " + diretorioDownload + "; nomeArquivo: " + nomeArquivo;
            //LogFTP("DownloadArquivo", host, porta, diretorio, usuario, senha, passivo, utilizaSFTP, outrasInfo);

            Dominio.Enumeradores.ConexaoFTP conexaoFTP = ResolveTypeConnection(porta, utilizaSFTP);

            switch (conexaoFTP)
            {
                case Dominio.Enumeradores.ConexaoFTP.FTP:
                    RemoteCertificateValidationCallback orgCallback = ServicePointManager.ServerCertificateValidationCallback;
                    try
                    {
                        ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(OnValidateCertificate);
                        ServicePointManager.Expect100Continue = true;

                        bool OnValidateCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
                        {
                            return true;
                        }

                        string url = MontarURL(host, porta, diretorio) + arquivo;

                        try
                        {
                            // Inicia procedimentos de autenticacao
                            FtpWebRequest ftpRequest = (FtpWebRequest)FtpWebRequest.Create(url);

                            ftpRequest.Method = WebRequestMethods.Ftp.DownloadFile;
                            ftpRequest.Credentials = new NetworkCredential(usuario, senha);
                            ftpRequest.UsePassive = passivo;
                            ftpRequest.UseBinary = true;
                            ftpRequest.EnableSsl = ssl;

                            // Executa download
                            using (var response = (FtpWebResponse)ftpRequest.GetResponse())
                            {
                                using (var stream = response.GetResponseStream())
                                {
                                    // Cria arquivo local
                                    StreamReader readerArquivo;
                                    if (usarISO8859)
                                        readerArquivo = new StreamReader(stream, Encoding.GetEncoding("iso-8859-1"));
                                    else if (usarUTF8)
                                        readerArquivo = new StreamReader(stream, Encoding.UTF8);
                                    else
                                        readerArquivo = new StreamReader(stream, Encoding.ASCII);

                                    Utilidades.IO.FileStorageService.Storage.WriteAllText(Utilidades.IO.FileStorageService.Storage.Combine(diretorioDownload, nomeArquivo), readerArquivo.ReadToEnd());

                                    // Limpa metodos
                                    readerArquivo.Close();
                                    readerArquivo.Dispose();
                                }
                                response.Close();
                                response.Dispose();
                            }

                            // Retorna o local do arquivo baixado
                            erro = string.Empty;
                            return diretorioDownload + nomeArquivo;
                        }
                        catch (Exception ex)
                        {
                            erro = "Falha no downlod arquivo " + nomeArquivo + " do FTP " + url + ": " + ex.Message;
                            return null;
                        }
                    }
                    finally
                    {
                        ServicePointManager.ServerCertificateValidationCallback = orgCallback;
                    }
                case Dominio.Enumeradores.ConexaoFTP.SFPT:
                    using (SftpClient sFTP = ConnectSFTP(host, porta, usuario, senha, certificado))
                    {
                        sFTP.Connect();

                        // Confere diretorios
                        if (!diretorio.EndsWith("/"))
                            diretorio = diretorio + "/";

                        // Cria arquivo local
                        using (Stream fileStream = Utilidades.IO.FileStorageService.Storage.OpenWrite(diretorioDownload + nomeArquivo))
                        {
                            sFTP.DownloadFile(diretorio + arquivo, fileStream);

                            // Limpa metodos
                            fileStream.Close();
                        }

                        sFTP.Disconnect();
                    }

                    erro = string.Empty;
                    return diretorioDownload + nomeArquivo;

                case Dominio.Enumeradores.ConexaoFTP.FTPS:
                    using (FtpClient client = ConnectFTPS(host, diretorio, porta, usuario, senha))
                    {
                        // Confere diretorios
                        if (!diretorio.StartsWith("/")) diretorio = "/" + diretorio;
                        if (!diretorio.EndsWith("/")) diretorio = diretorio + "/";

                        using (Stream ftpStream = client.OpenRead(diretorio + arquivo))
                        {
                            try
                            {
                                StreamReader readerArquivo;
                                if (usarISO8859)
                                    readerArquivo = new StreamReader(ftpStream, Encoding.GetEncoding("iso-8859-1"));
                                else if (usarUTF8)
                                    readerArquivo = new StreamReader(ftpStream, Encoding.UTF8);
                                else
                                    readerArquivo = new StreamReader(ftpStream, Encoding.ASCII);

                                Utilidades.IO.FileStorageService.Storage.WriteAllText(Utilidades.IO.FileStorageService.Storage.Combine(diretorioDownload, nomeArquivo), readerArquivo.ReadToEnd());
                            }
                            finally
                            {
                                ftpStream.Close();
                            }
                        }

                        client.Disconnect();
                    }
                    erro = string.Empty;
                    return diretorioDownload + nomeArquivo;

                default:
                    erro = string.Empty;
                    return string.Empty;
            }
        }

        private static string MontarURL(string host, string porta, string diretorio)
        {
            string h = (!host.Contains("ftp://") ? "ftp://" : string.Empty) +
                       host +
                       (!string.IsNullOrWhiteSpace(porta) ? ":" + porta : string.Empty) +
                       (!diretorio.StartsWith("/") ? "/" : "") +
                       (!string.IsNullOrWhiteSpace(diretorio) ? diretorio : string.Empty);

            if (!h.EndsWith("/"))
                h += "/";

            return h;
        }

        private static void LogFTP(string metodo, string host, string porta, string diretorio, string usuario, string senha, bool passivo, bool utilizaSFTP, string outrasInfo = "")
        {
            string log = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff") + " ";

            log = log + "metodo: " + metodo + "; ";
            log = log + "host: " + host + "; ";
            log = log + "porta: " + porta + "; ";
            log = log + "diretorio: " + diretorio + "; ";
            log = log + "usuario: " + usuario + "; ";
            //log = log + "senha: " + senha + "; ";
            log = log + "passivo: " + (passivo ? "true" : "false") + "; ";
            log = log + "utilizaSFTP: " + (utilizaSFTP ? "true" : "false") + "; ";

            if (!string.IsNullOrWhiteSpace(outrasInfo))
                log = log + outrasInfo;

            Servicos.Log.TratarErro(log, "LogEnvioFTP");
        }

        private static Dominio.Enumeradores.ConexaoFTP ResolveTypeConnection(string porta, bool utilizaSFTP)
        {
            int.TryParse(porta, out int port);

            Dominio.Enumeradores.ConexaoFTP conexaoFTP = Dominio.Enumeradores.ConexaoFTP.FTP;

            if (utilizaSFTP)
                conexaoFTP = Dominio.Enumeradores.ConexaoFTP.SFPT;

            if (port == 990)
                conexaoFTP = Dominio.Enumeradores.ConexaoFTP.FTPS;

            return conexaoFTP;
        }

        private static FtpClient ConnectFTPS(string host, string diretorio, string porta, string usuario, string senha)
        {
            int.TryParse(porta, out int port);
            //if (!diretorio.EndsWith("/"))
            //    diretorio = "/" + diretorio;

            FtpClient client = new FtpClient(host, port, usuario, senha)
            {
                EncryptionMode = FtpEncryptionMode.Implicit,
                SslProtocols = System.Security.Authentication.SslProtocols.Tls,
                DataConnectionEncryption = true,
            };

            client.ValidateCertificate += new FtpSslValidation(OnValidateCertificate);
            void OnValidateCertificate(FtpClient control, FtpSslValidationEventArgs e)
            {
                e.Accept = true;
            }

            return client;
        }

        private static SftpClient ConnectSFTP(string host, string porta, string usuario, string senha, string certificado)
        {
            SftpClient client = null;
            Servicos.Embarcador.Anexo.Anexo<Dominio.Entidades.Embarcador.Cargas.CargaAnexo, Dominio.Entidades.Embarcador.Cargas.Carga>
            servicoAnexo = new Servicos.Embarcador.Anexo.Anexo<Dominio.Entidades.Embarcador.Cargas.CargaAnexo, Dominio.Entidades.Embarcador.Cargas.Carga>(null);

            if (string.IsNullOrWhiteSpace(porta))
                return new SftpClient(host, usuario, senha);

            ConnectionInfo connecte = null;

            if (string.IsNullOrWhiteSpace(certificado))
            {
                SenhaSFTP = senha;
                KeyboardInteractiveAuthenticationMethod keybAuth = new KeyboardInteractiveAuthenticationMethod(usuario);
                keybAuth.AuthenticationPrompt += new EventHandler<AuthenticationPromptEventArgs>(HandleKeyEvent);

                connecte = new ConnectionInfo(host, porta.ToInt(), usuario, keybAuth);
            }
            else
            {
                bool attempted = false;
            Retry:
                var memStream = new MemoryStream(Encoding.UTF8.GetBytes(certificado));
                try
                {

                    byte[] data = System.Text.Encoding.UTF8.GetBytes(certificado);
                    string caminho = servicoAnexo.ObterCaminhoArquivos(null);
                    string filePath = Utilidades.IO.FileStorageService.Storage.Combine(caminho, "CertificadoTemporario.txt");
                    using (MemoryStream memoryStream = new MemoryStream(data))
                    {
                        using (Stream fileStream = Utilidades.IO.FileStorageService.Storage.OpenWrite(filePath))
                        {
                            memoryStream.WriteTo(fileStream);
                        }
                    }

                    using (var privateKeyFile = new PrivateKeyFile(filePath))
                    {
                        var authenticationMethod = new PrivateKeyAuthenticationMethod(usuario, privateKeyFile);
                        connecte = new ConnectionInfo(host, porta.ToInt(), usuario, authenticationMethod);

                        using (var sftpClient = new SftpClient(connecte))
                        {
                            sftpClient.Connect();
                            sftpClient.Disconnect();
                        }
                    }

                    //var file = new PrivateKeyFile(memStream);
                    //var authMethod = new PrivateKeyAuthenticationMethod(usuario, file);
                    //connecte = new ConnectionInfo(host, usuario, authMethod);
                }
                catch (SshException ex)
                {
                    if (ex.Message == "Invalid private key file." && !attempted && IsBase64String(certificado, out string certificadoConvertedFromBase64))
                    {
                        certificado = certificadoConvertedFromBase64;
                        attempted = true;
                        goto Retry;
                    }
                    throw;
                }
            }

            try
            {
                client = new SftpClient(connecte);
                client.Connect();
                client.Disconnect();
            }
            catch
            {
                client = new SftpClient(host, porta.ToInt(), usuario, senha);
            }


            return client;
        }


        private static bool IsBase64String(string base64, out string converted)
        {
            converted = null;

            if (string.IsNullOrEmpty(base64))
            {
                return false;
            }

            if (base64.Length % 4 != 0)
            {
                return false;
            }

            try
            {
                converted = Encoding.UTF8.GetString(Convert.FromBase64String(base64));
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        private static void HandleKeyEvent(object sender, AuthenticationPromptEventArgs e)
        {
            foreach (AuthenticationPrompt prompt in e.Prompts)
            {
                if (prompt.Request.IndexOf("Password:", StringComparison.InvariantCultureIgnoreCase) != -1)
                {
                    prompt.Response = SenhaSFTP;
                }
            }
        }

        private static bool ValidarPrefixos(List<string> prefixos, string arquivo)
        {
            bool prefixoValido = true;

            if (prefixos != null && prefixos.Count > 0)
            {
                prefixoValido = false;
                foreach (string prefixo in prefixos)
                {
                    if (arquivo.Contains(prefixo))
                    {
                        prefixoValido = true;
                        break;
                    }
                }
            }
            return prefixoValido;
        }

        #endregion
    }
}
