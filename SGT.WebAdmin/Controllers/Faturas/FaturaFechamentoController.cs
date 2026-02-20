using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Newtonsoft.Json;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.AspNetCore.Mvc;


namespace SGT.WebAdmin.Controllers.Faturas
{
    [CustomAuthorize("Faturas/Fatura", "SAC/AtendimentoCliente")]
    public class FaturaFechamentoController : BaseController
    {
		#region Construtores

		public FaturaFechamentoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> ObterTipoGeracaoTitulosTomador()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                double cpfCnpjTomador;
                double.TryParse(Utilidades.String.OnlyNumbers(Request.Params("CPFCNPJTomador")), out cpfCnpjTomador);

                Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeTrabalho);

                Dominio.Entidades.Cliente cliente = repCliente.BuscarPorCPFCNPJ(cpfCnpjTomador);

                if (cliente == null)
                    return new JsonpResult(false, true, "Tomador não encontrado.");

                bool gerarTituloPorDocumento = false;

                if (cliente.NaoUsarConfiguracaoEmissaoGrupo)
                {
                    if (cliente.GerarTituloPorDocumentoFiscal)
                        gerarTituloPorDocumento = true;
                }
                else if (cliente.GrupoPessoas != null && cliente.GrupoPessoas.GerarTituloPorDocumentoFiscal)
                {
                    gerarTituloPorDocumento = true;
                }

                return new JsonpResult(new
                {
                    GerarTituloPorDocumentoFiscal = gerarTituloPorDocumento
                });
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao obter a informação de geração de títulos do tomador.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        [AllowAnonymous]
        public async Task<IActionResult> CarregarDadosFechamento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Fatura.Fatura repFatura = new Repositorio.Embarcador.Fatura.Fatura(unitOfWork);

                int codigoFatura = 0;
                int.TryParse(Request.Params("CodigoFatura"), out codigoFatura);

                Dominio.Entidades.Embarcador.Fatura.Fatura fatura = repFatura.BuscarPorCodigo(codigoFatura);

                bool gerarTituloPorDocumento = false;

                if (fatura.ClienteTomadorFatura != null)
                {
                    if (fatura.ClienteTomadorFatura.NaoUsarConfiguracaoEmissaoGrupo)
                    {
                        if (fatura.ClienteTomadorFatura.GerarTituloPorDocumentoFiscal)
                            gerarTituloPorDocumento = true;
                    }
                    else if (fatura.ClienteTomadorFatura.GrupoPessoas != null && fatura.ClienteTomadorFatura.GrupoPessoas.GerarTituloPorDocumentoFiscal)
                    {
                        gerarTituloPorDocumento = true;
                    }
                }

                Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao = null;
                bool permiteRemoverCTesIntegracao = false;

                if (fatura.Cliente != null && fatura.Cliente.NaoUsarConfiguracaoEmissaoGrupo)
                    tipoIntegracao = fatura.Cliente.TipoIntegracao;
                else if (fatura.GrupoPessoas != null)
                    tipoIntegracao = fatura.GrupoPessoas.TipoIntegracao;

                if (tipoIntegracao != null && tipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Avon)
                    permiteRemoverCTesIntegracao = true;

