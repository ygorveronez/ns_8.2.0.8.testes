using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Logistica;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Servicos.Embarcador.Monitoramento.Integracoes
{
    public class IntegracaoAutovision : Abstract.AbstractIntegracaoREST
    {
        #region Atributos privados

        private static IntegracaoAutovision Instance;
        private static readonly string nameConfigSection = "Autovision";
        Dominio.ObjetosDeValor.Embarcador.Logistica.ContaIntegracao conta;
        private const string KEY_TOKEN = "partialToken";
        #endregion

        #region Construtor privado

        private IntegracaoAutovision(AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente) : base(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Autovision, nameConfigSection, cliente) { }

        #endregion

        #region Métodos públicos

        /**
         * Implementação de Singleton
         */
        public static IntegracaoAutovision GetInstance(AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente)
        {
            if (Instance == null) Instance = new IntegracaoAutovision(cliente);
            return Instance;
        }

        #endregion

        #region Métodos abstratos obrigatórios

        protected override void ComplementarConfiguracoes()
        {

        }
        protected override void Validar()
        {
            base.ValidarConfiguracaoDiretorioEArquivoControle(base.contasIntegracao);
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
                List<Dominio.ObjetosDeValor.Embarcador.Integracao.Autovision.DadosRetorno> retornoRequisicao = BuscarPosicoesAutovision();

                Log($"Recebidas posiçoes de {retornoRequisicao.Count} veículos", 3);

                foreach (Dominio.ObjetosDeValor.Embarcador.Integracao.Autovision.DadosRetorno p in retornoRequisicao)
                {
                    posicoes.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao
                    {

                        Data = DateTime.Parse(p.ReceptionDate),
                        DataCadastro = DateTime.Now,
                        DataVeiculo = DateTime.Parse(p.Date),
                        IDEquipamento = p.ModuloID,
                        Placa = p.EquipmentGpsUnitId,
                        Latitude = double.Parse(p.Position.Latitude.Replace(".",",")),
                        Longitude = double.Parse(p.Position.Longitude.Replace(".", ",")),
                        Velocidade = p.Speed,
                        Temperatura = 0,
                        SensorTemperatura = false,                        
                        km = p.Odometer,
                        Ignicao = p.EngineOn == true ? 1 : 0,
                        Rastreador = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EnumTecnologiaRastreador.Autovision
                    });

                }

                Log($"{posicoes.Count} posicoes", 3);

            }
            catch (Exception ex)
            {
                Log("Erro ObterPosicoes " + ex.Message, 3);
            }

            return posicoes;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.Autovision.DadosRetorno> BuscarPosicoesAutovision(){
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.Autovision.DadosRetorno> retorno = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Autovision.DadosRetorno>();            
            try
            {   
                string responseJson = Request();

                string stringsalvarLog = ObterValorMonitorar("SalvarLogResponse");
                bool salvarLog = (!string.IsNullOrWhiteSpace(stringsalvarLog)) && bool.Parse(stringsalvarLog);
                if (salvarLog)
                    LogNomeArquivo(responseJson, DateTime.Now, "ResponsePosicoesAutoVision", 0, true);

                retorno = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Integracao.Autovision.DadosRetorno>>(responseJson);
            }
            catch (Exception ex)
            {
                Log("Erro ObterPosicoes " + ex.Message, 3);
            }
            return retorno;
        }

        /**
        * Executa a requisição ao WebService
        */
        private string Request()
        {
            string response = "";
            string token = ObterToken();
            // Monta a URL para a requisição
            var url = (conta.Protocolo == Protocolo.HTTPS ? "https" : "http") + "://" + conta.Servidor + "/" + conta.URI + "?partialToken=" + token;

            // Requisição
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";

            // Headers das requisição            
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
                Log($"Requisicao obter veiculos", inicio, 3);

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


        private string ObterToken()
        {
            try
            {
                return Servicos.Embarcador.Logistica.ContaIntegracao.ObterValorListaParametrosAdicionais(KEY_TOKEN, this.conta.ListaParametrosAdicionais);                
            }
            catch
            {
                return "";
            }
        }

        #endregion
    }
}
