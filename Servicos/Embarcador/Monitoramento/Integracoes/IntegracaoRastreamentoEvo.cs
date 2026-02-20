using Dominio.ObjetosDeValor.Embarcador.Integracao.Evo;
using Dominio.ObjetosDeValor.Embarcador.Logistica;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace Servicos.Embarcador.Monitoramento.Integracoes
{
    public class IntegracaoRastreamentoEvo : Abstract.AbstractIntegracaoREST
    {
        #region Atributos privados

        private static IntegracaoRastreamentoEvo Instance;
        private static readonly string nameConfigSection = "Evo";
        private static List<int>listaIds;
        Dominio.ObjetosDeValor.Embarcador.Logistica.ContaIntegracao conta;

        #endregion

        #region Atributos privados específicos para a Evo Solutions

        private long ultimoIdPosicao;
        private string chaveIntegracao;
        private DateTime validadeToken;

        #endregion

        #region Constantes com as chaves dos dados/configurações

        private const string KEY_ULTIMO_ID_POSICAO = "UltimoIDPosicao";
        //Parâmetro que define a busca por posição
        private const int FLAGS = 4194305;
        private const int QUANTIDA_REGISTROS = 50;

        #endregion

        #region Construtor privado

        private IntegracaoRastreamentoEvo(AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente) : base(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Evo, nameConfigSection, cliente) { }

        #endregion

        #region Métodos públicos

        /**
         * Implementação de Singleton
         */
        public static IntegracaoRastreamentoEvo GetInstance(AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente)
        {
            if (Instance == null) 
            {
                Instance = new IntegracaoRastreamentoEvo(cliente);
            }

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
            ObtemOuRevalidaToken();
            ObterUltimoIdPosicaoDoArquivo();
            IntegrarPosicao();
            SalvarUltimoIdPosicaoNoArquivo();
        }

        #endregion

        #region Métodos privados

        /**
         * Autentica e obtém o token para as demais requisições
         */
        private void ObterCheveIntegracao()
        {
            Log("Obtendo Chave através do Token", 2);

            string svc = "token/login";
            string token = ObterToken();
            string jsonParams = $"{{\"token\":\"{token}\"}}";

            string responseJson = Request(svc, jsonParams, "POST");
            var response = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Evo.Root>(responseJson);

            if (!string.IsNullOrWhiteSpace(response.eid))
            {
                this.chaveIntegracao = response.eid;
                this.validadeToken = DateTime.Now.AddSeconds(1000);

                string[] ids = response.user.prp.monu.Trim('[', ']').Split(',');

                //Armazena os ids em uma lista 
                listaIds = ids.Select( i => int.Parse(i)).ToList();

            }
            else
            {
                this.chaveIntegracao = null;
                this.validadeToken = DateTime.MinValue;
                Log($"Falha na autenticação {response.error}", 3);
            }

        }

        /**
         * Confirma que o token ainda está valido
         */
        private void ObtemOuRevalidaToken()
        {
            if (!TokenEstaValido()) ObterCheveIntegracao();
        }

        /**
         * Verifica a validadedo token
         */
        private bool TokenEstaValido()
        {
            return (!string.IsNullOrWhiteSpace(this.chaveIntegracao) && this.validadeToken > DateTime.Now);
        }

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

        private List<Posicao> ObterPosicoes()
        {
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoes = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao>();

            if (TokenEstaValido())
            {
                try
                {

                    // Busca os eventos normais, que contém as posições dos veículos
                    List<Dominio.ObjetosDeValor.Embarcador.Integracao.Evo.PosicaoResponse> posicoesEvo =  ObterPacotePosicoes();
                    long total = posicoesEvo.Count();


                    Log($"Recebidas {total} posicoes", 3);

                    for (int i = 0; i < total; i++)
                    {

                        DateTime dataPosicao = DateTimeOffset.FromUnixTimeSeconds(posicoesEvo[i].Item.Posicao.HoraUTC).DateTime.ToLocalTime();

                        posicoes.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao
                        {
                            ID = posicoesEvo[i].Item.IdItem,
                            Data = dataPosicao,
                            DataCadastro = DateTime.Now,
                            DataVeiculo = dataPosicao,
                            IDEquipamento = posicoesEvo[i].Item.IdItem.ToString(),
                            Placa = posicoesEvo[i].Item.Placa,
                            Latitude = posicoesEvo[i].Item.Posicao.Latitude,
                            Longitude = posicoesEvo[i].Item.Posicao.Longitude,
                            Velocidade = posicoesEvo[i].Item.Posicao.Velocidade,
                            SensorTemperatura = false,
                            Rastreador = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EnumTecnologiaRastreador.Evo 
                        });
                    }
                    Log($"{posicoes.Count} posicoes", 3);

                    // Extrai o maior ID entre as posições recebidas para iniciar por ela na próxima requisição
                    AtualizarUltimoIdPosicao(posicoesEvo);

                }
                catch (Exception ex)
                {
                    Log("Erro ObterPacotePosicoes " + ex.Message, 3);
                }
            }
            else
            {
                Log("Token invalido", 3);
            }
            return posicoes;
        }

        /**
         * Requisição ao servico "ObterPacotePosicoes"
         */
        private List<PosicaoResponse> ObterPacotePosicoes()
        {
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.Evo.PosicaoResponse> listaPosicoes = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Evo.PosicaoResponse>();

            string svc = "core/search_item";
            string url = ObterURL();

            if (listaIds.Count > 0)
            {
                int IndiceInicio = 0;

                if (this.ultimoIdPosicao > 1)
                {
                    IndiceInicio = listaIds.FindIndex(id => id == (int)this.ultimoIdPosicao);

                    if (IndiceInicio == -1)
                        IndiceInicio = 0;

                }
                    
                var IdsMonitoramento = listaIds.Skip(IndiceInicio).Take(QUANTIDA_REGISTROS).ToList();

                foreach (int id in IdsMonitoramento)
                {
                    string jsonParams = $"{{\"id\":{id},\"flags\":{FLAGS}}}";
                    string responseJson = Request(svc, jsonParams, "GET");

                    var response = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Evo.PosicaoResponse>(responseJson);

                    listaPosicoes.Add(response);
                }
            }
           
            return listaPosicoes;
        }

        /**
         * Executa a requisição ao WebService
         */
        private string Request(string svc, string jsonParams, string method)
        {
            string response = string.Empty;
            string urlCompleta = string.Empty;

            string url = ObterURL();

            string encodedJsonParams = HttpUtility.UrlEncode(jsonParams);

            if (method == "POST") 
            {
                urlCompleta = $"{url}?svc={svc}&params={encodedJsonParams}";
            }
            else
            {
                urlCompleta = $"{url}?svc={svc}&params={encodedJsonParams}&sid={this.chaveIntegracao}";
            }

            // Requisição
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlCompleta);
            request.Method = method;

            // Headers das requisição
            request.Headers["Cache-Control"] = "no-cache";
            request.Accept = "application/json";

            // Execução da reequisição
            DateTime inicio = DateTime.UtcNow;

            // Leitura da resposta
            using (HttpWebResponse resp = (HttpWebResponse)request.GetResponse())
            {
                using (var responseStream = resp.GetResponseStream())
                using (var responseStreamReader = new StreamReader(responseStream))
                {
                    response = responseStreamReader.ReadToEnd();
                }
                Log($"Requisicao {urlCompleta}", inicio, 3);

                // Verificação do StatusCode
                switch (resp.StatusCode)
                {
                    case HttpStatusCode.OK: break;
                    case HttpStatusCode.Unauthorized:
                        throw new Exception("Requer autenticação.");
                    case HttpStatusCode.Forbidden:
                        throw new Exception("Acesso a requisição negada.");
                    default:
                        throw new Exception("Erro na requisicao: HTTP Status " + resp.StatusCode);
                }
            }

            return response;
        }

        /**
         * Obtém a URL da requisição
         */
        private string ObterURL()
        {
            return Servicos.Embarcador.Logistica.ContaIntegracao.ObterURLPadrao(this.conta);
        }

        private string ObterToken()
        {
            List<KeyValuePair<string, string>> listaParametosAdicionais = Servicos.Embarcador.Logistica.ContaIntegracao.ObterListaParametrosAdicionais(this.conta);
            return Servicos.Embarcador.Logistica.ContaIntegracao.ObterValorListaParametrosAdicionais("token", listaParametosAdicionais);
        }

        /**
         * Extrai o maior ID entre as posições rescebidas
         */
        private void AtualizarUltimoIdPosicao(List<Dominio.ObjetosDeValor.Embarcador.Integracao.Evo.PosicaoResponse> posicoes)
        {
            int cont = posicoes.Count;
            for (int i = 0; i < cont; i++)
            {
                if (posicoes[i].Item.IdItem > this.ultimoIdPosicao)
                {
                    this.ultimoIdPosicao = posicoes[i].Item.IdItem;
                }
            }

            if (posicoes[cont - 1].Item.IdItem == this.ultimoIdPosicao)
                this.ultimoIdPosicao = 1;
        }

        /**
         * Busca o último ID de posições já recebido
         */
        private void ObterUltimoIdPosicaoDoArquivo()
        {
            List<KeyValuePair<string, string>> dadosControle = base.CarregarDadosDeControleDoArquivo(this.conta);
            string value = Servicos.Embarcador.Logistica.ContaIntegracao.ObterValorListaParametrosAdicionais(KEY_ULTIMO_ID_POSICAO, dadosControle);
            this.ultimoIdPosicao = (String.IsNullOrWhiteSpace(value)) ? 1 : Int64.Parse(value);
            Log($"Ultimo ID Posicao {this.ultimoIdPosicao}", 2);
        }

        /**
         * Registra no arquivo o último ID das posições
         */
        private void SalvarUltimoIdPosicaoNoArquivo()
        {
            List<KeyValuePair<string, string>> dadosControle = new List<KeyValuePair<string, string>>();
            dadosControle.Add(new KeyValuePair<string, string>(KEY_ULTIMO_ID_POSICAO, this.ultimoIdPosicao.ToString()));
            base.SalvarDadosDeControleNoArquivo(this.conta, dadosControle);
            Log($"Atualizando ultimo ID de posicao {this.ultimoIdPosicao}", 2);
        }

        #endregion
    }
}
