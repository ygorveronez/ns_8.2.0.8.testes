using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Newtonsoft.Json;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Dominio.Entidades.Embarcador.Financeiro;

namespace SGT.WebAdmin.Controllers.Financeiros
{
    [CustomAuthorize("Financeiros/BaixaTituloReceberNovo")]
    public class BaixaTituloReceberNovoController : BaseController
    {
		#region Construtores

		public BaixaTituloReceberNovoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int.TryParse(Request.Params("Operador"), out int codigoOperador);
                int.TryParse(Request.Params("NumeroTitulo"), out int numeroTitulo);
                int.TryParse(Request.Params("NumeroFatura"), out int numeroFatura);
                int.TryParse(Request.Params("GrupoPessoa"), out int grupoPessoa);
                int.TryParse(Request.Params("Conhecimento"), out int codigoCTe);
                int.TryParse(Request.Params("Carga"), out int codigoCarga);
                int.TryParse(Request.Params("TipoPagamento"), out int codigoTipoPagamento);
                int.TryParse(Request.Params("NumeroDocOriginario"), out int numeroDocOriginario);

                double.TryParse(Request.Params("Pessoa"), out double pessoa);

                DateTime.TryParse(Request.Params("DataInicial"), out DateTime dataInicial);
                DateTime.TryParse(Request.Params("DataFinal"), out DateTime dataFinal);
                DateTime.TryParse(Request.Params("DataBaseInicial"), out DateTime dataBaseInicial);
                DateTime.TryParse(Request.Params("DataBaseFinal"), out DateTime dataBaseFinal);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoBaixaTitulo? situacaoBaixa = null;
                if (Enum.TryParse(Request.Params("Situacao"), out Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoBaixaTitulo situacaoBaixaAux))
                    situacaoBaixa = situacaoBaixaAux;

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Títulos", "CodigosTitulos", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Faturas", "NumeroFaturas", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Grupo Pessoa", "GrupoPessoa", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Cargas", "NumeroCargas", 30, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Situação", "Situacao", 10, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Data Base", "DataBase", 8, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Data da Baixa", "DataBaixa", 8, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Valor Original", "ValorOriginal", 8, Models.Grid.Align.right, false);
                grid.AdicionarCabecalho("Valor", "Valor", 8, Models.Grid.Align.right, false);

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;

                int codigoEmpresa = 0;

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    codigoEmpresa = this.Usuario.Empresa.Codigo;

                Repositorio.Embarcador.Financeiro.TituloBaixa repTituloBaixa = new Repositorio.Embarcador.Financeiro.TituloBaixa(unitOfWork);

                var lista = repTituloBaixa.ConsultaBaixaReceberNovo(codigoCTe, codigoCarga, codigoEmpresa, numeroTitulo, numeroFatura, codigoTipoPagamento, dataInicial, dataFinal, dataBaseInicial, dataBaseFinal, situacaoBaixa, grupoPessoa, pessoa, codigoOperador, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite, numeroDocOriginario);
                grid.setarQuantidadeTotal(repTituloBaixa.ContarConsultaBaixaReceberNovo(codigoCTe, codigoCarga, codigoEmpresa, numeroTitulo, numeroFatura, codigoTipoPagamento, dataInicial, dataFinal, dataBaseInicial, dataBaseFinal, situacaoBaixa, grupoPessoa, pessoa, codigoOperador, numeroDocOriginario));

                //var lista = (from p in listaTitulo
                //             select new
                //             {
                //                 p.Codigo,
                //                 CodigosTitulos = p.CodigosTitulos,
                //                 NumeroFaturas = p.NumeroFaturas,
                //                 GrupoPessoa = p.GrupoPessoas?.Descricao ?? p.Pessoa?.GrupoPessoas?.Descricao ?? string.Empty,
                //                 NumeroCargas = p.NumeroCargas,
                //                 Situacao = p.DescricaoSituacaoBaixaTitulo,
                //                 DataBase = p.DataBase?.ToString("dd/MM/yyyy") ?? string.Empty,
                //                 DataBaixa = p.DataBaixa.Value.ToString("dd/MM/yyyy"),
                //                 ValorOriginal = p.ValorOriginal.ToString("n2"),
                //                 Valor = p.Valor.ToString("n2")
                //             }).ToList();

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
                Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaTituloReceberPendente filtrosPesquisa = ObterFiltrosPesquisa();

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("CNPJPessoa", false);
                grid.AdicionarCabecalho("Documento(s)", "Documentos", 12, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Fatura", "Fatura", 8, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Parcela", "NumeroParcela", 7, Models.Grid.Align.right, false);
                grid.AdicionarCabecalho("Nº Doc. Original", "NumeroDocumentoTituloOriginal", 8, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Nota Fiscal", "NotaFiscal", 8, Models.Grid.Align.right, true, false);
                grid.AdicionarCabecalho("Data Vencimento", "DataVencimento", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Cliente", "Pessoa", 20, Models.Grid.Align.left, false);

                if (ConfiguracaoEmbarcador.UtilizaMoedaEstrangeira)
                {
                    grid.AdicionarCabecalho("Moeda", "Moeda", 8, Models.Grid.Align.left, false);
                    grid.AdicionarCabecalho("Valor Moeda", "ValorMoeda", 8, Models.Grid.Align.right, false);
                }

                grid.AdicionarCabecalho("Valor", "Valor", 8, Models.Grid.Align.right, true);

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                if (propOrdenar == "NumeroParcela")
                    propOrdenar = "Sequencia";
                else if (propOrdenar == "Valor")
                    propOrdenar = "ValorOriginal";
                else if (propOrdenar == "Fatura")
                    propOrdenar = "FaturaParcela.Fatura.Numero";

                Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);

                int count = repTitulo.ContarConsultaTitulosAReceberPendentes(filtrosPesquisa);

                List<Dominio.Entidades.Embarcador.Financeiro.Titulo> listaTitulos = count > 0 ? repTitulo.ConsultarTitulosAReceberPendentes(filtrosPesquisa, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite) : new List<Dominio.Entidades.Embarcador.Financeiro.Titulo>();

                grid.setarQuantidadeTotal(count);

                var lista = (from p in listaTitulos select ObterDetalhesTituloGrid(p)).ToList();

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
        public async Task<IActionResult> ObterDetalhesTitulosPendentesSelecionados()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaTituloReceberPendente filtrosPesquisa = ObterFiltrosPesquisa();

                Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);

                List<Dominio.ObjetosDeValor.Embarcador.Financeiro.AgrupamentoValoresTitulo> valores = repTitulo.ObterDetalhesTitulosReceberPendentes(filtrosPesquisa);

