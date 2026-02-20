using Dominio.ObjetosDeValor.Embarcador.Integracao.Maxtrack;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Servicos.Embarcador.Monitoramento.Integracoes
{

    public class IntegracaoMaxtrack : Abstract.AbstractIntegracaoREST
    {
        #region Atributos privados

        private static IntegracaoMaxtrack Instance;
        private static readonly string nameConfigSection = "Maxtrack";
        Dominio.ObjetosDeValor.Embarcador.Logistica.ContaIntegracao conta;

        #endregion

        #region Atributos privados Positrion

        private int timeout = 0;
        private int maximoMensagensConsumidas = 10;

        #endregion

        #region Constantes com as chaves dos dados/configurações

        private const string KEY_TIMEOUT = "Timeout";
        private const string KEY_MAXIMO_MENSAGENS_CONSUMIDAS = "MaximoMensagensConsumidas";
        private const string KEY_FILA_POSICOES = "FilaPosicoes";

        #endregion

        #region Construtor privado

        private IntegracaoMaxtrack(AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente) : base(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Maxtrack, nameConfigSection, cliente) { }

        #endregion

        #region Métodos públicos

        /**
         * Implementação de Singleton
         */
        public static IntegracaoMaxtrack GetInstance(AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente)
        {
            if (Instance == null) Instance = new IntegracaoMaxtrack(cliente);
            return Instance;
        }

        #endregion

        #region Métodos abstratos obrigatórios

        /**
         * Complementa configurações gerais
         */
        override protected void ComplementarConfiguracoes()
        {
            try
            {
                this.timeout = int.Parse(ObterValorOpcao(KEY_TIMEOUT));
                this.maximoMensagensConsumidas = int.Parse(ObterValorOpcao(KEY_MAXIMO_MENSAGENS_CONSUMIDAS));
            }
            catch (Exception e)
            {
                Log($"Erro ao ComplementarConfiguracoes {e.Message}", 2);
            }
        }

        /**
         * Confirmação de parâmetros corretos, executada apenas uma vez
         */
        override protected void Validar()
        {

        }

        /**
         * Preparação para iniciar a execução, executada apenas uma vez
         */
        override protected void Preparar()
        {

        }

        /**
         * Execução principal de cada iteração da thread, repetida infinitamente
         */
        override protected void Executar(Dominio.ObjetosDeValor.Embarcador.Logistica.ContaIntegracao contaIntegracao)
        {
            this.conta = contaIntegracao;
            IntegrarPosicao();
        }

        #endregion

        #region Métodos privados

        /**
         * Executa a integração das posições, consultando nq fila e registrando na tabela T_POSICAO
         */
        private void IntegrarPosicao()
        {
            //Log($"Consultando posicoes", 2);
            //
            //// Busca as posições do WebService
            //List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoes = ObterPosicoes();
            //
            //// quando for fila e iremos escutar indefinidamente a fila deveremos iniciar a conexão aqui. IntegrarPossicaoFila  dentro do metodo que escuta a fila deveremos chamar o insere posições em lote.
            //// isso se aplica a filas cuja o volume de dados seja maior que nossa necessidade de leitura. pois caso o volume da fila seja baixo podemos seguir a mesma logica das demais integrações
            //// do contrario quando o volume da fila é maior que nossa capacidade de leitura teremos dois problemas ao continuar lendo da forma atual 
            //// problema 01 -> criar um while para ler ate que não seja fim de fila. neste caso o sistema não iria sair do while. 
            //// problema 02 -> criar leitura de um em um registro da fila mesmo que seja gravado em lote teremos um delay crescente 
            //
            //
            //// Registra as posições recebidas dos veículos
            //Log($"Integrando {posicoes.Count} posicoes", 2);
            //base.InserirPosicoes(posicoes);
            IntegrarPossicaoFila();
        }


        private void IntegrarPossicaoFila()
        {
            bool Erros = false;
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoes = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao>();
            List<string> messages = new List<string>();
            string urlFila = ObterUrlFila();
            TimeSpan timeoutTS = TimeSpan.FromSeconds(this.timeout);
            DateTime UltimoLote = DateTime.Now;
            int quantidadeMinimaDePosicoesParaAdicionar = ObterValorMonitorar("QuantidadeMinimaDePosicoesParaAdicionar")?.ToInt() ?? 1000;

            ConnectionFactory factory = new ConnectionFactory()
            {
                HostName = conta.Servidor,
                Port = Protocols.DefaultProtocol.DefaultPort,
                UserName = conta.Usuario,
                Password = conta.Senha,
                VirtualHost = "/",
                ContinuationTimeout = new TimeSpan(10, 0, 0, 0)
            };
            string Fila = conta.Nome;

            RabbitMQ.Client.IConnection connection = null;
            RabbitMQ.Client.IModel channel = null;
            try
            {
                connection = factory.CreateConnection();
                channel = connection.CreateModel();
                EventingBasicConsumer consumer = new EventingBasicConsumer(channel);
                string message = "";
                consumer.Received += (model, ea) =>
                {
                    byte[] body = ea.Body.ToArray();
                    message = Encoding.UTF8.GetString(body);
                    Integration integration = JsonConvert.DeserializeObject<Integration>(message);
                    try
                    {
                        if (integration != null && integration.newReportData != null)
                        {
                            string jsonResponse = string.Empty;
                            string stringsalvarLog = ObterValorMonitorar("SalvarLogResponse");
                            bool salvarLog = (!string.IsNullOrWhiteSpace(stringsalvarLog)) && bool.Parse(stringsalvarLog);
                            if (salvarLog)
                            {
                                jsonResponse = JsonConvert.SerializeObject(integration.newReportData);
                                LogNomeArquivo(jsonResponse, DateTime.Now, "ResponsePosicoes", 0, true);
                            }

                            foreach (PositionInfo p in integration.newReportData.positionInfo)
                            {
                                double latitude = converteLatLong(p.latitude);
                                double longitude = converteLatLong(p.longitude);
                                if (longitude != 0 && latitude != 0)
                                {
                                    posicoes.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao
                                    {
                                        Data = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(integration.newReportData.dateTime).AddHours(-3),
                                        DataVeiculo = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(integration.newReportData.dateTime).AddHours(-3),
                                        IDEquipamento = integration.deviceID.ToString(),
                                        Placa = "",
                                        Latitude = latitude,
                                        Longitude = longitude,
                                        Velocidade = integration.newReportData.telemetry.speed.gps,
                                        Ignicao = (integration.newReportData.flags.deviceInfo.ignitionWakesUp) ? 1 : 0,
                                        SensorTemperatura = false,
                                        Rastreador = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EnumTecnologiaRastreador.Maxtrack
                                    });
                                }
                                else
                                {
                                    Log($"Erro convertendo Latitude e longitude", 2);
                                }
                            }
                            if (posicoes.Count >= quantidadeMinimaDePosicoesParaAdicionar)
                            {
                                UltimoLote = DateTime.Now;
                                Log($"Consultando posicoes", 2);
                                base.InserirPosicoes(posicoes);
                                Log($"Integrando {posicoes.Count} posicoes", 2);
                                posicoes = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao>();
                                //base.unitOfWork.FlushAndClear();
                                //GC.Collect();
                                //GC.WaitForPendingFinalizers();
                                //GC.Collect();
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("ERRO ID: " + integration.deviceID + e.Message.ToString());
                        if (channel.IsOpen)
                            channel.Close();
                        if (connection.IsOpen)
                            connection.Close();
                        posicoes = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao>();
                        Erros = true;
                        throw;
                    }
                };
                channel.BasicConsume(queue: Fila, autoAck: true, consumer: consumer);

                while (UltimoLote.AddMinutes(5) > DateTime.Now) // caso a tecnologia deles ficar mais que 5 minutos fora  o fluxo seguirá finalizando nossa trad principal 
                {
                    if (Erros)
                    {
                        if (channel.IsOpen)
                            channel.Close();

                        if (connection.IsOpen)
                            connection.Close();
                        break;
                    }
                    if (channel.IsClosed || !connection.IsOpen)
                    {
                        if (channel.IsOpen)
                            channel.Close();
                        if (connection.IsOpen)
                            connection.Close();
                        break;
                    }
                    Thread.Sleep(30000);
                }
                if (connection.IsOpen)
                    connection.Close();
            }
            catch (Exception ex)
            {
                if (connection != null && connection.IsOpen)
                    connection.Close();
                posicoes = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao>();
                Console.WriteLine("ERRO FORA: " + ex.Message.ToString());
                throw;
            }
        }

        private double converteLatLong(double latLog)
        {
            try
            {
                return latLog / 10000000.0;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        /**
         * Busca as posições de todos os veículos
         */
        private List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> ObterPosicoes()
        {
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoes = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao>();
            List<string> messages = ConsumeMessages();
            int total = messages.Count;
            try
            {
                for (int i = 0; i < total; i++)
                {
                    Integration integration = JsonConvert.DeserializeObject<Integration>(messages[i]);
                    if (integration.newReportData != null)
                    {
                        Console.WriteLine("Device ID String: " + new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(integration.newReportData.dateTime));
                        foreach (PositionInfo p in integration.newReportData.positionInfo)
                        {
                            //Console.WriteLine("lati : " + p.latitude);Console.WriteLine("long : " + p.longitude);
                            // Obtem as posições da Maxtrack
                            posicoes.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao
                            {
                                Data = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(integration.newReportData.dateTime),
                                DataVeiculo = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(integration.newReportData.dateTime),
                                IDEquipamento = integration.deviceID.ToString(),
                                Placa = "",
                                Latitude = converteLatLong(p.latitude),
                                Longitude = converteLatLong(p.longitude),
                                Velocidade = integration.newReportData.telemetry.speed.gps,
                                Ignicao = (integration.newReportData.flags.deviceInfo.ignitionWakesUp) ? 1 : 0,
                                SensorTemperatura = false,
                                Rastreador = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EnumTecnologiaRastreador.Maxtrack
                            });
                        }
                    }
                    else
                    {
                        Console.WriteLine(" ***************** NULLO ******************** ");
                    }
                    
                }
            }
            catch (Exception ex)
            {
                Log("Erro ObterIntegrations " + ex.Message, 3);
            }
            return posicoes;
        }



        /**
         * Busca as posições de todos os veículos
         */
        /**
         * Consumir a fila de posições do broker
         */
        private List<string> ConsumeMessages()
        {
            List<string> messages = new List<string>();

            string urlFila = ObterUrlFila();
            TimeSpan timeoutTS = TimeSpan.FromSeconds(this.timeout);

            ConnectionFactory factory = new ConnectionFactory()
            {
                HostName = conta.Servidor,
                Port = Protocols.DefaultProtocol.DefaultPort,
                UserName = conta.Usuario,
                Password = conta.Senha,
                VirtualHost = "/",
                ContinuationTimeout = new TimeSpan(10, 0, 0, 0)
            };
            IConnection connection = factory.CreateConnection();
            IModel channel = connection.CreateModel();
            EventingBasicConsumer consumer = new EventingBasicConsumer(channel);
            bool continua = false;
            do
            {
                //  esse codigo deveria ser usado se mudasemos o serviço para ficar escutando indefinidamente: consumer.Received += (model, ea) =>{byte[] body = ea.Body.ToArray(); var message = Encoding.UTF8.GetString(body);messages.Add(message); }; channel.BasicConsume(queue: conta.Nome,autoAck: true,consumer: consumer);
                BasicGetResult msg = channel.BasicGet(conta.Nome, true);
                if (msg == null || msg.Body.ToArray().Length <= 0)
                    continua = false;
                else
                {
                    continua = true;
                    try
                    {
                        byte[] body = msg.Body.ToArray();
                        string message = Encoding.UTF8.GetString(body);
                        /*foreach (var h in msg.BasicProperties.Headers)
                        {
                            if (Encoding.UTF8.GetString(h.Value as byte[]) == "1" && Encoding.UTF8.GetString(h.Value as byte[]) == "y")
                            {
                                byte[] body = msg.Body.ToArray();
                            }
                        }*/
                    }
                    catch (Exception)
                    {

                    }
                }
            }
            while (continua);

            return messages;
        }

        private string ObterUrl()
        {
            string url = Servicos.Embarcador.Logistica.ContaIntegracao.ObterURL(this.conta);
            return "activemq:" + url;
        }

        private string ObterUrlFila()
        {
            string nomeFilePosicoes = ObterNomeFilaPosicoes();
            return "queue://" + nomeFilePosicoes;
        }

        private string ObterNomeFilaPosicoes()
        {
            string value = Servicos.Embarcador.Logistica.ContaIntegracao.ObterValorListaParametrosAdicionais(KEY_FILA_POSICOES, this.conta.ListaParametrosAdicionais);
            return value;
        }

        #endregion

    }
}
