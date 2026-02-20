using AdminMultisoftware.Dominio.Enumeradores;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Terceiros
{
    public class ContratoFreteAcrescimoDesconto : RegraAutorizacao.AprovacaoAlcada
    <
        Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto,
        Dominio.Entidades.Embarcador.Terceiros.AlcadasContratoFreteAcrescimoDesconto.RegraAutorizacaoContratoFreteAcrescimoDesconto,
        Dominio.Entidades.Embarcador.Terceiros.AlcadasContratoFreteAcrescimoDesconto.AprovacaoAlcadaContratoFreteAcrescimoDesconto
    >
    {
        #region Construtores

        public ContratoFreteAcrescimoDesconto(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos

        public dynamic ObterDetalhesContratoFreteAcrescimoDesconto(Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto contrato, Repositorio.UnitOfWork unitOfWork)
        {
            if (contrato == null)
                return null;

            Repositorio.Embarcador.Pessoas.ModalidadeTransportadoraPessoas repModalidadeTransportadoraPessoas = new Repositorio.Embarcador.Pessoas.ModalidadeTransportadoraPessoas(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCIOT repCargaCIOT = new Repositorio.Embarcador.Cargas.CargaCIOT(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT = repCargaCIOT.BuscarPorContrato(contrato.Codigo);

            bool tacAgregado = false;
            if (cargaCIOT != null)
            {
                var modalidadePessoas = cargaCIOT.CIOT.Transportador.Modalidades.FirstOrDefault(o => o.TipoModalidade == TipoModalidade.TransportadorTerceiro);

                if (modalidadePessoas != null)
                {
                    var modalidadeTransportadoraPessoas = repModalidadeTransportadoraPessoas.BuscarPorModalidade(modalidadePessoas.Codigo);

                    if (modalidadeTransportadoraPessoas?.TipoTransportador == TipoProprietarioVeiculo.TACAgregado)
                        tacAgregado = true;
                }
            }

            var retorno = new
            {
                contrato.Codigo,
                contrato.Situacao,
                ContratoFrete = contrato.ContratoFrete == null ? null : new { Codigo = contrato.ContratoFrete.Codigo, Descricao = contrato.ContratoFrete.NumeroContrato.ToString() },
                Justificativa = new { Codigo = contrato.Justificativa.Codigo, Descricao = contrato.Justificativa.Descricao },
                Valor = contrato.Valor.ToString("n2"),
                contrato.Observacao,
                contrato.MotivoRejeicao,
                contrato.PagamentoAutorizado,
                Anexos = ObterAnexos(contrato),
                TacAgregado = tacAgregado,
                contrato.DisponibilizarFechamentoDeAgregado
            };

            return retorno;
        }

        public static object ObterDetalhesAprovacao(Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto contrato, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Terceiros.AlcadasContratoFreteAcrescimoDesconto.AprovacaoAlcadaContratoFreteAcrescimoDesconto repositorioAprovacao = new Repositorio.Embarcador.Terceiros.AlcadasContratoFreteAcrescimoDesconto.AprovacaoAlcadaContratoFreteAcrescimoDesconto(unitOfWork);

            int aprovacoes = repositorioAprovacao.ContarAprovacoes(contrato.Codigo);
            int aprovacoesNecessarias = repositorioAprovacao.ContarAprovacoesNecessarias(contrato.Codigo);
            int reprovacoes = repositorioAprovacao.ContarReprovacoes(contrato.Codigo);

            return new
            {
                AprovacoesNecessarias = aprovacoesNecessarias,
                Aprovacoes = aprovacoes,
                Reprovacoes = reprovacoes,
                DescricaoSituacao = contrato.Situacao.ObterDescricao(),
                contrato.Situacao,
                Solicitante = contrato.Usuario.Nome,
                DataSolicitacao = contrato.Data.ToDateString(),
                contrato.Codigo
            };
        }

        public void EtapaAprovacao(Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto contrato, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            List<Dominio.Entidades.Embarcador.Terceiros.AlcadasContratoFreteAcrescimoDesconto.RegraAutorizacaoContratoFreteAcrescimoDesconto> regras = ObterRegrasAutorizacao(contrato);

            if (regras.Count > 0)
                CriarRegrasAprovacao(contrato, regras, tipoServicoMultisoftware);
            else
                contrato.Situacao = SituacaoContratoFreteAcrescimoDesconto.SemRegra;
        }

        public void GerarIntegracoes(AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Terceiros.ContratoFreteAcrescimoDescontoIntegracao repContratoFreteAcrescimoDescontoIntegracao = new Repositorio.Embarcador.Terceiros.ContratoFreteAcrescimoDescontoIntegracao(_unitOfWork);
            Repositorio.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto repContratoFreteAcrescimoDesconto = new Repositorio.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCIOT repCargaCIOT = new Repositorio.Embarcador.Cargas.CargaCIOT(_unitOfWork);
            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(_unitOfWork);
            Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFrete = new Repositorio.Embarcador.Terceiros.ContratoFrete(_unitOfWork);

            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> integracoes = new List<TipoIntegracao>() { TipoIntegracao.CIOT, TipoIntegracao.Totvs };
            List<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao> tiposIntegracao = repTipoIntegracao.BuscarPorTipos(integracoes);

            if (tiposIntegracao.Count == 0)
                return;

            List<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto> contratos = repContratoFreteAcrescimoDesconto.BuscarPorSituacao(SituacaoContratoFreteAcrescimoDesconto.Aprovado);
            foreach (Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto contratoAcrescimoDesconto in contratos)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT = repCargaCIOT.BuscarPorContrato(contratoAcrescimoDesconto.ContratoFrete.Codigo);

                if (cargaCIOT != null && cargaCIOT.CIOT != null)
                {
                    foreach (Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipo in tiposIntegracao)
                    {
                        Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDescontoIntegracao contratoIntegracao = new Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDescontoIntegracao
                        {
                            TipoIntegracao = tipo,
                            DataIntegracao = DateTime.Now,
                            ProblemaIntegracao = "",
                            SituacaoIntegracao = SituacaoIntegracao.AgIntegracao,
                            ContratoFreteAcrescimoDesconto = contratoAcrescimoDesconto,
                            CIOT = cargaCIOT.CIOT
                        };

                        repContratoFreteAcrescimoDescontoIntegracao.Inserir(contratoIntegracao);
                    }

                    contratoAcrescimoDesconto.Situacao = SituacaoContratoFreteAcrescimoDesconto.AgIntegracao;
                    repContratoFreteAcrescimoDesconto.Atualizar(contratoAcrescimoDesconto);
                }
                else
                {
                    AplicarValorNoContratoFrete(out string mensagem, contratoAcrescimoDesconto, tipoServicoMultisoftware);

                    if (string.IsNullOrWhiteSpace(mensagem))
                    {
                        contratoAcrescimoDesconto.Situacao = SituacaoContratoFreteAcrescimoDesconto.Finalizado;
                        repContratoFreteAcrescimoDesconto.Atualizar(contratoAcrescimoDesconto);

                        contratoAcrescimoDesconto.ContratoFrete.Integrado = false;
                        repContratoFrete.Atualizar(contratoAcrescimoDesconto.ContratoFrete);
                    }
                }
            }
        }

        public void VerificarIntegracoesPendentes(AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Terceiros.ContratoFreteAcrescimoDescontoIntegracao repContratoFreteAcrescimoDescontoIntegracao = new Repositorio.Embarcador.Terceiros.ContratoFreteAcrescimoDescontoIntegracao(_unitOfWork);
            Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregadoDocumento repPagamentoAgregadoDocumento = new Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregadoDocumento(_unitOfWork);
            Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregadoAcrescimoDesconto repPagamentoAgregadoAcrescimoDesconto = new Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregadoAcrescimoDesconto(_unitOfWork);
            Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFrete = new Repositorio.Embarcador.Terceiros.ContratoFrete(_unitOfWork);
            Repositorio.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto repContratoFreteAcrescimoDesconto = new Repositorio.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto(_unitOfWork);
            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDescontoIntegracao> contratosIntegracoes = repContratoFreteAcrescimoDescontoIntegracao.BuscarIntegracaoPendente(1, 5, "Codigo", "asc", 20, TipoEnvioIntegracao.Individual);

            Servicos.Embarcador.Integracao.Totvs.Movimento svsMovimento = new Servicos.Embarcador.Integracao.Totvs.Movimento();

            foreach (Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDescontoIntegracao integracaoContrato in contratosIntegracoes)
            {
                if (integracaoContrato.TipoIntegracao.Tipo == TipoIntegracao.Totvs)
                {
                    bool sucesso = false;
                    string mensagemErro = string.Empty;

                    if (integracaoContrato.ContratoFreteAcrescimoDesconto.Situacao != SituacaoContratoFreteAcrescimoDesconto.Cancelado)
                        sucesso = svsMovimento.IntegrarAcrescimoDescontoContratoTerceiro(integracaoContrato, _unitOfWork, out mensagemErro);
                    else
                        sucesso = svsMovimento.IntegrarCancelamentoAcrescimoDescontoContratoTerceiro(integracaoContrato, _unitOfWork, out mensagemErro);

                    if (sucesso)
                    {
                        integracaoContrato.DataIntegracao = DateTime.Now;
                        integracaoContrato.NumeroTentativas++;
                        integracaoContrato.ProblemaIntegracao = mensagemErro;
                        integracaoContrato.SituacaoIntegracao = sucesso ? SituacaoIntegracao.Integrado : SituacaoIntegracao.ProblemaIntegracao;
                    }
                    else
                    {
                        integracaoContrato.DataIntegracao = DateTime.Now;
                        integracaoContrato.NumeroTentativas++;
                        integracaoContrato.ProblemaIntegracao = mensagemErro;
                        integracaoContrato.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                    }
                }
                else if (integracaoContrato.TipoIntegracao.Tipo == TipoIntegracao.CIOT)
                {
                    bool sucesso = AplicarValorNoContratoFrete(out string mensagemErro, integracaoContrato.ContratoFreteAcrescimoDesconto, tipoServicoMultisoftware);

                    if (sucesso)
                    {
                        Repositorio.Embarcador.Cargas.CargaCIOT repCargaCIOT = new Repositorio.Embarcador.Cargas.CargaCIOT(_unitOfWork);
                        Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT = repCargaCIOT.BuscarPorContrato(integracaoContrato.ContratoFreteAcrescimoDesconto.ContratoFrete.Codigo);

                        sucesso = Servicos.Embarcador.CIOT.CIOT.IntegrarMovimentoFinanceiro(out mensagemErro, cargaCIOT, integracaoContrato.ContratoFreteAcrescimoDesconto.Justificativa, integracaoContrato.ContratoFreteAcrescimoDesconto.Valor, tipoServicoMultisoftware, _unitOfWork);

                        integracaoContrato.DataIntegracao = DateTime.Now;
                        integracaoContrato.NumeroTentativas++;
                        integracaoContrato.ProblemaIntegracao = mensagemErro;
                        integracaoContrato.SituacaoIntegracao = sucesso ? SituacaoIntegracao.Integrado : SituacaoIntegracao.ProblemaIntegracao;

                        if (sucesso)
                        {
                            integracaoContrato.ContratoFreteAcrescimoDesconto.Situacao = SituacaoContratoFreteAcrescimoDesconto.Finalizado;
                            repContratoFreteAcrescimoDesconto.Atualizar(integracaoContrato.ContratoFreteAcrescimoDesconto);

                            Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregado pagamentoAgregado = repPagamentoAgregadoDocumento.BuscarPorContratoEmAberto(integracaoContrato.ContratoFreteAcrescimoDesconto?.ContratoFrete?.Codigo ?? 0);
                            if (pagamentoAgregado != null)
                            {
                                Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregadoAcrescimoDesconto descontoAcrescimo = new Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregadoAcrescimoDesconto()
                                {
                                    ContratoFreteAcrescimoDesconto = integracaoContrato.ContratoFreteAcrescimoDesconto,
                                    Justificativa = integracaoContrato.ContratoFreteAcrescimoDesconto.Justificativa,
                                    PagamentoAgregado = pagamentoAgregado,
                                    Valor = integracaoContrato.ContratoFreteAcrescimoDesconto.Valor
                                };
                                repPagamentoAgregadoAcrescimoDesconto.Inserir(descontoAcrescimo);
                            }

                            integracaoContrato.ContratoFreteAcrescimoDesconto.ContratoFrete.Integrado = false;
                            repContratoFrete.Atualizar(integracaoContrato.ContratoFreteAcrescimoDesconto.ContratoFrete);

                            if (repTipoIntegracao.ExistePorTipo(TipoIntegracao.SAP_AV))
                                GerarIntegracaoSAP_AV(integracaoContrato, repContratoFreteAcrescimoDescontoIntegracao, repTipoIntegracao);

                            if (repTipoIntegracao.ExistePorTipo(TipoIntegracao.KMM))
                                GerarIntegracaoKMM(integracaoContrato, repContratoFreteAcrescimoDescontoIntegracao, repTipoIntegracao);
                        }
                        else
                        {
                            ReverterValorNoContratoFrete(integracaoContrato.ContratoFreteAcrescimoDesconto, tipoServicoMultisoftware, null);

                            integracaoContrato.ContratoFreteAcrescimoDesconto.Situacao = SituacaoContratoFreteAcrescimoDesconto.FalhaIntegracao;
                            repContratoFreteAcrescimoDesconto.Atualizar(integracaoContrato.ContratoFreteAcrescimoDesconto);
                        }
                    }
                    else
                    {
                        integracaoContrato.DataIntegracao = DateTime.Now;
                        integracaoContrato.NumeroTentativas++;
                        integracaoContrato.ProblemaIntegracao = mensagemErro;
                        integracaoContrato.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                    }
                }
                else if (integracaoContrato.TipoIntegracao.Tipo == TipoIntegracao.SAP_AV)
                {
                    bool sucesso = new Servicos.Embarcador.Integracao.SAP.IntegracaoSAP(_unitOfWork).IntegrarCIOTAcrescimosDescontos(integracaoContrato, out string mensagemErro);

                    if (sucesso)
                    {
                        integracaoContrato.DataIntegracao = DateTime.Now;
                        integracaoContrato.NumeroTentativas++;
                        integracaoContrato.ProblemaIntegracao = mensagemErro;
                        integracaoContrato.SituacaoIntegracao = sucesso ? SituacaoIntegracao.Integrado : SituacaoIntegracao.ProblemaIntegracao;
                    }
                    else
                    {
                        integracaoContrato.DataIntegracao = DateTime.Now;
                        integracaoContrato.NumeroTentativas++;
                        integracaoContrato.ProblemaIntegracao = mensagemErro;
                        integracaoContrato.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                    }
                }
                else if (integracaoContrato.TipoIntegracao.Tipo == TipoIntegracao.KMM)
                {
                    bool sucesso = new Servicos.Embarcador.Integracao.KMM.IntegracaoKMM(_unitOfWork).IntegrarCIOTAcrescimosDescontos(integracaoContrato, out string mensagemErro);

                    integracaoContrato.DataIntegracao = DateTime.Now;
                    integracaoContrato.NumeroTentativas++;
                    integracaoContrato.ProblemaIntegracao = mensagemErro;
                    integracaoContrato.SituacaoIntegracao = sucesso ? SituacaoIntegracao.Integrado : SituacaoIntegracao.ProblemaIntegracao;
                }
                else
                {
                    integracaoContrato.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                    integracaoContrato.ProblemaIntegracao = "Tipo de integração não implementada";
                    integracaoContrato.NumeroTentativas++;
                }

                repContratoFreteAcrescimoDescontoIntegracao.Atualizar(integracaoContrato);
            }
        }

        public void AplicarValorNoContratoFrete(Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto contratoAcrescimoDesconto, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, bool controlarTransacao = true)
        {
            if (contratoAcrescimoDesconto.ValorAplicado)
                return;

            Repositorio.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto repContratoFreteAcrescimoDesconto = new Repositorio.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto(_unitOfWork);
            Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFrete = new Repositorio.Embarcador.Terceiros.ContratoFrete(_unitOfWork);
            Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraContratoFreteTerceiros repConfiguracaoFinanceiraContratoFreteTerceiros = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraContratoFreteTerceiros(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro repConfiguracaoFinanceiro = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro(_unitOfWork);

            Servicos.Embarcador.Financeiro.ProcessoMovimento servProcessoMovimento = new Servicos.Embarcador.Financeiro.ProcessoMovimento(_unitOfWork.StringConexao);
            Servicos.Embarcador.Terceiros.ContratoFrete servicoContratoFrete = new Servicos.Embarcador.Terceiros.ContratoFrete(_unitOfWork, tipoServicoMultisoftware);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraContratoFreteTerceiros configuracaoFinanceiraContratoFreteTerceiros = repConfiguracaoFinanceiraContratoFreteTerceiros.BuscarPrimeiroRegistro();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiro configuracaoFinanceiro = repConfiguracaoFinanceiro.BuscarPrimeiroRegistro();
            Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contratoFrete = contratoAcrescimoDesconto.ContratoFrete;
            Dominio.Entidades.Embarcador.Fatura.Justificativa justificativa = contratoAcrescimoDesconto.Justificativa;

            if (controlarTransacao)
                _unitOfWork.Start();

            if (!servicoContratoFrete.AdicionarValorAoContrato(out string erro, ref contratoFrete, contratoAcrescimoDesconto.Valor, justificativa.Codigo, "Gerado automaticamente pelo acréscimo/desconto do contrato de frete", 0, auditado, true))
            {
                if (controlarTransacao)
                    _unitOfWork.Rollback();

                throw new ServicoException(erro);
            }

            Servicos.Embarcador.Terceiros.ContratoFrete.CalcularImpostos(ref contratoFrete, _unitOfWork, tipoServicoMultisoftware, false);

            if (contratoFrete.SaldoAReceber < 0m || contratoFrete.ValorFreteSubcontratacao < 0m || contratoFrete.ValorAdiantamento < 0m)
            {
                if (controlarTransacao)
                    _unitOfWork.Rollback();

                throw new ServicoException("Não é possível aplicar o valor, pois o contrato de frete ficará com valores negativos.");
            }

            repContratoFrete.Atualizar(contratoFrete, auditado);

            DateTime dataMovimento = servicoContratoFrete.ObterDataParaMovimentoFinanceiroDoContrato(contratoAcrescimoDesconto.ContratoFrete, justificativa);

            //Atualizar o título
            Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = repTitulo.BuscarPorContratoFrete(contratoFrete.Codigo, justificativa.AplicacaoValorContratoFrete == AplicacaoValorJustificativaContratoFrete.NoAdiantamento);

            if (titulo != null)
            {
                if (titulo.StatusTitulo != StatusTitulo.EmAberto)
                {
                    if (controlarTransacao)
                        _unitOfWork.Rollback();

                    throw new ServicoException("Título do contrato de frete não está mais em aberto");
                }

                if (configuracaoFinanceiro?.UtilizarDataVencimentoTituloMovimentoContrato ?? false)
                    dataMovimento = titulo.DataVencimento.HasValue ? titulo.DataVencimento.Value : dataMovimento;

                decimal valorAntigo = titulo.ValorOriginal;
                if (justificativa.TipoJustificativa == TipoJustificativa.Acrescimo)
                {
                    titulo.ValorOriginal += contratoAcrescimoDesconto.Valor;
                    titulo.ValorPendente += contratoAcrescimoDesconto.Valor;
                }
                else
                {
                    titulo.ValorOriginal -= contratoAcrescimoDesconto.Valor;
                    titulo.ValorPendente -= contratoAcrescimoDesconto.Valor;
                }

                repTitulo.Atualizar(titulo, auditado);

                if (titulo.TipoMovimento != null)
                {
                    string msgErro;
                    //Reverte o valor antigo
                    if (!servProcessoMovimento.GerarMovimentacao(out msgErro, null, dataMovimento, valorAntigo, contratoFrete.NumeroContrato.ToString(), $"Referente a reversão de movimento para aplicação de acréscimo/desconto no contrato de frete nº {contratoFrete.NumeroContrato}, título {titulo.Codigo}.", _unitOfWork, TipoDocumentoMovimento.ContratoFrete, tipoServicoMultisoftware, 0, titulo.TipoMovimento.PlanoDeContaDebito, titulo.TipoMovimento.PlanoDeContaCredito, titulo.Codigo, null, titulo.Pessoa, titulo.Pessoa?.GrupoPessoas ?? null))
                    {
                        if (controlarTransacao)
                            _unitOfWork.Rollback();

                        throw new ServicoException(msgErro);
                    }

                    //Gera movimento com o novo valor
                    if (!servProcessoMovimento.GerarMovimentacao(out msgErro, titulo.TipoMovimento, dataMovimento, titulo.ValorOriginal, contratoFrete.NumeroContrato.ToString(), $"Referente a aplicação de acréscimo/desconto no contrato de frete nº {contratoFrete.NumeroContrato}, título {titulo.Codigo}.", _unitOfWork, TipoDocumentoMovimento.ContratoFrete, tipoServicoMultisoftware, 0, null, null, titulo.Codigo, null, titulo.Pessoa, titulo.Pessoa?.GrupoPessoas ?? null))
                    {
                        if (controlarTransacao)
                            _unitOfWork.Rollback();

                        throw new ServicoException(msgErro);
                    }
                }
            }
            else if ((justificativa.AplicacaoValorContratoFrete == AplicacaoValorJustificativaContratoFrete.NoAdiantamento || contratoAcrescimoDesconto.ContratoFrete.SituacaoContratoFrete == SituacaoContratoFrete.Finalizada) && configuracaoFinanceiraContratoFreteTerceiros != null)
            {
                string obsAdiantamento = $"Referente ao {justificativa.TipoJustificativa.ObterDescricao().ToLower()} concedido {(justificativa.AplicacaoValorContratoFrete.HasValue ? justificativa.AplicacaoValorContratoFrete.Value.ObterDescricao().ToLower() : "")} do contrato de frete nº " + contratoAcrescimoDesconto.ContratoFrete.NumeroContrato + ", liberado automaticamente à partir do CIOT.";

                Dominio.Entidades.Embarcador.CIOT.CIOTConfiguracaoFinanceira configuracaoFinanceiraCIOT = null;
                if (contratoFrete.ConfiguracaoCIOT?.ConfiguracaoMovimentoFinanceiro ?? false)
                {
                    configuracaoFinanceiraCIOT = servicoContratoFrete.ObterCIOTConfiguracaoFinanceira(contratoFrete, contratoFrete.ConfiguracaoCIOT, _unitOfWork);
                    if (configuracaoFinanceiraCIOT != null)
                        servProcessoMovimento.GerarMovimentacao(justificativa.TipoJustificativa == TipoJustificativa.Acrescimo ? configuracaoFinanceiraCIOT.TipoMovimentoParaUso : configuracaoFinanceiraCIOT.TipoMovimentoParaReversao, dataMovimento, contratoAcrescimoDesconto.Valor, contratoAcrescimoDesconto.ContratoFrete.NumeroContrato.ToString(), obsAdiantamento, _unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.ContratoFrete, tipoServicoMultisoftware, 0, null, null, 0, null, contratoAcrescimoDesconto.ContratoFrete.TransportadorTerceiro);
                }

                if (configuracaoFinanceiraCIOT == null)
                {
                    if (configuracaoFinanceiraContratoFreteTerceiros.GerarMovimentoAutomaticoNaGeracaoContratoFrete && configuracaoFinanceiraContratoFreteTerceiros.TipoMovimentoValorPagoTerceiroCIOT != null && configuracaoFinanceiraContratoFreteTerceiros.TipoMovimentoReversaoValorPagoTerceiroCIOT != null)
                        servProcessoMovimento.GerarMovimentacao(justificativa.TipoJustificativa == TipoJustificativa.Acrescimo ? configuracaoFinanceiraContratoFreteTerceiros.TipoMovimentoValorPagoTerceiroCIOT : configuracaoFinanceiraContratoFreteTerceiros.TipoMovimentoReversaoValorPagoTerceiroCIOT, dataMovimento, contratoAcrescimoDesconto.Valor, contratoAcrescimoDesconto.ContratoFrete.NumeroContrato.ToString(), obsAdiantamento, _unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.ContratoFrete, tipoServicoMultisoftware, 0, null, null, 0, null, contratoAcrescimoDesconto.ContratoFrete.TransportadorTerceiro);
                    else if (configuracaoFinanceiraContratoFreteTerceiros.GerarMovimentoAutomaticoPorTipoOperacao)
                    {
                        Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraContratoFreteTerceirosTipoOperacao configuracaoFinanceiraTipoOperacao = Servicos.Embarcador.Terceiros.ContratoFrete.ObterConfiguracaoFinanceiraPorTipoOperacao(contratoAcrescimoDesconto.ContratoFrete, configuracaoFinanceiraContratoFreteTerceiros, _unitOfWork);

                        if (configuracaoFinanceiraTipoOperacao != null && configuracaoFinanceiraTipoOperacao.TipoMovimentoPagamentoViaCIOT != null && configuracaoFinanceiraTipoOperacao.TipoMovimentoReversaoPagamentoViaCIOT != null)
                            servProcessoMovimento.GerarMovimentacao(justificativa.TipoJustificativa == TipoJustificativa.Acrescimo ? configuracaoFinanceiraTipoOperacao.TipoMovimentoPagamentoViaCIOT : configuracaoFinanceiraTipoOperacao.TipoMovimentoReversaoPagamentoViaCIOT, dataMovimento, contratoAcrescimoDesconto.Valor, contratoAcrescimoDesconto.ContratoFrete.NumeroContrato.ToString(), obsAdiantamento, _unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.ContratoFrete, tipoServicoMultisoftware, 0, null, null, 0, null, contratoAcrescimoDesconto.ContratoFrete.TransportadorTerceiro);
                    }
                }
            }

            contratoAcrescimoDesconto.MotivoRejeicao = string.Empty;
            contratoAcrescimoDesconto.ValorAplicado = true;

            repContratoFreteAcrescimoDesconto.Atualizar(contratoAcrescimoDesconto, auditado);

            if (controlarTransacao)
                _unitOfWork.CommitChanges();
        }

        public void GerarIntegracaoCancelamentoTotvs(Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto contratoAcrescimoDesconto, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            if (string.IsNullOrWhiteSpace(contratoAcrescimoDesconto.CodigoIntegracaoBaixa) && string.IsNullOrWhiteSpace(contratoAcrescimoDesconto.CodigoIntegracaoLancamento))
                return;

            Repositorio.Embarcador.Terceiros.ContratoFreteAcrescimoDescontoIntegracao repContratoFreteAcrescimoDescontoIntegracao = new Repositorio.Embarcador.Terceiros.ContratoFreteAcrescimoDescontoIntegracao(_unitOfWork);
            Repositorio.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto repContratoFreteAcrescimoDesconto = new Repositorio.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto(_unitOfWork);
            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(_unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoTotvs = repTipoIntegracao.BuscarPorTipo(TipoIntegracao.Totvs);

            if (tipoTotvs == null)
                return;

            Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDescontoIntegracao contratoIntegracao = new Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDescontoIntegracao
            {
                TipoIntegracao = tipoTotvs,
                DataIntegracao = DateTime.Now,
                ProblemaIntegracao = "",
                SituacaoIntegracao = SituacaoIntegracao.AgIntegracao,
                ContratoFreteAcrescimoDesconto = contratoAcrescimoDesconto,
                CIOT = null
            };
            repContratoFreteAcrescimoDescontoIntegracao.Inserir(contratoIntegracao);

        }

        public void ReverterValorNoContratoFrete(Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto contratoAcrescimoDesconto, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, bool controlarTransacao = true)
        {
            if (!contratoAcrescimoDesconto.ValorAplicado)
                return;

            Repositorio.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto repContratoFreteAcrescimoDesconto = new Repositorio.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto(_unitOfWork);
            Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFrete = new Repositorio.Embarcador.Terceiros.ContratoFrete(_unitOfWork);
            Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraContratoFreteTerceiros repConfiguracaoFinanceiraContratoFreteTerceiros = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraContratoFreteTerceiros(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro repConfiguracaoFinanceiro = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro(_unitOfWork);

            Servicos.Embarcador.Financeiro.ProcessoMovimento servProcessoMovimento = new Servicos.Embarcador.Financeiro.ProcessoMovimento(_unitOfWork.StringConexao);
            Servicos.Embarcador.Terceiros.ContratoFrete servicoContratoFrete = new Servicos.Embarcador.Terceiros.ContratoFrete(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraContratoFreteTerceiros configuracaoFinanceiraContratoFreteTerceiros = repConfiguracaoFinanceiraContratoFreteTerceiros.BuscarPrimeiroRegistro();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiro configuracaoFinanceiro = repConfiguracaoFinanceiro.BuscarPrimeiroRegistro();
            Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contratoFrete = contratoAcrescimoDesconto.ContratoFrete;
            Dominio.Entidades.Embarcador.Fatura.Justificativa justificativa = contratoAcrescimoDesconto.Justificativa;

            if (controlarTransacao)
                _unitOfWork.Start();

            if (!Servicos.Embarcador.Terceiros.ContratoFrete.ReverterValorAoContrato(out string erro, ref contratoFrete, contratoAcrescimoDesconto.Valor, justificativa.Codigo, "Reversão do acréscimo/desconto do contrato de frete", 0, _unitOfWork, auditado))
            {
                if (controlarTransacao)
                    _unitOfWork.Rollback();

                throw new ServicoException(erro);
            }

            Servicos.Embarcador.Terceiros.ContratoFrete.CalcularImpostos(ref contratoFrete, _unitOfWork, tipoServicoMultisoftware, false);

            repContratoFrete.Atualizar(contratoFrete, auditado);

            DateTime dataMovimento = servicoContratoFrete.ObterDataParaMovimentoFinanceiroDoContrato(contratoAcrescimoDesconto.ContratoFrete, _unitOfWork);

            //Atualizar o título
            Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = repTitulo.BuscarPorContratoFrete(contratoFrete.Codigo, justificativa.AplicacaoValorContratoFrete == AplicacaoValorJustificativaContratoFrete.NoAdiantamento);

            if (titulo != null)
            {
                if (titulo.StatusTitulo != StatusTitulo.EmAberto)
                {
                    if (controlarTransacao)
                        _unitOfWork.Rollback();

                    throw new ServicoException("Título do contrato de frete não está mais em aberto");
                }

                if (configuracaoFinanceiro?.UtilizarDataVencimentoTituloMovimentoContrato ?? false)
                    dataMovimento = titulo.DataVencimento.HasValue ? titulo.DataVencimento.Value : dataMovimento;

                decimal valorAntigo = titulo.ValorOriginal;
                if (justificativa.TipoJustificativa == TipoJustificativa.Acrescimo)
                {
                    titulo.ValorOriginal -= contratoAcrescimoDesconto.Valor;
                    titulo.ValorPendente -= contratoAcrescimoDesconto.Valor;
                }
                else
                {
                    titulo.ValorOriginal += contratoAcrescimoDesconto.Valor;
                    titulo.ValorPendente += contratoAcrescimoDesconto.Valor;
                }

                repTitulo.Atualizar(titulo, auditado);

                if (titulo.TipoMovimento != null)
                {
                    //Reverte o valor antigo
                    if (!servProcessoMovimento.GerarMovimentacao(out string msgErro, null, dataMovimento, valorAntigo, contratoFrete.NumeroContrato.ToString(), $"Referente a reversão de movimento para aplicação de acréscimo/desconto no contrato de frete nº {contratoFrete.NumeroContrato}, título {titulo.Codigo}.", _unitOfWork, TipoDocumentoMovimento.ContratoFrete, tipoServicoMultisoftware, 0, titulo.TipoMovimento.PlanoDeContaDebito, titulo.TipoMovimento.PlanoDeContaCredito, titulo.Codigo, null, titulo.Pessoa, titulo.Pessoa?.GrupoPessoas ?? null))
                    {
                        if (controlarTransacao)
                            _unitOfWork.Rollback();

                        throw new ServicoException(msgErro);
                    }
                }
            }
            else if (justificativa.AplicacaoValorContratoFrete == AplicacaoValorJustificativaContratoFrete.NoAdiantamento && configuracaoFinanceiraContratoFreteTerceiros != null)
            {
                string obsAdiantamento = $"Referente à reversão do {(justificativa.TipoJustificativa == TipoJustificativa.Acrescimo ? "acréscimo" : "desconto")} concedido no adiantamento do contrato de frete nº " + contratoAcrescimoDesconto.ContratoFrete.NumeroContrato + ", liberado automaticamente à partir do CIOT.";

                Dominio.Entidades.Embarcador.CIOT.CIOTConfiguracaoFinanceira configuracaoFinanceiraCIOT = null;
                if (contratoFrete.ConfiguracaoCIOT?.ConfiguracaoMovimentoFinanceiro ?? false)
                {
                    configuracaoFinanceiraCIOT = servicoContratoFrete.ObterCIOTConfiguracaoFinanceira(contratoFrete, contratoFrete.ConfiguracaoCIOT, _unitOfWork);
                    if (configuracaoFinanceiraCIOT != null)
                        servProcessoMovimento.GerarMovimentacao(justificativa.TipoJustificativa == TipoJustificativa.Acrescimo ? configuracaoFinanceiraCIOT.TipoMovimentoParaReversao : configuracaoFinanceiraCIOT.TipoMovimentoParaUso, dataMovimento, contratoAcrescimoDesconto.Valor, contratoAcrescimoDesconto.ContratoFrete.NumeroContrato.ToString(), obsAdiantamento, _unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.ContratoFrete, tipoServicoMultisoftware, 0, null, null, 0, null, contratoAcrescimoDesconto.ContratoFrete.TransportadorTerceiro);
                }

                if (configuracaoFinanceiraCIOT == null)
                {
                    if (configuracaoFinanceiraContratoFreteTerceiros.GerarMovimentoAutomaticoNaGeracaoContratoFrete && configuracaoFinanceiraContratoFreteTerceiros.TipoMovimentoValorPagoTerceiroCIOT != null && configuracaoFinanceiraContratoFreteTerceiros.TipoMovimentoReversaoValorPagoTerceiroCIOT != null)
                        servProcessoMovimento.GerarMovimentacao(justificativa.TipoJustificativa == TipoJustificativa.Acrescimo ? configuracaoFinanceiraContratoFreteTerceiros.TipoMovimentoReversaoValorPagoTerceiroCIOT : configuracaoFinanceiraContratoFreteTerceiros.TipoMovimentoValorPagoTerceiroCIOT, dataMovimento, contratoAcrescimoDesconto.Valor, contratoAcrescimoDesconto.ContratoFrete.NumeroContrato.ToString(), obsAdiantamento, _unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.ContratoFrete, tipoServicoMultisoftware, 0, null, null, 0, null, contratoAcrescimoDesconto.ContratoFrete.TransportadorTerceiro);
                    else if (configuracaoFinanceiraContratoFreteTerceiros.GerarMovimentoAutomaticoPorTipoOperacao)
                    {
                        Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraContratoFreteTerceirosTipoOperacao configuracaoFinanceiraTipoOperacao = Servicos.Embarcador.Terceiros.ContratoFrete.ObterConfiguracaoFinanceiraPorTipoOperacao(contratoAcrescimoDesconto.ContratoFrete, configuracaoFinanceiraContratoFreteTerceiros, _unitOfWork);

                        if (configuracaoFinanceiraTipoOperacao != null && configuracaoFinanceiraTipoOperacao.TipoMovimentoPagamentoViaCIOT != null && configuracaoFinanceiraTipoOperacao.TipoMovimentoReversaoPagamentoViaCIOT != null)
                            servProcessoMovimento.GerarMovimentacao(justificativa.TipoJustificativa == TipoJustificativa.Acrescimo ? configuracaoFinanceiraTipoOperacao.TipoMovimentoReversaoPagamentoViaCIOT : configuracaoFinanceiraTipoOperacao.TipoMovimentoPagamentoViaCIOT, dataMovimento, contratoAcrescimoDesconto.Valor, contratoAcrescimoDesconto.ContratoFrete.NumeroContrato.ToString(), obsAdiantamento, _unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.ContratoFrete, tipoServicoMultisoftware, 0, null, null, 0, null, contratoAcrescimoDesconto.ContratoFrete.TransportadorTerceiro);
                    }
                }
            }

            contratoAcrescimoDesconto.MotivoRejeicao = string.Empty;
            contratoAcrescimoDesconto.ValorAplicado = false;

            repContratoFreteAcrescimoDesconto.Atualizar(contratoAcrescimoDesconto, auditado);

            if (controlarTransacao)
                _unitOfWork.CommitChanges();
        }

        public bool? GerarIntegracaoContratoFreteAcrescimoDesconto(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contratoFrete, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(_unitOfWork);

            List<TipoIntegracao> tiposIntegracaoAutorizados = new List<TipoIntegracao> { TipoIntegracao.KMM };

            List<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao> tiposIntegracao = repTipoIntegracao.BuscarPorTipos(tiposIntegracaoAutorizados);

            if (tiposIntegracao.Count == 0)
                return null;

            if (carga == null)
                return null;

            foreach (Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao in tiposIntegracao)
            {
                if (tipoIntegracao.Tipo == TipoIntegracao.KMM)
                {
                    Repositorio.Embarcador.Configuracoes.IntegracaoKMM repIntegracaoKMM = new Repositorio.Embarcador.Configuracoes.IntegracaoKMM(_unitOfWork);
                    Dominio.Entidades.Embarcador.Configuracoes.IntegracaoKMM integracaoKMM = repIntegracaoKMM.Buscar();
                    Servicos.Embarcador.Integracao.KMM.IntegracaoKMM serIntegracaoKMM = new Integracao.KMM.IntegracaoKMM(_unitOfWork, tipoServicoMultisoftware);
                    Repositorio.Embarcador.Pessoas.ModalidadeTransportadoraPessoas repModalidadeTerceiro = new Repositorio.Embarcador.Pessoas.ModalidadeTransportadoraPessoas(_unitOfWork);
                    Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidadeTransportadoraPessoas = repModalidadeTerceiro.BuscarPorPessoa(carga.Terceiro.CPF_CNPJ);

                    if (integracaoKMM == null)
                        continue;

                    if (integracaoKMM.PossuiIntegracao)
                    {
                        if ((carga.TipoOperacao?.ConfiguracaoIntegracao?.ConsultarTaxasKMM ?? false) &&
                            carga.TipoOperacao.ConfiguracaoIntegracao.TiposTerceiros.Contains(modalidadeTransportadoraPessoas?.TipoTerceiro))
                            return serIntegracaoKMM.AdicionarContratoFreteValoresAcrescimoDescontoIntegracao(carga, tipoIntegracao, contratoFrete, auditado, tipoServicoMultisoftware);
                    }
                }
            }
            return null;
        }

        public bool GerarContratoFreteAcrescimoDescontoApartirChamado(Dominio.Entidades.Embarcador.Chamados.Chamado chamado, ref string mensagemErro, Dominio.Entidades.Usuario usuario, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, bool controlarTransacao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoContratoFreteTerceiro repConfiguracaoContratoFreteTerceiro = new Repositorio.Embarcador.Configuracoes.ConfiguracaoContratoFreteTerceiro(unitOfWork);
            Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFrete = new Repositorio.Embarcador.Terceiros.ContratoFrete(unitOfWork);

            mensagemErro = string.Empty;
            if (!(chamado?.MotivoChamado?.GerarAcrescimoDescontoContratoFrete ?? false) || chamado.Carga == null || chamado?.MotivoChamado?.JustificativaAcrescimoDescontoContratoFrete == null || chamado.Valor == 0)
                return true;

            Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contratoFrete = repContratoFrete.BuscarPorCarga(chamado.Carga.Codigo);

            if (contratoFrete == null)
                return true;

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoProprietarioVeiculo? tipoTransportador = this.ObterTipoTransportadorContratoFrete(contratoFrete.Codigo, unitOfWork);

            if ((chamado?.MotivoChamado?.TipoTransportadorAcrescimoDesconto ?? TipoProprietarioVeiculo.Todos) != TipoProprietarioVeiculo.Todos && chamado?.MotivoChamado?.TipoTransportadorAcrescimoDesconto != tipoTransportador)
                return true;

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoContratoFreteTerceiro configuracaoContratoFreteTerceiro = repConfiguracaoContratoFreteTerceiro.BuscarConfiguracaoPadrao();

            int codigoContratoFrete = contratoFrete.Codigo;
            int codigoJustificativa = chamado.MotivoChamado.JustificativaAcrescimoDescontoContratoFrete.Codigo;
            decimal valor = chamado.Valor;

            // Caso seja Tac Agregado já marca para disponibilizar para o fechamento de agregado automaticamente.
            bool disponibilizarFechamentoDeAgregado = tipoTransportador == TipoProprietarioVeiculo.TACAgregado && configuracaoContratoFreteTerceiro.UtilizarFechamentoDeAgregado ? true : false;
            string observacao = $"GERADO A PARTIR DO ATENDIMENTO Nº {chamado.Numero} REFERENTE AO ADIANTAMENTO DO MOTORISTA";

            bool gerou = this.GerarContratoFreteAcrescimoDesconto(ref mensagemErro, codigoContratoFrete, codigoJustificativa, valor, disponibilizarFechamentoDeAgregado, observacao, usuario, tipoServicoMultisoftware, auditado, controlarTransacao, unitOfWork);

            if (!gerou)
                mensagemErro = $"Falha ao incluir acréscimo ou desconto no contrato de frete: {mensagemErro}";

            return gerou;
        }

        public bool GerarContratoFreteAcrescimoDesconto(ref string mensagemErro, int codigoContratoFrete, int codigoJustificativa, decimal valor, bool disponibilizarFechamentoDeAgregado, string observacao, Dominio.Entidades.Usuario usuario, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, bool controlarTransacao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto repContratoFreteAcrescimoDesconto = new Repositorio.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto(unitOfWork);
            Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto contrato = new Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto();

            if (!this.ValidarAdicionarContratoFreteAcrescimoDesconto(ref contrato, ref mensagemErro, codigoContratoFrete, codigoJustificativa, valor, disponibilizarFechamentoDeAgregado, unitOfWork))
                return false;

            if (controlarTransacao)
                unitOfWork.Start();

            contrato.Data = DateTime.Now;
            contrato.Usuario = usuario;
            contrato.Situacao = SituacaoContratoFreteAcrescimoDesconto.SemRegra;
            contrato.Valor = valor;
            contrato.Observacao = observacao;
            contrato.DisponibilizarFechamentoDeAgregado = disponibilizarFechamentoDeAgregado;

            repContratoFreteAcrescimoDesconto.Inserir(contrato, auditado);

            this.EtapaAprovacao(contrato, tipoServicoMultisoftware);
            repContratoFreteAcrescimoDesconto.Atualizar(contrato);

            if (controlarTransacao)
                unitOfWork.CommitChanges();

            return true;
        }

        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoProprietarioVeiculo? ObterTipoTransportadorContratoFrete(int codigoContratoFrete, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pessoas.ModalidadeTransportadoraPessoas repModalidadeTransportadoraPessoas = new Repositorio.Embarcador.Pessoas.ModalidadeTransportadoraPessoas(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCIOT repCargaCIOT = new Repositorio.Embarcador.Cargas.CargaCIOT(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT = repCargaCIOT.BuscarPorContrato(codigoContratoFrete);

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoProprietarioVeiculo? tipoTransportador = null;
            if (cargaCIOT != null)
            {
                var modalidadePessoas = cargaCIOT.CIOT.Transportador.Modalidades.FirstOrDefault(o => o.TipoModalidade == TipoModalidade.TransportadorTerceiro);

                if (modalidadePessoas != null)
                {
                    var modalidadeTransportadoraPessoas = repModalidadeTransportadoraPessoas.BuscarPorModalidade(modalidadePessoas.Codigo);
                    tipoTransportador = modalidadeTransportadoraPessoas?.TipoTransportador;
                }
            }

            return tipoTransportador;
            ;
        }

        public bool VerificarContratoFreteTacAgregado(int codigoContratoFrete, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pessoas.ModalidadeTransportadoraPessoas repModalidadeTransportadoraPessoas = new Repositorio.Embarcador.Pessoas.ModalidadeTransportadoraPessoas(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCIOT repCargaCIOT = new Repositorio.Embarcador.Cargas.CargaCIOT(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT = repCargaCIOT.BuscarPorContrato(codigoContratoFrete);

            bool tacAgregado = false;
            if (cargaCIOT != null)
            {
                var modalidadePessoas = cargaCIOT.CIOT.Transportador.Modalidades.FirstOrDefault(o => o.TipoModalidade == TipoModalidade.TransportadorTerceiro);

                if (modalidadePessoas != null)
                {
                    var modalidadeTransportadoraPessoas = repModalidadeTransportadoraPessoas.BuscarPorModalidade(modalidadePessoas.Codigo);

                    if (modalidadeTransportadoraPessoas?.TipoTransportador == TipoProprietarioVeiculo.TACAgregado)
                        tacAgregado = true;
                }
            }

            return tacAgregado;
            ;
        }

        public bool ValidarAdicionarContratoFreteAcrescimoDesconto(ref Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto contrato, ref string mensagemErro, int codigoContratoFrete, int codigoJustificativa, decimal valor, bool disponibilizarFechamentoDeAgregado, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto repContratoFreteAcrescimoDesconto = new Repositorio.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto(unitOfWork);
            Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFrete = new Repositorio.Embarcador.Terceiros.ContratoFrete(unitOfWork);
            Repositorio.Embarcador.Fatura.Justificativa repJustificativa = new Repositorio.Embarcador.Fatura.Justificativa(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCIOT repCargaCIOT = new Repositorio.Embarcador.Cargas.CargaCIOT(unitOfWork);

            if (repContratoFreteAcrescimoDesconto.ContratoEstaQuitado(codigoContratoFrete))
            {
                mensagemErro = "O contrato já possui parcelas quitadas, não podendo aplicar acréscimo/desconto.";
                return false;
            }

            List<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto> listaContratoFreteAcrescimoDesconto = repContratoFreteAcrescimoDesconto.BuscarPorSituacaoEContrato(SituacaoContratoFreteAcrescimoDesconto.FalhaIntegracao, codigoContratoFrete);

            if (listaContratoFreteAcrescimoDesconto?.Count > 0)
            {
                mensagemErro = "Existem integrações com falha. Será necessário tratar o evento antes de adicionar outros.";
                return false;
            }

            Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contratoFrete = repContratoFrete.BuscarPorCodigo(codigoContratoFrete);

            Dominio.Entidades.Embarcador.Fatura.Justificativa justificativa = repJustificativa.BuscarPorCodigo(codigoJustificativa);

            if (justificativa.AplicacaoValorContratoFrete == AplicacaoValorJustificativaContratoFrete.NoAdiantamento)
            {
                //if (justificativa.TipoJustificativa == TipoJustificativa.Acrescimo && ((valor + contratoFrete.ValorAdiantamento) > contratoFrete.ValorFreteSubcontratacao))
                //    throw new ControllerException("Valor do adiantamento será maior que o valor total do contrato!");

                if (justificativa.TipoJustificativa == TipoJustificativa.Desconto && ((contratoFrete.ValorAdiantamento - valor) < 0))
                {
                    mensagemErro = "Valor do adiantamento ficará menor que zero!";
                    return false;
                }
            }
            else if (justificativa.TipoJustificativa == TipoJustificativa.Desconto)
            {
                if ((contratoFrete.ValorSaldo - valor) <= 0)
                {
                    mensagemErro = "Valor do contrato ficará menor que zero!";
                    return false;
                }
            }

            Dominio.Entidades.Embarcador.Documentos.CIOT ciot = null;
            if (disponibilizarFechamentoDeAgregado)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT = repCargaCIOT.BuscarPorContrato(contratoFrete.Codigo);

                if (cargaCIOT?.CIOT == null)
                {
                    mensagemErro = "Contrato de frete não está vinculado a CIOT";
                    return false;
                }

                if (cargaCIOT.CIOT.Situacao != SituacaoCIOT.Aberto)
                {
                    mensagemErro = "CIOT deve estar na situação aberta para inclusão de acréscimo/desconto.";
                    return false;
                }

                ciot = cargaCIOT.CIOT;
            }

            contrato.ContratoFrete = contratoFrete;
            contrato.Justificativa = justificativa;
            contrato.CIOT = ciot;

            return true;
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void NotificarAprovador(Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto contratoFreteAcrescimoDesconto, Dominio.Entidades.Embarcador.Terceiros.AlcadasContratoFreteAcrescimoDesconto.AprovacaoAlcadaContratoFreteAcrescimoDesconto aprovacao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Notificacao.Notificacao servicoNotificacao = new Notificacao.Notificacao(_unitOfWork.StringConexao, cliente: null, tipoServicoMultisoftware: tipoServicoMultisoftware, adminStringConexao: string.Empty);

            servicoNotificacao.GerarNotificacaoEmail(
                usuario: aprovacao.Usuario,
                usuarioGerouNotificacao: null,
                codigoObjeto: contratoFreteAcrescimoDesconto.Codigo,
                URLPagina: "Terceiros/ContratoFreteAcrescimoDesconto",
                titulo: Localization.Resources.Fretes.ContratoFrete.AcrescimoDescontoContratoFrete,
                nota: string.Format(Localization.Resources.Fretes.ContratoFrete.CriadaSolicitacaoAprovacaoAcrescimoDescontoContratoFrete, contratoFreteAcrescimoDesconto.ContratoFrete.NumeroContrato),
                icone: IconesNotificacao.cifra,
                tipoNotificacao: TipoNotificacao.credito,
                tipoServicoMultisoftwareNotificar: tipoServicoMultisoftware,
                unitOfWork: _unitOfWork
            );
        }

        #endregion

        #region Métodos Privados

        private void CriarRegrasAprovacao(Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto contrato, List<Dominio.Entidades.Embarcador.Terceiros.AlcadasContratoFreteAcrescimoDesconto.RegraAutorizacaoContratoFreteAcrescimoDesconto> regras, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            bool existeRegraSemAprovacao = false;
            Repositorio.Embarcador.Terceiros.AlcadasContratoFreteAcrescimoDesconto.AprovacaoAlcadaContratoFreteAcrescimoDesconto repositorio = new Repositorio.Embarcador.Terceiros.AlcadasContratoFreteAcrescimoDesconto.AprovacaoAlcadaContratoFreteAcrescimoDesconto(_unitOfWork);
            int menorPrioridadeAprovacao = regras.Where(regra => regra.NumeroAprovadores > 0).Select(regra => (int?)regra.PrioridadeAprovacao).Min() ?? 0;

            foreach (Dominio.Entidades.Embarcador.Terceiros.AlcadasContratoFreteAcrescimoDesconto.RegraAutorizacaoContratoFreteAcrescimoDesconto regra in regras)
            {
                if (regra.NumeroAprovadores > 0)
                {
                    existeRegraSemAprovacao = true;

                    foreach (var aprovador in regra.Aprovadores)
                    {
                        Dominio.Entidades.Embarcador.Terceiros.AlcadasContratoFreteAcrescimoDesconto.AprovacaoAlcadaContratoFreteAcrescimoDesconto aprovacao = new Dominio.Entidades.Embarcador.Terceiros.AlcadasContratoFreteAcrescimoDesconto.AprovacaoAlcadaContratoFreteAcrescimoDesconto()
                        {
                            OrigemAprovacao = contrato,
                            Bloqueada = regra.PrioridadeAprovacao > menorPrioridadeAprovacao,
                            Usuario = aprovador,
                            RegraAutorizacao = regra,
                            Situacao = SituacaoAlcadaRegra.Pendente,
                            DataCriacao = contrato.Data,
                            NumeroAprovadores = regra.NumeroAprovadores
                        };

                        repositorio.Inserir(aprovacao);

                        if (!aprovacao.Bloqueada)
                            NotificarAprovador(contrato, aprovacao, tipoServicoMultisoftware);
                    }
                }
                else
                {
                    var aprovacao = new Dominio.Entidades.Embarcador.Terceiros.AlcadasContratoFreteAcrescimoDesconto.AprovacaoAlcadaContratoFreteAcrescimoDesconto()
                    {
                        OrigemAprovacao = contrato,
                        Usuario = null,
                        RegraAutorizacao = regra,
                        Situacao = SituacaoAlcadaRegra.Aprovada,
                        Data = DateTime.Now,
                        Motivo = $"Alçada aprovada pela Regra {regra.Descricao}",
                        DataCriacao = contrato.Data
                    };

                    repositorio.Inserir(aprovacao);
                }
            }

            if (existeRegraSemAprovacao)
                contrato.Situacao = SituacaoContratoFreteAcrescimoDesconto.AgAprovacao;
            else
                contrato.Situacao = SituacaoContratoFreteAcrescimoDesconto.Aprovado;
        }

        private List<Dominio.Entidades.Embarcador.Terceiros.AlcadasContratoFreteAcrescimoDesconto.RegraAutorizacaoContratoFreteAcrescimoDesconto> ObterRegrasAutorizacao(Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto contrato)
        {
            Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacaoPadrao<Dominio.Entidades.Embarcador.Terceiros.AlcadasContratoFreteAcrescimoDesconto.RegraAutorizacaoContratoFreteAcrescimoDesconto> repositorioRegraAutorizacao = new Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacaoPadrao<Dominio.Entidades.Embarcador.Terceiros.AlcadasContratoFreteAcrescimoDesconto.RegraAutorizacaoContratoFreteAcrescimoDesconto>(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Terceiros.AlcadasContratoFreteAcrescimoDesconto.RegraAutorizacaoContratoFreteAcrescimoDesconto> listaRegras = repositorioRegraAutorizacao.BuscarPorAtiva();
            List<Dominio.Entidades.Embarcador.Terceiros.AlcadasContratoFreteAcrescimoDesconto.RegraAutorizacaoContratoFreteAcrescimoDesconto> listaRegrasFiltradas = new List<Dominio.Entidades.Embarcador.Terceiros.AlcadasContratoFreteAcrescimoDesconto.RegraAutorizacaoContratoFreteAcrescimoDesconto>();

            foreach (Dominio.Entidades.Embarcador.Terceiros.AlcadasContratoFreteAcrescimoDesconto.RegraAutorizacaoContratoFreteAcrescimoDesconto regra in listaRegras)
            {
                if (regra.RegraPorJustificativa && !ValidarAlcadas<Dominio.Entidades.Embarcador.Terceiros.AlcadasContratoFreteAcrescimoDesconto.AlcadaJustificativa, Dominio.Entidades.Embarcador.Fatura.Justificativa>(regra.AlcadasJustificativa, contrato.Justificativa.Codigo))
                    continue;

                if (regra.RegraPorValor && !ValidarAlcadas<Dominio.Entidades.Embarcador.Terceiros.AlcadasContratoFreteAcrescimoDesconto.AlcadaValor, decimal>(regra.AlcadasValor, contrato.Valor))
                    continue;

                listaRegrasFiltradas.Add(regra);
            }

            return listaRegrasFiltradas;
        }

        private bool AplicarValorNoContratoFrete(out string mensagemErro, Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto contratoAcrescimoDesconto, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            mensagemErro = null;

            if (contratoAcrescimoDesconto.ValorAplicado)
                return true;

            Repositorio.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto repContratoFreteAcrescimoDesconto = new Repositorio.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto(_unitOfWork);

            try
            {
                AplicarValorNoContratoFrete(contratoAcrescimoDesconto, tipoServicoMultisoftware, null);

                return true;
            }
            catch (ServicoException ex)
            {
                contratoAcrescimoDesconto = repContratoFreteAcrescimoDesconto.BuscarPorCodigo(contratoAcrescimoDesconto.Codigo);

                contratoAcrescimoDesconto.MotivoRejeicao = ex.Message;
                contratoAcrescimoDesconto.Situacao = SituacaoContratoFreteAcrescimoDesconto.AplicacaoValorRejeitado;

                repContratoFreteAcrescimoDesconto.Atualizar(contratoAcrescimoDesconto);

                mensagemErro = ex.Message;
                return false;
            }
        }

        private dynamic ObterAnexos(Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto contrato)
        {
            if (contrato?.Anexos == null)
                return null;

            return (
                from anexo in contrato.Anexos
                select new
                {
                    anexo.Codigo,
                    anexo.Descricao,
                    anexo.NomeArquivo,
                }
            ).ToList();
        }

        private void GerarIntegracaoSAP_AV(Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDescontoIntegracao integracaoContrato, Repositorio.Embarcador.Terceiros.ContratoFreteAcrescimoDescontoIntegracao repContratoFreteAcrescimoDescontoIntegracao, Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao)
        {
            Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao = repTipoIntegracao.BuscarPorTipo(TipoIntegracao.SAP_AV);

            Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDescontoIntegracao contratoIntegracao = new Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDescontoIntegracao
            {
                TipoIntegracao = tipoIntegracao,
                DataIntegracao = DateTime.Now,
                ProblemaIntegracao = "",
                SituacaoIntegracao = SituacaoIntegracao.AgIntegracao,
                ContratoFreteAcrescimoDesconto = integracaoContrato.ContratoFreteAcrescimoDesconto,
                CIOT = integracaoContrato.CIOT
            };

            repContratoFreteAcrescimoDescontoIntegracao.Inserir(contratoIntegracao);
        }

        private void GerarIntegracaoKMM(Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDescontoIntegracao integracaoContrato, Repositorio.Embarcador.Terceiros.ContratoFreteAcrescimoDescontoIntegracao repContratoFreteAcrescimoDescontoIntegracao, Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao)
        {
            Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao = repTipoIntegracao.BuscarPorTipo(TipoIntegracao.KMM);

            Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDescontoIntegracao contratoIntegracao = new Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDescontoIntegracao
            {
                TipoIntegracao = tipoIntegracao,
                DataIntegracao = DateTime.Now,
                ProblemaIntegracao = "",
                SituacaoIntegracao = SituacaoIntegracao.AgIntegracao,
                ContratoFreteAcrescimoDesconto = integracaoContrato.ContratoFreteAcrescimoDesconto,
                CIOT = integracaoContrato.CIOT
            };

            repContratoFreteAcrescimoDescontoIntegracao.Inserir(contratoIntegracao);
        }

        #endregion
    }
}
