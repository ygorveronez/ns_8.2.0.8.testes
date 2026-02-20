using SGTAdmin.Controllers;
using System;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Configuracoes
{
    [CustomAuthorize("Configuracoes/ConfiguracaoDocumentacaoAFRMM")]
    public class ConfiguracaoDocumentacaoAFRMMController : BaseController
    {
		#region Construtores

		public ConfiguracaoDocumentacaoAFRMMController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> ObterConfiguracao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Configuracoes.ConfiguracaoDocumentacaoAFRMM repConfiguracaoDocumentacaoAFRMM = new Repositorio.Embarcador.Configuracoes.ConfiguracaoDocumentacaoAFRMM(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoDocumentacaoAFRMM configuracao = repConfiguracaoDocumentacaoAFRMM.BuscarConfiguracaoPadrao();

                return new JsonpResult(new { 
                    configuracao?.Codigo,
                    configuracao?.QuantidadeDiasAposDescarga,
                    configuracao?.EnderecoFTP,
                    configuracao?.PortaFTP,
                    configuracao?.DiretorioFTP,
                    configuracao?.DiretorioFTPSubcontratacao,
                    configuracao?.DiretorioFTPRedespacho,
                    configuracao?.UsuarioFTP,
                    configuracao?.SenhaFTP,
                    configuracao?.FTPPassivo,
                    configuracao?.SFTP,
                    configuracao?.SSL,
                    configuracao?.EmailFalhaEnvio
                });
            }
            catch(Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao obter a configuração.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AtualizarConfiguracao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Configuracoes.ConfiguracaoDocumentacaoAFRMM repConfiguracaoDocumentacaoAFRMM = new Repositorio.Embarcador.Configuracoes.ConfiguracaoDocumentacaoAFRMM(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoDocumentacaoAFRMM configuracao = repConfiguracaoDocumentacaoAFRMM.BuscarConfiguracaoPadrao();

                if (configuracao == null)
                    configuracao = new Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoDocumentacaoAFRMM();

                configuracao.QuantidadeDiasAposDescarga = Request.GetIntParam("QuantidadeDiasAposDescarga");
                configuracao.EnderecoFTP = Request.GetStringParam("EnderecoFTP");
                configuracao.PortaFTP = Request.GetStringParam("PortaFTP");
                configuracao.DiretorioFTP = Request.GetStringParam("DiretorioFTP");
                configuracao.DiretorioFTPSubcontratacao = Request.GetStringParam("DiretorioFTPSubcontratacao");
                configuracao.DiretorioFTPRedespacho = Request.GetStringParam("DiretorioFTPRedespacho");
                configuracao.UsuarioFTP = Request.GetStringParam("UsuarioFTP");
                configuracao.SenhaFTP = Request.GetStringParam("SenhaFTP");
                configuracao.FTPPassivo = Request.GetBoolParam("FTPPassivo");
                configuracao.SFTP = Request.GetBoolParam("SFTP");
                configuracao.SSL = Request.GetBoolParam("SSL");
                configuracao.EmailFalhaEnvio = Request.GetStringParam("EmailFalhaEnvio");

                if (configuracao.Codigo == 0)
                    repConfiguracaoDocumentacaoAFRMM.Inserir(configuracao);
                else
                    repConfiguracaoDocumentacaoAFRMM.Atualizar(configuracao);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar a configuração.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
        #endregion
    }
}
