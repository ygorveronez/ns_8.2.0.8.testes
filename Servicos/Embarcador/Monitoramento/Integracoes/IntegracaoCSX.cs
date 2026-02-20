using Infrastructure.Services.HttpClientFactory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Servicos.Embarcador.Monitoramento.Integracoes
{
    public class IntegracaoCSX : Abstract.AbstractIntegracaoREST
    {

        #region Atributos privados

        private static IntegracaoCSX Instance;
        private static readonly string nameConfigSection = "CSX";
        Dominio.ObjetosDeValor.Embarcador.Logistica.ContaIntegracao conta;

        #endregion

        #region Atributos privados específicos para a CSX

        private string token;
        private DateTime tokenExpiration;
        //private DateTime ultimaDataConsultada;
        //private DateTime dataAtual;

        #endregion

        #region Constantes com as chaves dos dados/configurações

        //private const string KEY_ULTIMA_DATA = "UltimaData";
        private const string URI_HANDSHAKE = "api/v1/usuarios/login?idSistema=3";
        private const string URI_ULTIMA_POSICAO = "api/ObjetosRastreaveis/BuscarPosicoes";

        #endregion

        #region Construtor privado

        private IntegracaoCSX(AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente) : base(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.RastreamentoCSX, nameConfigSection, cliente) { }

        #endregion


        #region Métodos públicos

        /**
         * Implementação de Singleton
         */
        public static IntegracaoCSX GetInstance(AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente)
        {
            if (Instance == null) Instance = new IntegracaoCSX(cliente);
            return Instance;
        }

        #endregion

        #region Métodos abstratos obrigatórios

        /**
         * Complementa configurações gerais
         */
        override protected void ComplementarConfiguracoes()
        {

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
            ObterToken();
            IntegrarPosicao();

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
         * Busca as posições de todos os veículos
         */
        private List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> ObterPosicoes()
        {
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoes = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao>();

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.CSX.RetornoUltimasPosicoes retorno = ObterUltimasPosicoes();
                int total = retorno.objetoRastreavelPosicaoAg.Count;
                Log($"Recebidas {total} ultimas posicoes de veiculos", 3);

                if (total > 0)
                {
                    for (int i = 0; i < total; i++)
                    {
                        posicoes.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao
                        {
                            Data = retorno.objetoRastreavelPosicaoAg[i].ultimaPosicao.data,
                            DataVeiculo = retorno.objetoRastreavelPosicaoAg[i].ultimaPosicao.data,
                            IDEquipamento = retorno.objetoRastreavelPosicaoAg[i].objetoRastreavel,
                            //Placa = retorno.objetoRastreavelPosicaoAg[i].licensePlate,
                            Latitude = retorno.objetoRastreavelPosicaoAg[i].ultimaPosicao.latitude,
                            Longitude = retorno.objetoRastreavelPosicaoAg[i].ultimaPosicao.longitude,
                            Velocidade = retorno.objetoRastreavelPosicaoAg[i].ultimaPosicao.velocidade,
                            Ignicao = (retorno.objetoRastreavelPosicaoAg[i].ultimaPosicao.estadoIgnicao == "ON") ? 1 : 0,
                            //Temperatura = retorno.objetoRastreavelPosicaoAg[i].temperature1,
                            SensorTemperatura = false,
                            Rastreador = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EnumTecnologiaRastreador.CSX
                        });
                    }

                    Log($"{total} posicoes", 3);
                }
            }
            catch (Exception ex)
            {
                Log("Erro ObterUltimasPosicoes " + ex.Message, 3);
            }

            return posicoes;
        }


        /**
         * Requisição ao servico "posicoes/ultimaPosicao"
         */
        private Dominio.ObjetosDeValor.Embarcador.Integracao.CSX.RetornoUltimasPosicoes ObterUltimasPosicoes()
        {
            // Extrai os números únicos dos equipamento
            Dominio.ObjetosDeValor.Embarcador.Integracao.CSX.RequestUltimasPosicoes request = new Dominio.ObjetosDeValor.Embarcador.Integracao.CSX.RequestUltimasPosicoes();
            List<string> numerosEquipamentos = base.ObterNumerosEquipamentosDosVeiculos();
            request.objetosRastreavies = numerosEquipamentos.ToArray();

            string jsonRequestBody = JsonConvert.SerializeObject(request, Formatting.Indented);

            string bodyResponseUltimaPosicao = Request(URI_ULTIMA_POSICAO, jsonRequestBody, this.token);

            Dominio.ObjetosDeValor.Embarcador.Integracao.CSX.RetornoUltimasPosicoes posicoes = new Dominio.ObjetosDeValor.Embarcador.Integracao.CSX.RetornoUltimasPosicoes();
            posicoes = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.CSX.RetornoUltimasPosicoes>(bodyResponseUltimaPosicao);

            return posicoes;
        }

        /**
         * Autentica e obtém o token para as demais requisições
         */
        private void ObterToken()
        {
            Log("Obtendo Token", 2);
            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("pt-BR");

            Dominio.ObjetosDeValor.Embarcador.Integracao.CSX.Handshake handshake = new Dominio.ObjetosDeValor.Embarcador.Integracao.CSX.Handshake();
            handshake.nomeUsuario = this.conta.Usuario;
            handshake.senhaUsuario = this.conta.Senha;
            string jsonRequestBody = JsonConvert.SerializeObject(handshake, Formatting.Indented);

            // Request
            string bodyResponseHandshake = Request(URI_HANDSHAKE, jsonRequestBody, null);

            Dominio.ObjetosDeValor.Embarcador.Integracao.CSX.RetornoHandshake responseHandshake = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.CSX.RetornoHandshake>(bodyResponseHandshake);

            this.token = responseHandshake.tokenAutorizacao;
        }



        // * Executa a requisição ao WebService
        // */
        private string Request(string uri, string body = null, string token = "")
        {
            string response = "";

            string url = Servicos.Embarcador.Logistica.ContaIntegracao.ObterURL(this.conta);

            if (string.IsNullOrEmpty(token))
                url += ":5056/";
            else
                url = "https://api-ag-coopanexos.csxinovacao.com.br:5099/";

            url += uri;

            // Requisição
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoCSX));
            client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            if (!string.IsNullOrEmpty(token))
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Execução da reequisição
            DateTime inicio = DateTime.UtcNow;
            Log($"Requisicao {uri}", inicio, 3);

            StringContent content;

            if (!string.IsNullOrEmpty(token))
            {
                content = new StringContent(body, Encoding.UTF8, "application/json");
                var result = client.PostAsync(url, content).Result;

                // Leitura da resposta
                response = result.Content.ReadAsStringAsync().Result;

                // Verificação do StatusCode
                switch (result.StatusCode)
                {
                    case HttpStatusCode.OK: break;
                    case HttpStatusCode.Unauthorized:
                        throw new Exception("Requer autenticação.");
                    case HttpStatusCode.Forbidden:
                        throw new Exception("Acesso a requisição negada.");
                    default:
                        throw new Exception("Erro na requisicao: HTTP Status " + result.StatusCode + " - " + result.RequestMessage);
                }
            }
            else
            {
                content = new StringContent(body, Encoding.UTF8, "application/json");
                var result = client.PutAsync(url, content).Result; //buscar token é put

                // Leitura da resposta
                response = result.Content.ReadAsStringAsync().Result;

                // Verificação do StatusCode
                switch (result.StatusCode)
                {
                    case HttpStatusCode.OK: break;
                    case HttpStatusCode.Unauthorized:
                        throw new Exception("Requer autenticação.");
                    case HttpStatusCode.Forbidden:
                        throw new Exception("Acesso a requisição negada.");
                    default:
                        throw new Exception("Erro na requisicao: HTTP Status " + result.StatusCode + " - " + result.RequestMessage);
                }
            }

            return response;
        }

        /**
         * Cria a URL para a requisição
         */
        private string ObterUrl(string uri)
        {
            string url = Servicos.Embarcador.Logistica.ContaIntegracao.ObterURL(this.conta);

            if (!string.IsNullOrWhiteSpace(uri))
                url += uri;

            return url;
        }


        #endregion

    }

}
