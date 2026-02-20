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
    public class AccountVehicles
    {
        public Dominio.ObjetosDeValor.Embarcador.Integracao.Autotrac.Account Account;
        public List<Dominio.ObjetosDeValor.Embarcador.Integracao.Autotrac.Vehicle> Vehicles;
    }

    public class IntegracaoAutotrac : Abstract.AbstractIntegracaoREST
    {
        #region Atributos privados

        private static IntegracaoAutotrac Instance;
        private static readonly string nameConfigSection = "Autotrac";
        Dominio.ObjetosDeValor.Embarcador.Logistica.ContaIntegracao conta;

        #endregion

        #region Atributos privados específicos para a Autotrac

        private List<AccountVehicles> Accounts;
        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.Autotrac.VehicleLastPosition> vehiclesLastPosition = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Autotrac.VehicleLastPosition>();
        private DateTime dataAtual;
        private DateTime dataLimiteConsulta;
        private int maximoHorasPassado = 12;

        #endregion

        #region Constantes com as chaves dos dados/configurações

        private const string KEY_CHAVE = "Chave";

        #endregion

        #region Construtor privado

        /**
         * Construtor 
         */
        private IntegracaoAutotrac(AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente) : base(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Autotrac, nameConfigSection, cliente) { }

        #endregion

        #region Métodos públicos

        /**
         * Implementação de Singleton
         */
        public static IntegracaoAutotrac GetInstance(AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente)
        {
            if (Instance == null) Instance = new IntegracaoAutotrac(cliente);
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

            dataAtual = DateTime.Now;
            dataLimiteConsulta = dataAtual.AddHours(-maximoHorasPassado);

            BuscarAccountsAndVehicles();
            IntegrarPosicoes();
        }

        #endregion

        #region Métodos privados

        /**
         * Executa a integração das posições, consultando no WebService e registrando na tabela T_POSICAO
         */
        private void IntegrarPosicoes()
        {
            Log($"Consultando posicoes", 2);

            // Busca as posições do WebService
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoes = ObterPosicoes();
            Log($"Integrando {posicoes.Count} posicoes", 2);
            base.InserirPosicoes(posicoes);
        }

        /**
         * Busca as posições de todos os veículos
         */
        private List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> ObterPosicoes()
        {
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoes = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao>();
            DateTime dateTo = DateTime.MinValue;

            // Extrai os números únicos dos equipamento
            List<string> numerosEquipamentos = base.ObterNumerosEquipamentosDosVeiculos();

            // Percorre cada um dos veículos para consultar suas posições
            int total = numerosEquipamentos.Count;

            for (int i = 0; i < total; i++)
            {
                // Verifica em qual account está o veículo
                int accountCode = IdentificarAccountCode(numerosEquipamentos[i]);
                if (accountCode > 0)
                {

                    // Busca o código do veículo a partir do address
                    int vehicleCode = BuscarVehicleCode(numerosEquipamentos[i]);
                    if (vehicleCode > 0)
                    {
                        Log($"Veiculo {numerosEquipamentos[i]} {vehicleCode}", 4);
                        try
                        {

                            // Busca a data da última posição
                            Dominio.ObjetosDeValor.Embarcador.Integracao.Autotrac.VehicleLastPosition vehicleLastPosition = ObtemUltimaPosicaoVehicle(numerosEquipamentos[i]);

                            // Consulta a partir do segundo seguinte
                            DateTime dataInicial = vehicleLastPosition.PositionTime.AddSeconds(1);

                            // Busca as posições disponíveis do veículo a partir da data
                            List<Dominio.ObjetosDeValor.Embarcador.Integracao.Autotrac.Position> positions = GetPositionsRepeat(accountCode, vehicleCode.ToString(), dataInicial, dateTo);
                            Log($"Positions {positions.Count}", 5);
                            if (positions.Count > 0)
                            {
                                foreach (Dominio.ObjetosDeValor.Embarcador.Integracao.Autotrac.Position position in positions)
                                {
                                    posicoes.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao
                                    {
                                        ID = position.ID,
                                        Data = position.PositionTimeDT,
                                        DataVeiculo = position.PositionTimeDT,
                                        IDEquipamento = numerosEquipamentos[i],
                                        Latitude = Math.Round(position.Latitude, 6),
                                        Longitude = Math.Round(position.Longitude, 6),
                                        Velocidade = (int)position.Speed,
                                        km = position.Hodometer,
                                        Ignicao = position.Ignition,
                                        Descricao = position.Landmark,
                                        SensorTemperatura = false,
                                        Rastreador = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EnumTecnologiaRastreador.AutoTrack
                                    });
                                }
                            }

                            // Busca os alertas disponíveis do veículo a partir da data
                            List<Dominio.ObjetosDeValor.Embarcador.Integracao.Autotrac.ExpandedAlert> expandedAlerts = GetExpandedAlertRepeat(accountCode, vehicleCode.ToString(), dataInicial, dateTo);
                            Log($"ExpandedAlerts {expandedAlerts.Count}", 5);
                            if (expandedAlerts.Count > 0)
                            {
                                foreach (Dominio.ObjetosDeValor.Embarcador.Integracao.Autotrac.ExpandedAlert expandedAlert in expandedAlerts)
                                {
                                    posicoes.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao
                                    {
                                        ID = expandedAlert.ID,
                                        Data = expandedAlert.MessageTimeDT,
                                        DataVeiculo = expandedAlert.PositionTimeDT,
                                        IDEquipamento = numerosEquipamentos[i],
                                        Latitude = Math.Round(expandedAlert.Latitude, 6),
                                        Longitude = Math.Round(expandedAlert.Longitude, 6),
                                        Velocidade = (int)expandedAlert.Speed,
                                        km = expandedAlert.Hodometer.HasValue ? expandedAlert.Hodometer.Value : 0D,
                                        Ignicao = expandedAlert.Ignition,
                                        Descricao = expandedAlert.Landmark,
                                        Temperatura = expandedAlert.Temperature1,
                                        SensorTemperatura = expandedAlert.Temperature1.HasValue ? true : false,
                                        Rastreador = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EnumTecnologiaRastreador.AutoTrack
                                    }); ;
                                }
                            }

                            // Armazena a data mais atual entre as posições e alertas do veículo, registrando por referência
                            if (posicoes.Count > 0)
                            {
                                vehicleLastPosition.PositionTime = (from obj in posicoes select obj.DataVeiculo).OrderByDescending(obj => obj).FirstOrDefault();
                            }
                        }
                        catch (Exception e)
                        {
                            Log($"Veiculo {numerosEquipamentos[i]} \"{e.Message}\"", 4, true);
                        }
                    }
                }
                else
                {
                    Log($"Veiculo {numerosEquipamentos[i]} nao esta em nenhuma account", 4);
                }
            }

            return posicoes;
        }

        /**
         * Busca a data da última posição recebida do veículo
         */
        private Dominio.ObjetosDeValor.Embarcador.Integracao.Autotrac.VehicleLastPosition ObtemUltimaPosicaoVehicle(string numeroEquipamento)
        {
            // Busca a data da última posição
            Dominio.ObjetosDeValor.Embarcador.Integracao.Autotrac.VehicleLastPosition vehicleLastPosition = (from obj in vehiclesLastPosition where obj.VehicleCode == numeroEquipamento select obj).OrderByDescending(obj => obj.PositionTime).FirstOrDefault();

            // Se não há nenhuma, busca a última registrada anteriormente
            if (vehicleLastPosition == null)
            {

                DateTime dataUltimaPosicao = dataAtual;

                // Busca o código do veículo a partir do número do equipamento
                List<Dominio.ObjetosDeValor.Embarcador.Veiculos.VeiculoMonitoramento> veiculos = base.ObterVeiculoPorEquipamento(numeroEquipamento);
                if (veiculos.Count > 0)
                {
                    // Busca a última posição do veículo no banco de dados, se não encontrar, considera a data atual como a última data
                    Repositorio.Embarcador.Logistica.Posicao repPosicao = new Repositorio.Embarcador.Logistica.Posicao(base.unitOfWork);
                    Dominio.Entidades.Embarcador.Logistica.Posicao ultimaPosicao = repPosicao.BuscarUltimaPosicaoDataVeiculo(veiculos[0].Codigo);
                    if (ultimaPosicao != null)
                    {
                        dataUltimaPosicao = ultimaPosicao.DataVeiculo;
                    }
                }

                // Adiciona na lista das últumas posições dos veículos
                vehicleLastPosition = new Dominio.ObjetosDeValor.Embarcador.Integracao.Autotrac.VehicleLastPosition();
                vehicleLastPosition.VehicleCode = numeroEquipamento;
                vehicleLastPosition.PositionTime = dataUltimaPosicao;
                vehiclesLastPosition.Add(vehicleLastPosition);
            }

            // Retringe o período mónimo de consulta
            if (vehicleLastPosition.PositionTime < dataAtual.AddHours(-maximoHorasPassado))
            {
                vehicleLastPosition.PositionTime = dataAtual.AddHours(-maximoHorasPassado);
            }

            return vehicleLastPosition;
        }

        /**
         * Consulta as contas e respectivos veiculos vinculados
         */
        private void BuscarAccountsAndVehicles()
        {
            Log("Buscando \"accounts\"", 2);

            var accounts = GetAccounts();
            int total = accounts?.Count ?? 0;
            Log($"{total} accounts", 3);
            if (total >= 0)
            {
                this.Accounts = new List<AccountVehicles>();
                for (int i = 0; i < total; i++)
                {
                    List<Dominio.ObjetosDeValor.Embarcador.Integracao.Autotrac.Vehicle> vehicles = GetVehicles(accounts[i].Code);
                    this.Accounts.Add(new AccountVehicles()
                    {
                        Account = accounts[i],
                        Vehicles = vehicles
                    });
                    Log($"Account \"{accounts[i].Code}-{accounts[i].Name}\" com {vehicles?.Count ?? 0} veiculos", 3);
                }
            }

        }

        /**
         * Requisição ao servico "accounts"
         */
        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.Autotrac.Account> GetAccounts()
        {
            NameValueCollection queryParams = new NameValueCollection();
            queryParams.Add("_limit", "1000");
            string responseJson = RequestGET(null, null, null, queryParams);
            if (!string.IsNullOrWhiteSpace(responseJson))
            {
                var accounts = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Integracao.Autotrac.Account>>(responseJson);
                return accounts;
            }
            return null;
        }

        /**
         * Requisição ao servico "vechicles"
         */
        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.Autotrac.Vehicle> GetVehicles(int accountCode)
        {
            NameValueCollection queryParams = new NameValueCollection();
            queryParams.Add("_limit", "1000");
            string responseJson = RequestGET(accountCode, "vehicles", null, queryParams);
            if (!string.IsNullOrWhiteSpace(responseJson))
            {
                var vechiclesResponse = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Autotrac.VehiclestResponse>(responseJson);
                return vechiclesResponse.Data;
            }
            return null;
        }

        /**
         * Requisição ao servico "expandedalerts"
         */
        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.Autotrac.ExpandedAlert> GetExpandedAlert(int accountCode, string vehicleCode, DateTime dateTimeFrom, DateTime dateTimeTo)
        {
            // Parâmetros path
            NameValueCollection pathParams = new NameValueCollection();
            pathParams.Add("vehicles", vehicleCode);

            // Parâmetros query string
            NameValueCollection queryParams = new NameValueCollection();
            if (dateTimeFrom != null && dateTimeFrom > DateTime.MinValue)
            {
                if (dateTimeFrom < dataLimiteConsulta)
                {
                    dateTimeFrom = dataLimiteConsulta;
                }
                queryParams.Add("_dateTimeFrom", dateTimeFrom.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss"));
            }
            if (dateTimeTo != null && dateTimeTo > DateTime.MinValue)
            {
                queryParams.Add("_dateTimeTo", dateTimeTo.ToUniversalTime().ToString("o"));
            }
            queryParams.Add("_limit", "1000");

            string responseJson = RequestGET(accountCode, "expandedalerts", pathParams, queryParams);
            Dominio.ObjetosDeValor.Embarcador.Integracao.Autotrac.ExpandedAlertResponse response = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Autotrac.ExpandedAlertResponse>(responseJson);
            return response.Data;
        }

        /**
         * Requisição ao servico "expandedalerts" tolerando o erro da lista branca e repetindo a consulta
         */
        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.Autotrac.ExpandedAlert> GetExpandedAlertRepeat(int accountCode, string vehicleCode, DateTime dateTimeFrom, DateTime dateTimeTo)
        {
            try
            {
                return GetExpandedAlert(accountCode, vehicleCode, dateTimeFrom, dateTimeTo);
            }
            catch (Dominio.ObjetosDeValor.Embarcador.Integracao.Autotrac.Exceptions.VeiculoNaoEstaNaLista)
            {
                AddToAuthorizedVehicles(accountCode, vehicleCode);
                return GetExpandedAlert(accountCode, vehicleCode, dateTimeFrom, dateTimeTo);
            }
        }

        /**
         * Requisição ao servico "positions"
         */
        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.Autotrac.Position> GetPositions(int accountCode, string vehicleCode, DateTime dateTimeFrom, DateTime dateTimeTo)
        {
            // Parâmetros path
            NameValueCollection pathParams = new NameValueCollection();
            pathParams.Add("vehicles", vehicleCode);

            // Parâmetros query string
            NameValueCollection queryParams = new NameValueCollection();
            if (dateTimeFrom != null && dateTimeFrom > DateTime.MinValue)
            {
                queryParams.Add("_dateTimeFrom", dateTimeFrom.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss"));
            }
            if (dateTimeTo != null && dateTimeTo > DateTime.MinValue)
            {
                queryParams.Add("_dateTimeTo", dateTimeTo.ToUniversalTime().ToString("o"));
            }
            queryParams.Add("_limit", "1000");

            string responseJson = RequestGET(accountCode, "positions", pathParams, queryParams);
            if (!string.IsNullOrWhiteSpace(responseJson))
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.Autotrac.PositionsResponse response = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Autotrac.PositionsResponse>(responseJson);
                if (response?.Data != null) return response.Data;
            }
            return null;
        }

        /**
         * Requisição ao servico "positions" tolerando o erro da lista branca e repetindo a consulta
         */
        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.Autotrac.Position> GetPositionsRepeat(int accountCode, string vehicleCode, DateTime dateTimeFrom, DateTime dateTimeTo)
        {
            try
            {
                return GetPositions(accountCode, vehicleCode, dateTimeFrom, dateTimeTo);
            }
            catch (Dominio.ObjetosDeValor.Embarcador.Integracao.Autotrac.Exceptions.VeiculoNaoEstaNaLista)
            {
                AddToAuthorizedVehicles(accountCode, vehicleCode);
                return GetPositions(accountCode, vehicleCode, dateTimeFrom, dateTimeTo);
            }
        }

        /**
         * Requisição ao servico "whitelist" para adicionar um veículos na lista branca
         */
        private void AddToWhiteList(int accountCode, string vehicleCode)
        {
            RequestPOST(accountCode, $"whiteList/{vehicleCode}");
            Log("Veiculo adicionado na white list", 3);
        }

        /**
         * Requisição ao servico "whitelist" para adicionar um veículos na lista branca
         */
        private void AddToAuthorizedVehicles(int accountCode, string vehicleCode)
        {
            RequestPOST(accountCode, $"authorizedvehicle/{vehicleCode}");
            Log("Veiculo adicionado na white list", 3);
        }

        /**
         * Requisição GET
         */
        private string RequestGET(int? accountCode = null, string operacao = null, NameValueCollection pathParams = null, NameValueCollection queryParams = null)
        {
            return Request("GET", accountCode, operacao, pathParams, queryParams);
        }

        /**
         * Requisição POST
         */
        private string RequestPOST(int? accountCode = null, string operacao = null, NameValueCollection pathParams = null, NameValueCollection queryParams = null)
        {
            return Request("POST", accountCode, operacao, pathParams, queryParams, "{}");
        }

        /**
         * Requisição PUT
         */
        private string RequestPUT(int? accountCode = null, string operacao = null, NameValueCollection pathParams = null, NameValueCollection queryParams = null)
        {
            return Request("PUT", accountCode, operacao, pathParams, queryParams, "{}");
        }

        /**
         * Requisição DELETE
         */
        private string RequestDELETE(int? accountCode = null, string operacao = null, NameValueCollection pathParams = null, NameValueCollection queryParams = null)
        {
            return Request("DELETE", accountCode, operacao, pathParams, queryParams, "{}");
        }

        /**
         * Executa a requisição ao WebService
         */
        private string Request(string method, int? accountCode = null, string operacao = null, NameValueCollection pathParams = null, NameValueCollection queryParams = null, string bodyRequest = "")
        {
            string response = "";

            // Monta a URL para a requisição
            string url = BuildURL(accountCode, operacao, pathParams, queryParams);

            // Requisição
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = method;

            // Headers das requisição
            request.Headers["Ocp-Apim-Subscription-Key"] = ObterChaveConta();
            request.Headers["Authorization"] = $"Basic {this.conta.Usuario}:{this.conta.Senha}";
            request.Accept = "application/json";

            // Execução da reequisição
            DateTime inicio = DateTime.UtcNow;
            try
            {

                // Escrita da requisição
                if (!string.IsNullOrWhiteSpace(bodyRequest))
                {
                    byte[] sendData = Encoding.UTF8.GetBytes(bodyRequest);
                    Stream requestStream = request.GetRequestStream();
                    requestStream.Write(sendData, 0, sendData.Length);
                    requestStream.Flush();
                    requestStream.Dispose();
                }

                // Leitura da resposta
                using (HttpWebResponse resp = (HttpWebResponse)request.GetResponse())
                {
                    using (var responseStream = resp.GetResponseStream())
                    using (var responseStreamReader = new StreamReader(responseStream))
                    {
                        response = responseStreamReader.ReadToEnd();
                    }
                    Log("Requisicao " + accountCode + " " + method + " " + operacao, inicio, 5);

                    // Verificação do StatusCode
                    switch (resp.StatusCode)
                    {
                        case HttpStatusCode.OK:
                        case HttpStatusCode.Created:
                        case HttpStatusCode.NoContent:
                            break;
                        default:
                            throw new Dominio.ObjetosDeValor.Embarcador.Integracao.Autotrac.Exceptions.RegraDeNegocioInvalida();
                    }
                }
            }
            catch (WebException e)
            {
                if (e.Response != null)
                {
                    Log("Requisicao " + accountCode + " " + method + " " + operacao, inicio, 5);
                    using (WebResponse responseError = e.Response)
                    {
                        HttpWebResponse httpResponse = (HttpWebResponse)responseError;
                        using (Stream data = responseError.GetResponseStream())
                        using (var reader = new StreamReader(data))
                        {
                            response = reader.ReadToEnd();
                        }

                        switch (httpResponse.StatusCode)
                        {
                            case HttpStatusCode.BadRequest:
                                throw new Dominio.ObjetosDeValor.Embarcador.Integracao.Autotrac.Exceptions.ParametroNoFormatoErrado();
                            case HttpStatusCode.Unauthorized:
                                throw new Dominio.ObjetosDeValor.Embarcador.Integracao.Autotrac.Exceptions.RequerAutorizacao();
                            case HttpStatusCode.Forbidden:
                                throw new Dominio.ObjetosDeValor.Embarcador.Integracao.Autotrac.Exceptions.AcessoNegado();
                            default:
                                if ((int)httpResponse.StatusCode == 422 && response.ToLower().Contains("lista"))
                                {
                                    throw new Dominio.ObjetosDeValor.Embarcador.Integracao.Autotrac.Exceptions.VeiculoNaoEstaNaLista();
                                }
                                throw new Dominio.ObjetosDeValor.Embarcador.Integracao.Autotrac.Exceptions.RegraDeNegocioInvalida(response);
                        }

                    }
                }
                else
                {
                    Log("Falha na requisicao " + e.Message, inicio, 5);
                }
            }
            return response;
        }

        /**
         * Cria a URL para a requisição
         */
        private string BuildURL(int? accountCode, string operacao = null, NameValueCollection pathParams = null, NameValueCollection queryParams = null)
        {
            string url = Servicos.Embarcador.Logistica.ContaIntegracao.ObterURL(this.conta);
            url += "/accounts";
            if (accountCode != null)
            {
                url += $"/{accountCode}";
            }

            // Concatena os parâmetros do path da URL
            if (pathParams != null && pathParams.Count > 0)
            {
                foreach (string key in pathParams)
                {
                    url += $"/{key}/{pathParams[key]}";
                }
            }

            if (operacao != null)
            {
                url += $"/{operacao}";
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
         * Identifica em qual account está o veículo
         */
        private int IdentificarAccountCode(string vehicleAddress)
        {
            int total = this.Accounts.Count;
            for (int i = 0; i < total; i++)
            {
                int totalVehicles = this.Accounts[i].Vehicles?.Count ?? 0;
                for (int j = 0; j < totalVehicles; j++)
                {
                    if (this.Accounts[i].Vehicles[j].Address == vehicleAddress)
                    {
                        return this.Accounts[i].Account.Code;
                    }
                }
            }
            return 0;
        }

        /**
        * Busca o "vehicle code" do veículo
        */
        private int BuscarVehicleCode(string vehicleAddress)
        {
            int total = this.Accounts.Count;
            for (int i = 0; i < total; i++)
            {
                int totalVehicles = this.Accounts[i].Vehicles?.Count ?? 0;
                for (int j = 0; j < totalVehicles; j++)
                {
                    if (this.Accounts[i].Vehicles[j].Address == vehicleAddress)
                    {
                        return this.Accounts[i].Vehicles[j].Code;
                    }
                }
            }
            return 0;
        }

        private string ObterChaveConta()
        {
            string value = Servicos.Embarcador.Logistica.ContaIntegracao.ObterValorListaParametrosAdicionais(KEY_CHAVE, this.conta.ListaParametrosAdicionais);
            return value;
        }

        #endregion

    }
}