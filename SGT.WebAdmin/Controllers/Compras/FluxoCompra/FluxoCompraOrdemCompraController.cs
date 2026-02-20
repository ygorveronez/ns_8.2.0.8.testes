using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Compras.FluxoCompra
{
    [CustomAuthorize("Compras/FluxoCompra")]
    public class FluxoCompraOrdemCompraController : BaseController
    {
        #region Construtores

        public FluxoCompraOrdemCompraController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        public async Task<IActionResult> GerarOrdemCompra()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Compras.FluxoCompra repFluxoCompra = new Repositorio.Embarcador.Compras.FluxoCompra(unitOfWork);
                Repositorio.Embarcador.Compras.OrdemCompra repOrdemCompra = new Repositorio.Embarcador.Compras.OrdemCompra(unitOfWork);
                Repositorio.Embarcador.Compras.Cotacao repCotacao = new Repositorio.Embarcador.Compras.Cotacao(unitOfWork);
                Servicos.Embarcador.Compras.CotacaoCompra serCotacaoCompra = new Servicos.Embarcador.Compras.CotacaoCompra(unitOfWork);

                Dominio.Entidades.Embarcador.Compras.FluxoCompra fluxoCompra = repFluxoCompra.BuscarPorCodigo(codigo);

                if (fluxoCompra == null)
                    return new JsonpResult(false, true, "Fluxo de compra não encontrado.");

                if (fluxoCompra.EtapaAtual != EtapaFluxoCompra.RetornoCotacao)
                    return new JsonpResult(false, true, "Ordem de compra já foi gerada para esse fluxo de compra.");

                unitOfWork.Start();

                fluxoCompra.EtapaAtual = EtapaFluxoCompra.OrdemCompra;

                Dominio.Entidades.Embarcador.Compras.CotacaoCompra cotacao = fluxoCompra.Cotacao;
                if (!serCotacaoCompra.GerarOrdemCompra(cotacao, unitOfWork, TipoServicoMultisoftware, unitOfWork.StringConexao, Auditado, false))
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, "Ocorreu uma falha ao gerar ordem de compra.");
                }

                List<Dominio.Entidades.Embarcador.Compras.OrdemCompra> ordensCompra = repOrdemCompra.BuscarPorCotacao(cotacao.Codigo);
                if (ordensCompra.Count == 0)
                    throw new ControllerException("Nenhuma ordem de compra foi gerada! Favor verificar se a cotação já teve retorno.");

                fluxoCompra.OrdensCompra = ordensCompra;

                repFluxoCompra.Atualizar(fluxoCompra);

                cotacao.Situacao = SituacaoCotacao.Finalizado;
                repCotacao.Atualizar(cotacao);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, fluxoCompra, null, "Gerou ordem de compra", unitOfWork);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, cotacao, null, "Finalizou a cotação pelo fluxo de compra nº " + fluxoCompra.Numero.ToString(), unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException ex)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao gerar ordem de compra da cotação.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarOrdensCompra()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Compras.FluxoCompra repFluxoCompra = new Repositorio.Embarcador.Compras.FluxoCompra(unitOfWork);

                Dominio.Entidades.Embarcador.Compras.FluxoCompra fluxoCompra = repFluxoCompra.BuscarPorCodigo(codigo);

                if (fluxoCompra == null)
                    return new JsonpResult(false, true, "Fluxo de compra não encontrado.");

                dynamic retorno = new
                {
                    fluxoCompra.Codigo,
                    OrdensCompra = (from obj in fluxoCompra.OrdensCompra
                                    select new
                                    {
                                        obj.Codigo,
                                        obj.Numero,
                                        Fornecedor = obj.Fornecedor.Nome,
                                        Data = obj.Data.ToString("dd/MM/yyyy"),
                                        DataPrevisaoRetorno = obj.DataPrevisaoRetorno.ToString("dd/MM/yyyy"),
                                        Situacao = obj.DescricaoSituacao,
                                        SituacaoTratativa = obj.SituacaoTratativa,
                                        ValorTotal = obj.ValorTotal.ToString("n2")
                                    }).ToList()
                };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar os ordens de compras.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> FinalizarOrdemCompra()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Compras.FluxoCompra repFluxoCompra = new Repositorio.Embarcador.Compras.FluxoCompra(unitOfWork);
                Repositorio.Embarcador.Compras.OrdemCompra repOrdemCompra = new Repositorio.Embarcador.Compras.OrdemCompra(unitOfWork);
                Repositorio.Embarcador.Compras.Cotacao repCotacao = new Repositorio.Embarcador.Compras.Cotacao(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro repConfiguracaoFinanceiro = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro(unitOfWork);

                Dominio.Entidades.Embarcador.Compras.FluxoCompra fluxoCompra = repFluxoCompra.BuscarPorCodigo(codigo);
                Dominio.Entidades.Embarcador.Compras.CotacaoCompra cotacaoCompra = repCotacao.BuscarPorCodigo(fluxoCompra.Cotacao.Codigo);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiro configuracaoFinanceiro = repConfiguracaoFinanceiro.BuscarConfiguracaoPadrao();

                if (fluxoCompra == null)
                    return new JsonpResult(false, true, "Fluxo de compra não encontrado.");

                if (fluxoCompra.EtapaAtual != EtapaFluxoCompra.OrdemCompra)
                    return new JsonpResult(false, true, "Ordem de compra já foi finalizada para esse fluxo de compra.");

                int codigoEmpresa = 0;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    codigoEmpresa = this.Usuario.Empresa.Codigo;

                unitOfWork.Start();

                foreach (Dominio.Entidades.Embarcador.Compras.OrdemCompra ordem in fluxoCompra.OrdensCompra)
                {
                    Dominio.Entidades.Embarcador.Compras.OrdemCompra ordemCompra = repOrdemCompra.BuscarPorCodigo(ordem.Codigo);
                    if (ordemCompra.Situacao == SituacaoOrdemCompra.Aberta)
                        Servicos.Embarcador.Compras.OrdemCompra.EtapaAprovacao(ref ordemCompra, unitOfWork, TipoServicoMultisoftware, unitOfWork.StringConexao, Auditado, codigoEmpresa);

                    bool fornecedorEstaPresenteNaCotacao = cotacaoCompra.Fornecedores.Any(fornecedoresCotacao => fornecedoresCotacao.Fornecedor.CPF_CNPJ == ordemCompra.Fornecedor.CPF_CNPJ);
                    if (!fornecedorEstaPresenteNaCotacao && configuracaoFinanceiro.TravarFluxoCompraCasoFornecedorDivergenteNaOrdemCompra)
                    {
                        throw new ControllerException("Não é possível finalizar a ordem de compra pois o fornecedor escolhido não consta na cotação!");
                    }
                }

                fluxoCompra.EtapaAtual = EtapaFluxoCompra.AprovacaoOrdemCompra;
                repFluxoCompra.Atualizar(fluxoCompra);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, fluxoCompra, null, "Finalizou ordem de compra", unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException ex)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao finalizar ordem de compra.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AprovarFluxoCompra()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Compras.FluxoCompra repFluxoCompra = new Repositorio.Embarcador.Compras.FluxoCompra(unitOfWork);
                Repositorio.Embarcador.Compras.FluxoCompraRecebimentoProduto repFluxoCompraRecebimentoProduto = new Repositorio.Embarcador.Compras.FluxoCompraRecebimentoProduto(unitOfWork);
                Servicos.Embarcador.Compras.CotacaoCompra serCotacaoCompra = new Servicos.Embarcador.Compras.CotacaoCompra(unitOfWork);

                Dominio.Entidades.Embarcador.Compras.FluxoCompra fluxoCompra = repFluxoCompra.BuscarPorCodigo(codigo);

                if (fluxoCompra == null)
                    return new JsonpResult(false, true, "Fluxo de compra não encontrado.");

                if (fluxoCompra.EtapaAtual != EtapaFluxoCompra.AprovacaoOrdemCompra)
                    return new JsonpResult(false, true, "Fluxo já foi aprovado.");

                if (fluxoCompra.OrdensCompra.Any(o => o.Situacao == SituacaoOrdemCompra.AgAprovacao || o.Situacao == SituacaoOrdemCompra.SemRegra))
                    return new JsonpResult(false, true, "Há ordens de compras ainda aguardando aprovação.");

                if (fluxoCompra.OrdensCompra.Any(o => o.Situacao == SituacaoOrdemCompra.Rejeitada))
                    return new JsonpResult(false, true, "Há ordens de compras rejeitadas, assim impedindo a aprovação do fluxo!");

                unitOfWork.Start();

                List<Dominio.Entidades.Embarcador.Compras.OrdemCompra> ordensCompra = fluxoCompra.OrdensCompra.Where(o => o.Situacao == SituacaoOrdemCompra.Aprovada).ToList();

                bool finalizarFluxo = true;
                bool existeMercadoria = false;
                foreach (Dominio.Entidades.Embarcador.Compras.OrdemCompra ordemCompra in ordensCompra)
                {
                    foreach (Dominio.Entidades.Embarcador.Compras.OrdemCompraMercadoria ordemCompraMercadoria in ordemCompra.Mercadorias)
                    {
                        existeMercadoria = true;
                        Dominio.Entidades.Embarcador.Compras.FluxoCompraRecebimentoProduto recebimentoProduto = new Dominio.Entidades.Embarcador.Compras.FluxoCompraRecebimentoProduto();
                        recebimentoProduto.OrdemCompraMercadoria = ordemCompraMercadoria;
                        recebimentoProduto.FluxoCompra = fluxoCompra;
                        recebimentoProduto.QuantidadeRecebida = 0;
                        recebimentoProduto.QuantidadeDocumentoFiscal = ObterQuantidadeDocumentoFiscal(ordemCompra, ordemCompraMercadoria, unitOfWork);

                        if (recebimentoProduto.QuantidadeDocumentoFiscal != recebimentoProduto.OrdemCompraMercadoria.Quantidade)
                            finalizarFluxo = false;

                        repFluxoCompraRecebimentoProduto.Inserir(recebimentoProduto);
                    }
                }
                if (!existeMercadoria)
                    finalizarFluxo = false;

                fluxoCompra.EtapaAtual = EtapaFluxoCompra.RecebimentoProduto;

                if (finalizarFluxo)
                    fluxoCompra.Situacao = SituacaoFluxoCompra.Finalizado;

                repFluxoCompra.Atualizar(fluxoCompra);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, fluxoCompra, null, "Aprovou o fluxo de compra", unitOfWork);

                unitOfWork.CommitChanges();

                serCotacaoCompra.EnviarEmailAvisoParaVencedoresPerdedores(fluxoCompra.Cotacao, unitOfWork, TipoServicoMultisoftware);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao aprovar o fluxo de compra.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> PesquisaRecebimentoProduto()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoFluxoCompra = Request.GetIntParam("Codigo");

                Models.Grid.EditableCell editableQuantidadeRecebida = new Models.Grid.EditableCell(TipoColunaGrid.aDecimal, 10);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Número O.C.", "NumeroOrdemCompra", 8, Models.Grid.Align.right, false);
                grid.AdicionarCabecalho("Fornecedor", "Fornecedor", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Produto", "Produto", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Quantidade", "Quantidade", 10, Models.Grid.Align.right, false);
                grid.AdicionarCabecalho("Quantidade Recebida", "QuantidadeRecebida", 20, Models.Grid.Align.right, true, false, false, false, true, editableQuantidadeRecebida);
                grid.AdicionarCabecalho("Quantidade Documento Fiscal", "QuantidadeDocumentoFiscal", 20, Models.Grid.Align.right, true, false, false, false, true, editableQuantidadeRecebida);

                Repositorio.Embarcador.Compras.FluxoCompraRecebimentoProduto repFluxoCompraRecebimentoProduto = new Repositorio.Embarcador.Compras.FluxoCompraRecebimentoProduto(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();

                List<Dominio.Entidades.Embarcador.Compras.FluxoCompraRecebimentoProduto> listaRegistros = repFluxoCompraRecebimentoProduto.Consultar(codigoFluxoCompra, parametrosConsulta);
                int quantidadeRegistros = repFluxoCompraRecebimentoProduto.ContarConsulta(codigoFluxoCompra);

                grid.setarQuantidadeTotal(quantidadeRegistros);

                var lista = (from p in listaRegistros
                             select new
                             {
                                 p.Codigo,
                                 NumeroOrdemCompra = p.OrdemCompraMercadoria.OrdemCompra.Numero,
                                 Fornecedor = p.OrdemCompraMercadoria.OrdemCompra.Fornecedor.Nome,
                                 Produto = p.OrdemCompraMercadoria.Produto.Descricao,
                                 Quantidade = p.OrdemCompraMercadoria.Quantidade.ToString("n2"),                                 
                                 QuantidadeRecebida = p.QuantidadeRecebida.ToString("n2"),
                                 QuantidadeDocumentoFiscal = ObterQuantidadeDocumentoFiscal(p.OrdemCompraMercadoria.OrdemCompra, p.OrdemCompraMercadoria, unitOfWork).ToString("n2")
                             }).ToList();

                grid.AdicionaRows(lista);
                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar os recebimentos de produtos.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AtualizarRecebimentoProduto()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Compras.FluxoCompraRecebimentoProduto repFluxoCompraRecebimentoProduto = new Repositorio.Embarcador.Compras.FluxoCompraRecebimentoProduto(unitOfWork);
                Dominio.Entidades.Embarcador.Compras.FluxoCompraRecebimentoProduto recebimentoProduto = repFluxoCompraRecebimentoProduto.BuscarPorCodigo(codigo);

                if (recebimentoProduto == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                recebimentoProduto.QuantidadeRecebida = Request.GetDecimalParam("QuantidadeRecebida");

                repFluxoCompraRecebimentoProduto.Atualizar(recebimentoProduto);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar o recebimento do produto.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> FinalizarFluxoCompra()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Compras.FluxoCompra repFluxoCompra = new Repositorio.Embarcador.Compras.FluxoCompra(unitOfWork);
                Repositorio.Embarcador.Compras.FluxoCompraRecebimentoProduto repFluxoCompraRecebimentoProduto = new Repositorio.Embarcador.Compras.FluxoCompraRecebimentoProduto(unitOfWork);

                Dominio.Entidades.Embarcador.Compras.FluxoCompra fluxoCompra = repFluxoCompra.BuscarPorCodigo(codigo);

                if (fluxoCompra == null)
                    return new JsonpResult(false, true, "Fluxo de compra não encontrado.");

                if (fluxoCompra.EtapaAtual != EtapaFluxoCompra.RecebimentoProduto)
                    return new JsonpResult(false, true, "Fluxo ainda não está na etapa de recebimento.");

                if (fluxoCompra.Situacao == SituacaoFluxoCompra.Finalizado)
                    return new JsonpResult(false, true, "Fluxo já foi finalizado.");

                if (repFluxoCompraRecebimentoProduto.PossuiQuantidadeRecebidaNaoInformada(codigo))
                    return new JsonpResult(false, true, "Há mercadorias sem a Quantidade Recebida informada.");

                unitOfWork.Start();

                fluxoCompra.Situacao = SituacaoFluxoCompra.Finalizado;
                repFluxoCompra.Atualizar(fluxoCompra);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, fluxoCompra, null, "Finalizou o fluxo de compra", unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao finalizar o fluxo de compra.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region método privados
        private decimal ObterQuantidadeDocumentoFiscal(Dominio.Entidades.Embarcador.Compras.OrdemCompra ordemCompra, Dominio.Entidades.Embarcador.Compras.OrdemCompraMercadoria ordemCompraMercadoria, Repositorio.UnitOfWork unitOfWork)
        {
            if (ordemCompra == null || ordemCompraMercadoria == null)
                return 0;

            Repositorio.Embarcador.Financeiro.DocumentoEntradaItem repDocumentoEntradaItem = new Repositorio.Embarcador.Financeiro.DocumentoEntradaItem(unitOfWork);
            return Convert.ToDecimal(repDocumentoEntradaItem.BuscarPorOrdemCompraEProduto(ordemCompra?.Codigo ?? 0, ordemCompraMercadoria.Produto?.Codigo ?? 0)?.Sum(x => x.Quantidade) ?? 0);
        }
        #endregion
    }
}
