using Dominio.Excecoes.Embarcador;
using Infrastructure.Services.HttpClientFactory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;

namespace Servicos.Embarcador.Integracao.Nstech
{
    public class IntegracaoPosicoes
    {
        #region Atributos privados

        public string token { get; set; }
        public int validadeTokenSegundos { get; set; }
        public DateTime validadeToken { get; set; }
        private static IntegracaoPosicoes Instance;
        private Servicos.InspectorBehavior inspector;

        private string url;
        private string usuario;
        private string senha;
        private string solicitanteId;
        private string solicitanteSenha;
        private string URI;
        private string CNPJConsulta;
        private long UltimoSequencial;

        private Dominio.ObjetosDeValor.Embarcador.Enumeradores.EnumTecnologiaGerenciadora rastreadorId;

        #endregion

        #region Construtor privado

        private IntegracaoPosicoes() { }

        #endregion

        #region Métodos públicos

        public static IntegracaoPosicoes GetInstance()
        {
            if (Instance == null) Instance = new IntegracaoPosicoes();
            return Instance;
        }

        public Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.Rastreamento.Token_detail ObterToken()
        {
            VerificarConfiguracoes();

            Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.Autenticacao handshakeAut = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.Autenticacao();
            handshakeAut.Senha = this.solicitanteSenha;
            handshakeAut.ID = this.solicitanteId;
            string jsonRequestBody = JsonConvert.SerializeObject(handshakeAut, Formatting.Indented);

            // Request
            string jsonResponse = Request(jsonRequestBody);

            dynamic objetoRetorno = JsonConvert.DeserializeObject<dynamic>(jsonResponse);

            if ((bool)objetoRetorno?.verify == true)
            {
                if (!string.IsNullOrWhiteSpace((string)objetoRetorno?.token_detail?.access_token))
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.Rastreamento.Token_detail dadosToken = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.Rastreamento.Token_detail { 
                        Access_token = (string)objetoRetorno.token_detail.access_token,
                        Expires_in = (int)objetoRetorno.token_detail.expires_in
                    };
                    return dadosToken;
                }

                else
                    throw new WebServiceException("Autenticação não retornou token.");
            }
            else
            {
                throw new WebServiceException("Não foi possível obter Token " + objetoRetorno?.msg);
            }
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.Rastreamento.Posicoes> BuscarUltimasPosicoes(ref string jsonResponse, Repositorio.UnitOfWork unitOfWork)
        {
            VerificarConfiguracoes();

            Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.Rastreamento.Rastreamento handshakeRast = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.Rastreamento.Rastreamento();
            handshakeRast.ID = this.solicitanteId;
            handshakeRast.Token = this.token;
            handshakeRast.Rastreador = (int)this.rastreadorId;
            handshakeRast.Fornecedor = (int)this.rastreadorId;
            handshakeRast.Usuario = this.usuario;
            handshakeRast.Senha = this.senha;

            if (this.rastreadorId == Dominio.ObjetosDeValor.Embarcador.Enumeradores.EnumTecnologiaGerenciadora.Trafegus)
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.Rastreamento.Filtro handshakeFiltro = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.Rastreamento.Filtro();
                handshakeFiltro.UltimaPosicao = "S";
                handshakeFiltro.Sequencial = this.UltimoSequencial <= 0 ? 100 : this.UltimoSequencial;
                handshakeFiltro.terminal_associado = "S";

                handshakeRast.TagsLogin = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.Rastreamento.TagsLogin();
                handshakeRast.TagsLogin.Dominio = "1";
                handshakeRast.TagsLogin.IppPorta = this.URI;
                handshakeRast.Filtro = handshakeFiltro;
            }

            string jsonRequestBody = JsonConvert.SerializeObject(handshakeRast, Formatting.Indented);


            // Request
            jsonResponse = Request(jsonRequestBody);

            dynamic objetoRetorno = JsonConvert.DeserializeObject<dynamic>(jsonResponse);

