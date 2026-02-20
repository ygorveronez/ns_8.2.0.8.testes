using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.AspNetCore.Mvc;
using System.IO;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Relatorios;
using Newtonsoft.Json;
using Servicos.Extensions;

namespace SGT.WebAdmin.Controllers.Financeiros.Conciliacao
{
    [CustomAuthorize("Financeiros/ConciliacaoBancaria")]
    public class ConciliacaoBancariaController : BaseController
    {
		#region Construtores

		public ConciliacaoBancariaController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaConciliacaoBancaria filtrosPesquisa = ObterFiltrosPesquisa();

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Plano de Contas", "PlanoConta", 30, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Data Inicial", "DataInicial", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Data Final", "DataFinal", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Data Geração", "DataGeracaoMovimento", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Situação", "SituacaoConciliacaoBancaria", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Valor Extrato", "ValorTotalExtrato", 10, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho("Valor Movimento", "ValorTotalMovimento", 10, Models.Grid.Align.right, true);

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;

                Repositorio.Embarcador.Financeiro.Conciliacao.ConciliacaoBancaria repConciliacaoBancaria = new Repositorio.Embarcador.Financeiro.Conciliacao.ConciliacaoBancaria(unitOfWork);
                List<Dominio.Entidades.Embarcador.Financeiro.Conciliacao.ConciliacaoBancaria> listaConciliacaoBancaria = repConciliacaoBancaria.Consultar(filtrosPesquisa, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repConciliacaoBancaria.ContarConsulta(filtrosPesquisa));

