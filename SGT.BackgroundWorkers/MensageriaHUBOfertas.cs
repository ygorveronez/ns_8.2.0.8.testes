using Azure.Messaging.ServiceBus;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Servicos.Embarcador.Integracao.HUB.Base;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SGT.BackgroundWorkers
{
    public sealed class MensageriaHUBOfertas : LongRunningProcessBase<MensageriaHUBOfertas>
    {
        private const string TopicoDemandaTransporte = "transport-demand-topic";
        private const string TopicoOfertas = "offer-topic";
        private const string TopicoTransportador = "carrier-topic";

        private ServiceBusClient _client;
        private ServiceBusProcessor _processadorDemanda;
        private ServiceBusProcessor _processadorOferta;
        private ServiceBusProcessor _processadorTransportador;

        private string _conexaoServiceBus;
        private string _subscription;

        private volatile bool _inicializado;
        private volatile bool _parando;
        private int _hookCancelamentoRegistrado;

        private readonly SemaphoreSlim _startGate = new SemaphoreSlim(1, 1);

        protected override async Task ExecuteInternalAsync(
            Repositorio.UnitOfWork unitOfWork,
            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin,
            CancellationToken cancellationToken)
        {
            if (!_iniciarProcessoAtivo)
                return;

            if (_parando || cancellationToken.IsCancellationRequested)
            {
                await FinalizarMensageriaAsync(CancellationToken.None).ConfigureAwait(false);
                return;
            }

            if (_inicializado)
            {
                RegistrarHookCancelamentoUmaVez(cancellationToken);
                return;
            }

            await _startGate.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                if (_inicializado)
                {
                    RegistrarHookCancelamentoUmaVez(cancellationToken);
                    return;
                }

                if (!ValidarTipoIntegracaoHUB(unitOfWork))
                    return;

                if (!CarregarConfiguracaoHUB(unitOfWork))
                    return;

                IniciarClientEProcessadores();
                RegistrarHandlers();

                await Task.WhenAll(
                    _processadorDemanda.StartProcessingAsync(cancellationToken),
                    _processadorOferta.StartProcessingAsync(cancellationToken),
                    _processadorTransportador.StartProcessingAsync(cancellationToken)
                ).ConfigureAwait(false);

                _inicializado = true;

                Servicos.Log.GravarInfo(
                    string.Format("[HUBOfertas] Processadores iniciados. Subscricao={0}", _subscription),
                    "HUBOfertas");

                RegistrarHookCancelamentoUmaVez(cancellationToken);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("[HUBOfertas] Falha ao iniciar processadores: " + ex, "HUBOfertas");
                await FinalizarMensageriaAsync(CancellationToken.None).ConfigureAwait(false);
                throw;
            }
            finally
            {
                _startGate.Release();
            }
        }

        private void RegistrarHookCancelamentoUmaVez(CancellationToken cancellationToken)
        {
            if (Interlocked.Exchange(ref _hookCancelamentoRegistrado, 1) == 1)
                return;

            cancellationToken.Register(delegate
            {
                _parando = true;

                Task.Run(async delegate
                {
                    try
                    {
                        await FinalizarMensageriaAsync(CancellationToken.None).ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        Servicos.Log.TratarErro("[HUBOfertas] Erro ao finalizar durante cancelamento: " + ex, "HUBOfertas");
                    }
                });
            });
        }

        private void IniciarClientEProcessadores()
        {
            _client = new ServiceBusClient(_conexaoServiceBus, new ServiceBusClientOptions
            {
                TransportType = ServiceBusTransportType.AmqpWebSockets
            });

            _processadorDemanda = CriarProcessador(_client, TopicoDemandaTransporte, _subscription);
            _processadorOferta = CriarProcessador(_client, TopicoOfertas, _subscription);
            _processadorTransportador = CriarProcessador(_client, TopicoTransportador, _subscription);
        }

        private static ServiceBusProcessor CriarProcessador(ServiceBusClient client, string topic, string subscription)
        {
            var options = new ServiceBusProcessorOptions
            {
                AutoCompleteMessages = false,
                MaxConcurrentCalls = 1,
                PrefetchCount = 0,
                MaxAutoLockRenewalDuration = TimeSpan.FromMinutes(5)
            };

            return client.CreateProcessor(topic, subscription, options);
        }

        private void RegistrarHandlers()
        {
            _processadorDemanda.ProcessMessageAsync += delegate (ProcessMessageEventArgs args)
            {
                return ProcessarMensagem(TopicoDemandaTransporte, args, ExecutarDemanda);
            };

            _processadorOferta.ProcessMessageAsync += delegate (ProcessMessageEventArgs args)
            {
                return ProcessarMensagem(TopicoOfertas, args, ExecutarOferta);
            };

            _processadorTransportador.ProcessMessageAsync += delegate (ProcessMessageEventArgs args)
            {
                return ProcessarMensagem(TopicoTransportador, args, ExecutarTransportador);
            };

            _processadorDemanda.ProcessErrorAsync += delegate (ProcessErrorEventArgs e) { return OnProcessorError(e, TopicoDemandaTransporte); };
            _processadorOferta.ProcessErrorAsync += delegate (ProcessErrorEventArgs e) { return OnProcessorError(e, TopicoOfertas); };
            _processadorTransportador.ProcessErrorAsync += delegate (ProcessErrorEventArgs e) { return OnProcessorError(e, TopicoTransportador); };
        }

        private Task OnProcessorError(ProcessErrorEventArgs args, string topic)
        {
            Servicos.Log.TratarErro(
                string.Format("[HUBOfertas] Erro no processador topico={0} entidade={1} origem={2} excecao={3}",
                    topic, args.EntityPath, args.ErrorSource, args.Exception),
                "HUBOfertas");

            return Task.CompletedTask;
        }

        private async Task ProcessarMensagem(
            string topic,
            ProcessMessageEventArgs args,
            Func<Repositorio.UnitOfWork, string, CancellationToken, Task<bool>> executarServico)
        {
            var mensagem = args.Message;

            Servicos.Log.GravarInfo(
                string.Format("[HUBOfertas] Mensagem recebida topico={0} messageId={1} deliveryCount={2}",
                    topic, mensagem.MessageId, mensagem.DeliveryCount),
                "HUBOfertas");

            var conteudo = mensagem.Body.ToString();
            if (string.IsNullOrWhiteSpace(conteudo))
            {
                Servicos.Log.TratarErro(
                    string.Format("[HUBOfertas] Corpo vazio. Enviando para DeadLetter topico={0} messageId={1}",
                        topic, mensagem.MessageId),
                    "HUBOfertas");

                await args.DeadLetterMessageAsync(mensagem, "INVALID_PAYLOAD", "Body vazio/nulo.").ConfigureAwait(false);
                return;
            }

            var unitOfWorkMensagem = new Repositorio.UnitOfWork(_stringConexao);

            try
            {
                await unitOfWorkMensagem.StartAsync().ConfigureAwait(false);

                bool retorno = await executarServico(unitOfWorkMensagem, conteudo, args.CancellationToken).ConfigureAwait(false);

                if (retorno)
                {
                    await unitOfWorkMensagem.CommitChangesAsync().ConfigureAwait(false);
                    await args.CompleteMessageAsync(mensagem).ConfigureAwait(false);

                    Servicos.Log.GravarInfo(
                        string.Format("[HUBOfertas] Sucesso. Commit+Complete topico={0} messageId={1}",
                            topic, mensagem.MessageId),
                        "HUBOfertas");
                }
                else
                {
                    await unitOfWorkMensagem.RollbackAsync().ConfigureAwait(false);
                    await args.AbandonMessageAsync(mensagem).ConfigureAwait(false);

                    Servicos.Log.TratarErro(
                        string.Format("[HUBOfertas] Retorno=false. Rollback+Abandon topico={0} messageId={1}",
                            topic, mensagem.MessageId),
                        "HUBOfertas");
                }
            }
            catch (Exception ex)
            {
                try { await unitOfWorkMensagem.RollbackAsync().ConfigureAwait(false); } catch { }

                var acao = ServiceBusPoliticaDeFalha.Decidir(ex);

                Servicos.Log.TratarErro(
                    string.Format("[HUBOfertas] Excecao. acao={0} topico={1} messageId={2} excecao={3}",
                        acao, topic, mensagem.MessageId, ex),
                    "HUBOfertas");

                if (acao == EnumFalhaMensageria.DeadLetter)
                    await args.DeadLetterMessageAsync(mensagem, "PROCESSING_ERROR", ex.Message).ConfigureAwait(false);
                else
                    await args.AbandonMessageAsync(mensagem).ConfigureAwait(false);
            }
            finally
            {
                try { unitOfWorkMensagem.Dispose(); } catch { }
            }
        }

        private Task<bool> ExecutarDemanda(Repositorio.UnitOfWork uow, string payload, CancellationToken ct)
        {
            var handler = new Servicos.Embarcador.Integracao.HUB.Mensageria.Demanda.RetornoDemandaHUBOfertas(uow);
            return handler.ProcessarRetornoAsync(payload);
        }

        private Task<bool> ExecutarOferta(Repositorio.UnitOfWork uow, string payload, CancellationToken ct)
        {
            var handler = new Servicos.Embarcador.Integracao.HUB.Mensageria.Oferta.RetornoOfertaHUBOfertas(uow);
            return handler.ProcessarRetornoOfertaHUBOfertas(payload);
        }

        private Task<bool> ExecutarTransportador(Repositorio.UnitOfWork uow, string payload, CancellationToken ct)
        {
            var handler = new Servicos.Embarcador.Integracao.HUB.Mensageria.Transportador.RetornoTransportadorHUBOfertas(uow);
            return handler.ProcessarRetornoAsync(payload);
        }

        private bool ValidarTipoIntegracaoHUB(Repositorio.UnitOfWork unitOfWork)
        {
            var tipoIntegracaoRepo = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
            var tipoIntegracao = tipoIntegracaoRepo.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.HUB);

            if (tipoIntegracao != null)
                return true;

            Servicos.Log.GravarInfo("[HUBOfertas] TipoIntegracao HUB nao encontrado. Processo nao iniciara.", "HUBOfertas");
            return false;
        }

        private bool CarregarConfiguracaoHUB(Repositorio.UnitOfWork unitOfWork)
        {
            var repoConfig = new Repositorio.Embarcador.Configuracoes.IntegracaoHUB(unitOfWork);
            var configuracao = repoConfig.BuscarPrimeiroRegistro();

            if (configuracao == null || string.IsNullOrWhiteSpace(configuracao.ConexaoServiceBUS))
            {
                Servicos.Log.GravarInfo("[HUBOfertas] Configuracao HUB ausente/sem ConexaoServiceBUS. Processo nao iniciara.", "HUBOfertas");
                return false;
            }

            _conexaoServiceBus = configuracao.ConexaoServiceBUS;
            _subscription = configuracao.IdOrganizacao;

            return true;
        }

        public async Task FinalizarMensageriaAsync(CancellationToken token)
        {
            if (!_inicializado && _client == null && _processadorDemanda == null && _processadorOferta == null && _processadorTransportador == null)
                return;

            _parando = true;

            await PararEFinalizar(_processadorDemanda, token).ConfigureAwait(false);
            await PararEFinalizar(_processadorOferta, token).ConfigureAwait(false);
            await PararEFinalizar(_processadorTransportador, token).ConfigureAwait(false);

            _processadorDemanda = null;
            _processadorOferta = null;
            _processadorTransportador = null;

            if (_client != null)
            {
                try { await _client.DisposeAsync().ConfigureAwait(false); } catch { }
                _client = null;
            }

            _inicializado = false;
            Interlocked.Exchange(ref _hookCancelamentoRegistrado, 0);
        }

        private static async Task PararEFinalizar(ServiceBusProcessor processor, CancellationToken token)
        {
            if (processor == null)
                return;

            try { await processor.StopProcessingAsync(token).ConfigureAwait(false); } catch { }
            try { await processor.DisposeAsync().ConfigureAwait(false); } catch { }
        }
    }
}