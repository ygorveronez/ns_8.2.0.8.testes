using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using RestSharp;

namespace Servicos.Embarcador.Integracao.Arquivei
{
    public class IntegracaoArquivei
    {
        #region Atributos Globais

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion Atributos Globais

        #region Construtores

        public IntegracaoArquivei(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion Construtores

        #region Métodos Públicos

        public bool IsPossuiIntegracaoArquivei()
        {
            Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(_unitOfWork);

            return repositorioTipoIntegracao.ExistePorTipo(TipoIntegracao.Arquivei);
        }

        public void ConsultarXMLCTes(AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            Repositorio.Embarcador.Configuracoes.IntegracaoArquivei repIntegracaoArquivei = new Repositorio.Embarcador.Configuracoes.IntegracaoArquivei(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoArquivei integracaoArquivei = repIntegracaoArquivei.Buscar();

            if (integracaoArquivei == null || string.IsNullOrWhiteSpace(integracaoArquivei.URLArquivei) || string.IsNullOrWhiteSpace(integracaoArquivei.IDArquivei) || string.IsNullOrWhiteSpace(integracaoArquivei.KeyArquivei) || integracaoArquivei.CodigoInicialConsultaXMLCTeArquivei == 0)
                return;

            try
            {
                string url = $"{integracaoArquivei.URLArquivei}/cte/taker?limit=5&cursor={integracaoArquivei.CodigoInicialConsultaXMLCTeArquivei}";

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                var client = new RestClient(url);
                client.Timeout = -1;
                var request = new RestRequest(Method.GET);
                request.AddHeader("X-API-ID", integracaoArquivei.IDArquivei);
                request.AddHeader("X-API-KEY", integracaoArquivei.KeyArquivei);
                request.AddHeader("Content-Type", "application/json");
                IRestResponse response = client.Execute(request);
                string responseString = response.Content;

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Arquivei.RetornoConsultaXMLArquivei retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Arquivei.RetornoConsultaXMLArquivei>(responseString);

                    if (retorno.Quantidade > 0)
                    {
                        foreach (Dominio.ObjetosDeValor.Embarcador.Integracao.Arquivei.RetornoConsultaXmlListaArquivei objeto in retorno.ListaXML)
                        {
                            byte[] byteArray = Convert.FromBase64String(objeto.XML);
                            MemoryStream arquivo = new MemoryStream(byteArray);

                            Servicos.Embarcador.CTe.CTe.ProcessarXMLCTe(arquivo, _unitOfWork, _unitOfWork.StringConexao, tipoServicoMultisoftware, "");
                        }

                        integracaoArquivei.CodigoInicialConsultaXMLCTeArquivei = integracaoArquivei.CodigoInicialConsultaXMLCTeArquivei + retorno.Quantidade;
                        repIntegracaoArquivei.Atualizar(integracaoArquivei);
                    }
                    else
                        Log.TratarErro($"Retorno Arquivei: {retorno.StatusCode} - {retorno.Mensagem}");
                }
                else if (response.StatusCode != HttpStatusCode.NoContent)
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Arquivei.RetornoConsultaXMLArquivei json = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Arquivei.RetornoConsultaXMLArquivei>(responseString);
                    Log.TratarErro($"Falha ao conectar no WS Arquivei {response.StatusCode}: {json.Erro} ");
                }
            }
            catch (Exception ex)
            {
                Log.TratarErro("Falha Arquivei: " + ex);
            }
        }

        #endregion Métodos Públicos

        #region Métodos Privados

        #endregion Métodos Privados
    }
}
