using SGTAdmin.Controllers;
using System;
using Microsoft.AspNetCore.Mvc;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace SGT.WebAdmin.Controllers.Configuracoes
{
    [CustomAuthorize("Configuracoes/ConfiguracaoConciliacaoTransportador")]
    public class ConfiguracaoConciliacaoTransportadorController : BaseController
    {
		#region Construtores

		public ConfiguracaoConciliacaoTransportadorController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> ObterConfiguracao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Configuracoes.ConfiguracaoConciliacaoTransportador repConfiguracaoConciliacaoTransportador = new Repositorio.Embarcador.Configuracoes.ConfiguracaoConciliacaoTransportador(unitOfWork);
                var configuracao = repConfiguracaoConciliacaoTransportador.BuscarConfiguracaoPadrao();
                return new JsonpResult(new
                {
                    configuracao?.Codigo,
                    configuracao?.HabilitarGeracaoAutomatica,
                    configuracao?.DiasParaContestacao,
                    configuracao?.Periodicidade,
                    configuracao?.SequenciaPeriodicidade,
                    configuracao?.TipoCnpj,
                });
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao obter a configuração.");
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
                Repositorio.Embarcador.Configuracoes.ConfiguracaoConciliacaoTransportador repConfiguracaoConciliacaoTransportador = new Repositorio.Embarcador.Configuracoes.ConfiguracaoConciliacaoTransportador(unitOfWork);
                var configuracao = repConfiguracaoConciliacaoTransportador.BuscarConfiguracaoPadrao();

                if(configuracao == null)
                {
                    configuracao = new Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoConciliacaoTransportador();
                }

                configuracao.HabilitarGeracaoAutomatica = Request.GetBoolParam("HabilitarGeracaoAutomatica");
                configuracao.DiasParaContestacao = Request.GetIntParam("DiasParaContestacao");
                configuracao.Periodicidade = Request.GetEnumParam<PeriodicidadeConciliacaoTransportador>("Periodicidade");
                configuracao.SequenciaPeriodicidade = Request.GetNullableEnumParam<SequenciaPeriodicidadeConciliacaoTransportador>("SequenciaPeriodicidade");
                configuracao.TipoCnpj = Request.GetEnumParam<TipoCnpjConciliacaoTransportador>("TipoCnpj");

                if(configuracao.Codigo == 0)
                {
                    repConfiguracaoConciliacaoTransportador.Inserir(configuracao, Auditado);
                } else
                {
                    repConfiguracaoConciliacaoTransportador.Atualizar(configuracao, Auditado);
                }
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
