using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml.Linq;

namespace Servicos
{
    public class Buonny
    {
        public static void EnviarViagem(ref Dominio.Entidades.SMViagemMDFe smViagemMDFe, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            InspectorBehavior inspector = new InspectorBehavior();

            Repositorio.SMViagemMDFe repSMViagemMDFe = new Repositorio.SMViagemMDFe(unidadeDeTrabalho);

            smViagemMDFe.DataIntegracao = DateTime.Now;

            if (smViagemMDFe.MDFe.Empresa.Configuracao == null)
            {
                smViagemMDFe.Status = Dominio.Enumeradores.StatusIntegracaoSM.Rejeitado;
                smViagemMDFe.Mensagem = "Empresas sem configuração para integração com a Buonny";

                repSMViagemMDFe.Atualizar(smViagemMDFe);
            }

            string urlWebService = smViagemMDFe.MDFe.Empresa.Configuracao.BuonnyURL;
            string tokenBuonny = smViagemMDFe.MDFe.Empresa.Configuracao.BuonnyToken;
            string cnpjjGerenciadoraRisco = smViagemMDFe.MDFe.Empresa.Configuracao.BuonnyGerenciadora;
            string tipoProduto = smViagemMDFe.MDFe.Empresa.Configuracao.BuonnyCodigoTipoProduto;

            ServicoBuonny.viagem viagem = ObterObjetoViagem(smViagemMDFe, tokenBuonny, cnpjjGerenciadoraRisco, tipoProduto, unidadeDeTrabalho);

            string codigoSM = string.Empty;
            string mensagemRetorno;
            string xmlRequisicao = "";
            string xmlResposta = "";
            try
            {
                //ServicoBuonny.IncluirSmPortClient svcBuonny = ObterClient(urlWebService, "");

                //ServicoBuonny.IncluirSmPortClient svcBuonny = new ServicoBuonny.IncluirSmPortClient();
                //svcBuonny.Endpoint.EndpointBehaviors.Add(inspector);

                //ServicoBuonny.viagem_result retorno = svcBuonny.incluirSm(viagem);

                //xmlRequisicao = inspector.LastRequestXML != null ? inspector.LastRequestXML : string.Empty;
                //xmlResposta = inspector.LastResponseXML != null ? inspector.LastResponseXML : string.Empty;

                //string codigo_sm = retorno?.codigo_sm;
                //string erro = retorno?.erro;

                //ExtrairInformacoesRetornoBuonny(retorno, inspector, out mensagemRetorno, out codigoSM);

                xmlRequisicao = GerarXMLRequisicaoSolicitarMonitoramento(viagem, urlWebService);
                WebRequest requisicao = CriaRequisicao(urlWebService, "POST", xmlRequisicao);
                HttpWebResponse retornoRequisicao = ExecutarRequisicao(requisicao);

                xmlResposta = ObterRetorno(retornoRequisicao);

                ServicoBuonny.viagem_result viagem_result = new ServicoBuonny.viagem_result()
                {
                    codigo_sm = Utilidades.XML.ObterConteudoTag(xmlResposta, tag: "codigo_sm") ?? string.Empty,
                    erro = Utilidades.XML.ObterConteudoTag(xmlResposta, tag: "erro")
                };

                ExtrairInformacoesRetornoBuonny(viagem_result, null, out mensagemRetorno, out codigoSM);
            }
            catch (Exception ex)
            {
                xmlRequisicao = inspector.LastRequestXML != null ? inspector.LastRequestXML : string.Empty;
                xmlResposta = inspector.LastResponseXML != null ? inspector.LastResponseXML : string.Empty;

                mensagemRetorno = ex.InnerException?.Message ?? ex.Message;
                Servicos.Log.TratarErro(ex);
            }

            string mensagemIntegracao = mensagemRetorno;
            if (!string.IsNullOrWhiteSpace(codigoSM))
                mensagemIntegracao = mensagemIntegracao + " - Código SM: " + codigoSM + ".";

            if (string.IsNullOrWhiteSpace(codigoSM))
            {
                SalvarLogIntegracao(xmlRequisicao, xmlResposta, smViagemMDFe, unidadeDeTrabalho);

                smViagemMDFe.Mensagem = !string.IsNullOrWhiteSpace(mensagemIntegracao) ? mensagemIntegracao : "Não foi possível enviar integração para a Buonny, consulte o log.";
                smViagemMDFe.Status = Dominio.Enumeradores.StatusIntegracaoSM.Rejeitado;
            }
            else
            {
                SalvarLogIntegracao(xmlRequisicao, xmlResposta, smViagemMDFe, unidadeDeTrabalho);

                smViagemMDFe.Mensagem = mensagemIntegracao;
                smViagemMDFe.CodigoIntegracaoViagem = codigoSM;
                smViagemMDFe.Status = Dominio.Enumeradores.StatusIntegracaoSM.Sucesso;
            }

            repSMViagemMDFe.Atualizar(smViagemMDFe);
        }

