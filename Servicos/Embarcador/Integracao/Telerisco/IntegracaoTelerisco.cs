using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Dominio.ObjetosDeValor.Embarcador.Integracao.Telerisco;
using Newtonsoft.Json;

namespace Servicos.Embarcador.Integracao.Telerisco
{
    public class IntegracaoTelerisco
    {
        #region Propriedades Privadas

        private HttpWebRequest ClientRequest { get; set; }
        private string ClientRequestContent { get; set; }
        private string ClientResponseContent { get; set; }

        #endregion

        #region Construtores

        public static IntegracaoTelerisco GetInstance()
        {
            return new IntegracaoTelerisco();
        }

        #endregion

        #region Métodos Públicos

        public static ConsultaMotoristaResponse ConsultaMotorista(Dominio.Entidades.Usuario motorista, Dominio.Entidades.Embarcador.Filiais.Filial filial, DateTime dataHoraEmbarque, ref string mensagemErro, ref string jsonRequest, ref string jsonResponse, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, string placa)
        {
            try
            {
                Repositorio.Embarcador.Configuracoes.Integracao repIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracoes.Integracao integracao = repIntegracao.Buscar();

                if (integracao == null)
                {
                    mensagemErro = "Sem integração configurada.";
                    return null;
                }
                if (string.IsNullOrWhiteSpace(integracao.URLIntegracaoTelerisco))
                {
                    mensagemErro = "Integração sem URL configurada para integração com a Telerisco.";
                    return null;
                }

                if (motorista == null)
                {
                    mensagemErro = "Obrigatório informar um motorista na consulta";
                    return null;
                }

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                string urlAPI = integracao.URLIntegracaoTelerisco;
                if (urlAPI == "https://www.gps-pamcary.com.br" && !integracao.IntegracaoViaPOSTTelerisco) //Telerisco mudou o padão, validação para o antigo
                    urlAPI += "/telerisco/webservices/v2/consulta/";

                string senhaCertificado = string.Empty;
                string caminhoCertificado = string.Empty;

                Dominio.Entidades.Empresa empresaPadrao = integracao.EmpresaFixaTelerisco != null ? integracao.EmpresaFixaTelerisco : repEmpresa.BuscarEmpresaPadraoEmissao();

                if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                {
                    if (empresaPadrao != null)
                    {
                        senhaCertificado = empresaPadrao.SenhaCertificado;
                        caminhoCertificado = empresaPadrao.NomeCertificado;

                        if (string.IsNullOrWhiteSpace(caminhoCertificado))
                        {
                            mensagemErro = "Certificado da empresa " + empresaPadrao.Descricao + " não configurado.";
                            return null;
                        }

                        if (!Utilidades.IO.FileStorageService.Storage.Exists(caminhoCertificado))
                        {
                            mensagemErro = "Certificado da empresa " + empresaPadrao.Descricao + "não localizado.";
                            return null;
                        }
                    }
                }
                else if (filial != null)
                {
                    senhaCertificado = filial.SenhaCertificado;
                    caminhoCertificado = filial.NomeCertificado;

                    if (string.IsNullOrWhiteSpace(caminhoCertificado))
                    {
                        mensagemErro = "Certificado da filial " + filial.Descricao + " não configurado";
                        return null;
                    }

                    if (!Utilidades.IO.FileStorageService.Storage.Exists(caminhoCertificado))
                    {
                        mensagemErro = "Certificado da filial " + filial.Descricao + "não localizado";
                        return null;
                    }
                }
                else
                {
                    Repositorio.Embarcador.Cargas.CargaMotorista repCargaMotorista = new Repositorio.Embarcador.Cargas.CargaMotorista(unitOfWork);
                    Dominio.Entidades.Embarcador.Cargas.Carga cargaMotorista = repCargaMotorista.BuscarUltimaCargaMotorista(motorista.CPF);

                    if (cargaMotorista != null && cargaMotorista.Filial != null)
                    {
                        filial = cargaMotorista.Filial;
                        senhaCertificado = cargaMotorista.Filial.SenhaCertificado;
                        caminhoCertificado = cargaMotorista.Filial.NomeCertificado;
                    }
                    else
                    {
                        senhaCertificado = motorista.Empresa?.SenhaCertificado;
                        caminhoCertificado = motorista.Empresa?.NomeCertificado;
                    }
                }

                if (!string.IsNullOrWhiteSpace(integracao.CaminhoCertificadoTelerisco) && !string.IsNullOrWhiteSpace(integracao.SenhaCertificadoTelerisco))
                {
                    senhaCertificado = integracao.SenhaCertificadoTelerisco;
                    caminhoCertificado = integracao.CaminhoCertificadoTelerisco;
                }

                if (string.IsNullOrWhiteSpace(caminhoCertificado))
                {
                    mensagemErro = "Certificado não configurado";
                    return null;
                }

                if (!Utilidades.IO.FileStorageService.Storage.Exists(caminhoCertificado))
                {
                    mensagemErro = "Certificado não localizado";
                    return null;
                }

                if (!integracao.IntegracaoViaPOSTTelerisco)
                {
                    if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                        urlAPI = urlAPI + "?cpfMotorista=" + motorista.CPF + "&cnpjTransportadora=" + empresaPadrao.CNPJ_SemFormato;
                    else
                        urlAPI = urlAPI + "?cpfMotorista=" + motorista.CPF + "&cnpjTransportadora=" + motorista.Empresa.CNPJ_SemFormato;
                    
                    string cnpjEmbarcador = (integracao?.EnviarEmpresaFixaComoCNPJEmbarcadorNaIntegracaoTelerisco ?? false) ? (integracao?.EmpresaFixaTelerisco?.CNPJ_SemFormato ?? (filial?.CNPJ ?? "")) : (filial?.CNPJ ?? "");

                    if (!string.IsNullOrWhiteSpace(cnpjEmbarcador))
                        urlAPI = urlAPI + "&identificador=2&cnpjEmbarcador=" + cnpjEmbarcador;
                    else
                        urlAPI = urlAPI + "&identificador=1";

                    if (!integracao.NaoEnviarDataEmbarqueGrMotoristaTelerisco && dataHoraEmbarque > DateTime.MinValue)
                        urlAPI = urlAPI + "&datahoraEmbarque=" + dataHoraEmbarque.ToString("yyyy-MM-dd HH:mm:ss");

                    jsonRequest = urlAPI;

                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                    HttpWebRequest client = (HttpWebRequest)WebRequest.Create(urlAPI);

                    client.Method = System.Net.WebRequestMethods.Http.Get;
                    client.ProtocolVersion = HttpVersion.Version10;
                    client.ContentType = "application/json";
                    client.KeepAlive = false;

                    var certificado = new System.Security.Cryptography.X509Certificates.X509Certificate2(caminhoCertificado, senhaCertificado, System.Security.Cryptography.X509Certificates.X509KeyStorageFlags.MachineKeySet);

                    client.ClientCertificates.Add(certificado);
                    client.PreAuthenticate = true;

                    HttpWebResponse objResponse = (HttpWebResponse)client.GetResponse();
                    using (StreamReader sr = new StreamReader(objResponse.GetResponseStream()))
                    {
                        jsonResponse = sr.ReadToEnd();
                        sr.Close();
                    }

                    Dominio.ObjetosDeValor.Embarcador.Integracao.Telerisco.ConsultaMotoristaResponse retornoConsulta = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Telerisco.ConsultaMotoristaResponse>(jsonResponse);

                    return retornoConsulta;
                }
                else
                {
                    Dominio.Entidades.Empresa empresaMatriz = repEmpresa.BuscarEmpresaMatriz(empresaPadrao);

                    Dominio.ObjetosDeValor.Embarcador.Integracao.Telerisco.ConsultaMotorista consultaMotorista = new Dominio.ObjetosDeValor.Embarcador.Integracao.Telerisco.ConsultaMotorista()
                    {
                        cnpjCliente = empresaPadrao.CNPJ_SemFormato,
                        cpfUsuario = motorista.CPF,
                        cnpjEmbarcador = (integracao?.EnviarEmpresaFixaComoCNPJEmbarcadorNaIntegracaoTelerisco ?? false) ? (integracao?.EmpresaFixaTelerisco?.CNPJ_SemFormato ?? empresaPadrao.CNPJ_SemFormato) : empresaPadrao.CNPJ_SemFormato,
                        cnpjTransportadora = empresaMatriz != null ? empresaMatriz.CNPJ_SemFormato : empresaPadrao.CNPJ_SemFormato,
                        motorista = new Dominio.ObjetosDeValor.Embarcador.Integracao.Telerisco.Motorista()
                        {
                            cpf = motorista.CPF,
                            senhaMot = motorista.SenhaGR
                        },
                        veiculos = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Telerisco.Veiculo>()
                    };
                    consultaMotorista.veiculos.Add(new Dominio.ObjetosDeValor.Embarcador.Integracao.Telerisco.Veiculo()
                    {
                        tipo = "T",
                        placa = placa,
                        proprietario = new Dominio.ObjetosDeValor.Embarcador.Integracao.Telerisco.Proprietario()
                        {
                            tipoPessoa = motorista.ClienteTerceiro == null ? "F" : motorista.ClienteTerceiro?.Tipo ?? "F",
                            cnpj = (motorista.ClienteTerceiro?.Tipo == "J") ? motorista.ClienteTerceiro?.CPF_CNPJ_SemFormato : "",
                            cpf = motorista.ClienteTerceiro == null ? motorista.CPF : (motorista.ClienteTerceiro?.Tipo == "F") ? motorista.ClienteTerceiro?.CPF_CNPJ_SemFormato : "",
                            codigoRntrc = ""
                        },
                    }
                    );

                    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                    //TESTE 2 COM HttpClient
                    //var clientCertificate = new X509Certificate2(caminhoCertificado, senhaCertificado);
                    //var httpClientHandler = new HttpClientHandler();
                    //httpClientHandler.ClientCertificates.Add(clientCertificate);
                    //httpClientHandler.ClientCertificateOptions = ClientCertificateOption.Manual;
                    ////httpClientHandler.ServerCertificateCustomValidationCallback = ByPassCertErrorsForTestPurposesDoNotDoThisInTheWild;
                    //httpClientHandler.CheckCertificateRevocationList = false;

                    //var httpClient = new HttpClient(httpClientHandler);
                    //httpClient.BaseAddress = new Uri(urlAPI);

                    //jsonRequest = JsonConvert.SerializeObject(consultaMotorista, Formatting.Indented);
                    //var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
                    //HttpRequestMessage httpRequestMessage = new HttpRequestMessage()
                    //{
                    //    Content = content,
                    //    Method = HttpMethod.Post,
                    //    RequestUri = new Uri(urlAPI)
                    //};

                    //httpClient.DefaultRequestHeaders.Accept.Clear();
                    //httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));                    

                    //var response = httpClient.SendAsync(httpRequestMessage, HttpCompletionOption.ResponseHeadersRead).Result;

                    //var stream = response.Content.ReadAsStringAsync().Result;
                    //Dominio.ObjetosDeValor.Embarcador.Integracao.Telerisco.ConsultaMotoristaResponse retornoConsulta = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Telerisco.ConsultaMotoristaResponse>(stream);

                    //return retornoConsulta;

                    //TESTE 1 COM HttpClient
                    HttpClientHandler handler = new HttpClientHandler();
                    var certificado = new System.Security.Cryptography.X509Certificates.X509Certificate2(caminhoCertificado, senhaCertificado, X509KeyStorageFlags.UserKeySet | X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.Exportable);

                    handler.ClientCertificateOptions = ClientCertificateOption.Manual;
                    handler.ClientCertificates.Add(certificado);
                    HttpClient requisicao = new HttpClient(handler);

                    requisicao.BaseAddress = new Uri(urlAPI);
                    requisicao.DefaultRequestHeaders.Accept.Clear();
                    requisicao.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    jsonRequest = JsonConvert.SerializeObject(consultaMotorista, Formatting.Indented);
                    Servicos.Log.TratarErro(jsonRequest, "Telerisco");

                    StringContent conteudoRequisicao = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
                    HttpResponseMessage retornoRequisicao = requisicao.PostAsync(urlAPI, conteudoRequisicao).Result;
                    string jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;
                    Servicos.Log.TratarErro(jsonRetorno, "Telerisco");

                    dynamic retorno = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(jsonRetorno);

                    if ((retornoRequisicao.StatusCode == HttpStatusCode.OK) || (retornoRequisicao.StatusCode == HttpStatusCode.Created))
                    {
                        Dominio.ObjetosDeValor.Embarcador.Integracao.Telerisco.ConsultaMotoristaResponse retornoConsulta = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Telerisco.ConsultaMotoristaResponse>(jsonRetorno);

                        return retornoConsulta;
                    }
                    else
                    {
                        mensagemErro = "SERVIÇO DA TELERISCO INDISPONÍVEL";
                        return null;
                    }

                    //TESTE COM HttpWebRequest
                    //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    //ServicePointManager.CheckCertificateRevocationList = false;
                    //ServicePointManager.ServerCertificateValidationCallback = (a, b, c, d) => true;
                    //ServicePointManager.Expect100Continue = true;

                    //HttpWebRequest client = (HttpWebRequest)WebRequest.Create(urlAPI);
                    //var cookieContainer = new CookieContainer();

                    //client.Method = System.Net.WebRequestMethods.Http.Post;
                    //client.ProtocolVersion = HttpVersion.Version11;
                    //client.KeepAlive = false;
                    //client.ServerCertificateValidationCallback += ServerCertificateValidationCallback;
                    //client.Proxy.Credentials = System.Net.CredentialCache.DefaultCredentials;

                    //jsonRequest = JsonConvert.SerializeObject(consultaMotorista, Formatting.Indented);
                    //var data = Encoding.UTF8.GetBytes(jsonRequest);

                    //client.UserAgent = @".NET Framework";
                    //client.Accept = "application/json;charset=UTF-8";
                    //client.ContentType = "application/json;charset=UTF-8";
                    //client.Headers["Cache-Control"] = "no-cache";
                    //client.AllowAutoRedirect = true;
                    //client.CookieContainer = cookieContainer;
                    //client.ContentLength = data.Length;

                    //var certificado = new System.Security.Cryptography.X509Certificates.X509Certificate2(caminhoCertificado, senhaCertificado, X509KeyStorageFlags.UserKeySet | X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.Exportable);

                    //client.ClientCertificates.Add(certificado);
                    //client.PreAuthenticate = true;

                    //StreamWriter dataWriter = null;

                    //try
                    //{
                    //    dataWriter = new StreamWriter(client.GetRequestStream());
                    //    dataWriter.Write(jsonRequest);
                    //    Servicos.Log.TratarErro(jsonRequest, "Telerisco");
                    //}
                    //catch (Exception e)
                    //{
                    //    Servicos.Log.TratarErro(e, "Telerisco");
                    //    Servicos.Log.TratarErro(jsonRequest, "Telerisco");
                    //    Servicos.Log.TratarErro("Erro ao preparar dados de requisição", "Telerisco");
                    //}
                    //finally
                    //{
                    //    if (dataWriter != null)
                    //        dataWriter.Close();
                    //}

                    //HttpWebResponse objResponse = (HttpWebResponse)client.GetResponse();
                    //using (StreamReader sr = new StreamReader(objResponse.GetResponseStream()))
                    //{
                    //    jsonResponse = sr.ReadToEnd();
                    //    sr.Close();
                    //}

                    //Dominio.ObjetosDeValor.Embarcador.Integracao.Telerisco.ConsultaMotoristaResponse retornoConsulta = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Telerisco.ConsultaMotoristaResponse>(jsonResponse);

                    //return retornoConsulta;

                    //TESTE COM RestClient
                    //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    //ServicePointManager.Expect100Continue = true;
                    //ServicePointManager.DefaultConnectionLimit = 9999;
                    //System.Net.ServicePointManager.ServerCertificateValidationCallback += (s, cert, chain, sslPolicyErrors) => true;

                    //var client = new RestClient(urlAPI);

                    //var certificado = new System.Security.Cryptography.X509Certificates.X509Certificate2(caminhoCertificado, senhaCertificado, X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet);

                    ////using (X509Store store = new X509Store(StoreName.TrustedPeople, StoreLocation.LocalMachine))
                    ////{
                    ////    store.Open(OpenFlags.ReadWrite);
                    ////    store.Add(certificado); //where cert is an X509Certificate object
                    ////}

                    //jsonRequest = JsonConvert.SerializeObject(consultaMotorista, Formatting.Indented);

                    //client.ClientCertificates = new X509CertificateCollection() { certificado };
                    //client.Proxy = new WebProxy();
                    //client.Timeout = -1;

                    //var request = new RestRequest(Method.POST);
                    //request.AddHeader("Content-Type", "application/json");
                    //request.AddHeader("Cache-Control", "no-cache");
                    //request.AddHeader("Accept", "application/json");

                    //request.AddParameter("application/json", jsonRequest, ParameterType.RequestBody);                    
                    //IRestResponse response = client.Execute(request);

                    //jsonResponse = response.Content;

                    //Dominio.ObjetosDeValor.Embarcador.Integracao.Telerisco.ConsultaMotoristaResponse retornoConsulta = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Telerisco.ConsultaMotoristaResponse>(jsonResponse);

                    //return retornoConsulta;
                }

            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro("ConsultaMotorista exceção: " + excecao, "Telerisco");
                mensagemErro = "SERVIÇO DA TELERISCO INDISPONÍVEL";// result.ReasonPhrase;
                return null;
            }
        }