                var dynRetorno = new
                {
                    Codigo = fatura.Codigo,
                    ValorTotal = fatura.NovoModelo ? fatura.TotalLiquido.ToString("n2") : fatura.Total.ToString("n2"),
                    Descontos = fatura.NovoModelo ? fatura.Desconto.ToString("n2") : fatura.Descontos.ToString("n2"),
                    Acrescimos = fatura.NovoModelo ? fatura.Acrescimo.ToString("n2") : fatura.Acrescimos.ToString("n2"),
                    TotalGeral = fatura.NovoModelo ? fatura.Total.ToString("n2") : (fatura.Total - fatura.Descontos + fatura.Acrescimos).ToString("n2"),
                    AcrescimosMoeda = fatura.AcrescimosMoeada.ToString("n2"),
                    DescontosMoeda = fatura.DescontosMoeda.ToString("n2"),
                    ValorTotalMoeda = (fatura.TotalMoedaEstrangeira - fatura.DescontosMoeda + fatura.AcrescimosMoeada).ToString("n2"),
                    Moeda = fatura.MoedaCotacaoBancoCentral?.ObterDescricao(),
                    TipoMoeda = fatura.MoedaCotacaoBancoCentral,
                    ValorDesconto = 0.ToString("n2"),
                    JustificativaDesconto = string.Empty,
                    ValorAcrescimo = 0.ToString("n2"),
                    JustificativaAcrescimo = string.Empty,
                    ObservacaoFatura = fatura.ObservacaoFatura,
                    ImprimirObservacaoFatura = fatura.ImprimeObservacaoFatura,
                    GerarTituloPorDocumentoFiscal = gerarTituloPorDocumento,
                    PermiteRemoverCTesIntegracao = permiteRemoverCTesIntegracao,
                    Parcelas = fatura.Parcelas != null ? (from obj in fatura.Parcelas
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
                                                              Valor = obj.Valor.ToString("n2"),
                                                              obj.FormaTitulo
                                                          }).ToList() : null,
                    TomadorFatura = fatura.ClienteTomadorFatura != null ? new { Codigo = fatura.ClienteTomadorFatura.CPF_CNPJ, Descricao = fatura.ClienteTomadorFatura.Nome + " ( " + fatura.ClienteTomadorFatura.CPF_CNPJ_Formatado + " ) " } : null,
                    EmpresaFatura = fatura.Empresa != null ? new { Codigo = fatura.Empresa.Codigo, Descricao = fatura.Empresa.RazaoSocial + " ( " + fatura.Empresa.Localidade.DescricaoCidadeEstado + " ) " } : null,
                    Banco = fatura.Banco != null ? new { Codigo = fatura.Banco.Codigo, Descricao = fatura.Banco.Descricao } : null,
                    Agencia = fatura.Agencia,
                    Digito = fatura.DigitoAgencia,
                    NumeroConta = fatura.NumeroConta,
                    TipoConta = fatura.TipoContaBanco
                };

                return new JsonpResult(dynRetorno);

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar os dados para o fechamento.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAnonymous]
        public async Task<IActionResult> PesquisaParcelaFatura()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoFatura = 0;
                int.TryParse(Request.Params("Codigo"), out codigoFatura);

                Repositorio.Embarcador.Fatura.FaturaParcela repFaturaParcela = new Repositorio.Embarcador.Fatura.FaturaParcela(unitOfWork);
                Repositorio.Embarcador.Fatura.Fatura repFatura = new Repositorio.Embarcador.Fatura.Fatura(unitOfWork);

                Dominio.Entidades.Embarcador.Fatura.Fatura fatura = repFatura.BuscarPorCodigo(codigoFatura);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("CodigoFatura", false);
                grid.AdicionarCabecalho("CodigoTitulo", false);
                grid.AdicionarCabecalho("Parcela", "Parcela", 8, Models.Grid.Align.center, true);

                if (fatura != null && !fatura.NovoModelo)
                {
                    grid.AdicionarCabecalho("Valor", "Valor", 10, Models.Grid.Align.right, false);
                    grid.AdicionarCabecalho("Desconto", "Desconto", 10, Models.Grid.Align.right, false);
                    grid.AdicionarCabecalho("Data Vencimento", "DataVencimento", 20, Models.Grid.Align.center, true);
                    grid.AdicionarCabecalho("Situação", "DescricaoSituacao", 20, Models.Grid.Align.left, false);
                    grid.AdicionarCabecalho("Forma", "FormaTitulo", 20, Models.Grid.Align.left, false);
                }
                else
                {
                    if ((fatura?.MoedaCotacaoBancoCentral.HasValue ?? false) && fatura.MoedaCotacaoBancoCentral != MoedaCotacaoBancoCentral.Real)
                    {
                        grid.AdicionarCabecalho("Data Vencimento", "DataVencimento", 15, Models.Grid.Align.center, true);
                        grid.AdicionarCabecalho("Valor", "Valor", 9, Models.Grid.Align.right, false);
                        grid.AdicionarCabecalho("Moeda", "Moeda", 15, Models.Grid.Align.left, false);
                        grid.AdicionarCabecalho("Valor Moeda", "ValorTotalMoeda", 9, Models.Grid.Align.right, false);
                        grid.AdicionarCabecalho("Situação", "DescricaoSituacao", 15, Models.Grid.Align.left, false);
                        grid.AdicionarCabecalho("Forma", "FormaTitulo", 15, Models.Grid.Align.left, false);

                    }
                    else
                    {
                        grid.AdicionarCabecalho("Data Vencimento", "DataVencimento", 15, Models.Grid.Align.center, true);
                        grid.AdicionarCabecalho("Valor", "Valor", 9, Models.Grid.Align.right, false);
                        grid.AdicionarCabecalho("Situação", "DescricaoSituacao", 20, Models.Grid.Align.left, false);
                        grid.AdicionarCabecalho("Forma", "FormaTitulo", 15, Models.Grid.Align.left, false);
                    }
                }

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                if (propOrdenar == "Parcela")
                    propOrdenar = "Sequencia";

                List<Dominio.Entidades.Embarcador.Fatura.FaturaParcela> listaFaturaParcela = repFaturaParcela.BuscarPorFatura(codigoFatura, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repFaturaParcela.ContarBuscarPorFatura(codigoFatura));
                var dynRetorno = (from obj in listaFaturaParcela
                                  select new
                                  {
                                      obj.Codigo,
                                      CodigoFatura = obj.Fatura.Codigo,
                                      obj.CodigoTitulo,
                                      Parcela = obj.Sequencia.ToString("n0"),
                                      Valor = obj.Valor.ToString("n2"),
                                      Moeda = fatura?.MoedaCotacaoBancoCentral?.ObterDescricao() ?? string.Empty,
                                      ValorTotalMoeda = obj.ValorTotalMoeda.ToString("n2"),
                                      Desconto = obj.Desconto.ToString("n2"),
                                      DataVencimento = obj.DataVencimento.ToString("dd/MM/yyyy"),
                                      DescricaoSituacao = obj.Titulos?.Count > 0 ? (from o in obj.Titulos select o.DescricaoSituacao).FirstOrDefault() : obj.DescricaoSituacao,
                                      FormaTitulo = obj.FormaTitulo.HasValue ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaTituloHelper.ObterDescricao(obj.FormaTitulo.Value) : ""
                                  }).ToList();

                grid.AdicionaRows(dynRetorno);
                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar as parcelas da fatura.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> SalvarValores()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Faturas/Fatura");
                if (!(this.Usuario.UsuarioAdministrador || permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Fatura_PermiteSalvarValoresFechamento)))
                    return new JsonpResult(false, "Seu usuário não possui permissão para salvar os valores informados.");

                Servicos.Embarcador.Fatura.Fatura servFatura = new Servicos.Embarcador.Fatura.Fatura(unitOfWork);
                Repositorio.Embarcador.Fatura.Fatura repFatura = new Repositorio.Embarcador.Fatura.Fatura(unitOfWork);
                Repositorio.Embarcador.Fatura.FaturaParcela repFaturaParcela = new Repositorio.Embarcador.Fatura.FaturaParcela(unitOfWork);
                Repositorio.Embarcador.Fatura.Justificativa repJustificativa = new Repositorio.Embarcador.Fatura.Justificativa(unitOfWork);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Repositorio.Banco repBanco = new Repositorio.Banco(unitOfWork);

                int codigoFatura = 0;
                int.TryParse(Request.Params("Codigo"), out codigoFatura);

                double tomadorFatura;
                int empresaFatura = 0;
                double.TryParse(Request.Params("TomadorFatura"), out tomadorFatura);
                int.TryParse(Request.Params("EmpresaFatura"), out empresaFatura);

                int banco = 0;
                int.TryParse(Request.Params("Banco"), out banco);

                string agencia = Request.Params("Agencia");
                string digito = Request.Params("Digito");
                string numeroConta = Request.Params("NumeroConta");

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaBanco tipoConta;
                Enum.TryParse(Request.Params("TipoConta"), out tipoConta);

                string observacaoFatura = Request.Params("ObservacaoFatura");

                bool imprimirObservacaoFatura = false;
                bool.TryParse(Request.Params("ImprimirObservacaoFatura"), out imprimirObservacaoFatura);

                Dominio.Entidades.Embarcador.Fatura.Fatura fatura = repFatura.BuscarPorCodigo(codigoFatura, true);

                unitOfWork.Start();

                if (tomadorFatura > 0)
                    fatura.ClienteTomadorFatura = repCliente.BuscarPorCPFCNPJ(tomadorFatura);
                else
                    fatura.ClienteTomadorFatura = null;

                if (empresaFatura > 0)
                    fatura.Empresa = repEmpresa.BuscarPorCodigo(empresaFatura);
                else
                    fatura.Empresa = null;

                if (banco > 0)
                    fatura.Banco = repBanco.BuscarPorCodigo(banco);
                else
                    fatura.Banco = null;

                fatura.Agencia = agencia;
                fatura.DigitoAgencia = digito;
                fatura.NumeroConta = numeroConta;
                fatura.TipoContaBanco = tipoConta;

                fatura.ObservacaoFatura = observacaoFatura;
                fatura.ImprimeObservacaoFatura = imprimirObservacaoFatura;


                //fatura.MoedaCotacaoBancoCentral = Request.GetNullableEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral>("MoedaCotacaoBancoCentral");
                //fatura.DataBaseCRT = Request.GetNullableDateTimeParam("DataBaseCRT");
                //fatura.ValorMoedaCotacao = Request.GetDecimalParam("ValorMoedaCotacao");
                //fatura.TotalMoedaEstrangeira = Request.GetDecimalParam("TotalMoedaEstrangeira");

                repFatura.Atualizar(fatura, Auditado);

                servFatura.InserirLog(fatura, unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLogFatura.SalvouValorFatura, this.Usuario);

                unitOfWork.CommitChanges();

                return new JsonpResult(true, "Sucesso");
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha salvar os valores.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> SalvarObservacaoFatura()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Fatura.Fatura repFatura = new Repositorio.Embarcador.Fatura.Fatura(unitOfWork);

                int codigoFatura = Request.GetIntParam("Codigo");

                string observacaoFatura = Request.GetStringParam("ObservacaoFatura");

                bool imprimirObservacaoFatura = Request.GetBoolParam("ImprimirObservacaoFatura");

                Dominio.Entidades.Embarcador.Fatura.Fatura fatura = repFatura.BuscarPorCodigo(codigoFatura, true);

                if (fatura.Situacao != SituacaoFatura.Fechado && fatura.Situacao != SituacaoFatura.Liquidado)
                    return new JsonpResult(false, true, "A situação da fatura não permite que a observação seja alterada.");

                if (fatura.FaturaRecebidaDeIntegracao)
                    return new JsonpResult(false, true, "Não é possível realizar a operação para uma fatura recebida pela integração.");

                unitOfWork.Start();

                fatura.ObservacaoFatura = observacaoFatura;
                fatura.ImprimeObservacaoFatura = imprimirObservacaoFatura;

                repFatura.Atualizar(fatura, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha salvar a observação da fatura.");
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
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Faturas/Fatura");
                if (!(this.Usuario.UsuarioAdministrador || permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Fatura_PermiteGerarParcelas)))
                    return new JsonpResult(false, "Seu usuário não possui permissão para gerar as parcelas da fatura.");

                Servicos.Embarcador.Fatura.Fatura servFatura = new Servicos.Embarcador.Fatura.Fatura(unitOfWork);

                Repositorio.Embarcador.Fatura.Fatura repFatura = new Repositorio.Embarcador.Fatura.Fatura(unitOfWork);
                Repositorio.Embarcador.Fatura.FaturaParcela repFaturaParcela = new Repositorio.Embarcador.Fatura.FaturaParcela(unitOfWork);
                Repositorio.Embarcador.Fatura.FaturaDocumento repFaturaDocumento = new Repositorio.Embarcador.Fatura.FaturaDocumento(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoFatura repConfiguracaoFatura = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFatura(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro repConfiguracaoFinanceiro = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFatura configuracaoFatura = repConfiguracaoFatura.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiro configuracaoFinanceiro = repConfiguracaoFinanceiro.BuscarConfiguracaoPadrao();

                int codigoFatura, quantidadeParcelas, intervaloDeDias = 0;
                int.TryParse(Request.Params("Codigo"), out codigoFatura);
                int.TryParse(Request.Params("QuantidadeParcelas"), out quantidadeParcelas);
                int.TryParse(Request.Params("IntervaloDeDias"), out intervaloDeDias);

                string observacaoFatura = Request.Params("ObservacaoFatura");
                bool imprimirObservacaoFatura = false;
                bool.TryParse(Request.Params("ImprimirObservacaoFatura"), out imprimirObservacaoFatura);

                Repositorio.Banco repBanco = new Repositorio.Banco(unitOfWork);
                int banco = 0;
                int.TryParse(Request.Params("Banco"), out banco);

                string agencia = Request.Params("Agencia");
                string digito = Request.Params("Digito");
                string numeroConta = Request.Params("NumeroConta");

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaBanco tipoConta;
                Enum.TryParse(Request.Params("TipoConta"), out tipoConta);

                DateTime dataPrimeiroVencimento;
                DateTime.TryParse(Request.Params("DataPrimeiroVencimento"), out dataPrimeiroVencimento);

                if (configuracaoFinanceiro.QuantidadeDiasLimiteVencimentoTitulo > 0)
                {
                    DateTime dataLimiteVencimento = DateTime.Now.Date.AddDays(configuracaoFinanceiro.QuantidadeDiasLimiteVencimentoTitulo);
                    int result = DateTime.Compare(dataPrimeiroVencimento, dataLimiteVencimento);

                    if (result > 0)
                        return new JsonpResult(false, $"A data {dataPrimeiroVencimento.ToDateString()} é maior que a data limite estipulada nas configurações.");
                }

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArredondamento tipoArredondamento;
                Enum.TryParse(Request.Params("TipoArredondamento"), out tipoArredondamento);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaTitulo formaTitulo;
                Enum.TryParse(Request.Params("FormaTitulo"), out formaTitulo);

                Dominio.Entidades.Embarcador.Fatura.Fatura fatura = repFatura.BuscarPorCodigo(codigoFatura, true);

                if (fatura.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFatura.Fechado || fatura.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFatura.Cancelado)
                    return new JsonpResult(false, true, "A situação atual da fatura não permite a geração das parcelas.");

                if (fatura.FaturaRecebidaDeIntegracao)
                    return new JsonpResult(false, true, "Não é possível realizar a operação para uma fatura recebida pela integração.");

                bool possuiMoedaEstrangeira = fatura.NaoUtilizarMoedaEstrangeira ? false : repFaturaDocumento.PossuiMoedaEstrangeira(fatura.Codigo);

                List<Dominio.Entidades.Embarcador.Fatura.FaturaParcela> listaParcelas = repFaturaParcela.BuscarPorFatura(codigoFatura);

                unitOfWork.Start();

                fatura.TipoArredondamentoParcelas = tipoArredondamento;
                fatura.ObservacaoFatura = observacaoFatura;
                fatura.ImprimeObservacaoFatura = imprimirObservacaoFatura;

                if (banco > 0)
                    fatura.Banco = repBanco.BuscarPorCodigo(banco);
                else
                    fatura.Banco = null;

                fatura.Agencia = agencia;
                fatura.DigitoAgencia = digito;
                fatura.NumeroConta = numeroConta;
                fatura.TipoContaBanco = tipoConta;

                repFatura.Atualizar(fatura, Auditado);

                for (int i = 0; i < listaParcelas.Count; i++)
                    repFaturaParcela.Deletar(listaParcelas[i], Auditado);

                decimal valorTotal = Math.Round(fatura.Total - fatura.Descontos + fatura.Acrescimos, 2, MidpointRounding.ToEven);
                decimal valorParcela = Math.Round(valorTotal / quantidadeParcelas, 2, MidpointRounding.ToEven);
                decimal valorDiferenca = valorTotal - Math.Round(valorParcela * quantidadeParcelas, 2, MidpointRounding.ToEven);

                decimal valorTotalMoeda = fatura.TotalMoedaEstrangeira - fatura.DescontosMoeda + fatura.AcrescimosMoeada;
                decimal valorMoedaParcela = Math.Round(valorTotalMoeda / quantidadeParcelas, 2, MidpointRounding.ToEven);
                decimal valorDiferencaMoeda = valorTotalMoeda - Math.Round(valorMoedaParcela * quantidadeParcelas, 2, MidpointRounding.ToEven);

                DateTime dataUltimaParcela = dataPrimeiroVencimento;

                for (int i = 0; i < quantidadeParcelas; i++)
                {
                    Dominio.Entidades.Embarcador.Fatura.FaturaParcela parcela = new Dominio.Entidades.Embarcador.Fatura.FaturaParcela();

                    parcela.DataEmissao = fatura.DataFatura;

                    if (i == 0)
                        parcela.DataVencimento = dataPrimeiroVencimento;
                    else
                        parcela.DataVencimento = dataUltimaParcela.AddDays(intervaloDeDias);

                    if (!(configuracaoFatura?.PermitirVencimentoRetroativoFatura ?? false) && parcela.DataVencimento.Date < fatura.DataFatura)
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, false, "Não é possível gerar a parcela com data retroativa da emissão da fatura.");
                    }

                    if (ConfiguracaoEmbarcador.QuantidadeDiasLimiteVencimentoFaturaManual > 0 && (parcela.DataVencimento.Date - DateTime.Now.Date).TotalDays >= ConfiguracaoEmbarcador.QuantidadeDiasLimiteVencimentoFaturaManual)
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, false, "Não é possível gerar a parcela com data de vencimento superior a " + ConfiguracaoEmbarcador.QuantidadeDiasLimiteVencimentoFaturaManual.ToString("n0") + " da data atual.");
                    }

                    dataUltimaParcela = parcela.DataVencimento;

                    parcela.Desconto = 0;
                    parcela.Fatura = fatura;
                    parcela.FormaTitulo = formaTitulo;
                    parcela.Sequencia = i + 1;
                    parcela.SituacaoFaturaParcela = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFaturaParcela.EmAberto;

                    if (i == 0 && tipoArredondamento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArredondamento.Primeira)
                    {
                        parcela.Valor = valorParcela + valorDiferenca;
                        parcela.ValorTotalMoeda = valorMoedaParcela + valorDiferenca;
                    }
                    else if ((i + 1) == quantidadeParcelas && tipoArredondamento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArredondamento.Ultima)
                    {
                        parcela.Valor = valorParcela + valorDiferenca;
                        parcela.ValorTotalMoeda = valorMoedaParcela + valorDiferenca;
                    }
                    else
                    {
                        parcela.Valor = valorParcela;
                        parcela.ValorTotalMoeda = valorMoedaParcela;
                    }

                    repFaturaParcela.Inserir(parcela, Auditado);
                }

                repFatura.Atualizar(fatura);

                servFatura.InserirLog(fatura, unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLogFatura.GerouParcela, this.Usuario);
                servFatura.AtualizarValorVencimento(dataUltimaParcela, fatura.Codigo, unitOfWork);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, fatura, null, "Gerou " + quantidadeParcelas.ToString() + " parcelas para a fatura.", unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha gerar as parcelas.");
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
                Repositorio.Embarcador.Fatura.FaturaParcela repFaturaParcela = new Repositorio.Embarcador.Fatura.FaturaParcela(unitOfWork);

                int codigoParcela = 0;
                int.TryParse(Request.Params("Codigo"), out codigoParcela);
                Dominio.Entidades.Embarcador.Fatura.FaturaParcela parcela = repFaturaParcela.BuscarPorCodigo(codigoParcela);

                var dynRetorno = new
                {
                    Codigo = parcela.Codigo,
                    Sequencia = parcela.Sequencia.ToString("n0"),
                    Valor = parcela.Valor.ToString("n2"),
                    ValorDesconto = parcela.Desconto.ToString("n2"),
                    DataEmissao = parcela.DataEmissao.ToString("dd/MM/yyyy"),
                    DataVencimento = parcela.DataVencimento.ToString("dd/MM/yyyy"),
                    parcela.FormaTitulo
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
                bool permiteAlterarDataVencimento = true;
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Faturas/Fatura");
                if (!this.Usuario.UsuarioAdministrador && permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Fatura_NaoPermitirEditarDataVencimentoParcela))
                    permiteAlterarDataVencimento = false;

                Servicos.Embarcador.Fatura.Fatura servFatura = new Servicos.Embarcador.Fatura.Fatura(unitOfWork);

                Repositorio.Embarcador.Fatura.FaturaParcela repFaturaParcela = new Repositorio.Embarcador.Fatura.FaturaParcela(unitOfWork);
                Repositorio.Embarcador.Fatura.Fatura repFatura = new Repositorio.Embarcador.Fatura.Fatura(unitOfWork);
                Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoFatura repConfiguracaoFatura = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFatura(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro repConfiguracaoFinanceiro = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFatura configuracaoFatura = repConfiguracaoFatura.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiro configuracaoFinanceiro = repConfiguracaoFinanceiro.BuscarConfiguracaoPadrao();

                int codigoParcela, codigoFatura, sequencia = 0, codigoTitulo = 0;
                int.TryParse(Request.Params("CodigoParcela"), out codigoParcela);
                int.TryParse(Request.Params("Codigo"), out codigoFatura);
                int.TryParse(Request.Params("CodigoTitulo"), out codigoTitulo);
                int.TryParse(Request.Params("Sequencia"), out sequencia);

                decimal valor, valorDesconto = 0;
                decimal.TryParse(Request.Params("Valor"), out valor);
                decimal.TryParse(Request.Params("ValorDesconto"), out valorDesconto);

                DateTime dataEmissao, dataVencimento;
                DateTime.TryParse(Request.Params("DataVencimento"), out dataVencimento);
                DateTime.TryParse(Request.Params("DataEmissao"), out dataEmissao);

                if (configuracaoFinanceiro.QuantidadeDiasLimiteVencimentoTitulo > 0)
                {
                    DateTime dataLimiteVencimento = DateTime.Now.Date.AddDays(configuracaoFinanceiro.QuantidadeDiasLimiteVencimentoTitulo);
                    int result = DateTime.Compare(dataVencimento, dataLimiteVencimento);

                    if (result > 0)
                        return new JsonpResult(false, $"A data {dataVencimento.ToDateString()} é maior que a data limite estipulada nas configurações.");
                }

                string observacaoFatura = Request.Params("ObservacaoFatura");
                bool imprimirObservacaoFatura = false;
                bool.TryParse(Request.Params("ImprimirObservacaoFatura"), out imprimirObservacaoFatura);

                Repositorio.Banco repBanco = new Repositorio.Banco(unitOfWork);
                int banco = 0;
                int.TryParse(Request.Params("Banco"), out banco);

                string agencia = Request.Params("Agencia");
                string digito = Request.Params("Digito");
                string numeroConta = Request.Params("NumeroConta");

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaBanco tipoConta;
                Enum.TryParse(Request.Params("TipoConta"), out tipoConta);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaTitulo formaTitulo;
                Enum.TryParse(Request.Params("FormaTitulo"), out formaTitulo);

                Dominio.Entidades.Embarcador.Fatura.FaturaParcela parcela = repFaturaParcela.BuscarPorCodigo(codigoParcela, true);

                if (dataVencimento < parcela.DataEmissao)
                    return new JsonpResult(false, "A data de vencimento não pode ser menor que a data de emissão da parcela.");

                Dominio.Entidades.Embarcador.Fatura.Fatura fatura = repFatura.BuscarPorCodigo(codigoFatura, true);

                if (fatura.FaturaRecebidaDeIntegracao)
                    return new JsonpResult(false, true, "Não é possível realizar a operação para uma fatura recebida pela integração.");

                List<Dominio.Entidades.Embarcador.Financeiro.Titulo> titulos = repTitulo.BuscarPorParcelaFatura(codigoParcela);

                if (titulos.Count == 0)
                    titulos = repTitulo.BuscarPorFatura(codigoFatura);

                if (codigoTitulo > 0m && !titulos.Any(o => o.Codigo == codigoTitulo))
                    titulos.Add(repTitulo.BuscarPorCodigo(codigoTitulo));

                unitOfWork.Start();

                if (titulos.Count > 0)
                {
                    foreach (var titulo in titulos)
                    {
                        if (titulo == null)
                            continue;

                        if (titulo.StatusTitulo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.Quitada)
                        {
                            unitOfWork.Rollback();
                            return new JsonpResult(false, $"O título {titulo.Codigo} já se encontra quitado, não sendo possível altera-lo.");
                        }

                        if (!string.IsNullOrWhiteSpace(titulo.NossoNumero))
                        {
                            unitOfWork.Rollback();
                            return new JsonpResult(false, $"É necessário remover os dados de boleto do título {titulo.Codigo} antes de edita-lo.");
                        }

                        titulo.Initialize();

                        if (permiteAlterarDataVencimento)
                            titulo.DataVencimento = dataVencimento;

                        titulo.DataProgramacaoPagamento = dataVencimento;
                        titulo.FormaTitulo = formaTitulo;

                        repTitulo.Atualizar(titulo, Auditado);
                    }
                }

                parcela.Valor = valor;
                parcela.Desconto = valorDesconto;

                if (permiteAlterarDataVencimento)
                    parcela.DataVencimento = dataVencimento;

                parcela.FormaTitulo = formaTitulo;

                decimal valorTotal = Math.Round(fatura.Total - fatura.Descontos + fatura.Acrescimos, 2, MidpointRounding.ToEven);
                decimal percentual = valor / valorTotal;

                parcela.ValorTotalMoeda = Math.Round((fatura.TotalMoedaEstrangeira - fatura.DescontosMoeda + fatura.AcrescimosMoeada) * percentual, 2, MidpointRounding.ToEven);

                if (!(configuracaoFatura?.PermitirVencimentoRetroativoFatura ?? false) && parcela.DataVencimento.Date < fatura.DataFatura)
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, false, "Não é possível gerar a parcela com data retroativa da emissão da fatura.");
                }

                if (ConfiguracaoEmbarcador.QuantidadeDiasLimiteVencimentoFaturaManual > 0 && (parcela.DataVencimento.Date - DateTime.Now.Date).TotalDays >= ConfiguracaoEmbarcador.QuantidadeDiasLimiteVencimentoFaturaManual)
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, false, "Não é possível gerar a parcela com data de vencimento superior a " + ConfiguracaoEmbarcador.QuantidadeDiasLimiteVencimentoFaturaManual.ToString("n0") + " da data atual.");
                }

                fatura.ObservacaoFatura = observacaoFatura;
                fatura.ImprimeObservacaoFatura = imprimirObservacaoFatura;

                if (banco > 0)
                    fatura.Banco = repBanco.BuscarPorCodigo(banco);
                else
                    fatura.Banco = null;

                fatura.Agencia = agencia;
                fatura.DigitoAgencia = digito;
                fatura.NumeroConta = numeroConta;
                fatura.TipoContaBanco = tipoConta;

                repFatura.Atualizar(fatura, Auditado);
                repFaturaParcela.Atualizar(parcela, Auditado);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, fatura, null, $"Atualizou a parcela {parcela.Descricao}.", unitOfWork);

                servFatura.InserirLog(fatura, unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLogFatura.AlterouParcela, this.Usuario);
                servFatura.AtualizarValorVencimento(dataVencimento, fatura.Codigo, unitOfWork);

                decimal valorParcelas = Math.Round(fatura.Parcelas?.Sum(o => o.Valor) ?? 0m, 2, MidpointRounding.ToEven);

                decimal valorMoedaTotal = fatura.TotalMoedaEstrangeira - fatura.DescontosMoeda + fatura.AcrescimosMoeada;
                decimal valorMoedaParcelas = Math.Round(fatura.Parcelas?.Sum(o => o.ValorTotalMoeda) ?? 0m, 2, MidpointRounding.ToEven);

                int qtdParcelas = fatura.Parcelas?.Count ?? 0;

                if (valorParcelas > 0m && (valorParcelas != valorTotal || valorMoedaParcelas != valorMoedaTotal))
                {
                    decimal valorDiferenca = Math.Round(valorTotal - valorParcelas, 2, MidpointRounding.ToEven);
                    decimal valorMoedaDiferenca = Math.Round(valorMoedaTotal - valorMoedaParcelas, 2, MidpointRounding.ToEven);

                    int parcelasRatear = qtdParcelas - sequencia;

                    if (parcelasRatear > 0)
                    {
                        decimal valorRatearParcelas = Math.Round(valorDiferenca / parcelasRatear, 2, MidpointRounding.ToEven);
                        decimal valorMoedaRatearParcelas = Math.Round(valorMoedaDiferenca / parcelasRatear, 2, MidpointRounding.ToEven);

                        for (int i = sequencia; i < qtdParcelas; i++)
                        {
                            fatura.Parcelas[i].Valor += valorRatearParcelas;
                            fatura.Parcelas[i].ValorTotalMoeda += valorMoedaRatearParcelas;

                            repFaturaParcela.Atualizar(fatura.Parcelas[i]);
                        }
                    }

                    valorParcelas = fatura.Parcelas?.Sum(o => o.Valor) ?? 0m;
                    valorDiferenca = Math.Round(valorTotal - valorParcelas, 2, MidpointRounding.ToEven);

                    valorMoedaParcelas = fatura.Parcelas?.Sum(o => o.ValorTotalMoeda) ?? 0m;
                    valorMoedaDiferenca = Math.Round(valorMoedaTotal - valorMoedaParcelas, 2, MidpointRounding.ToEven);

                    if ((valorDiferenca != 0m || valorMoedaDiferenca != 0m) && qtdParcelas > sequencia)
                    {
                        fatura.Parcelas[sequencia + 1].Valor += valorDiferenca;
                        fatura.Parcelas[sequencia + 1].ValorTotalMoeda += valorMoedaDiferenca;

                        repFaturaParcela.Atualizar(fatura.Parcelas[sequencia + 1]);
                    }
                }

                unitOfWork.CommitChanges();

                return new JsonpResult(true, "Sucesso");
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

        public async Task<IActionResult> FecharFatura()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Faturas/Fatura");
                if (!(this.Usuario.UsuarioAdministrador || permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Fatura_PermiteFecharFatura)))
                    return new JsonpResult(false, "Seu usuário não possui permissão para fechar uma fatura.");

                Servicos.Embarcador.Carga.CargaDadosSumarizados serCargaDadosSumarizados = new Servicos.Embarcador.Carga.CargaDadosSumarizados(unitOfWork);
                Servicos.Embarcador.Fatura.Fatura servFatura = new Servicos.Embarcador.Fatura.Fatura(unitOfWork);
                Servicos.Embarcador.Financeiro.Titulo servicoTitulo = new Servicos.Embarcador.Financeiro.Titulo(unitOfWork);

                Repositorio.Embarcador.Fatura.Fatura repFatura = new Repositorio.Embarcador.Fatura.Fatura(unitOfWork);
                Repositorio.Embarcador.Fatura.FaturaIntegracao repFaturaIntegracao = new Repositorio.Embarcador.Fatura.FaturaIntegracao(unitOfWork);
                Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Repositorio.Banco repBanco = new Repositorio.Banco(unitOfWork);
                Repositorio.Embarcador.Fatura.FaturaDocumento repFaturaDocumento = new Repositorio.Embarcador.Fatura.FaturaDocumento(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraFatura repConfiguracaoFinanceiraFatura = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraFatura(unitOfWork);

                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);
                double tomadorFatura;
                int empresaFatura, banco = 0;
                double.TryParse(Request.Params("TomadorFatura"), out tomadorFatura);
                int.TryParse(Request.Params("EmpresaFatura"), out empresaFatura);
                int.TryParse(Request.Params("Banco"), out banco);

                string agencia = Request.Params("Agencia");
                string digito = Request.Params("Digito");
                string numeroConta = Request.Params("NumeroConta");

                string observacaoFatura = Request.Params("ObservacaoFatura");
                bool imprimirObservacaoFatura = false;
                bool.TryParse(Request.Params("ImprimirObservacaoFatura"), out imprimirObservacaoFatura);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaBanco tipoConta;
                Enum.TryParse(Request.Params("TipoConta"), out tipoConta);

                Dominio.ObjetosDeValor.Embarcador.Fatura.FaturaInserir objValorFecharFatura = new Dominio.ObjetosDeValor.Embarcador.Fatura.FaturaInserir();
                objValorFecharFatura.Codigo = codigo;

                if (tomadorFatura > 0D)
                    objValorFecharFatura.ClienteTomadorFatura = new Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa {CPFCNPJ = tomadorFatura.ToString() };  //repCliente.BuscarPorCPFCNPJ(__tomadorFatura);
                else
                    objValorFecharFatura.ClienteTomadorFatura = null;

                if (empresaFatura > 0)
                    objValorFecharFatura.Empresa = new Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa { Codigo = empresaFatura};// repEmpresa.BuscarPorCodigo(empresaFatura);
                else
                    objValorFecharFatura.Empresa = null;

                if (banco > 0)
                    objValorFecharFatura.CodigoBanco = banco;// repBanco.BuscarPorCodigo(banco);
                else
                    objValorFecharFatura.CodigoBanco = 0;

                objValorFecharFatura.Agencia = agencia;
                objValorFecharFatura.DigitoAgencia = digito;
                objValorFecharFatura.NumeroConta = numeroConta;
                objValorFecharFatura.TipoContaBanco = tipoConta;
                objValorFecharFatura.ObservacaoFatura = observacaoFatura;
                objValorFecharFatura.ImprimeObservacaoFatura = imprimirObservacaoFatura;
                objValorFecharFatura.ImprimirObservacaoFatura = imprimirObservacaoFatura;

                unitOfWork.Start();
                string msgErro = "";
                if (!servFatura.FecharFatura(objValorFecharFatura, null, out msgErro, ConfiguracaoEmbarcador, TipoServicoMultisoftware, Auditado, this.Usuario))
                {
                    return new JsonpResult(false, true, msgErro);
                }
                unitOfWork.CommitChanges();
                var dynRetorno = servFatura.RetornaObjetoCompletoFatura(objValorFecharFatura.Codigo, unitOfWork);
                return new JsonpResult(dynRetorno);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao fechar fatura.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ObsoletaFecharFatura()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Faturas/Fatura");
                if (!(this.Usuario.UsuarioAdministrador || permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Fatura_PermiteFecharFatura)))
                    return new JsonpResult(false, "Seu usuário não possui permissão para fechar uma fatura.");

                Servicos.Embarcador.Carga.CargaDadosSumarizados serCargaDadosSumarizados = new Servicos.Embarcador.Carga.CargaDadosSumarizados(unitOfWork);
                Servicos.Embarcador.Fatura.Fatura servFatura = new Servicos.Embarcador.Fatura.Fatura(unitOfWork);
                Servicos.Embarcador.Financeiro.Titulo servicoTitulo = new Servicos.Embarcador.Financeiro.Titulo(unitOfWork);

                Repositorio.Embarcador.Fatura.Fatura repFatura = new Repositorio.Embarcador.Fatura.Fatura(unitOfWork);
                Repositorio.Embarcador.Fatura.FaturaIntegracao repFaturaIntegracao = new Repositorio.Embarcador.Fatura.FaturaIntegracao(unitOfWork);
                Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Repositorio.Banco repBanco = new Repositorio.Banco(unitOfWork);
                Repositorio.Embarcador.Fatura.FaturaDocumento repFaturaDocumento = new Repositorio.Embarcador.Fatura.FaturaDocumento(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraFatura repConfiguracaoFinanceiraFatura = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraFatura(unitOfWork);

                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);
                double tomadorFatura;
                int empresaFatura, banco = 0;
                double.TryParse(Request.Params("TomadorFatura"), out tomadorFatura);
                int.TryParse(Request.Params("EmpresaFatura"), out empresaFatura);
                int.TryParse(Request.Params("Banco"), out banco);

                string agencia = Request.Params("Agencia");
                string digito = Request.Params("Digito");
                string numeroConta = Request.Params("NumeroConta");

                string observacaoFatura = Request.Params("ObservacaoFatura");
                bool imprimirObservacaoFatura = false;
                bool.TryParse(Request.Params("ImprimirObservacaoFatura"), out imprimirObservacaoFatura);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaBanco tipoConta;
                Enum.TryParse(Request.Params("TipoConta"), out tipoConta);

                Dominio.Entidades.Embarcador.Fatura.Fatura fatura = repFatura.BuscarPorCodigo(codigo, true);

                if (fatura.NovoModelo)
                {
                    if (!ConfiguracaoEmbarcador.NaoValidarDataCancelamentoTituloNoFechamentoDaFatura)
                    {
                        DateTime maiorDataEmissaoDocumentos = repFaturaDocumento.ObterMaiorDataEmissaoPorFatura(fatura.Codigo);

                        if (fatura.DataFatura.Date < maiorDataEmissaoDocumentos.Date)
                            return new JsonpResult(false, true, "A data da fatura não pode ser menor que a maior data de emissão dos documentos (" + maiorDataEmissaoDocumentos.ToString("dd/MM/yyyy") + ").");

                        if (fatura.Parcelas.Any(o => o.DataEmissao.Date < maiorDataEmissaoDocumentos.Date))
                            return new JsonpResult(false, true, "A data de emissão das parcelas não pode ser menor que a maior data de emissão dos documentos (" + maiorDataEmissaoDocumentos.ToString("dd/MM/yyyy") + ").");
                    }
                }

                // So faz movimento financeiro quando o sistema for multi tms
                if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                {
                    Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraFatura configuracaoFinanceiraFatura = repConfiguracaoFinanceiraFatura.BuscarPrimeiroRegistro();

                    if (configuracaoFinanceiraFatura == null || !configuracaoFinanceiraFatura.GerarMovimentoAutomatico || configuracaoFinanceiraFatura.TipoMovimentoReversao == null || configuracaoFinanceiraFatura.TipoMovimentoUso == null)
                        return new JsonpResult(false, true, "Não existe configuração para a geração de movimentos da fatura.");

                    Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento documentoFaturamento = repFaturaDocumento.BuscarPrimeiroDocumentoFaturamento(codigo);

                    fatura.TipoMovimentoUso = servicoTitulo.ObterTipoMovimentoConfiguracaoFinanceiraFatura(documentoFaturamento.CTe, documentoFaturamento.Carga, configuracaoFinanceiraFatura);
                    fatura.TipoMovimentoReversao = servicoTitulo.ObterTipoMovimentoConfiguracaoFinanceiraFatura(documentoFaturamento.CTe, documentoFaturamento.Carga, configuracaoFinanceiraFatura, true);
                }

                if (fatura == null)
                    return new JsonpResult(false, true, "Fatura não encontrada.");

                if (fatura.Situacao != SituacaoFatura.EmAntamento)
                    return new JsonpResult(false, true, "A situação atual da fatura não permite o fechamento da mesma.");

                if (fatura.Parcelas == null || fatura.Parcelas.Count <= 0)
                    return new JsonpResult(false, true, "É necessário gerar as parcelas da fatura antes de fechar a mesma.");

                if (fatura.FaturaRecebidaDeIntegracao)
                    return new JsonpResult(false, true, "Não é possível realizar a operação para uma fatura recebida pela integração.");

                if (ConfiguracaoEmbarcador.BloquearBaixaParcialOuParcelamentoFatura)
                {
                    if (fatura.Parcelas.Count > 1)
                        return new JsonpResult(false, true, "Não é permitido o parcelamento de faturas (mais que uma parcela).");

                    if (repFaturaDocumento.PossuiFaturamentoParcialDocumento(fatura.Codigo))
                        return new JsonpResult(false, true, "Não é permitido o faturamento parcial de documentos.");
                }

                decimal valorTotal = Math.Round(fatura.Total - fatura.Descontos + fatura.Acrescimos, 2, MidpointRounding.ToEven);
                decimal valorParcelas = fatura.Parcelas?.Sum(o => o.Valor) ?? 0m;
                decimal valorDiferenca = Math.Round(valorTotal - valorParcelas, 2, MidpointRounding.ToEven);

                if (valorDiferenca != 0m)
                    return new JsonpResult(false, true, $"O valor total das parcelas não está de acordo com o valor total da fatura. Valor da fatura R$ {valorTotal:n2} / Valor das parcelas R$ {valorParcelas:n2}.");

                if (fatura.MoedaCotacaoBancoCentral.HasValue && fatura.MoedaCotacaoBancoCentral.Value != MoedaCotacaoBancoCentral.Real)
                {
                    decimal valorMoedaTotal = fatura.TotalMoedaEstrangeira - fatura.DescontosMoeda + fatura.AcrescimosMoeada;
                    decimal valorMoedaParcelas = fatura.Parcelas?.Sum(o => o.ValorTotalMoeda) ?? 0m;
                    decimal valorDiferencaMoeda = valorMoedaTotal - valorMoedaParcelas;

                    if (valorDiferencaMoeda > 0m)
                        return new JsonpResult(false, true, $"O valor total em moeda estrangeira das parcelas não está de acordo com o valor total em moeda estrangeira da fatura. Valor em moeda da fatura {valorMoedaTotal:n2} / Valor em moeda das parcelas {valorMoedaParcelas:n2}.");
                }

                if (repTitulo.ContemTitulosPagosFatura(codigo))
                    return new JsonpResult(false, true, "Esta fatura já possui títulos quitados, não sendo possível fechar a mesma.");

                string retornoConhotos = servFatura.ValidarCanhotosCTes(fatura, TipoServicoMultisoftware, unitOfWork);
                if (string.IsNullOrWhiteSpace(retornoConhotos))
                    retornoConhotos = servFatura.ValidarOcorrenciaFinalizadoraCTes(fatura, TipoServicoMultisoftware, unitOfWork);

                if (!string.IsNullOrWhiteSpace(retornoConhotos))
                    return new JsonpResult(false, true, retornoConhotos);

                string erro = string.Empty;

                unitOfWork.Start();

                if (fatura.Usuario == null)
                    fatura.Usuario = this.Usuario;

                if (fatura.NovoModelo)
                    fatura.Situacao = SituacaoFatura.EmFechamento;
                else
                    fatura.Situacao = SituacaoFatura.Fechado;

                fatura.DataFechamento = DateTime.Now;
                fatura.Etapa = EtapaFatura.Fechamento;

                if (tomadorFatura > 0D)
                    fatura.ClienteTomadorFatura = repCliente.BuscarPorCPFCNPJ(tomadorFatura);
                else
                    fatura.ClienteTomadorFatura = null;

                if (empresaFatura > 0)
                    fatura.Empresa = repEmpresa.BuscarPorCodigo(empresaFatura);
                else
                    fatura.Empresa = null;

                if (banco > 0)
                    fatura.Banco = repBanco.BuscarPorCodigo(banco);
                else
                    fatura.Banco = null;

                fatura.Agencia = agencia;
                fatura.DigitoAgencia = digito;
                fatura.NumeroConta = numeroConta;
                fatura.TipoContaBanco = tipoConta;
                fatura.ObservacaoFatura = observacaoFatura;
                fatura.ImprimeObservacaoFatura = imprimirObservacaoFatura;

                if (fatura.Numero == 0)
                {
                    if (ConfiguracaoEmbarcador.GerarNumeracaoFaturaAnual)
                    {
                        int anoAtual = DateTime.Now.Year;
                        fatura.Numero = repFatura.UltimoNumeracao(anoAtual) + 1;
                        anoAtual = (anoAtual % 100);
                        if (fatura.Numero == 0 || (fatura.Numero < ((anoAtual * 1000000) + 1)))
                            fatura.Numero = (anoAtual * 1000000) + 1;
                    }
                    else
                        fatura.Numero = repFatura.UltimoNumeracao() + 1;

                    fatura.ControleNumeracao = null;
                }

                repFatura.Atualizar(fatura, Auditado);

                servFatura.EtapaAprovacao(fatura, TipoServicoMultisoftware,unitOfWork);
                if (!fatura.Situacao.Equals(SituacaoFatura.AguardandoAprovacao))
                {
                    servFatura.GerarIntegracoesFatura(fatura, unitOfWork, TipoServicoMultisoftware, Auditado, ConfiguracaoEmbarcador);
                    servFatura.SalvarTituloVencimentoDocumentoFaturamento(fatura, unitOfWork);
                    serCargaDadosSumarizados.AtualizarDadosCTesFaturados(fatura.Codigo, unitOfWork);
                }

                servFatura.InserirLog(fatura, unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLogFatura.FechouFatura, this.Usuario);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, fatura, null, "Fechou a fatura", unitOfWork);

                unitOfWork.CommitChanges();

                var dynRetorno = servFatura.RetornaObjetoCompletoFatura(fatura.Codigo, unitOfWork);

                return new JsonpResult(dynRetorno);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao fechar fatura.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> LiquidarFatura()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Faturas/Fatura");
                if (!(this.Usuario.UsuarioAdministrador || permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Fatura_PermiteLiquidarFatura)))
                    return new JsonpResult(false, true, "Seu usuário não possui permissão para liquidar uma fatura.");

                int codigo = Request.GetIntParam("Codigo");
                int codigoFormaPagamento = Request.GetIntParam("FormaPagamento");

                DateTime dataBaixa = Request.GetDateTimeParam("DataBaixa");
                DateTime dataBase = Request.GetDateTimeParam("DataBase");
                DateTime? dataBaseCRT = Request.GetNullableDateTimeParam("DataBaseCRT");

                Servicos.Embarcador.Fatura.Fatura servFatura = new Servicos.Embarcador.Fatura.Fatura(unitOfWork);
                Servicos.Embarcador.Financeiro.ProcessoMovimento servProcessoMovimento = new Servicos.Embarcador.Financeiro.ProcessoMovimento();

                Repositorio.Embarcador.Fatura.Fatura repFatura = new Repositorio.Embarcador.Fatura.Fatura(unitOfWork);
                Repositorio.Embarcador.Fatura.FaturaIntegracao repFaturaIntegracao = new Repositorio.Embarcador.Fatura.FaturaIntegracao(unitOfWork);
                Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);
                Repositorio.Embarcador.Financeiro.TituloBaixa repTituloBaixa = new Repositorio.Embarcador.Financeiro.TituloBaixa(unitOfWork);
                Repositorio.Embarcador.Financeiro.TituloBaixaAgrupado repTituloBaixaAgrupado = new Repositorio.Embarcador.Financeiro.TituloBaixaAgrupado(unitOfWork);
                Repositorio.Embarcador.Financeiro.TituloBaixaAgrupadoDocumento repTituloBaixaAgrupadoDocumento = new Repositorio.Embarcador.Financeiro.TituloBaixaAgrupadoDocumento(unitOfWork);
                Repositorio.Embarcador.Financeiro.TipoPagamentoRecebimento repTipoPagamentoRecebimento = new Repositorio.Embarcador.Financeiro.TipoPagamentoRecebimento(unitOfWork);
                Repositorio.Embarcador.Financeiro.DocumentoFaturamento repDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(unitOfWork);
                Repositorio.Embarcador.Financeiro.TituloDocumento repTituloDocumento = new Repositorio.Embarcador.Financeiro.TituloDocumento(unitOfWork);
                Repositorio.Embarcador.Moedas.Cotacao repCotacao = new Repositorio.Embarcador.Moedas.Cotacao(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraBaixaTituloReceber repConfiguracaoFinanceiraBaixaTituloReceber = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraBaixaTituloReceber(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraBaixaTituloReceber configuracaoFinanceiraBaixaTituloReceber = null;
                Dominio.Entidades.Embarcador.Financeiro.TipoPagamentoRecebimento formaPagamento = repTipoPagamentoRecebimento.BuscarPorCodigo(codigoFormaPagamento);
                Dominio.Entidades.Embarcador.Fatura.Fatura fatura = repFatura.BuscarPorCodigo(codigo, true);

                if (formaPagamento == null)
                    return new JsonpResult(false, true, "Forma de pagamento não encontrada.");

                if (dataBaixa == DateTime.MinValue)
                    return new JsonpResult(false, true, "Data da baixa não selecionada.");

                if (dataBase == DateTime.MinValue)
                    return new JsonpResult(false, true, "Data base não selecionada.");

                if (fatura == null)
                    return new JsonpResult(false, true, "Fatura não encontrada.");

                if (fatura.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFatura.Fechado)
                    return new JsonpResult(false, true, "A situação da fatura não permite que ela seja liquidada.");

                if (fatura.Parcelas == null || fatura.Parcelas.Count() <= 0)
                    return new JsonpResult(false, true, "É necessário gerar as parcelas da fatura antes de liquidar a mesma.");

                if (repTitulo.ContemTitulosPagosFatura(codigo))
                    return new JsonpResult(false, true, "Esta fatura já possui títulos quitados, impossível de liquidar a mesma.");

                if (fatura.FaturaRecebidaDeIntegracao)
                    return new JsonpResult(false, true, "Não é possível realizar a operação para uma fatura recebida pela integração.");

                List<Dominio.Entidades.Embarcador.Financeiro.Titulo> titulos = repTitulo.BuscarPorFatura(codigo);

                if (titulos == null || titulos.Count() <= 0)
                    return new JsonpResult(false, true, "É necessário gerar as parcelas da fatura antes de liquidar a mesma.");

                List<Dominio.ObjetosDeValor.Embarcador.Financeiro.BaixaTituloReceberAcrescimoDesconto> acrescimosDescontos = ObterAcrescimosDescontosLiquidacao(unitOfWork, permissoesPersonalizadas);

                unitOfWork.Start();

                fatura.FormaPagamento = formaPagamento;
                fatura.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFatura.Liquidado;

                repFatura.Atualizar(fatura, Auditado);

                string erro = string.Empty;

                Dominio.Entidades.Embarcador.Financeiro.TituloBaixa tituloBaixa = new Dominio.Entidades.Embarcador.Financeiro.TituloBaixa()
                {
                    DataBaixa = dataBaixa,
                    DataBase = dataBase,
                    DataBaseCRT = dataBaseCRT,
                    DataOperacao = DateTime.Now,
                    Numero = 1,
                    Observacao = (fatura.GrupoPessoas != null ? fatura.GrupoPessoas.Descricao : fatura.Cliente != null ? fatura.Cliente.Nome : "") + " - FATURA Nº " + fatura.Numero.ToString() + " (" + dataBaixa.ToString("dd/MM/yyyy") + ")",
                    SituacaoBaixaTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoBaixaTitulo.Finalizada,
                    Sequencia = 1,
                    Valor = titulos.Sum(o => o.ValorOriginal),
                    ValorTotalAPagar = titulos.Sum(o => o.ValorTotal),
                    TipoBaixaTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTitulo.Receber,
                    TipoPagamentoRecebimento = formaPagamento,
                    Usuario = Usuario,
                    Pessoa = fatura.ClienteTomadorFatura,
                    GrupoPessoas = fatura.GrupoPessoas
                };

                repTituloBaixa.Inserir(tituloBaixa, Auditado);

                for (int i = 0; i < titulos.Count; i++)
                {
                    Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = titulos[i];

                    if (titulo.DataEmissao.Value.Date > tituloBaixa.DataBaixa.Value.Date)
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, "O título " + titulo.Codigo.ToString() + " possui a data de emissão maior que a data da baixa.");
                    }

                    titulo.Initialize();

                    Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado tituloBaixaAgrupado = Servicos.Embarcador.Financeiro.BaixaTituloReceber.AdicionarTituloABaixa(tituloBaixa, titulo.Codigo, unitOfWork, Usuario, 0m, 0m, false);

                    foreach (Dominio.Entidades.Embarcador.Financeiro.TituloDocumento tituloDocumento in titulo.Documentos)
                    {
                        Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento documentoFaturamento = null;

                        if (tituloDocumento != null)
                        {
                            if (tituloDocumento.TipoDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoTitulo.CTe)
                                documentoFaturamento = repDocumentoFaturamento.BuscarPorCTe(tituloDocumento.CTe.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLiquidacao.Fatura);
                            else
                                documentoFaturamento = repDocumentoFaturamento.BuscarPorCarga(tituloDocumento.Carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLiquidacao.Fatura);
                        }

                        if (documentoFaturamento == null)
                            continue;

                        Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupadoDocumento tituloBaixaAgrupadoDocumento = repTituloBaixaAgrupadoDocumento.BuscarPorTituloBaixaAgrupadoETituloDocumento(tituloBaixaAgrupado.Codigo, tituloDocumento.Codigo);

                        if (!fatura.NaoUtilizarMoedaEstrangeira && documentoFaturamento.Moeda.HasValue && documentoFaturamento.Moeda != Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral.Real)
                        {
                            Dominio.Entidades.Embarcador.Moedas.Cotacao cotacao = repCotacao.BuscarCotacao(documentoFaturamento.Moeda.Value, tituloBaixa.DataBaseCRT.Value);

                            if (cotacao == null || cotacao.ValorMoeda <= 0m)
                            {
                                unitOfWork.Rollback();
                                return new JsonpResult(false, true, $"Cotação não encontrada para {documentoFaturamento.Moeda.Value.ObterDescricao()} no dia {tituloBaixa.DataBaseCRT.Value:dd/MM/yyyy HH:mm}.");
                            }

                            decimal valorPago = Math.Round(cotacao.ValorMoeda * (documentoFaturamento.ValorTotalMoeda ?? 0m), 2, MidpointRounding.AwayFromZero);

                            decimal diferenca = valorPago - documentoFaturamento.ValorDocumento;

                            if (diferenca != 0m)
                            {
                                if (configuracaoFinanceiraBaixaTituloReceber == null)
                                {
                                    configuracaoFinanceiraBaixaTituloReceber = repConfiguracaoFinanceiraBaixaTituloReceber.BuscarPrimeiroRegistro();

                                    if (configuracaoFinanceiraBaixaTituloReceber == null)
                                    {
                                        unitOfWork.Rollback();
                                        return new JsonpResult(false, true, "Configuração financeira para baixa de título a receber não realizada.");
                                    }
                                }

                                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraBaixaTituloReceberMoeda configuracaoFinanceiraBaixaTituloReceberMoeda = configuracaoFinanceiraBaixaTituloReceber.ConfiguracoesMoedas.Where(o => o.Moeda == documentoFaturamento.Moeda.Value).FirstOrDefault();

                                if (configuracaoFinanceiraBaixaTituloReceberMoeda == null)
                                {
                                    unitOfWork.Rollback();
                                    return new JsonpResult(false, true, $"Configuração financeira para baixa de título a receber com {documentoFaturamento.Moeda.Value.ObterDescricao()} não realizada.");
                                }


                                Dominio.Entidades.Embarcador.Fatura.Justificativa justificativa = null;

                                if (diferenca > 0m)
                                    justificativa = configuracaoFinanceiraBaixaTituloReceberMoeda.JustificativaAcrescimo;
                                else if (diferenca < 0m)
                                {
                                    justificativa = configuracaoFinanceiraBaixaTituloReceberMoeda.JustificativaDesconto;
                                    diferenca = -diferenca;
                                }

                                if (!Servicos.Embarcador.Financeiro.BaixaTituloReceber.AdicionarValorAoDocumento(out erro, tituloBaixaAgrupadoDocumento, justificativa, diferenca, $"Referente à diferença da cotação do {documentoFaturamento.Moeda.Value.ObterDescricao()} na liquidação do título.", unitOfWork, Usuario))
                                {
                                    unitOfWork.Rollback();
                                    return new JsonpResult(false, true, erro);
                                }
                            }
                        }

                        foreach (Dominio.ObjetosDeValor.Embarcador.Financeiro.BaixaTituloReceberAcrescimoDesconto acrescimoDesconto in acrescimosDescontos)
                        {
                            decimal valorTotalPendente = acrescimoDesconto.Valor - acrescimoDesconto.ValorTotalRateado;

                            if (valorTotalPendente <= 0m)
                                continue;

                            decimal valorUtilizar = valorTotalPendente;

                            if (acrescimoDesconto.Justificativa.TipoJustificativa == TipoJustificativa.Desconto && valorTotalPendente > tituloBaixaAgrupadoDocumento.ValorTotalAPagar)
                                valorUtilizar = tituloBaixaAgrupadoDocumento.ValorTotalAPagar;

                            acrescimoDesconto.ValorTotalRateado += valorUtilizar;

                            if (!Servicos.Embarcador.Financeiro.BaixaTituloReceber.AdicionarValorAoDocumento(out erro, tituloBaixaAgrupadoDocumento, acrescimoDesconto.Justificativa, valorUtilizar, acrescimoDesconto.Observacao, unitOfWork, Usuario))
                            {
                                unitOfWork.Rollback();
                                return new JsonpResult(false, true, erro);
                            }
                        }
                    }

                    Servicos.Embarcador.Financeiro.BaixaTituloReceber.AtualizarTotaisTituloBaixaAgrupado(ref tituloBaixaAgrupado, unitOfWork);

                    int countDocumentosBaixados = 0;

                    if (!Servicos.Embarcador.Financeiro.BaixaTituloReceber.BaixarTitulo(out erro, tituloBaixa, tituloBaixaAgrupado, tituloBaixa.Observacao, null, unitOfWork, TipoServicoMultisoftware, null, false, 0, ref countDocumentosBaixados))
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, true, erro);
                    }
                }

                Servicos.Embarcador.Financeiro.BaixaTituloReceber.AtualizarTotaisTituloBaixa(ref tituloBaixa, unitOfWork, true);

                if (acrescimosDescontos.Any(o => o.Valor != o.ValorTotalRateado))
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, "O valor dos acréscimos/descontos concedidos são incondizentes com o valor dos títulos, não sendo possível aplicar os mesmos.");
                }

                servFatura.InserirLog(fatura, unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLogFatura.LiquidouFatura, this.Usuario);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, fatura, null, "Liquidou a fatura.", unitOfWork);

                unitOfWork.CommitChanges();

                var dynRetorno = servFatura.RetornaObjetoCompletoFatura(fatura.Codigo, unitOfWork);

                return new JsonpResult(dynRetorno);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao liquidar a fatura.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ReAbrirFatura()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Faturas/Fatura");
                if (!(this.Usuario.UsuarioAdministrador || permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Fatura_PermiteReAbrirFatura)))
                    return new JsonpResult(false, true, "Seu usuário não possui permissão para reabrir a fatura.");

                Servicos.Embarcador.Fatura.Fatura servicoFatura = new Servicos.Embarcador.Fatura.Fatura(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");

                string motivo = Request.GetStringParam("Motivo");

                DateTime? dataCancelamento = Request.GetNullableDateTimeParam("DataCancelamento");

                bool duplicarFatura = Request.GetBoolParam("DuplicarFatura");

                unitOfWork.Start();

                servicoFatura.IniciarCancelamentoFatura(codigo, motivo, Usuario, ConfiguracaoEmbarcador, Auditado, duplicarFatura, dataCancelamento);

                unitOfWork.CommitChanges();

                dynamic dynRetorno = servicoFatura.RetornaObjetoCompletoFatura(codigo, unitOfWork);

                return new JsonpResult(dynRetorno);
            }
            catch (ServicoException ex)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao cancelar a fatura.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ValidarCancelamentoFatura()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                DateTime? dataCancelamento = Request.GetNullableDateTimeParam("DataCancelamento");

                Servicos.Embarcador.Fatura.Fatura servicoFatura = new Servicos.Embarcador.Fatura.Fatura(unitOfWork);

                servicoFatura.ValidarCancelamentoFatura(codigo, dataCancelamento);

                return new JsonpResult(new
                {
                    Valido = true,
                    PermiteCancelarFatura = true,
                    Mensagem = string.Empty
                });
            }
            catch (ServicoException ex)
            {
                unitOfWork.Rollback();
                return new JsonpResult(new
                {
                    Valido = false,
                    PermiteCancelarFatura = true,
                    Mensagem = ex.Message
                });
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao validar o cancelamento da fatura.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAnonymous]
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
                grid.AdicionarCabecalho("CodigoFatura", false);
                grid.AdicionarCabecalho("Justificativa", "Descricao", 30, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Tipo", "DescricaoTipo", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Valor", "Valor", 10, Models.Grid.Align.right, false);

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;

                Repositorio.Embarcador.Fatura.FaturaAcrescimoDesconto repFaturaAcrescimoDesconto = new Repositorio.Embarcador.Fatura.FaturaAcrescimoDesconto(unitOfWork);
                List<Dominio.Entidades.Embarcador.Fatura.FaturaAcrescimoDesconto> listaFaturaAcrescimoDesconto = repFaturaAcrescimoDesconto.ConsultarAcrescimoDesconto(codigoBaixa, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repFaturaAcrescimoDesconto.ContarAcrescimoDesconto(codigoBaixa));

                var lista = (from p in listaFaturaAcrescimoDesconto
                             select new
                             {
                                 p.Codigo,
                                 CodigoFatura = p.Fatura.Codigo,
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
                int codigo = int.Parse(Request.Params("Codigo"));
                decimal valor = 0;
                decimal.TryParse(Request.Params("Valor"), out valor);
                int codigoJustificativa = 0;
                int.TryParse(Request.Params("Justificativa"), out codigoJustificativa);

                string observacaoFatura = Request.Params("ObservacaoFatura");

                bool imprimirObservacaoFatura = false;
                bool.TryParse(Request.Params("ImprimirObservacaoFatura"), out imprimirObservacaoFatura);

                Repositorio.Banco repBanco = new Repositorio.Banco(unitOfWork);
                int banco = 0;
                int.TryParse(Request.Params("Banco"), out banco);

                string agencia = Request.Params("Agencia");
                string digito = Request.Params("Digito");
                string numeroConta = Request.Params("NumeroConta");

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaBanco tipoConta;
                Enum.TryParse(Request.Params("TipoConta"), out tipoConta);

                Repositorio.Embarcador.Fatura.FaturaAcrescimoDesconto repFaturaAcrescimoDesconto = new Repositorio.Embarcador.Fatura.FaturaAcrescimoDesconto(unitOfWork);
                Repositorio.Embarcador.Fatura.Fatura repFatura = new Repositorio.Embarcador.Fatura.Fatura(unitOfWork);
                Repositorio.Embarcador.Fatura.FaturaParcela repFaturaParcela = new Repositorio.Embarcador.Fatura.FaturaParcela(unitOfWork);
                Repositorio.Embarcador.Fatura.Justificativa repJustificativa = new Repositorio.Embarcador.Fatura.Justificativa(unitOfWork);

                Dominio.Entidades.Embarcador.Fatura.Fatura fatura = repFatura.BuscarPorCodigo(codigo, true);

                unitOfWork.Start();

                Dominio.Entidades.Embarcador.Fatura.FaturaAcrescimoDesconto faturaAcrescimoDesconto = new Dominio.Entidades.Embarcador.Fatura.FaturaAcrescimoDesconto();
                faturaAcrescimoDesconto.Fatura = fatura;
                faturaAcrescimoDesconto.Justificativa = repJustificativa.BuscarPorCodigo(codigoJustificativa);
                faturaAcrescimoDesconto.Valor = valor;

                repFaturaAcrescimoDesconto.Inserir(faturaAcrescimoDesconto);

                fatura.ObservacaoFatura = observacaoFatura;
                fatura.ImprimeObservacaoFatura = imprimirObservacaoFatura;

                //fatura.MoedaCotacaoBancoCentral = Request.GetNullableEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral>("MoedaCotacaoBancoCentral");
                //fatura.DataBaseCRT = Request.GetNullableDateTimeParam("DataBaseCRT");
                //fatura.ValorMoedaCotacao = Request.GetDecimalParam("ValorMoedaCotacao");
                //fatura.TotalMoedaEstrangeira = Request.GetDecimalParam("TotalMoedaEstrangeira");

                if (banco > 0)
                    fatura.Banco = repBanco.BuscarPorCodigo(banco);
                else
                    fatura.Banco = null;

                fatura.Agencia = agencia;
                fatura.DigitoAgencia = digito;
                fatura.NumeroConta = numeroConta;
                fatura.TipoContaBanco = tipoConta;

                repFatura.Atualizar(fatura, Auditado);

                List<Dominio.Entidades.Embarcador.Fatura.FaturaParcela> listaParcelas = repFaturaParcela.BuscarPorFatura(codigo);
                for (int i = 0; i < listaParcelas.Count; i++)
                    repFaturaParcela.Deletar(listaParcelas[i]);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, fatura, null, "Adicionou acrescimo a fatura", unitOfWork);
                unitOfWork.CommitChanges();

                return new JsonpResult(true, "Sucesso");
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
                int codigo = int.Parse(Request.Params("Codigo"));
                int codigoFatura = int.Parse(Request.Params("CodigoFatura"));
                string observacaoFatura = Request.Params("ObservacaoFatura");
                bool imprimirObservacaoFatura = false;
                bool.TryParse(Request.Params("ImprimirObservacaoFatura"), out imprimirObservacaoFatura);

                Repositorio.Banco repBanco = new Repositorio.Banco(unitOfWork);
                int banco = 0;
                int.TryParse(Request.Params("Banco"), out banco);

                string agencia = Request.Params("Agencia");
                string digito = Request.Params("Digito");
                string numeroConta = Request.Params("NumeroConta");

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaBanco tipoConta;
                Enum.TryParse(Request.Params("TipoConta"), out tipoConta);

                Repositorio.Embarcador.Fatura.FaturaAcrescimoDesconto repFaturaAcrescimoDesconto = new Repositorio.Embarcador.Fatura.FaturaAcrescimoDesconto(unitOfWork);
                Repositorio.Embarcador.Fatura.Fatura repFatura = new Repositorio.Embarcador.Fatura.Fatura(unitOfWork);
                Repositorio.Embarcador.Fatura.FaturaParcela repFaturaParcela = new Repositorio.Embarcador.Fatura.FaturaParcela(unitOfWork);
                Repositorio.Embarcador.Fatura.Justificativa repJustificativa = new Repositorio.Embarcador.Fatura.Justificativa(unitOfWork);

                unitOfWork.Start();

                Dominio.Entidades.Embarcador.Fatura.FaturaAcrescimoDesconto faturaAcrescimoDesconto = repFaturaAcrescimoDesconto.BuscarPorCodigo(codigo);

                repFaturaAcrescimoDesconto.Deletar(faturaAcrescimoDesconto);

                List<Dominio.Entidades.Embarcador.Fatura.FaturaParcela> listaParcelas = repFaturaParcela.BuscarPorFatura(codigoFatura);
                for (int i = 0; i < listaParcelas.Count; i++)
                    repFaturaParcela.Deletar(listaParcelas[i]);

                Dominio.Entidades.Embarcador.Fatura.Fatura fatura = repFatura.BuscarPorCodigo(codigoFatura, true);
                fatura.ObservacaoFatura = observacaoFatura;
                fatura.ImprimeObservacaoFatura = imprimirObservacaoFatura;

                //fatura.MoedaCotacaoBancoCentral = Request.GetNullableEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral>("MoedaCotacaoBancoCentral");
                //fatura.DataBaseCRT = Request.GetNullableDateTimeParam("DataBaseCRT");
                //fatura.ValorMoedaCotacao = Request.GetDecimalParam("ValorMoedaCotacao");
                //fatura.TotalMoedaEstrangeira = Request.GetDecimalParam("TotalMoedaEstrangeira");

                if (banco > 0)
                    fatura.Banco = repBanco.BuscarPorCodigo(banco);
                else
                    fatura.Banco = null;

                fatura.Agencia = agencia;
                fatura.DigitoAgencia = digito;
                fatura.NumeroConta = numeroConta;
                fatura.TipoContaBanco = tipoConta;

                repFatura.Atualizar(fatura, Auditado);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, fatura, null, "Removeu acrescimo da fatura", unitOfWork);

                unitOfWork.CommitChanges();
                return new JsonpResult(true, "Sucesso");
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


        public async Task<IActionResult> ValidarFechamentoFatura()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                if (!ConfiguracaoEmbarcador.NaoValidarDataCancelamentoTituloNoFechamentoDaFatura)
                {
                    int codigo = int.Parse(Request.Params("Codigo"));
                    Repositorio.Embarcador.Fatura.Fatura repFatura = new Repositorio.Embarcador.Fatura.Fatura(unitOfWork);
                    Dominio.Entidades.Embarcador.Fatura.Fatura fatura = repFatura.BuscarPorCodigo(codigo);
                    Servicos.Embarcador.Fatura.Fatura svcFatura = new Servicos.Embarcador.Fatura.Fatura(unitOfWork);

                    if (!svcFatura.ValidarFechamentoFatura(out string msgErro, fatura, ConfiguracaoEmbarcador))
                    {
                        return new JsonpResult(new
                        {
                            Valido = false,
                            PermiteFecharFatura = false,
                            Mensagem = msgErro
                        });
                    }
                }
                
                return new JsonpResult(new
                {
                    Valido = true,
                    PermiteFecharFatura = true,
                    Mensagem = string.Empty
                });
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao validar o fechamento da fatura.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [Obsolete("Esta função está obsoleta. o Função foi convertida para um serviço.")]

        public async Task<IActionResult> ObsoletaValidarFechamentoFatura()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                if (!ConfiguracaoEmbarcador.NaoValidarDataCancelamentoTituloNoFechamentoDaFatura)
                {
                    int codigo = int.Parse(Request.Params("Codigo"));

                    Repositorio.Embarcador.Fatura.Fatura repFatura = new Repositorio.Embarcador.Fatura.Fatura(unitOfWork);

                    Dominio.Entidades.Embarcador.Fatura.Fatura fatura = repFatura.BuscarPorCodigo(codigo);

                    IList<Dominio.ObjetosDeValor.Embarcador.Financeiro.TituloCancelamento> titulos = repFatura.ObterTitulosComDataCancelamentoSuperiorAFaturaPorCTe(codigo);

                    if (!titulos.Any())
                        titulos = repFatura.ObterTitulosComDataCancelamentoSuperiorAFaturaPorCarga(codigo);

                    if (titulos.Count > 0)
                    {
                        DateTime maiorDataCancelamento = titulos.Max(o => o.DataCancelamento.Value);
                        return new JsonpResult(new
                        {
                            Valido = false,
                            PermiteFecharFatura = false,
                            Mensagem = "Existem documentos com títulos cancelados. A data da fatura (" + fatura.DataFatura.ToString("dd/MM/yyyy") + ") não pode ser menor que a data de cancelamento dos títulos (" + maiorDataCancelamento.ToString("dd/MM/yyyy") + "). Títulos: " + string.Join(", ", titulos.Select(o => o.Codigo)) + "."
                        });
                    }
                }

                return new JsonpResult(new
                {
                    Valido = true,
                    PermiteFecharFatura = true,
                    Mensagem = string.Empty
                });
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao validar o fechamento da fatura.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ObterDetalhesContabilizacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = int.Parse(Request.Params("CodigoFatura"));

                Repositorio.Embarcador.Fatura.Fatura repFatura = new Repositorio.Embarcador.Fatura.Fatura(unitOfWork);

                Dominio.Entidades.Embarcador.Fatura.Fatura fatura = repFatura.BuscarPorCodigo(codigo);

                Repositorio.Embarcador.Fatura.FaturaIntegracao repFaturaIntegracao = new Repositorio.Embarcador.Fatura.FaturaIntegracao(unitOfWork);

                Dominio.Enumeradores.TipoLayoutEDI[] layouts = { Dominio.Enumeradores.TipoLayoutEDI.INTPFAR };

                List<dynamic> retorno = new List<dynamic>();

                bool ediTipoOperacao = false;
                if (fatura.TipoOperacao != null)
                {
                    if (fatura.TipoOperacao.LayoutsEDI != null && fatura.TipoOperacao.LayoutsEDI.Count() > 0)
                    {
                        for (int i = 0; i < fatura.TipoOperacao.LayoutsEDI.Count(); i++)
                        {
                            ediTipoOperacao = true;
                            Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoLayoutEDI layoutEDI = fatura.TipoOperacao.LayoutsEDI[i];
                            if (layouts.Contains(layoutEDI.LayoutEDI.Tipo))
                            {
                                List<Dominio.ObjetosDeValor.Embarcador.Fatura.ContabilizacaoFatura> listaContabil = Servicos.Embarcador.Fatura.Fatura.ObterDadosContabilizacao(fatura, null, fatura.Empresa, unitOfWork);
                                if (listaContabil.Count > 0)
                                {
                                    var resumo = new
                                    {
                                        layoutEDI.LayoutEDI.Descricao,
                                        ListaContabil = listaContabil.ToList()
                                    };
                                    retorno.Add(resumo);
                                }
                            }
                        }
                    }
                }

                if (!ediTipoOperacao)
                {
                    if (fatura.Cliente != null)
                    {
                        if (fatura.Cliente.LayoutsEDI != null && fatura.Cliente.LayoutsEDI.Count() > 0)
                        {
                            for (int i = 0; i < fatura.Cliente.LayoutsEDI.Count(); i++)
                            {
                                Dominio.Entidades.Embarcador.Pessoas.ClienteLayoutEDI layoutEDI = fatura.Cliente.LayoutsEDI[i];

                                if (layouts.Contains(layoutEDI.LayoutEDI.Tipo))
                                {
                                    List<Dominio.ObjetosDeValor.Embarcador.Fatura.ContabilizacaoFatura> listaContabil = Servicos.Embarcador.Fatura.Fatura.ObterDadosContabilizacao(fatura, null, fatura.Transportador, unitOfWork);
                                    if (listaContabil.Count > 0)
                                    {
                                        var resumo = new
                                        {
                                            layoutEDI.LayoutEDI.Descricao,
                                            ListaContabil = listaContabil.ToList()
                                        };
                                        retorno.Add(resumo);
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (fatura.Cliente.GrupoPessoas != null)
                            {
                                if (fatura.Cliente.GrupoPessoas.LayoutsEDI != null && fatura.Cliente.GrupoPessoas.LayoutsEDI.Count() > 0)
                                {
                                    for (int i = 0; i < fatura.Cliente.GrupoPessoas.LayoutsEDI.Count(); i++)
                                    {
                                        Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasLayoutEDI layoutEDI = fatura.Cliente.GrupoPessoas.LayoutsEDI[i];

                                        if (layouts.Contains(layoutEDI.LayoutEDI.Tipo))
                                        {
                                            List<Dominio.ObjetosDeValor.Embarcador.Fatura.ContabilizacaoFatura> listaContabil = Servicos.Embarcador.Fatura.Fatura.ObterDadosContabilizacao(fatura, null, fatura.Transportador, unitOfWork);
                                            if (listaContabil.Count > 0)
                                            {
                                                var resumo = new
                                                {
                                                    layoutEDI.LayoutEDI.Descricao,
                                                    ListaContabil = listaContabil.ToList()
                                                };
                                                retorno.Add(resumo);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (fatura.GrupoPessoas != null)
                    {
                        if (fatura.GrupoPessoas.LayoutsEDI != null && fatura.GrupoPessoas.LayoutsEDI.Count() > 0)
                        {
                            for (int i = 0; i < fatura.GrupoPessoas.LayoutsEDI.Count(); i++)
                            {
                                Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasLayoutEDI layoutEDI = fatura.GrupoPessoas.LayoutsEDI[i];
                                if (layouts.Contains(layoutEDI.LayoutEDI.Tipo))
                                {
                                    List<Dominio.ObjetosDeValor.Embarcador.Fatura.ContabilizacaoFatura> listaContabil = Servicos.Embarcador.Fatura.Fatura.ObterDadosContabilizacao(fatura, null, fatura.Transportador, unitOfWork);
                                    if (listaContabil.Count > 0)
                                    {
                                        var resumo = new
                                        {
                                            layoutEDI.LayoutEDI.Descricao,
                                            ListaContabil = listaContabil.ToList()
                                        };
                                        retorno.Add(resumo);
                                    }
                                }
                            }
                        }
                    }

                    if (fatura.Transportador != null)
                    {
                        if (fatura.Transportador.TransportadorLayoutsEDI != null && fatura.Transportador.TransportadorLayoutsEDI.Count() > 0)
                        {
                            for (int i = 0; i < fatura.Transportador.TransportadorLayoutsEDI.Count(); i++)
                            {
                                Dominio.Entidades.Embarcador.Transportadores.TransportadorLayoutEDI layoutEDI = fatura.Transportador.TransportadorLayoutsEDI[i];
                                if (layouts.Contains(layoutEDI.LayoutEDI.Tipo))
                                {
                                    List<Dominio.ObjetosDeValor.Embarcador.Fatura.ContabilizacaoFatura> listaContabil = Servicos.Embarcador.Fatura.Fatura.ObterDadosContabilizacao(fatura, layoutEDI.Empresa, fatura.Transportador, unitOfWork);
                                    if (listaContabil.Count > 0)
                                    {
                                        var resumo = new
                                        {
                                            layoutEDI.LayoutEDI.Descricao,
                                            ListaContabil = listaContabil.ToList()
                                        };
                                        retorno.Add(resumo);
                                    }
                                }
                            }
                        }
                    }
                }

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao os detalhes da contabilização.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadPreviewDOCCOB()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                if (!ConfiguracaoEmbarcador.GerarPreviewDOCCOBFatura)
                    return new JsonpResult(false, true, "Funcionalidade não disponível para este ambiente.");

                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Faturas/Fatura");
                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_DownloadArquivoIntegracoes))
                    return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

                Repositorio.Embarcador.Fatura.Fatura repFatura = new Repositorio.Embarcador.Fatura.Fatura(unitOfWork);

                int codigoFatura = Request.GetIntParam("Codigo");

                Dominio.Entidades.Embarcador.Fatura.Fatura fatura = repFatura.BuscarPorCodigo(codigoFatura);

                if (fatura == null)
                    return new JsonpResult(false, true, "Fatura não encontrada.");

                Servicos.Embarcador.Fatura.Fatura svcFatura = new Servicos.Embarcador.Fatura.Fatura(unitOfWork);
                List<Dominio.Enumeradores.TipoLayoutEDI> doccobs = new List<Dominio.Enumeradores.TipoLayoutEDI>();
                doccobs.Add(Dominio.Enumeradores.TipoLayoutEDI.DOCCOB);
                doccobs.Add(Dominio.Enumeradores.TipoLayoutEDI.DOCCOB_CT);

                Dominio.Entidades.LayoutEDI layoutEDI = svcFatura.ObterLayoutEDIFatura(fatura, doccobs);

                if (layoutEDI == null)
                    return new JsonpResult(false, true, "Layout de DOCCOB não encontrado.");

                Servicos.Embarcador.Integracao.EDI.DOCCOB svcDOCCOB = new Servicos.Embarcador.Integracao.EDI.DOCCOB();
                Servicos.GeracaoEDI serGeracaoEDI = new Servicos.GeracaoEDI(unitOfWork, layoutEDI, fatura.Empresa);

                if (layoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.DOCCOB)
                {
                    Dominio.ObjetosDeValor.EDI.DOCCOB.EDIDOCCOB doccob = svcDOCCOB.ConverterFaturaParaDOCCOB(fatura, unitOfWork);
                    return Arquivo(serGeracaoEDI.GerarArquivoRecursivo(doccob), "plain/text", $"DOCCOB_Fatura_{fatura.Numero}.txt");
                }
                else
                {
                    Dominio.ObjetosDeValor.EDI.DOCCOB.EDIDOCCOBCaterpillar doccob = svcDOCCOB.ConverterFaturaParaDOCCOBCaterpillar(fatura, unitOfWork);
                    return Arquivo(serGeracaoEDI.GerarArquivoRecursivo(doccob), "plain/text", $"DOCCOB_Fatura_{fatura.Numero}.txt");
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao gerar o DOCCOB.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private List<Dominio.ObjetosDeValor.Embarcador.Financeiro.BaixaTituloReceberAcrescimoDesconto> ObterAcrescimosDescontosLiquidacao(Repositorio.UnitOfWork unitOfWork, List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas)
        {
            bool permiteConcederAcrescimoDesconto = this.Usuario.UsuarioAdministrador || !permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Fatura_BloquearAcrescimoDesconto);
            bool permiteConcederDesconto = this.Usuario.UsuarioAdministrador || !permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Fatura_NaoPermitirLancarDesconto);
            bool permiteConcederAcrescimo = this.Usuario.UsuarioAdministrador || !permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Fatura_NaoPermitirLancarAcrescimo);

            Repositorio.Embarcador.Fatura.Justificativa repJustificativa = new Repositorio.Embarcador.Fatura.Justificativa(unitOfWork);

            dynamic acrescimosDescontos = JsonConvert.DeserializeObject(Request.Params("AcrescimosDescontos"));

            List<Dominio.ObjetosDeValor.Embarcador.Financeiro.BaixaTituloReceberAcrescimoDesconto> acrescimosDescontosAplicar = new List<Dominio.ObjetosDeValor.Embarcador.Financeiro.BaixaTituloReceberAcrescimoDesconto>();

            if (permiteConcederAcrescimoDesconto)
            {
                foreach (dynamic acrescimoDesconto in acrescimosDescontos)
                {
                    Dominio.Entidades.Embarcador.Fatura.Justificativa justificativa = repJustificativa.BuscarPorCodigo((int)acrescimoDesconto.Justificativa.Codigo);

                    if (!permiteConcederDesconto && justificativa.TipoJustificativa == TipoJustificativa.Desconto)
                        continue;
                    else if (!permiteConcederAcrescimo && justificativa.TipoJustificativa == TipoJustificativa.Acrescimo)
                        continue;

                    acrescimosDescontosAplicar.Add(new Dominio.ObjetosDeValor.Embarcador.Financeiro.BaixaTituloReceberAcrescimoDesconto()
                    {
                        Justificativa = justificativa,
                        Observacao = (string)acrescimoDesconto.Observacao,
                        Valor = Utilidades.Decimal.Converter((string)acrescimoDesconto.Valor)
                    });
                }
            }

            return acrescimosDescontosAplicar;
        }

        #endregion
    }
}