        private static void SalvarLogIntegracao(string request, string response, Dominio.Entidades.SMViagemMDFe smViagemMDFe, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.SMViagemMDFeLog repSMViagemMDFeLog = new Repositorio.SMViagemMDFeLog(unidadeDeTrabalho);

            Dominio.Entidades.SMViagemMDFeLog log = new Dominio.Entidades.SMViagemMDFeLog()
            {
                SMViagemMDFe = smViagemMDFe,
                DataHora = DateTime.Now,
                Requisicao = request,
                Resposta = response
            };

            repSMViagemMDFeLog.Inserir(log);
        }

        private static WebRequest CriaRequisicao(string url, string metodo, string body, List<(string Nome, string Valor)> headers = null, string contentType = "application/json")
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            WebRequest requisicao = WebRequest.Create(url);

            byte[] byteArrayDadosRequisicao = Encoding.ASCII.GetBytes(body);

            requisicao.Method = metodo;
            requisicao.ContentType = contentType;
            requisicao.Timeout = 60 * 1000;

            foreach (var header in (headers ?? new List<(string Nome, string Valor)>()))
                requisicao.Headers[header.Nome] = header.Valor;

            requisicao.ContentLength = byteArrayDadosRequisicao.Length;

            System.IO.Stream streamDadosRequisicao = requisicao.GetRequestStream();
            streamDadosRequisicao.Write(byteArrayDadosRequisicao, 0, byteArrayDadosRequisicao.Length);
            streamDadosRequisicao.Close();

            return requisicao;
        }

        private static HttpWebResponse ExecutarRequisicao(WebRequest request)
        {
            try
            {
                WebResponse retornoRequisicao = request.GetResponse();
                return (HttpWebResponse)retornoRequisicao;
            }
            catch (WebException webException)
            {
                if (webException.Response == null)
                    throw new Exception("Falha ao processar o retorno da API");

                return (HttpWebResponse)webException.Response;
            }
        }

        private static string ObterRetorno(HttpWebResponse response)
        {
            string jsonDadosRetornoRequisicao;
            using (System.IO.Stream streamDadosRetornoRequisicao = response.GetResponseStream())
            {
                System.IO.StreamReader leitorDadosRetornoRequisicao = new System.IO.StreamReader(streamDadosRetornoRequisicao);
                jsonDadosRetornoRequisicao = leitorDadosRetornoRequisicao.ReadToEnd();
                leitorDadosRetornoRequisicao.Close();
            }

            response.Close();

            return jsonDadosRetornoRequisicao.Replace("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n", "");
        }