            bool ambienteProducao = Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().AmbienteProducao;
            if (ambienteProducao)
            {
                if (objetoRetorno.body != null)
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.Rastreamento.retornoPosicoes retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.Rastreamento.retornoPosicoes>(jsonResponse);
                    return retorno.body;
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(objetoRetorno?.errormsg?.mfg))
                        throw new WebServiceException(objetoRetorno.errormsg.mfg);
                    else
                        throw new WebServiceException("Falha ao obter posições");
                }
            }
            else
            {
                //Sim em dev eles respondem diferente -.-.
                Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.Rastreamento.RetornoPosicoesDev retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.Rastreamento.RetornoPosicoesDev>(jsonResponse);
                List<Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.Rastreamento.Posicoes> posicoes = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.Rastreamento.Posicoes>();

                foreach (var ponto in retorno.body.pontos_rastreamento)
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.Rastreamento.Posicoes novaPosicao = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.Rastreamento.Posicoes
                    {
                        numero = ponto.numero,
                        dataEnvio = ponto.data_ocorrencia,
                        dataLocalizacao = ponto.data_ocorrencia,
                        placaCavalo = ponto.id_veiculo?.Trim().Replace("-", ""),
                        numeroTerminalEquipamento = ponto.id_device,
                        latitudeLocalizacao = ponto.latitude,
                        longitudeLocalizacao = ponto.longitude,
                        nomeTecnologia = ponto.tecnologia,
                        velocidadeVeiculo = ponto.velocidade_veiculo,
                    };

                    posicoes.Add(novaPosicao);
                }

                return posicoes;
            }


        }
        public void DefinirConfiguracoes(Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.ParametrosConfiguracaoRastreamento parametros)
        {
            if (!string.IsNullOrEmpty(parametros.url))
                this.url = parametros.url;

            if (!string.IsNullOrEmpty(parametros.usuario))
                this.usuario = parametros.usuario;

            if (!string.IsNullOrEmpty(parametros.senha))
                this.senha = parametros.senha;

            if (!string.IsNullOrEmpty(parametros.solicitanteId))
                this.solicitanteId = parametros.solicitanteId;

            if (!string.IsNullOrEmpty(parametros.solicitanteSenha))
                this.solicitanteSenha = parametros.solicitanteSenha;

            if (!string.IsNullOrEmpty(parametros.URI))
                this.URI = parametros.URI;

            if (!string.IsNullOrEmpty(parametros.CNPJConsulta))
                this.CNPJConsulta = parametros.CNPJConsulta;

            if (parametros.UltimoSequencial > 0)
                this.UltimoSequencial = parametros.UltimoSequencial;

            this.rastreadorId = parametros.rastreadorId.Value;
        }

        public void DefinirToken(string token)
        {
            this.token = token;
        }

        public void DefinirValidadeTokenSegundos(int validade)
        {
            this.validadeTokenSegundos = validade;
        }

        public void DefinirValidadeToken(DateTime data)
        {
            this.validadeToken = data;
        }
        #endregion

        #region Métodos privados

        private string Request(string body)
        {
            // Requisição
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoPosicoes));
            client.BaseAddress = new Uri(this.url);
            client.DefaultRequestHeaders.Accept.Clear();

            StringContent content;

            content = new StringContent(body, Encoding.UTF8, "application/json");
            var result = client.PostAsync(url, content).Result;

            //Tratamento para seguir o redirect de requisições, quando o retorno for Permanent Redirect (308).
            if ((int)result.StatusCode == 308)
            {
                client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoPosicoes));
                client.BaseAddress = new Uri(result.Headers.Location.ToString());
                client.DefaultRequestHeaders.Accept.Clear();
                content = new StringContent(body, Encoding.UTF8, "application/json");
                result = client.PostAsync(client.BaseAddress, content).Result;
            }

            // Leitura da resposta
            string response = result.Content.ReadAsStringAsync().Result;

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

            return response;
        }
        private void VerificarConfiguracoes()
        {
            if (string.IsNullOrWhiteSpace(this.url)) throw new ServicoException("URL NSTech não definida");
            if (string.IsNullOrWhiteSpace(this.usuario)) throw new ServicoException("Usuario da conta acesso a posições NSTech não definido");
            if (string.IsNullOrWhiteSpace(this.senha)) throw new ServicoException("Senha da conta acesso a posições NSTech não definida");
            if (string.IsNullOrWhiteSpace(this.solicitanteSenha)) throw new ServicoException("solicitanteSenha NSTech não definida");
            if (string.IsNullOrWhiteSpace(this.solicitanteId)) throw new ServicoException("solicitanteId NSTech não definida");
        }

        #endregion

    }

}