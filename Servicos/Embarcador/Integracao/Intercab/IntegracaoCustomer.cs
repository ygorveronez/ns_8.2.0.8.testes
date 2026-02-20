using com.maersk.customer.smds.commercial.msk;
using Confluent.Kafka;
using Confluent.SchemaRegistry;
using System;
using System.Text;
using System.Threading;
using Confluent.Kafka.SyncOverAsync;

namespace Servicos.Embarcador.Integracao.Intercab
{
    public class IntegracaoCustomer
    {
        #region Propriedades Privadas

        Repositorio.UnitOfWork _unitOfWork;

        #endregion

        #region Construtores

        public IntegracaoCustomer(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Métodos Públicos

        public void IntegrarConsumerEMP(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoEMP configuracaoIntegracaoIntegracaoEMP, CancellationToken cancellationToken)
        {
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
            using (var c =
                    new ConsumerBuilder<string, CustomerMessage>(conf)
                        .SetKeyDeserializer(Deserializers.Utf8)
                        .SetValueDeserializer(new Confluent.SchemaRegistry.Serdes.AvroDeserializer<CustomerMessage>(schemaRegistry).AsSyncOverAsync())
                        .SetErrorHandler((_, e) => Console.WriteLine($"Error: {e.Reason}"))
                        .Build())
            {

                c.Subscribe(configuracaoIntegracaoIntegracaoEMP.TopicRecebimentoIntegracaoCustomerEMP);

                try
                {
                    try
                    {
                        while (true)
                        {
                            ConsumeResult<string, CustomerMessage> cr = c.Consume(cancellationToken);

                            if (configuracaoIntegracaoIntegracaoEMP?.AtivarLeituraHeaderCustomer ?? false
                               && (!string.IsNullOrWhiteSpace(configuracaoIntegracaoIntegracaoEMP?.AtivarLeituraHeadersConsumoKeyEMP ?? ""))
                               && (!string.IsNullOrWhiteSpace(configuracaoIntegracaoIntegracaoEMP?.AtivarLeituraHeadersConsumoValueEMP ?? ""))
                               && cr.Message != null
                               && cr.Message.Headers != null)
                            {
                                if (cr.Message.Headers.TryGetLastBytes(configuracaoIntegracaoIntegracaoEMP?.AtivarLeituraHeadersConsumoKeyEMP ?? "", out byte[] header))
                                {
                                    if (configuracaoIntegracaoIntegracaoEMP?.AtivarLeituraHeadersConsumoValueEMP == Encoding.UTF8.GetString(header))
                                    {
                                        InterpretarArquivoCustomer(cr.Message.Value, configuracaoIntegracaoIntegracaoEMP?.TopicRecebimentoIntegracaoCustomerEMP ?? "");
                                        c.Commit(cr);
                                    }
                                    else
                                        c.Close();
                                } else
                                    c.Close();
                            }
                            else
                            {
                                InterpretarArquivoCustomer(cr.Message.Value, configuracaoIntegracaoIntegracaoEMP?.TopicRecebimentoIntegracaoCustomerEMP ?? "");

                                c.Commit(cr);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Servicos.Log.TratarErro(e);
                    }
                }
                catch (OperationCanceledException e)
                {
                    Servicos.Log.TratarErro(e);
					c.Commit();
					c.Close();
                }
            }
        }

        #endregion

        #region Métodos Privados

        private void InterpretarArquivoCustomer(CustomerMessage customer, string topic)
        {
            Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado()
            {
                TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Sistema,
                Texto = "Integração EMP",
                OrigemAuditado = Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.Sistema
            };
            new Servicos.Embarcador.Integracao.EMP.IntegracaoEMP(_unitOfWork).RecebimentoPessoa(topic, customer, out string msgRetorno, auditado);
        }

        #endregion
    }
}