                var lista = (from p in listaConciliacaoBancaria
                             select new
                             {
                                 p.Codigo,
                                 PlanoConta = p.PlanoConta?.BuscarDescricao ?? p.PlanoContaSintetico?.Descricao ?? "",
                                 DataInicial = p.DataInicial.HasValue ? p.DataInicial.Value.ToString("dd/MM/yyyy") : "",
                                 DataFinal = p.DataFinal.HasValue ? p.DataFinal.Value.ToString("dd/MM/yyyy") : "",
                                 DataGeracaoMovimento = p.DataGeracaoMovimento.ToString("dd/MM/yyyy"),
                                 SituacaoConciliacaoBancaria = p.DescricaoSituacaoConciliacaoBancaria,
                                 ValorTotalExtrato = p.ValorTotalExtrato.ToString("n2"),
                                 ValorTotalMovimento = p.ValorTotalMovimento.ToString("n2")
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

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaMovimentos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Financeiro.MovimentoFinanceiroDebitoCredito repMovimentoFinanceiro = new Repositorio.Embarcador.Financeiro.MovimentoFinanceiroDebitoCredito(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaConciliacaoBancariaMovimento filtrosPesquisa = ObterFiltrosPesquisaMovimento();

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("CodigoPlano", false);
                grid.AdicionarCabecalho("MovimentoConcolidado", false);
                grid.AdicionarCabecalho("Data", "DescricaoDataMovimento", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Nº Doc.", "Documento", 9, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Nº Cheques", "Cheques", 9, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Desc. Doc.", "DescricaoTipoDocumentoMovimento", 9, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Observação", "Observacao", 12, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Débito", "ValorDebito", 9, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho("Crédito", "ValorCredito", 9, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho("Pessoa", "Pessoa", 12, Models.Grid.Align.left, true);

                Models.Grid.GridPreferencias gridPreferencias = new Models.Grid.GridPreferencias(unitOfWork, "ConciliacaoBancaria/PesquisaMovimentos", "grid-conciliacao-movimento");
                grid.AplicarPreferenciasGrid(gridPreferencias.ObterPreferenciaGrid(this.Usuario.Codigo, grid.modelo));

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;

                if (propOrdenar == "DescricaoDataMovimento")
                    propOrdenar = "DataMovimento";
                else if (propOrdenar == "DescricaoTipoDocumentoMovimento")
                    propOrdenar = "Tipo";

                IList<Dominio.ObjetosDeValor.Embarcador.Financeiro.MovimentoConciliacaoBancaria> listaMovimentos = repMovimentoFinanceiro.ConsultarMovimentoConciliacaoBancaria(filtrosPesquisa, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repMovimentoFinanceiro.ContarMovimentoConciliacaoBancaria(filtrosPesquisa));

                var lista = (from p in listaMovimentos
                             select new
                             {
                                 DT_RowId = p.Codigo.ToString(),
                                 p.Codigo,
                                 p.CodigoPlano,
                                 p.MovimentoConcolidado,
                                 p.DescricaoDataMovimento,
                                 p.Documento,
                                 p.Cheques,
                                 p.DescricaoTipoDocumentoMovimento,
                                 p.Observacao,
                                 p.ValorDebito,
                                 p.ValorCredito,
                                 p.Pessoa,
                                 //DT_RowColor = p.MovimentoConcolidado ? "#d9edf7" : string.Empty,
                                 //DT_FontColor = p.MovimentoConcolidado ? "#3a87ad" : string.Empty
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

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaExtratoBancario()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Financeiro.Conciliacao.ExtratoBancario repExtratoBancario = new Repositorio.Embarcador.Financeiro.Conciliacao.ExtratoBancario(unitOfWork);
                int codigo = int.Parse(Request.Params("Codigo"));

                DateTime dataPesquisaExtrato = Request.GetDateTimeParam("DataPesquisaExtrato");
                DateTime dataAtePesquisaExtrato = Request.GetDateTimeParam("DataAtePesquisaExtrato");
                decimal valorPesquisaExtrato = Request.GetDecimalParam("ValorPesquisaExtrato");
                decimal valorAtePesquisaExtrato = Request.GetDecimalParam("ValorAtePesquisaExtrato");
                string numeroDocumentoPesquisaExtrato = Request.GetStringParam("NumeroDocumentoPesquisaExtrato");
                string codigoLancamentoPesquisaExtrato = Request.GetStringParam("CodigoLancamentoPesquisaExtrato");
                string observacaoExtrato = Request.GetStringParam("ObservacaoExtrato");
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.DebitoCredito debitoCreditoExtrato = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.DebitoCredito>("DebitoCreditoExtrato");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("CodigoPlano", false);
                grid.AdicionarCabecalho("ExtratoConcolidado", false);
                grid.AdicionarCabecalho("Data", "DescricaoDataMovimentoExtrato", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Nº Doc.", "DocumentoExtrato", 9, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Desc. Doc.", "DescricaoTipoDocumentoMovimentoExtrato", 9, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Observação", "ObservacaoExtrato", 12, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Cod. Lançamento", "CodigoLancamentoExtrato", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Débito", "ValorDebitoExtrato", 9, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho("Crédito", "ValorCreditoExtrato", 9, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho("Saldo", "SaldoExtrato", 9, Models.Grid.Align.right, false);

                Models.Grid.GridPreferencias gridPreferencias = new Models.Grid.GridPreferencias(unitOfWork, "ConciliacaoBancaria/PesquisaExtratoBancario", "grid-conciliacao-extrato");
                grid.AplicarPreferenciasGrid(gridPreferencias.ObterPreferenciaGrid(this.Usuario.Codigo, grid.modelo));

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;

                if (propOrdenar == "DescricaoDataMovimento")
                    propOrdenar = "DataMovimento";
                else if (propOrdenar == "DescricaoTipoDocumentoMovimento")
                    propOrdenar = "Tipo";
                else if (propOrdenar == "DescricaoDataMovimentoExtrato")
                    propOrdenar = "DataMovimento";
                else if (propOrdenar.EndsWith("Extrato"))
                    propOrdenar = propOrdenar.Replace("Extrato", "");

                IList<Dominio.ObjetosDeValor.Embarcador.Financeiro.ExtratoConciliacaoBancaria> listaExtratos = repExtratoBancario.ConsultarExtratoConciliacaoBancaria(observacaoExtrato, debitoCreditoExtrato, dataAtePesquisaExtrato, valorAtePesquisaExtrato, codigoLancamentoPesquisaExtrato, dataPesquisaExtrato, valorPesquisaExtrato, numeroDocumentoPesquisaExtrato, codigo, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repExtratoBancario.contarExtratoConciliacaoBancaria(observacaoExtrato, debitoCreditoExtrato, dataAtePesquisaExtrato, valorAtePesquisaExtrato, codigoLancamentoPesquisaExtrato, dataPesquisaExtrato, valorPesquisaExtrato, numeroDocumentoPesquisaExtrato, codigo));

                var lista = (from p in listaExtratos
                             select new
                             {
                                 DT_RowId = p.Codigo.ToString(),
                                 p.Codigo,
                                 p.CodigoPlano,
                                 p.ExtratoConcolidado,
                                 DescricaoDataMovimentoExtrato = p.DescricaoDataMovimento,
                                 DocumentoExtrato = p.Documento,
                                 DescricaoTipoDocumentoMovimentoExtrato = p.DescricaoTipoDocumentoMovimento,
                                 ObservacaoExtrato = p.Observacao,
                                 CodigoLancamentoExtrato = p.CodigoLancamento,
                                 ValorDebitoExtrato = p.ValorDebito,
                                 ValorCreditoExtrato = p.ValorCredito,
                                 SaldoExtrato = p.Saldo,
                                 //DT_RowColor = p.ExtratoConcolidado ? "#d9edf7" : string.Empty,
                                 //DT_FontColor = p.ExtratoConcolidado ? "#3a87ad" : string.Empty
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

        [AllowAuthenticate]
        public async Task<IActionResult> AtualizarMovimentosSelecionados()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Financeiro.MovimentoFinanceiroDebitoCredito repMovimentoFinanceiroDebitoCredito = new Repositorio.Embarcador.Financeiro.MovimentoFinanceiroDebitoCredito(unitOfWork);
                Repositorio.Embarcador.Financeiro.Conciliacao.ConciliacaoBancaria repConciliacaoBancaria = new Repositorio.Embarcador.Financeiro.Conciliacao.ConciliacaoBancaria(unitOfWork);
                Repositorio.Embarcador.Financeiro.MovimentoFinanceiroDebitoCredito repMovimentoFinanceiro = new Repositorio.Embarcador.Financeiro.MovimentoFinanceiroDebitoCredito(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaConciliacaoBancariaMovimento filtrosPesquisa = ObterFiltrosPesquisaMovimento();

                Dominio.Entidades.Embarcador.Financeiro.Conciliacao.ConciliacaoBancaria conciliacaoBancaria = repConciliacaoBancaria.BuscarPorCodigo(filtrosPesquisa.CodigoConciliacaoBancaria, true);

                if (conciliacaoBancaria == null)
                    return new JsonpResult(false, true, "Nenhuma conciliação selecionada.");

                bool selecionarTodosMovimentos = Request.GetBoolParam("SelecionarTodosMovimentos");

                List<int> codigosMovimentos = new List<int>();
                List<int> codigosMovimentosNaoSelecionados = new List<int>();

                codigosMovimentosNaoSelecionados = JsonConvert.DeserializeObject<List<int>>(Request.Params("ListaMovimentosNaoSelecionados"));

                if (!selecionarTodosMovimentos)
                {
                    codigosMovimentos = JsonConvert.DeserializeObject<List<int>>(Request.Params("ListaMovimentos"));
                    if (codigosMovimentos.Count == 0 && codigosMovimentosNaoSelecionados.Count == 0)
                    {
                        IList<Dominio.ObjetosDeValor.Embarcador.Financeiro.MovimentoConciliacaoBancaria> listaMovimentos = repMovimentoFinanceiro.ConsultarMovimentoConciliacaoBancaria(filtrosPesquisa, "Codigo", "desc", 0, 0);
                        if (listaMovimentos != null && listaMovimentos.Count > 0)
                            codigosMovimentosNaoSelecionados = listaMovimentos.Select(o => o.Codigo).Distinct().ToList();
                    }
                }
                else
                {
                    IList<Dominio.ObjetosDeValor.Embarcador.Financeiro.MovimentoConciliacaoBancaria> listaMovimentos = repMovimentoFinanceiro.ConsultarMovimentoConciliacaoBancaria(filtrosPesquisa, "Codigo", "desc", 0, 0);
                    if (listaMovimentos != null && listaMovimentos.Count > 0)
                        codigosMovimentos = listaMovimentos.Select(o => o.Codigo).Distinct().ToList();
                }

                codigosMovimentos = codigosMovimentos.Where(c => !codigosMovimentosNaoSelecionados.Contains(c)).ToList();

                unitOfWork.Start();

                decimal valorTotalCreditoMovimento = 0, valorTotalDebitoMovimento = 0;

                if (codigosMovimentos != null && codigosMovimentos.Count > 0)
                {
                    repMovimentoFinanceiroDebitoCredito.SetarMovimentosConcolidados(codigosMovimentos, true);
                    valorTotalCreditoMovimento = repMovimentoFinanceiro.SomaDocumentos(codigosMovimentos, Dominio.ObjetosDeValor.Embarcador.Enumeradores.DebitoCredito.Debito);
                    valorTotalDebitoMovimento = repMovimentoFinanceiro.SomaDocumentos(codigosMovimentos, Dominio.ObjetosDeValor.Embarcador.Enumeradores.DebitoCredito.Credito);
                }
                if (codigosMovimentosNaoSelecionados != null && codigosMovimentosNaoSelecionados.Count > 0)
                    repMovimentoFinanceiroDebitoCredito.SetarMovimentosConcolidados(codigosMovimentosNaoSelecionados, false);

                conciliacaoBancaria.ValorTotalCreditoMovimento = valorTotalCreditoMovimento;
                conciliacaoBancaria.ValorTotalDebitoMovimento = valorTotalDebitoMovimento;
                conciliacaoBancaria.ValorTotalMovimento = (valorTotalCreditoMovimento - valorTotalDebitoMovimento);

                repConciliacaoBancaria.Atualizar(conciliacaoBancaria);

                unitOfWork.CommitChanges();
                return new JsonpResult(Servicos.Embarcador.Financeiro.ConciliacaoBancaria.ObterDetalhesConciliacaoBancaria(conciliacaoBancaria.Codigo, unitOfWork));
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao obter os detalhes dos movimentos selecionados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> AtualizarExtratosSelecionados()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Financeiro.Conciliacao.ExtratoBancario repExtratoBancario = new Repositorio.Embarcador.Financeiro.Conciliacao.ExtratoBancario(unitOfWork);
                Repositorio.Embarcador.Financeiro.Conciliacao.ConciliacaoBancaria repConciliacaoBancaria = new Repositorio.Embarcador.Financeiro.Conciliacao.ConciliacaoBancaria(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");
                Dominio.Entidades.Embarcador.Financeiro.Conciliacao.ConciliacaoBancaria conciliacaoBancaria = repConciliacaoBancaria.BuscarPorCodigo(codigo, true);

                if (conciliacaoBancaria == null)
                    return new JsonpResult(false, true, "Nenhuma conciliação selecionada.");

                DateTime dataPesquisaExtrato = Request.GetDateTimeParam("DataPesquisaExtrato");
                DateTime dataAtePesquisaExtrato = Request.GetDateTimeParam("DataAtePesquisaExtrato");
                decimal valorPesquisaExtrato = Request.GetDecimalParam("ValorPesquisaExtrato");
                decimal valorAtePesquisaExtrato = Request.GetDecimalParam("ValorAtePesquisaExtrato");
                string numeroDocumentoPesquisaExtrato = Request.GetStringParam("NumeroDocumentoPesquisaExtrato");
                string codigoLancamentoPesquisaExtrato = Request.GetStringParam("CodigoLancamentoPesquisaExtrato");
                string observacaoExtrato = Request.GetStringParam("ObservacaoExtrato");
                bool selecionarTodosExtratos = Request.GetBoolParam("SelecionarTodosExtratos");
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.DebitoCredito debitoCreditoExtrato = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.DebitoCredito>("DebitoCreditoExtrato");

                List<int> codigosExtratos = new List<int>();
                List<int> codigosExtratosNaoSelecionados = new List<int>();

                codigosExtratosNaoSelecionados = JsonConvert.DeserializeObject<List<int>>(Request.Params("ListaExtratosNaoSelecionados"));

                if (!selecionarTodosExtratos)
                {
                    codigosExtratos = JsonConvert.DeserializeObject<List<int>>(Request.Params("ListaExtratos"));
                    if (codigosExtratos.Count == 0 && codigosExtratosNaoSelecionados.Count == 0)
                    {
                        IList<Dominio.ObjetosDeValor.Embarcador.Financeiro.ExtratoConciliacaoBancaria> listaExtratos = repExtratoBancario.ConsultarExtratoConciliacaoBancaria(observacaoExtrato, debitoCreditoExtrato, dataAtePesquisaExtrato, valorAtePesquisaExtrato, codigoLancamentoPesquisaExtrato, dataPesquisaExtrato, valorPesquisaExtrato, numeroDocumentoPesquisaExtrato, codigo, "Codigo", "desc", 0, 0);
                        if (listaExtratos != null && listaExtratos.Count > 0)
                            codigosExtratosNaoSelecionados = listaExtratos.Select(o => o.Codigo).Distinct().ToList();
                    }
                }
                else
                {
                    IList<Dominio.ObjetosDeValor.Embarcador.Financeiro.ExtratoConciliacaoBancaria> listaExtratos = repExtratoBancario.ConsultarExtratoConciliacaoBancaria(observacaoExtrato, debitoCreditoExtrato, dataAtePesquisaExtrato, valorAtePesquisaExtrato, codigoLancamentoPesquisaExtrato, dataPesquisaExtrato, valorPesquisaExtrato, numeroDocumentoPesquisaExtrato, codigo, "Codigo", "desc", 0, 0);
                    if (listaExtratos != null && listaExtratos.Count > 0)
                        codigosExtratos = listaExtratos.Select(o => o.Codigo).Distinct().ToList();
                }

                codigosExtratos = codigosExtratos.Where(c => !codigosExtratosNaoSelecionados.Contains(c)).ToList();

                unitOfWork.Start();

                decimal valorTotalCreditoExtrato = 0, valorTotalDebitoExtrato = 0;

                if (codigosExtratos != null && codigosExtratos.Count > 0)
                {
                    repExtratoBancario.SetarMovimentosConcolidados(codigosExtratos, true);
                    valorTotalCreditoExtrato = repExtratoBancario.SomaDocumentos(codigosExtratos, Dominio.ObjetosDeValor.Embarcador.Enumeradores.DebitoCredito.Credito);
                    valorTotalDebitoExtrato = repExtratoBancario.SomaDocumentos(codigosExtratos, Dominio.ObjetosDeValor.Embarcador.Enumeradores.DebitoCredito.Debito);
                }
                if (codigosExtratosNaoSelecionados != null && codigosExtratosNaoSelecionados.Count > 0)
                    repExtratoBancario.SetarMovimentosConcolidados(codigosExtratosNaoSelecionados, false);

                conciliacaoBancaria.ValorTotalCreditoExtrato = valorTotalCreditoExtrato;
                conciliacaoBancaria.ValorTotalDebitoExtrato = valorTotalDebitoExtrato;
                conciliacaoBancaria.ValorTotalExtrato = valorTotalCreditoExtrato - valorTotalDebitoExtrato;

                repConciliacaoBancaria.Atualizar(conciliacaoBancaria);

                unitOfWork.CommitChanges();
                return new JsonpResult(Servicos.Embarcador.Financeiro.ConciliacaoBancaria.ObterDetalhesConciliacaoBancaria(conciliacaoBancaria.Codigo, unitOfWork));
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao obter os detalhes dos títulos selecionados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ProcessarConciliacaoBancaria()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Financeiro.Conciliacao.ConciliacaoBancaria repConciliacaoBancaria = new Repositorio.Embarcador.Financeiro.Conciliacao.ConciliacaoBancaria(unitOfWork);
                Repositorio.Embarcador.Financeiro.PlanoConta repPlanoConta = new Repositorio.Embarcador.Financeiro.PlanoConta(unitOfWork);

                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");

                Dominio.Entidades.Embarcador.Financeiro.Conciliacao.ConciliacaoBancaria conciliacaoBancaria;
                if (codigo > 0)
                    conciliacaoBancaria = repConciliacaoBancaria.BuscarPorCodigo(codigo, true);
                else
                {
                    conciliacaoBancaria = new Dominio.Entidades.Embarcador.Financeiro.Conciliacao.ConciliacaoBancaria();
                    conciliacaoBancaria.Colaborador = this.Usuario;
                    conciliacaoBancaria.Empresa = this.Usuario.Empresa;
                    conciliacaoBancaria.DataGeracaoMovimento = DateTime.Now;
                    conciliacaoBancaria.SituacaoConciliacaoBancaria = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoConciliacaoBancaria.Aberto;
                    conciliacaoBancaria.ValorTotalCreditoExtrato = 0;
                    conciliacaoBancaria.ValorTotalCreditoMovimento = 0;
                    conciliacaoBancaria.ValorTotalDebitoExtrato = 0;
                    conciliacaoBancaria.ValorTotalDebitoMovimento = 0;
                    conciliacaoBancaria.ValorTotalExtrato = 0;
                    conciliacaoBancaria.ValorTotalMovimento = 0;
                    conciliacaoBancaria.ValorTotalGeralDebitoMovimento = 0;
                    conciliacaoBancaria.ValorTotalGeralCreditoMovimento = 0;
                    conciliacaoBancaria.ValorTotalGeralMovimento = 0;
                    conciliacaoBancaria.ValorTotalGeralDebitoExtrato = 0;
                    conciliacaoBancaria.ValorTotalGeralCreditoExtrato = 0;
                    conciliacaoBancaria.ValorTotalGeralExtrato = 0;
                }

                if (conciliacaoBancaria != null && conciliacaoBancaria.Codigo > 0 && conciliacaoBancaria.SituacaoConciliacaoBancaria == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoConciliacaoBancaria.Aberto)
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, "Não é possível realizar a Conciliação Automática de uma já em andamento, favor cancele a mesma a inicie uma nova.");
                }

                conciliacaoBancaria.DataFinal = Request.GetNullableDateTimeParam("DataFinal");
                conciliacaoBancaria.DataInicial = Request.GetNullableDateTimeParam("DataInicial");
                conciliacaoBancaria.PlanoConta = repPlanoConta.BuscarPorCodigo(Request.GetIntParam("PlanoConta"));
                conciliacaoBancaria.PlanoContaSintetico = repPlanoConta.BuscarPorCodigo(Request.GetIntParam("PlanoContaSintetico"));
                conciliacaoBancaria.RealizarConciliacaoAutomatica = Request.GetBoolParam("RealizarConciliacaoAutomatica");
                conciliacaoBancaria.AnaliticoSintetico = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.AnaliticoSintetico>("AnaliticoSintetico");

                int codigoEmpresa = 0;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    codigoEmpresa = conciliacaoBancaria.Empresa.Codigo;

                if (repConciliacaoBancaria.ExisteConciliacaoAberta(conciliacaoBancaria.Codigo, conciliacaoBancaria.PlanoContaSintetico?.Codigo ?? 0, conciliacaoBancaria.PlanoConta?.Codigo ?? 0, conciliacaoBancaria.DataInicial, conciliacaoBancaria.DataFinal, codigoEmpresa))
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, "Já existe uma Conciliação Bancária aberta para a mesma conta neste período.");
                }

                if (codigo > 0)
                    repConciliacaoBancaria.Atualizar(conciliacaoBancaria, Auditado);
                else
                    repConciliacaoBancaria.Inserir(conciliacaoBancaria, Auditado);

                Servicos.Embarcador.Financeiro.ConciliacaoBancaria.ProcessarConciliacaoBancaria(conciliacaoBancaria.Codigo, conciliacaoBancaria.PlanoContaSintetico?.Plano ?? "", conciliacaoBancaria.PlanoConta?.Codigo ?? 0, conciliacaoBancaria.DataInicial, conciliacaoBancaria.DataFinal, codigoEmpresa, conciliacaoBancaria.RealizarConciliacaoAutomatica, unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(Servicos.Embarcador.Financeiro.ConciliacaoBancaria.ObterDetalhesConciliacaoBancaria(conciliacaoBancaria.Codigo, unitOfWork));
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao abrir a conciliação bancária.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> FecharConciliacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Financeiro.Conciliacao.ConciliacaoBancaria repConciliacaoBancaria = new Repositorio.Embarcador.Financeiro.Conciliacao.ConciliacaoBancaria(unitOfWork);

                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");

                Dominio.Entidades.Embarcador.Financeiro.Conciliacao.ConciliacaoBancaria conciliacaoBancaria = repConciliacaoBancaria.BuscarPorCodigo(codigo, true);
                if (conciliacaoBancaria == null)
                    return new JsonpResult(false, true, "Nenhuma conciliação selecionada.");
                if (conciliacaoBancaria.SituacaoConciliacaoBancaria != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoConciliacaoBancaria.Aberto)
                    return new JsonpResult(false, true, "A conciliação selecionada não se encontra em aberto.");
                if (conciliacaoBancaria.ValorTotalExtrato != conciliacaoBancaria.ValorTotalMovimento)
                    return new JsonpResult(false, true, "Os valores totais da Conciliação não estão corretos.");

                if (conciliacaoBancaria.Movimentos != null)
                    repConciliacaoBancaria.DeletarMovimentoBancarioNaoConsolidado(conciliacaoBancaria.Codigo);

                if (conciliacaoBancaria.Extratos != null)
                    repConciliacaoBancaria.DeletarExtratoBancarioNaoConsolidado(conciliacaoBancaria.Codigo);

                conciliacaoBancaria.SituacaoConciliacaoBancaria = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoConciliacaoBancaria.Finalizado;
                repConciliacaoBancaria.Atualizar(conciliacaoBancaria, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(new
                {
                    Codigo = conciliacaoBancaria.Codigo,
                    AnaliticoSintetico = conciliacaoBancaria.AnaliticoSintetico,
                    PlanoConta = conciliacaoBancaria.PlanoConta?.Descricao ?? "",
                    CodigoPlanoConta = conciliacaoBancaria.PlanoConta?.Codigo ?? 0,
                    PlanoContaSintetico = conciliacaoBancaria.PlanoContaSintetico?.Descricao ?? "",
                    CodigoPlanoContaSintetico = conciliacaoBancaria.PlanoContaSintetico?.Codigo ?? 0,
                    DataInicial = conciliacaoBancaria.DataInicial.HasValue ? conciliacaoBancaria.DataInicial.Value.ToString("dd/MM/yyyy") : "",
                    DataFinal = conciliacaoBancaria.DataFinal.HasValue ? conciliacaoBancaria.DataFinal.Value.ToString("dd/MM/yyyy") : ""
                });
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao fechar a conciliação bancária.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ReAbrirConciliacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Financeiro.Conciliacao.ConciliacaoBancaria repConciliacaoBancaria = new Repositorio.Embarcador.Financeiro.Conciliacao.ConciliacaoBancaria(unitOfWork);

                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");

                Dominio.Entidades.Embarcador.Financeiro.Conciliacao.ConciliacaoBancaria conciliacaoBancaria = repConciliacaoBancaria.BuscarPorCodigo(codigo, true);
                if (conciliacaoBancaria == null)
                    return new JsonpResult(false, true, "Nenhuma conciliação selecionada.");

                int codigoEmpresa = 0;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    codigoEmpresa = conciliacaoBancaria.Empresa.Codigo;

                if (repConciliacaoBancaria.ExisteConciliacaoAberta(conciliacaoBancaria.Codigo, conciliacaoBancaria.PlanoContaSintetico?.Codigo ?? 0, conciliacaoBancaria.PlanoConta?.Codigo ?? 0, conciliacaoBancaria.DataInicial, conciliacaoBancaria.DataFinal, codigoEmpresa))
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, "Já existe uma Conciliação Bancária aberta para a mesma conta neste período, favor cancele a anterior antes re reabrir esta.");
                }

                conciliacaoBancaria.SituacaoConciliacaoBancaria = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoConciliacaoBancaria.Aberto;
                repConciliacaoBancaria.Atualizar(conciliacaoBancaria, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(Servicos.Embarcador.Financeiro.ConciliacaoBancaria.ObterDetalhesConciliacaoBancaria(conciliacaoBancaria.Codigo, unitOfWork));
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao re-abrir a conciliação bancária.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> EstornarConciliacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Financeiro.Conciliacao.ConciliacaoBancaria repConciliacaoBancaria = new Repositorio.Embarcador.Financeiro.Conciliacao.ConciliacaoBancaria(unitOfWork);
                Repositorio.Embarcador.Financeiro.Conciliacao.ExtratoBancario repExtratoBancario = new Repositorio.Embarcador.Financeiro.Conciliacao.ExtratoBancario(unitOfWork);
                Repositorio.Embarcador.Financeiro.MovimentoFinanceiroDebitoCredito repMovimentoFinanceiroDebitoCredito = new Repositorio.Embarcador.Financeiro.MovimentoFinanceiroDebitoCredito(unitOfWork);

                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");
                Dominio.Entidades.Embarcador.Financeiro.Conciliacao.ConciliacaoBancaria conciliacaoBancaria = repConciliacaoBancaria.BuscarPorCodigo(codigo, true);
                if (conciliacaoBancaria == null)
                    return new JsonpResult(false, true, "Nenhuma conciliação selecionada.");

                conciliacaoBancaria.SituacaoConciliacaoBancaria = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoConciliacaoBancaria.Cancelado;
                repConciliacaoBancaria.Atualizar(conciliacaoBancaria, Auditado);

                if (conciliacaoBancaria.Movimentos != null)
                {
                    foreach (var movimento in conciliacaoBancaria.Movimentos)
                    {
                        movimento.MovimentoConcolidado = false;
                        repMovimentoFinanceiroDebitoCredito.Atualizar(movimento);
                    }
                }
                if (conciliacaoBancaria.Extratos != null)
                {
                    foreach (var extrato in conciliacaoBancaria.Extratos)
                    {
                        extrato.ExtratoConcolidado = false;
                        repExtratoBancario.Atualizar(extrato);
                    }
                }

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao estornar a conciliação bancária.");
            }
            finally
            {
                unitOfWork.Dispose();
            }

        }

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Financeiro.Conciliacao.ConciliacaoBancaria repConciliacaoBancaria = new Repositorio.Embarcador.Financeiro.Conciliacao.ConciliacaoBancaria(unitOfWork);
                Dominio.Entidades.Embarcador.Financeiro.Conciliacao.ConciliacaoBancaria conciliacaoBancaria = repConciliacaoBancaria.BuscarPorCodigo(codigo);

                if (conciliacaoBancaria == null)
                    return new JsonpResult(false, true, "Conciliação não localizada.");

                return new JsonpResult(Servicos.Embarcador.Financeiro.ConciliacaoBancaria.ObterDetalhesConciliacaoBancaria(conciliacaoBancaria.Codigo, unitOfWork));
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar por código.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AdicionarMovimentoFinanceiro()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Financeiro.MovimentoFinanceiro repMovimentoFinanceiro = new Repositorio.Embarcador.Financeiro.MovimentoFinanceiro(unitOfWork);
                Repositorio.Embarcador.Financeiro.MovimentoFinanceiroDebitoCredito repMovimentoFinanceiroDebitoCredito = new Repositorio.Embarcador.Financeiro.MovimentoFinanceiroDebitoCredito(unitOfWork);
                Repositorio.Embarcador.Financeiro.TipoMovimento repTipoMovimento = new Repositorio.Embarcador.Financeiro.TipoMovimento(unitOfWork);
                Repositorio.Embarcador.Financeiro.PlanoConta repPlanoConta = new Repositorio.Embarcador.Financeiro.PlanoConta(unitOfWork);
                Repositorio.Embarcador.Financeiro.CentroResultado repCentroResultado = new Repositorio.Embarcador.Financeiro.CentroResultado(unitOfWork);
                Repositorio.Embarcador.Financeiro.Conciliacao.ConciliacaoBancaria repConciliacaoBancaria = new Repositorio.Embarcador.Financeiro.Conciliacao.ConciliacaoBancaria(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");

                Dominio.Entidades.Embarcador.Financeiro.Conciliacao.ConciliacaoBancaria conciliacaoBancaria = repConciliacaoBancaria.BuscarPorCodigo(codigo, true);

                if (conciliacaoBancaria == null)
                    return new JsonpResult(false, true, "Nenhuma conciliação selecionada.");

                unitOfWork.Start();

                int centroCusto, planoDebito, planoCredito, tipoMovimento = 0;
                int.TryParse(Request.Params("CentroResultado"), out centroCusto);
                int.TryParse(Request.Params("PlanoDebito"), out planoDebito);
                int.TryParse(Request.Params("PlanoCredito"), out planoCredito);
                int.TryParse(Request.Params("TipoMovimento"), out tipoMovimento);

                DateTime dataMovimento, dataGeracaoMovimento = DateTime.Now;
                DateTime.TryParse(Request.Params("DataMovimento"), out dataMovimento);

                DateTime dataBase;
                DateTime.TryParse(Request.Params("DataBase"), out dataBase);
                int codigoGrupoPessoa = 0;
                int.TryParse(Request.Params("GrupoPessoa"), out codigoGrupoPessoa);
                double cnpjPessoa = 0;
                double.TryParse(Request.Params("Pessoa"), out cnpjPessoa);

                decimal valorMovimento = 0;
                decimal.TryParse(Request.Params("ValorMovimento"), out valorMovimento);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento tipo;
                Enum.TryParse(Request.Params("TipoDocumento"), out tipo);

                if (Servicos.Embarcador.Financeiro.FechamentoDiario.VerificarSeExisteFechamento(0, dataMovimento, unitOfWork, tipo))
                    return new JsonpResult(false, true, "Já existe um fechamento diário igual ou posterior à data de " + dataMovimento.ToString("dd/MM/yyyy") + ", não sendo possível adicionar o movimento financeiro.");

                if (dataMovimento.Date > DateTime.Now.Date)
                    return new JsonpResult(false, true, "Atenção! A data informada não pode ser maior que a data atual.");

                Dominio.Entidades.Embarcador.Financeiro.MovimentoFinanceiro movimentoFinanceiro = new Dominio.Entidades.Embarcador.Financeiro.MovimentoFinanceiro();

                movimentoFinanceiro.DataGeracaoMovimento = dataGeracaoMovimento;
                movimentoFinanceiro.DataMovimento = dataMovimento;
                movimentoFinanceiro.Valor = valorMovimento;
                movimentoFinanceiro.TipoDocumentoMovimento = tipo;
                movimentoFinanceiro.Documento = Request.Params("NumeroDocumento");
                movimentoFinanceiro.Observacao = Request.Params("Observacao");
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    movimentoFinanceiro.Empresa = Usuario.Empresa;
                movimentoFinanceiro.TipoGeracaoMovimento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoGeracaoMovimento.Manual;
                movimentoFinanceiro.DataBase = dataBase;
                movimentoFinanceiro.GrupoPessoas = null;
                movimentoFinanceiro.Pessoa = null;

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    movimentoFinanceiro.TipoAmbiente = this.Usuario.Empresa.TipoAmbiente;

                if (planoDebito > 0)
                    movimentoFinanceiro.PlanoDeContaDebito = repPlanoConta.BuscarPorCodigo(planoDebito);

                if (planoCredito > 0)
                    movimentoFinanceiro.PlanoDeContaCredito = repPlanoConta.BuscarPorCodigo(planoCredito);

                if (centroCusto > 0)
                    movimentoFinanceiro.CentroResultado = repCentroResultado.BuscarPorCodigo(centroCusto);

                if (tipoMovimento > 0)
                {
                    movimentoFinanceiro.TipoMovimento = repTipoMovimento.BuscarPorCodigo(tipoMovimento);

                    if (!string.IsNullOrWhiteSpace(movimentoFinanceiro.TipoMovimento.Observacao))
                        movimentoFinanceiro.Observacao = movimentoFinanceiro.TipoMovimento.Observacao + " - " + movimentoFinanceiro.Observacao;
                }

                if (planoDebito == planoCredito)
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, "Atenção! Não é permitido cadastrar com a mesma conta em entrada e saída.");
                }

                if (movimentoFinanceiro.PlanoDeContaCredito.AnaliticoSintetico == Dominio.ObjetosDeValor.Embarcador.Enumeradores.AnaliticoSintetico.Sintetico)
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, "Atenção! Não é permitido realizar movimentações para uma conta sintética");
                }

                if (movimentoFinanceiro.PlanoDeContaDebito.AnaliticoSintetico == Dominio.ObjetosDeValor.Embarcador.Enumeradores.AnaliticoSintetico.Sintetico)
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, "Atenção! Não é permitido realizar movimentações para uma conta sintética");
                }

                repMovimentoFinanceiro.Inserir(movimentoFinanceiro, Auditado);

                Dominio.Entidades.Embarcador.Financeiro.MovimentoFinanceiroDebitoCredito movimentoFinanceiroDebito = new Dominio.Entidades.Embarcador.Financeiro.MovimentoFinanceiroDebitoCredito();
                movimentoFinanceiroDebito.DataGeracaoMovimento = dataGeracaoMovimento;
                movimentoFinanceiroDebito.DataMovimento = dataMovimento;
                movimentoFinanceiroDebito.DebitoCredito = Dominio.ObjetosDeValor.Embarcador.Enumeradores.DebitoCredito.Debito;
                movimentoFinanceiroDebito.MovimentoFinanceiro = movimentoFinanceiro;
                movimentoFinanceiroDebito.PlanoDeConta = repPlanoConta.BuscarPorCodigo(planoDebito);
                movimentoFinanceiroDebito.Valor = valorMovimento;

                repMovimentoFinanceiroDebitoCredito.Inserir(movimentoFinanceiroDebito, Auditado);

                Dominio.Entidades.Embarcador.Financeiro.MovimentoFinanceiroDebitoCredito movimentoFinanceiroCredito = new Dominio.Entidades.Embarcador.Financeiro.MovimentoFinanceiroDebitoCredito();
                movimentoFinanceiroCredito.DataGeracaoMovimento = dataGeracaoMovimento;
                movimentoFinanceiroCredito.DataMovimento = dataMovimento;
                movimentoFinanceiroCredito.DebitoCredito = Dominio.ObjetosDeValor.Embarcador.Enumeradores.DebitoCredito.Credito;
                movimentoFinanceiroCredito.MovimentoFinanceiro = movimentoFinanceiro;
                movimentoFinanceiroCredito.PlanoDeConta = repPlanoConta.BuscarPorCodigo(planoCredito);
                movimentoFinanceiroCredito.Valor = valorMovimento;

                repMovimentoFinanceiroDebitoCredito.Inserir(movimentoFinanceiroCredito, Auditado);

                int codigoEmpresa = 0;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    codigoEmpresa = conciliacaoBancaria.Empresa.Codigo;

                Servicos.Embarcador.Financeiro.ConciliacaoBancaria.ProcessarConciliacaoBancaria(conciliacaoBancaria.Codigo, conciliacaoBancaria.PlanoContaSintetico?.Plano ?? "", conciliacaoBancaria.PlanoConta?.Codigo ?? 0, conciliacaoBancaria.DataInicial, conciliacaoBancaria.DataFinal, codigoEmpresa, conciliacaoBancaria.RealizarConciliacaoAutomatica, unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(Servicos.Embarcador.Financeiro.ConciliacaoBancaria.ObterDetalhesConciliacaoBancaria(conciliacaoBancaria.Codigo, unitOfWork));
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao inserir movimentação.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AdicionarExtratoBancario()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Financeiro.Conciliacao.ConciliacaoBancaria repConciliacaoBancaria = new Repositorio.Embarcador.Financeiro.Conciliacao.ConciliacaoBancaria(unitOfWork);
                Dominio.Entidades.Embarcador.Financeiro.Conciliacao.ConciliacaoBancaria conciliacaoBancaria = repConciliacaoBancaria.BuscarPorCodigo(codigo, true);

                if (conciliacaoBancaria == null)
                    return new JsonpResult(false, true, "Nenhuma conciliação selecionada.");

                int planoConta = 0, empresa = 0, tipoLancamento = 0;
                int.TryParse(Request.Params("ExtratoBancarioTipoLancamento"), out tipoLancamento);
                int.TryParse(Request.Params("PlanoConta"), out planoConta);
                int.TryParse(Request.Params("Empresa"), out empresa);

                DateTime dataMovimento, dataGeracaoMovimento = DateTime.Now;
                DateTime.TryParse(Request.Params("DataMovimento"), out dataMovimento);

                decimal valorMovimento = 0;
                decimal.TryParse(Request.Params("Valor"), out valorMovimento);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento tipo;
                Enum.TryParse(Request.Params("TipoDocumentoMovimento"), out tipo);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.DebitoCredito debitoCredito;
                Enum.TryParse(Request.Params("DebitoCredito"), out debitoCredito);

                Repositorio.Embarcador.Financeiro.Conciliacao.ExtratoBancario repExtratoBancario = new Repositorio.Embarcador.Financeiro.Conciliacao.ExtratoBancario(unitOfWork);
                Repositorio.Embarcador.Financeiro.PlanoConta repPlanoConta = new Repositorio.Embarcador.Financeiro.PlanoConta(unitOfWork);
                Repositorio.Embarcador.Financeiro.Conciliacao.ExtratoBancarioTipoLancamento repExtratoBancarioTipoLancamento = new Repositorio.Embarcador.Financeiro.Conciliacao.ExtratoBancarioTipoLancamento(unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

                unitOfWork.Start();

                Dominio.Entidades.Embarcador.Financeiro.Conciliacao.ExtratoBancario extratoBancario = new Dominio.Entidades.Embarcador.Financeiro.Conciliacao.ExtratoBancario();

                extratoBancario.DataGeracaoMovimento = dataGeracaoMovimento;
                extratoBancario.DataMovimento = dataMovimento;
                extratoBancario.Valor = valorMovimento;
                extratoBancario.TipoDocumentoMovimento = tipo;
                extratoBancario.Documento = Request.Params("Documento");
                extratoBancario.Observacao = Request.Params("Observacao");
                extratoBancario.CodigoLancamento = Request.Params("CodigoLancamento");
                extratoBancario.DebitoCredito = debitoCredito;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    extratoBancario.Empresa = Usuario.Empresa;
                else
                    extratoBancario.Empresa = repEmpresa.BuscarPorCodigo(empresa);
                extratoBancario.TipoGeracaoMovimento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoGeracaoMovimento.Manual;
                extratoBancario.ExtratoConcolidado = false;
                extratoBancario.Colaborador = this.Usuario;
                if (planoConta > 0)
                    extratoBancario.PlanoConta = repPlanoConta.BuscarPorCodigo(planoConta);
                if (tipoLancamento > 0)
                    extratoBancario.ExtratoBancarioTipoLancamento = repExtratoBancarioTipoLancamento.BuscarPorCodigo(tipoLancamento);

                if (extratoBancario.PlanoConta.AnaliticoSintetico == Dominio.ObjetosDeValor.Embarcador.Enumeradores.AnaliticoSintetico.Sintetico)
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, "Atenção! Não é permitido realizar movimentações para uma conta sintética");
                }

                repExtratoBancario.Inserir(extratoBancario, Auditado);

                int codigoEmpresa = 0;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    codigoEmpresa = conciliacaoBancaria.Empresa.Codigo;

                Servicos.Embarcador.Financeiro.ConciliacaoBancaria.ProcessarConciliacaoBancaria(conciliacaoBancaria.Codigo, conciliacaoBancaria.PlanoContaSintetico?.Plano ?? "", conciliacaoBancaria.PlanoConta?.Codigo ?? 0, conciliacaoBancaria.DataInicial, conciliacaoBancaria.DataFinal, codigoEmpresa, conciliacaoBancaria.RealizarConciliacaoAutomatica, unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(Servicos.Embarcador.Financeiro.ConciliacaoBancaria.ObterDetalhesConciliacaoBancaria(conciliacaoBancaria.Codigo, unitOfWork));
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao inserir extrato manual.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> ImportarExtrato()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Financeiro.Conciliacao.ConciliacaoBancaria repConciliacaoBancaria = new Repositorio.Embarcador.Financeiro.Conciliacao.ConciliacaoBancaria(unidadeDeTrabalho);
                Repositorio.Embarcador.Financeiro.PlanoConta repPlanoConta = new Repositorio.Embarcador.Financeiro.PlanoConta(unidadeDeTrabalho);

                Servicos.DTO.CustomFile file = HttpContext.GetFile();
                int codigoPlanoConta = int.Parse(Request.Params("PlanoConta"));
                int codigoPlanoContaSintetico = int.Parse(Request.Params("PlanoContaSintetico"));
                int codigo = int.Parse(Request.Params("Codigo"));

                Dominio.Entidades.Embarcador.Financeiro.PlanoConta planoConta = repPlanoConta.BuscarPorCodigo(codigoPlanoConta);
                Dominio.Entidades.Embarcador.Financeiro.Conciliacao.ConciliacaoBancaria conciliacaoBancaria = repConciliacaoBancaria.BuscarPorCodigo(codigo, true);

                if (planoConta == null && codigoPlanoContaSintetico > 0)
                {
                    planoConta = repPlanoConta.BuscarPorCodigo(codigoPlanoConta);
                    planoConta = repPlanoConta.BuscarPrimeiroAnalitico(planoConta.Plano);
                }

                if (planoConta == null)
                    return new JsonpResult(false, true, "Nenhum plano de contas selecionado.");
                if (conciliacaoBancaria == null)
                    return new JsonpResult(false, true, "Nenhuma conciliação selecionada.");

                unidadeDeTrabalho.Start();

                StreamReader streamReader = new StreamReader(file.InputStream);
                decimal saldoInicial = 0m;
                decimal saldoFinal = 0m;
                Servicos.Embarcador.Financeiro.ConciliacaoBancaria.ImportarExtratoBancario(this.Usuario, this.Usuario.Empresa, planoConta.Codigo, streamReader, unidadeDeTrabalho, Auditado, false, out saldoInicial, out saldoFinal);

                int codigoEmpresa = 0;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    codigoEmpresa = conciliacaoBancaria.Empresa.Codigo;

                Servicos.Embarcador.Financeiro.ConciliacaoBancaria.ProcessarConciliacaoBancaria(conciliacaoBancaria.Codigo, conciliacaoBancaria.PlanoContaSintetico?.Plano ?? "", conciliacaoBancaria.PlanoConta?.Codigo ?? 0, conciliacaoBancaria.DataInicial, conciliacaoBancaria.DataFinal, codigoEmpresa, conciliacaoBancaria.RealizarConciliacaoAutomatica, unidadeDeTrabalho);

                unidadeDeTrabalho.CommitChanges();
                return new JsonpResult(Servicos.Embarcador.Financeiro.ConciliacaoBancaria.ObterDetalhesConciliacaoBancaria(conciliacaoBancaria.Codigo, unidadeDeTrabalho));
            }
            catch (ServicoException se)
            {
                unidadeDeTrabalho.Rollback();
                return new JsonpResult(false, true, se.Message);
            }
            catch (Exception ex)
            {
                unidadeDeTrabalho.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao importar o extrato bancário.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BaixarRelatorio()
        {
            try
            {
                var pdf = ReportRequest.WithType(ReportType.ConciliacaoBancaria)
                    .WithExecutionType(ExecutionType.Sync)
                    .AddExtraData("CodigoConciliacaoBancaria", Request.Params("Codigo"))
                    .CallReport()
                    .GetContentFile();
                
                return Arquivo(pdf, "application/pdf", "ConciliacaoBancaria.pdf");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao gerar o arquivo.");
            }
        }

        public async Task<IActionResult> BuscarMovimentosSelecionados()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Financeiro.Conciliacao.ConciliacaoBancaria repConciliacaoBancaria = new Repositorio.Embarcador.Financeiro.Conciliacao.ConciliacaoBancaria(unitOfWork);
                Repositorio.Embarcador.Financeiro.MovimentoFinanceiroDebitoCredito repMovimentoFinanceiro = new Repositorio.Embarcador.Financeiro.MovimentoFinanceiroDebitoCredito(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");

                Dominio.Entidades.Embarcador.Financeiro.Conciliacao.ConciliacaoBancaria conciliacaoBancaria = repConciliacaoBancaria.BuscarPorCodigo(codigo, true);
                if (conciliacaoBancaria == null)
                    return new JsonpResult(false, true, "Nenhuma conciliação selecionada.");

                IList<Dominio.ObjetosDeValor.Embarcador.Financeiro.MovimentoConciliacaoBancaria> listaMovimentosConcolidados = repMovimentoFinanceiro.ConsultarMovimentoConcolidados(codigo);

                var retorno = new
                {
                    ValorTotalCreditoExtrato = conciliacaoBancaria.ValorTotalCreditoExtrato.ToString("n2"),
                    ValorTotalCreditoMovimento = conciliacaoBancaria.ValorTotalCreditoMovimento.ToString("n2"),
                    ValorTotalDebitoExtrato = conciliacaoBancaria.ValorTotalDebitoExtrato.ToString("n2"),
                    ValorTotalDebitoMovimento = conciliacaoBancaria.ValorTotalDebitoMovimento.ToString("n2"),
                    ValorTotalExtrato = conciliacaoBancaria.ValorTotalExtrato.ToString("n2"),
                    ValorTotalMovimento = conciliacaoBancaria.ValorTotalMovimento.ToString("n2"),
                    MovimentosConcolidados = listaMovimentosConcolidados != null ? (
                        from p in listaMovimentosConcolidados
                        select new
                        {
                            DT_RowId = p.Codigo.ToString(),
                            p.Codigo,
                            p.CodigoPlano,
                            p.MovimentoConcolidado,
                            p.DescricaoDataMovimento,
                            p.Documento,
                            p.DescricaoTipoDocumentoMovimento,
                            p.Observacao,
                            p.ValorDebito,
                            p.ValorCredito,
                            p.Pessoa
                        }
                    ).ToList() : null,
                    ValorTotalGeralDebitoMovimento = conciliacaoBancaria.ValorTotalGeralDebitoMovimento.ToString("n2"),
                    ValorTotalGeralCreditoMovimento = conciliacaoBancaria.ValorTotalGeralCreditoMovimento.ToString("n2"),
                    ValorTotalGeralMovimento = conciliacaoBancaria.ValorTotalGeralMovimento.ToString("n2"),
                    ValorTotalGeralDebitoExtrato = conciliacaoBancaria.ValorTotalGeralDebitoExtrato.ToString("n2"),
                    ValorTotalGeralCreditoExtrato = conciliacaoBancaria.ValorTotalGeralCreditoExtrato.ToString("n2"),
                    ValorTotalGeralExtrato = conciliacaoBancaria.ValorTotalGeralExtrato.ToString("n2")
                };

                return new JsonpResult(retorno, true, "Sucesso");
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar os movimentos selecionados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarExtratosSelecionados()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Financeiro.Conciliacao.ConciliacaoBancaria repConciliacaoBancaria = new Repositorio.Embarcador.Financeiro.Conciliacao.ConciliacaoBancaria(unitOfWork);
                Repositorio.Embarcador.Financeiro.Conciliacao.ExtratoBancario repExtratoBancario = new Repositorio.Embarcador.Financeiro.Conciliacao.ExtratoBancario(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");

                Dominio.Entidades.Embarcador.Financeiro.Conciliacao.ConciliacaoBancaria conciliacaoBancaria = repConciliacaoBancaria.BuscarPorCodigo(codigo, true);
                if (conciliacaoBancaria == null)
                    return new JsonpResult(false, true, "Nenhuma conciliação selecionada.");

                IList<Dominio.ObjetosDeValor.Embarcador.Financeiro.ExtratoConciliacaoBancaria> listaExtratosConcolidados = repExtratoBancario.ConsultarExtratoConcolidados(codigo);

                var retorno = new
                {
                    ValorTotalCreditoExtrato = conciliacaoBancaria.ValorTotalCreditoExtrato.ToString("n2"),
                    ValorTotalCreditoMovimento = conciliacaoBancaria.ValorTotalCreditoMovimento.ToString("n2"),
                    ValorTotalDebitoExtrato = conciliacaoBancaria.ValorTotalDebitoExtrato.ToString("n2"),
                    ValorTotalDebitoMovimento = conciliacaoBancaria.ValorTotalDebitoMovimento.ToString("n2"),
                    ValorTotalExtrato = conciliacaoBancaria.ValorTotalExtrato.ToString("n2"),
                    ValorTotalMovimento = conciliacaoBancaria.ValorTotalMovimento.ToString("n2"),
                    ExtratosConcolidados = listaExtratosConcolidados != null ? (
                        from p in listaExtratosConcolidados
                        select new
                        {
                            DT_RowId = p.Codigo.ToString(),
                            p.Codigo,
                            p.CodigoPlano,
                            p.ExtratoConcolidado,
                            p.DescricaoDataMovimento,
                            p.Documento,
                            p.DescricaoTipoDocumentoMovimento,
                            p.Observacao,
                            p.CodigoLancamento,
                            p.ValorDebito,
                            p.ValorCredito,
                            p.Saldo
                        }
                    ).ToList() : null,
                    ValorTotalGeralDebitoMovimento = conciliacaoBancaria.ValorTotalGeralDebitoMovimento.ToString("n2"),
                    ValorTotalGeralCreditoMovimento = conciliacaoBancaria.ValorTotalGeralCreditoMovimento.ToString("n2"),
                    ValorTotalGeralMovimento = conciliacaoBancaria.ValorTotalGeralMovimento.ToString("n2"),
                    ValorTotalGeralDebitoExtrato = conciliacaoBancaria.ValorTotalGeralDebitoExtrato.ToString("n2"),
                    ValorTotalGeralCreditoExtrato = conciliacaoBancaria.ValorTotalGeralCreditoExtrato.ToString("n2"),
                    ValorTotalGeralExtrato = conciliacaoBancaria.ValorTotalGeralExtrato.ToString("n2")
                };

                return new JsonpResult(retorno, true, "Sucesso");
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar os extratos selecionados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaConciliacaoBancaria ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaConciliacaoBancaria()
            {
                DataInicial = Request.GetDateTimeParam("DataInicial"),
                DataFinal = Request.GetDateTimeParam("DataFinal"),
                CodigoPlanoConta = Request.GetIntParam("PlanoConta"),
                SituacaoConciliacaoBancaria = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoConciliacaoBancaria>("SituacaoConciliacaoBancaria"),
                CodigoEmpresa = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe ? this.Usuario.Empresa.Codigo : 0,
                ValorExtratoInicial = Request.GetDecimalParam("ValorExtratoInicial"),
                ValorExtratoFinal = Request.GetDecimalParam("ValorExtratoFinal"),
                ValorMovimentoInicial = Request.GetDecimalParam("ValorMovimentoInicial"),
                ValorMovimentoFinal = Request.GetDecimalParam("ValorMovimentoFinal"),
                DataGeracaoMovimentoFinanceiro = Request.GetDateTimeParam("DataGeracaoMovimentoFinanceiro"),
                NumeroDocumentoMovimentoFinanceiro = Request.GetStringParam("NumeroDocumentoMovimentoFinanceiro"),
                ValorMovimentoFinanceiroInicial = Request.GetDecimalParam("ValorMovimentoFinanceiroInicial"),
                ValorMovimentoFinanceiroFinal = Request.GetDecimalParam("ValorMovimentoFinanceiroFinal"),
                CodigoOperador = Request.GetIntParam("Operador"),
                CodigoTitulo = Request.GetIntParam("CodigoTitulo")
            };
        }

        private Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaConciliacaoBancariaMovimento ObterFiltrosPesquisaMovimento()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaConciliacaoBancariaMovimento()
            {
                CodigoConciliacaoBancaria = Request.GetIntParam("Codigo"),
                DataPesquisaMovimento = Request.GetDateTimeParam("DataPesquisaMovimento"),
                DataAtePesquisaMovimento = Request.GetDateTimeParam("DataAtePesquisaMovimento"),
                ValorPesquisaMovimento = Request.GetDecimalParam("ValorPesquisaMovimento"),
                ValorAtePesquisaMovimento = Request.GetDecimalParam("ValorAtePesquisaMovimento"),
                NumeroDocumentoPesquisaMovimento = Request.GetStringParam("NumeroDocumentoPesquisaMovimento"),
                NumeroChequePesquisaMovimento = Request.GetStringParam("NumeroChequePesquisaMovimento"),
                CnpjPessoa = Request.GetDoubleParam("PessoaPesquisaMovimento"),
                CodigoGrupoPessoa = Request.GetIntParam("GrupoPessoaPesquisaMovimento"),
                DebitoCreditoMovimento = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.DebitoCredito>("DebitoCreditoMovimento"),
                ObservacaoMovimento = Request.GetStringParam("ObservacaoMovimento"),
                CodigoTitulo = Request.GetIntParam("CodigoTituloMovimento")
            };
        }

        #endregion
    }
}
