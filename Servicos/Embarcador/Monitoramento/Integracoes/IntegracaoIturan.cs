using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Logistica;
using System;
using System.Collections.Generic;

namespace Servicos.Embarcador.Monitoramento.Integracoes
{
    public class IntegracaoIturan : Abstract.AbstractIntegracaoSOAP
    {
        #region Atributos privados

        private static IntegracaoIturan Instance;
        private static readonly string nameConfigSection = "Ituran";
        Dominio.ObjetosDeValor.Embarcador.Logistica.ContaIntegracao conta;

        #endregion

        #region Construtor privado

        private IntegracaoIturan(AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente) : base(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Ituran, nameConfigSection, cliente) { }

        #endregion

        #region Métodos públicos

        /**
         * Implementação de Singleton
         */
        public static IntegracaoIturan GetInstance(AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente)
        {
            if (Instance == null) Instance = new IntegracaoIturan(cliente);
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
                Servicos.Ituran.GetAllPlataformsData.ServiceAllPlatformData retornoRequisicao = BuscaUltimaPosicaoVeiculo();

                if (retornoRequisicao.ReturnCode != "200")
                {
                    Log($"Recebidas posiçoes de {retornoRequisicao.VehList.Length} veículos", 3);

                    foreach (Ituran.GetAllPlataformsData.ServicePlatformData posicao in retornoRequisicao.VehList)
                    {
                        posicoes.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao
                        {

                            Data = posicao.LastLocationTime,
                            DataVeiculo = posicao.LastLocationTime,
                            DataCadastro = DateTime.Now,
                            IDEquipamento = "",
                            Placa = posicao.Plate,
                            Latitude = posicao.LastLatitude,
                            Longitude = posicao.LastLongitude,
                            Velocidade = posicao.LastSpeed,
                            Temperatura = 0,
                            SensorTemperatura = false,
                            Descricao = posicao.LastAddress,
                            Rastreador = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EnumTecnologiaRastreador.Ituran
                        });

                    }

                    Log($"{posicoes.Count} posicoes", 3);
                }
            }
            catch (Exception ex)
            {
                Log("Erro ObterPosicoes " + ex.Message, 3);
            }

            return posicoes;
        }


        private Servicos.Ituran.GetAllPlataformsData.ServiceAllPlatformData BuscaUltimaPosicaoVeiculo()
        {
            Servicos.Ituran.GetAllPlataformsData.Service3SoapClient servicoIturan = ObterClient();
            Servicos.Ituran.GetAllPlataformsData.ServiceAllPlatformData response = servicoIturan.GetAllPlatformsData(conta.Usuario, conta.Senha, "True", "True", "False", "True");

            return response;
        }

        private Servicos.Ituran.GetAllPlataformsData.Service3SoapClient ObterClient()
        {
            string url = $"{conta.Protocolo}://{conta.Servidor}/{conta.URI}";

            System.ServiceModel.EndpointAddress endpointAddress = new System.ServiceModel.EndpointAddress(url);
            System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding();

            binding.MaxReceivedMessageSize = int.MaxValue;
            binding.ReceiveTimeout = new TimeSpan(0, 5, 0);
            binding.SendTimeout = new TimeSpan(0, 5, 0);

            if (conta.Protocolo == Protocolo.HTTPS)
                binding.Security.Mode = System.ServiceModel.BasicHttpSecurityMode.Transport;

            return new Servicos.Ituran.GetAllPlataformsData.Service3SoapClient(binding, endpointAddress);
        }

        #endregion
    }
}
