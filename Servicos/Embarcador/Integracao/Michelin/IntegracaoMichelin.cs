using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Integracao.Michelin;
using ICSharpCode.SharpZipLib.Zip;
using Infrastructure.Services.HttpClientFactory;
using iTextSharp.text.pdf.parser;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Servicos.Embarcador.Integracao.Michelin
{
    public sealed class IntegracaoMichelin
    {
        #region Atributos

        private readonly Repositorio.UnitOfWork _unitOfWork;
        private Dominio.ObjetosDeValor.Embarcador.Integracao.Michelin.ConfiguracaoIntegracao _configuracaoIntegracao = null;

        #endregion

        #region Construtores

        public IntegracaoMichelin(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Métodos Privados

        private SecurityProtocolType GetSecurityProtocolType()
        {
            return (SecurityProtocolType)12288;
        }

        private string ObterToken()
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Michelin.ConfiguracaoIntegracao configuracao = ObterConfiguracao();

            string token = string.Empty;

            if (configuracao != null)
            {
                string urlAPI = configuracao.UrlAutenticacao;
                ServicePointManager.SecurityProtocol = GetSecurityProtocolType();
                ServicePointManager.Expect100Continue = true;

                System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

                HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoMichelin));
                client.BaseAddress = new Uri(urlAPI);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                Dominio.ObjetosDeValor.Embarcador.Integracao.Michelin.Auth antenticar = new Dominio.ObjetosDeValor.Embarcador.Integracao.Michelin.Auth
                {
                    username = configuracao.Identificacao,
                    password = configuracao.Senha
                };

                string jsonPost = JsonConvert.SerializeObject(antenticar, Formatting.Indented);

                var content = new StringContent(jsonPost.ToString(), Encoding.UTF8, "application/json");
                var result = client.PostAsync(urlAPI, content).Result;

                if (result.IsSuccessStatusCode)
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Michelin.RespostaAutenticacao antenticarRetorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Michelin.RespostaAutenticacao>(result.Content.ReadAsStringAsync().Result);

                    token = antenticarRetorno.token;

                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }
                else
                {
                    Servicos.Log.TratarErro("Login: " + jsonPost, "Michelin");
                    Servicos.Log.TratarErro("Login retorno: Não autenticou", "Michelin");
                }
            }
            else
                Servicos.Log.TratarErro("Login: Não possui configuração para logar", "Michelin");

            return token;

        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Michelin.ConfiguracaoIntegracao ObterConfiguracao()
        {
            if (_configuracaoIntegracao != null)
                return _configuracaoIntegracao;

            Repositorio.Embarcador.Configuracoes.Integracao repositorioIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.Integracao integracao = repositorioIntegracao.Buscar();

            if (!(integracao?.PossuiIntegracaoMichelin ?? false))
                throw new ServicoException("Não foram configurados os dados de integração com a Michelin");

            Dominio.ObjetosDeValor.Embarcador.Integracao.Michelin.ConfiguracaoIntegracao configuracaoIntegracao = new Dominio.ObjetosDeValor.Embarcador.Integracao.Michelin.ConfiguracaoIntegracao()
            {
                UrlAutenticacao = integracao.URLHomologacaoMichelin + "/api/authenticate",
                UrlIntegracao = integracao.URLHomologacaoMichelin,
                Identificacao = integracao.UsuarioMichelin,
                Senha = integracao.SenhaMichelin,
                Code = integracao.CodigoTransportadoraMichelin,
                Document = integracao.CnpjTransportadoraMichelin
            };

            return _configuracaoIntegracao = configuracaoIntegracao;
        }

        #endregion        

        #region Métodos Públicos

        public Dominio.ObjetosDeValor.EDI.Pedido.Pedido ObterPedidos(string cnpjEmpresa)
        {
            string token = ObterToken();

            if (_configuracaoIntegracao == null || string.IsNullOrWhiteSpace(token))
                return null;

            string document = !string.IsNullOrWhiteSpace(cnpjEmpresa) ? cnpjEmpresa : _configuracaoIntegracao.Document;

            string urlAPI = _configuracaoIntegracao.UrlIntegracao + "/api/delivery-line/find?document=" + document + "&code=" + _configuracaoIntegracao.Code + "&sort=created,desc";

            ServicePointManager.SecurityProtocol = GetSecurityProtocolType();

            HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoMichelin));
            client.BaseAddress = new Uri(urlAPI);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var result = client.GetAsync(urlAPI).Result;
            if (result.IsSuccessStatusCode)
            {
                string bodyResposta = result.Content.ReadAsStringAsync().Result;

                var respostaIntegracao = ObterBodyConvertidoRespostaIntegracao(bodyResposta);

                if (respostaIntegracao.Dados.Conteudo.Count <= 0)
                    throw new ServicoException($"Michelin não dispnonibilizou nenhum pedido para integração.");

                return FormatarPedidos(respostaIntegracao.Dados);
            }
            else
            {
                throw new ServicoException($"Ocorreu uma falha ao integrar com a API. Mensagem: {result.Content.ReadAsStringAsync().Result}");
            }

        }

        public void EnviarMontagemCarga(System.IO.MemoryStream arquivo, string nomeArquivo, out string msgRetorno)
        {
            string token = ObterToken();
            msgRetorno = string.Empty;

            if ((_configuracaoIntegracao == null) || string.IsNullOrWhiteSpace(token))
                throw new ServicoException("Não possui configuração para a integração com a Michelin");

            string urlAPI = $"{_configuracaoIntegracao.UrlIntegracao}/api/shipment-plan/upload?code={_configuracaoIntegracao.Code}&document={_configuracaoIntegracao.Document}";
            ServicePointManager.SecurityProtocol = GetSecurityProtocolType();
            HttpClient requisicao = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoMichelin));
            MultipartFormDataContent conteudoRequisicao = new MultipartFormDataContent();
            ByteArrayContent conteudoArquivo = new ByteArrayContent(arquivo.ToArray());

            requisicao.BaseAddress = new Uri(urlAPI);
            requisicao.DefaultRequestHeaders.Accept.Clear();
            requisicao.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            requisicao.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            conteudoArquivo.Headers.ContentType = MediaTypeHeaderValue.Parse("text/txt");
            conteudoRequisicao.Add(conteudoArquivo, "file", $"{nomeArquivo}.txt");

            HttpResponseMessage retornoRequisicao = requisicao.PostAsync(urlAPI, conteudoRequisicao).Result;

            if (!retornoRequisicao.IsSuccessStatusCode)
                msgRetorno = ($"Ocorreu uma falha ao integrar com a API. Mensagem: {retornoRequisicao.Content.ReadAsStringAsync().Result}");
        }

        public void EnviarCONEMB(ref Dominio.Entidades.Embarcador.Cargas.CargaEDIIntegracao cargaEDIIntegracao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unidadeDeTrabalho, string stringConexao)
        {
            if (cargaEDIIntegracao.TipoIntegracao.Tipo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Michelin)
                return;

            string mensagemErro = string.Empty;
            string extensao = string.Empty;

            try
            {
                using (System.IO.MemoryStream arquivoEDI = IntegracaoEDI.GerarEDI(cargaEDIIntegracao, tipoServicoMultisoftware, unidadeDeTrabalho, stringConexao, out extensao))
                {
                    string nomeArquivo = IntegracaoEDI.ObterNomeArquivoEDI(cargaEDIIntegracao, extensao, unidadeDeTrabalho);

                    string token = ObterToken();

                    if (_configuracaoIntegracao == null || string.IsNullOrWhiteSpace(token))
                    {
                        mensagemErro = "Não possui configuração para a integração com a Michelin";
                        cargaEDIIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    }

                    string urlAPI = _configuracaoIntegracao.UrlIntegracao + "/api/invoice-knowledge/upload?code=" + _configuracaoIntegracao.Code + "&document=" + _configuracaoIntegracao.Document;

                    ServicePointManager.SecurityProtocol = GetSecurityProtocolType();

                    HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoMichelin));
                    client.BaseAddress = new Uri(urlAPI);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                    var requestContent = new MultipartFormDataContent();
                    var ediContent = new ByteArrayContent(arquivoEDI.ToArray());
                    ediContent.Headers.ContentType = MediaTypeHeaderValue.Parse("text/txt");

                    requestContent.Add(ediContent, "file", nomeArquivo + ".txt");

                    var result = client.PostAsync(urlAPI, requestContent).Result;
                    if (result.IsSuccessStatusCode)
                    {
                        mensagemErro = "Envio realizado com sucesso.";
                        cargaEDIIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                    }
                    else
                    {
                        cargaEDIIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                        mensagemErro = ($"Ocorreu uma falha ao integrar com a API. Mensagem: {result.Content.ReadAsStringAsync().Result}");
                    }
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "Michelin");

                mensagemErro = "Falha genérica ao enviar o EDI " + cargaEDIIntegracao.LayoutEDI.Descricao + " para a Michelin.";

                cargaEDIIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
            }
            finally
            {
                cargaEDIIntegracao.IniciouConexaoExterna = false;
                cargaEDIIntegracao.ProblemaIntegracao = mensagemErro;
                cargaEDIIntegracao.DataIntegracao = DateTime.Now;
                cargaEDIIntegracao.NumeroTentativas++;
            }
        }

        public void EnviarOCORREN(Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaEDIIntegracao ocorrenciaEDIIntegracao, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (ocorrenciaEDIIntegracao.TipoIntegracao.Tipo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Michelin)
                return;

            string mensagemErro = string.Empty;
            string extensao = string.Empty;

            try
            {
                using (System.IO.MemoryStream arquivoEDI = IntegracaoEDI.GerarEDI(ocorrenciaEDIIntegracao, unidadeDeTrabalho))
                {
                    string nomeArquivo = IntegracaoEDI.ObterNomeArquivoEDI(ocorrenciaEDIIntegracao, false, unidadeDeTrabalho);

                    string token = ObterToken();

                    if (_configuracaoIntegracao == null || string.IsNullOrWhiteSpace(token))
                    {
                        mensagemErro = "Não possui configuração para a integração com a Michelin";
                        ocorrenciaEDIIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    }

                    string urlAPI = _configuracaoIntegracao.UrlIntegracao + "/api/delivery-proof/upload?code=" + _configuracaoIntegracao.Code + "&document=" + _configuracaoIntegracao.Document;

                    ServicePointManager.SecurityProtocol = GetSecurityProtocolType();

                    HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoMichelin));
                    client.BaseAddress = new Uri(urlAPI);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                    var requestContent = new MultipartFormDataContent();
                    var ediContent = new ByteArrayContent(arquivoEDI.ToArray());
                    ediContent.Headers.ContentType = MediaTypeHeaderValue.Parse("text/txt");

                    requestContent.Add(ediContent, "file", nomeArquivo + ".txt");

                    var result = client.PostAsync(urlAPI, requestContent).Result;
                    if (result.IsSuccessStatusCode)
                    {
                        mensagemErro = "Envio realizado com sucesso.";
                        ocorrenciaEDIIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                    }
                    else
                    {
                        ocorrenciaEDIIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                        mensagemErro = ($"Ocorreu uma falha ao integrar com a API. Mensagem: {result.Content.ReadAsStringAsync().Result}");
                    }
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "Michelin");

                mensagemErro = "Falha genérica ao enviar o EDI " + ocorrenciaEDIIntegracao.LayoutEDI.Descricao + " para a Michelin.";

                ocorrenciaEDIIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
            }
            finally
            {
                ocorrenciaEDIIntegracao.IniciouConexaoExterna = false;
                ocorrenciaEDIIntegracao.ProblemaIntegracao = mensagemErro;
                ocorrenciaEDIIntegracao.DataIntegracao = DateTime.Now;
                ocorrenciaEDIIntegracao.NumeroTentativas++;
            }
        }

        public void BuscarNOTFISAsync(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, string stringConexao, string adminStringConexao, Dominio.Entidades.Empresa empresa, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, out string msgRetorno, out bool alteradoTipoDeCarga)
        {
            msgRetorno = string.Empty;
            alteradoTipoDeCarga = false;

            try
            {
                string token = ObterToken();

                if (_configuracaoIntegracao == null || string.IsNullOrWhiteSpace(token))
                {
                    msgRetorno = "Não possui configuração para a integração com a Michelin";
                    return;
                }

                string urlAPI = _configuracaoIntegracao.UrlIntegracao + "/api/delivery-line/download?format=txt&list=" + cargaPedido.Pedido.MessageIdentifierCodeMichelin;
                string caminhoArquivo = $"{Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "MICHELIN", "NOTFIS" })}";
                
                string guidArquivo = Guid.NewGuid().ToString().Replace("-", "");
                string arquivoProcessarZio = Utilidades.IO.FileStorageService.Storage.Combine(caminhoArquivo, guidArquivo + ".zip");

                ServicePointManager.SecurityProtocol = GetSecurityProtocolType();

                List<Stream> stream = null;
                try
                {
                    stream = DownloadURL(urlAPI, token, arquivoProcessarZio);
                }
                catch (Exception e)
                {
                    Servicos.Log.TratarErro("Download Arquivo: " + e, "Michelin");
                    msgRetorno = "A Michelin não disponibilizou o arquivo para download. " + e.Message;
                    return;
                }
                if (stream == null || stream.Count == 0)
                {
                    msgRetorno = "A Michelin não disponibilizou nenhum arquivo para download.";
                    return;
                }


                List<string> arquivosProcessar = new List<string>();

                foreach (var str in stream)
                {
                    guidArquivo = Guid.NewGuid().ToString().Replace("-", "");
                    string arquivoProcessar = Utilidades.IO.FileStorageService.Storage.Combine(caminhoArquivo, guidArquivo + ".txt");

                    using (var fileStream = Utilidades.IO.FileStorageService.Storage.Create(arquivoProcessar))
                    {
                        str.Seek(0, SeekOrigin.Begin);
                        str.CopyTo(fileStream);
                        arquivosProcessar.Add(arquivoProcessar);
                    }
                }

                if (arquivosProcessar != null && arquivosProcessar.Count > 0)
                {
                    foreach (string arquivoProcessar in arquivosProcessar)
                    {
                        string fileName = Utilidades.IO.FileStorageService.Storage.Combine(caminhoArquivo, System.IO.Path.GetFileName(arquivoProcessar));
                        string extensao = System.IO.Path.GetExtension(arquivoProcessar).ToLower();
                        string nomeArquivo = System.IO.Path.GetFileName(arquivoProcessar);

                        if (extensao.Equals(".txt") || extensao.Equals(".pal") || extensao.Equals(".edi") || extensao.Equals(".nnn") || extensao.Equals(".sao"))
                        {
                            Dominio.Entidades.LayoutEDI layoutEDI = null;
                            if (cargaPedido.Carga.TipoOperacao != null)
                                layoutEDI = (from obj in cargaPedido.Carga.TipoOperacao.LayoutsEDI where (obj.LayoutEDI.ExtensaoArquivo.Contains(extensao.Replace(".", "")) || obj.LayoutEDI.ExtensaoArquivo.Contains(extensao) || obj.LayoutEDI.ExtensaoArquivo == "") && (obj.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.NOTFIS || obj.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.NOTFIS_NOVA_IMPORTACAO) && obj.LayoutEDI.Status == "A" select obj.LayoutEDI).FirstOrDefault();

                            if (layoutEDI == null && cargaPedido.Pedido.Remetente == null && cargaPedido.Pedido.GrupoPessoas != null)
                                layoutEDI = (from obj in cargaPedido.Pedido.GrupoPessoas.LayoutsEDI where (obj.LayoutEDI.ExtensaoArquivo.Contains(extensao.Replace(".", "")) || obj.LayoutEDI.ExtensaoArquivo.Contains(extensao) || obj.LayoutEDI.ExtensaoArquivo == "") && (obj.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.NOTFIS || obj.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.NOTFIS_NOVA_IMPORTACAO) && obj.LayoutEDI.Status == "A" select obj.LayoutEDI).FirstOrDefault();

                            if (layoutEDI == null && cargaPedido.Pedido.Remetente != null)
                                layoutEDI = (from obj in cargaPedido.Pedido.Remetente.LayoutsEDI where (obj.LayoutEDI.ExtensaoArquivo.Contains(extensao.Replace(".", "")) || obj.LayoutEDI.ExtensaoArquivo.Contains(extensao) || obj.LayoutEDI.ExtensaoArquivo == "") && (obj.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.NOTFIS || obj.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.NOTFIS_NOVA_IMPORTACAO) && obj.LayoutEDI.Status == "A" select obj.LayoutEDI).FirstOrDefault();

                            if (layoutEDI == null && cargaPedido.Pedido.Remetente != null)
                            {
                                if (cargaPedido.Pedido.Remetente.GrupoPessoas != null)
                                    layoutEDI = (from obj in cargaPedido.Pedido.Remetente.GrupoPessoas.LayoutsEDI where (obj.LayoutEDI.ExtensaoArquivo.Contains(extensao.Replace(".", "")) || obj.LayoutEDI.ExtensaoArquivo.Contains(extensao) || obj.LayoutEDI.ExtensaoArquivo == "") && (obj.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.NOTFIS || obj.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.NOTFIS_NOVA_IMPORTACAO) && obj.LayoutEDI.Status == "A" select obj.LayoutEDI).FirstOrDefault();
                            }

                            if (layoutEDI != null)
                            {
                                AdminMultisoftware.Repositorio.UnitOfWork adminUnitOfWork = new AdminMultisoftware.Repositorio.UnitOfWork(adminStringConexao);
                                try
                                {
                                    using (StreamReader inputStream = new StreamReader(Utilidades.IO.FileStorageService.Storage.OpenRead(fileName)))
                                    {
                                        string retorono = ProcessarEDI(cargaPedido, unitOfWork, adminUnitOfWork, inputStream.BaseStream, layoutEDI, string.Empty, tipoServicoMultisoftware, out alteradoTipoDeCarga, empresa, stringConexao, adminStringConexao, auditado);
                                        if (!string.IsNullOrWhiteSpace(retorono))
                                        {
                                            msgRetorno = retorono;
                                            return;
                                        }
                                    }
                                }
                                catch
                                {
                                    throw;
                                }
                                finally
                                {
                                    if (adminUnitOfWork.IsActiveTransaction())
                                        adminUnitOfWork.Dispose();
                                }

                            }
                        }
                    }

                    //DeletarArquivosMichelin(caminhoArquivo);
                }
                else
                {
                    msgRetorno = "A Michelin não disponibilizou nenhum arquivo para download.";
                    return;
                }

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "Michelin");

                msgRetorno = "Falha genérica ao buscar o NOFTIS com a Michelin.";
                return;
            }
        }

        #endregion

        #region Métodos Privados Integração

        private List<Stream> DownloadURL(string url, string token, string arquivoProcessar)
        {
            List<Stream> stream = new List<Stream>();
            try
            {
                HttpWebRequest fileReq = (HttpWebRequest)HttpWebRequest.Create(url);
                fileReq.Method = "GET";
                fileReq.Headers.Add("Authorization", "Bearer " + token);

                //Create a response for this request
                WebResponse webResponse = fileReq.GetResponse();
                using (var responseStream = webResponse.GetResponseStream())
                using (var ms = new MemoryStream())
                {
                    responseStream.CopyTo(ms);

                    var zipFile = new ICSharpCode.SharpZipLib.Zip.ZipFile(ms);

                    foreach (ZipEntry item in zipFile)
                    {
                        using (var s = new StreamReader(zipFile.GetInputStream(item)))
                        {
                            if (System.IO.Path.GetExtension(item.Name) == ".txt" && item.CanDecompress)
                            {
                                byte[] byteArray = Encoding.UTF8.GetBytes(s.ReadToEnd());
                                stream.Add(new MemoryStream(byteArray));
                            }
                        }
                    }
                }
                return stream;
            }
            finally
            {

            }
        }

        private void DeletarArquivosMichelin(string caminhoArquivo)
        {
            //deleta todos os arquivos que foram processados
            IEnumerable<string> arquivos = Utilidades.IO.FileStorageService.Storage.GetFiles(caminhoArquivo);

            for (int i = 0; i < arquivos.Count(); i++)
            {
                Utilidades.IO.FileStorageService.Storage.Delete(arquivos.ElementAt(i));
            }
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Michelin.RespostaIntegracao ObterBodyConvertidoRespostaIntegracao(string body)
        {
            return JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Michelin.RespostaIntegracao>(body);
        }

        private Dominio.ObjetosDeValor.EDI.Pedido.Pedido FormatarPedidos(RespostaData dados)
        {
            Dominio.ObjetosDeValor.EDI.Pedido.Pedido retornoPedido = new Dominio.ObjetosDeValor.EDI.Pedido.Pedido()
            {
                Pedidos = new List<Dominio.ObjetosDeValor.EDI.Pedido.DetalhePedido>()
            };
            retornoPedido.CabecalhoDocumento = new Dominio.ObjetosDeValor.EDI.Pedido.CabecalhoDocumento();
            foreach (var conteudo in dados.Conteudo)
            {
                retornoPedido.CabecalhoDocumento.CNPJEmissora = conteudo.Header.CNPJRemetente;

                foreach (var detalhe in conteudo.Detalhes)
                {
                    Dominio.ObjetosDeValor.EDI.Pedido.DetalhePedido pedido = new Dominio.ObjetosDeValor.EDI.Pedido.DetalhePedido();
                    pedido.Cidade = detalhe.Cidade;
                    pedido.CodigoCliente = detalhe.CodigoCliente;
                    pedido.CodigoItem = detalhe.CodigoItem;
                    pedido.DescricaoItem = detalhe.DescricaoItem;
                    pedido.NumeroCarga = detalhe.NumeroPedidoEmbarcador;
                    pedido.Quantidade = detalhe.Quantidade;
                    pedido.Peso = detalhe.Peso;
                    pedido.DataFaturamento = detalhe.DataFaturamento;

                    pedido.DescricaoCliente = detalhe.ClienteNome;
                    pedido.UF = detalhe.Uf;
                    pedido.NumeroPedido = detalhe.NumeroPedido;
                    pedido.Item = detalhe.TipoCarga;
                    pedido.FileId = detalhe.FileId;
                    pedido.MessageIdentifierCode = detalhe.MessageIdentifierCode;
                    pedido.CNPJRemetente = conteudo.Header.CNPJRemetente;
                    pedido.DataCriacao = conteudo.DataCriacao;
                    pedido.NomeArquivo = conteudo.NomeArquivo;

                    retornoPedido.Pedidos.Add(pedido);
                }
            }

            return retornoPedido;
        }

        private string ProcessarEDI(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork adminUnitOfWork, System.IO.Stream Stream, Dominio.Entidades.LayoutEDI layoutEDI, string rotas, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, out bool alteradoTipoDeCarga, Dominio.Entidades.Empresa empresa, string stringConexao, string adminStringConexao, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            alteradoTipoDeCarga = false;
            bool validarRotasEDI = layoutEDI.ValidarRota;
            if (validarRotasEDI && string.IsNullOrWhiteSpace(rotas))
                return "É obrigatório informar as rotas para processar este arquivo EDI";

            Stream.Position = 0;

            if (layoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.NOTFIS)
            {
                MemoryStream StreamEDI = new MemoryStream();
                Stream.CopyTo(StreamEDI);

                if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                {
                    Servicos.LeituraEDI serLeituraEDI = new Servicos.LeituraEDI(empresa, layoutEDI, StreamEDI, unitOfWork, 0, 0, 0, 0, 0, 0, 0, 0, true, true, Encoding.UTF8);
                    List<Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal> notasFiscais = serLeituraEDI.GerarNotasFiscais();

                    if (notasFiscais.Count == 0)
                        return "Nenhuma Nota Fiscal importada.";

                    return ProcessarNotasFiscaisArquivo(cargaPedido, validarRotasEDI, rotas, notasFiscais, unitOfWork, adminUnitOfWork, layoutEDI, out alteradoTipoDeCarga, stringConexao, tipoServicoMultisoftware, adminStringConexao, auditado);
                }
                else
                {
                    Servicos.LeituraEDI serLeituraEDI = new Servicos.LeituraEDI(empresa, layoutEDI, StreamEDI, unitOfWork);
                    List<Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal> notasFiscais = serLeituraEDI.GerarNotasFiscais();

                    return ProcessarNotasFiscaisArquivo(cargaPedido, validarRotasEDI, rotas, notasFiscais, unitOfWork, adminUnitOfWork, layoutEDI, out alteradoTipoDeCarga, stringConexao, tipoServicoMultisoftware, adminStringConexao, auditado);
                }

            }
            else
            {
                return "Não foi encontrado nenhuma configuração de leitura de NOTFIS para o arquivo disponibilizado pela Michelin, favor verifique as configurações.";
            }
        }

        private string ProcessarNotasFiscaisArquivo(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, bool validarRotas, string rotas, List<Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal> notasFiscais, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork adminUnitOfWork, Dominio.Entidades.LayoutEDI layoutEDI, out bool alteradoTipoDeCarga, string stringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, string adminStringConexao, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            alteradoTipoDeCarga = false;
            bool notaAceita = false;
            string mensagem = "";
            string[] idenNotas = rotas.Replace(" ", "").Split('/');

            AdminMultisoftware.Repositorio.Localidades.Endereco repEndereco = new AdminMultisoftware.Repositorio.Localidades.Endereco(adminUnitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoRotas repCargaPedidoRotas = new Repositorio.Embarcador.Cargas.CargaPedidoRotas(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa repDocumentoDestinadoEmpresa = new Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork).BuscarPrimeiroRegistro();

            Servicos.Embarcador.Pedido.NotaFiscal serCargaNotaFiscal = new Servicos.Embarcador.Pedido.NotaFiscal(unitOfWork);
            Servicos.Cliente servicoCliente = new Servicos.Cliente(stringConexao);
            Servicos.WebService.Pessoas.Pessoa serPessoa = new Servicos.WebService.Pessoas.Pessoa();

            bool separarNotasEDIPorPedido = configuracaoTMS.GerarPedidoImportacaoNotfisEtapaNFe;

            //Se não tiver notas ou não tem numero do pedido não sepera
            if (separarNotasEDIPorPedido && (notasFiscais.Count == 0 || string.IsNullOrWhiteSpace(notasFiscais.FirstOrDefault().NumeroPedido)))
                separarNotasEDIPorPedido = false;

            List<string> identificaoesUsadas = new List<string>();

            if (layoutEDI?.AgruparNotasFiscaisDosCTesParaSubcontratacao ?? false)
            {
                return "Layout não configurado para o processamento do NOTFIS da Michelin.";
            }
            else
            {
                int notas = 0;

                foreach (Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal notaFiscal in notasFiscais)
                {
                    if (!validarRotas || idenNotas.Contains(notaFiscal.Rota.Replace(" ", "")))
                    {
                        if (separarNotasEDIPorPedido)
                        {
                            //Se pedido não tem nota ou o numero do pedido é o mesmo que da nota
                            if (notas == 0 && repPedidoXMLNotaFiscal.ContarPorCargaPedido(cargaPedido.Codigo) == 0)
                            {
                                cargaPedido.Pedido.NumeroPedidoEmbarcador = notaFiscal.NumeroPedido;
                                repPedido.Atualizar(cargaPedido.Pedido);
                            }
                            else
                            {
                                //Criar novo carga pedido
                                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoNovo = new Dominio.Entidades.Embarcador.Cargas.CargaPedido();
                                Servicos.Embarcador.Carga.CargaPedido.Duplicar(out mensagem, out cargaPedidoNovo, cargaPedido, notaFiscal.NumeroPedido, unitOfWork, stringConexao, tipoServicoMultisoftware, configuracaoTMS, configuracaoGeralCarga);
                                cargaPedido = cargaPedidoNovo;
                            }
                        }

                        if (!string.IsNullOrWhiteSpace(notaFiscal.Rota) && !identificaoesUsadas.Contains(notaFiscal.Rota.Replace(" ", "")))
                            identificaoesUsadas.Add(notaFiscal.Rota.Replace(" ", ""));

                        notaFiscal.Modelo = "55";
                        notaFiscal.SituacaoNFeSefaz = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoNFeSefaz.Autorizada;
                        notaFiscal.TipoOperacaoNotaFiscal = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Saida;
                        notaFiscal.Chave = Utilidades.String.OnlyNumbers(notaFiscal.Chave);

                        if (layoutEDI != null && layoutEDI.UtilizarEmitenteDaChave)
                        {
                            notaFiscal.Emitente.CPFCNPJ = Utilidades.Chave.ObterCNPJEmitente(notaFiscal.Chave);

                            double cnpjEmitente = 0D;

                            double.TryParse(notaFiscal.Emitente.CPFCNPJ, out cnpjEmitente);

                            Dominio.Entidades.Cliente emitente = cnpjEmitente > 0D ? repCliente.BuscarPorCPFCNPJ(cnpjEmitente) : null;

                            if (emitente == null)
                                return "O emitente da nota fiscal " + notaFiscal.Chave + " não foi encontrado.";

                            notaFiscal.Emitente.AtualizarEnderecoPessoa = false;
                            notaFiscal.Emitente.TipoPessoa = Dominio.Enumeradores.TipoPessoa.Juridica;
                        }
                        else
                        {
                            if (notaFiscal.Emitente != null)
                            {
                                string[] splitEnderecorEmitente = notaFiscal.Emitente.Endereco.Logradouro.Split(',');
                                notaFiscal.Emitente.Endereco.Logradouro = splitEnderecorEmitente[0].Trim();

                                if (string.IsNullOrWhiteSpace(notaFiscal.Emitente.Endereco.Logradouro) || notaFiscal.Emitente.Endereco.Logradouro.Length <= 2)
                                    notaFiscal.Emitente.Endereco.Logradouro = "NAO INFORMADO";

                                if (splitEnderecorEmitente.Length > 1)
                                {
                                    string[] splitNumero = splitEnderecorEmitente[1].Split('-');
                                    notaFiscal.Emitente.Endereco.Numero = splitNumero[0].Trim().Replace("-", "");

                                    if (notaFiscal.Emitente.Endereco.Numero == "0")
                                        notaFiscal.Emitente.Endereco.Numero = "S/N";
                                    if (splitNumero.Count() > 1)
                                        notaFiscal.Emitente.Endereco.Complemento = splitNumero[1].Trim();
                                }
                                else
                                {
                                    if (string.IsNullOrWhiteSpace(notaFiscal.Emitente.Endereco.Numero))
                                        notaFiscal.Emitente.Endereco.Numero = "S/N";
                                }

                                if (notaFiscal.Emitente.Endereco.Bairro == null || notaFiscal.Emitente.Endereco.Bairro.Length < 3 || string.IsNullOrWhiteSpace(notaFiscal.Emitente.Endereco.Logradouro))
                                {
                                    if (!string.IsNullOrWhiteSpace(notaFiscal.Emitente.Endereco.CEP))
                                    {
                                        if (!adminUnitOfWork.IsOpenSession())
                                        {
                                            adminUnitOfWork = new AdminMultisoftware.Repositorio.UnitOfWork(adminStringConexao);
                                            repEndereco = new AdminMultisoftware.Repositorio.Localidades.Endereco(adminUnitOfWork);
                                        }

                                        AdminMultisoftware.Dominio.Entidades.Localidades.Endereco endereco = repEndereco.BuscarCEP(int.Parse(Utilidades.String.OnlyNumbers(notaFiscal.Emitente.Endereco.CEP)).ToString());

                                        if (endereco != null)
                                            notaFiscal.Emitente.Endereco.Bairro = endereco.Bairro?.Descricao;

                                        if (string.IsNullOrWhiteSpace(notaFiscal.Emitente.Endereco.Logradouro))
                                            notaFiscal.Emitente.Endereco.Logradouro = endereco.Logradouro;
                                    }
                                }

                                notaFiscal.Emitente.CPFCNPJ = Utilidades.String.OnlyNumbers(notaFiscal.Emitente.CPFCNPJ);
                                notaFiscal.Emitente.AtualizarEnderecoPessoa = false;

                                if (notaFiscal.Emitente.CPFCNPJ.Length >= 14)
                                {
                                    notaFiscal.Emitente.CPFCNPJ = notaFiscal.Emitente.CPFCNPJ.Substring(notaFiscal.Emitente.CPFCNPJ.Length - 14);
                                    if (Utilidades.Validate.ValidarCNPJ(notaFiscal.Emitente.CPFCNPJ))
                                        notaFiscal.Emitente.TipoPessoa = Dominio.Enumeradores.TipoPessoa.Juridica;
                                    else
                                    {
                                        notaFiscal.Emitente.CPFCNPJ = notaFiscal.Emitente.CPFCNPJ.Substring(notaFiscal.Emitente.CPFCNPJ.Length - 11);
                                        notaFiscal.Emitente.TipoPessoa = Dominio.Enumeradores.TipoPessoa.Fisica;
                                    }
                                }
                                else
                                    notaFiscal.Emitente.TipoPessoa = Dominio.Enumeradores.TipoPessoa.Fisica;

                                notaFiscal.Emitente.Endereco.Telefone = Utilidades.String.OnlyNumbers(notaFiscal.Emitente.Endereco.Telefone);
                            }
                            else if (cargaPedido.Pedido.Remetente != null) //Pega emitente do pedido
                            {
                                notaFiscal.Emitente = new Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa();
                                notaFiscal.Emitente = serPessoa.ConverterObjetoPessoa(cargaPedido.Pedido.Remetente);
                            }
                            else
                            {
                                return "Emitente não localizado.";
                            }
                        }

                        if (notaFiscal.Destinatario != null)
                        {
                            string[] splitEnderecorDestinatario = notaFiscal.Destinatario.Endereco?.Logradouro?.Split(',') ?? "".Split(',');
                            if (notaFiscal.Destinatario.Endereco == null)
                                notaFiscal.Destinatario.Endereco = new Dominio.ObjetosDeValor.Embarcador.Localidade.Endereco();
                            notaFiscal.Destinatario.Endereco.Logradouro = splitEnderecorDestinatario[0].Trim();

                            if (notaFiscal.Destinatario.Endereco == null || string.IsNullOrWhiteSpace(notaFiscal.Destinatario.Endereco.Logradouro) || notaFiscal.Destinatario.Endereco.Logradouro.Length <= 2)
                                notaFiscal.Destinatario.Endereco.Logradouro = "NAO INFORMADO";

                            if (splitEnderecorDestinatario.Length > 1)
                            {
                                string[] splitNumero = splitEnderecorDestinatario[1].Split('-');
                                notaFiscal.Destinatario.Endereco.Numero = splitNumero[0].Trim().Replace("-", "");
                                if (notaFiscal.Destinatario.Endereco.Numero == "0")
                                    notaFiscal.Destinatario.Endereco.Numero = "S/N";
                                if (splitNumero.Length > 1)
                                {
                                    notaFiscal.Destinatario.Endereco.Complemento = splitNumero[1].Trim();
                                }
                            }
                            else
                            {
                                if (string.IsNullOrWhiteSpace(notaFiscal.Destinatario.Endereco.Numero))
                                    notaFiscal.Destinatario.Endereco.Numero = "S/N";
                            }

                            if (notaFiscal.Destinatario.Endereco.Bairro == null || notaFiscal.Destinatario.Endereco.Bairro.Length < 3 || string.IsNullOrWhiteSpace(notaFiscal.Destinatario.Endereco.Logradouro))
                            {
                                if (!string.IsNullOrWhiteSpace(notaFiscal.Destinatario.Endereco.CEP))
                                {
                                    if (!adminUnitOfWork.IsOpenSession())
                                    {
                                        adminUnitOfWork = new AdminMultisoftware.Repositorio.UnitOfWork(adminStringConexao);
                                        repEndereco = new AdminMultisoftware.Repositorio.Localidades.Endereco(adminUnitOfWork);
                                    }

                                    AdminMultisoftware.Dominio.Entidades.Localidades.Endereco endereco = repEndereco.BuscarCEP(int.Parse(Utilidades.String.OnlyNumbers(notaFiscal.Destinatario.Endereco.CEP)).ToString());
                                    if (endereco != null)
                                        notaFiscal.Destinatario.Endereco.Bairro = endereco.Bairro?.Descricao;

                                    if (string.IsNullOrWhiteSpace(notaFiscal.Destinatario.Endereco.Logradouro))
                                        notaFiscal.Destinatario.Endereco.Logradouro = endereco.Logradouro;
                                }
                            }

                            notaFiscal.Destinatario.CPFCNPJ = Utilidades.String.OnlyNumbers(notaFiscal.Destinatario.CPFCNPJ);
                            notaFiscal.Destinatario.AtualizarEnderecoPessoa = true;

                            if (notaFiscal.Destinatario.CPFCNPJ.Length >= 14)
                            {
                                notaFiscal.Destinatario.CPFCNPJ = notaFiscal.Destinatario.CPFCNPJ.Substring(notaFiscal.Destinatario.CPFCNPJ.Length - 14);
                                if (Utilidades.Validate.ValidarCNPJ(notaFiscal.Destinatario.CPFCNPJ))
                                    notaFiscal.Destinatario.TipoPessoa = Dominio.Enumeradores.TipoPessoa.Juridica;
                                else if (notaFiscal.Destinatario.CPFCNPJ == "00000000000000" || string.IsNullOrWhiteSpace(notaFiscal.Destinatario.CPFCNPJ))
                                {
                                    notaFiscal.Destinatario.RGIE = "ISENTO";
                                    notaFiscal.Destinatario.ClienteExterior = true;
                                }
                                else
                                {
                                    notaFiscal.Destinatario.CPFCNPJ = notaFiscal.Destinatario.CPFCNPJ.Substring(notaFiscal.Destinatario.CPFCNPJ.Length - 11);
                                    notaFiscal.Destinatario.RGIE = "ISENTO";
                                    notaFiscal.Destinatario.TipoPessoa = Dominio.Enumeradores.TipoPessoa.Fisica;
                                }
                            }
                            else
                                notaFiscal.Destinatario.TipoPessoa = Dominio.Enumeradores.TipoPessoa.Fisica;


                            notaFiscal.Destinatario.Endereco.Telefone = Utilidades.String.OnlyNumbers(notaFiscal.Destinatario.Endereco.Telefone);
                        }
                        else if (cargaPedido.Pedido.Destinatario != null) //Pega destinatário do pedido
                        {
                            notaFiscal.Destinatario = new Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa();
                            notaFiscal.Destinatario = serPessoa.ConverterObjetoPessoa(cargaPedido.Pedido.Destinatario);
                        }
                        else
                        {
                            return "Destinatário não localizado.";
                        }

                        if (notaFiscal.Tomador != null)
                        {
                            if (layoutEDI != null && layoutEDI.UtilizarTomadorExistente)
                            {
                                double cpfCnpjTomador = 0D;

                                double.TryParse(Utilidades.String.OnlyNumbers(notaFiscal.Tomador.CPFCNPJ), out cpfCnpjTomador);

                                Dominio.Entidades.Cliente tomador = cpfCnpjTomador > 0D ? repCliente.BuscarPorCPFCNPJ(cpfCnpjTomador) : null;

                                if (tomador == null)
                                {
                                    return "O tomador (" + notaFiscal.Tomador.CPFCNPJ + ") da nota fiscal (" + notaFiscal.Chave + ") não está cadastrado.";
                                }

                                notaFiscal.Tomador.AtualizarEnderecoPessoa = false;
                                notaFiscal.Tomador.TipoPessoa = tomador.Tipo == "F" ? Dominio.Enumeradores.TipoPessoa.Fisica : Dominio.Enumeradores.TipoPessoa.Juridica;
                            }
                            else
                            {
                                notaFiscal.Tomador = servicoCliente.SetarDadosPessoa(notaFiscal.Tomador, adminUnitOfWork, unitOfWork, false);
                            }
                        }

                        string retorno = serCargaNotaFiscal.InformarDadosNotaCarga(notaFiscal, cargaPedido, tipoServicoMultisoftware, configuracaoTMS, auditado, out alteradoTipoDeCarga);

                        if (!string.IsNullOrWhiteSpace(retorno))
                            mensagem = retorno;
                        else
                            notaAceita = true;

                        notas++;
                    }
                }
            }

            if (notaAceita)
                mensagem = "";

            if (string.IsNullOrWhiteSpace(mensagem))
            {
                foreach (string identi in identificaoesUsadas)
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaPedidoRotas cargaPedidoIdentificacaoNotas = repCargaPedidoRotas.BuscarPorCodigoIdentificacao(cargaPedido.Codigo, identi);
                    if (cargaPedidoIdentificacaoNotas == null)
                    {
                        cargaPedidoIdentificacaoNotas = new Dominio.Entidades.Embarcador.Cargas.CargaPedidoRotas();
                        cargaPedidoIdentificacaoNotas.IdenticacaoRota = identi;
                        cargaPedidoIdentificacaoNotas.CargaPedido = cargaPedido;
                        repCargaPedidoRotas.Inserir(cargaPedidoIdentificacaoNotas);
                    }
                }
            }
            
            return mensagem;
        }

        #endregion
    }
}
