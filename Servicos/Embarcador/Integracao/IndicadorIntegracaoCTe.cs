using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Repositorio;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Servicos.Embarcador.Integracao
{
    public sealed class IndicadorIntegracaoCTe : ServicoBase
    {
        #region Contrutores        

        public IndicadorIntegracaoCTe(UnitOfWork unitOfWork, CancellationToken cancelationToken = default) : base(unitOfWork, cancelationToken)
        {
        }

        #endregion

        #region Métodos Privados

        private async Task ConsultarIntegracaoAsync(IGrouping<string, Dominio.Entidades.Embarcador.Integracao.IndicadorIntegracaoCTe> agrupadorIndicadorIntegracaoCTe, Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao)
        {
            SecurityProtocolType protocoloAnterior = ServicePointManager.SecurityProtocol;

            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                HttpWebRequest requisicao = CriarRequisicao(configuracaoIntegracao);
                XmlDocument xmlRequisicaoEnvelopado = CriarXmlRequisicaoEnvelopado(configuracaoIntegracao, agrupadorIndicadorIntegracaoCTe.Key);
                List<Dominio.ObjetosDeValor.Embarcador.Integracao.RetornoConsultaIndicadorIntegracaoCTe> listaRetornoConsultaIndicadorIntegracaoCTe = await ObterRetornoRequisicaoAsync(requisicao, xmlRequisicaoEnvelopado);

                Repositorio.Embarcador.Integracao.IndicadorIntegracaoCTe repositorioIndicadorIntegracaoCTe = new Repositorio.Embarcador.Integracao.IndicadorIntegracaoCTe(_unitOfWork, _cancellationToken);

                foreach (Dominio.Entidades.Embarcador.Integracao.IndicadorIntegracaoCTe indicadorIntegracaoCTe in agrupadorIndicadorIntegracaoCTe.ToList())
                {
                    indicadorIntegracaoCTe.DataIntegracao = (from o in listaRetornoConsultaIndicadorIntegracaoCTe where indicadorIntegracaoCTe.Integradora.Descricao.ToLower().Contains(o.NomeIntegradora.ToLower()) select (DateTime?)o.DataIntegracao).FirstOrDefault();
                    indicadorIntegracaoCTe.DataUltimaConsultaIntegracao = DateTime.Now;

                    await repositorioIndicadorIntegracaoCTe.AtualizarAsync(indicadorIntegracaoCTe);
                }
            }
            catch (Exception excecao)
            {
                Log.TratarErro($"Ocorreu uma falha ao consultar a integração do CT-e");
                Log.TratarErro(excecao);
            }
            finally
            {
                ServicePointManager.SecurityProtocol = protocoloAnterior;
            }
        }

        private HttpWebRequest CriarRequisicao(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao)
        {
            HttpWebRequest requisicao = (HttpWebRequest)WebRequest.Create(configuracaoIntegracao.URLCarrefourIndicadorIntegracaoCTe);

            requisicao.Headers.Add("SOAPAction", "https://mapa/batimento_nfe/ConsultaChaves");
            requisicao.Headers.Add("x-api-key", configuracaoIntegracao.TokenCarrefour);
            requisicao.ContentType = "text/xml;charset=\"utf-8\"";
            requisicao.Accept = "text/xml";
            requisicao.Method = "POST";
            requisicao.KeepAlive = true;
            requisicao.UserAgent = "multi-embarcador-ms";

            return requisicao;
        }

        private XmlDocument CriarXmlRequisicaoEnvelopado(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, string chaveCTe)
        {
            XmlDocument xmlRequisicaoEnvelopado = new XmlDocument();

            xmlRequisicaoEnvelopado.LoadXml($@"
                <soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:bat=""https://mapa/batimento_nfe/"">
                    <soapenv:Header/>
                    <soapenv:Body>
                        <bat:ConsultaChaves>
                            <bat:Token>{configuracaoIntegracao.TokenCarrefourIndicadorIntegracaoCTe}</bat:Token>
                            <bat:inChaves>
                                 <bat:Chaves>
                                     <bat:nf_ChaveAcesso>{chaveCTe}</bat:nf_ChaveAcesso>
                                 </bat:Chaves>
                             </bat:inChaves>
                         </bat:ConsultaChaves>
                    </soapenv:Body>
                </soapenv:Envelope>"
            );

            return xmlRequisicaoEnvelopado;
        }

        private async Task<List<Dominio.ObjetosDeValor.Embarcador.Integracao.RetornoConsultaIndicadorIntegracaoCTe>> ObterRetornoRequisicaoAsync(HttpWebRequest requisicao, XmlDocument xmlRequisicaoEnvelopado)
        {
            using (Stream stream = await requisicao.GetRequestStreamAsync())
            {
                xmlRequisicaoEnvelopado.Save(stream);
            }

            IAsyncResult retornoRequisicaoAssincrono = requisicao.BeginGetResponse(null, null);

            retornoRequisicaoAssincrono.AsyncWaitHandle.WaitOne();

            using (WebResponse retornoRequisicao = requisicao.EndGetResponse(retornoRequisicaoAssincrono))
            {
                using (StreamReader leitorRetornoRequisicao = new StreamReader(retornoRequisicao.GetResponseStream()))
                {
                    XmlDocument xmlRetorno = new XmlDocument();

                    xmlRetorno.LoadXml(await leitorRetornoRequisicao.ReadToEndAsync());

                    XmlNodeList retornoConsultaChaves = xmlRetorno.GetElementsByTagName("outChaves");
                    List<Dominio.ObjetosDeValor.Embarcador.Integracao.RetornoConsultaIndicadorIntegracaoCTe> listaRetornoConsultaIndicadorIntegracaoCTe = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.RetornoConsultaIndicadorIntegracaoCTe>();

                    foreach (XmlElement retornoChave in retornoConsultaChaves)
                    {
                        listaRetornoConsultaIndicadorIntegracaoCTe.Add(new Dominio.ObjetosDeValor.Embarcador.Integracao.RetornoConsultaIndicadorIntegracaoCTe()
                        {

                            DataIntegracao = retornoChave["dataCriacao"].InnerText.ToDateTime("yyyy-MM-dd'T'HH:mm:ss"),
                            NomeIntegradora = retornoChave["App_Name_Dest"].InnerText
                        });
                    }

                    return listaRetornoConsultaIndicadorIntegracaoCTe;
                }
            }
        }

        private string ObterXmlCtes(List<Dominio.ObjetosDeValor.WebServiceCarrefour.CTe.CTe> listaCte)
        {
            XElement xmlCtes = new XElement("Ctes",
                from o in listaCte
                select new XElement("Id", o.CodigoCargaCTe)
            );

            return xmlCtes.ToString();
        }

        #endregion

        #region Métodos Públicos

        public void Adicionar(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            Repositorio.WebService.Integradora repositorioIntegradora = new Repositorio.WebService.Integradora(_unitOfWork);
            List<Dominio.Entidades.WebService.Integradora> integradoras = repositorioIntegradora.BuscarPorIndicarIntegracao();

            if (integradoras.Count == 0)
                return;

            Repositorio.Embarcador.Integracao.IndicadorIntegracaoCTe repositorioIndicadorIntegracaoCTe = new Repositorio.Embarcador.Integracao.IndicadorIntegracaoCTe(_unitOfWork, _cancellationToken);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe in carga.CargaCTes)
            {
                foreach (Dominio.Entidades.WebService.Integradora integradora in integradoras)
                {
                    if (repositorioIndicadorIntegracaoCTe.VerificarRegistroExistente(cargaCTe.Codigo, integradora.Codigo))
                        continue;

                    Dominio.Entidades.Embarcador.Integracao.IndicadorIntegracaoCTe indicadorIntegracaoCTe = new Dominio.Entidades.Embarcador.Integracao.IndicadorIntegracaoCTe()
                    {
                        CargaCTe = cargaCTe,
                        Integradora = integradora
                    };

                    repositorioIndicadorIntegracaoCTe.Inserir(indicadorIntegracaoCTe);
                }
            }
        }

        public void Atualizar(List<Dominio.ObjetosDeValor.WebServiceCarrefour.CTe.CTe> listaCte, Dominio.Entidades.WebService.Integradora integradora)
        {
            if (integradora.TipoIndicadorIntegracao != TipoIndicadorIntegracao.Sistema)
                return;

            if ((listaCte == null) || (listaCte.Count == 0))
                return;

            string xmlCTes = ObterXmlCtes(listaCte);
            Repositorio.Embarcador.Integracao.IndicadorIntegracaoCTe repositorioIndicadorIntegracaoCTe = new Repositorio.Embarcador.Integracao.IndicadorIntegracaoCTe(_unitOfWork, _cancellationToken);

            repositorioIndicadorIntegracaoCTe.Atualizar(xmlCTes, integradora.Codigo);
        }

        public async Task VerificarIntegracoesPendentesAsync()
        {
            Repositorio.Embarcador.Configuracoes.Integracao repositorioConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(_unitOfWork, _cancellationToken);
            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = await repositorioConfiguracaoIntegracao.BuscarPrimeiroRegistroAsync();

            if ((configuracaoIntegracao == null) || string.IsNullOrWhiteSpace(configuracaoIntegracao.URLCarrefourIndicadorIntegracaoCTe) || string.IsNullOrWhiteSpace(configuracaoIntegracao.TokenCarrefourIndicadorIntegracaoCTe))
                return;

            Repositorio.Embarcador.Integracao.IndicadorIntegracaoCTe repositorioIndicadorIntegracaoCTe = new Repositorio.Embarcador.Integracao.IndicadorIntegracaoCTe(_unitOfWork, _cancellationToken);
            List<Dominio.Entidades.Embarcador.Integracao.IndicadorIntegracaoCTe> listaIndicadorIntegracaoCTe = await repositorioIndicadorIntegracaoCTe.BuscarPorConsultaIntegracaoPendenteAsync(20);

            if (listaIndicadorIntegracaoCTe.Count == 0)
                listaIndicadorIntegracaoCTe = await repositorioIndicadorIntegracaoCTe.BuscarPorIntegracaoPendenteAsync(20);

            List<IGrouping<string, Dominio.Entidades.Embarcador.Integracao.IndicadorIntegracaoCTe>> listaIndicadorIntegracaoCTeAgrupada = (from o in listaIndicadorIntegracaoCTe group o by o.CargaCTe.CTe.Chave into g select g).ToList();

            foreach (IGrouping<string, Dominio.Entidades.Embarcador.Integracao.IndicadorIntegracaoCTe> agrupadorIndicadorIntegracaoCTe in listaIndicadorIntegracaoCTeAgrupada)
                await ConsultarIntegracaoAsync(agrupadorIndicadorIntegracaoCTe, configuracaoIntegracao);
        }

        #endregion
    }
}
