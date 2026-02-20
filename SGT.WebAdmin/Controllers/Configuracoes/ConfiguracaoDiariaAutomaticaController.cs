using SGTAdmin.Controllers;
using System;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Configuracoes
{
    [CustomAuthorize("Configuracoes/ConfiguracaoDiariaAutomatica")]
    public class ConfiguracaoDiariaAutomaticaController : BaseController
    {
		#region Construtores

		public ConfiguracaoDiariaAutomaticaController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> ObterConfiguracao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Configuracoes.ConfiguracaoDiariaAutomatica repFeriadoConfiguracaoDiariaAutomatica = new Repositorio.Embarcador.Configuracoes.ConfiguracaoDiariaAutomatica(unitOfWork);
                var configuracao = repFeriadoConfiguracaoDiariaAutomatica.BuscarConfiguracaoPadrao();
                return new JsonpResult(new
                {
                    configuracao.HabilitarDiariaAutomatica,
                    configuracao.FrequenciaAtualizacao
                });
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar.");
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
                Repositorio.Embarcador.Configuracoes.ConfiguracaoDiariaAutomatica repFeriadoConfiguracaoDiariaAutomatica = new Repositorio.Embarcador.Configuracoes.ConfiguracaoDiariaAutomatica(unitOfWork);
                var configuracao = repFeriadoConfiguracaoDiariaAutomatica.BuscarConfiguracaoPadrao();

                configuracao.HabilitarDiariaAutomatica = Request.GetBoolParam("HabilitarDiariaAutomatica");
                configuracao.FrequenciaAtualizacao = Request.GetIntParam("FrequenciaAtualizacao");

                repFeriadoConfiguracaoDiariaAutomatica.Atualizar(configuracao, Auditado);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion
    }
}
