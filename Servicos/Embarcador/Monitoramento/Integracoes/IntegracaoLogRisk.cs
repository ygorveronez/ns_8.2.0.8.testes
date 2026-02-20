using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Logistica;
using System;
using System.Collections.Generic;

namespace Servicos.Embarcador.Monitoramento.Integracoes
{
    public class IntegracaoLogRisk : Abstract.AbstractIntegracaoSOAP
    {
        #region Atributos privados

        private static IntegracaoLogRisk Instance;
        private static readonly string nameConfigSection = "LogRisk";
        Dominio.ObjetosDeValor.Embarcador.Logistica.ContaIntegracao conta;

        #endregion

        #region Construtor privado

        private IntegracaoLogRisk(AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente) : base(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.LogRisk, nameConfigSection, cliente) { }

        #endregion

        #region Métodos públicos

        /**
         * Implementação de Singleton
         */
        public static IntegracaoLogRisk GetInstance(AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente)
        {
            if (Instance == null) Instance = new IntegracaoLogRisk(cliente);
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

                Servicos.ServicoLogRisk.sgrData login = EfetuarLoginWS();


                ServicoLogRisk.sgrData retornoRequisicao = BuscaUltimaPosicaoVeiculos(login);
                System.Data.DataSet ds = retornoRequisicao.ReturnDataset;

                Log($"Recebidas posiçoes de {ds.Tables[0].Rows.Count} veículos", 3);

                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao posicao = new Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao()
                    {                        
                        DataCadastro = DateTime.Now,                        
                        Placa = ds.Tables[0].Rows[i]["NRPLACACAVALO"]?.ToString().Replace("-", string.Empty).Replace(" ", string.Empty),                                                
                        Descricao = ds.Tables[0].Rows[i]["POSICAO"].ToString(),
                        Rastreador = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EnumTecnologiaRastreador.LogRisk
                    };

                    int.TryParse(ds.Tables[0].Rows[i]["VLVELOCIDADE"].ToString(), out int velocidadeTemp);
                    posicao.Velocidade = velocidadeTemp;

                    int.TryParse(ds.Tables[0].Rows[i]["VLTEMPERATURASENSOR1"].ToString(), out int temperaturaTemp);
                    posicao.Temperatura = velocidadeTemp;
                    posicao.SensorTemperatura = posicao.Temperatura > 0;

                    int.TryParse(ds.Tables[0].Rows[i]["FLIGNICAO"].ToString(), out int ignicaoTemp);
                    posicao.Ignicao = ignicaoTemp;
                    posicao.IDEquipamento = posicao.Placa;
                    try
                    {
                        posicao.Data = DateTime.Parse(ds.Tables[0].Rows[i]["DTPOSICAO"].ToString());
                        posicao.DataVeiculo = posicao.Data;
                        posicao.Latitude = double.Parse(ds.Tables[0].Rows[i]["VLLAT"].ToString().Replace(".", ","));
                        posicao.Longitude = double.Parse(ds.Tables[0].Rows[i]["VLLONG"].ToString().Replace(".", ","));
                    }
                    catch
                    {
                        continue;
                    }
                    
                    posicoes.Add(posicao);
                }
                Log($"{posicoes.Count} posicoes", 3);
            }
            catch (Exception ex)
            {
                Log("Erro ObterPosicoes " + ex.Message, 3);
            }

            return posicoes;
        }


        private Servicos.ServicoLogRisk.sgrData EfetuarLoginWS()
        {

            Servicos.ServicoLogRisk.sgrOpentechSoapClient client = ObterClient();

            return client.sgrLogin(conta.Usuario, conta.Senha, ObterParamentroAdicionalConta("dominio"));
        }

        private ServicoLogRisk.sgrData BuscaUltimaPosicaoVeiculos(Servicos.ServicoLogRisk.sgrData login)
        {
            Servicos.ServicoLogRisk.sgrOpentechSoapClient client = ObterClient();
            string cdpas = ObterParamentroAdicionalConta("cdpas");
            string cdcli = ObterParamentroAdicionalConta("cdcli");
            //return client.sgrRetornaPosicaoVeiculo(login.ReturnKey, cdpas.ToInt(), cdcli.ToInt(), DateTime.Today, "41AG2H");
            return client.sgrRetornaPosicaoFrota(login.ReturnKey, cdpas.ToInt(), cdcli.ToInt(), DateTime.Today, 1);
        }

        private Servicos.ServicoLogRisk.sgrOpentechSoapClient ObterClient()
        {
            string url = $"{conta.Protocolo}://{conta.Servidor}/{conta.URI}";

            System.ServiceModel.EndpointAddress endpointAddress = new System.ServiceModel.EndpointAddress(url);
            System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding();

            binding.MaxReceivedMessageSize = int.MaxValue;
            binding.ReceiveTimeout = new TimeSpan(0, 5, 0);
            binding.SendTimeout = new TimeSpan(0, 5, 0);

            if (conta.Protocolo == Protocolo.HTTPS)
                binding.Security.Mode = System.ServiceModel.BasicHttpSecurityMode.Transport;

            return new Servicos.ServicoLogRisk.sgrOpentechSoapClient(binding, endpointAddress);
        }

        private string ObterParamentroAdicionalConta(string paramKey)
        {
            string value = Servicos.Embarcador.Logistica.ContaIntegracao.ObterValorListaParametrosAdicionais(paramKey, this.conta.ListaParametrosAdicionais);
            return value;
        }

        #endregion
    }
}
