using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Financeiros.Bordero
{
    [CustomAuthorize("Financeiros/Bordero")]
    public class BorderoTituloDocumentoAcrescimoDescontoController : BaseController
    {
		#region Construtores

		public BorderoTituloDocumentoAcrescimoDescontoController(Conexao conexao) : base(conexao) { }

		#endregion

        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int.TryParse(Request.Params("Documento"), out int codigoBorderoTituloDocumento);

                Repositorio.Embarcador.Financeiro.BorderoTituloDocumentoAcrescimoDesconto repBorderoTituloDocumentoAcrescimoDesconto = new Repositorio.Embarcador.Financeiro.BorderoTituloDocumentoAcrescimoDesconto(unitOfWork);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Justificativa", "Justificativa", 25, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Tipo", "DescricaoTipoJustificativa", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Observação", "Observacao", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Valor", "Valor", 20, Models.Grid.Align.left, false);

                string propOrdena = grid.header[grid.indiceColunaOrdena].data;

                if (propOrdena == "Justificativa")
                    propOrdena += ".Descricao";
                else if (propOrdena == "DescricaoTipoJustificativa")
                    propOrdena = "TipoJustificativa";

                List<Dominio.Entidades.Embarcador.Financeiro.BorderoTituloDocumentoAcrescimoDesconto> listaAcrescimoDesconto = repBorderoTituloDocumentoAcrescimoDesconto.Consultar(codigoBorderoTituloDocumento, propOrdena, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repBorderoTituloDocumentoAcrescimoDesconto.ContarConsulta(codigoBorderoTituloDocumento));

                var lista = (from p in listaAcrescimoDesconto
                             select new
                             {
                                 p.Codigo,
                                 Justificativa = p.Justificativa.Descricao,
                                 p.DescricaoTipoJustificativa,
                                 p.Observacao,
                                 Valor = p.Valor.ToString("n2")
                             }).ToList();

                grid.AdicionaRows(lista);

                return new JsonpResult(grid);
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

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Financeiro.BorderoTituloDocumento repBorderoTituloDocumento = new Repositorio.Embarcador.Financeiro.BorderoTituloDocumento(unidadeTrabalho);
                Repositorio.Embarcador.Financeiro.BorderoTituloDocumentoAcrescimoDesconto repBorderoTituloDocumentoAcrescimoDesconto = new Repositorio.Embarcador.Financeiro.BorderoTituloDocumentoAcrescimoDesconto(unidadeTrabalho);
                Repositorio.Embarcador.Fatura.Justificativa repJustificativa = new Repositorio.Embarcador.Fatura.Justificativa(unidadeTrabalho);

                int.TryParse(Request.Params("Documento"), out int codigoBorderoTituloDocumento);
                int.TryParse(Request.Params("Justificativa"), out int codigoJustificativa);

                decimal.TryParse(Request.Params("Valor"), out decimal valor);

                string observacao = Request.Params("Observacao");

                Dominio.Entidades.Embarcador.Financeiro.BorderoTituloDocumento borderoTituloDocumento = repBorderoTituloDocumento.BuscarPorCodigo(codigoBorderoTituloDocumento);

                if (borderoTituloDocumento.BorderoTitulo.Bordero.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoBordero.EmAndamento)
                    return new JsonpResult(false, true, "Não é possível adicionar um acréscimo/desconto na situação atual do borderô.");

                unidadeTrabalho.Start();

                Dominio.Entidades.Embarcador.Financeiro.BorderoTituloDocumentoAcrescimoDesconto borderoTituloDocumentoAcrescimoDesconto = new Dominio.Entidades.Embarcador.Financeiro.BorderoTituloDocumentoAcrescimoDesconto()
                {
                    BorderoTituloDocumento = borderoTituloDocumento,
                    Observacao = observacao,
                    Justificativa = repJustificativa.BuscarPorCodigo(codigoJustificativa),
                    Valor = valor
                };

                if (!borderoTituloDocumentoAcrescimoDesconto.Justificativa.GerarMovimentoAutomatico)
                {
                    unidadeTrabalho.Rollback();
                    return new JsonpResult(false, true, "A justificativa não possui a movimentação financeira configurada, não sendo possível adicioná-la.");
                }

                borderoTituloDocumentoAcrescimoDesconto.TipoJustificativa = borderoTituloDocumentoAcrescimoDesconto.Justificativa.TipoJustificativa;
                borderoTituloDocumentoAcrescimoDesconto.TipoMovimentoUso = borderoTituloDocumentoAcrescimoDesconto.Justificativa.TipoMovimentoUsoJustificativa;
                borderoTituloDocumentoAcrescimoDesconto.TipoMovimentoReversao = borderoTituloDocumentoAcrescimoDesconto.Justificativa.TipoMovimentoReversaoUsoJustificativa;

                repBorderoTituloDocumentoAcrescimoDesconto.Inserir(borderoTituloDocumentoAcrescimoDesconto, Auditado);

                if (borderoTituloDocumentoAcrescimoDesconto.TipoJustificativa == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoJustificativa.Acrescimo)
                    borderoTituloDocumento.ValorTotalAcrescimo += borderoTituloDocumentoAcrescimoDesconto.Valor;
                else
                    borderoTituloDocumento.ValorTotalDesconto += borderoTituloDocumentoAcrescimoDesconto.Valor;

                borderoTituloDocumento.ValorTotalACobrar = borderoTituloDocumento.ValorACobrar + borderoTituloDocumento.ValorTotalAcrescimo - borderoTituloDocumento.ValorTotalDesconto;

                repBorderoTituloDocumento.Atualizar(borderoTituloDocumento);

                Dominio.Entidades.Embarcador.Financeiro.BorderoTitulo borderoTitulo = borderoTituloDocumento.BorderoTitulo;
                Dominio.Entidades.Embarcador.Financeiro.Bordero bordero = borderoTituloDocumento.BorderoTitulo.Bordero;

                Servicos.Embarcador.Financeiro.Bordero.AtualizarTotaisBorderoTitulo(ref borderoTitulo, unidadeTrabalho);
                Servicos.Embarcador.Financeiro.Bordero.AtualizarTotaisBordero(ref bordero, unidadeTrabalho);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, borderoTituloDocumento, null, "Adicionou um " + borderoTituloDocumentoAcrescimoDesconto.DescricaoTipoJustificativa + ".", unidadeTrabalho);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, borderoTitulo, null, "Adicionou um " + borderoTituloDocumentoAcrescimoDesconto.DescricaoTipoJustificativa + " ao título.", unidadeTrabalho);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, bordero, null, "Adicionou um " + borderoTituloDocumentoAcrescimoDesconto.DescricaoTipoJustificativa + " ao título " + borderoTitulo.Descricao + ".", unidadeTrabalho);

                unidadeTrabalho.CommitChanges();

                return new JsonpResult(Servicos.Embarcador.Financeiro.Bordero.ObterDetalhesBordero(bordero));
            }
            catch (Exception ex)
            {
                unidadeTrabalho.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, true, "Ocorreu uma falha ao adicionar o valor.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> Atualizar()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int.TryParse(Request.Params("Justificativa"), out int codigoJustificativa);
                int.TryParse(Request.Params("Codigo"), out int codigo);

                decimal.TryParse(Request.Params("Valor"), out decimal valor);

                string observacao = Request.Params("Observacao");

                Repositorio.Embarcador.Financeiro.BorderoTituloDocumento repBorderoTituloDocumento = new Repositorio.Embarcador.Financeiro.BorderoTituloDocumento(unidadeTrabalho);
                Repositorio.Embarcador.Financeiro.BorderoTituloDocumentoAcrescimoDesconto repBorderoTituloDocumentoAcrescimoDesconto = new Repositorio.Embarcador.Financeiro.BorderoTituloDocumentoAcrescimoDesconto(unidadeTrabalho);
                Repositorio.Embarcador.Fatura.Justificativa repJustificativa = new Repositorio.Embarcador.Fatura.Justificativa(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Financeiro.BorderoTituloDocumentoAcrescimoDesconto borderoTituloDocumentoAcrescimoDesconto = repBorderoTituloDocumentoAcrescimoDesconto.BuscarPorCodigo(codigo, true);
                Dominio.Entidades.Embarcador.Financeiro.BorderoTituloDocumento borderoTituloDocumento = borderoTituloDocumentoAcrescimoDesconto.BorderoTituloDocumento;

                if (borderoTituloDocumento.BorderoTitulo.Bordero.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoBordero.EmAndamento)
                    return new JsonpResult(false, true, "Não é possível atualizar um acréscimo/desconto na situação atual do borderô.");

                unidadeTrabalho.Start();

                if (borderoTituloDocumentoAcrescimoDesconto.TipoJustificativa == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoJustificativa.Acrescimo)
                    borderoTituloDocumento.ValorTotalAcrescimo -= borderoTituloDocumentoAcrescimoDesconto.Valor;
                else
                    borderoTituloDocumento.ValorTotalDesconto -= borderoTituloDocumentoAcrescimoDesconto.Valor;

                borderoTituloDocumentoAcrescimoDesconto.Justificativa = repJustificativa.BuscarPorCodigo(codigoJustificativa);

                if (!borderoTituloDocumentoAcrescimoDesconto.Justificativa.GerarMovimentoAutomatico)
                {
                    unidadeTrabalho.Rollback();
                    return new JsonpResult(false, true, "A justificativa não possui a movimentação financeira configurada, não sendo possível adicioná-la.");
                }

                borderoTituloDocumentoAcrescimoDesconto.TipoJustificativa = borderoTituloDocumentoAcrescimoDesconto.Justificativa.TipoJustificativa;
                borderoTituloDocumentoAcrescimoDesconto.TipoMovimentoUso = borderoTituloDocumentoAcrescimoDesconto.Justificativa.TipoMovimentoUsoJustificativa;
                borderoTituloDocumentoAcrescimoDesconto.TipoMovimentoReversao = borderoTituloDocumentoAcrescimoDesconto.Justificativa.TipoMovimentoReversaoUsoJustificativa;
                borderoTituloDocumentoAcrescimoDesconto.Valor = valor;
                borderoTituloDocumentoAcrescimoDesconto.Observacao = observacao;

                repBorderoTituloDocumentoAcrescimoDesconto.Atualizar(borderoTituloDocumentoAcrescimoDesconto, Auditado);

                if (borderoTituloDocumentoAcrescimoDesconto.TipoJustificativa == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoJustificativa.Acrescimo)
                    borderoTituloDocumento.ValorTotalAcrescimo += borderoTituloDocumentoAcrescimoDesconto.Valor;
                else
                    borderoTituloDocumento.ValorTotalDesconto += borderoTituloDocumentoAcrescimoDesconto.Valor;

                borderoTituloDocumento.ValorTotalACobrar = borderoTituloDocumento.ValorACobrar + borderoTituloDocumento.ValorTotalAcrescimo - borderoTituloDocumento.ValorTotalDesconto;

                repBorderoTituloDocumento.Atualizar(borderoTituloDocumento);

                Dominio.Entidades.Embarcador.Financeiro.BorderoTitulo borderoTitulo = borderoTituloDocumento.BorderoTitulo;
                Dominio.Entidades.Embarcador.Financeiro.Bordero bordero = borderoTitulo.Bordero;

                Servicos.Embarcador.Financeiro.Bordero.AtualizarTotaisBorderoTitulo(ref borderoTitulo, unidadeTrabalho);
                Servicos.Embarcador.Financeiro.Bordero.AtualizarTotaisBordero(ref bordero, unidadeTrabalho);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, borderoTituloDocumento, null, "Atualizou um " + borderoTituloDocumentoAcrescimoDesconto.DescricaoTipoJustificativa + ".", unidadeTrabalho);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, borderoTitulo, null, "Atualizou um " + borderoTituloDocumentoAcrescimoDesconto.DescricaoTipoJustificativa + " do título.", unidadeTrabalho);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, bordero, null, "Atualizou um " + borderoTituloDocumentoAcrescimoDesconto.DescricaoTipoJustificativa + " do título " + borderoTitulo.Descricao + ".", unidadeTrabalho);

                unidadeTrabalho.CommitChanges();

                return new JsonpResult(Servicos.Embarcador.Financeiro.Bordero.ObterDetalhesBordero(bordero));
            }
            catch (Exception ex)
            {
                unidadeTrabalho.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, true, "Ocorreu uma falha ao atualizar o valor.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> Excluir()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Financeiro.BorderoTituloDocumento repBorderoTituloDocumento = new Repositorio.Embarcador.Financeiro.BorderoTituloDocumento(unidadeTrabalho);
                Repositorio.Embarcador.Financeiro.BorderoTituloDocumentoAcrescimoDesconto repBorderoTituloDocumentoAcrescimoDesconto = new Repositorio.Embarcador.Financeiro.BorderoTituloDocumentoAcrescimoDesconto(unidadeTrabalho);

                unidadeTrabalho.Start();

                Dominio.Entidades.Embarcador.Financeiro.BorderoTituloDocumentoAcrescimoDesconto borderoTituloDocumentoAcrescimoDesconto = repBorderoTituloDocumentoAcrescimoDesconto.BuscarPorCodigo(codigo);
                Dominio.Entidades.Embarcador.Financeiro.BorderoTituloDocumento borderoTituloDocumento = borderoTituloDocumentoAcrescimoDesconto.BorderoTituloDocumento;

                if (borderoTituloDocumento.BorderoTitulo.Bordero.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoBordero.EmAndamento)
                    return new JsonpResult(false, true, "Não é possível excluir um acréscimo/desconto na situação atual do borderô.");

                if (borderoTituloDocumentoAcrescimoDesconto.TipoJustificativa == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoJustificativa.Acrescimo)
                    borderoTituloDocumento.ValorTotalAcrescimo -= borderoTituloDocumentoAcrescimoDesconto.Valor;
                else
                    borderoTituloDocumento.ValorTotalDesconto -= borderoTituloDocumentoAcrescimoDesconto.Valor;

                borderoTituloDocumento.ValorTotalACobrar = borderoTituloDocumento.ValorACobrar + borderoTituloDocumento.ValorTotalAcrescimo - borderoTituloDocumento.ValorTotalDesconto;

                string descricaoTipoJustificativa = borderoTituloDocumentoAcrescimoDesconto.DescricaoTipoJustificativa;

                repBorderoTituloDocumento.Atualizar(borderoTituloDocumento);
                repBorderoTituloDocumentoAcrescimoDesconto.Deletar(borderoTituloDocumentoAcrescimoDesconto, Auditado);

                Dominio.Entidades.Embarcador.Financeiro.BorderoTitulo borderoTitulo = borderoTituloDocumento.BorderoTitulo;
                Dominio.Entidades.Embarcador.Financeiro.Bordero bordero = borderoTituloDocumento.BorderoTitulo.Bordero;

                Servicos.Embarcador.Financeiro.Bordero.AtualizarTotaisBorderoTitulo(ref borderoTitulo, unidadeTrabalho);
                Servicos.Embarcador.Financeiro.Bordero.AtualizarTotaisBordero(ref bordero, unidadeTrabalho);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, borderoTitulo, null, "Removeu um " + descricaoTipoJustificativa + " do título.", unidadeTrabalho);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, borderoTituloDocumento, null, "Removeu um " + descricaoTipoJustificativa + ".", unidadeTrabalho);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, bordero, null, "Removeu um " + descricaoTipoJustificativa + " do título " + borderoTitulo.Descricao + ".", unidadeTrabalho);

                unidadeTrabalho.CommitChanges();

                return new JsonpResult(Servicos.Embarcador.Financeiro.Bordero.ObterDetalhesBordero(bordero));
            }
            catch (Exception ex)
            {
                unidadeTrabalho.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, true, "Ocorreu uma falha ao remover o acréscimo/desconto.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Financeiro.BorderoTituloDocumentoAcrescimoDesconto repBorderoTituloDocumentoAcrescimoDesconto = new Repositorio.Embarcador.Financeiro.BorderoTituloDocumentoAcrescimoDesconto(unidadeTrabalho);
                
                Dominio.Entidades.Embarcador.Financeiro.BorderoTituloDocumentoAcrescimoDesconto borderoTituloDocumentoAcrescimoDesconto = repBorderoTituloDocumentoAcrescimoDesconto.BuscarPorCodigo(codigo);

                return new JsonpResult(new
                {
                    borderoTituloDocumentoAcrescimoDesconto.Codigo,
                    Documento = borderoTituloDocumentoAcrescimoDesconto.BorderoTituloDocumento.Codigo,
                    Valor = borderoTituloDocumentoAcrescimoDesconto.Valor.ToString("n2"),
                    Justificativa = new
                    {
                        Codigo = borderoTituloDocumentoAcrescimoDesconto.Justificativa.Codigo,
                        Descricao = borderoTituloDocumentoAcrescimoDesconto.Justificativa.Descricao
                    },
                    borderoTituloDocumentoAcrescimoDesconto.Observacao
                });
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, true, "Ocorreu uma falha ao atualizar o valor.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }
    }
}
