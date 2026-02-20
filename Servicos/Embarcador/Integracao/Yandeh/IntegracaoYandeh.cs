using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Servicos.Embarcador.Integracao.Yandeh
{
    public class IntegracaoYandeh
    {
        #region Atributos Globais

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion Atributos Globais

        #region Construtores

        public IntegracaoYandeh(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion Construtores

        #region Métodos Públicos

        public void IntegrarCargaCTe(ref Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracao cargaCTeIntegracao)
        {
            Repositorio.Embarcador.Cargas.CargaCTeIntegracao repCargaCTeIntegracao = new Repositorio.Embarcador.Cargas.CargaCTeIntegracao(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            cargaCTeIntegracao.DataIntegracao = DateTime.Now;
            cargaCTeIntegracao.NumeroTentativas++;
            string xmlRequest = "", xmlResponse = "";

            try
            {
                IntegrarCTe(cargaCTeIntegracao, out xmlRequest, out xmlResponse);

                cargaCTeIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                cargaCTeIntegracao.ProblemaIntegracao = "Integrado com sucesso";
            }
            catch (ServicoException excecao)
            {
                cargaCTeIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaCTeIntegracao.ProblemaIntegracao = excecao.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                cargaCTeIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaCTeIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração com Yandeh";
            }

            servicoArquivoTransacao.Adicionar(cargaCTeIntegracao, xmlRequest, xmlResponse, "xml");
            repCargaCTeIntegracao.Atualizar(cargaCTeIntegracao);
        }

        #endregion Métodos Públicos

        #region Métodos Privados

        private void IntegrarCTe(Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracao cargaCTeIntegracao, out string xmlRequest, out string xmlResponse)
        {
            xmlRequest = null;
            xmlResponse = null;

            Repositorio.Embarcador.Configuracoes.IntegracaoYandeh repIntegracaoYandeh = new Repositorio.Embarcador.Configuracoes.IntegracaoYandeh(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoYandeh configuracaoIntegracaoYandeh = repIntegracaoYandeh.Buscar();

            if (!ValidarDadosIntegracaoYandeh(configuracaoIntegracaoYandeh))
                return;

            ServicoYandeh.IwsDataServerClient iwsDataServerClient = ObterClient(configuracaoIntegracaoYandeh.URLComunicacao, configuracaoIntegracaoYandeh);
            InspectorBehavior inspector = new InspectorBehavior();
            iwsDataServerClient.Endpoint.EndpointBehaviors.Add(inspector);

            using (OperationContextScope scope = new OperationContextScope(iwsDataServerClient.InnerChannel))
            {
                HttpRequestMessageProperty httpRequestProperty = new HttpRequestMessageProperty();
                httpRequestProperty.Headers[System.Net.HttpRequestHeader.Authorization] = "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(iwsDataServerClient.ClientCredentials.UserName.UserName + ":" + iwsDataServerClient.ClientCredentials.UserName.Password));
                OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = httpRequestProperty;

                string retorno = iwsDataServerClient.SaveRecord("movmovimentotbcdata", ObterXmlPayload(cargaCTeIntegracao), ObterXmlContexto(configuracaoIntegracaoYandeh));
                xmlRequest = inspector.LastRequestXML;
                xmlResponse = inspector.LastResponseXML;

                if (retorno.Contains("SaveRecord"))
                {
                    if (retorno.Contains("="))
                        retorno = retorno.Substring(0, retorno.IndexOf("="));

                    throw new ServicoException(retorno);
                }
            }
        }

        private ServicoYandeh.IwsDataServerClient ObterClient(string url, Dominio.Entidades.Embarcador.Configuracoes.IntegracaoYandeh configuracaoIntegracaoYandeh)
        {
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
            System.ServiceModel.EndpointAddress endpointAddress = new System.ServiceModel.EndpointAddress($"{url}");
            System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding();

            binding.MaxReceivedMessageSize = int.MaxValue;
            binding.ReceiveTimeout = new TimeSpan(0, 5, 0);
            binding.SendTimeout = new TimeSpan(0, 5, 0);
            binding.Security.Transport.ClientCredentialType = System.ServiceModel.HttpClientCredentialType.Basic;
            binding.Security.Message.ClientCredentialType = System.ServiceModel.BasicHttpMessageCredentialType.UserName;

            if (url.StartsWith("https"))
                binding.Security.Mode = System.ServiceModel.BasicHttpSecurityMode.Transport;

            ServicoYandeh.IwsDataServerClient client = new ServicoYandeh.IwsDataServerClient(binding, endpointAddress);
            client.ClientCredentials.UserName.UserName = configuracaoIntegracaoYandeh.Usuario;
            client.ClientCredentials.UserName.Password = configuracaoIntegracaoYandeh.Senha;

            return client;
        }

        private bool ValidarDadosIntegracaoYandeh(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoYandeh configuracaoIntegracaoYandeh)
        {

            if (string.IsNullOrWhiteSpace(configuracaoIntegracaoYandeh?.URLComunicacao))
                throw new ServicoException("Favor informar a URL de comunicação com Yandeh!");

            if (string.IsNullOrWhiteSpace(configuracaoIntegracaoYandeh?.Usuario))
                throw new ServicoException("Favor informar ao usuário!");

            if (string.IsNullOrWhiteSpace(configuracaoIntegracaoYandeh?.URLComunicacao))
                throw new ServicoException("Favor informar a senha!");

            return true;
        }

        private string ObterXmlPayload(Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracao cargaCTeIntegracao)
        {
            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repNotas = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(_unitOfWork);
            Repositorio.Embarcador.Pedidos.XMLNotaFiscalProduto repProdutoXML = new Repositorio.Embarcador.Pedidos.XMLNotaFiscalProduto(_unitOfWork);

            Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = cargaCTeIntegracao.CargaCTe.CTe;
            Dominio.Entidades.Embarcador.Cargas.Carga carga = cargaCTeIntegracao.CargaCTe.Carga;

            List<int> codigosCargaCTes = new List<int>();
            codigosCargaCTes.Add(cargaCTeIntegracao.CargaCTe.Codigo);
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe> notas = repNotas.BuscarPorCargaCTes(codigosCargaCTes);

            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

            StringBuilder stXML = new StringBuilder();
            stXML.Append("<MovMovimento>");
            stXML.Append("<TMOV>");
            stXML.Append("<CODCOLIGADA>15</CODCOLIGADA>");
            stXML.Append("<IDMOV>-1</IDMOV>");
            stXML.Append("<CODFILIAL>1</CODFILIAL>");
            stXML.Append("<CODLOC>000001</CODLOC>");
            stXML.Append($"<CODCFO>{cte.Destinatario.CodigoIntegracao ?? ""}</CODCFO>");
            stXML.Append($"<NUMEROMOV>{cte.Numero.ToString("D")}</NUMEROMOV>");
            stXML.Append($"<SERIE>{cte.Serie.Numero.ToString("D")}</SERIE>");
            stXML.Append("<CODTMV>2.2.32</CODTMV>");
            stXML.Append("<TIPO>A</TIPO>");
            stXML.Append("<STATUS>N</STATUS>");
            stXML.Append($"<DATAEMISSAO>{cte.DataEmissao.Value.ToString("dd/MM/yyyy")}</DATAEMISSAO>");
            stXML.Append($"<OBSERVACAO>{cte.ObservacoesGerais}</OBSERVACAO>");
            stXML.Append("<CODMEN>001</CODMEN>");
            stXML.Append("<VIADETRANSPORTE>Rodoviário</VIADETRANSPORTE>");
            stXML.Append("<MARCA>Própria</MARCA>");
            stXML.Append("<QUANTIDADE>1.0000</QUANTIDADE>");
            stXML.Append("<ESPECIE>Mercadorias</ESPECIE>");
            stXML.Append("<CODMOEVALORLIQUIDO>R$</CODMOEVALORLIQUIDO>");
            stXML.Append($"<FRETECIFOUFOB>{cte.ModalidadeFreteEBS}</FRETECIFOUFOB>");
            stXML.Append("<CODTRA>001</CODTRA>");
            stXML.Append("<CODCOLCFO>15</CODCOLCFO>");
            stXML.Append($"<DATAENTREGA>{cte.DataEntrega?.ToString("dd/MM/yyyy") ?? string.Empty}</DATAENTREGA>");
            stXML.Append($"<IDNAT>{cte.NaturezaDaOperacao.Numero}</IDNAT>");
            stXML.Append("<CODTDO>DACTE</CODTDO>");
            stXML.Append("<CODVIATRANSPORTE>1</CODVIATRANSPORTE>");
            stXML.Append($"<DISTANCIA>{carga.Distancia}</DISTANCIA>");
            stXML.Append("<UNCALCULO>UN</UNCALCULO>");
            stXML.Append($"<VALORMERCADORIAS>{cte.ValorTotalMercadoria.ToString("n4", cultura)}</VALORMERCADORIAS>");
            stXML.Append($"<NATUREZAVOLUMES>{cte.ProdutoPredominanteCTe}</NATUREZAVOLUMES>");
            stXML.Append($"<VLRDESPACHO>{cte.ValorPrestacaoServico.ToString("n4", cultura)}</VLRDESPACHO>");
            stXML.Append($"<VLRPEDAGIO>{string.Join(", ", cte.ValesPedagio.Select(valePedagio => valePedagio.ValorValePedagio.ToString("n4", cultura))) ?? "0.0000"}</VLRPEDAGIO>");
            stXML.Append($"<VLRFRETEOUTROS>{cte.ValorOutrasRetencoes.ToString("n4", cultura)}</VLRFRETEOUTROS>");
            stXML.Append($"<HISTORICOCURTO>{cte.InformacaoAdicionalFisco}</HISTORICOCURTO>");
            stXML.Append("</TMOV>");
            stXML.Append("<TITMMOV>");
            stXML.Append("<CODCOLIGADA>15</CODCOLIGADA>");
            stXML.Append("<IDMOV>-1</IDMOV>");
            stXML.Append("<NSEQITMMOV>1</NSEQITMMOV>");
            stXML.Append("<CODFILIAL>1</CODFILIAL>");
            stXML.Append("<NUMEROSEQUENCIAL>1</NUMEROSEQUENCIAL>");
            stXML.Append("<IDPRD>653170</IDPRD>");
            stXML.Append("<QUANTIDADE>1.0000</QUANTIDADE>");
            stXML.Append($"<PRECOUNITARIO>{cte.ValorPrestacaoServico.ToString("n4", cultura)}</PRECOUNITARIO>");
            stXML.Append("<CODUND>UN</CODUND>");
            stXML.Append("<CODLOC>000001</CODLOC>");
            stXML.Append("</TITMMOV>");
            stXML.Append("<TCOMPONENTECARGA>");
            stXML.Append("<CODCOLIGADA>15</CODCOLIGADA>");
            stXML.Append("<IDMOV>-1</IDMOV>");
            stXML.Append($"<CODETDPLACA>{string.Join(", ", cte.Veiculos.Select(veic => veic.Estado.Sigla))}</CODETDPLACA>");
            stXML.Append($"<PLACA>{string.Join(", ", cte.Veiculos.Select(veic => veic.Placa))}</PLACA>");
            stXML.Append($"<NUMEROCARGA>{carga.Numero}</NUMEROCARGA>");
            stXML.Append("</TCOMPONENTECARGA>");
            stXML.Append("<TMOVTRANSP>");
            stXML.Append("<CODCOLIGADA>15</CODCOLIGADA>");
            stXML.Append("<IDMOV>-1</IDMOV>");
            stXML.Append("<TIPOREMETENTE>C</TIPOREMETENTE>");
            stXML.Append("<REMETENTECODCOLCFO>15</REMETENTECODCOLCFO>");
            stXML.Append($"<REMETENTECODCFO>{cte.Remetente.CodigoIntegracao ?? ""}</REMETENTECODCFO>");
            stXML.Append("<REMETENTEFILIAL>1</REMETENTEFILIAL>");
            stXML.Append("<TIPODESTINATARIO>C</TIPODESTINATARIO>");
            stXML.Append("<DESTINATARIOCODCOLCFO>15</DESTINATARIOCODCOLCFO>");
            stXML.Append($"<DESTINATARIOCODCFO>{cte.Destinatario.CodigoIntegracao ?? ""}</DESTINATARIOCODCFO>");
            stXML.Append("<DESTINATARIOFILIAL>1</DESTINATARIOFILIAL>");
            stXML.Append("<RETIRAMERCADORIA>0</RETIRAMERCADORIA>");
            stXML.Append("<TIPOCTE>0</TIPOCTE>");
            stXML.Append("<TIPOSERVICOCTE>0</TIPOSERVICOCTE>");
            stXML.Append("</TMOVTRANSP>");

            int i = 0;
            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe nota in notas)
            {
                i++;
                decimal quantidadeProdutos = 0;
                List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalProduto> produtos = repProdutoXML.BuscarPorNotaFiscal(nota.PedidoXMLNotaFiscal.Codigo);
                foreach (var produto in produtos)
                    quantidadeProdutos += produto.Quantidade;

                stXML.Append("<TCTRC>");
                stXML.Append("<CODCOLIGADA>15</CODCOLIGADA>");
                stXML.Append("<IDMOV>-1</IDMOV>");
                stXML.Append($"<NUMEROSEQNOTA>{i}</NUMEROSEQNOTA>");
                stXML.Append("<CODTDO>NF55</CODTDO>");
                stXML.Append($"<SERIEDOC>{nota.PedidoXMLNotaFiscal.XMLNotaFiscal.Serie}</SERIEDOC>");
                stXML.Append($"<NUMERODOC>{nota.PedidoXMLNotaFiscal.XMLNotaFiscal.Numero}</NUMERODOC>");
                stXML.Append($"<VALORTOTAL>{nota.PedidoXMLNotaFiscal.XMLNotaFiscal.Valor.ToString("n4", cultura)}</VALORTOTAL>");
                stXML.Append($"<VALORMERCADORIAS>{nota.PedidoXMLNotaFiscal.XMLNotaFiscal.ValorTotalProdutos.ToString("n4", cultura)}</VALORMERCADORIAS>");
                stXML.Append($"<QUANTIDADE>{quantidadeProdutos.ToString("n4", cultura)}</QUANTIDADE>");
                stXML.Append($"<PESOBRUTO>{nota.PedidoXMLNotaFiscal.XMLNotaFiscal.Peso.ToString("n4", cultura)}</PESOBRUTO>");
                stXML.Append($"<PESOLIQUIDO>{nota.PedidoXMLNotaFiscal.XMLNotaFiscal.PesoLiquido.ToString("n4", cultura)}</PESOLIQUIDO>");
                stXML.Append($"<CHAVEACESSONFE>{nota.PedidoXMLNotaFiscal.XMLNotaFiscal.Chave}</CHAVEACESSONFE>");
                stXML.Append("</TCTRC>");
            }

            stXML.Append("</MovMovimento>");

            return stXML.ToString();
        }

        private string ObterXmlContexto(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoYandeh configuracaoIntegracaoYandeh)
        {
            StringBuilder stXML = new StringBuilder();
            stXML.Append($"CODCOLIGADA=15;CODSISTEMA=G;CODUSUARIO={configuracaoIntegracaoYandeh.Usuario}");
            return stXML.ToString();
        }

        #endregion Métodos Privados
    }
}