        #endregion

        #region Métodos Privados

        private void MontaRequest(string endpoint, string metodo)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            HttpWebRequest client = (HttpWebRequest)WebRequest.Create(endpoint);

            client.Method = metodo;
            client.ProtocolVersion = HttpVersion.Version10;
            client.ContentType = "application/json";
            client.KeepAlive = false;

            ClientRequest = client;
        }

        private bool SetDadosRequisicao(dynamic dados)
        {
            StreamWriter dataWriter = null;
            string jsonPost = JsonConvert.SerializeObject(dados, Formatting.Indented);

            ClientRequestContent = jsonPost;

            try
            {
                dataWriter = new StreamWriter(ClientRequest.GetRequestStream());
                dataWriter.Write(jsonPost);
            }
            catch (Exception e)
            {
                Servicos.Log.TratarErro(e, "IntegracaoGPA");
                Servicos.Log.TratarErro("Erro ao preparar dados de requisição", "IntegracaoGPA");
                return false;
            }
            finally
            {
                if (dataWriter != null)
                    dataWriter.Close();
            }

            return true;
        }

        private void AdicionarCertificadoRequisicao(string caminhoCertificado, string senhaCertificado)
        {
            // Buscar Certificado Configurado
            var certificado = new X509Certificate2(caminhoCertificado, senhaCertificado);

            ClientRequest.ClientCertificates.Add(certificado);
            ClientRequest.PreAuthenticate = true;
        }

