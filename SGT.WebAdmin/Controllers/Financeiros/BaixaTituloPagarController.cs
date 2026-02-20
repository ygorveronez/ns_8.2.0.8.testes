using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Newtonsoft.Json;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Financeiros
{
    [CustomAuthorize("Financeiros/BaixaTituloPagar")]
    public class BaixaTituloPagarController : BaseController
    {
		#region Construtores

		public BaixaTituloPagarController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoOperador, numeroTitulo;
                int.TryParse(Request.Params("Operador"), out codigoOperador);
                int.TryParse(Request.Params("NumeroTitulo"), out numeroTitulo);
                int.TryParse(Request.Params("GrupoPessoa"), out int codigoGrupoPessoa);

                double pessoa;
                double.TryParse(Request.Params("Pessoa"), out pessoa);

                DateTime dataInicial, dataFinal;
                DateTime.TryParse(Request.Params("DataInicial"), out dataInicial);
                DateTime.TryParse(Request.Params("DataFinal"), out dataFinal);

                SituacaoBaixaTitulo etapaBaixa;
                Enum.TryParse(Request.Params("Situacao"), out etapaBaixa);

                TipoBaixa tipoBaixa = TipoBaixa.Todos;
                Enum.TryParse(Request.Params("TipoBaixa"), out tipoBaixa);

                DateTime dataEmissaoInicial, dataEmissaoFinal;
                DateTime.TryParse(Request.Params("DataEmissaoInicial"), out dataEmissaoInicial);
                DateTime.TryParse(Request.Params("DataEmissaoFinal"), out dataEmissaoFinal);

                DateTime dataVencimentoInicial, dataVencimentoFinal;
                DateTime.TryParse(Request.Params("DataInicialVencimento"), out dataVencimentoInicial);
                DateTime.TryParse(Request.Params("DataFinalVencimento"), out dataVencimentoFinal);

                decimal valorInicial, valorFinal;
                decimal.TryParse(Request.Params("ValorInicial"), out valorInicial);
                decimal.TryParse(Request.Params("ValorFinal"), out valorFinal);

                int codigoEmpresa = 0;
                Dominio.Enumeradores.TipoAmbiente tipoAmbiente = 0;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                {
                    codigoEmpresa = this.Usuario.Empresa.Codigo;
                    tipoAmbiente = this.Usuario.Empresa.TipoAmbiente;
                }

                string numeroDocumentoOriginario = Request.Params("NumeroDocumentoOriginario");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Títulos", "CodigosTitulos", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Fornecedor(es)", "Fornecedores", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Situação", "DescricaoSituacaoBaixaTitulo", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Data da Baixa", "DataBaixa", 8, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Valor Pago", "Valor", 8, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho("Valor Pendente", "ValorPendente", 8, Models.Grid.Align.right, false);
                grid.AdicionarCabecalho("Total Acréscimo", "ValorTotalAcrescimo", 8, Models.Grid.Align.right, false);
                grid.AdicionarCabecalho("Total Desconto", "ValorTotalDesconto", 8, Models.Grid.Align.right, false);
                grid.AdicionarCabecalho("Saldo", "ValorTotalSaldo", 6, Models.Grid.Align.right, false);

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                if (propOrdenar == "DescricaoSituacaoBaixaTitulo")
                    propOrdenar = "SituacaoBaixaTitulo";

                Repositorio.Embarcador.Financeiro.TituloBaixa repTituloBaixa = new Repositorio.Embarcador.Financeiro.TituloBaixa(unitOfWork);
                List<Dominio.Entidades.Embarcador.Financeiro.TituloBaixa> listaTitulo = repTituloBaixa.ConsultaBaixaPagar(tipoBaixa, dataEmissaoInicial, dataEmissaoFinal, dataVencimentoInicial, dataVencimentoFinal, valorInicial, valorFinal, codigoEmpresa, numeroTitulo, dataInicial, dataFinal, etapaBaixa, pessoa, codigoOperador, TipoServicoMultisoftware, tipoAmbiente, numeroDocumentoOriginario, codigoGrupoPessoa, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repTituloBaixa.ContaConsultaBaixaPagar(tipoBaixa, dataEmissaoInicial, dataEmissaoFinal, dataVencimentoInicial, dataVencimentoFinal, valorInicial, valorFinal, codigoEmpresa, numeroTitulo, dataInicial, dataFinal, etapaBaixa, pessoa, codigoOperador, TipoServicoMultisoftware, tipoAmbiente, numeroDocumentoOriginario, codigoGrupoPessoa));

                var lista = (from p in listaTitulo
                             select new
                             {
                                 p.Codigo,
                                 CodigosTitulos = p.CodigosTitulos,
                                 p.Fornecedores,
                                 p.DescricaoSituacaoBaixaTitulo,
                                 DataBaixa = p.DataBaixa.Value.ToString("dd/MM/yyyy"),
                                 Valor = p.Valor.ToString("n2"),
                                 ValorPendente = p.ValorPendenteBaixa.ToString("n2"),
                                 ValorTotalAcrescimo = p.ValorTotalAcrescimo.ToString("n2"),
                                 ValorTotalDesconto = p.ValorTotalDesconto.ToString("n2"),
                                 ValorTotalSaldo = p.ValorTotalSaldo.ToString("n2")
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
        public async Task<IActionResult> PesquisaTitulosPendentes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                return ObterGridPesquisaTitulosPendentes(unitOfWork);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar os títulos.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarPesquisaTitulosPendentes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                return ObterGridPesquisaTitulosPendentes(unitOfWork, true);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao exportar os títulos.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarDocumentosSelecionados()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaBaixaTituloPendente filtrosPesquisa = ObterFiltrosPesquisaTituloPendente(unitOfWork);

                bool.TryParse(Request.Params("SelecionarTodos"), out bool selecionarTodos);

                List<int> codigosTitulos = JsonConvert.DeserializeObject<List<int>>(Request.Params("ListaTitulos"));

                Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);

                List<Dominio.Entidades.Embarcador.Financeiro.Titulo> listaTitulos = repTitulo.ObterDocumentosSelecionadosTitulosAPagar(selecionarTodos, codigosTitulos, filtrosPesquisa);

                return new JsonpResult(new
                {
                    ValorTotalPendente = listaTitulos.Count > 0 ? listaTitulos.Select(obj => obj.Saldo).Sum().ToString("n2") : 0.ToString("n2"),
                    ValorOriginalMoedaEstrangeira = listaTitulos.Count > 0 ? listaTitulos.Select(obj => obj.ValorOriginalMoedaEstrangeira).Sum().ToString("n2") : 0.ToString("n2"),
                    DocumentosTitulo = listaTitulos.Count > 0 ? string.Join(", ", listaTitulos.Select(o =>
                        !string.IsNullOrWhiteSpace(o.NumeroDocumentoTituloOriginal) ? o.NumeroDocumentoTituloOriginal : o.DuplicataDocumentoEntrada != null &&
                        o.DuplicataDocumentoEntrada.DocumentoEntrada != null ? o.DuplicataDocumentoEntrada.DocumentoEntrada.Numero.ToString("n0") : string.Empty)) : string.Empty
                });
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
        public async Task<IActionResult> PesquisarTiposDesPagamentos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoBaixa = 0;
                int.TryParse(Request.Params("Codigo"), out codigoBaixa);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("CodigoBaixa", false);
                grid.AdicionarCabecalho("Conta de Pagamento", "Descricao", 50, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Valor", "Valor", 20, Models.Grid.Align.right, false);

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                if (propOrdenar == "Descricao")
                    propOrdenar = "TipoPagamentoRecebimento.Descricao";

                Repositorio.Embarcador.Financeiro.TituloBaixaTipoPagamentoRecebimento repTituloBaixaTipoPagamentoRecebimento = new Repositorio.Embarcador.Financeiro.TituloBaixaTipoPagamentoRecebimento(unitOfWork);
                List<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaTipoPagamentoRecebimento> listaTituloBaixaTipoPagamentoRecebimento = repTituloBaixaTipoPagamentoRecebimento.Consultar(codigoBaixa, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repTituloBaixaTipoPagamentoRecebimento.ContarConsulta(codigoBaixa));

                var lista = (from p in listaTituloBaixaTipoPagamentoRecebimento
                             select new
                             {
                                 p.Codigo,
                                 CodigoBaixa = p.TituloBaixa.Codigo,
                                 Descricao = p.TipoPagamentoRecebimento.Descricao,
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

        public async Task<IActionResult> InserirTipoPagamento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = Request.GetIntParam("Codigo");
                int codigoTipoDePagamento = Request.GetIntParam("TipoDePagamento");

                decimal valor = Request.GetDecimalParam("Valor");

                Repositorio.Embarcador.Financeiro.TituloBaixaTipoPagamentoRecebimento repTituloBaixaTipoPagamentoRecebimento = new Repositorio.Embarcador.Financeiro.TituloBaixaTipoPagamentoRecebimento(unitOfWork);
                Repositorio.Embarcador.Financeiro.TituloBaixa repTituloBaixa = new Repositorio.Embarcador.Financeiro.TituloBaixa(unitOfWork);
                Repositorio.Embarcador.Financeiro.TipoPagamentoRecebimento repTipoPagamentoRecebimento = new Repositorio.Embarcador.Financeiro.TipoPagamentoRecebimento(unitOfWork);
                Repositorio.Embarcador.Financeiro.TituloBaixaAcrescimo repTituloBaixaAcrescimo = new Repositorio.Embarcador.Financeiro.TituloBaixaAcrescimo(unitOfWork);

                Dominio.Entidades.Embarcador.Financeiro.TituloBaixa tituloBaixa = repTituloBaixa.BuscarPorCodigo(codigo);

                if (tituloBaixa.SituacaoBaixaTitulo != SituacaoBaixaTitulo.Iniciada)
                    return new JsonpResult(false, true, "Esta baixa não está mais aberta.");

                decimal acrescimos = repTituloBaixaAcrescimo.TotalPorTituloBaixa(codigo, TipoJustificativa.Acrescimo);
                decimal valorTipoPagamento = repTituloBaixaTipoPagamentoRecebimento.TotalPorTituloBaixa(codigo);
                if (tituloBaixa.Valor < (valorTipoPagamento + valor - acrescimos))
                    return new JsonpResult(false, true, "A soma dos pagamentos ultrapassa o valor pago.");

                unitOfWork.Start();

                Dominio.Entidades.Embarcador.Financeiro.TituloBaixaTipoPagamentoRecebimento tituloBaixaTipoPagamentoRecebimento = new Dominio.Entidades.Embarcador.Financeiro.TituloBaixaTipoPagamentoRecebimento();
                tituloBaixaTipoPagamentoRecebimento.TituloBaixa = tituloBaixa;
                tituloBaixaTipoPagamentoRecebimento.TipoPagamentoRecebimento = repTipoPagamentoRecebimento.BuscarPorCodigo(codigoTipoDePagamento);
                tituloBaixaTipoPagamentoRecebimento.Valor = valor;

                tituloBaixaTipoPagamentoRecebimento.MoedaCotacaoBancoCentral = Request.GetNullableEnumParam<MoedaCotacaoBancoCentral>("MoedaCotacaoBancoCentral");
                tituloBaixaTipoPagamentoRecebimento.DataBaseCRT = Request.GetNullableDateTimeParam("DataBaseCRT");
                tituloBaixaTipoPagamentoRecebimento.ValorMoedaCotacao = Request.GetDecimalParam("ValorMoedaCotacao");
                if (!ConfiguracaoEmbarcador.GerarMovimentacaoNaBaixaIndividualmente)
                    tituloBaixaTipoPagamentoRecebimento.ValorOriginalMoedaEstrangeira = tituloBaixaTipoPagamentoRecebimento.ValorMoedaCotacao > 0 ? Math.Round(valor / tituloBaixaTipoPagamentoRecebimento.ValorMoedaCotacao, 2) : 0;

                repTituloBaixaTipoPagamentoRecebimento.Inserir(tituloBaixaTipoPagamentoRecebimento);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, tituloBaixa, null, "Adicionou um Tipo de Pagamento no valor de " + valor.ToString("n2") + ".", unitOfWork);

                unitOfWork.CommitChanges();

                valorTipoPagamento = repTituloBaixaTipoPagamentoRecebimento.TotalPorTituloBaixa(codigo);
                valorTipoPagamento = tituloBaixa.Valor - valorTipoPagamento - acrescimos;
                if (valorTipoPagamento < 0)
                    valorTipoPagamento = 0;

                var dynRetorno = new
                {
                    ValorTipoPagamento = valorTipoPagamento.ToString("n2")
                };

                return new JsonpResult(dynRetorno, true, "Sucesso");
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar um tipo de pagamento.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> RemoverTipoPagamento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = Request.GetIntParam("Codigo");
                int codigoBaixa = Request.GetIntParam("CodigoBaixa");

                Repositorio.Embarcador.Financeiro.TituloBaixa repTituloBaixa = new Repositorio.Embarcador.Financeiro.TituloBaixa(unitOfWork);
                Repositorio.Embarcador.Financeiro.TituloBaixaTipoPagamentoRecebimento repTituloBaixaTipoPagamentoRecebimento = new Repositorio.Embarcador.Financeiro.TituloBaixaTipoPagamentoRecebimento(unitOfWork);
                Repositorio.Embarcador.Financeiro.TituloBaixaAcrescimo repTituloBaixaAcrescimo = new Repositorio.Embarcador.Financeiro.TituloBaixaAcrescimo(unitOfWork);

                Dominio.Entidades.Embarcador.Financeiro.TituloBaixaTipoPagamentoRecebimento tituloBaixaTipoPagamentoRecebimento = repTituloBaixaTipoPagamentoRecebimento.BuscarPorCodigo(codigo);

                unitOfWork.Start();

                Dominio.Entidades.Embarcador.Financeiro.TituloBaixa tituloBaixa = repTituloBaixa.BuscarPorCodigo(codigoBaixa);

                if (tituloBaixa.SituacaoBaixaTitulo != SituacaoBaixaTitulo.Iniciada)
                    return new JsonpResult(false, true, "Esta baixa não está mais aberta.");

                Servicos.Auditoria.Auditoria.Auditar(Auditado, tituloBaixa, null, "Removeu um Tipo de Pagamento no valor de " + tituloBaixaTipoPagamentoRecebimento.Valor.ToString("n2") + ".", unitOfWork);

                repTituloBaixaTipoPagamentoRecebimento.Deletar(tituloBaixaTipoPagamentoRecebimento);

                unitOfWork.CommitChanges();

                decimal acrescimos = tituloBaixa != null ? repTituloBaixaAcrescimo.TotalPorTituloBaixa(codigoBaixa, TipoJustificativa.Acrescimo) : 0;
                decimal valorTipoPagamento = repTituloBaixaTipoPagamentoRecebimento.TotalPorTituloBaixa(codigoBaixa);
                valorTipoPagamento = tituloBaixa.Valor - valorTipoPagamento - acrescimos;
                if (valorTipoPagamento < 0)
                    valorTipoPagamento = 0;

                var dynRetorno = new
                {
                    ValorTipoPagamento = valorTipoPagamento.ToString("n2")
                };

                return new JsonpResult(dynRetorno, true, "Sucesso");
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao remover um tipo de pagamento.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisarAcrescimoDesconto()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoBaixa = 0;
                int.TryParse(Request.Params("Codigo"), out codigoBaixa);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("CodigoBaixa", false);
                grid.AdicionarCabecalho("Justificativa", "Descricao", 30, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Tipo", "DescricaoTipo", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Valor", "Valor", 10, Models.Grid.Align.right, false);

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;

                Repositorio.Embarcador.Financeiro.TituloBaixaAcrescimo repTituloBaixaAcrescimo = new Repositorio.Embarcador.Financeiro.TituloBaixaAcrescimo(unitOfWork);
                List<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAcrescimo> listaTituloBaixaAcrescimo = repTituloBaixaAcrescimo.ConsultarAcrescimoDesconto(codigoBaixa, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repTituloBaixaAcrescimo.ContarAcrescimoDesconto(codigoBaixa));

                var lista = (from p in listaTituloBaixaAcrescimo
                             select new
                             {
                                 p.Codigo,
                                 CodigoBaixa = p.TituloBaixa.Codigo,
                                 Descricao = p.Justificativa.Descricao,
                                 DescricaoTipo = p.Justificativa.DescricaoTipoJustificativa,
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

        public async Task<IActionResult> InserirAcrescimoDesconto()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Financeiros/BaixaTituloPagar");
                if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe && !this.Usuario.UsuarioAdministrador)
                    if (permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.BaixaPagar_NaoPermitirLancarDescontoAcrescimo))
                        return new JsonpResult(false, true, "Seu usuário não possui permissão para inserir acréscimo ou desconto na baixa de título a pagar.");

                int codigo = Request.GetIntParam("Codigo");
                int codigoJustificativa = Request.GetIntParam("Justificativa");

                decimal valor = Request.GetDecimalParam("Valor");

                Repositorio.Embarcador.Financeiro.TituloBaixaAcrescimo repTituloBaixaAcrescimo = new Repositorio.Embarcador.Financeiro.TituloBaixaAcrescimo(unitOfWork);
                Repositorio.Embarcador.Financeiro.TituloBaixa repTituloBaixa = new Repositorio.Embarcador.Financeiro.TituloBaixa(unitOfWork);
                Repositorio.Embarcador.Fatura.Justificativa repJustificativa = new Repositorio.Embarcador.Fatura.Justificativa(unitOfWork);
                Repositorio.Embarcador.Financeiro.TituloBaixaNegociacao repTituloBaixaNegociacao = new Repositorio.Embarcador.Financeiro.TituloBaixaNegociacao(unitOfWork);

                unitOfWork.Start();

                Dominio.Entidades.Embarcador.Financeiro.TituloBaixa tituloBaixa = repTituloBaixa.BuscarPorCodigo(codigo);

                if (tituloBaixa.SituacaoBaixaTitulo != SituacaoBaixaTitulo.Iniciada)
                    return new JsonpResult(false, true, "Esta baixa não está mais aberta.");

                Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAcrescimo tituloAcrescimoDesconto = new Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAcrescimo();
                tituloAcrescimoDesconto.TituloBaixa = tituloBaixa;
                tituloAcrescimoDesconto.Justificativa = repJustificativa.BuscarPorCodigo(codigoJustificativa);
                tituloAcrescimoDesconto.Valor = valor;

                tituloAcrescimoDesconto.MoedaCotacaoBancoCentral = Request.GetNullableEnumParam<MoedaCotacaoBancoCentral>("MoedaCotacaoBancoCentral");
                tituloAcrescimoDesconto.DataBaseCRT = Request.GetNullableDateTimeParam("DataBaseCRT");
                tituloAcrescimoDesconto.ValorMoedaCotacao = Request.GetDecimalParam("ValorMoedaCotacao");
                tituloAcrescimoDesconto.ValorOriginalMoedaEstrangeira = Request.GetDecimalParam("ValorOriginalMoedaEstrangeira");
                if (!ConfiguracaoEmbarcador.GerarMovimentacaoNaBaixaIndividualmente)
                {
                    if (tituloAcrescimoDesconto.ValorOriginalMoedaEstrangeira == 0)
                        tituloAcrescimoDesconto.ValorOriginalMoedaEstrangeira = tituloAcrescimoDesconto.ValorMoedaCotacao > 0 ? Math.Round(valor / tituloAcrescimoDesconto.ValorMoedaCotacao, 2) : 0;
                }
                else
                    tituloAcrescimoDesconto.ValorOriginalMoedaEstrangeira = 0;

                repTituloBaixaAcrescimo.Inserir(tituloAcrescimoDesconto);

                decimal saldoDevedor = tituloBaixa.ValorPendente;
                decimal descontos = repTituloBaixaAcrescimo.TotalPorTituloBaixa(codigo, TipoJustificativa.Desconto);
                decimal acrescimos = repTituloBaixaAcrescimo.TotalPorTituloBaixa(codigo, TipoJustificativa.Acrescimo);
                decimal valorPendente = tituloBaixa.ValorPendente - tituloBaixa.Valor - descontos + acrescimos;
                if (valorPendente < 0)
                    valorPendente = valorPendente * -1;
                tituloBaixa.ValorPendente = valorPendente;

                repTituloBaixa.Atualizar(tituloBaixa);

                List<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaNegociacao> listaParcelas = repTituloBaixaNegociacao.BuscarPorTituloBaixa(codigo);
                for (int i = 0; i < listaParcelas.Count; i++)
                    repTituloBaixaNegociacao.Deletar(listaParcelas[i]);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, tituloBaixa, null, "Adicionou um Acréscimo/Desconto no valor de " + valor.ToString("n2") + ".", unitOfWork);

                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar acréscimo / desconto.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> RemoverAcrescimoDesconto()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Financeiros/BaixaTituloPagar");
                if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe && !this.Usuario.UsuarioAdministrador)
                    if (permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.BaixaPagar_NaoPermitirLancarDescontoAcrescimo))
                        return new JsonpResult(false, true, "Seu usuário não possui permissão para excluir acréscimo ou desconto na baixa de título a pagar.");

                int codigo = Request.GetIntParam("Codigo");
                int codigoBaixa = Request.GetIntParam("CodigoBaixa");

                Repositorio.Embarcador.Financeiro.TituloBaixa repTituloBaixa = new Repositorio.Embarcador.Financeiro.TituloBaixa(unitOfWork);
                Repositorio.Embarcador.Financeiro.TituloBaixaAcrescimo repTituloBaixaAcrescimo = new Repositorio.Embarcador.Financeiro.TituloBaixaAcrescimo(unitOfWork);
                Repositorio.Embarcador.Financeiro.TituloBaixaNegociacao repTituloBaixaNegociacao = new Repositorio.Embarcador.Financeiro.TituloBaixaNegociacao(unitOfWork);
                Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAcrescimo tituloAcrescimoDesconto = repTituloBaixaAcrescimo.BuscarPorCodigo(codigo);

                unitOfWork.Start();

                Dominio.Entidades.Embarcador.Financeiro.TituloBaixa tituloBaixa = repTituloBaixa.BuscarPorCodigo(codigoBaixa);

                if (tituloBaixa.SituacaoBaixaTitulo != SituacaoBaixaTitulo.Iniciada)
                    return new JsonpResult(false, true, "Esta baixa não está mais aberta.");

                decimal valor = tituloAcrescimoDesconto.Valor;
                TipoJustificativa tipo = tituloAcrescimoDesconto.Justificativa.TipoJustificativa;

                Servicos.Auditoria.Auditoria.Auditar(Auditado, tituloBaixa, null, "Removeu um Acréscimo/Desconto no valor de " + tituloAcrescimoDesconto.Valor.ToString("n2") + ".", unitOfWork);

                repTituloBaixaAcrescimo.Deletar(tituloAcrescimoDesconto);

                List<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaNegociacao> listaParcelas = repTituloBaixaNegociacao.BuscarPorTituloBaixa(codigoBaixa);
                for (int i = 0; i < listaParcelas.Count; i++)
                    repTituloBaixaNegociacao.Deletar(listaParcelas[i]);

                decimal saldoDevedor = tituloBaixa.ValorPendente;
                decimal descontos = tituloBaixa != null ? repTituloBaixaAcrescimo.TotalPorTituloBaixa(codigo, TipoJustificativa.Desconto) : 0;
                decimal acrescimos = tituloBaixa != null ? repTituloBaixaAcrescimo.TotalPorTituloBaixa(codigo, TipoJustificativa.Acrescimo) : 0;
                decimal valorPendente = tituloBaixa.ValorPendente - tituloBaixa.Valor - descontos + acrescimos;
                if (valorPendente < 0)
                    valorPendente = valorPendente * -1;
                if (valorPendente == valor && TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                {
                    if (tipo == TipoJustificativa.Acrescimo)
                        tituloBaixa.Valor -= valor;
                    else
                        tituloBaixa.Valor += valor;
                }
                repTituloBaixa.Atualizar(tituloBaixa);

                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao remover acréscimo/desconto.");
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

                Servicos.Embarcador.Financeiro.BaixaTituloPagar servBaixaTituloPagar = new Servicos.Embarcador.Financeiro.BaixaTituloPagar(unitOfWork);
                dynamic dynRetorno = servBaixaTituloPagar.RetornaObjetoCompletoTitulo(codigo, unitOfWork);

                return new JsonpResult(dynRetorno);
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

        public async Task<IActionResult> BaixarTitulo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Financeiros/BaixaTituloPagar");
                if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    if (!(this.Usuario.UsuarioAdministrador || permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.BaixaPagar_PermiteBaixarTitulo)))
                        return new JsonpResult(false, "Seu usuário não possui permissão para iniciar a baixa do título.");

                Servicos.Embarcador.Financeiro.BaixaTituloPagar servBaixaTituloPagar = new Servicos.Embarcador.Financeiro.BaixaTituloPagar(unitOfWork);

                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);
                Repositorio.Embarcador.Financeiro.TituloBaixa repTituloBaixa = new Repositorio.Embarcador.Financeiro.TituloBaixa(unitOfWork);
                Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);
                Repositorio.Embarcador.Financeiro.TituloBaixaAcrescimo repTituloBaixaAcrescimo = new Repositorio.Embarcador.Financeiro.TituloBaixaAcrescimo(unitOfWork);
                Repositorio.Embarcador.Financeiro.TituloBaixaDesconto repTituloBaixaDesconto = new Repositorio.Embarcador.Financeiro.TituloBaixaDesconto(unitOfWork);
                Repositorio.Embarcador.Financeiro.TituloBaixaNegociacao repTituloBaixaNegociacao = new Repositorio.Embarcador.Financeiro.TituloBaixaNegociacao(unitOfWork);
                Repositorio.Embarcador.Financeiro.TituloBaixaConhecimentoRemovido repTituloBaixaConhecimentoRemovido = new Repositorio.Embarcador.Financeiro.TituloBaixaConhecimentoRemovido(unitOfWork);
                Repositorio.Embarcador.Financeiro.TituloBaixaAgrupado repTituloBaixaAgrupado = new Repositorio.Embarcador.Financeiro.TituloBaixaAgrupado(unitOfWork);

                int.TryParse(Request.Params("Codigo"), out int codigo);
                int.TryParse(Request.Params("CodigoTitulo"), out int codigoTitulo);
                string observacao = Request.Params("Observacao");
                decimal.TryParse(Request.Params("ValorBaixado"), out decimal valorBaixado);
                DateTime.TryParse(Request.Params("DataBaixa"), out DateTime dataBaixa);
                Enum.TryParse(Request.Params("Etapa"), out SituacaoBaixaTitulo etapaBaixaTitulo);

                #region Filtros busca títulos

                Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaBaixaTituloPendente filtrosPesquisa = ObterFiltrosPesquisaTituloPendente(unitOfWork);

                bool.TryParse(Request.Params("SelecionarTodos"), out bool selecionarTodos);

                List<int> codigosTitulos = JsonConvert.DeserializeObject<List<int>>(Request.Params("ListaTitulos"));

                #endregion

                List<int> listaCodigos = repTitulo.ObterCodigosTitulosAPagar(selecionarTodos, codigosTitulos, filtrosPesquisa);

                if (listaCodigos.Count() == 0 && codigo == 0)
                    return new JsonpResult(false, "Nenhum título selecionado.");

                Dominio.Entidades.Embarcador.Financeiro.TituloBaixa tituloBaixa;
                if (codigo > 0)
                    tituloBaixa = repTituloBaixa.BuscarPorCodigo(codigo, true);
                else
                    tituloBaixa = new Dominio.Entidades.Embarcador.Financeiro.TituloBaixa();

                if (dataBaixa > DateTime.Now)
                    return new JsonpResult(false, "Não é possível realizar uma baixa com data maior que a atual.");

                unitOfWork.Start();

                tituloBaixa.DataBaixa = dataBaixa;
                tituloBaixa.DataBase = dataBaixa;
                tituloBaixa.DataOperacao = DateTime.Now;
                tituloBaixa.Numero = 1;
                tituloBaixa.Observacao = observacao;
                tituloBaixa.SituacaoBaixaTitulo = etapaBaixaTitulo;
                tituloBaixa.Sequencia = 1;
                tituloBaixa.Valor = valorBaixado;
                tituloBaixa.TipoBaixaTitulo = TipoTitulo.Pagar;
                tituloBaixa.Usuario = this.Usuario;

                tituloBaixa.MoedaCotacaoBancoCentral = Request.GetNullableEnumParam<MoedaCotacaoBancoCentral>("MoedaCotacaoBancoCentral");
                tituloBaixa.DataBaseCRT = Request.GetNullableDateTimeParam("DataBaseCRT");
                tituloBaixa.ValorMoedaCotacao = Request.GetDecimalParam("ValorMoedaCotacao");
                tituloBaixa.ValorOriginalMoedaEstrangeira = Request.GetDecimalParam("ValorOriginalMoedaEstrangeira");

                double codigoPessoa = 0;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                {
                    for (int i = 0; i < listaCodigos.Count(); i++)
                    {
                        if (codigoPessoa == 0)
                            codigoPessoa = repTitulo.BuscarPorCodigo(listaCodigos[i]).Pessoa.CPF_CNPJ;
                        else
                        {
                            double codigoPessoaNova = repTitulo.BuscarPorCodigo(listaCodigos[i]).Pessoa.CPF_CNPJ;
                            if (codigoPessoaNova != codigoPessoa)
                            {
                                codigoPessoa = 0;
                                break;
                            }
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < listaCodigos.Count(); i++)
                    {
                        if (codigoPessoa == 0)
                            codigoPessoa = repTitulo.BuscarPorCodigo(listaCodigos[i]).Pessoa.CPF_CNPJ;
                    }
                }
                if (codigoPessoa > 0)
                    tituloBaixa.Pessoa = repCliente.BuscarPorCPFCNPJ(codigoPessoa);
                else
                    tituloBaixa.Pessoa = null;

                int codigoGrupoPessoas = 0;
                for (int i = 0; i < listaCodigos.Count(); i++)
                {
                    if (repTitulo.BuscarPorCodigo(listaCodigos[i])?.GrupoPessoas != null)
                    {
                        if (codigoGrupoPessoas == 0)
                            codigoGrupoPessoas = repTitulo.BuscarPorCodigo(listaCodigos[i]).GrupoPessoas.Codigo;
                        else
                        {
                            if (codigoGrupoPessoas != repTitulo.BuscarPorCodigo(listaCodigos[i]).GrupoPessoas.Codigo)
                            {
                                codigoGrupoPessoas = 0;
                                break;
                            }
                        }
                    }
                }
                if (codigoGrupoPessoas > 0)
                    tituloBaixa.GrupoPessoas = repGrupoPessoas.BuscarPorCodigo(codigoGrupoPessoas);
                else
                    tituloBaixa.GrupoPessoas = null;

                if (codigo > 0)
                {
                    repTituloBaixa.Atualizar(tituloBaixa);
                    List<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaNegociacao> listaParcelas = repTituloBaixaNegociacao.BuscarPorTituloBaixa(codigo);
                    for (int i = 0; i < listaParcelas.Count; i++)
                        repTituloBaixaNegociacao.Deletar(listaParcelas[i]);

                    List<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAcrescimo> listaTituloBaixaAcrescimo = repTituloBaixaAcrescimo.BuscarPorBaixaTitulo(codigo);
                    for (int i = 0; i < listaTituloBaixaAcrescimo.Count; i++)
                        repTituloBaixaAcrescimo.Deletar(listaTituloBaixaAcrescimo[i]);

                    List<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaDesconto> listaTituloBaixaDesconto = repTituloBaixaDesconto.BuscarPorBaixaTitulo(codigo);
                    for (int i = 0; i < listaTituloBaixaDesconto.Count; i++)
                        repTituloBaixaDesconto.Deletar(listaTituloBaixaDesconto[i]);
                }
                else
                    repTituloBaixa.Inserir(tituloBaixa, Auditado);

                bool exigeDataAutorizacaoPagamento = ConfiguracaoEmbarcador.ExigirDataAutorizacaoParaPagamento && valorBaixado > 0;

                if (codigo == 0)
                {
                    for (int i = 0; i < listaCodigos.Count(); i++)
                    {
                        Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = repTitulo.BuscarPorCodigo(listaCodigos[i]);
                        Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado tituloAgrupado = new Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado();
                        tituloAgrupado.TituloBaixa = tituloBaixa;
                        tituloAgrupado.Titulo = titulo;
                        tituloAgrupado.DataBaixa = dataBaixa;
                        tituloAgrupado.DataBase = dataBaixa;

                        if (exigeDataAutorizacaoPagamento && (!titulo.DataAutorizacao.HasValue || titulo.DataAutorizacao.Value == DateTime.MinValue))
                        {
                            unitOfWork.Rollback();
                            return new JsonpResult(false, "O título de código " + titulo.Codigo.ToString() + " não possui data de autorização para realizar o seu pagamento.");
                        }

                        if (titulo.ContratoFrete != null)
                        {
                            List<Dominio.Entidades.Embarcador.Financeiro.Titulo> titulosContrato = repTitulo.BuscarTodosPorContratoFrete(titulo.ContratoFrete.Codigo);

                            List<int> codigosTitulosContratoNaoQuitados = titulosContrato.Where(o => o.Sequencia < titulo.Sequencia && o.StatusTitulo != StatusTitulo.Quitada && o.StatusTitulo != StatusTitulo.Cancelado).Select(o => o.Codigo).ToList();

                            if (codigosTitulosContratoNaoQuitados.Count > 0 && !codigosTitulosContratoNaoQuitados.All(o => listaCodigos.Contains(o)))
                            {
                                unitOfWork.Rollback();
                                return new JsonpResult(false, $"O título {titulo.Codigo} não pode ser baixado pois existem parcelas anteriores ({string.Join(", ", codigosTitulosContratoNaoQuitados)}) que não foram quitadas.");
                            }
                        }

                        repTituloBaixaAgrupado.Inserir(tituloAgrupado);
                    }
                }

                Servicos.Auditoria.Auditoria.Auditar(Auditado, tituloBaixa, null, "Baixou o Título.", unitOfWork);

                unitOfWork.CommitChanges();

                var dynRetorno = servBaixaTituloPagar.RetornaObjetoCompletoTitulo(tituloBaixa.Codigo, unitOfWork);

                return new JsonpResult(dynRetorno, true, "Sucesso");
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> CarregarNegociacaoBaixa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);
                Repositorio.Embarcador.Financeiro.TituloBaixa repTituloBaixa = new Repositorio.Embarcador.Financeiro.TituloBaixa(unitOfWork);
                Repositorio.Embarcador.Financeiro.TituloBaixaAcrescimo repTituloBaixaAcrescimo = new Repositorio.Embarcador.Financeiro.TituloBaixaAcrescimo(unitOfWork);
                Repositorio.Embarcador.Financeiro.TituloBaixaTipoPagamentoRecebimento repTituloBaixaTipoPagamento = new Repositorio.Embarcador.Financeiro.TituloBaixaTipoPagamentoRecebimento(unitOfWork);
                Repositorio.Embarcador.Financeiro.TituloBaixaAgrupado repTituloBaixaAgrupado = new Repositorio.Embarcador.Financeiro.TituloBaixaAgrupado(unitOfWork);
                Repositorio.Embarcador.Financeiro.MovimentoFinanceiro repMovimento = new Repositorio.Embarcador.Financeiro.MovimentoFinanceiro(unitOfWork);
                Repositorio.Cliente repPessoa = new Repositorio.Cliente(unitOfWork);

                int.TryParse(Request.Params("Codigo"), out int codigo);
                int.TryParse(Request.Params("TipoDePagamento"), out int codigoTipoDePagamento);
                double.TryParse(Request.Params("PessoaNegociacao"), out double codigoPessoaNegociacao);
                Dominio.Entidades.Embarcador.Financeiro.TituloBaixa tituloBaixa = repTituloBaixa.BuscarPorCodigo(codigo);
                Dominio.Entidades.Embarcador.Financeiro.TipoPagamentoRecebimento tipoPagamento = null;

                DateTime dataEmissao = DateTime.MinValue;
                DateTime dataEmissaoTitulo = DateTime.MinValue;

                List<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado> titulosAgrupados = repTituloBaixaAgrupado.BuscarPorBaixaTitulo(codigo);
                for (int i = 0; i < titulosAgrupados.Count(); i++)
                {
                    Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado tituloAgrupado = titulosAgrupados[i];
                    dataEmissaoTitulo = tituloAgrupado.Titulo.DataEmissao.Value;
                    if (dataEmissaoTitulo > dataEmissao)
                        dataEmissao = dataEmissaoTitulo;
                }

                if (dataEmissao == DateTime.MinValue)
                    dataEmissao = DateTime.Now;

                Dominio.Entidades.Cliente pessoaNegociacao = null;
                if (codigoPessoaNegociacao > 0)
                    pessoaNegociacao = repPessoa.BuscarPorCPFCNPJ(codigoPessoaNegociacao);
                else if (tituloBaixa.Pessoa != null)
                    pessoaNegociacao = repPessoa.BuscarPorCPFCNPJ(tituloBaixa.Pessoa.CPF_CNPJ);

                decimal valorABaixar = 0;
                if (tituloBaixa != null)
                    valorABaixar = tituloBaixa.Valor;

                decimal saldoDevedor = tituloBaixa.ValorPendente;
                decimal valorTipoPagamento = 0;
                decimal descontos = tituloBaixa != null ? repTituloBaixaAcrescimo.TotalPorTituloBaixa(codigo, TipoJustificativa.Desconto) : 0;
                decimal acrescimos = tituloBaixa != null ? repTituloBaixaAcrescimo.TotalPorTituloBaixa(codigo, TipoJustificativa.Acrescimo) : 0;
                decimal valorPendente = tituloBaixa.ValorPendente - valorABaixar - descontos + acrescimos;
                if (tituloBaixa.SituacaoBaixaTitulo == SituacaoBaixaTitulo.Finalizada)
                {
                    valorPendente = 0;
                    valorABaixar = 0;
                    saldoDevedor = 0;
                    valorTipoPagamento = 0;
                }
                if (valorABaixar > 0)
                {
                    valorTipoPagamento = repTituloBaixaTipoPagamento.TotalPorTituloBaixa(codigo);
                    if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                        valorTipoPagamento = valorABaixar - valorTipoPagamento;// - acrescimos; REMOVIDO POR CAUSA DO DILNEI
                    else
                        valorTipoPagamento = valorABaixar - valorTipoPagamento - acrescimos;
                    if (valorTipoPagamento < 0)
                        valorTipoPagamento = 0;
                }

                if (tituloBaixa != null && tituloBaixa.TipoPagamentoRecebimento != null && tipoPagamento == null)
                    tipoPagamento = tituloBaixa.TipoPagamentoRecebimento;
                else if (tituloBaixa != null && tituloBaixa.GrupoPessoas != null && tituloBaixa.GrupoPessoas.FormaPagamento != null && tipoPagamento == null)
                    tipoPagamento = tituloBaixa.GrupoPessoas.FormaPagamento;
                else if (tituloBaixa != null && tituloBaixa.Pessoa != null && tituloBaixa.Pessoa.FormaPagamento != null && tipoPagamento == null)
                    tipoPagamento = tituloBaixa.Pessoa.FormaPagamento;

                string codigos = tituloBaixa.CodigosTitulos;
                if (!string.IsNullOrWhiteSpace(codigos) && codigos.Length > 30)
                    codigos = codigos.Substring(0, 29) + "...";

                decimal saldoContaAdiantamento = 0m;
                if (ConfiguracaoEmbarcador.PlanoContaAdiantamentoFornecedor != null && tituloBaixa != null && tituloBaixa.Pessoa != null && (tituloBaixa.SituacaoBaixaTitulo == SituacaoBaixaTitulo.EmNegociacao || tituloBaixa.SituacaoBaixaTitulo == SituacaoBaixaTitulo.Iniciada))
                    saldoContaAdiantamento = repMovimento.BuscarSaldoContaCliente(ConfiguracaoEmbarcador.PlanoContaAdiantamentoFornecedor.Codigo, tituloBaixa.Pessoa.CPF_CNPJ);

                var dynRetorno = new
                {
                    Codigo = tituloBaixa != null ? tituloBaixa.Codigo : 0,
                    NumeroTitulo = codigos,
                    ValorOriginal = tituloBaixa.ValorOriginal.ToString("n2"),
                    ValorABaixar = valorABaixar.ToString("n2"),
                    ValorTipoPagamento = valorTipoPagamento.ToString("n2"),
                    DataBaixar = tituloBaixa != null ? tituloBaixa.DataBaixa.Value.ToString("dd/MM/yyyy") : string.Empty,
                    tituloBaixa.MoedaCotacaoBancoCentral,
                    DataBaseCRT = tituloBaixa.DataBaseCRT.HasValue ? tituloBaixa.DataBaseCRT.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty,
                    ValorMoedaCotacao = tituloBaixa.ValorMoedaCotacao.ToString("n10"),
                    ValorOriginalMoedaEstrangeira = tituloBaixa.ValorOriginalMoedaEstrangeira.ToString("n2"),
                    SaldoDevedor = saldoDevedor.ToString("n2"),
                    Operador = tituloBaixa.Usuario?.Nome ?? this.Usuario.Nome,
                    Descontos = descontos.ToString("n2"),
                    Acrescimos = acrescimos.ToString("n2"),
                    ValorPendente = valorPendente.ToString("n2"),
                    CodigoFornecedor = tituloBaixa.Pessoa != null ? tituloBaixa.Pessoa.CPF_CNPJ : 0,
                    CodigoGrupoPessoa = tituloBaixa.QuantidadeGrupoPessoa > 1 ? tituloBaixa.CodigoGrupoPessoa : 0,
                    tituloBaixa.QuantidadePessoa,
                    PessoaNegociacao = new { Codigo = pessoaNegociacao != null ? pessoaNegociacao.Codigo : 0, Descricao = pessoaNegociacao != null ? pessoaNegociacao.Nome : "" },
                    Parcelas = tituloBaixa != null && tituloBaixa.TitulosNegociacao != null ? (from obj in tituloBaixa.TitulosNegociacao
                                                                                               orderby obj.Sequencia
                                                                                               select new
                                                                                               {
                                                                                                   obj.Codigo,
                                                                                                   Acrescimo = obj.Acrescimo.ToString("n2"),
                                                                                                   DataEmissao = obj.DataEmissao.ToString("dd/MM/yyyy"),
                                                                                                   DataVencimento = obj.DataVencimento.ToString("dd/MM/yyyy"),
                                                                                                   Desconto = obj.Desconto.ToString("n2"),
                                                                                                   obj.DescricaoSituacao,
                                                                                                   obj.Sequencia,
                                                                                                   obj.SituacaoFaturaParcela,
                                                                                                   Valor = obj.Valor.ToString("n2")
                                                                                               }).ToList() : null,
                    TipoDePagamento = tituloBaixa.TipoPagamentoRecebimento == null ? new { Codigo = tipoPagamento != null ? tipoPagamento.Codigo : 0, Descricao = tipoPagamento != null ? tipoPagamento.Descricao : "" } : new { Codigo = tituloBaixa.TipoPagamentoRecebimento != null ? tituloBaixa.TipoPagamentoRecebimento.Codigo : 0, Descricao = tituloBaixa.TipoPagamentoRecebimento != null ? tituloBaixa.TipoPagamentoRecebimento.Descricao : "" },
                    DataEmissao = dataEmissao.ToString("dd/MM/yyyy"),
                    MoedaCotacaoBancoCentralNegociacao = tituloBaixa.MoedaCotacaoBancoCentral.HasValue ? tituloBaixa.MoedaCotacaoBancoCentral.Value.ObterDescricaoSimplificada() : "",
                    DataBaseCRTNegociacao = tituloBaixa.DataBaseCRT.HasValue ? tituloBaixa.DataBaseCRT.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty,
                    ValorMoedaCotacaoNegociacao = tituloBaixa.ValorMoedaCotacao.ToString("n10"),
                    ValorOriginalMoedaEstrangeiraNegociacao = tituloBaixa.ValorOriginalMoedaEstrangeira.ToString("n2"),
                    SaldoContaAdiantamento = saldoContaAdiantamento.ToString("n2")
                };

                return new JsonpResult(dynRetorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar os dados para a negociação.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> PesquisaParcelasNegociacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int.TryParse(Request.Params("Codigo"), out int codigo);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("CodigoTituloBaixa", false);
                grid.AdicionarCabecalho("NumeroBoleto", false);
                grid.AdicionarCabecalho("FormaParcela", false);
                grid.AdicionarCabecalho("Cód. Título", "CodigoTitulo", 10, Models.Grid.Align.right, false);
                grid.AdicionarCabecalho("Parcela", "Parcela", 8, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Valor", "Valor", 10, Models.Grid.Align.right, false);
                grid.AdicionarCabecalho("Desconto", false);
                grid.AdicionarCabecalho("Data Vencimento", "DataVencimento", 15, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Situação", "DescricaoSituacao", 20, Models.Grid.Align.left, false);

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                if (propOrdenar == "Parcela")
                    propOrdenar = "Sequencia";
                if (propOrdenar == "CodigoTitulo")
                    propOrdenar = "Sequencia";

                Repositorio.Embarcador.Financeiro.TituloBaixaNegociacao repTituloBaixaNegociacao = new Repositorio.Embarcador.Financeiro.TituloBaixaNegociacao(unitOfWork);

                List<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaNegociacao> listaTituloBaixaNegociacao = repTituloBaixaNegociacao.BuscarPorTituloBaixa(codigo, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repTituloBaixaNegociacao.ContarBuscarPorTituloBaixa(codigo));
                var dynRetorno = (from obj in listaTituloBaixaNegociacao
                                  select new
                                  {
                                      obj.Codigo,
                                      CodigoTituloBaixa = obj.TituloBaixa != null ? obj.TituloBaixa.Codigo : 0,
                                      obj.NumeroBoleto,
                                      obj.FormaParcela,
                                      CodigoTitulo = obj.CodigoTitulo.ToString("n0"),
                                      Parcela = obj.Sequencia.ToString("n0"),
                                      Valor = obj.Valor.ToString("n2"),
                                      Desconto = obj.Desconto.ToString("n2"),
                                      DataVencimento = obj.DataVencimento.ToString("dd/MM/yyyy"),
                                      obj.DescricaoSituacao
                                  }).ToList();

                grid.AdicionaRows(dynRetorno);
                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar as parcelas da negociação.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> GerarParcelas()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Financeiros/BaixaTituloPagar");
                if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    if (!(this.Usuario.UsuarioAdministrador || permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.BaixaPagar_PermiteGerarParcelas)))
                        return new JsonpResult(false, "Seu usuário não possui permissão para gerar parcelas de negociação.");

                Repositorio.Embarcador.Financeiro.TituloBaixaAcrescimo repTituloBaixaAcrescimo = new Repositorio.Embarcador.Financeiro.TituloBaixaAcrescimo(unitOfWork);
                Repositorio.Embarcador.Financeiro.TituloBaixa repTituloBaixa = new Repositorio.Embarcador.Financeiro.TituloBaixa(unitOfWork);
                Repositorio.Embarcador.Financeiro.TituloBaixaNegociacao repTituloBaixaNegociacao = new Repositorio.Embarcador.Financeiro.TituloBaixaNegociacao(unitOfWork);
                Repositorio.Embarcador.Financeiro.TituloBaixaAgrupado repTituloBaixaAgrupado = new Repositorio.Embarcador.Financeiro.TituloBaixaAgrupado(unitOfWork);

                int codigoBaixaTitulo, quantidadeParcelas, intervaloDeDias = 0;
                int.TryParse(Request.Params("Codigo"), out codigoBaixaTitulo);
                int.TryParse(Request.Params("QuantidadeParcelas"), out quantidadeParcelas);
                int.TryParse(Request.Params("IntervaloDeDias"), out intervaloDeDias);
                double codigoPessoaNegociacao = 0;
                double.TryParse(Request.Params("PessoaNegociacao"), out codigoPessoaNegociacao);

                int codigoTipoDePagamento = 0;
                int.TryParse(Request.Params("TipoDePagamento"), out codigoTipoDePagamento);

                DateTime dataPrimeiroVencimento;
                DateTime.TryParse(Request.Params("DataPrimeiroVencimento"), out dataPrimeiroVencimento);

                DateTime dataEmissao;
                DateTime.TryParse(Request.Params("DataEmissao"), out dataEmissao);

                TipoArredondamento tipoArredondamento;
                Enum.TryParse(Request.Params("TipoArredondamento"), out tipoArredondamento);
                FormaTitulo formaTitulo;
                Enum.TryParse(Request.Params("FormaTitulo"), out formaTitulo);

                Dominio.Entidades.Embarcador.Financeiro.TituloBaixa tituloBaixa = repTituloBaixa.BuscarPorCodigo(codigoBaixaTitulo);

                List<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado> titulosAgrupados = repTituloBaixaAgrupado.BuscarPorBaixaTitulo(codigoBaixaTitulo);
                DateTime dataEmissaoTitulo = DateTime.MinValue;
                DateTime dataEmissaoAux = DateTime.MinValue;
                for (int i = 0; i < titulosAgrupados.Count(); i++)
                {
                    Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado tituloAgrupado = new Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado();
                    dataEmissaoTitulo = titulosAgrupados[i].Titulo.DataEmissao.Value;
                    if (dataEmissaoTitulo > dataEmissaoAux)
                        dataEmissaoAux = dataEmissaoTitulo;
                }
                if (dataEmissaoAux == DateTime.MinValue)
                    dataEmissaoAux = DateTime.Now;

                if (dataEmissao.Date < dataEmissaoAux.Date || dataEmissao.Date > DateTime.Now.Date)
                    return new JsonpResult(false, "A data de emissão não pode ser inferior que a maior data dos títulos selecionados (" + dataEmissaoAux.ToString("dd/MM/yyyy") + "), e também não pode ser superior que a data atual.");

                if (tituloBaixa.Pessoa == null && codigoPessoaNegociacao == 0)
                    return new JsonpResult(false, "Não é possível gerar parcelas de uma baixa com multiplos fornecedores.");

                List<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaNegociacao> listaParcelas = repTituloBaixaNegociacao.BuscarPorTituloBaixa(codigoBaixaTitulo);
                for (int i = 0; i < listaParcelas.Count; i++)
                    repTituloBaixaNegociacao.Deletar(listaParcelas[i]);

                decimal valorABaixar = tituloBaixa.Valor;
                decimal descontos = tituloBaixa != null ? repTituloBaixaAcrescimo.TotalPorTituloBaixa(codigoBaixaTitulo, TipoJustificativa.Desconto) : 0;
                decimal acrescimos = tituloBaixa != null ? repTituloBaixaAcrescimo.TotalPorTituloBaixa(codigoBaixaTitulo, TipoJustificativa.Acrescimo) : 0;
                decimal valorTotal = Math.Round(tituloBaixa.ValorPendente - valorABaixar, 2); //- descontos + acrescimos

                decimal valorParcela = Math.Round(valorTotal / quantidadeParcelas, 2);
                decimal valorDiferenca = valorTotal - Math.Round(valorParcela * quantidadeParcelas, 2);

                decimal valorParcelaDesconto = Math.Round(descontos / quantidadeParcelas, 2);
                decimal valorDiferencaDesconto = descontos - Math.Round(valorParcelaDesconto * quantidadeParcelas, 2);

                decimal valorParcelaAcrescimo = Math.Round(acrescimos / quantidadeParcelas, 2);
                decimal valorDiferencaAcrescimo = acrescimos - Math.Round(valorParcelaAcrescimo * quantidadeParcelas, 2);

                DateTime dataUltimaParcela = dataPrimeiroVencimento;

                if (valorParcela <= 0)
                    return new JsonpResult(false, "O valor pendente está zerado. Favor verifique os lançamentos antes de gerar a negociação.");

                for (int i = 0; i < quantidadeParcelas; i++)
                {
                    Dominio.Entidades.Embarcador.Financeiro.TituloBaixaNegociacao parcela = new Dominio.Entidades.Embarcador.Financeiro.TituloBaixaNegociacao();
                    parcela.DataEmissao = dataEmissao;

                    if (i == 0)
                        parcela.DataVencimento = dataPrimeiroVencimento;
                    else
                        parcela.DataVencimento = dataUltimaParcela.AddDays(intervaloDeDias);

                    dataUltimaParcela = parcela.DataVencimento;
                    parcela.TituloBaixa = tituloBaixa;
                    parcela.Sequencia = i + 1;
                    parcela.SituacaoFaturaParcela = SituacaoFaturaParcela.EmAberto;

                    if (i == 0 && tipoArredondamento == TipoArredondamento.Primeira)
                    {
                        parcela.Valor = valorParcela + valorDiferenca;
                        parcela.Desconto = valorParcelaDesconto + valorDiferencaDesconto;
                        parcela.Acrescimo = valorParcelaAcrescimo + valorDiferencaAcrescimo;
                    }
                    else if ((i + 1) == quantidadeParcelas && tipoArredondamento == TipoArredondamento.Ultima)
                    {
                        parcela.Valor = valorParcela + valorDiferenca;
                        parcela.Desconto = valorParcelaDesconto + valorDiferencaDesconto;
                        parcela.Acrescimo = valorParcelaAcrescimo + valorDiferencaAcrescimo;
                    }
                    else
                    {
                        parcela.Valor = valorParcela;
                        parcela.Desconto = valorParcelaDesconto;
                        parcela.Acrescimo = valorParcelaAcrescimo;
                    }

                    parcela.FormaParcela = formaTitulo;
                    parcela.NumeroBoleto = string.Empty;
                    if (tituloBaixa.ValorMoedaCotacao > 0)
                        parcela.ValorOriginalMoedaEstrangeira = Math.Round(parcela.Valor / tituloBaixa.ValorMoedaCotacao, 2);

                    repTituloBaixaNegociacao.Inserir(parcela);
                }

                Servicos.Auditoria.Auditoria.Auditar(Auditado, tituloBaixa, null, "Gerou " + quantidadeParcelas.ToString() + " Parcelas(s).", unitOfWork);

                repTituloBaixa.Atualizar(tituloBaixa);

                return new JsonpResult(true, "Sucesso");
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao gerar as parcelas.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> CarregarDadosParcela()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Financeiro.TituloBaixaNegociacao repTituloBaixaNegociacao = new Repositorio.Embarcador.Financeiro.TituloBaixaNegociacao(unitOfWork);

                int.TryParse(Request.Params("Codigo"), out int codigoParcela);
                Dominio.Entidades.Embarcador.Financeiro.TituloBaixaNegociacao parcela = repTituloBaixaNegociacao.BuscarPorCodigo(codigoParcela);

                var dynRetorno = new
                {
                    parcela.Codigo,
                    Sequencia = parcela.Sequencia.ToString("n0"),
                    Valor = parcela.Valor.ToString("n2"),
                    ValorDesconto = parcela.Desconto.ToString("n2"),
                    DataEmissao = parcela.DataEmissao.ToString("dd/MM/yyyy"),
                    DataVencimento = parcela.DataVencimento.ToString("dd/MM/yyyy"),
                    FormaTitulo = parcela.FormaParcela,
                    parcela.NumeroBoleto,
                    Portador = parcela.Portador != null ? new { parcela.Portador.Codigo, parcela.Portador.Descricao } : null,
                    ValorOriginalMoedaEstrangeira = parcela.ValorOriginalMoedaEstrangeira.ToString("n2"),
                    Desconto = parcela.Desconto.ToString("n2"),
                    Acrescimo = parcela.Acrescimo.ToString("n2"),
                    Total = (parcela.Valor + parcela.Acrescimo - parcela.Desconto).ToString("n2")
                };

                return new JsonpResult(dynRetorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar os dados da parcela.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AtualizarParcela()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Financeiro.TituloBaixaNegociacao repTituloBaixaNegociacao = new Repositorio.Embarcador.Financeiro.TituloBaixaNegociacao(unitOfWork);
                Repositorio.Embarcador.Financeiro.TituloBaixaAcrescimo repTituloBaixaAcrescimo = new Repositorio.Embarcador.Financeiro.TituloBaixaAcrescimo(unitOfWork);
                Repositorio.Embarcador.Financeiro.TituloBaixa repTituloBaixa = new Repositorio.Embarcador.Financeiro.TituloBaixa(unitOfWork);
                Repositorio.Embarcador.Financeiro.TituloBaixaAgrupado repTituloBaixaAgrupado = new Repositorio.Embarcador.Financeiro.TituloBaixaAgrupado(unitOfWork);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);

                int.TryParse(Request.Params("CodigoParcela"), out int codigoParcela);
                int.TryParse(Request.Params("Codigo"), out int codigoBaixa);
                int.TryParse(Request.Params("Sequencia"), out int sequencia);

                decimal.TryParse(Request.Params("Valor"), out decimal valor);
                decimal.TryParse(Request.Params("ValorDesconto"), out decimal valorDesconto);
                decimal.TryParse(Request.Params("ValorOriginalMoedaEstrangeira"), out decimal valorOriginalMoedaEstrangeira);

                double.TryParse(Request.Params("Portador"), out double codigoPortador);

                string numeroBoleto = Request.Params("NumeroBoleto");

                DateTime.TryParse(Request.Params("DataVencimento"), out DateTime dataVencimento);
                DateTime.TryParse(Request.Params("DataEmissao"), out DateTime dataEmissao);

                Enum.TryParse(Request.Params("FormaTitulo"), out FormaTitulo formaTitulo);

                unitOfWork.Start();

                List<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado> titulosAgrupados = repTituloBaixaAgrupado.BuscarPorBaixaTitulo(codigoBaixa);
                DateTime dataEmissaoTitulo = DateTime.MinValue;
                DateTime dataEmissaoAux = DateTime.MinValue;

                for (int i = 0; i < titulosAgrupados.Count(); i++)
                {
                    Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado tituloAgrupado = new Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado();
                    dataEmissaoTitulo = titulosAgrupados[i].Titulo.DataEmissao.Value;
                    if (dataEmissaoTitulo > dataEmissaoAux)
                        dataEmissaoAux = dataEmissaoTitulo;
                }
                if (dataEmissaoAux == DateTime.MinValue)
                    dataEmissaoAux = DateTime.Now;

                if (dataEmissao.Date < dataEmissaoAux.Date || dataEmissao.Date > DateTime.Now.Date)
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, "A data de emissão não pode ser inferior que a maior data dos títulos selecionados (" + dataEmissaoAux.ToString("dd/MM/yyyy") + "), e também não pode ser superior que a data atual.");
                }

                Dominio.Entidades.Embarcador.Financeiro.TituloBaixaNegociacao parcela = repTituloBaixaNegociacao.BuscarPorCodigo(codigoParcela);

                if (!string.IsNullOrWhiteSpace(numeroBoleto))
                {
                    if (repTitulo.ContemTituloNossoNumeroDuplicado(parcela.CodigoTitulo > 0 ? parcela.CodigoTitulo : 0, numeroBoleto))
                        return new JsonpResult(false, false, "Já existe um título a pagar lançado com o mesmo número de boleto para o pagamento eletrônico.");
                }

                parcela.Valor = valor;
                parcela.Desconto = valorDesconto;
                parcela.DataVencimento = dataVencimento;
                parcela.FormaParcela = formaTitulo;
                parcela.NumeroBoleto = numeroBoleto;
                parcela.Portador = codigoPortador > 0 ? repCliente.BuscarPorCPFCNPJ(codigoPortador) : null;
                parcela.ValorOriginalMoedaEstrangeira = valorOriginalMoedaEstrangeira;

                repTituloBaixaNegociacao.Atualizar(parcela);

                Dominio.Entidades.Embarcador.Financeiro.TituloBaixa tituloBaixa = repTituloBaixa.BuscarPorCodigo(codigoBaixa);

                decimal valorABaixar = tituloBaixa.Valor;
                decimal descontos = tituloBaixa != null ? repTituloBaixaAcrescimo.TotalPorTituloBaixa(codigoBaixa, TipoJustificativa.Desconto) : 0;
                decimal acrescimos = tituloBaixa != null ? repTituloBaixaAcrescimo.TotalPorTituloBaixa(codigoBaixa, TipoJustificativa.Acrescimo) : 0;
                decimal valorTotal = Math.Round(tituloBaixa.ValorPendente - valorABaixar - descontos + acrescimos, 2);

                decimal valorParcelas = tituloBaixa.TitulosNegociacao != null ? (from p in tituloBaixa.TitulosNegociacao select p.Valor).Sum() : 0;
                valorParcelas = Math.Round(valorParcelas, 2);
                int qtdParcelas = tituloBaixa.TitulosNegociacao != null ? (from p in tituloBaixa.TitulosNegociacao select p).Count() : 0;

                if ((valorParcelas > 0) && (valorParcelas != valorTotal))
                {
                    decimal valorDiferenca = Math.Round(valorTotal - valorParcelas, 2);
                    int parcelasRatear = qtdParcelas - sequencia;
                    if (parcelasRatear > 0)
                    {
                        decimal valorRatearParcelas = Math.Round(valorDiferenca / parcelasRatear, 2);

                        for (int i = sequencia; i < tituloBaixa.TitulosNegociacao.Count(); i++)
                        {
                            tituloBaixa.TitulosNegociacao[i].Valor = tituloBaixa.TitulosNegociacao[i].Valor + valorRatearParcelas;
                            repTituloBaixaNegociacao.Atualizar(tituloBaixa.TitulosNegociacao[i]);
                        }
                    }

                    valorTotal = Math.Round(tituloBaixa.ValorPendente - valorABaixar - descontos + acrescimos, 2);
                    valorParcelas = tituloBaixa.TitulosNegociacao != null ? (from p in tituloBaixa.TitulosNegociacao select p.Valor).Sum() : 0;
                    valorDiferenca = Math.Round(valorTotal - valorParcelas, 2);
                    if (valorDiferenca != 0 && tituloBaixa.TitulosNegociacao.Count() > sequencia)
                    {
                        tituloBaixa.TitulosNegociacao[sequencia + 1].Valor = tituloBaixa.TitulosNegociacao[sequencia + 1].Valor + valorDiferenca;
                        repTituloBaixaNegociacao.Atualizar(tituloBaixa.TitulosNegociacao[sequencia + 1]);
                    }
                }

                Servicos.Auditoria.Auditoria.Auditar(Auditado, tituloBaixa, null, "Alterou a Parcela " + parcela.Sequencia.ToString() + ".", unitOfWork);

                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar os valores da parcela.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> FecharBaixa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Financeiros/BaixaTituloPagar");
                if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    if (!(this.Usuario.UsuarioAdministrador || permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.BaixaPagar_PermiteFecharBaixa)))
                        return new JsonpResult(false, "Seu usuário não possui permissão para fechar a baixa do título a pagar.");

                Servicos.Embarcador.Financeiro.ProcessoMovimento servProcessoMovimento = new Servicos.Embarcador.Financeiro.ProcessoMovimento(_conexao.StringConexao);
                Servicos.Embarcador.Financeiro.BaixaTituloPagar servBaixaTituloPagar = new Servicos.Embarcador.Financeiro.BaixaTituloPagar(unitOfWork);

                Repositorio.Embarcador.Financeiro.TituloBaixaNegociacao repTituloBaixaNegociacao = new Repositorio.Embarcador.Financeiro.TituloBaixaNegociacao(unitOfWork);
                Repositorio.Embarcador.Financeiro.TituloBaixaAcrescimo repTituloBaixaAcrescimo = new Repositorio.Embarcador.Financeiro.TituloBaixaAcrescimo(unitOfWork);
                Repositorio.Embarcador.Financeiro.TituloBaixa repTituloBaixa = new Repositorio.Embarcador.Financeiro.TituloBaixa(unitOfWork);
                Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);
                Repositorio.Embarcador.Financeiro.TituloBaixaTipoPagamentoRecebimento repTituloBaixaTipoPagamentoRecebimento = new Repositorio.Embarcador.Financeiro.TituloBaixaTipoPagamentoRecebimento(unitOfWork);
                Repositorio.Cliente repPessoa = new Repositorio.Cliente(unitOfWork);
                Repositorio.Embarcador.Financeiro.TituloBaixaCheque repTituloBaixaCheque = new Repositorio.Embarcador.Financeiro.TituloBaixaCheque(unitOfWork);
                Repositorio.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto repContratoFreteAcrescimoDesconto = new Repositorio.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro repConfiguracaoFinanceiro = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");

                double codigoPessoaNegociacao = Request.GetDoubleParam("PessoaNegociacao");
                Enum.TryParse(Request.Params("Etapa"), out SituacaoBaixaTitulo etapa);

                Dominio.Entidades.Embarcador.Financeiro.TituloBaixa tituloBaixa = repTituloBaixa.BuscarPorCodigo(codigo);

                if (tituloBaixa.Valor > 0)
                {
                    if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe && !this.Usuario.UsuarioAdministrador)
                        if (permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.BaixaPagar_NaoPermiteQuitarTitulo))
                            return new JsonpResult(false, "Seu usuário não possui permissão para quitar um título na baixa a pagar.");
                }

                decimal valorABaixar = tituloBaixa.Valor;
                decimal descontos = repTituloBaixaAcrescimo.TotalPorTituloBaixa(codigo, TipoJustificativa.Desconto);
                decimal acrescimos = repTituloBaixaAcrescimo.TotalPorTituloBaixa(codigo, TipoJustificativa.Acrescimo);
                decimal valorPendente = tituloBaixa.ValorPendente - valorABaixar - descontos + acrescimos;

                if (valorPendente > 0)
                {
                    if (codigoPessoaNegociacao > 0 && (tituloBaixa.TitulosNegociacao == null || tituloBaixa.TitulosNegociacao.Count() <= 0))
                        return new JsonpResult(false, "Por favor gere as parcelas de negociação com o saldo pendente antes de finalizar a baixa.");
                    else if (tituloBaixa.Pessoa != null && (tituloBaixa.TitulosNegociacao == null || tituloBaixa.TitulosNegociacao.Count() <= 0))
                        return new JsonpResult(false, "Por favor gere as parcelas de negociação com o saldo pendente antes de finalizar a baixa.");
                    else if (tituloBaixa.Pessoa == null && codigoPessoaNegociacao <= 0)
                        return new JsonpResult(false, "Por favor verifique o saldo pendente na baixa de títulos agrupada com diferentes fornecedores.");

                    decimal valorTotal = valorPendente;
                    decimal valorParcelas = tituloBaixa.TitulosNegociacao != null ? (from p in tituloBaixa.TitulosNegociacao select p.Valor).Sum() + acrescimos - descontos : 0;
                    decimal valorDiferenca = Math.Round(valorTotal - valorParcelas, 2);
                    if (valorDiferenca != 0)
                        return new JsonpResult(false, "O total das parcelas não está de acordo com o total pendente do título. Valor pendente R$ " + valorTotal.ToString("n2") + " Valor das parcelas R$ " + valorParcelas.ToString("n2"));
                }
                else if (valorPendente < 0)
                    return new JsonpResult(false, "Por favor verifique o valor pendente pois não pode ser negativo.");

                if (repTitulo.ContemTitulosPagosBaixaTitulo(codigo))
                    return new JsonpResult(false, "Esta baixa de títulos já possui títulos quitados, impossível de fechar a mesma.");

                decimal valorTipoPagamento = repTituloBaixaTipoPagamentoRecebimento.TotalPorTituloBaixa(codigo);
                if (tituloBaixa.Valor > 0 && valorTipoPagamento > 0 && tituloBaixa.Valor < valorTipoPagamento)
                    return new JsonpResult(false, "A soma dos pagamentos ultrapassa o valor pago.");
                if (tituloBaixa.Valor > 0 && valorTipoPagamento > 0 && tituloBaixa.Valor < valorTipoPagamento)
                    return new JsonpResult(false, "A soma dos pagamentos é inferior ao valor pago.");

                bool obrigaCheque = repTituloBaixaTipoPagamentoRecebimento.ObrigaChequeBaixaTitulo(codigo);
                bool temCheque = repTituloBaixaCheque.PossuiChequeBaixaTitulo(codigo);
                if (obrigaCheque && !temCheque)
                    return new JsonpResult(false, "Favor informar o(s) cheque(s) utilizado para o pagamento.");

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && repContratoFreteAcrescimoDesconto.AcrescimoDescontoContratoEmAberto(codigo))
                    return new JsonpResult(false, "Essa baixa possui título(s) em que o contrato de frete está com Acréscimo/Desconto em aberto! Não sendo possível quitar até o procedimento ser concluído.");

                if (tituloBaixa.TitulosNegociacao != null && tituloBaixa.TitulosNegociacao.Count() > 0)
                {
                    foreach (var parcela in tituloBaixa.TitulosNegociacao)
                    {
                        if (!string.IsNullOrWhiteSpace(parcela.NumeroBoleto))
                        {
                            if (repTitulo.ContemTituloNossoNumeroDuplicado(parcela.CodigoTitulo > 0 ? parcela.CodigoTitulo : 0, parcela.NumeroBoleto))
                                return new JsonpResult(false, $"Já existe um título a pagar lançado com o mesmo número de boleto da parcela {parcela.Sequencia}.");
                        }
                    }
                }

                unitOfWork.Start();

                tituloBaixa.SituacaoBaixaTitulo = etapa;

                if (codigoPessoaNegociacao > 0)
                    tituloBaixa.Pessoa = repPessoa.BuscarPorCPFCNPJ(codigoPessoaNegociacao);

                List<Dominio.Entidades.Embarcador.Financeiro.Titulo> listaTitulos = repTitulo.BuscarPorBaixaTitulo(codigo);
                for (int i = 0; i < listaTitulos.Count(); i++)
                    repTitulo.Deletar(listaTitulos[i]);

                List<Dominio.Entidades.Embarcador.Financeiro.Titulo> listaTitulo = repTituloBaixa.BuscarTitulosPorCodigo(codigo);

                decimal valorDesconto = Math.Round(descontos, 2), valorAcrescimo = Math.Round(acrescimos, 2);

                GerarTitulosNegociacao(tituloBaixa, listaTitulo, codigoPessoaNegociacao, valorDesconto, unitOfWork);

                valorABaixar = Math.Round(valorABaixar, 2);
                decimal valorTotalTitulos = listaTitulo.Sum(o => o.Saldo);

                decimal totalValorPago = 0, totalDesconto = 0, totalAcrescimo = 0;
                int codigoUltimoTitulo = 0;
                for (int i = 0; i < listaTitulo.Count; i++)
                {
                    Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = listaTitulo[i];

                    if (titulo.DataEmissao.Value.Date > tituloBaixa.DataBaixa.Value.Date)
                        throw new ControllerException("O título " + titulo.Codigo.ToString() + " possui a data de emissão maior que a data da baixa.");
                    else if (titulo.DataAutorizacao.HasValue && titulo.DataAutorizacao.Value.Date > tituloBaixa.DataBaixa.Value.Date && valorABaixar > 0)
                        throw new ControllerException("O título " + titulo.Codigo.ToString() + " possui a data de autorização maior que a data da baixa.");

                    titulo.DataLiquidacao = tituloBaixa.DataBaixa;
                    titulo.DataBaseLiquidacao = tituloBaixa.DataBaixa;
                    titulo.Provisao = false;

                    if (valorABaixar > 0)
                    {
                        titulo.ValorPago = Math.Round(((valorABaixar * titulo.Saldo) / valorTotalTitulos), 2);
                        totalValorPago += titulo.ValorPago;
                    }
                    else
                        titulo.ValorPago = 0;

                    if (valorDesconto > 0)
                    {
                        totalDesconto += Math.Round(((valorDesconto * titulo.Saldo) / valorTotalTitulos), 2);
                        titulo.Desconto += Math.Round(((valorDesconto * titulo.Saldo) / valorTotalTitulos), 2);
                    }
                    else
                        titulo.Desconto += 0;

                    if (valorAcrescimo > 0)
                    {
                        totalAcrescimo += Math.Round(((valorAcrescimo * titulo.Saldo) / valorTotalTitulos), 2);
                        titulo.Acrescimo += Math.Round(((valorAcrescimo * titulo.Saldo) / valorTotalTitulos), 2);
                    }
                    else
                        titulo.Acrescimo += 0;

                    titulo.ValorPendente = 0;

                    if (titulo.ValorPendente == 0)
                        titulo.StatusTitulo = StatusTitulo.Quitada;
                    else if (valorPendente > 0)
                    {
                        titulo.ValorPendente = 0;
                        titulo.StatusTitulo = StatusTitulo.Quitada;
                    }

                    titulo.NossoNumero = "";

                    titulo.DataAlteracao = DateTime.Now;
                    codigoUltimoTitulo = titulo.Codigo;
                    repTitulo.Atualizar(titulo);
                }
                if (valorABaixar > 0m && totalValorPago != valorABaixar)
                {
                    decimal valorDiferenca = Math.Round((valorABaixar - totalValorPago), 2);
                    if (valorDiferenca != 0m)
                    {
                        Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = repTitulo.BuscarPorCodigo(codigoUltimoTitulo);
                        titulo.ValorPago = Math.Round((titulo.ValorPago + (valorDiferenca)), 2);
                        repTitulo.Atualizar(titulo);
                    }
                }
                if (valorDesconto > 0m && totalDesconto != valorDesconto)
                {
                    decimal valorDiferenca = Math.Round((valorDesconto - totalDesconto), 2);
                    if (valorDiferenca != 0m)
                    {
                        Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = repTitulo.BuscarPorCodigo(codigoUltimoTitulo);
                        titulo.Desconto = Math.Round((titulo.Desconto + (valorDiferenca)), 2);
                        repTitulo.Atualizar(titulo);
                    }
                }
                if (valorAcrescimo > 0m && totalAcrescimo != valorAcrescimo)
                {
                    decimal valorDiferenca = Math.Round((valorAcrescimo - totalAcrescimo), 2);
                    if (valorDiferenca != 0m)
                    {
                        Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = repTitulo.BuscarPorCodigo(codigoUltimoTitulo);
                        titulo.Acrescimo = Math.Round((titulo.Acrescimo + (valorDiferenca)), 2);
                        repTitulo.Atualizar(titulo);
                    }
                }

                string erro = "";
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                {
                    if (ConfiguracaoEmbarcador.GerarMovimentacaoNaBaixaIndividualmente)
                    {
                        if (!servBaixaTituloPagar.GeraReverteMovimentacaoFinanceiraIndividual(out erro, codigo, unitOfWork, _conexao.StringConexao, TipoServicoMultisoftware, false, null))
                            throw new ControllerException(erro);
                    }
                    else if (!servBaixaTituloPagar.GeraReverteMovimentacaoFinanceira(out erro, codigo, unitOfWork, _conexao.StringConexao, TipoServicoMultisoftware, false, null))
                        throw new ControllerException(erro);
                }
                else
                {
                    Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiro configuracaoFinanceiro = repConfiguracaoFinanceiro.BuscarConfiguracaoPadrao();

                    if (configuracaoFinanceiro.AtivarControleDespesas)
                        servBaixaTituloPagar.GeraReverteMovimentacaoFinanceiraControleDespesas(codigo, unitOfWork, TipoServicoMultisoftware, false);
                    else if (ConfiguracaoEmbarcador.GerarMovimentacaoNaBaixaIndividualmente)
                    {
                        if (!servBaixaTituloPagar.GeraReverteMovimentacaoFinanceiraIndividual(out erro, codigo, unitOfWork, _conexao.StringConexao, TipoServicoMultisoftware, false, null))
                            throw new ControllerException(erro);
                    }
                    else if (!servBaixaTituloPagar.GeraReverteMovimentacaoFinanceira(out erro, codigo, unitOfWork, _conexao.StringConexao, TipoServicoMultisoftware, false, null))
                        throw new ControllerException(erro);
                }

                GerarReverterVinculoTituloDocumentoAcrescimoDesconto(tituloBaixa, unitOfWork);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, tituloBaixa, null, "Fechou Baixa.", unitOfWork);

                unitOfWork.CommitChanges();

                if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe && this.Usuario.Empresa != null && !string.IsNullOrWhiteSpace(this.Usuario.Empresa.EmailAdministrativo))
                    servBaixaTituloPagar.GeraIntegracaoBaixaTituloPagar(codigo, this.Usuario.Nome, this.Usuario.Empresa.EmailAdministrativo, unitOfWork, false);
                dynamic dynRetorno = servBaixaTituloPagar.RetornaObjetoCompletoTitulo(codigo, unitOfWork);

                return new JsonpResult(dynRetorno, true, "Sucesso");
            }
            catch (BaseException ex)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, ex.Message);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao fechar a baixa do título.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> CancelarBaixa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Financeiros/BaixaTituloPagar");
                if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    if (!(this.Usuario.UsuarioAdministrador || permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.BaixaPagar_PermiteCancelarBaixa)))
                        return new JsonpResult(false, "Seu usuário não possui permissão para cancelar a baixa do título a pagar.");

                Servicos.Embarcador.Financeiro.BaixaTituloPagar servBaixaTituloPagar = new Servicos.Embarcador.Financeiro.BaixaTituloPagar(unitOfWork);

                Repositorio.Embarcador.Financeiro.TituloBaixa repTituloBaixa = new Repositorio.Embarcador.Financeiro.TituloBaixa(unitOfWork);
                Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro repConfiguracaoFinanceiro = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");

                Dominio.Entidades.Embarcador.Financeiro.TituloBaixa tituloBaixa = repTituloBaixa.BuscarPorCodigo(codigo);

                if (codigo <= 0)
                    return new JsonpResult(false, "Não é possível cancelar uma baixa sem ter iniciado a mesma.");

                if (tituloBaixa.SituacaoBaixaTitulo == SituacaoBaixaTitulo.Cancelada)
                    return new JsonpResult(false, "Esta baixa já se encontra cancelada.");

                if (repTituloBaixa.ContemParcelaQuitada(codigo))
                    return new JsonpResult(false, "Esta baixa já possui parcela de negociação quitada.");

                if (repTituloBaixa.ContemTitulosNegociadosEmOutraBaixa(codigo))
                    return new JsonpResult(false, "Há parcelas de negociação em outra baixa.");

                if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe && !this.Usuario.UsuarioAdministrador)
                    if (permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.BaixaPagar_NaoPermitirCancelarBaixaComPagamentoEletronico) && repTituloBaixa.ContemTitulosComPagamentoEletronico(codigo))
                        return new JsonpResult(false, "Seu usuário não possui permissão para cancelar baixa a pagar com títulos provenientes de Pagamento Eletrônico.");

                List<Dominio.Entidades.Embarcador.Financeiro.Titulo> listaTitulos = repTitulo.BuscarPorBaixaTitulo(codigo);
                List<Dominio.Entidades.Embarcador.Financeiro.Titulo> listaTitulo = repTituloBaixa.BuscarTitulosPorCodigo(codigo);

                unitOfWork.Start();

                string erro = "";
                if (tituloBaixa.SituacaoBaixaTitulo == SituacaoBaixaTitulo.Finalizada)
                {
                    if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    {
                        if (ConfiguracaoEmbarcador.GerarMovimentacaoNaBaixaIndividualmente)
                        {
                            if (!servBaixaTituloPagar.GeraReverteMovimentacaoFinanceiraIndividual(out erro, codigo, unitOfWork, _conexao.StringConexao, TipoServicoMultisoftware, true, tituloBaixa.TipoPagamentoRecebimento?.PlanoConta ?? null))
                                throw new ControllerException(erro);
                        }
                        else if (!servBaixaTituloPagar.GeraReverteMovimentacaoFinanceira(out erro, codigo, unitOfWork, _conexao.StringConexao, TipoServicoMultisoftware, true, tituloBaixa.TipoPagamentoRecebimento?.PlanoConta ?? null))
                            throw new ControllerException(erro);
                    }
                    else
                    {
                        Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiro configuracaoFinanceiro = repConfiguracaoFinanceiro.BuscarConfiguracaoPadrao();

                        if (configuracaoFinanceiro.AtivarControleDespesas)
                            servBaixaTituloPagar.GeraReverteMovimentacaoFinanceiraControleDespesas(codigo, unitOfWork, TipoServicoMultisoftware, true);
                        else if (ConfiguracaoEmbarcador.GerarMovimentacaoNaBaixaIndividualmente)
                        {
                            if (!servBaixaTituloPagar.GeraReverteMovimentacaoFinanceiraIndividual(out erro, codigo, unitOfWork, _conexao.StringConexao, TipoServicoMultisoftware, true, tituloBaixa.TipoPagamentoRecebimento?.PlanoConta ?? null))
                                throw new ControllerException(erro);
                        }
                        else if (!servBaixaTituloPagar.GeraReverteMovimentacaoFinanceira(out erro, codigo, unitOfWork, _conexao.StringConexao, TipoServicoMultisoftware, true, tituloBaixa.TipoPagamentoRecebimento?.PlanoConta ?? null))
                            throw new ControllerException(erro);
                    }
                }

                GerarReverterVinculoTituloDocumentoAcrescimoDesconto(tituloBaixa, unitOfWork, true);

                for (int i = 0; i < listaTitulos.Count; i++)
                {
                    Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = listaTitulos[i];

                    titulo.DataCancelamento = DateTime.Now.Date;
                    titulo.StatusTitulo = StatusTitulo.Cancelado;
                    titulo.DataAlteracao = DateTime.Now;
                    titulo.TituloBaixaNegociacao = null;

                    repTitulo.Atualizar(titulo);
                }

                for (int i = 0; i < listaTitulo.Count; i++)
                {
                    Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = listaTitulo[i];

                    titulo.DataLiquidacao = null;
                    titulo.DataBaseLiquidacao = null;
                    if (tituloBaixa.SituacaoBaixaTitulo == SituacaoBaixaTitulo.Finalizada)
                    {
                        titulo.Desconto = 0;
                        titulo.Acrescimo = 0;
                        titulo.ValorPago = 0;
                        titulo.ValorPendente = titulo.ValorOriginal;
                    }
                    titulo.StatusTitulo = StatusTitulo.EmAberto;
                    titulo.DataAlteracao = DateTime.Now;
                    repTitulo.Atualizar(titulo);
                }

                tituloBaixa.SituacaoBaixaTitulo = SituacaoBaixaTitulo.Cancelada;
                repTituloBaixa.Atualizar(tituloBaixa);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, tituloBaixa, null, "Cancelou Baixa.", unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true, "Sucesso");
            }
            catch (BaseException ex)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, ex.Message);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                if (ex.Message.Contains("Plano"))
                    return new JsonpResult(true, false, ex.Message);
                else
                    return new JsonpResult(false, "Ocorreu uma falha ao cancelar baixa.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> SalvarObservacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Servicos.Embarcador.Financeiro.BaixaTituloPagar servBaixaTituloPagar = new Servicos.Embarcador.Financeiro.BaixaTituloPagar(unitOfWork);

                Repositorio.Embarcador.Financeiro.TituloBaixa repTituloBaixa = new Repositorio.Embarcador.Financeiro.TituloBaixa(unitOfWork);

                unitOfWork.Start();

                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);
                Dominio.Entidades.Embarcador.Financeiro.TituloBaixa tituloBaixa;
                if (codigo > 0)
                {
                    tituloBaixa = repTituloBaixa.BuscarPorCodigo(codigo, true);
                    tituloBaixa.Observacao = Request.Params("Observacao");
                    repTituloBaixa.Atualizar(tituloBaixa, Auditado);
                }
                else
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, "Não é possível salvar a observação de uma baixa sem ter iniciado a mesma.");
                }

                unitOfWork.CommitChanges();

                var dynRetorno = servBaixaTituloPagar.RetornaObjetoCompletoTitulo(codigo, unitOfWork);
                return new JsonpResult(dynRetorno, true, "Sucesso");
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao salvar observacai baixa.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ConfiguracaoImportacao()
        {
            try
            {
                List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = ConfiguracaoImportacaoPlanilha();

                return new JsonpResult(configuracoes);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, true, "Ocorreu uma falha ao obter as configurações para importação.");
            }
        }

        public async Task<IActionResult> Importar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);
                Repositorio.Embarcador.Financeiro.DocumentoEntradaTMS repDocumentoEntrada = new Repositorio.Embarcador.Financeiro.DocumentoEntradaTMS(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao retornoImportacao = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao
                {
                    Retornolinhas = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha>()
                };

                List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = ConfiguracaoImportacaoPlanilha();

                string erro = string.Empty;
                int contador = 0;
                string dados = Request.Params("Dados");

                decimal valorPendente = 0m;

                List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha> linhas = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha>>(dados);
                int totalLinhas = linhas.Count;
                int contadorTitulosNaoEncontrados = 0;

                var colunasPlanilhas = linhas.Select(l => l.Colunas).ToList();
                var colCodigoTitulo = colunasPlanilhas.Where(o => o.Any(p => p.NomeCampo == "CodigoTitulo")).ToList();

                List<object> titulosRetornar = new List<object>();
                List<int> codigoDocumentos = new List<int>();
                if (colCodigoTitulo == null || colCodigoTitulo.Count == 0)
                {
                    for (int i = 0; i < totalLinhas; i++)
                    {
                        try
                        {
                            Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha linha = linhas[i];

                            Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colNumeroDocumento = linha.Colunas?.Where(o => o.NomeCampo == "NumeroDocumento").FirstOrDefault();
                            Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colDataEmissaoInicial = linha.Colunas?.Where(o => o.NomeCampo == "DataEmissaoInicial").FirstOrDefault();
                            Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colDataEmissaoFinal = linha.Colunas?.Where(o => o.NomeCampo == "DataEmissaoFinal").FirstOrDefault();
                            Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colCNPJFornecedor = linha.Colunas?.Where(o => o.NomeCampo == "CNPJFornecedor").FirstOrDefault();

                            DateTime.TryParseExact(colDataEmissaoInicial?.Valor ?? string.Empty, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataEmissaoInicial);
                            DateTime.TryParseExact(colDataEmissaoFinal?.Valor ?? string.Empty, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataEmissaoFinal);

                            double.TryParse(Utilidades.String.OnlyNumbers(colCNPJFornecedor?.Valor), out double cnpjFornecedor);

                            int.TryParse(Utilidades.String.OnlyNumbers(colNumeroDocumento?.Valor), out int numeroDocumento);
                            string numeroDocumentoStr = (string)colNumeroDocumento?.Valor;

                            if (numeroDocumento <= 0 && (dataEmissaoInicial == DateTime.MinValue || dataEmissaoFinal == DateTime.MinValue) && (colCodigoTitulo == null || colCodigoTitulo.Count == 0))
                            {
                                retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha("É necessário que o número do documento ou a data de emissão inicial e final sejam informados.", i));
                                continue;
                            }

                            List<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS> documentos = repDocumentoEntrada.BuscarPorFornecedorNumeroEPeriodo(numeroDocumento, cnpjFornecedor, dataEmissaoInicial, dataEmissaoFinal);

                            bool falha = false;

                            if (documentos.Count == 0)
                            {
                                List<Dominio.Entidades.Embarcador.Financeiro.Titulo> titulos = repTitulo.BuscarPorFornecedorNumeroEPeriodo(numeroDocumentoStr, cnpjFornecedor, dataEmissaoInicial, dataEmissaoFinal);

                                if (titulos.Count <= 0)
                                {
                                    retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha("Não foram encontrados títulos em aberto para o documento/fornecedor/data.", i));
                                    falha = true;
                                    continue;
                                }
                                else
                                    retornoImportacao.Retornolinhas.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha() { indice = i, processou = true, mensagemFalha = "" });

                                titulosRetornar.AddRange(titulos.Select(o => ObterDetalhesTituloGrid(o)));
                                valorPendente += titulos.Sum(o => o.ValorPendente);
                                contador++;
                            }
                            else
                            {

                                for (int k = 0; k < documentos.Count; k++)
                                {
                                    if (codigoDocumentos.Contains(documentos[k].Codigo))
                                    {
                                        retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha("O arquivo possui número de notas duplicadas.", i));
                                        continue;
                                    }
                                    else
                                        codigoDocumentos.Add(documentos[k].Codigo);
                                }

                                if (documentos.Any(o => o.Situacao != SituacaoDocumentoEntrada.Finalizado))
                                {
                                    retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha("Foram encontrados documentos ainda não finalizados (" + string.Join(", ", from obj in documentos where obj.Situacao != SituacaoDocumentoEntrada.Finalizado select obj.Numero) + ") com estas informações.", i));
                                    continue;
                                }

                                foreach (Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS documentoEntrada in documentos)
                                {
                                    List<Dominio.Entidades.Embarcador.Financeiro.Titulo> titulos = repTitulo.BuscarPorDocumentoEntrada(documentoEntrada.Codigo);

                                    if (titulos.Count <= 0)
                                    {
                                        retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha("Não foram encontrados títulos em aberto para o documento " + documentoEntrada.Numero.ToString() + ".", i));
                                        falha = true;
                                        break;
                                    }
                                    else
                                        falha = false;

                                    titulosRetornar.AddRange(titulos.Select(o => ObterDetalhesTituloGrid(o)));
                                    valorPendente += titulos.Sum(o => o.ValorPendente);
                                    contador++;
                                }

                                if (!falha)
                                    retornoImportacao.Retornolinhas.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha() { indice = i, processou = true, mensagemFalha = "" });
                            }
                        }
                        catch (Exception ex2)
                        {
                            Servicos.Log.TratarErro(ex2);
                            retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha("Ocorreu uma falha ao processar a linha.", i));
                            continue;
                        }
                    }
                }
                else
                {
                    bool falha = false;

                    List<int> codigosTitulos = new List<int>();
                    int indice = 0;
                    foreach (var codigoTitulo in colCodigoTitulo)
                    {
                        int.TryParse(Utilidades.String.OnlyNumbers(codigoTitulo.Where(o => o.NomeCampo == "CodigoTitulo").FirstOrDefault().Valor), out int codTitulo);
                        Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = repTitulo.BuscarPorCodigo(codTitulo);
                        if (titulo != null)
                        {
                            List<Dominio.Entidades.Embarcador.Financeiro.Titulo> tituloPendente = repTitulo.BuscarTituloPagarPendentePorCodigo(codTitulo);
                            if (tituloPendente == null || tituloPendente.Count <= 0)
                            {
                                retornoImportacao.Retornolinhas.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha() { indice = indice, processou = false, mensagemFalha = "Título não está em aberto!" });
                                contadorTitulosNaoEncontrados++;
                            }
                            else
                            {
                                codigosTitulos.Add(codTitulo);
                                retornoImportacao.Retornolinhas.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha() { indice = indice, processou = true, mensagemFalha = "" });
                                indice++;
                            }
                        }
                        else
                        {
                            retornoImportacao.Retornolinhas.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha() { indice = indice, processou = false, mensagemFalha = "Título não encontrado!" });
                            contadorTitulosNaoEncontrados++;
                        }
                    }

                    if (codigosTitulos == null || codigosTitulos.Count <= 0)
                        return new JsonpResult(true, false, "Não foram encontrados títulos em aberto para a planilha importada.");

                    List<Dominio.Entidades.Embarcador.Financeiro.Titulo> titulos = repTitulo.BuscarPorCodigos(codigosTitulos, TipoTitulo.Pagar, StatusTitulo.EmAberto);

                    if (titulos.Count <= 0)
                        return new JsonpResult(true, false, "Não foram encontrados títulos em aberto para a planilha importada.");

                    valorPendente = titulos.Sum(o => o.ValorPendente);
                    titulosRetornar.AddRange(titulos.Select(o => ObterDetalhesTituloGrid(o)));
                    if (!falha)
                        contador = linhas.Count() - contadorTitulosNaoEncontrados;
                }

                retornoImportacao.Retorno = new { ValorPendente = valorPendente.ToString("n2"), Titulos = titulosRetornar };
                retornoImportacao.Total = linhas.Count();
                retornoImportacao.Importados = contador;

                return new JsonpResult(retornoImportacao);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao importar arquivo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> CarregarDadosCheque()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Financeiro.TituloBaixaCheque repositorio = new Repositorio.Embarcador.Financeiro.TituloBaixaCheque(unitOfWork);
                List<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaCheque> listaCheque = repositorio.BuscarPorTituloBaixa(codigo);

                var listaChequeRetornar = (
                    from cheque in listaCheque
                    select new
                    {
                        cheque.Codigo,
                        cheque.Cheque.NumeroCheque,
                        Banco = cheque.Cheque.Banco.Descricao,
                        Pessoa = cheque.Cheque.Pessoa.Descricao,
                        Status = cheque.Cheque.Status.ObterDescricao(),
                        Tipo = cheque.Cheque.Tipo.ObterDescricao(),
                        Valor = cheque.Cheque.Valor.ToString("n2")
                    }
                ).ToList();

                return new JsonpResult(listaChequeRetornar);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar os cheques.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AdicionarDadosCheque()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Financeiro.TituloBaixa repositorioTituloBaixa = new Repositorio.Embarcador.Financeiro.TituloBaixa(unitOfWork);
                Dominio.Entidades.Embarcador.Financeiro.TituloBaixa tituloBaixa = repositorioTituloBaixa.BuscarPorCodigo(codigo);

                if (tituloBaixa == null)
                    throw new ControllerException("Negociação da baixa de títulos não encontrada.");

                if (!IsPermitirGerenciarCheque(tituloBaixa))
                    throw new ControllerException("Situação da baixa de títulos não permite adicionar cheques.");

                List<int> listaCodigoCheque = Request.GetListParam<int>("Cheques");

                if (listaCodigoCheque.Count == 0)
                    throw new ControllerException("Nenhum cheque selecionado.");

                Repositorio.Embarcador.Financeiro.Cheque repositorioCheque = new Repositorio.Embarcador.Financeiro.Cheque(unitOfWork);
                List<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaCheque> listaTituloBaixaChequeInserir = new List<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaCheque>();

                foreach (int codigoCheque in listaCodigoCheque)
                {
                    Dominio.Entidades.Embarcador.Financeiro.TituloBaixaCheque tituloBaixaCheque = new Dominio.Entidades.Embarcador.Financeiro.TituloBaixaCheque()
                    {
                        TituloBaixa = tituloBaixa,
                        Cheque = repositorioCheque.BuscarPorCodigo(codigoCheque) ?? throw new ControllerException("Cheque não encontrado.")
                    };

                    listaTituloBaixaChequeInserir.Add(tituloBaixaCheque);
                }

                unitOfWork.Start();

                Repositorio.Embarcador.Financeiro.TituloBaixaCheque repositorioTituloBaixaCheque = new Repositorio.Embarcador.Financeiro.TituloBaixaCheque(unitOfWork);

                foreach (Dominio.Entidades.Embarcador.Financeiro.TituloBaixaCheque tituloBaixaCheque in listaTituloBaixaChequeInserir)
                {
                    repositorioTituloBaixaCheque.Inserir(tituloBaixaCheque);
                }

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar os cheques.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ExcluirChequePorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Financeiro.TituloBaixaCheque repositorio = new Repositorio.Embarcador.Financeiro.TituloBaixaCheque(unitOfWork);
                Dominio.Entidades.Embarcador.Financeiro.TituloBaixaCheque tituloBaixaCheque = repositorio.BuscarPorCodigo(codigo);

                if (tituloBaixaCheque == null)
                    throw new ControllerException("Cheque não encontrado.");

                if (!IsPermitirGerenciarCheque(tituloBaixaCheque.TituloBaixa))
                    throw new ControllerException("Situação da baixa de títulos não permite remover cheque.");

                repositorio.Deletar(tituloBaixaCheque);

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao deletar o cheque.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AdicionarTitulo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<int> codigosTitulos = Request.GetListParam<int>("Titulos");
                int codigoBaixa = Request.GetIntParam("BaixaTituloPagar");

                Servicos.Embarcador.Financeiro.BaixaTituloPagar svcBaixaTituloPagar = new Servicos.Embarcador.Financeiro.BaixaTituloPagar(unitOfWork);

                Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                Repositorio.Embarcador.Financeiro.TituloBaixaAgrupado repTituloBaixaAgrupado = new Repositorio.Embarcador.Financeiro.TituloBaixaAgrupado(unitOfWork);
                Repositorio.Embarcador.Financeiro.TituloBaixa repTituloBaixa = new Repositorio.Embarcador.Financeiro.TituloBaixa(unitOfWork);
                Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);

                List<Dominio.Entidades.Embarcador.Financeiro.Titulo> titulos = repTitulo.BuscarPorCodigos(codigosTitulos);
                Dominio.Entidades.Embarcador.Financeiro.TituloBaixa tituloBaixa = repTituloBaixa.BuscarPorCodigo(codigoBaixa);

                if (tituloBaixa == null)
                    return new JsonpResult(false, true, "Baixa não encontrada.");

                if (tituloBaixa.SituacaoBaixaTitulo != SituacaoBaixaTitulo.Iniciada)
                    return new JsonpResult(false, true, "A situação da baixa não permite que seja adicionado um título.");

                if (titulos.Count == 0)
                    return new JsonpResult(false, true, "Títulos não encontrados.");

                unitOfWork.Start();

                foreach (Dominio.Entidades.Embarcador.Financeiro.Titulo titulo in titulos)
                {
                    if (titulo.TipoTitulo != TipoTitulo.Pagar)
                        throw new ControllerException($"Tipo de título {titulo.Codigo} é inválido.");

                    if (titulo.StatusTitulo != StatusTitulo.EmAberto)
                        throw new ControllerException($"Situação do título {titulo.Codigo} é inválida.");

                    if (titulo.Baixas.Any(o => o.TituloBaixa.SituacaoBaixaTitulo != SituacaoBaixaTitulo.Cancelada))
                        throw new ControllerException($"O título {titulo.Codigo} já se encontra em outra baixa.");

                    if (tituloBaixa.Valor == tituloBaixa.ValorPendente)
                    {
                        tituloBaixa.Valor += titulo.Saldo;
                        repTituloBaixa.Atualizar(tituloBaixa);
                    }

                    Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado tituloAgrupado = new Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado
                    {
                        TituloBaixa = tituloBaixa,
                        Titulo = titulo,
                        DataBaixa = tituloBaixa.DataBaixa.Value,
                        DataBase = tituloBaixa.DataBaixa.Value
                    };

                    repTituloBaixaAgrupado.Inserir(tituloAgrupado);

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, tituloBaixa, $"Adicionou o título {titulo.Codigo} manualmente.", unitOfWork);
                }

                List<double> tomadores = repTituloBaixaAgrupado.BuscarTomadoresPorTituloBaixa(tituloBaixa.Codigo);
                List<int> grupoPessoas = repTituloBaixaAgrupado.BuscarGrupoPessoasPorTituloBaixa(tituloBaixa.Codigo);

                if (tomadores.Count == 1)
                    tituloBaixa.Pessoa = repCliente.BuscarPorCPFCNPJ(tomadores[0]);
                else
                    tituloBaixa.Pessoa = null;

                if (grupoPessoas.Count == 1)
                    tituloBaixa.GrupoPessoas = repGrupoPessoas.BuscarPorCodigo(grupoPessoas[0]);
                else
                    tituloBaixa.GrupoPessoas = null;

                repTituloBaixa.Atualizar(tituloBaixa);

                unitOfWork.CommitChanges();

                unitOfWork.Clear(tituloBaixa);

                return new JsonpResult(new
                {
                    Detalhes = svcBaixaTituloPagar.RetornaObjetoCompletoTitulo(tituloBaixa.Codigo, unitOfWork)
                });
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
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar o título.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> RemoverTitulo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoTitulo = Request.GetIntParam("Titulo");
                int codigoBaixa = Request.GetIntParam("BaixaTituloPagar");

                Servicos.Embarcador.Financeiro.BaixaTituloPagar svcBaixaTituloPagar = new Servicos.Embarcador.Financeiro.BaixaTituloPagar(unitOfWork);

                Repositorio.Embarcador.Financeiro.TituloBaixaAgrupado repTituloBaixaAgrupado = new Repositorio.Embarcador.Financeiro.TituloBaixaAgrupado(unitOfWork);
                Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                Repositorio.Embarcador.Financeiro.TituloBaixa repTituloBaixa = new Repositorio.Embarcador.Financeiro.TituloBaixa(unitOfWork);

                Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado tituloBaixaAgrupado = repTituloBaixaAgrupado.BuscarPorTituloEBaixa(codigoBaixa, codigoTitulo);
                Dominio.Entidades.Embarcador.Financeiro.TituloBaixa tituloBaixa = repTituloBaixa.BuscarPorCodigo(codigoBaixa);

                if (tituloBaixaAgrupado == null)
                    return new JsonpResult(false, true, "Título não encontrado.");

                if (tituloBaixaAgrupado.Titulo.TipoTitulo != TipoTitulo.Pagar)
                    return new JsonpResult(false, true, "Tipo de título inválido.");

                if (tituloBaixaAgrupado.Titulo.StatusTitulo != StatusTitulo.EmAberto)
                    return new JsonpResult(false, true, "Situação do título inválida.");

                if (tituloBaixa == null)
                    return new JsonpResult(false, true, "Baixa não encontrada.");

                if (tituloBaixa.SituacaoBaixaTitulo != SituacaoBaixaTitulo.Iniciada)
                    return new JsonpResult(false, true, "A situação da baixa não permite que seja removido um título.");

                unitOfWork.Start();

                if (tituloBaixa.Valor == tituloBaixa.ValorPendente)
                {
                    tituloBaixa.Valor -= tituloBaixaAgrupado.Titulo.Saldo;

                    if (tituloBaixa.Valor < 0m)
                        tituloBaixa.Valor = 0m;
                }

                repTituloBaixaAgrupado.Deletar(tituloBaixaAgrupado);

                List<double> tomadores = repTituloBaixaAgrupado.BuscarTomadoresPorTituloBaixa(tituloBaixa.Codigo);
                List<int> grupoPessoas = repTituloBaixaAgrupado.BuscarGrupoPessoasPorTituloBaixa(tituloBaixa.Codigo);

                if (tomadores.Count == 1)
                    tituloBaixa.Pessoa = repCliente.BuscarPorCPFCNPJ(tomadores[0]);
                else
                    tituloBaixa.Pessoa = null;

                if (grupoPessoas.Count == 1)
                    tituloBaixa.GrupoPessoas = repGrupoPessoas.BuscarPorCodigo(grupoPessoas[0]);
                else
                    tituloBaixa.GrupoPessoas = null;

                repTituloBaixa.Atualizar(tituloBaixa);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, tituloBaixa, $"Removeu o título {codigoTitulo} manualmente.", unitOfWork);

                unitOfWork.CommitChanges();

                unitOfWork.Clear(tituloBaixa);

                return new JsonpResult(new
                {
                    Detalhes = svcBaixaTituloPagar.RetornaObjetoCompletoTitulo(tituloBaixa.Codigo, unitOfWork)
                });
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao remover o título.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaBaixaTituloPendente ObterFiltrosPesquisaTituloPendente(Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaBaixaTituloPendente filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaBaixaTituloPendente()
            {
                DataEmissaoInicial = Request.GetDateTimeParam("DataInicial"),
                DataEmissaoFinal = Request.GetDateTimeParam("DataFinal"),
                DataVencimentoInicial = Request.GetDateTimeParam("DataInicialVencimento"),
                DataVencimentoFinal = Request.GetDateTimeParam("DataFinalVencimento"),
                DataAutorizacaoInicial = Request.GetDateTimeParam("AutorizacaoInicial"),
                DataAutorizacaoFinal = Request.GetDateTimeParam("AutorizacaoFinal"),
                ValorInicial = Request.GetDecimalParam("ValorInicial"),
                ValorFinal = Request.GetDecimalParam("ValorFinal"),
                NumeroTitulo = Request.GetIntParam("NumeroTitulo"),
                CodigoBaixa = Request.GetIntParam("Codigo"),
                CodigoGrupoPessoa = Request.GetIntParam("GrupoPessoa"),
                CodigoNaturezaOperacaoEntrada = Request.GetIntParam("NaturezaOperacaoEntrada"),
                CnpjPessoa = Request.GetDoubleParam("Pessoa"),
                NumeroDocumento = Request.GetStringParam("NumeroDocumento"),
                SomenteTitulosDeNegociacao = Request.GetEnumParam<Dominio.Enumeradores.OpcaoSimNao>("TitulosDeAgrupamento"),
                FormaTitulo = Request.GetEnumParam<FormaTitulo>("FormaTitulo"),
                TipoTitulo = TipoTitulo.Pagar,
                TipoServico = TipoServicoMultisoftware,
                TipoAmbiente = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe ? this.Usuario.Empresa.TipoAmbiente : Dominio.Enumeradores.TipoAmbiente.Nenhum,
                CodigoEmpresa = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe ? this.Usuario.Empresa.Codigo : 0,
                RaizCnpjPessoa = Request.GetStringParam("RaizCnpjPessoa").ObterSomenteNumeros(),
                CnpjCpfPortador = Request.GetDoubleParam("Portador"),
                CodigoTipoMovimento = Request.GetIntParam("TipoMovimento")
            };

            int codigoDocumentoEntrada = Request.GetIntParam("DocumentoEntrada");
            if (codigoDocumentoEntrada > 0)
            {
                Repositorio.Embarcador.Financeiro.DocumentoEntradaTMS repDocumentoEntradaTMS = new Repositorio.Embarcador.Financeiro.DocumentoEntradaTMS(unitOfWork);
                filtrosPesquisa.DocumentoEntrada = repDocumentoEntradaTMS.BuscarPorCodigo(codigoDocumentoEntrada);
            }

            return filtrosPesquisa;
        }

        private string ObterPropriedadeOrdenarTituloPendente(string propOrdenar)
        {
            if (propOrdenar == "CodigoTitulo")
                propOrdenar = "Codigo";
            else if (propOrdenar == "NumeroParcela")
                propOrdenar = "Sequencia";
            else if (propOrdenar == "Valor")
                propOrdenar = "(ValorOriginal - Desconto + Acrescimo)";
            else if (propOrdenar == "Pessoa")
                propOrdenar = "Pessoa.Nome";
            else if (propOrdenar == "NumeroDocumentoEntrada")
                propOrdenar = "NumeroDocumentoTituloOriginal";

            return propOrdenar;
        }

        private bool IsPermitirGerenciarCheque(Dominio.Entidades.Embarcador.Financeiro.TituloBaixa tituloBaixa)
        {
            return (tituloBaixa.SituacaoBaixaTitulo == SituacaoBaixaTitulo.Iniciada) || (tituloBaixa.SituacaoBaixaTitulo == SituacaoBaixaTitulo.EmNegociacao);
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> ConfiguracaoImportacaoPlanilha()
        {
            List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao>();

            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 1, Descricao = "CNPJ do Fornecedor", Propriedade = "CNPJFornecedor", Tamanho = 100, Obrigatorio = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 2, Descricao = "Número do Documento", Propriedade = "NumeroDocumento", Tamanho = 100, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 3, Descricao = "Data de Emissão Inicial", Propriedade = "DataEmissaoInicial", Tamanho = 100, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 4, Descricao = "Data de Emissão Final", Propriedade = "DataEmissaoFinal", Tamanho = 100, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 5, Descricao = "Código do Título", Propriedade = "CodigoTitulo", Tamanho = 100, Obrigatorio = false, Regras = new List<string> { } });

            return configuracoes;
        }

        private Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha RetornarFalhaLinha(string mensagem, int indice)
        {
            Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha retorno = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha() { indice = indice, mensagemFalha = mensagem, processou = false };
            return retorno;
        }

        private IActionResult ObterGridPesquisaTitulosPendentes(Repositorio.UnitOfWork unitOfWork, bool exportacao = false)
        {
            Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaBaixaTituloPendente filtrosPesquisa = ObterFiltrosPesquisaTituloPendente(unitOfWork);

            Models.Grid.Grid grid = new Models.Grid.Grid(Request);
            grid.header = new List<Models.Grid.Head>();
            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("CNPJPessoa", false);
            grid.AdicionarCabecalho("Título", "CodigoTitulo", 6, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Parcela", "NumeroParcela", 8, Models.Grid.Align.right, true);
            grid.AdicionarCabecalho("Data Emissão", "DataEmissao", 11, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Data Vencimento", "DataVencimento", 11, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Fornecedor", "Pessoa", 32, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Nº Doc", "NumeroDocumentoEntrada", 10, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Valor", "Valor", 8, Models.Grid.Align.right, true);
            if (ConfiguracaoEmbarcador.UtilizaMoedaEstrangeira)
                grid.AdicionarCabecalho("Valor Moeda Estrangeira", "ValorOriginalMoedaEstrangeira", 12, Models.Grid.Align.right, true);
            else
                grid.AdicionarCabecalho("ValorOriginalMoedaEstrangeira", false);
            grid.AdicionarCabecalho("Valor Original", "ValorOriginal", 10, Models.Grid.Align.right, true);

            string propOrdenar = ObterPropriedadeOrdenarTituloPendente(grid.header[grid.indiceColunaOrdena].data);

            Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);

            List<Dominio.Entidades.Embarcador.Financeiro.Titulo> listaTitulos = repTitulo.ConsultarTitulosPendentes(filtrosPesquisa, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
            grid.setarQuantidadeTotal(repTitulo.ContarTitulosPendentes(filtrosPesquisa));

            var lista = (from p in listaTitulos select ObterDetalhesTituloGrid(p)).ToList();

            grid.AdicionaRows(lista);

            if (exportacao)
            {
                byte[] bArquivo = grid.GerarExcel();

                if (bArquivo != null)
                    return Arquivo(bArquivo, "application/octet-stream", grid.tituloExportacao + "." + grid.extensaoCSV);
                else
                    return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");
            }
            else
                return new JsonpResult(grid);
        }

        private object ObterDetalhesTituloGrid(Dominio.Entidades.Embarcador.Financeiro.Titulo titulo)
        {
            return new
            {
                DT_RowId = titulo.Codigo,
                titulo.Codigo,
                CNPJPessoa = titulo.Pessoa != null ? titulo.Pessoa.CPF_CNPJ : 0,
                CodigoTitulo = titulo.Codigo.ToString("n0"),
                NumeroParcela = titulo.Sequencia.ToString("n0"),
                DataVencimento = titulo.DataVencimento?.ToString("dd/MM/yyyy"),
                DataEmissao = titulo.DataEmissao?.ToString("dd/MM/yyyy"),
                Pessoa = titulo.Pessoa?.Descricao ?? titulo.GrupoPessoas?.Descricao ?? string.Empty,
                NumeroDocumentoEntrada = !string.IsNullOrWhiteSpace(titulo.NumeroDocumentoTituloOriginal) ? titulo.NumeroDocumentoTituloOriginal : titulo.DuplicataDocumentoEntrada != null && titulo.DuplicataDocumentoEntrada.DocumentoEntrada != null ? titulo.DuplicataDocumentoEntrada.DocumentoEntrada.Numero.ToString("n0") : string.Empty,
                Valor = titulo.Saldo.ToString("n2"),
                ValorOriginal = titulo.ValorOriginal.ToString("n2"),
                ValorOriginalMoedaEstrangeira = titulo.ValorOriginalMoedaEstrangeira.ToString("n2"),
                DT_RowColor = TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe ?
                    titulo.TituloBaixaNegociacao != null ? CorGrid.Verde : titulo.PossuiPagamentoDigital ? CorGrid.Orange : CorGrid.Amarelo :
                    titulo.DataVencimento.Value.Date < DateTime.Now.Date ? CorGrid.Red : titulo.DataVencimento.Value.Date == DateTime.Now.Date ?
                    CorGrid.Orange : CorGrid.Branco,
                DT_FontColor = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe && (titulo.DataVencimento.Value.Date < DateTime.Now.Date || titulo.DataVencimento.Value.Date == DateTime.Now.Date) ?
                    CorGrid.Branco : CorGrid.Cinza
            };
        }

        private void GerarTitulosNegociacao(Dominio.Entidades.Embarcador.Financeiro.TituloBaixa tituloBaixa, List<Dominio.Entidades.Embarcador.Financeiro.Titulo> listaTitulo, double codigoPessoaNegociacao, decimal valorDesconto, Repositorio.UnitOfWork unitOfWork)
        {
            if (tituloBaixa.TitulosNegociacao == null || tituloBaixa.TitulosNegociacao.Count == 0)
                return;

            Repositorio.Embarcador.Financeiro.TituloBaixa repTituloBaixa = new Repositorio.Embarcador.Financeiro.TituloBaixa(unitOfWork);
            Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);
            Repositorio.Cliente repPessoa = new Repositorio.Cliente(unitOfWork);
            Repositorio.Embarcador.Financeiro.TituloCentroResultadoTipoDespesa repTituloCentroResultadoTipoDespesa = new Repositorio.Embarcador.Financeiro.TituloCentroResultadoTipoDespesa(unitOfWork);

            List<(Dominio.Entidades.Embarcador.Financeiro.Despesa.TipoDespesaFinanceira TipoDespesa, Dominio.Entidades.Embarcador.Financeiro.CentroResultado CentroResultado, decimal Percentual)> listaGeracao = new List<(Dominio.Entidades.Embarcador.Financeiro.Despesa.TipoDespesaFinanceira, Dominio.Entidades.Embarcador.Financeiro.CentroResultado, decimal percentual)>();

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
            {
                List<Dominio.Entidades.Embarcador.Financeiro.TituloCentroResultadoTipoDespesa> centrosTiposDespesas = repTituloCentroResultadoTipoDespesa.BuscarPorTitulos(listaTitulo.Select(o => o.Codigo).ToList());

                decimal totalPercentual = 0;
                int countRegistros = 0;
                decimal valorTotalTitulos = listaTitulo.Sum(o => o.Saldo);

                foreach (Dominio.Entidades.Embarcador.Financeiro.TituloCentroResultadoTipoDespesa item in centrosTiposDespesas)
                {
                    decimal novoPercentual = Math.Round((item.Titulo.Saldo * 100) / valorTotalTitulos, 2);
                    novoPercentual = Math.Round(item.Percentual * (novoPercentual / 100), 2);

                    countRegistros++;
                    totalPercentual += novoPercentual;

                    if (countRegistros == centrosTiposDespesas.Count && totalPercentual != 100)
                    {
                        decimal diferenca = 100 - totalPercentual;
                        novoPercentual += diferenca;
                        totalPercentual += diferenca;
                    }

                    if (novoPercentual <= 0)
                        throw new ControllerException("O percentual rateado dos Centros de Resultado/Tipos de Despesa dos títulos originais ficou menor ou igual a zero, favor acionar o suporte");

                    listaGeracao.Add(ValueTuple.Create(item.TipoDespesaFinanceira, item.CentroResultado, novoPercentual));
                }

                if (totalPercentual > 0 && totalPercentual != 100)
                    throw new ControllerException("O percentual rateado dos Centros de Resultado/Tipos de Despesa dos títulos originais difere de 100%, favor acionar o suporte");
            }

            List<(Dominio.Entidades.Embarcador.Financeiro.Despesa.TipoDespesaFinanceira TipoDespesa, Dominio.Entidades.Embarcador.Financeiro.CentroResultado CentroResultado, decimal Percentual)> listaGeracaoAgrupada = listaGeracao
                .GroupBy(obj => new { obj.TipoDespesa, obj.CentroResultado })
                .Select(obj => ValueTuple.Create(obj.Key.TipoDespesa, obj.Key.CentroResultado, obj.Sum(p => p.Percentual)))
                .ToList();

            Dominio.Entidades.Embarcador.Financeiro.TipoMovimento tipoMovimentoPadrao = repTituloBaixa.BuscarTipoMovimentoPadrao(tituloBaixa.Codigo);
            Dominio.Entidades.Empresa empresa = tituloBaixa.TitulosAgrupados?.Select(o => o.Titulo?.Empresa)?.FirstOrDefault() ?? null;
            Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS pagamentoMotorista = listaTitulo.Where(o => o.PagamentoMotorista != null).Select(o => o.PagamentoMotorista)?.FirstOrDefault() ?? null;

            decimal valorTotalTitulosNegociacao = tituloBaixa.TitulosNegociacao.Sum(o => o.Valor);
            Dominio.Entidades.Cliente pessoaNegociacao = codigoPessoaNegociacao > 0 ? repPessoa.BuscarPorCPFCNPJ(codigoPessoaNegociacao) : null;
            for (int i = 0; i < tituloBaixa.TitulosNegociacao.Count(); i++)
            {
                Dominio.Entidades.Embarcador.Financeiro.TituloBaixaNegociacao tituloNegociacao = tituloBaixa.TitulosNegociacao[i];

                Dominio.Entidades.Embarcador.Financeiro.Titulo novoTitulo = new Dominio.Entidades.Embarcador.Financeiro.Titulo();
                novoTitulo.Acrescimo = tituloNegociacao.Acrescimo;
                novoTitulo.DataEmissao = tituloNegociacao.DataEmissao;
                novoTitulo.DataVencimento = tituloNegociacao.DataVencimento;
                novoTitulo.DataProgramacaoPagamento = tituloNegociacao.DataVencimento;
                novoTitulo.Desconto = tituloNegociacao.Desconto;
                novoTitulo.TituloBaixaNegociacao = tituloNegociacao;
                if (tituloBaixa.GrupoPessoas != null)
                    novoTitulo.GrupoPessoas = tituloBaixa.GrupoPessoas;
                novoTitulo.Historico = "TITULO A PAGAR GERADO PELO USUÁRIO " + this.Usuario.Nome + " A PARTIR DA NEGOCIAÇÃO DO TÍTULO " + tituloBaixa.CodigosTitulos;
                if (tituloBaixa.Pessoa != null)
                    novoTitulo.Pessoa = tituloBaixa.Pessoa;
                if (pessoaNegociacao != null)
                    novoTitulo.Pessoa = pessoaNegociacao;
                if (novoTitulo.GrupoPessoas == null && novoTitulo.Pessoa != null && novoTitulo.Pessoa.GrupoPessoas != null)
                    novoTitulo.GrupoPessoas = novoTitulo.Pessoa.GrupoPessoas;
                novoTitulo.Sequencia = tituloNegociacao.Sequencia;
                novoTitulo.StatusTitulo = StatusTitulo.EmAberto;
                novoTitulo.DataAlteracao = DateTime.Now;
                novoTitulo.TipoTitulo = TipoTitulo.Pagar;
                novoTitulo.ValorOriginal = tituloNegociacao.Valor;
                novoTitulo.ValorPago = 0;
                novoTitulo.ValorPendente = tituloNegociacao.Valor;
                novoTitulo.Empresa = empresa;
                novoTitulo.TipoMovimento = tipoMovimentoPadrao;

                novoTitulo.Usuario = this.Usuario;
                novoTitulo.DataLancamento = DateTime.Now;

                novoTitulo.ValorTituloOriginal = tituloBaixa.ValorTituloOriginal;
                novoTitulo.TipoDocumentoTituloOriginal = tituloBaixa.TipoDocumentoTituloOriginal;
                if (!string.IsNullOrWhiteSpace(novoTitulo.TipoDocumentoTituloOriginal) && novoTitulo.TipoDocumentoTituloOriginal.Length > 500)
                    novoTitulo.TipoDocumentoTituloOriginal = novoTitulo.TipoDocumentoTituloOriginal.Substring(0, 499);
                novoTitulo.NumeroDocumentoTituloOriginal = tituloBaixa.NumeroDocumentoTituloOriginal;
                if (!string.IsNullOrWhiteSpace(novoTitulo.NumeroDocumentoTituloOriginal) && novoTitulo.NumeroDocumentoTituloOriginal.Length > 4000)
                    novoTitulo.NumeroDocumentoTituloOriginal = novoTitulo.NumeroDocumentoTituloOriginal.Substring(0, 3999);
                novoTitulo.FormaTitulo = tituloNegociacao.FormaParcela;
                novoTitulo.NossoNumero = tituloNegociacao.NumeroBoleto;
                if (novoTitulo.NossoNumero.Length > 50)
                    novoTitulo.NossoNumero = novoTitulo.NossoNumero.Substring(0, 49);
                novoTitulo.Portador = tituloNegociacao.Portador;
                novoTitulo.PagamentoMotorista = pagamentoMotorista;

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    novoTitulo.TipoAmbiente = this.Usuario.Empresa.TipoAmbiente;

                novoTitulo.MoedaCotacaoBancoCentral = tituloBaixa.MoedaCotacaoBancoCentral.HasValue ? tituloBaixa.MoedaCotacaoBancoCentral.Value : MoedaCotacaoBancoCentral.Real;
                novoTitulo.ValorMoedaCotacao = tituloBaixa.ValorMoedaCotacao;
                novoTitulo.DataBaseCRT = tituloBaixa.DataBaseCRT;
                novoTitulo.ValorOriginalMoedaEstrangeira = tituloNegociacao.ValorOriginalMoedaEstrangeira;
                if (novoTitulo.ValorMoedaCotacao > 0 && novoTitulo.ValorOriginalMoedaEstrangeira == 0)
                    novoTitulo.ValorOriginalMoedaEstrangeira = Math.Round(tituloNegociacao.Valor / novoTitulo.ValorMoedaCotacao, 2);

                novoTitulo.DescontoAplicadoNegociacao = valorDesconto > 0 ? Math.Round(valorDesconto * tituloNegociacao.Valor / valorTotalTitulosNegociacao, 2) : 0;

                repTitulo.Inserir(novoTitulo);

                foreach (var item in listaGeracaoAgrupada)
                {
                    Dominio.Entidades.Embarcador.Financeiro.TituloCentroResultadoTipoDespesa tituloCentroResultadoTipoDespesa = new Dominio.Entidades.Embarcador.Financeiro.TituloCentroResultadoTipoDespesa();
                    tituloCentroResultadoTipoDespesa.Titulo = novoTitulo;
                    tituloCentroResultadoTipoDespesa.TipoDespesaFinanceira = item.TipoDespesa;
                    tituloCentroResultadoTipoDespesa.CentroResultado = item.CentroResultado;
                    tituloCentroResultadoTipoDespesa.Percentual = item.Percentual;

                    repTituloCentroResultadoTipoDespesa.Inserir(tituloCentroResultadoTipoDespesa);
                }
            }
        }

        private void GerarReverterVinculoTituloDocumentoAcrescimoDesconto(Dominio.Entidades.Embarcador.Financeiro.TituloBaixa tituloBaixa, Repositorio.UnitOfWork unitOfWork, bool reverter = false)
        {
            if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                return;

            Repositorio.Embarcador.Financeiro.TituloBaixaAcrescimo repTituloBaixaAcrescimo = new Repositorio.Embarcador.Financeiro.TituloBaixaAcrescimo(unitOfWork);
            Repositorio.Embarcador.Financeiro.TituloDocumentoAcrescimoDesconto repTituloDocumentoAcrescimoDesconto = new Repositorio.Embarcador.Financeiro.TituloDocumentoAcrescimoDesconto(unitOfWork);

            List<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAcrescimo> listaTituloBaixaAcrescimo = repTituloBaixaAcrescimo.BuscarPorBaixaTitulo(tituloBaixa.Codigo);

            if (reverter)
            {
                List<Dominio.Entidades.Embarcador.Financeiro.TituloDocumentoAcrescimoDesconto> listaTituloDocumentoAcrescimoDesconto = repTituloDocumentoAcrescimoDesconto.BuscarPorTituloBaixa(tituloBaixa.Codigo);
                foreach (Dominio.Entidades.Embarcador.Financeiro.TituloDocumentoAcrescimoDesconto tituloDocumentoAcrescimoDesconto in listaTituloDocumentoAcrescimoDesconto)
                    repTituloDocumentoAcrescimoDesconto.Deletar(tituloDocumentoAcrescimoDesconto);

                return;
            }

            foreach (Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAcrescimo acrescimoDesconto in listaTituloBaixaAcrescimo)
            {
                Dominio.Entidades.Embarcador.Fatura.Justificativa justificativa = acrescimoDesconto.Justificativa;

                if (justificativa.TipoMovimentoUsoJustificativa == null || justificativa.TipoMovimentoReversaoUsoJustificativa == null)
                    continue;

                Dominio.Entidades.Embarcador.Financeiro.TituloDocumentoAcrescimoDesconto tituloDocumentoAcrescimoDesconto = new Dominio.Entidades.Embarcador.Financeiro.TituloDocumentoAcrescimoDesconto()
                {
                    TituloBaixaAcrescimo = acrescimoDesconto,
                    Justificativa = justificativa,
                    Tipo = EnumTipoAcrescimoDescontoTituloDocumento.Baixa,
                    TipoJustificativa = justificativa.TipoJustificativa,
                    TipoMovimentoUso = justificativa.TipoMovimentoUsoJustificativa,
                    TipoMovimentoReversao = justificativa.TipoMovimentoReversaoUsoJustificativa,
                    Valor = acrescimoDesconto.Valor,
                    Usuario = Usuario
                };

                repTituloDocumentoAcrescimoDesconto.Inserir(tituloDocumentoAcrescimoDesconto);
            }
        }

        #endregion
    }
}
