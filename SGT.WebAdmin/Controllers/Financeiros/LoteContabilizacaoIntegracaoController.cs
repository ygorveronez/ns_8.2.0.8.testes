using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Financeiros
{
    [CustomAuthorize(new string[] { "ObterTotais" }, "Financeiros/LoteContabilizacao")]
    public class LoteContabilizacaoIntegracaoController : BaseController
    {
		#region Construtores

		public LoteContabilizacaoIntegracaoController(Conexao conexao) : base(conexao) { }

		#endregion

        public async Task<IActionResult> ObterDadosIntegracoes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoLoteContabilizacao = Request.GetIntParam("LoteContabilizacao");

                Repositorio.Embarcador.Financeiro.LoteContabilizacaoIntegracaoEDI repLoteContabilizacaoIntegracaoEDI = new Repositorio.Embarcador.Financeiro.LoteContabilizacaoIntegracaoEDI(unitOfWork);

                List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> tiposIntegracoesCTe = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao>();

                List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> tiposIntegracoesEDI = repLoteContabilizacaoIntegracaoEDI.BuscarTipoIntegracaoPorLoteContabilizacao(codigoLoteContabilizacao);

                return new JsonpResult(new
                {
                    TiposIntegracoesCTe = tiposIntegracoesCTe,
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
                //List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/Carga", "Logistica/JanelaCarregamento");
                //if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_ConfirmarIntegracao))
                //    return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

                int codigoLoteEscrituracao = Request.GetIntParam("LoteContabilizacao");

                Repositorio.Embarcador.Financeiro.LoteContabilizacao repLoteContabilizacao = new Repositorio.Embarcador.Financeiro.LoteContabilizacao(unitOfWork);
                //Repositorio.Embarcador.Cargas.CargaLog repLog = new Repositorio.Embarcador.Cargas.CargaLog(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Financeiro.LoteContabilizacao loteContabilizacao = repLoteContabilizacao.BuscarPorCodigo(codigoLoteEscrituracao, true);

                if (loteContabilizacao == null)
                    return new JsonpResult(true, false, "Lote de contabilização não encontrado.");

                if (loteContabilizacao.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLoteContabilizacao.AgIntegracao)
                    return new JsonpResult(true, false, "A situação do lote de contabilização não permite a finalização da etapa.");

                unitOfWork.Start();

                loteContabilizacao.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLoteContabilizacao.Finalizado;

                repLoteContabilizacao.Atualizar(loteContabilizacao);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, loteContabilizacao, null, "Finalizou.", unitOfWork);

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
