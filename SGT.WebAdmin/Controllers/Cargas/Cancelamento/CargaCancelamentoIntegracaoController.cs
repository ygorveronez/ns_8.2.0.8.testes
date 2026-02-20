using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Cargas.Cancelamento
{
    [CustomAuthorize(new string[] { "ObterDadosIntegracoes" }, "Cargas/CancelamentoCarga")]
    public class CargaCancelamentoIntegracaoController : BaseController
    {
		#region Construtores

		public CargaCancelamentoIntegracaoController(Conexao conexao) : base(conexao) { }

		#endregion

        public async Task<IActionResult> ObterDadosIntegracoes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCargaCancelamento;
                int.TryParse(Request.Params("CargaCancelamento"), out codigoCargaCancelamento);

                Repositorio.Embarcador.Cargas.CargaCancelamentoIntegracaoEDI repCargaCancelamentoIntegracaoEDI = new Repositorio.Embarcador.Cargas.CargaCancelamentoIntegracaoEDI(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaCancelamentoCargaCTeIntegracao repCargaCancelamentoCargaCTeIntegracao = new Repositorio.Embarcador.Cargas.CargaCancelamentoCargaCTeIntegracao(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaCancelamentoCargaIntegracao repCargaCancelamentoCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCancelamentoCargaIntegracao(unitOfWork);

                List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> tiposIntegracoesCTe = repCargaCancelamentoCargaCTeIntegracao.BuscarTipoIntegracaoPorCargaCancelamento(codigoCargaCancelamento);
                List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> tiposIntegracoesEDI = repCargaCancelamentoIntegracaoEDI.BuscarTipoIntegracaoPorCargaCancelamento(codigoCargaCancelamento);
                List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> tiposIntegracoesCarga = repCargaCancelamentoCargaIntegracao.BuscarTipoIntegracaoPorCargaCancelamento(codigoCargaCancelamento);
                
                return new JsonpResult(new
                {
                    TiposIntegracoesCTe = tiposIntegracoesCTe,
                    TiposIntegracoesEDI = tiposIntegracoesEDI,
                    TiposIntegracoesCarga = tiposIntegracoesCarga
                });
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, Localization.Resources.Cargas.CancelamentoCarga.OcorreuUmaFalhaObterDadosIntegracoes);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Finalizar()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCargaCancelamento;
                int.TryParse(Request.Params("CargaCancelamento"), out codigoCargaCancelamento);

                Repositorio.Embarcador.Cargas.CargaCancelamento repCargaCancelamento = new Repositorio.Embarcador.Cargas.CargaCancelamento(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento = repCargaCancelamento.BuscarPorCodigo(codigoCargaCancelamento);

                if (cargaCancelamento == null)
                    return new JsonpResult(true, false, Localization.Resources.Cargas.CancelamentoCarga.CancelamentoNaoEncontrados);

                if (cargaCancelamento.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoCarga.RejeicaoCancelamento)
                    return new JsonpResult(true, false, Localization.Resources.Cargas.CancelamentoCarga.SituacaoCancelamentoNaoPermiteFinalizacaoEtapas);

                unidadeDeTrabalho.Start();

                cargaCancelamento.LiberarCancelamentoComIntegracaoRejeitada = true;
                cargaCancelamento.MensagemRejeicaoCancelamento = "";
                cargaCancelamento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoCarga.EmCancelamento;

                repCargaCancelamento.Atualizar(cargaCancelamento);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaCancelamento, Localization.Resources.Cargas.CancelamentoCarga.LiberouEtapaIntegracao, unidadeDeTrabalho);

                unidadeDeTrabalho.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unidadeDeTrabalho.Rollback();

                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, Localization.Resources.Cargas.CancelamentoCarga.OcorreuUmaFalhaFinalizarEtapa);
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }
    }
}