        private static ServicoBuonny.viagem ObterObjetoViagem(Dominio.Entidades.SMViagemMDFe smViagemMDFe, string tokenBuonny, string cnpjjGerenciadoraRisco, string tipoProduto, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.DocumentoMunicipioDescarregamentoMDFe repDocumentosMDFe = new Repositorio.DocumentoMunicipioDescarregamentoMDFe(unidadeDeTrabalho);
            List<Dominio.Entidades.DocumentoMunicipioDescarregamentoMDFe> documentosMDFE = repDocumentosMDFe.BuscarPorMDFe(smViagemMDFe.MDFe.Codigo);

            Repositorio.MotoristaMDFe repMotoista = new Repositorio.MotoristaMDFe(unidadeDeTrabalho);
            List<Dominio.Entidades.MotoristaMDFe> motoristas = repMotoista.BuscarPorMDFe(smViagemMDFe.MDFe.Codigo);

            DateTime dataPrevisao = DateTime.Now.AddDays(3);

            ServicoBuonny.viagem viagem = new ServicoBuonny.viagem()
            {
                autenticacao = new ServicoBuonny.viagemAutenticacao()
                {
                    token = tokenBuonny
                },
                cnpj_cliente = smViagemMDFe.MDFe.Empresa.CNPJ,
                cnpj_embarcador = documentosMDFE.FirstOrDefault().CTe.Remetente.CPF_CNPJ,
                cnpj_transportador = smViagemMDFe.MDFe.Empresa.CNPJ,
                cnpj_gerenciadora_de_risco = cnpjjGerenciadoraRisco,
                pedido_cliente = smViagemMDFe.MDFe.Serie.Numero.ToString() + smViagemMDFe.MDFe.Numero.ToString(),
                tipo_de_transporte = "2",
                motorista = new ServicoBuonny.viagemMotorista()
                {
                    cpf = motoristas.FirstOrDefault().CPF,
                    nome = motoristas.FirstOrDefault().Nome,
                    telefone = Utilidades.String.OnlyNumbers(smViagemMDFe.MDFe.Empresa.Telefone),
                    radio = ""
                },
                veiculos = new string[] { smViagemMDFe.MDFe.Veiculos.FirstOrDefault().Placa },
                origem = ObterObjetoOrigemViagem(documentosMDFE.FirstOrDefault().CTe.Remetente),
                data_previsao_inicio = dataPrevisao.AddDays(-3),
                data_previsao_fim = dataPrevisao.AddMinutes(5),
                itinerario = ObterListaItinerarios(documentosMDFE, dataPrevisao, tipoProduto),
                monitorar_retorno = false,
                operacao_sm = ServicoBuonny.viagemOperacao_sm.I
            };

            return viagem;
        }

        private static ServicoBuonny.viagemOrigem ObterObjetoOrigemViagem(Dominio.Entidades.ParticipanteCTe origem)
        {
            return new ServicoBuonny.viagemOrigem()
            {
                codigo_externo = origem.CPF_CNPJ_SemFormato,
                descricao = Utilidades.String.Left(origem.Nome, 50),
                logradouro = Utilidades.String.Left(origem.Endereco, 100),
                numero = Utilidades.String.Left(Utilidades.String.OnlyNumbers(origem.Numero), 5),
                complemento = Utilidades.String.Left(origem.Complemento, 10),
                bairro = Utilidades.String.Left(origem.Bairro, 50),
                cidade = Utilidades.String.Left(origem.Localidade.Descricao, 50),
                estado = origem.Localidade.Estado.Sigla
            };
        }


        private static ServicoBuonny.viagemAlvo[] ObterListaItinerarios(List<Dominio.Entidades.DocumentoMunicipioDescarregamentoMDFe> documentosMDFE, DateTime dataPrevisao, string tipoProduto)
        {
            Dominio.Entidades.ParticipanteCTe clienteDestino = documentosMDFE.FirstOrDefault().CTe.Recebedor != null ? documentosMDFE.FirstOrDefault().CTe.Recebedor : documentosMDFE.FirstOrDefault().CTe.Destinatario;

            List<ServicoBuonny.viagemAlvo> itinerarios = new List<ServicoBuonny.viagemAlvo>();

            itinerarios.Add(ObterObjetoItinerario(clienteDestino, dataPrevisao, "3", tipoProduto, documentosMDFE.FirstOrDefault().MunicipioDescarregamento.MDFe.Numero.ToString(), documentosMDFE.FirstOrDefault().MunicipioDescarregamento.MDFe.Serie.Numero.ToString(), documentosMDFE.FirstOrDefault().MunicipioDescarregamento.MDFe.ValorTotalMercadoria, "1"));

            return itinerarios.ToArray();
        }

