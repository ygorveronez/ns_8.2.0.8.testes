using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

using Microsoft.AspNetCore.Mvc;
using Dominio.ObjetosDeValor.Embarcador.Importacao;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Threading.Tasks;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Relatorios;
using Servicos.Extensions;
using Utilidades.Extensions;

namespace SGT.WebAdmin.Controllers.Financeiros
{
    [CustomAuthorize("Financeiros/TituloFinanceiro", "Pessoas/Pessoa", "Financeiros/PlanoOrcamentario", "Financeiros/LancamentoConta")]
    public class TituloFinanceiroController : BaseController
    {
		#region Construtores

		public TituloFinanceiroController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Models.Grid.Grid grid = GridPesquisa();

                string propOrdenar = ObterPropriedadeOrdenar(grid.header[grid.indiceColunaOrdena].data);

                int totalRegistros = 0;
                var lista = ExecutaPesquisa(ref totalRegistros, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(totalRegistros);

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
        public async Task<IActionResult> PesquisaConsulta()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Models.Grid.Grid grid = GridPesquisaConsulta();

                string propOrdenar = ObterPropriedadeOrdenar(grid.header[grid.indiceColunaOrdena].data);

                int totalRegistros = 0;
                var lista = ExecutaPesquisa(ref totalRegistros, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(totalRegistros);

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
        public async Task<IActionResult> ExportarPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Models.Grid.Grid grid = GridPesquisa();

                string propOrdenar = ObterPropriedadeOrdenar(grid.header[grid.indiceColunaOrdena].data);

                int totalRegistros = 0;
                var lista = ExecutaPesquisa(ref totalRegistros, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                grid.AdicionaRows(lista);

                byte[] bArquivo = grid.GerarExcel();

                if (bArquivo != null)
                    return Arquivo(bArquivo, "application/octet-stream", grid.tituloExportacao + "." + grid.extensaoCSV);
                else
                    return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao exportar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaConhecimentos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoTitulo = 0;
                int.TryParse(Request.Params("Codigo"), out codigoTitulo);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Fatura", "NumeroFatura", 8, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Número", "Numero", 8, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Série", "Serie", 8, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Tomador", "Tomador", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Chave", "Chave", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Valor", "Valor", 8, Models.Grid.Align.right, false);

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;

                Repositorio.Embarcador.Fatura.FaturaCargaDocumento repFaturaCargaDocumento = new Repositorio.Embarcador.Fatura.FaturaCargaDocumento(unitOfWork);
                List<Dominio.Entidades.Embarcador.Fatura.FaturaCargaDocumento> listaConhecimento = repFaturaCargaDocumento.ConhecimentosTitulo(codigoTitulo, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repFaturaCargaDocumento.ContarConhecimentosTitulo(codigoTitulo));
                var lista = (from p in listaConhecimento
                             select new
                             {
                                 p.ConhecimentoDeTransporteEletronico.Codigo,
                                 NumeroFatura = p.Fatura.Numero.ToString("n0"),
                                 Numero = p.ConhecimentoDeTransporteEletronico.Numero.ToString("n0"),
                                 Serie = p.ConhecimentoDeTransporteEletronico.Serie.Numero.ToString("n0"),
                                 Tomador = p.ConhecimentoDeTransporteEletronico.Tomador.Nome,
                                 p.ConhecimentoDeTransporteEletronico.Chave,
                                 Valor = p.ConhecimentoDeTransporteEletronico.ValorAReceber.ToString("n2")
                             }).ToList();
                grid.AdicionaRows(lista);
                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                unitOfWork.Dispose();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
        }

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Servicos.Embarcador.Financeiro.ProcessoMovimento servProcessoMovimento = new Servicos.Embarcador.Financeiro.ProcessoMovimento(_conexao.StringConexao);
                Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);
                Repositorio.Embarcador.Financeiro.TituloDocumento repTituloDocumento = new Repositorio.Embarcador.Financeiro.TituloDocumento(unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFrete = new Repositorio.Embarcador.Terceiros.ContratoFrete(unitOfWork);
                Repositorio.Embarcador.Financeiro.DocumentoEntradaDuplicata repDocumentoEntradaDuplicata = new Repositorio.Embarcador.Financeiro.DocumentoEntradaDuplicata(unitOfWork);
                Repositorio.Embarcador.Financeiro.TituloBaixaNegociacao repTituloBaixaNegociacao = new Repositorio.Embarcador.Financeiro.TituloBaixaNegociacao(unitOfWork);
                Repositorio.Embarcador.Financeiro.TipoMovimento repTipoMovimento = new Repositorio.Embarcador.Financeiro.TipoMovimento(unitOfWork);
                Repositorio.Embarcador.Fatura.FaturaParcela repFaturaParcela = new Repositorio.Embarcador.Fatura.FaturaParcela(unitOfWork);
                Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);
                Repositorio.Embarcador.NotaFiscal.NotaFiscal repNotaFiscal = new Repositorio.Embarcador.NotaFiscal.NotaFiscal(unitOfWork);
                Repositorio.Embarcador.Financeiro.DocumentoFaturamento repDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(unitOfWork);
                Repositorio.Embarcador.Financeiro.LancamentoCentroResultado repLancamentoCentroResultado = new Repositorio.Embarcador.Financeiro.LancamentoCentroResultado(unitOfWork);
                Repositorio.Embarcador.Financeiro.Tributo.TributoCodigoReceita repTributoCodigoReceita = new Repositorio.Embarcador.Financeiro.Tributo.TributoCodigoReceita(unitOfWork);
                Repositorio.Embarcador.Financeiro.Tributo.TributoTipoDocumento repTributoTipoDocumento = new Repositorio.Embarcador.Financeiro.Tributo.TributoTipoDocumento(unitOfWork);
                Repositorio.Embarcador.Financeiro.Tributo.TributoTipoImposto repTributoTipoImposto = new Repositorio.Embarcador.Financeiro.Tributo.TributoTipoImposto(unitOfWork);
                Repositorio.Embarcador.Financeiro.Tributo.TributoVariacaoImposto repTributoVariacaoImposto = new Repositorio.Embarcador.Financeiro.Tributo.TributoVariacaoImposto(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro repConfiguracaoFinanceiro = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiro configuracaoFinanceiro = repConfiguracaoFinanceiro.BuscarConfiguracaoPadrao();

                int notaFiscal, duplicataDocumentoEntrada, contratoFrete, faturaParcela, tituloBaixaNegociacao, grupoPessoas = 0;
                int.TryParse(Request.Params("NotaFiscal"), out notaFiscal);
                int.TryParse(Request.Params("DuplicataDocumentoEntrada"), out duplicataDocumentoEntrada);
                int.TryParse(Request.Params("ContratoFrete"), out contratoFrete);
                int.TryParse(Request.Params("FaturaParcela"), out faturaParcela);
                int.TryParse(Request.Params("TituloBaixaNegociacao"), out tituloBaixaNegociacao);
                int.TryParse(Request.Params("GrupoPessoas"), out grupoPessoas);
                int.TryParse(Request.Params("DocumentoFaturamento"), out int codigoDocumentoFaturamento);
                int.TryParse(Request.Params("Empresa"), out int codigoEmpresa);

                double pessoa = 0, portador = 0, contribuinte;
                double.TryParse(Request.Params("Pessoa"), out pessoa);
                double.TryParse(Request.Params("Portador"), out portador);
                double.TryParse(Request.Params("Contribuinte"), out contribuinte);

                int codigoTipoMovimento = 0, sequencia = 0, codigoConhecimentoEletronico, aprovacaoPlanoOrcamentario = 0;
                int.TryParse(Request.Params("Sequencia"), out sequencia);
                int.TryParse(Request.Params("TipoMovimento"), out codigoTipoMovimento);
                int.TryParse(Request.Params("ConhecimentoEletronico"), out codigoConhecimentoEletronico);
                int.TryParse(Request.Params("AprovacaoPlanoOrcamentario"), out aprovacaoPlanoOrcamentario);

                decimal valorOriginal, valorPendente, valorPago, desconto, acrescimo = 0;
                decimal.TryParse(Request.Params("ValorOriginal"), out valorOriginal);
                decimal.TryParse(Request.Params("ValorPendente"), out valorPendente);
                decimal.TryParse(Request.Params("ValorPago"), out valorPago);
                decimal.TryParse(Request.Params("Desconto"), out desconto);
                decimal.TryParse(Request.Params("Acrescimo"), out acrescimo);

                DateTime dataEmissao, dataVencimento, dataLiquidacao, dataAutorizacao;
                DateTime.TryParse(Request.Params("DataEmissao"), out dataEmissao);
                DateTime.TryParse(Request.Params("DataVencimento"), out dataVencimento);

                if (configuracaoFinanceiro.QuantidadeDiasLimiteVencimentoTitulo > 0)
                {
                    DateTime dataLimiteVencimento = DateTime.Now.Date.AddDays(configuracaoFinanceiro.QuantidadeDiasLimiteVencimentoTitulo);
                    int result = DateTime.Compare(dataVencimento, dataLimiteVencimento);

                    if (result > 0)
                        throw new ControllerException($"A data de vencimento {dataVencimento.ToDateString()} é maior que a data limite estipulada nas configurações.");
                }

                DateTime.TryParse(Request.Params("DataLiquidacao"), out dataLiquidacao);
                DateTime.TryParseExact(Request.Params("DataAutorizacao"), "dd/MM/yyyy", null, DateTimeStyles.None, out dataAutorizacao);
                DateTime? periodoApuracao;
                periodoApuracao = Request.GetNullableDateTimeParam("PeriodoApuracao");

                string observacao = Request.Params("Observacao");
                string observacaoInterna = Request.Params("ObservacaoInterna");
                string historico = Request.Params("Historico");
                string numeroDocumentoTituloOriginal = Request.Params("NumeroDocumentoTituloOriginal");
                string tipoDocumentoTituloOriginal = Request.Params("TipoDocumentoTituloOriginal");
                string numeroDocumento = Request.Params("NumeroDocumento");
                string nossoNumero = Request.Params("NossoNumero");
                string linhaDigitavelBoleto = Request.Params("LinhaDigitavelBoleto");
                string codigoReceitaTributo = Request.Params("CodigoReceitaTributo");
                string codigoIdentificacaoTributo = Request.Params("CodigoIdentificacaoTributo");

                if (string.IsNullOrWhiteSpace(numeroDocumentoTituloOriginal) && !string.IsNullOrWhiteSpace(numeroDocumento))
                    numeroDocumentoTituloOriginal = numeroDocumento;

                bool liberadoPagamento = false, adiantado = false, provisao = false;
                bool.TryParse(Request.Params("LiberadoPagamento"), out liberadoPagamento);
                bool.TryParse(Request.Params("Adiantado"), out adiantado);
                bool.TryParse(Request.Params("Provisao"), out provisao);

                Enum.TryParse(Request.Params("StatusTitulo"), out StatusTitulo statusTitulo);
                Enum.TryParse(Request.Params("FormaTitulo"), out FormaTitulo formaTitulo);

                //Tributos
                string tributoReferencia = Request.Params("TributoReferencia");
                int.TryParse(Request.Params("TributoTipoDocumento"), out int tributoTipoDocumento);
                int.TryParse(Request.Params("TributoVariacaoImposto"), out int tributoVariacaoImposto);
                int.TryParse(Request.Params("TributoCodigoReceita"), out int tributoCodigoReceita);
                int.TryParse(Request.Params("TributoTipoImposto"), out int tributoTipoImposto);

                TipoTitulo? tipoTitulo = Request.GetNullableEnumParam<TipoTitulo>("TipoTitulo");

                if (!tipoTitulo.HasValue)
                    throw new ControllerException("O tipo do tipo deve ser informado");

                if (!ConfiguracaoEmbarcador.NaoValidarCodigoBarrasBoletoTituloAPagar && tipoTitulo == TipoTitulo.Pagar &&
                    (!string.IsNullOrWhiteSpace(nossoNumero) || !string.IsNullOrWhiteSpace(linhaDigitavelBoleto)) && Utilidades.String.OnlyNumbers(nossoNumero).Length != 44)
                    throw new ControllerException("O código de barras do boleto deve ter 44 dígitos.");

                valorPendente = valorOriginal - valorPago;
                if (valorPendente < 0m)
                    valorPendente = 0m;

                Repositorio.Embarcador.Financeiro.FechamentoDiario repFechamentoDiario = new Repositorio.Embarcador.Financeiro.FechamentoDiario(unitOfWork);

                if (repFechamentoDiario.VerificarSeExistePorDataFechamento(codigoEmpresa, dataEmissao))
                    throw new ControllerException("Já existe um fechamento diário igual ou posterior à data " + dataEmissao.ToString("dd/MM/yyyy") + ", não sendo possível realizar o lançamento do título.");

                if (dataEmissao > dataVencimento)
                    throw new ControllerException("A data de emissão não pode ser maior que a data de vencimento.");

                if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                {
                    if (codigoTipoMovimento <= 0)
                        throw new ControllerException("Favor informe o tipo de movimento para o lançamento do título.");

                    if (configuracaoFinanceiro.ValidarDuplicidadeTituloSemData)
                    {
                        if (repTitulo.ContemTituloDuplicado(0, pessoa, tipoTitulo.Value, sequencia, tipoDocumentoTituloOriginal, numeroDocumentoTituloOriginal))
                            throw new ControllerException("Já existe um título lançado com a mesma Pessoa, Tipo, Sequência, Tipo de Documento e Número do Documento.");
                    }
                    else
                    {
                        if (repTitulo.ContemTituloDuplicado(dataEmissao, dataVencimento, pessoa, valorOriginal, tipoTitulo.Value, 0, numeroDocumentoTituloOriginal))
                            throw new ControllerException("Já existe um título lançado com a mesma Data de Emissão, Vencimento, Valor, Número do Documento e Pessoa.");
                    }
                }

                Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento documentoFaturamento = repDocumentoFaturamento.BuscarPorCodigo(codigoDocumentoFaturamento);

                if (documentoFaturamento != null)
                {
                    if (documentoFaturamento.ValorAFaturar < valorOriginal)
                        throw new ControllerException("O valor original não pode ser maior que o valor a faturar (" + documentoFaturamento.ValorAFaturar.ToString("n2") + ")");
                }

                unitOfWork.Start();

                Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = new Dominio.Entidades.Embarcador.Financeiro.Titulo();

                titulo.LinhaDigitavelBoleto = linhaDigitavelBoleto;
                titulo.BoletoStatusTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.BoletoStatusTitulo.Nenhum;
                titulo.Acrescimo = acrescimo;
                titulo.Usuario = this.Usuario;
                titulo.DataLancamento = DateTime.Now;
                //titulo.ContratoFrete = repContratoFrete.BuscarPorCodigo(contratoFrete);
                titulo.DataEmissao = dataEmissao;
                titulo.PeriodoApuracao = periodoApuracao.HasValue && periodoApuracao > DateTime.MinValue ? periodoApuracao : null;
                if (dataLiquidacao > DateTime.MinValue)
                {
                    titulo.DataLiquidacao = dataLiquidacao;
                    titulo.DataBaseLiquidacao = dataLiquidacao;
                }
                else
                {
                    titulo.DataLiquidacao = null;
                    titulo.DataBaseLiquidacao = null;
                }
                titulo.DataVencimento = dataVencimento;
                titulo.DataProgramacaoPagamento = Request.GetNullableDateTimeParam("DataProgramacaoPagamento");
                if (dataAutorizacao > DateTime.MinValue)
                    titulo.DataAutorizacao = dataAutorizacao;
                else
                    titulo.DataAutorizacao = null;
                titulo.Desconto = desconto;
                //titulo.DuplicataDocumentoEntrada = repDocumentoEntradaDuplicata.BuscarPorCodigo(duplicataDocumentoEntrada);

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    titulo.Empresa = repEmpresa.BuscarPorCodigo(this.Usuario.Empresa.Codigo);
                else
                    titulo.Empresa = codigoEmpresa > 0 ? repEmpresa.BuscarPorCodigo(codigoEmpresa) : null;

                //titulo.FaturaParcela = repFaturaParcela.BuscarPorCodigo(faturaParcela);
                titulo.Historico = historico;
                //titulo.NotaFiscal = repNotaFiscal.BuscarPorCodigo(notaFiscal);
                titulo.Observacao = observacao;
                titulo.ObservacaoInterna = observacaoInterna;
                titulo.Pessoa = repCliente.BuscarPorCPFCNPJ(pessoa);
                titulo.GrupoPessoas = grupoPessoas > 0 ? repGrupoPessoas.BuscarPorCodigo(grupoPessoas) : null;
                if (titulo.GrupoPessoas == null && titulo.Pessoa != null && titulo.Pessoa.GrupoPessoas != null)
                    titulo.GrupoPessoas = titulo.Pessoa.GrupoPessoas;
                if (sequencia > 0)
                    titulo.Sequencia = sequencia;
                else
                    titulo.Sequencia = 1;
                titulo.StatusTitulo = statusTitulo;
                titulo.DataAlteracao = DateTime.Now;
                titulo.TipoTitulo = tipoTitulo.Value;
                titulo.FormaTitulo = formaTitulo;
                titulo.Adiantado = adiantado;
                //titulo.TituloBaixaNegociacao = repTituloBaixaNegociacao.BuscarPorCodigo(tituloBaixaNegociacao);
                titulo.ValorOriginal = valorOriginal;
                titulo.ValorPago = valorPago;
                titulo.ValorPendente = valorPendente;
                titulo.TipoMovimento = codigoTipoMovimento > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimento) : null;
                titulo.Valor = valorOriginal;
                titulo.ValorTotal = valorOriginal;
                titulo.NumeroDocumentoTituloOriginal = numeroDocumentoTituloOriginal;
                titulo.TipoDocumentoTituloOriginal = tipoDocumentoTituloOriginal;
                titulo.CodigoFavorecido = Request.GetStringParam("CodigoFavorecido");

