using AdminMultisoftware.Dominio.Enumeradores;
using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using Dominio.MSMQ;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Notificacao;
using MSMQ.Messaging;
using Newtonsoft.Json;
using Servicos.Embarcador.Notificacao;
using Servicos.SignalR;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Servicos.MSMQ
{
    public class MSMQ
    {
        private static Dominio.MSMQ.MSMQQueue _GlobalMSMQQueueListener;

        public static void SendPrivateMessage(Dominio.MSMQ.Notification notification, string prefixoMSMQ = null)
        {
            string conectionString = ObterConectionStringFila();
            if (conectionString == "MSMQ")
                SendPrivateMessageMSMQ(notification, prefixoMSMQ);
            else
                SendPrivateMessageBUS(notification, conectionString, prefixoMSMQ);

        }

        public static void ListenerPrivateMessage(Repositorio.UnitOfWork unitOfWork, Dominio.MSMQ.MSMQQueue msmqQueue, string stringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, int codigoClienteMultisoftware)
        {
            if (unitOfWork == null)
                unitOfWork = new Repositorio.UnitOfWork(stringConexao);

            string conectionString = ObterConectionStringFila();
            if (conectionString == "MSMQ")
            {
                Servicos.Log.GravarInfo("Listener MSMQ", "MessageQueue");
                ListenerPrivateMessageMSMQ(unitOfWork, msmqQueue, stringConexao, tipoServicoMultisoftware, codigoClienteMultisoftware);
            }
            else
            {
                Servicos.Log.GravarInfo("Listener ServiceBUS", "MessageQueue");
                ListenerPrivateMessageBUS(unitOfWork, msmqQueue, stringConexao, tipoServicoMultisoftware, codigoClienteMultisoftware);
            }
        }
        private static void SendPrivateMessageMSMQ(Dominio.MSMQ.Notification notification, string prefixoMSMQ = null)
        {
            try
            {
                using (MessageQueue messageQueue = new MessageQueue())
                {
                    string codigoCliente = "";
                    if (notification.MSMQQueue != MSMQQueue.SGTMobile)
                        codigoCliente = notification.ClientMultisoftwareID + "_";

                    string enderecoDoComputadorFila = ObterEnderecoComputadorFila();

                    if (string.IsNullOrWhiteSpace(prefixoMSMQ))
                        prefixoMSMQ = ObterPrefixoMSMQ(null);

                    messageQueue.Path = enderecoDoComputadorFila + @"\private$\" + prefixoMSMQ + codigoCliente + notification.MSMQQueue.GetDescription();

                    // Não é possível verificar a existências de filas privadas em outros computadores
                    if (enderecoDoComputadorFila == ".")
                    {
                        PathValidate(messageQueue);
                    }

                    Message sendMessage = new Message();
                    sendMessage.Body = JsonConvert.SerializeObject(notification);
                    messageQueue.Send(sendMessage);
                }
            }
            catch (MessageQueueException ex)
            {
                Servicos.Log.TratarErro(ex, "MessageQueue");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "MessageQueue");
            }
        }

        private static void ListenerPrivateMessageMSMQ(Repositorio.UnitOfWork unitOfWork, Dominio.MSMQ.MSMQQueue msmqQueue, string stringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, int codigoClienteMultisoftware)
        {
            using (MessageQueue messageQueue = new MessageQueue())
            {
                _GlobalMSMQQueueListener = msmqQueue;

                SignalRConnection.GetInstance().SetInstance(stringConexao, tipoServicoMultisoftware, codigoClienteMultisoftware);

                string codigoCliente = codigoClienteMultisoftware > 0 ? (codigoClienteMultisoftware.ToString() + "_") : "";

                messageQueue.Path = @".\private$\" + ObterPrefixoMSMQ(unitOfWork) + codigoCliente + msmqQueue.GetDescription();

                PathValidate(messageQueue);

                messageQueue.Formatter = new XmlMessageFormatter(new Type[] { typeof(String) });

                messageQueue.ReceiveCompleted += new ReceiveCompletedEventHandler(ReceiveCompleted);

                messageQueue.BeginReceive();
            }
        }

        private static void SendPrivateMessageBUS(Dominio.MSMQ.Notification notification, string conectionString, string prefixoMSMQ = null)
        {
            try
            {
                string codigoCliente = "";
                if (notification.MSMQQueue != MSMQQueue.SGTMobile)
                    codigoCliente = notification.ClientMultisoftwareID + "_";

                if (string.IsNullOrWhiteSpace(prefixoMSMQ))
                    prefixoMSMQ = ObterPrefixoMSMQ(null);

                string nomeFila = $"{prefixoMSMQ}{codigoCliente}{notification.MSMQQueue.GetDescription()}";

                //if (enderecoDoComputadorFila == ".")
                //    PathValidate(nomeFila);

                Servicos.Log.GravarInfo($"nomeFila SendPrivateMessageBUS: {nomeFila}", "MessageQueue");


                ServiceBusSender sender = new ServiceBusClient(conectionString).CreateSender(nomeFila);
                try
                {
                    string jsonMessage = JsonConvert.SerializeObject(notification);
                    byte[] body = Encoding.UTF8.GetBytes(jsonMessage);
                    ServiceBusMessage message = new ServiceBusMessage(body);
                    sender.SendMessageAsync(message).Wait();
                    //sender.SendMessageAsync(new ServiceBusMessage(Encoding.UTF8.GetBytes("teste1"))).Wait();


                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex, "MessageQueue");
                }
            }
            catch (MessageQueueException ex)
            {
                Servicos.Log.TratarErro(ex, "MessageQueue");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "MessageQueue");
            }
        }

        private static void ListenerPrivateMessageBUS(Repositorio.UnitOfWork unitOfWork, Dominio.MSMQ.MSMQQueue msmqQueue, string stringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, int codigoClienteMultisoftware)
        {
            string codigoCliente = codigoClienteMultisoftware > 0 ? (codigoClienteMultisoftware.ToString() + "_") : "";
            string nomeFila = $"{ObterPrefixoMSMQ(unitOfWork)}{codigoCliente}{msmqQueue.GetDescription()}";

            PathValidate(nomeFila);
            Servicos.Log.GravarInfo($"nomeFila ServiceBus: {nomeFila}", "MessageQueue");
            ServiceBusClient client = new ServiceBusClient(ObterConectionStringFila());
            ServiceBusReceiver receiver = client.CreateReceiver(nomeFila);

            Task.Run(async () =>
            {
                while (true)
                {
                    try
                    {
                        ServiceBusReceivedMessage message = await receiver.ReceiveMessageAsync();
                        if (message != null)
                        {
                            ReceiveCompleted_(message);
                            await receiver.CompleteMessageAsync(message);
                        }
                        else
                        {
                            await Task.Delay(1000);
                        }
                    }
                    catch (Exception e)
                    {
                        Servicos.Log.TratarErro(e, "MessageQueue");
                    }
                }
            });
        }


        private static string ObterPrefixoMSMQ(Repositorio.UnitOfWork unitOfWork)
        {
            string prefixoMSMQ = "";
            if (unitOfWork == null)
                prefixoMSMQ = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance()?.ObterConfiguracaoAmbiente().PrefixoMSMQ;
            else
            {
                Repositorio.Embarcador.Configuracoes.ConfiguracaoAmbiente rep = new Repositorio.Embarcador.Configuracoes.ConfiguracaoAmbiente(unitOfWork);
                prefixoMSMQ = rep.BuscarConfiguracaoPadrao()?.PrefixoMSMQ ?? "";
#if DEBUG
                prefixoMSMQ = $"LOCAL_{prefixoMSMQ}";
#endif
            }

            if (string.IsNullOrWhiteSpace(prefixoMSMQ))
                prefixoMSMQ = string.Empty;

            return prefixoMSMQ;
        }

        private static string ObterEnderecoComputadorFila()
        {
            // Por padrão é ".", que é o computador local
            string endereco = ".";
            if (!string.IsNullOrWhiteSpace(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance()?.ObterConfiguracaoAmbiente().EnderecoComputadorRemotoFila))
            {
                // Esse endereço pode ser o nome da máquina ou o endereço de IP no formato "FormatName:Direct=TCP:XXX.XXX.XXX.XXX"
                endereco = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance()?.ObterConfiguracaoAmbiente().EnderecoComputadorRemotoFila;
            }

            return endereco;
        }

        private static void ReceiveCompleted_(ServiceBusReceivedMessage message)
        {
            try
            {
                string strMessage = message?.Body?.ToString() ?? "";

                if (!string.IsNullOrEmpty(strMessage))
                {
                    Dominio.MSMQ.Notification notification = JsonConvert.DeserializeObject<Dominio.MSMQ.Notification>(strMessage);
                    if (notification.Hub == Dominio.SignalR.Hubs.NotificacaoOneSignal)
                    {
                        string stringConexao = notification.Content.stringConexao;
                        string stringAdminConexao = notification.Content.stringAdminConexao;

                        Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);
                        Repositorio.Usuario repMotorista = new Repositorio.Usuario(unitOfWork);
                        NotificacaoOneSignal serNotificacaoOneSignal = new NotificacaoOneSignal(unitOfWork);

                        int codigoMotorista = notification.Content.codigoMotorista;
                        Dominio.Entidades.Usuario motorista = repMotorista.BuscarPorCodigo(codigoMotorista);
                        MobileHubs? tipo = (MobileHubs?)notification.Content.tipo;
                        OneSignalHeadings headings = new OneSignalHeadings(
                            pt: (string)notification.Content.headings.pt,
                            en: (string)notification.Content.headings.en,
                            es: (string)notification.Content.headings.es
                        );
                        OneSignalContents contents = new OneSignalContents(
                            pt: (string)notification.Content.contents.pt,
                            en: (string)notification.Content.contents.en,
                            es: (string)notification.Content.contents.es
                        );
                        OneSignalData data = new OneSignalData
                        {
                            CodigoCarga = (int?)notification.Content.data.CodigoCarga,
                            CodigoCargaEntrega = (int?)notification.Content.data.CodigoCargaEntrega,
                            Observacao = (string)notification.Content.data.Observacao,
                            SituacaoCargaEntrega = (SituacaoEntrega?)notification.Content.data.SituacaoCargaEntrega,

                            // Chamadods
                            CodigoChamado = (int?)notification.Content.data.CodigoChamado,
                            NomeAnalistaChamado = (string)notification.Content.data.NomeAnalistaChamado,
                            SituacaoChamado = (SituacaoChamado?)notification.Content.data.SituacaoChamado,
                            DiferencaDevolucao = (bool?)notification.Content.data.DiferencaDevolucao,
                        };

                        AdminMultisoftware.Repositorio.UnitOfWork adminUnitOfWork = new AdminMultisoftware.Repositorio.UnitOfWork(stringAdminConexao);
                        if (tipo.HasValue)
                        {
                            serNotificacaoOneSignal.EnviarNotificacaoSync(motorista, tipo.Value, headings, contents, data, adminUnitOfWork);
                        }
                        else
                        {
                            Log.TratarErro("Erro: MobileHubs não foi convertido");
                        }
                        adminUnitOfWork.Dispose();
                    }
                    else
                    {
                        Servicos.SignalR.Mobile.ProcessHubNotification(notification);
                    }
                }
            }
            catch (MessageQueueException ex)
            {
                Servicos.Log.TratarErro(ex, "MessageQueue");
                if (ex.MessageQueueErrorCode == MessageQueueErrorCode.QueueDeleted)
                {
                    ListenerPrivateMessage(null, _GlobalMSMQQueueListener, SignalRConnection.GetInstance().ConnectionString, SignalRConnection.GetInstance().TipoServicoMultisoftware, SignalRConnection.GetInstance().CodigoClienteMultisoftware);
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "MessageQueue");
            }
            finally
            {
            }
        }

        private static void ReceiveCompleted(Object source, ReceiveCompletedEventArgs asyncResult)
        {
            MessageQueue sourceQueue = null;
            try
            {
                sourceQueue = (MessageQueue)source;
                Message receiveMessage = sourceQueue.EndReceive(asyncResult.AsyncResult);

                string strMessage = receiveMessage?.Body?.ToString() ?? "";

                if (!string.IsNullOrEmpty(strMessage))
                {
                    Dominio.MSMQ.Notification notification = JsonConvert.DeserializeObject<Dominio.MSMQ.Notification>(strMessage);
                    if (notification.Hub == Dominio.SignalR.Hubs.NotificacaoOneSignal)
                    {
                        string stringConexao = notification.Content.stringConexao;
                        string stringAdminConexao = notification.Content.stringAdminConexao;

                        Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);
                        Repositorio.Usuario repMotorista = new Repositorio.Usuario(unitOfWork);
                        NotificacaoOneSignal serNotificacaoOneSignal = new NotificacaoOneSignal(unitOfWork);

                        int codigoMotorista = notification.Content.codigoMotorista;
                        Dominio.Entidades.Usuario motorista = repMotorista.BuscarPorCodigo(codigoMotorista);
                        MobileHubs? tipo = (MobileHubs?)notification.Content.tipo;
                        OneSignalHeadings headings = new OneSignalHeadings(
                            pt: (string)notification.Content.headings.pt,
                            en: (string)notification.Content.headings.en,
                            es: (string)notification.Content.headings.es
                        );
                        OneSignalContents contents = new OneSignalContents(
                            pt: (string)notification.Content.contents.pt,
                            en: (string)notification.Content.contents.en,
                            es: (string)notification.Content.contents.es
                        );
                        OneSignalData data = new OneSignalData
                        {
                            CodigoCarga = (int?)notification.Content.data.CodigoCarga,
                            CodigoCargaEntrega = (int?)notification.Content.data.CodigoCargaEntrega,
                            Observacao = (string)notification.Content.data.Observacao,
                            SituacaoCargaEntrega = (SituacaoEntrega?)notification.Content.data.SituacaoCargaEntrega,

                            // Chamadods
                            CodigoChamado = (int?)notification.Content.data.CodigoChamado,
                            NomeAnalistaChamado = (string)notification.Content.data.NomeAnalistaChamado,
                            SituacaoChamado = (SituacaoChamado?)notification.Content.data.SituacaoChamado,
                            DiferencaDevolucao = (bool?)notification.Content.data.DiferencaDevolucao,
                        };

                        AdminMultisoftware.Repositorio.UnitOfWork adminUnitOfWork = new AdminMultisoftware.Repositorio.UnitOfWork(stringAdminConexao);
                        if (tipo.HasValue)
                        {
                            serNotificacaoOneSignal.EnviarNotificacaoSync(motorista, tipo.Value, headings, contents, data, adminUnitOfWork);
                        }
                        else
                        {
                            Log.TratarErro("Erro: MobileHubs não foi convertido");
                        }
                        adminUnitOfWork.Dispose();
                    }
                    else
                    {
                        Servicos.SignalR.Mobile.ProcessHubNotification(notification);
                    }
                }

                receiveMessage.Dispose();
            }
            catch (MessageQueueException ex)
            {
                Servicos.Log.TratarErro(ex, "MessageQueue");
                if (ex.MessageQueueErrorCode == MessageQueueErrorCode.QueueDeleted)
                {
                    ListenerPrivateMessage(null, _GlobalMSMQQueueListener, SignalRConnection.GetInstance().ConnectionString, SignalRConnection.GetInstance().TipoServicoMultisoftware, SignalRConnection.GetInstance().CodigoClienteMultisoftware);
                    sourceQueue = null;
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "MessageQueue");
            }
            finally
            {
                sourceQueue?.BeginReceive();
            }
        }

        private static void PathValidate(MessageQueue messageQueue)
        {
            if (!MessageQueue.Exists(messageQueue.Path))
                MessageQueue.Create(messageQueue.Path);
        }

        private static void PathValidate(string nomeFila)
        {
            ServiceBusAdministrationClient adminClient = new ServiceBusAdministrationClient(ObterConectionStringFila());
            if (!adminClient.QueueExistsAsync(nomeFila).Result.Value)
                adminClient.CreateQueueAsync(nomeFila).Wait();
        }

        private static string ObterConectionStringFila()
        {
            string EndpointServiceFila = "";
            if (string.IsNullOrWhiteSpace(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance()?.ObterConfiguracaoAmbiente().EndpointServiceFila))
                EndpointServiceFila = "Endpoint=sb://sb-multi-prod.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=1lCef7DZZVA9PhszRvIQnlfb9e5f2uEqX+ASbI6osJA="; //  por padrao vai para fila de produção 
            else
                EndpointServiceFila = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance()?.ObterConfiguracaoAmbiente().EndpointServiceFila;
            // HML  "Endpoint=sb://sb-multi-dev.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=NSxZDGeUrZuGTQ9zVpBp+mEb2I8oUVB+5+ASbIYHkaI=";
            // prod "Endpoint=sb://sb-multi-prod.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=1lCef7DZZVA9PhszRvIQnlfb9e5f2uEqX+ASbI6osJA=";
            return EndpointServiceFila;
        }
    }
}