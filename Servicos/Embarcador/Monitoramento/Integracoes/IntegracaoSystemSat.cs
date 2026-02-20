using System;
using System.Collections.Generic;

namespace Servicos.Embarcador.Monitoramento.Integracoes
{

    public class IntegracaoSystemSat : Abstract.AbstractIntegracaoSOAP
    {
        #region Atributos privados

        private static IntegracaoSystemSat Instance;
        private static readonly string nameConfigSection = "SystemSat";
        private Dominio.ObjetosDeValor.Embarcador.Logistica.ContaIntegracao conta;

        private Servicos.Embarcador.Integracao.SystemSat.IntegracaoSystemSat ServicoIntegracaoSystemSat;

        #endregion

        #region Constantes com as chaves dos dados/configurações

        private const string KEY_EMP_CLIENTE = "EmpCliente";

        #endregion

        #region Construtor privado

        private IntegracaoSystemSat(AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente) : base(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.SystemSat, nameConfigSection, cliente) { }

        #endregion

        #region Métodos públicos

        public static IntegracaoSystemSat GetInstance(AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente)
        {
            if (Instance == null) Instance = new IntegracaoSystemSat(cliente);
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
            InicializarWS();
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

        private List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> ObterPosicoes()
        {
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoes = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao>();

            try
            {
                string url = Servicos.Embarcador.Logistica.ContaIntegracao.ObterURL(this.conta);
                string empCliente = ObterEmpCliente();
                this.ServicoIntegracaoSystemSat.DefinirConfiguracoes(url, this.conta.Usuario, this.conta.Senha, empCliente);
                List<Servicos.SystemSat.Posicao> ultimasPosicoes = this.ServicoIntegracaoSystemSat.BuscarUltimasPosicoes(unitOfWork);
                int total = ultimasPosicoes?.Count ?? 0;
                for (int i = 0; i < total; i++)
                {
                    Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao novaPosicao = new Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao
                    {
                        ID = ultimasPosicoes[i].Id_Posicao,
                        Data = ultimasPosicoes[i].DataServidor,
                        DataVeiculo = ultimasPosicoes[i].DataGPS,
                        Placa = ultimasPosicoes[i].Placa.Trim().Replace("-", ""),
                        IDEquipamento = ultimasPosicoes[i].IdentificacaoVeiculo,
                        Latitude = ultimasPosicoes[i].Latitude,
                        Longitude = ultimasPosicoes[i].Longitude,
                        Descricao = ultimasPosicoes[i].Localizacao,
                        Velocidade = (int)ultimasPosicoes[i].Velocidade,
                        Temperatura = (decimal)(ultimasPosicoes[i].Temperatura ?? 0),
                        Ignicao = (ultimasPosicoes[i].Ignicao) ? 1 : 0,
                        Rastreador = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EnumTecnologiaRastreador.SystemSat
                    };

                    posicoes.Add(novaPosicao);
                }
            }
            catch (Exception e)
            {
                Log($"Erro BuscarUltimasPosicoes " + e.Message, 3);
            }
            return posicoes;
        }

        private void InicializarWS()
        {
            Log("Inicializando WS", 2);
            this.ServicoIntegracaoSystemSat = Servicos.Embarcador.Integracao.SystemSat.IntegracaoSystemSat.GetInstance();
        }

        private string ObterEmpCliente()
        {
            string value = Servicos.Embarcador.Logistica.ContaIntegracao.ObterValorListaParametrosAdicionais(KEY_EMP_CLIENTE, this.conta.ListaParametrosAdicionais);
            return value;
        }

        #endregion

    }

}
