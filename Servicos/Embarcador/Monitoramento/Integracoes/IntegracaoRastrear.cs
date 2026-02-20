using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Monitoramento.Integracoes
{
    public class IntegracaoRastrear : Abstract.AbstractIntegracaoSOAP
    {
        #region Atributos privados

        private static IntegracaoRastrear Instance;
        private string token;
        private DateTime validadeToken;
        private static readonly string nameConfigSection = "Rastrear";
        private Dominio.ObjetosDeValor.Embarcador.Logistica.ContaIntegracao conta;

        private Servicos.Embarcador.Integracao.Rastrear.IntegracaoRastrear ServicoIntegracaoRastrear;

        #endregion

        #region Constantes com as chaves dos dados/configurações
        private const string KEY_TOKEN = "Token";
        private const string KEY_VALIDADE_TOKEN = "ValidadeToken";
        #endregion

        #region Construtor privado

        private IntegracaoRastrear(AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente) : base(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Rastrear, nameConfigSection, cliente) { }

        #endregion

        #region Métodos públicos

        public static IntegracaoRastrear GetInstance(AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente)
        {
            if (Instance == null) Instance = new IntegracaoRastrear(cliente);
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
            base.ValidarConfiguracaoDeContasRastrear(base.contasIntegracao);
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
            this.ServicoIntegracaoRastrear = Integracao.Rastrear.IntegracaoRastrear.GetInstance();
            ObterToken();
            IntegrarPosicao();
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
                parametrosIntegracao.rastreadorId = this.conta.RastreadorId.ToEnum(Dominio.ObjetosDeValor.Embarcador.Enumeradores.EnumTecnologiaGerenciadora.NaoDefinido);

                this.ServicoIntegracaoRastrear.DefinirConfiguracoes(parametrosIntegracao);
            }
            catch (Exception e)
            {
                Log($"Erro ao ComplementarConfiguracoes {e.Message}", 2);
            }
        }

        private void ObterToken()
        {
            Log("Obtendo Token", 2);

            InicializarIntegracao("Auth");

            this.ServicoIntegracaoRastrear.DefinirToken("");

            DateTime inicio = DateTime.UtcNow;
            ObterTokenArmazenado();

            if (!TokenValido())
            {
                Log($"Requisitando Token", inicio, 3);

                Dominio.ObjetosDeValor.Embarcador.Integracao.Rastrear.AutReturn resutToken = this.ServicoIntegracaoRastrear.ObterToken();
                this.ServicoIntegracaoRastrear.DefinirToken(resutToken.tokenInfo.accessToken);

                this.validadeToken = resutToken.tokenInfo.expiration.ToDateTime("yyyy-MM-dd HH:mm:ss");
                this.token = resutToken.tokenInfo.accessToken;

                this.ServicoIntegracaoRastrear.DefinirValidadeToken(this.validadeToken);
            }
            else
            {
                Log($"Token Armazenado", inicio, 3);

                this.ServicoIntegracaoRastrear.DefinirToken(this.token);
                this.ServicoIntegracaoRastrear.DefinirValidadeToken(this.validadeToken);
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

                DateTime inicio = DateTime.UtcNow;

                Log($"Requisicao Posicoes", inicio, 3);

                Log($"Consultando {ListaVeiculosMonitorados.Count} posicoes", 3);

                for (int i = 0; i <= ListaVeiculosMonitorados.Count - 1; i++)
                {
                    var veiculoConsultar = ListaVeiculosMonitorados[i];

                    Log($"Consultando placa '{veiculoConsultar?.Placa}'", 5);

                    InicializarIntegracao($"Posicao/GetPosicoes?nrPlaca={veiculoConsultar?.Placa}");

                    Dominio.ObjetosDeValor.Embarcador.Integracao.Rastrear.ResponsePosicoes resutPosicoes = this.ServicoIntegracaoRastrear.ObterPoscioesVeiculo();

                    Log($"Tratando placa '{veiculoConsultar?.Placa}'", 5);

                    var jsonResponse = JsonConvert.SerializeObject(resutPosicoes, Formatting.None);

                    string stringsalvarLog = ObterValorMonitorar("SalvarLogResponse");
                    bool salvarLog = (!string.IsNullOrWhiteSpace(stringsalvarLog)) && bool.Parse(stringsalvarLog);

                    if (resutPosicoes != null && resutPosicoes.posicoes.Count() > 0)
                    {
                        try
                        {
                            Log($"Tratando placa '{veiculoConsultar?.Placa}'", 5);

                            if (salvarLog)
                                LogNomeArquivo("Placa: " + veiculoConsultar?.Placa + " - " + jsonResponse, DateTime.Now, "ResponsePosicoesRastrear", 0, true);

                            // Mapeamento da posição do embarcador para a posição de logística
                            Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao posicaoSalvar = MapearPosicao(resutPosicoes.posicoes[0], this.conta.RastreadorId);

                            // Adicionar à lista de posições
                            posicoes.Add(posicaoSalvar);
                        }
                        catch (Exception ex)
                        {
                            // Log de exceção
                            Log($"Erro no processamento da posição: {ex.ToString()}", 4);
                            Log($"Dados da posição com erro: {resutPosicoes.posicoes[0]}", 4);
                            // Considerar o que fazer em caso de erro, por exemplo, continuar ou interromper
                            continue; // Se necessário
                        }
                    }
                    else
                    {
                        if (salvarLog)
                            LogNomeArquivo("Placa: " + veiculoConsultar?.Placa + " \"posicoes\": []", DateTime.Now, "ResponsePosicoesRastrear", 0);
                    }
                }
                Log($"{posicoes.Count} posicoes", 3);
            }
            catch (Exception e)
            {
                Log($"Erro BuscarUltimasPosicoes " + e.Message, 3);
            }


            return posicoes;
        }

        // Função para mapear uma posição do Embarcador para a Posição de Logística
        private Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao MapearPosicao(Dominio.ObjetosDeValor.Embarcador.Integracao.Rastrear.Posicao posicao, string rastreadorId)
        {
            // Mapeamento da posição
            Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao posicaoSalvar = new Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao()
            {
                DataCadastro = DateTime.Now,
                Placa = posicao.nrPlacaCavalo.Replace("-", ""),
                Descricao = "",
                Ignicao = posicao.flIgnicao ? 1 : 0,
                Gerenciadora = rastreadorId.ToEnum(Dominio.ObjetosDeValor.Embarcador.Enumeradores.EnumTecnologiaGerenciadora.NaoDefinido),
                Data = posicao.dtReceb,
                DataVeiculo = posicao.dtPos,
                Velocidade = posicao.vlVelocidade
            };

            try
            {
                string lat = (string)posicao.vlLat;
                string lon = (string)posicao.vlLong;

                posicaoSalvar.Temperatura = 0;
                posicaoSalvar.SensorTemperatura = false;
                posicaoSalvar.Latitude = lat.ToDouble();
                posicaoSalvar.Longitude = lon.ToDouble();
                posicaoSalvar.IDEquipamento = posicao.cdViag > 0 ? posicao.cdViag.ToString() : "99";
            }
            catch (Exception ex)
            {
                Log($"Erro no mapeamento: {ex.ToString()}", 4);
                throw;
            }

            return posicaoSalvar;
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


        #endregion

    }

}
