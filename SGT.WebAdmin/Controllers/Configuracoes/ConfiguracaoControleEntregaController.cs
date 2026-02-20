using SGTAdmin.Controllers;
using System;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Configuracoes
{
    [CustomAuthorize("Configuracoes/ConfiguracaoControleEntrega")]
    public class ConfiguracaoControleEntregaController : BaseController
    {
		#region Construtores

		public ConfiguracaoControleEntregaController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Atualizar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Configuracoes.ConfiguracaoControleEntrega repConfiguracaoControleEntrega = new Repositorio.Embarcador.Configuracoes.ConfiguracaoControleEntrega(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoControleEntrega configuracaoControleEntrega = repConfiguracaoControleEntrega.ObterConfiguracaoPadrao();

                string mensagemChatAssumirMonitoramentoCarga = Request.Params("MensagemChatAssumirMonitoramentoCarga");
                configuracaoControleEntrega.MensagemChatAssumirMonitoramentoCarga = mensagemChatAssumirMonitoramentoCarga;
                configuracaoControleEntrega.TempoInicioViagemAposEmissaoDoc = Request.GetIntParam("TempoInicioViagemAposEmissaoDoc");
                configuracaoControleEntrega.TempoInicioViagemAposFinalizacaoFluxoPatio = Request.GetIntParam("TempoInicioViagemAposFinalizacaoFluxoPatio");

                configuracaoControleEntrega.MensagemChatAssumirMonitoramentoCarga = mensagemChatAssumirMonitoramentoCarga;
                repConfiguracaoControleEntrega.Atualizar(configuracaoControleEntrega, Auditado);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoAtualizar) ;
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarPorPadrao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Configuracoes.ConfiguracaoControleEntrega repConfiguracaoControleEntrega = new Repositorio.Embarcador.Configuracoes.ConfiguracaoControleEntrega(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoControleEntrega configuracao = repConfiguracaoControleEntrega.ObterConfiguracaoPadrao();

                var retorno = new
                {
                    configuracao.Codigo,
                    configuracao.MensagemChatAssumirMonitoramentoCarga,
                    configuracao.TempoInicioViagemAposEmissaoDoc,
                    configuracao.TempoInicioViagemAposFinalizacaoFluxoPatio
                };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoBuscarConfiguracaoPadrao );
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion
    }
}
