using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;
using Dominio.ObjetosDeValor.Relatorios;
using Servicos.Extensions;


namespace Servicos.Embarcador.Financeiro
{
    public class AvisoPeriodico
    {
        protected readonly Repositorio.UnitOfWork _unitOfWork;

        #region Construtores

        public AvisoPeriodico(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Metodos Publicos

        public void VerificarAvisosPeriodicosAGerar(AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteAdmin, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin)
        {
            Repositorio.Empresa repositorioTransportador = new Repositorio.Empresa(_unitOfWork);
            List<int> transportadores = repositorioTransportador.BuscarCodigosTransportadoresParaAvisoPeriodico();

            foreach (var transportador in transportadores)
            {
                try
                {
                    if (!repositorioTransportador.TransportadorFilial(transportador))
                    {

                        Dominio.Entidades.Embarcador.Financeiro.AvisoPeriodicoQuitacao avisoGerado = ProcessarCriacaoNovoAvisoPeriodico(transportador, clienteAdmin, unitOfWorkAdmin);

                        if (avisoGerado == null)
                            continue;

                        if (avisoGerado != null)
                            EnviarEmailTransportadorInformandoGeracaoAviso(avisoGerado, clienteAdmin, unitOfWorkAdmin);
                    }
                }
                catch (ServicoException exe)
                {
                    Servicos.Log.TratarErro(exe, "VerificarAvisosPeriodicosAGerar");
                    _unitOfWork.Rollback();
                }
                catch (Exception exe)
                {
                    Servicos.Log.TratarErro(exe, "VerificarAvisosPeriodicosAGerar");
                    _unitOfWork.Rollback();
                }
            }
        }


        public byte[] ExportarExcelResumoAvisoPeriodico(Dominio.Entidades.Embarcador.Financeiro.AvisoPeriodicoQuitacao avisoPeriodico)
        {
            return ReportRequest.WithType(ReportType.ResumoAvisoPeriodico)
                .WithExecutionType(ExecutionType.Sync)
                .AddExtraData("CodigoAvisoPeriodico", avisoPeriodico.Codigo.ToString())
                .CallReport()
                .GetContentFile();
        }



        #endregion

        #region Metodos Privados
        private Dominio.Entidades.Embarcador.Financeiro.AvisoPeriodicoQuitacao ProcessarCriacaoNovoAvisoPeriodico(int Codtransportador, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteAdmin, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin)
        {

            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(_unitOfWork);
            Repositorio.Embarcador.Financeiro.MovimentacaoContaPagar repositorioMovimentacaoContaPagar = new Repositorio.Embarcador.Financeiro.MovimentacaoContaPagar(_unitOfWork);
            Repositorio.Embarcador.Financeiro.AvisoPeriodicoQuitacao repositorioAvisoPeriodicoQuitacao = new Repositorio.Embarcador.Financeiro.AvisoPeriodicoQuitacao(_unitOfWork);
            Dominio.Entidades.Embarcador.Financeiro.AvisoPeriodicoQuitacao ultimoAvisoGerado = null;

            Dominio.Entidades.Empresa transportador = repEmpresa.BuscarPorCodigoFetch(Codtransportador);

            ultimoAvisoGerado = repositorioAvisoPeriodicoQuitacao.BuscarUltimoAvisoGeradoPorTransportador(transportador.Codigo);

            DateTime? dataInicial = ultimoAvisoGerado != null ? ultimoAvisoGerado.DataFinal : transportador.DataFimTermoQuitacaoInicial;

            if (!dataInicial.HasValue)
                return null;

            int aCadaTempo = transportador?.ACadaAvisoPeriodico ?? 0;
            int totalDiasGeracaoNovoAviso = transportador?.PeriodoAvisoPeriodico.RetornarQuantidadeDias(aCadaTempo) ?? 0;

            if (DateTime.Now < CalcularProximaGeracaoAviso(dataInicial.Value, aCadaTempo, transportador.PeriodoAvisoPeriodico))
                return null;

            DateTime? dataInicialProxima = dataInicial.Value.AddDays(1).Date;

            DateTime? dataFinal = dataInicialProxima.Value.AddDays(totalDiasGeracaoNovoAviso);
            _unitOfWork.Start();
            ultimoAvisoGerado = GerarAviso(transportador, dataInicialProxima, dataFinal);

            ProcessarMovimentacaoFinanceiraAvisoPeriodico(ultimoAvisoGerado, transportador, ultimoAvisoGerado.DataInicial, ultimoAvisoGerado.DataFinal);

            transportador.DataUltimoAvisoTermoQuitacaoGerado = DateTime.Now;
            ultimoAvisoGerado.Numero = repositorioAvisoPeriodicoQuitacao.BuscarUltimoNumeroGerado() + 1;
            repEmpresa.Atualizar(transportador);
            repositorioAvisoPeriodicoQuitacao.Atualizar(ultimoAvisoGerado);


            _unitOfWork.CommitChanges();

            _unitOfWork.FlushAndClear();
            return ultimoAvisoGerado;

        }

        private DateTime CalcularProximaGeracaoAviso(DateTime dataUltimo, int aCada, DiaSemanaMesAno periodo)
        {
            DateTime proximaGeracao = dataUltimo.Date;

            if (periodo == DiaSemanaMesAno.Mes)
                proximaGeracao = proximaGeracao.AddMonths(aCada);
            else if (periodo == DiaSemanaMesAno.Ano)
                proximaGeracao = proximaGeracao.AddYears(aCada);
            else if (periodo == DiaSemanaMesAno.Semana)
                proximaGeracao = proximaGeracao.AddDays(aCada * 7);
            else
                proximaGeracao = proximaGeracao.AddDays(aCada);

            return proximaGeracao;
        }

        private Dominio.Entidades.Embarcador.Financeiro.AvisoPeriodicoQuitacao GerarAviso(Dominio.Entidades.Empresa transpotador, DateTime? dataInicio, DateTime? dataFim)
        {
            Repositorio.Embarcador.Financeiro.AvisoPeriodicoQuitacao repositorioAviso = new Repositorio.Embarcador.Financeiro.AvisoPeriodicoQuitacao(_unitOfWork);

            Dominio.Entidades.Embarcador.Financeiro.AvisoPeriodicoQuitacao novoAviso = new Dominio.Entidades.Embarcador.Financeiro.AvisoPeriodicoQuitacao()
            {
                DataCriacao = DateTime.Now,
                DataInicial = dataInicio,
                DataFinal = dataFim,
                SituacaoAvisoPeriodico = SituacaoAvisoPeriodicoQuitacao.AguardandoConfirmacao,
                Transportador = transpotador
            };

            repositorioAviso.Inserir(novoAviso);
            return novoAviso;
        }

        private void ProcessarMovimentacaoFinanceiraAvisoPeriodico(Dominio.Entidades.Embarcador.Financeiro.AvisoPeriodicoQuitacao AvisoPeriodicoQuitacao, Dominio.Entidades.Empresa transportador, DateTime? dataInicio = null, DateTime? dataFinal = null)
        {
            Repositorio.Embarcador.Financeiro.MovimentacaoContaPagar repositorioMovimentacaoContaPagar = new Repositorio.Embarcador.Financeiro.MovimentacaoContaPagar(_unitOfWork);
            Repositorio.Embarcador.Financeiro.AvisoPeriodicoQuitacao repositorioAvisoPeriodicoQuitacao = new Repositorio.Embarcador.Financeiro.AvisoPeriodicoQuitacao(_unitOfWork);
            Repositorio.Embarcador.Financeiro.ContaPagar repositorioContaPagar = new Repositorio.Embarcador.Financeiro.ContaPagar(_unitOfWork);
            Repositorio.Embarcador.Documentos.ControleDocumento repositorioControleDocumentos = new Repositorio.Embarcador.Documentos.ControleDocumento(_unitOfWork);
            Repositorio.Embarcador.Financeiro.DocumentoFaturamento repositorioDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(_unitOfWork);
            Repositorio.Embarcador.Documentos.HistoricoIrregularidade repositorioHistorioIrregulidade = new Repositorio.Embarcador.Documentos.HistoricoIrregularidade(_unitOfWork);
            Repositorio.Empresa repositorioTransportador = new Repositorio.Empresa(_unitOfWork);

            List<int> codTransportadores = new List<int>();
            codTransportadores.Add(transportador.Codigo);
            var codigoFiliais = repositorioTransportador.BuscarCodigosFiliaisTransportador(transportador.Codigo);

            if (codigoFiliais.Count > 0)
                codTransportadores.AddRange(codigoFiliais);

            List<Dominio.Entidades.Embarcador.Financeiro.MovimentacaoContaPagar> listarMovimentacao = repositorioMovimentacaoContaPagar.BuscarMovimentacaoFinanceiraContaPagar(dataInicio.Value.Date, dataFinal.Value.Date, codTransportadores, true);

            decimal valorViaCreditoEmConta = listarMovimentacao.Where(x => x.TipoRegistro == TipoRegistro.Pagoviacreditoemconta).Select(x => x.ValorMonetario).Sum();
            decimal valorViaConfirming = listarMovimentacao.Where(x => x.TipoRegistro == TipoRegistro.PagoviaConfirming).Select(x => x.ValorMonetario).Sum();
            decimal valorTotalAdiantamento = listarMovimentacao.Where(x => x.TipoRegistro == TipoRegistro.TotaldeAdiantamento).Select(x => x.ValorMonetario).Sum();
            decimal valorCompensado = listarMovimentacao.Where(x => x.TipoRegistro == TipoRegistro.NotasCompensadasXAdiantamento).Select(x => x.ValorMonetario).Sum();
            decimal valorDebitosCompensados = listarMovimentacao.Where(x => x.TipoRegistro == TipoRegistro.Debitoscompensados).Select(x => x.ValorMonetario).Sum();
            decimal valorDebitosBaixa = listarMovimentacao.Where(x => x.TipoRegistro == TipoRegistro.BaixaResultado).Select(x => x.ValorMonetario).Sum();
            List<int> codigoMovimentacao = listarMovimentacao.Select(x => x.Codigo).ToList();

            decimal valorCreditoEmConta = (valorViaCreditoEmConta + valorViaConfirming);

            AvisoPeriodicoQuitacao.TotalPagamentoEDescontosViaConfirming = valorViaConfirming;
            AvisoPeriodicoQuitacao.TotalPagamentoEDescontosViaCreditoConta = valorViaCreditoEmConta;
            AvisoPeriodicoQuitacao.TotalPagamentoEDescontosEmConta = valorCreditoEmConta;
            AvisoPeriodicoQuitacao.TotalAdiantamento = valorTotalAdiantamento;
            AvisoPeriodicoQuitacao.TotalNotasCompensadasAdiantamento = valorCompensado;
            AvisoPeriodicoQuitacao.TotalSaldoAdiantamentoEmAberto = (valorTotalAdiantamento - valorCompensado);
            AvisoPeriodicoQuitacao.TotalDebitoBaixa = valorDebitosBaixa;
            AvisoPeriodicoQuitacao.TotalAvariasEmAberto = valorDebitosCompensados;
            AvisoPeriodicoQuitacao.TotalGeralPagamentos = (valorCreditoEmConta + valorTotalAdiantamento);

            listarMovimentacao = listarMovimentacao.Where(x => x.CTe != null).ToList();

            if (valorViaCreditoEmConta == 0 && valorViaConfirming == 0 && valorTotalAdiantamento == 0 && valorCompensado == 0 && valorDebitosCompensados == 0 && valorDebitosBaixa == 0)
                throw new ServicoException("Não é possivel gerar aviso peridodico porque não tem valores");

            List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctesVencidos = listarMovimentacao.Where(x => x.TipoRegistro == TipoRegistro.NotasCompensadasXAdiantamento && x.DataCompensamento.HasValue ? x.DataCompensamento.Value < DateTime.Now : x.DueData.HasValue && x.DueData.Value < DateTime.Now).Select(x => x.CTe).ToList();
            List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctesAvencer = listarMovimentacao.Where(x => x.TipoRegistro == TipoRegistro.NotasCompensadasXAdiantamento && x.DataCompensamento.HasValue ? x.DataCompensamento.Value >= DateTime.Now : x.DueData.HasValue && x.DueData.Value >= DateTime.Now).Select(x => x.CTe).ToList();
            List<int> codigoCteVencidos = ctesVencidos.Select(x => x.Codigo).ToList();
            List<int> codigoCteAvencer = ctesAvencer.Select(x => x.Codigo).ToList();

            List<Dominio.Entidades.Embarcador.Documentos.ControleDocumento> documentosVencidos = repositorioControleDocumentos.BuscarPorCodigosCTes(codigoCteVencidos);
            List<Dominio.Entidades.Embarcador.Documentos.ControleDocumento> documentosAVencer = repositorioControleDocumentos.BuscarPorCodigosCTes(codigoCteAvencer);

            //Registro BLoqueados R Irregularidade 

            List<(int codigoRegistro, int codigoCte)> codigoCTesBloqueadosVencidos = repositorioDocumentoFaturamento.BuscarPorCTeComSemBloqueio(codigoCteVencidos, "R");
            List<(int codigoRegistro, int codigoCte)> codigoCTesBloqueadosAVencer = repositorioDocumentoFaturamento.BuscarPorCTeComSemBloqueio(codigoCteAvencer, "R");
            AvisoPeriodicoQuitacao.TotalPendentesAVencerBloqueioIrregularidade = listarMovimentacao.Where(x => codigoCTesBloqueadosAVencer.Any(q => q.codigoCte == x.CTe.Codigo)).Select(x => x.ValorMonetario).Sum();
            AvisoPeriodicoQuitacao.TotalPendentesVencidaBloqueioIrregularidade = listarMovimentacao.Where(x => codigoCTesBloqueadosVencidos.Any(q => q.codigoCte == x.CTe.Codigo)).Select(x => x.ValorMonetario).Sum();

            //Registro BLoqueados P Irregularidade 
            List<(int codigoRegistro, int codigoCte)> codigoCTesBloqueadosPVencidos = repositorioDocumentoFaturamento.BuscarPorCTeComSemBloqueio(codigoCteVencidos, "P");
            List<(int codigoRegistro, int codigoCte)> codigoCTesBloqueadosPAVencer = repositorioDocumentoFaturamento.BuscarPorCTeComSemBloqueio(codigoCteAvencer, "P");
            AvisoPeriodicoQuitacao.TotalPendenciasAVencerBloqueioPOD = listarMovimentacao.Where(x => codigoCTesBloqueadosPAVencer.Any(q => q.codigoCte == x.CTe.Codigo)).Select(x => x.ValorMonetario).Sum();
            AvisoPeriodicoQuitacao.TotalPendenciasVencidoBloqueioPOD = listarMovimentacao.Where(x => codigoCTesBloqueadosPVencidos.Any(q => q.codigoCte == x.CTe.Codigo)).Select(x => x.ValorMonetario).Sum();

            //Registro Com Irregulariada Pendentes Do transportador
            List<int> codigosCtePendetesAvencerTransportador = repositorioHistorioIrregulidade.BuscarPorCodigoControleDocumentos(documentosVencidos.Select(x => x.Codigo).ToList(), ServicoResponsavel.Transporador);
            List<int> codigosCtePendetesVencidaTransportador = repositorioHistorioIrregulidade.BuscarPorCodigoControleDocumentos(documentosAVencer.Select(x => x.Codigo).ToList(), ServicoResponsavel.Transporador);
            AvisoPeriodicoQuitacao.TotalPendenciasVencidoTransportador = listarMovimentacao.Where(x => codigosCtePendetesAvencerTransportador.Contains(x.CTe.Codigo)).Select(x => x.ValorMonetario).Sum();
            AvisoPeriodicoQuitacao.TotalPendenciasAVencerTransportador = listarMovimentacao.Where(x => codigosCtePendetesVencidaTransportador.Contains(x.CTe.Codigo)).Select(x => x.ValorMonetario).Sum();

            //Registro Com Irregulariada Pendentes Do embarcador
            List<int> codigosCtePendetesAvencerEmbarcador = repositorioHistorioIrregulidade.BuscarPorCodigoControleDocumentos(documentosVencidos.Select(x => x.Codigo).ToList(), ServicoResponsavel.Embarcador);
            List<int> codigosCtePendetesVencidaEmbarcador = repositorioHistorioIrregulidade.BuscarPorCodigoControleDocumentos(documentosAVencer.Select(x => x.Codigo).ToList(), ServicoResponsavel.Embarcador);
            AvisoPeriodicoQuitacao.TotalPendenciasVencidoUnilever = listarMovimentacao.Where(x => codigosCtePendetesAvencerEmbarcador.Contains(x.CTe.Codigo)).Select(x => x.ValorMonetario).Sum();
            AvisoPeriodicoQuitacao.TotalPendenciasAVencerUnilever = listarMovimentacao.Where(x => codigosCtePendetesVencidaEmbarcador.Contains(x.CTe.Codigo)).Select(x => x.ValorMonetario).Sum();

            //if "Todo registro que estiver como liberado OR (caso tenha irregularidade AND esteja finalizada)  AND BLOQUEIO IS NULL"
            AvisoPeriodicoQuitacao.TotalPendenciasVencidoDesbloqueado = codigoCteVencidos.Count > 0 ? repositorioHistorioIrregulidade.BuscarPorCodigoControleDocumentos(codigoCteVencidos) : 0m;
            AvisoPeriodicoQuitacao.TotalPendenciasAVencerDesbloqueado = codigoCteAvencer.Count > 0 ? repositorioHistorioIrregulidade.BuscarPorCodigoControleDocumentos(codigoCteAvencer) : 0m;

            AvisoPeriodicoQuitacao.TotalProjecoesRecebimento = AvisoPeriodicoQuitacao.TotalPendenciasVencidoDesbloqueado - AvisoPeriodicoQuitacao.TotalSaldoAdiantamentoEmAberto - AvisoPeriodicoQuitacao.TotalAvariasEmAberto - AvisoPeriodicoQuitacao.TotalDebitoBaixa;

            repositorioAvisoPeriodicoQuitacao.Atualizar(AvisoPeriodicoQuitacao);
        }

        private void EnviarEmailTransportadorInformandoGeracaoAviso(Dominio.Entidades.Embarcador.Financeiro.AvisoPeriodicoQuitacao aviso, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteAdmin, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin)
        {
            AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso clienteURLAcesso = new AdminMultisoftware.Repositorio.Pessoas.ClienteURLAcesso(unitOfWorkAdmin).BuscarPorClienteETipoProducao(clienteAdmin.Codigo, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe);
            Dominio.ObjetosDeValor.Email.Mensagem msg = new Dominio.ObjetosDeValor.Email.Mensagem()
            {
                Assunto = "Aviso Periódico de Contas a Receber",

                Corpo = $@"<h4>Olá Transportador!</h4>
                        <p>Informamos que foi gerado um aviso periódico para acompanhamento e validação de seus valores a receber.</p>
                        <p>O aviso com todos os detalhes está disponível no seu acesso no portal: <a href='https://{clienteURLAcesso.URLAcesso}'>Unilever - MultiTransportador</a>.</p>
                        <p>Por favor, verifique as informações e confirme o aviso dando conhecimento do mesmo.</p>",

                Destinatarios = new List<string>()
                {
                    aviso?.Transportador?.Email,
                }
            };

            Servicos.Email.EnviarMensagensAsync(new List<Dominio.ObjetosDeValor.Email.Mensagem>() { msg }, _unitOfWork);
        }
        
        #endregion

    }
}
