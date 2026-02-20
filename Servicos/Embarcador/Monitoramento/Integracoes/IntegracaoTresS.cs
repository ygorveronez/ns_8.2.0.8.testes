using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Logistica;
using System;
using System.Collections.Generic;
using System.Xml;

namespace Servicos.Embarcador.Monitoramento.Integracoes
{
    public class IntegracaoTresS : Abstract.AbstractIntegracaoSOAP
    {
        #region Atributos privados

        private static IntegracaoTresS Instance;
        private static readonly string nameConfigSection = "TresS";
        Dominio.ObjetosDeValor.Embarcador.Logistica.ContaIntegracao conta;

        #endregion

        #region Construtor privado

        private IntegracaoTresS(AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente) : base(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.TresSTecnologia, nameConfigSection, cliente) { }

        #endregion

        #region Métodos públicos

        /**
         * Implementação de Singleton
         */
        public static IntegracaoTresS GetInstance(AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente)
        {
            if (Instance == null) Instance = new IntegracaoTresS(cliente);
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
                XmlNodeList retornoRequisicao = BuscaUltimaPosicaoVeiculos();

                Log($"Recebidas posiçoes de {retornoRequisicao.Count} veículos", 3);

                foreach (XmlNode p in retornoRequisicao)
                {
                    posicoes.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao
                    {

                        Data = DateTime.Parse(p.SelectSingleNode("Data").InnerText),
                        DataVeiculo = DateTime.Parse(p.SelectSingleNode("Data").InnerText),
                        DataCadastro = DateTime.Now,
                        IDEquipamento = p.SelectSingleNode("idEquipamento").InnerText,
                        Placa = p.SelectSingleNode("Placa").InnerText.Trim(),
                        Latitude = double.Parse(p.SelectSingleNode("Latitude").InnerText.Replace(".", ",")),
                        Longitude = double.Parse(p.SelectSingleNode("Longitude").InnerText.Replace(".", ",")),
                        Velocidade = int.Parse(p.SelectSingleNode("Velocidade").InnerText),
                        Temperatura = 0,
                        SensorTemperatura = false,
                        Descricao = p.SelectSingleNode("Cidade").InnerText,
                        Rastreador = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EnumTecnologiaRastreador.TresSTecnologia
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


        private XmlNodeList BuscaUltimaPosicaoVeiculos()
        {
            Servicos.TresSTecnologia.Data_ExportSoapClient servico3S = ObterClient();
            var response = servico3S.ListaUltimaPosicaoVeiculos(conta.Usuario, conta.Senha);
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(response);
            XmlNodeList xnList = xml.SelectNodes("/Posicao/tbPosicao");
            return xnList;
        }

        private Servicos.TresSTecnologia.Data_ExportSoapClient ObterClient()
        {
            string url = $"{conta.Protocolo}://{conta.Servidor}/{conta.URI}";

            System.ServiceModel.EndpointAddress endpointAddress = new System.ServiceModel.EndpointAddress(url);
            System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding();

            binding.MaxReceivedMessageSize = int.MaxValue;
            binding.ReceiveTimeout = new TimeSpan(0, 5, 0);
            binding.SendTimeout = new TimeSpan(0, 5, 0);

            if (conta.Protocolo == Protocolo.HTTPS)
                binding.Security.Mode = System.ServiceModel.BasicHttpSecurityMode.Transport;

            return new Servicos.TresSTecnologia.Data_ExportSoapClient(binding, endpointAddress);
        }

        #endregion
    }
}
