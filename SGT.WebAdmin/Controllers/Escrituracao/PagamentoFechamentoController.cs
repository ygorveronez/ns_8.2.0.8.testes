using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;


namespace SGT.WebAdmin.Controllers.Escrituracao
{
    [CustomAuthorize("Escrituracao/Pagamento")]
    public class PagamentoFechamentoController : BaseController
    {
		#region Construtores

		public PagamentoFechamentoController(Conexao conexao) : base(conexao) { }

		#endregion

        public async Task<IActionResult> ObterDetalhesFechamento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int.TryParse(Request.Params("Codigo"), out int codigo);
                Repositorio.Embarcador.ConfiguracaoContabil.DocumentoContabil repDocumentoContabil = new Repositorio.Embarcador.ConfiguracaoContabil.DocumentoContabil(unitOfWork);
                Repositorio.Embarcador.Escrituracao.Pagamento repPagamento = new Repositorio.Embarcador.Escrituracao.Pagamento(unitOfWork);
                Dominio.Entidades.Embarcador.Escrituracao.Pagamento pagamento = repPagamento.BuscarPorCodigo(codigo);

                List<Dominio.ObjetosDeValor.Embarcador.Escrituracao.DocumentoContabil> documentosContabeis = repDocumentoContabil.BuscarSumarizadoPorPagamento(codigo, pagamento.LotePagamentoLiberado);

                return new JsonpResult(documentosContabeis);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar os detalhes do Fechamento para a pagamento");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ConfirmarFechamentoPagamento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int.TryParse(Request.Params("Codigo"), out int codigo);
                Repositorio.Embarcador.Escrituracao.Pagamento repPagamento = new Repositorio.Embarcador.Escrituracao.Pagamento(unitOfWork);
                Repositorio.Embarcador.ConfiguracaoContabil.DocumentoContabil repDocumentoContabil = new Repositorio.Embarcador.ConfiguracaoContabil.DocumentoContabil(unitOfWork);
                Repositorio.Embarcador.Financeiro.DocumentoFaturamento repDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(unitOfWork);
                Repositorio.Embarcador.Escrituracao.DocumentoProvisao repDocumentoProvisao = new Repositorio.Embarcador.Escrituracao.DocumentoProvisao(unitOfWork);
                Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);

                Dominio.Entidades.Embarcador.Escrituracao.Pagamento pagamento = repPagamento.BuscarPorCodigo(codigo);

                if (pagamento.Situacao != SituacaoPagamento.EmFechamento || pagamento.GerandoMovimentoFinanceiro)
                    return new JsonpResult(false, true, "Não é possivel efetuar essa ação na atual situação do pagamento (" + pagamento.DescricaoSituacao + ")");

                unitOfWork.Start();