        private static bool ServerCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.None)
            {
                Console.WriteLine("Certificate OK");
                return true;
            }
            else
            {
                Console.WriteLine("Certificate ERROR");
                return false;
            }
        }

        //public class HttpWebRequestClientCertificateTest : ICertificatePolicy
        //{

        //    public bool CheckValidationResult(ServicePoint sp, X509Certificate certificate,
        //            WebRequest request, int error)
        //    {
        //        return true; // server certificate's CA is not known to windows.
        //    }

        //    static void Main(string[] args)
        //    {
        //        string host = "https://contingencia.telerisco.com.br/hub/consulta/";
        //        if (args.Length > 0)
        //            host = args[0];

        //        X509Certificate2 certificate = null;
        //        if (args.Length > 1)
        //        {
        //            string password = null;
        //            if (args.Length > 2)
        //                password = args[2];
        //            certificate = new X509Certificate2(args[1], password);
        //        }

        //        ServicePointManager.CertificatePolicy = new HttpWebRequestClientCertificateTest();

        //        HttpWebRequest req = (HttpWebRequest)WebRequest.Create(host);
        //        if (certificate != null)
        //            req.ClientCertificates.Add(certificate);

        //        WebResponse resp = req.GetResponse();
        //        Stream stream = resp.GetResponseStream();
        //        StreamReader sr = new StreamReader(stream, Encoding.UTF8);
        //        Console.WriteLine(sr.ReadToEnd());
        //    }
        //}

        #endregion
    }
}