                titulo.MoedaCotacaoBancoCentral = Request.GetNullableEnumParam<MoedaCotacaoBancoCentral>("MoedaCotacaoBancoCentral");
                titulo.DataBaseCRT = Request.GetNullableDateTimeParam("DataBaseCRT");
                titulo.ValorMoedaCotacao = Request.GetDecimalParam("ValorMoedaCotacao");
                titulo.ValorOriginalMoedaEstrangeira = Request.GetDecimalParam("ValorOriginalMoedaEstrangeira");
                titulo.AdiantamentoFornecedor = Request.GetBoolParam("AdiantamentoFornecedor");

                titulo.TributoReferencia = tributoReferencia;
                titulo.TributoTipoImposto = tributoTipoImposto > 0 ? repTributoTipoImposto.BuscarPorCodigo(tributoTipoImposto) : null;
                titulo.TributoCodigoReceita = tributoCodigoReceita > 0 ? repTributoCodigoReceita.BuscarPorCodigo(tributoCodigoReceita) : null;
                titulo.TributoVariacaoImposto = tributoVariacaoImposto > 0 ? repTributoVariacaoImposto.BuscarPorCodigo(tributoVariacaoImposto) : null;
                titulo.TributoTipoDocumento = tributoTipoDocumento > 0 ? repTributoTipoDocumento.BuscarPorCodigo(tributoTipoDocumento) : null;

                if (titulo.TipoTitulo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTitulo.Pagar)
                    titulo.NossoNumero = nossoNumero;
                if (portador > 0)
                    titulo.Portador = repCliente.BuscarPorCPFCNPJ(portador);
                else
                    titulo.Portador = null;
                if (contribuinte > 0)
                    titulo.Contribuinte = repCliente.BuscarPorCPFCNPJ(contribuinte);
                else
                    titulo.Contribuinte = null;

                if (documentoFaturamento != null)
                {
                    Dominio.Entidades.Embarcador.Financeiro.TituloDocumento tituloDocumento = new Dominio.Entidades.Embarcador.Financeiro.TituloDocumento();

                    if (documentoFaturamento.TipoDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoFaturamento.CTe)
                    {
                        tituloDocumento.CTe = documentoFaturamento.CTe;
                        tituloDocumento.TipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoTitulo.CTe;
                    }
                    else
                    {
                        tituloDocumento.Carga = documentoFaturamento.Carga;
                        tituloDocumento.TipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoTitulo.Carga;
                    }

                    tituloDocumento.Valor = titulo.ValorOriginal;
                    tituloDocumento.ValorTotal = titulo.ValorOriginal;
                    tituloDocumento.ValorPendente = titulo.ValorOriginal;

                    repTituloDocumento.Inserir(tituloDocumento);

                    documentoFaturamento.ValorAFaturar -= titulo.ValorOriginal;

                    repDocumentoFaturamento.Atualizar(documentoFaturamento);
                }
                else if (codigoConhecimentoEletronico > 0)
                {
                    titulo.ConhecimentoDeTransporteEletronico = repCTe.BuscarPorCodigo(codigoConhecimentoEletronico);
                    if (titulo.ConhecimentoDeTransporteEletronico != null)
                    {
                        titulo.Observacao += " CT-e Nº: " + titulo.ConhecimentoDeTransporteEletronico.Numero.ToString() + " chave: " + titulo.ConhecimentoDeTransporteEletronico.Chave;
                        titulo.NumeroDocumentoTituloOriginal = titulo.ConhecimentoDeTransporteEletronico.Numero.ToString();
                    }
                }

                titulo.LiberadoPagamento = liberadoPagamento;
                titulo.Provisao = provisao;

                if (liberadoPagamento)
                    titulo.Historico += " OPERADOR " + this.Usuario.Nome + " LIBEROU O PAGAMENTO DO TÍTULO " + DateTime.Now.ToString("dd/MM/yyyy");

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    titulo.TipoAmbiente = this.Usuario.Empresa.TipoAmbiente;

                if (titulo.TipoTitulo == TipoTitulo.Pagar && !string.IsNullOrWhiteSpace(titulo.NossoNumero))
                {
                    if (repTitulo.ContemTituloNossoNumeroDuplicado(0, titulo.NossoNumero))
                        throw new ControllerException("Já existe um título a pagar lançado com o mesmo número de boleto para o pagamento eletrônico.");
                }

                SalvarVeiculos(titulo, unitOfWork);

                repTitulo.Inserir(titulo);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, titulo, null, "Adicionou título.", unitOfWork);

                SalvarCentrosResultado(titulo, unitOfWork);
                SalvarCentrosResultadoTiposDespesa(titulo, unitOfWork);

                if ((titulo.TipoMovimento != null && !titulo.Provisao) || (titulo.TipoMovimento != null && titulo.Provisao && configuracaoFinanceiro.MovimentacaoFinanceiraParaTitulosDeProvisao))
                {
                    List<Dominio.Entidades.Embarcador.Financeiro.LancamentoCentroResultado> lancamentosCentroResultado = repLancamentoCentroResultado.BuscarPorTitulo(titulo.Codigo);

                    if (!titulo.AdiantamentoFornecedor)
                    {
                        if (lancamentosCentroResultado.Count > 0)
                        {
                            foreach (Dominio.Entidades.Embarcador.Financeiro.LancamentoCentroResultado lancamentoCentroResultado in lancamentosCentroResultado)
                            {
                                if (!servProcessoMovimento.GerarMovimentacao(out string erro, titulo.TipoMovimento, titulo.DataEmissao.Value, lancamentoCentroResultado.Valor, titulo.Codigo.ToString(), "GERAÇÃO DO TÍTULO MANUAL " + observacao, unitOfWork, TipoDocumentoMovimento.Manual, TipoServicoMultisoftware, 0, null, null, titulo.Codigo, null, titulo.Pessoa, titulo.Pessoa?.GrupoPessoas ?? null, null, lancamentoCentroResultado.CentroResultado, null, null, null, null, null, 0, 0, null, null, titulo.FormaTitulo))
                                    throw new ControllerException(erro);
                            }
                        }
                        else
                        {
                            if (!servProcessoMovimento.GerarMovimentacao(out string erro, titulo.TipoMovimento, titulo.DataEmissao.Value, titulo.ValorOriginal, titulo.Codigo.ToString(), "GERAÇÃO DO TÍTULO MANUAL " + observacao, unitOfWork, TipoDocumentoMovimento.Manual, TipoServicoMultisoftware, 0, null, null, titulo.Codigo, null, titulo.Pessoa, titulo.Pessoa?.GrupoPessoas ?? null, null, null, null, null, null, null, null, 0, 0, null, null, titulo.FormaTitulo))
                                throw new ControllerException(erro);
                        }
                    }
                }
                new Servicos.Embarcador.Integracao.IntegracaoTitulo(unitOfWork).IniciarIntegracoesDeTitulos(titulo, TipoAcaoIntegracao.Criacao);

                if(titulo.FormaTitulo != FormaTitulo.Boleto)
                    new Servicos.Embarcador.Integracao.IntegracaoTitulo(unitOfWork).IniciarIntegracoesDeTitulosAReceber(titulo, TipoAcaoIntegracao.Criacao);

                unitOfWork.CommitChanges();

