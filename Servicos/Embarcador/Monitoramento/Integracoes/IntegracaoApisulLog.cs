using Dominio.ObjetosDeValor.Embarcador.Logistica;
using Infrastructure.Services.HttpClientFactory;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Dynamic;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Servicos.Embarcador.Monitoramento.Integracoes
{
    public class IntegracaoApisulLog : Abstract.AbstractIntegracaoREST
    {
        #region Atributos privados

        private static IntegracaoApisulLog Instance;
        private static readonly string nameConfigSection = "ApisulLog";
        Dominio.ObjetosDeValor.Embarcador.Logistica.ContaIntegracao conta;

        #endregion

        #region Construtor privado

        private IntegracaoApisulLog(AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente) : base(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.ApisulLog, nameConfigSection, cliente) { }

        #endregion

        #region Métodos públicos

        /**
         * Implementação de Singleton
         */
        public static IntegracaoApisulLog GetInstance(AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente)
        {
            if (Instance == null) Instance = new IntegracaoApisulLog(cliente);
            return Instance;
        }

        #endregion

        #region Métodos abstratos obrigatórios

        protected override void ComplementarConfiguracoes()
        {

        }
        protected override void Validar()
        {

        }
        protected override void Preparar()
        {

        }

        protected override void Executar(ContaIntegracao contaIntegracao)
        {
            this.conta = contaIntegracao;
            IntegrarPosicao();
        }

        #endregion

        #region Métodos privados 

        private void IntegrarPosicao()
        {
            Log($"Consultando posicoes", 2);

            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoes = ObterPosicoes();

            Log($"Integrando {posicoes.Count} posicoes", 2);
            base.InserirPosicoes(posicoes);
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> ObterPosicoes()
        {
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoes = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao>();
            try
            {
                DateTime inicio = DateTime.UtcNow;

                Log($"Requisicao Posicoes", inicio, 3);

                int take = 100;
                int skip = 0;
                int totalVeiculosMonitorados = ListaVeiculosMonitorados?.Count ?? 0;

                Log($"Consultando {totalVeiculosMonitorados} placas em monitoramento", 3);

                while (skip < totalVeiculosMonitorados)
                {
                    DateTime inicioPlacas = DateTime.UtcNow;

                    List<string> placas = ListaVeiculosMonitorados.Skip(skip).Take(take).Select(veiculo => veiculo.Placa).Distinct().ToList();

                    List<RetornoBuscaListaPosicaoAtualModeloIntegracao_Veiculo> veiculos = BuscaPosicoesVeiculos(placas);

                    foreach (RetornoBuscaListaPosicaoAtualModeloIntegracao_Veiculo p in veiculos)
                    {
                        posicoes.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao
                        {
                            Data = p.DataUltimaPosicao,
                            DataCadastro = DateTime.Now,
                            DataVeiculo = p.DataUltimaPosicao,
                            Placa = p.Placa,
                            Latitude = p.Latitude,
                            Longitude = p.Longitude,
                            Velocidade = p.Velocidade,
                            Temperatura = 0,
                            SensorTemperatura = false,
                            Descricao = p.Localizacao,
                            Ignicao = p.Ignicao ? 1 : 0,
                            Rastreador = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EnumTecnologiaRastreador.ApisulLog,
                            IDEquipamento = ""
                        });
                    }
                    Log($"Requisicao de {placas.Count} placas", inicioPlacas, 4);
                    skip += take;
                }

                Log($"Recebidas {posicoes.Count} posiçoes de {posicoes.GroupBy(x => x.Placa).ToList().Count} veículos", 3);
            }
            catch (Exception ex)
            {
                Log("Erro ObterPosicoes " + ex.Message, 3);
            }

            return posicoes;
        }

        private List<RetornoBuscaListaPosicaoAtualModeloIntegracao_Veiculo> BuscaPosicoesVeiculos(List<string> placas)
        {
            string listaPlacas = ObterListaPlacas(placas);
            List<RetornoBuscaListaPosicaoAtualModeloIntegracao_Veiculo> veiculos = new List<RetornoBuscaListaPosicaoAtualModeloIntegracao_Veiculo>();
            try
            {
                string url = Servicos.Embarcador.Logistica.ContaIntegracao.ObterURL(this.conta);
                string token = ObterToken(this.conta.Senha);
                Log($"URL= {url}", 4);
                Log($"Token= {token}", 4);

                // Corpo da requisição SOAP
                string soapBody = $@"<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:tem=""http://tempuri.org/"" xmlns:apis=""http://schemas.datacontract.org/2004/07/ApisulLog.Integracao.Modelo.Chamada"" xmlns:arr=""http://schemas.microsoft.com/2003/10/Serialization/Arrays"">
                                        <soapenv:Header/>
                                        <soapenv:Body>
                                            <tem:BuscaListaUltimaPosicao>
                                                <tem:identificacao>{this.conta.Usuario}</tem:identificacao>
                                                <tem:token>{token}</tem:token>
                                                <tem:buscaPosicaoAtual>
                                                    <apis:ListaPlaca>
                                                        {listaPlacas}
                                                    </apis:ListaPlaca>
                                                </tem:buscaPosicaoAtual>
                                            </tem:BuscaListaUltimaPosicao>
                                        </soapenv:Body>
                                    </soapenv:Envelope>";

                string stringsalvarLog = ObterValorMonitorar("SalvarLogResponse");
                bool salvarLog = (!string.IsNullOrWhiteSpace(stringsalvarLog)) && bool.Parse(stringsalvarLog);
                if (salvarLog)
                    LogNomeArquivo(soapBody, DateTime.Now, "RequestPosicoesAPISUL", 0, true);

                var httpClient = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoApisulLog));
                // Configurando os headers da requisição
                httpClient.DefaultRequestHeaders.Add("SOAPAction", "\"http://tempuri.org/IVeiculo/BuscaListaUltimaPosicao\"");

                var content = new StringContent(soapBody, Encoding.UTF8, "text/xml");

                // Enviando a requisição POST de forma síncrona
                var task = httpClient.PostAsync(url, content);
                task.Wait(); // Bloqueia o fluxo até que a tarefa seja concluída

                // Obtendo a resposta
                var response = task.Result;
                var responseBody = response.Content.ReadAsStringAsync().Result;

                if (salvarLog)
                    LogNomeArquivo(responseBody, DateTime.Now, "ResponsePosicoesAPISUL", 0, true);

                var responseXmlDoc = new XmlDocument();
                responseXmlDoc.LoadXml(responseBody);

                // Verificando se a requisição foi bem-sucedida
                if (response.IsSuccessStatusCode)
                {
                    if ((responseXmlDoc.GetElementsByTagName("TransacaoOk").Item(0)?.InnerText ?? "false") == "true")
                    {
                        // Carregar o XML em um XmlDocument
                        XmlDocument xmlDoc = new XmlDocument();
                        xmlDoc.LoadXml(responseBody);

                        // Namespace manager para lidar com namespaces
                        XmlNamespaceManager nsManager = new XmlNamespaceManager(xmlDoc.NameTable);
                        nsManager.AddNamespace("s", "http://schemas.xmlsoap.org/soap/envelope/");
                        nsManager.AddNamespace("a", "http://schemas.datacontract.org/2004/07/ApisulLog.Integracao.Modelo.Retorno");

                        // Navegar até o elemento que contém a lista de veículos
                        XmlNodeList veiculosNodes = xmlDoc.SelectNodes("//a:RetornoBuscaListaPosicaoAtualModeloIntegracao.Veiculo", nsManager);

                        // Iterar sobre os nós de veículo e criar objetos de veículo
                        foreach (XmlNode veiculoNode in veiculosNodes)
                        {
                            RetornoBuscaListaPosicaoAtualModeloIntegracao_Veiculo veiculo = new RetornoBuscaListaPosicaoAtualModeloIntegracao_Veiculo();
                            veiculo.DataTecnologia = DateTime.Parse(veiculoNode.SelectSingleNode("a:DataTecnologia", nsManager)?.InnerText ?? "");
                            veiculo.DataUltimaPosicao = DateTime.Parse(veiculoNode.SelectSingleNode("a:DataUltimaPosicao", nsManager)?.InnerText ?? "");
                            veiculo.Placa = veiculoNode.SelectSingleNode("a:Placa", nsManager)?.InnerText ?? "";

                            veiculo.Localizacao = veiculoNode.SelectSingleNode("a:Localizacao", nsManager)?.InnerText ?? "";
                            veiculo.Latitude = double.Parse(veiculoNode.SelectSingleNode("a:Latitude", nsManager)?.InnerText ?? "0", CultureInfo.InvariantCulture);
                            veiculo.Longitude = double.Parse(veiculoNode.SelectSingleNode("a:Longitude", nsManager)?.InnerText ?? "0", CultureInfo.InvariantCulture);

                            veiculo.Ignicao = bool.Parse(!string.IsNullOrEmpty(veiculoNode.SelectSingleNode("a:Ignicao", nsManager)?.InnerText) ? veiculoNode.SelectSingleNode("a:Ignicao", nsManager)?.InnerText : "false");
                            veiculo.Velocidade = int.Parse(veiculoNode.SelectSingleNode("a:Velocidade", nsManager)?.InnerText ?? "0", CultureInfo.InvariantCulture);

                            //veiculo.AnoSMP = veiculoNode.SelectSingleNode("a:AnoSMP", nsManager)?.Value ?? "";
                            //veiculo.Frota = veiculoNode.SelectSingleNode("a:Frota", nsManager)?.InnerText ?? "";
                            //veiculo.GPRS = bool.Parse(veiculoNode.SelectSingleNode("a:GPRS", nsManager)?.InnerText ?? "false");
                            //veiculo.GPS = bool.Parse(veiculoNode.SelectSingleNode("a:GPS", nsManager)?.InnerText ?? "false");
                            //veiculo.NumeroSMP = veiculoNode.SelectSingleNode("a:NumeroSMP", nsManager)?.Value ?? "";
                            //veiculo.ReferenciaPonto = veiculoNode.SelectSingleNode("a:ReferenciaPonto", nsManager)?.InnerText ?? "";

                            veiculos.Add(veiculo);
                        }
                    }
                }
                else
                {
                    Log($"Falha na consulta: Code= {responseXmlDoc.GetElementsByTagName("faultcode").Item(0)?.InnerText ?? ""}", 4);
                    Log($"Falha na consulta: Fault= {responseXmlDoc.GetElementsByTagName("faultstring").Item(0)?.InnerText ?? ""}", 4);
                    Log($"Falha na consulta: Token= {responseXmlDoc.GetElementsByTagName("Token").Item(0)?.InnerText}", 4);
                }
            }
            catch (Exception ex)
            {
                Log("Exception BuscaPosicoesVeiculos " + ex.Message, 4);
            }
            return veiculos;
        }

        private string ObterListaPlacas(List<string> placas)
        {
            StringBuilder bld = new StringBuilder();
            for (int i = 0; i < placas.Count; ++i)
            {
                bld.Append("<arr:string>" + placas[i] + "</arr:string>");
            }
            return bld.ToString();
        }

        private string ObterToken(string senhaInterna)
        {
            // Cria o salte baseado na data hora e minuto UTC 
            string dateUTC = DateTime.UtcNow.ToString("ddMMyyyyHHmm");
            byte[] Salt = Encoding.ASCII.GetBytes(senhaInterna.Length.ToString());

            // Cria uma chave secreta baseada no salt
            PasswordDeriveBytes chaveScreta = new PasswordDeriveBytes(senhaInterna, Salt, "SHA1", 1);

            // Cripitografa a chave secreta
            RijndaelManaged RijndaelCipher = new RijndaelManaged();
            ICryptoTransform encripitador = RijndaelCipher.CreateEncryptor(chaveScreta.GetBytes(16), chaveScreta.GetBytes(16));

            // Acionado a Chave ao objeto em memoria para realizar a o encript do TOKEN
            MemoryStream memoryStream = new MemoryStream();
            CryptoStream encripChave = new CryptoStream(memoryStream, encripitador, CryptoStreamMode.Write);

            // Montagem do Token sobre a senha forneceida pela APISUL
            byte[] planoTexto = System.Text.Encoding.Unicode.GetBytes(dateUTC);
            encripChave.Write(planoTexto, 0, planoTexto.Length);
            encripChave.FlushFinalBlock();
            byte[] tokenBytes = memoryStream.ToArray();
            memoryStream.Close();
            encripChave.Close();

            // Transfere o TOKEN para Base 
            string token = Convert.ToBase64String(tokenBytes);
            return token;
        }
        #endregion

        #region Classe Retorno Posição
        public class RetornoBuscaListaPosicaoAtualModeloIntegracao_Veiculo
        {
            public string AnoSMP { get; set; }
            public DateTime DataTecnologia { get; set; }
            public DateTime DataUltimaPosicao { get; set; }
            public string Frota { get; set; }
            public bool GPRS { get; set; }
            public bool GPS { get; set; }
            public bool Ignicao { get; set; }
            public double Latitude { get; set; }
            public string Localizacao { get; set; }
            public double Longitude { get; set; }
            public string NumeroSMP { get; set; }
            public string Placa { get; set; }
            public string ReferenciaPonto { get; set; }
            public int Velocidade { get; set; }
        }
        #endregion
    }
}



