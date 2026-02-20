using SGTAdmin.Controllers;
using System;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Fretes
{
    [CustomAuthorize("Fretes/ConfiguracaoNotificacaoContrato")]
    public class ConfiguracaoNotificacaoContratoController : BaseController
    {
		#region Construtores

		public ConfiguracaoNotificacaoContratoController(Conexao conexao) : base(conexao) { }

		#endregion

        public async Task<IActionResult> ObterConfiguracao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

                // Formata retorno
                var retorno = new
                {
                    Dias = configuracao.DiasAvisoVencimentoCotratoFrete,
                    Emails = configuracao.EmailsAvisoVencimentoCotratoFrete
                };

                // Retorna informacoes
                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Atualizar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

                configuracao.Initialize();

                int.TryParse(Request.Params("Dias"), out int dias);
                string emails = Request.Params("Emails") ?? string.Empty;

                configuracao.DiasAvisoVencimentoCotratoFrete = dias;
                configuracao.EmailsAvisoVencimentoCotratoFrete = emails;

                // Persiste dados
                unitOfWork.Start();

                repConfiguracaoTMS.Atualizar(configuracao);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracao, configuracao.GetChanges(), "Alterou os dados de notificação de contrato de frete.", unitOfWork);

                unitOfWork.CommitChanges();

                // Retorna sucesso
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
    }
}