                Servicos.Embarcador.Escrituracao.Pagamento.FinalizarPagamento(pagamento, unitOfWork);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, pagamento, null, "Solicitou o fechamento da pagamento", unitOfWork, Dominio.ObjetosDeValor.Enumerador.AcaoBancoDados.Registro);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar os detalhes do Fechamento para a pagamento");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ReprocessarPagamento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int.TryParse(Request.Params("Codigo"), out int codigo);

                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Escrituracao/Pagamento");

                Repositorio.Embarcador.Escrituracao.Pagamento repPagamento = new Repositorio.Embarcador.Escrituracao.Pagamento(unitOfWork);
                Repositorio.Embarcador.ConfiguracaoContabil.DocumentoContabil repDocumentoContabil = new Repositorio.Embarcador.ConfiguracaoContabil.DocumentoContabil(unitOfWork);
                Repositorio.Embarcador.Escrituracao.DocumentoProvisao repDocumentoProvisao = new Repositorio.Embarcador.Escrituracao.DocumentoProvisao(unitOfWork);
                Repositorio.Embarcador.Financeiro.DocumentoFaturamento repDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(unitOfWork);

                Dominio.Entidades.Embarcador.Escrituracao.Pagamento pagamento = repPagamento.BuscarPorCodigo(codigo);

                if (pagamento.LotePagamentoLiberado)
                    return new JsonpResult(false, true, "Não é possivel reprocessar um pagamento liberado.");

                if (!pagamento.Situacao.PermitirCancelarOuReprocessarPagamento())
                    return new JsonpResult(false, true, "Não é possivel efetuar essa ação na atual situação do pagamento (" + pagamento.DescricaoSituacao + ")");

                if (pagamento.UltimaCargaEmCancelamento != null && !permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Pagamento_AutorizarPagmentoComCargaCancelada))
                    return new JsonpResult(false, true, "Você não possui permissão para efetuar essa operação.");

                pagamento.Initialize();

                unitOfWork.Start();

                if (pagamento.Situacao != SituacaoPagamento.PendenciaFechamento)
                {
                    repDocumentoContabil.ExcluirTodosPorPagamento(pagamento.Codigo);
                    repDocumentoFaturamento.SetarDocumentosGerarMovimentoPagamento(pagamento.Codigo);
                    repDocumentoProvisao.LimparDocumentosProvisaoPorPagamento(pagamento.Codigo);
                }

                pagamento.GerandoMovimentoFinanceiro = true;
                pagamento.GeradoAutomaticamente = false;
                pagamento.Situacao = SituacaoPagamento.EmFechamento;
                pagamento.MotivoRejeicaoFechamentoPagamento = "";

                if (pagamento.UltimaCargaEmCancelamento != null)
                {
                    pagamento.AutorizadorCargaCancelamento = this.Usuario;
                    pagamento.DataAutorizacaoCargaEmCancelamento = DateTime.Now;
                    pagamento.CargasLiberadas.Add(pagamento.UltimaCargaEmCancelamento);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, pagamento, "Autorizou pagamento mesmo com a carga " + pagamento.UltimaCargaEmCancelamento.CodigoCargaEmbarcador + " com registro de cancelamento", unitOfWork);
                    pagamento.UltimaCargaEmCancelamento = null;
                }

                repPagamento.Atualizar(pagamento, Auditado);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, pagamento, null, "Solicitou o reprocessamento da pagamento", unitOfWork, Dominio.ObjetosDeValor.Enumerador.AcaoBancoDados.Registro);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar os detalhes do Fechamento para a pagamento");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> CancelarPagamento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int.TryParse(Request.Params("Codigo"), out int codigo);
                Repositorio.Embarcador.Escrituracao.Pagamento repPagamento = new Repositorio.Embarcador.Escrituracao.Pagamento(unitOfWork);
                Repositorio.Embarcador.ConfiguracaoContabil.DocumentoContabil repDocumentoContabil = new Repositorio.Embarcador.ConfiguracaoContabil.DocumentoContabil(unitOfWork);
                Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);
                Repositorio.Embarcador.Escrituracao.DocumentoProvisao repDocumentoProvisao = new Repositorio.Embarcador.Escrituracao.DocumentoProvisao(unitOfWork);
                Repositorio.Embarcador.Financeiro.DocumentoFaturamento repDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(unitOfWork);
                Repositorio.Embarcador.Escrituracao.AlcadasPagamento.AprovacaoAlcadaPagamento repAprovacaoAlcadaPagamento = new Repositorio.Embarcador.Escrituracao.AlcadasPagamento.AprovacaoAlcadaPagamento(unitOfWork);

                Dominio.Entidades.Embarcador.Escrituracao.Pagamento pagamento = repPagamento.BuscarPorCodigo(codigo);

                if (!pagamento.Situacao.PermitirCancelarOuReprocessarPagamento())
                    return new JsonpResult(false, true, "Não é possivel efetuar essa ação na atual situação do pagamento (" + pagamento.DescricaoSituacao + ")");

                unitOfWork.Start();
   
                if (!pagamento.LotePagamentoLiberado)
                {
                    new Servicos.Embarcador.Escrituracao.PagamentoAprovacao(unitOfWork).RemoverAprovacao(pagamento);
                    repDocumentoContabil.ExcluirTodosPorPagamento(pagamento.Codigo);
                    repDocumentoFaturamento.SetarDocumentosLiberadosPagamento(pagamento.Codigo);
                    repTitulo.LiberarPagamentosPorPagamento(pagamento.Codigo);
                    repDocumentoProvisao.LimparDocumentosProvisaoPorPagamento(pagamento.Codigo);
                }
                else
                {
                    repDocumentoContabil.LiberarPorPagamento(pagamento.Codigo);
                    repDocumentoFaturamento.SetarDocumentosLiberadosPagamentoLiberado(pagamento.Codigo);
                }

                pagamento.CargasLiberadas.Clear();

                repPagamento.Deletar(pagamento);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, pagamento, null, "Solicitou a exclusão da pagamento", unitOfWork, Dominio.ObjetosDeValor.Enumerador.AcaoBancoDados.Registro);
                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar os detalhes do Fechamento para a pagamento");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
    }
}
