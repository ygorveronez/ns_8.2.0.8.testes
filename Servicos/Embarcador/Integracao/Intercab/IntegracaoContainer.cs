using Confluent.Kafka;
using Confluent.SchemaRegistry;
using System;
using System.Threading;
using Confluent.Kafka.SyncOverAsync;
using Alianca.PushService.Domain.Models.Avro;
using System.Text;

namespace Servicos.Embarcador.Integracao.Intercab
{
    public class IntegracaoContainer
    {
        #region Propriedades Privadas

        Repositorio.UnitOfWork _unitOfWork;
        AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware _tipoServicoMultisoftware;

        #endregion

        #region Construtores

        public IntegracaoContainer(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            _unitOfWork = unitOfWork;
            _tipoServicoMultisoftware = tipoServicoMultisoftware;
        }

        #endregion

        #region Métodos Públicos

        public void IntegrarConsumerEMP(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoEMP configuracaoIntegracaoIntegracaoEMP, CancellationToken cancellationToken)
        {
            Servicos.Log.TratarErro($"Iniciando integração Container...");

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
            using (IConsumer<string, ContainerViagem> c =
                    new ConsumerBuilder<string, ContainerViagem>(conf)
                        .SetKeyDeserializer(Deserializers.Utf8)
                        .SetValueDeserializer(new Confluent.SchemaRegistry.Serdes.AvroDeserializer<ContainerViagem>(schemaRegistry).AsSyncOverAsync())
                        .SetErrorHandler((_, e) => Servicos.Log.TratarErro($"Consume error: {e.Reason}. Code: {e.Code}. IsFatal: {e.IsFatal}. IsBrokerError: {e.IsBrokerError}."))
                        .SetLogHandler((_, l) => Servicos.Log.TratarErro(l.Message, "Booking"))
                        .Build())
            {
                c.Subscribe(configuracaoIntegracaoIntegracaoEMP.TopicEnvioIntegracaoContainerEMP);

                try
                {
                    while (true)
                    {
                        ConsumeResult<string, ContainerViagem> cr = c.Consume(cancellationToken);

                        if ((!string.IsNullOrWhiteSpace(configuracaoIntegracaoIntegracaoEMP?.AtivarLeituraHeadersConsumoKeyEMP ?? ""))
                           && (!string.IsNullOrWhiteSpace(configuracaoIntegracaoIntegracaoEMP?.AtivarLeituraHeadersConsumoValueEMP ?? ""))
                           && cr.Message != null
                           && cr.Message.Headers != null)
                        {
                            if (cr.Message.Headers.TryGetLastBytes(configuracaoIntegracaoIntegracaoEMP?.AtivarLeituraHeadersConsumoKeyEMP ?? "", out byte[] header))
                            {
                                if (configuracaoIntegracaoIntegracaoEMP?.AtivarLeituraHeadersConsumoValueEMP == Encoding.UTF8.GetString(header))
                                {
                                    InterpretarArquivoContainer(cr.Message.Value, configuracaoIntegracaoIntegracaoEMP?.TopicEnvioIntegracaoContainerEMP ?? "");
                                    c.Commit(cr);
                                }
                                else
                                    c.Close();
                            }
                            else
                                c.Close();
                        }
                        else
                        {
                            InterpretarArquivoContainer(cr.Message.Value, configuracaoIntegracaoIntegracaoEMP?.TopicEnvioIntegracaoContainerEMP ?? "");
                            c.Commit(cr);
                        }
                    }
                }
                catch (OperationCanceledException e)
                {
                    Servicos.Log.TratarErro(e);
                    c.Close();
                }
                catch (Exception e)
                {
                    Servicos.Log.TratarErro(e);
					c.Commit();
					c.Close();
                }
            }
        }

        public void IntegrarConsumerNFTPEMP(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoEMP configuracaoIntegracaoIntegracaoEMP, CancellationToken cancellationToken)
        {
            Servicos.Log.TratarErro($"Iniciando integração Container...");

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
            using (IConsumer<string, ContainerViagem> c =
                    new ConsumerBuilder<string, ContainerViagem>(conf)
                        .SetKeyDeserializer(Deserializers.Utf8)
                        .SetValueDeserializer(new Confluent.SchemaRegistry.Serdes.AvroDeserializer<ContainerViagem>(schemaRegistry).AsSyncOverAsync())
                        .SetErrorHandler((_, e) => Servicos.Log.TratarErro($"Consume error: {e.Reason}. Code: {e.Code}. IsFatal: {e.IsFatal}. IsBrokerError: {e.IsBrokerError}."))
                        .SetLogHandler((_, l) => Servicos.Log.TratarErro(l.Message, "Booking"))
                        .Build())
            {
                c.Subscribe(configuracaoIntegracaoIntegracaoEMP.TopicEnvioIntegracaoNFTPEMP);

                try
                {
                    while (true)
                    {
                        ConsumeResult<string, ContainerViagem> cr = c.Consume(cancellationToken);

                        if ((!string.IsNullOrWhiteSpace(configuracaoIntegracaoIntegracaoEMP?.AtivarLeituraHeadersConsumoKeyEMP ?? ""))
                           && (!string.IsNullOrWhiteSpace(configuracaoIntegracaoIntegracaoEMP?.AtivarLeituraHeadersConsumoValueEMP ?? ""))
                           && cr.Message != null
                           && cr.Message.Headers != null)
                        {
                            if (cr.Message.Headers.TryGetLastBytes(configuracaoIntegracaoIntegracaoEMP?.AtivarLeituraHeadersConsumoKeyEMP ?? "", out byte[] header))
                            {
                                if (configuracaoIntegracaoIntegracaoEMP?.AtivarLeituraHeadersConsumoValueEMP == Encoding.UTF8.GetString(header))
                                {
                                    InterpretarArquivoContainer(cr.Message.Value, configuracaoIntegracaoIntegracaoEMP?.TopicEnvioIntegracaoNFTPEMP ?? "");
                                    c.Commit(cr);
                                }
                                else
                                    c.Close();
                            }
                            else
                                c.Close();
                        }
                        else
                        {
                            InterpretarArquivoContainer(cr.Message.Value, configuracaoIntegracaoIntegracaoEMP?.TopicEnvioIntegracaoNFTPEMP ?? "");
                            c.Commit(cr);
                        }
                    }
                }
                catch (OperationCanceledException e)
                {
                    Servicos.Log.TratarErro(e);
                    c.Close();
                }
                catch (Exception e)
                {
                    Servicos.Log.TratarErro(e);
                    c.Commit();
                    c.Close();
                }
            }
        }

        #endregion

        #region Métodos Privados

        private void InterpretarArquivoContainer(ContainerViagem container, string topic)
        {
            Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado()
            {
                TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Sistema,
                Texto = "Integração EMP",
                OrigemAuditado = Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.Sistema
            };
            new Servicos.Embarcador.Integracao.EMP.IntegracaoEMP(_unitOfWork).RecebimentoContainer(topic, container, out string msgRetorno, auditado, _tipoServicoMultisoftware);
        }

        #endregion
    }
}
