using System;
using System.Collections.Generic;

namespace Servicos.Embarcador.Monitoramento.Integracoes
{
    public class IntegracaoNSTech : Abstract.AbstractIntegracaoSOAP
    {
        #region Atributos privados

        private static IntegracaoNSTech Instance;
        private string token;
        private DateTime validadeToken;
        private static readonly string nameConfigSection = "NSTech";
        private Dominio.ObjetosDeValor.Embarcador.Logistica.ContaIntegracao conta;

        private long ultimoSequencial;

        private Servicos.Embarcador.Integracao.Nstech.IntegracaoPosicoes ServicoIntegracaoNSTech;

        #endregion

        #region Constantes com as chaves dos dados/configurações
        private const string KEY_TOKEN = "Token";
        private const string KEY_VALIDADE_TOKEN = "ValidadeToken";
        private const string KEY_ULTIMO_SEQUENCIAL = "UltimoSequencial";
        private const string KEY_CNPJ_CONSULTA = "CNPJ";
        private const string KEY_URL_CONSULTA = "URL";

        #endregion

        #region Construtor privado

        private IntegracaoNSTech(AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente) : base(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NSTech, nameConfigSection, cliente) { }

        #endregion

        #region Métodos públicos

        public static IntegracaoNSTech GetInstance(AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente)
        {
            if (Instance == null) Instance = new IntegracaoNSTech(cliente);
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
            base.ValidarConfiguracaoDeContasNSTech(base.contasIntegracao);
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
            this.ServicoIntegracaoNSTech = Integracao.Nstech.IntegracaoPosicoes.GetInstance();


            if (this.conta.Nome == "Trafegus")
                ObterUltimosSequenciaisDoArquivo();

            ObterToken();

            IntegrarPosicao();

            if (this.conta.Nome == "Trafegus")
                SalvarUltimosSequenciaisNoArquivo();
        }

        #endregion

        #region Métodos privados

        private void InicializarIntegracao(string uri)
        {
            try
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.ParametrosConfiguracaoRastreamento parametrosIntegracao = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.ParametrosConfiguracaoRastreamento();
                string url = Servicos.Embarcador.Logistica.ContaIntegracao.ObterURL(this.conta);
                url += $"{uri}";

                parametrosIntegracao.url = url;
                parametrosIntegracao.usuario = this.conta.Usuario;
                parametrosIntegracao.senha = this.conta.Senha;
                parametrosIntegracao.solicitanteSenha = this.conta.SolicitanteSenha;
                parametrosIntegracao.solicitanteId = this.conta.SolicitanteId;
                parametrosIntegracao.rastreadorId = this.conta.RastreadorId.ToEnum(Dominio.ObjetosDeValor.Embarcador.Enumeradores.EnumTecnologiaGerenciadora.NaoDefinido);
                parametrosIntegracao.UltimoSequencial = this.ultimoSequencial;

                if (parametrosIntegracao.rastreadorId == Dominio.ObjetosDeValor.Embarcador.Enumeradores.EnumTecnologiaGerenciadora.Trafegus)
                {
                    parametrosIntegracao.CNPJConsulta = Servicos.Embarcador.Logistica.ContaIntegracao.ObterValorListaParametrosAdicionais(KEY_CNPJ_CONSULTA, this.conta.ListaParametrosAdicionais);
                    parametrosIntegracao.URI = Servicos.Embarcador.Logistica.ContaIntegracao.ObterValorListaParametrosAdicionais(KEY_URL_CONSULTA, this.conta.ListaParametrosAdicionais);
                }

                this.ServicoIntegracaoNSTech.DefinirConfiguracoes(parametrosIntegracao);
            }
            catch (Exception e)
            {
                Log($"Erro ao ComplementarConfiguracoes {e.Message}", 2);
            }
        }

        private void ObterToken()
        {
            Log("Obtendo Token", 2);

            InicializarIntegracao("gera/v1"); //"autenticacao-grs"
            this.ServicoIntegracaoNSTech.DefinirToken("");

            DateTime inicio = DateTime.UtcNow;
            ObterTokenArmazenado();
            if (!TokenValido())
            {
                Log($"Requisicao Token", inicio, 3);
                Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.Rastreamento.Token_detail dadosToken = this.ServicoIntegracaoNSTech.ObterToken();
                this.ServicoIntegracaoNSTech.DefinirToken(dadosToken.Access_token);
                this.ServicoIntegracaoNSTech.DefinirValidadeTokenSegundos(dadosToken.Expires_in);
                this.validadeToken = DateTime.Now.AddSeconds(dadosToken.Expires_in);
                this.token = dadosToken.Access_token;
                this.ServicoIntegracaoNSTech.DefinirValidadeToken(this.validadeToken);
                SalvarUltimosSequenciaisNoArquivo();
            }
            else
            {
                this.ServicoIntegracaoNSTech.DefinirToken(this.token);
                this.ServicoIntegracaoNSTech.DefinirValidadeToken(this.validadeToken);
            }

        }

        private void IntegrarPosicao()
        {

            Log($"Consultando posicoes", 2);

            // Busca as posições do WebService
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoes = ObterPosicoes();

            // Registra as posições recebidas dos veículos
            Log($"Integrando {posicoes.Count} posicoes", 2);
            base.InserirPosicoes(posicoes);

        }

