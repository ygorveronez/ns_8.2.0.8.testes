using System;
using System.Collections.Generic;

namespace Servicos.Embarcador.Monitoramento.Integracoes
{

    public class IntegracaoUnitop : Abstract.AbstractIntegracaoREST
    {
        #region Atributos privados

        private static IntegracaoUnitop Instance;
        private static readonly string nameConfigSection = "Unitop";
        private Dominio.ObjetosDeValor.Embarcador.Logistica.ContaIntegracao conta;

        private Servicos.Embarcador.Integracao.Unitop.IntegracaoUnitop ServicoIntegracaoUnitop;

        #endregion

        #region Constantes com as chaves dos dados/configurações

        private const string KEY_CLASS = "Class";
        private const string KEY_METHOD = "Method";
        private const string KEY_TOKEN = "Token";

        #endregion

        #region Construtor privado

        private IntegracaoUnitop(AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente) : base(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Unitop, nameConfigSection, cliente) { }

        #endregion

        #region Métodos públicos

        /**
         * Implementação de Singleton
         */
        public static IntegracaoUnitop GetInstance(AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente)
        {
            if (Instance == null) Instance = new IntegracaoUnitop(cliente);
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
            this.ServicoIntegracaoUnitop = Servicos.Embarcador.Integracao.Unitop.IntegracaoUnitop.GetInstance();
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
                this.ServicoIntegracaoUnitop.SetURL(Servicos.Embarcador.Logistica.ContaIntegracao.ObterURL(this.conta));
                this.ServicoIntegracaoUnitop.SetSolicitanteId(this.conta.SolicitanteId);
                this.ServicoIntegracaoUnitop.SetToken(Servicos.Embarcador.Logistica.ContaIntegracao.ObterValorListaParametrosAdicionais(KEY_TOKEN, this.conta.ListaParametrosAdicionais));
                this.ServicoIntegracaoUnitop.SetClasse(Servicos.Embarcador.Logistica.ContaIntegracao.ObterValorListaParametrosAdicionais(KEY_CLASS, this.conta.ListaParametrosAdicionais));
                this.ServicoIntegracaoUnitop.SetMethod(Servicos.Embarcador.Logistica.ContaIntegracao.ObterValorListaParametrosAdicionais(KEY_METHOD, this.conta.ListaParametrosAdicionais));

                // Autenticação
                this.ServicoIntegracaoUnitop.MontarUriConsulta();
                if (this.ServicoIntegracaoUnitop.VerificarUrlConsulta())
                {
                    // Busca os eventos normais, que contém as posições dos veículos
                    List<Dominio.ObjetosDeValor.Embarcador.Integracao.Unitop.Veiculo> veiculos = this.ServicoIntegracaoUnitop.BuscarLocalizacoes();
                    int total = veiculos.Count;
                    Log($"Recebidas {total} veiculos", 3);

                    for (int i = 0; i < total; i++)
                    {
                        string[] placas = veiculos[i].Placa.Trim().Split(' ');
                        int totalPlacas = placas.Length;
                        for (int j = 0; j < totalPlacas; j++)
                        {
                            string placa = placas[j].Trim().ToUpper().Replace("-", "");
                            DateTime data = DateTime.ParseExact($"{veiculos[i].DataMovimento} {veiculos[i].Hora}", "yyyy-MM-dd HH:mm:ss", null);
                            posicoes.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao
                            {
                                Data = data,
                                DataVeiculo = data,
                                Placa = placa,
                                Latitude = veiculos[i].Latitude ?? 0,
                                Longitude = veiculos[i].Longitude ?? 0,
                                Velocidade = veiculos[i].Velocidade,
                                Ignicao = veiculos[i].Ignicao == "L" ? 1 : 0,
                                SensorTemperatura = false,
                                IDEquipamento = placa,
                                Rastreador = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EnumTecnologiaRastreador.Unitop
                            });
                        }
                    }
                    Log($"{posicoes.Count} posicoes", 3);

                }
                else
                {
                    Log("Token invalido", 3);
                }

            }
            catch (Exception ex)
            {
                Log("Erro ObterPacotePosicoes " + ex.Message, 3);
            }

            return posicoes;
        }

        #endregion

    }

}