using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Integracao.EDI
{
    public class EBSBaixaFinanceiro
    {
        public static Dominio.ObjetosDeValor.EDI.EBS.BaixaFinanceiro GerarEBS(DateTime dataInicial, DateTime dataFinal, Repositorio.UnitOfWork unidadeTrabalho, string stringConexao)
        {
            Repositorio.Embarcador.Financeiro.TituloBaixa repTituloBaixa = new Repositorio.Embarcador.Financeiro.TituloBaixa(unidadeTrabalho);
            Repositorio.Embarcador.Financeiro.MovimentoFinanceiro repMovimentoFinanceiro = new Repositorio.Embarcador.Financeiro.MovimentoFinanceiro(unidadeTrabalho);
            Servicos.Embarcador.Financeiro.BaixaTituloPagar svcBaixaTituloPagar = new Financeiro.BaixaTituloPagar(unidadeTrabalho);

            List<Dominio.Entidades.Embarcador.Financeiro.TituloBaixa> baixas = repTituloBaixa.BuscarPorPeriodo(dataInicial, dataFinal, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoBaixaTitulo.Finalizada);

            int sequencia = 1;

            Dominio.ObjetosDeValor.EDI.EBS.BaixaFinanceiro ebs = new Dominio.ObjetosDeValor.EDI.EBS.BaixaFinanceiro();

            ebs.TipoLote = "M";
            ebs.DataLote = dataFinal;
            ebs.TotalLote = 0m;
            ebs.Descricao = string.Empty;
            ebs.Origem = "OUT";
            ebs.Identificador = ""; //dataInicial.ToString("ddMM") + dataFinal.ToString("ddMMyy");
            ebs.SituacaoLote = ""; //"L";
            ebs.CPFCNPJEstabelecimento = string.Empty;
            ebs.Sequencia = sequencia;

            ebs.Lancamentos = new List<Dominio.ObjetosDeValor.EDI.EBS.BaixaFinanceiroLancamento>();



            foreach (Dominio.Entidades.Embarcador.Financeiro.TituloBaixa tituloBaixa in baixas)
            {
                if (tituloBaixa.TitulosAgrupados.Count <= 0)
                    continue;

                sequencia++;

                Dominio.Entidades.Embarcador.Financeiro.PlanoConta contaCredito = null, contaDebito = null;
                decimal valorLancamento = 0m;

                contaDebito = tituloBaixa.TipoPagamentoRecebimento?.PlanoConta;

                Dominio.Entidades.Embarcador.Financeiro.Titulo primeiroTitulo = tituloBaixa.TitulosAgrupados[0].Titulo;

                if (tituloBaixa.TipoBaixaTitulo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTitulo.Pagar)
                {
                    valorLancamento = tituloBaixa.Valor;

                    if (primeiroTitulo.TituloBaixaNegociacao == null)
                    {
                        if (primeiroTitulo.ContratoFrete != null)
                            contaCredito = repMovimentoFinanceiro.BuscarContaCreditoTituloContratoFrete(primeiroTitulo.ContratoFrete.Codigo);
                        else
                            contaCredito = repMovimentoFinanceiro.BuscarContaCreditoTitulo(primeiroTitulo.Codigo);
                    }
                    else if (primeiroTitulo.TituloBaixaNegociacao.TituloBaixa.TitulosAgrupados != null && primeiroTitulo.TituloBaixaNegociacao.TituloBaixa.TitulosAgrupados.Count() > 0 && primeiroTitulo.TituloBaixaNegociacao.TituloBaixa.TitulosAgrupados[0].Titulo != null)
                    {
                        Dominio.Entidades.Embarcador.Financeiro.Titulo tituloPai = primeiroTitulo.TituloBaixaNegociacao.TituloBaixa.TitulosAgrupados[0].Titulo;
                        Dominio.Entidades.Embarcador.Financeiro.Titulo tituloPaiNovo = null;

                        tituloPaiNovo = svcBaixaTituloPagar.RetornaTituloPai(primeiroTitulo.TituloBaixaNegociacao.TituloBaixa.TitulosAgrupados[0].Titulo);

                        if (tituloPaiNovo != null)
                        {
                            tituloPai = tituloPaiNovo;
                            while (tituloPaiNovo != null)
                            {
                                tituloPaiNovo = svcBaixaTituloPagar.RetornaTituloPai(tituloPai);
                                if (tituloPaiNovo != null)
                                    tituloPai = tituloPaiNovo;
                            }
                        }

                        contaCredito = repMovimentoFinanceiro.BuscarContaCreditoTitulo(tituloPai.Codigo);
                        if (contaCredito == null && tituloPai.ContratoFrete != null)
                            contaCredito = repMovimentoFinanceiro.BuscarContaCreditoTituloContratoFrete(tituloPai.ContratoFrete.Codigo);
                    }
                }
                else if (tituloBaixa.TipoBaixaTitulo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTitulo.Receber)
                {
                    if (tituloBaixa.ModeloAntigo)
                    {
                        valorLancamento = tituloBaixa.Valor;

                        if (primeiroTitulo.TituloBaixaNegociacao == null)
                            contaCredito = repMovimentoFinanceiro.BuscarContaDebitoTitulo(primeiroTitulo.Codigo);
                        else if (primeiroTitulo.TituloBaixaNegociacao.TituloBaixa.TitulosAgrupados != null && primeiroTitulo.TituloBaixaNegociacao.TituloBaixa.TitulosAgrupados.Count() > 0 && primeiroTitulo.TituloBaixaNegociacao.TituloBaixa.TitulosAgrupados[0].Titulo != null)
                        {
                            Dominio.Entidades.Embarcador.Financeiro.Titulo tituloPai = primeiroTitulo.TituloBaixaNegociacao.TituloBaixa.TitulosAgrupados[0].Titulo;
                            Dominio.Entidades.Embarcador.Financeiro.Titulo tituloPaiNovo = null;
                            tituloPaiNovo = svcBaixaTituloPagar.RetornaTituloPai(primeiroTitulo.TituloBaixaNegociacao.TituloBaixa.TitulosAgrupados[0].Titulo);

                            if (tituloPaiNovo != null)
                            {
                                tituloPai = tituloPaiNovo;
                                while (tituloPaiNovo != null)
                                {
                                    tituloPaiNovo = svcBaixaTituloPagar.RetornaTituloPai(tituloPai);
                                    if (tituloPaiNovo != null)
                                        tituloPai = tituloPaiNovo;
                                }
                            }

                            contaCredito = repMovimentoFinanceiro.BuscarContaDebitoTitulo(tituloPai.Codigo);
                        }
                    }
                    else
                    {
                        valorLancamento = tituloBaixa.ValorPago;
                        contaCredito = primeiroTitulo.TipoMovimento.PlanoDeContaDebito;
                    }
                }

                int numeroContaDebito = 0, numeroContaCredito = 0;

                if (contaDebito != null && !string.IsNullOrWhiteSpace(contaDebito.PlanoContabilidade))
                    int.TryParse(contaDebito.PlanoContabilidade, out numeroContaDebito);

                if (contaCredito != null && !string.IsNullOrWhiteSpace(contaCredito.PlanoContabilidade))
                    int.TryParse(contaCredito.PlanoContabilidade, out numeroContaCredito);

                Dominio.ObjetosDeValor.EDI.EBS.BaixaFinanceiroLancamento lancamento = new Dominio.ObjetosDeValor.EDI.EBS.BaixaFinanceiroLancamento()
                {
                    DataLancamento = tituloBaixa.DataBaixa.Value,
                    ContaDebito = numeroContaDebito,
                    ContaCredito = numeroContaCredito,
                    Historico = 1,
                    Complemento = string.Empty,
                    ValorLancamento = valorLancamento,
                    Sequencia = sequencia,
                    Historicos = new List<Dominio.ObjetosDeValor.EDI.EBS.BaixaFinanceiroLancamentoHistorico>()
                };

                if (!string.IsNullOrWhiteSpace(tituloBaixa.Observacao))
                {
                    sequencia++;

                    lancamento.Historicos.Add(new Dominio.ObjetosDeValor.EDI.EBS.BaixaFinanceiroLancamentoHistorico()
                    {
                        Historico = tituloBaixa.Observacao,
                        Sequencia = sequencia
                    });
                }

                ebs.Lancamentos.Add(lancamento);
            }

            return ebs;
        }
    }
}
