using System;
using System.Collections.Generic;


namespace Servicos.Embarcador.Monitoramento.Integracoes
{
    public class IntegracaoA2 : Abstract.AbstractIntegracaoSOAP
    {

        #region Atributos privados

        private static IntegracaoA2 Instance;
        private static readonly string nameConfigSection = "A2";
        Dominio.ObjetosDeValor.Embarcador.Logistica.ContaIntegracao conta;

        #endregion

        #region Atributos privados específicos para a A2

        private DateTime dataAtual;
        private DateTime dataUltimaConsulta;

        #endregion

        #region Constantes com as chaves dos dados/configurações

        private const string KEY_DATA_ULTIMA_CONSULTA = "DataUltimaConsulta";

        #endregion

        #region Construtor privado

        private IntegracaoA2(AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente) : base(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.A2, nameConfigSection, cliente) { }

        #endregion

        #region Métodos públicos

        /**
         * Implementação de Singleton
         */
        public static IntegracaoA2 GetInstance(AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente)
        {
            if (Instance == null) Instance = new IntegracaoA2(cliente);
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
            IntegrarPosicao();
            SalvarDataUltimaConsultaNoArquivo();
        }

        #endregion

        #region Métodos privados

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
                A2Rastreadores.Evento[] posicoesA2 = BuscaUltimaPosicaoVeiculo();

                if (posicoesA2 != null)
                {
                    Log($"Recebidas {posicoesA2.Length} posicoes", 3);

                    // Converte os tele eventos para posição
                    foreach (A2Rastreadores.Evento posicao in posicoesA2)
                    {

                        posicoes.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao
                        {

                            Data = DateTime.Parse(posicao.dataEquipamento),
                            DataVeiculo = DateTime.Parse(posicao.dataGps),
                            IDEquipamento = "",//equipamento,
                            Placa = posicao.placa,
                            Latitude = posicao.latitude,
                            Longitude = posicao.longitude,
                            Velocidade = posicao.velocidade,
                            Temperatura = 0,
                            SensorTemperatura = false,
                            Descricao = posicao.localizacao,
                            Rastreador = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EnumTecnologiaRastreador.A2
                        });
                    }

                    Log($"{posicoes.Count} posicoes", 3);

                    // Extrai o maior ID entre as posições recebidas para iniciar por ela na próxima requisição
                    ExtrairDataUltimaConsulta(posicoes);
                    SalvarDataUltimaConsultaNoArquivo();
                }
                else
                {
                    Log($"Recebidas 0 posicoes", 3);
                }
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
        private A2Rastreadores.Evento[] BuscaUltimaPosicaoVeiculo()
        {
            Servicos.A2Rastreadores.InterfaceExternaServicePortTypeClient webA2 = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unitOfWork).ObterClient<Servicos.A2Rastreadores.InterfaceExternaServicePortTypeClient, Servicos.A2Rastreadores.InterfaceExternaServicePortType>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.A2_InterfaceExternaServicePortType);
            A2Rastreadores.RetornoInterfaceExterna response = webA2.obterUltimaPosicaoData(conta.Usuario.ToInt(), conta.Senha, this.dataUltimaConsulta.ToString("yyyy-MM-dd HH:mm:ss"));

            return response.eventos;
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

            Log($"Ultima data consulta {dataUltimaConsulta}", 2);

        }

        /**
         * Registra no arquivo a última data consultada. Sobrescreve o conteúdo do arquivo
         */
        private void SalvarDataUltimaConsultaNoArquivo()
        {
            List<KeyValuePair<string, string>> dadosControle = new List<KeyValuePair<string, string>>();
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
