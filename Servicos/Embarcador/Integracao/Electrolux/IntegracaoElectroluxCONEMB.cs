using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Integracao;
using Infrastructure.Services.HttpClientFactory;
using Servicos.Servico.Electrolux.CONEMB;
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
    public class IntegracaoElectroluxCONEMB
    {

        #region Atributos Globais

        private readonly Repositorio.UnitOfWork _unitOfWork;
        private readonly Repositorio.Embarcador.Configuracoes.IntegracaoElectrolux _configuracaoIntegracaoRepositorio;
        private readonly Repositorio.Embarcador.Cargas.CargaCargaIntegracao _cargaIntegracaoRepositorio;
        private readonly Repositorio.ComponentePrestacaoCTE _componentePrestacaoCTERepositorio;
        private readonly Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo _cargaCTeIntegracaoArquivoRepositorio;
        private readonly Repositorio.Embarcador.Cargas.CargaPedido _cargaPedidoRepositorio;

        private readonly Dominio.Entidades.Embarcador.Cargas.Carga _cargaDominio;

        private Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao _cargaIntegracaoDominio;
        private Dominio.Entidades.Embarcador.Configuracoes.IntegracaoElectrolux _configuracaoIntegracaoDominio;
        private Dominio.Entidades.ComponentePrestacaoCTE _componentePrestacaoCTEDominio;
        private Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo _arquivoIntegracaoDominio;
        private Dominio.Entidades.Embarcador.Cargas.CargaPedido _cargaPedidoDominio;

        private DT_RequestSendConemb _requestConemb;
        private HttpRequisicaoResposta _httpRequisicaoResposta;

        private string _tipoCifFob = "";
        private decimal _baseICMS = 0m;
        private decimal _valorICMS = 0m;
        private decimal _baseAliquota = 0m;
        private decimal _baseICMSFrete = 0m;
        private decimal _valorICMSFrete = 0m;

        #endregion Atributos Globais

        #region CONSTRUTOR

        public IntegracaoElectroluxCONEMB(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao)
        {
            _unitOfWork = unitOfWork;
            _configuracaoIntegracaoRepositorio = new Repositorio.Embarcador.Configuracoes.IntegracaoElectrolux(_unitOfWork);
            _cargaIntegracaoRepositorio = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(_unitOfWork);
            _componentePrestacaoCTERepositorio = new Repositorio.ComponentePrestacaoCTE(_unitOfWork);
            _cargaCTeIntegracaoArquivoRepositorio = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(_unitOfWork);
            _cargaPedidoRepositorio = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            _configuracaoIntegracaoDominio = new Dominio.Entidades.Embarcador.Configuracoes.IntegracaoElectrolux();
            _componentePrestacaoCTEDominio = new Dominio.Entidades.ComponentePrestacaoCTE();
            _arquivoIntegracaoDominio = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo();
            _cargaIntegracaoDominio = cargaIntegracao;
            _cargaDominio = cargaIntegracao.Carga;
            _httpRequisicaoResposta = new HttpRequisicaoResposta();
        }

        #endregion

        #region PÚBLICOS

        public HttpRequisicaoResposta IntegrarCarga()
        {
            _httpRequisicaoResposta.sucesso = false;

            if (_cargaIntegracaoDominio == null || _cargaIntegracaoDominio.Carga?.CargaCTes?.Count == 0)
            {
                _httpRequisicaoResposta.mensagem = "Não existe dado de integração disponível para Electrolux";
                InformarSituacaoIntegracao(_httpRequisicaoResposta.sucesso, _httpRequisicaoResposta.mensagem, null, null);
                return _httpRequisicaoResposta;
            }

            _configuracaoIntegracaoDominio = _configuracaoIntegracaoRepositorio.BuscarPrimeiroRegistro();

            if (!_configuracaoIntegracaoDominio.ElectroluxPossuiIntegracao)
            {
                _httpRequisicaoResposta.mensagem = "Configuração da integração com a Electrolux não encontrada";
                InformarSituacaoIntegracao(_httpRequisicaoResposta.sucesso, _httpRequisicaoResposta.mensagem, null, null);
                return _httpRequisicaoResposta;
            }

            ExecutarIntegracaoAsync().Wait();

            return _httpRequisicaoResposta;
        }


        #endregion

        #region CONEMB

        private async Task ExecutarIntegracaoAsync()
        {
            try
            {
                string soapEnvelope = GerarRequestEnvelope();

                try
                {
                    var httpClient = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoElectroluxCONEMB));

                    var auth = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_configuracaoIntegracaoDominio.Usuario}:{_configuracaoIntegracaoDominio.Senha}"));
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", auth);
                    var content = new StringContent(soapEnvelope, Encoding.UTF8, "text/xml");

                    var response = await httpClient.PostAsync(_configuracaoIntegracaoDominio.URLCONEMB, content).ConfigureAwait(false);

                    _httpRequisicaoResposta.sucesso = response.IsSuccessStatusCode;

                    if (_httpRequisicaoResposta.sucesso)
                    {
                        _httpRequisicaoResposta.conteudoResposta = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                        var responseXmlDoc = new XmlDocument();
                        responseXmlDoc.LoadXml(_httpRequisicaoResposta.conteudoResposta);
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
                    InformarSituacaoIntegracao(false, $"Erro ao executrar a integração com Electrolux CONEMB: {ex.Message}", null, null);
                    Log.TratarErro(ex);
                }
            }
            catch (Exception ex)
            {
                InformarSituacaoIntegracao(false, $"Erro ao executar a integração com Electrolux CONEMB: {ex.Message}", null, null);
                Log.TratarErro(ex);
            }
        }

        private string GerarRequestEnvelope()
        {
            ObterRequestCONEMB();
            return GerarXmlSoap(_requestConemb);
        }

        private void ObterRequestCONEMB()
        {
            var hoje = DateTime.Now;

            var identificador = _cargaDominio.Empresa?.Configuracao?.IdentificadorTransportadorElectrolux;
            // (_cargaDominio.Empresa.Localidade.Estado.Sigla.Equals("PR")) ? "1107588" : "1168178"

            //CON50DDMMSSS
            var identIntercambio = $"CON50{DateTime.Now:ddMMfff}";
            var identDocumento = $"CONHE50{DateTime.Now:ddMMfff}";

            var totalValorCTes = _cargaDominio.CargaCTes.Sum(carga => carga.CTe.ValorTotalMercadoria);

            _cargaPedidoDominio = _cargaPedidoRepositorio.Busca(_cargaDominio.Codigo);

            _tipoCifFob = _cargaPedidoDominio.DescricaoTipoPagamentoCIFFOB;

            _requestConemb = new DT_RequestSendConemb
            {
                identService = new DT_IdentService_SAP
                {
                    identCarrier = identificador
                },
                registro000 = new DT_RequestSendConembRegistro000
                {
                    detail = new DT_Registro000_SAP
                    {
                        identRegistro = "000",
                        identRemetente = sf(_cargaDominio.CargaCTes[0].CTe.Remetente.Nome, 35),
                        identDest = sf(_cargaDominio.CargaCTes[0].CTe.Destinatario.Nome, 35),
                        data = hoje,
                        hora = hoje,
                        identIntercambio = identIntercambio,
                        //filler = null
                    },
                    registro520 = new DT_RequestSendConembRegistro000Registro520[]
                    {

                        new DT_RequestSendConembRegistro000Registro520
                        {
                            identRegistro = "520",
                            identDocumento = identDocumento,
                            //filler = null,
                            registro521 = new DT_RequestSendConembRegistro000Registro520Registro521
                            {
                                identRegistro = "521",
                                CnpjTransp = sf(_cargaDominio.Empresa.CNPJ, 14),
                                NomeTransp = sf(_cargaDominio.Empresa.Descricao, 50),
                                //Filler = null,
                                registro522 = ObterCTesDaCarga()

                            },
                            registro529 = new DT_RequestSendConembRegistro000Registro520Registro529
                            {
                                identRegistro = "529",
                                QtdeTotConhec = sf(_cargaIntegracaoDominio.Carga.CargaCTes.Count, 4),
                                ValorTotConhec = toDecimal(totalValorCTes, 2),
                                //FILLER = null
                            }
                        }
                    }


                }
            };

        }

        private DT_RequestSendConembRegistro000Registro520Registro521Registro522[] ObterCTesDaCarga()
        {
            var protocolo = _cargaDominio.Protocolo.ToString();
            var placa = _cargaDominio.Veiculo.Placa;
            var numeroCarregamento = String.Join(",", _cargaDominio.Pedidos.Select(x => x.Pedido.NumeroPedidoEmbarcador).ToArray());
            var numeroPreCalculo = _cargaDominio.Pedidos.Where(x => x.Pedido.NumeroControle != null).Select(x => x.Pedido.NumeroControle).FirstOrDefault().ToString();
            var retorno = new List<DT_RequestSendConembRegistro000Registro520Registro521Registro522>();

            foreach (var cteDaCarga in _cargaDominio.CargaCTes)
            {
                ObterCalculos(cteDaCarga.CTe);

                var placaVeiculo = placa ?? sf(cteDaCarga.CTe.Veiculos.FirstOrDefault()?.Placa, 9);

                var condFrete = _tipoCifFob.Equals("FOB")
                    ? DT_RequestSendConembRegistro000Registro520Registro521Registro522CondFrete.F
                    : DT_RequestSendConembRegistro000Registro520Registro521Registro522CondFrete.C;

                var codNumChaveAcesso = !string.IsNullOrEmpty(cteDaCarga.CTe?.NumeroRecibo)
                    ? sf(cteDaCarga.CTe?.NumeroRecibo, 9)
                    : null;

                retorno.Add(new DT_RequestSendConembRegistro000Registro520Registro521Registro522
                {
                    identRegistro = "522",
                    SerieConhec = sf(cteDaCarga.CTe.Serie.Numero, 5),
                    NumeroConhec = sf(cteDaCarga.CTe.Numero.ToString(), 12),
                    DataEmConhec = cteDaCarga.CTe.DataEmissao ?? DateTime.MinValue,
                    CondFrete = condFrete,
                    CnpjEmConhec = sf(_cargaDominio.Empresa.CNPJ, 14), // <-- Mesno que CnpjTransp
                    CnpjEmNf = sf(cteDaCarga.CTe.Empresa.CNPJ, 14),
                    ChaveConsDv = !string.IsNullOrEmpty(cteDaCarga.CTe.ChaveAcesso) ? cteDaCarga.CTe.ChaveAcesso : null,
                    Protocolo = sf(protocolo, 15),
                    CodNumChaveAcesso = codNumChaveAcesso,
                    TipoConhec = DT_RequestSendConembRegistro000Registro520Registro521Registro522TipoConhec.N,
                    TipoFrete = DT_RequestSendConembRegistro000Registro520Registro521Registro522TipoFrete.Item1,
                    AcaoDoc = DT_RequestSendConembRegistro000Registro520Registro521Registro522AcaoDoc.I,
                    CalcFreteDif = DT_RequestSendConembRegistro000Registro520Registro521Registro522CalcFreteDif.N,
                    PlanoCargaRapida = DT_RequestSendConembRegistro000Registro520Registro521Registro522PlanoCargaRapida.N,
                    PlacaVeiculo = placaVeiculo,
                    NumCarregamento = sf(numeroCarregamento, 20),
                    //TpMeioTransp = "BR11",
                    //CnpjDestNfConhec = sf(cteDaCarga.CTe.Destinatario?.CPF_CNPJ, 14),
                    NumPreCalc = sf(numeroPreCalculo, 20),
                    //FilialEmConhec = sf(cteDaCarga.CTe.Empresa.CNPJ, 10),
                    //Filler = null,
                    registro523 = new DT_RequestSendConembRegistro000Registro520Registro521Registro522Registro523
                    {
                        identRegistro = "523",
                        QtdeTotVolEmb = toDecimal(cteDaCarga.NotasFiscais.Sum(p => Convert.ToDecimal(p.PedidoXMLNotaFiscal.XMLNotaFiscal?.Volumes)), 2),
                        PesoTotTransp = toDecimal(cteDaCarga.CTe?.Peso, 4),
                        PesoTotCubado = toDecimal(cteDaCarga.CTe?.PesoCubado, 4),
                        Frete = toDecimal(cteDaCarga.CTe?.ValorFrete, 2),
                        TotalFrete = toDecimal(cteDaCarga.CTe?.ValorAReceber, 2),     
                        
                        BaseIcms = toDecimal(_baseICMS, 2),
                        RateIcms = toDecimal(_baseAliquota, 2),
                        ValueIcms = toDecimal(_valorICMS, 2),                       
                        IcmsStFlag = (cteDaCarga.CTe?.CST == "60")
                                                ? DT_RequestSendConembRegistro000Registro520Registro521Registro522Registro523IcmsStFlag.Item2
                                                : DT_RequestSendConembRegistro000Registro520Registro521Registro522Registro523IcmsStFlag.Item3,
                        //Filler = null,

                    },
                    registro524 = ObterNotasFiscaisCTes(cteDaCarga)
                });

            }

            return retorno.ToArray();
        }

        private DT_RequestSendConembRegistro000Registro520Registro521Registro522Registro524[] ObterNotasFiscaisCTes(Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe)
        {
            var retorno = new List<DT_RequestSendConembRegistro000Registro520Registro521Registro522Registro524>();

            foreach (var nf in cargaCTe.NotasFiscais)
            {
                retorno.Add(new DT_RequestSendConembRegistro000Registro520Registro521Registro522Registro524
                {
                    identRegistro = "524",
                    CnpjEmNf = nf.CargaCTe.CTe.Empresa.CNPJ,
                    NumeroNf = sf(nf.CargaCTe.CTe.Numero, 9),
                    SerieEmNf = sf(nf.CargaCTe.CTe.Serie.Numero, 3),
                    DataEmNf = nf.CargaCTe.CTe.DataEmissao ?? DateTime.MinValue,
                    ValorNf = toDecimal(nf.PedidoXMLNotaFiscal.ValorTotalAReceberComICMSeISS, 2),
                    QtdeTotVolEmb = toDecimal(nf.CargaCTe.CTe.Volumes, 2),
                    PesoBrutoTot = toDecimal(nf.CargaCTe.CTe.Peso, 3),
                    PesoCubado = toDecimal(nf.CargaCTe.CTe.PesoCubado, 4),
                    IdPedido = nf.CargaCTe.CTe.NumeroPedido,                    
                    TipoNf = nf.CargaCTe.CTe.CFOP.Tipo == Dominio.Enumeradores.TipoCFOP.Entrada
                        ? DT_RequestSendConembRegistro000Registro520Registro521Registro522Registro524TipoNf.Item0
                        : DT_RequestSendConembRegistro000Registro520Registro521Registro522Registro524TipoNf.Item1,
                    CodFiscOperacao = sf((int)nf.CargaCTe.CTe.CFOP.Tipo, 4),
                    filler = null
                });
            }

            return retorno.ToArray();
        }

        private void ObterCalculos(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)
        {
            try
            {
                _componentePrestacaoCTEDominio = _componentePrestacaoCTERepositorio.BuscarPorCTeTipo(cte.Codigo, TipoComponenteFrete.ADVALOREM)?.FirstOrDefault()
                    ?? new Dominio.Entidades.ComponentePrestacaoCTE();

                _baseICMS = Math.Round(_componentePrestacaoCTEDominio.Valor, 2, MidpointRounding.ToEven);
                _valorICMS = Math.Round((_componentePrestacaoCTEDominio.Valor * cte.AliquotaICMS / 100), 2, MidpointRounding.ToEven);

                _baseICMSFrete = Math.Round((cte.ValorFrete), 2, MidpointRounding.ToEven);
                _valorICMSFrete = cte.AliquotaICMS > 0 ? Math.Round(((cte.ValorFrete) * cte.AliquotaICMS / 100), 2, MidpointRounding.ToEven) : 0;

                _baseAliquota = 0m;
                if (cte.CST != "60")
                {
                    _baseICMS = _componentePrestacaoCTEDominio.Valor > 0 ? Math.Round((_componentePrestacaoCTEDominio.Valor / _baseAliquota), 2, MidpointRounding.ToEven) : 0;
                    _valorICMS = Math.Round((_baseICMS * cte.AliquotaICMS / 100), 2, MidpointRounding.ToEven);
                    _baseAliquota = cte.AliquotaICMS > 0 ? (100 - cte.AliquotaICMS) / 100 : 0;
                    _baseICMSFrete = _baseAliquota > 0 ? Math.Round((cte.ValorFrete / _baseAliquota), 2, MidpointRounding.ToEven) : 0;
                    _valorICMSFrete = _baseICMSFrete > 0 ? Math.Round((_baseICMSFrete * cte.AliquotaICMS / 100), 2, MidpointRounding.ToEven) : 0;
                }
            }
            catch (Exception ex)
            {
                _httpRequisicaoResposta.mensagem = $"Erro ao executar cálculos da CTe: {ex.Message}";
            }

        }

        #endregion

        #region PRIVADOS GERAIS

        public static string GerarXmlSoap<T>(T objeto)
        {
            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add("con", "http://electrolux.com/SD/OL/Conemb");

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
                    writer.WriteAttributeString("xmlns", "con", null, "http://electrolux.com/SD/OL/Conemb");

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
                FormatXmlDateTime(xmlDoc, "registro542", "DataEmConhec", "date");
                FormatXmlDateTime(xmlDoc, "registro524", "DataEmNf", "date");

                // Ajusta a tag inicial para atender o request
                var xmlFormatado = AjustarPrefixoTag(xmlDoc.OuterXml);

                return xmlFormatado;
            }
        }

        private void InformarSituacaoIntegracao(bool isIntegrado, string mensagem, string requestXML, string responseXML)
        {
            _cargaIntegracaoDominio.NumeroTentativas += 1;
            _cargaIntegracaoDominio.DataIntegracao = DateTime.Now;

            _cargaIntegracaoDominio.SituacaoIntegracao = isIntegrado
                ? SituacaoIntegracao.Integrado
                : SituacaoIntegracao.ProblemaIntegracao;

            _cargaIntegracaoDominio.ProblemaIntegracao = !isIntegrado ? mensagem : "";

            if (!string.IsNullOrEmpty(requestXML))
            {
                _arquivoIntegracaoDominio.Mensagem = _cargaIntegracaoDominio.ProblemaIntegracao;
                _arquivoIntegracaoDominio.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(requestXML, "xml", _unitOfWork);

                if (!string.IsNullOrEmpty(responseXML))
                    _arquivoIntegracaoDominio.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(responseXML, "xml", _unitOfWork);

                _arquivoIntegracaoDominio.Data = DateTime.Now;

                _arquivoIntegracaoDominio.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.RetornoDoProcessamento;

                _cargaCTeIntegracaoArquivoRepositorio.Inserir(_arquivoIntegracaoDominio);

                _cargaIntegracaoDominio.ArquivosTransacao.Add(_arquivoIntegracaoDominio);
            }

            _cargaIntegracaoRepositorio.Atualizar(_cargaIntegracaoDominio);
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

        private decimal toDecimal(decimal valor, int casasDecimais)
        {
            return Math.Round(valor, casasDecimais);
        }

        private decimal toDecimal(decimal? valor, int casasDecimais)
        {
            var v = valor != null ? valor : 0;
            return Math.Round((decimal)v, casasDecimais);
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
                            fieldNode.InnerText = dateValue.ToString("yyyy-MM-dd");
                    }
                    else if (formatType == "time")
                    {
                        DateTime timeValue;
                        if (DateTime.TryParse(fieldNode.InnerText, out timeValue))
                            fieldNode.InnerText = timeValue.ToString("HH:mm:ss");
                    }
                }
            }
        }

        private static string AjustarPrefixoTag(string xmlContent)
        {
            return xmlContent.Replace($"<DT_RequestSendConemb>", $"<con:MT_RequestSendConemb_EDI>").Replace($"</DT_RequestSendConemb>", $"</con:MT_RequestSendConemb_EDI>");
        }

        private async Task SetErrorMessageXML(HttpResponseMessage httpResponseMessage)
        {
            try
            {
                _httpRequisicaoResposta.conteudoResposta = await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
                var responseXmlDoc = new XmlDocument();
                responseXmlDoc.LoadXml(_httpRequisicaoResposta.conteudoResposta);
                var faultStringNode = responseXmlDoc.GetElementsByTagName("message").Item(0);

                _httpRequisicaoResposta.mensagem = (faultStringNode != null)
                    ? faultStringNode.InnerText
                    : $"Erro desconhecido. Detalhes: {httpResponseMessage.ReasonPhrase}";
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