                var dynTipoMovimento = new
                {
                    titulo.Codigo,
                    TipoDocumento = titulo.TipoDocumentoTituloOriginal,
                    NumeroDocumento = titulo.NumeroDocumentoTituloOriginal,
                    Valor = titulo.Valor.ToString("n2"),
                    Pessoa = new
                    {
                        Codigo = titulo.Pessoa?.Codigo ?? 0,
                        Descricao = titulo.Pessoa?.Descricao ?? string.Empty
                    }
                };
                return new JsonpResult(dynTipoMovimento);
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
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar.");
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
                unitOfWork.Start();

                Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFrete = new Repositorio.Embarcador.Terceiros.ContratoFrete(unitOfWork);
                Repositorio.Embarcador.Financeiro.DocumentoEntradaDuplicata repDocumentoEntradaDuplicata = new Repositorio.Embarcador.Financeiro.DocumentoEntradaDuplicata(unitOfWork);
                Repositorio.Embarcador.Financeiro.TituloBaixaNegociacao repTituloBaixaNegociacao = new Repositorio.Embarcador.Financeiro.TituloBaixaNegociacao(unitOfWork);
                Repositorio.Embarcador.Fatura.FaturaParcela repFaturaParcela = new Repositorio.Embarcador.Fatura.FaturaParcela(unitOfWork);
                Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);
                Repositorio.Embarcador.NotaFiscal.NotaFiscal repNotaFiscal = new Repositorio.Embarcador.NotaFiscal.NotaFiscal(unitOfWork);
                Repositorio.Embarcador.Financeiro.LancamentoCentroResultado repLancamentoCentroResultado = new Repositorio.Embarcador.Financeiro.LancamentoCentroResultado(unitOfWork);
                Repositorio.Embarcador.Financeiro.Tributo.TributoCodigoReceita repTributoCodigoReceita = new Repositorio.Embarcador.Financeiro.Tributo.TributoCodigoReceita(unitOfWork);
                Repositorio.Embarcador.Financeiro.Tributo.TributoTipoDocumento repTributoTipoDocumento = new Repositorio.Embarcador.Financeiro.Tributo.TributoTipoDocumento(unitOfWork);
                Repositorio.Embarcador.Financeiro.Tributo.TributoTipoImposto repTributoTipoImposto = new Repositorio.Embarcador.Financeiro.Tributo.TributoTipoImposto(unitOfWork);
                Repositorio.Embarcador.Financeiro.Tributo.TributoVariacaoImposto repTributoVariacaoImposto = new Repositorio.Embarcador.Financeiro.Tributo.TributoVariacaoImposto(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro repConfiguracaoFinanceiro = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro(unitOfWork);

                Servicos.Embarcador.Financeiro.ProcessoMovimento servProcessoMovimento = new Servicos.Embarcador.Financeiro.ProcessoMovimento(unitOfWork.StringConexao);
                Servicos.Embarcador.Financeiro.Titulo servicoTitulo = new Servicos.Embarcador.Financeiro.Titulo(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiro configuracaoFinanceiro = repConfiguracaoFinanceiro.BuscarConfiguracaoPadrao();

                int notaFiscal, duplicataDocumentoEntrada, contratoFrete, faturaParcela, tituloBaixaNegociacao, grupoPessoas = 0;
                int.TryParse(Request.Params("NotaFiscal"), out notaFiscal);
                int.TryParse(Request.Params("DuplicataDocumentoEntrada"), out duplicataDocumentoEntrada);
                int.TryParse(Request.Params("ContratoFrete"), out contratoFrete);
                int.TryParse(Request.Params("FaturaParcela"), out faturaParcela);
                int.TryParse(Request.Params("TituloBaixaNegociacao"), out tituloBaixaNegociacao);
                int.TryParse(Request.Params("GrupoPessoas"), out grupoPessoas);
                int.TryParse(Request.Params("Empresa"), out int codigoEmpresa);

                double pessoa = 0, portador = 0, contribuinte;
                double.TryParse(Request.Params("Pessoa"), out pessoa);
                double.TryParse(Request.Params("Portador"), out portador);
                double.TryParse(Request.Params("Contribuinte"), out contribuinte);

                bool liberadoPagamento = false, adiantado = false, provisao = false, adiantamentoFornecedor = false;
                bool.TryParse(Request.Params("LiberadoPagamento"), out liberadoPagamento);
                bool.TryParse(Request.Params("Adiantado"), out adiantado);
                bool.TryParse(Request.Params("Provisao"), out provisao);
                bool.TryParse(Request.Params("AdiantamentoFornecedor"), out adiantamentoFornecedor);

                int sequencia = 0, codigoConhecimentoEletronico = 0;
                int.TryParse(Request.Params("Sequencia"), out sequencia);
                int.TryParse(Request.Params("ConhecimentoEletronico"), out codigoConhecimentoEletronico);

                decimal valorOriginal, valorPendente, valorPago, desconto, acrescimo = 0;
                decimal.TryParse(Request.Params("ValorOriginal"), out valorOriginal);
                decimal.TryParse(Request.Params("ValorPendente"), out valorPendente);
                decimal.TryParse(Request.Params("ValorPago"), out valorPago);
                decimal.TryParse(Request.Params("Desconto"), out desconto);
                decimal.TryParse(Request.Params("Acrescimo"), out acrescimo);

                DateTime dataEmissao, dataVencimento, dataLiquidacao, dataAutorizacao;
                DateTime.TryParse(Request.Params("DataEmissao"), out dataEmissao);
                DateTime.TryParse(Request.Params("DataVencimento"), out dataVencimento);

                if (configuracaoFinanceiro.QuantidadeDiasLimiteVencimentoTitulo > 0)
                {
                    DateTime dataLimiteVencimento = DateTime.Now.Date.AddDays(configuracaoFinanceiro.QuantidadeDiasLimiteVencimentoTitulo);
                    int result = DateTime.Compare(dataVencimento, dataLimiteVencimento);

                    if (result > 0)
                        throw new ControllerException($"A data de vencimento {dataVencimento.ToDateString()} é maior que a data limite estipulada nas configurações.");
                }

                DateTime.TryParse(Request.Params("DataLiquidacao"), out dataLiquidacao);
                DateTime.TryParseExact(Request.Params("DataAutorizacao"), "dd/MM/yyyy", null, DateTimeStyles.None, out dataAutorizacao);
                DateTime? periodoApuracao;
                periodoApuracao = Request.GetNullableDateTimeParam("PeriodoApuracao");
                if (dataVencimento == DateTime.MinValue)
                    dataVencimento = Request.GetDateTimeParam("DataVencimento");
                if (dataEmissao == DateTime.MinValue)
                    dataEmissao = Request.GetDateTimeParam("DataEmissao");

                DateTime? dataProgramacaoPagamento = Request.GetNullableDateTimeParam("DataProgramacaoPagamento");

                string observacao = Request.Params("Observacao");
                string observacaoInterna = Request.Params("ObservacaoInterna");
                string historico = Request.Params("Historico");
                string numeroDocumentoTituloOriginal = Request.Params("NumeroDocumentoTituloOriginal");
                string tipoDocumentoTituloOriginal = Request.Params("TipoDocumentoTituloOriginal");
                string numeroDocumento = Request.Params("NumeroDocumento");
                string nossoNumero = Request.Params("NossoNumero");
                string linhaDigitavelBoleto = Request.Params("LinhaDigitavelBoleto");
                string codigoReceitaTributo = Request.Params("CodigoReceitaTributo");
                string codigoIdentificacaoTributo = Request.Params("CodigoIdentificacaoTributo");

                Dominio.Entidades.Empresa empresa;

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    empresa = repEmpresa.BuscarPorCodigo(this.Usuario.Empresa.Codigo);
                else
                    empresa = codigoEmpresa > 0 ? repEmpresa.BuscarPorCodigo(codigoEmpresa) : null;

                if (string.IsNullOrWhiteSpace(numeroDocumentoTituloOriginal) && !string.IsNullOrWhiteSpace(numeroDocumento))
                    numeroDocumentoTituloOriginal = numeroDocumento;

                Enum.TryParse(Request.Params("StatusTitulo"), out StatusTitulo statusTitulo);
                Enum.TryParse(Request.Params("FormaTitulo"), out FormaTitulo formaTitulo);

                //Tributos
                string tributoReferencia = Request.Params("TributoReferencia");
                int.TryParse(Request.Params("TributoTipoDocumento"), out int tributoTipoDocumento);
                int.TryParse(Request.Params("TributoVariacaoImposto"), out int tributoVariacaoImposto);
                int.TryParse(Request.Params("TributoCodigoReceita"), out int tributoCodigoReceita);
                int.TryParse(Request.Params("TributoTipoImposto"), out int tributoTipoImposto);

                TipoTitulo? tipoTitulo = Request.GetNullableEnumParam<TipoTitulo>("TipoTitulo");

                if (!tipoTitulo.HasValue)
                    throw new ControllerException("O tipo do tipo deve ser informado");

                if (!ConfiguracaoEmbarcador.NaoValidarCodigoBarrasBoletoTituloAPagar && tipoTitulo == TipoTitulo.Pagar &&
                    (!string.IsNullOrWhiteSpace(linhaDigitavelBoleto) || !string.IsNullOrWhiteSpace(nossoNumero)) && Utilidades.String.OnlyNumbers(nossoNumero).Length != 44)
                    throw new ControllerException("O código de barras do boleto deve ter 44 dígitos.");

                valorPendente = valorOriginal - valorPago;
                if (valorPendente < 0)
                    valorPendente = 0;

                if (dataEmissao > dataVencimento)
                    throw new ControllerException("A data de emissão não pode ser maior que a data de vencimento.");

                int codigo = Request.GetIntParam("Codigo");

                if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                {
                    if (configuracaoFinanceiro.ValidarDuplicidadeTituloSemData)
                    {
                        if (repTitulo.ContemTituloDuplicado(codigo, pessoa, tipoTitulo.Value, sequencia, tipoDocumentoTituloOriginal, numeroDocumentoTituloOriginal))
                            throw new ControllerException("Já existe um título lançado com a mesma Pessoa, Tipo, Sequência, Tipo de Documento e Número do Documento.");
                    }
                    else
                    {
                        if (repTitulo.ContemTituloDuplicado(dataEmissao, dataVencimento, pessoa, valorOriginal, tipoTitulo.Value, codigo, numeroDocumentoTituloOriginal))
                            throw new ControllerException("Já existe um título lançado com a mesma Data de Emissão, Vencimento, Valor, Número do Documento e Pessoa.");
                    }
                }

                Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensalClienteServico repFaturamentoMensalClienteServico = new Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensalClienteServico(unitOfWork);
                Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensalClienteServico faturamentoMensalClienteServico = repFaturamentoMensalClienteServico.BuscarPorTitulo(codigo);

                bool atualizouValorOriginal = false, reverteuMovimentacao = false, eraProvisao = false;

                if (faturamentoMensalClienteServico != null && faturamentoMensalClienteServico.FaturamentoMensal.StatusFaturamentoMensal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusFaturamentoMensal.Finalizado)
                    throw new ControllerException("Este título possui um Faturamento Mensal vinculado, favor Finalizar o mesmo antes de Atualizar.");

                Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = repTitulo.BuscarPorCodigo(codigo, true);

                List<Dominio.Entidades.Embarcador.Financeiro.LancamentoCentroResultado> lancamentosCentroResultado = repLancamentoCentroResultado.BuscarPorTitulo(titulo.Codigo);

                decimal valorAntigo = 0;

                if (!(titulo.StatusTitulo == StatusTitulo.Quitada || titulo.StatusTitulo == StatusTitulo.Cancelado))
                {
                    titulo.LinhaDigitavelBoleto = linhaDigitavelBoleto;

                    if (titulo.ValorOriginal != valorOriginal)
                    {
                        atualizouValorOriginal = true;
                        valorAntigo = titulo.ValorOriginal;
                    }

                    if (titulo.TipoMovimento != null && lancamentosCentroResultado.Count > 0)
                    {
                        atualizouValorOriginal = true;
                        reverteuMovimentacao = true;

                        foreach (Dominio.Entidades.Embarcador.Financeiro.LancamentoCentroResultado lancamentoCentroResultado in lancamentosCentroResultado)
                        {
                            //Reverte o valor antigo
                            if (!servProcessoMovimento.GerarMovimentacao(out string msgErro, null, titulo.DataEmissao.Value.Date, lancamentoCentroResultado.Valor, titulo.Codigo.ToString(), "REVERSÃO DO TÍTULO MANUAL " + observacao, unitOfWork, TipoDocumentoMovimento.Manual, TipoServicoMultisoftware, 0, titulo.TipoMovimento.PlanoDeContaDebito, titulo.TipoMovimento.PlanoDeContaCredito, titulo.Codigo, null, titulo.Pessoa, titulo.Pessoa?.GrupoPessoas ?? null, null, lancamentoCentroResultado.CentroResultado, null, null, null, null, null, 0, 0, null, null, titulo.FormaTitulo))
                                throw new ControllerException(msgErro);
                        }
                    }

                    titulo.Acrescimo = acrescimo;
                    //titulo.ContratoFrete = repContratoFrete.BuscarPorCodigo(contratoFrete);
                    titulo.DataEmissao = dataEmissao;
                    titulo.PeriodoApuracao = periodoApuracao.HasValue && periodoApuracao > DateTime.MinValue ? periodoApuracao : null;
                    if (dataLiquidacao > DateTime.MinValue)
                    {
                        titulo.DataLiquidacao = dataLiquidacao;
                        titulo.DataBaseLiquidacao = dataLiquidacao;
                    }
                    else
                    {
                        titulo.DataLiquidacao = null;
                        titulo.DataBaseLiquidacao = null;
                    }
                    titulo.DataVencimento = dataVencimento;
                    titulo.DataProgramacaoPagamento = dataProgramacaoPagamento;
                    if (dataAutorizacao > DateTime.MinValue)
                        titulo.DataAutorizacao = dataAutorizacao;
                    else
                        titulo.DataAutorizacao = null;
                    titulo.Desconto = desconto;
                    //titulo.DuplicataDocumentoEntrada = repDocumentoEntradaDuplicata.BuscarPorCodigo(duplicataDocumentoEntrada);

                    titulo.Empresa = empresa;

                    //titulo.FaturaParcela = repFaturaParcela.BuscarPorCodigo(faturaParcela);
                    titulo.GrupoPessoas = repGrupoPessoas.BuscarPorCodigo(grupoPessoas);
                    titulo.Historico = historico;
                    //titulo.NotaFiscal = repNotaFiscal.BuscarPorCodigo(notaFiscal);
                    titulo.Observacao = observacao;
                    titulo.ObservacaoInterna = observacaoInterna;
                    titulo.Pessoa = pessoa > 0 ? repCliente.BuscarPorCPFCNPJ(pessoa) : null;
                    if (sequencia > 0)
                        titulo.Sequencia = sequencia;
                    else
                        titulo.Sequencia = 1;
                    titulo.StatusTitulo = statusTitulo;
                    titulo.DataAlteracao = DateTime.Now;
                    titulo.TipoTitulo = tipoTitulo.Value;
                    titulo.FormaTitulo = formaTitulo;
                    titulo.Adiantado = adiantado;
                    //titulo.TituloBaixaNegociacao = repTituloBaixaNegociacao.BuscarPorCodigo(tituloBaixaNegociacao);
                    titulo.ValorOriginal = valorOriginal;
                    titulo.ValorPago = valorPago;
                    titulo.ValorPendente = valorPendente;
                    titulo.NumeroDocumentoTituloOriginal = numeroDocumentoTituloOriginal;
                    titulo.TipoDocumentoTituloOriginal = tipoDocumentoTituloOriginal;
                    titulo.CodigoFavorecido = Request.GetStringParam("CodigoFavorecido");

                    titulo.TributoReferencia = tributoReferencia;
                    titulo.TributoTipoImposto = tributoTipoImposto > 0 ? repTributoTipoImposto.BuscarPorCodigo(tributoTipoImposto) : null;
                    titulo.TributoCodigoReceita = tributoCodigoReceita > 0 ? repTributoCodigoReceita.BuscarPorCodigo(tributoCodigoReceita) : null;
                    titulo.TributoVariacaoImposto = tributoVariacaoImposto > 0 ? repTributoVariacaoImposto.BuscarPorCodigo(tributoVariacaoImposto) : null;
                    titulo.TributoTipoDocumento = tributoTipoDocumento > 0 ? repTributoTipoDocumento.BuscarPorCodigo(tributoTipoDocumento) : null;

                    titulo.MoedaCotacaoBancoCentral = Request.GetNullableEnumParam<MoedaCotacaoBancoCentral>("MoedaCotacaoBancoCentral");
                    titulo.DataBaseCRT = Request.GetNullableDateTimeParam("DataBaseCRT");
                    titulo.ValorMoedaCotacao = Request.GetDecimalParam("ValorMoedaCotacao");
                    titulo.ValorOriginalMoedaEstrangeira = Request.GetDecimalParam("ValorOriginalMoedaEstrangeira");

                    if (portador > 0)
                        titulo.Portador = repCliente.BuscarPorCPFCNPJ(portador);
                    else
                        titulo.Portador = null;
                    if (contribuinte > 0)
                        titulo.Contribuinte = repCliente.BuscarPorCPFCNPJ(contribuinte);
                    else
                        titulo.Contribuinte = null;

                    if (codigoConhecimentoEletronico > 0)
                        titulo.ConhecimentoDeTransporteEletronico = repCTe.BuscarPorCodigo(codigoConhecimentoEletronico);

                    if (liberadoPagamento && !titulo.LiberadoPagamento)
                        titulo.Historico += " OPERADOR " + this.Usuario.Nome + " LIBEROU O PAGAMENTO DO TÍTULO " + DateTime.Now.ToString("dd/MM/yyyy");
                    else if (!liberadoPagamento && titulo.LiberadoPagamento)
                        titulo.Historico += " OPERADOR " + this.Usuario.Nome + " BLOQUEOU O PAGAMENTO DO TÍTULO " + DateTime.Now.ToString("dd/MM/yyyy");
                    titulo.LiberadoPagamento = liberadoPagamento;
                    titulo.AdiantamentoFornecedor = adiantamentoFornecedor;
                    if (titulo.TipoTitulo == TipoTitulo.Pagar)
                        titulo.NossoNumero = nossoNumero;
                    titulo.Integrado = false;

                    if (titulo.Provisao == true && provisao == false)
                        eraProvisao = true;
                    titulo.Provisao = provisao;

                    if (titulo.DataVencimento.Value >= DateTime.Now.Date)//Se o título estava vencido, e foi atualizado para uma data maior, precisa remover do cliente
                        servicoTitulo.RemoverTituloBloqueioFinanceiroPessoa(titulo, TipoServicoMultisoftware);
                }
                else if (titulo.StatusTitulo == StatusTitulo.Quitada || titulo.StatusTitulo == StatusTitulo.Cancelado)
                {
                    titulo.Observacao = observacao;
                    titulo.ObservacaoInterna = observacaoInterna;
                    titulo.DataProgramacaoPagamento = dataProgramacaoPagamento;
                    titulo.Empresa = empresa;
                    titulo.NossoNumero = nossoNumero;
                }

                if (titulo.TipoTitulo == TipoTitulo.Pagar && !string.IsNullOrWhiteSpace(titulo.NossoNumero))
                {
                    if (repTitulo.ContemTituloNossoNumeroDuplicado(titulo.Codigo, titulo.NossoNumero))
                        throw new ControllerException("Já existe um título a pagar lançado com o mesmo número de boleto para o pagamento eletrônico.");
                }

                if (titulo.StatusTitulo == StatusTitulo.EmAberto)
                    SalvarVeiculos(titulo, unitOfWork);

                repTitulo.Atualizar(titulo, Auditado);

                SalvarCentrosResultado(titulo, unitOfWork);
                SalvarCentrosResultadoTiposDespesa(titulo, unitOfWork);
                ExcluirAnexos(unitOfWork);

                lancamentosCentroResultado = repLancamentoCentroResultado.BuscarPorTitulo(titulo.Codigo);

                if ((titulo.TipoMovimento != null && atualizouValorOriginal) ||
                    (titulo.Provisao && configuracaoFinanceiro.MovimentacaoFinanceiraParaTitulosDeProvisao && atualizouValorOriginal) ||
                    eraProvisao)
                {
                    //Reverte o valor antigo
                    if (!reverteuMovimentacao &&
                        ((eraProvisao || titulo.Provisao ) && configuracaoFinanceiro.MovimentacaoFinanceiraParaTitulosDeProvisao)
                        )
                    {
                        if (!servProcessoMovimento.GerarMovimentacao(out string msgErro, null, titulo.DataEmissao.Value.Date, valorAntigo, titulo.Codigo.ToString(), "REVERSÃO DO TÍTULO MANUAL", unitOfWork, TipoDocumentoMovimento.Manual, TipoServicoMultisoftware, 0, titulo.TipoMovimento.PlanoDeContaDebito, titulo.TipoMovimento.PlanoDeContaCredito, titulo.Codigo, null, titulo.Pessoa, titulo.Pessoa?.GrupoPessoas ?? null, null, null, null, null, null, null, null, 0, 0, null, null, titulo.FormaTitulo))
                            throw new ControllerException(msgErro);
                    }

                    //Gera movimento com o novo valor
                    if (lancamentosCentroResultado.Count > 0)
                    {
                        foreach (Dominio.Entidades.Embarcador.Financeiro.LancamentoCentroResultado lancamentoCentroResultado in lancamentosCentroResultado)
                        {
                            if (!servProcessoMovimento.GerarMovimentacao(out string msgErro, titulo.TipoMovimento, titulo.DataEmissao.Value, lancamentoCentroResultado.Valor, titulo.Codigo.ToString(), "GERAÇÃO DO TÍTULO MANUAL " + observacao, unitOfWork, TipoDocumentoMovimento.Manual, TipoServicoMultisoftware, 0, null, null, titulo.Codigo, null, titulo.Pessoa, titulo.Pessoa?.GrupoPessoas ?? null, null, lancamentoCentroResultado.CentroResultado, null, null, null, null, null, 0, 0, null, null, titulo.FormaTitulo))
                                throw new ControllerException(msgErro);
                        }
                    }
                    else
                    {
                        if (!servProcessoMovimento.GerarMovimentacao(out string msgErro, titulo.TipoMovimento, titulo.DataEmissao.Value, titulo.ValorOriginal, titulo.Codigo.ToString(), "GERAÇÃO DO TÍTULO MANUAL " + observacao, unitOfWork, TipoDocumentoMovimento.Manual, TipoServicoMultisoftware, 0, null, null, titulo.Codigo, null, titulo.Pessoa, titulo.Pessoa?.GrupoPessoas ?? null, null, null, null, null, null, null, null, 0, 0, null, null, titulo.FormaTitulo))
                            throw new ControllerException(msgErro);
                    }
                }
                new Servicos.Embarcador.Integracao.IntegracaoTitulo(unitOfWork).IniciarIntegracoesDeTitulos(titulo, TipoAcaoIntegracao.Alteracao);

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
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar.");
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

                Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);
                Repositorio.Embarcador.Financeiro.DocumentoFaturamento repDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(unitOfWork);
                Repositorio.Embarcador.Frota.Infracao repInfracao = new Repositorio.Embarcador.Frota.Infracao(unitOfWork);
                Repositorio.Embarcador.Financeiro.TituloCentroResultadoTipoDespesa repTituloCentroResultadoTipoDespesa = new Repositorio.Embarcador.Financeiro.TituloCentroResultadoTipoDespesa(unitOfWork);
                Repositorio.Embarcador.Financeiro.PagamentoEletronicoTitulo repPagamentoEletronicoTitulo = new Repositorio.Embarcador.Financeiro.PagamentoEletronicoTitulo(unitOfWork);
                Repositorio.Embarcador.Financeiro.AplicarAcrescimoDescontoNoTitulo repAplicarAcrescimoDescontoNoTitulo = new Repositorio.Embarcador.Financeiro.AplicarAcrescimoDescontoNoTitulo(unitOfWork);
                Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = repTitulo.BuscarPorCodigo(codigo);

                if (titulo == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                Dominio.Entidades.Embarcador.Financeiro.TituloDocumento tituloDocumento = titulo.Documentos.FirstOrDefault();
                Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento documentoFaturamento = null;
                Dominio.Entidades.Embarcador.Financeiro.AplicarAcrescimoDescontoNoTitulo acrescimosDescontos = repAplicarAcrescimoDescontoNoTitulo.BuscarPorTitulo(titulo.Codigo);
                if (tituloDocumento != null)
                {
                    if (tituloDocumento.TipoDocumento == TipoDocumentoTitulo.CTe)
                        documentoFaturamento = repDocumentoFaturamento.BuscarPorCTe(tituloDocumento.CTe.Codigo, TipoLiquidacao.Fatura);
                    else
                        documentoFaturamento = repDocumentoFaturamento.BuscarPorCarga(tituloDocumento.Carga.Codigo, TipoLiquidacao.Fatura);
                }

                List<Dominio.Entidades.Embarcador.Financeiro.TituloCentroResultadoTipoDespesa> centrosResultadoTiposDespesa = repTituloCentroResultadoTipoDespesa.BuscarPorTitulo(codigo);

                bool tituloDeInfracao = TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe ? repInfracao.TituloContemInfracao(titulo.Codigo) : false;

                var dynTitulo = new
                {
                    titulo.Codigo,
                    titulo.CodigoRecebidoIntegracao,
                    DocumentoFaturamento = new
                    {
                        Codigo = documentoFaturamento?.Codigo,
                        Descricao = documentoFaturamento?.DescricaoNumeroDocumento
                    },
                    Fatura = titulo.FaturaParcela != null && titulo.FaturaParcela.Fatura != null ? titulo.FaturaParcela.Fatura.Numero.ToString("n0") : string.Empty,
                    titulo.Acrescimo,
                    ContratoFrete = new { Codigo = titulo.ContratoFrete != null ? titulo.ContratoFrete.Codigo : 0, Descricao = titulo.ContratoFrete != null ? titulo.ContratoFrete.NumeroContrato.ToString() : "" },
                    DataEmissao = titulo.DataEmissao.HasValue ? titulo.DataEmissao.Value.ToString("dd/MM/yyyy") : string.Empty,
                    PeriodoApuracao = titulo.PeriodoApuracao.HasValue ? titulo.PeriodoApuracao.Value.ToString("dd/MM/yyyy") : string.Empty,
                    DataLiquidacao = titulo.DataLiquidacao.HasValue ? titulo.DataLiquidacao.Value.ToString("dd/MM/yyyy") : string.Empty,
                    DataVencimento = titulo.DataVencimento.HasValue ? titulo.DataVencimento.Value.ToString("dd/MM/yyyy") : string.Empty,
                    DataProgramacaoPagamento = titulo.DataProgramacaoPagamento.HasValue ? titulo.DataProgramacaoPagamento.Value.ToString("dd/MM/yyyy") : string.Empty,
                    DataAutorizacao = (acrescimosDescontos?.DataAutorizacao ?? DateTime.MinValue) != DateTime.MinValue ? acrescimosDescontos?.DataAutorizacao?.ToString("dd/MM/yyyy") : titulo.DataAutorizacao.HasValue ? titulo.DataAutorizacao.Value.ToString("dd/MM/yyyy") : string.Empty,
                    titulo.Desconto,
                    DuplicataDocumentoEntrada = new { Codigo = titulo.DuplicataDocumentoEntrada != null ? titulo.DuplicataDocumentoEntrada.Codigo : 0, Descricao = titulo.DuplicataDocumentoEntrada != null ? titulo.DuplicataDocumentoEntrada.Numero.ToString() : "" },
                    Empresa = new { Codigo = titulo.Empresa != null ? titulo.Empresa.Codigo : 0, Descricao = titulo.Empresa != null ? titulo.Empresa.RazaoSocial : "" },
                    FaturaParcela = new { Codigo = titulo.FaturaParcela != null ? titulo.FaturaParcela.Codigo : 0, Descricao = titulo.FaturaParcela != null ? titulo.FaturaParcela.Fatura.Numero.ToString() : "" },
                    GrupoPessoas = new { Codigo = titulo.GrupoPessoas != null ? titulo.GrupoPessoas.Codigo : 0, Descricao = titulo.GrupoPessoas != null ? titulo.GrupoPessoas.Descricao : "" },
                    titulo.Historico,
                    titulo.Observacao,
                    titulo.ObservacaoInterna,
                    Pessoa = new { Codigo = titulo.Pessoa != null ? titulo.Pessoa.Codigo : 0, Descricao = titulo.Pessoa != null ? titulo.Pessoa.Descricao : "" },
                    titulo.Sequencia,
                    titulo.StatusTitulo,
                    titulo.TipoTitulo,
                    TituloBaixaNegociacao = new { Codigo = titulo.TituloBaixaNegociacao != null ? titulo.TituloBaixaNegociacao.Codigo : 0, Descricao = titulo.TituloBaixaNegociacao != null ? titulo.TituloBaixaNegociacao.Codigo.ToString() : "" },
                    titulo.ValorOriginal,
                    titulo.ValorPago,
                    titulo.ValorPendente,
                    titulo.NossoNumero,
                    titulo.BoletoStatusTitulo,
                    titulo.AdiantamentoFornecedor,
                    TipoMovimento = new { Codigo = titulo.TipoMovimento?.Codigo ?? 0, Descricao = titulo.TipoMovimento?.Descricao ?? "" },
                    ConhecimentoEletronico = new { Codigo = titulo.ConhecimentoDeTransporteEletronico != null ? titulo.ConhecimentoDeTransporteEletronico.Codigo : 0, Descricao = titulo.ConhecimentoDeTransporteEletronico != null ? titulo.ConhecimentoDeTransporteEletronico.Chave : "" },
                    titulo.LiberadoPagamento,
                    DataBaseLiquidacao = titulo.DataBaseLiquidacao.HasValue && titulo.StatusTitulo == StatusTitulo.Quitada ? titulo.DataBaseLiquidacao.Value.ToString("dd/MM/yyyy") : string.Empty,
                    titulo.NumeroDocumentoTituloOriginal,
                    titulo.TipoDocumentoTituloOriginal,
                    titulo.Adiantado,
                    titulo.TributoReferencia,
                    TributoTipoImposto = new { Codigo = titulo.TributoTipoImposto != null ? titulo.TributoTipoImposto.Codigo : 0, Descricao = titulo.TributoTipoImposto != null ? titulo.TributoTipoImposto.DescricaoCompleta : "" },
                    TributoCodigoReceita = new { Codigo = titulo.TributoCodigoReceita != null ? titulo.TributoCodigoReceita.Codigo : 0, Descricao = titulo.TributoCodigoReceita != null ? titulo.TributoCodigoReceita.DescricaoCompleta : "" },
                    TributoVariacaoImposto = new { Codigo = titulo.TributoVariacaoImposto != null ? titulo.TributoVariacaoImposto.Codigo : 0, Descricao = titulo.TributoVariacaoImposto != null ? titulo.TributoVariacaoImposto.DescricaoCompleta : "" },
                    TributoTipoDocumento = new { Codigo = titulo.TributoTipoDocumento != null ? titulo.TributoTipoDocumento.Codigo : 0, Descricao = titulo.TributoTipoDocumento != null ? titulo.TributoTipoDocumento.DescricaoCompleta : "" },
                    Portador = new { Codigo = titulo.Portador != null ? titulo.Portador.Codigo : 0, Descricao = titulo.Portador != null ? titulo.Portador.Descricao : "" },
                    Contribuinte = new { Codigo = titulo.Contribuinte != null ? titulo.Contribuinte.Codigo : 0, Descricao = titulo.Contribuinte != null ? titulo.Contribuinte.Nome : "" },
                    ValorSaldo = titulo.StatusTitulo == StatusTitulo.Quitada ? 0.ToString("n2") : (titulo.ValorOriginal - titulo.Desconto + titulo.Acrescimo).ToString("n2"),
                    NumeroDocumento = titulo.NumeroDocumentoTituloOriginal,
                    titulo.FormaTitulo,
                    titulo.Provisao,
                    titulo.LinhaDigitavelBoleto,
                    HabilitarLinhaDigitavelBoleto = !string.IsNullOrWhiteSpace(titulo.LinhaDigitavelBoleto),
                    CentrosResultado = (from obj in titulo.LancamentosCentroResultado
                                        select new
                                        {
                                            obj.Codigo,
                                            CentroResultado = obj.CentroResultado?.Descricao ?? string.Empty,
                                            CodigoCentroResultado = obj.CentroResultado?.Codigo ?? 0,
                                            Percentual = new { val = obj.Percentual, tipo = "decimal" }
                                        }).ToList(),
                    CentrosResultadoTiposDespesa = (from obj in centrosResultadoTiposDespesa
                                                    select new
                                                    {
                                                        obj.Codigo,
                                                        CentroResultado = new { obj.CentroResultado?.Codigo, obj.CentroResultado?.Descricao },
                                                        TipoDespesa = new { obj.TipoDespesaFinanceira?.Codigo, obj.TipoDespesaFinanceira?.Descricao },
                                                        Percentual = new { val = obj.Percentual, tipo = "decimal" }
                                                    }).ToList(),
                    Veiculos = (from obj in titulo.Veiculos
                                select new
                                {
                                    obj.Codigo,
                                    obj.Placa,
                                    ModeloVeicularCarga = obj.ModeloVeicularCarga?.Descricao,
                                    obj.NumeroFrota
                                }).ToList(),
                    titulo.MoedaCotacaoBancoCentral,
                    DataBaseCRT = titulo.DataBaseCRT.HasValue ? titulo.DataBaseCRT.Value.ToString("dd/MM/yyyy HH:mm") : "",
                    ValorMoedaCotacao = titulo.ValorMoedaCotacao.ToString("n10"),
                    ValorOriginalMoedaEstrangeira = titulo.ValorOriginalMoedaEstrangeira.ToString("n2"),
                    TituloDeInfracao = tituloDeInfracao,
                    titulo.CodigoFavorecido,
                    ContemRemessaPagamento = titulo.TipoTitulo == TipoTitulo.Pagar ? repPagamentoEletronicoTitulo.ContemRemessaPorTitulo(titulo.Codigo) : false,
                    DescontoAplicadoNegociacao = titulo.DescontoAplicadoNegociacao.ToString("n2"),
                    TipoChavePixPessoa = titulo.Pessoa?.TipoChavePix != null ? titulo.Pessoa?.TipoChavePix : 0,
                    ChavePixPessoa = titulo.Pessoa?.ChavePix != null ? titulo.Pessoa?.ChavePix : string.Empty,
                    ListaAnexos = titulo.Anexos != null ? (from obj in titulo.Anexos
                                                                          select new
                                                                          {
                                                                              obj.Codigo,
                                                                              DescricaoAnexo = obj.Descricao,
                                                                              Arquivo = obj.NomeArquivo
                                                                          }).ToList() : null
                };

                return new JsonpResult(dynTitulo);
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

        public async Task<IActionResult> ExcluirPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Servicos.Embarcador.Financeiro.Titulo servicoTitulo = new Servicos.Embarcador.Financeiro.Titulo(unitOfWork, TipoServicoMultisoftware, Auditado);

                unitOfWork.Start();

                servicoTitulo.CancelarTitulo(codigo);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ServicoException ex)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, ex.Message);
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
                return new JsonpResult(false, "Ocorreu uma falha ao cancelar o título.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> GerarBoleto()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);
                Repositorio.Embarcador.Financeiro.BoletoConfiguracao repBoletoConfiguracao = new Repositorio.Embarcador.Financeiro.BoletoConfiguracao(unitOfWork);

                int codigoBoletoConfiguracao = 0;
                int.TryParse(Request.Params("Codigo"), out codigoBoletoConfiguracao);

                int codigoTitulo = 0;
                int.TryParse(Utilidades.String.OnlyNumbers(Request.Params("CodigoTitulo")), out codigoTitulo);

                Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = repTitulo.BuscarPorCodigo(codigoTitulo);

                if (titulo == null)
                    return new JsonpResult(false, "Título não encontrado.");
                if (!string.IsNullOrWhiteSpace(titulo.NossoNumero))
                    return new JsonpResult(false, "O título já possui boleto gerado.");

                titulo.BoletoConfiguracao = repBoletoConfiguracao.BuscarPorCodigo(codigoBoletoConfiguracao);
                titulo.BoletoStatusTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.BoletoStatusTitulo.Emitido;

                Servicos.Embarcador.Financeiro.Titulo servTitulo = new Servicos.Embarcador.Financeiro.Titulo(unitOfWork);
                servTitulo.IntegrarEmitido(titulo, unitOfWork);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, titulo, null, "Atualizado título.", unitOfWork);
                repTitulo.Atualizar(titulo);

                if (titulo.FormaTitulo == FormaTitulo.Boleto)
                    new Servicos.Embarcador.Integracao.IntegracaoTitulo(unitOfWork).IniciarIntegracoesDeTitulosAReceber(titulo, TipoAcaoIntegracao.Criacao);


                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> LimparDadosBoleto()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);

                Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = repTitulo.BuscarPorCodigo(int.Parse(Request.Params("CodigoTitulo")));

                if (titulo == null)
                    return new JsonpResult(false, "Título não encontrado!");

                Servicos.Embarcador.Financeiro.BoletoRemessa svcBoletoRemessa = new Servicos.Embarcador.Financeiro.BoletoRemessa(unitOfWork);
                svcBoletoRemessa.GerarRemessaDeCancelamento(titulo, unitOfWork, Auditado, this.Usuario.Empresa);

                Servicos.Embarcador.Financeiro.Titulo.LimparDadosBoleto(titulo, this.Usuario, unitOfWork, Auditado);

                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> LimparDadosRemessa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);

                Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = repTitulo.BuscarPorCodigo(int.Parse(Request.Params("CodigoTitulo")));

                titulo.BoletoStatusTitulo = BoletoStatusTitulo.Gerado;
                titulo.Historico += " BOLETO REMOVIDO DA REMESSA " + this.Usuario.Nome + " " + DateTime.Now.ToString("dd/MM/yyyy");
                if (titulo.BoletoRemessa != null)
                    titulo.Historico += " REMESSA ANTERIOR: " + titulo.BoletoRemessa.NumeroSequencial.ToString();

                titulo.BoletoRemessa = null;

                Servicos.Auditoria.Auditoria.Auditar(Auditado, titulo, null, "Atualizado título.", unitOfWork);
                repTitulo.Atualizar(titulo);

                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> EnviarArquivoParaEmissao()
        {

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();
                List<RetornoArquivo> retornoArquivos = new List<RetornoArquivo>();

                List<Servicos.DTO.CustomFile> files = HttpContext.GetFiles();

                if (files.Count > 0)
                {
                    for (int i = 0; i < files.Count; i++)
                    {
                        Servicos.DTO.CustomFile file = files[i];
                        string extensao = System.IO.Path.GetExtension(file.FileName).ToLower();
                        string nomeArquivo = System.IO.Path.GetFileName(file.FileName);

                        RetornoArquivo retornoArquivo = new RetornoArquivo();
                        retornoArquivo.nome = file.FileName;
                        retornoArquivo.processada = true;
                        retornoArquivo.mensagem = "";
                        retornoArquivo.codigo = 0;

                        if (extensao.Equals(".xml"))
                        {
                            try
                            {
                                var objCTe = MultiSoftware.CTe.Servicos.Leitura.Ler(file.InputStream);
                                if (objCTe != null)
                                {
                                    string retorno = ProcessarXMLCTe(objCTe, file.InputStream, unitOfWork);
                                    int codigoCTe = 0;
                                    int.TryParse(retorno, out codigoCTe);
                                    if (codigoCTe > 0)
                                    {
                                        retorno = "";
                                        retornoArquivo.codigo = codigoCTe;
                                    }
                                    if (!string.IsNullOrEmpty(retorno))
                                    {
                                        retornoArquivo.processada = false;
                                        retornoArquivo.mensagem = retorno;
                                    }
                                }
                                else
                                {
                                    retornoArquivo.processada = false;
                                    retornoArquivo.mensagem = "O xml informado não é uma NF-e ou um CT-e, por favor verifique.";
                                }
                            }
                            catch (Exception ex)
                            {
                                Servicos.Log.TratarErro(ex);
                                retornoArquivo.processada = false;
                                retornoArquivo.mensagem = "Ocorreu uma falha ao enviar o xml, verifique se o arquivo é um documento fiscal válido. " + ex.Message;
                            }
                            finally
                            {
                                file.InputStream.Dispose();
                            }
                        }
                        else
                        {
                            retornoArquivo.processada = false;
                            retornoArquivo.mensagem = "A extensão do arquivo é inválida.";
                        }
                        retornoArquivos.Add(retornoArquivo);
                    }

                    unitOfWork.CommitChanges();
                    var dadosRetorno = new
                    {
                        Arquivos = retornoArquivos
                    };
                    return new JsonpResult(dadosRetorno);
                }
                else
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, "Não foi enviado o arquivo.");
                }

            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao enviar o CT-e.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadBoleto()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int.TryParse(Utilidades.String.OnlyNumbers(Request.Params("Codigo")), out int codigoTitulo);
                int.TryParse(Utilidades.String.OnlyNumbers(Request.Params("CodigoFatura")), out int codigoFatura);

                Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);

                if (codigoFatura > 0 && codigoTitulo == 0)
                {
                    var titulo = repTitulo.BuscarPorFatura(codigoFatura).FirstOrDefault();
                    codigoTitulo = titulo?.Codigo ?? 0;
                }

                if (codigoTitulo > 0)
                {
                    Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = repTitulo.BuscarPorCodigo(codigoTitulo);

                    titulo = titulo ?? repTitulo.BuscarPorCodigoRecebidoIntegracao(codigoTitulo);

                    string caminhoArquivo = string.IsNullOrWhiteSpace(titulo?.CaminhoBoletoIntegracao) ? titulo?.CaminhoBoleto : titulo?.CaminhoBoletoIntegracao;

                    if (titulo != null && !string.IsNullOrWhiteSpace(caminhoArquivo))
                    {
                        try
                        {
                            if (Utilidades.IO.FileStorageService.Storage.Exists(caminhoArquivo))
                                return Arquivo(Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminhoArquivo), "application/x-pkcs12", titulo.Codigo.ToString() + ".pdf");
                            else
                                return new JsonpResult(false, "O arquivo do boleto " + caminhoArquivo + " não foi encontrado.");
                        }
                        catch (Exception ex)
                        {
                            Servicos.Log.TratarErro(ex);

                            return new JsonpResult(false, "Ocorreu uma falha ao realizar o download do pdf do boleto.");
                        }
                    }
                    else
                        return new JsonpResult(false, true, "Este título não possui o pdf disponível para download.");
                }
                return new JsonpResult(false, true, "Título não encontrado");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao realizar o download do PDF.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadRemessa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoTitulo = 0;
                int.TryParse(Request.Params("Codigo"), out codigoTitulo);

                if (codigoTitulo > 0)
                {
                    Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);
                    Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = repTitulo.BuscarPorCodigo(codigoTitulo);

                    if (titulo != null && titulo.BoletoRemessa != null && !string.IsNullOrWhiteSpace(titulo.BoletoRemessa.Observacao))
                    {
                        try
                        {
                            if (Utilidades.IO.FileStorageService.Storage.Exists(titulo.BoletoRemessa.Observacao))
                                return Arquivo(Utilidades.IO.FileStorageService.Storage.ReadAllBytes(titulo.BoletoRemessa.Observacao), "application/x-pkcs12", System.IO.Path.GetFileName(titulo.BoletoRemessa.Observacao));
                            else
                                return new JsonpResult(false, "O arquivo da remessa " + titulo.BoletoRemessa.Observacao + " não foi encontrado.");
                        }
                        catch (Exception ex)
                        {
                            Servicos.Log.TratarErro(ex);

                            return new JsonpResult(false, "Ocorreu uma falha ao realizar o download do txt do remessa.");
                        }
                    }
                    else
                        return new JsonpResult(false, true, "Este boleto não possui o txt disponível para download.");
                }
                return new JsonpResult(false, true, "Título não encontrado");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao realizar o download da remessa.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ConsultarDocumentos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int.TryParse(Request.Params("Codigo"), out int codigoTitulo);

                Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);

                Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = codigoTitulo > 0 ? repTitulo.BuscarPorCodigo(codigoTitulo) : null;

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Documento", "NumeroDocumento", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Valor Documento", "ValorDocumento", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Valor no Título", "Valor", 10, Models.Grid.Align.left, true);

                if (titulo?.MoedaCotacaoBancoCentral != null && titulo.MoedaCotacaoBancoCentral != MoedaCotacaoBancoCentral.Real)
                    grid.AdicionarCabecalho("Vl. Moeda no Título", "ValorMoeda", 10, Models.Grid.Align.left, true);

                grid.AdicionarCabecalho("Acréscimo Geração", "ValorAcrescimo", 8, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Desconto Geração", "ValorDesconto", 8, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Total", "ValorTotal", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Acréscimo Baixa", "ValorAcrescimoBaixa", 8, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Desconto Baixa", "ValorDescontoBaixa", 8, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Valor Pendente", "ValorPendente", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Valor Pago", "ValorPago", 10, Models.Grid.Align.left, true);

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;

                Repositorio.Embarcador.Financeiro.TituloDocumento repTituloDocumento = new Repositorio.Embarcador.Financeiro.TituloDocumento(unitOfWork);

                List<Dominio.Entidades.Embarcador.Financeiro.TituloDocumento> listaDocumentos = repTituloDocumento.ConsultarPorTitulo(codigoTitulo, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);

                grid.setarQuantidadeTotal(repTituloDocumento.ContarConsultaPorTitulo(codigoTitulo));

                var lista = (from obj in listaDocumentos
                             select new
                             {
                                 obj.Codigo,
                                 obj.NumeroDocumento,
                                 ValorDocumento = (obj.TipoDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoTitulo.CTe ? obj.CTe.ValorAReceber : obj.Carga.ValorFreteAPagar).ToString("n2"),
                                 Valor = obj.Valor.ToString("n2"),
                                 ValorAcrescimo = obj.ValorAcrescimo.ToString("n2"),
                                 ValorAcrescimoBaixa = obj.ValorAcrescimoBaixa.ToString("n2"),
                                 ValorDesconto = obj.ValorDesconto.ToString("n2"),
                                 ValorDescontoBaixa = obj.ValorDescontoBaixa.ToString("n2"),
                                 ValorTotal = obj.ValorTotal.ToString("n2"),
                                 ValorPendente = obj.ValorPendente.ToString("n2"),
                                 ValorPago = obj.ValorPago.ToString("n2"),
                                 ValorMoeda = obj.ValorMoeda.ToString("n2")
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

        public async Task<IActionResult> BaixarRelatorioAutorizacaoPagamento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Relatorios.Relatorio repRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork);
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = repRelatorio.BuscarPadraoPorCodigoControleRelatorio(Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R209_AutorizacaoPagamentoTitulo, TipoServicoMultisoftware);
                Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);

                string titulo = ConfiguracaoEmbarcador.UsarReciboPagamentoGeracaoAutorizacaoTitulo ? "Recibo de Pagamento de Título" : "Autorização de Pagamento de Título";
                string rptTitulo = ConfiguracaoEmbarcador.UsarReciboPagamentoGeracaoAutorizacaoTitulo ? "ReciboPagamentoTitulo.rpt" : "AutorizacaoPagamentoTitulo.rpt";

                if (relatorio == null)
                    relatorio = serRelatorio.BuscarConfiguracaoPadrao(Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R209_AutorizacaoPagamentoTitulo, TipoServicoMultisoftware, titulo, "Financeiros", rptTitulo, Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "Codigo", "desc", "", "", 0, unitOfWork, false, false);
                else
                    relatorio.Titulo = ConfiguracaoEmbarcador.UsarReciboPagamentoGeracaoAutorizacaoTitulo ? "Recibo de Pagamento de Título" : "Autorização de Pagamento de Título";

                int codigo = Request.GetIntParam("Codigo");

                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = serRelatorio.AdicionarRelatorioParaGeracao(relatorio, this.Usuario, Dominio.Enumeradores.TipoArquivoRelatorio.PDF, unitOfWork);

                string stringConexao = _conexao.StringConexao;
                string nomeCliente = Cliente.NomeFantasia;
                IList<Dominio.Relatorios.Embarcador.DataSource.Financeiros.AutorizacaoPagamentoTitulo> dadosTitulo = repTitulo.RelatorioAutorizacaoPagamentoTitulo(codigo, this.Usuario.Nome);
                if (dadosTitulo.Count > 0)
                {
                    Task.Factory.StartNew(() => GerarRelatorioAutorizacaoPagamento(nomeCliente, stringConexao, relatorioControleGeracao, dadosTitulo, rptTitulo));
                    return new JsonpResult(true);
                }
                else
                    return new JsonpResult(false, false, "Nenhum registro de título para regar o relatório.");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao gerar o relatório.");
            }
        }

        public async Task<IActionResult> LimparRemessaPagamento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);
                Repositorio.Embarcador.Financeiro.PagamentoEletronicoTitulo repPagamentoEletronicoTitulo = new Repositorio.Embarcador.Financeiro.PagamentoEletronicoTitulo(unitOfWork);
                Repositorio.Embarcador.Financeiro.PagamentoEletronico repPagamentoEletronico = new Repositorio.Embarcador.Financeiro.PagamentoEletronico(unitOfWork);
                Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = repTitulo.BuscarPorCodigo(codigo);

                List<int> codigosPagamentosTitulos = repPagamentoEletronicoTitulo.BuscarTodosPorTitulo(codigo);
                if (codigosPagamentosTitulos != null && codigosPagamentosTitulos.Count > 0)
                {
                    foreach (var codigoPagamento in codigosPagamentosTitulos)
                    {
                        Dominio.Entidades.Embarcador.Financeiro.PagamentoEletronicoTitulo pagamentoEletronicoTitulo = repPagamentoEletronicoTitulo.BuscarPorCodigo(codigoPagamento);
                        Dominio.Entidades.Embarcador.Financeiro.PagamentoEletronico pagamentoEletronico = repPagamentoEletronico.BuscarPorCodigo(pagamentoEletronicoTitulo.PagamentoEletronico?.Codigo ?? 0, true);
                        repPagamentoEletronicoTitulo.Deletar(pagamentoEletronicoTitulo);

                        pagamentoEletronico.QuantidadeTitulos -= 1;
                        pagamentoEletronico.ValorTotal -= titulo.ValorOriginal;
                        repPagamentoEletronico.Atualizar(pagamentoEletronico, Auditado);
                    }
                }

                Servicos.Auditoria.Auditoria.Auditar(Auditado, titulo, "Realizou a limpeza das remessas de pagamentos geradas.", unitOfWork);

                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao limpar as remessas de pagamento.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> EnviarAnexos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.Financeiro.Titulo repTituloFinanceiro = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);
                Repositorio.Embarcador.Financeiro.TituloAnexo repTituloFinanceiroAnexo = new Repositorio.Embarcador.Financeiro.TituloAnexo(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");

                Dominio.Entidades.Embarcador.Financeiro.Titulo tituloFinanceiro = repTituloFinanceiro.BuscarPorCodigo(codigo);

                if (tituloFinanceiro == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                IList<Servicos.DTO.CustomFile> arquivos = HttpContext.GetFiles("Arquivo");
                string[] descricoes = Request.GetStringParam("Descricao").Split(',');

                if (arquivos.Count <= 0)
                    return new JsonpResult(false, true, "Nenhum arquivo selecionado para envio.");

                string caminhoSave = Utilidades.IO.FileStorageService.Storage.Combine(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().Anexos, "TituloFinanceiro");

                for (var i = 0; i < arquivos.Count; i++)
                {
                    Dominio.Entidades.Embarcador.Financeiro.TituloAnexo tituloFinanceiroAnexo = new Dominio.Entidades.Embarcador.Financeiro.TituloAnexo();

                    Servicos.DTO.CustomFile file = arquivos[i];

                    var nomeArquivo = file.FileName;
                    var guidArquivo = Guid.NewGuid().ToString().Replace("-", "");
                    var extensaoArquivo = Path.GetExtension(nomeArquivo).ToLower();

                    string caminho = Utilidades.IO.FileStorageService.Storage.Combine(caminhoSave, guidArquivo + extensaoArquivo);
                    file.SaveAs(caminho);

                    tituloFinanceiroAnexo.CaminhoArquivo = caminho;
                    tituloFinanceiroAnexo.NomeArquivo = Utilidades.String.RemoveAllSpecialCharacters(Utilidades.String.RemoveDiacritics(Path.GetFileName(nomeArquivo)));
                    tituloFinanceiroAnexo.Descricao = descricoes[i];
                    tituloFinanceiroAnexo.Titulo = tituloFinanceiro;

                    repTituloFinanceiroAnexo.Inserir(tituloFinanceiroAnexo, Auditado);
                }

                unitOfWork.CommitChanges();

                object retorno = new
                {
                    tituloFinanceiro.Codigo,
                    TipoDocumento = tituloFinanceiro.Observacao,
                    NumeroDocumento = tituloFinanceiro.NumeroFatura,
                    Valor = tituloFinanceiro.ValorTotal.ToString("n2")
                };
                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao anexar o(s) arquivo(s).");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> DownloadAnexo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoAnexo = Request.GetIntParam("CodigoAnexo");

                Repositorio.Embarcador.Financeiro.TituloAnexo repTituloFinanceiroAnexo = new Repositorio.Embarcador.Financeiro.TituloAnexo(unitOfWork);
                Dominio.Entidades.Embarcador.Financeiro.TituloAnexo tituloFinanceiroAnexo = repTituloFinanceiroAnexo.BuscarPorCodigo(codigoAnexo);

                if (tituloFinanceiroAnexo == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (!Utilidades.IO.FileStorageService.Storage.Exists(tituloFinanceiroAnexo.CaminhoArquivo))
                    return new JsonpResult(false, true, "Anexo não encontrado no Servidor.");

                return Arquivo(Utilidades.IO.FileStorageService.Storage.ReadAllBytes(tituloFinanceiroAnexo.CaminhoArquivo), "application/octet-stream", tituloFinanceiroAnexo.NomeArquivo);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao fazer download do anexo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
        #endregion

        #region Métodos Privados

        private dynamic ExecutaPesquisa(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaTituloFinanceiro filtrosPesquisa = ObterFiltrosPesquisa();
            Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);

            List<Dominio.Entidades.Embarcador.Financeiro.Titulo> listaTitulo = repTitulo.Consultar(filtrosPesquisa, propOrdenar, dirOrdena, inicio, limite);
            totalRegistros = repTitulo.ContarConsulta(filtrosPesquisa);

            var lista = (from p in listaTitulo
                         select new
                         {
                             p.Codigo,
                             Fatura = p.FaturaParcela?.Fatura?.Numero.ToString("n0") ?? string.Empty,
                             p.NumeroDocumentoTituloOriginal,
                             DescricaoTipo = p.TipoTitulo.ObterDescricao(),
                             DescricaoSituacao = p.StatusTitulo.ObterDescricao(),
                             p.Sequencia,
                             Pessoa = p.Pessoa != null ? p.Pessoa.Nome + " (" + p.Pessoa.CPF_CNPJ_Formatado + ")" : string.Empty,
                             Valor = p.ValorOriginal.ToString("n2"),
                             DataEmissao = p.DataEmissao?.ToString("dd/MM/yyyy") ?? string.Empty,
                             DataVencimento = p.DataVencimento?.ToString("dd/MM/yyyy") ?? string.Empty,
                             ValorDesconto = p.Desconto.ToString("n2"),
                             ValorAcrescimo = p.Acrescimo.ToString("n2"),
                             ValorPago = p.ValorPago.ToString("n2"),
                             ValorSaldo = p.StatusTitulo == StatusTitulo.Quitada ? 0.ToString("n2") : (p.ValorOriginal - p.Desconto + p.Acrescimo).ToString("n2"),
                             p.NossoNumero,
                             p.Observacao,
                             Remessa = p.TipoTitulo == TipoTitulo.Receber ? (p.BoletoRemessa?.NumeroSequencial.ToString("n0") ?? string.Empty) : p.NumeroPagamentoDigital,
                             p.ObservacaoInterna,
                             MoedaCotacaoBancoCentral = p.MoedaCotacaoBancoCentral?.ObterDescricao() ?? string.Empty,
                             ValorOriginalMoedaEstrangeira = p.ValorOriginalMoedaEstrangeira.ToString("n2"),
                             Renegociado = p.StatusTitulo == StatusTitulo.Quitada && (p.ValorPago == 0 || p.ValorPago < (p.ValorOriginal - p.Desconto + p.Acrescimo)) ? "Sim" : "Não",
                             p.Descricao
                         }).ToList();

            return lista.ToList();
        }

        private void GerarRelatorioAutorizacaoPagamento(string nomeEmpresa, string stringConexao, Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao, IList<Dominio.Relatorios.Embarcador.DataSource.Financeiros.AutorizacaoPagamentoTitulo> dadosTitulo, string rptTitulo)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);
            Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);

            try
            {
                ReportRequest.WithType(ReportType.TituloFinanceiro)
                    .WithExecutionType(ExecutionType.Async)
                    .AddExtraData("NomeEmpresa", nomeEmpresa)
                    .AddExtraData("RptTitulo", rptTitulo)
                    .AddExtraData("DadosTitulo", dadosTitulo.ToJson())
                    .AddExtraData("RelatorioControleGeracao", relatorioControleGeracao.Codigo)
                    .AddExtraData("CodigoEmpresa", Empresa.Codigo)
                    .CallReport();
            }
            catch (Exception ex)
            {
                serRelatorio.RegistrarFalhaGeracaoRelatorio(relatorioControleGeracao, unitOfWork, ex);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private Models.Grid.Grid GridPesquisa()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid(Request);
            grid.header = new List<Models.Grid.Head>();

            grid.AdicionarCabecalho("Código", "Codigo", 6, Models.Grid.Align.right, true);

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
            {
                grid.AdicionarCabecalho("Fatura", false);
                grid.AdicionarCabecalho("Doc. Original", "NumeroDocumentoTituloOriginal", 10, Models.Grid.Align.left, true);
            }
            else
            {
                grid.AdicionarCabecalho("Doc.", "NumeroDocumentoTituloOriginal", 6, Models.Grid.Align.left, false, true);
                grid.AdicionarCabecalho("Fatura", "Fatura", 6, Models.Grid.Align.left, true);
            }

            grid.AdicionarCabecalho("Tipo", "DescricaoTipo", 8, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Situação", "DescricaoSituacao", 8, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Renegociado", "Renegociado", 5, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Seq.", "Sequencia", 5, Models.Grid.Align.right, true);
            grid.AdicionarCabecalho("Pessoa", "Pessoa", 15, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Valor", "Valor", 9, Models.Grid.Align.right, true);
            grid.AdicionarCabecalho("Emissão", "DataEmissao", 8, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Vencimento", "DataVencimento", 8, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Desconto", "ValorDesconto", 6, Models.Grid.Align.right, false);
            grid.AdicionarCabecalho("Acréscimo", "ValorAcrescimo", 6, Models.Grid.Align.right, false);
            grid.AdicionarCabecalho("Valor Pago", "ValorPago", 7, Models.Grid.Align.right, false);
            grid.AdicionarCabecalho("Saldo", "ValorSaldo", 7, Models.Grid.Align.right, false);

            if (ConfiguracaoEmbarcador.UtilizaMoedaEstrangeira)
            {
                grid.AdicionarCabecalho("Moeda", "MoedaCotacaoBancoCentral", 6, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Valor Original Moeda", "ValorOriginalMoedaEstrangeira", 6, Models.Grid.Align.right, true);
            }

            grid.AdicionarCabecalho("Nº Boleto", "NossoNumero", 6, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Remessa", "Remessa", 6, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Observação", "Observacao", 8, Models.Grid.Align.left, true);

            if (ConfiguracaoEmbarcador.UtilizaMoedaEstrangeira)
                grid.AdicionarCabecalho("Obs. Interna", "ObservacaoInterna", 6, Models.Grid.Align.left, true);

            grid.AdicionarCabecalho("Descricao", false);
            return grid;
        }

        private Models.Grid.Grid GridPesquisaConsulta()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid(Request);
            grid.header = new List<Models.Grid.Head>();

            grid.AdicionarCabecalho("Código", "Codigo", 6, Models.Grid.Align.right, true);

            grid.AdicionarCabecalho("Doc.", "NumeroDocumentoTituloOriginal", 6, Models.Grid.Align.left, false, true);

            grid.AdicionarCabecalho("Pessoa", "Pessoa", 15, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Valor", "Valor", 9, Models.Grid.Align.right, true);
            grid.AdicionarCabecalho("Emissão", "DataEmissao", 8, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Vencimento", "DataVencimento", 8, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Desconto", "ValorDesconto", 6, Models.Grid.Align.right, false);
            grid.AdicionarCabecalho("Acréscimo", "ValorAcrescimo", 6, Models.Grid.Align.right, false);

            grid.AdicionarCabecalho("Descricao", false);
            return grid;
        }


        private void SalvarCentrosResultado(Dominio.Entidades.Embarcador.Financeiro.Titulo titulo, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Financeiro.LancamentoCentroResultado repLancamentoCentroResultado = new Repositorio.Embarcador.Financeiro.LancamentoCentroResultado(unitOfWork);
            Repositorio.Embarcador.Financeiro.CentroResultado repCentroResultado = new Repositorio.Embarcador.Financeiro.CentroResultado(unitOfWork);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);

            dynamic centrosResultado = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("CentrosResultado"));

            if (titulo.LancamentosCentroResultado != null && titulo.LancamentosCentroResultado.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (dynamic centroResultado in centrosResultado)
                {
                    int codigo = 0;
                    if (centroResultado.Codigo != null && int.TryParse((string)centroResultado.Codigo, out codigo) && codigo > 0)
                        codigos.Add(codigo);
                }

                List<Dominio.Entidades.Embarcador.Financeiro.LancamentoCentroResultado> centrosResultadoDeletar = (from obj in titulo.LancamentosCentroResultado where !codigos.Contains(obj.Codigo) select obj).ToList();

                for (var i = 0; i < centrosResultadoDeletar.Count; i++)
                {
                    Dominio.Entidades.Embarcador.Financeiro.LancamentoCentroResultado centroResultado = centrosResultadoDeletar[i];

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, titulo, "Removeu o centro de resultados " + centroResultado.Descricao, unitOfWork);

                    repLancamentoCentroResultado.Deletar(centroResultado, Auditado);
                }
            }

            decimal valorTotalRateado = 0m, percentual = 0m;
            int countCentroResultado = centrosResultado.Count;

            for (int i = 0; i < countCentroResultado; i++)
            {
                dynamic centroResultado = centrosResultado[i];

                int codigoCentro = 0, codigoCentroResultado = 0;
                int.TryParse((string)centroResultado.Codigo, out codigoCentro);
                int.TryParse((string)centroResultado.CodigoCentroResultado, out codigoCentroResultado);

                Dominio.Entidades.Embarcador.Financeiro.LancamentoCentroResultado lancamento = codigoCentro > 0 ? repLancamentoCentroResultado.BuscarPorCodigo(codigoCentro, true) : null;

                if (lancamento == null)
                    lancamento = new Dominio.Entidades.Embarcador.Financeiro.LancamentoCentroResultado();

                lancamento.TipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoLancamentoCentroResultado.Titulo;
                lancamento.Ativo = true;
                lancamento.Titulo = titulo;
                lancamento.Data = DateTime.Now;
                lancamento.CentroResultado = codigoCentroResultado > 0 ? repCentroResultado.BuscarPorCodigo(codigoCentroResultado) : null;
                lancamento.Percentual = Utilidades.Decimal.Converter((string)centroResultado.Percentual);

                if ((i + 1) == countCentroResultado)
                    lancamento.Valor = titulo.ValorTotal - valorTotalRateado;
                else
                    lancamento.Valor = Math.Round(Math.Floor(titulo.ValorTotal * lancamento.Percentual) / 100, 2, MidpointRounding.AwayFromZero);

                valorTotalRateado += lancamento.Valor;
                percentual += lancamento.Percentual;

                if (lancamento.Codigo > 0)
                    repLancamentoCentroResultado.Atualizar(lancamento, Auditado);
                else
                {
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, titulo, "Adicionou o centro de resultados " + lancamento.Descricao, unitOfWork);
                    repLancamentoCentroResultado.Inserir(lancamento, Auditado);
                }

                if (lancamento.CentroResultado != null && lancamento.CentroResultado.Veiculos != null && lancamento.CentroResultado.Veiculos.Count > 0)
                {
                    if (titulo.Veiculos == null)
                        titulo.Veiculos = new List<Dominio.Entidades.Veiculo>();
                    foreach (var veiculo in lancamento.CentroResultado.Veiculos)
                    {
                        if (!titulo.Veiculos.Any(v => v.Codigo == veiculo.Codigo))
                        {
                            Dominio.Entidades.Veiculo veic = repVeiculo.BuscarPorCodigo(veiculo.Codigo);

                            titulo.Veiculos.Add(veic);
                        }
                    }
                }
            }

            if (countCentroResultado > 0)
            {
                if (percentual != 100m)
                    throw new ControllerException("O percentual rateado entre os centros de resultado difere de 100%.");

                if (valorTotalRateado != titulo.ValorTotal)
                    throw new ControllerException("Ocorreram problemas ao realizar o rateio dos valores entre os centros de resultado.");
            }
        }

        private void SalvarCentrosResultadoTiposDespesa(Dominio.Entidades.Embarcador.Financeiro.Titulo titulo, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Financeiro.TituloCentroResultadoTipoDespesa repTituloCentroResultadoTipoDespesa = new Repositorio.Embarcador.Financeiro.TituloCentroResultadoTipoDespesa(unitOfWork);
            Repositorio.Embarcador.Financeiro.CentroResultado repCentroResultado = new Repositorio.Embarcador.Financeiro.CentroResultado(unitOfWork);
            Repositorio.Embarcador.Financeiro.Despesa.TipoDespesaFinanceira repTipoDespesaFinanceira = new Repositorio.Embarcador.Financeiro.Despesa.TipoDespesaFinanceira(unitOfWork);

            dynamic dynCentrosResultadoTiposDespesa = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("CentrosResultadoTiposDespesa"));

            List<Dominio.Entidades.Embarcador.Financeiro.TituloCentroResultadoTipoDespesa> centrosResultadoTiposDespesa = repTituloCentroResultadoTipoDespesa.BuscarPorTitulo(titulo.Codigo);

            if (centrosResultadoTiposDespesa.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (dynamic centroResultadoTipoDespesa in dynCentrosResultadoTiposDespesa)
                {
                    int codigo = ((string)centroResultadoTipoDespesa.Codigo).ToInt();
                    if (codigo > 0)
                        codigos.Add(codigo);
                }

                List<Dominio.Entidades.Embarcador.Financeiro.TituloCentroResultadoTipoDespesa> deletar = (from obj in centrosResultadoTiposDespesa where !codigos.Contains(obj.Codigo) select obj).ToList();

                for (var i = 0; i < deletar.Count; i++)
                    repTituloCentroResultadoTipoDespesa.Deletar(deletar[i]);
            }

            decimal percentual = 0m;
            foreach (dynamic centroResultadoTipoDespesa in dynCentrosResultadoTiposDespesa)
            {
                int codigo = ((string)centroResultadoTipoDespesa.Codigo).ToInt();

                Dominio.Entidades.Embarcador.Financeiro.TituloCentroResultadoTipoDespesa tituloCentroResultadoTipoDespesa = codigo > 0 ? repTituloCentroResultadoTipoDespesa.BuscarPorCodigo(codigo, false) : null;

                if (tituloCentroResultadoTipoDespesa == null)
                {
                    tituloCentroResultadoTipoDespesa = new Dominio.Entidades.Embarcador.Financeiro.TituloCentroResultadoTipoDespesa();

                    int codigoTipoDespesa = ((string)centroResultadoTipoDespesa.TipoDespesa.Codigo).ToInt();
                    int codigoCentroResultado = ((string)centroResultadoTipoDespesa.CentroResultado.Codigo).ToInt();

                    tituloCentroResultadoTipoDespesa.Titulo = titulo;
                    tituloCentroResultadoTipoDespesa.TipoDespesaFinanceira = repTipoDespesaFinanceira.BuscarPorCodigo(codigoTipoDespesa);
                    tituloCentroResultadoTipoDespesa.CentroResultado = repCentroResultado.BuscarPorCodigo(codigoCentroResultado);
                    tituloCentroResultadoTipoDespesa.Percentual = ((string)centroResultadoTipoDespesa.Percentual).ToDecimal();

                    percentual += tituloCentroResultadoTipoDespesa.Percentual;

                    repTituloCentroResultadoTipoDespesa.Inserir(tituloCentroResultadoTipoDespesa);
                }
                else
                    percentual += tituloCentroResultadoTipoDespesa.Percentual;
            }

            if (dynCentrosResultadoTiposDespesa.Count > 0 && percentual != 100m)
                throw new ControllerException("O percentual rateado entre os Centros de Resultado/Tipos de Despesa difere de 100%.");
        }

        private void SalvarVeiculos(Dominio.Entidades.Embarcador.Financeiro.Titulo titulo, Repositorio.UnitOfWork unitOfWork)
        {
            if (titulo.StatusTitulo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.EmAberto)
                return;

            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);

            dynamic veiculos = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Veiculos"));

            if (titulo.Veiculos == null)
                titulo.Veiculos = new List<Dominio.Entidades.Veiculo>();
            else
            {
                List<int> codigos = new List<int>();

                foreach (dynamic veiculo in veiculos)
                    codigos.Add((int)veiculo);

                List<Dominio.Entidades.Veiculo> veiculosDeletar = titulo.Veiculos.Where(o => !codigos.Contains(o.Codigo)).ToList();

                foreach (Dominio.Entidades.Veiculo veiculoDeletar in veiculosDeletar)
                    titulo.Veiculos.Remove(veiculoDeletar);
            }
            if (veiculos != null)
            {
                foreach (var veiculo in veiculos)
                {
                    if (!titulo.Veiculos.Any(o => o.Codigo == (int)veiculo))
                    {
                        Dominio.Entidades.Veiculo veic = repVeiculo.BuscarPorCodigo((int)veiculo);

                        titulo.Veiculos.Add(veic);
                    }
                }
            }
        }

        private string ProcessarXMLCTe(dynamic objCTe, Stream xml, Repositorio.UnitOfWork unitOfWork)
        {
            string retorno = "";
            Servicos.CTe svcCTe = new Servicos.CTe(unitOfWork);
            Servicos.Embarcador.CTe.CTe serCte = new Servicos.Embarcador.CTe.CTe(unitOfWork);;

            if (objCTe.GetType() == typeof(MultiSoftware.CTe.v200.ConhecimentoDeTransporteProcessado.cteProc))
            {
                MultiSoftware.CTe.v200.ConhecimentoDeTransporteProcessado.cteProc cteProc = (MultiSoftware.CTe.v200.ConhecimentoDeTransporteProcessado.cteProc)objCTe;
                Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte = serCte.ConverterProcCTeParaCTe(cteProc);

                Repositorio.Embarcador.Cargas.CargaPedidoDocumentoCTe repCargaPedidoDocumentoCTe = new Repositorio.Embarcador.Cargas.CargaPedidoDocumentoCTe(unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCNPJ(cte.Emitente.CNPJ);
                if (empresa != null)
                {
                    object cteRetorno = svcCTe.GerarCTeAnterior(empresa, cteProc, xml, unitOfWork, string.Empty, string.Empty, false, false, false);

                    if (cteRetorno.GetType() == typeof(string))
                    {
                        retorno = (string)cteRetorno;
                    }
                    else if (cteRetorno.GetType() == typeof(Dominio.Entidades.ConhecimentoDeTransporteEletronico))
                    {
                        Dominio.Entidades.ConhecimentoDeTransporteEletronico cteConvertido = (Dominio.Entidades.ConhecimentoDeTransporteEletronico)cteRetorno;
                        cteConvertido.CTeSemCarga = true;
                        repCTe.Atualizar(cteConvertido);
                        retorno = cteConvertido.Codigo.ToString();
                    }
                    else
                    {
                        retorno = "Conhecimento de transporte inválido.";
                    }
                }
                else
                {
                    retorno = "O CT-e informado não foi emitido por uma transportadora cadastrada.";
                }

                return retorno;
            }
            else if (objCTe.GetType() == typeof(MultiSoftware.CTe.v300.ConhecimentoDeTransporteProcessado.cteProc))
            {
                //var cteLido = MultiSoftware.CTe.Servicos.Leitura.Ler(xml);

                MultiSoftware.CTe.v300.ConhecimentoDeTransporteProcessado.cteProc cteProc = (MultiSoftware.CTe.v300.ConhecimentoDeTransporteProcessado.cteProc)objCTe;
                Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte = serCte.ConverterProcCTeParaCTe(cteProc);

                Repositorio.Embarcador.Cargas.CargaPedidoDocumentoCTe repCargaPedidoDocumentoCTe = new Repositorio.Embarcador.Cargas.CargaPedidoDocumentoCTe(unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCNPJ(cte.Emitente.CNPJ);
                if (empresa != null)
                {
                    object cteRetorno = svcCTe.GerarCTeAnterior(empresa, cteProc, xml, unitOfWork, string.Empty, string.Empty, false, false, false);

                    if (cteRetorno.GetType() == typeof(string))
                    {
                        retorno = (string)cteRetorno;
                    }
                    else if (cteRetorno.GetType() == typeof(Dominio.Entidades.ConhecimentoDeTransporteEletronico))
                    {
                        Dominio.Entidades.ConhecimentoDeTransporteEletronico cteConvertido = (Dominio.Entidades.ConhecimentoDeTransporteEletronico)cteRetorno;
                        cteConvertido.CTeSemCarga = true;
                        repCTe.Atualizar(cteConvertido);
                        retorno = cteConvertido.Codigo.ToString();
                    }
                    else
                    {
                        retorno = "Conhecimento de transporte inválido.";
                    }
                }
            }
            else if (objCTe.GetType() == typeof(MultiSoftware.CTe.v400.ConhecimentoDeTransporteProcessado.cteProc))
            {
                MultiSoftware.CTe.v400.ConhecimentoDeTransporteProcessado.cteProc cteProc = (MultiSoftware.CTe.v400.ConhecimentoDeTransporteProcessado.cteProc)objCTe;
                Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte = serCte.ConverterProcCTeParaCTe(cteProc);

                Repositorio.Embarcador.Cargas.CargaPedidoDocumentoCTe repCargaPedidoDocumentoCTe = new Repositorio.Embarcador.Cargas.CargaPedidoDocumentoCTe(unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCNPJ(cte.Emitente.CNPJ);
                if (empresa != null)
                {
                    object cteRetorno = svcCTe.GerarCTeAnterior(empresa, cteProc, xml, unitOfWork, string.Empty, string.Empty, false, false, false);

                    if (cteRetorno.GetType() == typeof(string))
                    {
                        retorno = (string)cteRetorno;
                    }
                    else if (cteRetorno.GetType() == typeof(Dominio.Entidades.ConhecimentoDeTransporteEletronico))
                    {
                        Dominio.Entidades.ConhecimentoDeTransporteEletronico cteConvertido = (Dominio.Entidades.ConhecimentoDeTransporteEletronico)cteRetorno;
                        cteConvertido.CTeSemCarga = true;
                        repCTe.Atualizar(cteConvertido);
                        retorno = cteConvertido.Codigo.ToString();
                    }
                    else
                    {
                        retorno = "Conhecimento de transporte inválido.";
                    }
                }
            }
            else
            {
                retorno = "A versão do CT-e não é compativel, por favor, verique com a multisoftware";
            }
            return retorno;
        }

        private class RetornoArquivo
        {
            public int codigo { get; set; }
            public string nome { get; set; }
            public bool processada { get; set; }
            public string mensagem { get; set; }
        }

        private Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaTituloFinanceiro ObterFiltrosPesquisa()
        {
            Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaTituloFinanceiro filtrosPesquisaTituloFinanceiro = new Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaTituloFinanceiro()
            {
                DataInicialVencimento = Request.GetDateTimeParam("DataInicialVencimento"),
                DataFinalVencimento = Request.GetDateTimeParam("DataFinalVencimento"),
                DataInicialEmissao = Request.GetDateTimeParam("DataInicialEmissao"),
                DataFinalEmissao = Request.GetDateTimeParam("DataFinalEmissao"),
                DataBaseLiquidacaoInicial = Request.GetDateTimeParam("DataBaseLiquidacaoInicial"),
                DataBaseLiquidacaoFinal = Request.GetDateTimeParam("DataBaseLiquidacaoFinal"),

                ValorMovimento = Request.GetDecimalParam("ValorMovimento"),
                ValorPago = Request.GetDecimalParam("ValorPago"),
                ValorPagoAte = Request.GetDecimalParam("ValorPagoAte"),

                CodigoPessoa = Request.GetDoubleParam("Pessoa"),
                CodigoPortador = Request.GetDoubleParam("Portador"),

                CodigoTitulo = Request.GetIntParam("Titulo"),
                CodigoCTe = Request.GetIntParam("Conhecimento"),
                CodigoFatura = Request.GetIntParam("Fatura"),
                CodigoGrupoPessoa = Request.GetIntParam("GrupoPessoa"),
                NumeroDocumentoOriginario = Request.GetIntParam("NumeroDocumentoOriginario"),
                CodigoVeiculo = Request.GetIntParam("Veiculo"),
                CodigoTipoMovimento = Request.GetIntParam("TipoMovimento"),
                CodigoRemessa = Request.GetIntParam("Remessa"),
                Adiantado = Request.GetIntParam("Adiantado"),

                NumeroPedido = Request.GetStringParam("NumeroPedido"),
                NumeroOcorrencia = Request.GetStringParam("NumeroOcorrencia"),
                NossoNumero = Request.GetStringParam("NossoNumero"),
                DocumentoOriginal = Request.GetStringParam("DocumentoOriginal"),

                StatusTitulo = Request.GetListEnumParam<StatusTitulo>("StatusTitulo"),
                TipoTitulo = Request.GetEnumParam<TipoTitulo>("TipoTitulo"),
                FormaTitulo = Request.GetEnumParam<FormaTitulo>("FormaTitulo"),
                TipoDeDocumento = Request.GetNullableEnumParam<TipoDocumentoPesquisaTitulo>("TipoDeDocumento"),
                ProvisaoPesquisaTitulo = Request.GetEnumParam<ProvisaoPesquisaTitulo>("Provisao"),
                TipoBoleto = Request.GetEnumParam<TipoBoletoPesquisaTitulo>("TipoBoleto"),
                TipoAmbiente = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe ? this.Usuario.Empresa.TipoAmbiente : Dominio.Enumeradores.TipoAmbiente.Nenhum,
                CodigoEmpresa = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe ? this.Usuario.Empresa.Codigo : Request.GetIntParam("Empresa"),

                NumeroBooking = Request.GetStringParam("NumeroBooking"),
                NumeroOS = Request.GetStringParam("NumeroOS"),
                NumeroCarga = Request.GetStringParam("NumeroCarga"),
                NumeroNota = Request.GetIntParam("NumeroNota"),
                NumeroControleCliente = Request.GetStringParam("NumeroControleCliente"),
                NumeroControle = Request.GetStringParam("NumeroControle"),
                TipoProposta = Request.GetEnumParam<TipoPropostaMultimodal>("TipoProposta"),
                TiposPropostasMultimodal = this.Usuario != null && this.Usuario.PerfilAcesso != null && this.Usuario.PerfilAcesso.TiposPropostasMultimodal != null ? this.Usuario.PerfilAcesso.TiposPropostasMultimodal.ToList() : null,
                CodigoTerminalOrigem = Request.GetIntParam("TerminalOrigem"),
                CodigoTerminalDestino = Request.GetIntParam("TerminalDestino"),
                CodigoViagem = Request.GetIntParam("Viagem"),
                DataProgramacaoPagamentoInicial = Request.GetDateTimeParam("DataProgramacaoPagamentoInicial"),
                DataProgramacaoPagamentoFinal = Request.GetDateTimeParam("DataProgramacaoPagamentoFinal"),

                ValorDe = Request.GetDecimalParam("ValorDe"),
                ValorAte = Request.GetDecimalParam("ValorAte"),
                CodigoCategoriaPessoa = Request.GetIntParam("CategoriaPessoa"),

                TipoDocumento = Dominio.Enumeradores.TipoDocumento.Todos,
                MoedaCotacaoBancoCentral = Request.GetNullableEnumParam<MoedaCotacaoBancoCentral>("MoedaCotacaoBancoCentral"),
                RaizCnpjPessoa = Request.GetStringParam("RaizCnpjPessoa").ObterSomenteNumeros(),
                VisualizarTitulosPagamentoSalario = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS ? Usuario.PermiteVisualizarTitulosPagamentoSalario : true,
                StatusEmAberto = Request.GetNullableBoolParam("StatusEmAberto"),
                TipoAPagar = Request.GetNullableBoolParam("TipoAPagar"),
            };

            if (filtrosPesquisaTituloFinanceiro.CodigoTitulo == 0)
                filtrosPesquisaTituloFinanceiro.CodigoTitulo = Request.GetIntParam("Descricao");

            return filtrosPesquisaTituloFinanceiro;
        }

        private string ObterPropriedadeOrdenar(string propOrdena)
        {
            if (propOrdena == "Valor")
                propOrdena = "ValorOriginal";
            else if (propOrdena == "DescricaoTipo")
                propOrdena = "TipoTitulo";
            else if (propOrdena == "DescricaoSituacao")
                propOrdena = "StatusTitulo";
            else if (propOrdena == "Fatura")
                propOrdena = "FaturaParcela.Fatura.Numero";
            else if (propOrdena == "Remessa")
                propOrdena = "BoletoRemessa.NumeroSequencial";

            return propOrdena;
        }

        #endregion

        #region Importação

        public async Task<IActionResult> ConfiguracaoImportacao()
        {
            List<ConfiguracaoImportacao> configuracoes = ConfiguracaoImportacaoTitulos();

            return new JsonpResult(configuracoes.ToList());
        }

        public async Task<IActionResult> Importar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro repConfiguracaoFinanceiro = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracao.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiro configuracaoFinanceiro = repConfiguracaoFinanceiro.BuscarConfiguracaoPadrao();

                unitOfWork.Start();

                List<ConfiguracaoImportacao> configuracoes = ConfiguracaoImportacaoTitulos();
                List<Dominio.Entidades.Embarcador.Financeiro.Titulo> titulos = new List<Dominio.Entidades.Embarcador.Financeiro.Titulo>();

                RetornoImportacao retorno = Servicos.Embarcador.Importacao.Importacao.PreencherImportacaoManual(Request, titulos, ((dados) =>
                {
                    Servicos.Embarcador.Financeiro.TituloImportacao servicoTituloImportar = new Servicos.Embarcador.Financeiro.TituloImportacao(unitOfWork, TipoServicoMultisoftware, TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe ? this.Empresa : null, dados, configuracao, configuracaoFinanceiro);

                    return servicoTituloImportar.ObterTituloImportar(this.Usuario);
                }));

                if (retorno == null)
                    return new JsonpResult(false, "Ocorreu uma falha ao importar arquivo.");

                int totalRegistrosImportados = 0;
                dynamic parametro = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Parametro"));
                bool permiteInserir = (bool)parametro.Inserir;
                bool permiteAtualizar = (bool)parametro.Atualizar;

                Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);
                Repositorio.Embarcador.Financeiro.LancamentoCentroResultado repLancamentoCentroResultado = new Repositorio.Embarcador.Financeiro.LancamentoCentroResultado(unitOfWork);
                Repositorio.Embarcador.Financeiro.TipoMovimentoCentroResultado repTipoMovimentoCentroResultado = new Repositorio.Embarcador.Financeiro.TipoMovimentoCentroResultado(unitOfWork);
                Repositorio.Embarcador.Financeiro.TipoMovimentoTipoDespesa repTipoMovimentoTipoDespesa = new Repositorio.Embarcador.Financeiro.TipoMovimentoTipoDespesa(unitOfWork);
                Repositorio.Embarcador.Financeiro.TituloCentroResultadoTipoDespesa repTituloCentroResultadoTipoDespesa = new Repositorio.Embarcador.Financeiro.TituloCentroResultadoTipoDespesa(unitOfWork);

                Servicos.Embarcador.Financeiro.ProcessoMovimento servProcessoMovimento = new Servicos.Embarcador.Financeiro.ProcessoMovimento(unitOfWork.StringConexao);

                foreach (Dominio.Entidades.Embarcador.Financeiro.Titulo titulo in titulos)
                {
                    if (permiteInserir)
                    {
                        Dominio.Entidades.Embarcador.Financeiro.LancamentoCentroResultado lancamentoCentroResultado = null;
                        if (titulo.LancamentosCentroResultado != null && titulo.LancamentosCentroResultado.Count == 1)
                        {
                            lancamentoCentroResultado = titulo.LancamentosCentroResultado[0];
                            titulo.LancamentosCentroResultado = null;
                        }

                        repTitulo.Inserir(titulo, Auditado);

                        if (lancamentoCentroResultado != null)
                            repLancamentoCentroResultado.Inserir(lancamentoCentroResultado);

                        if ((titulo.TipoMovimento != null && !titulo.Provisao) || (titulo.TipoMovimento != null && titulo.Provisao && configuracaoFinanceiro.MovimentacaoFinanceiraParaTitulosDeProvisao))
                        {
                            if (!servProcessoMovimento.GerarMovimentacao(out string erro, titulo.TipoMovimento, titulo.DataEmissao.Value, titulo.ValorOriginal, titulo.Codigo.ToString(), "GERAÇÃO DO TÍTULO POR IMPORTAÇÃO " + titulo.Observacao, unitOfWork, TipoDocumentoMovimento.Outros, TipoServicoMultisoftware, 0, null, null, titulo.Codigo, null, titulo.Pessoa, titulo.Pessoa?.GrupoPessoas ?? null, null, lancamentoCentroResultado?.CentroResultado ?? null, null, null, null, null, null, 0, 0, null, null, titulo.FormaTitulo))
                            {
                                unitOfWork.Rollback();
                                return new JsonpResult(false, erro);
                            }
                        }

                        if (configuracaoFinanceiro.AtivarControleDespesas && titulo.TipoMovimento != null)
                        {
                            Dominio.Entidades.Embarcador.Financeiro.CentroResultado centroResultado = titulo.Veiculos?.Count > 0 ? titulo.Veiculos.FirstOrDefault().CentroResultado : null;
                            if (centroResultado == null)
                                centroResultado = repTipoMovimentoCentroResultado.BuscarCentroResultado(titulo.TipoMovimento.Codigo);
                            Dominio.Entidades.Embarcador.Financeiro.Despesa.TipoDespesaFinanceira tipoDespesa = repTipoMovimentoTipoDespesa.BuscarTipoDespesaFinanceira(titulo.TipoMovimento.Codigo);

                            if (centroResultado != null && tipoDespesa != null)
                            {
                                Dominio.Entidades.Embarcador.Financeiro.TituloCentroResultadoTipoDespesa tituloCentroResultadoTipoDespesa = new Dominio.Entidades.Embarcador.Financeiro.TituloCentroResultadoTipoDespesa();
                                tituloCentroResultadoTipoDespesa.Titulo = titulo;
                                tituloCentroResultadoTipoDespesa.CentroResultado = centroResultado;
                                tituloCentroResultadoTipoDespesa.TipoDespesaFinanceira = tipoDespesa;
                                tituloCentroResultadoTipoDespesa.Percentual = 100;
                                repTituloCentroResultadoTipoDespesa.Inserir(tituloCentroResultadoTipoDespesa);
                            }
                        }

                        totalRegistrosImportados++;
                    }
                }

                unitOfWork.CommitChanges();

                retorno.Importados = totalRegistrosImportados;

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao importar arquivo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private List<ConfiguracaoImportacao> ConfiguracaoImportacaoTitulos()
        {
            List<ConfiguracaoImportacao> configuracoes = new List<ConfiguracaoImportacao>();
            int tamanho = 150;

            configuracoes.Add(new ConfiguracaoImportacao() { Id = 1, Descricao = "CNPJ/CPF Pessoa", Propriedade = "CnpjCpfPessoa", Tamanho = tamanho, Obrigatorio = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 2, Descricao = "Tipo", Propriedade = "TipoTitulo", Tamanho = tamanho, Obrigatorio = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 3, Descricao = "Data Emissão", Propriedade = "DataEmissao", Tamanho = tamanho, Obrigatorio = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 4, Descricao = "Data Vencimento", Propriedade = "DataVencimento", Tamanho = tamanho, Obrigatorio = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 5, Descricao = "Valor Original", Propriedade = "ValorOriginal", Tamanho = tamanho, Obrigatorio = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 6, Descricao = "Cód. Tipo Movimento", Propriedade = "TipoMovimento", Tamanho = tamanho, Obrigatorio = true, Regras = new List<string> { "required" } });

            configuracoes.Add(new ConfiguracaoImportacao() { Id = 7, Descricao = "Sequência", Propriedade = "Sequencia", Tamanho = tamanho, CampoInformacao = true });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 8, Descricao = "Valor Desconto", Propriedade = "ValorDesconto", Tamanho = tamanho, CampoInformacao = true });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 9, Descricao = "Valor Acréscimo", Propriedade = "ValorAcrescimo", Tamanho = tamanho, CampoInformacao = true });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 10, Descricao = "Tipo de Documento", Propriedade = "TipoDocumento", Tamanho = tamanho, CampoInformacao = true });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 11, Descricao = "Número do Documento", Propriedade = "NumeroDocumento", Tamanho = tamanho, CampoInformacao = true });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 12, Descricao = "Observação", Propriedade = "Observacao", Tamanho = tamanho, CampoInformacao = true });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 13, Descricao = "Centro Resultado", Propriedade = "CentroResultado", Tamanho = tamanho, CampoInformacao = true });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 14, Descricao = "Veículo", Propriedade = "Veiculo", Tamanho = tamanho, CampoInformacao = true });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 15, Descricao = "Empresa", Propriedade = "Empresa", Tamanho = tamanho, CampoInformacao = true });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 16, Descricao = "Forma do Título", Propriedade = "FormaTitulo", Tamanho = tamanho, CampoInformacao = true });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 17, Descricao = "CNPJ/CPF Portador", Propriedade = "CnpjCpfPortador", Tamanho = tamanho, CampoInformacao = true });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 18, Descricao = "Código de Barras / Número Boleto", Propriedade = "NossoNumero", Tamanho = tamanho, CampoInformacao = true });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 19, Descricao = "Provisão", Propriedade = "Provisao", Tamanho = tamanho, CampoInformacao = true });

            return configuracoes;
        }


        public async Task<IActionResult> ConfiguracaoImportacaoLiquidosFolha()
        {
            List<ConfiguracaoImportacao> configuracoes = ConfiguracaoImportacaoTitulosLiquidosFolha();

            return new JsonpResult(configuracoes.ToList());
        }

        public async Task<IActionResult> ImportarLiquidosFolha()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro repConfiguracaoFinanceiro = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracao.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiro configuracaoFinanceiro = repConfiguracaoFinanceiro.BuscarConfiguracaoPadrao();

                unitOfWork.Start();

                List<ConfiguracaoImportacao> configuracoes = ConfiguracaoImportacaoTitulos();
                List<Dominio.Entidades.Embarcador.Financeiro.Titulo> titulos = new List<Dominio.Entidades.Embarcador.Financeiro.Titulo>();

                RetornoImportacao retorno = Servicos.Embarcador.Importacao.Importacao.PreencherImportacaoManual(Request, titulos, ((dados) =>
                {
                    Servicos.Embarcador.Financeiro.TituloImportacao servicoTituloImportar = new Servicos.Embarcador.Financeiro.TituloImportacao(unitOfWork, TipoServicoMultisoftware, TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe ? this.Empresa : null, dados, configuracao, configuracaoFinanceiro);

                    return servicoTituloImportar.ObterTituloLiquidosFolhaImportar(this.Usuario);
                }));

                if (retorno == null)
                    return new JsonpResult(false, "Ocorreu uma falha ao importar arquivo.");

                int totalRegistrosImportados = 0;
                dynamic parametro = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Parametro"));
                bool permiteInserir = (bool)parametro.Inserir;
                bool permiteAtualizar = (bool)parametro.Atualizar;

                Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);
                Servicos.Embarcador.Financeiro.ProcessoMovimento servProcessoMovimento = new Servicos.Embarcador.Financeiro.ProcessoMovimento(unitOfWork.StringConexao);

                foreach (var titulo in titulos)
                {
                    if (permiteInserir)
                    {
                        repTitulo.Inserir(titulo, Auditado);

                        if (titulo.TipoMovimento != null)
                            if (!servProcessoMovimento.GerarMovimentacao(out string erro, titulo.TipoMovimento, titulo.DataEmissao.Value, titulo.ValorOriginal, titulo.Codigo.ToString(), "GERAÇÃO DO TÍTULO POR IMPORTAÇÃO DO LÍQUIDO DA FOLHA " + titulo.Observacao, unitOfWork, TipoDocumentoMovimento.Outros, TipoServicoMultisoftware, 0, null, null, titulo.Codigo, null, titulo.Pessoa, titulo.Pessoa?.GrupoPessoas ?? null, null, null, null, null, null, null, null, 0, 0, null, null, titulo.FormaTitulo))
                            {
                                unitOfWork.Rollback();
                                return new JsonpResult(false, erro);
                            }

                        totalRegistrosImportados++;
                    }
                }

                unitOfWork.CommitChanges();

                retorno.Importados = totalRegistrosImportados;

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao importar arquivo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private List<ConfiguracaoImportacao> ConfiguracaoImportacaoTitulosLiquidosFolha()
        {
            List<ConfiguracaoImportacao> configuracoes = new List<ConfiguracaoImportacao>();
            int tamanho = 150;

            configuracoes.Add(new ConfiguracaoImportacao() { Id = 1, Descricao = "CPF Motorista", Propriedade = "CnpjCpfPessoa", Tamanho = tamanho, Obrigatorio = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 3, Descricao = "Data Competencia", Propriedade = "DataEmissao", Tamanho = tamanho, Obrigatorio = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 5, Descricao = "Saldo Líquido", Propriedade = "ValorOriginal", Tamanho = tamanho, Obrigatorio = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 6, Descricao = "Cód. Tipo Movimento", Propriedade = "TipoMovimento", Tamanho = tamanho, Obrigatorio = true, Regras = new List<string> { "required" } });

            configuracoes.Add(new ConfiguracaoImportacao() { Id = 7, Descricao = "Código", Propriedade = "NumeroDocumento", Tamanho = tamanho, CampoInformacao = true });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 8, Descricao = "Nome", Propriedade = "Nome", Tamanho = tamanho, CampoInformacao = true });

            return configuracoes;
        }

        private void ExcluirAnexos(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Financeiro.TituloAnexo repTituloFinanceiroAnexo = new Repositorio.Embarcador.Financeiro.TituloAnexo(unitOfWork);

            dynamic listaAnexos = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ListaAnexosExcluidos"));
            if (listaAnexos.Count > 0)
            {
                foreach (var anexo in listaAnexos)
                {
                    Dominio.Entidades.Embarcador.Financeiro.TituloAnexo tituloFinanceiroAnexo = repTituloFinanceiroAnexo.BuscarPorCodigo((int)anexo.Codigo, true);

                    if (Utilidades.IO.FileStorageService.Storage.Exists(tituloFinanceiroAnexo.CaminhoArquivo))
                        Utilidades.IO.FileStorageService.Storage.Delete(tituloFinanceiroAnexo.CaminhoArquivo);

                    repTituloFinanceiroAnexo.Deletar(tituloFinanceiroAnexo, Auditado);
                }
            }
        }
        #endregion
    }
}
