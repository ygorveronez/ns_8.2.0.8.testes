using com.alianca.intercab.emp.doc.booking;
using Confluent.Kafka;
using Confluent.SchemaRegistry;
using System;
using System.Threading;
using Confluent.Kafka.SyncOverAsync;
using System.Text;
using com.schedule.dto;

namespace Servicos.Embarcador.Integracao.Intercab
{
    public class IntegracaoSchedule
    {
        #region Propriedades Privadas

        Repositorio.UnitOfWork _unitOfWork;
        AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware _tipoServicoMultisoftware;

        #endregion

        #region Construtores

        public IntegracaoSchedule(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            _unitOfWork = unitOfWork;
            _tipoServicoMultisoftware = tipoServicoMultisoftware;
        }

        #endregion

        #region Métodos Públicos

        public void IntegrarConsumerEMP(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoEMP configuracaoIntegracaoEMP, CancellationToken cancellationToken)
        {
            Servicos.Log.TratarErro($"Iniciando integração Schedule...");

#if DEBUG
            ConsumerConfig conf = new ConsumerConfig
            {
                GroupId = configuracaoIntegracaoEMP.StatusTopicRecebimentoIntegracaoScheduleEMP == "A" ? configuracaoIntegracaoEMP.GroupIDRetina : configuracaoIntegracaoEMP.GroupIdEMP,
                BootstrapServers = configuracaoIntegracaoEMP.StatusTopicRecebimentoIntegracaoScheduleEMP == "A" ? configuracaoIntegracaoEMP.BootstrapServerRetina : configuracaoIntegracaoEMP.BoostrapServersEMP,
                SecurityProtocol = configuracaoIntegracaoEMP.StatusTopicRecebimentoIntegracaoScheduleEMP == "A" ? SecurityProtocol.SaslSsl : SecurityProtocol.SaslSsl,
                SaslMechanism = configuracaoIntegracaoEMP.StatusTopicRecebimentoIntegracaoScheduleEMP == "A" ? SaslMechanism.ScramSha512 : SaslMechanism.Plain,
                SaslUsername = configuracaoIntegracaoEMP.StatusTopicRecebimentoIntegracaoScheduleEMP == "A" ? configuracaoIntegracaoEMP.UsuarioServerRetina : configuracaoIntegracaoEMP.UsuarioEMP,
                SaslPassword = configuracaoIntegracaoEMP.StatusTopicRecebimentoIntegracaoScheduleEMP == "A" ? configuracaoIntegracaoEMP.SenhaServerRetina : configuracaoIntegracaoEMP.SenhaEMP,
                SslEndpointIdentificationAlgorithm = configuracaoIntegracaoEMP.StatusTopicRecebimentoIntegracaoScheduleEMP == "A" ? SslEndpointIdentificationAlgorithm.None : SslEndpointIdentificationAlgorithm.Https,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                SslCaLocation = "D:\\Certificado P12 Schema Registry Retina\\caCorreto.crt",
                SslKeyPassword = configuracaoIntegracaoEMP.StatusTopicRecebimentoIntegracaoScheduleEMP == "A" ? configuracaoIntegracaoEMP.SenhaServerRetina : "",
                EnableAutoCommit = false
            };
            using (CachedSchemaRegistryClient schemaRegistry = new CachedSchemaRegistryClient(
                new SchemaRegistryConfig
                {
                    Url = configuracaoIntegracaoEMP.StatusTopicRecebimentoIntegracaoScheduleEMP == "A" ? configuracaoIntegracaoEMP.URLSchemaRegistryRetina : configuracaoIntegracaoEMP.UrlSchemaRegistry,
                    BasicAuthCredentialsSource = configuracaoIntegracaoEMP.StatusTopicRecebimentoIntegracaoScheduleEMP == "A" ? AuthCredentialsSource.UserInfo : AuthCredentialsSource.UserInfo,
                    BasicAuthUserInfo = configuracaoIntegracaoEMP.StatusTopicRecebimentoIntegracaoScheduleEMP == "A" ? "" : configuracaoIntegracaoEMP.UsuarioSchemaRegistry + ":" + configuracaoIntegracaoEMP.SenhaSchemaRegistry,
                    SslKeystorePassword = configuracaoIntegracaoEMP.StatusTopicRecebimentoIntegracaoScheduleEMP == "A" ? configuracaoIntegracaoEMP.SenhaSchemaRegistryRetina : "",
                    SslKeystoreLocation = "D:\\Certificado P12 Schema Registry Retina\\ca.p12"
                }))
#else
            ConsumerConfig conf = new ConsumerConfig
            {
                GroupId = configuracaoIntegracaoEMP.StatusTopicRecebimentoIntegracaoScheduleEMP == "A" ? configuracaoIntegracaoEMP.GroupIDRetina : configuracaoIntegracaoEMP.GroupIdEMP,
                BootstrapServers = configuracaoIntegracaoEMP.StatusTopicRecebimentoIntegracaoScheduleEMP == "A" ? configuracaoIntegracaoEMP.BootstrapServerRetina : configuracaoIntegracaoEMP.BoostrapServersEMP,
                SecurityProtocol = configuracaoIntegracaoEMP.StatusTopicRecebimentoIntegracaoScheduleEMP == "A" ? SecurityProtocol.SaslSsl : SecurityProtocol.SaslSsl,
                SaslMechanism = configuracaoIntegracaoEMP.StatusTopicRecebimentoIntegracaoScheduleEMP == "A" ? SaslMechanism.ScramSha512 : SaslMechanism.Plain,
                SaslUsername = configuracaoIntegracaoEMP.StatusTopicRecebimentoIntegracaoScheduleEMP == "A" ? configuracaoIntegracaoEMP.UsuarioServerRetina : configuracaoIntegracaoEMP.UsuarioEMP,
                SaslPassword = configuracaoIntegracaoEMP.StatusTopicRecebimentoIntegracaoScheduleEMP == "A" ? configuracaoIntegracaoEMP.SenhaServerRetina : configuracaoIntegracaoEMP.SenhaEMP,
                SslEndpointIdentificationAlgorithm = configuracaoIntegracaoEMP.StatusTopicRecebimentoIntegracaoScheduleEMP == "A" ? SslEndpointIdentificationAlgorithm.None : SslEndpointIdentificationAlgorithm.Https,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                SslCaLocation = configuracaoIntegracaoEMP.StatusTopicRecebimentoIntegracaoBooking == "A" ? (configuracaoIntegracaoEMP.CertificadoCRTServerRetina?.NomeArquivo ?? "") : "",//"D:\\Certificado P12 Schema Registry Retina\\caCorreto.crt",
                SslKeyPassword = configuracaoIntegracaoEMP.StatusTopicRecebimentoIntegracaoScheduleEMP == "A" ? configuracaoIntegracaoEMP.SenhaServerRetina : "",
                EnableAutoCommit = false
            };
            using (CachedSchemaRegistryClient schemaRegistry = new CachedSchemaRegistryClient(
                new SchemaRegistryConfig
                {
                    Url = configuracaoIntegracaoEMP.StatusTopicRecebimentoIntegracaoScheduleEMP == "A" ? configuracaoIntegracaoEMP.URLSchemaRegistryRetina : configuracaoIntegracaoEMP.UrlSchemaRegistry,
                    BasicAuthCredentialsSource = configuracaoIntegracaoEMP.StatusTopicRecebimentoIntegracaoScheduleEMP == "A" ? AuthCredentialsSource.UserInfo : AuthCredentialsSource.UserInfo,
                    BasicAuthUserInfo = configuracaoIntegracaoEMP.StatusTopicRecebimentoIntegracaoScheduleEMP == "A" ? "" : configuracaoIntegracaoEMP.UsuarioSchemaRegistry + ":" + configuracaoIntegracaoEMP.SenhaSchemaRegistry,
                    SslKeystorePassword = configuracaoIntegracaoEMP.StatusTopicRecebimentoIntegracaoScheduleEMP == "A" ? configuracaoIntegracaoEMP.SenhaSchemaRegistryRetina : "",
                    SslKeystoreLocation = configuracaoIntegracaoEMP.StatusTopicRecebimentoIntegracaoBooking == "A" ? (configuracaoIntegracaoEMP.CertificadoShemaRegistryRetina?.NomeArquivo ?? "") : ""//"D:\\Certificado P12 Schema Registry Retina\\ca.p12"
                }))
#endif
            using (IConsumer<string, ScheduleEvent> c =
                    new ConsumerBuilder<string, ScheduleEvent>(conf)
                        .SetKeyDeserializer(Deserializers.Utf8)
                        .SetValueDeserializer(new Confluent.SchemaRegistry.Serdes.AvroDeserializer<ScheduleEvent>(schemaRegistry).AsSyncOverAsync())
                        .SetErrorHandler((_, e) => Servicos.Log.TratarErro($"Consume error: {e.Reason}. Code: {e.Code}. IsFatal: {e.IsFatal}. IsBrokerError: {e.IsBrokerError}."))
                        .SetLogHandler((_, l) => Servicos.Log.TratarErro(l.Message, "Schedule"))
                        .Build())
            {
                c.Subscribe(configuracaoIntegracaoEMP.TopicRecebimentoIntegracaoScheduleEMP);
                Servicos.Log.TratarErro("Iniciando consumo topic " + configuracaoIntegracaoEMP.TopicRecebimentoIntegracaoScheduleEMP, "Schedule");

                try
                {
                    while (true)
                    {
                        ConsumeResult<string, ScheduleEvent> cr = c.Consume(cancellationToken);

                        if (configuracaoIntegracaoEMP?.AtivarLeituraHeaderSchedule ?? false
                            && (!string.IsNullOrWhiteSpace(configuracaoIntegracaoEMP?.AtivarLeituraHeadersConsumoKeyEMP ?? ""))
                            && (!string.IsNullOrWhiteSpace(configuracaoIntegracaoEMP?.AtivarLeituraHeadersConsumoValueEMP ?? ""))
                            && cr.Message != null
                            && cr.Message.Headers != null)
                        {
                            Servicos.Log.TratarErro("Recebido integração com headers", "Schedule");

                            if (cr.Message.Headers.TryGetLastBytes(configuracaoIntegracaoEMP?.AtivarLeituraHeadersConsumoKeyEMP ?? "", out byte[] header))
                            {
                                if (configuracaoIntegracaoEMP?.AtivarLeituraHeadersConsumoValueEMP == Encoding.UTF8.GetString(header))
                                {
                                    Servicos.Log.TratarErro("Realizando interpretação do header", "Schedule");
                                    InterpretarArquivoSchedule(cr.Message.Value, configuracaoIntegracaoEMP?.TopicRecebimentoIntegracaoScheduleEMP ?? "");
                                    c.Commit(cr);
                                    Servicos.Log.TratarErro("Realizou o commit da interpretação do header", "Schedule");
                                }
                                else
                                {
                                    Servicos.Log.TratarErro("Header configurado " + configuracaoIntegracaoEMP?.AtivarLeituraHeadersConsumoValueEMP + " não bate com o recebido " + Encoding.UTF8.GetString(header), "Schedule");
                                    c.Close();
                                }
                            }
                            else
                            {
                                Servicos.Log.TratarErro("Headers não localizado", "Schedule");
                                c.Close();
                            }
                        }
                        else
                        {
                            Servicos.Log.TratarErro("Realizando interpretação sem header", "Schedule");
                            InterpretarArquivoSchedule(cr.Message.Value, configuracaoIntegracaoEMP?.TopicRecebimentoIntegracaoScheduleEMP ?? "");
                            c.Commit(cr);
                            Servicos.Log.TratarErro("Realizou o commit da interpretação sem header", "Schedule");
                        }
                    }
                }
                catch (OperationCanceledException e)
                {
                    Servicos.Log.TratarErro(e, "Schedule");
                    c.Close();
                }
                catch (Exception e)
                {
                    Servicos.Log.TratarErro(e, "Schedule");
					c.Commit();
					c.Close();
                }
            }
        }

        #endregion

        #region Métodos Privados

        private void InterpretarArquivoSchedule(string strSchedule, string topic)
        {
            ScheduleEvent schedule = Newtonsoft.Json.JsonConvert.DeserializeObject<ScheduleEvent>(strSchedule);

            Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado()
            {
                TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Sistema,
                Texto = "Integração EMP",
                OrigemAuditado = Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.Sistema
            };
            new Servicos.Embarcador.Integracao.EMP.IntegracaoEMP(_unitOfWork).RecebimentoSchedule(topic, schedule, out string msgRetorno, auditado);
        }

        private void InterpretarArquivoSchedule(ScheduleEvent schedule, string topic)
        {
            Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado()
            {
                TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Sistema,
                Texto = "Integração EMP",
                OrigemAuditado = Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.Sistema
            };
            new Servicos.Embarcador.Integracao.EMP.IntegracaoEMP(_unitOfWork).RecebimentoSchedule(topic, schedule, out string msgRetorno, auditado);
        }

        #endregion
    }
}
