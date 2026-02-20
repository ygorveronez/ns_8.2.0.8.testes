using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Escrituracao.Integracao
{
    [CustomAuthorize(new string[] { "ObterTotais" }, "Escrituracao/LoteEscrituracaoCancelamento")]
    public class LoteEscrituracaoCancelamentoIntegracaoController : BaseController
    {
		#region Construtores

		public LoteEscrituracaoCancelamentoIntegracaoController(Conexao conexao) : base(conexao) { }

		#endregion

        public async Task<IActionResult> ObterDadosIntegracoes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoLoteEscrituracaoCancelamento = Request.GetIntParam("LoteEscrituracaoCancelamento");

                Repositorio.Embarcador.Escrituracao.LoteEscrituracaoCancelamentoEDIIntegracao repLoteEscrituracaoCancelamentoEDIIntegracao = new Repositorio.Embarcador.Escrituracao.LoteEscrituracaoCancelamentoEDIIntegracao(unitOfWork);

                List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> tiposIntegracoesEDI = repLoteEscrituracaoCancelamentoEDIIntegracao.BuscarTipoIntegracaoPorLoteEscrituracao(codigoLoteEscrituracaoCancelamento);

                return new JsonpResult(new
                {
                    TiposIntegracoesEDI = tiposIntegracoesEDI
                });
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao obter os dados das integrações.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Finalizar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoLoteEscrituracaoCancelamento = Request.GetIntParam("LoteEscrituracaoCancelamento");

                Repositorio.Embarcador.Escrituracao.LoteEscrituracaoCancelamento repLoteEscrituracaoCancelamento = new Repositorio.Embarcador.Escrituracao.LoteEscrituracaoCancelamento(unitOfWork);

                Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoCancelamento loteEscrituracaoCancelamento = repLoteEscrituracaoCancelamento.BuscarPorCodigo(codigoLoteEscrituracaoCancelamento, true);

                if (loteEscrituracaoCancelamento == null)
                    return new JsonpResult(true, false, "Escrituração não encontrada.");

                if (loteEscrituracaoCancelamento.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLoteEscrituracaoCancelamento.AgIntegracao)
                    return new JsonpResult(true, false, "A situação da escrituração não permite a finalização da etapa.");

                unitOfWork.Start();

                loteEscrituracaoCancelamento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLoteEscrituracaoCancelamento.Finalizado;

                repLoteEscrituracaoCancelamento.Atualizar(loteEscrituracaoCancelamento, Auditado);
                
                unitOfWork.CommitChanges();
                
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao finalizar a etapa.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
    }
}
