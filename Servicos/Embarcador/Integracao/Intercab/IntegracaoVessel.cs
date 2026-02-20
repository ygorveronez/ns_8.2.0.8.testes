using com.maersk.vessel.smds.operations.MSK;
using Confluent.Kafka;
using Confluent.SchemaRegistry;
using System;
using System.Threading;
using Confluent.Kafka.SyncOverAsync;
using System.Text;

namespace Servicos.Embarcador.Integracao.Intercab
{
    public class IntegracaoVessel
    {
        #region Propriedades Privadas

        Repositorio.UnitOfWork _unitOfWork;

        #endregion

        #region Construtores

        public IntegracaoVessel(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Métodos Públicos

        public void IntegrarConsumerEMP(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoEMP configuracaoIntegracaoIntegracaoEMP, CancellationToken cancellationToken)
        {
            Servicos.Log.TratarErro($"Iniciando integração Vessel...");

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
                GroupId = configuracaoIntegracaoIntegracaoEMP.StatusTopicRecebimentoIntegracaoVesselEMP == "A" ? !string.IsNullOrWhiteSpace(configuracaoIntegracaoIntegracaoEMP.ConsumerGroupVessel) ? configuracaoIntegracaoIntegracaoEMP.ConsumerGroupVessel : configuracaoIntegracaoIntegracaoEMP.GroupIDRetina : configuracaoIntegracaoIntegracaoEMP.GroupIdEMP,
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
            using (IConsumer<string, vesselMessage> c =
                    new ConsumerBuilder<string, vesselMessage>(conf)
                        .SetKeyDeserializer(Deserializers.Utf8)
                        .SetValueDeserializer(new Confluent.SchemaRegistry.Serdes.AvroDeserializer<vesselMessage>(schemaRegistry).AsSyncOverAsync())
                        .SetErrorHandler((_, e) => Servicos.Log.TratarErro($"Consume error: {e.Reason}. Code: {e.Code}. IsFatal: {e.IsFatal}. IsBrokerError: {e.IsBrokerError}."))
                        .SetLogHandler((_, l) => Servicos.Log.TratarErro(l.Message, "Vessel"))
                        .Build())
            {
                c.Subscribe(configuracaoIntegracaoIntegracaoEMP.TopicRecebimentoIntegracaoVesselEMP);

                try
                {
                    while (true)
                    {
                        Servicos.Log.TratarErro("Aguardando consumo do Vessel", "Vessel");
                        ConsumeResult<string, vesselMessage> cr = c.Consume(cancellationToken);

                        if (configuracaoIntegracaoIntegracaoEMP?.AtivarLeituraHeaderVessel?? false
                           && (!string.IsNullOrWhiteSpace(configuracaoIntegracaoIntegracaoEMP?.AtivarLeituraHeadersConsumoKeyEMP ?? ""))
                           && (!string.IsNullOrWhiteSpace(configuracaoIntegracaoIntegracaoEMP?.AtivarLeituraHeadersConsumoValueEMP ?? ""))
                           && cr.Message != null
                           && cr.Message.Headers != null)
                        {
                            if (cr.Message.Headers.TryGetLastBytes(configuracaoIntegracaoIntegracaoEMP?.AtivarLeituraHeadersConsumoKeyEMP ?? "", out byte[] header))
                            {
                                if (configuracaoIntegracaoIntegracaoEMP?.AtivarLeituraHeadersConsumoValueEMP == Encoding.UTF8.GetString(header))
                                {
                                    InterpretarArquivoVessel(cr.Message.Value, configuracaoIntegracaoIntegracaoEMP?.TopicRecebimentoIntegracaoVesselEMP ?? "");
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
                            Servicos.Log.TratarErro("Realizando interpretação do Vessel", "Vessel");
                            InterpretarArquivoVessel(cr.Message.Value, configuracaoIntegracaoIntegracaoEMP?.TopicRecebimentoIntegracaoVesselEMP ?? "");
                            c.Commit(cr);
                            Servicos.Log.TratarErro("Realizou o commit do Vessel", "Vessel");
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

        private void InterpretarArquivoVessel(vesselMessage vessel, string topic)
        {
            Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado()
            {
                TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Sistema,
                Texto = "Integração EMP",
                OrigemAuditado = Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.Sistema
            };
            new Servicos.Embarcador.Integracao.EMP.IntegracaoEMP(_unitOfWork).RecebimentoNavio(topic, vessel, out string msgRetorno, auditado);
        }

        #endregion
    }
}
