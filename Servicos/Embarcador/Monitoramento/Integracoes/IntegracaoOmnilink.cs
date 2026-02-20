using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Xml.Serialization;

namespace Servicos.Embarcador.Monitoramento.Integracoes
{
    public class IntegracaoOmnilink : Abstract.AbstractIntegracaoSOAP
    {
        #region Atributos privados

        private static IntegracaoOmnilink Instance;
        private static readonly string nameConfigSection = "Omnilink";
        Dominio.ObjetosDeValor.Embarcador.Logistica.ContaIntegracao conta;

        #endregion

        #region Atributos privados específicos para a Omnilink

        private Servicos.Omnilink.ASWSSoapClient omnilinkWSClient;
        private Servicos.OmnilinkInterno.ASWSSoapClient omnilinkWSClientInterno;

        private long ultimoIDSequencialParaIniciar;
        private int segundosParaAguardarEntreRequisicoes;
        private long ultimoSequencialEventoNormal;
        private long ultimoSequencialEventoCtrl;

        #endregion

        #region Constantes com as chaves dos dados/configurações

        private const string KEY_ULTIMO_SEQUENCIAL_EVENTO_NORMAL = "UltimoSequencialEventoNormal";
        private const string KEY_ULTIMO_SEQUENCIAL_EVENTO_CTRL = "UltimoSequencialEventoCtrl";
        private const string KEY_CONF_ULTIMO_ID_SEQUENCIAL_PARA_INICIAR = "UltimoIDSequencialParaIniciar";
        private const string KEY_CONF_SEGUNDOS_AGUARDAR_ENTRE_REQUISICOES = "SegundosParaAguardarEntreRequisicoes";

        #endregion

        #region Construtor privado

        private IntegracaoOmnilink(AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente) : base(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Omnilink, nameConfigSection, cliente) { }

        #endregion

        #region Métodos públicos

        /**
         * Implementação de Singleton
         */
        public static IntegracaoOmnilink GetInstance(AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente)
        {
            if (Instance == null) Instance = new IntegracaoOmnilink(cliente);
            return Instance;
        }

        #endregion

        #region Métodos abstratos obrigatórios

        /**
         * Complementa configurações gerais
         */
        override protected void ComplementarConfiguracoes()
        {
            // Configuração do ID a ser usado para recuperar a última posição disponível do WebService, conforme documentação Omnilink
            long.TryParse(ObterValorOpcao(KEY_CONF_ULTIMO_ID_SEQUENCIAL_PARA_INICIAR), out this.ultimoIDSequencialParaIniciar);
            // Deve aguardar um tempo entre requisições subsequentes, conforme documentação Omnilink
            this.segundosParaAguardarEntreRequisicoes = Int32.Parse(ObterValorOpcao(KEY_CONF_SEGUNDOS_AGUARDAR_ENTRE_REQUISICOES));

        }

        /**
         * Confirmação de parâmetros corretos, executada apenas uma vez
         */
        override protected void Validar()
        {
            base.ValidarConfiguracaoDiretorioEArquivoControle(base.contasIntegracao);
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
            ObterUltimosSequenciaisDoArquivo();
            InicializarWSOmnilink();
            IntegrarPosicao();
            SalvarUltimosSequenciaisNoArquivo();
        }

        #endregion

        #region Métodos privados