        private static ServicoBuonny.viagemAlvo ObterObjetoItinerario(Dominio.Entidades.ParticipanteCTe cliente, DateTime dataPrevisaoChegada, string tipoParada, string tipoProduto, string numero, string serie, decimal valor, string volume)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            ServicoBuonny.viagemAlvo itinerario = new ServicoBuonny.viagemAlvo
            {
                bairro = cliente.Bairro,
                cep = cliente.CEP,
                cidade = cliente.Localidade.Descricao,
                complemento = cliente.Complemento,
                dados_da_carga = new ServicoBuonny.viagemAlvoCarga[] {
                    new ServicoBuonny.viagemAlvoCarga(){
                        nf = numero,
                        serie_nf = serie,
                        peso = 0,
                        tipo_produto = string.IsNullOrWhiteSpace(tipoProduto) ? "33" : tipoProduto,
                        valor_total_nf = valor,
                        volume = volume
                    }
                },
                descricao = cliente.Nome,
                email = cliente.Email,
                estado = cliente.Localidade.Estado.Sigla,
                telefone = cliente.Telefone1,
                tipo_parada = tipoParada,
                codigo_externo = cliente.CPF_CNPJ_SemFormato,
                //janela_inicio = dataPrevisaoChegada.AddDays(-3),
                previsao_de_chegada = dataPrevisaoChegada
                //janela_fim = dataPrevisaoChegada.AddMinutes(5)
            };

            return itinerario;
        }

        private static string GerarXMLRequisicaoSolicitarMonitoramento(ServicoBuonny.viagem viagem, string urlWebService)
        {
            string objetoXml = XML.ConvertObjectToXMLString(viagem);

            bool isUrlProducao = !urlWebService.Contains("tstportal");
            string urlBaseNameSpaceXML = isUrlProducao ? "https://api.buonny.com.br" : "http://tstportal.buonny.com.br";

            objetoXml = objetoXml.Replace(" xmlns=\"https://api.buonny.com.br/portal/soap/buonny_soap\"", "");
            objetoXml = objetoXml.Replace("<viagem>", $"<viagem xmlns=\"{urlBaseNameSpaceXML}/portal/wsdl/buonny\">");

            return $@"<s:Envelope xmlns:s=""http://schemas.xmlsoap.org/soap/envelope/"">
  <s:Header>
    <Action s:mustUnderstand=""0"" xmlns=""http://schemas.microsoft.com/ws/2005/05/addressing/none"" />
  </s:Header>
  <s:Body xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
    {objetoXml}
  </s:Body>
</s:Envelope>";
        }


        private static void ExtrairInformacoesRetornoBuonny(ServicoBuonny.viagem_result response, InspectorBehavior inspector, out string mensagemRetorno, out string codigoSM)
        {
            mensagemRetorno = "";
            codigoSM = "";

            if (response != null)
            {
                mensagemRetorno = response.erro;
                codigoSM = response.codigo_sm;

                return;
            }

            if (inspector != null)
            {
                XDocument doc = XDocument.Parse(inspector.LastResponseXML);

                mensagemRetorno = doc.Descendants("viagem_result").Select(o => o.Element("erro").Value).FirstOrDefault();
                codigoSM = doc.Descendants("viagem_result").Select(o => o.Element("codigo_sm").Value).FirstOrDefault();
            }
        }


        public static ServicoBuonny.IncluirSmPortClient ObterClient(string url, string token)
        {
            url = url.ToLower();

            if (!url.EndsWith("/"))
                url += "/";

            ServicoBuonny.IncluirSmPortClient client = null;
            System.ServiceModel.EndpointAddress endpointAddress = new System.ServiceModel.EndpointAddress(url);

            if (url.StartsWith("https"))
            {
                //System.ServiceModel.WSHttpBinding binding = new System.ServiceModel.WSHttpBinding(System.ServiceModel.SecurityMode.None);
                System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding();

                binding.MaxReceivedMessageSize = int.MaxValue;
                binding.ReceiveTimeout = new TimeSpan(0, 5, 0);
                binding.SendTimeout = new TimeSpan(0, 5, 0);

                binding.Security.Mode = System.ServiceModel.BasicHttpSecurityMode.Transport;
                //binding.Security.Message.ClientCredentialType = System.ServiceModel.BasicHttpMessageCredentialType.Certificate;

                client = new ServicoBuonny.IncluirSmPortClient(binding, endpointAddress);
            }
            else
            {
                System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding();

                binding.MaxReceivedMessageSize = int.MaxValue;
                binding.ReceiveTimeout = new TimeSpan(0, 5, 0);
                binding.SendTimeout = new TimeSpan(0, 5, 0);

                client = new ServicoBuonny.IncluirSmPortClient(binding, endpointAddress);
            }

            System.ServiceModel.OperationContextScope scope = new System.ServiceModel.OperationContextScope(client.InnerChannel);

            return client;
        }

    }
}
