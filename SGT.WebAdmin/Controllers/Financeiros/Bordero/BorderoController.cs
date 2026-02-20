using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Financeiros.Bordero
{
    [CustomAuthorize("Financeiros/Bordero")]
    public class BorderoController : BaseController
    {
		#region Construtores

		public BorderoController(Conexao conexao) : base(conexao) { }

		#endregion

        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int.TryParse(Request.Params("Numero"), out int numero);
                int.TryParse(Request.Params("GrupoPessoas"), out int codigoGrupoPessoas);
                int.TryParse(Request.Params("Titulo"), out int codigoTitulo);
                int.TryParse(Request.Params("CTe"), out int numeroCTe);

                DateTime.TryParseExact(Request.Params("DataEmissaoInicial"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataEmissaoInicial);
                DateTime.TryParseExact(Request.Params("DataEmissaoFinal"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataEmissaoFinal);
                DateTime.TryParseExact(Request.Params("DataVencimentoInicial"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataVencimentoInicial);
                DateTime.TryParseExact(Request.Params("DataVencimentoFinal"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataVencimentoFinal);

                decimal.TryParse(Request.Params("ValorACobrarInicial"), out decimal valorACobrarInicial);
                decimal.TryParse(Request.Params("ValorACobrarFinal"), out decimal valorACobrarFinal);
                decimal.TryParse(Request.Params("ValorTotalACobrarInicial"), out decimal valorTotalACobrarInicial);
                decimal.TryParse(Request.Params("ValorTotalACobrarFinal"), out decimal valorTotalACobrarFinal);

                double.TryParse(Utilidades.String.OnlyNumbers(Request.Params("Pessoa")), out double cpfCnpjPessoa);

                Enum.TryParse(Request.Params("TipoPessoa"), out Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPessoa tipoPessoa);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoBordero? situacao = null;
                if (Enum.TryParse(Request.Params("Situacao"), out Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoBordero situacaoAux))
                    situacao = situacaoAux;

                string numeroCarga = Request.Params("Carga");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Número", "Numero", 8, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Data Emissão", "DataEmissao", 8, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Data Vencto.", "DataVencimento", 8, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Tomador", "Tomador", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Vl. a Cobrar", "ValorACobrar", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Vl. Acréscimo", "ValorTotalAcrescimo", 8, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Vl. Desconto", "ValorTotalDesconto", 8, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Vl. Total", "ValorTotalACobrar", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Situação", "Situacao", 10, Models.Grid.Align.left, true);

                string propOrdena = grid.header[grid.indiceColunaOrdena].data;

                Repositorio.Embarcador.Financeiro.Bordero repBordero = new Repositorio.Embarcador.Financeiro.Bordero(unitOfWork);

                List<Dominio.Entidades.Embarcador.Financeiro.Bordero> listaBordero = repBordero.Consultar(numero, situacao, codigoTitulo, numeroCTe, numeroCarga, dataEmissaoInicial, dataEmissaoFinal, dataVencimentoInicial, dataVencimentoFinal, valorACobrarInicial, valorACobrarFinal, valorTotalACobrarInicial, valorTotalACobrarFinal, tipoPessoa, codigoGrupoPessoas, cpfCnpjPessoa, propOrdena, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repBordero.ContarConsulta(numero, situacao, codigoTitulo, numeroCTe, numeroCarga, dataEmissaoInicial, dataEmissaoFinal, dataVencimentoInicial, dataVencimentoFinal, valorACobrarInicial, valorACobrarFinal, valorTotalACobrarInicial, valorTotalACobrarFinal, tipoPessoa, codigoGrupoPessoas, cpfCnpjPessoa));

                var lista = (from p in listaBordero
                             select new
                             {
                                 p.Codigo,
                                 Numero = p.Numero.ToString(),
                                 DataEmissao = p.DataEmissao.ToString("dd/MM/yyyy"),
                                 DataVencimento = p.DataVencimento.ToString("dd/MM/yyyy"),
                                 Tomador = p.TipoPessoa == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPessoa.Pessoa ? p.Cliente.Nome + " (" + p.Cliente.CPF_CNPJ_Formatado + ")" : p.GrupoPessoas.Descricao,
                                 ValorACobrar = p.ValorACobrar.ToString("n2"),
                                 ValorTotalAcrescimo = p.ValorTotalAcrescimo.ToString("n2"),
                                 ValorTotalDesconto = p.ValorTotalDesconto.ToString("n2"),
                                 ValorTotalACobrar = p.ValorTotalACobrar.ToString("n2"),
                                 Situacao = p.DescricaoSituacao
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

        public async Task<IActionResult> Salvar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int.TryParse(Request.Params("Codigo"), out int codigo);
                int.TryParse(Request.Params("GrupoPessoas"), out int codigoGrupoPessoas);
                int.TryParse(Request.Params("Banco"), out int codigoBanco);
                int.TryParse(Request.Params("Empresa"), out int codigoEmpresa);

                DateTime.TryParseExact(Request.Params("DataEmissao"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataEmissao);
                DateTime.TryParseExact(Request.Params("DataVencimento"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataVencimento);

                double.TryParse(Utilidades.String.OnlyNumbers(Request.Params("Pessoa")), out double cpfCnpjPessoa);
                double.TryParse(Utilidades.String.OnlyNumbers(Request.Params("Tomador")), out double cpfCnpjTomador);

                Enum.TryParse(Request.Params("TipoPessoa"), out Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPessoa tipoPessoa);
                Enum.TryParse(Request.Params("TipoConta"), out Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaBanco tipoConta);

                bool.TryParse(Request.Params("ImprimirObservacao"), out bool imprimirObservacao);

                string observacao = Request.Params("Observacao");
                string agencia = Request.Params("Agencia");
                string digitoAgencia = Request.Params("DigitoAgencia");
                string numeroConta = Request.Params("NumeroConta");

                Repositorio.Embarcador.Financeiro.Bordero repBordero = new Repositorio.Embarcador.Financeiro.Bordero(unitOfWork);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Repositorio.Banco repBanco = new Repositorio.Banco(unitOfWork);

                Dominio.Entidades.Embarcador.Financeiro.Bordero bordero = codigo > 0 ? repBordero.BuscarPorCodigo(codigo, true) : new Dominio.Entidades.Embarcador.Financeiro.Bordero();

                if (codigo > 0 && bordero.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoBordero.EmAndamento)
                    return new JsonpResult(false, true, "O borderô não pode ser alterado na situação atual.");

                unitOfWork.Start();

                bordero.DataEmissao = dataEmissao;
                bordero.DataVencimento = dataVencimento;
                bordero.Observacao = observacao;
                bordero.ImprimirObservacao = imprimirObservacao;
                bordero.Agencia = agencia;
                bordero.DigitoAgencia = digitoAgencia;
                bordero.NumeroConta = numeroConta;
                bordero.TipoConta = tipoConta;
                bordero.Empresa = codigoEmpresa > 0 ? repEmpresa.BuscarPorCodigo(codigoEmpresa) : null;
                bordero.Banco = codigoBanco > 0 ? repBanco.BuscarPorCodigo(codigoBanco) : null;
                bordero.Tomador = cpfCnpjTomador > 0d ? repCliente.BuscarPorCPFCNPJ(cpfCnpjTomador) : null;

                if (bordero.Codigo <= 0)
                {
                    bordero.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoBordero.EmAndamento;
                    bordero.TipoPessoa = tipoPessoa;

                    if (tipoPessoa == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPessoa.Pessoa)
                    {
                        bordero.Cliente = cpfCnpjPessoa > 0d ? repCliente.BuscarPorCPFCNPJ(cpfCnpjPessoa) : null;
                        bordero.GrupoPessoas = null;
                    }
                    else
                    {
                        bordero.GrupoPessoas = codigoGrupoPessoas > 0 ? repGrupoPessoas.BuscarPorCodigo(codigoGrupoPessoas) : null;
                        bordero.Cliente = null;
                    }

                    if (bordero.TipoPessoa == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPessoa.Pessoa && bordero.Cliente == null)
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, true, "É obrigatório informar uma pessoa para o borderô.");
                    }
                    else if (bordero.TipoPessoa == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPessoa.GrupoPessoa && bordero.GrupoPessoas == null)
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, true, "É obrigatório informar um grupo de pessoas para o borderô.");
                    }

                    bordero.Numero = repBordero.ObterUltimoNumero() + 1;

                    repBordero.Inserir(bordero, Auditado);
                }
                else
                {
                    repBordero.Atualizar(bordero, Auditado);
                }

                unitOfWork.CommitChanges();

                return new JsonpResult(Servicos.Embarcador.Financeiro.Bordero.ObterDetalhesBordero(bordero));
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
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
                int.TryParse(Request.Params("Codigo"), out int codigo);

                Repositorio.Embarcador.Financeiro.Bordero repBordero = new Repositorio.Embarcador.Financeiro.Bordero(unitOfWork);

                Dominio.Entidades.Embarcador.Financeiro.Bordero bordero = repBordero.BuscarPorCodigo(codigo);

                return new JsonpResult(Servicos.Embarcador.Financeiro.Bordero.ObterDetalhesBordero(bordero));
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

        public async Task<IActionResult> Cancelar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int.TryParse(Request.Params("Codigo"), out int codigo);

                Repositorio.Embarcador.Financeiro.Bordero repBordero = new Repositorio.Embarcador.Financeiro.Bordero(unitOfWork);

                Dominio.Entidades.Embarcador.Financeiro.Bordero bordero = repBordero.BuscarPorCodigo(codigo);
                                
                if (!Servicos.Embarcador.Financeiro.Bordero.Cancelar(out string erro, bordero, unitOfWork, TipoServicoMultisoftware))
                    return new JsonpResult(false, true, erro);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, bordero, null, "Cancelou o Borderô.", unitOfWork);

                return new JsonpResult(Servicos.Embarcador.Financeiro.Bordero.ObterDetalhesBordero(bordero));
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ValidarCancelamentoBordero()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int.TryParse(Request.Params("Codigo"), out int codigo);

                Repositorio.Embarcador.Financeiro.Bordero repBordero = new Repositorio.Embarcador.Financeiro.Bordero(unitOfWork);
                Repositorio.Embarcador.Financeiro.TituloBaixa repTituloBaixa = new Repositorio.Embarcador.Financeiro.TituloBaixa(unitOfWork);

                Dominio.Entidades.Embarcador.Financeiro.Bordero bordero = repBordero.BuscarPorCodigo(codigo);
                Dominio.Entidades.Embarcador.Financeiro.TituloBaixa tituloBaixa = repTituloBaixa.BuscarPorBordero(bordero.Codigo);
                
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

        public async Task<IActionResult> Finalizar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int.TryParse(Request.Params("Codigo"), out int codigo);

                Repositorio.Embarcador.Financeiro.Bordero repBordero = new Repositorio.Embarcador.Financeiro.Bordero(unitOfWork);
                Repositorio.Embarcador.Financeiro.BorderoTitulo repBorderoTitulo = new Repositorio.Embarcador.Financeiro.BorderoTitulo(unitOfWork);

                Dominio.Entidades.Embarcador.Financeiro.Bordero bordero = repBordero.BuscarPorCodigo(codigo);

                if (bordero.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoBordero.EmAndamento)
                    return new JsonpResult(false, true, "A situação do borderô não permite que ele seja finalizado.");

                if (!repBorderoTitulo.ExistePorBordero(bordero.Codigo))
                    return new JsonpResult(false, true, "Não existem títulos no borderô, não sendo possível finalizar o mesmo.");

                bordero.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoBordero.Finalizado;

                repBordero.Atualizar(bordero);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, bordero, null, "Finalizou o Borderô.", unitOfWork);

                return new JsonpResult(Servicos.Embarcador.Financeiro.Bordero.ObterDetalhesBordero(bordero));
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

        public async Task<IActionResult> Liquidar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int.TryParse(Request.Params("Codigo"), out int codigo);
                int.TryParse(Request.Params("TipoPagamento"), out int codigoTipoPagamento);

                DateTime.TryParseExact(Request.Params("DataBase"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataBase);
                DateTime.TryParseExact(Request.Params("DataBaixa"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataBaixa);

                Repositorio.Embarcador.Financeiro.Bordero repBordero = new Repositorio.Embarcador.Financeiro.Bordero(unitOfWork);
                Repositorio.Embarcador.Financeiro.TipoPagamentoRecebimento repTipoPagamento = new Repositorio.Embarcador.Financeiro.TipoPagamentoRecebimento(unitOfWork);

                Dominio.Entidades.Embarcador.Financeiro.Bordero bordero = repBordero.BuscarPorCodigo(codigo);
                Dominio.Entidades.Embarcador.Financeiro.TipoPagamentoRecebimento tipoPagamento = repTipoPagamento.BuscarPorCodigo(codigoTipoPagamento);

                if (!Servicos.Embarcador.Financeiro.Bordero.Liquidar(out string erro, Usuario, bordero, dataBase, dataBaixa, tipoPagamento, unitOfWork, TipoServicoMultisoftware))
                    return new JsonpResult(false, true, erro);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, bordero, null, "Liquidou o Borderô.", unitOfWork);

                return new JsonpResult(Servicos.Embarcador.Financeiro.Bordero.ObterDetalhesBordero(bordero));
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Imprimir()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int.TryParse(Request.Params("Codigo"), out int codigo);

                Repositorio.Embarcador.Financeiro.Bordero repBordero = new Repositorio.Embarcador.Financeiro.Bordero(unitOfWork);

                Dominio.Entidades.Embarcador.Financeiro.Bordero bordero = repBordero.BuscarPorCodigo(codigo);

                return Arquivo(Servicos.Embarcador.Financeiro.Bordero.GerarImpressaoBordero(bordero, unitOfWork), "application/pdf", "Borderô " + bordero.Numero.ToString() + ".pdf");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao gerar a impressão do borderô.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ObterDadosPagamentoTomador()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unidadeTrabalho);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeTrabalho);

                int.TryParse(Request.Params("GrupoPessoas"), out int codigoGrupoPessoas);
                double.TryParse(Utilidades.String.OnlyNumbers(Request.Params("CPFCNPJ")), out double cpfCnpj);

                object retorno = null;

                if (codigoGrupoPessoas > 0)
                {
                    Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas = repGrupoPessoas.BuscarPorCodigo(codigoGrupoPessoas);

                    if (grupoPessoas != null)
                    {
                        retorno = new
                        {
                            grupoPessoas.Agencia,
                            grupoPessoas.DigitoAgencia,
                            grupoPessoas.NumeroConta,
                            TipoConta = grupoPessoas.TipoContaBanco,
                            Banco = new
                            {
                                Codigo = grupoPessoas.Banco?.Codigo ?? 0,
                                Descricao = grupoPessoas.Banco?.Descricao ?? string.Empty
                            },
                            Tomador = new
                            {
                                Codigo = grupoPessoas.ClienteTomadorFatura?.CPF_CNPJ_SemFormato,
                                Descricao = grupoPessoas.ClienteTomadorFatura != null ? grupoPessoas.ClienteTomadorFatura.Nome + " (" + grupoPessoas.ClienteTomadorFatura.CPF_CNPJ_Formatado + ")" : string.Empty
                            }
                        };
                    }
                }
                else if (cpfCnpj > 0d)
                {
                    Dominio.Entidades.Cliente pessoa = repCliente.BuscarPorCPFCNPJ(cpfCnpj);

                    if (pessoa != null && pessoa.NaoUsarConfiguracaoFaturaGrupo)
                    {
                        retorno = new
                        {
                            pessoa.Agencia,
                            pessoa.DigitoAgencia,
                            pessoa.NumeroConta,
                            TipoConta = pessoa.TipoContaBanco,
                            Banco = new
                            {
                                Codigo = pessoa.Banco?.Codigo ?? 0,
                                Descricao = pessoa.Banco?.Descricao ?? string.Empty
                            },
                            Tomador = new
                            {
                                Codigo = pessoa.ClienteTomadorFatura?.CPF_CNPJ_SemFormato,
                                Descricao = pessoa.ClienteTomadorFatura != null ? pessoa.ClienteTomadorFatura.Nome + " (" + pessoa.ClienteTomadorFatura.CPF_CNPJ_Formatado + ")" : string.Empty
                            }
                        };
                    }
                    else if (pessoa.GrupoPessoas != null)
                    {
                        retorno = new
                        {
                            pessoa.GrupoPessoas.Agencia,
                            pessoa.GrupoPessoas.DigitoAgencia,
                            pessoa.GrupoPessoas.NumeroConta,
                            TipoConta = pessoa.GrupoPessoas.TipoContaBanco,
                            Banco = new
                            {
                                Codigo = pessoa.GrupoPessoas.Banco?.Codigo ?? 0,
                                Descricao = pessoa.GrupoPessoas.Banco?.Descricao ?? string.Empty
                            },
                            Tomador = new
                            {
                                Codigo = pessoa.GrupoPessoas.ClienteTomadorFatura?.CPF_CNPJ_SemFormato,
                                Descricao = pessoa.GrupoPessoas.ClienteTomadorFatura != null ? pessoa.GrupoPessoas.ClienteTomadorFatura.Nome + " (" + pessoa.GrupoPessoas.ClienteTomadorFatura.CPF_CNPJ_Formatado + ")" : string.Empty
                            }
                        };
                    }
                }

                if (retorno == null)
                {
                    retorno = new
                    {
                        Agencia = string.Empty,
                        DigitoAgencia = string.Empty,
                        NumeroConta = string.Empty,
                        TipoConta = string.Empty,
                        Banco = new
                        {
                            Codigo = 0,
                            Descricao = string.Empty
                        },
                        Tomador = new
                        {
                            Codigo = "",
                            Descricao = string.Empty
                        }
                    };
                }

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao obter as informações para pagamento do tomador.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }
    }
}
