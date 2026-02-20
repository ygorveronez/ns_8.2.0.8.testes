using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.ServiceModel.Channels;

namespace Servicos.Embarcador.Integracao
{
    public class ConfiguracaoWebService
    {
        #region Propriedades 

        private readonly Repositorio.UnitOfWork _unitOfWork;
        private readonly Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado _auditado;

        #endregion

        #region Construtores

        public ConfiguracaoWebService(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado()
            {
                OrigemAuditado = Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.Sistema,
                TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Sistema,
            };
        }

        public ConfiguracaoWebService(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            _unitOfWork = unitOfWork;
            _auditado = auditado;
        }

        #endregion

        #region Metodos Publicos

        public T ObterClient<T, TChannel>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao tipoWebServiceIntegracao) where TChannel : class where T : System.ServiceModel.ClientBase<TChannel>, new()
        {
            Repositorio.Embarcador.Integracao.ConfiguracaoWebServiceIntegracao repConfiguracaoWebService = new Repositorio.Embarcador.Integracao.ConfiguracaoWebServiceIntegracao(_unitOfWork);

            Dominio.Entidades.Embarcador.Integracao.ConfiguracaoWebServiceIntegracao configuracaoWebService = repConfiguracaoWebService.BuscarPorTipo(tipoWebServiceIntegracao);

            T client = null;

            if (configuracaoWebService == null)
            {
                try
                {
                    client = (T)Activator.CreateInstance(typeof(T));

                    configuracaoWebService = new Dominio.Entidades.Embarcador.Integracao.ConfiguracaoWebServiceIntegracao()
                    {
                        Tipo = tipoWebServiceIntegracao,
                        Endpoint = client.Endpoint.Address.ToString(),
                    };

                    switch (client.Endpoint.Binding)
                    {
                        case System.ServiceModel.BasicHttpBinding:
                            configuracaoWebService.TipoBinding = TipoWebServiceIntegracaoBinding.BasicHttpBinding;
                            configuracaoWebService.ConfiguracaoBinding = ObterConfiguracaoBasicHttpBinding((System.ServiceModel.BasicHttpBinding)client.Endpoint.Binding);
                            break;

                        case System.ServiceModel.WSHttpBinding:
                            configuracaoWebService.TipoBinding = TipoWebServiceIntegracaoBinding.WsHttpBinding;
                            configuracaoWebService.ConfiguracaoBinding = ObterConfiguracaoWsHttpBinding((System.ServiceModel.WSHttpBinding)client.Endpoint.Binding);
                            break;

                        case System.ServiceModel.Channels.CustomBinding:
                            configuracaoWebService.TipoBinding = TipoWebServiceIntegracaoBinding.CustomBinding;
                            configuracaoWebService.ConfiguracaoBinding = ObterConfiguracaoCustomBinding((System.ServiceModel.Channels.CustomBinding)client.Endpoint.Binding);
                            break;
                    }

                    repConfiguracaoWebService.Inserir(configuracaoWebService, _auditado);
                }
                catch (Exception excecao)
                {
                    Servicos.Log.TratarErro(excecao, "ConfiguracaoWebService");
                    throw new ServicoException("Integração não configurada no ambiente! Necessário solicitar a configuração para a equipe de desenvolvimento.");
                }
            }
            else
            {
                System.ServiceModel.EndpointAddress endpointAddress = new System.ServiceModel.EndpointAddress(configuracaoWebService.Endpoint);
                System.ServiceModel.Channels.Binding binding = ObterBinding(configuracaoWebService);

                //SemPararValePedagio.ValePedagioClient valePedagioClient = new SemPararValePedagio.ValePedagioClient(binding, endpointAddress);
                //valePedagioClient.autenticarUsuario("2022184763", "ADMINISTRADOR", "Helena@2022");

                client = (T)Activator.CreateInstance(typeof(T), binding, endpointAddress);
            }

            return client;
        }

