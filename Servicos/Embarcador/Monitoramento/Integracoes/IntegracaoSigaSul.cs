using Dominio.ObjetosDeValor.Embarcador.Logistica;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Servicos.Embarcador.Monitoramento.Integracoes
{
    public class IntegracaoSigaSul : Abstract.AbstractIntegracaoREST
    {
        #region Atributos privados

        private static IntegracaoSigaSul Instance;
        private static readonly string nameConfigSection = "SigaSul";
        Dominio.ObjetosDeValor.Embarcador.Logistica.ContaIntegracao conta;

        #endregion

        #region Construtor privado

        private IntegracaoSigaSul(AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente) : base(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.SigaSul, nameConfigSection, cliente) { }

        #endregion

        #region Métodos públicos

        /**
         * Implementação de Singleton
         */
        public static IntegracaoSigaSul GetInstance(AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente)
        {
            if (Instance == null) Instance = new IntegracaoSigaSul(cliente);
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
                var retornoRequisicao = BuscaUltimaPosicaoVeiculos();
                
                Log($"Recebidas posiçoes de {retornoRequisicao.Count} veículos", 3);

                foreach (Dominio.ObjetosDeValor.Embarcador.Integracao.SigaSul.RetornoPosicoes p in retornoRequisicao)
                {
                    posicoes.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao
                    {
                        DataCadastro = DateTime.Now,
                        DataVeiculo = DateTime.Parse(p.DataHora),
                        Data = DateTime.Parse(p.DataHora),                        
                        Placa = p.Placa.Replace("-",""),
                        Latitude = double.Parse(p.Latitude.Replace(".",",")),
                        Longitude = double.Parse(p.Longitude.Replace(".", ",")),
                        Velocidade = int.Parse(p.Velociadade),
                        Temperatura = 0,
                        SensorTemperatura = false,
                        IDEquipamento = p.IDEquipamento,
                        Descricao = p.Modelo,
                        Rastreador = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EnumTecnologiaRastreador.SigaSul,
                        Ignicao = p.Ignicao ? 1 : 0
                    });

                    Log($"{posicoes.Count} posicoes", 3);
                }
                
            
            }
            catch (Exception ex)
            {
                Log("Erro ObterPosicoes " + ex.Message, 3);
            }

            return posicoes;
        }

        
        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.SigaSul.RetornoPosicoes> BuscaUltimaPosicaoVeiculos()
        {            
            var ret = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.SigaSul.RetornoPosicoes>();
            
            var httpWebRequest = (HttpWebRequest)WebRequest.Create($"{conta.Protocolo}://{conta.Servidor}/{conta.URI}");
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Headers.Add("x-auth-token", "63014613f080bf37b77bc03b638be0bd"); //token de autenticacao do cliente
            httpWebRequest.Method = "POST";

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
                ret = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Integracao.SigaSul.RetornoPosicoes>>(result);
            }            

            return ret;
        }


        #endregion
    }
}