        /**
         * Executa a integração das posições, consultando no WebService e registrando na tabela T_POSICAO
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
         * Consome os serviços que oferevem as posições dos veículos
         */
        private List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> ObterPosicoes()
        {
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoes = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao>();

            // Busca os eventos de controle, que contém as posições dos veículos, velocidade e temperatura
            try
            {
                List<Dominio.ObjetosDeValor.Embarcador.Integracao.Omnilink.TeleEvento> teleEventosCtrl = ObtemEventosCtrl();
                Log($"Recebidos {teleEventosCtrl.Count} eventos de controle", 4);
                List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoesEventoCrl = ConverterParaPosicao(teleEventosCtrl);
                ultimoSequencialEventoCtrl = ObtemUltimoSequencial(ultimoSequencialEventoCtrl, teleEventosCtrl);
                posicoes = posicoes.Concat(posicoesEventoCrl).ToList();
            }
            catch (Exception ex)
            {
                Log("Erro ObtemEventosCtrl " + ex.Message, 3);
            }

            // Deve aguardar alguns segundos entre as chamadas subsequentes
            Log($"Aguardando {this.segundosParaAguardarEntreRequisicoes}s entre as requisicoes...", 3);
            System.Threading.Thread.Sleep(this.segundosParaAguardarEntreRequisicoes * 1000);

            // Busca os eventos normais, que contém apenas as posições dos veículos
            try
            {
                List<Dominio.ObjetosDeValor.Embarcador.Integracao.Omnilink.TeleEvento> teleEventosNormais = ObtemEventosNormais();
                Log($"Recebidos {teleEventosNormais.Count} eventos normais", 4);
                List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoesEventoNormal = ConverterParaPosicao(teleEventosNormais);
                ultimoSequencialEventoNormal = ObtemUltimoSequencial(ultimoSequencialEventoNormal, teleEventosNormais);
                posicoes = posicoes.Concat(posicoesEventoNormal).ToList();
            }
            catch (Exception ex)
            {
                Log("Erro ObtemEventosNormais " + ex.Message, 3);
            }

            return posicoes;
        }

        /**
         * Requisição ao serviço ObtemEventosCtrl que contém as posições dos veículos, velocidade e temperatura
         */
        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.Omnilink.TeleEvento> ObtemEventosCtrl()
        {
            // Consulta os eventos normais que retornam, entre outras informações, as posições dos veículos
            DateTime inicio = DateTime.UtcNow;
            string response;
            if (this.conta.Protocolo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.Protocolo.HTTPS)
                response = omnilinkWSClient.ObtemEventosCtrl(this.conta.Usuario, this.conta.Senha, this.ultimoSequencialEventoCtrl);
            else
                response = omnilinkWSClientInterno.ObtemEventosCtrl(this.conta.Usuario, this.conta.Senha, (int)this.ultimoSequencialEventoCtrl);

            Log("Requisicao ObtemEventosCtrl ", inicio, 3);
            VerificarErro(response);

            // Sanitização e ajuste no retorno para fazer o parser do XML corretamente
            string tag = "ObtemEventosCtrlResult";
            string responseXmlCorrigido = $"<{tag}>{response.Replace("> ", ">").Replace(" <", "<")}</{tag}>";
            XmlSerializer serializer = new XmlSerializer(typeof(Dominio.ObjetosDeValor.Embarcador.Integracao.Omnilink.ResponseObtemEventosCtrl));
            StringReader stringReader = new StringReader(responseXmlCorrigido);
            Dominio.ObjetosDeValor.Embarcador.Integracao.Omnilink.ResponseObtemEventosCtrl responseObtemEventosCtrl = (Dominio.ObjetosDeValor.Embarcador.Integracao.Omnilink.ResponseObtemEventosCtrl)serializer.Deserialize(stringReader);
            return responseObtemEventosCtrl.TeleEventos;
        }