        public T ObterClient<T, TChannel>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao tipoWebServiceIntegracao, string endpointAddress) where TChannel : class where T : System.ServiceModel.ClientBase<TChannel>, new()
        {
            T client = ObterClient<T, TChannel>(tipoWebServiceIntegracao);

            client.Endpoint.Address = new System.ServiceModel.EndpointAddress(endpointAddress);

            return client;
        }

        public T ObterClient<T, TChannel>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao tipoWebServiceIntegracao, out Servicos.Models.Integracao.InspectorBehavior inspectorBehavior) where TChannel : class where T : System.ServiceModel.ClientBase<TChannel>, new()
        {
            inspectorBehavior = new Models.Integracao.InspectorBehavior();

            T client = ObterClient<T, TChannel>(tipoWebServiceIntegracao);

            client.Endpoint.EndpointBehaviors.Add(inspectorBehavior);

            return client;
        }

        public T ObterClient<T, TChannel>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao tipoWebServiceIntegracao, string endpointAddress, out Servicos.Models.Integracao.InspectorBehavior inspectorBehavior) where TChannel : class where T : System.ServiceModel.ClientBase<TChannel>, new()
        {
            T client = ObterClient<T, TChannel>(tipoWebServiceIntegracao, out inspectorBehavior);

            client.Endpoint.Address = new System.ServiceModel.EndpointAddress(endpointAddress);

            return client;
        }

        #endregion

        #region Metodos Privados

        private string ObterConfiguracaoBasicHttpBinding(System.ServiceModel.BasicHttpBinding basicHttpBinding)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.ConfiguracaoBasicHttpBinding config = new Dominio.ObjetosDeValor.Embarcador.Integracao.ConfiguracaoBasicHttpBinding()
            {
                CloseTimeout = basicHttpBinding.CloseTimeout,
                OpenTimeout = basicHttpBinding.OpenTimeout,
                MaxReceivedMessageSize = basicHttpBinding.MaxReceivedMessageSize,
                ReceiveTimeout = basicHttpBinding.ReceiveTimeout,
                SendTimeout = basicHttpBinding.SendTimeout
            };

            if (basicHttpBinding.Security != null)
            {
                config.Security = new Dominio.ObjetosDeValor.Embarcador.Integracao.ConfiguracaoBasicHttpBindingSecurity
                {
                    Mode = basicHttpBinding.Security.Mode.ToString()
                };

                if (basicHttpBinding.Security.Transport != null)
                {
                    config.Security.Transport = new Dominio.ObjetosDeValor.Embarcador.Integracao.ConfiguracaoHttpTransportSecurity()
                    {
                        ClientCredentialType = basicHttpBinding.Security.Transport.ClientCredentialType.ToString(),
                        ProxyCredentialType = basicHttpBinding.Security.Transport.ProxyCredentialType.ToString(),
                        //Realm = basicHttpBinding.Security.Transport.Realm
                    };
                }
            }

            return JsonConvert.SerializeObject(config);
        }

        private string ObterConfiguracaoWsHttpBinding(System.ServiceModel.WSHttpBinding wsHttpBinding)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.ConfiguracaoWsHttpBinding config = new Dominio.ObjetosDeValor.Embarcador.Integracao.ConfiguracaoWsHttpBinding()
            {
                CloseTimeout = wsHttpBinding.CloseTimeout,
                OpenTimeout = wsHttpBinding.OpenTimeout,
                MaxReceivedMessageSize = wsHttpBinding.MaxReceivedMessageSize,
                ReceiveTimeout = wsHttpBinding.ReceiveTimeout,
                SendTimeout = wsHttpBinding.SendTimeout
            };

            if (wsHttpBinding.Security != null)
            {
                config.Security = new Dominio.ObjetosDeValor.Embarcador.Integracao.ConfiguracaoWsHttpBindingSecurity
                {
                    Mode = wsHttpBinding.Security.Mode.ToString()
                };
            }

            return JsonConvert.SerializeObject(config);
        }

        private string ObterConfiguracaoCustomBinding(System.ServiceModel.Channels.CustomBinding customBinding)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.ConfiguracaoCustomBinding config = new Dominio.ObjetosDeValor.Embarcador.Integracao.ConfiguracaoCustomBinding()
            {
                CloseTimeout = customBinding.CloseTimeout,
                MessageVersion = customBinding.MessageVersion.ToString(),
                Name = customBinding.Name,
                Namespace = customBinding.Namespace,
                OpenTimeout = customBinding.OpenTimeout,
                ReceiveTimeout = customBinding.ReceiveTimeout,
                SendTimeout = customBinding.SendTimeout
            };

            foreach (System.ServiceModel.Channels.BindingElement element in customBinding.Elements)
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.ConfiguracaoCustomBindingElement bindingElementValue = new Dominio.ObjetosDeValor.Embarcador.Integracao.ConfiguracaoCustomBindingElement
                {
                    ElementType = element.GetType().Name
                };

                foreach (PropertyInfo property in element.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    if (property.CanRead)
                    {
                        object value = property.GetValue(element);
                        bindingElementValue.Properties[property.Name] = value;
                    }
                }

                config.Elements.Add(bindingElementValue);
            }

            return JsonConvert.SerializeObject(config);
        }

        private System.ServiceModel.Channels.Binding ObterBinding(Dominio.Entidades.Embarcador.Integracao.ConfiguracaoWebServiceIntegracao configuracaoWebService)
        {
            if (configuracaoWebService?.TipoBinding == null
                || string.IsNullOrWhiteSpace(configuracaoWebService.ConfiguracaoBinding))
                return null;

            switch (configuracaoWebService.TipoBinding.Value)
            {
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracaoBinding.BasicHttpBinding:
                    return ObterBasicHttpBinding(ObterConfiguracao<Dominio.ObjetosDeValor.Embarcador.Integracao.ConfiguracaoBasicHttpBinding>(configuracaoWebService.ConfiguracaoBinding));

                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracaoBinding.WsHttpBinding:
                    return ObterWsHttpBinding(ObterConfiguracao<Dominio.ObjetosDeValor.Embarcador.Integracao.ConfiguracaoWsHttpBinding>(configuracaoWebService.ConfiguracaoBinding));

                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracaoBinding.CustomBinding:
                    return ObterCustomBinding(ObterConfiguracao<Dominio.ObjetosDeValor.Embarcador.Integracao.ConfiguracaoCustomBinding>(configuracaoWebService.ConfiguracaoBinding));
            }

            return null;
        }

        private T ObterConfiguracao<T>(string configuracaoJson)
        {
            return JsonConvert.DeserializeObject<T>(configuracaoJson);
        }

        private System.ServiceModel.BasicHttpBinding ObterBasicHttpBinding(Dominio.ObjetosDeValor.Embarcador.Integracao.ConfiguracaoBasicHttpBinding configuracaoBasicHttpBinding)
        {
            System.ServiceModel.BasicHttpBinding basicHttpBinding = new System.ServiceModel.BasicHttpBinding();

            basicHttpBinding.SendTimeout = configuracaoBasicHttpBinding.SendTimeout;
            basicHttpBinding.MaxReceivedMessageSize = configuracaoBasicHttpBinding.MaxReceivedMessageSize;
            basicHttpBinding.ReceiveTimeout = configuracaoBasicHttpBinding.ReceiveTimeout;

            if (configuracaoBasicHttpBinding.Security != null)
            {
                basicHttpBinding.Security.Mode = configuracaoBasicHttpBinding.Security.Mode.ToEnum(System.ServiceModel.BasicHttpSecurityMode.None);

                if (configuracaoBasicHttpBinding.Security.Transport != null)
                {
                    basicHttpBinding.Security.Transport = new System.ServiceModel.HttpTransportSecurity()
                    {
                        ClientCredentialType = configuracaoBasicHttpBinding.Security.Transport.ClientCredentialType.ToEnum(System.ServiceModel.HttpClientCredentialType.None),
                        ProxyCredentialType = configuracaoBasicHttpBinding.Security.Transport.ProxyCredentialType.ToEnum(System.ServiceModel.HttpProxyCredentialType.None),
                        //Realm = configuracaoBasicHttpBinding.Security.Transport.Realm
                    };
                }
            }

            return basicHttpBinding;
        }

        private System.ServiceModel.WSHttpBinding ObterWsHttpBinding(Dominio.ObjetosDeValor.Embarcador.Integracao.ConfiguracaoWsHttpBinding configuracaoWsHttpBinding)
        {
            System.ServiceModel.WSHttpBinding wsHttpBinding = new System.ServiceModel.WSHttpBinding();

            wsHttpBinding.SendTimeout = configuracaoWsHttpBinding.SendTimeout;
            wsHttpBinding.MaxReceivedMessageSize = configuracaoWsHttpBinding.MaxReceivedMessageSize;
            wsHttpBinding.ReceiveTimeout = configuracaoWsHttpBinding.ReceiveTimeout;

            if (configuracaoWsHttpBinding.Security != null)
            {
                wsHttpBinding.Security.Mode = configuracaoWsHttpBinding.Security.Mode.ToEnum(System.ServiceModel.SecurityMode.None);
            }

            return wsHttpBinding;
        }

        private System.ServiceModel.Channels.CustomBinding ObterCustomBinding(Dominio.ObjetosDeValor.Embarcador.Integracao.ConfiguracaoCustomBinding configuracaoCustomBinding)
        {
            System.ServiceModel.Channels.CustomBinding customBinding = new System.ServiceModel.Channels.CustomBinding()
            {
                Namespace = configuracaoCustomBinding.Namespace,
                Name = configuracaoCustomBinding.Name,
                OpenTimeout = configuracaoCustomBinding.OpenTimeout,
                CloseTimeout = configuracaoCustomBinding.CloseTimeout,
                SendTimeout = configuracaoCustomBinding.SendTimeout,
                ReceiveTimeout = configuracaoCustomBinding.ReceiveTimeout
            };

            foreach (Dominio.ObjetosDeValor.Embarcador.Integracao.ConfiguracaoCustomBindingElement elementValue in configuracaoCustomBinding.Elements)
            {
                Type elementType = Type.GetType($"System.ServiceModel.Channels.{elementValue.ElementType}, System.ServiceModel.Http");
                if (elementType == null)
                    elementType = Type.GetType($"System.ServiceModel.Channels.{elementValue.ElementType}, System.ServiceModel.Primitives");

                if (elementType != null)
                {
                    object element = Activator.CreateInstance(elementType);

                    foreach (KeyValuePair<string, object> property in elementValue.Properties)
                    {
                        PropertyInfo propInfo = elementType.GetProperty(property.Key);

                        if (propInfo != null && propInfo.CanWrite && !propInfo.PropertyType.IsAbstract && property.Value != null)
                        {
                            try
                            {
                                if (propInfo.PropertyType == typeof(MessageVersion))
                                {
                                    MessageVersion messageVersion = ConvertStringToMessageVersion(property.Value.ToString());
                                    propInfo.SetValue(element, messageVersion);
                                }
                                else
                                {
                                    if (property.Value is object)
                                        propInfo.SetValue(element, JsonConvert.DeserializeObject(JsonConvert.SerializeObject(property.Value), propInfo.PropertyType));
                                    else
                                        propInfo.SetValue(element, Convert.ChangeType(property.Value, propInfo.PropertyType));
                                }
                            }
                            catch (Exception ex)
                            {
                                Servicos.Log.TratarErro("Falha ao carregar o customBinding: " + ex, "ConfiguracaoWebService");
                            }
                        }
                    }

                    customBinding.Elements.Add((BindingElement)element);
                }
            }

            return customBinding;
        }
        public static MessageVersion ConvertStringToMessageVersion(string versionString)
        {
            switch (versionString)
            {
                case "Soap12WSAddressing10":
                    return MessageVersion.Soap12WSAddressing10;
                case "Soap12WSAddressingAugust2004":
                    return MessageVersion.Soap12WSAddressingAugust2004;
                case "Soap11":
                    return MessageVersion.Soap11;
                default:
                    return MessageVersion.Soap12WSAddressing10;
            }
        }
        #endregion
    }
}
