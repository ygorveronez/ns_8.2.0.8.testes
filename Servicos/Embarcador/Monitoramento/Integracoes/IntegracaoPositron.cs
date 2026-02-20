using Apache.NMS.Util;
using Apache.NMS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace Servicos.Embarcador.Monitoramento.Integracoes
{

    public class IntegracaoPositron : Abstract.AbstractIntegracaoREST
    {
        #region Atributos privados

        private static IntegracaoPositron Instance;
        private static readonly string nameConfigSection = "Positron";
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

        private IntegracaoPositron(AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente) : base(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Positron, nameConfigSection, cliente) { }

        #endregion

        #region Métodos públicos

        /**
         * Implementação de Singleton
         */
        public static IntegracaoPositron GetInstance(AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente)
        {
            if (Instance == null) Instance = new IntegracaoPositron(cliente);
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
            Log($"Consultando posicoes", 2);

            // Busca as posições do WebService
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoes = ObterPosicoes();

            // Registra as posições recebidas dos veículos
            Log($"Integrando {posicoes.Count} posicoes", 2);
            base.InserirPosicoes(posicoes);

        }

        /**
         * Busca as posições de todos os veículos
         */
        private List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> ObterPosicoes()
        {
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoes = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao>();
            try
            {
                // Obtem as posições da Positron
                List<Dominio.ObjetosDeValor.Embarcador.Integracao.Positron.Integration> integrations = ObterIntegrations();
                int total = integrations.Count;
                Log($"Recebidas {total} posicoes", 3);
                for (int i = 0; i < total; i++)
                {
                    posicoes.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao
                    {
                        Data = integrations[i].serverTime,
                        DataVeiculo = integrations[i].moduleTime,
                        IDEquipamento = integrations[i].idVeic.ToString(),
                        Placa = integrations[i].vehicle?.plate,
                        Latitude = integrations[i].latitude,
                        Longitude = integrations[i].longitude,
                        Velocidade = integrations[i].speed,
                        Ignicao = (integrations[i].ignition) ? 1 : 0,
                        SensorTemperatura = false,
                        Rastreador = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EnumTecnologiaRastreador.Positron
                    });
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
        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.Positron.Integration> ObterIntegrations()
        {
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.Positron.Integration> integrations = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Positron.Integration>();

            // Busca as posições na fila ActiveMQ
            List<string> messages = ConsumeMessages();
            int total = messages.Count;
            for (int i = 0; i < total; i++)
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Dominio.ObjetosDeValor.Embarcador.Integracao.Positron.Integration));
                StringReader stringReader = new StringReader(messages[i]);
                Dominio.ObjetosDeValor.Embarcador.Integracao.Positron.Integration integration = (Dominio.ObjetosDeValor.Embarcador.Integracao.Positron.Integration)serializer.Deserialize(stringReader);
                integrations.Add(integration);
            }

            return integrations;
        }

        /**
         * Consumir a fila de posições do broker
         */
        private List<string> ConsumeMessages()
        {
            List<string> messages = new List<string>();

            string url = ObterUrl();
            string urlFila = ObterUrlFila();
            TimeSpan timeoutTS = TimeSpan.FromSeconds(this.timeout);
            Uri uri = new Uri(url);

            NMSConnectionFactory factory = new NMSConnectionFactory(uri);
            using (IConnection connection = factory.CreateConnection(conta.Usuario, conta.Senha))
            using (ISession session = connection.CreateSession())
            {
                IDestination destination = SessionUtil.GetDestination(session, urlFila);
                using (IMessageConsumer consumer = session.CreateConsumer(destination))
                {
                    connection.Start();

                    Log($"Consumindo fila {urlFila}", 4);
                    while (true)
                    {
                        ITextMessage message = consumer.Receive(timeoutTS) as ITextMessage;
                        if (message != null) messages.Add(message.Text);
                        else break;

                        if (messages.Count == this.maximoMensagensConsumidas) break;
                    }
                    Log($"Recebidas {messages.Count} mensagens", 4);
                }
            }
            

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