        /**
         * Requisição ao serviço ObtemEventosNormais que contém apenas posições dos veículos
         */
        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.Omnilink.TeleEvento> ObtemEventosNormais()
        {
            // Consulta os eventos normais que retornam, entre outras informações, as posições dos veículos
            DateTime inicio = DateTime.UtcNow;
            string response;
            if (this.conta.Protocolo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.Protocolo.HTTPS)
                response = omnilinkWSClient.ObtemEventosNormais(this.conta.Usuario, this.conta.Senha, this.ultimoSequencialEventoNormal);
            else
                response = omnilinkWSClientInterno.ObtemEventosNormais(this.conta.Usuario, this.conta.Senha, (int)this.ultimoSequencialEventoNormal);

            Log("Requisicao ObtemEventosNormais ", inicio, 3);
            VerificarErro(response);

            // Sanitização e ajuste no retorno para fazer o parser do XML corretamente
            string tag = "ObtemEventosNormaisResult";
            string responseXmlCorrigido = $"<{tag}>{response.Replace("> ", ">").Replace(" <", "<")}</{tag}>";
            XmlSerializer serializer = new XmlSerializer(typeof(Dominio.ObjetosDeValor.Embarcador.Integracao.Omnilink.ResponseObtemEventosNormais));
            StringReader stringReader = new StringReader(responseXmlCorrigido);
            Dominio.ObjetosDeValor.Embarcador.Integracao.Omnilink.ResponseObtemEventosNormais responseObtemEventosNormais = (Dominio.ObjetosDeValor.Embarcador.Integracao.Omnilink.ResponseObtemEventosNormais)serializer.Deserialize(stringReader);
            return responseObtemEventosNormais.TeleEventos;
        }

        /**
         * Converte os tele eventos para posição
         */
        private List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> ConverterParaPosicao(List<Dominio.ObjetosDeValor.Embarcador.Integracao.Omnilink.TeleEvento> teleEventos)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoes = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao>();

