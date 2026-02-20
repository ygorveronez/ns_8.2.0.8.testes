using Dominio.Excecoes.Embarcador;
using Dominio.Interfaces.Database;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Servicos.Embarcador.Financeiro
{
    public class BaixaTituloPagar : ServicoBase
    {        
        public BaixaTituloPagar(Repositorio.UnitOfWork unitOfWork, CancellationToken cancelationToken = default) : base(unitOfWork, cancelationToken) { }
        #region Métodos Públicos

        public dynamic RetornaObjetoCompletoTitulo(int codigoBaixaTitulo, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Financeiro.TituloBaixa repTituloBaixa = new Repositorio.Embarcador.Financeiro.TituloBaixa(unidadeDeTrabalho);
            Repositorio.Embarcador.Financeiro.TituloBaixaNegociacao repTituloBaixaNegociacao = new Repositorio.Embarcador.Financeiro.TituloBaixaNegociacao(unidadeDeTrabalho);

            Dominio.Entidades.Embarcador.Financeiro.TituloBaixa tituloBaixa = repTituloBaixa.BuscarPorCodigo(codigoBaixaTitulo);

            string detalhe = tituloBaixa.CodigosTitulos;

            return new
            {
                Codigo = tituloBaixa != null ? tituloBaixa.Codigo : 0,
                Etapa = tituloBaixa != null ? tituloBaixa.SituacaoBaixaTitulo : SituacaoBaixaTitulo.Iniciada,
                ParcelaTitulo = "1",
                ValorPendenteTitulo = tituloBaixa.ValorPendente.ToString("n2"),
                ValorBaixado = tituloBaixa != null ? tituloBaixa.Valor.ToString("n2") : tituloBaixa.ValorPendente.ToString("n2"),
                tituloBaixa.MoedaCotacaoBancoCentral,
                DataBaseCRT = tituloBaixa.DataBaseCRT.HasValue ? tituloBaixa.DataBaseCRT.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty,
                ValorMoedaCotacao = tituloBaixa.ValorMoedaCotacao.ToString("n10"),
                ValorOriginalMoedaEstrangeira = tituloBaixa.ValorOriginalMoedaEstrangeira.ToString("n2"),
                ListaParcelasNegociacao = tituloBaixa != null ? repTituloBaixaNegociacao.BuscarPorBaixaTitulo(tituloBaixa.Codigo) : null,
                NumeroTitulo = tituloBaixa.CodigosTitulos,
                DataBaixa = tituloBaixa != null ? tituloBaixa.DataBaixa.Value.ToString("dd/MM/yyyy") : string.Empty,
                Observacao = tituloBaixa != null ? tituloBaixa.Observacao : string.Empty,
                DescricaoSituacao = tituloBaixa != null ? tituloBaixa.DescricaoSituacaoBaixaTitulo : string.Empty,
                TituloDeAgrupamento = tituloBaixa != null && repTituloBaixa.ContemTitulosGeradosDeNegociacao(codigoBaixaTitulo) ? "Atenção! Contem título(s) gerado(s) a partir de uma negociação" : string.Empty,
                NomePessoa = tituloBaixa != null && tituloBaixa.Pessoa != null ? tituloBaixa.Pessoa.Nome : "MÚLTIPLOS FORNECEDORES",
                Detalhe = detalhe,
                MoedaCotacaoBancoCentralNegociacao = tituloBaixa.MoedaCotacaoBancoCentral.HasValue ? tituloBaixa.MoedaCotacaoBancoCentral.Value.ObterDescricaoSimplificada() : "",
                DataBaseCRTNegociacao = tituloBaixa.DataBaseCRT.HasValue ? tituloBaixa.DataBaseCRT.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty,
                ValorMoedaCotacaoNegociacao = tituloBaixa.ValorMoedaCotacao.ToString("n10"),
                ValorOriginalMoedaEstrangeiraNegociacao = tituloBaixa.ValorOriginalMoedaEstrangeira.ToString("n2"),
                ValorPendenteTituloMoedaEstrangeira = tituloBaixa.ValorPendenteMoedaEstrangeira.ToString("n2"),
                Situacao = tituloBaixa != null ? tituloBaixa.SituacaoBaixaTitulo : SituacaoBaixaTitulo.Iniciada
            };
        }

        public void GeraIntegracaoBaixaTituloPagar(int codigo, string operador, string emailAdministrativo, Repositorio.UnitOfWork unidadeDeTrabalho, bool enviarEmail)
        {
            Repositorio.Embarcador.Financeiro.TituloBaixa repTituloBaixa = new Repositorio.Embarcador.Financeiro.TituloBaixa(unidadeDeTrabalho);
            Repositorio.Embarcador.Financeiro.TituloBaixaIntegracao repTituloBaixaIntegracao = new Repositorio.Embarcador.Financeiro.TituloBaixaIntegracao(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Financeiro.TituloBaixa baixa = repTituloBaixa.BuscarPorCodigo(codigo);

            Dominio.Entidades.Embarcador.Financeiro.TituloBaixaIntegracao integracao = new Dominio.Entidades.Embarcador.Financeiro.TituloBaixaIntegracao();
            integracao.Assunto = "Baixa de título a pagar Nº " + baixa.CodigosTitulos;
            if (integracao.Assunto.Length >= 300)
                integracao.Assunto = integracao.Assunto.Substring(0, 299);
            integracao.DataIntegracao = DateTime.Now;
            integracao.Destinatarios = emailAdministrativo;
            integracao.Mensagem = "Foi realizado a baixa do título a pagar de número " + baixa.CodigosTitulos + " pelo operador " + operador;
            if (integracao.Mensagem.Length >= 300)
                integracao.Mensagem = integracao.Mensagem.Substring(0, 299);
            integracao.TipoIntegracaoTituloBaixa = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracaoTituloBaixa.Email;
            integracao.TituloBaixa = baixa;

            repTituloBaixaIntegracao.Inserir(integracao);

            Repositorio.Embarcador.Email.ConfigEmailDocTransporte repConfigEmailDocTransporte = new Repositorio.Embarcador.Email.ConfigEmailDocTransporte(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte email = repConfigEmailDocTransporte.BuscarEmailEnviaDocumentoAtivo();
            if (email != null && !string.IsNullOrWhiteSpace(emailAdministrativo) && enviarEmail)
            {
                string mensagemErro = "";
                Servicos.Email.EnviarEmail(email.Email, email.Email, email.Senha, emailAdministrativo, null, null, integracao.Assunto, integracao.Mensagem, email.Smtp, out mensagemErro, email.DisplayEmail, null, "", email.RequerAutenticacaoSmtp, "", email.PortaSmtp, unidadeDeTrabalho);
            }
        }

        public bool GeraReverteMovimentacaoFinanceira(out string erro, int codigoBaixa, Repositorio.UnitOfWork unidadeDeTrabalho, string stringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServico, bool reversao, Dominio.Entidades.Embarcador.Financeiro.PlanoConta planoBaixaTitulo, Dominio.Entidades.Embarcador.Financeiro.PlanoConta planoCreditoTitulo = null, int codigoTitulo = 0)
        {
            Repositorio.Embarcador.Financeiro.TituloBaixa repTituloBaixa = new Repositorio.Embarcador.Financeiro.TituloBaixa(unidadeDeTrabalho);
            Repositorio.Embarcador.Financeiro.TituloBaixaAcrescimo repTituloBaixaAcrescimo = new Repositorio.Embarcador.Financeiro.TituloBaixaAcrescimo(unidadeDeTrabalho);
            Repositorio.Embarcador.Financeiro.TituloBaixaDesconto repTituloBaixaDesconto = new Repositorio.Embarcador.Financeiro.TituloBaixaDesconto(unidadeDeTrabalho);
            Repositorio.Embarcador.Financeiro.PlanoConta repPlanoConta = new Repositorio.Embarcador.Financeiro.PlanoConta(unidadeDeTrabalho);
            Repositorio.Embarcador.Financeiro.TituloBaixaTipoPagamentoRecebimento repTituloBaixaTipoPagamentoRecebimento = new Repositorio.Embarcador.Financeiro.TituloBaixaTipoPagamentoRecebimento(unidadeDeTrabalho);
            Repositorio.Embarcador.Financeiro.TituloBaixaAgrupado repTituloBaixaAgrupado = new Repositorio.Embarcador.Financeiro.TituloBaixaAgrupado(unidadeDeTrabalho);
            Repositorio.Embarcador.Financeiro.MovimentoFinanceiro repMovimentoFinanceiro = new Repositorio.Embarcador.Financeiro.MovimentoFinanceiro(unidadeDeTrabalho);

            Servicos.Embarcador.Financeiro.ProcessoMovimento servProcessoMovimento = new Servicos.Embarcador.Financeiro.ProcessoMovimento(stringConexao);

            erro = "";
            string obs = "";
            Dominio.Entidades.Embarcador.Financeiro.TituloBaixa tituloBaixa = repTituloBaixa.BuscarPorCodigo(codigoBaixa);
            List<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado> listaTituloBaixaAgrupado = repTituloBaixaAgrupado.BuscarPorBaixaTitulo(codigoBaixa);
            List<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaTipoPagamentoRecebimento> listaPagamentos = repTituloBaixaTipoPagamentoRecebimento.BuscarPorBaixaTitulo(codigoBaixa);
            Dominio.Entidades.Embarcador.Financeiro.PlanoConta creditoTitulo = null;
            Dominio.Entidades.Embarcador.Financeiro.PlanoConta planoBaixa = null;
            FormaTitulo? formaTitulo = null;

            var observacaoAdicionalMovimentacao = string.Empty;
            if (!string.IsNullOrWhiteSpace(tituloBaixa.Observacao))
                observacaoAdicionalMovimentacao = " " + tituloBaixa.Observacao;
            if (tituloBaixa.Cheques?.Count > 0)
                observacaoAdicionalMovimentacao += " Cheque(s) nº: " + string.Join(", ", tituloBaixa.Cheques.Select(o => o.Cheque.NumeroCheque));
            if (listaTituloBaixaAgrupado?.Count > 0)
                observacaoAdicionalMovimentacao += " Doc(s): " + string.Join(", ", listaTituloBaixaAgrupado.Select(o => o.Titulo.NumeroDocumentoTituloOriginal).Distinct());

            if (tituloBaixa.Valor > 0 && planoBaixaTitulo != null && (listaPagamentos == null || listaPagamentos.Count == 0))
            {
                int countTitulosAgrupados = listaTituloBaixaAgrupado != null ? listaTituloBaixaAgrupado.Count : 0;
                for (int i = 0; i < countTitulosAgrupados; i++)
                {
                    Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = listaTituloBaixaAgrupado[i].Titulo;
                    formaTitulo = titulo.FormaTitulo;

                    creditoTitulo = BuscarContaCreditoTitulo(out erro, titulo, unidadeDeTrabalho, planoCreditoTitulo);
                    if (!string.IsNullOrWhiteSpace(erro) && tituloBaixa.Valor > 0)
                        return false;
                    else
                        planoCreditoTitulo = creditoTitulo;
                }

                if (planoCreditoTitulo == null)
                {
                    erro = "Não foi possível localizar a conta de saída para realizar a movimentação financeira";
                    return false;
                }
                else
                {
                    if (reversao)
                    {
                        if (servProcessoMovimento.MovimentacaoFinanceiraJaConciliada(null, tituloBaixa.Valor, tituloBaixa.Codigo.ToString(), unidadeDeTrabalho, TipoDocumentoMovimento.Pagamento, planoBaixaTitulo, planoCreditoTitulo, 0))
                        {
                            erro = "Não é possível reverter esta baixa pois já se encontra conciliada.";
                            return false;
                        }
                        servProcessoMovimento.GerarMovimentacao(null, tituloBaixa.DataBaixa.Value, tituloBaixa.Valor, tituloBaixa.Codigo.ToString(), "REVERSÃO BAIXA DO TITULO A PAGAR." + observacaoAdicionalMovimentacao, unidadeDeTrabalho, TipoDocumentoMovimento.Pagamento, tipoServico, 0, planoCreditoTitulo, planoBaixaTitulo, codigoTitulo, null, tituloBaixa.Pessoa, tituloBaixa.Pessoa?.GrupoPessoas ?? tituloBaixa.GrupoPessoas, null, null, null, null, null, tituloBaixa.MoedaCotacaoBancoCentral, tituloBaixa.DataBaseCRT, tituloBaixa.ValorMoedaCotacao, tituloBaixa.ValorOriginalMoedaEstrangeira, null, null, formaTitulo);
                    }
                    else
                        servProcessoMovimento.GerarMovimentacao(null, tituloBaixa.DataBaixa.Value, tituloBaixa.Valor, tituloBaixa.Codigo.ToString(), "BAIXA DO TITULO A PAGAR." + observacaoAdicionalMovimentacao, unidadeDeTrabalho, TipoDocumentoMovimento.Pagamento, tipoServico, 0, planoBaixaTitulo, planoCreditoTitulo, codigoTitulo, null, tituloBaixa.Pessoa, tituloBaixa.Pessoa?.GrupoPessoas ?? tituloBaixa.GrupoPessoas, null, null, null, null, null, tituloBaixa.MoedaCotacaoBancoCentral, tituloBaixa.DataBaseCRT, tituloBaixa.ValorMoedaCotacao, tituloBaixa.ValorOriginalMoedaEstrangeira, null, null, formaTitulo);
                }
            }
            else if (tituloBaixa.Valor > 0 && listaPagamentos.Count > 0)
            {
                List<Dominio.ObjetosDeValor.Embarcador.Financeiro.ContaCreditoBaixaTitulo> listaContas = new List<Dominio.ObjetosDeValor.Embarcador.Financeiro.ContaCreditoBaixaTitulo>();
                List<Dominio.ObjetosDeValor.Embarcador.Financeiro.ContaCreditoBaixaTitulo> listaContasPagamento = new List<Dominio.ObjetosDeValor.Embarcador.Financeiro.ContaCreditoBaixaTitulo>();
                bool achouConta = false;
                foreach (Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado tituloBaixaAgrupado in listaTituloBaixaAgrupado)
                {
                    Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = tituloBaixaAgrupado.Titulo;
                    formaTitulo = titulo.FormaTitulo;

                    creditoTitulo = BuscarContaCreditoTitulo(out erro, titulo, unidadeDeTrabalho);
                    if (!string.IsNullOrWhiteSpace(erro))
                        return false;

                    achouConta = false;
                    foreach (Dominio.ObjetosDeValor.Embarcador.Financeiro.ContaCreditoBaixaTitulo conta in listaContas)
                    {
                        if (conta.CodigoConta == creditoTitulo.Codigo)
                        {
                            conta.Valor += titulo.ValorPago;// - titulo.Titulo.Acrescimo;
                            achouConta = true;
                            break;
                        }
                    }
                    if (!achouConta)
                    {
                        listaContas.Add(new Dominio.ObjetosDeValor.Embarcador.Financeiro.ContaCreditoBaixaTitulo()
                        {
                            CodigoConta = creditoTitulo.Codigo,
                            Valor = titulo.ValorPago// - titulo.Titulo.Acrescimo
                        });
                    }
                }
                foreach (Dominio.Entidades.Embarcador.Financeiro.TituloBaixaTipoPagamentoRecebimento pagamento in listaPagamentos)
                {
                    listaContasPagamento.Add(new Dominio.ObjetosDeValor.Embarcador.Financeiro.ContaCreditoBaixaTitulo()
                    {
                        CodigoConta = pagamento.TipoPagamentoRecebimento.PlanoConta.Codigo,
                        Valor = pagamento.Valor,
                        ValorOriginal = pagamento.Valor,
                        TipoPagamentoRecebimento = pagamento.TipoPagamentoRecebimento,
                        MoedaCotacaoBancoCentral = pagamento.MoedaCotacaoBancoCentral,
                        DataBaseCRT = pagamento.DataBaseCRT,
                        ValorMoedaCotacao = pagamento.ValorMoedaCotacao,
                        ValorOriginalMoedaEstrangeira = pagamento.ValorOriginalMoedaEstrangeira
                    });
                }

                /*decimal somaAcrescimos = 0;
                decimal somaDescontos = 0;
                somaDescontos = repTituloBaixaAcrescimo.TotalPorTituloBaixa(tituloBaixa.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoJustificativa.Desconto);
                somaAcrescimos = repTituloBaixaAcrescimo.TotalPorTituloBaixa(tituloBaixa.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoJustificativa.Acrescimo);*/
                decimal somaValorListaPagamento = listaContasPagamento.Sum(o => o.Valor);
                decimal somaValorListaContas = listaContas.Sum(o => o.Valor);

                //somaValorListaContas = somaValorListaContas;// - somaAcrescimos; REMOVIDO DEVIDO ALTERAÇÕES NO AJUSTE DO VALOR PAGO NA ETAPA ANTERIOR

                if (!reversao && Math.Round(somaValorListaPagamento, 2) != Math.Round(somaValorListaContas, 2))
                {
                    erro = "A soma dos valores dos títulos com a lista de pagamento não está correta. Lista Pagamento: " + somaValorListaPagamento.ToString("n2") + " Lista Contas: " + somaValorListaContas.ToString("n2");
                    return false;
                }
                foreach (Dominio.ObjetosDeValor.Embarcador.Financeiro.ContaCreditoBaixaTitulo pagamento in listaContasPagamento)
                {
                    if (pagamento.Valor > 0)
                    {
                        decimal valorAcumulado = 0;
                        foreach (var conta in listaContas)
                        {
                            if (conta.Valor > 0 && pagamento.Valor > 0)
                            {
                                decimal valorBaixar = 0;
                                if (conta.Valor > pagamento.Valor)
                                    valorBaixar = pagamento.Valor;
                                else
                                    valorBaixar = conta.Valor;

                                if (valorBaixar < 0)
                                    valorBaixar = 0;

                                if (valorBaixar > 0)
                                {
                                    conta.Valor -= valorBaixar;
                                    pagamento.Valor -= valorBaixar;
                                    valorAcumulado += valorBaixar;
                                    if (pagamento.ValorOriginal > valorAcumulado)
                                    {
                                        decimal diferenta = pagamento.ValorOriginal - valorAcumulado;
                                        if (diferenta > 0 && diferenta < (decimal)(0.10))
                                            valorBaixar += diferenta;
                                    }

                                    planoCreditoTitulo = repPlanoConta.BuscarPorCodigo(conta.CodigoConta);
                                    planoBaixa = repPlanoConta.BuscarPorCodigo(pagamento.CodigoConta);
                                    if (reversao)
                                    {
                                        if (servProcessoMovimento.MovimentacaoFinanceiraJaConciliada(null, valorBaixar, tituloBaixa.Codigo.ToString(), unidadeDeTrabalho, TipoDocumentoMovimento.Pagamento, planoBaixa, planoCreditoTitulo, 0))
                                        {
                                            erro = "Não é possível reverter esta baixa pois já se encontra conciliada.";
                                            return false;
                                        }
                                        servProcessoMovimento.GerarMovimentacao(null, tituloBaixa.DataBaixa.Value, valorBaixar, tituloBaixa.Codigo.ToString(), "REVERSÃO BAIXA DO TITULO A PAGAR." + observacaoAdicionalMovimentacao, unidadeDeTrabalho, TipoDocumentoMovimento.Pagamento, tipoServico, 0, planoCreditoTitulo, planoBaixa, codigoTitulo, null, tituloBaixa.Pessoa, tituloBaixa.Pessoa?.GrupoPessoas ?? tituloBaixa.GrupoPessoas, null, null, pagamento.TipoPagamentoRecebimento.Exportar ? pagamento.TipoPagamentoRecebimento.ContasExportacao.Where(o => o.Reversao == true).ToList() : null, tituloBaixa, TipoMovimentoExportacao.ReversaoPagamentoContratoFrete, pagamento.MoedaCotacaoBancoCentral, pagamento.DataBaseCRT, pagamento.ValorMoedaCotacao, pagamento.ValorOriginalMoedaEstrangeira, null, null, formaTitulo);
                                    }
                                    else
                                        servProcessoMovimento.GerarMovimentacao(null, tituloBaixa.DataBaixa.Value, valorBaixar, tituloBaixa.Codigo.ToString(), "BAIXA DO TITULO A PAGAR." + observacaoAdicionalMovimentacao, unidadeDeTrabalho, TipoDocumentoMovimento.Pagamento, tipoServico, 0, planoBaixa, planoCreditoTitulo, codigoTitulo, null, tituloBaixa.Pessoa, tituloBaixa.Pessoa?.GrupoPessoas ?? tituloBaixa.GrupoPessoas, null, null, pagamento.TipoPagamentoRecebimento.Exportar ? pagamento.TipoPagamentoRecebimento.ContasExportacao.Where(o => !o.Reversao.HasValue || o.Reversao == false).ToList() : null, tituloBaixa, TipoMovimentoExportacao.PagamentoContratoFrete, pagamento.MoedaCotacaoBancoCentral, pagamento.DataBaseCRT, pagamento.ValorMoedaCotacao, pagamento.ValorOriginalMoedaEstrangeira, null, null, formaTitulo);
                                }
                            }
                        }
                    }

                }
            }
            else if (tituloBaixa.Valor > 0 && listaPagamentos != null && listaPagamentos.Count == 0 && tipoServico != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
            {
                erro = "Favor informe ao menos uma conta de pagamento para realizar a baixa do título";
                return false;
            }

            if (tituloBaixa.Acrescimos?.Count > 0)
            {
                foreach (Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAcrescimo acrescimoDesconto in tituloBaixa.Acrescimos)
                {
                    if (acrescimoDesconto.Justificativa.GerarMovimentoAutomatico)
                    {
                        if (reversao)
                            obs = "REVERSÃO ";
                        else
                            obs = "";

                        if (acrescimoDesconto.Justificativa.TipoJustificativa == TipoJustificativa.Acrescimo)
                            obs += "ACRESCIMO";
                        else
                            obs += "DESCONTO";

                        obs += " NA BAIXA DO(S) TITULO(S) " + tituloBaixa.CodigosTitulos + ".";

                        if (reversao)
                        {
                            if (acrescimoDesconto.Justificativa.TipoMovimentoReversaoUsoJustificativa == null)
                            {
                                erro = "A justificativa " + acrescimoDesconto.Justificativa.Descricao + " não possui tipo de movimento vinculado para a sua reversão.";
                                return false;
                            }
                            if (servProcessoMovimento.MovimentacaoFinanceiraJaConciliada(null, acrescimoDesconto.Valor, tituloBaixa.Codigo.ToString(), unidadeDeTrabalho, TipoDocumentoMovimento.Pagamento, acrescimoDesconto.Justificativa.TipoMovimentoReversaoUsoJustificativa.PlanoDeContaDebito, acrescimoDesconto.Justificativa.TipoMovimentoReversaoUsoJustificativa.PlanoDeContaCredito, 0))
                            {
                                erro = "Não é possível reverter esta baixa pois já se encontra conciliada.";
                                return false;
                            }

                            if (acrescimoDesconto.Justificativa.TipoJustificativa == TipoJustificativa.Desconto)
                                servProcessoMovimento.GerarMovimentacao(acrescimoDesconto.Justificativa.TipoMovimentoReversaoUsoJustificativa, tituloBaixa.DataBaixa.Value, acrescimoDesconto.Valor, tituloBaixa.Codigo.ToString(), obs + observacaoAdicionalMovimentacao, unidadeDeTrabalho, TipoDocumentoMovimento.Pagamento, tipoServico, 0, acrescimoDesconto.Justificativa.TipoMovimentoReversaoUsoJustificativa.PlanoDeContaDebito, acrescimoDesconto.Justificativa.TipoMovimentoReversaoUsoJustificativa.PlanoDeContaCredito, codigoTitulo, null, tituloBaixa.Pessoa, tituloBaixa.Pessoa?.GrupoPessoas ?? tituloBaixa.GrupoPessoas, null, null, null, null, null, acrescimoDesconto.MoedaCotacaoBancoCentral, acrescimoDesconto.DataBaseCRT, acrescimoDesconto.ValorMoedaCotacao, acrescimoDesconto.ValorOriginalMoedaEstrangeira, null, null, formaTitulo);
                            else
                                servProcessoMovimento.GerarMovimentacao(acrescimoDesconto.Justificativa.TipoMovimentoReversaoUsoJustificativa, tituloBaixa.DataBaixa.Value, acrescimoDesconto.Valor, tituloBaixa.Codigo.ToString(), obs + observacaoAdicionalMovimentacao, unidadeDeTrabalho, TipoDocumentoMovimento.Pagamento, tipoServico, 0, acrescimoDesconto.Justificativa.TipoMovimentoReversaoUsoJustificativa.PlanoDeContaDebito, acrescimoDesconto.Justificativa.TipoMovimentoReversaoUsoJustificativa.PlanoDeContaCredito, codigoTitulo, null, tituloBaixa.Pessoa, tituloBaixa.Pessoa?.GrupoPessoas ?? tituloBaixa.GrupoPessoas, null, null, null, null, null, acrescimoDesconto.MoedaCotacaoBancoCentral, acrescimoDesconto.DataBaseCRT, acrescimoDesconto.ValorMoedaCotacao, acrescimoDesconto.ValorOriginalMoedaEstrangeira, null, null, formaTitulo);
                        }
                        else
                        {
                            if (acrescimoDesconto.Justificativa.TipoMovimentoUsoJustificativa == null)
                            {
                                erro = "A justificativa " + acrescimoDesconto.Justificativa.Descricao + " não possui tipo de movimento vinculado para realizar a movimentação.";
                                return false;
                            }
                            if (acrescimoDesconto.Justificativa.TipoJustificativa == TipoJustificativa.Desconto)
                                servProcessoMovimento.GerarMovimentacao(acrescimoDesconto.Justificativa.TipoMovimentoUsoJustificativa, tituloBaixa.DataBaixa.Value, acrescimoDesconto.Valor, tituloBaixa.Codigo.ToString(), obs + observacaoAdicionalMovimentacao, unidadeDeTrabalho, TipoDocumentoMovimento.Pagamento, tipoServico, 0, acrescimoDesconto.Justificativa.TipoMovimentoUsoJustificativa.PlanoDeContaCredito, acrescimoDesconto.Justificativa.TipoMovimentoUsoJustificativa.PlanoDeContaDebito, codigoTitulo, null, tituloBaixa.Pessoa, tituloBaixa.Pessoa?.GrupoPessoas ?? tituloBaixa.GrupoPessoas, null, null, null, null, null, acrescimoDesconto.MoedaCotacaoBancoCentral, acrescimoDesconto.DataBaseCRT, acrescimoDesconto.ValorMoedaCotacao, acrescimoDesconto.ValorOriginalMoedaEstrangeira, null, null, formaTitulo);
                            else
                                servProcessoMovimento.GerarMovimentacao(acrescimoDesconto.Justificativa.TipoMovimentoUsoJustificativa, tituloBaixa.DataBaixa.Value, acrescimoDesconto.Valor, tituloBaixa.Codigo.ToString(), obs + observacaoAdicionalMovimentacao, unidadeDeTrabalho, TipoDocumentoMovimento.Pagamento, tipoServico, 0, acrescimoDesconto.Justificativa.TipoMovimentoUsoJustificativa.PlanoDeContaCredito, acrescimoDesconto.Justificativa.TipoMovimentoUsoJustificativa.PlanoDeContaDebito, codigoTitulo, null, tituloBaixa.Pessoa, tituloBaixa.Pessoa?.GrupoPessoas ?? tituloBaixa.GrupoPessoas, null, null, null, null, null, acrescimoDesconto.MoedaCotacaoBancoCentral, acrescimoDesconto.DataBaseCRT, acrescimoDesconto.ValorMoedaCotacao, acrescimoDesconto.ValorOriginalMoedaEstrangeira, null, null, formaTitulo);
                        }
                    }
                }

            }

            return true;
        }

        public bool GeraReverteMovimentacaoFinanceiraIndividual(out string erro, int codigoBaixa, Repositorio.UnitOfWork unidadeDeTrabalho, string stringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServico, bool reversao, Dominio.Entidades.Embarcador.Financeiro.PlanoConta planoBaixaTitulo, Dominio.Entidades.Embarcador.Financeiro.PlanoConta planoCreditoTitulo = null)
        {
            Repositorio.Embarcador.Financeiro.TituloBaixa repTituloBaixa = new Repositorio.Embarcador.Financeiro.TituloBaixa(unidadeDeTrabalho);
            Repositorio.Embarcador.Financeiro.PlanoConta repPlanoConta = new Repositorio.Embarcador.Financeiro.PlanoConta(unidadeDeTrabalho);
            Repositorio.Embarcador.Financeiro.TituloBaixaTipoPagamentoRecebimento repTituloBaixaTipoPagamentoRecebimento = new Repositorio.Embarcador.Financeiro.TituloBaixaTipoPagamentoRecebimento(unidadeDeTrabalho);
            Repositorio.Embarcador.Financeiro.TituloBaixaAgrupado repTituloBaixaAgrupado = new Repositorio.Embarcador.Financeiro.TituloBaixaAgrupado(unidadeDeTrabalho);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro repConfiguracaoFinanceiro = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro(unidadeDeTrabalho);
            Servicos.Embarcador.Financeiro.ProcessoMovimento servProcessoMovimento = new Servicos.Embarcador.Financeiro.ProcessoMovimento(unidadeDeTrabalho.StringConexao);

            erro = "";
            Dominio.Entidades.Embarcador.Financeiro.TituloBaixa tituloBaixa = repTituloBaixa.BuscarPorCodigo(codigoBaixa);
            List<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado> listaTituloBaixaAgrupado = repTituloBaixaAgrupado.BuscarPorBaixaTitulo(codigoBaixa);
            List<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaTipoPagamentoRecebimento> listaPagamentos = repTituloBaixaTipoPagamentoRecebimento.BuscarPorBaixaTitulo(codigoBaixa);
            Dominio.Entidades.Embarcador.Financeiro.PlanoConta creditoTitulo = null;
            Dominio.Entidades.Embarcador.Financeiro.PlanoConta planoBaixa = null;
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiro configuracaoFinanceiro = repConfiguracaoFinanceiro.BuscarConfiguracaoPadrao();

            string observacaoPadrao = string.Empty;
            string codigosTitulos = string.Empty;
            FormaTitulo formaTitulo = FormaTitulo.Outros;
            if (!string.IsNullOrWhiteSpace(tituloBaixa.Observacao))
                observacaoPadrao = " " + tituloBaixa.Observacao;
            if (tituloBaixa.Cheques?.Count > 0)
                observacaoPadrao += " Cheque(s) nº: " + string.Join(", ", tituloBaixa.Cheques.Select(o => o.Cheque.NumeroCheque));

            if (listaTituloBaixaAgrupado?.Count > 0)
            {
                foreach (Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado tituloBaixaAgrupado in listaTituloBaixaAgrupado)
                {
                    Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = tituloBaixaAgrupado.Titulo;
                    formaTitulo = titulo.FormaTitulo;

                    string observacaoAdicionalMovimentacao = observacaoPadrao + " Doc: " + titulo.NumeroDocumentoTituloOriginal + " - " + (titulo.Pessoa?.Nome ?? "");
                    if (string.IsNullOrWhiteSpace(codigosTitulos))
                        codigosTitulos = titulo.Codigo.ToString("D");
                    else
                        codigosTitulos += ", " + titulo.Codigo.ToString("D");

                    if (titulo.ValorPago > 0 && planoBaixaTitulo != null && (listaPagamentos == null || listaPagamentos.Count == 0))
                    {
                        creditoTitulo = BuscarContaCreditoTitulo(out erro, titulo, unidadeDeTrabalho, planoCreditoTitulo);
                        if (!string.IsNullOrWhiteSpace(erro) && titulo.ValorPago > 0)
                            return false;
                        else
                            planoCreditoTitulo = creditoTitulo;

                        if (planoCreditoTitulo == null)
                        {
                            erro = "Não foi possível localizar a conta de saída para realizar a movimentação financeira";
                            return false;
                        }
                        else
                        {
                            decimal valorOriginalMoedaEstrangeira = tituloBaixa.ValorMoedaCotacao > 0 ? Math.Round(titulo.ValorPago / tituloBaixa.ValorMoedaCotacao, 2) : 0;

                            if (reversao)
                            {
                                if (servProcessoMovimento.MovimentacaoFinanceiraJaConciliada(null, titulo.ValorPago, tituloBaixa.Codigo.ToString(), unidadeDeTrabalho, TipoDocumentoMovimento.Pagamento, planoBaixaTitulo, planoCreditoTitulo, 0))
                                {
                                    erro = "Não é possível reverter esta baixa pois já se encontra conciliada.";
                                    return false;
                                }
                                servProcessoMovimento.GerarMovimentacao(null, tituloBaixa.DataBaixa.Value, titulo.ValorPago, tituloBaixa.Codigo.ToString(), "REVERSÃO BAIXA DO TITULO A PAGAR." + observacaoAdicionalMovimentacao, unidadeDeTrabalho, TipoDocumentoMovimento.Pagamento, tipoServico, 0, planoCreditoTitulo, planoBaixaTitulo, titulo.Codigo, null, titulo.Pessoa, titulo.Pessoa?.GrupoPessoas ?? null, null, null, null, null, null, tituloBaixa.MoedaCotacaoBancoCentral, tituloBaixa.DataBaseCRT, tituloBaixa.ValorMoedaCotacao, valorOriginalMoedaEstrangeira, null, null, formaTitulo);
                            }
                            else
                                servProcessoMovimento.GerarMovimentacao(null, tituloBaixa.DataBaixa.Value, titulo.ValorPago, tituloBaixa.Codigo.ToString(), "BAIXA DO TITULO A PAGAR." + observacaoAdicionalMovimentacao, unidadeDeTrabalho, TipoDocumentoMovimento.Pagamento, tipoServico, 0, planoBaixaTitulo, planoCreditoTitulo, titulo.Codigo, null, titulo.Pessoa, titulo.Pessoa?.GrupoPessoas ?? null, null, null, null, null, null, tituloBaixa.MoedaCotacaoBancoCentral, tituloBaixa.DataBaseCRT, tituloBaixa.ValorMoedaCotacao, valorOriginalMoedaEstrangeira, null, null, formaTitulo);
                        }
                    }
                    else if (titulo.ValorPago > 0 && listaPagamentos.Count > 0)
                    {
                        List<Dominio.ObjetosDeValor.Embarcador.Financeiro.ContaCreditoBaixaTitulo> listaContas = new List<Dominio.ObjetosDeValor.Embarcador.Financeiro.ContaCreditoBaixaTitulo>();
                        List<Dominio.ObjetosDeValor.Embarcador.Financeiro.ContaCreditoBaixaTitulo> listaContasPagamento = new List<Dominio.ObjetosDeValor.Embarcador.Financeiro.ContaCreditoBaixaTitulo>();
                        bool achouConta = false;

                        creditoTitulo = BuscarContaCreditoTitulo(out erro, titulo, unidadeDeTrabalho);
                        if (!string.IsNullOrWhiteSpace(erro))
                            return false;

                        achouConta = false;
                        foreach (Dominio.ObjetosDeValor.Embarcador.Financeiro.ContaCreditoBaixaTitulo conta in listaContas)
                        {
                            if (conta.CodigoConta == creditoTitulo.Codigo)
                            {
                                conta.Valor += titulo.ValorPago;
                                achouConta = true;
                                break;
                            }
                        }
                        if (!achouConta)
                        {
                            listaContas.Add(new Dominio.ObjetosDeValor.Embarcador.Financeiro.ContaCreditoBaixaTitulo()
                            {
                                CodigoConta = creditoTitulo.Codigo,
                                Valor = titulo.ValorPago
                            });
                        }

                        foreach (Dominio.Entidades.Embarcador.Financeiro.TituloBaixaTipoPagamentoRecebimento pagamento in listaPagamentos)
                        {
                            listaContasPagamento.Add(new Dominio.ObjetosDeValor.Embarcador.Financeiro.ContaCreditoBaixaTitulo()
                            {
                                CodigoConta = pagamento.TipoPagamentoRecebimento.PlanoConta.Codigo,
                                Valor = pagamento.Valor,
                                ValorOriginal = pagamento.Valor,
                                TipoPagamentoRecebimento = pagamento.TipoPagamentoRecebimento,
                                MoedaCotacaoBancoCentral = pagamento.MoedaCotacaoBancoCentral,
                                DataBaseCRT = pagamento.DataBaseCRT,
                                ValorMoedaCotacao = pagamento.ValorMoedaCotacao
                            });
                        }

                        decimal somaValorListaPagamento = listaContasPagamento.Sum(o => o.Valor);
                        decimal somaValorListaContas = listaContas.Sum(o => o.Valor);

                        foreach (Dominio.ObjetosDeValor.Embarcador.Financeiro.ContaCreditoBaixaTitulo pagamento in listaContasPagamento)
                        {
                            if (pagamento.Valor > 0)
                            {
                                decimal valorAcumulado = 0;
                                foreach (var conta in listaContas)
                                {
                                    if (conta.Valor > 0 && pagamento.Valor > 0)
                                    {
                                        decimal valorBaixar = 0;
                                        if (conta.Valor > pagamento.Valor)
                                            valorBaixar = pagamento.Valor;
                                        else
                                            valorBaixar = conta.Valor;

                                        if (valorBaixar < 0)
                                            valorBaixar = 0;

                                        if (valorBaixar > 0)
                                        {
                                            conta.Valor -= valorBaixar;
                                            pagamento.Valor -= valorBaixar;
                                            valorAcumulado += valorBaixar;
                                            if (pagamento.ValorOriginal > valorAcumulado)
                                            {
                                                decimal diferenta = pagamento.ValorOriginal - valorAcumulado;
                                                if (diferenta > 0 && diferenta < (decimal)(0.10))
                                                    valorBaixar += diferenta;
                                            }

                                            decimal valorOriginalMoedaEstrangeira = pagamento.ValorMoedaCotacao > 0 ? Math.Round(valorBaixar / pagamento.ValorMoedaCotacao, 2) : 0;
                                            planoCreditoTitulo = repPlanoConta.BuscarPorCodigo(conta.CodigoConta);
                                            planoBaixa = repPlanoConta.BuscarPorCodigo(pagamento.CodigoConta);
                                            if (reversao)
                                            {
                                                if (servProcessoMovimento.MovimentacaoFinanceiraJaConciliada(null, valorBaixar, tituloBaixa.Codigo.ToString(), unidadeDeTrabalho, TipoDocumentoMovimento.Pagamento, planoBaixa, planoCreditoTitulo, 0))
                                                {
                                                    erro = "Não é possível reverter esta baixa pois já se encontra conciliada.";
                                                    return false;
                                                }
                                                servProcessoMovimento.GerarMovimentacao(null, tituloBaixa.DataBaixa.Value, valorBaixar, tituloBaixa.Codigo.ToString(), "REVERSÃO BAIXA DO TITULO A PAGAR." + observacaoAdicionalMovimentacao, unidadeDeTrabalho, TipoDocumentoMovimento.Pagamento, tipoServico, 0, planoCreditoTitulo, planoBaixa, titulo.Codigo, null, titulo.Pessoa, titulo.Pessoa?.GrupoPessoas ?? null, null, null, pagamento.TipoPagamentoRecebimento.Exportar ? pagamento.TipoPagamentoRecebimento.ContasExportacao.Where(o => o.Reversao == true).ToList() : null, tituloBaixa, TipoMovimentoExportacao.ReversaoPagamentoContratoFrete, pagamento.MoedaCotacaoBancoCentral, pagamento.DataBaseCRT, pagamento.ValorMoedaCotacao, valorOriginalMoedaEstrangeira, null, null, formaTitulo);
                                            }
                                            else
                                                servProcessoMovimento.GerarMovimentacao(null, tituloBaixa.DataBaixa.Value, valorBaixar, tituloBaixa.Codigo.ToString(), "BAIXA DO TITULO A PAGAR." + observacaoAdicionalMovimentacao, unidadeDeTrabalho, TipoDocumentoMovimento.Pagamento, tipoServico, 0, planoBaixa, planoCreditoTitulo, titulo.Codigo, null, titulo.Pessoa, titulo.Pessoa?.GrupoPessoas ?? null, null, null, pagamento.TipoPagamentoRecebimento.Exportar ? pagamento.TipoPagamentoRecebimento.ContasExportacao.Where(o => !o.Reversao.HasValue || o.Reversao == false).ToList() : null, tituloBaixa, TipoMovimentoExportacao.PagamentoContratoFrete, pagamento.MoedaCotacaoBancoCentral, pagamento.DataBaseCRT, pagamento.ValorMoedaCotacao, valorOriginalMoedaEstrangeira, null, null, formaTitulo);
                                        }
                                    }
                                }
                            }

                        }
                    }
                    else if (titulo.ValorPago > 0 && listaPagamentos != null && listaPagamentos.Count == 0 && tipoServico != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    {
                        erro = "Favor informe ao menos uma conta de pagamento para realizar a baixa do título";
                        return false;
                    }
                }

                if (tituloBaixa.Acrescimos?.Count > 0)
                {
                    if (configuracaoFinanceiro.RatearMovimentosDescontosAcrescimosBaixaTitulosPagar && listaTituloBaixaAgrupado.Count > 1)
                        GerarMovimentoFinanceiroAcrescimoDescontoComRateio(out erro, unidadeDeTrabalho, tituloBaixa, formaTitulo, tipoServico, listaTituloBaixaAgrupado, reversao);
                    else
                        GerarMovimentoFinanceiroAcrescimoDesconto(out erro, unidadeDeTrabalho, tituloBaixa, formaTitulo, tipoServico, reversao, codigosTitulos);
                }
            }

            return true;
        }

        public void GeraReverteMovimentacaoFinanceiraControleDespesas(int codigoBaixa, Repositorio.UnitOfWork unidadeDeTrabalho, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServico, bool reversao)
        {
            if (tipoServico != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                throw new ServicoException("Controle de Despesas não é permitido para o seu ambiente");

            Repositorio.Embarcador.Financeiro.TituloBaixa repTituloBaixa = new Repositorio.Embarcador.Financeiro.TituloBaixa(unidadeDeTrabalho);
            Repositorio.Embarcador.Financeiro.TituloBaixaAgrupado repTituloBaixaAgrupado = new Repositorio.Embarcador.Financeiro.TituloBaixaAgrupado(unidadeDeTrabalho);
            Repositorio.Embarcador.Financeiro.TituloBaixaTipoPagamentoRecebimento repTituloBaixaTipoPagamentoRecebimento = new Repositorio.Embarcador.Financeiro.TituloBaixaTipoPagamentoRecebimento(unidadeDeTrabalho);
            Repositorio.Embarcador.Financeiro.TituloCentroResultadoTipoDespesa repTituloCentroResultadoTipoDespesa = new Repositorio.Embarcador.Financeiro.TituloCentroResultadoTipoDespesa(unidadeDeTrabalho);
            Repositorio.Embarcador.Financeiro.PlanoConta repPlanoConta = new Repositorio.Embarcador.Financeiro.PlanoConta(unidadeDeTrabalho);

            Servicos.Embarcador.Financeiro.ProcessoMovimento servProcessoMovimento = new Servicos.Embarcador.Financeiro.ProcessoMovimento(unidadeDeTrabalho.StringConexao);

            Dominio.Entidades.Embarcador.Financeiro.TituloBaixa tituloBaixa = repTituloBaixa.BuscarPorCodigo(codigoBaixa);
            List<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado> listaTituloBaixaAgrupado = repTituloBaixaAgrupado.BuscarPorBaixaTitulo(codigoBaixa);
            List<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaTipoPagamentoRecebimento> listaPagamentos = repTituloBaixaTipoPagamentoRecebimento.BuscarPorBaixaTitulo(codigoBaixa);

            string observacaoPadrao = string.Empty;
            if (!string.IsNullOrWhiteSpace(tituloBaixa.Observacao))
                observacaoPadrao = " " + tituloBaixa.Observacao;
            if (tituloBaixa.Cheques?.Count > 0)
                observacaoPadrao += " Cheque(s) nº: " + string.Join(", ", tituloBaixa.Cheques.Select(o => o.Cheque.NumeroCheque));

            foreach (Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado tituloBaixaAgrupado in listaTituloBaixaAgrupado)
            {
                Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = tituloBaixaAgrupado.Titulo;
                FormaTitulo formaTitulo = titulo.FormaTitulo;

                string observacaoAdicionalMovimentacao = observacaoPadrao + " Doc: " + titulo.NumeroDocumentoTituloOriginal + " - " + (titulo.Pessoa?.Nome ?? "");

                List<Dominio.Entidades.Embarcador.Financeiro.TituloCentroResultadoTipoDespesa> centrosTiposDespesas = repTituloCentroResultadoTipoDespesa.BuscarPorTitulo(titulo.Codigo);
                if (centrosTiposDespesas.Count == 0)
                    throw new ServicoException($"O título {titulo.Codigo} não possui Centros de Resultado / Tipos de Despesa para a geração dos movimentos.");

                if (titulo.ValorPago > 0)
                {
                    List<Dominio.ObjetosDeValor.Embarcador.Financeiro.ContaCreditoBaixaTitulo> listaContas = new List<Dominio.ObjetosDeValor.Embarcador.Financeiro.ContaCreditoBaixaTitulo>();
                    List<Dominio.ObjetosDeValor.Embarcador.Financeiro.ContaCreditoBaixaTitulo> listaContasPagamento = new List<Dominio.ObjetosDeValor.Embarcador.Financeiro.ContaCreditoBaixaTitulo>();
                    bool achouConta = false;

                    string erro = "";
                    Dominio.Entidades.Embarcador.Financeiro.PlanoConta creditoTitulo = BuscarContaCreditoTitulo(out erro, titulo, unidadeDeTrabalho);
                    if (!string.IsNullOrWhiteSpace(erro))
                        throw new ServicoException(erro);

                    achouConta = false;
                    foreach (Dominio.ObjetosDeValor.Embarcador.Financeiro.ContaCreditoBaixaTitulo conta in listaContas)
                    {
                        if (conta.CodigoConta == creditoTitulo.Codigo)
                        {
                            conta.Valor += titulo.ValorPago;
                            achouConta = true;
                            break;
                        }
                    }
                    if (!achouConta)
                    {
                        listaContas.Add(new Dominio.ObjetosDeValor.Embarcador.Financeiro.ContaCreditoBaixaTitulo()
                        {
                            CodigoConta = creditoTitulo.Codigo,
                            Valor = titulo.ValorPago
                        });
                    }

                    foreach (Dominio.Entidades.Embarcador.Financeiro.TituloBaixaTipoPagamentoRecebimento pagamento in listaPagamentos)
                    {
                        listaContasPagamento.Add(new Dominio.ObjetosDeValor.Embarcador.Financeiro.ContaCreditoBaixaTitulo()
                        {
                            CodigoConta = pagamento.TipoPagamentoRecebimento.PlanoConta.Codigo,
                            Valor = pagamento.Valor,
                            ValorOriginal = pagamento.Valor,
                            TipoPagamentoRecebimento = pagamento.TipoPagamentoRecebimento,
                            MoedaCotacaoBancoCentral = pagamento.MoedaCotacaoBancoCentral,
                            DataBaseCRT = pagamento.DataBaseCRT,
                            ValorMoedaCotacao = pagamento.ValorMoedaCotacao
                        });
                    }

                    decimal somaValorListaPagamento = listaContasPagamento.Sum(o => o.Valor);
                    decimal somaValorListaContas = listaContas.Sum(o => o.Valor);

                    foreach (Dominio.ObjetosDeValor.Embarcador.Financeiro.ContaCreditoBaixaTitulo pagamento in listaContasPagamento)
                    {
                        if (pagamento.Valor == 0)
                            continue;

                        decimal valorAcumulado = 0;
                        foreach (Dominio.ObjetosDeValor.Embarcador.Financeiro.ContaCreditoBaixaTitulo conta in listaContas)
                        {
                            if (conta.Valor == 0 || pagamento.Valor == 0)
                                continue;

                            decimal valorBaixar = 0;
                            if (conta.Valor > pagamento.Valor)
                                valorBaixar = pagamento.Valor;
                            else
                                valorBaixar = conta.Valor;

                            if (valorBaixar < 0)
                                valorBaixar = 0;

                            if (valorBaixar == 0)
                                continue;

                            conta.Valor -= valorBaixar;
                            pagamento.Valor -= valorBaixar;
                            valorAcumulado += valorBaixar;
                            if (pagamento.ValorOriginal > valorAcumulado)
                            {
                                decimal diferenta = pagamento.ValorOriginal - valorAcumulado;
                                if (diferenta > 0 && diferenta < (decimal)(0.10))
                                    valorBaixar += diferenta;
                            }

                            Dominio.Entidades.Embarcador.Financeiro.PlanoConta planoCreditoTitulo = repPlanoConta.BuscarPorCodigo(conta.CodigoConta);
                            Dominio.Entidades.Embarcador.Financeiro.PlanoConta planoBaixa = repPlanoConta.BuscarPorCodigo(pagamento.CodigoConta);

                            foreach (Dominio.Entidades.Embarcador.Financeiro.TituloCentroResultadoTipoDespesa centroResultadoTipoDespesa in centrosTiposDespesas)
                            {
                                decimal valorPorCentroTipoDespesa = Math.Round(valorBaixar * (centroResultadoTipoDespesa.Percentual / 100), 2);
                                decimal valorOriginalMoedaEstrangeira = pagamento.ValorMoedaCotacao > 0 ? Math.Round(valorPorCentroTipoDespesa / pagamento.ValorMoedaCotacao, 2) : 0;

                                if (reversao)
                                {
                                    if (servProcessoMovimento.MovimentacaoFinanceiraJaConciliada(null, valorPorCentroTipoDespesa, tituloBaixa.Codigo.ToString(), unidadeDeTrabalho, TipoDocumentoMovimento.Pagamento, planoBaixa, planoCreditoTitulo, 0))
                                        throw new ServicoException("Não é possível reverter esta baixa, pois já se encontra conciliada.");

                                    servProcessoMovimento.GerarMovimentacao(null, tituloBaixa.DataBaixa.Value, valorPorCentroTipoDespesa, tituloBaixa.Codigo.ToString(), "REVERSÃO BAIXA DO TITULO A PAGAR." + observacaoAdicionalMovimentacao, unidadeDeTrabalho, TipoDocumentoMovimento.Pagamento, tipoServico, 0, planoCreditoTitulo, planoBaixa, titulo.Codigo, null, titulo.Pessoa, titulo.Pessoa?.GrupoPessoas ?? null, null, centroResultadoTipoDespesa.CentroResultado, pagamento.TipoPagamentoRecebimento.Exportar ? pagamento.TipoPagamentoRecebimento.ContasExportacao.Where(o => o.Reversao == true).ToList() : null, tituloBaixa, TipoMovimentoExportacao.ReversaoPagamentoContratoFrete, pagamento.MoedaCotacaoBancoCentral, pagamento.DataBaseCRT, pagamento.ValorMoedaCotacao, valorOriginalMoedaEstrangeira, centroResultadoTipoDespesa.TipoDespesaFinanceira, null, formaTitulo);
                                }
                                else
                                    servProcessoMovimento.GerarMovimentacao(null, tituloBaixa.DataBaixa.Value, valorPorCentroTipoDespesa, tituloBaixa.Codigo.ToString(), "BAIXA DO TITULO A PAGAR." + observacaoAdicionalMovimentacao, unidadeDeTrabalho, TipoDocumentoMovimento.Pagamento, tipoServico, 0, planoBaixa, planoCreditoTitulo, titulo.Codigo, null, titulo.Pessoa, titulo.Pessoa?.GrupoPessoas ?? null, null, centroResultadoTipoDespesa.CentroResultado, pagamento.TipoPagamentoRecebimento.Exportar ? pagamento.TipoPagamentoRecebimento.ContasExportacao.Where(o => !o.Reversao.HasValue || o.Reversao == false).ToList() : null, tituloBaixa, TipoMovimentoExportacao.PagamentoContratoFrete, pagamento.MoedaCotacaoBancoCentral, pagamento.DataBaseCRT, pagamento.ValorMoedaCotacao, valorOriginalMoedaEstrangeira, centroResultadoTipoDespesa.TipoDespesaFinanceira, null, formaTitulo);
                            }
                        }
                    }
                }

                if (tituloBaixa.Acrescimos?.Count > 0)
                {
                    string obs = "";
                    foreach (Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAcrescimo acrescimoDesconto in tituloBaixa.Acrescimos)
                    {
                        if (!acrescimoDesconto.Justificativa.GerarMovimentoAutomatico)
                            continue;

                        if (reversao)
                            obs = "REVERSÃO ";
                        else
                            obs = "";

                        foreach (Dominio.Entidades.Embarcador.Financeiro.TituloCentroResultadoTipoDespesa centroResultadoTipoDespesa in centrosTiposDespesas)
                        {
                            decimal valorAcrescimoPorCentroTipoDespesa = 0;
                            decimal valorDescontoPorCentroTipoDespesa = 0;
                            decimal valorOriginalMoedaEstrangeira = 0;
                            if (acrescimoDesconto.Justificativa.TipoJustificativa == TipoJustificativa.Acrescimo)
                            {
                                obs += "ACRESCIMO";
                                valorAcrescimoPorCentroTipoDespesa = titulo.Acrescimo > 0 ? Math.Round(titulo.Acrescimo * (centroResultadoTipoDespesa.Percentual / 100), 2) : 0;
                                valorOriginalMoedaEstrangeira = acrescimoDesconto.ValorMoedaCotacao > 0 ? Math.Round(valorAcrescimoPorCentroTipoDespesa / acrescimoDesconto.ValorMoedaCotacao, 2) : 0;
                            }
                            else
                            {
                                obs += "DESCONTO";
                                valorDescontoPorCentroTipoDespesa = titulo.Desconto > 0 ? Math.Round(titulo.Desconto * (centroResultadoTipoDespesa.Percentual / 100), 2) : 0;
                                valorOriginalMoedaEstrangeira = acrescimoDesconto.ValorMoedaCotacao > 0 ? Math.Round(valorDescontoPorCentroTipoDespesa / acrescimoDesconto.ValorMoedaCotacao, 2) : 0;
                            }

                            obs += " NA BAIXA DO TITULO " + titulo.Codigo.ToString("D") + ".";

                            if (reversao)
                            {
                                if (acrescimoDesconto.Justificativa.TipoMovimentoReversaoUsoJustificativa == null)
                                    throw new ServicoException("A justificativa " + acrescimoDesconto.Justificativa.Descricao + " não possui tipo de movimento vinculado para a sua reversão.");
                                if (servProcessoMovimento.MovimentacaoFinanceiraJaConciliada(acrescimoDesconto.Justificativa.TipoMovimentoReversaoUsoJustificativa, valorAcrescimoPorCentroTipoDespesa, tituloBaixa.Codigo.ToString(), unidadeDeTrabalho, TipoDocumentoMovimento.Pagamento, null, null, 0))
                                    throw new ServicoException("Não é possível reverter esta baixa, pois já se encontra conciliada.");

                                if (acrescimoDesconto.Justificativa.TipoJustificativa == TipoJustificativa.Desconto)
                                    servProcessoMovimento.GerarMovimentacao(acrescimoDesconto.Justificativa.TipoMovimentoReversaoUsoJustificativa, tituloBaixa.DataBaixa.Value, valorDescontoPorCentroTipoDespesa, tituloBaixa.Codigo.ToString(), obs + observacaoAdicionalMovimentacao, unidadeDeTrabalho, TipoDocumentoMovimento.Pagamento, tipoServico, 0, null, null, titulo.Codigo, null, titulo.Pessoa, titulo.Pessoa?.GrupoPessoas ?? null, null, centroResultadoTipoDespesa.CentroResultado, null, null, null, acrescimoDesconto.MoedaCotacaoBancoCentral, acrescimoDesconto.DataBaseCRT, acrescimoDesconto.ValorMoedaCotacao, valorOriginalMoedaEstrangeira, centroResultadoTipoDespesa.TipoDespesaFinanceira, null, formaTitulo);
                                else
                                    servProcessoMovimento.GerarMovimentacao(acrescimoDesconto.Justificativa.TipoMovimentoReversaoUsoJustificativa, tituloBaixa.DataBaixa.Value, valorAcrescimoPorCentroTipoDespesa, tituloBaixa.Codigo.ToString(), obs + observacaoAdicionalMovimentacao, unidadeDeTrabalho, TipoDocumentoMovimento.Pagamento, tipoServico, 0, null, null, titulo.Codigo, null, titulo.Pessoa, titulo.Pessoa?.GrupoPessoas ?? null, null, centroResultadoTipoDespesa.CentroResultado, null, null, null, acrescimoDesconto.MoedaCotacaoBancoCentral, acrescimoDesconto.DataBaseCRT, acrescimoDesconto.ValorMoedaCotacao, valorOriginalMoedaEstrangeira, centroResultadoTipoDespesa.TipoDespesaFinanceira, null, formaTitulo);
                            }
                            else
                            {
                                if (acrescimoDesconto.Justificativa.TipoMovimentoUsoJustificativa == null)
                                    throw new ServicoException("A justificativa " + acrescimoDesconto.Justificativa.Descricao + " não possui tipo de movimento vinculado para realizar a movimentação.");

                                if (acrescimoDesconto.Justificativa.TipoJustificativa == TipoJustificativa.Desconto)
                                    servProcessoMovimento.GerarMovimentacao(acrescimoDesconto.Justificativa.TipoMovimentoUsoJustificativa, tituloBaixa.DataBaixa.Value, valorDescontoPorCentroTipoDespesa, tituloBaixa.Codigo.ToString(), obs + observacaoAdicionalMovimentacao, unidadeDeTrabalho, TipoDocumentoMovimento.Pagamento, tipoServico, 0, acrescimoDesconto.Justificativa.TipoMovimentoUsoJustificativa.PlanoDeContaCredito, acrescimoDesconto.Justificativa.TipoMovimentoUsoJustificativa.PlanoDeContaDebito, titulo.Codigo, null, titulo.Pessoa, titulo.Pessoa?.GrupoPessoas ?? null, null, centroResultadoTipoDespesa.CentroResultado, null, null, null, acrescimoDesconto.MoedaCotacaoBancoCentral, acrescimoDesconto.DataBaseCRT, acrescimoDesconto.ValorMoedaCotacao, valorOriginalMoedaEstrangeira, centroResultadoTipoDespesa.TipoDespesaFinanceira, null, formaTitulo);
                                else
                                    servProcessoMovimento.GerarMovimentacao(acrescimoDesconto.Justificativa.TipoMovimentoUsoJustificativa, tituloBaixa.DataBaixa.Value, valorAcrescimoPorCentroTipoDespesa, tituloBaixa.Codigo.ToString(), obs + observacaoAdicionalMovimentacao, unidadeDeTrabalho, TipoDocumentoMovimento.Pagamento, tipoServico, 0, acrescimoDesconto.Justificativa.TipoMovimentoUsoJustificativa.PlanoDeContaCredito, acrescimoDesconto.Justificativa.TipoMovimentoUsoJustificativa.PlanoDeContaDebito, titulo.Codigo, null, titulo.Pessoa, titulo.Pessoa?.GrupoPessoas ?? null, null, centroResultadoTipoDespesa.CentroResultado, null, null, null, acrescimoDesconto.MoedaCotacaoBancoCentral, acrescimoDesconto.DataBaseCRT, acrescimoDesconto.ValorMoedaCotacao, valorOriginalMoedaEstrangeira, centroResultadoTipoDespesa.TipoDespesaFinanceira, null, formaTitulo);
                            }
                        }
                    }
                }
            }
        }

        public Dominio.Entidades.Embarcador.Financeiro.Titulo RetornaTituloPai(Dominio.Entidades.Embarcador.Financeiro.Titulo titulo)
        {
            if (titulo != null && titulo.TituloBaixaNegociacao != null && titulo.TituloBaixaNegociacao.TituloBaixa != null && titulo.TituloBaixaNegociacao.TituloBaixa.TitulosAgrupados[0] != null && titulo.TituloBaixaNegociacao.TituloBaixa.TitulosAgrupados[0].Titulo != null)
                return titulo.TituloBaixaNegociacao.TituloBaixa.TitulosAgrupados[0].Titulo;
            else
                return null;
        }

        #endregion

        #region Métodos Privados

        private Dominio.Entidades.Embarcador.Financeiro.PlanoConta BuscarContaCreditoTitulo(out string erro, Dominio.Entidades.Embarcador.Financeiro.Titulo titulo, Repositorio.UnitOfWork unidadeDeTrabalho, Dominio.Entidades.Embarcador.Financeiro.PlanoConta planoCreditoTitulo = null)
        {
            erro = string.Empty;
            Dominio.Entidades.Embarcador.Financeiro.PlanoConta creditoTitulo = null;
            Repositorio.Embarcador.Financeiro.MovimentoFinanceiro repMovimentoFinanceiro = new Repositorio.Embarcador.Financeiro.MovimentoFinanceiro(unidadeDeTrabalho);
            Repositorio.Embarcador.Financeiro.TipoMovimento repositorioTipoMovimento = new Repositorio.Embarcador.Financeiro.TipoMovimento(unidadeDeTrabalho);

            if (titulo.TituloBaixaNegociacao == null)
            {
                if (titulo.ContratoFrete != null)
                    creditoTitulo = repMovimentoFinanceiro.BuscarContaCreditoTituloContratoFrete(titulo.ContratoFrete.Codigo);
                else if (titulo.AdiantamentoFornecedor && titulo.TipoMovimento != null && titulo.TipoMovimento.PlanoDeContaCredito != null)
                    creditoTitulo = titulo.TipoMovimento.PlanoDeContaCredito;
                else
                    creditoTitulo = repMovimentoFinanceiro.BuscarContaCreditoTitulo(titulo.Codigo);

                if (creditoTitulo == null)
                {
                    erro = "Título de código " + titulo.Codigo.ToString() + " não possui conta de saída para realizar a baixa";
                    return null;
                }
                else if (planoCreditoTitulo != null)
                {
                    if (planoCreditoTitulo != creditoTitulo)
                    {
                        erro = "Os títulos selecionados não possuem as mesmas contas de saída";
                        return null;
                    }
                    else
                        planoCreditoTitulo = creditoTitulo;
                }
                else
                    planoCreditoTitulo = creditoTitulo;
            }
            else
            {
                if (titulo.TituloBaixaNegociacao.TituloBaixa.TitulosAgrupados != null && titulo.TituloBaixaNegociacao.TituloBaixa.TitulosAgrupados.Count() > 0 && titulo.TituloBaixaNegociacao.TituloBaixa.TitulosAgrupados[0].Titulo != null)
                {
                    Dominio.Entidades.Embarcador.Financeiro.Titulo tituloPai = titulo.TituloBaixaNegociacao.TituloBaixa.TitulosAgrupados[0].Titulo;
                    Dominio.Entidades.Embarcador.Financeiro.Titulo tituloPaiNovo = null;
                    tituloPaiNovo = RetornaTituloPai(titulo.TituloBaixaNegociacao.TituloBaixa.TitulosAgrupados[0].Titulo);
                    if (tituloPaiNovo != null)
                    {
                        tituloPai = tituloPaiNovo;
                        while (tituloPaiNovo != null)
                        {
                            tituloPaiNovo = RetornaTituloPai(tituloPai);
                            if (tituloPaiNovo != null)
                                tituloPai = tituloPaiNovo;
                        }
                    }

                    if (tituloPai.AdiantamentoFornecedor && tituloPai.TipoMovimento != null && tituloPai.TipoMovimento.PlanoDeContaCredito != null)
                        creditoTitulo = tituloPai.TipoMovimento.PlanoDeContaCredito;
                    else
                        creditoTitulo = repMovimentoFinanceiro.BuscarContaCreditoTitulo(tituloPai.Codigo);

                    if (creditoTitulo == null)
                    {
                        if (tituloPai.ContratoFrete != null)
                            creditoTitulo = repMovimentoFinanceiro.BuscarContaCreditoTituloContratoFrete(tituloPai.ContratoFrete.Codigo);
                    }

                    if (creditoTitulo == null)
                    {
                        erro = "Existem títulos gerados de negociações anteriores que não possuem conta de saída para realizar a baixa";
                        return null;
                    }
                    else if (planoCreditoTitulo != null)
                    {
                        if (planoCreditoTitulo != creditoTitulo)
                        {
                            erro = "Existem títulos gerados de negociações anteriores que não possuem as mesmas contas de saída";
                            return null;
                        }
                        else
                            planoCreditoTitulo = creditoTitulo;
                    }
                    else
                        planoCreditoTitulo = creditoTitulo;

                }
            }
            return creditoTitulo;
        }

        private bool GerarMovimentoFinanceiroAcrescimoDesconto(out string erro, Repositorio.UnitOfWork unidadeDeTrabalho, Dominio.Entidades.Embarcador.Financeiro.TituloBaixa tituloBaixa, FormaTitulo formaTitulo, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServico, bool reversao, string codigosTitulos)
        {
            Servicos.Embarcador.Financeiro.ProcessoMovimento serProcessoMovimento = new Servicos.Embarcador.Financeiro.ProcessoMovimento(unidadeDeTrabalho.StringConexao);
            bool acrescimoLancado = false;
            bool descontoLancado = false;
            string obs = "";
            Dominio.Entidades.Cliente fornecedor = tituloBaixa.Pessoa != null ? tituloBaixa.Pessoa : tituloBaixa.TitulosAgrupados != null && tituloBaixa.TitulosAgrupados.Count > 0 ? tituloBaixa.TitulosAgrupados.FirstOrDefault()?.Titulo?.Pessoa : null;
            foreach (Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAcrescimo acrescimoDesconto in tituloBaixa.Acrescimos)
            {
                if (acrescimoDesconto.Justificativa.GerarMovimentoAutomatico)
                {
                    if (reversao)
                        obs = "REVERSÃO ";
                    else
                        obs = "";

                    decimal valorOriginalMoedaEstrangeira = acrescimoDesconto.ValorMoedaCotacao > 0 ? Math.Round(acrescimoDesconto.Valor / acrescimoDesconto.ValorMoedaCotacao, 2) : 0;
                    if (acrescimoDesconto.Justificativa.TipoJustificativa == TipoJustificativa.Acrescimo)
                        obs += "ACRESCIMO";
                    else
                        obs += "DESCONTO";

                    obs += " NA BAIXA DO TITULO " + codigosTitulos + ".";

                    if (reversao)
                    {
                        if (acrescimoDesconto.Justificativa.TipoMovimentoReversaoUsoJustificativa == null)
                        {
                            erro = "A justificativa " + acrescimoDesconto.Justificativa.Descricao + " não possui tipo de movimento vinculado para a sua reversão.";
                            return false;
                        }

                        if (serProcessoMovimento.MovimentacaoFinanceiraJaConciliada(acrescimoDesconto.Justificativa.TipoMovimentoReversaoUsoJustificativa, acrescimoDesconto.Valor, tituloBaixa.Codigo.ToString(), unidadeDeTrabalho, TipoDocumentoMovimento.Pagamento, null, null, 0))
                        {
                            erro = "Não é possível reverter esta baixa pois já se encontra conciliada.";
                            return false;
                        }

                        if (acrescimoDesconto.Justificativa.TipoJustificativa == TipoJustificativa.Desconto && !descontoLancado)
                        {
                            descontoLancado = true;
                            serProcessoMovimento.GerarMovimentacao(acrescimoDesconto.Justificativa.TipoMovimentoReversaoUsoJustificativa, tituloBaixa.DataBaixa.Value, acrescimoDesconto.Valor, tituloBaixa.Codigo.ToString(), obs, unidadeDeTrabalho, TipoDocumentoMovimento.Pagamento, tipoServico, 0, null, null, 0, null, fornecedor, fornecedor?.GrupoPessoas ?? null, null, null, null, null, null, acrescimoDesconto.MoedaCotacaoBancoCentral, acrescimoDesconto.DataBaseCRT, acrescimoDesconto.ValorMoedaCotacao, valorOriginalMoedaEstrangeira, null, null, formaTitulo);
                        }
                        else if (!acrescimoLancado)
                        {
                            acrescimoLancado = true;
                            serProcessoMovimento.GerarMovimentacao(acrescimoDesconto.Justificativa.TipoMovimentoReversaoUsoJustificativa, tituloBaixa.DataBaixa.Value, acrescimoDesconto.Valor, tituloBaixa.Codigo.ToString(), obs, unidadeDeTrabalho, TipoDocumentoMovimento.Pagamento, tipoServico, 0, null, null, 0, null, fornecedor, fornecedor?.GrupoPessoas ?? null, null, null, null, null, null, acrescimoDesconto.MoedaCotacaoBancoCentral, acrescimoDesconto.DataBaseCRT, acrescimoDesconto.ValorMoedaCotacao, valorOriginalMoedaEstrangeira, null, null, formaTitulo);
                        }
                    }
                    else
                    {
                        if (acrescimoDesconto.Justificativa.TipoMovimentoUsoJustificativa == null)
                        {
                            erro = "A justificativa " + acrescimoDesconto.Justificativa.Descricao + " não possui tipo de movimento vinculado para realizar a movimentação.";
                            return false;
                        }

                        if (acrescimoDesconto.Justificativa.TipoJustificativa == TipoJustificativa.Desconto && !descontoLancado)
                        {
                            descontoLancado = true;
                            serProcessoMovimento.GerarMovimentacao(acrescimoDesconto.Justificativa.TipoMovimentoUsoJustificativa, tituloBaixa.DataBaixa.Value, acrescimoDesconto.Valor, tituloBaixa.Codigo.ToString(), obs, unidadeDeTrabalho, TipoDocumentoMovimento.Pagamento, tipoServico, 0, acrescimoDesconto.Justificativa.TipoMovimentoUsoJustificativa.PlanoDeContaCredito, acrescimoDesconto.Justificativa.TipoMovimentoUsoJustificativa.PlanoDeContaDebito, 0, null, fornecedor, fornecedor?.GrupoPessoas ?? null, null, null, null, null, null, acrescimoDesconto.MoedaCotacaoBancoCentral, acrescimoDesconto.DataBaseCRT, acrescimoDesconto.ValorMoedaCotacao, valorOriginalMoedaEstrangeira, null, null, formaTitulo);
                        }
                        else if (!acrescimoLancado)
                        {
                            acrescimoLancado = true;
                            serProcessoMovimento.GerarMovimentacao(acrescimoDesconto.Justificativa.TipoMovimentoUsoJustificativa, tituloBaixa.DataBaixa.Value, acrescimoDesconto.Valor, tituloBaixa.Codigo.ToString(), obs, unidadeDeTrabalho, TipoDocumentoMovimento.Pagamento, tipoServico, 0, acrescimoDesconto.Justificativa.TipoMovimentoUsoJustificativa.PlanoDeContaCredito, acrescimoDesconto.Justificativa.TipoMovimentoUsoJustificativa.PlanoDeContaDebito, 0, null, fornecedor, fornecedor?.GrupoPessoas ?? null, null, null, null, null, null, acrescimoDesconto.MoedaCotacaoBancoCentral, acrescimoDesconto.DataBaseCRT, acrescimoDesconto.ValorMoedaCotacao, valorOriginalMoedaEstrangeira, null, null, formaTitulo);
                        }
                    }
                }
            }

            erro = "";
            return true;
        }

        private bool GerarMovimentoFinanceiroAcrescimoDescontoComRateio(out string erro, Repositorio.UnitOfWork unidadeDeTrabalho, Dominio.Entidades.Embarcador.Financeiro.TituloBaixa tituloBaixa, FormaTitulo formaTitulo, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServico, List<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado> listaTituloBaixaAgrupado, bool reversao)
        {
            Repositorio.Embarcador.Financeiro.TituloBaixaAcrescimo repTituloBaixaAcrescimo = new Repositorio.Embarcador.Financeiro.TituloBaixaAcrescimo(unidadeDeTrabalho);
            Servicos.Embarcador.Financeiro.ProcessoMovimento serProcessoMovimento = new Servicos.Embarcador.Financeiro.ProcessoMovimento(unidadeDeTrabalho.StringConexao);

            if (listaTituloBaixaAgrupado.Count == 0)
            {
                erro = "";
                return true;
            }

            decimal valorTotalTitulos = 0;
            foreach (Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado tituloAgrupado in listaTituloBaixaAgrupado)
                valorTotalTitulos += tituloAgrupado.Titulo.ValorOriginal;

            foreach (Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado tituloAgrupado in listaTituloBaixaAgrupado)
            {
                List<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAcrescimo> titulosBaixaAcrescimo = repTituloBaixaAcrescimo.BuscarPorBaixaTitulo(tituloAgrupado.TituloBaixa.Codigo);

                foreach (Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAcrescimo tituloBaixaAcrescimo in titulosBaixaAcrescimo)
                {
                    if (!tituloBaixaAcrescimo.Justificativa.GerarMovimentoAutomatico)
                        continue;

                    string obs = reversao ? "REVERSÃO " : "";
                    obs += tituloBaixaAcrescimo.Justificativa.TipoJustificativa == TipoJustificativa.Acrescimo ? "ACRÉSCIMO" : "DESCONTO";
                    obs += " NA BAIXA DO TITULO " + tituloAgrupado.Titulo.Codigo + ".";
                    decimal valorOriginalMoedaEstrangeira = tituloBaixaAcrescimo.ValorMoedaCotacao > 0 ? Math.Round(tituloBaixaAcrescimo.Valor / tituloBaixaAcrescimo.ValorMoedaCotacao, 2) : 0;
                    decimal valorDesconto = Math.Round(tituloBaixaAcrescimo.Justificativa.TipoJustificativa == TipoJustificativa.Desconto ? tituloBaixaAcrescimo.Valor : 0, 2);
                    decimal valorAcrescimo = Math.Round(tituloBaixaAcrescimo.Justificativa.TipoJustificativa == TipoJustificativa.Acrescimo ? tituloBaixaAcrescimo.Valor : 0, 2);
                    decimal totalDesconto = 0, totalAcrescimo = 0;
                    decimal valorMovimento = 0;

                    if (valorDesconto > 0)
                        totalDesconto += Math.Round(((valorDesconto * tituloAgrupado.Titulo.ValorOriginal) / valorTotalTitulos), 2);

                    if (valorAcrescimo > 0)
                        totalAcrescimo += Math.Round(((valorAcrescimo * tituloAgrupado.Titulo.ValorOriginal) / valorTotalTitulos), 2);

                    valorMovimento = tituloBaixaAcrescimo.Justificativa.TipoJustificativa == TipoJustificativa.Acrescimo ? totalAcrescimo : totalDesconto;

                    if (reversao)
                    {
                        if (serProcessoMovimento.MovimentacaoFinanceiraJaConciliada(tituloBaixaAcrescimo.Justificativa.TipoMovimentoReversaoUsoJustificativa, valorMovimento, tituloBaixa.Codigo.ToString(), unidadeDeTrabalho, TipoDocumentoMovimento.Pagamento, null, null, 0))
                        {
                            erro = "Não é possível reverter esta baixa pois já se encontra conciliada.";
                            return false;
                        }

                        if (tituloBaixaAcrescimo.Justificativa.TipoMovimentoReversaoUsoJustificativa == null)
                        {
                            erro = "A justificativa " + tituloBaixaAcrescimo.Justificativa.Descricao + " não possui tipo de movimento vinculado para a sua reversão.";
                            return false;
                        }
                        else
                            serProcessoMovimento.GerarMovimentacao(tituloBaixaAcrescimo.Justificativa.TipoMovimentoReversaoUsoJustificativa, tituloBaixa.DataBaixa.Value, valorMovimento, tituloBaixa.Codigo.ToString(), obs, unidadeDeTrabalho, TipoDocumentoMovimento.Pagamento, tipoServico, 0, null, null, 0, null, tituloAgrupado.Titulo.Fornecedor, tituloAgrupado.Titulo.Fornecedor?.GrupoPessoas ?? null, null, null, null, null, null, tituloBaixaAcrescimo.MoedaCotacaoBancoCentral, tituloBaixaAcrescimo.DataBaseCRT, tituloBaixaAcrescimo.ValorMoedaCotacao, valorOriginalMoedaEstrangeira, null, null, formaTitulo);
                    }
                    else
                    {
                        if (tituloBaixaAcrescimo.Justificativa.TipoMovimentoUsoJustificativa == null)
                        {
                            erro = "A justificativa " + tituloBaixaAcrescimo.Justificativa.Descricao + " não possui tipo de movimento vinculado para realizar a movimentação.";
                            return false;
                        }
                        else
                            serProcessoMovimento.GerarMovimentacao(tituloBaixaAcrescimo.Justificativa.TipoMovimentoUsoJustificativa, tituloBaixa.DataBaixa.Value, valorMovimento, tituloBaixa.Codigo.ToString(), obs, unidadeDeTrabalho, TipoDocumentoMovimento.Pagamento, tipoServico, 0, tituloBaixaAcrescimo.Justificativa.TipoMovimentoUsoJustificativa.PlanoDeContaCredito, tituloBaixaAcrescimo.Justificativa.TipoMovimentoUsoJustificativa.PlanoDeContaDebito, 0, null, tituloAgrupado.Titulo.Fornecedor, tituloAgrupado.Titulo.Fornecedor?.GrupoPessoas ?? null, null, null, null, null, null, tituloBaixaAcrescimo.MoedaCotacaoBancoCentral, tituloBaixaAcrescimo.DataBaseCRT, tituloBaixaAcrescimo.ValorMoedaCotacao, valorOriginalMoedaEstrangeira, null, null, formaTitulo);
                    }
                }
            }

            erro = "";
            return true;
        }

        #endregion Métodos Privados
    }
}
