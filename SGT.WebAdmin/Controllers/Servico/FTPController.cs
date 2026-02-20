using SGTAdmin.Controllers;
using System;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Servico
{
    [CustomAuthorize("Servicos/FTP")]
    public class FTPController : BaseController
    {
		#region Construtores

		public FTPController(Conexao conexao) : base(conexao) { }

		#endregion

        [AllowAuthenticate]
        public async Task<IActionResult> TestarConexao()
        {
            try
            {
                string endereco = Request.Params("Host");
                string usuario = Request.Params("Usuario");
                string senha = Request.Params("Senha");
                string porta = Request.Params("Porta");
                string diretorio = Request.Params("Diretorio");
                string certificado = Request.Params("CertificadoChavePrivadaBase64");

                bool.TryParse(Request.Params("Passivo"), out bool passivo);
                bool.TryParse(Request.Params("UtilizarSFTP"), out bool utilizarSFTP);
                bool.TryParse(Request.Params("SSL"), out bool ssl);

                string erro;

                if (Servicos.FTP.TestarConexao(endereco, porta, diretorio, usuario, senha, passivo, ssl, out erro, utilizarSFTP, certificado))
                    return new JsonpResult(true);

                return new JsonpResult(false, erro);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, Localization.Resources.Transportadores.Transportador.OcorreuUmaFalhaAoTestarConexaoAoFTP);
            }
        }

        //    public async Task<IActionResult> TestarConexaoFTPs()
        //    {
        //        try
        //        {
        //            string endereco = "ftp.ferrero.com.br";
        //            string usuario = "ftpdanonehom";
        //            string senha = "(F3rr3r0!FTP#Dan0n3)hom";
        //            string porta = "990";
        //            string diretorio = "/Test/";

        //            bool passivo = true;
        //            string erro = "";

        //            // TEST
        //            //if (!Servicos.FTP.TestarConexao(endereco, porta, diretorio, usuario, senha, passivo, out erro, false))
        //            //    return new JsonpResult(false, erro);

        //            // UPLOAD
        //            string nomeArquivo = "text.txt";
        //            string texto = "TESTE 13/08/2018";
        //            System.IO.MemoryStream file = new System.IO.MemoryStream(System.Text.Encoding.UTF8.GetBytes(texto));
        //            if (!Servicos.FTP.EnviarArquivo(file, nomeArquivo, endereco, porta, diretorio, usuario, senha, passivo, out erro, false))
        //                return new JsonpResult(false, erro);

        //            // DOWNLOAD
        //            string diretorioDownload = @"C:\Arquivos\FTPS\";
        //            var download = Servicos.FTP.DownloadArquivo(endereco, porta, diretorio, usuario, senha, passivo, nomeArquivo, diretorioDownload, nomeArquivo, out erro, false);
        //            if (!string.IsNullOrWhiteSpace(erro))
        //                return new JsonpResult(false, erro);

        //            // DELETE
        //            Servicos.FTP.DeletarArquivo(endereco, porta, diretorio, usuario, senha, passivo, nomeArquivo, out erro, false);
        //            if (!string.IsNullOrWhiteSpace(erro))
        //                return new JsonpResult(false, erro);

        //            return new JsonpResult(true);
        //        }
        //        catch (Exception ex)
        //        {
        //            Servicos.Log.TratarErro(ex);

        //            return new JsonpResult(false, false, "Ocorreu uma falha ao testar a conexão ao FTP.");
        //        }
        //    }
    }
}
