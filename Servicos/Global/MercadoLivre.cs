using Infrastructure.Services.HttpClientFactory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Servicos
{
    public class MercadoLivre
    {
        public static string ObterToken(Dominio.Entidades.Empresa empresa)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            string endPoint = empresa.Configuracao?.URLMercadoLivre; https://api.mercadolibre.com/oauth/token
            string client_id = empresa.Configuracao?.IDMercadoLivre;
            string client_secret = empresa.Configuracao?.SecretKeyMercadoLivre;

            HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(MercadoLivre));

            client.BaseAddress = new Uri(endPoint);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            endPoint = endPoint + "oauth/token?client_id=" + client_id + "&client_secret=" + client_secret + "&grant_type=client_credentials";
            Servicos.Log.TratarErro(endPoint, "MercadoLivre");

            var result = client.PostAsync(endPoint, null).Result;

            string jsonResponse = result.Content.ReadAsStringAsync().Result;

            Servicos.Log.TratarErro(jsonResponse, "MercadoLivre");

            if (!result.IsSuccessStatusCode)
                Servicos.Log.TratarErro("Retorno ObterToken: " + result.StatusCode.ToString(), "MercadoLivre");
            else
            {
                var retorno = (string)(result.Content.ReadAsStringAsync().Result);
                dynamic objetoRetorno = JsonConvert.DeserializeObject<dynamic>(retorno);

                return objetoRetorno?.access_token ?? string.Empty;
            }

            return string.Empty;
        }

        public static List<Dominio.ObjetosDeValor.MercadoLivre.HandlingUnit> ObterShipmentID(Dominio.Entidades.Empresa empresa, string token, List<Dominio.ObjetosDeValor.MercadoLivre.Barras> listaCodigosBarras)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            string endPoint = empresa.Configuracao?.URLMercadoLivre; //https://api.mercadolibre.com/handling_units/{handling_unit}?access_token={accessToken}

            HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(MercadoLivre));

            client.BaseAddress = new Uri(endPoint);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            List<Dominio.ObjetosDeValor.MercadoLivre.HandlingUnit> listaHandlingUnit = new List<Dominio.ObjetosDeValor.MercadoLivre.HandlingUnit>();

            foreach (var codigoBarra in listaCodigosBarras)
            {
                string urlRequest = endPoint + "handling_units/" + codigoBarra.CodigoBarras.Trim() + "?access_token=" + token;
                Servicos.Log.TratarErro(urlRequest, "MercadoLivre");
                var result = client.GetAsync(urlRequest).Result;
                string jsonResponse = result.Content.ReadAsStringAsync().Result;

                if (!result.IsSuccessStatusCode)
                {
                    var retorno = (string)(result.Content.ReadAsStringAsync().Result);
                    dynamic objetoRetorno = JsonConvert.DeserializeObject<dynamic>(retorno);

                    Servicos.Log.TratarErro("Retorno ObterShipmentID: " + result.StatusCode.ToString(), "MercadoLivre");
                    Servicos.Log.TratarErro("Retorno ObterShipmentID: " + jsonResponse, "MercadoLivre");
                    throw new Exception("Falha ao consultar (" + codigoBarra.CodigoBarras + "): " + objetoRetorno?.message ?? result.StatusCode.ToString());
                }
                else
                {
                    var retorno = (string)(result.Content.ReadAsStringAsync().Result);
                    Dominio.ObjetosDeValor.MercadoLivre.HandlingUnit objetoRetorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.MercadoLivre.HandlingUnit>(retorno);

                    listaHandlingUnit.Add(objetoRetorno);
                }
            }

            return listaHandlingUnit;
        }

        public static Dominio.ObjetosDeValor.Embarcador.CTe.CTe ObterCTe(string endPoint, string token, string shipmentID, string hu, Repositorio.UnitOfWork unitOfWork)
        {
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                //string endPoint = empresa.Configuracao?.URLMercadoLivre; //https://api.mercadolibre.com/shipments/shipment_id/cte?access_token=accessToken

                HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(MercadoLivre));

                client.BaseAddress = new Uri(endPoint);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                string urlRequest = endPoint + "shipments/" + shipmentID + "/cte?access_token=" + token + "&doctype=xml";
                Servicos.Log.TratarErro(urlRequest, "MercadoLivre");
                var result = client.GetAsync(urlRequest).Result;
                string jsonResponse = result.Content.ReadAsStringAsync().Result;

                Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte = null;

                if (!result.IsSuccessStatusCode)
                {
                    var retorno = (string)(result.Content.ReadAsStringAsync().Result);
                    dynamic objetoRetorno = JsonConvert.DeserializeObject<dynamic>(retorno);

                    Servicos.Log.TratarErro("Shipment " + shipmentID + " HU " + hu, "MercadoLivre");
                    Servicos.Log.TratarErro("Retorno ObterCTe: " + result.StatusCode.ToString(), "MercadoLivre");
                    Servicos.Log.TratarErro("Retorno ObterCTe: " + jsonResponse, "MercadoLivre");
                    throw new Exception(objetoRetorno?.message ?? result.StatusCode.ToString());
                }
                else
                {
                    Servicos.Embarcador.CTe.CTe serCte = new Servicos.Embarcador.CTe.CTe(unitOfWork);

                    var stringCTe = (string)(result.Content.ReadAsStringAsync().Result);
                    byte[] data = Encoding.ASCII.GetBytes(stringCTe);
                    var stream = new MemoryStream(Encoding.UTF8.GetBytes(System.Text.ASCIIEncoding.ASCII.GetString(data) ?? ""));
                    StreamReader reader = new StreamReader(stream);

                    var objCTe = MultiSoftware.CTe.Servicos.Leitura.Ler(stream);
                    if (objCTe != null)
                    {
                        Type tipoCTe = objCTe.GetType();
                        if (tipoCTe == typeof(MultiSoftware.CTe.v300.ConhecimentoDeTransporteProcessado.cteProc) ||
                            tipoCTe == typeof(MultiSoftware.CTe.v400.ConhecimentoDeTransporteProcessado.cteProc))
                            cte = serCte.ConverterProcCTeParaCTePorObjeto(objCTe);
                        else
                            throw new Exception("XML retornado não é de CT-e. " + "Shipment " + shipmentID + " HU " + hu);
                    }

                    return cte;
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "MercadoLivre");
                return null;
            }
        }

        public static List<Dominio.ObjetosDeValor.MercadoLivre.NotaFiscal> ObterNFeMercadoLivre(Dominio.Entidades.Empresa empresa, string token, string shipmentID, Repositorio.UnitOfWork unitOfWork)
        {
            string urlRequest = string.Empty;
            string jsonResponse = string.Empty;
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            string endPoint = empresa.Configuracao?.URLMercadoLivre; //https://api.mercadolibre.com/shipments/shipment_id/cte?access_token=accessToken

            HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(MercadoLivre));

            client.BaseAddress = new Uri(endPoint);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            urlRequest = endPoint + "shipments/" + shipmentID + "/fiscal-info?access_token=" + token + "&doctype=xml";
            Servicos.Log.TratarErro(urlRequest, "MercadoLivre");
            var result = client.GetAsync(urlRequest).Result;
            jsonResponse = result.Content.ReadAsStringAsync().Result;
            Servicos.Log.TratarErro("Retorno ObterNFe: " + jsonResponse, "MercadoLivre");

            if (!result.IsSuccessStatusCode)
            {
                var retorno = (string)(result.Content.ReadAsStringAsync().Result);
                dynamic objetoRetorno = JsonConvert.DeserializeObject<dynamic>(retorno);

                Servicos.Log.TratarErro("Retorno ObterNFe: " + result.StatusCode.ToString(), "MercadoLivre");
                throw new Exception(objetoRetorno?.message ?? result.StatusCode.ToString());
            }
            else
            {
                var retorno = (string)(result.Content.ReadAsStringAsync().Result);
                dynamic objetoRetorno = JsonConvert.DeserializeObject<dynamic>(retorno);
                if (objetoRetorno != null)
                {
                    if (objetoRetorno.fiscal_data != null && objetoRetorno.fiscal_data.Count > 0)
                    {
                        List<Dominio.ObjetosDeValor.MercadoLivre.NotaFiscal> listaNotas = new List<Dominio.ObjetosDeValor.MercadoLivre.NotaFiscal>();
                        foreach (var nota_fiscal in objetoRetorno.fiscal_data)
                        {
                            if (nota_fiscal.invoice != null)
                            {
                                Dominio.ObjetosDeValor.MercadoLivre.NotaFiscal nota = new Dominio.ObjetosDeValor.MercadoLivre.NotaFiscal();

                                if (!Utilidades.Validate.ValidarChave((string)nota_fiscal.invoice.key))
                                    throw new Exception("Chave NFe retornada é inválida (" + nota_fiscal.invoice.key + ")");

                                nota.CNPJEmissor = Utilidades.Chave.ObterCNPJEmitente((string)nota_fiscal.invoice.key);
                                nota.Chave = (string)nota_fiscal.invoice.key;
                                nota.Numero = (int)nota_fiscal.invoice.number;
                                nota.Serie = (int)nota_fiscal.invoice.serie;
                                nota.Valor = (decimal)nota_fiscal.invoice.amount;

                                //double.TryParse(nota.CNPJEmissor, out double cnpjEmissor);
                                //if (cnpjEmissor == 0)
                                //    throw new Exception("CNPJ do emissor da NFe (" + nota.CNPJEmissor + ") inválido.");
                                //if (repCliente.BuscarPorCPFCNPJ(cnpjEmissor) == null)
                                //    throw new Exception("Emissor da NFe (" + nota.CNPJEmissor + ") não cadastrado, favor cadastrar e importar novamente.");

                                listaNotas.Add(nota);
                            }
                            else
                                throw new Exception("fiscal_data está sem informações da nota (invoice)");
                        }

                        return listaNotas;
                    }
                    else
                        throw new Exception("fiscal-info está sem informação em fiscal_data para obter dados das notas");
                }
                else
                    throw new Exception("fiscal-info não teve retorno de informações");
            }
        }

        public static List<Dominio.ObjetosDeValor.XMLNFe> ObterNotaFiscal(string endPoint, int codigoEmpresa, string token, string shipmentID, string hu, Repositorio.UnitOfWork unitOfWork)
        {
            string urlRequest = string.Empty;
            string jsonResponse = string.Empty;
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            //string endPoint = empresa.Configuracao?.URLMercadoLivre; //https://api.mercadolibre.com/shipments/shipment_id/cte?access_token=accessToken

            HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(MercadoLivre));

            client.BaseAddress = new Uri(endPoint);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            urlRequest = endPoint + "shipments/" + shipmentID + "/fiscal-info?access_token=" + token + "&doctype=xml";
            Servicos.Log.TratarErro(urlRequest, "MercadoLivre");
            var result = client.GetAsync(urlRequest).Result;
            jsonResponse = result.Content.ReadAsStringAsync().Result;
            Servicos.Log.TratarErro("Retorno ObterNFe: " + jsonResponse, "MercadoLivre");

            if (!result.IsSuccessStatusCode)
            {
                var retorno = (string)(result.Content.ReadAsStringAsync().Result);
                dynamic objetoRetorno = JsonConvert.DeserializeObject<dynamic>(retorno);

                Servicos.Log.TratarErro("Shipment " + shipmentID + "  HU " + hu);
                Servicos.Log.TratarErro("Retorno ObterNFe: " + result.StatusCode.ToString(), "MercadoLivre");
                throw new Exception(objetoRetorno?.message ?? result.StatusCode.ToString());
            }
            else
            {
                try
                {
                    var retorno = (string)(result.Content.ReadAsStringAsync().Result);
                    dynamic objetoRetorno = JsonConvert.DeserializeObject<dynamic>(retorno);
                    if (objetoRetorno != null)
                    {
                        if (objetoRetorno.fiscal_data != null && objetoRetorno.fiscal_data.Count > 0)
                        {
                            List<Dominio.ObjetosDeValor.XMLNFe> listaNotas = new List<Dominio.ObjetosDeValor.XMLNFe>();
                            foreach (var nota_fiscal in objetoRetorno.fiscal_data)
                            {
                                if (nota_fiscal.invoice != null)
                                {
                                    if (nota_fiscal.tax.type != "No_Tax")
                                    {
                                        if (nota_fiscal.tax.type == "ICMS")
                                            throw new Exception("NFe " + nota_fiscal.invoice.key + " com type ICMS e sem CTe. (Shipment " + shipmentID + "  HU " + hu + ")");
                                        else
                                            throw new WebException("NFe " + nota_fiscal.invoice.key + " com type " + nota_fiscal.tax.type + ", ignorada. (Shipment " + shipmentID + "  HU " + hu + ")");
                                    }

                                    if (!Utilidades.Validate.ValidarChave((string)nota_fiscal.invoice.key))
                                        throw new Exception("Chave NFe retornada é inválida (" + nota_fiscal.invoice.key + "). (Shipment " + shipmentID + "  HU " + hu + ")");

                                    string urlNFe = (string)nota_fiscal.invoice.document.href;
                                    urlNFe = urlNFe + "&access_token=" + token;
                                    Servicos.Log.TratarErro(urlNFe, "MercadoLivre");
                                    var resultXMLNFe = client.GetAsync(urlNFe).Result;
                                    string jsonResponseXMLNFe = resultXMLNFe.Content.ReadAsStringAsync().Result;

                                    if (!resultXMLNFe.IsSuccessStatusCode)
                                    {
                                        var retornoXMLNFe = (string)(resultXMLNFe.Content.ReadAsStringAsync().Result);
                                        dynamic objetoRetornoXMLNFe = JsonConvert.DeserializeObject<dynamic>(retornoXMLNFe);

                                        Servicos.Log.TratarErro("Shipment " + shipmentID + "  HU " + hu);
                                        Servicos.Log.TratarErro("Retorno ObterNFe: " + resultXMLNFe.StatusCode.ToString(), "MercadoLivre");
                                        Servicos.Log.TratarErro("Retorno ObterNFe: " + jsonResponseXMLNFe, "MercadoLivre");
                                        throw new Exception(objetoRetornoXMLNFe?.message ?? resultXMLNFe.StatusCode.ToString());
                                    }
                                    else
                                    {
                                        var stringNFe = (string)(resultXMLNFe.Content.ReadAsStringAsync().Result);
                                        byte[] data = Encoding.ASCII.GetBytes(stringNFe);
                                        var stream = new MemoryStream(Encoding.UTF8.GetBytes(System.Text.ASCIIEncoding.ASCII.GetString(data) ?? ""));
                                        //StreamReader reader = new StreamReader(stream);
                                        //var objNFe = MultiSoftware.NFe.Servicos.Leitura.Ler(stream);

                                        Servicos.NFe svcNFe = new Servicos.NFe(unitOfWork);
                                        object objNFe = svcNFe.ObterDocumentoPorXML(stream, codigoEmpresa, null, unitOfWork);

                                        if (objNFe != null)
                                        {
                                            string stringXmlNFe = Newtonsoft.Json.JsonConvert.SerializeObject(objNFe);
                                            Dominio.ObjetosDeValor.XMLNFe xmlNFe = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.XMLNFe>(stringXmlNFe);

                                            listaNotas.Add(xmlNFe);
                                        }
                                    }
                                }
                                else
                                    throw new WebException("fiscal_data está sem informações da nota (invoice). (Shipment " + shipmentID + "  HU " + hu + ")");
                            }

                            return listaNotas;
                        }
                        else
                            throw new WebException("fiscal-info está sem informação em fiscal_data para obter dados das notas. (Shipment " + shipmentID + "  HU " + hu + ")");
                    }
                    else
                        throw new WebException("fiscal-info não teve retorno de informações. (Shipment " + shipmentID + "  HU " + hu + ")");
                }
                catch (WebException ex)
                {
                    Servicos.Log.TratarErro(ex, "MercadoLivre");
                    return null;
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex, "MercadoLivre");
                    throw;
                }
            }
        }


        public static void ObterDocumentos(ref List<Dominio.ObjetosDeValor.Embarcador.CTe.CTe> listaCtes, ref List<Dominio.ObjetosDeValor.XMLNFe> listaNotas, string endPoint, int codigoEmpresa, string token, string shipmentID, string hu, Repositorio.UnitOfWork unitOfWork)
        {
            string urlRequest = string.Empty;
            string jsonResponse = string.Empty;
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(MercadoLivre));

            client.BaseAddress = new Uri(endPoint);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            urlRequest = endPoint + "shipments/" + shipmentID + "/fiscal-info?access_token=" + token + "&doctype=xml";
            Servicos.Log.TratarErro(urlRequest, "MercadoLivre");
            var result = client.GetAsync(urlRequest).Result;
            jsonResponse = result.Content.ReadAsStringAsync().Result;
            Servicos.Log.TratarErro("Retorno ObterNFe: " + jsonResponse, "MercadoLivre");

            if (!result.IsSuccessStatusCode)
            {
                var retorno = (string)(result.Content.ReadAsStringAsync().Result);
                dynamic objetoRetorno = JsonConvert.DeserializeObject<dynamic>(retorno);

                Servicos.Log.TratarErro("Shipment " + shipmentID + "  HU " + hu);
                Servicos.Log.TratarErro("Retorno ObterNFe: " + result.StatusCode.ToString(), "MercadoLivre");
                throw new Exception(objetoRetorno?.message ?? result.StatusCode.ToString());
            }
            else
            {
                try
                {
                    var retorno = (string)(result.Content.ReadAsStringAsync().Result);
                    dynamic objetoRetorno = JsonConvert.DeserializeObject<dynamic>(retorno);
                    if (objetoRetorno != null)
                    {
                        if (objetoRetorno.fiscal_data != null && objetoRetorno.fiscal_data.Count > 0)
                        {
                            foreach (var nota_fiscal in objetoRetorno.fiscal_data)
                            {
                                try
                                {
                                    if (nota_fiscal.invoice != null)
                                    {
                                        if (nota_fiscal.tax.type == "No_Tax")
                                        {
                                            if (!Utilidades.Validate.ValidarChave((string)nota_fiscal.invoice.key))
                                                throw new Exception("Chave NFe retornada é inválida (" + nota_fiscal.invoice.key + "). (Shipment " + shipmentID + "  HU " + hu + ")");

                                            string urlNFe = (string)nota_fiscal.invoice.document.href;
                                            urlNFe = urlNFe + "&access_token=" + token;
                                            Servicos.Log.TratarErro(urlNFe, "MercadoLivre");
                                            var resultXMLNFe = client.GetAsync(urlNFe).Result;
                                            string jsonResponseXMLNFe = resultXMLNFe.Content.ReadAsStringAsync().Result;

                                            if (!resultXMLNFe.IsSuccessStatusCode)
                                            {
                                                var retornoXMLNFe = (string)(resultXMLNFe.Content.ReadAsStringAsync().Result);
                                                dynamic objetoRetornoXMLNFe = JsonConvert.DeserializeObject<dynamic>(retornoXMLNFe);

                                                Servicos.Log.TratarErro("Shipment " + shipmentID + "  HU " + hu);
                                                Servicos.Log.TratarErro("Retorno ObterNFe: " + resultXMLNFe.StatusCode.ToString(), "MercadoLivre");
                                                Servicos.Log.TratarErro("Retorno ObterNFe: " + jsonResponseXMLNFe, "MercadoLivre");
                                                throw new Exception(objetoRetornoXMLNFe?.message ?? resultXMLNFe.StatusCode.ToString());
                                            }
                                            else
                                            {
                                                var stringNFe = (string)(resultXMLNFe.Content.ReadAsStringAsync().Result);
                                                byte[] data = Encoding.ASCII.GetBytes(stringNFe);
                                                var stream = new MemoryStream(Encoding.UTF8.GetBytes(System.Text.ASCIIEncoding.ASCII.GetString(data) ?? ""));

                                                Servicos.NFe svcNFe = new Servicos.NFe(unitOfWork);
                                                object objNFe = svcNFe.ObterDocumentoPorXML(stream, codigoEmpresa, null, unitOfWork);

                                                if (objNFe != null)
                                                {
                                                    string stringXmlNFe = Newtonsoft.Json.JsonConvert.SerializeObject(objNFe);
                                                    Dominio.ObjetosDeValor.XMLNFe xmlNFe = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.XMLNFe>(stringXmlNFe);

                                                    listaNotas.Add(xmlNFe);
                                                }
                                            }
                                        }
                                        else if (nota_fiscal.tax.type == "ICMS")
                                        {
                                            string urlCTe = (string)nota_fiscal.tax.document?.href;

                                            if (string.IsNullOrWhiteSpace(urlCTe))
                                                throw new Exception("NFe " + nota_fiscal.invoice.key + " com type ICMS e sem CTe. (Shipment " + shipmentID + "  HU " + hu + ")");


                                            urlCTe = urlCTe + "&access_token=" + token;
                                            Servicos.Log.TratarErro(urlCTe, "MercadoLivre");
                                            var resultXMLCTe = client.GetAsync(urlCTe).Result;
                                            string jsonResponseXMLCTe = resultXMLCTe.Content.ReadAsStringAsync().Result;

                                            if (!resultXMLCTe.IsSuccessStatusCode)
                                            {
                                                var retornoXMLCTe = (string)(resultXMLCTe.Content.ReadAsStringAsync().Result);
                                                dynamic objetoRetornoXMLCTe = JsonConvert.DeserializeObject<dynamic>(retornoXMLCTe);

                                                Servicos.Log.TratarErro("Shipment " + shipmentID + "  HU " + hu);
                                                Servicos.Log.TratarErro("Retorno ObterCTe: " + resultXMLCTe.StatusCode.ToString(), "MercadoLivre");
                                                Servicos.Log.TratarErro("Retorno ObterCTe: " + jsonResponseXMLCTe, "MercadoLivre");
                                                throw new Exception(objetoRetornoXMLCTe?.message ?? resultXMLCTe.StatusCode.ToString());
                                            }
                                            else
                                            {
                                                Servicos.Embarcador.CTe.CTe serCte = new Servicos.Embarcador.CTe.CTe(unitOfWork);

                                                var stringCTe = (string)(resultXMLCTe.Content.ReadAsStringAsync().Result);
                                                byte[] data = Encoding.ASCII.GetBytes(stringCTe);
                                                var stream = new MemoryStream(Encoding.UTF8.GetBytes(System.Text.ASCIIEncoding.ASCII.GetString(data) ?? ""));

                                                StreamReader reader = new StreamReader(stream);

                                                var objCTe = MultiSoftware.CTe.Servicos.Leitura.Ler(stream);
                                                if (objCTe != null)
                                                {
                                                    Type tipoCTe = objCTe.GetType();
                                                    if (tipoCTe == typeof(MultiSoftware.CTe.v300.ConhecimentoDeTransporteProcessado.cteProc) ||
                                                        tipoCTe == typeof(MultiSoftware.CTe.v400.ConhecimentoDeTransporteProcessado.cteProc))
                                                        listaCtes.Add(serCte.ConverterProcCTeParaCTePorObjeto(objCTe));
                                                    else
                                                        throw new Exception("XML retornado não é de CT-e. " + "Shipment " + shipmentID + " HU " + hu);
                                                }
                                            }
                                        }
                                        else
                                            throw new WebException("NFe " + nota_fiscal.invoice.key + " com type " + nota_fiscal.tax.type + ", ignorada. (Shipment " + shipmentID + "  HU " + hu + ")");

                                    }
                                    else
                                        throw new Exception("fiscal_data está sem informações da nota (invoice). (Shipment " + shipmentID + "  HU " + hu + ")");

                                    unitOfWork.FlushAndClear();
                                }
                                catch (WebException ex)
                                {
                                    Servicos.Log.TratarErro(ex, "MercadoLivre");
                                }
                                catch (Exception ex)
                                {
                                    Servicos.Log.TratarErro(ex, "MercadoLivre");
                                    throw;
                                }
                            }

                        }
                        else
                            throw new Exception("fiscal-info está sem informação em fiscal_data para obter dados das notas. (Shipment " + shipmentID + "  HU " + hu + ")");
                    }
                    else
                        throw new Exception("fiscal-info não teve retorno de informações. (Shipment " + shipmentID + "  HU " + hu + ")");
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex, "MercadoLivre");
                    throw;
                }
            }
        }


        public static string SalvarCTePorHandlingUnit(Dominio.Entidades.Empresa empresa, Dominio.Entidades.Cliente tomador1, Dominio.Entidades.Cliente tomador2, Dominio.Entidades.Cliente expedidor, Dominio.Entidades.Cliente recebedor, Dominio.Entidades.Veiculo tracao, Dominio.Entidades.Veiculo reboque, Dominio.Entidades.Usuario motorista, decimal valorFrete, decimal valorPedagio, decimal valorOutros, decimal percentualGris, string observacaoCTe, Dominio.Entidades.Usuario usuarioAdministrativo, Dominio.Entidades.Usuario usuario, List<Dominio.ObjetosDeValor.MercadoLivre.HandlingUnit> listaHandlingUnit, string token, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.CTe svcCTe = new CTe(unitOfWork);
            Servicos.NFSe svcNFSe = new NFSe(unitOfWork);

            List<Dominio.ObjetosDeValor.Embarcador.CTe.CTe> listaCTes = new List<Dominio.ObjetosDeValor.Embarcador.CTe.CTe>();
            //List<Dominio.ObjetosDeValor.MercadoLivre.NotaFiscal> listaNFes = new List<Dominio.ObjetosDeValor.MercadoLivre.NotaFiscal>();
            List<Dominio.ObjetosDeValor.XMLNFe> listaNFes = new List<Dominio.ObjetosDeValor.XMLNFe>();

            string urlMercadoLivre = empresa.Configuracao?.URLMercadoLivre;

            if (string.IsNullOrWhiteSpace(urlMercadoLivre))
                throw new Exception("URL do Mercado livre não está configurada.");

            foreach (var handlingUnit in listaHandlingUnit)
            {
                foreach (var detail in handlingUnit.details)
                {
                    if (detail.status == "FORWARD")
                    {
                        //var cteMercadoLivre = ObterCTe(urlMercadoLivre, token, detail.shipment_id, handlingUnit.id);
                        //if (cteMercadoLivre != null)
                        //    listaCTes.Add(cteMercadoLivre);
                        //else
                        //{
                        //    //Rotina quando gerava NFSe
                        //    //var notasMercadoLivre = ObterNFeMercadoLivre(empresa, token, detail.shipment_id, unitOfWork);
                        //    //if (notasMercadoLivre != null && notasMercadoLivre.Count > 0)
                        //    //    listaNFes.AddRange(notasMercadoLivre);
                        //    //else
                        //    //    throw new Exception("Shipment " + detail.shipment_id + " não retornou um CT-e e NF-e");

                        //    List<Dominio.ObjetosDeValor.XMLNFe> notas = ObterNotaFiscal(urlMercadoLivre, empresa.Codigo, token, detail.shipment_id, handlingUnit.id, unitOfWork);
                        //    if (notas != null && notas.Count > 0)
                        //        listaNFes.AddRange(notas);
                        //    else
                        //        Servicos.Log.TratarErro("Shipment " + detail.shipment_id + " não retornou um CT-e e NF-e. (HU (" + handlingUnit.id + ")", "MercadoLivre");
                        //    //throw new Exception("Shipment " + detail.shipment_id + " não retornou um CT-e e NF-e");
                        //}

                        ObterDocumentos(ref listaCTes, ref listaNFes, urlMercadoLivre, empresa.Codigo, token, detail.shipment_id, handlingUnit.id, unitOfWork);

                        unitOfWork.FlushAndClear();
                    }
                    else
                        Servicos.Log.TratarErro("Shipment " + detail.shipment_id + " status " + detail.status + ". (HU " + handlingUnit.id + ")", "MercadoLivre");
                }
            }

            if (listaNFes.Count == 0 && listaCTes.Count == 0)
                throw new Exception("HUs não retornaram nem CT-e e NF-e");

            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Usuario repMotorista = new Repositorio.Usuario(unitOfWork);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);

            //Validação quando gerava NFSe
            //if (listaNFes != null && listaNFes.Count > 0 && string.IsNullOrWhiteSpace(empresa.Configuracao?.SerieRPSNFSe ?? string.Empty))
            //    throw new Exception("Consulta retornou notas municipais e transportadora não configurada para emissão de NFSe");

            decimal valorFretePorDocumento = 0;
            decimal valorPedagioPorDocumento = 0;
            decimal valorOutrosPorDocumento = 0;

            int quantidadeDocumentos = listaCTes.Count() + listaNFes.Count();
            valorFretePorDocumento = valorFrete > 0 ? Math.Round((valorFrete / quantidadeDocumentos), 2, MidpointRounding.ToEven) : 0;
            valorPedagioPorDocumento = valorPedagio > 0 ? Math.Round((valorPedagio / quantidadeDocumentos), 2, MidpointRounding.ToEven) : 0;
            valorOutrosPorDocumento = valorOutros > 0 ? Math.Round((valorOutros / quantidadeDocumentos), 2, MidpointRounding.ToEven) : 0;

            decimal valorFreteCTe = valorFretePorDocumento * listaCTes.Count();
            decimal valorPedagioCte = valorPedagioPorDocumento * listaCTes.Count();
            decimal valorOutrosCte = valorOutrosPorDocumento * listaCTes.Count();

            decimal valorFreteNFe = valorFretePorDocumento * listaNFes.Count();
            decimal valorPedagioNFe = valorPedagioPorDocumento * listaNFes.Count();
            decimal valorOutrosNFe = valorOutrosPorDocumento * listaNFes.Count();

            if ((valorFreteCTe + valorFreteNFe) != valorFrete)
                valorFreteCTe += valorFrete - (valorFreteCTe + valorFreteNFe);

            if ((valorPedagioCte + valorPedagioNFe) != valorPedagio)
                valorPedagioCte += valorPedagio - (valorPedagioCte + valorPedagioNFe);

            if ((valorOutrosCte + valorOutrosNFe) != valorOutros)
                valorOutrosCte += valorOutros - (valorOutrosCte + valorOutrosNFe);


            List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> listaCTesPorNota = null;
            Dominio.Entidades.ConhecimentoDeTransporteEletronico cte1 = null;
            Dominio.Entidades.ConhecimentoDeTransporteEletronico cte2 = null;
            Dominio.Entidades.NFSe nfse = null;

            try
            {
                empresa = repEmpresa.BuscarPorCodigo(empresa.Codigo);
                if (tomador1 != null)
                    tomador1 = repCliente.BuscarPorCPFCNPJ(tomador1.CPF_CNPJ);
                if (tomador2 != null)
                    tomador2 = repCliente.BuscarPorCPFCNPJ(tomador2.CPF_CNPJ);
                if (expedidor != null)
                    expedidor = repCliente.BuscarPorCPFCNPJ(expedidor.CPF_CNPJ);
                if (recebedor != null)
                    recebedor = repCliente.BuscarPorCPFCNPJ(recebedor.CPF_CNPJ);
                if (tracao != null)
                    tracao = repVeiculo.BuscarPorCodigo(tracao.Codigo);
                if (reboque != null)
                    reboque = repVeiculo.BuscarPorCodigo(reboque.Codigo);
                if (motorista != null)
                    motorista = repMotorista.BuscarPorCodigo(motorista.Codigo);
                if (usuario != null)
                    usuario = repMotorista.BuscarPorCodigo(usuario.Codigo);
                if (usuarioAdministrativo != null)
                    usuarioAdministrativo = repMotorista.BuscarPorCodigo(usuarioAdministrativo.Codigo);

                List<Dominio.ObjetosDeValor.Embarcador.CTe.CTe> documentosTomador1 = tomador1 != null ? listaCTes.Where(o => o.Emitente.CNPJ.Substring(0, 8) == tomador1.RaizCnpjSemFormato).ToList() : new List<Dominio.ObjetosDeValor.Embarcador.CTe.CTe>();
                List<Dominio.ObjetosDeValor.Embarcador.CTe.CTe> documentosTomador2 = tomador2 != null ? listaCTes.Where(o => o.Emitente.CNPJ.Substring(0, 8) == tomador2.RaizCnpjSemFormato).ToList() : new List<Dominio.ObjetosDeValor.Embarcador.CTe.CTe>();

                if (documentosTomador1.Count > 0 && documentosTomador2.Count > 0)
                    valorFreteCTe = (valorFreteCTe / 2);

                unitOfWork.Start();

                if (documentosTomador1.Count > 0)
                    cte1 = svcCTe.GerarCTePorListaCTe(empresa, tomador1, expedidor, recebedor, tracao, reboque, motorista, valorFreteCTe, valorPedagioCte, valorOutrosCte, percentualGris, observacaoCTe, usuarioAdministrativo, usuario, documentosTomador1, Dominio.Enumeradores.TipoServico.RedIntermediario, unitOfWork);

                if (documentosTomador2.Count > 0)
                    cte2 = svcCTe.GerarCTePorListaCTe(empresa, tomador2, expedidor, recebedor, tracao, reboque, motorista, valorFreteCTe, valorPedagioCte, valorOutrosCte, percentualGris, observacaoCTe, usuarioAdministrativo, usuario, documentosTomador2, Dominio.Enumeradores.TipoServico.RedIntermediario, unitOfWork);

                //Rotina quando gerava NFSe
                //if (listaNFes.Count > 0)
                //    nfse = svcNFSe.GerarNFSePorNFeMercadoLivre(listaNFes, empresa, tomador1, expedidor, tracao?.Codigo ?? 0, empresa.Configuracao?.ObservacaoPadraoNFSe ?? string.Empty, valorFreteNFSe + valorPedagioNFSe + valorOutrosNFSe, Dominio.Enumeradores.StatusNFSe.EmDigitacao, unitOfWork, unitOfWork.StringConexao);

                if (listaNFes.Count > 0)
                {
                    bool notaSalva = false;
                    listaCTesPorNota = svcCTe.SalvarCTePorObjetoNFe(empresa, null, null, false, false, false, false, listaNFes, unitOfWork, 0, tracao?.Codigo ?? 0, reboque?.Codigo ?? 0, motorista?.Codigo ?? 0, true, valorFreteNFe, valorPedagioNFe, percentualGris, Dominio.Enumeradores.TipoRateioTabelaFreteValor.PorCTe, Dominio.Enumeradores.TipoTomador.Remetente, 0, observacaoCTe, string.Empty, string.Empty, valorOutrosNFe, ref notaSalva);
                }

                unitOfWork.CommitChanges();

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "MercadoLivre");

                if (unitOfWork != null)
                    unitOfWork.Rollback();

                throw new Exception("Documentos não gerados, tente novamente.");
            }

            if (cte1 == null && cte2 == null && nfse == null && (listaCTesPorNota == null || listaCTesPorNota.Count == 0))
                throw new Exception("Consulta Mercado Livre não gerou CTe/NFe");
            else
            {
                string mensagemRetorno = "Documentos salvos com sucesso. ";

                //if(cte1 != null && cte2 != null)
                //    mensagemRetorno = "CT-e(s) " + cte1.Numero + " e " + cte2.Numero + " salvoa com sucesso. ";
                //else if (cte1 != null)
                //    mensagemRetorno = "CT-e " + cte1.Numero + " salvo com sucesso. ";
                //else if (cte2 != null)
                //    mensagemRetorno = "CT-e " + cte2.Numero + " salvo com sucesso. ";
                //if (nfse != null)
                //    mensagemRetorno = mensagemRetorno + "NFS-e " + nfse.Numero + " salvo com sucesso. ";

                return mensagemRetorno;
            }
        }

    }
}
