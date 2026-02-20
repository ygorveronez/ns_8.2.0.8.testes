using AdminMultisoftware.Dominio.Enumeradores;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Auditoria;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores.NotificacaoMobile;
using Dominio.ObjetosDeValor.Enumerador;
using Dominio.ObjetosDeValor.WebService.Rest.Webhook.SuperApp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.SuperApp.Eventos
{
    public class EventSubmit : IntegracaoSuperApp
    {
        private Dominio.Entidades.Embarcador.SuperApp.IntegracaoSuperApp _IntegracaoSuperApp;
        private int _codigoCarga;
        private int _codigoCargaEntrega;
        private double _identificacaoCliente;
        private string _cpfMotorista;
        private string _IDTrizy;
        private string _IDTrizyEntrega;

        public EventSubmit(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, unitOfWorkAdmin, clienteMultisoftware)
        {
            _unitOfWork = unitOfWork;
            _unitOfWorkAdmin = unitOfWorkAdmin;
            _clienteMultisoftware = clienteMultisoftware;
        }

        public void ProcessarEvento(Dominio.Entidades.Embarcador.SuperApp.IntegracaoSuperApp integracaoSuperApp, out RetornoIntegracaoSuperApp retornoIntegracaoSuperApp)
        {
            retornoIntegracaoSuperApp = new RetornoIntegracaoSuperApp();
            try
            {
                string jsonRequisicao = integracaoSuperApp.ArquivoRequisicao != null ? obterJsonRequisicao(integracaoSuperApp.ArquivoRequisicao) : integracaoSuperApp.StringJsonRequest;

                if (string.IsNullOrEmpty(jsonRequisicao))
                    throw new ServicoException($"Arquivo de integração/Request não encontrado.");

                EventoEventsSubmit eventoEventsSubmit = Newtonsoft.Json.JsonConvert.DeserializeObject<EventoEventsSubmit>(jsonRequisicao);
                if (eventoEventsSubmit == null)
                    throw new ServicoException("Falha na conversão da requisição para objeto.");


                string[] codigoExterno = eventoEventsSubmit.Data.StoppingPoint?.ExternalId?.Split(';');

                _IDTrizy = eventoEventsSubmit.Data.Travel._id;
                _IDTrizyEntrega = eventoEventsSubmit.Data.StoppingPoint._id;

                _codigoCarga = eventoEventsSubmit.Data.Travel.ExternalID.ToInt(0);

                _codigoCargaEntrega = codigoExterno != null && codigoExterno.Length > 0 ? codigoExterno[0].ToInt() : 0;
                _identificacaoCliente = codigoExterno != null && codigoExterno.Length > 1 ? codigoExterno[1].ToDouble() : 0;

                _cpfMotorista = eventoEventsSubmit.Data.Driver.Document.Value;
                _IntegracaoSuperApp = integracaoSuperApp;

                switch (eventoEventsSubmit.Data.Event.Type)
                {
                    case "START_TRAVEL":
                        Servicos.Log.TratarErro("Inicio IniciarViagem", "IntegracaoSuperAPPEventos");
                        retornoIntegracaoSuperApp = IniciarViagem(eventoEventsSubmit);
                        Servicos.Log.TratarErro("Fim IniciarViagem", "IntegracaoSuperAPPEventos");
                        break;

                    case "START_OPERATION":
                        Servicos.Log.TratarErro("Inicio InformarChegada", "IntegracaoSuperAPPEventos");
                        retornoIntegracaoSuperApp = InformarChegada(eventoEventsSubmit);
                        Servicos.Log.TratarErro("Fim InformarChegada", "IntegracaoSuperAPPEventos");
                        break;

                    case "CUSTOM":
                        Servicos.Log.TratarErro("Inicio CUSTOM", "IntegracaoSuperAPPEventos");
                        string externalID = eventoEventsSubmit.Data.Event.ExternalId;
                        TipoCustomEventAppTrizy customEvent = TipoCustomEventAppTrizyHelper.ObterEnumerador(externalID.Length > 1 ? externalID : externalID.ToInt());
                        switch (customEvent)
                        {
                            case TipoCustomEventAppTrizy.EstouIndo:
                                retornoIntegracaoSuperApp = MotoristaACaminho(eventoEventsSubmit);
                                break;

                            case TipoCustomEventAppTrizy.SolicitacaoDataeHoraCanhoto:
                                _IDTrizy = eventoEventsSubmit.Data.StoppingPoint._id;

                                retornoIntegracaoSuperApp = InformarDataeHoraCanhoto(eventoEventsSubmit);
                                break;
                        }
                        Servicos.Log.TratarErro("Fim CUSTOM", "IntegracaoSuperAPPEventos");
                        break;

                    case "END_OPERATION":
                        Servicos.Log.TratarErro("Inicio Confirmar", "IntegracaoSuperAPPEventos");
                        retornoIntegracaoSuperApp = Confirmar(eventoEventsSubmit);
                        Servicos.Log.TratarErro("Fim Confirmar", "IntegracaoSuperAPPEventos");
                        break;
                    default:
                        retornoIntegracaoSuperApp.Sucesso = true;
                        retornoIntegracaoSuperApp.Mensagem = "Evento Ignorado - " + $"Evento não tratado: {eventoEventsSubmit.Data.Event.Type} - Carga: {eventoEventsSubmit.Data.Travel.ExternalInfo.Id}";
                        Servicos.Log.TratarErro(retornoIntegracaoSuperApp.Mensagem);
                        break;
                }
            }
            catch (ServicoException ex)
            {
                retornoIntegracaoSuperApp.Mensagem = ex.Message;
            }
            catch (Exception)
            {
                retornoIntegracaoSuperApp.Mensagem = "Falha genérica ao processar " + TipoEventoApp.EventsSubmit.ObterDescricao();
            }
        }

        #region Métodos Privados
        private RetornoIntegracaoSuperApp InformarChegada(EventoEventsSubmit eventoEventsSubmit)
        {
            RetornoIntegracaoSuperApp retornoIntegracaoSuperApp = new RetornoIntegracaoSuperApp();
            try
            {
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(_unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);
                Repositorio.Embarcador.Cargas.ControleEntrega.ConfiguracaoOcorrenciaEntrega repositorioConfiguracaoOcorrenciaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.ConfiguracaoOcorrenciaEntrega(_unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoControleEntrega repConfiguracaoControleEntrega = new Repositorio.Embarcador.Configuracoes.ConfiguracaoControleEntrega(_unitOfWork);

                Servicos.Embarcador.Monitoramento.Monitoramento servMonitoramento = new Servicos.Embarcador.Monitoramento.Monitoramento();
                Servicos.Embarcador.Carga.ControleEntrega.OcorrenciaEntrega servicoOcorrenciaEntrega = new Servicos.Embarcador.Carga.ControleEntrega.OcorrenciaEntrega(_unitOfWork);

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoControleEntrega configuracaoControleEntrega = repConfiguracaoControleEntrega.BuscarPrimeiroRegistro();
                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = repCargaEntrega.BuscarPorIdTrizy(_IDTrizyEntrega) ?? repCargaEntrega.BuscarPorCodigo(_codigoCargaEntrega) ?? repCargaEntrega.BuscarPorClienteECarga(_codigoCarga, _identificacaoCliente);

                if (cargaEntrega == null) throw new ServicoException($"Coleta/Entrega não foi encontrada. ID:{_IDTrizyEntrega} | Código:{_codigoCargaEntrega}", CodigoExcecao.RegistroIgnorado);

                AtualizarCargaIntegracaoSuperApp(_IntegracaoSuperApp, cargaEntrega.Carga, cargaEntrega);

                DateTime? dataChegada = eventoEventsSubmit.Data.Event?.RealizedAt.ToLocalTime() ?? null;

                if (!dataChegada.HasValue) throw new ServicoException($"Sem data definida.", CodigoExcecao.RegistroIgnorado);

                cargaEntrega.DataEntradaRaio = dataChegada.Value;

                if (!cargaEntrega.Coleta)
                    cargaEntrega.ProntoParaDescarregar = true;

                Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint wayPoint = ObterWaypointEvento(eventoEventsSubmit.Data.Event.Location);
                cargaEntrega.LatitudeConfirmacaoChegada = (decimal)wayPoint.Latitude;
                cargaEntrega.LongitudeConfirmacaoChegada = (decimal)wayPoint.Longitude;

                Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.IniciarEntrega(cargaEntrega, wayPoint, dataChegada.Value, configuracaoTMS, TipoServicoMultisoftware.MultiMobile, _clienteMultisoftware, Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemSituacaoEntrega.App, configuracaoControleEntrega, _unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado();
                auditado.TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Sistema;
                auditado.OrigemAuditado = Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.WebServiceMobile;

                bool existeConfiguracaoUltimaChegada = repositorioConfiguracaoOcorrenciaEntrega.ExisteConfiguracaoOcorrenciaPorEvento(EventoColetaEntrega.UltimaChegadaNoAlvo);

                if (existeConfiguracaoUltimaChegada)
                {
                    bool ultimaChegada = Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.UltimaEntregaPendente(cargaEntrega.Carga, _unitOfWork);
                    EventoColetaEntrega eventoASerExecutado = ultimaChegada ? EventoColetaEntrega.UltimaChegadaNoAlvo : EventoColetaEntrega.ChegadaNoAlvo;
                    servicoOcorrenciaEntrega.GerarOcorrenciaEntrega(cargaEntrega, dataChegada.Value, eventoASerExecutado, cargaEntrega.LatitudeFinalizada, cargaEntrega.LongitudeFinalizada, configuracaoTMS, TipoServicoMultisoftware.MultiMobile, _clienteMultisoftware, OrigemSituacaoEntrega.WebService, configuracaoControleEntrega, auditado);
                }
                else
                    servicoOcorrenciaEntrega.GerarOcorrenciaEntrega(cargaEntrega, dataChegada.Value, EventoColetaEntrega.ChegadaNoAlvo, cargaEntrega.LatitudeFinalizada, cargaEntrega.LongitudeFinalizada, configuracaoTMS, TipoServicoMultisoftware.MultiMobile, _clienteMultisoftware, OrigemSituacaoEntrega.WebService, configuracaoControleEntrega, auditado);

                repCargaEntrega.Atualizar(cargaEntrega);
                Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.SincronizarEntregaOrigem(cargaEntrega, repCargaEntrega, _unitOfWork);

                if (configuracaoTMS?.UtilizaAppTrizy ?? false)
                    servMonitoramento.RegistrarPosicaoEventosRelevantesTrizy(cargaEntrega.Carga.Codigo, cargaEntrega.DataEntradaRaio ?? DateTime.Now, (double?)cargaEntrega.LatitudeConfirmacaoChegada, (double?)cargaEntrega.LongitudeConfirmacaoChegada, EventoRelevanteMonitoramento.ChegadaRaio, _unitOfWork);

                Servicos.Auditoria.Auditoria.Auditar(auditado, cargaEntrega, "Chegada confirmada às " + dataChegada.Value.ToString("dd/MM/yyyy HH:mm:ss"), _unitOfWork);

                Servicos.Log.TratarErro($"SuperApp - InformarChegada - Dados salvos com sucesso", "IntegracaoSuperAPPEventos");
                retornoIntegracaoSuperApp.Sucesso = true;
            }
            catch (ServicoException ex) when (ex.ErrorCode == Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.RegistroIgnorado)
            {
                retornoIntegracaoSuperApp.Sucesso = true;
                retornoIntegracaoSuperApp.Mensagem = ex.Message;
                Servicos.Log.TratarErro(ex, "IntegracaoSuperAPPEventos");
            }
            catch (ServicoException ex)
            {
                retornoIntegracaoSuperApp.Mensagem = ex.Message;
                Servicos.Log.TratarErro(ex, "IntegracaoSuperAPPEventos");
            }
            catch (Exception ex)
            {
                retornoIntegracaoSuperApp.Mensagem = "Falha genérica ao processar InformarChegada - " + TipoEventoApp.EventsSubmit.ObterDescricao();
                Servicos.Log.TratarErro(ex, "IntegracaoSuperAPPEventos");
            }
            return retornoIntegracaoSuperApp;
        }

        private RetornoIntegracaoSuperApp IniciarViagem(EventoEventsSubmit eventoEventsSubmit)
        {
            RetornoIntegracaoSuperApp retornoIntegracaoSuperApp = new RetornoIntegracaoSuperApp();
            try
            {
                int codigoCarga = eventoEventsSubmit.Data.Travel.ExternalID.ToInt(valorPadrao: 0);

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);
                Repositorio.Embarcador.Logistica.Monitoramento repMonitoramento = new Repositorio.Embarcador.Logistica.Monitoramento(_unitOfWork);
                Servicos.Embarcador.TorreControle.AlertaAcompanhamentoCarga servAlertaAcompanhamentoCarga = new Servicos.Embarcador.TorreControle.AlertaAcompanhamentoCarga(_unitOfWork);

                // Validação
                if (carga == null) throw new ServicoException("Carga não encontrada", CodigoExcecao.RegistroIgnorado);
                if (carga.DataInicioViagem != null) throw new ServicoException("Carga já foi iniciada", CodigoExcecao.RegistroIgnorado);

                AtualizarCargaIntegracaoSuperApp(_IntegracaoSuperApp, carga, null);

                Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint wayPoint = null;
                if (!(carga.TipoOperacao?.ConfiguracaoMobile?.BloquearRastreamento ?? false))
                {
                    wayPoint = ObterWaypointEvento(eventoEventsSubmit.Data.Event.Location);
                }

                Repositorio.Usuario repositorioMotorista = new Repositorio.Usuario(_unitOfWork);
                Dominio.Entidades.Usuario motorista = repositorioMotorista.BuscarMotoristaPorCPF(_cpfMotorista);

                Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado();
                auditado.TipoAuditado = TipoAuditado.Usuario;
                auditado.OrigemAuditado = OrigemAuditado.WebServiceMobile;

                if (Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.IniciarViagem(carga.Codigo, eventoEventsSubmit.Data.Event.RealizedAt.ToLocalTime(), OrigemSituacaoEntrega.App, wayPoint, _configuracaoTMS, TipoServicoMultisoftware.MultiMobile, _clienteMultisoftware, auditado, _unitOfWork))
                {
                    if (motorista != null)
                        Servicos.Auditoria.Auditoria.Auditar(auditado, carga, $"Início de viagem informado pelo motorista {motorista.Descricao} via SuperApp", _unitOfWork);

                    if (carga.TipoOperacao?.ConfiguracaoMobile?.IniciarViagemNoControleDePatioAoIniciarViagemNoApp ?? false)
                    {
                        Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio servicoFluxoGestaoPatio = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio(_unitOfWork);
                        Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio = servicoFluxoGestaoPatio.ObterFluxoGestaoPatio(carga);

                        servicoFluxoGestaoPatio.LiberarProximaEtapa(fluxoGestaoPatio, EtapaFluxoGestaoPatio.InicioViagem, DateTime.Now);

                        Servicos.Auditoria.Auditoria.Auditar(auditado, fluxoGestaoPatio, $"Etapa do Fluxo de pátio confirmada por {motorista.Descricao} via SuperApp", _unitOfWork);
                    }

                    List<Dominio.Entidades.Embarcador.Logistica.Monitoramento> listMonitoramento = repMonitoramento.BuscarMonitoramentoEmAbertoPorVeiculoPlaca(carga.Veiculo.Placa);

                    if (listMonitoramento.Count > 0)
                    {
                        Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramentoCarga = listMonitoramento.Find(p => p.Carga.CodigoCargaEmbarcador != carga.CodigoCargaEmbarcador);
                        if (monitoramentoCarga != null)
                        {
                            Dominio.Entidades.Embarcador.Cargas.Carga cargaComMonitoramentoAtivo = repCarga.BuscarPorCodigo(monitoramentoCarga.Carga.Codigo);
                            Servicos.Auditoria.Auditoria.Auditar(auditado, cargaComMonitoramentoAtivo, $"Monitoramento encerrado na carga {cargaComMonitoramentoAtivo.CodigoCargaEmbarcador}, visto que foi iniciada viagem na carga {carga.CodigoCargaEmbarcador} pelo motorista {motorista.Descricao} via SuperApp.", _unitOfWork);
                        }
                    }
                }

                //forçar mostrar alerta na tela de acompanhamento carga
                servAlertaAcompanhamentoCarga.informarAtualizacaoCardCargasAcompanamentoMSMQ(carga, _clienteMultisoftware.Codigo);

                Servicos.Log.TratarErro($"SuperApp - IniciarViagem - Finalizado", "IntegracaoSuperAPPEventos");

                retornoIntegracaoSuperApp.Sucesso = true;
            }
            catch (ServicoException ex) when (ex.ErrorCode == Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.RegistroIgnorado)
            {
                retornoIntegracaoSuperApp.Sucesso = true;
                retornoIntegracaoSuperApp.Mensagem = ex.Message;
                Servicos.Log.TratarErro(ex, "IntegracaoSuperAPPEventos");
            }
            catch (BaseException ex)
            {
                retornoIntegracaoSuperApp.Mensagem = ex.Message;
                Servicos.Log.TratarErro(ex, "IntegracaoSuperAPPEventos");
            }
            catch (Exception ex)
            {
                retornoIntegracaoSuperApp.Mensagem = "Falha genérica ao processar IniciarViagem - " + TipoEventoApp.EventsSubmit.ObterDescricao();
                Servicos.Log.TratarErro(ex, "IntegracaoSuperAPPEventos");
            }

            return retornoIntegracaoSuperApp;
        }

        private RetornoIntegracaoSuperApp Confirmar(EventoEventsSubmit eventoEventsSubmit)
        {
            RetornoIntegracaoSuperApp retornoIntegracaoSuperApp = new RetornoIntegracaoSuperApp();
            try
            {
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(_unitOfWork);
                Repositorio.Usuario repositorioMotorista = new Repositorio.Usuario(_unitOfWork);

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoControleEntrega configuracaoControleEntrega = new Repositorio.Embarcador.Configuracoes.ConfiguracaoControleEntrega(_unitOfWork).BuscarPrimeiroRegistro();

                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = repCargaEntrega.BuscarPorIdTrizy(_IDTrizyEntrega) ?? repCargaEntrega.BuscarPorCodigo(_codigoCargaEntrega) ?? repCargaEntrega.BuscarPorCargaECliente(_codigoCarga, _identificacaoCliente) ?? throw new ServicoException($"Entrega não localizada. ID:{_IDTrizyEntrega} | Código:{_codigoCargaEntrega}");

                AtualizarCargaIntegracaoSuperApp(_IntegracaoSuperApp, cargaEntrega.Carga, cargaEntrega);

                Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacaoParametro = new Repositorio.Embarcador.Pedidos.TipoOperacao(_unitOfWork).BuscarPorCodigoFetch(cargaEntrega.Carga.TipoOperacao?.Codigo ?? 0);
                Dominio.Entidades.Usuario motorista = repositorioMotorista.BuscarMotoristaPorCPF(_cpfMotorista);

                List<SituacaoEntrega> listaSituacaoEmAberto = SituacaoEntregaHelper.ObterListaSituacaoEntregaEmAberto();
                bool permitirRetificarColeta = cargaEntrega.Coleta && (cargaEntrega.Carga.TipoOperacao?.PermiteRetificarMobile ?? false); //TRIZY NÃO TEM ESSA RETIFICAÇÃO: Aurora retifica coleta já confirmada, deve permitir confirmar mesmo entregue
                bool entregaAgAtendimentoTrizy = cargaEntrega.Situacao == SituacaoEntrega.AgAtendimento;

                if (!listaSituacaoEmAberto.Contains(cargaEntrega.Situacao) && !permitirRetificarColeta)
                {
                    if (cargaEntrega.Situacao == SituacaoEntrega.Entregue)
                    {
                        retornoIntegracaoSuperApp.Sucesso = true;
                        retornoIntegracaoSuperApp.Mensagem = "Coleta/Entrega finalizada";
                        return retornoIntegracaoSuperApp;
                    }
                    else if (!entregaAgAtendimentoTrizy) throw new ServicoException($"Situação da entrega " + cargaEntrega.Situacao.ObterDescricao() + " não permite confirmar.");
                }

                // Parse datas
                DateTime dataInicioColetaEntrega = DateTime.Now;
                DateTime dataConfirmacaoChegada = eventoEventsSubmit.Data.Event?.PreviousAt.ToLocalTime() ?? eventoEventsSubmit.Data.Event?.RealizedAt.ToLocalTime() ?? DateTime.MinValue;
                DateTime dataTerminoColetaEntrega = DateTime.Now;
                DateTime dataConfirmacao = eventoEventsSubmit.Data.Event?.RealizedAt.ToLocalTime() ?? DateTime.MinValue;

                if (dataConfirmacaoChegada != DateTime.MinValue)
                    dataInicioColetaEntrega = dataConfirmacaoChegada;

                // Convertendo as coordenadas para waypoints
                Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint wayPointConfirmacaoColetaEntrega = null;
                Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint wayPointConfirmacaoChegada = null;
                if (!(cargaEntrega.Carga.TipoOperacao?.ConfiguracaoMobile?.BloquearRastreamento ?? false))
                {
                    wayPointConfirmacaoColetaEntrega = ObterWaypointEvento(eventoEventsSubmit.Data.Event.Location);
                    wayPointConfirmacaoChegada = ObterWaypointEvento(eventoEventsSubmit.Data.Event.Location);
                }

                DateTime? dataConfirmacaoChegadaAux = dataConfirmacaoChegada;
                if (_configuracaoTMS?.RegistrarChegadaAppEmMetodoDiferenteDoConfirmar ?? false)
                    dataConfirmacaoChegadaAux = null;

                Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.FinalizarEntregaParametros parametrosFinalizarEntrega = new Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.FinalizarEntregaParametros
                {
                    cargaEntrega = cargaEntrega,
                    dataInicioEntrega = dataInicioColetaEntrega,
                    dataTerminoEntrega = dataTerminoColetaEntrega,
                    dataConfirmacao = dataConfirmacao,
                    wayPoint = wayPointConfirmacaoColetaEntrega,
                    motivoFalhaGTA = 0,
                    configuracaoEmbarcador = _configuracaoTMS,
                    tipoServicoMultisoftware = TipoServicoMultisoftware.MultiMobile,
                    sistemaOrigem = Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.Sistema,

                    // Confirmação de chegada
                    dataConfirmacaoChegada = dataConfirmacaoChegadaAux,
                    wayPointConfirmacaoChegada = wayPointConfirmacaoChegada,

                    // Avaliação da coleta/entrega
                    avaliacaoColetaEntrega = 0,

                    motorista = motorista,
                    observacao = string.Empty,

                    OrigemSituacaoEntrega = Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemSituacaoEntrega.App,
                    auditado = ObterAuditado(motorista),
                    configuracaoControleEntrega = configuracaoControleEntrega,
                    tipoOperacaoParametro = tipoOperacaoParametro,
                    TornarFinalizacaoDeEntregasAssincrona = configuracaoControleEntrega.TornarFinalizacaoDeEntregasAssincrona,
                    FinalizandoViagem = false
                };

                Servicos.Log.TratarErro($"Inicia finalizar pelo Confirmar do SuperApp-EventSubmit {cargaEntrega.Codigo}", "FinalizarEntrega");
                Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.FinalizarEntrega(parametrosFinalizarEntrega, _unitOfWork);
                Servicos.Log.TratarErro($"Passou pelo finalizar no Confirmar do SuperApp-EventSubmit {cargaEntrega.Codigo}", "FinalizarEntrega");

                if (cargaEntrega.Carga != null)
                {
                    Servicos.Embarcador.CargaOcorrencia.OcorrenciaAutomaticaPorPeriodo servicoOcorrenciaAutomaticaPorPeriodo = new Servicos.Embarcador.CargaOcorrencia.OcorrenciaAutomaticaPorPeriodo(_unitOfWork);

                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAplicacaoGatilhoTracking tipoAplicacaoGatilhoTracking = cargaEntrega.Coleta ? TipoAplicacaoGatilhoTracking.Coleta : TipoAplicacaoGatilhoTracking.Entrega;

                    if (cargaEntrega.Fronteira)
                        servicoOcorrenciaAutomaticaPorPeriodo.GerarOcorrenciaPorTracking(cargaEntrega.Carga, cargaEntrega.Cliente.CPF_CNPJ, GatilhoFinalTraking.SaidaFronteira, dataConfirmacaoChegada, dataConfirmacao, TipoServicoMultisoftware.MultiMobile, _clienteMultisoftware, tipoAplicacaoGatilhoTracking, cargaEntrega.DataAgendamento);
                    else if (cargaEntrega.Parqueamento)
                        servicoOcorrenciaAutomaticaPorPeriodo.GerarOcorrenciaPorTracking(cargaEntrega.Carga, cargaEntrega.Cliente.CPF_CNPJ, GatilhoFinalTraking.SaidaParqueamento, dataConfirmacaoChegada, dataConfirmacao, TipoServicoMultisoftware.MultiMobile, _clienteMultisoftware, tipoAplicacaoGatilhoTracking, cargaEntrega.DataAgendamento);
                    else
                        servicoOcorrenciaAutomaticaPorPeriodo.GerarOcorrenciaPorTracking(cargaEntrega.Carga, codigoFronteiraParqueamento: 0d, GatilhoFinalTraking.FimEntrega, dataInicioColetaEntrega, dataTerminoColetaEntrega, TipoServicoMultisoftware.MultiMobile, _clienteMultisoftware, tipoAplicacaoGatilhoTracking, cargaEntrega.DataAgendamento);
                }

                Servicos.Log.TratarErro($"SuperApp - Confirmar - Dados salvos com sucesso", "IntegracaoSuperAPPEventos");
                Servicos.Log.TratarErro($"Finalizou a entrega pelo finalizar no Confirmar do EventSubmit.cs {cargaEntrega.Codigo}", "FinalizarEntrega");

                retornoIntegracaoSuperApp.Sucesso = true;
            }

            catch (ServicoException ex) when (ex.ErrorCode == Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.RegistroIgnorado)
            {
                retornoIntegracaoSuperApp.Sucesso = true;
                retornoIntegracaoSuperApp.Mensagem = ex.Message;

                Servicos.Log.TratarErro(ex, "IntegracaoSuperAPPEventos");
            }
            catch (ServicoException ex)
            {
                retornoIntegracaoSuperApp.Mensagem = ex.Message;
                Servicos.Log.TratarErro(ex, "IntegracaoSuperAPPEventos");
            }
            catch (BaseException ex)
            {
                retornoIntegracaoSuperApp.Mensagem = ex.Message;
                Servicos.Log.TratarErro(ex, "IntegracaoSuperAPPEventos");
            }
            catch (Exception ex)
            {
                retornoIntegracaoSuperApp.Mensagem = "Falha genérica ao processar Confirmar Coleta/Entrega - " + TipoEventoApp.EventsSubmit.ObterDescricao();
                Servicos.Log.TratarErro(ex, "IntegracaoSuperAPPEventos");
            }

            return retornoIntegracaoSuperApp;
        }

        private RetornoIntegracaoSuperApp MotoristaACaminho(EventoEventsSubmit eventoEventsSubmit)
        {
            RetornoIntegracaoSuperApp retornoIntegracaoSuperApp = new RetornoIntegracaoSuperApp();

            Auditado auditado = new Auditado()
            {
                TipoAuditado = TipoAuditado.Usuario,
                OrigemAuditado = OrigemAuditado.WebServiceMobile,
                Usuario = ObterUsuario(_unitOfWork, _cpfMotorista)
            };

            try
            {
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(_unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorIDIdentificacaoTrizy(_IDTrizy) ?? repCarga.BuscarPorCodigo(_codigoCarga);

                if (carga == null) throw new ServicoException($"Carga não foi encontrada. IDTrizy: {_IDTrizy}", CodigoExcecao.RegistroIgnorado);
                if (carga.DataPreViagemFim.HasValue) throw new ServicoException($"Pré Trip já foi finalizada dia {carga.DataPreViagemFim.Value.ToString("dd/MM/yyyy HH:mm:ss")}. IDTrizy: {_IDTrizy}", CodigoExcecao.RegistroIgnorado);

                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = repCargaEntrega.BuscarPorIdTrizy(_IDTrizyEntrega) ?? repCargaEntrega.BuscarPorCodigo(_codigoCargaEntrega) ?? repCargaEntrega.BuscarPorCargaECliente(carga.Codigo, _identificacaoCliente);

                AtualizarCargaIntegracaoSuperApp(_IntegracaoSuperApp, carga, cargaEntrega);

                //Marcar data início da PréTrip.
                if (_configuracaoTMS.QuandoIniciarMonitoramento == QuandoIniciarMonitoramento.EstouIndoAoIniciarViagem)
                {
                    if (!carga.DataPreViagemInicio.HasValue)
                    {
                        carga.DataPreViagemInicio = eventoEventsSubmit.Data.Event?.RealizedAt.ToLocalTime() ?? DateTime.Now;
                        repCarga.Atualizar(carga);
                    }
                    Servicos.Embarcador.Monitoramento.Monitoramento.GerarMonitoramentoEIniciar(carga, _configuracaoTMS, auditado, "Motorista sinalizou que está indo (PreTrip)", _unitOfWork);

                    Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint wayPoint = ObterWaypointEvento(eventoEventsSubmit.Data.Event.Location);
                    AdicionarPosicaoProcessar(carga, wayPoint.Latitude, wayPoint.Longitude, _unitOfWork);
                    Servicos.Auditoria.Auditoria.Auditar(auditado, carga, "Motorista sinalizou que está indo (PreTrip)", _unitOfWork);
                }

                //Marcar Motorista a Caminho na entrega.
                if (cargaEntrega != null)
                {
                    if (!(cargaEntrega.Situacao == SituacaoEntrega.Entregue || cargaEntrega.Situacao == SituacaoEntrega.Rejeitado))
                    {
                        cargaEntrega.MotoristaACaminho = true;
                        repCargaEntrega.Atualizar(cargaEntrega);
                        Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.SincronizarEntregaOrigem(cargaEntrega, repCargaEntrega, _unitOfWork);
                        Servicos.Auditoria.Auditoria.Auditar(auditado, cargaEntrega, "Motorista sinalizou que está a caminho", _unitOfWork);
                    }
                    else
                        retornoIntegracaoSuperApp.Mensagem = $"Processado - Situação da {cargaEntrega.TipoCargaEntrega} não permite - Situação: {cargaEntrega.Situacao}";
                }
                else
                    retornoIntegracaoSuperApp.Mensagem = $"Processado - Coleta/Entrega não encontrado. ID:{_IDTrizyEntrega} | Código:{_codigoCargaEntrega}";

                Servicos.Log.TratarErro($"SuperApp - RequestMotoristaACaminho - Dados salvos com sucesso", "IntegracaoSuperAPPEventos");
                retornoIntegracaoSuperApp.Sucesso = true;
            }
            catch (ServicoException ex) when (ex.ErrorCode == Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.RegistroIgnorado)
            {
                retornoIntegracaoSuperApp.Sucesso = true;
                retornoIntegracaoSuperApp.Mensagem = ex.Message;
                Servicos.Log.TratarErro(ex, "IntegracaoSuperAPPEventos");
            }
            catch (ServicoException ex)
            {
                retornoIntegracaoSuperApp.Mensagem = ex.Message;
                Servicos.Log.TratarErro(ex, "IntegracaoSuperAPPEventos");
            }
            catch (Exception ex)
            {
                retornoIntegracaoSuperApp.Mensagem = "Falha genérica ao processar MotoristaACaminho - " + TipoEventoApp.EventsSubmit.ObterDescricao();
                Servicos.Log.TratarErro(ex, "IntegracaoSuperAPPEventos");
            }
            return retornoIntegracaoSuperApp;
        }

        private RetornoIntegracaoSuperApp InformarDataeHoraCanhoto(EventoEventsSubmit eventoEventsSubmit)
        {
            RetornoIntegracaoSuperApp retornoIntegracaoSuperApp = new RetornoIntegracaoSuperApp();

            Auditado auditado = new Auditado()
            {
                TipoAuditado = TipoAuditado.Usuario,
                OrigemAuditado = OrigemAuditado.WebServiceMobile,
                Usuario = ObterUsuario(_unitOfWork, _cpfMotorista)
            };

            try
            {
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(_unitOfWork);
                Repositorio.Embarcador.Canhotos.Canhoto repCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(_unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = repCargaEntrega.BuscarPorIdTrizy(_IDTrizyEntrega) ?? repCargaEntrega.BuscarPorCodigo(_codigoCargaEntrega) ?? repCargaEntrega.BuscarPorCargaECliente(_codigoCarga, _identificacaoCliente);

                if (cargaEntrega == null) throw new ServicoException($"Coleta/Entrega não foi encontrada. ID:{_IDTrizy} | Código:{_codigoCargaEntrega}");

                AtualizarCargaIntegracaoSuperApp(_IntegracaoSuperApp, cargaEntrega.Carga, cargaEntrega);

                cargaEntrega.DataConfirmacaoApp = eventoEventsSubmit.Data.Event?.RealizedAt.ToLocalTime() ?? eventoEventsSubmit.Data.Event?.SynchronizedAt.ToLocalTime() ?? DateTime.Now;
                repCargaEntrega.Atualizar(cargaEntrega);

                if (cargaEntrega.Carga.TipoOperacao?.ConfiguracaoTrizy?.VincularDataEHoraSolicitadaNoCanhoto ?? false)
                {
                    List<int> numerosCanhotos = cargaEntrega.NotasFiscais?.Select(o => o.PedidoXMLNotaFiscal?.XMLNotaFiscal?.Codigo ?? 0).ToList();
                    List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> listaCanhotos = repCanhoto.BuscarCanhotosPorNFs(numerosCanhotos);
                    foreach (Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto in listaCanhotos)
                    {
                        canhoto.DataEntregaNotaCliente = cargaEntrega.DataConfirmacaoApp;
                        repCanhoto.Atualizar(canhoto);
                    }
                }

                Servicos.Auditoria.Auditoria.Auditar(auditado, cargaEntrega, "Motorista confirmou data e hora do canhoto.", _unitOfWork);

                Servicos.Log.TratarErro($"SuperApp - RequestSolicitacaoDataeHoraCanhoto - Dados salvos com sucesso", "IntegracaoSuperAPPEventos");
                retornoIntegracaoSuperApp.Sucesso = true;
            }
            catch (ServicoException ex)
            {
                retornoIntegracaoSuperApp.Mensagem = ex.Message;
                Servicos.Log.TratarErro(ex, "IntegracaoSuperAPPEventos");
            }
            catch (Exception ex)
            {
                retornoIntegracaoSuperApp.Mensagem = "Falha genérica ao processar InformarDataeHoraCanhoto - " + TipoEventoApp.EventsSubmit.ObterDescricao();
                Servicos.Log.TratarErro(ex, "IntegracaoSuperAPPEventos");
            }

            return retornoIntegracaoSuperApp;
        }

        private Dominio.Entidades.Usuario ObterUsuario(Repositorio.UnitOfWork unitOfWork, string cpf)
        {
            Repositorio.Usuario repositorioUsuario = new Repositorio.Usuario(unitOfWork);

            return repositorioUsuario.BuscarPorCPF(cpf);
        }

        private void AdicionarPosicaoProcessar(Dominio.Entidades.Embarcador.Cargas.Carga carga, double latitude, double longitude, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Logistica.Monitoramento repositorioMonitoramento = new Repositorio.Embarcador.Logistica.Monitoramento(unitOfWork);
            Repositorio.Embarcador.Logistica.Posicao repositorioPosicao = new Repositorio.Embarcador.Logistica.Posicao(unitOfWork);
            Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento;

            if (carga == null || !Servicos.Embarcador.Logistica.WayPointUtil.ValidarCoordenadas(latitude, longitude))
                return;

            monitoramento = repositorioMonitoramento.BuscarUltimoPorCarga(carga.Codigo);
            Dominio.Entidades.Embarcador.Logistica.Posicao posicaoPendenteIntegracao = new Dominio.Entidades.Embarcador.Logistica.Posicao()
            {
                Data = DateTime.Now,
                DataVeiculo = DateTime.Now,
                DataCadastro = DateTime.Now,
                IDEquipamento = string.IsNullOrEmpty(monitoramento?.UltimaPosicao?.IDEquipamento) ? carga.Veiculo?.Codigo.ToString() : monitoramento.UltimaPosicao?.IDEquipamento,
                Descricao = $"{latitude}, {longitude}",
                Veiculo = carga.Veiculo,
                Latitude = latitude,
                Longitude = longitude,
                Processar = ProcessarPosicao.Pendente,
                Rastreador = monitoramento?.UltimaPosicao?.Rastreador ?? EnumTecnologiaRastreador.Mobile
            };

            repositorioPosicao.Inserir(posicaoPendenteIntegracao);
        }
        #endregion
    }
}