using AdminMultisoftware.Dominio.Enumeradores;
using Dominio.Entidades.Embarcador.Escrituracao.AlcadasProvisaoPendente;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dominio.ObjetosDeValor.Relatorios;
using Servicos.Extensions;

namespace Servicos.Embarcador.Financeiro
{
    public class TermoQuitacaoFinanceiro : RegraAutorizacao.AprovacaoAlcada
    <
        Dominio.Entidades.Embarcador.Escrituracao.CancelamentoProvisao,
        Dominio.Entidades.Embarcador.Escrituracao.AlcadasProvisaoPendente.RegraAutorizacaoProvisaoPendente,
        Dominio.Entidades.Embarcador.Escrituracao.AlcadasProvisaoPendente.AprovacaoAlcadaEstornoProvisao
    >
    {
        #region Construtores
        public TermoQuitacaoFinanceiro(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }
        #endregion

        #region Metodos Publicos
        public void VerificarTermosAGerar()
        {
            Repositorio.Empresa repositorioTransportador = new Repositorio.Empresa(_unitOfWork);
            List<Dominio.Entidades.Empresa> transportadores = repositorioTransportador.BuscarTransportadoresParaGerarTermo();

            foreach (var transportador in transportadores)
            {
                try
                {
                    _unitOfWork.Start();

                    bool retorno = ProcessarCriacaoNovoTermo(transportador, out int codigoTermoGerado);

                    if (!retorno)
                        continue;

                    _unitOfWork.CommitChanges();
                    _unitOfWork.FlushAndClear();
                    continue;



                }
                catch (ServicoException exe)
                {
                    _unitOfWork.Rollback();
                }
                catch (Exception exe)
                {
                    Servicos.Log.TratarErro(exe);
                    _unitOfWork.Rollback();
                }
            }
        }

        public bool ProcessarCriacaoNovoTermo(Dominio.Entidades.Empresa transportador, out int codigoTermoGerado, DateTime? dataInicialParam = null, DateTime? dataFinalParam = null)
        {
            codigoTermoGerado = 0;

            Servicos.Embarcador.Financeiro.TermoQuitacaoControleDocumento svcTermoControleDocumento = new Servicos.Embarcador.Financeiro.TermoQuitacaoControleDocumento(_unitOfWork);
            Repositorio.Embarcador.Financeiro.MovimentacaoContaPagar repositorioMovimentacaoContaPagar = new Repositorio.Embarcador.Financeiro.MovimentacaoContaPagar(_unitOfWork);
            Repositorio.Embarcador.Financeiro.TermoQuitacao repositorioTermoQuitacaoFinancerio = new Repositorio.Embarcador.Financeiro.TermoQuitacao(_unitOfWork);
            Dominio.Entidades.Embarcador.Financeiro.TermoQuitacaoFinanceiro ultimoTermoGerado = repositorioTermoQuitacaoFinancerio.BuscarUltimoTermoGeradoPorTransportador(transportador.Codigo);

            DateTime? dataInicial = ultimoTermoGerado != null ? ultimoTermoGerado.DataFinal : transportador.DataFimTermoQuitacaoInicial;

            if (dataInicial == null && dataInicialParam.HasValue)
                dataInicial = dataInicialParam.Value;

            if (!dataInicial.HasValue)
                throw new ServicoException("Data Fim do Termo Quitação Não Configurada");

            int aCadaTempo = transportador?.ACadaAvisoPeriodico ?? 0;
            int totalDiasGeracaoNovoTermo = transportador?.PeriodoAvisoPeriodico.RetornarQuantidadeDias(aCadaTempo) ?? 0;

            if (!dataInicialParam.HasValue || !dataFinalParam.HasValue)
                if (DateTime.Now < CalcularProximaValidacao(dataInicial.Value, aCadaTempo, transportador.PeriodoAvisoPeriodico, transportador?.TempoAguardarParaGerarTermo ?? 0))
                    throw new ServicoException("Fora do prazo de geração de termo de quitação");

            DateTime? dataInicialProxima = dataInicialParam.HasValue
                                                ? dataInicialParam
                                                : dataInicial.Value.AddDays(1);

            DateTime? dataFinal = dataFinalParam.HasValue
                                                ? dataFinalParam
                                                : dataInicialProxima.Value.AddDays(totalDiasGeracaoNovoTermo);

            if (repositorioMovimentacaoContaPagar.ExisteRegistroPendentesEmAbertoParaTransportador(transportador.Codigo, dataInicialProxima, dataFinal))
                throw new ServicoException("Existe Registro Pendentes em aberto para o transportador");


            ultimoTermoGerado = GerarTermo(transportador, dataInicialProxima, dataFinal);

            ProcessarMovimentacaoFinanceiraTermo(ultimoTermoGerado);
            ValidarProvisaoPendente(ultimoTermoGerado);
            ValidarAprovacaoTransportador(ultimoTermoGerado);
            svcTermoControleDocumento.AtualizarPDF(ultimoTermoGerado);

            codigoTermoGerado = ultimoTermoGerado.Codigo;

            if (ultimoTermoGerado.NumeroTermo == 0)
                ultimoTermoGerado.NumeroTermo = repositorioTermoQuitacaoFinancerio.ObterUltimoNumero() + 1;

            repositorioTermoQuitacaoFinancerio.Atualizar(ultimoTermoGerado);
            return true;
        }

        public void FinalizarTermoQuitacaoFinanceiroTransportador(Dominio.Entidades.Embarcador.Financeiro.AprovacaoTermoQuitacaoFinanceiroTransportador aprovacaoTermoQuitacao)
        {
            Repositorio.Embarcador.Financeiro.TermoQuitacao repositorioTermoquitacao = new Repositorio.Embarcador.Financeiro.TermoQuitacao(_unitOfWork);
            Repositorio.Embarcador.Financeiro.AprovacaoTermoQuitacaoFinanceiroTransportador repositorioAprovacao = new Repositorio.Embarcador.Financeiro.AprovacaoTermoQuitacaoFinanceiroTransportador(_unitOfWork);

            Dominio.Entidades.Embarcador.Financeiro.TermoQuitacaoFinanceiro termoQuitacao = aprovacaoTermoQuitacao.TermoQuitacaoFinanceiro;

            aprovacaoTermoQuitacao.Situacao = SituacaoAprovacaoTermoQuitacaoTransportador.Aprovado;
            termoQuitacao.SituacaoTermoQuitacao = SituacaoTermoQuitacaoFinanceiro.Finalizada;

            repositorioTermoquitacao.Atualizar(termoQuitacao);
            repositorioAprovacao.Atualizar(aprovacaoTermoQuitacao);
        }

        public Dominio.Entidades.Embarcador.Financeiro.TermoQuitacaoFinanceiro GerarTermo(Dominio.Entidades.Empresa transpotador, DateTime? dataInicio, DateTime? dataFim)
        {
            Repositorio.Embarcador.Financeiro.TermoQuitacao repositorioTermoQuitaca = new Repositorio.Embarcador.Financeiro.TermoQuitacao(_unitOfWork);

            Dominio.Entidades.Embarcador.Financeiro.TermoQuitacaoFinanceiro novoTermo = new Dominio.Entidades.Embarcador.Financeiro.TermoQuitacaoFinanceiro()
            {
                DataCriacao = DateTime.Now,
                DataInicial = dataInicio,
                DataFinal = dataFim,
                SituacaoTermoQuitacao = SituacaoTermoQuitacaoFinanceiro.AguardandoAprovacaoTransportador,
                Transportador = transpotador
            };

            repositorioTermoQuitaca.Inserir(novoTermo);
            return novoTermo;
        }
        public void RejeitarTermoQuitacaoTransportador(Dominio.Entidades.Embarcador.Financeiro.AprovacaoTermoQuitacaoFinanceiroTransportador aprovacaoTermoQuitacao)
        {
            Repositorio.Embarcador.Financeiro.TermoQuitacao repositorioTermoquitacao = new Repositorio.Embarcador.Financeiro.TermoQuitacao(_unitOfWork);
            Repositorio.Embarcador.Financeiro.AprovacaoTermoQuitacaoFinanceiroTransportador repositorioAprovacao = new Repositorio.Embarcador.Financeiro.AprovacaoTermoQuitacaoFinanceiroTransportador(_unitOfWork);

            Dominio.Entidades.Embarcador.Financeiro.TermoQuitacaoFinanceiro termoQuitacao = aprovacaoTermoQuitacao.TermoQuitacaoFinanceiro;

            aprovacaoTermoQuitacao.Situacao = SituacaoAprovacaoTermoQuitacaoTransportador.Reprovado;
            termoQuitacao.SituacaoTermoQuitacao = SituacaoTermoQuitacaoFinanceiro.RejeitadoTransportador;

            repositorioTermoquitacao.Atualizar(termoQuitacao);
            repositorioAprovacao.Atualizar(aprovacaoTermoQuitacao);
        }

        public void AprovarTermoQuitacaoProvisaoPendente(Dominio.Entidades.Embarcador.Escrituracao.AlcadasProvisaoPendente.AprovacaoAlcadaTermoQuitacaoFinanceiro aprovacaoAlcada)
        {
            Repositorio.Embarcador.Financeiro.TermoQuitacao repositorioTermoquitacao = new Repositorio.Embarcador.Financeiro.TermoQuitacao(_unitOfWork);
            Repositorio.Embarcador.Escrituracao.AlcadaProvisaoPendente.AprovacaoAlcadoTermoQuitacaoFinanceiro repositorioAprovacao = new Repositorio.Embarcador.Escrituracao.AlcadaProvisaoPendente.AprovacaoAlcadoTermoQuitacaoFinanceiro(_unitOfWork);

            Dominio.Entidades.Embarcador.Financeiro.TermoQuitacaoFinanceiro termoQuitacao = aprovacaoAlcada.OrigemAprovacao;

            aprovacaoAlcada.Situacao = SituacaoAlcadaRegra.Aprovada;
            termoQuitacao.SituacaoTermoQuitacao = SituacaoTermoQuitacaoFinanceiro.AguardandoAprovacaoTransportador;

            repositorioTermoquitacao.Atualizar(termoQuitacao);
            repositorioAprovacao.Atualizar(aprovacaoAlcada);
        }

        public void RejeitarTermoQuitacaoProvisaoPendente(Dominio.Entidades.Embarcador.Escrituracao.AlcadasProvisaoPendente.AprovacaoAlcadaTermoQuitacaoFinanceiro aprovacaoAlcada)
        {
            Repositorio.Embarcador.Financeiro.TermoQuitacao repositorioTermoquitacao = new Repositorio.Embarcador.Financeiro.TermoQuitacao(_unitOfWork);
            Repositorio.Embarcador.Escrituracao.AlcadaProvisaoPendente.AprovacaoAlcadoTermoQuitacaoFinanceiro repositorioAprovacao = new Repositorio.Embarcador.Escrituracao.AlcadaProvisaoPendente.AprovacaoAlcadoTermoQuitacaoFinanceiro(_unitOfWork);

            Dominio.Entidades.Embarcador.Financeiro.TermoQuitacaoFinanceiro termoQuitacao = aprovacaoAlcada.OrigemAprovacao;

            aprovacaoAlcada.Situacao = SituacaoAlcadaRegra.Rejeitada;
            termoQuitacao.SituacaoTermoQuitacao = SituacaoTermoQuitacaoFinanceiro.RejeitadoProvisao;

            repositorioTermoquitacao.Atualizar(termoQuitacao);
            repositorioAprovacao.Atualizar(aprovacaoAlcada);
        }

        public object ObterDetalhesAprovacao(Dominio.Entidades.Embarcador.Financeiro.TermoQuitacaoFinanceiro termo)
        {
            Repositorio.Embarcador.Escrituracao.AlcadaProvisaoPendente.AprovacaoAlcadaEstornoProvisao repositorioAprovacaoAlcadaTermoQuitacao = new Repositorio.Embarcador.Escrituracao.AlcadaProvisaoPendente.AprovacaoAlcadaEstornoProvisao(_unitOfWork);
            int aprovacoes = repositorioAprovacaoAlcadaTermoQuitacao.ContarAprovacoes(termo.Codigo, SituacaoAlcadaRegra.Aprovada);
            int aprovacoesNecessarias = repositorioAprovacaoAlcadaTermoQuitacao.ContarAprovacoes(termo.Codigo, SituacaoAlcadaRegra.Pendente);
            int reprovacoes = repositorioAprovacaoAlcadaTermoQuitacao.ContarAprovacoes(termo.Codigo, SituacaoAlcadaRegra.Rejeitada);

            return new
            {
                AprovacoesNecessarias = aprovacoesNecessarias,
                Aprovacoes = aprovacoes,
                Reprovacoes = reprovacoes,
                Situacao = termo.SituacaoTermoQuitacao.ObterDescricao(),
                termo.SituacaoTermoQuitacao,
                DataSolicitacao = termo.DataCriacao.HasValue ? termo.DataCriacao.Value.ToString("dd/MM/yyyy") : "",
                Solicitante = "Automático",
                termo.Codigo
            };
        }

        public void ValidarProvisaoPendente(Dominio.Entidades.Embarcador.Financeiro.TermoQuitacaoFinanceiro termoGerado)
        {
            Repositorio.Embarcador.Escrituracao.DocumentoProvisao repositorioProvisao = new Repositorio.Embarcador.Escrituracao.DocumentoProvisao(_unitOfWork);
            Repositorio.Embarcador.Financeiro.TermoQuitacao repositorioTermoquitacao = new Repositorio.Embarcador.Financeiro.TermoQuitacao(_unitOfWork);

            bool existeProvisaoEmAbertoSemMiro = repositorioProvisao.BuscarDocumenttosProvisaoGeradosNumIntervalo(termoGerado.DataInicial, termoGerado.DataFinal);

            if (!existeProvisaoEmAbertoSemMiro)
                return;

            var regrasProvisao = ObterRegrasAutorizacao(termoGerado);

            if (regrasProvisao.Count > 0)
                CriarRegrasAprovacao(termoGerado, regrasProvisao, TipoServicoMultisoftware.MultiEmbarcador);

            repositorioTermoquitacao.Atualizar(termoGerado);
        }

        public void ProcessarMovimentacaoFinanceiraTermo(Dominio.Entidades.Embarcador.Financeiro.TermoQuitacaoFinanceiro TermoQuitacao, List<Dominio.Entidades.Embarcador.Financeiro.MovimentacaoContaPagar> movimentacoesProcessar = null)
        {
            Repositorio.Embarcador.Financeiro.MovimentacaoContaPagar repositorioMovimentacaoContaPagar = new Repositorio.Embarcador.Financeiro.MovimentacaoContaPagar(_unitOfWork);
            Repositorio.Embarcador.Financeiro.TermoQuitacao repositorioTermoQuitacao = new Repositorio.Embarcador.Financeiro.TermoQuitacao(_unitOfWork);
            Repositorio.Embarcador.Financeiro.ContaPagar repositorioContaPagar = new Repositorio.Embarcador.Financeiro.ContaPagar(_unitOfWork);
            Repositorio.Empresa repositorioTransportador = new Repositorio.Empresa(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Financeiro.MovimentacaoContaPagar> listarMovimentacao = new List<Dominio.Entidades.Embarcador.Financeiro.MovimentacaoContaPagar>();

            if (movimentacoesProcessar != null)
                listarMovimentacao = movimentacoesProcessar;
            else
            {
                List<int> codigoTransportadores = new List<int>()
                {
                    TermoQuitacao.Transportador.Codigo
                };
                codigoTransportadores.AddRange(repositorioTransportador.BuscarCodigosFiliaisTransportador(TermoQuitacao.Transportador.Codigo));
                listarMovimentacao = repositorioMovimentacaoContaPagar.BuscarMovimentacaoFinanceiraContaPagar(TermoQuitacao.DataInicial, TermoQuitacao.DataFinal, codigoTransportadores);
            }

            if (listarMovimentacao == null || listarMovimentacao.Count == 0)
                throw new ServicoException("Não foi possivel processar a movimentação financeira do termo de quitação");

            List<TipoRegistro> tiposRegistroPermitidosGeracao = new List<TipoRegistro>() {
            TipoRegistro.Pagoviacreditoemconta,
            TipoRegistro.PagoviaConfirming,
            TipoRegistro.NotasCompensadasXAdiantamento,
            TipoRegistro.Descontos,
            TipoRegistro.TotaldeAdiantamento,
            TipoRegistro.Debitoscompensados
            };

            if (!listarMovimentacao.Any(x => tiposRegistroPermitidosGeracao.Contains(x.TipoRegistro) && !string.IsNullOrEmpty(x.ClrngDoc)))
                throw new ServicoException("Existem documentos não quitados para o período informado, termo não pode ser gerado");

            DateTime? menorData = listarMovimentacao.Select(x => x.DataCompensamento).Min();

            if (!(menorData.HasValue && menorData.Value.Month < DateTime.Now.Month))
                throw new ServicoException("Registros com data de pagamento no mês atual. Termo não pode ser gerado");

            if (listarMovimentacao.Any(x => x.TipoRegistro == TipoRegistro.Cockpit))
                throw new ServicoException("Possui registro do tipo Cockpit ");

            if (listarMovimentacao.Any(x => x.TipoRegistro == TipoRegistro.BaixaResultado && string.IsNullOrEmpty(x.ClrngDoc)))
                throw new ServicoException("Existem documentos não quitados para o período informado, termo não pode ser gerado");

            foreach (var movimentacao in listarMovimentacao)
            {
                movimentacao.TermoQuitacaoFinanceiro = TermoQuitacao;
                if (movimentacao.ContaPagar != null)
                {
                    movimentacao.ContaPagar.TermoQuitacaoFinanceiro = TermoQuitacao;
                    repositorioContaPagar.Atualizar(movimentacao.ContaPagar);
                }
                repositorioMovimentacaoContaPagar.Atualizar(movimentacao);
            }

            decimal valorViaCreditoEmConta = listarMovimentacao.Where(x => x.TipoRegistro == TipoRegistro.Pagoviacreditoemconta).Select(x => x.ValorMonetario).Sum();
            decimal valorViaConfirming = listarMovimentacao.Where(x => x.TipoRegistro == TipoRegistro.PagoviaConfirming).Select(x => x.ValorMonetario).Sum();
            decimal valorTotalAdiantamento = listarMovimentacao.Where(x => x.TipoRegistro == TipoRegistro.TotaldeAdiantamento).Select(x => x.ValorMonetario).Sum();
            decimal valorCompensado = listarMovimentacao.Where(x => x.TipoRegistro == TipoRegistro.NotasCompensadasXAdiantamento).Select(x => x.ValorMonetario).Sum();
            List<int> codigoMovimentacao = listarMovimentacao.Select(x => x.Codigo).ToList();

            decimal valorCreditoEmConta = (valorViaCreditoEmConta + valorViaConfirming);


            if (valorViaCreditoEmConta == 0 && valorViaConfirming == 0 && valorTotalAdiantamento == 0 && valorCompensado == 0 && valorCreditoEmConta == 0 && (valorCreditoEmConta + valorTotalAdiantamento) == 0)
                throw new ServicoException("Todos os valores das movimentações estão zerados não sendo possivel gerar o termo.");

            TermoQuitacao.PagamentosEDescontosViaConfiming = valorViaConfirming;
            TermoQuitacao.PagamentosEDescontosViaCreditoEmConta = valorViaCreditoEmConta;
            TermoQuitacao.CreditoEmConta = valorCreditoEmConta;
            TermoQuitacao.TotalAdiantamento = valorTotalAdiantamento;
            TermoQuitacao.NotasCompensadasAdiantamentos = valorCompensado;
            TermoQuitacao.TotalSaldoEmAberto = (valorTotalAdiantamento - valorCompensado);
            TermoQuitacao.TotalGeralPagamento = (valorCreditoEmConta + valorTotalAdiantamento);
            repositorioTermoQuitacao.Atualizar(TermoQuitacao);

        }

        public void EnviarEmailsPendenciaAprovacaoTransportadores(AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteAdmin, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin)
        {
            Repositorio.Embarcador.Financeiro.AprovacaoTermoQuitacaoFinanceiroTransportador repAprovacaoTermo = new Repositorio.Embarcador.Financeiro.AprovacaoTermoQuitacaoFinanceiroTransportador(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Financeiro.AprovacaoTermoQuitacaoFinanceiroTransportador> aprovacoesPendetes = repAprovacaoTermo.BuscarPendentesNaoNotificadasAposIntervaloDias(15, 30, 60);
            List<Dominio.ObjetosDeValor.Email.Mensagem> msgs = new List<Dominio.ObjetosDeValor.Email.Mensagem>();
            AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso clienteURLAcesso = new AdminMultisoftware.Repositorio.Pessoas.ClienteURLAcesso(unitOfWorkAdmin).BuscarPorClienteETipoProducao(clienteAdmin.Codigo, TipoServicoMultisoftware.MultiCTe);
            try
            {
                foreach (var aprovacaoTermo in aprovacoesPendetes)
                {
                    var corpo = new StringBuilder();
                    corpo.Append("<h4>Olá Transportador!</h4>");
                    corpo.Append($"<p>Informamos que foi gerado um Termo de Quitação para o período [{aprovacaoTermo?.TermoQuitacaoFinanceiro?.DataInicial?.ToDateString() ?? string.Empty} até {aprovacaoTermo?.TermoQuitacaoFinanceiro?.DataFinal?.ToDateString() ?? string.Empty}] em relação a seus valores recebidos.<p>");
                    corpo.Append($"<p>O Termo de Quitação com todos os detalhes está disponível no Portal: <a href='https://{clienteURLAcesso.URLAcesso}'>Unilever - MultiTransportador</a>.</p>");
                    corpo.Append("<p>Por favor verifique as informações e registre a assinatura do mesmo.</p>");

                    var msg = new Dominio.ObjetosDeValor.Email.Mensagem
                    {
                        Assunto = "Termo de Quitação Disponível",
                        Corpo = corpo.ToString(),
                        Destinatarios = new List<string>() { aprovacaoTermo?.TermoQuitacaoFinanceiro?.Transportador?.Email }
                    };

                    msgs.Add(msg);

                    aprovacaoTermo.DataUltimaNotificacaoEmail = DateTime.Now;
                    repAprovacaoTermo.Atualizar(aprovacaoTermo);
                }

                Servicos.Email.EnviarMensagensAsync(msgs, _unitOfWork);
            }
            catch (ServicoException exe)
            {
                Servicos.Log.TratarErro(exe);
                _unitOfWork.Rollback();
            }
            catch (Exception exe)
            {
                Servicos.Log.TratarErro(exe);
                _unitOfWork.Rollback();
            }
        }

        public byte[] ExportarExcelResumoTermo(Dominio.Entidades.Embarcador.Financeiro.TermoQuitacaoFinanceiro termo)
        {
            return ReportRequest.WithType(ReportType.ResumoTermoQuitacao)
                .WithExecutionType(ExecutionType.Sync)
                .AddExtraData("CodigoTermo", termo.Codigo.ToString())
                .CallReport()
                .GetContentFile();
        }

        #endregion Metodos Publicos

        #region Metodos Privados

        private DateTime CalcularProximaValidacao(DateTime dataFimUltimoTermo, int aCada, DiaSemanaMesAno periodo, int tempoAguardar)
        {
            DateTime proximaValidacao = dataFimUltimoTermo;

            if (periodo == DiaSemanaMesAno.Mes)
                proximaValidacao = proximaValidacao.AddMonths(aCada);
            else if (periodo == DiaSemanaMesAno.Ano)
                proximaValidacao = proximaValidacao.AddYears(aCada);
            else if (periodo == DiaSemanaMesAno.Semana)
                proximaValidacao = proximaValidacao.AddDays(aCada * 7);
            else
                proximaValidacao = proximaValidacao.AddDays(aCada);

            proximaValidacao = proximaValidacao.AddDays(tempoAguardar);

            return proximaValidacao;
        }

        public void ValidarAprovacaoTransportador(Dominio.Entidades.Embarcador.Financeiro.TermoQuitacaoFinanceiro termoGerado)
        {
            if (termoGerado.SituacaoTermoQuitacao != SituacaoTermoQuitacaoFinanceiro.AguardandoAprovacaoTransportador)
                return;

            Repositorio.Embarcador.Financeiro.AprovacaoTermoQuitacaoFinanceiroTransportador repositorioAprovacaoTermoqQuitacao = new Repositorio.Embarcador.Financeiro.AprovacaoTermoQuitacaoFinanceiroTransportador(_unitOfWork);

            Dominio.Entidades.Embarcador.Financeiro.AprovacaoTermoQuitacaoFinanceiroTransportador aprovacaoTermoQuitacao = new Dominio.Entidades.Embarcador.Financeiro.AprovacaoTermoQuitacaoFinanceiroTransportador()
            {
                DataCriacao = DateTime.Now,
                Situacao = SituacaoAprovacaoTermoQuitacaoTransportador.Pendente,
                TermoQuitacaoFinanceiro = termoGerado,
                Transportador = termoGerado.Transportador
            };

            repositorioAprovacaoTermoqQuitacao.Inserir(aprovacaoTermoQuitacao);
        }

        private void CriarRegrasAprovacao(Dominio.Entidades.Embarcador.Financeiro.TermoQuitacaoFinanceiro termoquitacao, List<Dominio.Entidades.Embarcador.Escrituracao.AlcadasProvisaoPendente.RegraAutorizacaoProvisaoPendente> regras, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            bool existeRegraSemAprovacao = false;
            Repositorio.Embarcador.Escrituracao.AlcadaProvisaoPendente.AprovacaoAlcadaEstornoProvisao repositorioAprovacao = new Repositorio.Embarcador.Escrituracao.AlcadaProvisaoPendente.AprovacaoAlcadaEstornoProvisao(_unitOfWork);
            int menorPrioridadeAprovacao = regras.Where(regra => regra.NumeroAprovadores > 0).Select(regra => (int?)regra.PrioridadeAprovacao).Min() ?? 0;

            foreach (Dominio.Entidades.Embarcador.Escrituracao.AlcadasProvisaoPendente.RegraAutorizacaoProvisaoPendente regra in regras)
            {
                if (regra.NumeroAprovadores > 0)
                {
                    existeRegraSemAprovacao = true;

                    foreach (var aprovador in regra.Aprovadores)
                    {

                        var aprovacao = new AprovacaoAlcadaEstornoProvisao
                        {


                            TermoQuitacaoFinanceiro = termoquitacao,
                            Bloqueada = regra.PrioridadeAprovacao > menorPrioridadeAprovacao,
                            Usuario = aprovador,
                            RegraAutorizacao = regra,
                            Situacao = SituacaoAlcadaRegra.Pendente,
                            DataCriacao = termoquitacao.DataCriacao,
                            NumeroAprovadores = regra.NumeroAprovadores
                        };

                        if(!repositorioAprovacao.ExisteDuplicidade(aprovacao))
                        {
                            repositorioAprovacao.Inserir(aprovacao);

                            if (!aprovacao.Bloqueada)
                                NotificarAprovador(termoquitacao, aprovacao, tipoServicoMultisoftware);
                        }
    
                       
                    }
                }
                else
                {
                    var aprovacao = new AprovacaoAlcadaEstornoProvisao()
                    {
                        TermoQuitacaoFinanceiro = termoquitacao,
                        Usuario = null,
                        RegraAutorizacao = regra,
                        Situacao = SituacaoAlcadaRegra.Aprovada,
                        Data = DateTime.Now,
                        Motivo = $"Alçada aprovada pela Regra {regra.Descricao}",
                        DataCriacao = termoquitacao.DataCriacao
                    };

                    repositorioAprovacao.Inserir(aprovacao);
                }
            }

            termoquitacao.SituacaoTermoQuitacao = existeRegraSemAprovacao ? SituacaoTermoQuitacaoFinanceiro.AguardandoAprovacaoProvisao : SituacaoTermoQuitacaoFinanceiro.SemRegraProvisao;
        }

        private List<Dominio.Entidades.Embarcador.Escrituracao.AlcadasProvisaoPendente.RegraAutorizacaoProvisaoPendente> ObterRegrasAutorizacao(Dominio.Entidades.Embarcador.Financeiro.TermoQuitacaoFinanceiro termoGerado)
        {
            Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacaoPadrao<Dominio.Entidades.Embarcador.Escrituracao.AlcadasProvisaoPendente.RegraAutorizacaoProvisaoPendente> repositorioRegraAutorizacao = new Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacaoPadrao<Dominio.Entidades.Embarcador.Escrituracao.AlcadasProvisaoPendente.RegraAutorizacaoProvisaoPendente>(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Escrituracao.AlcadasProvisaoPendente.RegraAutorizacaoProvisaoPendente> regrasAtivas = repositorioRegraAutorizacao.BuscarPorAtiva();
            List<Dominio.Entidades.Embarcador.Escrituracao.AlcadasProvisaoPendente.RegraAutorizacaoProvisaoPendente> listaRegrasFiltradas = new List<Dominio.Entidades.Embarcador.Escrituracao.AlcadasProvisaoPendente.RegraAutorizacaoProvisaoPendente>();

            foreach (Dominio.Entidades.Embarcador.Escrituracao.AlcadasProvisaoPendente.RegraAutorizacaoProvisaoPendente regra in regrasAtivas)
            {

                if (regra.RegraPorFilial && !ValidarAlcadas<Dominio.Entidades.Embarcador.Escrituracao.AlcadasProvisaoPendente.AlcadaFilial, Dominio.Entidades.Embarcador.Filiais.Filial>(regra.AlcadasFilial, termoGerado?.Transportador?.FiliaisEmbarcadorHabilitado.Select(x => x.Codigo).ToList()))
                    continue;

                if (regra.RegraPorValorProvisao && !ValidarAlcadas<Dominio.Entidades.Embarcador.Escrituracao.AlcadasProvisaoPendente.AlcadaValorProvisao, decimal>(regra.AlcadaValorProvisao, termoGerado.TotalGeralPagamento))
                    continue;


                listaRegrasFiltradas.Add(regra);
            }
            return listaRegrasFiltradas;
        }


        private void NotificarAprovador(Dominio.Entidades.Embarcador.Financeiro.TermoQuitacaoFinanceiro origemAprovacao, AprovacaoAlcadaEstornoProvisao aprovacao, TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Notificacao.Notificacao servicoNotificacao = new Notificacao.Notificacao(_unitOfWork.StringConexao, cliente: null, tipoServicoMultisoftware: tipoServicoMultisoftware, adminStringConexao: string.Empty);

            servicoNotificacao.GerarNotificacaoEmail(
                usuario: aprovacao.Usuario,
                usuarioGerouNotificacao: null,
                codigoObjeto: origemAprovacao.Codigo,
                URLPagina: "Financeiro/TermoQuitacao",
                titulo: "Escrituração Termo Quitação",
                nota: $"Criada solicitação aprovação termo quitação {origemAprovacao.Codigo}",
                icone: IconesNotificacao.cifra,
                tipoNotificacao: TipoNotificacao.credito,
                tipoServicoMultisoftwareNotificar: tipoServicoMultisoftware,
                unitOfWork: _unitOfWork
            );
        }
        #endregion

        #region Metodos Protegidos
        protected override void NotificarAprovador(Dominio.Entidades.Embarcador.Escrituracao.CancelamentoProvisao origemAprovacao, AprovacaoAlcadaEstornoProvisao aprovacao, TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Notificacao.Notificacao servicoNotificacao = new Notificacao.Notificacao(_unitOfWork.StringConexao, cliente: null, tipoServicoMultisoftware: tipoServicoMultisoftware, adminStringConexao: string.Empty);

            servicoNotificacao.GerarNotificacaoEmail(
                usuario: aprovacao.Usuario,
                usuarioGerouNotificacao: null,
                codigoObjeto: origemAprovacao.Codigo,
                URLPagina: "Financeiro/TermoQuitacao",
                titulo: "Escrituração Termo Quitação",
                nota: $"Criada solicitação aprovação termo quitação {origemAprovacao.Codigo}",
                icone: IconesNotificacao.cifra,
                tipoNotificacao: TipoNotificacao.credito,
                tipoServicoMultisoftwareNotificar: tipoServicoMultisoftware,
                unitOfWork: _unitOfWork
            );
        }
        #endregion

    }
}