            int total = teleEventos.Count;
            for (int i = 0; i < total; i++)
            {
                string idEquipamento = teleEventos[i].NumeroSerie > 0 ? teleEventos[i].NumeroSerie.ToString() : string.Empty;
                if (
                    (teleEventos[i].NumeroSequenciaCtrl > 0 || teleEventos[i].NumeroSequencia > 0) &&
                    teleEventos[i].DataHoraEventoDT != null &&
                    teleEventos[i].DataHoraEmissaoDT != null &&
                    !string.IsNullOrWhiteSpace(idEquipamento)
                )
                {

                    string endereco = teleEventos[i].Cidade + ' ' + teleEventos[i].UF;
                    if (!string.IsNullOrWhiteSpace(teleEventos[i].Localizacao)) endereco = (teleEventos[i].Localizacao + " " + endereco).Trim();

                    // Conversão das coordenadas em GMS para decimal
                    Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint wayPoint = new Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint(teleEventos[i].Latitude, teleEventos[i].Longitude);
                    posicoes.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao
                    {
                        ID = (teleEventos[i].NumeroSequenciaCtrl > 0) ? teleEventos[i].NumeroSequenciaCtrl : teleEventos[i].NumeroSequencia,
                        Data = teleEventos[i].DataHoraEventoDT.Value,
                        DataVeiculo = teleEventos[i].DataHoraEmissaoDT.Value,
                        IDEquipamento = idEquipamento,
                        Latitude = wayPoint.Latitude,
                        Longitude = wayPoint.Longitude,
                        Velocidade = teleEventos[i].Velocidade,
                        Ignicao = teleEventos[i].Ignicao,
                        Temperatura = teleEventos[i].Temperatura_ValorD,
                        SensorTemperatura = (teleEventos[i].Temperatura_Codigo == 1) ? true : false,
                        Descricao = endereco,
                        Rastreador = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EnumTecnologiaRastreador.Omnilink
                    });
                }
            }
            return posicoes;
        }

        /**
         * 
         */
        private void VerificarErro(string response)
        {
            if (response.Contains("msgerro"))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Dominio.ObjetosDeValor.Embarcador.Integracao.Omnilink.ResponseErro));
                StringReader stringReader = new StringReader(response);
                Dominio.ObjetosDeValor.Embarcador.Integracao.Omnilink.ResponseErro error = (Dominio.ObjetosDeValor.Embarcador.Integracao.Omnilink.ResponseErro)serializer.Deserialize(stringReader);
                throw new Exception($"\"{error.CodMsg}={error.MsgErro}\"");
            }
        }

        private void InicializarWSOmnilink()
        {
            Log("Inicializando WS", 2);

            string url = Servicos.Embarcador.Logistica.ContaIntegracao.ObterURL(this.conta);

            if (url.StartsWith("https"))
            {
                omnilinkWSClient = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unitOfWork).ObterClient<Servicos.Omnilink.ASWSSoapClient, Servicos.Omnilink.IASWSSoap>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Omnilink_IASWSSoap, url);
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                omnilinkWSClient.ClientCredentials.UserName.UserName = this.conta.Usuario;
                omnilinkWSClient.ClientCredentials.UserName.Password = this.conta.Senha;
            }
            else
            {
                omnilinkWSClientInterno = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unitOfWork).ObterClient<Servicos.OmnilinkInterno.ASWSSoapClient, Servicos.OmnilinkInterno.IASWSSoap>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Omnilink_IASWSSoapInterno, url);
            }
        }

        /**
         * Extrai o maior número sequencial das posições
         */
        private long ObtemUltimoSequencial(long ultimoID, List<Dominio.ObjetosDeValor.Embarcador.Integracao.Omnilink.TeleEvento> teleEventos)
        {
            int cont = teleEventos.Count;
            for (int i = 0; i < cont; i++)
            {
                if (ultimoID == ultimoIDSequencialParaIniciar || teleEventos[i].NumeroSequenciaCtrl > ultimoID || teleEventos[i].NumeroSequencia > ultimoID)
                {
                    ultimoID = teleEventos[i].NumeroSequenciaCtrl > 0 ? teleEventos[i].NumeroSequenciaCtrl : teleEventos[i].NumeroSequencia;
                }
            }
            return ultimoID;
        }

        /**
         * Busca os últimos sequenciais dos eventos das posições já lidas
         */
        private void ObterUltimosSequenciaisDoArquivo()
        {
            string value;
            List<KeyValuePair<string, string>> dadosControle = base.CarregarDadosDeControleDoArquivo(this.conta);

            value = Servicos.Embarcador.Logistica.ContaIntegracao.ObterValorListaParametrosAdicionais(KEY_ULTIMO_SEQUENCIAL_EVENTO_NORMAL, dadosControle);
            this.ultimoSequencialEventoNormal = long.Parse((String.IsNullOrWhiteSpace(value)) ? ObterValorOpcao(KEY_CONF_ULTIMO_ID_SEQUENCIAL_PARA_INICIAR) : value);

            value = Servicos.Embarcador.Logistica.ContaIntegracao.ObterValorListaParametrosAdicionais(KEY_ULTIMO_SEQUENCIAL_EVENTO_CTRL, dadosControle);
            this.ultimoSequencialEventoCtrl = long.Parse((String.IsNullOrWhiteSpace(value)) ? ObterValorOpcao(KEY_CONF_ULTIMO_ID_SEQUENCIAL_PARA_INICIAR) : value);

            Log($"Lido ultimo sequencial normal {ultimoSequencialEventoNormal}", 2);
            Log($"Lido ultimo sequenciail de controle {ultimoSequencialEventoCtrl}", 2);

        }

        /**
         * Registra no arquivo os últimos números sequenciais dos eventos
         */
        private void SalvarUltimosSequenciaisNoArquivo()
        {
            List<KeyValuePair<string, string>> dadosControle = new List<KeyValuePair<string, string>>();
            dadosControle.Add(new KeyValuePair<string, string>(KEY_ULTIMO_SEQUENCIAL_EVENTO_NORMAL, ultimoSequencialEventoNormal.ToString()));
            dadosControle.Add(new KeyValuePair<string, string>(KEY_ULTIMO_SEQUENCIAL_EVENTO_CTRL, ultimoSequencialEventoCtrl.ToString()));
            base.SalvarDadosDeControleNoArquivo(this.conta, dadosControle);
            Log($"Salvo ultimo sequencial normal {ultimoSequencialEventoNormal}", 2);
            Log($"Salvo ultimo sequencial de controle {ultimoSequencialEventoCtrl}", 2);
        }

        #endregion

    }
}