using com.alianca.intercab.emp.doc.booking;
using Confluent.Kafka;
using Confluent.SchemaRegistry;
using System;
using System.Threading;
using Confluent.Kafka.SyncOverAsync;

using System.Text;

namespace Servicos.Embarcador.Integracao.Intercab
{
    public class IntegracaoBooking
    {
        #region Propriedades Privadas

        Repositorio.UnitOfWork _unitOfWork;
        AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware _tipoServicoMultisoftware;

        #endregion

        #region Construtores

        public IntegracaoBooking(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            _unitOfWork = unitOfWork;
            _tipoServicoMultisoftware = tipoServicoMultisoftware;
        }

        #endregion

        #region Métodos Públicos

        public void IntegrarConsumerEMP(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoEMP configuracaoIntegracaoIntegracaoEMP, CancellationToken cancellationToken)
        {
            Servicos.Log.TratarErro($"Iniciando integração Booking...");

#if DEBUG
            ConsumerConfig conf = new ConsumerConfig
            {
                GroupId = configuracaoIntegracaoIntegracaoEMP.StatusTopicRecebimentoIntegracaoVesselEMP == "A" ? configuracaoIntegracaoIntegracaoEMP.GroupIDRetina : configuracaoIntegracaoIntegracaoEMP.GroupIdEMP,
                BootstrapServers = configuracaoIntegracaoIntegracaoEMP.StatusTopicRecebimentoIntegracaoVesselEMP == "A" ? configuracaoIntegracaoIntegracaoEMP.BootstrapServerRetina : configuracaoIntegracaoIntegracaoEMP.BoostrapServersEMP,
                SecurityProtocol = configuracaoIntegracaoIntegracaoEMP.StatusTopicRecebimentoIntegracaoVesselEMP == "A" ? SecurityProtocol.SaslSsl : SecurityProtocol.SaslSsl,
                SaslMechanism = configuracaoIntegracaoIntegracaoEMP.StatusTopicRecebimentoIntegracaoVesselEMP == "A" ? SaslMechanism.ScramSha512 : SaslMechanism.Plain,
                SaslUsername = configuracaoIntegracaoIntegracaoEMP.StatusTopicRecebimentoIntegracaoVesselEMP == "A" ? configuracaoIntegracaoIntegracaoEMP.UsuarioServerRetina : configuracaoIntegracaoIntegracaoEMP.UsuarioEMP,
                SaslPassword = configuracaoIntegracaoIntegracaoEMP.StatusTopicRecebimentoIntegracaoVesselEMP == "A" ? configuracaoIntegracaoIntegracaoEMP.SenhaServerRetina : configuracaoIntegracaoIntegracaoEMP.SenhaEMP,
                SslEndpointIdentificationAlgorithm = configuracaoIntegracaoIntegracaoEMP.StatusTopicRecebimentoIntegracaoVesselEMP == "A" ? SslEndpointIdentificationAlgorithm.None : SslEndpointIdentificationAlgorithm.Https,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                SslCaLocation = "D:\\Certificado P12 Schema Registry Retina\\caCorreto.crt",
                SslKeyPassword = configuracaoIntegracaoIntegracaoEMP.StatusTopicRecebimentoIntegracaoVesselEMP == "A" ? configuracaoIntegracaoIntegracaoEMP.SenhaServerRetina : "",
                EnableAutoCommit = false
            };
            using (CachedSchemaRegistryClient schemaRegistry = new CachedSchemaRegistryClient(
                new SchemaRegistryConfig
                {
                    Url = configuracaoIntegracaoIntegracaoEMP.StatusTopicRecebimentoIntegracaoVesselEMP == "A" ? configuracaoIntegracaoIntegracaoEMP.URLSchemaRegistryRetina : configuracaoIntegracaoIntegracaoEMP.UrlSchemaRegistry,
                    BasicAuthCredentialsSource = configuracaoIntegracaoIntegracaoEMP.StatusTopicRecebimentoIntegracaoVesselEMP == "A" ? AuthCredentialsSource.UserInfo : AuthCredentialsSource.UserInfo,
                    BasicAuthUserInfo = configuracaoIntegracaoIntegracaoEMP.StatusTopicRecebimentoIntegracaoVesselEMP == "A" ? "" : configuracaoIntegracaoIntegracaoEMP.UsuarioSchemaRegistry + ":" + configuracaoIntegracaoIntegracaoEMP.SenhaSchemaRegistry,
                    SslKeystorePassword = configuracaoIntegracaoIntegracaoEMP.StatusTopicRecebimentoIntegracaoVesselEMP == "A" ? configuracaoIntegracaoIntegracaoEMP.SenhaSchemaRegistryRetina : "",
                    SslKeystoreLocation = "D:\\Certificado P12 Schema Registry Retina\\ca.p12"
                }))
#else
            ConsumerConfig conf = new ConsumerConfig
            {
                GroupId = configuracaoIntegracaoIntegracaoEMP.StatusTopicRecebimentoIntegracaoVesselEMP == "A" ? configuracaoIntegracaoIntegracaoEMP.GroupIDRetina : configuracaoIntegracaoIntegracaoEMP.GroupIdEMP,
                BootstrapServers = configuracaoIntegracaoIntegracaoEMP.StatusTopicRecebimentoIntegracaoVesselEMP == "A" ? configuracaoIntegracaoIntegracaoEMP.BootstrapServerRetina : configuracaoIntegracaoIntegracaoEMP.BoostrapServersEMP,
                SecurityProtocol = configuracaoIntegracaoIntegracaoEMP.StatusTopicRecebimentoIntegracaoVesselEMP == "A" ? SecurityProtocol.SaslSsl : SecurityProtocol.SaslSsl,
                SaslMechanism = configuracaoIntegracaoIntegracaoEMP.StatusTopicRecebimentoIntegracaoVesselEMP == "A" ? SaslMechanism.ScramSha512 : SaslMechanism.Plain,
                SaslUsername = configuracaoIntegracaoIntegracaoEMP.StatusTopicRecebimentoIntegracaoVesselEMP == "A" ? configuracaoIntegracaoIntegracaoEMP.UsuarioServerRetina : configuracaoIntegracaoIntegracaoEMP.UsuarioEMP,
                SaslPassword = configuracaoIntegracaoIntegracaoEMP.StatusTopicRecebimentoIntegracaoVesselEMP == "A" ? configuracaoIntegracaoIntegracaoEMP.SenhaServerRetina : configuracaoIntegracaoIntegracaoEMP.SenhaEMP,
                SslEndpointIdentificationAlgorithm = configuracaoIntegracaoIntegracaoEMP.StatusTopicRecebimentoIntegracaoVesselEMP == "A" ? SslEndpointIdentificationAlgorithm.None : SslEndpointIdentificationAlgorithm.Https,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                SslCaLocation = configuracaoIntegracaoIntegracaoEMP.StatusTopicRecebimentoIntegracaoBooking == "A" ? (configuracaoIntegracaoIntegracaoEMP.CertificadoCRTServerRetina?.NomeArquivo ?? "") : "",//"D:\\Certificado P12 Schema Registry Retina\\caCorreto.crt",
                SslKeyPassword = configuracaoIntegracaoIntegracaoEMP.StatusTopicRecebimentoIntegracaoVesselEMP == "A" ? configuracaoIntegracaoIntegracaoEMP.SenhaServerRetina : "",
                EnableAutoCommit = false
            };
            using (CachedSchemaRegistryClient schemaRegistry = new CachedSchemaRegistryClient(
                new SchemaRegistryConfig
                {
                    Url = configuracaoIntegracaoIntegracaoEMP.StatusTopicRecebimentoIntegracaoVesselEMP == "A" ? configuracaoIntegracaoIntegracaoEMP.URLSchemaRegistryRetina : configuracaoIntegracaoIntegracaoEMP.UrlSchemaRegistry,
                    BasicAuthCredentialsSource = configuracaoIntegracaoIntegracaoEMP.StatusTopicRecebimentoIntegracaoVesselEMP == "A" ? AuthCredentialsSource.UserInfo : AuthCredentialsSource.UserInfo,
                    BasicAuthUserInfo = configuracaoIntegracaoIntegracaoEMP.StatusTopicRecebimentoIntegracaoVesselEMP == "A" ? "" : configuracaoIntegracaoIntegracaoEMP.UsuarioSchemaRegistry + ":" + configuracaoIntegracaoIntegracaoEMP.SenhaSchemaRegistry,
                    SslKeystorePassword = configuracaoIntegracaoIntegracaoEMP.StatusTopicRecebimentoIntegracaoVesselEMP == "A" ? configuracaoIntegracaoIntegracaoEMP.SenhaSchemaRegistryRetina : "",
                    SslKeystoreLocation = configuracaoIntegracaoIntegracaoEMP.StatusTopicRecebimentoIntegracaoBooking == "A" ? (configuracaoIntegracaoIntegracaoEMP.CertificadoShemaRegistryRetina?.NomeArquivo ?? "") : ""//"D:\\Certificado P12 Schema Registry Retina\\ca.p12"
                }))
#endif
            using (IConsumer<string, IntercabDocBooking> c =
                    new ConsumerBuilder<string, IntercabDocBooking>(conf)
                        .SetKeyDeserializer(Deserializers.Utf8)
                        .SetValueDeserializer(new Confluent.SchemaRegistry.Serdes.AvroDeserializer<IntercabDocBooking>(schemaRegistry).AsSyncOverAsync())
                        .SetErrorHandler((_, e) => Servicos.Log.TratarErro($"Consume error: {e.Reason}. Code: {e.Code}. IsFatal: {e.IsFatal}. IsBrokerError: {e.IsBrokerError}."))
                        .SetLogHandler((_, l) => Servicos.Log.TratarErro(l.Message, "Booking"))
                        .Build())
            {
                c.Subscribe(configuracaoIntegracaoIntegracaoEMP.TopicBooking);
                Servicos.Log.TratarErro("Iniciando consumo topic " + configuracaoIntegracaoIntegracaoEMP.TopicBooking, "Booking");

                try
                {
                    while (true)
                    {
                        ConsumeResult<string, IntercabDocBooking> cr = c.Consume(cancellationToken);

                        if (configuracaoIntegracaoIntegracaoEMP?.AtivarLeituraHeaderBooking ?? false
                            && (!string.IsNullOrWhiteSpace(configuracaoIntegracaoIntegracaoEMP?.AtivarLeituraHeadersConsumoKeyEMP ?? ""))
                            && (!string.IsNullOrWhiteSpace(configuracaoIntegracaoIntegracaoEMP?.AtivarLeituraHeadersConsumoValueEMP ?? ""))
                            && cr.Message != null
                            && cr.Message.Headers != null)
                        {
                            Servicos.Log.TratarErro("Recebido integração com headers", "Booking");

                            if (cr.Message.Headers.TryGetLastBytes(configuracaoIntegracaoIntegracaoEMP?.AtivarLeituraHeadersConsumoKeyEMP ?? "", out byte[] header))
                            {
                                if (configuracaoIntegracaoIntegracaoEMP?.AtivarLeituraHeadersConsumoValueEMP == Encoding.UTF8.GetString(header))
                                {
                                    Servicos.Log.TratarErro("Realizando interpretação do header", "Booking");
                                    InterpretarArquivoBooking(cr.Message.Value, configuracaoIntegracaoIntegracaoEMP?.TopicBooking ?? "");
                                    c.Commit(cr);
                                    Servicos.Log.TratarErro("Realizou o commit da interpretação do header", "Booking");
                                }
                                else
                                {
                                    Servicos.Log.TratarErro("Header configurado " + configuracaoIntegracaoIntegracaoEMP?.AtivarLeituraHeadersConsumoValueEMP + " não bate com o recebido " + Encoding.UTF8.GetString(header), "Booking");
                                    c.Close();
                                }
                            }
                            else
                            {
                                Servicos.Log.TratarErro("Headers não localizado", "Booking");
                                c.Close();
                            }
                        }
                        else
                        {
                            Servicos.Log.TratarErro("Realizando interpretação sem header", "Booking");
                            InterpretarArquivoBooking(cr.Message.Value, configuracaoIntegracaoIntegracaoEMP?.TopicBooking ?? "");
                            c.Commit(cr);
                            Servicos.Log.TratarErro("Realizou o commit da interpretação sem header", "Booking");
                        }
                    }
                }
                catch (OperationCanceledException e)
                {
                    Servicos.Log.TratarErro(e, "Booking");
                    c.Close();
                }
                catch (Exception e)
                {
                    Servicos.Log.TratarErro(e, "Booking");
					c.Commit();
					c.Close();
                }
            }
        }

        #endregion

        #region Métodos Privados

        private void InterpretarArquivoBooking(string strBooking, string topic)
        {
            IntercabDocBooking booking = Newtonsoft.Json.JsonConvert.DeserializeObject<IntercabDocBooking>(strBooking);

            Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado()
            {
                TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Sistema,
                Texto = "Integração EMP",
                OrigemAuditado = Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.Sistema
            };
            new Servicos.Embarcador.Integracao.EMP.IntegracaoEMP(_unitOfWork).RecebimentoBooking(topic, booking, out string msgRetorno, auditado, _tipoServicoMultisoftware);
        }

        private void InterpretarArquivoBooking(IntercabDocBooking booking, string topic)
        {
            Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado()
            {
                TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Sistema,
                Texto = "Integração EMP",
                OrigemAuditado = Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.Sistema
            };
            new Servicos.Embarcador.Integracao.EMP.IntegracaoEMP(_unitOfWork).RecebimentoBooking(topic, booking, out string msgRetorno, auditado, _tipoServicoMultisoftware);
        }

        #endregion
    }
}
