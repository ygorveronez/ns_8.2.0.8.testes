using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace Servicos.Embarcador.Monitoramento.Integracoes
{

    public class IntegracaoTrafegus : Abstract.AbstractIntegracaoREST
    {
        #region Atributos privados

        private static IntegracaoTrafegus Instance;
        private static readonly string nameConfigSection = "Trafegus";
        Dominio.ObjetosDeValor.Embarcador.Logistica.ContaIntegracao conta;

        #endregion

        #region Atributos privados específicos para a Omnilink

        private long ultimoIdPosicao;

        #endregion

        #region Constantes com as chaves dos dados/configurações

        private const string KEY_ULTIMO_ID_POSICAO = "UltimoIDPosicao";
        private const string KEY_TECNOLOGIAS = "Tecnologias";
        private const string KEY_DOCUMENTO = "Documento";
        private const string KEY_GET_TERMINAL_NAO_ASSOCIADO = "getTerminalNaoAssociado";

        #endregion

        #region Construtor privado

        private IntegracaoTrafegus(AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente) : base(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Trafegus, nameConfigSection, cliente) { }

        #endregion

        #region Métodos públicos

        /**
         * Implementação de Singleton
         */
        public static IntegracaoTrafegus GetInstance(AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente)
        {
            if (Instance == null) Instance = new IntegracaoTrafegus(cliente);
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
            ObterUltimoIdPosicaoDoArquivo();
            IntegrarPosicao();
            SalvarUltimoIdPosicaoNoArquivo();
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

                // Busca os eventos normais, que contém as posições dos veículos
                List<Dominio.ObjetosDeValor.Embarcador.Integracao.Trafegus.Posicao> posicoesTrafegus = BuscaUltimaPosicaoVeiculo();
                Log($"Recebidas {posicoesTrafegus.Count} posicoes", 3);

                // Tecnologias que serão consideradas pela integração
                string[] tecnologiasHabilitadas = ObterTecnologiasConta();

                // Converte os tele eventos para posição
                foreach (Dominio.ObjetosDeValor.Embarcador.Integracao.Trafegus.Posicao posicaoTrafegus in posicoesTrafegus)
                {
                    // Filtra as tecnologias configuradas
                    if (tecnologiasHabilitadas.Contains(posicaoTrafegus.Tecnologia.ToUpper()) || string.IsNullOrEmpty(tecnologiasHabilitadas[0]))
                    {

                        // Descrição do local
                        string endereco = "";
                        if (!string.IsNullOrEmpty(posicaoTrafegus.DescricaoSistema))
                            endereco = posicaoTrafegus.DescricaoSistema.Trim();
                        else
                            endereco = (posicaoTrafegus.DescricaoTecnologia != null) ? posicaoTrafegus.DescricaoTecnologia.Trim() : "";

                        // ID do equipamento possui dois números separados por um "pipe", Considera apenas a primera parte
                        string equipamento = posicaoTrafegus.NumeroTerminal.Trim().Split('|').First();

                        posicoes.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao
                        {
                            ID = posicaoTrafegus.IdPosicao,
                            Data = posicaoTrafegus.DataTecnologia ?? posicaoTrafegus.DataCadastro,
                            DataVeiculo = posicaoTrafegus.DataBordo,
                            IDEquipamento = equipamento,
                            Placa = posicaoTrafegus.Placa,
                            Latitude = posicaoTrafegus.Latitude,
                            Longitude = posicaoTrafegus.Longitude,
                            Velocidade = posicaoTrafegus.Velocidade ?? 0,
                            Ignicao = posicaoTrafegus.Ignicao?.Trim().ToUpper() == "S" ? 1 : 0,
                            Temperatura = 0,
                            SensorTemperatura = false,
                            Descricao = endereco,
                            Rastreador = this.ObterEnumRastreadorDescricao(posicaoTrafegus.DescricaoTecnologia),
                            Gerenciadora = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EnumTecnologiaGerenciadora.Trafegus
                        });
                    }
                }
                Log($"{posicoes.Count} posicoes para \"{String.Join(",", tecnologiasHabilitadas)}\"", 3);

                // Extrai o maior ID entre as posições recebidas para iniciar por ela na próxima requisição
                AtualizarUltimoIdPosicao(posicoesTrafegus);

            }
            catch (Exception ex)
            {
                Log("Erro BuscaUltimaPosicaoVeiculo " + ex.Message, 3);
            }
            return posicoes;
        }

        /**
         * Requisição ao servico "BuscaUltimaPosicaoVeiculo"
         */
        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.Trafegus.Posicao> BuscaUltimaPosicaoVeiculo()
        {
            // Parâmetros query string
            NameValueCollection queryParams = new NameValueCollection();
            queryParams.Add("IdPosicao", ultimoIdPosicao.ToString());
            queryParams.Add(KEY_DOCUMENTO, Servicos.Embarcador.Logistica.ContaIntegracao.ObterValorListaParametrosAdicionais(KEY_DOCUMENTO, this.conta.ListaParametrosAdicionais));
            queryParams.Add(KEY_GET_TERMINAL_NAO_ASSOCIADO, Servicos.Embarcador.Logistica.ContaIntegracao.ObterValorListaParametrosAdicionais(KEY_GET_TERMINAL_NAO_ASSOCIADO, this.conta.ListaParametrosAdicionais));

            string responseJson = Request("posicaoVeiculo", null, queryParams);
            var response = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Trafegus.PosicaoTrafegus>(responseJson);
            return response.Posicao;
        }

        /**
         * Executa a requisição ao WebService
         */
        private string Request(string operacao = null, NameValueCollection pathParams = null, NameValueCollection queryParams = null)
        {
            string response = "";

            // Monta a URL para a requisição
            string url = BuildURL(operacao, pathParams, queryParams);

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            // Requisição
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";

            // Headers das requisição
            request.Headers["Authorization"] = $"Basic {Convert.ToBase64String(Encoding.UTF8.GetBytes(this.conta.Usuario + ":" + this.conta.Senha))}";
            request.Accept = "application/json";

            // Execução da reequisição
            DateTime inicio = DateTime.UtcNow;
            using (HttpWebResponse resp = (HttpWebResponse)request.GetResponse())
            {

                // Laitura da resposta
                using (var responseStream = resp.GetResponseStream())
                using (var responseStreamReader = new StreamReader(responseStream))
                {
                    response = responseStreamReader.ReadToEnd();
                }
                Log($"Requisicao {operacao}", inicio, 3);

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
         * Cria a URL para a requisição
         */
        private string BuildURL(string operacao = null, NameValueCollection pathParams = null, NameValueCollection queryParams = null)
        {
            string url = Servicos.Embarcador.Logistica.ContaIntegracao.ObterURL(this.conta);
            if (operacao != null)
                url += $"/{operacao}";

            // Concatena os parâmetros do path da URL
            if (pathParams != null && pathParams.Count > 0)
            {
                foreach (string key in pathParams)
                {
                    url += $"/{key}/{pathParams[key]}";
                }
            }

            // Concatena os parâmetros da queryString
            if (queryParams != null && queryParams.Count > 0)
            {
                url += "?";
                foreach (string key in queryParams)
                {
                    url += $"{key}={Uri.EscapeUriString(queryParams[key])}&";
                }
            }
            return url;
        }

        /**
         * Extrai o maior ID entre as posições rescebidas
         */
        private void AtualizarUltimoIdPosicao(List<Dominio.ObjetosDeValor.Embarcador.Integracao.Trafegus.Posicao> posicoesTrafegus)
        {
            int cont = posicoesTrafegus.Count;
            for (int i = 0; i < cont; i++)
            {
                if (posicoesTrafegus[i].IdPosicao > this.ultimoIdPosicao)
                {
                    this.ultimoIdPosicao = posicoesTrafegus[i].IdPosicao;
                }
            }
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

        private string[] ObterTecnologiasConta()
        {
            string tecnologias = Servicos.Embarcador.Logistica.ContaIntegracao.ObterValorListaParametrosAdicionais(KEY_TECNOLOGIAS, this.conta.ListaParametrosAdicionais);
            return tecnologias.ToUpper().Split(',');
        }

        #endregion

    }

}
