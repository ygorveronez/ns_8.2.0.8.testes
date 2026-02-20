using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Monitoramento.Integracoes
{

    public class IntegracaoSighra : Abstract.AbstractIntegracaoDatabase
    {

        #region Atributos privados

        private static IntegracaoSighra Instance;
        private static readonly string nameConfigSection = "Sighra";
        Dominio.ObjetosDeValor.Embarcador.Logistica.ContaIntegracao conta;

        #endregion

        #region Atributos privados específicos para a Sighra

        private DateTime dataAtual;
        private DateTime dataUltimaConsulta;

        #endregion

        #region Constantes com as chaves dos dados/configurações

        private const string KEY_DATA_ULTIMA_CONSULTA = "DataUltimaConsulta";

        #endregion

        #region Construtor privado

        private IntegracaoSighra(AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente) : base(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Sighra, nameConfigSection, cliente) { }

        #endregion

        #region Métodos públicos

        /**
         * Implementação de Singleton
         */
        public static IntegracaoSighra GetInstance(AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente)
        {
            if (Instance == null) Instance = new IntegracaoSighra(cliente);
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
            dataAtual = DateTime.Now;
            ObterDataUltimaConsultaDoArquivo();
            IntegrarPosicoes();
            SalvarDataUltimaConsultaNoArquivo();
        }

        #endregion

        #region Métodos privados

        /**
         * Executa a integração das posições, consultando e registrando na tabela T_POSICAO
         */
        private void IntegrarPosicoes()
        {

            Log($"Consultando posicoes", 2);

            // Busca as posições do database
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoes = ObterPosicoes();

            // Registra as posições recebidas dos veículos
            Log($"Integrando {posicoes.Count} posicoes", 2);
            base.InserirPosicoes(posicoes);

            ExtrairDataUltimaConsulta(posicoes);
        }

        /**
         * Busca a lista de posições
         */
        private List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> ObterPosicoes()
        {
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoes = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao>();

            try
            {
                string stringConexaoMySQL = $"Server={this.conta.Servidor}; Port={this.conta.Porta}; Database={this.conta.BancoDeDados}; Uid={this.conta.Usuario}; Pwd={this.conta.Senha};";
                using (MySql.Data.MySqlClient.MySqlConnection db = new MySql.Data.MySqlClient.MySqlConnection(stringConexaoMySQL))
                {

                    // Consulta as posições do Sighra
                    List<Dominio.ObjetosDeValor.Embarcador.Integracao.Sighra.Posicao> posicoesSighra = ObterPosicoesSighra(db, dataUltimaConsulta);
                    int total = posicoesSighra.Count;
                    for (int i = 0; i < total; i++)
                    {
                        posicoes.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao()
                        {
                            IDEquipamento = posicoesSighra[i].IdVeiculo.ToString(),
                            Placa = posicoesSighra[i].Placa.Trim().Replace("-", "").ToUpper(),
                            Data = posicoesSighra[i].DataRecepcao,
                            DataVeiculo = posicoesSighra[i].DataEvento,
                            Latitude = posicoesSighra[i].Latitude,
                            Longitude = posicoesSighra[i].Longitude,
                            Velocidade = posicoesSighra[i].Velocidade,
                            Temperatura = (posicoesSighra[i].Temperatura.HasValue && posicoesSighra[i].Temperatura.Value == -50) ? null : posicoesSighra[i].Temperatura,
                            Ignicao = (posicoesSighra[i].Ignicao == 'S') ? 1 : 0,
                            Descricao = $"{posicoesSighra[i].Latitude};{posicoesSighra[i].Longitude}",
                            Rastreador = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EnumTecnologiaRastreador.Sighra
                        });
                    }
                    Log($"{posicoes.Count} posicoes encontradas", 3);
                }
            }
            catch (Exception ex)
            {
                Log("Erro ObterPosicoesSighra " + ex.ToString(), 3);
            }
            return posicoes;
        }

        /**
         * Busca as posições de todos os veículos da base de dados do Sighra
         */
        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.Sighra.Posicao> ObterPosicoesSighra(MySql.Data.MySqlClient.MySqlConnection db, DateTime dataInicial)
        {
            string sql = @"
                SELECT
                    lupo_cvei_id IdVeiculo,
                    cvei_placa Placa,
                    lupo_data_gps DataEvento,
                    lupo_data_status DataRecepcao,
                    lupo_latitude Latitude,
                    lupo_longitude Longitude,
                    lupo_ignicao Ignicao,
                    lupo_velocidade Velocidade,
                    lupo_ltemp_1 Temperatura
                FROM
                    log_ultima_posicao
                JOIN
                    cad_veiculo ON cad_veiculo.cvei_id = log_ultima_posicao.lupo_cvei_id
                WHERE 
                    lupo_data_gps > @data
                ORDER BY
                    lupo_data_gps";

            object param = new { data = dataInicial };
            var posicoesSighra = db.Query<Dominio.ObjetosDeValor.Embarcador.Integracao.Sighra.Posicao>(sql, param).ToList();
            return ConversoesSighra(posicoesSighra);
        }

        /**
         * Conversões
         */
        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.Sighra.Posicao> ConversoesSighra(List<Dominio.ObjetosDeValor.Embarcador.Integracao.Sighra.Posicao> posicoesSighra)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.Sighra.Posicao> novasPosicoesSighra = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Sighra.Posicao>();
            int total = posicoesSighra?.Count ?? 0;
            for (int i = 0; i < total; i++)
            {
                // Conversão da temperatura conforme padrão da Sighra
                if (posicoesSighra[i].Temperatura != null) posicoesSighra[i].Temperatura = (posicoesSighra[i].Temperatura - 100) / 2;

                novasPosicoesSighra.Add(posicoesSighra[i]);
            }
            return novasPosicoesSighra;
        }

        /**
         * Busca a última data consultada
         */
        private void ObterDataUltimaConsultaDoArquivo()
        {
            string value;
            List<KeyValuePair<string, string>> dadosControle = base.CarregarDadosDeControleDoArquivo(this.conta);

            value = Servicos.Embarcador.Logistica.ContaIntegracao.ObterValorListaParametrosAdicionais(KEY_DATA_ULTIMA_CONSULTA, dadosControle);
            try
            {
                this.dataUltimaConsulta = DateTime.Parse(value);
            }
            catch
            {
                this.dataUltimaConsulta = this.dataAtual;
            }

            if (this.dataUltimaConsulta > this.dataAtual.AddMinutes(10))
                this.dataUltimaConsulta = this.dataAtual;

            Log($"Ultima data consulta {dataUltimaConsulta}", 2);

        }

        /**
         * Registra no arquivo a última data consultada. Sobrescreve o conteúdo do arquivo
         */
        private void SalvarDataUltimaConsultaNoArquivo()
        {
            List<KeyValuePair<string, string>> dadosControle = new List<KeyValuePair<string, string>>();

            if (this.dataUltimaConsulta != DateTime.MinValue && this.dataUltimaConsulta > DateTime.Now)
                this.dataUltimaConsulta = DateTime.Now.AddMinutes(-10);

            dadosControle.Add(new KeyValuePair<string, string>(KEY_DATA_ULTIMA_CONSULTA, this.dataUltimaConsulta.ToString()));
            base.SalvarDadosDeControleNoArquivo(this.conta, dadosControle);
            Log($"Atualizando ultima data consulta {this.dataUltimaConsulta}", 2);
        }

        private void ExtrairDataUltimaConsulta(List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoes)
        {
            int cont = posicoes.Count;
            for (int i = 0; i < cont; i++)
            {
                if (posicoes[i].DataVeiculo > this.dataUltimaConsulta)
                {
                    this.dataUltimaConsulta = posicoes[i].DataVeiculo;
                }
            }
        }

        #endregion

    }

}