                return new JsonpResult(valores);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao obter os detalhes dos títulos selecionados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Servicos.Embarcador.Financeiro.BaixaTituloReceber servBaixaTituloReceber = new Servicos.Embarcador.Financeiro.BaixaTituloReceber(unitOfWork);
            try
            {
                int codigo = int.Parse(Request.Params("Codigo"));

                var dynRetorno = servBaixaTituloReceber.RetornaObjetoCompletoTitulo(codigo, unitOfWork);

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

        public async Task<IActionResult> GerarBaixa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Servicos.Embarcador.Financeiro.BaixaTituloReceber servBaixaTituloReceber = new Servicos.Embarcador.Financeiro.BaixaTituloReceber(unitOfWork);

                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);
                Repositorio.Embarcador.Financeiro.TituloBaixa repTituloBaixa = new Repositorio.Embarcador.Financeiro.TituloBaixa(unitOfWork);
                Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);
                Repositorio.Embarcador.Financeiro.TituloBaixaAcrescimo repTituloBaixaAcrescimo = new Repositorio.Embarcador.Financeiro.TituloBaixaAcrescimo(unitOfWork);
                Repositorio.Embarcador.Financeiro.TituloBaixaDesconto repTituloBaixaDesconto = new Repositorio.Embarcador.Financeiro.TituloBaixaDesconto(unitOfWork);
                Repositorio.Embarcador.Financeiro.TituloBaixaNegociacao repTituloBaixaNegociacao = new Repositorio.Embarcador.Financeiro.TituloBaixaNegociacao(unitOfWork);
                Repositorio.Embarcador.Financeiro.TituloBaixaConhecimentoRemovido repTituloBaixaConhecimentoRemovido = new Repositorio.Embarcador.Financeiro.TituloBaixaConhecimentoRemovido(unitOfWork);
                Repositorio.Embarcador.Financeiro.TituloBaixaAgrupado repTituloBaixaAgrupado = new Repositorio.Embarcador.Financeiro.TituloBaixaAgrupado(unitOfWork);
                Repositorio.Embarcador.Moedas.Cotacao repCotacao = new Repositorio.Embarcador.Moedas.Cotacao(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraBaixaTituloReceberMoeda repConfiguracaoFinanceiraBaixaTituloReceberMoeda = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraBaixaTituloReceberMoeda(unitOfWork);

                string observacao = Request.GetStringParam("Observacao");

                DateTime dataBaixa = Request.GetDateTimeParam("DataBaixa");
                DateTime dataBase = Request.GetDateTimeParam("DataBase");
                DateTime? dataBaseCRT = Request.GetNullableDateTimeParam("DataBaseCRT");

                Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaTituloReceberPendente filtrosPesquisa = ObterFiltrosPesquisa();

                List<Dominio.ObjetosDeValor.Embarcador.Financeiro.DetalheTituloGeracaoBaixa> listaTitulos = repTitulo.ObterDetalhesTitulosAReceberGeracaoBaixa(filtrosPesquisa);

                List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral> moedas = listaTitulos.Select(o => o.Moeda ?? MoedaCotacaoBancoCentral.Real).Distinct().ToList();

                if (listaTitulos.Count == 0)
                    return new JsonpResult(false, true, "Nenhum título selecionado.");

                if (moedas.Count > 1)
                    return new JsonpResult(false, true, $"Existe mais de uma moeda nos títulos selecionados ({string.Join(", ", moedas.Select(o => o.ObterDescricaoSimplificada()))}), não sendo possível prosseguir.");

                if (dataBaixa > DateTime.Now)
                    return new JsonpResult(false, true, "Não é possível realizar uma baixa com data maior que a atual.");

                if (dataBase > DateTime.Now)
                    return new JsonpResult(false, true, "Não é possível realizar uma baixa com data da base maior que a atual.");

                if (dataBaseCRT > DateTime.Now)
                    return new JsonpResult(false, true, "Não é possível realizar uma baixa com data base CRT maior que a atual.");

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral moeda = moedas.FirstOrDefault();

                Dominio.Entidades.Embarcador.Moedas.Cotacao cotacao = null;

                if (moeda != MoedaCotacaoBancoCentral.Real)
                {
                    if (!dataBaseCRT.HasValue)
                        return new JsonpResult(false, true, "É necessário selecionar a data base CRT para baixas em moeda estrangeira.");

                    cotacao = repCotacao.BuscarCotacao(moeda, dataBaseCRT.Value);

                    if (cotacao == null)
                        return new JsonpResult(false, true, $"Não foi encontrada uma cotação em {moeda.ObterDescricao()} para a data base CRT selecionada.");

                    Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraBaixaTituloReceberMoeda configuracaoMoeda = repConfiguracaoFinanceiraBaixaTituloReceberMoeda.BuscarPorMoeda(moeda);

                    if (configuracaoMoeda == null)
                        return new JsonpResult(false, true, $"Configuração financeira para baixa de títulos a receber em {moeda.ObterDescricao()} não encontrada.");
                }

                unitOfWork.Start();

                Dominio.Entidades.Embarcador.Financeiro.TituloBaixa tituloBaixa = new Dominio.Entidades.Embarcador.Financeiro.TituloBaixa
                {
                    DataBaixa = dataBaixa,
                    DataBase = dataBase,
                    DataBaseCRT = dataBaseCRT,
                    DataOperacao = DateTime.Now,
                    Observacao = observacao,
                    SituacaoBaixaTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoBaixaTitulo.EmGeracao,
                    TipoBaixaTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTitulo.Receber,
                    TitulosPendentesGeracao = listaTitulos.Select(o => new Dominio.Entidades.Embarcador.Financeiro.Titulo() { Codigo = o.Codigo }).ToList(),
                    Usuario = this.Usuario,
                    ValorMoedaCotacao = cotacao?.ValorMoeda ?? 0m,
                    MoedaCotacaoBancoCentral = moeda
                };

                repTituloBaixa.Inserir(tituloBaixa, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(Servicos.Embarcador.Financeiro.BaixaTituloReceber.ObterDetalhesTituloBaixa(tituloBaixa, unitOfWork));
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
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Financeiro.TituloBaixa repositorioTituloBaixa = new Repositorio.Embarcador.Financeiro.TituloBaixa(unitOfWork);
                Dominio.Entidades.Embarcador.Financeiro.TituloBaixa tituloBaixa = repositorioTituloBaixa.BuscarPorCodigo(codigo);

                if (tituloBaixa == null)
                    return new JsonpResult(false, true, "Negociação da baixa de títulos não encontrada.");

                return new JsonpResult(Servicos.Embarcador.Financeiro.BaixaTituloReceber.ObterDetalhesNegociacaoBaixa(tituloBaixa, this.Usuario, ConfiguracaoEmbarcador, unitOfWork));
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
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Financeiro.TituloBaixa repTituloBaixa = new Repositorio.Embarcador.Financeiro.TituloBaixa(unitOfWork);

                Dominio.Entidades.Embarcador.Financeiro.TituloBaixa tituloBaixa = repTituloBaixa.BuscarPorCodigo(codigo);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("CodigoTituloBaixa", false);
                grid.AdicionarCabecalho("Parcela", "Parcela", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Valor", "Valor", 15, Models.Grid.Align.right, false);

                if ((tituloBaixa?.MoedaCotacaoBancoCentral.HasValue ?? false) && tituloBaixa?.MoedaCotacaoBancoCentral != MoedaCotacaoBancoCentral.Real)
                    grid.AdicionarCabecalho("Valor em Moeda", "ValorMoeda", 15, Models.Grid.Align.right, false);

                grid.AdicionarCabecalho("Data Vencimento", "DataVencimento", 20, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Situação", "DescricaoSituacao", 20, Models.Grid.Align.left, false);

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                if (propOrdenar == "Parcela")
                    propOrdenar = "Sequencia";

                Repositorio.Embarcador.Financeiro.TituloBaixaNegociacao repTituloBaixaNegociacao = new Repositorio.Embarcador.Financeiro.TituloBaixaNegociacao(unitOfWork);

                List<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaNegociacao> listaTituloBaixaNegociacao = repTituloBaixaNegociacao.BuscarPorTituloBaixa(codigo, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repTituloBaixaNegociacao.ContarBuscarPorTituloBaixa(codigo));
                var dynRetorno = (from obj in listaTituloBaixaNegociacao
                                  select new
                                  {
                                      obj.Codigo,
                                      CodigoTituloBaixa = obj.TituloBaixa.Codigo,
                                      Parcela = obj.Sequencia.ToString("n0"),
                                      Valor = obj.Valor.ToString("n2"),
                                      ValorMoeda = obj.ValorOriginalMoedaEstrangeira.ToString("n2"),
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
                Repositorio.Embarcador.Financeiro.TituloBaixaAcrescimo repTituloBaixaAcrescimo = new Repositorio.Embarcador.Financeiro.TituloBaixaAcrescimo(unitOfWork);
                Repositorio.Embarcador.Financeiro.TituloBaixaDesconto repTituloBaixaDesconto = new Repositorio.Embarcador.Financeiro.TituloBaixaDesconto(unitOfWork);
                Repositorio.Embarcador.Financeiro.TituloBaixa repTituloBaixa = new Repositorio.Embarcador.Financeiro.TituloBaixa(unitOfWork);
                Repositorio.Embarcador.Financeiro.TituloBaixaNegociacao repTituloBaixaNegociacao = new Repositorio.Embarcador.Financeiro.TituloBaixaNegociacao(unitOfWork);
                Repositorio.Embarcador.Financeiro.TituloBaixaDetalheConhecimento repTituloBaixaDetalheConhecimento = new Repositorio.Embarcador.Financeiro.TituloBaixaDetalheConhecimento(unitOfWork);
                Repositorio.Embarcador.Financeiro.TipoPagamentoRecebimento repTipoPagamentoRecebimento = new Repositorio.Embarcador.Financeiro.TipoPagamentoRecebimento(unitOfWork);
                Repositorio.Embarcador.Financeiro.TituloBaixaAgrupado repTituloBaixaAgrupado = new Repositorio.Embarcador.Financeiro.TituloBaixaAgrupado(unitOfWork);
                Repositorio.Embarcador.Financeiro.TituloBaixaAgrupadoDocumento repTituloBaixaAgrupadoDocumento = new Repositorio.Embarcador.Financeiro.TituloBaixaAgrupadoDocumento(unitOfWork);
                Repositorio.Embarcador.Financeiro.TituloDocumento repTituloDocumento = new Repositorio.Embarcador.Financeiro.TituloDocumento(unitOfWork);

                int.TryParse(Request.Params("Codigo"), out int codigoBaixaTitulo);
                int.TryParse(Request.Params("QuantidadeParcelas"), out int quantidadeParcelas);
                int.TryParse(Request.Params("IntervaloDeDias"), out int intervaloDeDias);
                int.TryParse(Request.Params("TipoDePagamento"), out int codigoTipoDePagamento);

                DateTime.TryParse(Request.Params("DataPrimeiroVencimento"), out DateTime dataPrimeiroVencimento);
                DateTime.TryParse(Request.Params("DataEmissao"), out DateTime dataEmissao);

                Enum.TryParse(Request.Params("TipoArredondamento"), out Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArredondamento tipoArredondamento);

                Dominio.Entidades.Embarcador.Financeiro.TituloBaixa tituloBaixa = repTituloBaixa.BuscarPorCodigo(codigoBaixaTitulo);

                if (tituloBaixa.SituacaoBaixaTitulo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoBaixaTitulo.EmNegociacao &&
                    tituloBaixa.SituacaoBaixaTitulo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoBaixaTitulo.Iniciada)
                    return new JsonpResult(false, true, "A situação da baixa não permite que sejam geradas parcelas.");

                DateTime dataUltimaParcela = dataPrimeiroVencimento;

                List<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado> titulosBaixaAgrupado = repTituloBaixaAgrupado.BuscarPorBaixaTitulo(tituloBaixa.Codigo);
                List<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado> titulosComValorPendente = (from obj in titulosBaixaAgrupado where obj.ValorPago != obj.ValorTotalAPagar select obj).ToList();

                if (titulosComValorPendente.Count <= 0)
                    return new JsonpResult(false, true, "Não existem títulos com valor pendente para geração de parcelas.");

                unitOfWork.Start();

                tituloBaixa.TipoArredondamento = tipoArredondamento;

                repTituloBaixa.Atualizar(tituloBaixa);

                List<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaNegociacao> listaParcelas = repTituloBaixaNegociacao.BuscarPorTituloBaixa(codigoBaixaTitulo);

                for (int i = 0; i < listaParcelas.Count; i++)
                    repTituloBaixaNegociacao.Deletar(listaParcelas[i], Auditado);

                for (int i = 0; i < quantidadeParcelas; i++)
                {
                    Dominio.Entidades.Embarcador.Financeiro.TituloBaixaNegociacao parcela = new Dominio.Entidades.Embarcador.Financeiro.TituloBaixaNegociacao();

                    parcela.DataEmissao = tituloBaixa.DataBaixa.Value.Date;

                    if (i == 0)
                        parcela.DataVencimento = dataPrimeiroVencimento;
                    else
                        parcela.DataVencimento = dataUltimaParcela.AddDays(intervaloDeDias);

                    dataUltimaParcela = parcela.DataVencimento;
                    parcela.TituloBaixa = tituloBaixa;
                    parcela.Sequencia = i + 1;
                    parcela.SituacaoFaturaParcela = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFaturaParcela.EmAberto;

                    foreach (Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado tituloBaixaAgrupado in titulosComValorPendente)
                    {
                        List<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupadoDocumento> tituloBaixaAgrupadoDocumentos = repTituloBaixaAgrupadoDocumento.BuscarPorTituloBaixaAgrupado(tituloBaixaAgrupado.Codigo);

                        foreach (Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupadoDocumento tituloBaixaAgrupadoDocumento in tituloBaixaAgrupadoDocumentos)
                        {
                            if (tituloBaixaAgrupadoDocumento.ValorTotalAPagar != tituloBaixaAgrupadoDocumento.ValorPago)
                            {
                                decimal valorPago = tituloBaixaAgrupadoDocumento.ValorPago;
                                decimal valorAcrescimoTotal = tituloBaixaAgrupadoDocumento.TituloDocumento.ValorAcrescimo + tituloBaixaAgrupadoDocumento.ValorAcrescimo - tituloBaixaAgrupadoDocumento.ValorDesconto - tituloBaixaAgrupadoDocumento.TituloDocumento.ValorDesconto;

                                if (valorAcrescimoTotal > valorPago)
                                {
                                    decimal valorAcrescimoRatear = valorAcrescimoTotal - valorPago;
                                    decimal valorAcrescimoRateado = Math.Floor(valorAcrescimoRatear / quantidadeParcelas * 100m) / 100m;

                                    decimal valorAcrescimoDiferenca = valorAcrescimoRatear - (valorAcrescimoRateado * quantidadeParcelas);

                                    if ((!tituloBaixa.TipoArredondamento.HasValue || tituloBaixa.TipoArredondamento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArredondamento.Primeira) && i == 0)
                                        parcela.Valor += valorAcrescimoRateado + valorAcrescimoDiferenca;
                                    else if (tituloBaixa.TipoArredondamento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArredondamento.Ultima && i == (quantidadeParcelas - 1))
                                        parcela.Valor += valorAcrescimoRateado + valorAcrescimoDiferenca;
                                    else
                                        parcela.Valor += valorAcrescimoRateado;

                                    valorPago = 0;
                                }
                                else
                                {
                                    valorPago -= valorAcrescimoTotal;
                                }

                                decimal valorDocumentoMoedaRatear = tituloBaixaAgrupadoDocumento.ValorTotalAPagarMoeda - tituloBaixaAgrupadoDocumento.ValorPagoMoeda;
                                decimal valorDocumentoMoedaRateado = Math.Floor(valorDocumentoMoedaRatear / quantidadeParcelas * 100m) / 100m;
                                decimal valorDocumentoMoedaDiferenca = valorDocumentoMoedaRatear - (valorDocumentoMoedaRateado * quantidadeParcelas);

                                decimal valorDocumentoRatear = tituloBaixaAgrupadoDocumento.TituloDocumento.Valor - valorPago;
                                decimal valorDocumentoRateado = Math.Floor(valorDocumentoRatear / quantidadeParcelas * 100m) / 100m;
                                decimal valorDocumentoDiferenca = valorDocumentoRatear - (valorDocumentoRateado * quantidadeParcelas);

                                if ((!tituloBaixa.TipoArredondamento.HasValue || tituloBaixa.TipoArredondamento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArredondamento.Primeira) && i == 0)
                                {
                                    parcela.ValorOriginalMoedaEstrangeira += valorDocumentoMoedaRateado + valorDocumentoMoedaDiferenca;
                                    parcela.Valor += valorDocumentoRateado + valorDocumentoDiferenca;
                                }
                                else if (tituloBaixa.TipoArredondamento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArredondamento.Ultima && i == (quantidadeParcelas - 1))
                                {
                                    parcela.ValorOriginalMoedaEstrangeira += valorDocumentoMoedaRateado + valorDocumentoMoedaDiferenca;
                                    parcela.Valor += valorDocumentoRateado + valorDocumentoDiferenca;
                                }
                                else
                                {
                                    parcela.ValorOriginalMoedaEstrangeira += valorDocumentoMoedaRateado;
                                    parcela.Valor += valorDocumentoRateado;
                                }
                            }
                        }
                    }

                    if (parcela.Valor < 0.01m)
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, true, "O valor de uma das parcelas ficou menor que R$ 0,01, não sendo possível realizar a geração das mesmas.");
                    }

                    repTituloBaixaNegociacao.Inserir(parcela, Auditado);
                }

                Servicos.Auditoria.Auditoria.Auditar(Auditado, tituloBaixa, null, "Gerou " + quantidadeParcelas.ToString() + " Parcela(s).", unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
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
                    Codigo = parcela.Codigo,
                    Sequencia = parcela.Sequencia.ToString("n0"),
                    Valor = parcela.Valor.ToString("n2"),
                    ValorDesconto = parcela.Desconto.ToString("n2"),
                    DataEmissao = parcela.DataEmissao.ToString("dd/MM/yyyy"),
                    DataVencimento = parcela.DataVencimento.ToString("dd/MM/yyyy")
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
                Repositorio.Embarcador.Financeiro.TituloBaixaDesconto repTituloBaixaDesconto = new Repositorio.Embarcador.Financeiro.TituloBaixaDesconto(unitOfWork);
                Repositorio.Embarcador.Financeiro.TituloBaixaAcrescimo repTituloBaixaAcrescimo = new Repositorio.Embarcador.Financeiro.TituloBaixaAcrescimo(unitOfWork);
                Repositorio.Embarcador.Financeiro.TituloBaixa repTituloBaixa = new Repositorio.Embarcador.Financeiro.TituloBaixa(unitOfWork);
                Repositorio.Embarcador.Financeiro.TituloBaixaAgrupado repTituloBaixaAgrupado = new Repositorio.Embarcador.Financeiro.TituloBaixaAgrupado(unitOfWork);

                int.TryParse(Request.Params("CodigoParcela"), out int codigoParcela);
                int.TryParse(Request.Params("Codigo"), out int codigoBaixa);

                DateTime.TryParse(Request.Params("DataVencimento"), out DateTime dataVencimento);

                decimal valor = Request.GetDecimalParam("Valor");

                Dominio.Entidades.Embarcador.Financeiro.TituloBaixa tituloBaixa = repTituloBaixa.BuscarPorCodigo(codigoBaixa);

                if (tituloBaixa.SituacaoBaixaTitulo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoBaixaTitulo.EmNegociacao &&
                    tituloBaixa.SituacaoBaixaTitulo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoBaixaTitulo.Iniciada)
                    return new JsonpResult(false, true, "A situação da baixa não permite que seja alterada a parcela.");

                List<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaNegociacao> parcelas = repTituloBaixaNegociacao.BuscarPorTituloBaixa(codigoBaixa, codigoParcela);

                Dominio.Entidades.Embarcador.Financeiro.TituloBaixaNegociacao parcela = repTituloBaixaNegociacao.BuscarPorCodigo(codigoParcela, true);

                if (tituloBaixa.MoedaCotacaoBancoCentral.HasValue && tituloBaixa.MoedaCotacaoBancoCentral != MoedaCotacaoBancoCentral.Real && parcela.Valor != valor)
                    return new JsonpResult(false, true, "Alteração de valores para baixas com moeda estrangeira indisponível.");

                decimal valorTotal = tituloBaixa.ValorTotalSaldo - tituloBaixa.ValorPago;
                decimal percentual = valor / valorTotal;

                unitOfWork.Start();

                if (parcela.Valor != valor)
                    parcela.ValorAjustadoManualmente = true;

                parcela.DataVencimento = dataVencimento;
                parcela.Valor = valor;

                decimal valorParcelas = Math.Round((parcelas?.Sum(o => o.Valor) ?? 0m) + parcela.Valor, 2, MidpointRounding.ToEven);

                List<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaNegociacao> parcelasAlterar = parcelas.Where(o => !o.ValorAjustadoManualmente).ToList();
                int qtdParcelas = parcelasAlterar.Count();
                int sequencia = 0;

                if (valorParcelas > 0m && valorParcelas != valorTotal)
                {
                    decimal valorDiferenca = Math.Round(valorTotal - valorParcelas, 2, MidpointRounding.ToEven);

                    int parcelasRatear = qtdParcelas - sequencia;

                    if (parcelasRatear <= 0)
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, true, "Quantidade de parcelas invalida para alteracao de valores.");
                    }

                    if (parcelasRatear > 0)
                    {
                        decimal valorRatearParcelas = Math.Round(valorDiferenca / parcelasRatear, 2, MidpointRounding.ToEven);

                        for (int i = sequencia; i < qtdParcelas; i++)
                        {
                            if (parcelas[i].ValorAjustadoManualmente)
                                continue;

                            parcelas[i].Valor += valorRatearParcelas;

                            repTituloBaixaNegociacao.Atualizar(parcelas[i]);
                        }
                    }

                    valorParcelas = parcelas.Sum(o => o.Valor) + parcela.Valor;
                    valorDiferenca = Math.Round(valorTotal - valorParcelas, 2, MidpointRounding.ToEven);

                    if (valorDiferenca != 0m && qtdParcelas > sequencia)
                    {
                        foreach (Dominio.Entidades.Embarcador.Financeiro.TituloBaixaNegociacao parcelaAlterar in parcelasAlterar)
                        {
                            decimal valorAplicar = valorDiferenca;

                            if (valorDiferenca < 0m && parcelaAlterar.Valor + valorDiferenca < 0m)
                                valorAplicar = -parcelaAlterar.Valor;

                            parcelaAlterar.Valor += valorAplicar;
                            valorDiferenca -= valorAplicar;

                            repTituloBaixaNegociacao.Atualizar(parcelaAlterar);

                            if (valorDiferenca == 0m)
                                break;
                        }

                        if (valorDiferenca != 0m)
                        {
                            unitOfWork.Rollback();
                            return new JsonpResult(false, true, "Nao foi possivel aplicar o valor, uma das parcelas ficara com valor negativo.");
                        }
                    }
                }

                repTituloBaixaNegociacao.Atualizar(parcela, Auditado);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, tituloBaixa, null, "Atualizou a parcela " + parcela.Sequencia.ToString() + ".", unitOfWork);

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
                Servicos.Embarcador.Financeiro.ProcessoMovimento servProcessoMovimento = new Servicos.Embarcador.Financeiro.ProcessoMovimento(_conexao.StringConexao);
                Servicos.Embarcador.Financeiro.BaixaTituloReceber servBaixaTituloReceber = new Servicos.Embarcador.Financeiro.BaixaTituloReceber(unitOfWork);
                Repositorio.Embarcador.Financeiro.TituloBaixaNegociacao repTituloBaixaNegociacao = new Repositorio.Embarcador.Financeiro.TituloBaixaNegociacao(unitOfWork);
                Repositorio.Embarcador.Financeiro.TituloBaixa repTituloBaixa = new Repositorio.Embarcador.Financeiro.TituloBaixa(unitOfWork);
                Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);
                Repositorio.Embarcador.Financeiro.TipoPagamentoRecebimento repTipoPagamentoRecebimento = new Repositorio.Embarcador.Financeiro.TipoPagamentoRecebimento(unitOfWork);
                Repositorio.Embarcador.Financeiro.TituloBaixaAgrupado repTituloBaixaAgrupado = new Repositorio.Embarcador.Financeiro.TituloBaixaAgrupado(unitOfWork);
                Repositorio.Embarcador.Financeiro.TituloBaixaAgrupadoDocumento repTituloBaixaAgrupadoDocumento = new Repositorio.Embarcador.Financeiro.TituloBaixaAgrupadoDocumento(unitOfWork);
                Repositorio.Embarcador.Financeiro.TituloDocumento repTituloDocumento = new Repositorio.Embarcador.Financeiro.TituloDocumento(unitOfWork);
                Repositorio.Embarcador.Financeiro.TituloBaixaAgrupadoDocumentoAcrescimoDesconto repTituloBaixaAgrupadoDocumentoAcrescimoDesconto = new Repositorio.Embarcador.Financeiro.TituloBaixaAgrupadoDocumentoAcrescimoDesconto(unitOfWork);
                Repositorio.Embarcador.Financeiro.TituloDocumentoAcrescimoDesconto repTituloDocumentoAcrescimoDesconto = new Repositorio.Embarcador.Financeiro.TituloDocumentoAcrescimoDesconto(unitOfWork);
                Repositorio.Embarcador.Financeiro.DocumentoFaturamento repDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(unitOfWork);

                string erro = string.Empty;

                int.TryParse(Request.Params("Codigo"), out int codigo);
                int.TryParse(Request.Params("TipoPagamento"), out int codigoTipoPagamento);

                Dominio.Entidades.Embarcador.Financeiro.TituloBaixa tituloBaixa = repTituloBaixa.BuscarPorCodigo(codigo);
                List<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado> titulosBaixaAgrupado = repTituloBaixaAgrupado.BuscarPorBaixaTitulo(tituloBaixa.Codigo);

                if (tituloBaixa.SituacaoBaixaTitulo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoBaixaTitulo.EmNegociacao && tituloBaixa.SituacaoBaixaTitulo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoBaixaTitulo.Iniciada)
                    return new JsonpResult(false, true, "A situação da baixa não permite que a mesma seja finalizada.");

                decimal valorPendente = tituloBaixa.ValorTotalAPagar - tituloBaixa.ValorPago;

                if (valorPendente > 0m)
                {
                    if (ConfiguracaoEmbarcador.BloquearBaixaParcialOuParcelamentoFatura)
                        return new JsonpResult(false, true, "Não é permitido realizar a baixa parcial/renegociação de títulos.");

                    if (tituloBaixa.Pessoa == null)
                        return new JsonpResult(false, true, "Não é possível renegociar títulos com diferentes tomadores. É necessário abrir uma baixa para cada tomador.");

                    if (tituloBaixa.TitulosNegociacao.Count <= 0)
                        return new JsonpResult(false, true, "É necessário gerar as parcelas com o saldo pendente antes de finalizar a baixa.");

                    if (tituloBaixa.TitulosNegociacao.Sum(o => o.Valor) != valorPendente)
                        return new JsonpResult(false, true, "O saldo pendente difere do valor das parcelas da negociação.");

                    if (tituloBaixa.TitulosNegociacao.Any(o => o.Valor <= 0m))
                        return new JsonpResult(false, true, "O valor das parcelas da negociação não pode estar zerado ou negativo.");

                    if (repTituloBaixaAgrupado.ContarTipoMovimentoTituloPagamentoParcialPorBaixa(tituloBaixa.Codigo) > 1)
                        return new JsonpResult(false, true, "Os tipos de movimentações dos títulos em negociação são diferentes, não sendo possível realizar a negociação.");
                }

                if (valorPendente < 0m)
                    return new JsonpResult(false, "O saldo pendente não pode ser negativo.");

                if (repTitulo.ContemTitulosPagosBaixaTitulo(codigo))
                    return new JsonpResult(false, "Existem títulos quitados nesta baixa, não sendo possível finalizar a mesma.");

                List<int> codigosTitulosDataEmissaoMaior = repTituloBaixaAgrupado.BuscarCodigosTitulosComDataEmissaoSuperiorDataBaixa(tituloBaixa.Codigo);

                if (codigosTitulosDataEmissaoMaior.Count > 0)
                    return new JsonpResult(false, true, "A data de emissão dos títulos (" + string.Join(", ", codigosTitulosDataEmissaoMaior) + ") não pode ser maior que a data da baixa.");

                List<int> codigosTitulosSemTipoMovimento = repTituloBaixaAgrupado.BuscarCodigosTitulosSemTipoMovimento(tituloBaixa.Codigo);

                if (codigosTitulosSemTipoMovimento.Count > 0)
                    return new JsonpResult(false, true, "Existem títulos (" + string.Join(", ", codigosTitulosSemTipoMovimento) + ") sem tipo de movimentação vinculado.");

                unitOfWork.Start();

                tituloBaixa.TipoPagamentoRecebimento = repTipoPagamentoRecebimento.BuscarPorCodigo(codigoTipoPagamento);
                tituloBaixa.SituacaoBaixaTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoBaixaTitulo.EmFinalizacao;

                if (tituloBaixa.Usuario == null)
                    tituloBaixa.Usuario = Usuario;

                repTituloBaixa.Atualizar(tituloBaixa);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, tituloBaixa, null, "Fechou Baixa de Título.", unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(Servicos.Embarcador.Financeiro.BaixaTituloReceber.ObterDetalhesTituloBaixa(tituloBaixa, unitOfWork));
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao finalizar a baixa.");
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
                Servicos.Embarcador.Financeiro.BaixaTituloReceber servBaixaTituloReceber = new Servicos.Embarcador.Financeiro.BaixaTituloReceber(unitOfWork);
                Servicos.Embarcador.Financeiro.ProcessoMovimento servProcessoMovimento = new Servicos.Embarcador.Financeiro.ProcessoMovimento(_conexao.StringConexao);
                Repositorio.Embarcador.Financeiro.TituloBaixa repTituloBaixa = new Repositorio.Embarcador.Financeiro.TituloBaixa(unitOfWork);
                Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);
                Repositorio.Embarcador.Fatura.Fatura repFatura = new Repositorio.Embarcador.Fatura.Fatura(unitOfWork);
                Repositorio.Embarcador.Financeiro.TituloBaixaAcrescimo repTituloBaixaAcrescimo = new Repositorio.Embarcador.Financeiro.TituloBaixaAcrescimo(unitOfWork);
                Repositorio.Embarcador.Financeiro.TituloBaixaDesconto repTituloBaixaDesconto = new Repositorio.Embarcador.Financeiro.TituloBaixaDesconto(unitOfWork);
                Repositorio.Embarcador.Financeiro.TituloBaixaNegociacao repTituloBaixaNegociacao = new Repositorio.Embarcador.Financeiro.TituloBaixaNegociacao(unitOfWork);
                Repositorio.Embarcador.Financeiro.TituloBaixaIntegracao repTituloBaixaIntegracao = new Repositorio.Embarcador.Financeiro.TituloBaixaIntegracao(unitOfWork);
                Repositorio.Embarcador.Financeiro.TituloBaixaConhecimentoRemovido repTituloBaixaConhecimentoRemovido = new Repositorio.Embarcador.Financeiro.TituloBaixaConhecimentoRemovido(unitOfWork);
                Repositorio.Embarcador.Financeiro.TituloBaixaDetalheConhecimento repTituloBaixaDetalheConhecimento = new Repositorio.Embarcador.Financeiro.TituloBaixaDetalheConhecimento(unitOfWork);
                Repositorio.Embarcador.Financeiro.TituloBaixaAgrupadoDocumento repTituloBaixaAgrupadoDocumento = new Repositorio.Embarcador.Financeiro.TituloBaixaAgrupadoDocumento(unitOfWork);
                Repositorio.Embarcador.Financeiro.TituloBaixaAgrupadoDocumentoAcrescimoDesconto repTituloBaixaAgrupadoDocumentoAcrescimoDesconto = new Repositorio.Embarcador.Financeiro.TituloBaixaAgrupadoDocumentoAcrescimoDesconto(unitOfWork);
                Repositorio.Embarcador.Financeiro.DocumentoFaturamento repDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(unitOfWork);
                Repositorio.Embarcador.Financeiro.TituloDocumento repTituloDocumento = new Repositorio.Embarcador.Financeiro.TituloDocumento(unitOfWork);
                Repositorio.Embarcador.Financeiro.TituloDocumentoAcrescimoDesconto repTituloDocumentoAcrescimoDesconto = new Repositorio.Embarcador.Financeiro.TituloDocumentoAcrescimoDesconto(unitOfWork);

                string erro = string.Empty;

                Repositorio.Embarcador.Financeiro.TituloBaixaAgrupado repTituloBaixaAgrupado = new Repositorio.Embarcador.Financeiro.TituloBaixaAgrupado(unitOfWork);

                int.TryParse(Request.Params("Codigo"), out int codigo);

                Dominio.Entidades.Embarcador.Financeiro.TituloBaixa tituloBaixa = repTituloBaixa.BuscarPorCodigo(codigo);

                unitOfWork.Start();

                Servicos.Auditoria.Auditoria.Auditar(Auditado, tituloBaixa, null, "Cancelou Baixa de Título.", unitOfWork);
                if (!Servicos.Embarcador.Financeiro.BaixaTituloReceber.CancelarBaixa(out erro, tituloBaixa, unitOfWork, TipoServicoMultisoftware))
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, erro);
                }

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao cancelar baixa.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ValidarCancelamentoBaixa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int.TryParse(Request.Params("Codigo"), out int codigo);

                Repositorio.Embarcador.Financeiro.TituloBaixa repTituloBaixa = new Repositorio.Embarcador.Financeiro.TituloBaixa(unitOfWork);

                Dominio.Entidades.Embarcador.Financeiro.TituloBaixa tituloBaixa = repTituloBaixa.BuscarPorCodigo(codigo);

                return new JsonpResult(Servicos.Embarcador.Financeiro.BaixaTituloReceber.ValidarCancelamentoBaixa(tituloBaixa, unitOfWork, TipoServicoMultisoftware));
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao validar o cancelamento da baixa.");
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
                Servicos.Embarcador.Financeiro.BaixaTituloReceber servBaixaTituloReceber = new Servicos.Embarcador.Financeiro.BaixaTituloReceber(unitOfWork);

                Repositorio.Embarcador.Financeiro.TituloBaixa repTituloBaixa = new Repositorio.Embarcador.Financeiro.TituloBaixa(unitOfWork);
                Repositorio.Embarcador.Financeiro.MovimentoFinanceiro repMovimentoFinanceiro = new Repositorio.Embarcador.Financeiro.MovimentoFinanceiro(unitOfWork);

                int.TryParse(Request.Params("Codigo"), out int codigo);

                string observacao = Request.Params("Observacao");

                Dominio.Entidades.Embarcador.Financeiro.TituloBaixa tituloBaixa = repTituloBaixa.BuscarPorCodigo(codigo, true);

                tituloBaixa.Observacao = observacao;

                repTituloBaixa.Atualizar(tituloBaixa, Auditado);

                return new JsonpResult(Servicos.Embarcador.Financeiro.BaixaTituloReceber.ObterDetalhesTituloBaixa(tituloBaixa, unitOfWork));
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao salvar observação da baixa.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> SalvarDatas()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Servicos.Embarcador.Financeiro.BaixaTituloPagar servBaixaTituloPagar = new Servicos.Embarcador.Financeiro.BaixaTituloPagar(unitOfWork);

                Repositorio.Embarcador.Financeiro.TituloBaixa repTituloBaixa = new Repositorio.Embarcador.Financeiro.TituloBaixa(unitOfWork);
                Repositorio.Embarcador.Financeiro.TituloBaixaAgrupadoDocumento repTituloBaixaAgrupadaDocumento = new Repositorio.Embarcador.Financeiro.TituloBaixaAgrupadoDocumento(unitOfWork);

                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");
                TituloBaixa tituloBaixa;
                List<TituloBaixaAgrupadoDocumento> titulosBaixaAgrupadoDocumento;
                if (codigo > 0)
                {
                    tituloBaixa = repTituloBaixa.BuscarPorCodigo(codigo, true);
                    tituloBaixa.DataBaixa = Request.GetDateTimeParam("DataBaixa");
                    tituloBaixa.DataBase = Request.GetDateTimeParam("DataBase");

                    repTituloBaixa.Atualizar(tituloBaixa, Auditado);

                    titulosBaixaAgrupadoDocumento = repTituloBaixaAgrupadaDocumento.BuscarPorBaixa(tituloBaixa.Codigo);
                    foreach (TituloBaixaAgrupadoDocumento tituloDocumento in titulosBaixaAgrupadoDocumento)
                    {
                        tituloDocumento.TituloBaixaAgrupado.DataBase = Request.GetDateTimeParam("DataBase");
                        tituloDocumento.TituloBaixaAgrupado.DataBaixa = Request.GetDateTimeParam("DataBaixa");
                        repTituloBaixaAgrupadaDocumento.Atualizar(tituloDocumento, Auditado);
                    }
                }
                else
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, "Não é possível salvar as Datas de uma baixa sem ter iniciado a mesma.");
                }

                unitOfWork.CommitChanges();

                return new JsonpResult(Servicos.Embarcador.Financeiro.BaixaTituloReceber.ObterDetalhesTituloBaixa(tituloBaixa, unitOfWork));
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao salvar Datas da baixa.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ObterDetalhesFinalizacaoBaixa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Financeiros/BaixaTituloReceberNovo");

                if (!Usuario.UsuarioAdministrador && !permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.BaixaTituloReceberNovo_PermiteFinalizarBaixaTitulo))
                    return new JsonpResult(false, true, "Usuário sem permissão para finalizar Baixa do Título.");

                Repositorio.Embarcador.Financeiro.TituloBaixa repTituloBaixa = new Repositorio.Embarcador.Financeiro.TituloBaixa(unitOfWork);

                int.TryParse(Request.Params("Codigo"), out int codigo);

                Dominio.Entidades.Embarcador.Financeiro.TituloBaixa tituloBaixa = repTituloBaixa.BuscarPorCodigo(codigo);

                Dominio.Entidades.Embarcador.Financeiro.TipoPagamentoRecebimento tipoPagamento = null;

                if (tituloBaixa.Titulo?.ContratoFrete?.Carga?.TipoOperacao?.UsarConfiguracaoFaturaPorTipoOperacao ?? false)
                    tipoPagamento = tituloBaixa.Titulo?.ContratoFrete?.Carga?.TipoOperacao?.ConfiguracaoTipoOperacaoFatura?.FormaPagamento;
                else if (tituloBaixa.Pessoa != null && tituloBaixa.Pessoa.NaoUsarConfiguracaoFaturaGrupo)
                    tipoPagamento = tituloBaixa.Pessoa.FormaPagamento;
                else if (tituloBaixa.GrupoPessoas != null)
                    tipoPagamento = tituloBaixa.GrupoPessoas.FormaPagamento;

                var retorno = new
                {
                    Validacao = Servicos.Embarcador.Financeiro.BaixaTituloReceber.ValidarBaixaTitulo(tituloBaixa, unitOfWork, TipoServicoMultisoftware),
                    ValorPago = tituloBaixa.ValorPago,
                    TipoPagamento = new
                    {
                        Codigo = tipoPagamento?.Codigo ?? 0,
                        Descricao = tipoPagamento?.Descricao ?? string.Empty
                    }
                };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao obter os detalhes para a finalização da baixa.");
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

                List<object> titulosRetornar = new List<object>();

                for (int i = 0; i < totalLinhas; i++)
                {
                    try
                    {
                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha linha = linhas[i];

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colNumeroTitulo = linha.Colunas?.Where(o => o.NomeCampo == "NumeroTitulo").FirstOrDefault();
                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colNumeroDocumento = linha.Colunas?.Where(o => o.NomeCampo == "NumeroDocumento").FirstOrDefault();
                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colSerieDocumento = linha.Colunas?.Where(o => o.NomeCampo == "SerieDocumento").FirstOrDefault();
                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colCNPJEmitenteDocumento = linha.Colunas?.Where(o => o.NomeCampo == "CNPJEmitenteDocumento").FirstOrDefault();
                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colNumeroFatura = linha.Colunas?.Where(o => o.NomeCampo == "NumeroFatura").FirstOrDefault();
                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colVencimento = linha.Colunas?.Where(o => o.NomeCampo == "Vencimento").FirstOrDefault();
                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colValor = linha.Colunas?.Where(o => o.NomeCampo == "Valor").FirstOrDefault();
                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colRazaoSocial = linha.Colunas?.Where(o => o.NomeCampo == "RazaoSocial").FirstOrDefault();

                        string cnpjEmitente = Utilidades.String.OnlyNumbers(colCNPJEmitenteDocumento?.Valor);
                        string razaoSocial = colRazaoSocial?.Valor;

                        int.TryParse(colNumeroDocumento?.Valor ?? "0", out int numeroDocumento);
                        int.TryParse(colSerieDocumento?.Valor ?? "0", out int serieDocumento);
                        int.TryParse(colNumeroFatura?.Valor ?? "0", out int numeroFatura);
                        int.TryParse(colNumeroTitulo?.Valor ?? "0", out int numeroTitulo);
                        DateTime.TryParse(colVencimento?.Valor, out DateTime vencimento);
                        decimal.TryParse(colValor?.Valor ?? "0", out decimal valor);

                        List<Dominio.Entidades.Embarcador.Financeiro.Titulo> titulos = null;

                        string mensagemErro = string.Empty;

                        if (numeroFatura > 0)
                        {
                            titulos = repTitulo.BuscarTituloPendentePorFatura(numeroFatura);
                        }
                        else if (numeroTitulo > 0)
                        {
                            titulos = repTitulo.BuscarTituloReceberPendentePorCodigo(numeroTitulo);

                            if (titulos.Count == 0)
                                mensagemErro = $"Não foi encontrado um título pendente com o número {numeroTitulo}. ";
                        }

                        if ((titulos == null || titulos.Count == 0) && !string.IsNullOrWhiteSpace(cnpjEmitente) && numeroDocumento > 0 && serieDocumento > 0)
                        {
                            titulos = repTitulo.BuscarTituloPendentePorCTe(cnpjEmitente, numeroDocumento, serieDocumento);

                            if (titulos.Count == 0)
                                mensagemErro = $"Não foi encontrado um título pendente para o emissor {cnpjEmitente} com o número {numeroDocumento} e série {serieDocumento}. ";
                        }

                        if ((titulos == null || titulos.Count == 0) && !string.IsNullOrWhiteSpace(razaoSocial) && vencimento != DateTime.MinValue && valor > 0)
                        {
                            titulos = repTitulo.BuscarTituloPendentePorTitulo(razaoSocial, vencimento, valor);

                            if (titulos.Count == 0)
                                mensagemErro = $"Não foi encontrado um título pendente para o emissor {razaoSocial} com o valor {valor} e vencimento {vencimento}. ";
                        }

                        if (titulos != null && titulos.Count > 0)
                        {
                            valorPendente += titulos.Sum(o => o.ValorPendente);

                            titulosRetornar.AddRange(titulos.Select(o => ObterDetalhesTituloGrid(o)));

                            retornoImportacao.Retornolinhas.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha() { indice = i, processou = true, mensagemFalha = "" });

                            contador++;
                        }
                        else
                         retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha(mensagemErro, i));
                            
                    }
                    catch (Exception ex2)
                    {
                        Servicos.Log.TratarErro(ex2);
                        retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha("Ocorreu uma falha ao processar a linha.", i));
                        continue;
                    }
                }

                retornoImportacao.Retorno = new { ValorPendente = valorPendente.ToString("n2"), Titulos = titulosRetornar };
                retornoImportacao.Total = linhas.Count;
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

        public async Task<IActionResult> AdicionarTitulo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoTitulo = Request.GetIntParam("Titulo");
                int codigoBaixa = Request.GetIntParam("BaixaTituloReceber");

                Repositorio.Embarcador.Fatura.Fatura repFatura = new Repositorio.Embarcador.Fatura.Fatura(unitOfWork);
                Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                Repositorio.Embarcador.Financeiro.TituloBaixaAgrupado repTituloBaixaAgrupado = new Repositorio.Embarcador.Financeiro.TituloBaixaAgrupado(unitOfWork);
                Repositorio.Embarcador.Financeiro.TituloBaixa repTituloBaixa = new Repositorio.Embarcador.Financeiro.TituloBaixa(unitOfWork);
                Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);

                Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = repTitulo.BuscarPorCodigo(codigoTitulo);
                Dominio.Entidades.Embarcador.Financeiro.TituloBaixa tituloBaixa = repTituloBaixa.BuscarPorCodigo(codigoBaixa);

                if (titulo == null)
                    return new JsonpResult(false, true, "Título não encontrado.");

                if (titulo.TipoTitulo != TipoTitulo.Receber)
                    return new JsonpResult(false, true, "Tipo de título inválido.");

                if (titulo.StatusTitulo != StatusTitulo.EmAberto)
                    return new JsonpResult(false, true, "Situação do título inválida.");

                if (titulo.Baixas.Any(o => o.TituloBaixa.SituacaoBaixaTitulo != SituacaoBaixaTitulo.Cancelada))
                    return new JsonpResult(false, true, "O título já se encontra em outra baixa.");

                if (titulo.ModeloAntigo)
                    return new JsonpResult(false, true, "Este título deve ser baixado pela baixa de títulos agrupados.");

                if (tituloBaixa == null)
                    return new JsonpResult(false, true, "Baixa não encontrada.");

                if (tituloBaixa.ModeloAntigo)
                    return new JsonpResult(false, true, "Esta baixa não suporte títulos com CT-es.");

                if (tituloBaixa.SituacaoBaixaTitulo != SituacaoBaixaTitulo.Iniciada)
                    return new JsonpResult(false, true, "A situação da baixa não permite que seja adicionado um título.");

                if ((tituloBaixa.MoedaCotacaoBancoCentral ?? MoedaCotacaoBancoCentral.Real) != (titulo.MoedaCotacaoBancoCentral ?? MoedaCotacaoBancoCentral.Real))
                    return new JsonpResult(false, true, "A moeda do título difere da moeda utilizada para esta baixa.");

                unitOfWork.Start();

                Servicos.Embarcador.Financeiro.BaixaTituloReceber.AdicionarTituloABaixa(tituloBaixa, titulo.Codigo, unitOfWork, Usuario, 0m, 0m, false);
                Servicos.Embarcador.Financeiro.BaixaTituloReceber.AtualizarTotaisTituloBaixa(ref tituloBaixa, unitOfWork, false);

                List<int> faturas = repTituloBaixaAgrupado.BuscarCodigoFaturasPorTituloBaixa(tituloBaixa.Codigo);
                List<double> tomadores = repTituloBaixaAgrupado.BuscarTomadoresPorTituloBaixa(tituloBaixa.Codigo);
                List<int> grupoPessoas = repTituloBaixaAgrupado.BuscarGrupoPessoasPorTituloBaixa(tituloBaixa.Codigo);

                if (faturas.Count == 1)
                    tituloBaixa.Fatura = repFatura.BuscarPorCodigo(faturas[0]);
                else
                    tituloBaixa.Fatura = null;

                if (tomadores.Count == 1)
                    tituloBaixa.Pessoa = repCliente.BuscarPorCPFCNPJ(tomadores[0]);
                else
                    tituloBaixa.Pessoa = null;

                if (grupoPessoas.Count == 1)
                    tituloBaixa.GrupoPessoas = repGrupoPessoas.BuscarPorCodigo(grupoPessoas[0]);
                else
                    tituloBaixa.GrupoPessoas = null;

                repTituloBaixa.Atualizar(tituloBaixa);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, tituloBaixa, $"Adicionou o título {titulo.Codigo} manualmente.", unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(new
                {
                    Negociacao = Servicos.Embarcador.Financeiro.BaixaTituloReceber.ObterDetalhesNegociacaoBaixa(tituloBaixa, this.Usuario, ConfiguracaoEmbarcador, unitOfWork),
                });
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
                int codigoBaixa = Request.GetIntParam("BaixaTituloReceber");

                Repositorio.Embarcador.Fatura.Fatura repFatura = new Repositorio.Embarcador.Fatura.Fatura(unitOfWork);
                Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                Repositorio.Embarcador.Financeiro.TituloBaixaAgrupado repTituloBaixaAgrupado = new Repositorio.Embarcador.Financeiro.TituloBaixaAgrupado(unitOfWork);
                Repositorio.Embarcador.Financeiro.TituloBaixa repTituloBaixa = new Repositorio.Embarcador.Financeiro.TituloBaixa(unitOfWork);
                Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);

                Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = repTitulo.BuscarPorCodigo(codigoTitulo);
                Dominio.Entidades.Embarcador.Financeiro.TituloBaixa tituloBaixa = repTituloBaixa.BuscarPorCodigo(codigoBaixa);
                Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado tituloBaixaAgrupado = repTituloBaixaAgrupado.BuscarPorTituloEBaixa(codigoBaixa, codigoTitulo);

                if (titulo == null)
                    return new JsonpResult(false, true, "Título não encontrado.");

                if (titulo.TipoTitulo != TipoTitulo.Receber)
                    return new JsonpResult(false, true, "Tipo de título inválido.");

                if (titulo.StatusTitulo != StatusTitulo.EmAberto)
                    return new JsonpResult(false, true, "Situação do título inválida.");

                if (titulo.ModeloAntigo)
                    return new JsonpResult(false, true, "Este título deve ser removido pela baixa de títulos agrupados.");

                if (tituloBaixa == null)
                    return new JsonpResult(false, true, "Baixa não encontrada.");

                if (tituloBaixa.ModeloAntigo)
                    return new JsonpResult(false, true, "Esta baixa não suporta títulos com CT-es.");

                if (tituloBaixa.SituacaoBaixaTitulo != SituacaoBaixaTitulo.Iniciada)
                    return new JsonpResult(false, true, "A situação da baixa não permite que seja removido um título.");

                unitOfWork.Start();

                Servicos.Embarcador.Financeiro.BaixaTituloReceber.RemoverTituloDaBaixa(tituloBaixa, titulo.Codigo, unitOfWork);

                List<int> faturas = repTituloBaixaAgrupado.BuscarCodigoFaturasPorTituloBaixa(tituloBaixa.Codigo);
                List<double> tomadores = repTituloBaixaAgrupado.BuscarTomadoresPorTituloBaixa(tituloBaixa.Codigo);
                List<int> grupoPessoas = repTituloBaixaAgrupado.BuscarGrupoPessoasPorTituloBaixa(tituloBaixa.Codigo);

                if (faturas.Count == 1)
                    tituloBaixa.Fatura = repFatura.BuscarPorCodigo(faturas[0]);
                else
                    tituloBaixa.Fatura = null;

                if (tomadores.Count == 1)
                    tituloBaixa.Pessoa = repCliente.BuscarPorCPFCNPJ(tomadores[0]);
                else
                    tituloBaixa.Pessoa = null;

                if (grupoPessoas.Count == 1)
                    tituloBaixa.GrupoPessoas = repGrupoPessoas.BuscarPorCodigo(grupoPessoas[0]);
                else
                    tituloBaixa.GrupoPessoas = null;

                repTituloBaixa.Atualizar(tituloBaixa);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, tituloBaixa, $"Removeu o título {titulo.Codigo} manualmente.", unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(new
                {
                    Negociacao = Servicos.Embarcador.Financeiro.BaixaTituloReceber.ObterDetalhesNegociacaoBaixa(tituloBaixa, this.Usuario, ConfiguracaoEmbarcador, unitOfWork),
                });
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

        #endregion

        #region Métodos Privados

        private List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> ConfiguracaoImportacaoPlanilha()
        {
            List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao>();

            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 1, Descricao = "Número do Título", Propriedade = "NumeroTitulo", Tamanho = 100, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 2, Descricao = "Número do Documento", Propriedade = "NumeroDocumento", Tamanho = 100, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 3, Descricao = "Série do Documento", Propriedade = "SerieDocumento", Tamanho = 100, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 4, Descricao = "CNPJ do Emissor do Documento", Propriedade = "CNPJEmitenteDocumento", Tamanho = 100, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 5, Descricao = "Número da Fatura", Propriedade = "NumeroFatura", Tamanho = 100, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 6, Descricao = "Vencimento", Propriedade = "Vencimento", Tamanho = 100, Obrigatorio = false });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 5, Descricao = "Valor", Propriedade = "Valor", Tamanho = 100, Obrigatorio = false });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 5, Descricao = "Razão Social", Propriedade = "RazaoSocial", Tamanho = 100, Obrigatorio = false });

            return configuracoes;
        }

        private Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha RetornarFalhaLinha(string mensagem, int indice)
        {
            Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha retorno = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha() { indice = indice, mensagemFalha = mensagem, processou = false };
            return retorno;
        }

        private bool IsPermitirGerenciarCheque(Dominio.Entidades.Embarcador.Financeiro.TituloBaixa tituloBaixa)
        {
            return (tituloBaixa.SituacaoBaixaTitulo == SituacaoBaixaTitulo.Iniciada) || (tituloBaixa.SituacaoBaixaTitulo == SituacaoBaixaTitulo.EmNegociacao);
        }

        private object ObterDetalhesTituloGrid(Dominio.Entidades.Embarcador.Financeiro.Titulo p)
        {
            return new
            {
                DT_RowId = p.Codigo.ToString(),
                p.Codigo,
                CNPJPessoa = p.Pessoa?.CPF_CNPJ ?? 0d,
                Documentos = p.NumeroDocumentos,
                p.NumeroDocumentoTituloOriginal,
                Fatura = p.FaturaParcela?.Fatura?.Numero.ToString("n0") ?? string.Empty,
                NotaFiscal = p.NotaFiscal?.Numero.ToString("n0") ?? string.Empty,
                NumeroParcela = p.Sequencia.ToString("n0"),
                DataVencimento = p.DataVencimento.Value.ToString("dd/MM/yyyy"),
                Pessoa = p.Pessoa?.Nome ?? p.GrupoPessoas?.Descricao ?? string.Empty,
                Valor = p.ValorOriginal.ToString("n2"),
                Moeda = p.MoedaCotacaoBancoCentral?.ObterDescricao() ?? "Real",
                ValorMoeda = p.ValorOriginalMoedaEstrangeira.ToString("n2")
            };
        }

        private Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaTituloReceberPendente ObterFiltrosPesquisa()
        {
            Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaTituloReceberPendente filtros = new Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaTituloReceberPendente()
            {
                CNPJPessoa = Request.GetDoubleParam("Pessoa"),
                CodigoBaixa = Request.GetIntParam("Codigo"),
                CodigoCarga = Request.GetIntParam("Carga"),
                CodigoConhecimento = Request.GetListParam<int>("Conhecimento"),
                CodigoEmpresa = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe ? this.Usuario.Empresa.Codigo : 0,
                CodigoFatura = Request.GetIntParam("Fatura"),
                CodigoGrupoPessoa = Request.GetIntParam("GrupoPessoa"),
                CodigosTitulos = Request.GetListParam<int>("ListaTitulos"),
                DataEmissaoFinal = Request.GetDateTimeParam("DataEmissaoFinal"),
                DataEmissaoInicial = Request.GetDateTimeParam("DataEmissaoInicial"),
                DataProgramacaoPagamentoFinal = Request.GetDateTimeParam("DataProgramacaoPagamentoFinal"),
                DataProgramacaoPagamentoInicial = Request.GetDateTimeParam("DataProgramacaoPagamentoInicial"),
                DataVencimentoFinal = Request.GetDateTimeParam("DataFinal"),
                DataVencimentoInicial = Request.GetDateTimeParam("DataInicial"),
                NumeroDocumentoOriginario = Request.GetIntParam("NumeroDocumentoOriginario"),
                NumeroOcorrencia = Request.GetStringParam("NumeroOcorrencia"),
                NumeroPedido = Request.GetStringParam("NumeroPedido"),
                NumeroTitulo = Request.GetIntParam("NumeroTitulo"),
                SelecionarTodos = Request.GetBoolParam("SelecionarTodos"),
                Valor = Request.GetDecimalParam("Valor"),
                SomenteTitulosDeNegociacao = Request.GetNullableEnumParam<Dominio.Enumeradores.OpcaoSimNao>("TitulosDeAgrupamento")
            };

            if (filtros.CodigoConhecimento.Count() == 0)
            {
                int CodigoConhecimentoUnico = Request.GetIntParam("Conhecimento");
                if(CodigoConhecimentoUnico > 0)
                    filtros.CodigoConhecimento.Add(CodigoConhecimentoUnico);
            }

            return filtros;
        }

        #endregion
    }
}

