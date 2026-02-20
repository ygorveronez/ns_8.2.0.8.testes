using Dominio.Excecoes.Embarcador;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Financeiros
{
    [CustomAuthorize("Financeiros/LancamentoConta", "Financeiros/TituloFinanceiro", "Pessoas/Pessoa", "Financeiros/PlanoOrcamentario")]
    public class LancamentoContaController : BaseController
    {
		#region Construtores

		public LancamentoContaController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Salvar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Financeiros/TituloFinanceiro");

                unitOfWork.Start();

                if (!this.Usuario.UsuarioAdministrador && !permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Titulo_GerarMultiplosTitulos))
                    return new JsonpResult(false, true, "Você não possui permissões para gerar múltiplos títulos.");

                Servicos.Embarcador.Financeiro.ProcessoMovimento servProcessoMovimento = new Servicos.Embarcador.Financeiro.ProcessoMovimento(unitOfWork.StringConexao);
                Repositorio.Embarcador.Financeiro.CobrancaManual repCobrancaManual = new Repositorio.Embarcador.Financeiro.CobrancaManual(unitOfWork);
                Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                Repositorio.Embarcador.Financeiro.TipoMovimento repTipoMovimento = new Repositorio.Embarcador.Financeiro.TipoMovimento(unitOfWork);
                Repositorio.Embarcador.Financeiro.TipoPagamentoRecebimento repTipoPagamentoRecebimento = new Repositorio.Embarcador.Financeiro.TipoPagamentoRecebimento(unitOfWork);
                Repositorio.Embarcador.Financeiro.TituloBaixa repTituloBaixa = new Repositorio.Embarcador.Financeiro.TituloBaixa(unitOfWork);
                Repositorio.Embarcador.Financeiro.TituloBaixaAgrupado repTituloBaixaAgrupado = new Repositorio.Embarcador.Financeiro.TituloBaixaAgrupado(unitOfWork);
                Repositorio.Embarcador.Financeiro.TituloBaixaTipoPagamentoRecebimento repTituloBaixaTipoPagamentoRecebimento = new Repositorio.Embarcador.Financeiro.TituloBaixaTipoPagamentoRecebimento(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro repConfiguracaoFinanceiro = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro(unitOfWork);
                Repositorio.Embarcador.Financeiro.TipoMovimentoCentroResultado repTipoMovimentoCentroResultado = new Repositorio.Embarcador.Financeiro.TipoMovimentoCentroResultado(unitOfWork);
                Repositorio.Embarcador.Financeiro.TipoMovimentoTipoDespesa repTipoMovimentoTipoDespesa = new Repositorio.Embarcador.Financeiro.TipoMovimentoTipoDespesa(unitOfWork);
                Repositorio.Embarcador.Financeiro.TituloCentroResultadoTipoDespesa repTituloCentroResultadoTipoDespesa = new Repositorio.Embarcador.Financeiro.TituloCentroResultadoTipoDespesa(unitOfWork);

                dynamic dynContas = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("Conta"));

                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCNPJ(this.Usuario.Empresa.CNPJ);

                DateTime dataVencimento, dataRecebimento, dataEmissao;
                DateTime.TryParse((string)dynContas.DataVencimento, out dataVencimento);
                DateTime.TryParse((string)dynContas.DataRecebimento, out dataRecebimento);
                DateTime.TryParse((string)dynContas.DataEmissao, out dataEmissao);
                DateTime? dataBaseCRT = ((string)dynContas.DataBaseCRT).ToNullableDateTime();

                decimal valor, valorMulta = 0, valorDesconto = 0, valorRecebido = 0, valorJuros = 0;
                decimal.TryParse((string)dynContas.Valor, out valor);
                decimal.TryParse((string)dynContas.ValorMulta, out valorMulta);
                decimal.TryParse((string)dynContas.ValorDesconto, out valorDesconto);
                decimal.TryParse((string)dynContas.ValorRecebido, out valorRecebido);
                decimal.TryParse((string)dynContas.ValorJuros, out valorJuros);
                decimal.TryParse((string)dynContas.ValorMoedaCotacao, out decimal valorMoedaCotacao);
                decimal.TryParse((string)dynContas.ValorOriginalMoedaEstrangeira, out decimal valorOriginalMoedaEstrangeira);

                int numeroOcorrencia = dynContas.NumeroOcorrencia != null ? (int)dynContas.NumeroOcorrencia : 0;
                bool recebido = (bool)dynContas.Recebido;
                bool repetir = (bool)dynContas.Repetir;
                bool dividir = (bool)dynContas.Dividir;
                bool provisao = (bool)dynContas.Provisao;
                bool simularParcelas = (bool)dynContas.SimularParcelas;

                int tipoRecebimentoPagamento = 0, diaVencimento = 0;
                int.TryParse((string)dynContas.TipoPagamentoRecebimento, out tipoRecebimentoPagamento);
                int.TryParse((string)dynContas.DiaVencimento, out diaVencimento);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo statusTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.EmAberto;
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTitulo tipoTitulo;
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.Periodicidade periodicidade;
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaTitulo formaTitulo;
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral moedaCotacaoBancoCentral;
                Enum.TryParse((string)dynContas.Tipo, out tipoTitulo);
                Enum.TryParse((string)dynContas.TipoRepetir, out periodicidade);
                Enum.TryParse((string)dynContas.FormaTitulo, out formaTitulo);
                Enum.TryParse((string)dynContas.MoedaCotacaoBancoCentral, out moedaCotacaoBancoCentral);

                double pessoa = 0;
                double.TryParse((string)dynContas.Pessoa, out pessoa);
                int codigoEmpresa = 0;
                int.TryParse((string)dynContas.Empresa, out codigoEmpresa);
                int tipoMovimento = (int)dynContas.TipoMovimento;
                int tipoMovimentoJuros = (int)dynContas.TipoMovimentoJuros;

                string observacao = (string)dynContas.Observacao;
                string documento = (string)dynContas.Documento;
                string tipoDocumento = (string)dynContas.TipoDocumento;

                Repositorio.Embarcador.Financeiro.FechamentoDiario repFechamentoDiario = new Repositorio.Embarcador.Financeiro.FechamentoDiario(unitOfWork);
                if (repFechamentoDiario.VerificarSeExistePorDataFechamento(codigoEmpresa, dataEmissao))
                    throw new ControllerException("Já existe um fechamento diário igual ou posterior à data " + dataEmissao.ToString("dd/MM/yyyy") + ", não sendo possível realizar o lançamento do título.");

                if (dataEmissao.Date > dataVencimento.Date)
                    throw new ControllerException("A data de emissão não pode ser maior que a data de vencimento.");

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                {
                    empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);
                    if (empresa == null)
                        throw new ControllerException("Favor selecione a empresa.");
                }

                Dominio.Entidades.Embarcador.Financeiro.CobrancaManual cobrancaManual = new Dominio.Entidades.Embarcador.Financeiro.CobrancaManual();
                cobrancaManual.DataEmissao = dataEmissao;
                cobrancaManual.DataVencimento = dataVencimento;
                cobrancaManual.DiaVencimento = diaVencimento;
                cobrancaManual.DividirLancamento = dividir;
                cobrancaManual.Documento = documento;
                cobrancaManual.TipoDocumento = tipoDocumento;
                cobrancaManual.Empresa = empresa;
                cobrancaManual.FormaTitulo = formaTitulo;
                cobrancaManual.Observacao = observacao;
                cobrancaManual.Ocorrencia = numeroOcorrencia;
                cobrancaManual.Pessoa = repCliente.BuscarPorCPFCNPJ(pessoa);
                cobrancaManual.Provisao = provisao;
                cobrancaManual.Periodicidade = periodicidade;
                cobrancaManual.RepetirLancamento = repetir;
                cobrancaManual.SimularParcelas = simularParcelas;
                cobrancaManual.TipoMovimentoValorDocumento = null;
                cobrancaManual.TipoMovimentoValorJuros = null;
                cobrancaManual.TipoTitulo = tipoTitulo;
                cobrancaManual.TituloJaFoiPago = recebido;
                cobrancaManual.ValorDocumento = valor;
                cobrancaManual.ValorJuros = valorJuros;

                cobrancaManual.MoedaCotacaoBancoCentral = moedaCotacaoBancoCentral;
                cobrancaManual.DataBaseCRT = dataBaseCRT;
                cobrancaManual.ValorMoedaCotacao = valorMoedaCotacao;
                cobrancaManual.ValorOriginalMoedaEstrangeira = valorOriginalMoedaEstrangeira;

                repCobrancaManual.Inserir(cobrancaManual);
                List<Dominio.Entidades.Embarcador.Financeiro.Titulo> titulos = null;

                if (simularParcelas)
                    titulos = RetornaTitulosSimulacao(unitOfWork, dataVencimento, recebido, numeroOcorrencia, repetir, valor, valorDesconto, valorMulta, valorRecebido, periodicidade, diaVencimento, dataEmissao, observacao,
                        tipoTitulo, documento, valorJuros, statusTitulo, tipoMovimento, pessoa, empresa,
                        dataRecebimento, formaTitulo, provisao, tipoDocumento, moedaCotacaoBancoCentral, dataBaseCRT, valorMoedaCotacao);
                else
                    titulos = RetornaListaTitulos(unitOfWork, dataVencimento, recebido, numeroOcorrencia, repetir, valor, valorDesconto, valorMulta, valorRecebido, periodicidade, diaVencimento, dataEmissao, observacao,
                        tipoTitulo, documento, valorJuros, statusTitulo, tipoMovimento, pessoa, empresa,
                        dataRecebimento, formaTitulo, provisao, tipoDocumento, moedaCotacaoBancoCentral, dataBaseCRT, valorMoedaCotacao);

                if (titulos == null || titulos.Count <= 0)
                    throw new ControllerException("Verifique os valores informados para a geração do título.");

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiro configuracaoFinanceiro = repConfiguracaoFinanceiro.BuscarConfiguracaoPadrao();

                bool primeiroTitulo = true;
                foreach (Dominio.Entidades.Embarcador.Financeiro.Titulo titulo in titulos)
                {
                    if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    {
                        if (configuracaoFinanceiro.ValidarDuplicidadeTituloSemData)
                        {
                            if (repTitulo.ContemTituloDuplicado(0, titulo.Pessoa.CPF_CNPJ, titulo.TipoTitulo, titulo.Sequencia, titulo.TipoDocumentoTituloOriginal, titulo.NumeroDocumentoTituloOriginal))
                                throw new ControllerException("Já existe um título lançado com a mesma Pessoa, Tipo, Sequência, Tipo de Documento e Número do Documento.");
                        }
                        else
                        {
                            if (repTitulo.ContemTituloDuplicado(titulo.DataEmissao.Value, titulo.DataVencimento.Value, titulo.Pessoa.CPF_CNPJ, titulo.ValorOriginal, titulo.TipoTitulo, 0, titulo.NumeroDocumentoTituloOriginal))
                                throw new ControllerException("Já existe um título lançado com a mesma Data de Emissão, Vencimento, Valor, Número do Documento e Pessoa.");
                        }
                    }

                    SalvarVeiculos(titulo, unitOfWork);

                    titulo.CobrancaManual = cobrancaManual;
                    repTitulo.Inserir(titulo, Auditado);

                    if (titulo.TipoMovimento != null)
                        servProcessoMovimento.GerarMovimentacao(titulo.TipoMovimento, titulo.DataEmissao.Value, titulo.ValorOriginal, titulo.Codigo.ToString(), "GERAÇÃO DO TÍTULO MANUAL " + observacao, unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Manual, TipoServicoMultisoftware, 0, null, null, titulo.Codigo, null, null, null, null, null, null, null, null, titulo.MoedaCotacaoBancoCentral, titulo.DataBaseCRT, titulo.ValorMoedaCotacao, titulo.ValorOriginalMoedaEstrangeira, null, null, titulo.FormaTitulo);
                    if (tipoMovimentoJuros > 0 && titulo.Acrescimo > 0)
                        servProcessoMovimento.GerarMovimentacao(repTipoMovimento.BuscarPorCodigo(tipoMovimentoJuros), titulo.DataEmissao.Value, titulo.Acrescimo, titulo.Codigo.ToString(), "JUROS DA GERAÇÃO DO TÍTULO MANUAL " + observacao, unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Manual, TipoServicoMultisoftware, 0, null, null, 0, null, null, null, null, null, null, null, null, titulo.MoedaCotacaoBancoCentral, titulo.DataBaseCRT, titulo.ValorMoedaCotacao, titulo.ValorOriginalMoedaEstrangeira, null, null, titulo.FormaTitulo);

                    if (configuracaoFinanceiro.AtivarControleDespesas && titulo.TipoMovimento != null)
                    {
                        Dominio.Entidades.Embarcador.Financeiro.CentroResultado centroResultado = repTipoMovimentoCentroResultado.BuscarCentroResultado(titulo.TipoMovimento.Codigo);
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

                    if (primeiroTitulo && recebido && tipoRecebimentoPagamento > 0 && titulo.TipoMovimento != null)
                    {
                        Dominio.Entidades.Embarcador.Financeiro.TipoPagamentoRecebimento tipoPagamentoRecebimento = repTipoPagamentoRecebimento.BuscarPorCodigo(tipoRecebimentoPagamento);
                        if (tipoPagamentoRecebimento != null)
                        {
                            Dominio.Entidades.Embarcador.Financeiro.TituloBaixa tituloBaixa = new Dominio.Entidades.Embarcador.Financeiro.TituloBaixa();
                            tituloBaixa.DataBaixa = titulo.DataLiquidacao;
                            tituloBaixa.DataBase = titulo.DataLiquidacao;
                            tituloBaixa.DataOperacao = DateTime.Now;
                            tituloBaixa.Observacao = "QUITAÇÃO AUTOMÁTICA";
                            tituloBaixa.Pessoa = titulo.Pessoa;
                            tituloBaixa.GrupoPessoas = titulo.Pessoa.GrupoPessoas;
                            tituloBaixa.Sequencia = 1;
                            tituloBaixa.SituacaoBaixaTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoBaixaTitulo.Finalizada;
                            tituloBaixa.Titulo = titulo;
                            tituloBaixa.Usuario = this.Usuario;
                            tituloBaixa.Valor = titulo.ValorPago;
                            tituloBaixa.ValorAcrescimo = valorJuros;
                            tituloBaixa.Numero = 1;
                            tituloBaixa.TipoBaixaTitulo = titulo.TipoTitulo;

                            if (titulo.TipoTitulo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTitulo.Receber)
                            {
                                tituloBaixa.TipoPagamentoRecebimento = tipoPagamentoRecebimento;
                                tituloBaixa.ModeloAntigo = true;
                            }

                            tituloBaixa.MoedaCotacaoBancoCentral = titulo.MoedaCotacaoBancoCentral;
                            tituloBaixa.DataBaseCRT = titulo.DataBaseCRT;
                            tituloBaixa.ValorMoedaCotacao = titulo.ValorMoedaCotacao;
                            tituloBaixa.ValorOriginalMoedaEstrangeira = titulo.ValorOriginalMoedaEstrangeira;

                            repTituloBaixa.Inserir(tituloBaixa, Auditado);

                            Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado tituloBaixaAgrupado = new Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado();
                            tituloBaixaAgrupado.Titulo = titulo;
                            tituloBaixaAgrupado.TituloBaixa = tituloBaixa;
                            tituloBaixaAgrupado.DataBaixa = tituloBaixa.DataOperacao.Value.Date;
                            tituloBaixaAgrupado.DataBase = tituloBaixa.DataBase.Value;
                            repTituloBaixaAgrupado.Inserir(tituloBaixaAgrupado, Auditado);

                            if (titulo.TipoTitulo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTitulo.Pagar)
                            {
                                Dominio.Entidades.Embarcador.Financeiro.TituloBaixaTipoPagamentoRecebimento tituloBaixaTipoPagamentoRecebimento = new Dominio.Entidades.Embarcador.Financeiro.TituloBaixaTipoPagamentoRecebimento();
                                tituloBaixaTipoPagamentoRecebimento.TituloBaixa = tituloBaixa;
                                tituloBaixaTipoPagamentoRecebimento.TipoPagamentoRecebimento = tipoPagamentoRecebimento;
                                tituloBaixaTipoPagamentoRecebimento.Valor = titulo.ValorPago;
                                tituloBaixaTipoPagamentoRecebimento.MoedaCotacaoBancoCentral = titulo.MoedaCotacaoBancoCentral;
                                tituloBaixaTipoPagamentoRecebimento.DataBaseCRT = titulo.DataBaseCRT;
                                tituloBaixaTipoPagamentoRecebimento.ValorMoedaCotacao = titulo.ValorMoedaCotacao;
                                tituloBaixaTipoPagamentoRecebimento.ValorOriginalMoedaEstrangeira = titulo.ValorOriginalMoedaEstrangeira;

                                repTituloBaixaTipoPagamentoRecebimento.Inserir(tituloBaixaTipoPagamentoRecebimento, Auditado);

                                servProcessoMovimento.GerarMovimentacao(null, titulo.DataLiquidacao.Value, titulo.ValorPago, tituloBaixa.Codigo.ToString(), "BAIXA DO TITULO A PAGAR" + observacao, unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Pagamento, TipoServicoMultisoftware, 0, tipoPagamentoRecebimento.PlanoConta, titulo.TipoMovimento.PlanoDeContaCredito, 0, null, titulo.Pessoa, titulo.Pessoa.GrupoPessoas, null, null, null, null, null, titulo.MoedaCotacaoBancoCentral, titulo.DataBaseCRT, titulo.ValorMoedaCotacao, titulo.ValorOriginalMoedaEstrangeira, null, null, titulo.FormaTitulo);
                            }
                            else if (titulo.TipoTitulo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTitulo.Receber)
                                servProcessoMovimento.GerarMovimentacao(null, titulo.DataLiquidacao.Value, titulo.ValorPago, tituloBaixa.Codigo.ToString(), "BAIXA DO TITULO A RECEBER " + observacao, unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Recebimento, TipoServicoMultisoftware, 0, titulo.TipoMovimento.PlanoDeContaDebito, tipoPagamentoRecebimento.PlanoConta, 0, null, titulo.Pessoa, titulo.Pessoa.GrupoPessoas, null, null, null, null, null, titulo.MoedaCotacaoBancoCentral, titulo.DataBaseCRT, titulo.ValorMoedaCotacao, titulo.ValorOriginalMoedaEstrangeira, null, null, titulo.FormaTitulo);
                        }
                    }
                    primeiroTitulo = false;
                }

                unitOfWork.CommitChanges();

                var dynLancamentoConta = new
                {
                    TipoDocumento = tipoDocumento,
                    NumeroDocumento = documento,
                    Valor = titulos.Sum(t => t.ValorOriginal).ToString("n2"),
                    NomePessoa = cobrancaManual?.Pessoa?.Nome ?? "",
                    CNPJPessoa = cobrancaManual?.Pessoa?.CPF_CNPJ ?? 0d,
                    NomeColaborador = this.Usuario?.Nome ?? "",
                    CodigoColaborador = this.Usuario?.Codigo ?? 0
                };
                return new JsonpResult(dynLancamentoConta);
            }
            catch (ControllerException ex)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, ex.Message);
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

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaTitulos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Dominio.Entidades.Empresa empresa = this.Usuario.Empresa;

                dynamic dynContas = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("Conta"));

                DateTime dataVencimento, dataRecebimento, dataEmissao;
                DateTime.TryParse((string)dynContas.DataVencimento, out dataVencimento);
                DateTime.TryParse((string)dynContas.DataRecebimento, out dataRecebimento);
                DateTime.TryParse((string)dynContas.DataEmissao, out dataEmissao);
                DateTime? dataBaseCRT = ((string)dynContas.DataBaseCRT).ToNullableDateTime();

                decimal valor, valorMulta = 0, valorDesconto = 0, valorRecebido = 0, valorJuros = 0;
                decimal.TryParse((string)dynContas.Valor, out valor);
                decimal.TryParse((string)dynContas.ValorMulta, out valorMulta);
                decimal.TryParse((string)dynContas.ValorDesconto, out valorDesconto);
                decimal.TryParse((string)dynContas.ValorRecebido, out valorRecebido);
                decimal.TryParse((string)dynContas.ValorJuros, out valorJuros);
                decimal.TryParse((string)dynContas.ValorMoedaCotacao, out decimal valorMoedaCotacao);

                int numeroOcorrencia = dynContas.NumeroOcorrencia != null ? (int)dynContas.NumeroOcorrencia : 0;
                bool recebido = (bool)dynContas.Recebido;
                bool repetir = (bool)dynContas.Repetir;
                bool dividir = (bool)dynContas.Dividir;
                bool provisao = (bool)dynContas.Provisao;
                bool simularParcelas = (bool)dynContas.SimularParcelas;

                int tipoRecebimentoPagamento = 0, diaVencimento = 0;
                int.TryParse((string)dynContas.TipoPagamentoRecebimento, out tipoRecebimentoPagamento);
                int.TryParse((string)dynContas.DiaVencimento, out diaVencimento);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo statusTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.EmAberto;
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTitulo tipoTitulo;
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.Periodicidade periodicidade;
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaTitulo formaTitulo;
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral moedaCotacaoBancoCentral;
                Enum.TryParse((string)dynContas.Tipo, out tipoTitulo);
                Enum.TryParse((string)dynContas.TipoRepetir, out periodicidade);
                Enum.TryParse((string)dynContas.FormaTitulo, out formaTitulo);
                Enum.TryParse((string)dynContas.MoedaCotacaoBancoCentral, out moedaCotacaoBancoCentral);

                double pessoa = 0;
                double.TryParse((string)dynContas.Pessoa, out pessoa);
                int codigoEmpresa = 0;
                int.TryParse((string)dynContas.Empresa, out codigoEmpresa);
                int tipoMovimento = (int)dynContas.TipoMovimento;
                int tipoMovimentoJuros = (int)dynContas.TipoMovimentoJuros;

                string observacao = (string)dynContas.Observacao;
                string documento = (string)dynContas.Documento;
                string tipoDocumento = (string)dynContas.TipoDocumento;

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                {
                    empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);
                    if (empresa == null)
                        return new JsonpResult(false, "Favor selecione a empresa.");
                }

                List<Dominio.Entidades.Embarcador.Financeiro.Titulo> titulos = RetornaListaTitulos(unitOfWork, dataVencimento, recebido, numeroOcorrencia, repetir, valor, valorDesconto, valorMulta, valorRecebido, periodicidade, diaVencimento, dataEmissao, observacao,
                    tipoTitulo, documento, valorJuros, statusTitulo, tipoMovimento, pessoa, empresa,
                    dataRecebimento, formaTitulo, provisao, tipoDocumento, moedaCotacaoBancoCentral, dataBaseCRT, valorMoedaCotacao);
                int i = 1;
                var dynRetorno = new
                {
                    ListaTitulos = (from p in titulos
                                    select new
                                    {
                                        Codigo = Guid.NewGuid().ToString(),
                                        Sequencia = i++,
                                        Observacao = p.Observacao,
                                        Documento = p.NumeroDocumentos,
                                        TipoDocumento = p.TipoDocumentoTituloOriginal,
                                        DataVencimento = p.DataVencimento.Value.ToString("dd/MM/yyyy"),
                                        Valor = p.ValorOriginal.ToString("n2"),
                                        Juros = p.Acrescimo.ToString("n2")
                                    }).ToList()
                };


                return new JsonpResult(dynRetorno);

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

        #endregion

        #region Métodos Privados

        private List<Dominio.Entidades.Embarcador.Financeiro.Titulo> RetornaListaTitulos(Repositorio.UnitOfWork unitOfWork, DateTime dataVencimento, bool recebido, int numeroOcorrencia, bool repetir, decimal valor, decimal valorDesconto, decimal valorMulta, decimal valorRecebido, Dominio.ObjetosDeValor.Embarcador.Enumeradores.Periodicidade periodicidade, int diaVencimento, DateTime dataEmissao, string observacao,
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTitulo tipoTitulo, string documento, decimal valorJuros, Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo statusTitulo, int tipoMovimento, double pessoa, Dominio.Entidades.Empresa empresa,
            DateTime dataRecebimento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaTitulo formaTitulo, bool provisao, string tipoDocumento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral moedaCotacaoBancoCentral, DateTime? dataBaseCRT, decimal valorMoedaCotacao)
        {
            Servicos.WebService.Empresa.Empresa serEmpresa = new Servicos.WebService.Empresa.Empresa(unitOfWork);
            Servicos.Cliente serCliente = new Servicos.Cliente(_conexao.StringConexao);
            Repositorio.Embarcador.Financeiro.TipoMovimento repTipoMovimento = new Repositorio.Embarcador.Financeiro.TipoMovimento(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

            List<Dominio.Entidades.Embarcador.Financeiro.Titulo> listaRetorno = new List<Dominio.Entidades.Embarcador.Financeiro.Titulo>();
            decimal valorParcela = 0, valorPendente = 0, valorRecebidoParcela = 0, descontoParcela = 0, multaParcela = 0, valorJurosParcela = 0;
            DateTime dataVencimentoParcela;
            dataVencimentoParcela = dataVencimento;
            if (recebido)
                numeroOcorrencia = numeroOcorrencia + 1;
            else if (numeroOcorrencia == 0)
                numeroOcorrencia = 1;
            if (repetir)
            {
                valorParcela = valor;
                valorJurosParcela = valorJuros;
            }
            else
            {
                valorParcela = valor / numeroOcorrencia;
                valorJurosParcela = valorJuros / numeroOcorrencia;
                if (valorDesconto > 0)
                    descontoParcela = valorDesconto / numeroOcorrencia;
                multaParcela = valorMulta / numeroOcorrencia;
                if (valorMulta > 0)
                    valorRecebidoParcela = valor / numeroOcorrencia;

                valorDesconto = descontoParcela;
                valorMulta = multaParcela;
            }

            for (int i = 0; i < numeroOcorrencia; i++)
            {
                if (i == 0)
                    valorRecebidoParcela = valorRecebido;
                else
                {
                    valorRecebidoParcela = 0;
                    if (periodicidade == Dominio.ObjetosDeValor.Embarcador.Enumeradores.Periodicidade.Semanal)
                        dataVencimentoParcela = dataVencimentoParcela.AddDays(7);
                    else if (periodicidade == Dominio.ObjetosDeValor.Embarcador.Enumeradores.Periodicidade.Mensal)
                        dataVencimentoParcela = dataVencimentoParcela.AddMonths(1);
                    else if (periodicidade == Dominio.ObjetosDeValor.Embarcador.Enumeradores.Periodicidade.Bimestral)
                        dataVencimentoParcela = dataVencimentoParcela.AddMonths(2);
                    else if (periodicidade == Dominio.ObjetosDeValor.Embarcador.Enumeradores.Periodicidade.Trimestral)
                        dataVencimentoParcela = dataVencimentoParcela.AddMonths(3);
                    else if (periodicidade == Dominio.ObjetosDeValor.Embarcador.Enumeradores.Periodicidade.Semestral)
                        dataVencimentoParcela = dataVencimentoParcela.AddMonths(6);
                    else if (periodicidade == Dominio.ObjetosDeValor.Embarcador.Enumeradores.Periodicidade.Anual)
                        dataVencimentoParcela = dataVencimentoParcela.AddYears(1);
                }

                if (diaVencimento > 0 && periodicidade != Dominio.ObjetosDeValor.Embarcador.Enumeradores.Periodicidade.Semanal)
                {
                    DateTime? novaData = null;
                    try
                    {
                        novaData = new DateTime(dataVencimentoParcela.Year, dataVencimentoParcela.Month, diaVencimento);
                    }
                    catch (Exception)
                    {
                        novaData = dataVencimentoParcela;
                    }
                    if (novaData == null)
                        novaData = dataVencimentoParcela;

                    dataVencimentoParcela = novaData.Value;
                }

                if (valorParcela > (valorRecebidoParcela + valorDesconto - valorMulta))
                    valorPendente = valorParcela - valorRecebidoParcela;
                else
                    valorPendente = 0;

                Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = new Dominio.Entidades.Embarcador.Financeiro.Titulo();

                titulo.BoletoStatusTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.BoletoStatusTitulo.Nenhum;
                titulo.DataEmissao = dataEmissao;
                titulo.DataVencimento = dataVencimentoParcela;
                titulo.Observacao = observacao;
                titulo.Sequencia = i + 1;
                titulo.TipoTitulo = tipoTitulo;
                titulo.ValorOriginal = valorParcela;
                titulo.ValorPendente = valorPendente;
                titulo.NumeroDocumentos = documento;
                titulo.NumeroDocumentoTituloOriginal = documento;
                titulo.TipoDocumentoTituloOriginal = tipoDocumento;
                titulo.Acrescimo = valorJurosParcela;
                titulo.Usuario = this.Usuario;
                titulo.DataLancamento = DateTime.Now;
                if (valorPendente == 0)
                    titulo.StatusTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.Quitada;
                else
                    titulo.StatusTitulo = statusTitulo;

                titulo.DataAlteracao = DateTime.Now;
                titulo.TipoMovimento = repTipoMovimento.BuscarPorCodigo(tipoMovimento);

                if (pessoa > 0)
                    titulo.Pessoa = repCliente.BuscarPorCPFCNPJ(pessoa);
                else
                {
                    Dominio.Entidades.Cliente cliente = repCliente.BuscarPorCPFCNPJ(double.Parse(empresa.CNPJ));
                    if (cliente != null)
                        titulo.Pessoa = cliente;
                    else
                    {
                        Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa clienteEmpresa = serEmpresa.ConverterObjetoEmpresa(empresa);
                        serCliente.ConverterParaTransportadorTerceiro(clienteEmpresa, "Empresa", unitOfWork);
                        titulo.Pessoa = repCliente.BuscarPorCPFCNPJ(double.Parse(empresa.CNPJ));
                    }
                }
                titulo.GrupoPessoas = titulo.Pessoa?.GrupoPessoas;
                titulo.Empresa = empresa;
                if (i == 0 && recebido)
                {
                    titulo.DataLiquidacao = dataRecebimento;
                    titulo.DataBaseLiquidacao = dataRecebimento;
                    titulo.Acrescimo = valorMulta;
                    titulo.Desconto = valorDesconto;
                    titulo.ValorPago = valorRecebidoParcela;
                }
                else
                {
                    titulo.DataLiquidacao = null;
                    titulo.DataBaseLiquidacao = null;
                }
                titulo.LiberadoPagamento = false;
                titulo.FormaTitulo = formaTitulo;
                titulo.Provisao = provisao;

                titulo.MoedaCotacaoBancoCentral = moedaCotacaoBancoCentral;
                titulo.DataBaseCRT = dataBaseCRT;
                titulo.ValorMoedaCotacao = valorMoedaCotacao;
                if (titulo.ValorMoedaCotacao > 0)
                    titulo.ValorOriginalMoedaEstrangeira = (titulo.ValorOriginal * titulo.ValorMoedaCotacao);
                else
                    titulo.ValorOriginalMoedaEstrangeira = 0;

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    titulo.TipoAmbiente = this.Usuario.Empresa.TipoAmbiente;

                listaRetorno.Add(titulo);
            }

            return listaRetorno;
        }

        private List<Dominio.Entidades.Embarcador.Financeiro.Titulo> RetornaTitulosSimulacao(Repositorio.UnitOfWork unidadeDeTrabalho, DateTime dataVencimento, bool recebido, int numeroOcorrencia, bool repetir, decimal valor, decimal valorDesconto, decimal valorMulta, decimal valorRecebido, Dominio.ObjetosDeValor.Embarcador.Enumeradores.Periodicidade periodicidade, int diaVencimento, DateTime dataEmissao, string observacao,
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTitulo tipoTitulo, string documento, decimal valorJuros, Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo statusTitulo, int tipoMovimento, double pessoa, Dominio.Entidades.Empresa empresa,
            DateTime dataRecebimento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaTitulo formaTitulo, bool provisao, string tipoDocumento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral moedaCotacaoBancoCentral, DateTime? dataBaseCRT, decimal valorMoedaCotacao)
        {
            Servicos.WebService.Empresa.Empresa serEmpresa = new Servicos.WebService.Empresa.Empresa(unidadeDeTrabalho);
            Servicos.Cliente serCliente = new Servicos.Cliente(_conexao.StringConexao);
            Repositorio.Embarcador.Financeiro.TipoMovimento repTipoMovimento = new Repositorio.Embarcador.Financeiro.TipoMovimento(unidadeDeTrabalho);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeDeTrabalho);

            List<Dominio.Entidades.Embarcador.Financeiro.Titulo> listaTitulos = new List<Dominio.Entidades.Embarcador.Financeiro.Titulo>();
            if (!string.IsNullOrWhiteSpace(Request.Params("ListaTitulos")))
            {
                dynamic listaTitulo = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ListaTitulos"));
                if (listaTitulo != null)
                {
                    int i = 0;
                    foreach (var vTitulo in listaTitulo)
                    {
                        Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = new Dominio.Entidades.Embarcador.Financeiro.Titulo();

                        titulo.BoletoStatusTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.BoletoStatusTitulo.Nenhum;
                        titulo.DataEmissao = dataEmissao;
                        titulo.DataVencimento = DateTime.Parse((string)vTitulo.Titulo.DataVencimento);
                        titulo.Observacao = (string)vTitulo.Titulo.Observacao;
                        titulo.Sequencia = (int)vTitulo.Titulo.Sequencia;
                        titulo.TipoTitulo = tipoTitulo;
                        titulo.ValorOriginal = decimal.Parse((string)vTitulo.Titulo.Valor);
                        titulo.ValorPendente = decimal.Parse((string)vTitulo.Titulo.Valor);
                        titulo.NumeroDocumentos = (string)vTitulo.Titulo.Documento;
                        titulo.NumeroDocumentoTituloOriginal = (string)vTitulo.Titulo.Documento;
                        titulo.TipoDocumentoTituloOriginal = (string)vTitulo.Titulo.TipoDocumento;
                        titulo.Acrescimo = decimal.Parse((string)vTitulo.Titulo.Juros);
                        titulo.StatusTitulo = statusTitulo;
                        titulo.DataAlteracao = DateTime.Now;
                        titulo.TipoMovimento = repTipoMovimento.BuscarPorCodigo(tipoMovimento);
                        titulo.Usuario = this.Usuario;
                        titulo.DataLancamento = DateTime.Now;
                        if (pessoa > 0)
                            titulo.Pessoa = repCliente.BuscarPorCPFCNPJ(pessoa);
                        else
                        {
                            Dominio.Entidades.Cliente cliente = repCliente.BuscarPorCPFCNPJ(double.Parse(empresa.CNPJ));
                            if (cliente != null)
                                titulo.Pessoa = cliente;
                            else
                            {
                                Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa clienteEmpresa = serEmpresa.ConverterObjetoEmpresa(empresa);
                                serCliente.ConverterParaTransportadorTerceiro(clienteEmpresa, "Empresa", unidadeDeTrabalho);
                                titulo.Pessoa = repCliente.BuscarPorCPFCNPJ(double.Parse(empresa.CNPJ));
                            }
                        }
                        titulo.GrupoPessoas = titulo.Pessoa?.GrupoPessoas;
                        titulo.Empresa = empresa;
                        if (i == 0 && recebido)
                        {
                            titulo.DataLiquidacao = dataRecebimento;
                            titulo.DataBaseLiquidacao = dataRecebimento;
                            titulo.Acrescimo = valorMulta;
                            titulo.Desconto = valorDesconto;
                            titulo.ValorPago = titulo.ValorOriginal;
                        }
                        else
                        {
                            titulo.DataLiquidacao = null;
                            titulo.DataBaseLiquidacao = null;
                        }
                        titulo.LiberadoPagamento = false;
                        titulo.FormaTitulo = formaTitulo;
                        titulo.Provisao = provisao;

                        titulo.MoedaCotacaoBancoCentral = moedaCotacaoBancoCentral;
                        titulo.DataBaseCRT = dataBaseCRT;
                        titulo.ValorMoedaCotacao = valorMoedaCotacao;
                        if (titulo.ValorMoedaCotacao > 0)
                            titulo.ValorOriginalMoedaEstrangeira = (titulo.ValorOriginal * titulo.ValorMoedaCotacao);
                        else
                            titulo.ValorOriginalMoedaEstrangeira = 0;

                        if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                            titulo.TipoAmbiente = this.Usuario.Empresa.TipoAmbiente;

                        listaTitulos.Add(titulo);
                        i = i + 1;
                    }
                }
            }
            return listaTitulos;
        }

        private void SalvarVeiculos(Dominio.Entidades.Embarcador.Financeiro.Titulo titulo, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);

            dynamic veiculos = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ListaVeiculos"));

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
                foreach (dynamic veiculo in veiculos)
                {
                    if (!titulo.Veiculos.Any(o => o.Codigo == (int)veiculo))
                    {
                        Dominio.Entidades.Veiculo veic = repVeiculo.BuscarPorCodigo((int)veiculo);
                        titulo.Veiculos.Add(veic);
                    }
                }
            }
        }

        #endregion
    }
}
