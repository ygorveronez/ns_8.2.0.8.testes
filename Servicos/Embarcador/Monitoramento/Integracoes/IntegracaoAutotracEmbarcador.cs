using Dominio.ObjetosDeValor.Embarcador.Logistica;
using Infrastructure.Services.HttpClientFactory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace Servicos.Embarcador.Monitoramento.Integracoes
{

    public class IntegracaoAutotracEmbarcador : Abstract.AbstractIntegracaoREST
    {
        #region Atributos privados

        private static IntegracaoAutotracEmbarcador Instance;
        private static readonly string nameConfigSection = "AutotracEmbarcador";
        Dominio.ObjetosDeValor.Embarcador.Logistica.ContaIntegracao conta;

        #endregion

        #region Atributos privados específicos para a Autotrac

        private DateTime dataAtual;
        private DateTime dataLimiteConsulta;
        private int maximoHorasPassado = 12;

        #endregion

        #region Constantes com as chaves dos dados/configurações

        private const string KEY_CHAVE = "Chave";
        private const string KEY_HASH = "Hash";

        #endregion

        #region Construtor privado

        /**
         * Construtor 
         */
        private IntegracaoAutotracEmbarcador(AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente) : base(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.AutotracEmbarcador, nameConfigSection, cliente) { }

        #endregion

        #region Métodos públicos

        /**
         * Implementação de Singleton
         */
        public static IntegracaoAutotracEmbarcador GetInstance(AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente)
        {
            if (Instance == null) Instance = new IntegracaoAutotracEmbarcador(cliente);
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
        private List<Posicao> ObterPosicoes()
        {
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoes = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao>();

            Dominio.ObjetosDeValor.Embarcador.Integracao.AutotracEmbarcador.RetornoAuthorizedVehicles veiculosAutorizados = ObterVeiculosCadastradosAutorizados("authorizedvehicles");

            Log($"Veiculos Autorizados na Autotrac {veiculosAutorizados?.Data?.Count}", 2);

            Dominio.ObjetosDeValor.Embarcador.Integracao.AutotracEmbarcador.RetornoAuthorizedVehicles veiculosCadastrados = ObterVeiculosCadastrados();

            Log($"Veiculos Cadastrados na Autotrac {veiculosCadastrados?.Data?.Count}", 2);

            if (veiculosCadastrados != null)
            {
                foreach (var veiculo in veiculosCadastrados.Data)
                {
                    if (veiculosAutorizados != null)
                    {
                        if (veiculosAutorizados.Data.Any(x => x.VehicleCode == veiculo.VehicleCode))
                        {
                            BuscarPosicoesVeiculo(posicoes, veiculo);
                        }
                        else
                        {
                            Log($"Adicionando Veiculo Na Lista Autorizados da Autotrac {veiculo.LicensePlate}", 4);
                            try
                            {
                                AdicionarVeiculoNaListaAutorizados(veiculo);
                            }
                            catch
                            {
                                try
                                {
                                    //tenta buscar mesmo nao estando como autorizado
                                    BuscarPosicoesVeiculo(posicoes, veiculo);
                                }
                                catch (Exception ex)
                                {
                                    Servicos.Log.TratarErro($"[Arquitetura-CatchNoAction] Erro ao buscar posições de veículo não autorizado Autotrac: {ex.ToString()}", "CatchNoAction");
                                }

                                Log($"Veiculo {veiculo.LicensePlate} Não pode ser adiciona na Lista de Autorizados da Autotrac", 4);
                            }

                        }
                    }
                }
            }
            Log($"Posicoes Encontradas {posicoes.Count} posicoes", 2);
            return posicoes;
        }

        private void BuscarPosicoesVeiculo(List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoes, Dominio.ObjetosDeValor.Embarcador.Integracao.AutotracEmbarcador.AuthorizedVehicles veiculo)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.AutotracEmbarcador.RetornoPosicoes posicoesVeiculo = ObterPosicoesVeiculo(veiculo);
            foreach (var position in posicoesVeiculo.Data)
            {
                posicoes.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao
                {
                    DataCadastro = DateTime.Now,
                    Data = position.PositionTimeDT,
                    DataVeiculo = position.PositionTimeDT,
                    Latitude = position.Latitude.ToString().ToDouble(),
                    Longitude = position.Longitude.ToString().ToDouble(),
                    km = position.Hodometer.ToString().ToInt(),
                    Ignicao = position.VehicleIgnition,
                    Descricao = position.Descricao,
                    SensorTemperatura = false,
                    Placa = position.LicensePlate,
                    IDEquipamento = position.VehicleCode.ToString() ?? position.LicensePlate,
                    Rastreador = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EnumTecnologiaRastreador.AutoTrackEmbarcador
                });
            }
        }


        private void AdicionarVeiculoNaListaAutorizados(Dominio.ObjetosDeValor.Embarcador.Integracao.AutotracEmbarcador.AuthorizedVehicles veiculo)
        {
            string url = $"{this.conta.Protocolo}://{this.conta.Servidor}/{this.conta.URI}/{ObterHashConta()}/authorizedvehicles/{veiculo.VehicleCode}?_accountCode={veiculo.AccountCode}";

            Request(HttpMethod.Post, url);
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.AutotracEmbarcador.RetornoAuthorizedVehicles ObterVeiculosCadastrados()
        {
            return ObterVeiculosCadastradosAutorizados("vehicles");

        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.AutotracEmbarcador.RetornoAuthorizedVehicles ObterVeiculosCadastradosAutorizados(string metodo)
        {
            string url = $"{this.conta.Protocolo}://{this.conta.Servidor}/{this.conta.URI}/{ObterHashConta()}/{metodo}?_limit=5000&_offset=0";

            string retornoVeiculosAutorizados = Request(HttpMethod.Get, url);
            Dominio.ObjetosDeValor.Embarcador.Integracao.AutotracEmbarcador.RetornoAuthorizedVehicles retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.AutotracEmbarcador.RetornoAuthorizedVehicles>(retornoVeiculosAutorizados);

            return retorno;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.AutotracEmbarcador.RetornoPosicoes ObterPosicoesVeiculo(Dominio.ObjetosDeValor.Embarcador.Integracao.AutotracEmbarcador.AuthorizedVehicles veiculo)
        {
            string url = $"{this.conta.Protocolo}://{this.conta.Servidor}/{this.conta.URI}/{ObterHashConta()}/vehicles/{veiculo.VehicleCode}/positions?_accountCode={veiculo.AccountCode}&_fromUtc={DateTime.Now.ToUniversalTime().AddHours(-2).ToString("yyyy-MM-ddTHH:mm:ss")}&_toUtc={DateTime.Now.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss")}&limit=5000";

            string retornoVeiculosAutorizados = Request(HttpMethod.Get, url);

            Dominio.ObjetosDeValor.Embarcador.Integracao.AutotracEmbarcador.RetornoPosicoes retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.AutotracEmbarcador.RetornoPosicoes>(retornoVeiculosAutorizados);

            return retorno;
        }

        private string Request(HttpMethod method, string url)
        {
            var client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoAutotracEmbarcador));
            var request = new HttpRequestMessage(method, url);


            request.Headers.Add("Ocp-Apim-Subscription-Key", ObterChaveConta());

            var response = client.SendAsync(request).Result;
            response.EnsureSuccessStatusCode();
            var ret = response.Content.ReadAsStringAsync().Result;

            return ret;
        }


        private string ObterChaveConta()
        {
            string value = Servicos.Embarcador.Logistica.ContaIntegracao.ObterValorListaParametrosAdicionais(KEY_CHAVE, this.conta.ListaParametrosAdicionais);
            return value;
        }

        private string ObterHashConta()
        {
            string value = Servicos.Embarcador.Logistica.ContaIntegracao.ObterValorListaParametrosAdicionais(KEY_HASH, this.conta.ListaParametrosAdicionais);
            return value;
        }

        #endregion

    }
}
