using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Integracao;
using Infrastructure.Services.HttpClientFactory;
using Servicos.Servico.Electrolux.OCOREN;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Servicos.Embarcador.Integracao.Electrolux
{
    public class IntegracaoElectroluxOCOREN
    {

        #region Atributos Globais

        private readonly Repositorio.UnitOfWork _unitOfWork;
        private readonly Repositorio.Embarcador.Configuracoes.IntegracaoElectrolux _repConfiguracaoIntegracao;
        private readonly Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoArquivo _repCargaCTeIntegracaoArquivo;
        private readonly Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao _repOcorrenciaCTeIntegracao;
        private readonly Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaDocumento _repCargaOcorrenciaDocumento;

        private readonly Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntregaIntegracao _ocorrenciaIntegracaoEntrega;
        private readonly Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao _ocorrenciaCTeIntegracao;
        private readonly Dominio.Entidades.ConhecimentoDeTransporteEletronico _cte;
        private readonly Dominio.Entidades.Embarcador.Cargas.Carga _cargaDominio;

        private readonly bool _isOcorrenciaEntrega;
        private readonly List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> _xmlNotasFiscaisIntegrar;

        private Dominio.Entidades.Embarcador.Configuracoes.IntegracaoElectrolux _configuracaoIntegracao;
        private Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal _xmlNotaFiscal;
        private Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoArquivo _arquivoIntegracao;

        private DT_RequestSendOcoren _requestOcoren;
        private HttpRequisicaoResposta _httpRequisicaoResposta;

        #endregion Atributos Globais

        #region CONSTRUTOR

        // Ocorrências gerais
        public IntegracaoElectroluxOCOREN(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao ocorrenciaCTeIntegracao)
        {
            _unitOfWork = unitOfWork;
            _ocorrenciaCTeIntegracao = ocorrenciaCTeIntegracao;
            _isOcorrenciaEntrega = false;
            _configuracaoIntegracao = new Dominio.Entidades.Embarcador.Configuracoes.IntegracaoElectrolux();
            _repConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.IntegracaoElectrolux(_unitOfWork);
            _repCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoArquivo(_unitOfWork);
            _repOcorrenciaCTeIntegracao = new Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao(unitOfWork);
            _repCargaOcorrenciaDocumento = new Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaDocumento(_unitOfWork);

            if (ocorrenciaCTeIntegracao.CargaOcorrencia.UtilizarSelecaoPorNotasFiscaisCTe)
            {
                Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaDocumento cargaOcorrenciaDocumento = _repCargaOcorrenciaDocumento.BuscarPorCargaCTeEOcorrencia(ocorrenciaCTeIntegracao.CargaCTe.Codigo, ocorrenciaCTeIntegracao.CargaOcorrencia.Codigo);
                _xmlNotasFiscaisIntegrar = cargaOcorrenciaDocumento.XMLNotaFiscais?.ToList();
            }
            else
            {
                _xmlNotasFiscaisIntegrar = ocorrenciaCTeIntegracao.CargaCTe.CTe.XMLNotaFiscais.ToList();
            }

            _cte = ocorrenciaCTeIntegracao.CargaCTe.CTe;
            _xmlNotaFiscal = new Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal();
            _arquivoIntegracao = new Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoArquivo();
            _cargaDominio = ocorrenciaCTeIntegracao.CargaCTe.Carga;
            _requestOcoren = new DT_RequestSendOcoren();
            _httpRequisicaoResposta = new HttpRequisicaoResposta();
        }

        // Ocorrências de entrega
        public IntegracaoElectroluxOCOREN(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntregaIntegracao ocorrenciaIntegracaoEntrega)
        {
            _unitOfWork = unitOfWork;
            _ocorrenciaIntegracaoEntrega = ocorrenciaIntegracaoEntrega;
            _isOcorrenciaEntrega = true;
            _configuracaoIntegracao = new Dominio.Entidades.Embarcador.Configuracoes.IntegracaoElectrolux();
            _repConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.IntegracaoElectrolux(_unitOfWork);
            _repOcorrenciaCTeIntegracao = new Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao(unitOfWork);
            _xmlNotasFiscaisIntegrar = _ocorrenciaIntegracaoEntrega.PedidoOcorrenciaColetaEntrega.Pedido.NotasFiscais.ToList();
            _xmlNotaFiscal = new Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal();
            _cte = new Dominio.Entidades.ConhecimentoDeTransporteEletronico();
            _cargaDominio = ocorrenciaIntegracaoEntrega.PedidoOcorrenciaColetaEntrega.Carga;
            _requestOcoren = new DT_RequestSendOcoren();
            _httpRequisicaoResposta = new HttpRequisicaoResposta();
        }

        #endregion

        #region PÚBLICOS

        public Dominio.ObjetosDeValor.Embarcador.Integracao.HttpRequisicaoResposta IntegrarOcorrencia()
        {
            if (_ocorrenciaCTeIntegracao == null && _ocorrenciaIntegracaoEntrega == null)
            {
                _httpRequisicaoResposta.sucesso = false;
                _httpRequisicaoResposta.mensagem = "Não existe dado de integração disponível para Electrolux";
                return _httpRequisicaoResposta;
            }

            _configuracaoIntegracao = _repConfiguracaoIntegracao.BuscarPrimeiroRegistro();

            if (!_configuracaoIntegracao.ElectroluxPossuiIntegracao)
            {
                _httpRequisicaoResposta.sucesso = false;
                _httpRequisicaoResposta.mensagem = "Configuração da integração com a Electrolux não encontrada";

                InformarSituacaoIntegracao(false, _httpRequisicaoResposta.mensagem, null, null);

                return _httpRequisicaoResposta;
            }

            ExecutarIntegracaoAsync().Wait();

            return _httpRequisicaoResposta;
        }

        #endregion

        #region OCOREN

        private async Task ExecutarIntegracaoAsync()
        {
            try
            {
                var soapEnvelope = GerarRequestEnvelope();

                var httpClient = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoElectroluxOCOREN));

                var auth = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_configuracaoIntegracao.Usuario}:{_configuracaoIntegracao.Senha}"));
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", auth);
                var content = new StringContent(soapEnvelope, Encoding.UTF8, "text/xml");

                var response = await httpClient.PostAsync(_configuracaoIntegracao.UrlOcorenService, content).ConfigureAwait(false);

                _httpRequisicaoResposta.sucesso = response.IsSuccessStatusCode;

                if (_httpRequisicaoResposta.sucesso)
                {
                    var errorMessage = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    var responseXmlDoc = new XmlDocument();
                    responseXmlDoc.LoadXml(errorMessage);
                    var faultStringNode = responseXmlDoc.GetElementsByTagName("status").Item(0);

                    if (faultStringNode != null && faultStringNode.InnerText != "ERROR")
                    {
                        _httpRequisicaoResposta.mensagem = "Registro transmitido com sucesso";
                    }
                    else
                    {
                        _httpRequisicaoResposta.sucesso = false;
                        var faultStringNodeMessage = responseXmlDoc.GetElementsByTagName("message").Item(0);

                        _httpRequisicaoResposta.mensagem = (faultStringNodeMessage != null)
                            ? faultStringNodeMessage.InnerText
                            : $"Erro desconhecido. Detalhes: {response.ReasonPhrase}";
                    }
                }
                else
                {
                    await SetErrorMessageXML(response).ConfigureAwait(false);
                }

                InformarSituacaoIntegracao(_httpRequisicaoResposta.sucesso, _httpRequisicaoResposta.mensagem, soapEnvelope, $"Mensagem Erro: {_httpRequisicaoResposta.mensagem}, Retorno WebService: {_httpRequisicaoResposta.conteudoResposta}");
            }
            catch (Exception ex)
            {
                _httpRequisicaoResposta.sucesso = false;
                _httpRequisicaoResposta.mensagem = $"Erro ao executar a integração com Electrolux OCOREN: {ex.Message}";
                Log.TratarErro(ex);

                InformarSituacaoIntegracao(_httpRequisicaoResposta.sucesso, _httpRequisicaoResposta.mensagem, null, null);
            }
        }

        private string GerarRequestEnvelope()
        {
            ObterRequest();
            return GerarXmlSoap(_requestOcoren);
        }

        private void ObterRequest()
        {

            var hoje = DateTime.Now;
            var identIntercambio = $"OCO50{DateTime.Now:ddMMfff}";

            // Inicia a montagem do request
            var r = _requestOcoren;

            r.identService = new Servico.Electrolux.OCOREN.DT_IdentService_SAP()
            {
                identCarrier = (_cargaDominio.Empresa.Localidade.Estado.Sigla.Equals("PR")) ? "1107588" : "1168178"
            };

            r.registro000 = new DT_RequestSendOcorenRegistro000
            {
                detail = new Servico.Electrolux.OCOREN.DT_Registro000_SAP
                {
                    identRegistro = "000",
                    identRemetente = sf(_cte.Remetente.Nome, 35),
                    identDest = sf(_cte.Destinatario.Nome, 35),
                    data = hoje,
                    hora = hoje,
                    identIntercambio = identIntercambio
                },
                registro540 = ObterInformacoesTransportadora()
            };

        }

        private DT_RequestSendOcorenRegistro000Registro540[] ObterInformacoesTransportadora()
        {

            var nfs = new DT_RequestSendOcorenRegistro000Registro540[1];

            var empresa = _isOcorrenciaEntrega
                ? _ocorrenciaIntegracaoEntrega.PedidoOcorrenciaColetaEntrega.Carga?.Empresa
                : _ocorrenciaCTeIntegracao.CargaCTe.Carga.Empresa;

            nfs[0] = new DT_RequestSendOcorenRegistro000Registro540
            {
                identRegistro = "540",
                identDocumento = $"OCORR50{DateTime.Now:ddMMfff}",
                registro541 = new DT_RequestSendOcorenRegistro000Registro540Registro541
                {
                    identRegistro = "541",
                    CnpjTransp = sf(empresa?.CNPJ_SemFormato ?? string.Empty, 14), // CNPJ (CGC) DA TRANSPORTADORA,
                    RazaoSocTransp = sf(empresa?.RazaoSocial, 14), // RAZÃO SOCIAL DA TRANSPORTADORA,
                    registro542 = ObterInformacoesOcorrenciaEntrega() // << Informações das notas ficais
                },
                registro549 = new DT_RequestSendOcorenRegistro000Registro540Registro549
                {
                    identRegistro = "549",
                    NumeroOcorrencia = sf(_xmlNotasFiscaisIntegrar.Count.ToString(), 4),
                    filler = " " //<< Precisa existir, se não da erro transmissão
                }
            };

            return nfs;
        }

        private DT_RequestSendOcorenRegistro000Registro540Registro541Registro542[] ObterInformacoesOcorrenciaEntrega()
        {
            var retorno = new List<DT_RequestSendOcorenRegistro000Registro540Registro541Registro542>();

            var codigoOcorrencia = "000";

            codigoOcorrencia = _isOcorrenciaEntrega // Trata a data posteriormente direto no xml
                ? sfZerosEsquerda(_ocorrenciaIntegracaoEntrega.PedidoOcorrenciaColetaEntrega.Codigo.ToString(), 3)
                : sfZerosEsquerda(_ocorrenciaCTeIntegracao.CargaOcorrencia.TipoOcorrencia.CodigoIntegracao, 3);

            var dataOcorrencia = _isOcorrenciaEntrega
                ? _ocorrenciaIntegracaoEntrega.PedidoOcorrenciaColetaEntrega.DataOcorrencia
                : _ocorrenciaCTeIntegracao.CargaOcorrencia.DataOcorrencia;

            var horaOcorrencia = dataOcorrencia; // Faz o tratamento de data e hora depois de gerar o xml

            foreach (var nf in _xmlNotasFiscaisIntegrar)
            {
                retorno.Add(new DT_RequestSendOcorenRegistro000Registro540Registro541Registro542
                {
                    identRegistro = "542",
                    CnpjEmissorNf = sf(nf.CNPJTranposrtador, 14),
                    SerieNf = sf(nf.Serie, 3),
                    NumeroNf = sf(nf.Numero, 9),
                    CodOcorrenciaEnt = sfZerosEsquerda(codigoOcorrencia, 3),
                    DataOcorrencia = dataOcorrencia,
                    HoraOcorrencia = horaOcorrencia,
                    SerieConhecimento = _isOcorrenciaEntrega ? " " : sf(_cte.Serie.Descricao, 3),
                    NumeroConhecimento = _isOcorrenciaEntrega ? " " : sf(_cte.Numero, 12)
                });
            }

            return retorno.ToArray();
        }

        #endregion

        #region PRIVADOS GERAIS

        public static string GerarXmlSoap<T>(T objeto)
        {
            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add("con", "http://electrolux.com/SD/OL/Ocoren");

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));

            using (StringWriter stringWriter = new StringWriter())
            {
                XmlWriterSettings settings = new XmlWriterSettings
                {
                    Encoding = Encoding.UTF8,
                    Indent = true,
                    OmitXmlDeclaration = true
                };

                using (XmlWriter writer = XmlWriter.Create(stringWriter, settings))
                {
                    writer.WriteStartElement("soapenv", "Envelope", "http://schemas.xmlsoap.org/soap/envelope/");
                    writer.WriteAttributeString("xmlns", "soapenv", null, "http://schemas.xmlsoap.org/soap/envelope/");
                    writer.WriteAttributeString("xmlns", "ocor", null, "http://electrolux.com/SD/OL/Ocoren");

                    writer.WriteStartElement("soapenv", "Header", "http://schemas.xmlsoap.org/soap/envelope/");
                    writer.WriteEndElement();
                    writer.WriteStartElement("soapenv", "Body", "http://schemas.xmlsoap.org/soap/envelope/");

                    xmlSerializer.Serialize(writer, objeto, namespaces);

                    writer.WriteEndElement();
                    writer.WriteEndElement();
                }

                var xmlDoc = new XmlDocument();
                xmlDoc.PreserveWhitespace = true;
                xmlDoc.LoadXml(stringWriter.ToString());

                // Formata a data e hora como especificado
                FormatXmlDateTime(xmlDoc, "detail", "data", "date");
                FormatXmlDateTime(xmlDoc, "detail", "hora", "time");
                FormatXmlDateTime(xmlDoc, "registro542", "DataOcorrencia", "date");
                FormatXmlDateTime(xmlDoc, "registro542", "HoraOcorrencia", "time");

                // Ajusta a tag inicial para atender o request
                var xmlFormatado = AjustarPrefixoTag(xmlDoc.OuterXml);

                return xmlFormatado;
            }
        }

        private void InformarSituacaoIntegracao(bool isIntegrado, string mensagem, string requestXML, string responseXML)
        {
            _ocorrenciaCTeIntegracao.NumeroTentativas += 1;
            _ocorrenciaCTeIntegracao.DataIntegracao = DateTime.Now;

            _ocorrenciaCTeIntegracao.SituacaoIntegracao = isIntegrado
                ? SituacaoIntegracao.Integrado
                : SituacaoIntegracao.ProblemaIntegracao;

            _ocorrenciaCTeIntegracao.ProblemaIntegracao = !isIntegrado ? mensagem : "";

            if (!string.IsNullOrEmpty(requestXML))
            {
                _arquivoIntegracao.Mensagem = _ocorrenciaCTeIntegracao.ProblemaIntegracao;
                _arquivoIntegracao.Data = _ocorrenciaCTeIntegracao.DataIntegracao;
                _arquivoIntegracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;

                _arquivoIntegracao.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(requestXML, "xml", _unitOfWork);
                if (!string.IsNullOrEmpty(responseXML))
                    _arquivoIntegracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(responseXML, "xml", _unitOfWork);

                _arquivoIntegracao.Mensagem = _ocorrenciaCTeIntegracao.ProblemaIntegracao;

                _repCargaCTeIntegracaoArquivo.Inserir(_arquivoIntegracao);
                _ocorrenciaCTeIntegracao.ArquivosTransacao.Add(_arquivoIntegracao);
            }

            _repOcorrenciaCTeIntegracao.Atualizar(_ocorrenciaCTeIntegracao);
        }

        /// <summary>
        /// Formata a string informada para o tamanho máximo exigido pelo serviço
        /// Deixei com o nome curto para não "poluir" o código
        /// </summary>
        /// <param name="texto">Texto para formatação</param>
        /// <param name="tamanhoMaximo">Quantidade de caracteres</param>
        /// <returns>String formatada</returns>
        private static string sf(string texto, int tamanhoMaximo)
        {
            if (string.IsNullOrEmpty(texto) || string.IsNullOrWhiteSpace(texto))
                return string.Empty;
            else if (texto.Length > tamanhoMaximo)
                return texto.Substring(0, tamanhoMaximo);
            else
                return texto;
        }

        private string sf(int numero, int tamanhoMaximo)
        {
            return sf(numero.ToString(), tamanhoMaximo);
        }

        private static string sfZerosEsquerda(string texto, int tamanhoMaximo)
        {
            if (texto == null)
                texto = string.Empty;

            if (texto.Length > tamanhoMaximo)
                texto = texto.Substring(0, tamanhoMaximo);

            return texto.PadLeft(tamanhoMaximo, '0');
        }

        private static void FormatXmlDateTime(XmlDocument doc, string parentTag, string fieldTag, string formatType)
        {
            XmlNodeList parentNodes = doc.GetElementsByTagName(parentTag);

            foreach (XmlNode parentNode in parentNodes)
            {
                XmlNode fieldNode = parentNode[fieldTag];

                if (fieldNode != null)
                {
                    if (formatType == "date")
                    {
                        DateTime dateValue;
                        if (DateTime.TryParse(fieldNode.InnerText, out dateValue))
                        {
                            fieldNode.InnerText = dateValue.ToString("yyyy-MM-dd");
                        }
                    }
                    else if (formatType == "time")
                    {
                        DateTime timeValue;
                        if (DateTime.TryParse(fieldNode.InnerText, out timeValue))
                        {
                            fieldNode.InnerText = timeValue.ToString("HH:mm:ss");
                        }
                    }
                }
            }
        }

        private static string AjustarPrefixoTag(string xmlContent)
        {
            return xmlContent.Replace($"<DT_RequestSendOcoren>", $"<ocor:MT_RequestSendOcoren_EDI>").Replace($"</DT_RequestSendOcoren>", $"</ocor:MT_RequestSendOcoren_EDI>");
        }

        private async Task SetErrorMessageXML(HttpResponseMessage httpResponseMessage)
        {
            try
            {
                _httpRequisicaoResposta.conteudoResposta = await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
                var responseXmlDoc = new XmlDocument();
                responseXmlDoc.LoadXml(_httpRequisicaoResposta.conteudoResposta);
                var faultStringNode = responseXmlDoc.GetElementsByTagName("message").Item(0);

                if (faultStringNode != null)
                {
                    _httpRequisicaoResposta.mensagem = faultStringNode.InnerText;
                }
                else
                {
                    _httpRequisicaoResposta.mensagem = $"Erro desconhecido. Detalhes: {httpResponseMessage.ReasonPhrase}";
                }
            }
            catch (Exception ex)
            {
                _httpRequisicaoResposta.mensagem = $"Erro ao processar a mensagem de erro XML: {ex.Message}";
            }

            Log.TratarErro(_httpRequisicaoResposta.mensagem);
        }

        #endregion


    }

}