        private List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> ObterPosicoes()
        {
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoes = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao>();

            try
            {
                InicializarIntegracao("rast/v1"); //"rastreamento"

                DateTime inicio = DateTime.UtcNow;
                Log($"Requisicao Posicoes", inicio, 3);

                string jsonResponse = string.Empty;
                List<Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.Rastreamento.Posicoes> ultimasPosicoes = this.ServicoIntegracaoNSTech.BuscarUltimasPosicoes(ref jsonResponse, unitOfWork);

                string stringsalvarLog = ObterValorMonitorar("SalvarLogResponse");
                bool salvarLog = (!string.IsNullOrWhiteSpace(stringsalvarLog)) && bool.Parse(stringsalvarLog);
                if (salvarLog)
                    LogNomeArquivo(jsonResponse, DateTime.Now, "ResponsePosicoesNstech", 0, true);

                int total = ultimasPosicoes?.Count ?? 0;
                for (int i = 0; i < total; i++)
                {
                    if (ultimasPosicoes[i].dataLocalizacao.HasValue && ultimasPosicoes[i].latitudeLocalizacao != null && ultimasPosicoes[i].longitudeLocalizacao != null)
                    {
                        Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao novaPosicao = new Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao
                        {
                            ID = ultimasPosicoes[i].numero ?? 0,
                            Data = ultimasPosicoes[i].dataLocalizacao.Value,
                            DataVeiculo = ultimasPosicoes[i].dataLocalizacao.Value,
                            Placa = ultimasPosicoes[i].placaCavalo == null ? ultimasPosicoes[i].numeroTerminalEquipamento?.Trim().Replace("-", "") : ultimasPosicoes[i].placaCavalo?.Trim().Replace("-", ""),
                            IDEquipamento = !string.IsNullOrEmpty(ultimasPosicoes[i].numeroTerminalEquipamento) ? ultimasPosicoes[i].numeroTerminalEquipamento : "000",
                            Latitude = ultimasPosicoes[i].latitudeLocalizacao.ToDouble(),
                            Longitude = ultimasPosicoes[i].longitudeLocalizacao.ToDouble(),
                            Descricao = "",
                            Velocidade = ultimasPosicoes[i].velocidadeVeiculo != null ? (int)ultimasPosicoes[i].velocidadeVeiculo : 0,
                            SensorTemperatura = ultimasPosicoes[i].temperatura != null && ultimasPosicoes[i].temperatura > 0,
                            Temperatura = ultimasPosicoes[i].temperatura,
                            Rastreador = this.ObterEnumRastreadorDescricao(ultimasPosicoes[i].nomeTecnologia),
                            Gerenciadora = this.conta.RastreadorId.ToEnum(Dominio.ObjetosDeValor.Embarcador.Enumeradores.EnumTecnologiaGerenciadora.NaoDefinido)
                        };

                        posicoes.Add(novaPosicao);
                    }
                }

                Log($"{ultimasPosicoes.Count} posicoes", 3);

            }
            catch (Exception e)
            {
                Log($"Erro BuscarUltimasPosicoes " + e.Message, 3);
            }

            if (conta.Nome == "Trafegus")
                ultimoSequencial = ObtemUltimoSequencial(ultimoSequencial, posicoes);

            return posicoes;
        }

        private void ObterUltimosSequenciaisDoArquivo()
        {
            string value;
            List<KeyValuePair<string, string>> dadosControle = base.CarregarDadosDeControleDoArquivo(this.conta);

            value = Servicos.Embarcador.Logistica.ContaIntegracao.ObterValorListaParametrosAdicionais(KEY_ULTIMO_SEQUENCIAL, dadosControle);
            this.ultimoSequencial = string.IsNullOrEmpty(value) ? 0 : long.Parse(value);

            Log($"Lido ultimo sequencial {ultimoSequencial}", 2);
        }

        private bool TokenValido()
        {
            return !string.IsNullOrEmpty(this.token) && this.validadeToken > DateTime.Now;
        }

        private void ObterTokenArmazenado()
        {
            string value;
            List<KeyValuePair<string, string>> dadosControle = base.CarregarDadosDeControleDoArquivo(this.conta);
            value = Servicos.Embarcador.Logistica.ContaIntegracao.ObterValorListaParametrosAdicionais(KEY_TOKEN, dadosControle);
            this.token = string.IsNullOrEmpty(value) ? "" : value;

            value = Servicos.Embarcador.Logistica.ContaIntegracao.ObterValorListaParametrosAdicionais(KEY_VALIDADE_TOKEN, dadosControle);
            this.validadeToken = string.IsNullOrEmpty(value) ? DateTime.Now.AddMinutes(-5) : DateTime.Parse(value);

            Log($"Lido token salvo", 2);
        }

        /**
         * Registra no arquivo os últimos números sequenciais dos eventos
         */
        private void SalvarUltimosSequenciaisNoArquivo()
        {
            List<KeyValuePair<string, string>> dadosControle = new List<KeyValuePair<string, string>>();
            dadosControle.Add(new KeyValuePair<string, string>(KEY_ULTIMO_SEQUENCIAL, ultimoSequencial.ToString()));
            dadosControle.Add(new KeyValuePair<string, string>(KEY_TOKEN, this.token));
            dadosControle.Add(new KeyValuePair<string, string>(KEY_VALIDADE_TOKEN, this.validadeToken.ToString("dd/MM/yyyy HH:mm:ss")));
            base.SalvarDadosDeControleNoArquivo(this.conta, dadosControle);
            Log($"Salvo ultimo sequencial: {ultimoSequencial}, Token e validadeToken", 2);
        }



        private long ObtemUltimoSequencial(long ultimoID, List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoes)
        {
            int cont = posicoes.Count;
            for (int i = 0; i < cont; i++)
            {
                if (posicoes[i].ID > ultimoID)
                {
                    ultimoID = posicoes[i].ID;
                }
            }
            return ultimoID;
        }

        #endregion

    }

}
