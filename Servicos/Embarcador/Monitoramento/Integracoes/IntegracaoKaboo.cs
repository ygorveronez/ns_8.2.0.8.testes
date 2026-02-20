using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;

namespace Servicos.Embarcador.Monitoramento.Integracoes
{

    public class IntegracaoKaboo : Abstract.AbstractIntegracaoREST
    {
        #region Atributos privados

        private static IntegracaoKaboo Instance;
        private static readonly string nameConfigSection = "Kaboo";
        Dominio.ObjetosDeValor.Embarcador.Logistica.ContaIntegracao conta;

        #endregion

        #region Constantes com as chaves dos dados/configurações

        private const string KEY_CLIENTE = "cliente";
        private const string URI_LAST_POSITION_CLIENTE = "LastPositionCliente.cfm";
        #endregion

        #region Construtor privado

        private IntegracaoKaboo(AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente) : base(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Kaboo, nameConfigSection, cliente) { }

        #endregion

        #region Métodos públicos

        /**
         * Implementação de Singleton
         */
        public static IntegracaoKaboo GetInstance(AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente)
        {
            if (Instance == null) Instance = new IntegracaoKaboo(cliente);
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
                List<Dominio.ObjetosDeValor.Embarcador.Integracao.Kaboo.Posicao> posicoesKaboo = ObterLastPositionCliente();
                int total = posicoesKaboo.Count;
                Log($"Recebidas {total} posicoes", 3);

                if (total > 0)
                {
                    for (int i = 0; i < total; i++)
                    {
                        posicoes.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao
                        {
                            Data = posicoesKaboo[i].data,
                            DataVeiculo = posicoesKaboo[i].data,
                            IDEquipamento = posicoesKaboo[i].veiculo.ToString(),
                            Placa = posicoesKaboo[i].placa.Replace("-", ""),
                            Latitude = posicoesKaboo[i].lat,
                            Longitude = posicoesKaboo[i].lng,
                            Velocidade = posicoesKaboo[i].vel,
                            Ignicao = (posicoesKaboo[i].ignicao == "On") ? 1 : 0,
                            SensorTemperatura = false,
                            Rastreador = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EnumTecnologiaRastreador.Kaboo
                        });
                    }
                    Log($"{posicoes.Count} posicoes", 3);
                }
            }
            catch (Exception ex)
            {
                Log("Erro ObterLastPositionCliente " + ex.Message, 3);
            }
            return posicoes;
        }

        /**
         * Requisição ao servico "LastPositionCliente"
         */
        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.Kaboo.Posicao> ObterLastPositionCliente()
        {
            string response = Request();
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.Kaboo.Posicao> posicoes = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Integracao.Kaboo.Posicao>>(response);
            return posicoes;
        }

        /**
         * Executa a requisição ao WebService
         */
        private string Request()
        {
            string response = "";

            string cliente = ObterParametroCliente();
            NameValueCollection requestParams = new NameValueCollection();
            requestParams.Add("cliente", cliente);

            string url = ObterUrl(URI_LAST_POSITION_CLIENTE, requestParams);

            // Requisição
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";

            // Headers das requisição
            request.Headers["Cache-Control"] = "no-cache";
            request.Accept = "application/json";
            request.ContentType = "application/json";

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
                Log($"Requisicao {url}", inicio, 3);

                // Verificação do StatusCode
                switch (resp.StatusCode)
                {
                    case HttpStatusCode.OK: break;
                    default:
                        throw new Exception("Erro na requisicao: HTTP Status " + resp.StatusCode);
                }
            }

            return response;
        }

        /**
         * Cria a URL para a requisição
         */
        private string ObterUrl(string uri, NameValueCollection requestParams = null)
        {
            string url = Servicos.Embarcador.Logistica.ContaIntegracao.ObterURL(this.conta);

            if (!string.IsNullOrWhiteSpace(uri))
                url += uri;

            string requestParamsEncoded = EncodeRequestParams(requestParams);
            if (!string.IsNullOrWhiteSpace(requestParamsEncoded))
                url += "?" + requestParamsEncoded;

            return url;
        }

        /**
         * Parâmetros da requisição
         */
        private string EncodeRequestParams(NameValueCollection queryParams)
        {
            string url = string.Empty;
            if (queryParams != null && queryParams.Count > 0)
            {
                foreach (string key in queryParams)
                {
                    url += $"{key}={Uri.EscapeUriString(queryParams[key])}&";
                }
            }
            return url;
        }

        private string ObterParametroCliente()
        {
            string value = Servicos.Embarcador.Logistica.ContaIntegracao.ObterValorListaParametrosAdicionais(KEY_CLIENTE, this.conta.ListaParametrosAdicionais);
            return value;
        }

        #endregion

    }

}
