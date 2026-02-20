using Dominio.Entidades.Embarcador.Financeiro;
using Dominio.Interfaces.Database;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Repositorio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Servicos.Embarcador.Financeiro
{
    public class BaixaTituloReceber : ServicoBase
    {
        #region Construtores
        
        public BaixaTituloReceber(Repositorio.UnitOfWork unitOfWork, CancellationToken cancelationToken = default) : base(unitOfWork, cancelationToken) { }
        #endregion

        #region Propriedades Privadas

        private static Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraBaixaTituloReceberMoeda ConfiguracaoFinanceiraBaixaTituloReceberMoeda;

        #endregion

        #region Métodos Públicos

        public dynamic RetornaObjetoCompletoTitulo(int codigoTituloBaixa, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unidadeDeTrabalho);
            Repositorio.Embarcador.Financeiro.TituloBaixa repTituloBaixa = new Repositorio.Embarcador.Financeiro.TituloBaixa(unidadeDeTrabalho);
            Repositorio.Embarcador.Financeiro.TituloBaixaNegociacao repTituloBaixaNegociacao = new Repositorio.Embarcador.Financeiro.TituloBaixaNegociacao(unidadeDeTrabalho);

            Dominio.Entidades.Embarcador.Financeiro.TituloBaixa tituloBaixa = repTituloBaixa.BuscarPorCodigo(codigoTituloBaixa);
            return new
            {
                CodigoFatura = repTituloBaixa.CodigoFaturaBaixaAReceber(codigoTituloBaixa),
                NumeroFatura = repTituloBaixa.NumeroFaturaBaixaAReceber(codigoTituloBaixa).ToString("n0"),
                Codigo = tituloBaixa != null ? tituloBaixa.Codigo : 0,
                Etapa = tituloBaixa != null ? tituloBaixa.SituacaoBaixaTitulo : Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoBaixaTitulo.Iniciada,
                ParcelaTitulo = 1.ToString("n0"),
                ValorPendenteTitulo = tituloBaixa.ValorPendente.ToString("n2"),
                ValorBaixado = tituloBaixa != null ? tituloBaixa.Valor.ToString("n2") : 0.ToString("n2"),
                ListaParcelasNegociacao = tituloBaixa != null ? repTituloBaixaNegociacao.BuscarPorBaixaTitulo(tituloBaixa.Codigo) : null,
                NumeroTitulo = tituloBaixa.CodigosTitulos,
                DataBaixa = tituloBaixa != null ? tituloBaixa.DataBaixa.Value.ToString("dd/MM/yyyy") : string.Empty,
                DataBase = tituloBaixa != null ? tituloBaixa.DataBase.Value.ToString("dd/MM/yyyy") : string.Empty,
                Observacao = tituloBaixa != null ? tituloBaixa.Observacao : string.Empty,
                DescricaoSituacao = tituloBaixa != null ? tituloBaixa.DescricaoSituacaoBaixaTitulo : string.Empty,
                TituloDeAgrupamento = tituloBaixa != null && repTituloBaixa.ContemTitulosGeradosDeNegociacao(codigoTituloBaixa) ? "Atenção! Contem título(s) gerado(s) a partir de uma negociação" : string.Empty,
                DescricaoGrupoPessoa = tituloBaixa.GrupoPessoas != null ? tituloBaixa.GrupoPessoas.Descricao : string.Empty,
                NomePessoa = tituloBaixa.Pessoa != null ? tituloBaixa.Pessoa.Nome : tituloBaixa.GrupoPessoas != null ? string.Empty : "MULTIPLOS CLIENTES",
                tituloBaixa.MoedaCotacaoBancoCentral,
                DataBaseCRT = tituloBaixa.DataBaseCRT.HasValue ? tituloBaixa.DataBaseCRT.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty,
                ValorMoedaCotacao = tituloBaixa.ValorMoedaCotacao.ToString("n10"),
                ValorOriginalMoedaEstrangeira = tituloBaixa.ValorOriginalMoedaEstrangeira.ToString("n2"),
                Situacao = tituloBaixa != null ? tituloBaixa.SituacaoBaixaTitulo : SituacaoBaixaTitulo.Iniciada
            };
        }

        public void GeraIntegracaoBaixaTituloReceber(int codigo, string operador, string emailAdministrativo, Repositorio.UnitOfWork unidadeDeTrabalho, bool enviarEmail)
        {
            Repositorio.Embarcador.Financeiro.TituloBaixa repTituloBaixa = new Repositorio.Embarcador.Financeiro.TituloBaixa(unidadeDeTrabalho);
            Repositorio.Embarcador.Financeiro.TituloBaixaIntegracao repTituloBaixaIntegracao = new Repositorio.Embarcador.Financeiro.TituloBaixaIntegracao(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Financeiro.TituloBaixa baixa = repTituloBaixa.BuscarPorCodigo(codigo);

            Dominio.Entidades.Embarcador.Financeiro.TituloBaixaIntegracao integracao = new Dominio.Entidades.Embarcador.Financeiro.TituloBaixaIntegracao();
            integracao.Assunto = "Baixa de título a receber Nº " + baixa.CodigosTitulos;
            if (integracao.Assunto.Length >= 300)
                integracao.Assunto = integracao.Assunto.Substring(0, 299);
            integracao.DataIntegracao = DateTime.Now;
            integracao.Destinatarios = emailAdministrativo;
            integracao.Mensagem = "Foi realizado a baixa do título de número " + baixa.CodigosTitulos + " pelo operador " + operador;
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

        public bool GeraReverteMovimentacaoFinanceira(out string erro, int codigoBaixa, Repositorio.UnitOfWork unidadeDeTrabalho, string stringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServico, bool reversao, Dominio.Entidades.Embarcador.Financeiro.PlanoConta planoBaixaTitulo, Dominio.Entidades.Embarcador.Financeiro.PlanoConta planoDebitoTitulo = null)
        {
            Repositorio.Embarcador.Financeiro.TituloBaixa repTituloBaixa = new Repositorio.Embarcador.Financeiro.TituloBaixa(unidadeDeTrabalho);
            Repositorio.Embarcador.Financeiro.TituloBaixaDetalheConhecimento repTituloBaixaDetalheConhecimento = new Repositorio.Embarcador.Financeiro.TituloBaixaDetalheConhecimento(unidadeDeTrabalho);
            Repositorio.Embarcador.Financeiro.MovimentoFinanceiro repMovimentoFinanceiro = new Repositorio.Embarcador.Financeiro.MovimentoFinanceiro(unidadeDeTrabalho);
            Repositorio.Embarcador.Financeiro.TituloBaixaAgrupado repTituloBaixaAgrupado = new Repositorio.Embarcador.Financeiro.TituloBaixaAgrupado(unidadeDeTrabalho);
            Repositorio.Embarcador.Financeiro.TituloBaixaAcrescimo repTituloBaixaAcrescimo = new Repositorio.Embarcador.Financeiro.TituloBaixaAcrescimo(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Financeiro.TituloBaixa tituloBaixa = repTituloBaixa.BuscarPorCodigo(codigoBaixa);
            Servicos.Embarcador.Financeiro.ProcessoMovimento servProcessoMovimento = new Servicos.Embarcador.Financeiro.ProcessoMovimento(stringConexao);
            erro = "";
            string obs = "";
            List<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado> listaTituloBaixaAgrupado = repTituloBaixaAgrupado.BuscarPorBaixaTitulo(codigoBaixa);
            Dominio.Entidades.Embarcador.Financeiro.PlanoConta debitoTitulo = null;

            string observacaoAdicionalMovimentacao = string.Empty;

            if (!string.IsNullOrWhiteSpace(tituloBaixa.Observacao))
                observacaoAdicionalMovimentacao = " " + tituloBaixa.Observacao;
            if (tituloBaixa.Cheques != null && tituloBaixa.Cheques.Count > 0)
                observacaoAdicionalMovimentacao += " Cheque(s) nº: " + string.Join(", ", tituloBaixa.Cheques.Select(o => o.Cheque.NumeroCheque));
            if (listaTituloBaixaAgrupado != null && listaTituloBaixaAgrupado.Count > 0)
                observacaoAdicionalMovimentacao += " Doc(s): " + string.Join(", ", listaTituloBaixaAgrupado.Select(o => o.Titulo.NumeroDocumentoTituloOriginal).Distinct());

            int countTitulosAgrupados = listaTituloBaixaAgrupado != null ? listaTituloBaixaAgrupado.Count : 0;
            for (int i = 0; i < countTitulosAgrupados; i++)
            {
                if (listaTituloBaixaAgrupado[i].Titulo.TituloBaixaNegociacao == null)
                {
                    debitoTitulo = repMovimentoFinanceiro.BuscarContaDebitoTitulo(listaTituloBaixaAgrupado[i].Titulo.Codigo);
                    if ((debitoTitulo == null) && (tipoServico != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe))
                    {
                        erro = "Título de código " + listaTituloBaixaAgrupado[i].Titulo.Codigo.ToString() + " não possui conta de entrada para realizar a baixa";
                        return false;
                    }
                    else if (planoDebitoTitulo != null)
                    {
                        if (planoDebitoTitulo != debitoTitulo)
                        {
                            erro = "Os títulos selecionados não possuem as mesmas contas de entrada";
                            return false;
                        }
                        else
                            planoDebitoTitulo = debitoTitulo;
                    }
                    else
                        planoDebitoTitulo = debitoTitulo;
                }
                else
                {
                    if (listaTituloBaixaAgrupado[i].Titulo.TituloBaixaNegociacao.TituloBaixa.TitulosAgrupados != null && listaTituloBaixaAgrupado[i].Titulo.TituloBaixaNegociacao.TituloBaixa.TitulosAgrupados.Count() > 0 && listaTituloBaixaAgrupado[i].Titulo.TituloBaixaNegociacao.TituloBaixa.TitulosAgrupados[0].Titulo != null)
                    {
                        Dominio.Entidades.Embarcador.Financeiro.Titulo tituloPai = listaTituloBaixaAgrupado[i].Titulo.TituloBaixaNegociacao.TituloBaixa.TitulosAgrupados[0].Titulo;
                        Dominio.Entidades.Embarcador.Financeiro.Titulo tituloPaiNovo = null;
                        tituloPaiNovo = RetornaTituloPai(listaTituloBaixaAgrupado[i].Titulo.TituloBaixaNegociacao.TituloBaixa.TitulosAgrupados[0].Titulo);
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

                        debitoTitulo = repMovimentoFinanceiro.BuscarContaDebitoTitulo(tituloPai.Codigo);
                        if (debitoTitulo == null)
                        {
                            erro = "Existem títulos gerados de negociações anteriores que não possuem conta de entrada para realizar a baixa";
                            return false;
                        }
                        else if (planoDebitoTitulo != null)
                        {
                            if (planoDebitoTitulo != debitoTitulo)
                            {
                                erro = "Existem títulos gerados de negociações anteriores que não possuem as mesmas contas de entrada";
                                return false;
                            }
                            else
                                planoDebitoTitulo = debitoTitulo;
                        }
                        else
                            planoDebitoTitulo = debitoTitulo;
                    }
                }

            }

            if (tituloBaixa.Valor > 0 && planoBaixaTitulo != null)
            {
                if ((planoDebitoTitulo == null) && (tipoServico != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe))
                {
                    erro = "Não foi possível localizar a conta de entrada para realizar a movimentação financeira";
                    return false;
                }
                else
                {
                    if (reversao)
                        obs = "REVERSÃO ";
                    else
                        obs = "";

                    obs += "BAIXA DO(S) TITULO(S) " + tituloBaixa.CodigosTitulos;

                    if (tituloBaixa.Pessoa != null)
                        obs += " - " + tituloBaixa.Pessoa.Nome;

                    if (tituloBaixa.GrupoPessoas != null)
                        obs += " - " + tituloBaixa.GrupoPessoas.Descricao;

                    if (!string.IsNullOrWhiteSpace(tituloBaixa.NumeroFaturas))
                        obs += " - FATURA(S) N. " + tituloBaixa.NumeroFaturas;

                    obs += ".";

                    if (reversao)
                    {
                        if (servProcessoMovimento.MovimentacaoFinanceiraJaConciliada(null, tituloBaixa.Valor, tituloBaixa.Codigo.ToString(), unidadeDeTrabalho, TipoDocumentoMovimento.Recebimento, planoDebitoTitulo, planoBaixaTitulo, 0))
                        {
                            erro = "Não é possível reverter esta baixa pois já se encontra conciliada.";
                            return false;
                        }
                        servProcessoMovimento.GerarMovimentacao(null, tituloBaixa.DataBaixa.Value, tituloBaixa.Valor, tituloBaixa.Codigo.ToString(), obs + observacaoAdicionalMovimentacao, unidadeDeTrabalho, TipoDocumentoMovimento.Recebimento, tipoServico, 0, planoBaixaTitulo, planoDebitoTitulo, 0, null, tituloBaixa.Pessoa, tituloBaixa.GrupoPessoas, tituloBaixa.DataBase, null, null, null, null, tituloBaixa.MoedaCotacaoBancoCentral, tituloBaixa.DataBaseCRT, tituloBaixa.ValorMoedaCotacao, tituloBaixa.ValorOriginalMoedaEstrangeira);
                    }
                    else
                        servProcessoMovimento.GerarMovimentacao(null, tituloBaixa.DataBaixa.Value, tituloBaixa.Valor, tituloBaixa.Codigo.ToString(), obs + observacaoAdicionalMovimentacao, unidadeDeTrabalho, TipoDocumentoMovimento.Recebimento, tipoServico, 0, planoDebitoTitulo, planoBaixaTitulo, 0, null, tituloBaixa.Pessoa, tituloBaixa.GrupoPessoas, tituloBaixa.DataBase, null, null, null, null, tituloBaixa.MoedaCotacaoBancoCentral, tituloBaixa.DataBaseCRT, tituloBaixa.ValorMoedaCotacao, tituloBaixa.ValorOriginalMoedaEstrangeira);
                }
            }

            List<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaDetalheConhecimento> detalhes = repTituloBaixaDetalheConhecimento.BuscarPorBaixa(codigoBaixa);
            if (detalhes != null && detalhes.Count > 0)
            {
                foreach (Dominio.Entidades.Embarcador.Financeiro.TituloBaixaDetalheConhecimento detalhe in detalhes)
                {
                    if (detalhe.JustificativaAcrescimo != null && detalhe.JustificativaAcrescimo.GerarMovimentoAutomatico && detalhe.ValorAcrescimo > 0)
                    {
                        if (reversao)
                            obs = "REVERSÃO ";
                        else
                            obs = "";

                        obs += "ACRESCIMO DO CONHECIMENTO " + detalhe.CTe.Numero.ToString() + " NA BAIXA DO(S) TITULO(S) " + tituloBaixa.CodigosTitulos;

                        if (tituloBaixa.Pessoa != null)
                            obs += " - " + tituloBaixa.Pessoa.Nome;

                        if (tituloBaixa.GrupoPessoas != null)
                            obs += " - " + tituloBaixa.GrupoPessoas.Descricao;

                        if (!string.IsNullOrWhiteSpace(tituloBaixa.NumeroFaturas))
                            obs += " - FATURA(S) N. " + tituloBaixa.NumeroFaturas;

                        obs += ".";

                        decimal valorOriginalMoedaEstrangeira = tituloBaixa.ValorMoedaCotacao > 0 ? Math.Round(detalhe.ValorAcrescimo / tituloBaixa.ValorMoedaCotacao, 2) : 0;

                        if (reversao)
                        {
                            if (detalhe.JustificativaAcrescimo.TipoMovimentoReversaoUsoJustificativa == null)
                            {
                                erro = "A justificativa " + detalhe.JustificativaAcrescimo.Descricao + " não possui tipo de movimento vinculado para a sua reversão.";
                                return false;
                            }
                            servProcessoMovimento.GerarMovimentacao(null, tituloBaixa.DataBaixa.Value, detalhe.ValorAcrescimo, tituloBaixa.Codigo.ToString(), obs + observacaoAdicionalMovimentacao, unidadeDeTrabalho, TipoDocumentoMovimento.Recebimento, tipoServico, 0, planoDebitoTitulo, detalhe.JustificativaAcrescimo.TipoMovimentoReversaoUsoJustificativa.PlanoDeContaDebito, 0, null, tituloBaixa.Pessoa, tituloBaixa.GrupoPessoas, tituloBaixa.DataBase, null, null, null, null, tituloBaixa.MoedaCotacaoBancoCentral, tituloBaixa.DataBaseCRT, tituloBaixa.ValorMoedaCotacao, valorOriginalMoedaEstrangeira);
                        }
                        else
                        {
                            if (detalhe.JustificativaAcrescimo.TipoMovimentoUsoJustificativa == null)
                            {
                                erro = "A justificativa " + detalhe.JustificativaAcrescimo.Descricao + " não possui tipo de movimento vinculado para realizar a movimentação.";
                                return false;
                            }
                            servProcessoMovimento.GerarMovimentacao(null, tituloBaixa.DataBaixa.Value, detalhe.ValorAcrescimo, tituloBaixa.Codigo.ToString(), obs + observacaoAdicionalMovimentacao, unidadeDeTrabalho, TipoDocumentoMovimento.Recebimento, tipoServico, 0, detalhe.JustificativaAcrescimo.TipoMovimentoUsoJustificativa.PlanoDeContaCredito, planoDebitoTitulo, 0, null, tituloBaixa.Pessoa, tituloBaixa.GrupoPessoas, tituloBaixa.DataBase, null, null, null, null, tituloBaixa.MoedaCotacaoBancoCentral, tituloBaixa.DataBaseCRT, tituloBaixa.ValorMoedaCotacao, valorOriginalMoedaEstrangeira);
                        }
                    }

                    if (detalhe.JustificativaDesconto != null && detalhe.JustificativaDesconto.GerarMovimentoAutomatico && detalhe.ValorDesconto > 0)
                    {
                        if (reversao)
                            obs = "REVERSÃO ";
                        else
                            obs = "";

                        obs += "DESCONTO DO CONHECIMENTO " + detalhe.CTe.Numero.ToString() + " NA BAIXA DO(S) TITULO(S) " + tituloBaixa.CodigosTitulos;

                        if (tituloBaixa.Pessoa != null)
                            obs += " - " + tituloBaixa.Pessoa.Nome;

                        if (tituloBaixa.GrupoPessoas != null)
                            obs += " - " + tituloBaixa.GrupoPessoas.Descricao;

                        if (!string.IsNullOrWhiteSpace(tituloBaixa.NumeroFaturas))
                            obs += " - FATURA(S) N. " + tituloBaixa.NumeroFaturas;

                        obs += ".";

                        decimal valorOriginalMoedaEstrangeira = tituloBaixa.ValorMoedaCotacao > 0 ? Math.Round(detalhe.ValorDesconto / tituloBaixa.ValorMoedaCotacao, 2) : 0;

                        if (reversao)
                        {
                            if (detalhe.JustificativaDesconto.TipoMovimentoReversaoUsoJustificativa == null)
                            {
                                erro = "A justificativa " + detalhe.JustificativaDesconto.Descricao + " não possui tipo de movimento vinculado para a sua reversão.";
                                return false;
                            }
                            if (servProcessoMovimento.MovimentacaoFinanceiraJaConciliada(null, detalhe.ValorDesconto, tituloBaixa.Codigo.ToString(), unidadeDeTrabalho, TipoDocumentoMovimento.Recebimento, planoDebitoTitulo, detalhe.JustificativaDesconto.TipoMovimentoUsoJustificativa.PlanoDeContaDebito, 0))
                            {
                                erro = "Não é possível reverter esta baixa pois já se encontra conciliada.";
                                return false;
                            }
                            servProcessoMovimento.GerarMovimentacao(null, tituloBaixa.DataBaixa.Value, detalhe.ValorDesconto, tituloBaixa.Codigo.ToString(), obs + observacaoAdicionalMovimentacao, unidadeDeTrabalho, TipoDocumentoMovimento.Recebimento, tipoServico, 0, detalhe.JustificativaDesconto.TipoMovimentoReversaoUsoJustificativa.PlanoDeContaCredito, planoDebitoTitulo, 0, null, tituloBaixa.Pessoa, tituloBaixa.GrupoPessoas, tituloBaixa.DataBase, null, null, null, null, tituloBaixa.MoedaCotacaoBancoCentral, tituloBaixa.DataBaseCRT, tituloBaixa.ValorMoedaCotacao, valorOriginalMoedaEstrangeira);
                        }
                        else
                        {
                            if (detalhe.JustificativaDesconto.TipoMovimentoUsoJustificativa == null)
                            {
                                erro = "A justificativa " + detalhe.JustificativaDesconto.Descricao + " não possui tipo de movimento vinculado para realizar a movimentação.";
                                return false;
                            }
                            servProcessoMovimento.GerarMovimentacao(null, tituloBaixa.DataBaixa.Value, detalhe.ValorDesconto, tituloBaixa.Codigo.ToString(), obs + observacaoAdicionalMovimentacao, unidadeDeTrabalho, TipoDocumentoMovimento.Recebimento, tipoServico, 0, planoDebitoTitulo, detalhe.JustificativaDesconto.TipoMovimentoUsoJustificativa.PlanoDeContaDebito, 0, null, tituloBaixa.Pessoa, tituloBaixa.GrupoPessoas, tituloBaixa.DataBase, null, null, null, null, tituloBaixa.MoedaCotacaoBancoCentral, tituloBaixa.DataBaseCRT, tituloBaixa.ValorMoedaCotacao, valorOriginalMoedaEstrangeira);
                        }
                    }
                }
            }

            List<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAcrescimo> acrescimos = repTituloBaixaAcrescimo.BuscarPorBaixaTitulo(codigoBaixa);
            if (acrescimos != null)
            {
                if ((planoDebitoTitulo == null) && (tipoServico != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe))
                {
                    erro = "Não foi possível localizar a conta de entrada para realizar a movimentação financeira dos acréscimos e descontos.";
                    return false;
                }
                else
                {
                    foreach (Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAcrescimo acrescimoDesconto in acrescimos)
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

                            obs += " NA BAIXA DO(S) TITULO(S) " + tituloBaixa.CodigosTitulos;

                            if (tituloBaixa.Pessoa != null)
                                obs += " - " + tituloBaixa.Pessoa.Nome;

                            if (tituloBaixa.GrupoPessoas != null)
                                obs += " - " + tituloBaixa.GrupoPessoas.Descricao;

                            if (!string.IsNullOrWhiteSpace(tituloBaixa.NumeroFaturas))
                                obs += " - FATURA(S) N. " + tituloBaixa.NumeroFaturas;

                            obs += ".";

                            if (reversao)
                            {
                                if (acrescimoDesconto.Justificativa.TipoMovimentoReversaoUsoJustificativa == null)
                                {
                                    erro = "A justificativa " + acrescimoDesconto.Justificativa.Descricao + " não possui tipo de movimento vinculado para a sua reversão.";
                                    return false;
                                }

                                if (acrescimoDesconto.Justificativa.TipoJustificativa == TipoJustificativa.Desconto)
                                {
                                    if (servProcessoMovimento.MovimentacaoFinanceiraJaConciliada(null, acrescimoDesconto.Valor, tituloBaixa.Codigo.ToString(), unidadeDeTrabalho, TipoDocumentoMovimento.Recebimento, acrescimoDesconto.Justificativa.TipoMovimentoReversaoUsoJustificativa.PlanoDeContaCredito, planoDebitoTitulo, 0))
                                    {
                                        erro = "Não é possível reverter esta baixa pois já se encontra conciliada.";
                                        return false;
                                    }
                                    servProcessoMovimento.GerarMovimentacao(null, tituloBaixa.DataBaixa.Value, acrescimoDesconto.Valor, tituloBaixa.Codigo.ToString(), obs + observacaoAdicionalMovimentacao, unidadeDeTrabalho, TipoDocumentoMovimento.Recebimento, tipoServico, 0, acrescimoDesconto.Justificativa.TipoMovimentoReversaoUsoJustificativa.PlanoDeContaCredito, planoDebitoTitulo, 0, null, tituloBaixa.Pessoa, tituloBaixa.GrupoPessoas, tituloBaixa.DataBase, null, null, null, null, acrescimoDesconto.MoedaCotacaoBancoCentral, acrescimoDesconto.DataBaseCRT, acrescimoDesconto.ValorMoedaCotacao, acrescimoDesconto.ValorOriginalMoedaEstrangeira);
                                }
                                else
                                {
                                    if (servProcessoMovimento.MovimentacaoFinanceiraJaConciliada(null, acrescimoDesconto.Valor, tituloBaixa.Codigo.ToString(), unidadeDeTrabalho, TipoDocumentoMovimento.Recebimento, planoDebitoTitulo, acrescimoDesconto.Justificativa.TipoMovimentoReversaoUsoJustificativa.PlanoDeContaDebito, 0))
                                    {
                                        erro = "Não é possível reverter esta baixa pois já se encontra conciliada.";
                                        return false;
                                    }
                                    servProcessoMovimento.GerarMovimentacao(null, tituloBaixa.DataBaixa.Value, acrescimoDesconto.Valor, tituloBaixa.Codigo.ToString(), obs + observacaoAdicionalMovimentacao, unidadeDeTrabalho, TipoDocumentoMovimento.Recebimento, tipoServico, 0, planoDebitoTitulo, acrescimoDesconto.Justificativa.TipoMovimentoReversaoUsoJustificativa.PlanoDeContaDebito, 0, null, tituloBaixa.Pessoa, tituloBaixa.GrupoPessoas, tituloBaixa.DataBase, null, null, null, null, acrescimoDesconto.MoedaCotacaoBancoCentral, acrescimoDesconto.DataBaseCRT, acrescimoDesconto.ValorMoedaCotacao, acrescimoDesconto.ValorOriginalMoedaEstrangeira);
                                }
                            }
                            else
                            {
                                if (acrescimoDesconto.Justificativa.TipoMovimentoUsoJustificativa == null)
                                {
                                    erro = "A justificativa " + acrescimoDesconto.Justificativa.Descricao + " não possui tipo de movimento vinculado para realizar a movimentação.";
                                    return false;
                                }
                                if (acrescimoDesconto.Justificativa.TipoJustificativa == TipoJustificativa.Desconto)
                                    servProcessoMovimento.GerarMovimentacao(null, tituloBaixa.DataBaixa.Value, acrescimoDesconto.Valor, tituloBaixa.Codigo.ToString(), obs + observacaoAdicionalMovimentacao, unidadeDeTrabalho, TipoDocumentoMovimento.Recebimento, tipoServico, 0, planoDebitoTitulo, acrescimoDesconto.Justificativa.TipoMovimentoUsoJustificativa.PlanoDeContaDebito, 0, null, tituloBaixa.Pessoa, tituloBaixa.GrupoPessoas, tituloBaixa.DataBase, null, null, null, null, acrescimoDesconto.MoedaCotacaoBancoCentral, acrescimoDesconto.DataBaseCRT, acrescimoDesconto.ValorMoedaCotacao, acrescimoDesconto.ValorOriginalMoedaEstrangeira);
                                else
                                    servProcessoMovimento.GerarMovimentacao(null, tituloBaixa.DataBaixa.Value, acrescimoDesconto.Valor, tituloBaixa.Codigo.ToString(), obs + observacaoAdicionalMovimentacao, unidadeDeTrabalho, TipoDocumentoMovimento.Recebimento, tipoServico, 0, acrescimoDesconto.Justificativa.TipoMovimentoUsoJustificativa.PlanoDeContaCredito, planoDebitoTitulo, 0, null, tituloBaixa.Pessoa, tituloBaixa.GrupoPessoas, tituloBaixa.DataBase, null, null, null, null, acrescimoDesconto.MoedaCotacaoBancoCentral, acrescimoDesconto.DataBaseCRT, acrescimoDesconto.ValorMoedaCotacao, acrescimoDesconto.ValorOriginalMoedaEstrangeira);

                            }
                        }
                    }
                }
            }

            return true;
        }

        public bool GeraReverteMovimentacaoFinanceiraIndividual(out string erro, int codigoBaixa, Repositorio.UnitOfWork unidadeDeTrabalho, string stringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServico, bool reversao, Dominio.Entidades.Embarcador.Financeiro.PlanoConta planoBaixaTitulo, Dominio.Entidades.Embarcador.Financeiro.PlanoConta planoDebitoTitulo = null)
        {
            Repositorio.Embarcador.Financeiro.TituloBaixa repTituloBaixa = new Repositorio.Embarcador.Financeiro.TituloBaixa(unidadeDeTrabalho);
            Repositorio.Embarcador.Financeiro.MovimentoFinanceiro repMovimentoFinanceiro = new Repositorio.Embarcador.Financeiro.MovimentoFinanceiro(unidadeDeTrabalho);
            Repositorio.Embarcador.Financeiro.TituloBaixaAgrupado repTituloBaixaAgrupado = new Repositorio.Embarcador.Financeiro.TituloBaixaAgrupado(unidadeDeTrabalho);
            Repositorio.Embarcador.Financeiro.TituloBaixaAcrescimo repTituloBaixaAcrescimo = new Repositorio.Embarcador.Financeiro.TituloBaixaAcrescimo(unidadeDeTrabalho);

            Dominio.Entidades.Embarcador.Financeiro.TituloBaixa tituloBaixa = repTituloBaixa.BuscarPorCodigo(codigoBaixa);
            Servicos.Embarcador.Financeiro.ProcessoMovimento servProcessoMovimento = new Servicos.Embarcador.Financeiro.ProcessoMovimento(stringConexao);
            erro = "";
            string obs = "";
            List<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado> listaTituloBaixaAgrupado = repTituloBaixaAgrupado.BuscarPorBaixaTitulo(codigoBaixa);
            Dominio.Entidades.Embarcador.Financeiro.PlanoConta debitoTitulo = null;

            string observacaoPadrao = string.Empty;

            if (!string.IsNullOrWhiteSpace(tituloBaixa.Observacao))
                observacaoPadrao = " " + tituloBaixa.Observacao;
            if (tituloBaixa.Cheques != null && tituloBaixa.Cheques.Count > 0)
                observacaoPadrao += " Cheque(s) nº: " + string.Join(", ", tituloBaixa.Cheques.Select(o => o.Cheque.NumeroCheque));
            if (listaTituloBaixaAgrupado != null && listaTituloBaixaAgrupado.Count > 0)
            {
                foreach (Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado tituloBaixarAgrupado in listaTituloBaixaAgrupado)
                {
                    Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = tituloBaixarAgrupado.Titulo;

                    string observacaoAdicionalMovimentacao = observacaoPadrao + " Doc: " + titulo.NumeroDocumentoTituloOriginal + " " + titulo.TipoDocumentoTituloOriginal;

                    if (titulo.TituloBaixaNegociacao == null)
                    {
                        debitoTitulo = repMovimentoFinanceiro.BuscarContaDebitoTitulo(titulo.Codigo);
                        if ((debitoTitulo == null) && (tipoServico != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe))
                        {
                            erro = "Título de código " + titulo.Codigo.ToString() + " não possui conta de entrada para realizar a baixa";
                            return false;
                        }
                        else if (planoDebitoTitulo != null)
                        {
                            if (planoDebitoTitulo != debitoTitulo)
                            {
                                erro = "Os títulos selecionados não possuem as mesmas contas de entrada";
                                return false;
                            }
                            else
                                planoDebitoTitulo = debitoTitulo;
                        }
                        else
                            planoDebitoTitulo = debitoTitulo;
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

                            debitoTitulo = repMovimentoFinanceiro.BuscarContaDebitoTitulo(tituloPai.Codigo);
                            if (debitoTitulo == null)
                            {
                                erro = "Existem títulos gerados de negociações anteriores que não possuem conta de entrada para realizar a baixa";
                                return false;
                            }
                            else if (planoDebitoTitulo != null)
                            {
                                if (planoDebitoTitulo != debitoTitulo)
                                {
                                    erro = "Existem títulos gerados de negociações anteriores que não possuem as mesmas contas de entrada";
                                    return false;
                                }
                                else
                                    planoDebitoTitulo = debitoTitulo;
                            }
                            else
                                planoDebitoTitulo = debitoTitulo;
                        }
                    }

                    if (titulo.ValorPago > 0 && planoBaixaTitulo != null)
                    {
                        if ((planoDebitoTitulo == null) && (tipoServico != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe))
                        {
                            erro = "Não foi possível localizar a conta de entrada para realizar a movimentação financeira";
                            return false;
                        }
                        else
                        {
                            if (reversao)
                                obs = "REVERSÃO ";
                            else
                                obs = "";

                            obs += "BAIXA DO TITULO " + titulo.Codigo.ToString("D");

                            if (tituloBaixa.Pessoa != null)
                                obs += " - " + tituloBaixa.Pessoa.Nome;
                            else if (titulo.Pessoa != null)
                                obs += " - " + titulo.Pessoa.Nome;
                            else if (tituloBaixa.GrupoPessoas != null)
                                obs += " - " + tituloBaixa.GrupoPessoas.Descricao;

                            if (!string.IsNullOrWhiteSpace(tituloBaixa.NumeroFaturas))
                                obs += " - FATURA(S) N. " + tituloBaixa.NumeroFaturas;

                            obs += ".";

                            decimal valorOriginalMoedaEstrangeira = tituloBaixa.ValorMoedaCotacao > 0 ? Math.Round(titulo.ValorPago / tituloBaixa.ValorMoedaCotacao, 2) : 0;

                            if (reversao)
                            {
                                if (servProcessoMovimento.MovimentacaoFinanceiraJaConciliada(null, titulo.ValorPago, tituloBaixa.Codigo.ToString(), unidadeDeTrabalho, TipoDocumentoMovimento.Recebimento, planoDebitoTitulo, planoBaixaTitulo, 0))
                                {
                                    erro = "Não é possível reverter esta baixa pois já se encontra conciliada.";
                                    return false;
                                }
                                servProcessoMovimento.GerarMovimentacao(null, tituloBaixa.DataBaixa.Value, titulo.ValorPago, tituloBaixa.Codigo.ToString(), obs + observacaoAdicionalMovimentacao, unidadeDeTrabalho, TipoDocumentoMovimento.Recebimento, tipoServico, 0, planoBaixaTitulo, planoDebitoTitulo, titulo.Codigo, null, (tituloBaixa.Pessoa == null ? titulo.Pessoa : tituloBaixa.Pessoa), tituloBaixa.GrupoPessoas, tituloBaixa.DataBase, null, null, null, null, tituloBaixa.MoedaCotacaoBancoCentral, tituloBaixa.DataBaseCRT, tituloBaixa.ValorMoedaCotacao, valorOriginalMoedaEstrangeira);
                            }
                            else
                                servProcessoMovimento.GerarMovimentacao(null, tituloBaixa.DataBaixa.Value, titulo.ValorPago, tituloBaixa.Codigo.ToString(), obs + observacaoAdicionalMovimentacao, unidadeDeTrabalho, TipoDocumentoMovimento.Recebimento, tipoServico, 0, planoDebitoTitulo, planoBaixaTitulo, titulo.Codigo, null, (tituloBaixa.Pessoa == null ? titulo.Pessoa : tituloBaixa.Pessoa), tituloBaixa.GrupoPessoas, tituloBaixa.DataBase, null, null, null, null, tituloBaixa.MoedaCotacaoBancoCentral, tituloBaixa.DataBaseCRT, tituloBaixa.ValorMoedaCotacao, valorOriginalMoedaEstrangeira);
                        }
                    }
                }
            }

            if (listaTituloBaixaAgrupado?.Count > 0)
                observacaoPadrao += " Doc(s): " + string.Join(", ", listaTituloBaixaAgrupado.Select(o => o.Titulo.NumeroDocumentoTituloOriginal).Distinct());

            List<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAcrescimo> acrescimos = repTituloBaixaAcrescimo.BuscarPorBaixaTitulo(codigoBaixa);
            if (acrescimos?.Count > 0)
            {
                if ((planoDebitoTitulo == null) && (tipoServico != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe))
                {
                    erro = "Não foi possível localizar a conta de entrada para realizar a movimentação financeira dos acréscimos e descontos.";
                    return false;
                }

                foreach (Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAcrescimo acrescimoDesconto in acrescimos)
                {
                    if (!acrescimoDesconto.Justificativa.GerarMovimentoAutomatico)
                        continue;
                    if (reversao)
                        obs = "REVERSÃO ";
                    else
                        obs = "";
                    if (acrescimoDesconto.Justificativa.TipoJustificativa == TipoJustificativa.Acrescimo)
                        obs += "ACRESCIMO";
                    else
                        obs += "DESCONTO";
                    obs += " NA BAIXA DO(S) TITULO(S) " + tituloBaixa.CodigosTitulos;
                    if (tituloBaixa.Pessoa != null)
                        obs += " - " + tituloBaixa.Pessoa.Nome;
                    if (tituloBaixa.GrupoPessoas != null)
                        obs += " - " + tituloBaixa.GrupoPessoas.Descricao;
                    if (!string.IsNullOrWhiteSpace(tituloBaixa.NumeroFaturas))
                        obs += " - FATURA(S) N. " + tituloBaixa.NumeroFaturas;
                    obs += ".";

                    if (reversao)
                    {
                        if (acrescimoDesconto.Justificativa.TipoMovimentoReversaoUsoJustificativa == null)
                        {
                            erro = "A justificativa " + acrescimoDesconto.Justificativa.Descricao + " não possui tipo de movimento vinculado para a sua reversão.";
                            return false;
                        }

                        if (acrescimoDesconto.Justificativa.TipoJustificativa == TipoJustificativa.Desconto)
                        {
                            if (servProcessoMovimento.MovimentacaoFinanceiraJaConciliada(null, acrescimoDesconto.Valor, tituloBaixa.Codigo.ToString(), unidadeDeTrabalho, TipoDocumentoMovimento.Recebimento, acrescimoDesconto.Justificativa.TipoMovimentoReversaoUsoJustificativa.PlanoDeContaCredito, planoDebitoTitulo, 0))
                            {
                                erro = "Não é possível reverter esta baixa pois já se encontra conciliada.";
                                return false;
                            }
                            servProcessoMovimento.GerarMovimentacao(null, tituloBaixa.DataBaixa.Value, acrescimoDesconto.Valor, tituloBaixa.Codigo.ToString(), obs + observacaoPadrao, unidadeDeTrabalho, TipoDocumentoMovimento.Recebimento, tipoServico, 0, acrescimoDesconto.Justificativa.TipoMovimentoReversaoUsoJustificativa.PlanoDeContaCredito, planoDebitoTitulo, 0, null, tituloBaixa.Pessoa, tituloBaixa.GrupoPessoas, tituloBaixa.DataBase, null, null, null, null, acrescimoDesconto.MoedaCotacaoBancoCentral, acrescimoDesconto.DataBaseCRT, acrescimoDesconto.ValorMoedaCotacao, acrescimoDesconto.ValorOriginalMoedaEstrangeira);
                        }
                        else
                        {
                            if (servProcessoMovimento.MovimentacaoFinanceiraJaConciliada(null, acrescimoDesconto.Valor, tituloBaixa.Codigo.ToString(), unidadeDeTrabalho, TipoDocumentoMovimento.Recebimento, planoDebitoTitulo, acrescimoDesconto.Justificativa.TipoMovimentoReversaoUsoJustificativa.PlanoDeContaDebito, 0))
                            {
                                erro = "Não é possível reverter esta baixa pois já se encontra conciliada.";
                                return false;
                            }
                            servProcessoMovimento.GerarMovimentacao(null, tituloBaixa.DataBaixa.Value, acrescimoDesconto.Valor, tituloBaixa.Codigo.ToString(), obs + observacaoPadrao, unidadeDeTrabalho, TipoDocumentoMovimento.Recebimento, tipoServico, 0, planoDebitoTitulo, acrescimoDesconto.Justificativa.TipoMovimentoReversaoUsoJustificativa.PlanoDeContaDebito, 0, null, tituloBaixa.Pessoa, tituloBaixa.GrupoPessoas, tituloBaixa.DataBase, null, null, null, null, acrescimoDesconto.MoedaCotacaoBancoCentral, acrescimoDesconto.DataBaseCRT, acrescimoDesconto.ValorMoedaCotacao, acrescimoDesconto.ValorOriginalMoedaEstrangeira);
                        }
                    }
                    else
                    {
                        if (acrescimoDesconto.Justificativa.TipoMovimentoUsoJustificativa == null)
                        {
                            erro = "A justificativa " + acrescimoDesconto.Justificativa.Descricao + " não possui tipo de movimento vinculado para realizar a movimentação.";
                            return false;
                        }
                        if (acrescimoDesconto.Justificativa.TipoJustificativa == TipoJustificativa.Desconto)
                            servProcessoMovimento.GerarMovimentacao(null, tituloBaixa.DataBaixa.Value, acrescimoDesconto.Valor, tituloBaixa.Codigo.ToString(), obs + observacaoPadrao, unidadeDeTrabalho, TipoDocumentoMovimento.Recebimento, tipoServico, 0, planoDebitoTitulo, acrescimoDesconto.Justificativa.TipoMovimentoUsoJustificativa.PlanoDeContaDebito, 0, null, tituloBaixa.Pessoa, tituloBaixa.GrupoPessoas, tituloBaixa.DataBase, null, null, null, null, acrescimoDesconto.MoedaCotacaoBancoCentral, acrescimoDesconto.DataBaseCRT, acrescimoDesconto.ValorMoedaCotacao, acrescimoDesconto.ValorOriginalMoedaEstrangeira);
                        else
                            servProcessoMovimento.GerarMovimentacao(null, tituloBaixa.DataBaixa.Value, acrescimoDesconto.Valor, tituloBaixa.Codigo.ToString(), obs + observacaoPadrao, unidadeDeTrabalho, TipoDocumentoMovimento.Recebimento, tipoServico, 0, acrescimoDesconto.Justificativa.TipoMovimentoUsoJustificativa.PlanoDeContaCredito, planoDebitoTitulo, 0, null, tituloBaixa.Pessoa, tituloBaixa.GrupoPessoas, tituloBaixa.DataBase, null, null, null, null, acrescimoDesconto.MoedaCotacaoBancoCentral, acrescimoDesconto.DataBaseCRT, acrescimoDesconto.ValorMoedaCotacao, acrescimoDesconto.ValorOriginalMoedaEstrangeira);
                    }
                }
            }

            return true;
        }

        public static object ObterDetalhesNegociacaoBaixa(TituloBaixa tituloBaixa, Dominio.Entidades.Usuario usuario, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Financeiro.MovimentoFinanceiro repMovimento = new Repositorio.Embarcador.Financeiro.MovimentoFinanceiro(unitOfWork);

            decimal saldoContaAdiantamento = 0m;
            if (configuracao.PlanoContaAdiantamentoCliente != null && tituloBaixa != null && tituloBaixa.Pessoa != null && (tituloBaixa.SituacaoBaixaTitulo == SituacaoBaixaTitulo.EmNegociacao || tituloBaixa.SituacaoBaixaTitulo == SituacaoBaixaTitulo.Iniciada))
                saldoContaAdiantamento = repMovimento.BuscarSaldoContaCliente(configuracao.PlanoContaAdiantamentoCliente.Codigo, tituloBaixa.Pessoa.CPF_CNPJ);

            return new
            {
                CodigoFatura = tituloBaixa.Fatura?.Codigo ?? 0,
                NumeroFatura = tituloBaixa.Fatura?.Numero ?? 0,
                Codigo = tituloBaixa.Codigo,
                DataBaixa = tituloBaixa.DataBaixa.Value.ToString("dd/MM/yyyy"),
                DataBase = tituloBaixa.DataBase.Value.ToString("dd/MM/yyyy"),
                DataEmissao = tituloBaixa.DataEmissao?.ToString("dd/MM/yyyy") ?? string.Empty,
                Operador = usuario?.Nome ?? "Integração",
                CodigoPessoa = tituloBaixa.Pessoa?.CPF_CNPJ ?? 0d,
                Valor = tituloBaixa.Valor.ToString("n2"),
                ValorPago = tituloBaixa.ValorPago.ToString("n2"),
                ValorAcrescimo = tituloBaixa.ValorAcrescimo.ToString("n2"),
                ValorDesconto = tituloBaixa.ValorDesconto.ToString("n2"),
                ValorTotalAPagar = tituloBaixa.ValorTotalAPagar.ToString("n2"),
                SaldoPendente = (tituloBaixa.ValorTotalAPagar - tituloBaixa.ValorPago).ToString("n2"),
                Moeda = tituloBaixa.MoedaCotacaoBancoCentral ?? MoedaCotacaoBancoCentral.Real,
                ValorCotacaoMoeda = tituloBaixa.ValorMoedaCotacao.ToString("n10"),
                ValorMoeda = tituloBaixa.ValorOriginalMoedaEstrangeira.ToString("n2"),
                ValorPagoMoeda = tituloBaixa.ValorPagoMoeda.ToString("n2"),
                ValorAcrescimoMoeda = tituloBaixa.ValorAcrescimoMoeda.ToString("n2"),
                ValorDescontoMoeda = tituloBaixa.ValorDescontoMoeda.ToString("n2"),
                ValorTotalAPagarMoeda = tituloBaixa.ValorTotalAPagarMoeda.ToString("n2"),
                SaldoPendenteMoeda = (tituloBaixa.ValorTotalAPagarMoeda - tituloBaixa.ValorPagoMoeda).ToString("n2"),
                TipoPagamentoRecebimento = tituloBaixa.TipoPagamentoRecebimento?.Descricao ?? string.Empty,
                PlanoConta = tituloBaixa.TipoPagamentoRecebimento?.PlanoConta?.Descricao ?? string.Empty,
                Situacao = tituloBaixa.SituacaoBaixaTitulo,
                SaldoContaAdiantamento = saldoContaAdiantamento.ToString("n2")
            };
        }

        public Dominio.Entidades.Embarcador.Financeiro.Titulo RetornaTituloPai(Dominio.Entidades.Embarcador.Financeiro.Titulo titulo)
        {
            if (titulo != null && titulo.TituloBaixaNegociacao != null && titulo.TituloBaixaNegociacao.TituloBaixa != null && titulo.TituloBaixaNegociacao.TituloBaixa.TitulosAgrupados[0] != null && titulo.TituloBaixaNegociacao.TituloBaixa.TitulosAgrupados[0].Titulo != null)
                return titulo.TituloBaixaNegociacao.TituloBaixa.TitulosAgrupados[0].Titulo;
            else
                return null;
        }

        public static void RemoverTituloDaBaixa(TituloBaixa tituloBaixa, int codigoTitulo, UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Financeiro.TituloBaixaAgrupado repTituloBaixaAgrupado = new Repositorio.Embarcador.Financeiro.TituloBaixaAgrupado(unitOfWork);
            Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);
            Repositorio.Embarcador.Financeiro.TituloDocumento repTituloDocumento = new Repositorio.Embarcador.Financeiro.TituloDocumento(unitOfWork);
            Repositorio.Embarcador.Financeiro.TituloBaixaAgrupadoDocumento repTituloBaixaAgrupadoDocumento = new Repositorio.Embarcador.Financeiro.TituloBaixaAgrupadoDocumento(unitOfWork);

            TituloBaixaAgrupado tituloBaixaAgrupado = repTituloBaixaAgrupado.BuscarPorTituloEBaixa(tituloBaixa.Codigo, codigoTitulo);

            repTituloBaixaAgrupadoDocumento.DeletarPorTituloBaixaAgrupado(tituloBaixaAgrupado.Codigo);
            repTituloBaixaAgrupado.Deletar(tituloBaixaAgrupado);

            AtualizarTotaisTituloBaixa(ref tituloBaixa, unitOfWork, false);
        }

        public static Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado AdicionarTituloABaixa(TituloBaixa tituloBaixa, int codigoTitulo, UnitOfWork unitOfWork, Dominio.Entidades.Usuario usuario, decimal valorAcrescimo, decimal valorDesconto, bool aplicarAcrescimoPrimeiroTitulo)
        {
            Repositorio.Embarcador.Financeiro.TituloBaixaAgrupado repTituloBaixaAgrupado = new Repositorio.Embarcador.Financeiro.TituloBaixaAgrupado(unitOfWork);
            Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);
            Repositorio.Embarcador.Financeiro.TituloDocumento repTituloDocumento = new Repositorio.Embarcador.Financeiro.TituloDocumento(unitOfWork);
            Repositorio.Embarcador.Financeiro.TituloBaixaAgrupadoDocumento repTituloBaixaAgrupadoDocumento = new Repositorio.Embarcador.Financeiro.TituloBaixaAgrupadoDocumento(unitOfWork);
            Repositorio.Embarcador.Financeiro.TituloBaixaAgrupadoDocumentoAcrescimoDesconto repTituloBaixaAgrupadoDocumentoAcrescimoDesconto = new Repositorio.Embarcador.Financeiro.TituloBaixaAgrupadoDocumentoAcrescimoDesconto(unitOfWork);
            Repositorio.Embarcador.Fatura.Justificativa repJustificativa = new Repositorio.Embarcador.Fatura.Justificativa(unitOfWork);

            TituloBaixaAgrupado tituloBaixaAgrupado = repTituloBaixaAgrupado.BuscarPorTituloEBaixa(tituloBaixa.Codigo, codigoTitulo);

            if (tituloBaixaAgrupado != null)
                return tituloBaixaAgrupado;

            tituloBaixaAgrupado = new TituloBaixaAgrupado
            {
                DataBase = tituloBaixa.DataBase.Value,
                DataBaixa = tituloBaixa.DataBaixa.Value,
                Titulo = repTitulo.BuscarPorCodigo(codigoTitulo),
                TituloBaixa = tituloBaixa
            };

            repTituloBaixaAgrupado.Inserir(tituloBaixaAgrupado);

            List<TituloDocumento> documentosTitulo = repTituloDocumento.BuscarPorTitulo(codigoTitulo);

            int countDocumentos = documentosTitulo.Count();
            decimal valorTotal = documentosTitulo.Sum(o => o.ValorTotal);
            decimal percentualAcrescimo = valorTotal > 0m ? valorAcrescimo / valorTotal : 0m;
            decimal percentualDesconto = valorTotal > 0m ? valorDesconto / valorTotal : 0m;
            decimal valorTotalAcrescimoRateado = 0m, valorTotalDescontoRateado = 0m;
            int i = 0;

            foreach (TituloDocumento documentoTitulo in documentosTitulo)
            {
                decimal valorAcrescimoRateado = 0m, valorDescontoRateado = 0m;

                if (countDocumentos == (i + 1))
                {
                    if (!aplicarAcrescimoPrimeiroTitulo)
                        valorAcrescimoRateado = valorAcrescimo - valorTotalAcrescimoRateado;
                    else
                        valorAcrescimoRateado = 0m;
                    valorDescontoRateado = valorDesconto - valorTotalDescontoRateado;
                }
                else
                {
                    if (!aplicarAcrescimoPrimeiroTitulo)
                        valorAcrescimoRateado = Math.Round((documentoTitulo.ValorTotal * percentualAcrescimo * 100) / 100, 2, MidpointRounding.ToEven);
                    else
                        valorAcrescimoRateado = 0m;
                    valorDescontoRateado = Math.Round((documentoTitulo.ValorTotal * percentualDesconto * 100) / 100, 2, MidpointRounding.ToEven);
                }

                if (valorAcrescimoRateado <= 0m && valorDescontoRateado <= 0m && valorAcrescimo > 0m && valorDesconto > 0m && !aplicarAcrescimoPrimeiroTitulo)
                    continue;

                valorTotalAcrescimoRateado += valorAcrescimoRateado;
                valorTotalDescontoRateado += valorDescontoRateado;

                i++;

                TituloBaixaAgrupadoDocumento tituloBaixaAgrupadoDocumento = new TituloBaixaAgrupadoDocumento()
                {
                    TituloBaixaAgrupado = tituloBaixaAgrupado,
                    TituloDocumento = documentoTitulo,
                    ValorPago = documentoTitulo.ValorTotal + (aplicarAcrescimoPrimeiroTitulo ? valorAcrescimo : valorAcrescimoRateado) - valorDescontoRateado,
                    ValorTotalAPagar = documentoTitulo.ValorTotal + (aplicarAcrescimoPrimeiroTitulo ? valorAcrescimo : valorAcrescimoRateado) - valorDescontoRateado,// + valorAcrescimo - valorDesconto,
                    ValorAcrescimo = aplicarAcrescimoPrimeiroTitulo ? valorAcrescimo : valorAcrescimoRateado,//valorAcrescimo,
                    ValorDesconto = valorDescontoRateado//valorDesconto
                };

                repTituloBaixaAgrupadoDocumento.Inserir(tituloBaixaAgrupadoDocumento);

                if (aplicarAcrescimoPrimeiroTitulo && valorAcrescimo > 0m)
                {
                    Dominio.Entidades.Embarcador.Fatura.Justificativa justificativa = null;
                    if (tituloBaixaAgrupado.Titulo != null && tituloBaixaAgrupado.Titulo.BoletoConfiguracao != null && tituloBaixaAgrupado.Titulo.BoletoConfiguracao.TipoMovimentoJuros != null)
                        justificativa = repJustificativa.BuscarPorTipoMovimento(tituloBaixaAgrupado.Titulo.BoletoConfiguracao.TipoMovimentoJuros.Codigo, TipoJustificativa.Acrescimo);

                    if (justificativa == null)
                        justificativa = repJustificativa.BuscarPrimeiraJustificativa(TipoJustificativa.Acrescimo);

                    if (justificativa != null)
                    {
                        TituloBaixaAgrupadoDocumentoAcrescimoDesconto tituloBaixaAgrupadoDocumentoAcrescimoDesconto = new TituloBaixaAgrupadoDocumentoAcrescimoDesconto()
                        {
                            Justificativa = justificativa,
                            TipoJustificativa = TipoJustificativa.Acrescimo,
                            TipoMovimentoReversao = justificativa.TipoMovimentoReversaoUsoJustificativa,
                            TipoMovimentoUso = justificativa.TipoMovimentoUsoJustificativa,
                            TituloBaixaAgrupadoDocumento = tituloBaixaAgrupadoDocumento,
                            Valor = valorAcrescimo,
                            Observacao = "",
                            ValorMoeda = 0m,
                            VariacaoCambial = false
                        };
                        repTituloBaixaAgrupadoDocumentoAcrescimoDesconto.Inserir(tituloBaixaAgrupadoDocumentoAcrescimoDesconto);
                    }
                    valorAcrescimo = 0m;
                    valorAcrescimoRateado = 0m;
                }
                else if (valorAcrescimoRateado > 0m)
                {
                    Dominio.Entidades.Embarcador.Fatura.Justificativa justificativa = null;
                    if (tituloBaixaAgrupado.Titulo != null && tituloBaixaAgrupado.Titulo.BoletoConfiguracao != null && tituloBaixaAgrupado.Titulo.BoletoConfiguracao.TipoMovimentoJuros != null)
                        justificativa = repJustificativa.BuscarPorTipoMovimento(tituloBaixaAgrupado.Titulo.BoletoConfiguracao.TipoMovimentoJuros.Codigo, TipoJustificativa.Acrescimo);

                    if (justificativa == null)
                        justificativa = repJustificativa.BuscarPrimeiraJustificativa(TipoJustificativa.Acrescimo);

                    if (justificativa != null)
                    {
                        TituloBaixaAgrupadoDocumentoAcrescimoDesconto tituloBaixaAgrupadoDocumentoAcrescimoDesconto = new TituloBaixaAgrupadoDocumentoAcrescimoDesconto()
                        {
                            Justificativa = justificativa,
                            TipoJustificativa = TipoJustificativa.Acrescimo,
                            TipoMovimentoReversao = justificativa.TipoMovimentoReversaoUsoJustificativa,
                            TipoMovimentoUso = justificativa.TipoMovimentoUsoJustificativa,
                            TituloBaixaAgrupadoDocumento = tituloBaixaAgrupadoDocumento,
                            Valor = valorAcrescimoRateado,
                            Observacao = "",
                            ValorMoeda = 0m,
                            VariacaoCambial = false
                        };
                        repTituloBaixaAgrupadoDocumentoAcrescimoDesconto.Inserir(tituloBaixaAgrupadoDocumentoAcrescimoDesconto);
                    }
                }

                if (valorDescontoRateado > 0m)
                {
                    Dominio.Entidades.Embarcador.Fatura.Justificativa justificativa = null;
                    if (tituloBaixaAgrupado.Titulo != null && tituloBaixaAgrupado.Titulo.BoletoConfiguracao != null && tituloBaixaAgrupado.Titulo.BoletoConfiguracao.TipoMovimentoDesconto != null)
                        justificativa = repJustificativa.BuscarPorTipoMovimento(tituloBaixaAgrupado.Titulo.BoletoConfiguracao.TipoMovimentoDesconto.Codigo, TipoJustificativa.Desconto);

                    if (justificativa == null)
                        justificativa = repJustificativa.BuscarPrimeiraJustificativa(TipoJustificativa.Desconto);

                    if (justificativa != null)
                    {
                        TituloBaixaAgrupadoDocumentoAcrescimoDesconto tituloBaixaAgrupadoDocumentoAcrescimoDesconto = new TituloBaixaAgrupadoDocumentoAcrescimoDesconto()
                        {
                            Justificativa = justificativa,
                            TipoJustificativa = TipoJustificativa.Acrescimo,
                            TipoMovimentoReversao = justificativa.TipoMovimentoReversaoUsoJustificativa,
                            TipoMovimentoUso = justificativa.TipoMovimentoUsoJustificativa,
                            TituloBaixaAgrupadoDocumento = tituloBaixaAgrupadoDocumento,
                            Valor = valorDescontoRateado,//valorDesconto,
                            Observacao = "",
                            ValorMoeda = 0m,
                            VariacaoCambial = false
                        };
                        repTituloBaixaAgrupadoDocumentoAcrescimoDesconto.Inserir(tituloBaixaAgrupadoDocumentoAcrescimoDesconto);
                    }
                }

                SetarInformacoesDocumentoBaixaMoedaEstrangeira(tituloBaixaAgrupadoDocumento, unitOfWork, usuario);
            }

            AtualizarTotaisTituloBaixaAgrupado(ref tituloBaixaAgrupado, unitOfWork);

            return tituloBaixaAgrupado;
        }

        public static void SetarInformacoesDocumentoBaixaMoedaEstrangeira(Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupadoDocumento tituloBaixaAgrupadoDocumento, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Usuario usuario)
        {
            if (!tituloBaixaAgrupadoDocumento.TituloBaixaAgrupado.TituloBaixa.MoedaCotacaoBancoCentral.HasValue ||
                tituloBaixaAgrupadoDocumento.TituloBaixaAgrupado.TituloBaixa.MoedaCotacaoBancoCentral == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral.Real)
                return;

            tituloBaixaAgrupadoDocumento.ValorPagoMoeda = tituloBaixaAgrupadoDocumento.TituloDocumento.ValorTotalMoeda;
            tituloBaixaAgrupadoDocumento.ValorTotalAPagarMoeda = tituloBaixaAgrupadoDocumento.TituloDocumento.ValorTotalMoeda;

            ProcessarVariacaoCambialDocumentoBaixa(tituloBaixaAgrupadoDocumento, unitOfWork, usuario);
        }

        public static void ProcessarVariacaoCambialDocumentoBaixa(Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupadoDocumento tituloBaixaAgrupadoDocumento, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Usuario usuario)
        {
            Repositorio.Embarcador.Financeiro.TituloBaixaAgrupadoDocumentoAcrescimoDesconto repTituloBaixaAgrupadoDocumentoAcrescimoDesconto = new Repositorio.Embarcador.Financeiro.TituloBaixaAgrupadoDocumentoAcrescimoDesconto(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraBaixaTituloReceberMoeda configuracaoFinanceiraBaixaTituloReceberMoeda = ObterConfiguracaoMoeda(tituloBaixaAgrupadoDocumento.TituloBaixaAgrupado.TituloBaixa.MoedaCotacaoBancoCentral.Value, unitOfWork);

            decimal diferenca = 0m;

            decimal valorDiferencaCotacao = tituloBaixaAgrupadoDocumento.TituloBaixaAgrupado.TituloBaixa.ValorMoedaCotacao - tituloBaixaAgrupadoDocumento.TituloDocumento.ValorCotacaoMoeda;

            if (valorDiferencaCotacao != 0m)
                diferenca = Math.Round(tituloBaixaAgrupadoDocumento.TituloDocumento.ValorMoeda * valorDiferencaCotacao, 2, MidpointRounding.ToEven);

            Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupadoDocumentoAcrescimoDesconto tituloBaixaAgrupadoDocumentoAcrescimoDesconto = repTituloBaixaAgrupadoDocumentoAcrescimoDesconto.BuscarPorTituloBaixaAgrupadoDocumentoComVariacaoCambial(tituloBaixaAgrupadoDocumento.Codigo);

            if (diferenca != 0m)
            {
                Dominio.Entidades.Embarcador.Fatura.Justificativa justificativa = null;

                if (diferenca > 0m)
                    justificativa = configuracaoFinanceiraBaixaTituloReceberMoeda.JustificativaAcrescimo;
                else if (diferenca < 0m)
                {
                    justificativa = configuracaoFinanceiraBaixaTituloReceberMoeda.JustificativaDesconto;
                    diferenca = -diferenca;
                }

                if (!AdicionarValorAoDocumento(out string erro, tituloBaixaAgrupadoDocumento, justificativa, diferenca, $"Referente à diferença da cotação do {tituloBaixaAgrupadoDocumento.TituloBaixaAgrupado.TituloBaixa.MoedaCotacaoBancoCentral.Value.ObterDescricaoSimplificada()}.", unitOfWork, usuario, tituloBaixaAgrupadoDocumentoAcrescimoDesconto, true))
                    throw new Exception(erro);
            }
            else if (tituloBaixaAgrupadoDocumentoAcrescimoDesconto != null)
            {
                if (!RemoverValorDoDocumento(out string erro, tituloBaixaAgrupadoDocumento, tituloBaixaAgrupadoDocumentoAcrescimoDesconto, unitOfWork))
                    throw new Exception(erro);
            }
        }

        public static void AtualizarTotaisTituloBaixa(ref TituloBaixa tituloBaixa, UnitOfWork unidadeTrabalho, bool atualizar = true)
        {
            Repositorio.Embarcador.Financeiro.TituloBaixaAgrupado repTituloBaixaAgrupado = new Repositorio.Embarcador.Financeiro.TituloBaixaAgrupado(unidadeTrabalho);
            Repositorio.Embarcador.Financeiro.TituloBaixa repTituloBaixa = new Repositorio.Embarcador.Financeiro.TituloBaixa(unidadeTrabalho);

            Dominio.ObjetosDeValor.Embarcador.Financeiro.ValoresSumarizadosBaixaTitulo valoresSumarizados = repTituloBaixaAgrupado.ObterValoresSumarizadosPorTituloBaixa(tituloBaixa.Codigo);

            tituloBaixa.ValorPago = valoresSumarizados?.ValorPago ?? 0m;// repTituloBaixaAgrupado.ObterValorPagoPorTituloBaixa(tituloBaixa.Codigo);
            tituloBaixa.ValorAcrescimo = valoresSumarizados?.ValorAcrescimo ?? 0m; // repTituloBaixaAgrupado.ObterValorAcrescimoPorTituloBaixa(tituloBaixa.Codigo);
            tituloBaixa.ValorDesconto = valoresSumarizados?.ValorDesconto ?? 0m; // repTituloBaixaAgrupado.ObterValorDescontoPorTituloBaixa(tituloBaixa.Codigo);
            tituloBaixa.ValorTotalAPagar = valoresSumarizados?.ValorTotalAPagar ?? 0m; // repTituloBaixaAgrupado.ObterValorTotalAPagarPorTituloBaixa(tituloBaixa.Codigo);
            tituloBaixa.Valor = valoresSumarizados?.Valor ?? 0m; // repTituloBaixa.ObterValorTotalTitulosPorTituloBaixa(tituloBaixa.Codigo);
            tituloBaixa.ValorPagoMoeda = valoresSumarizados?.ValorPagoMoeda ?? 0m;
            tituloBaixa.ValorAcrescimoMoeda = valoresSumarizados?.ValorAcrescimoMoeda ?? 0m;
            tituloBaixa.ValorDescontoMoeda = valoresSumarizados?.ValorDescontoMoeda ?? 0m;
            tituloBaixa.ValorTotalAPagarMoeda = valoresSumarizados?.ValorTotalAPagarMoeda ?? 0m;
            tituloBaixa.ValorOriginalMoedaEstrangeira = valoresSumarizados?.ValorMoeda ?? 0m;

            if (atualizar)
                repTituloBaixa.Atualizar(tituloBaixa);
        }

        public static void AtualizarTotaisTituloBaixaAgrupado(ref TituloBaixaAgrupado tituloBaixaAgrupado, UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Financeiro.TituloBaixaAgrupado repTituloBaixaAgrupado = new Repositorio.Embarcador.Financeiro.TituloBaixaAgrupado(unidadeTrabalho);
            Repositorio.Embarcador.Financeiro.TituloBaixaAgrupadoDocumento repTituloBaixaAgrupadoDocumento = new Repositorio.Embarcador.Financeiro.TituloBaixaAgrupadoDocumento(unidadeTrabalho);

            Dominio.ObjetosDeValor.Embarcador.Financeiro.ValoresSumarizadosBaixaTitulo valoresSumarizados = repTituloBaixaAgrupadoDocumento.ObterValoresSumarizadosPorTituloBaixaAgrupado(tituloBaixaAgrupado.Codigo);

            tituloBaixaAgrupado.ValorPago = valoresSumarizados?.ValorPago ?? 0m; //repTituloBaixaAgrupadoDocumento.ObterValorPagoPorTituloBaixaAgrupado(tituloBaixaAgrupado.Codigo);
            tituloBaixaAgrupado.ValorAcrescimo = valoresSumarizados?.ValorAcrescimo ?? 0m; //repTituloBaixaAgrupadoDocumento.ObterValorAcrescimoPorTituloBaixaAgrupado(tituloBaixaAgrupado.Codigo);
            tituloBaixaAgrupado.ValorDesconto = valoresSumarizados?.ValorDesconto ?? 0m; //repTituloBaixaAgrupadoDocumento.ObterValorDescontoPorTituloBaixaAgrupado(tituloBaixaAgrupado.Codigo);
            tituloBaixaAgrupado.ValorTotalAPagar = valoresSumarizados?.ValorTotalAPagar ?? 0m; //repTituloBaixaAgrupadoDocumento.ObterValorTotalAPagarPorTituloBaixaAgrupado(tituloBaixaAgrupado.Codigo);
            tituloBaixaAgrupado.ValorPagoMoeda = valoresSumarizados?.ValorPagoMoeda ?? 0m;
            tituloBaixaAgrupado.ValorAcrescimoMoeda = valoresSumarizados?.ValorAcrescimoMoeda ?? 0m;
            tituloBaixaAgrupado.ValorDescontoMoeda = valoresSumarizados?.ValorDescontoMoeda ?? 0m;
            tituloBaixaAgrupado.ValorTotalAPagarMoeda = valoresSumarizados?.ValorTotalAPagarMoeda ?? 0m;

            repTituloBaixaAgrupado.Atualizar(tituloBaixaAgrupado);
        }

        public static dynamic ObterDetalhesTituloBaixa(TituloBaixa tituloBaixa, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Financeiro.TituloBaixa repTituloBaixa = new Repositorio.Embarcador.Financeiro.TituloBaixa(unidadeTrabalho);

            return new
            {
                CodigoFatura = 0,
                NumeroFatura = "0",
                Codigo = tituloBaixa.Codigo,
                Etapa = tituloBaixa.SituacaoBaixaTitulo,
                ParcelaTitulo = "1",
                Valor = tituloBaixa.Valor.ToString("n2"),
                ValorAcrescimo = tituloBaixa.ValorAcrescimo.ToString("n2"),
                ValorDesconto = tituloBaixa.ValorDesconto.ToString("n2"),
                ValorTotalAPagar = tituloBaixa.ValorTotalAPagar.ToString("n2"),
                ValorPago = tituloBaixa.ValorPago.ToString("n2"),
                SaldoPendente = (tituloBaixa.ValorTotalAPagar - tituloBaixa.ValorPago).ToString("n2"),
                DataBaixa = tituloBaixa.DataBaixa.Value.ToString("dd/MM/yyyy"),
                DataBase = tituloBaixa.DataBase.Value.ToString("dd/MM/yyyy"),
                Observacao = tituloBaixa.Observacao,
                DescricaoSituacao = tituloBaixa.DescricaoSituacaoBaixaTitulo,
                TituloDeAgrupamento = repTituloBaixa.ContemTitulosGeradosDeNegociacao(tituloBaixa.Codigo) ? "Atenção! Contem título(s) gerado(s) a partir de uma negociação" : string.Empty,
                DescricaoGrupoPessoas = tituloBaixa.GrupoPessoas?.Descricao ?? string.Empty,
                NomePessoa = tituloBaixa.Pessoa?.Nome ?? "MÚLTIPLOS CLIENTES",
                Moeda = tituloBaixa.MoedaCotacaoBancoCentral ?? Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral.Real,
                ValorCotacaoMoeda = tituloBaixa.ValorMoedaCotacao.ToString("n10"),
                ValorMoeda = tituloBaixa.ValorOriginalMoedaEstrangeira.ToString("n2"),
                ValorAcrescimoMoeda = tituloBaixa.ValorAcrescimoMoeda.ToString("n2"),
                ValorDescontoMoeda = tituloBaixa.ValorDescontoMoeda.ToString("n2"),
                ValorTotalAPagarMoeda = tituloBaixa.ValorTotalAPagarMoeda.ToString("n2"),
                ValorPagoMoeda = tituloBaixa.ValorPagoMoeda.ToString("n2"),
                SaldoPendenteMoeda = (tituloBaixa.ValorTotalAPagarMoeda - tituloBaixa.ValorPagoMoeda).ToString("n2")
            };
        }

        public static bool BaixarTitulo(out string erro, Dominio.Entidades.Embarcador.Financeiro.TituloBaixa tituloBaixa, Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado tituloBaixaAgrupado, string observacaoBaixa, List<string> numerosCheques, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Financeiro.Bordero bordero, bool controlarTransacao, int countDocumentosTitulosBaixar, ref int countDocumentosTitulosBaixados)
        {
            Repositorio.Embarcador.Financeiro.TituloBaixaAgrupadoDocumento repTituloBaixaAgrupadoDocumento = new Repositorio.Embarcador.Financeiro.TituloBaixaAgrupadoDocumento(unitOfWork);

            if (tituloBaixaAgrupado.Titulo.StatusTitulo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.EmAberto)
            {
                countDocumentosTitulosBaixados += repTituloBaixaAgrupadoDocumento.ContarPorTituloBaixaAgrupado(tituloBaixaAgrupado.Codigo);

                erro = string.Empty;
                return true;
            }

            Repositorio.Embarcador.Financeiro.TituloBaixaAgrupado repTituloBaixaAgrupado = new Repositorio.Embarcador.Financeiro.TituloBaixaAgrupado(unitOfWork);
            Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);
            Repositorio.Embarcador.Financeiro.DocumentoFaturamento repDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(unitOfWork);
            Repositorio.Embarcador.Financeiro.TituloDocumento repTituloDocumento = new Repositorio.Embarcador.Financeiro.TituloDocumento(unitOfWork);
            Repositorio.Embarcador.Financeiro.TituloBaixaAgrupadoDocumentoAcrescimoDesconto repTituloBaixaAgrupadoDocumentoAcrescimoDesconto = new Repositorio.Embarcador.Financeiro.TituloBaixaAgrupadoDocumentoAcrescimoDesconto(unitOfWork);
            Repositorio.Embarcador.Financeiro.TituloDocumentoAcrescimoDesconto repTituloDocumentoAcrescimoDesconto = new Repositorio.Embarcador.Financeiro.TituloDocumentoAcrescimoDesconto(unitOfWork);

            Servicos.Embarcador.Financeiro.Titulo servicoTitulo = new Servicos.Embarcador.Financeiro.Titulo(unitOfWork);
            Hubs.BaixaTituloReceber servicoNotificacaoBaixaTituloReceber = new Hubs.BaixaTituloReceber();

            List<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupadoDocumento> tituloBaixaAgrupadoDocumentos = repTituloBaixaAgrupadoDocumento.BuscarPorTituloBaixaAgrupado(tituloBaixaAgrupado.Codigo);

            string observacaoAdicionalMovimentacao = string.Empty;

            if (!string.IsNullOrWhiteSpace(tituloBaixaAgrupado.Titulo.Observacao))
                observacaoAdicionalMovimentacao += " " + tituloBaixaAgrupado.Titulo.Observacao + ".";

            if (!string.IsNullOrWhiteSpace(observacaoBaixa))
                observacaoAdicionalMovimentacao += " " + observacaoBaixa;

            if (numerosCheques != null && numerosCheques.Count > 0)
                observacaoAdicionalMovimentacao += " Cheque(s) nº: " + string.Join(", ", numerosCheques);

            List<string> numerosDocumentos = tituloBaixaAgrupadoDocumentos.Where(o => o.TituloDocumento != null && o.TituloDocumento.TipoDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoTitulo.CTe && o.TituloDocumento.CTe != null).Select(o => o.TituloDocumento.CTe.Numero.ToString()).ToList();
            numerosDocumentos.AddRange(tituloBaixaAgrupadoDocumentos.Where(o => o.TituloDocumento != null && o.TituloDocumento.TipoDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoTitulo.Carga && o.TituloDocumento.Carga != null).Select(o => o.TituloDocumento.Carga.CodigoCargaEmbarcador).ToList());

            if (numerosDocumentos != null && numerosDocumentos.Count > 0)
                observacaoAdicionalMovimentacao += " Doc(s): " + string.Join(", ", numerosDocumentos);

            Servicos.Embarcador.Financeiro.ProcessoMovimento svcProcessoMovimento = new Servicos.Embarcador.Financeiro.ProcessoMovimento();

            foreach (Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupadoDocumento tituloBaixaAgrupadoDocumento in tituloBaixaAgrupadoDocumentos)
            {
                countDocumentosTitulosBaixados++;

                if (tituloBaixaAgrupadoDocumento.BaixaFinalizada)
                    continue;

                Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento documentoFaturamento = null;

                if (tituloBaixaAgrupadoDocumento.TituloDocumento.TipoDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoTitulo.CTe)
                    documentoFaturamento = repDocumentoFaturamento.BuscarPorCTe(tituloBaixaAgrupadoDocumento.TituloDocumento.CTe.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLiquidacao.Fatura);
                else
                    documentoFaturamento = repDocumentoFaturamento.BuscarPorCarga(tituloBaixaAgrupadoDocumento.TituloDocumento.Carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLiquidacao.Fatura);

                if (controlarTransacao)
                    unitOfWork.Start();

                tituloBaixaAgrupadoDocumento.TituloDocumento.ValorPago = tituloBaixaAgrupadoDocumento.ValorPago;

                decimal valorPago = tituloBaixaAgrupadoDocumento.ValorPago + tituloBaixaAgrupadoDocumento.ValorDesconto + tituloBaixaAgrupadoDocumento.TituloDocumento.ValorDesconto;
                decimal valorTotalAcrescimo = tituloBaixaAgrupadoDocumento.TituloDocumento.ValorAcrescimo + tituloBaixaAgrupadoDocumento.ValorAcrescimo;

                if (valorTotalAcrescimo <= valorPago)
                {
                    tituloBaixaAgrupadoDocumento.TituloDocumento.ValorPagoAcrescimo = valorTotalAcrescimo;
                    valorPago -= valorTotalAcrescimo;
                }
                else
                {
                    tituloBaixaAgrupadoDocumento.TituloDocumento.ValorPagoAcrescimo = valorPago;
                    valorPago = 0m;
                }

                tituloBaixaAgrupadoDocumento.TituloDocumento.ValorPagoDocumento = valorPago;
                tituloBaixaAgrupadoDocumento.TituloDocumento.ValorPendente = 0m;
                tituloBaixaAgrupadoDocumento.TituloDocumento.ValorAcrescimoBaixa = tituloBaixaAgrupadoDocumento.ValorAcrescimo;
                tituloBaixaAgrupadoDocumento.TituloDocumento.ValorDescontoBaixa = tituloBaixaAgrupadoDocumento.ValorDesconto;

                repTituloDocumento.Atualizar(tituloBaixaAgrupadoDocumento.TituloDocumento);

                if (documentoFaturamento != null)
                {
                    documentoFaturamento.ValorAcrescimo += tituloBaixaAgrupadoDocumento.ValorAcrescimo;
                    documentoFaturamento.ValorDesconto += tituloBaixaAgrupadoDocumento.ValorDesconto;
                    documentoFaturamento.ValorPago += tituloBaixaAgrupadoDocumento.ValorPago;

                    repDocumentoFaturamento.Atualizar(documentoFaturamento);
                }

                List<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupadoDocumentoAcrescimoDesconto> acrescimosDescontos = repTituloBaixaAgrupadoDocumentoAcrescimoDesconto.BuscarPorTituloBaixaAgrupadoDocumento(tituloBaixaAgrupadoDocumento.Codigo);

                foreach (Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupadoDocumentoAcrescimoDesconto acrescimoDesconto in acrescimosDescontos)
                {
                    Dominio.Entidades.Embarcador.Financeiro.TituloDocumentoAcrescimoDesconto acrescimoDescontoTituloDocumento = new Dominio.Entidades.Embarcador.Financeiro.TituloDocumentoAcrescimoDesconto()
                    {
                        Justificativa = acrescimoDesconto.Justificativa,
                        Observacao = acrescimoDesconto.Observacao,
                        Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EnumTipoAcrescimoDescontoTituloDocumento.Baixa,
                        TipoJustificativa = acrescimoDesconto.TipoJustificativa,
                        TipoMovimentoReversao = acrescimoDesconto.TipoMovimentoReversao,
                        TipoMovimentoUso = acrescimoDesconto.TipoMovimentoUso,
                        TituloDocumento = tituloBaixaAgrupadoDocumento.TituloDocumento,
                        Valor = acrescimoDesconto.Valor,
                        Usuario = tituloBaixaAgrupadoDocumento.Usuario
                    };

                    repTituloDocumentoAcrescimoDesconto.Inserir(acrescimoDescontoTituloDocumento);

                    if (acrescimoDesconto.TipoJustificativa == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoJustificativa.Desconto)
                    {
                        if (!svcProcessoMovimento.GerarMovimentacao(out erro, acrescimoDesconto.TipoMovimentoUso, tituloBaixaAgrupado.DataBaixa, acrescimoDesconto.Valor, tituloBaixaAgrupado.Titulo.Codigo.ToString(), "Referente ao desconto concedido na baixa do documento " + tituloBaixaAgrupadoDocumento.TituloDocumento.NumeroDocumento + " do título " + tituloBaixaAgrupado.Titulo.Codigo + "." + observacaoAdicionalMovimentacao, unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Recebimento, tipoServicoMultisoftware, 0, tituloBaixaAgrupado.Titulo.TipoMovimento?.PlanoDeContaDebito ?? acrescimoDesconto.TipoMovimentoUso.PlanoDeContaCredito, acrescimoDesconto.TipoMovimentoUso.PlanoDeContaDebito, tituloBaixaAgrupado.Titulo.Codigo, null, tituloBaixaAgrupado.Titulo.Pessoa, tituloBaixaAgrupado.Titulo.GrupoPessoas, tituloBaixaAgrupado.DataBase, null, acrescimoDesconto.TipoMovimentoUso.ContasExportacao.ToList(), acrescimoDesconto, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoExportacao.AcrescimoDescontoBaixaTituloReceber, tituloBaixa.MoedaCotacaoBancoCentral, tituloBaixa.DataBaseCRT, tituloBaixa.ValorMoedaCotacao, acrescimoDesconto.ValorMoeda))
                            return false;
                    }
                    else
                    {
                        if (!svcProcessoMovimento.GerarMovimentacao(out erro, acrescimoDesconto.TipoMovimentoUso, tituloBaixaAgrupado.DataBaixa, acrescimoDesconto.Valor, tituloBaixaAgrupado.Titulo.Codigo.ToString(), "Referente ao acréscimo concedido na baixa do documento " + tituloBaixaAgrupadoDocumento.TituloDocumento.NumeroDocumento + " do título " + tituloBaixaAgrupado.Titulo.Codigo + "." + observacaoAdicionalMovimentacao, unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Recebimento, tipoServicoMultisoftware, 0, acrescimoDesconto.TipoMovimentoUso.PlanoDeContaCredito, tituloBaixaAgrupado.Titulo.TipoMovimento?.PlanoDeContaDebito ?? acrescimoDesconto.TipoMovimentoUso.PlanoDeContaDebito, tituloBaixaAgrupado.Titulo.Codigo, null, tituloBaixaAgrupado.Titulo.Pessoa, tituloBaixaAgrupado.Titulo.GrupoPessoas, tituloBaixaAgrupado.DataBase, null, acrescimoDesconto.TipoMovimentoUso.ContasExportacao.ToList(), acrescimoDesconto, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoExportacao.AcrescimoDescontoBaixaTituloReceber, tituloBaixa.MoedaCotacaoBancoCentral, tituloBaixa.DataBaseCRT, tituloBaixa.ValorMoedaCotacao, acrescimoDesconto.ValorMoeda))
                            return false;
                    }

                    GerarSolicitacaoAvaria(acrescimoDescontoTituloDocumento, tituloBaixaAgrupadoDocumento, unitOfWork);
                }

                if (documentoFaturamento?.Empresa?.ProvisionarDocumentos ?? false)
                    Servicos.Embarcador.Financeiro.DocumentoFaturamento.AdicionarDocumentoProvisao(documentoFaturamento, unitOfWork);

                tituloBaixaAgrupadoDocumento.BaixaFinalizada = true;

                repTituloBaixaAgrupadoDocumento.SetarBaixaFinalizada(tituloBaixaAgrupadoDocumento.Codigo, true);

                if (controlarTransacao)
                {
                    unitOfWork.CommitChanges();

                    unitOfWork.FlushAndClear();
                }

                if (countDocumentosTitulosBaixar > 0 && (countDocumentosTitulosBaixar <= 10 || (countDocumentosTitulosBaixados % 5) == 0))
                    servicoNotificacaoBaixaTituloReceber.InformarQuantidadeTitulosFinalizados(tituloBaixa.Codigo, countDocumentosTitulosBaixar, countDocumentosTitulosBaixados);
            }

            if (controlarTransacao)
            {
                tituloBaixaAgrupado = repTituloBaixaAgrupado.BuscarPorCodigo(tituloBaixaAgrupado.Codigo);

                unitOfWork.Start();
            }

            tituloBaixaAgrupado.Titulo.DataLiquidacao = tituloBaixaAgrupado.DataBaixa;
            tituloBaixaAgrupado.Titulo.DataBaseLiquidacao = tituloBaixaAgrupado.DataBase;
            tituloBaixaAgrupado.Titulo.StatusTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.Quitada;
            tituloBaixaAgrupado.Titulo.DataAlteracao = DateTime.Now;
            tituloBaixaAgrupado.Titulo.ValorPendente = 0;
            tituloBaixaAgrupado.Titulo.ValorAcrescimoBaixa = tituloBaixaAgrupado.ValorAcrescimo;
            tituloBaixaAgrupado.Titulo.ValorDescontoBaixa = tituloBaixaAgrupado.ValorDesconto;
            tituloBaixaAgrupado.Titulo.ValorPago = tituloBaixaAgrupado.ValorPago;
            tituloBaixaAgrupado.Titulo.Bordero = bordero;

            repTitulo.Atualizar(tituloBaixaAgrupado.Titulo);

            servicoTitulo.RemoverTituloBloqueioFinanceiroPessoa(tituloBaixaAgrupado.Titulo, tipoServicoMultisoftware);

            if (tituloBaixaAgrupado.Titulo.ValorPago > 0m)
            {
                if (!svcProcessoMovimento.GerarMovimentacao(out erro, null, tituloBaixaAgrupado.DataBaixa, tituloBaixaAgrupado.Titulo.ValorPago, tituloBaixaAgrupado.Titulo.Codigo.ToString(), observacaoAdicionalMovimentacao + " Referente à baixa do título " + tituloBaixaAgrupado.Titulo.Codigo + ".", unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Recebimento, tipoServicoMultisoftware, 0, tituloBaixaAgrupado.Titulo.TipoMovimento?.PlanoDeContaDebito, tituloBaixaAgrupado.TituloBaixa.TipoPagamentoRecebimento?.PlanoConta, tituloBaixaAgrupado.Titulo.Codigo, null, tituloBaixaAgrupado.Titulo.Pessoa, tituloBaixaAgrupado.Titulo.GrupoPessoas, tituloBaixaAgrupado.DataBase, null, tituloBaixaAgrupado.TituloBaixa.TipoPagamentoRecebimento?.ContasExportacao?.Where(o => o.Reversao == null || o.Reversao == false).ToList(), tituloBaixaAgrupado, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoExportacao.BaixaTituloReceber, tituloBaixa.MoedaCotacaoBancoCentral, tituloBaixa.DataBaseCRT, tituloBaixa.ValorMoedaCotacao, tituloBaixaAgrupado.ValorPagoMoeda))
                    return false;
            }

            if (controlarTransacao)
            {
                unitOfWork.CommitChanges();

                unitOfWork.FlushAndClear();
            }

            erro = "";
            return true;
        }

        public static dynamic ValidarBaixaTitulo(Dominio.Entidades.Embarcador.Financeiro.TituloBaixa tituloBaixa, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Repositorio.Embarcador.Financeiro.TituloBaixa repTituloBaixa = new Repositorio.Embarcador.Financeiro.TituloBaixa(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            if (!configuracaoTMS.NaoValidarDataCancelamentoTituloNaBaixaTituloReceberPorCTe)
            {
                IList<Dominio.ObjetosDeValor.Embarcador.Financeiro.TituloCancelamento> titulos = repTituloBaixa.ObterTitulosComDataCancelamentoSuperiorABaixa(tituloBaixa.Codigo);

                if (titulos.Count > 0)
                {
                    DateTime maiorDataCancelamento = titulos.Max(o => o.DataCancelamento.Value);
                    return new
                    {
                        Valido = false,
                        PermiteFinalizarBaixa = false,
                        Mensagem = "Existem documentos com títulos cancelados. A data da baixa (" + tituloBaixa.DataBaixa.Value.ToString("dd/MM/yyyy") + ") não pode ser menor que a data de cancelamento dos títulos (" + maiorDataCancelamento.ToString("dd/MM/yyyy") + "). Títulos: " + string.Join(", ", titulos.Select(o => o.Codigo)) + "."
                    };
                }
            }

            Repositorio.Embarcador.Financeiro.TituloBaixaCheque repTituloBaixaCheque = new Repositorio.Embarcador.Financeiro.TituloBaixaCheque(unitOfWork);

            decimal valorTotalCheques = repTituloBaixaCheque.ObterValorTotalChequePorTituloBaixa(tituloBaixa.Codigo);

            if (valorTotalCheques > tituloBaixa.ValorTotalAPagar)
            {
                return new
                {
                    Valido = false,
                    PermiteFinalizarBaixa = false,
                    Mensagem = "O valor total dos cheques deve ser menor ou igual ao valor total a pagar."
                };
            }

            return new
            {
                Valido = true,
                PermiteFinalizarBaixa = true,
                Mensagem = string.Empty
            };
        }

        public static bool ReverterBaixaTitulo(out string erro, Dominio.Entidades.Embarcador.Financeiro.TituloBaixa tituloBaixa, Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado tituloBaixaAgrupado, List<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupadoDocumento> tituloBaixaAgrupadoDocumentosGeral, List<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento> documentosFaturamentoGeral, List<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupadoDocumentoAcrescimoDesconto> acrescimosDescontosGeral, Servicos.Embarcador.Financeiro.ProcessoMovimento svcProcessoMovimento, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);
            Repositorio.Embarcador.Financeiro.DocumentoFaturamento repDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(unitOfWork);
            Repositorio.Embarcador.Financeiro.TituloBaixaAgrupadoDocumento repTituloBaixaAgrupadoDocumento = new Repositorio.Embarcador.Financeiro.TituloBaixaAgrupadoDocumento(unitOfWork);
            Repositorio.Embarcador.Financeiro.TituloDocumento repTituloDocumento = new Repositorio.Embarcador.Financeiro.TituloDocumento(unitOfWork);
            Repositorio.Embarcador.Financeiro.TituloBaixaAgrupadoDocumentoAcrescimoDesconto repTituloBaixaAgrupadoDocumentoAcrescimoDesconto = new Repositorio.Embarcador.Financeiro.TituloBaixaAgrupadoDocumentoAcrescimoDesconto(unitOfWork);
            Repositorio.Embarcador.Financeiro.TituloDocumentoAcrescimoDesconto repTituloDocumentoAcrescimoDesconto = new Repositorio.Embarcador.Financeiro.TituloDocumentoAcrescimoDesconto(unitOfWork);

            tituloBaixaAgrupado.Titulo.DataLiquidacao = null;
            tituloBaixaAgrupado.Titulo.StatusTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.EmAberto;
            tituloBaixaAgrupado.Titulo.DataAlteracao = DateTime.Now;
            tituloBaixaAgrupado.Titulo.ValorPendente = tituloBaixaAgrupado.Titulo.ValorOriginal;
            tituloBaixaAgrupado.Titulo.ValorAcrescimoBaixa = 0;
            tituloBaixaAgrupado.Titulo.ValorDescontoBaixa = 0;
            tituloBaixaAgrupado.Titulo.ValorPago = 0;
            tituloBaixaAgrupado.Titulo.Bordero = null;

            repTitulo.Atualizar(tituloBaixaAgrupado.Titulo);

            List<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupadoDocumento> tituloBaixaAgrupadoDocumentos = tituloBaixaAgrupadoDocumentosGeral.Where(o => o.TituloBaixaAgrupado.Codigo == tituloBaixaAgrupado.Codigo).ToList();

            foreach (Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupadoDocumento tituloBaixaAgrupadoDocumento in tituloBaixaAgrupadoDocumentos)
            {
                Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento documentoFaturamento = null;

                if (tituloBaixaAgrupadoDocumento.TituloDocumento.TipoDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoTitulo.CTe)
                    documentoFaturamento = documentosFaturamentoGeral.Where(o => o.CTe != null && o.CTe.Codigo == tituloBaixaAgrupadoDocumento.TituloDocumento.CTe.Codigo).FirstOrDefault();
                else
                    documentoFaturamento = documentosFaturamentoGeral.Where(o => o.Carga != null && o.Carga.Codigo == tituloBaixaAgrupadoDocumento.TituloDocumento.Carga.Codigo).FirstOrDefault();

                tituloBaixaAgrupadoDocumento.TituloDocumento.ValorPendente = tituloBaixaAgrupadoDocumento.TituloDocumento.ValorTotal;
                tituloBaixaAgrupadoDocumento.TituloDocumento.ValorPago = 0;
                tituloBaixaAgrupadoDocumento.TituloDocumento.ValorPagoDocumento = 0;
                tituloBaixaAgrupadoDocumento.TituloDocumento.ValorPagoAcrescimo = 0;
                tituloBaixaAgrupadoDocumento.TituloDocumento.ValorAcrescimoBaixa = 0;
                tituloBaixaAgrupadoDocumento.TituloDocumento.ValorDescontoBaixa = 0;

                repTituloDocumento.Atualizar(tituloBaixaAgrupadoDocumento.TituloDocumento);

                documentoFaturamento.ValorAcrescimo -= tituloBaixaAgrupadoDocumento.ValorAcrescimo;
                documentoFaturamento.ValorDesconto -= tituloBaixaAgrupadoDocumento.ValorDesconto;
                documentoFaturamento.ValorPago -= tituloBaixaAgrupadoDocumento.ValorPago;

                repDocumentoFaturamento.Atualizar(documentoFaturamento);

                if (documentoFaturamento.Empresa?.ProvisionarDocumentos ?? false)
                {
                    if (documentoFaturamento.TipoDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoFaturamento.CTe)
                        Servicos.Embarcador.Financeiro.DocumentoFaturamento.RemoverDocumentoProvisao(documentoFaturamento.CTe, unitOfWork);
                    else if (documentoFaturamento.TipoDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoFaturamento.Carga)
                        Servicos.Embarcador.Financeiro.DocumentoFaturamento.RemoverDocumentoProvisao(documentoFaturamento.Carga, unitOfWork);
                }

                List<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupadoDocumentoAcrescimoDesconto> acrescimosDescontos = acrescimosDescontosGeral.Where(o => o.TituloBaixaAgrupadoDocumento.Codigo == tituloBaixaAgrupadoDocumento.Codigo).ToList();

                foreach (Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupadoDocumentoAcrescimoDesconto acrescimoDesconto in acrescimosDescontos)
                {
                    if (acrescimoDesconto.TipoJustificativa == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoJustificativa.Desconto)
                    {
                        if (!svcProcessoMovimento.GerarMovimentacao(out erro, null, tituloBaixaAgrupado.DataBaixa, acrescimoDesconto.Valor, tituloBaixaAgrupado.Titulo.Codigo.ToString(), "Referente à reversão do desconto concedido na baixa do documento " + tituloBaixaAgrupadoDocumento.TituloDocumento.NumeroDocumento + " do título " + tituloBaixaAgrupado.Titulo.Codigo + ".", unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Recebimento, tipoServicoMultisoftware, 0, acrescimoDesconto.TipoMovimentoUso.PlanoDeContaDebito, tituloBaixaAgrupado.Titulo.TipoMovimento.PlanoDeContaDebito, 0, null, tituloBaixaAgrupado.Titulo.Pessoa, tituloBaixaAgrupado.Titulo.GrupoPessoas, tituloBaixaAgrupado.DataBase, null, acrescimoDesconto.TipoMovimentoReversao.Exportar ? acrescimoDesconto.TipoMovimentoReversao.ContasExportacao.ToList() : new List<ConfiguracaoContaExportacao>(), acrescimoDesconto, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoExportacao.AcrescimoDescontoBaixaTituloReceber, tituloBaixa.MoedaCotacaoBancoCentral, tituloBaixa.DataBase, tituloBaixa.ValorMoedaCotacao, acrescimoDesconto.ValorMoeda))
                            return false;
                    }
                    else
                    {
                        if (!svcProcessoMovimento.GerarMovimentacao(out erro, null, tituloBaixaAgrupado.DataBaixa, acrescimoDesconto.Valor, tituloBaixaAgrupado.Titulo.Codigo.ToString(), "Referente à reversão do acréscimo concedido na baixa do documento " + tituloBaixaAgrupadoDocumento.TituloDocumento.NumeroDocumento + " do título " + tituloBaixaAgrupado.Titulo.Codigo + ".", unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Recebimento, tipoServicoMultisoftware, 0, tituloBaixaAgrupado.Titulo.TipoMovimento.PlanoDeContaDebito, acrescimoDesconto.TipoMovimentoUso.PlanoDeContaCredito, 0, null, tituloBaixaAgrupado.Titulo.Pessoa, tituloBaixaAgrupado.Titulo.GrupoPessoas, tituloBaixaAgrupado.DataBase, null, acrescimoDesconto.TipoMovimentoReversao.Exportar ? acrescimoDesconto.TipoMovimentoReversao.ContasExportacao.ToList() : new List<ConfiguracaoContaExportacao>(), acrescimoDesconto, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoExportacao.AcrescimoDescontoBaixaTituloReceber, tituloBaixa.MoedaCotacaoBancoCentral, tituloBaixa.DataBase, tituloBaixa.ValorMoedaCotacao, acrescimoDesconto.ValorMoeda))
                            return false;
                    }
                }
            }

            if (tituloBaixaAgrupado.ValorPago > 0m)
            {
                if (!svcProcessoMovimento.GerarMovimentacao(out erro, null, tituloBaixaAgrupado.DataBaixa, tituloBaixaAgrupado.ValorPago, tituloBaixaAgrupado.Titulo.Codigo.ToString(), "Referente à reversão da baixa do título " + tituloBaixaAgrupado.Titulo.Codigo, unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Recebimento, tipoServicoMultisoftware, 0, tituloBaixa.TipoPagamentoRecebimento?.PlanoConta, tituloBaixaAgrupado.Titulo.TipoMovimento.PlanoDeContaDebito, 0, null, tituloBaixaAgrupado.Titulo.Pessoa, tituloBaixaAgrupado.Titulo.GrupoPessoas, tituloBaixaAgrupado.DataBase, null, (tituloBaixa.TipoPagamentoRecebimento?.Exportar ?? false) ? tituloBaixa.TipoPagamentoRecebimento?.ContasExportacao.Where(o => o.Reversao == true).ToList() : new List<ConfiguracaoContaExportacao>(), tituloBaixaAgrupado, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoExportacao.CancelamentoBaixaTituloReceber, tituloBaixa.MoedaCotacaoBancoCentral, tituloBaixa.DataBaseCRT, tituloBaixa.ValorMoedaCotacao, tituloBaixaAgrupado.ValorPagoMoeda))
                    return false;
            }

            erro = string.Empty;
            return true;
        }

        public static bool CancelarBaixa(out string erro, Dominio.Entidades.Embarcador.Financeiro.TituloBaixa tituloBaixa, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);
            Repositorio.Embarcador.Financeiro.TituloBaixa repTituloBaixa = new Repositorio.Embarcador.Financeiro.TituloBaixa(unitOfWork);
            Repositorio.Embarcador.Financeiro.TituloBaixaAgrupado repTituloBaixaAgrupado = new Repositorio.Embarcador.Financeiro.TituloBaixaAgrupado(unitOfWork);
            Repositorio.Embarcador.Financeiro.FechamentoDiario repFechamentoDiario = new Repositorio.Embarcador.Financeiro.FechamentoDiario(unitOfWork);
            Repositorio.Embarcador.Financeiro.MovimentoFinanceiroDebitoCredito repMovimentoFinanceiroDebitoCredito = new Repositorio.Embarcador.Financeiro.MovimentoFinanceiroDebitoCredito(unitOfWork);
            Repositorio.Embarcador.Financeiro.TituloDocumentoAcrescimoDesconto repTituloDocumentoAcrescimoDesconto = new Repositorio.Embarcador.Financeiro.TituloDocumentoAcrescimoDesconto(unitOfWork);
            Repositorio.Embarcador.Financeiro.TituloBaixaAgrupadoDocumento repTituloBaixaAgrupadoDocumento = new Repositorio.Embarcador.Financeiro.TituloBaixaAgrupadoDocumento(unitOfWork);
            Repositorio.Embarcador.Financeiro.DocumentoFaturamento repDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(unitOfWork);
            Repositorio.Embarcador.Financeiro.TituloBaixaAgrupadoDocumentoAcrescimoDesconto repTituloBaixaAgrupadoDocumentoAcrescimoDesconto = new Repositorio.Embarcador.Financeiro.TituloBaixaAgrupadoDocumentoAcrescimoDesconto(unitOfWork);
            Repositorio.Embarcador.Fatura.Fatura repFatura = new Repositorio.Embarcador.Fatura.Fatura(unitOfWork);

            Servicos.Embarcador.Carga.CargaDadosSumarizados serCargaDadosSumarizados = new Servicos.Embarcador.Carga.CargaDadosSumarizados(unitOfWork);

            Servicos.Embarcador.Financeiro.ProcessoMovimento servProcessoMovimento = new Servicos.Embarcador.Financeiro.ProcessoMovimento(unitOfWork.StringConexao);
            Servicos.Embarcador.Financeiro.Titulo servicoTitulo = new Servicos.Embarcador.Financeiro.Titulo(unitOfWork);

            if (tituloBaixa.SituacaoBaixaTitulo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoBaixaTitulo.EmGeracao || tituloBaixa.SituacaoBaixaTitulo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoBaixaTitulo.EmFinalizacao)
            {
                erro = "Não é possível cancelar uma baixa que está em geração ou finalização.";
                return false;
            }

            if (tituloBaixa.SituacaoBaixaTitulo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoBaixaTitulo.Cancelada)
            {
                erro = "Esta baixa já se encontra cancelada.";
                return false;
            }

            if (repTituloBaixa.ContemParcelaQuitada(tituloBaixa.Codigo))
            {
                erro = "Esta baixa já possui parcela de negociação quitada.";
                return false;
            }

            if (repTituloBaixa.DocumentosContemPagamento(tituloBaixa.Codigo))
            {
                erro = "Os documentos desta baixa estão vinculados à um pagamento, não sendo possível cancelar.";
                return false;
            }

            if (repMovimentoFinanceiroDebitoCredito.MovimentacaoConcilidada(tituloBaixa.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Recebimento))
            {
                erro = "Não é possível reverter esta baixa pois as movimentações financeiras já estão conciliadas.";
                return false;
            }

            if (tituloBaixa.SituacaoBaixaTitulo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoBaixaTitulo.Finalizada)
            {
                List<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado> titulosBaixaAgrupado = repTituloBaixaAgrupado.BuscarPorBaixaTitulo(tituloBaixa.Codigo);

                if (titulosBaixaAgrupado.Any(o => o.Titulo.StatusTitulo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.Quitada))
                {
                    erro = "Não é possível reverter a baixa de um título não quitado.";
                    return false;
                }

                List<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento> documentosFaturamento = new List<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento>();

                List<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupadoDocumento> tituloBaixaAgrupadoDocumentos = repTituloBaixaAgrupadoDocumento.BuscarPorTituloBaixa(tituloBaixa.Codigo);
                List<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento> documentosFaturamentoCTe = repDocumentoFaturamento.BuscarPorTituloBaixaCTe(tituloBaixa.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLiquidacao.Fatura);
                List<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento> documentosFaturamentoCarga = repDocumentoFaturamento.BuscarPorTituloBaixaCarga(tituloBaixa.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLiquidacao.Fatura);
                List<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupadoDocumentoAcrescimoDesconto> acrescimosDescontos = repTituloBaixaAgrupadoDocumentoAcrescimoDesconto.BuscarPorTituloBaixa(tituloBaixa.Codigo);

                if (documentosFaturamentoCTe != null && documentosFaturamentoCTe.Count > 0)
                    documentosFaturamento.AddRange(documentosFaturamentoCTe);

                if (documentosFaturamentoCarga != null && documentosFaturamentoCarga.Count > 0)
                    documentosFaturamento.AddRange(documentosFaturamentoCarga);

                List<Dominio.Entidades.Embarcador.Financeiro.Titulo> listaTitulos = repTitulo.BuscarPorBaixaTitulo(tituloBaixa.Codigo);

                if (listaTitulos.Count > 0)
                {
                    DateTime? dataUltimoFechamento = repFechamentoDiario.ObterUltimaDataFechamento();

                    for (int i = 0; i < listaTitulos.Count; i++)
                    {
                        Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = listaTitulos[i];

                        if (dataUltimoFechamento.HasValue && dataUltimoFechamento.Value >= titulo.DataEmissao.Value)
                            titulo.DataCancelamento = dataUltimoFechamento.Value.AddDays(1);
                        else
                            titulo.DataCancelamento = titulo.DataEmissao;

                        titulo.ValorPendente = 0;
                        titulo.DataAlteracao = DateTime.Now;
                        titulo.DataCancelamento = titulo.DataEmissao;
                        titulo.StatusTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.Cancelado;

                        repTitulo.Atualizar(titulo);

                        servicoTitulo.RemoverTituloBloqueioFinanceiroPessoa(titulo, tipoServicoMultisoftware);
                    }
                }

                repTituloDocumentoAcrescimoDesconto.DeletarPorTituloBaixaETipo(tituloBaixa.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EnumTipoAcrescimoDescontoTituloDocumento.Baixa);

                List<int> codigosFaturas = new List<int>();

                for (int i = 0; i < titulosBaixaAgrupado.Count; i++)
                {
                    Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado tituloBaixaAgrupado = titulosBaixaAgrupado[i];

                    if (!Servicos.Embarcador.Financeiro.BaixaTituloReceber.ReverterBaixaTitulo(out erro, tituloBaixa, tituloBaixaAgrupado, tituloBaixaAgrupadoDocumentos, documentosFaturamento, acrescimosDescontos, servProcessoMovimento, unitOfWork, tipoServicoMultisoftware))
                        return false;

                    if (tituloBaixaAgrupado.Titulo?.FaturaParcela?.Fatura != null && tituloBaixaAgrupado.Titulo.FaturaParcela.Fatura.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFatura.Liquidado && !codigosFaturas.Contains(tituloBaixaAgrupado.Titulo.FaturaParcela.Fatura.Codigo))
                        codigosFaturas.Add(tituloBaixaAgrupado.Titulo.FaturaParcela.Fatura.Codigo);
                }

                for (int i = 0; i < codigosFaturas.Count; i++)
                {
                    Dominio.Entidades.Embarcador.Fatura.Fatura fatura = repFatura.BuscarPorCodigo(codigosFaturas[i]);

                    if (fatura.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFatura.Liquidado)
                    {
                        if (!repTitulo.ExisteTituloQuitadoPorFatura(fatura.Codigo))
                        {
                            fatura.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFatura.Fechado;
                            repFatura.Atualizar(fatura);

                            serCargaDadosSumarizados.AtualizarDadosCTesFaturados(fatura.Codigo, unitOfWork);
                        }
                    }
                }
            }

            tituloBaixa.SituacaoBaixaTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoBaixaTitulo.Cancelada;

            repTituloBaixa.Atualizar(tituloBaixa);

            erro = string.Empty;
            return true;
        }

        public static dynamic ValidarCancelamentoBaixa(Dominio.Entidades.Embarcador.Financeiro.TituloBaixa tituloBaixa, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            if (tituloBaixa != null && tituloBaixa.SituacaoBaixaTitulo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoBaixaTitulo.Finalizada)
            {
                Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);
                Repositorio.Embarcador.Financeiro.FechamentoDiario repFechamentoDiario = new Repositorio.Embarcador.Financeiro.FechamentoDiario(unitOfWork);

                List<Dominio.Entidades.Embarcador.Financeiro.Titulo> listaTitulos = repTitulo.BuscarPorBaixaTitulo(tituloBaixa.Codigo);

                if (listaTitulos.Count > 0)
                {
                    DateTime? dataUltimoFechamento = repFechamentoDiario.ObterUltimaDataFechamento();
                    DateTime maiorDataEmissaoTitulos = listaTitulos.Max(o => o.DataEmissao.Value);

                    if (dataUltimoFechamento.HasValue && dataUltimoFechamento.Value >= maiorDataEmissaoTitulos)
                    {
                        return new
                        {
                            Valido = false,
                            PermiteCancelarBaixa = true,
                            Mensagem = "Existe uma data de fechamento (" + dataUltimoFechamento.Value.ToString("dd/MM/yyyy") + ") posterior à data de emissão dos títulos renegociados (" + maiorDataEmissaoTitulos.ToString("dd/MM/yyyy") + "). O cancelamento dos títulos ocorrerá em " + dataUltimoFechamento.Value.AddDays(1).ToString("dd/MM/yyyy") + "."
                        };
                    }
                }
            }

            return new
            {
                Valido = true,
                PermiteCancelarBaixa = true,
                Mensagem = string.Empty
            };
        }

        public static void ProcessarBaixasEmGeracao(Repositorio.UnitOfWork unidadeTrabalho, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware TipoServicoMultisoftware)
        {
            try
            {
                Repositorio.Embarcador.Financeiro.TituloBaixa repTituloBaixa = new Repositorio.Embarcador.Financeiro.TituloBaixa(unidadeTrabalho);
                Repositorio.Embarcador.Financeiro.TituloBaixaAgrupado repTituloBaixaAgrupado = new Repositorio.Embarcador.Financeiro.TituloBaixaAgrupado(unidadeTrabalho);
                Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unidadeTrabalho);
                Hubs.BaixaTituloReceber servicoNotificacaoBaixaTituloReceber = new Hubs.BaixaTituloReceber();

                List<Dominio.Entidades.Embarcador.Financeiro.TituloBaixa> baixasEmGeracao = repTituloBaixa.BuscarPorSituacao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoBaixaTitulo.EmGeracao, "DataOperacao", "asc", 0, 2);

                for (int i = 0; i < baixasEmGeracao.Count; i++)
                {
                    Dominio.Entidades.Embarcador.Financeiro.TituloBaixa tituloBaixa = baixasEmGeracao[i];

                    List<int> codigosTitulosPendentesGeracao = repTituloBaixa.BuscarCodigosTitulosPendentesGeracao(tituloBaixa.Codigo);

                    int countTitulosPendentes = codigosTitulosPendentesGeracao.Count;

                    for (int l = 0; l < countTitulosPendentes; l++)
                    {
                        unidadeTrabalho.Start();

                        AdicionarTituloABaixa(tituloBaixa, codigosTitulosPendentesGeracao[l], unidadeTrabalho, null, 0m, 0m, false);

                        unidadeTrabalho.CommitChanges();

                        if (countTitulosPendentes <= 10 || ((l + 1) % 5) == 0)
                        {
                            servicoNotificacaoBaixaTituloReceber.InformarQuantidadeTitulosGerados(tituloBaixa.Codigo, countTitulosPendentes, (l + 1));

                            unidadeTrabalho.FlushAndClear();
                        }
                    }

                    servicoNotificacaoBaixaTituloReceber.InformarQuantidadeTitulosGerados(tituloBaixa.Codigo, countTitulosPendentes, countTitulosPendentes);

                    tituloBaixa = repTituloBaixa.BuscarPorCodigo(tituloBaixa.Codigo);

                    unidadeTrabalho.Start();

                    AtualizarTotaisTituloBaixa(ref tituloBaixa, unidadeTrabalho, false);

                    tituloBaixa.DataEmissao = repTituloBaixaAgrupado.BuscarMaiorDataEmissaoPorTituloBaixa(tituloBaixa.Codigo);

                    List<int> faturas = repTituloBaixaAgrupado.BuscarCodigoFaturasPorTituloBaixa(tituloBaixa.Codigo);
                    List<double> tomadores = repTituloBaixaAgrupado.BuscarTomadoresPorTituloBaixa(tituloBaixa.Codigo);
                    List<int> grupoPessoas = repTituloBaixaAgrupado.BuscarGrupoPessoasPorTituloBaixa(tituloBaixa.Codigo);

                    if (faturas.Count == 1)
                        tituloBaixa.Fatura = new Dominio.Entidades.Embarcador.Fatura.Fatura() { Codigo = faturas[0] };

                    if (tomadores.Count == 1)
                        tituloBaixa.Pessoa = new Dominio.Entidades.Cliente() { CPF_CNPJ = tomadores[0] };

                    if (grupoPessoas.Count == 1)
                        tituloBaixa.GrupoPessoas = new Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas() { Codigo = grupoPessoas[0] };

                    tituloBaixa.SituacaoBaixaTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoBaixaTitulo.Iniciada;

                    repTituloBaixa.Atualizar(tituloBaixa);

                    unidadeTrabalho.CommitChanges();

                    servicoNotificacaoBaixaTituloReceber.InformarBaixaAtualizada(tituloBaixa.Codigo);
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unidadeTrabalho.Rollback();
            }
        }

        public static void ProcessarBaixasEmFinalizacao(Repositorio.UnitOfWork unidadeTrabalho, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware TipoServicoMultisoftware)
        {
            try
            {
                Servicos.Embarcador.Financeiro.BaixaTituloReceber svcBaixaTituloReceber = new BaixaTituloReceber(unidadeTrabalho);

                Repositorio.Embarcador.Financeiro.TituloBaixa repTituloBaixa = new Repositorio.Embarcador.Financeiro.TituloBaixa(unidadeTrabalho);
                Repositorio.Embarcador.Financeiro.TituloBaixaCheque repTituloBaixaCheque = new Repositorio.Embarcador.Financeiro.TituloBaixaCheque(unidadeTrabalho);
                Repositorio.Embarcador.Financeiro.TituloBaixaAgrupado repTituloBaixaAgrupado = new Repositorio.Embarcador.Financeiro.TituloBaixaAgrupado(unidadeTrabalho);
                Repositorio.Embarcador.Financeiro.TituloBaixaAgrupadoDocumento repTituloBaixaAgrupadoDocumento = new Repositorio.Embarcador.Financeiro.TituloBaixaAgrupadoDocumento(unidadeTrabalho);                                
                Hubs.BaixaTituloReceber servicoNotificacaoBaixaTituloReceber = new Hubs.BaixaTituloReceber();
                Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro repConfiguracaoFinanceiro = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro(unidadeTrabalho);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiro configuracaoFinanceiro = repConfiguracaoFinanceiro.BuscarPrimeiroRegistro();

                List<Dominio.Entidades.Embarcador.Financeiro.TituloBaixa> baixasEmFinalizacao = repTituloBaixa.BuscarPorSituacao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoBaixaTitulo.EmFinalizacao, "DataOperacao", "asc", 0, 2);

                for (int i = 0; i < baixasEmFinalizacao.Count; i++)
                {
                    Dominio.Entidades.Embarcador.Financeiro.TituloBaixa tituloBaixa = repTituloBaixa.BuscarPorCodigo(baixasEmFinalizacao[i].Codigo);

                    List<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado> titulosBaixaAgrupado = repTituloBaixaAgrupado.BuscarPorBaixaTitulo(tituloBaixa.Codigo);

                    if (!tituloBaixa.GerouTitulosNegociacao)
                    {
                        unidadeTrabalho.Start();

                        decimal valorPendente = tituloBaixa.ValorTotalAPagar - tituloBaixa.ValorPago;

                        if (valorPendente > 0m)
                        {
                            if (configuracaoFinanceiro.BaixaTitulosRenegociacaoGerarNovoTituloPorDocumento)
                                GerarTituloBaixaPorDocumento(tituloBaixa, titulosBaixaAgrupado, repTituloBaixaAgrupadoDocumento, unidadeTrabalho, TipoServicoMultisoftware);
                            else
                                GerarTituloBaixa(tituloBaixa, titulosBaixaAgrupado, repTituloBaixaAgrupadoDocumento, unidadeTrabalho, TipoServicoMultisoftware);
                        }
                        

                        tituloBaixa.GerouTitulosNegociacao = true;

                        repTituloBaixa.Atualizar(tituloBaixa);

                        unidadeTrabalho.CommitChanges();
                    }

                    int countTitulosBaixar = titulosBaixaAgrupado.Count;

                    int countDocumentosTitulosBaixar = repTituloBaixaAgrupadoDocumento.ContarPorTituloBaixa(tituloBaixa.Codigo);
                    int countDocumentosTitulosBaixados = 0;

                    string observacaoBaixa = tituloBaixa.Observacao;
                    List<string> numerosCheques = repTituloBaixaCheque.BuscarNumeroChequePorTituloBaixa(tituloBaixa.Codigo);

                    for (int l = 0; l < countTitulosBaixar; l++)
                    {
                        Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado tituloBaixaAgrupado = repTituloBaixaAgrupado.BuscarPorCodigo(titulosBaixaAgrupado[l].Codigo);

                        if (!BaixarTitulo(out string erro, tituloBaixa, tituloBaixaAgrupado, observacaoBaixa, numerosCheques, unidadeTrabalho, TipoServicoMultisoftware, null, true, countDocumentosTitulosBaixar, ref countDocumentosTitulosBaixados))
                            throw new Exception("Falha ao realizar a baixa de títulos (código do titulo baixa " + tituloBaixa.Codigo.ToString() + "): " + erro);
                    }

                    servicoNotificacaoBaixaTituloReceber.InformarQuantidadeTitulosFinalizados(tituloBaixa.Codigo, countDocumentosTitulosBaixados, countDocumentosTitulosBaixar);

                    tituloBaixa = repTituloBaixa.BuscarPorCodigo(tituloBaixa.Codigo);

                    unidadeTrabalho.Start();

                    tituloBaixa.SituacaoBaixaTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoBaixaTitulo.Finalizada;

                    repTituloBaixa.Atualizar(tituloBaixa);

                    unidadeTrabalho.CommitChanges();

                    svcBaixaTituloReceber.GeraIntegracaoBaixaTituloReceber(tituloBaixa.Codigo, tituloBaixa.Usuario?.Nome ?? "Integração", tituloBaixa.Usuario?.Empresa.EmailAdministrativo ?? "", unidadeTrabalho, false);

                    servicoNotificacaoBaixaTituloReceber.InformarBaixaAtualizada(tituloBaixa.Codigo);
                }
            }
            catch (Exception ex)
            {
                unidadeTrabalho.Rollback();
                Servicos.Log.TratarErro(ex);
            }
        }

        public static void GerarTituloBaixa(Dominio.Entidades.Embarcador.Financeiro.TituloBaixa tituloBaixa, List<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado> titulosBaixaAgrupado, Repositorio.Embarcador.Financeiro.TituloBaixaAgrupadoDocumento repTituloBaixaAgrupadoDocumento, Repositorio.UnitOfWork unidadeTrabalho, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware TipoServicoMultisoftware)
        {
            List<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado> titulosComValorPendente = (from obj in titulosBaixaAgrupado where obj.ValorPago != obj.ValorTotalAPagar select obj).OrderBy(o => o.ValorTotalAPagar - o.ValorPago).ToList();
            List<Dominio.ObjetosDeValor.Embarcador.Financeiro.ItemRateio> itensRateados = new List<Dominio.ObjetosDeValor.Embarcador.Financeiro.ItemRateio>();
            Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unidadeTrabalho);
            Repositorio.Embarcador.Financeiro.TituloDocumento repTituloDocumento = new Repositorio.Embarcador.Financeiro.TituloDocumento(unidadeTrabalho);

            int numeroParcelas = tituloBaixa.TitulosNegociacao.Count();
            decimal valorTotalParcelas = tituloBaixa.TitulosNegociacao.Sum(o => o.Valor);

            for (int l = 0; l < numeroParcelas; l++)
            {
                Dominio.Entidades.Embarcador.Financeiro.TituloBaixaNegociacao parcela = tituloBaixa.TitulosNegociacao[l];

                Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = new Dominio.Entidades.Embarcador.Financeiro.Titulo
                {
                    DataEmissao = parcela.DataEmissao,
                    DataVencimento = parcela.DataVencimento,
                    DataProgramacaoPagamento = parcela.DataVencimento,
                    TituloBaixaNegociacao = parcela,
                    Pessoa = tituloBaixa.Pessoa,
                    GrupoPessoas = tituloBaixa.GrupoPessoas,
                    Historico = Utilidades.String.Left("Título gerado pelo operador " + (tituloBaixa.Usuario?.Nome ?? "Integração") + " à partir da negociação do(s) título(s) " + string.Join(", ", from obj in titulosBaixaAgrupado select obj.Titulo.Codigo), 299) + ".",
                    Sequencia = parcela.Sequencia,
                    StatusTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.EmAberto,
                    DataAlteracao = DateTime.Now,
                    TipoTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTitulo.Receber,
                    ValorOriginal = parcela.Valor,
                    ValorPendente = parcela.Valor,
                    MoedaCotacaoBancoCentral = tituloBaixa.MoedaCotacaoBancoCentral,
                    ValorMoedaCotacao = tituloBaixa.ValorMoedaCotacao,
                    DataLancamento = DateTime.Now,
                    Usuario = tituloBaixa.Usuario,
                    ObservacaoInterna = Utilidades.String.Left(string.Join(", ", from obj in titulosBaixaAgrupado select obj.Titulo.ObservacaoInterna), 299) + ".",
                    Observacao = Utilidades.String.Left(string.Join(", ", from obj in titulosBaixaAgrupado select obj.Titulo.Observacao), 299) + ".",
                    TipoMovimento = (from obj in titulosBaixaAgrupado where obj.ValorPago != obj.ValorTotalAPagar && obj.Titulo.TipoMovimento != null select obj.Titulo.TipoMovimento).FirstOrDefault(),
                    TipoDocumentoTituloOriginal = Utilidades.String.Left(tituloBaixa.TipoDocumentoTituloOriginal, 500),
                    NumeroDocumentoTituloOriginal = Utilidades.String.Left(tituloBaixa.NumeroDocumentoTituloOriginal, 4000)
                };

                if (titulo.GrupoPessoas == null && titulo.Pessoa != null && titulo.Pessoa.GrupoPessoas != null)
                    titulo.GrupoPessoas = titulo.Pessoa.GrupoPessoas;

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    titulo.TipoAmbiente = tituloBaixa.Usuario?.Empresa.TipoAmbiente ?? Dominio.Enumeradores.TipoAmbiente.Nenhum;

                repTitulo.Inserir(titulo);

                decimal percentualRateio = parcela.Valor / valorTotalParcelas;
                decimal valorTotalDocumentoRateado = 0m, valorTotalDocumentoMoedaRateado = 0m, valorTotalAcrescimoRateado = 0m;

                for (int idxTituloValorPendente = 0; idxTituloValorPendente < titulosComValorPendente.Count; idxTituloValorPendente++)
                {
                    Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado tituloBaixaAgrupado = titulosComValorPendente[idxTituloValorPendente];

                    List<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupadoDocumento> tituloBaixaAgrupadoDocumentos = repTituloBaixaAgrupadoDocumento.BuscarPorTituloBaixaAgrupado(tituloBaixaAgrupado.Codigo);

                    for (int idxTituloValorPendenteDocumento = 0; idxTituloValorPendenteDocumento < tituloBaixaAgrupadoDocumentos.Count; idxTituloValorPendenteDocumento++)
                    {
                        Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupadoDocumento tituloBaixaAgrupadoDocumento = tituloBaixaAgrupadoDocumentos[idxTituloValorPendenteDocumento];

                        if (tituloBaixaAgrupadoDocumento.ValorTotalAPagar != tituloBaixaAgrupadoDocumento.ValorPago)
                        {
                            Dominio.ObjetosDeValor.Embarcador.Financeiro.ItemRateio itemRateado = itensRateados.Where(o => o.Codigo == tituloBaixaAgrupadoDocumento.Codigo).FirstOrDefault();

                            if (itemRateado == null)
                                itemRateado = new Dominio.ObjetosDeValor.Embarcador.Financeiro.ItemRateio() { Codigo = tituloBaixaAgrupadoDocumento.Codigo };

                            decimal valorPago = tituloBaixaAgrupadoDocumento.ValorPago + tituloBaixaAgrupadoDocumento.ValorDesconto + tituloBaixaAgrupadoDocumento.TituloDocumento.ValorDesconto;
                            decimal valorAcrescimoTotal = tituloBaixaAgrupadoDocumento.TituloDocumento.ValorAcrescimo + tituloBaixaAgrupadoDocumento.ValorAcrescimo;

                            decimal valorAcrescimoTituloDocumento = 0m;

                            if (valorAcrescimoTotal > valorPago)
                            {
                                decimal valorAcrescimoRatear = valorAcrescimoTotal - valorPago - itemRateado.ValorAcrescimoRateado;
                                decimal valorAcrescimoRateado = valorAcrescimoRatear;//Math.Round(valorAcrescimoRatear * percentualRateio, 2, MidpointRounding.ToEven);

                                //if (valorAcrescimoRateado + itemRateado.ValorAcrescimoRateado > valorAcrescimoRatear)
                                //    valorAcrescimoRateado -= (itemRateado.ValorAcrescimoRateado + valorAcrescimoRateado) - valorAcrescimoRatear;

                                if (valorAcrescimoRateado + valorTotalAcrescimoRateado > parcela.Valor)
                                    valorAcrescimoRateado -= (valorAcrescimoRateado + valorTotalAcrescimoRateado) - parcela.Valor;

                                //decimal valorAcrescimoDiferenca = valorAcrescimoRatear - itemRateado.ValorAcrescimoRateado;

                                //if (l == (numeroParcelas - 1))
                                //    tituloDocumento.ValorAcrescimo = valorAcrescimoRateado + valorAcrescimoDiferenca;
                                //else
                                valorAcrescimoTituloDocumento = valorAcrescimoRateado;
                                valorTotalAcrescimoRateado += valorAcrescimoRateado;
                                valorPago = 0;
                            }
                            else
                                valorPago -= valorAcrescimoTotal;

                            decimal valorDocumentoMoedaRatear = tituloBaixaAgrupadoDocumento.ValorTotalAPagarMoeda - tituloBaixaAgrupadoDocumento.ValorPagoMoeda - itemRateado.ValorMoedaRateado;
                            decimal valorDocumentoMoedaRateado = valorDocumentoMoedaRatear;//Math.Round(valorDocumentoMoedaRatear * percentualRateio, MidpointRounding.ToEven);
                                                                                           //decimal valorDocumentoMoedaDiferenca = valorDocumentoMoedaRatear - (valorDocumentoMoedaRateado * numeroParcelas);

                            if (valorTotalDocumentoMoedaRateado + valorDocumentoMoedaRateado > parcela.ValorOriginalMoedaEstrangeira)
                                valorDocumentoMoedaRateado -= (valorTotalDocumentoMoedaRateado + valorDocumentoMoedaRateado) - parcela.ValorOriginalMoedaEstrangeira;

                            //if (valorDocumentoMoedaRateado + itemRateado.ValorMoedaRateado > valorDocumentoMoedaRatear)
                            //    valorDocumentoMoedaRateado -= (valorDocumentoMoedaRateado + itemRateado.ValorMoedaRateado) - valorDocumentoMoedaRatear;

                            decimal valorDocumentoRatear = tituloBaixaAgrupadoDocumento.TituloDocumento.Valor - valorPago - itemRateado.ValorRateado;
                            decimal valorDocumentoRateado = valorDocumentoRatear;//(Math.Floor(valorDocumentoRatear * percentualRateio * 100) / 100) + 1m;
                                                                                 //decimal valorDocumentoDiferenca = valorDocumentoRatear - (valorDocumentoRateado * numeroParcelas);

                            //if (valorDocumentoRateado + itemRateado.ValorRateado > valorDocumentoRatear)
                            //    valorDocumentoRateado -= (valorDocumentoRateado + itemRateado.ValorRateado) - valorDocumentoRatear;
                            //else if (valorDocumentoRateado == 0m && (valorDocumentoRatear - itemRateado.ValorRateado) >= parcela.Valor && valorTotalDocumentoRateado < parcela.Valor)
                            //    valorDocumentoRateado = parcela.Valor;

                            if (valorTotalDocumentoRateado + valorDocumentoRateado + valorAcrescimoTituloDocumento > parcela.Valor)
                                valorDocumentoRateado -= (valorTotalDocumentoRateado + valorDocumentoRateado + valorAcrescimoTituloDocumento) - parcela.Valor;

                            if (valorDocumentoRateado <= 0m && valorAcrescimoTituloDocumento <= 0m && valorDocumentoMoedaRateado <= 0m)
                                continue;

                            Dominio.Entidades.Embarcador.Financeiro.TituloDocumento tituloDocumento = new Dominio.Entidades.Embarcador.Financeiro.TituloDocumento();
                            tituloDocumento.Carga = tituloBaixaAgrupadoDocumento.TituloDocumento.Carga;
                            tituloDocumento.CTe = tituloBaixaAgrupadoDocumento.TituloDocumento.CTe;
                            tituloDocumento.TipoDocumento = tituloBaixaAgrupadoDocumento.TituloDocumento.TipoDocumento;
                            tituloDocumento.Titulo = titulo;
                            tituloDocumento.ValorCotacaoMoeda = tituloBaixa.ValorMoedaCotacao;
                            tituloDocumento.ValorAcrescimo = valorAcrescimoTituloDocumento;
                            tituloDocumento.ValorMoeda = valorDocumentoMoedaRateado;
                            tituloDocumento.Valor = valorDocumentoRateado;
                            tituloDocumento.ValorPendenteMoeda = tituloDocumento.ValorMoeda;
                            tituloDocumento.ValorTotalMoeda = tituloDocumento.ValorMoeda;
                            tituloDocumento.ValorPendente = tituloDocumento.Valor + tituloDocumento.ValorAcrescimo - tituloDocumento.ValorDesconto;
                            tituloDocumento.ValorTotal = tituloDocumento.Valor + tituloDocumento.ValorAcrescimo - tituloDocumento.ValorDesconto;

                            titulo.ValorOriginalMoedaEstrangeira += tituloDocumento.ValorTotalMoeda;

                            repTituloDocumento.Inserir(tituloDocumento);

                            valorTotalDocumentoRateado += valorDocumentoRateado;
                            valorTotalDocumentoMoedaRateado += valorDocumentoMoedaRateado;

                            itemRateado.ValorMoedaRateado += valorDocumentoMoedaRateado;
                            itemRateado.ValorRateado += valorDocumentoRateado;
                            itemRateado.ValorAcrescimoRateado += valorAcrescimoTituloDocumento;

                            int indexItemRateado = itensRateados.FindIndex(o => o.Codigo == itemRateado.Codigo);

                            if (indexItemRateado > -1)
                                itensRateados[indexItemRateado] = itemRateado;
                            else
                                itensRateados.Add(itemRateado);
                        }
                    }
                }

                repTitulo.Atualizar(titulo);
            }
        }

        public static void GerarTituloBaixaPorDocumento(Dominio.Entidades.Embarcador.Financeiro.TituloBaixa tituloBaixa, List<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado> titulosBaixaAgrupado, Repositorio.Embarcador.Financeiro.TituloBaixaAgrupadoDocumento repTituloBaixaAgrupadoDocumento, Repositorio.UnitOfWork unidadeTrabalho, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware TipoServicoMultisoftware)
        {
            List<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado> titulosComValorPendente = (from obj in titulosBaixaAgrupado where obj.ValorPago != obj.ValorTotalAPagar select obj).OrderBy(o => o.ValorTotalAPagar - o.ValorPago).ToList();
            Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unidadeTrabalho);
            Repositorio.Embarcador.Financeiro.TituloDocumento repTituloDocumento = new Repositorio.Embarcador.Financeiro.TituloDocumento(unidadeTrabalho);
            List<Dominio.Entidades.Embarcador.Financeiro.Titulo> titulos = new List<Dominio.Entidades.Embarcador.Financeiro.Titulo>();
            List<Dominio.Entidades.Embarcador.Financeiro.TituloDocumento> titulosdocumentos = new List<Dominio.Entidades.Embarcador.Financeiro.TituloDocumento>();

            int numeroParcelas = tituloBaixa.TitulosNegociacao.Count();
            decimal valorTotalParcelas = tituloBaixa.TitulosNegociacao.Sum(o => o.Valor);
            bool ultiparcela;

            List<Dominio.ObjetosDeValor.Embarcador.Financeiro.ItemRateio> itensRateadosdocumentos = new List<Dominio.ObjetosDeValor.Embarcador.Financeiro.ItemRateio>();

            for (int l = 0; l < numeroParcelas; l++)
            {
                ultiparcela = (l + 1 == numeroParcelas && numeroParcelas > 1);
                List<Dominio.ObjetosDeValor.Embarcador.Financeiro.ItemRateio> itensRateados = new List<Dominio.ObjetosDeValor.Embarcador.Financeiro.ItemRateio>();
                Dominio.Entidades.Embarcador.Financeiro.TituloBaixaNegociacao parcela = tituloBaixa.TitulosNegociacao[l];

                decimal percentualRateio = parcela.Valor / valorTotalParcelas;
                decimal valorTotalDocumentoRateado = 0m, valorTotalDocumentoMoedaRateado = 0m, valorTotalAcrescimoRateado = 0m;

                for (int idxTituloValorPendente = 0; idxTituloValorPendente < titulosComValorPendente.Count; idxTituloValorPendente++)
                {
                    Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado tituloBaixaAgrupado = titulosComValorPendente[idxTituloValorPendente];

                    List<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupadoDocumento> tituloBaixaAgrupadoDocumentos = repTituloBaixaAgrupadoDocumento.BuscarPorTituloBaixaAgrupado(tituloBaixaAgrupado.Codigo);

                    for (int idxTituloValorPendenteDocumento = 0; idxTituloValorPendenteDocumento < tituloBaixaAgrupadoDocumentos.Count; idxTituloValorPendenteDocumento++)
                    {
                        Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupadoDocumento tituloBaixaAgrupadoDocumento = tituloBaixaAgrupadoDocumentos[idxTituloValorPendenteDocumento];                        

                        if (tituloBaixaAgrupadoDocumento.ValorTotalAPagar != tituloBaixaAgrupadoDocumento.ValorPago)
                        {
                            Dominio.ObjetosDeValor.Embarcador.Financeiro.ItemRateio itemRateado = itensRateados.Where(o => o.Codigo == tituloBaixaAgrupadoDocumento.Codigo).FirstOrDefault();
                            Dominio.ObjetosDeValor.Embarcador.Financeiro.ItemRateio itemRateadoDocumento = itensRateadosdocumentos.Where(o => o.Codigo == tituloBaixaAgrupadoDocumento.Codigo && o.Parcela == parcela.Sequencia).FirstOrDefault();

                            if (itemRateado == null)
                                itemRateado = new Dominio.ObjetosDeValor.Embarcador.Financeiro.ItemRateio() { Codigo = tituloBaixaAgrupadoDocumento.Codigo };

                            if (itemRateadoDocumento == null)
                                itemRateadoDocumento = new Dominio.ObjetosDeValor.Embarcador.Financeiro.ItemRateio() { Codigo = tituloBaixaAgrupadoDocumento.Codigo, Parcela = parcela.Sequencia };

                            decimal valorPago = tituloBaixaAgrupadoDocumento.ValorPago + tituloBaixaAgrupadoDocumento.ValorDesconto + tituloBaixaAgrupadoDocumento.TituloDocumento.ValorDesconto;
                            decimal valorAcrescimoTotal = tituloBaixaAgrupadoDocumento.TituloDocumento.ValorAcrescimo + tituloBaixaAgrupadoDocumento.ValorAcrescimo;

                            decimal valorAcrescimoTituloDocumento = 0m;

                            if (valorAcrescimoTotal > valorPago)
                            {
                                decimal valorAcrescimoRatear = valorAcrescimoTotal - valorPago - itemRateado.ValorAcrescimoRateado;
                                decimal valorAcrescimoRateado = valorAcrescimoRatear;

                                if (valorAcrescimoRateado + valorTotalAcrescimoRateado > parcela.Valor)
                                    valorAcrescimoRateado -= (valorAcrescimoRateado + valorTotalAcrescimoRateado) - parcela.Valor;

                                valorAcrescimoTituloDocumento = valorAcrescimoRateado;
                                valorTotalAcrescimoRateado += valorAcrescimoRateado;
                                valorPago = 0;
                            }
                            else
                                valorPago -= valorAcrescimoTotal;

                            decimal valorDocumentoMoedaRatear = tituloBaixaAgrupadoDocumento.ValorTotalAPagarMoeda - tituloBaixaAgrupadoDocumento.ValorPagoMoeda - itemRateado.ValorMoedaRateado;
                            decimal valorDocumentoMoedaRateado = valorDocumentoMoedaRatear;

                            if (valorTotalDocumentoMoedaRateado + valorDocumentoMoedaRateado > parcela.ValorOriginalMoedaEstrangeira)
                                valorDocumentoMoedaRateado -= (valorTotalDocumentoMoedaRateado + valorDocumentoMoedaRateado) - parcela.ValorOriginalMoedaEstrangeira;

                            decimal valorDocumentoRatear = tituloBaixaAgrupadoDocumento.TituloDocumento.Valor - valorPago - itemRateado.ValorRateado;
                            decimal valorDocumentoRateado = valorDocumentoRatear;

                            valorDocumentoRateado = Math.Round((valorDocumentoRateado * percentualRateio * 100) / 100, 2, MidpointRounding.ToEven);

                            if (valorTotalDocumentoRateado + valorDocumentoRateado + valorAcrescimoTituloDocumento > parcela.Valor)
                                valorDocumentoRateado -= (valorTotalDocumentoRateado + valorDocumentoRateado + valorAcrescimoTituloDocumento) - parcela.Valor;

                            if (valorDocumentoRateado <= 0m && valorAcrescimoTituloDocumento <= 0m && valorDocumentoMoedaRateado <= 0m)
                                continue;

                            decimal soma = itensRateadosdocumentos.Where(o => o.Codigo == tituloBaixaAgrupadoDocumento.Codigo).Sum(o => o.ValorRateado);

                            if (ultiparcela)
                                valorDocumentoRateado = Math.Round((valorDocumentoRatear - soma), 2, MidpointRounding.ToEven);


                            string numerodocumento = tituloBaixaAgrupadoDocumento.TituloDocumento.CTe != null ?
                                                     tituloBaixaAgrupadoDocumento.TituloDocumento.CTe.Numero.ToString() + "-" + tituloBaixaAgrupadoDocumento.TituloDocumento.CTe.Serie.Numero.ToString() :
                                                     tituloBaixaAgrupadoDocumento.TituloDocumento.Carga != null ? tituloBaixaAgrupadoDocumento.TituloDocumento.Carga.CodigoCargaEmbarcador :
                                                     string.Empty;

                            var tituloOriginal = parcela.TituloBaixa.TitulosAgrupados.Where(x => x.Titulo.NumeroDocumentoTituloOriginal == numerodocumento).FirstOrDefault();

                            Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = new Dominio.Entidades.Embarcador.Financeiro.Titulo
                            {
                                DataEmissao = parcela.DataEmissao,
                                DataVencimento = tituloOriginal.Titulo.DataVencimento ?? parcela.DataVencimento,
                                DataProgramacaoPagamento = parcela.DataVencimento,
                                TituloBaixaNegociacao = parcela,
                                Pessoa = tituloBaixa.Pessoa,
                                GrupoPessoas = tituloBaixa.GrupoPessoas,
                                Historico = Utilidades.String.Left("Título gerado pelo operador " + (tituloBaixa.Usuario?.Nome ?? "Integração") + " à partir da negociação do título " + string.Join(", ", from obj in titulosBaixaAgrupado select obj.Titulo.Codigo), 299) + ".",
                                Sequencia = parcela.Sequencia,
                                StatusTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.EmAberto,
                                DataAlteracao = DateTime.Now,
                                TipoTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTitulo.Receber,
                                ValorOriginal = valorDocumentoRateado,
                                ValorPendente = valorDocumentoRateado,
                                MoedaCotacaoBancoCentral = tituloBaixa.MoedaCotacaoBancoCentral,
                                ValorMoedaCotacao = tituloBaixa.ValorMoedaCotacao,
                                DataLancamento = DateTime.Now,
                                Usuario = tituloBaixa.Usuario,
                                ObservacaoInterna = Utilidades.String.Left(string.Join(", ", from obj in titulosBaixaAgrupado select obj.Titulo.ObservacaoInterna), 299) + ".",
                                Observacao = tituloOriginal.Titulo.Observacao ?? "",
                                TipoMovimento = (from obj in titulosBaixaAgrupado where obj.ValorPago != obj.ValorTotalAPagar && obj.Titulo.TipoMovimento != null select obj.Titulo.TipoMovimento).FirstOrDefault(),
                                TipoDocumentoTituloOriginal = Utilidades.String.Left(tituloBaixa.TipoDocumentoTituloOriginal, 500),
                                Empresa = tituloOriginal.Titulo.Empresa,
                                NumeroDocumentoTituloOriginal = Utilidades.String.Left(numerodocumento, 4000)
                            };
                            
                            if (titulo.GrupoPessoas == null && titulo.Pessoa != null && titulo.Pessoa.GrupoPessoas != null)
                                titulo.GrupoPessoas = titulo.Pessoa.GrupoPessoas;

                            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                                titulo.TipoAmbiente = tituloBaixa.Usuario?.Empresa.TipoAmbiente ?? Dominio.Enumeradores.TipoAmbiente.Nenhum;

                            titulo.ValorOriginalMoedaEstrangeira += valorDocumentoRateado;

                            repTitulo.Inserir(titulo);

                            Dominio.Entidades.Embarcador.Financeiro.TituloDocumento tituloDocumento = new Dominio.Entidades.Embarcador.Financeiro.TituloDocumento();
                            tituloDocumento.Carga = tituloBaixaAgrupadoDocumento.TituloDocumento.Carga;
                            tituloDocumento.CTe = tituloBaixaAgrupadoDocumento.TituloDocumento.CTe;
                            tituloDocumento.TipoDocumento = tituloBaixaAgrupadoDocumento.TituloDocumento.TipoDocumento;
                            tituloDocumento.Titulo = titulo;
                            tituloDocumento.ValorCotacaoMoeda = tituloBaixa.ValorMoedaCotacao;
                            tituloDocumento.ValorAcrescimo = valorAcrescimoTituloDocumento;
                            tituloDocumento.ValorMoeda = valorDocumentoMoedaRateado;
                            tituloDocumento.Valor = valorDocumentoRateado;
                            tituloDocumento.ValorPendenteMoeda = tituloDocumento.ValorMoeda;
                            tituloDocumento.ValorTotalMoeda = tituloDocumento.ValorMoeda;
                            tituloDocumento.ValorPendente = tituloDocumento.Valor + tituloDocumento.ValorAcrescimo - tituloDocumento.ValorDesconto;
                            tituloDocumento.ValorTotal = tituloDocumento.Valor + tituloDocumento.ValorAcrescimo - tituloDocumento.ValorDesconto;

                            repTituloDocumento.Inserir(tituloDocumento);

                            valorTotalDocumentoRateado += valorDocumentoRateado;
                            valorTotalDocumentoMoedaRateado += valorDocumentoMoedaRateado;

                            itemRateado.ValorMoedaRateado += valorDocumentoMoedaRateado;
                            itemRateado.ValorRateado += valorDocumentoRateado;
                            itemRateado.ValorAcrescimoRateado += valorAcrescimoTituloDocumento;

                            int indexItemRateado = itensRateados.FindIndex(o => o.Codigo == itemRateado.Codigo);

                            if (indexItemRateado > -1)
                                itensRateados[indexItemRateado] = itemRateado;
                            else
                                itensRateados.Add(itemRateado);

                            repTitulo.Atualizar(titulo);
                            itemRateadoDocumento.ValorRateado += valorDocumentoRateado;
                            itensRateadosdocumentos.Add(itemRateadoDocumento);
                        }
                    }
                }
            }
        }

        public static void AjustarValorPagoDocumento(Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupadoDocumento tituloBaixaAgrupadoDocumento, bool moedaEstrangeira, ref decimal valorTotalPago, ref decimal valorTotalPagoMoeda, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Financeiro.TituloBaixaAgrupadoDocumento repTituloBaixaAgrupadoDocumento = new Repositorio.Embarcador.Financeiro.TituloBaixaAgrupadoDocumento(unitOfWork);

            if (moedaEstrangeira)
            {
                decimal valorPagoMoeda = valorTotalPagoMoeda;
                decimal valorPago;

                if (valorPagoMoeda >= tituloBaixaAgrupadoDocumento.ValorTotalAPagarMoeda)
                {
                    valorPagoMoeda = tituloBaixaAgrupadoDocumento.ValorTotalAPagarMoeda;
                    valorPago = tituloBaixaAgrupadoDocumento.ValorTotalAPagar;
                }
                else
                {
                    decimal percentual = valorPagoMoeda / tituloBaixaAgrupadoDocumento.ValorTotalAPagarMoeda;

                    valorPago = Math.Round(tituloBaixaAgrupadoDocumento.ValorTotalAPagar * percentual, 2, MidpointRounding.ToEven);
                }

                if (valorPago > tituloBaixaAgrupadoDocumento.ValorTotalAPagar)
                    valorPago = tituloBaixaAgrupadoDocumento.ValorTotalAPagar;

                tituloBaixaAgrupadoDocumento.ValorPagoMoeda = valorPagoMoeda;
                tituloBaixaAgrupadoDocumento.ValorPago = valorPago;

                repTituloBaixaAgrupadoDocumento.Atualizar(tituloBaixaAgrupadoDocumento);

                valorTotalPagoMoeda -= valorPagoMoeda;
            }
            else
            {
                decimal valorPago = valorTotalPago;

                if (valorPago > tituloBaixaAgrupadoDocumento.ValorTotalAPagar)
                    valorPago = tituloBaixaAgrupadoDocumento.ValorTotalAPagar;

                tituloBaixaAgrupadoDocumento.ValorPago = valorPago;

                repTituloBaixaAgrupadoDocumento.Atualizar(tituloBaixaAgrupadoDocumento);

                valorTotalPago -= valorPago;
            }
        }

        public static bool AdicionarValorAoDocumento(out string mensagem, Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupadoDocumento tituloBaixaAgrupadoDocumento, Dominio.Entidades.Embarcador.Fatura.Justificativa justificativa, decimal valor, string observacao, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Usuario usuario, Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupadoDocumentoAcrescimoDesconto tituloBaixaAgrupadoDocumentoAcrescimoDesconto = null, bool variacaoCambial = false, decimal valorMoeda = 0m)
        {
            Repositorio.Embarcador.Financeiro.TituloBaixaAgrupadoDocumento repTituloBaixaAgrupadoDocumento = new Repositorio.Embarcador.Financeiro.TituloBaixaAgrupadoDocumento(unitOfWork);
            Repositorio.Embarcador.Financeiro.TituloBaixaAgrupadoDocumentoAcrescimoDesconto repTituloBaixaAgrupadoDocumentoAcrescimoDesconto = new Repositorio.Embarcador.Financeiro.TituloBaixaAgrupadoDocumentoAcrescimoDesconto(unitOfWork);

            if (tituloBaixaAgrupadoDocumentoAcrescimoDesconto == null)
                tituloBaixaAgrupadoDocumentoAcrescimoDesconto = new Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupadoDocumentoAcrescimoDesconto();
            else
            {
                if (tituloBaixaAgrupadoDocumentoAcrescimoDesconto.TipoJustificativa == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoJustificativa.Acrescimo)
                {
                    tituloBaixaAgrupadoDocumento.ValorAcrescimoMoeda -= tituloBaixaAgrupadoDocumentoAcrescimoDesconto.ValorMoeda;
                    tituloBaixaAgrupadoDocumento.ValorAcrescimo -= tituloBaixaAgrupadoDocumentoAcrescimoDesconto.Valor;

                    if (tituloBaixaAgrupadoDocumento.ValorTotalAPagar == tituloBaixaAgrupadoDocumento.ValorPago)
                        tituloBaixaAgrupadoDocumento.ValorPago = tituloBaixaAgrupadoDocumento.ValorPago - tituloBaixaAgrupadoDocumentoAcrescimoDesconto.Valor;

                    if (tituloBaixaAgrupadoDocumento.ValorTotalAPagarMoeda == tituloBaixaAgrupadoDocumento.ValorPagoMoeda)
                        tituloBaixaAgrupadoDocumento.ValorPagoMoeda = tituloBaixaAgrupadoDocumento.ValorPagoMoeda - tituloBaixaAgrupadoDocumentoAcrescimoDesconto.ValorMoeda;
                }
                else
                {
                    tituloBaixaAgrupadoDocumento.ValorDesconto -= tituloBaixaAgrupadoDocumentoAcrescimoDesconto.Valor;
                    tituloBaixaAgrupadoDocumento.ValorDescontoMoeda -= tituloBaixaAgrupadoDocumentoAcrescimoDesconto.ValorMoeda;

                    if (tituloBaixaAgrupadoDocumento.ValorTotalAPagar == tituloBaixaAgrupadoDocumento.ValorPago)
                        tituloBaixaAgrupadoDocumento.ValorPago = tituloBaixaAgrupadoDocumento.ValorPago + tituloBaixaAgrupadoDocumentoAcrescimoDesconto.Valor;

                    if (tituloBaixaAgrupadoDocumento.ValorTotalAPagarMoeda == tituloBaixaAgrupadoDocumento.ValorPagoMoeda)
                        tituloBaixaAgrupadoDocumento.ValorPagoMoeda = tituloBaixaAgrupadoDocumento.ValorPagoMoeda + tituloBaixaAgrupadoDocumentoAcrescimoDesconto.ValorMoeda;
                }
            }

            tituloBaixaAgrupadoDocumentoAcrescimoDesconto.TituloBaixaAgrupadoDocumento = tituloBaixaAgrupadoDocumento;
            tituloBaixaAgrupadoDocumentoAcrescimoDesconto.Observacao = observacao;
            tituloBaixaAgrupadoDocumentoAcrescimoDesconto.Justificativa = justificativa;
            tituloBaixaAgrupadoDocumentoAcrescimoDesconto.Valor = valor;
            tituloBaixaAgrupadoDocumentoAcrescimoDesconto.ValorMoeda = valorMoeda;
            tituloBaixaAgrupadoDocumentoAcrescimoDesconto.VariacaoCambial = variacaoCambial;

            if (!tituloBaixaAgrupadoDocumentoAcrescimoDesconto.Justificativa.GerarMovimentoAutomatico)
            {
                mensagem = "A justificativa não possui a movimentação financeira configurada, não sendo possível adicioná-la.";
                return false;
            }

            tituloBaixaAgrupadoDocumentoAcrescimoDesconto.TipoJustificativa = tituloBaixaAgrupadoDocumentoAcrescimoDesconto.Justificativa.TipoJustificativa;
            tituloBaixaAgrupadoDocumentoAcrescimoDesconto.TipoMovimentoUso = tituloBaixaAgrupadoDocumentoAcrescimoDesconto.Justificativa.TipoMovimentoUsoJustificativa;
            tituloBaixaAgrupadoDocumentoAcrescimoDesconto.TipoMovimentoReversao = tituloBaixaAgrupadoDocumentoAcrescimoDesconto.Justificativa.TipoMovimentoReversaoUsoJustificativa;

            if (tituloBaixaAgrupadoDocumentoAcrescimoDesconto.Codigo > 0)
                repTituloBaixaAgrupadoDocumentoAcrescimoDesconto.Atualizar(tituloBaixaAgrupadoDocumentoAcrescimoDesconto);
            else
                repTituloBaixaAgrupadoDocumentoAcrescimoDesconto.Inserir(tituloBaixaAgrupadoDocumentoAcrescimoDesconto);

            if (tituloBaixaAgrupadoDocumentoAcrescimoDesconto.TipoJustificativa == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoJustificativa.Acrescimo)
            {
                tituloBaixaAgrupadoDocumento.ValorAcrescimo += tituloBaixaAgrupadoDocumentoAcrescimoDesconto.Valor;
                tituloBaixaAgrupadoDocumento.ValorAcrescimoMoeda += tituloBaixaAgrupadoDocumentoAcrescimoDesconto.ValorMoeda;

                if (tituloBaixaAgrupadoDocumento.ValorTotalAPagar == tituloBaixaAgrupadoDocumento.ValorPago)
                    tituloBaixaAgrupadoDocumento.ValorPago = tituloBaixaAgrupadoDocumento.ValorPago + tituloBaixaAgrupadoDocumentoAcrescimoDesconto.Valor;

                if (tituloBaixaAgrupadoDocumento.ValorTotalAPagarMoeda == tituloBaixaAgrupadoDocumento.ValorPagoMoeda)
                    tituloBaixaAgrupadoDocumento.ValorPagoMoeda = tituloBaixaAgrupadoDocumento.ValorPagoMoeda + tituloBaixaAgrupadoDocumentoAcrescimoDesconto.ValorMoeda;
            }
            else
            {
                tituloBaixaAgrupadoDocumento.ValorDesconto += tituloBaixaAgrupadoDocumentoAcrescimoDesconto.Valor;
                tituloBaixaAgrupadoDocumento.ValorDescontoMoeda += tituloBaixaAgrupadoDocumentoAcrescimoDesconto.ValorMoeda;

                if (tituloBaixaAgrupadoDocumento.ValorTotalAPagar == tituloBaixaAgrupadoDocumento.ValorPago)
                    tituloBaixaAgrupadoDocumento.ValorPago = tituloBaixaAgrupadoDocumento.ValorPago - tituloBaixaAgrupadoDocumentoAcrescimoDesconto.Valor;

                if (tituloBaixaAgrupadoDocumento.ValorTotalAPagarMoeda == tituloBaixaAgrupadoDocumento.ValorPagoMoeda)
                    tituloBaixaAgrupadoDocumento.ValorPagoMoeda = tituloBaixaAgrupadoDocumento.ValorPagoMoeda - tituloBaixaAgrupadoDocumentoAcrescimoDesconto.ValorMoeda;
            }

            tituloBaixaAgrupadoDocumento.ValorTotalAPagar = tituloBaixaAgrupadoDocumento.TituloDocumento.ValorTotal + tituloBaixaAgrupadoDocumento.ValorAcrescimo - tituloBaixaAgrupadoDocumento.ValorDesconto;
            tituloBaixaAgrupadoDocumento.ValorTotalAPagarMoeda = tituloBaixaAgrupadoDocumento.TituloDocumento.ValorTotalMoeda + tituloBaixaAgrupadoDocumento.ValorAcrescimoMoeda - tituloBaixaAgrupadoDocumento.ValorDescontoMoeda;

            if (tituloBaixaAgrupadoDocumento.ValorTotalAPagar < 0m)
            {
                mensagem = "Não é possível alterar o valor pois o valor total a pagar ficará negativo (" + tituloBaixaAgrupadoDocumento.ValorTotalAPagar.ToString("n2") + ").";
                return false;
            }

            if (tituloBaixaAgrupadoDocumento.ValorTotalAPagarMoeda < 0m)
            {
                mensagem = "Não é possível alterar o valor pois o valor total a pagar em moeda ficará negativo (" + tituloBaixaAgrupadoDocumento.ValorTotalAPagarMoeda.ToString("n2") + ").";
                return false;
            }

            if (tituloBaixaAgrupadoDocumento.ValorTotalAPagar < tituloBaixaAgrupadoDocumento.ValorPago)
                tituloBaixaAgrupadoDocumento.ValorPago = tituloBaixaAgrupadoDocumento.ValorTotalAPagar;

            if (tituloBaixaAgrupadoDocumento.ValorTotalAPagarMoeda < tituloBaixaAgrupadoDocumento.ValorPagoMoeda)
                tituloBaixaAgrupadoDocumento.ValorPagoMoeda = tituloBaixaAgrupadoDocumento.ValorTotalAPagarMoeda;

            tituloBaixaAgrupadoDocumento.DataAplicacaoDesconto = DateTime.Now;
            tituloBaixaAgrupadoDocumento.Usuario = usuario;

            repTituloBaixaAgrupadoDocumento.Atualizar(tituloBaixaAgrupadoDocumento);

            mensagem = string.Empty;
            return true;
        }

        public static bool RemoverValorDoDocumento(out string mensagem, Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupadoDocumento tituloBaixaAgrupadoDocumento, Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupadoDocumentoAcrescimoDesconto tituloBaixaAgrupadoDocumentoAcrescimoDesconto, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Financeiro.TituloBaixaAgrupadoDocumentoAcrescimoDesconto repTituloBaixaAgrupadoDocumentoAcrescimoDesconto = new Repositorio.Embarcador.Financeiro.TituloBaixaAgrupadoDocumentoAcrescimoDesconto(unitOfWork);
            Repositorio.Embarcador.Financeiro.TituloBaixaAgrupadoDocumento repTituloBaixaAgrupadoDocumento = new Repositorio.Embarcador.Financeiro.TituloBaixaAgrupadoDocumento(unitOfWork);

            if (tituloBaixaAgrupadoDocumentoAcrescimoDesconto.TipoJustificativa == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoJustificativa.Acrescimo)
            {
                tituloBaixaAgrupadoDocumento.ValorAcrescimo -= tituloBaixaAgrupadoDocumentoAcrescimoDesconto.Valor;
                tituloBaixaAgrupadoDocumento.ValorAcrescimoMoeda -= tituloBaixaAgrupadoDocumentoAcrescimoDesconto.ValorMoeda;

                if (tituloBaixaAgrupadoDocumento.ValorTotalAPagar == tituloBaixaAgrupadoDocumento.ValorPago)
                    tituloBaixaAgrupadoDocumento.ValorPago = tituloBaixaAgrupadoDocumento.ValorPago - tituloBaixaAgrupadoDocumentoAcrescimoDesconto.Valor;

                if (tituloBaixaAgrupadoDocumento.ValorTotalAPagarMoeda == tituloBaixaAgrupadoDocumento.ValorPagoMoeda)
                    tituloBaixaAgrupadoDocumento.ValorPagoMoeda = tituloBaixaAgrupadoDocumento.ValorPagoMoeda - tituloBaixaAgrupadoDocumentoAcrescimoDesconto.ValorMoeda;
            }
            else
            {
                tituloBaixaAgrupadoDocumento.ValorDesconto -= tituloBaixaAgrupadoDocumentoAcrescimoDesconto.Valor;
                tituloBaixaAgrupadoDocumento.ValorDescontoMoeda -= tituloBaixaAgrupadoDocumentoAcrescimoDesconto.ValorMoeda;

                if (tituloBaixaAgrupadoDocumento.ValorTotalAPagar == tituloBaixaAgrupadoDocumento.ValorPago)
                    tituloBaixaAgrupadoDocumento.ValorPago = tituloBaixaAgrupadoDocumento.ValorPago + tituloBaixaAgrupadoDocumentoAcrescimoDesconto.Valor;

                if (tituloBaixaAgrupadoDocumento.ValorTotalAPagarMoeda == tituloBaixaAgrupadoDocumento.ValorPagoMoeda)
                    tituloBaixaAgrupadoDocumento.ValorPagoMoeda = tituloBaixaAgrupadoDocumento.ValorPagoMoeda + tituloBaixaAgrupadoDocumentoAcrescimoDesconto.ValorMoeda;
            }

            tituloBaixaAgrupadoDocumento.ValorTotalAPagar = tituloBaixaAgrupadoDocumento.TituloDocumento.ValorTotal + tituloBaixaAgrupadoDocumento.ValorAcrescimo - tituloBaixaAgrupadoDocumento.ValorDesconto;
            tituloBaixaAgrupadoDocumento.ValorTotalAPagarMoeda = tituloBaixaAgrupadoDocumento.TituloDocumento.ValorTotalMoeda + tituloBaixaAgrupadoDocumento.ValorAcrescimoMoeda - tituloBaixaAgrupadoDocumento.ValorDescontoMoeda;

            if (tituloBaixaAgrupadoDocumento.ValorTotalAPagar < 0m)
            {
                mensagem = "Não é possível remover o valor pois o valor total a pagar ficará negativo (" + tituloBaixaAgrupadoDocumento.ValorTotalAPagar.ToString("n2") + ").";
                return false;
            }

            if (tituloBaixaAgrupadoDocumento.ValorTotalAPagarMoeda < 0m)
            {
                mensagem = "Não é possível remover o valor pois o valor total a pagar em moeda ficará negativo (" + tituloBaixaAgrupadoDocumento.ValorTotalAPagarMoeda.ToString("n2") + ").";
                return false;
            }

            if (tituloBaixaAgrupadoDocumento.ValorTotalAPagar < tituloBaixaAgrupadoDocumento.ValorPago)
                tituloBaixaAgrupadoDocumento.ValorPago = tituloBaixaAgrupadoDocumento.ValorTotalAPagar;

            if (tituloBaixaAgrupadoDocumento.ValorTotalAPagarMoeda < tituloBaixaAgrupadoDocumento.ValorPagoMoeda)
                tituloBaixaAgrupadoDocumento.ValorPagoMoeda = tituloBaixaAgrupadoDocumento.ValorTotalAPagarMoeda;

            repTituloBaixaAgrupadoDocumento.Atualizar(tituloBaixaAgrupadoDocumento);
            repTituloBaixaAgrupadoDocumentoAcrescimoDesconto.Deletar(tituloBaixaAgrupadoDocumentoAcrescimoDesconto);

            mensagem = null;
            return true;
        }

        #endregion

        #region Métodos Privados

        private static Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraBaixaTituloReceberMoeda ObterConfiguracaoMoeda(Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral moeda, Repositorio.UnitOfWork unitOfWork)
        {
            if (ConfiguracaoFinanceiraBaixaTituloReceberMoeda == null || ConfiguracaoFinanceiraBaixaTituloReceberMoeda.Moeda != moeda)
            {
                Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraBaixaTituloReceberMoeda repConfiguracaoFinanceiraBaixaTituloReceberMoeda = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraBaixaTituloReceberMoeda(unitOfWork);

                ConfiguracaoFinanceiraBaixaTituloReceberMoeda = repConfiguracaoFinanceiraBaixaTituloReceberMoeda.BuscarPorMoeda(moeda);

                if (ConfiguracaoFinanceiraBaixaTituloReceberMoeda == null)
                    throw new Exception($"Configuração financeira para baixa de títulos a receber em {moeda.ObterDescricao()} não encontrada.");
            }

            return ConfiguracaoFinanceiraBaixaTituloReceberMoeda;
        }

        private static void GerarSolicitacaoAvaria(Dominio.Entidades.Embarcador.Financeiro.TituloDocumentoAcrescimoDesconto acrescimoDescontoTituloDocumento, Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupadoDocumento tituloBaixaAgrupadoDocumento, Repositorio.UnitOfWork unitOfWork)
        {
            if (acrescimoDescontoTituloDocumento.Justificativa.MotivoAvaria == null)
                return;

            Repositorio.Embarcador.Avarias.SolicitacaoAvaria repSolicitacaoAvaria = new Repositorio.Embarcador.Avarias.SolicitacaoAvaria(unitOfWork);
            Repositorio.Embarcador.Avarias.TempoEtapaSolicitacao repTempoEtapaSolicitacao = new Repositorio.Embarcador.Avarias.TempoEtapaSolicitacao(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);

            Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = acrescimoDescontoTituloDocumento.TituloDocumento.CTe;
            Dominio.Entidades.Embarcador.Cargas.Carga carga = cte != null ? repCargaCTe.BuscarCargaPorCTe(cte.Codigo) : acrescimoDescontoTituloDocumento.TituloDocumento.Carga;
            if (carga == null)
                return;

            Dominio.Entidades.Usuario motorista = carga.Motoristas?.FirstOrDefault() ?? null;
            if (motorista == null)
                return;

            Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvaria solicitacaoAvaria = new Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvaria();

            solicitacaoAvaria.MotivoAvaria = acrescimoDescontoTituloDocumento.Justificativa.MotivoAvaria;
            solicitacaoAvaria.Justificativa = "Gerado automaticamente pela Baixa a Receber referente ao " + acrescimoDescontoTituloDocumento.TituloDocumento.NumeroDocumento;
            solicitacaoAvaria.DataAvaria = DateTime.Now.Date;
            solicitacaoAvaria.Transportador = carga.Empresa;
            solicitacaoAvaria.Carga = carga;
            solicitacaoAvaria.TituloBaixaAgrupadoDocumento = tituloBaixaAgrupadoDocumento;

            solicitacaoAvaria.MotoristaOriginal = motorista.Nome;
            solicitacaoAvaria.RGMotoristaOriginal = motorista.RG;
            solicitacaoAvaria.Motorista = motorista.Nome;
            solicitacaoAvaria.RGMotorista = motorista.RG;
            solicitacaoAvaria.CPFMotorista = motorista.CPF_Formatado;

            solicitacaoAvaria.DataSolicitacao = DateTime.Now;
            solicitacaoAvaria.Solicitante = tituloBaixaAgrupadoDocumento.TituloBaixaAgrupado.TituloBaixa.Usuario;
            solicitacaoAvaria.NumeroAvaria = repSolicitacaoAvaria.BuscarProximoCodigo();
            solicitacaoAvaria.Situacao = SituacaoAvaria.EmCriacao;
            repSolicitacaoAvaria.Inserir(solicitacaoAvaria);

            // Cria o controle de tempo
            Dominio.Entidades.Embarcador.Avarias.TempoEtapaSolicitacao tempoEtapa = new Dominio.Entidades.Embarcador.Avarias.TempoEtapaSolicitacao();
            tempoEtapa.SolicitacaoAvaria = solicitacaoAvaria;
            tempoEtapa.Etapa = EtapaSolicitacao.Criacao;
            tempoEtapa.Entrada = DateTime.Now;
            repTempoEtapaSolicitacao.Inserir(tempoEtapa);
        }

        #endregion
    }
}
