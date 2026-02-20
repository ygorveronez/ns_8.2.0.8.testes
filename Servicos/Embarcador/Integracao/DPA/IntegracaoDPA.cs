using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Infrastructure.Services.HttpClientFactory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Servicos.Embarcador.Integracao.DPA
{
    public class IntegracaoDPA
    {
        #region Atributos Globais

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion Atributos Globais

        #region Construtores

        public IntegracaoDPA(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Métodos Públicos

        public void IntegrarCarga(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao)
        {
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repositorioCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoDPA repositorioConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.IntegracaoDPA(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoDPA configuracaoIntegracao = repositorioConfiguracaoIntegracao.Buscar();

            if (configuracaoIntegracao == null || string.IsNullOrWhiteSpace(configuracaoIntegracao.URLAutenticacaoDPA) || string.IsNullOrWhiteSpace(configuracaoIntegracao.URLIntegracaoDPA))
            {
                cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                cargaIntegracao.ProblemaIntegracao = "Não existe configuração de integração disponível para a DPA.";
            }
            else
            {
                cargaIntegracao.NumeroTentativas += 1;
                cargaIntegracao.DataIntegracao = DateTime.Now;

                string endPointAuth = configuracaoIntegracao.URLAutenticacaoDPA;
                string usuario = configuracaoIntegracao.UsuarioAutenticacaoDPA;
                string senha = configuracaoIntegracao.SenhaAutenticacaoDPA;
                string mensagemRetorno = string.Empty;

                string token = ObterToken(endPointAuth, usuario, senha, out mensagemRetorno);

                if (string.IsNullOrWhiteSpace(token))
                {
                    cargaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                    cargaIntegracao.ProblemaIntegracao = !string.IsNullOrWhiteSpace(mensagemRetorno) ? mensagemRetorno : "Autenticação DPA não retornou Token.";
                    return;
                }

                string endPointCTe = configuracaoIntegracao.URLIntegracaoDPA;

                if (!endPointCTe.EndsWith("/"))
                    endPointCTe += "/";

                endPointCTe = endPointCTe + "http/YBRPP/CTE/Auth";


                string clientRequestContent = string.Empty;
                string clientResponseContent = string.Empty;

                try
                {
                    if (!VerificarCargaParaIntegrar(cargaIntegracao.Carga))
                    {
                        cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                        cargaIntegracao.ProblemaIntegracao = "Carga parametrizada para não ser integrada";
                        repositorioCargaIntegracao.Atualizar(cargaIntegracao);
                    }
                    else
                    {
                        bool retorno = RetornarCarga(cargaIntegracao.Carga, ref cargaIntegracao, endPointCTe, token, ref clientRequestContent, ref clientResponseContent, out mensagemRetorno);

                        if (!retorno)
                        {
                            cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                            cargaIntegracao.ProblemaIntegracao = mensagemRetorno;
                        }
                        else
                        {
                            cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                            cargaIntegracao.ProblemaIntegracao = string.Empty;
                            repositorioCargaIntegracao.Atualizar(cargaIntegracao);
                        }
                    }
                }
                catch (Exception excecao)
                {
                    Servicos.Log.TratarErro(excecao, "IntegracaoDPA");
                    cargaIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao comunicar com o Web Service da DPA.";
                    cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                    Servicos.Log.TratarErro(JsonConvert.SerializeObject(clientRequestContent), "IntegracaoDPA");
                    Servicos.Log.TratarErro(JsonConvert.SerializeObject(clientResponseContent), "IntegracaoDPA");
                }
            }
            repositorioCargaIntegracao.Atualizar(cargaIntegracao);
        }

        public void CancelarCarga(Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaIntegracao cargaCancelamentoCargaIntegracao)
        {
            Repositorio.Embarcador.Cargas.CargaCancelamentoCargaIntegracao repCargaCancelamentoCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCancelamentoCargaIntegracao(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoDPA repositorioConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.IntegracaoDPA(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoDPA configuracaoIntegracao = repositorioConfiguracaoIntegracao.Buscar();

            if ((configuracaoIntegracao == null) || string.IsNullOrWhiteSpace(configuracaoIntegracao.URLIntegracaoDPA))
            {
                cargaCancelamentoCargaIntegracao.ProblemaIntegracao = "Configuração para integração com DPA inválida.";
                cargaCancelamentoCargaIntegracao.DataIntegracao = DateTime.Now;
                cargaCancelamentoCargaIntegracao.NumeroTentativas++;
                cargaCancelamentoCargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                repCargaCancelamentoCargaIntegracao.Atualizar(cargaCancelamentoCargaIntegracao);

                return;
            }

            string mensagem = string.Empty;
            bool retorno = false;
            List<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> arquivosIntegracao = null;

            int codigoCarga = cargaCancelamentoCargaIntegracao.CargaCancelamento.Carga.Codigo;
            string endPointAuth = configuracaoIntegracao.URLAutenticacaoDPA;
            string usuario = configuracaoIntegracao.UsuarioAutenticacaoDPA;
            string senha = configuracaoIntegracao.SenhaAutenticacaoDPA;

            string token = ObterToken(endPointAuth, usuario, senha, out mensagem);

            if (string.IsNullOrWhiteSpace(token))
            {
                cargaCancelamentoCargaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaCancelamentoCargaIntegracao.ProblemaIntegracao = !string.IsNullOrWhiteSpace(mensagem) ? mensagem : "Autenticação DPA não retornou Token.";
                repCargaCancelamentoCargaIntegracao.Atualizar(cargaCancelamentoCargaIntegracao);

                return;
            }

            if (!VerificarCargaParaIntegrar(cargaCancelamentoCargaIntegracao.CargaCancelamento.Carga))
            {
                cargaCancelamentoCargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                cargaCancelamentoCargaIntegracao.ProblemaIntegracao = "Carga parametrizada para não ser integrada";
                repCargaCancelamentoCargaIntegracao.Atualizar(cargaCancelamentoCargaIntegracao);

                return;
            }

            string endPointCancel = configuracaoIntegracao.URLIntegracaoDPA;

            if (!endPointCancel.EndsWith("/"))
                endPointCancel += "/";

            endPointCancel = endPointCancel + "http/CTE/Cancel";

            retorno = RetornarCancelamentoCarga(configuracaoIntegracao, out arquivosIntegracao, out mensagem, codigoCarga, endPointCancel, token);

            if (arquivosIntegracao != null && arquivosIntegracao.Count > 0)
            {
                foreach (var arquivoIntegracao in arquivosIntegracao)
                    cargaCancelamentoCargaIntegracao.ArquivosTransacao.Add(arquivoIntegracao);
            }

            cargaCancelamentoCargaIntegracao.ProblemaIntegracao = mensagem;
            cargaCancelamentoCargaIntegracao.DataIntegracao = DateTime.Now;
            cargaCancelamentoCargaIntegracao.NumeroTentativas++;

            if (retorno)
                cargaCancelamentoCargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
            else
                cargaCancelamentoCargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

            repCargaCancelamentoCargaIntegracao.Atualizar(cargaCancelamentoCargaIntegracao);
        }

        public void IntegrarCIOT(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao)
        {
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repositorioCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCIOT repCargaCIOT = new Repositorio.Embarcador.Cargas.CargaCIOT(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoDPA repositorioConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.IntegracaoDPA(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoDPA configuracaoIntegracao = repositorioConfiguracaoIntegracao.Buscar();

            string jsonRequisicao = string.Empty;
            string jsonRetorno = string.Empty;

            cargaIntegracao.NumeroTentativas += 1;
            cargaIntegracao.DataIntegracao = DateTime.Now;

            try
            {
                if (configuracaoIntegracao == null || string.IsNullOrWhiteSpace(configuracaoIntegracao.URLAutenticacaoDPACiot) || string.IsNullOrWhiteSpace(configuracaoIntegracao.URLIntegracaoDPACiot))
                    throw new ServicoException("Não existe configuração de integração disponível para a DPA CIOT.");

                string endPointAuth = configuracaoIntegracao.URLAutenticacaoDPACiot;
                string usuario = configuracaoIntegracao.UsuarioAutenticacaoDPACiot;
                string senha = configuracaoIntegracao.SenhaAutenticacaoDPACiot;
                string mensagemRetorno = string.Empty;

                string token = ObterToken(endPointAuth, usuario, senha, out mensagemRetorno);

                if (string.IsNullOrWhiteSpace(token))
                    throw new ServicoException(!string.IsNullOrWhiteSpace(mensagemRetorno) ? mensagemRetorno : "Autenticação DPA não retornou Token.");

                Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT = repCargaCIOT.BuscarPorCarga(cargaIntegracao.Carga.Codigo);

                if (cargaCIOT == null || cargaCIOT.CIOT == null || cargaCIOT.CIOT.Situacao == SituacaoCIOT.Pendencia)
                {
                    cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                    cargaIntegracao.ProblemaIntegracao = "Carga não possui CIOT valido.";
                }
                else
                {
                    HttpClient client = CriarRequisicao(token);
                    Dominio.ObjetosDeValor.Embarcador.Integracao.DPA.Dados dados = ObterDadosIntegracaoCIOT(cargaCIOT);
                    jsonRequisicao = JsonConvert.SerializeObject(dados, Formatting.Indented);
                    StringContent conteudoRequisicao = new StringContent(jsonRequisicao.ToString(), Encoding.UTF8, "application/json");

                    HttpResponseMessage retornoRequisicao = client.PostAsync(configuracaoIntegracao.URLIntegracaoDPACiot + "/webhook/multitms/ciot", conteudoRequisicao).Result;
                    jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;
                    Dominio.ObjetosDeValor.Embarcador.Integracao.DPA.RetornoIntegracaoCiot retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.DPA.RetornoIntegracaoCiot>(jsonRetorno);

                    if (retornoRequisicao.StatusCode == HttpStatusCode.OK)
                    {
                        cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                        cargaIntegracao.ProblemaIntegracao = "Integração realizada com sucesso";
                    }
                    else
                    {
                        cargaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                        cargaIntegracao.ProblemaIntegracao = retorno.Mensagem;
                    }

                    servicoArquivoTransacao.Adicionar(cargaIntegracao, jsonRequisicao, jsonRetorno, "json");
                }
            }
            catch (ServicoException excecao)
            {
                cargaIntegracao.ProblemaIntegracao = excecao.Message;
                cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao, "IntegracaoDPA");
                cargaIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao comunicar com o Web Service da DPA.";
                cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                Servicos.Log.TratarErro(JsonConvert.SerializeObject(jsonRequisicao), "IntegracaoDPA");
                Servicos.Log.TratarErro(JsonConvert.SerializeObject(jsonRetorno), "IntegracaoDPA");
            }

            repositorioCargaIntegracao.Atualizar(cargaIntegracao);
        }

        #endregion

        #region Métodos Privados
        private HttpClient CriarRequisicao(string token)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoDPA));
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            return client;
        }

        private string ObterToken(string url, string user, string password, out string msgRetorno)
        {
            msgRetorno = "";
            try
            {
                var _client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoDPA));
                var authValue = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{user}:{password}"));
                _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", authValue);

                var parameters = new FormUrlEncodedContent(new[] { new KeyValuePair<string, string>("grant_type", "client_credentials"), });

                var response = _client.PostAsync(url, parameters).Result;

                if (response.IsSuccessStatusCode)
                {
                    var jsonObject = JObject.Parse(response.Content.ReadAsStringAsync().Result);
                    var token = jsonObject["access_token"]?.ToString();
                    return token;
                }
                else
                    return "";
            }
            catch (Exception e)
            {
                msgRetorno = e.Message;
                Servicos.Log.TratarErro(e, "IntegracaoDPA");
                return "";
            }
        }

        private bool RetornarCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, ref Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao, string endPoint, string token, ref string xmlRequest, ref string xmlResponse, out string erro)
        {
            try
            {
                erro = "";
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
                Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(_unitOfWork);

                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> listaCargaPedidos = repCargaPedido.BuscarPorCarga(carga.Codigo);

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in listaCargaPedidos)
                {
                    HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoDPA));
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                    Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe = repCargaCTe.BuscarPorCargaPedido(cargaPedido.Codigo);

                    xmlRequest = string.Empty;
                    xmlResponse = string.Empty;

                    if (cargaCTe != null)
                    {
                        try
                        {
                            xmlRequest = ObterXMLIntegracaoCTe(cargaCTe);

                            Servicos.Log.TratarErro("Request " + xmlRequest, "IntegracaoDPA");
                            var content = new StringContent(xmlRequest, Encoding.UTF8, "application/xml");
                            var result = client.PostAsync(endPoint, content).Result;
                            xmlResponse = result.Content.ReadAsStringAsync().Result;
                            Servicos.Log.TratarErro("Response Integrar CTe " + xmlResponse, "IntegracaoDPA");

                            if (result.IsSuccessStatusCode)
                            {
                                erro = string.Empty;
                                SalvarHistoricoIntegracao(ref cargaIntegracao, erro, xmlRequest, xmlResponse); ;
                            }
                            else
                            {
                                Servicos.Log.TratarErro("Retorno DPA: " + result.StatusCode.ToString(), "IntegracaoDPA");
                                erro = "Retorno DPA: " + result.StatusCode.ToString();
                                SalvarHistoricoIntegracao(ref cargaIntegracao, erro, xmlRequest, xmlResponse);

                                return false;
                            }
                        }
                        catch (Exception ex)
                        {
                            Servicos.Log.TratarErro(ex, "IntegracaoDPA");
                            erro = "Integração DPA não retornou sucesso.";
                            SalvarHistoricoIntegracao(ref cargaIntegracao, erro, xmlRequest, xmlResponse);

                            return false;
                        }
                    }
                    else
                    {
                        if (cargaPedido.PossuiNFSManual)
                        {
                            try
                            {
                                xmlRequest = ObterXMLIntegracaoNFSManual(cargaPedido);

                                Servicos.Log.TratarErro("Request " + xmlRequest, "IntegracaoDPA");
                                var content = new StringContent(xmlRequest, Encoding.UTF8, "application/xml");
                                var result = client.PostAsync(endPoint, content).Result;
                                xmlResponse = result.Content.ReadAsStringAsync().Result;
                                Servicos.Log.TratarErro("Response Integrar CTe " + xmlResponse, "IntegracaoDPA");

                                if (result.IsSuccessStatusCode)
                                {
                                    erro = string.Empty;
                                    SalvarHistoricoIntegracao(ref cargaIntegracao, erro, xmlRequest, xmlResponse);
                                }
                                else
                                {
                                    Servicos.Log.TratarErro("Retorno DPA: " + result.StatusCode.ToString(), "IntegracaoDPA");
                                    erro = "Retorno DPA: " + result.StatusCode.ToString();
                                    SalvarHistoricoIntegracao(ref cargaIntegracao, erro, xmlRequest, xmlResponse);

                                    return false;
                                }
                            }
                            catch (Exception ex)
                            {
                                Servicos.Log.TratarErro(ex, "IntegracaoDPA");
                                erro = "Integração DPA não retornou sucesso.";
                                SalvarHistoricoIntegracao(ref cargaIntegracao, erro, xmlRequest, xmlResponse);

                                return false;
                            }
                        }
                        else
                        {
                            Servicos.Log.TratarErro("Carga pedido " + cargaPedido.Codigo.ToString() + " sem CTe.", "IntegracaoMagalog");
                        }
                    }
                }

                return true;
            }
            catch (Exception e)
            {
                Servicos.Log.TratarErro(e, "IntegracaoDPA");
                erro = "Erro genérico ao enviar requisição para WebService.";
                return false;
            }
        }

        private bool RetornarCancelamentoCarga(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoDPA configuracaoIntegracao, out List<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> arquivosIntegracao, out string mensagem, int codigoCarga, string endPoint, string token)
        {
            InspectorBehavior inspector = new InspectorBehavior();
            Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(_unitOfWork);
            arquivosIntegracao = new List<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(); ;

            bool retorno = false;
            string xmlRequest = "";
            string xmlResponse = "";
            mensagem = "";

            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(_unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);

                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> listaCargaPedidos = repCargaPedido.BuscarPorCarga(codigoCarga);

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in listaCargaPedidos)
                {
                    HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoDPA));
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                    Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe = repCargaCTe.BuscarPorCargaPedido(cargaPedido.Codigo);

                    xmlRequest = string.Empty;
                    xmlResponse = string.Empty;

                    if (cargaCTe != null)
                    {
                        try
                        {
                            xmlRequest = ObterXMLCancelamentoCTe(configuracaoIntegracao, cargaCTe);

                            Servicos.Log.TratarErro("Request " + xmlRequest, "IntegracaoDPA");
                            var content = new StringContent(xmlRequest, Encoding.UTF8, "application/xml");
                            var result = client.PostAsync(endPoint, content).Result;
                            xmlResponse = result.Content.ReadAsStringAsync().Result;
                            Servicos.Log.TratarErro("Response Integrar CTe " + xmlResponse, "IntegracaoDPA");

                            if (!string.IsNullOrWhiteSpace(xmlRequest) || !string.IsNullOrWhiteSpace(xmlResponse))
                            {
                                var arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo
                                {
                                    ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(xmlRequest, "xml", _unitOfWork),
                                    ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(xmlResponse, "xml", _unitOfWork),
                                    Data = DateTime.Now,
                                    Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento,
                                    Mensagem = mensagem
                                };

                                repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

                                arquivosIntegracao.Add(arquivoIntegracao);
                            }

                            if (!result.IsSuccessStatusCode)
                            {
                                Servicos.Log.TratarErro("Retorno DPA: " + result.StatusCode.ToString(), "IntegracaoDPA");

                                mensagem = "Retorno DPA: " + result.StatusCode.ToString();
                                return false;
                            }
                        }
                        catch (Exception ex)
                        {
                            Servicos.Log.TratarErro(ex, "IntegracaoDPA");

                            mensagem = "Integração DPA não retornou sucesso.";
                            return false;
                        }
                    }
                }

                mensagem = string.Empty;
                return true;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("RetonarCancelamentoCarga: " + ex, "IntegracaoMinerva");
                mensagem = "Falha ao enviar o cancelamento da carga protocolo " + codigoCarga;
                retorno = false;
            }

            return retorno;
        }

        private string ObterXMLIntegracaoCTe(Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe)
        {
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(_unitOfWork);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(_unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTeSubcontratado = null;
            bool possuiFilialEmissora = cargaCTe.Carga.EmpresaFilialEmissora != null;

            if (possuiFilialEmissora)
                cargaCTeSubcontratado = repCargaCTe.BuscarPorCodigoCargaCTeFilialEmissora(cargaCTe.Codigo);

            string tipoCarga = possuiFilialEmissora ? "D" : "C";

            Dominio.Entidades.ConhecimentoDeTransporteEletronico cteOriginal = null;
            if (!possuiFilialEmissora && cargaCTe.CTe.TipoServico == Dominio.Enumeradores.TipoServico.SubContratacao)
                cteOriginal = repCTe.BuscarPorChave(cargaCTe.CTe.ChaveCTeAnterior);

            string xml = string.Empty;
            xml = $@"<CTE>
                        <HEADER>
                            <Codempresa>BR24</Codempresa>
                            <Centro>{cargaCTe.Carga.Filial?.CodigoFilialEmbarcador ?? "0"}</Centro>
                            <NrCarga>{cargaCTe.Carga.CodigoCargaEmbarcador}</NrCarga>   
                            <TipoCarga>{tipoCarga}</TipoCarga>
                            <CTETranspIDF>{(possuiFilialEmissora ? cargaCTeSubcontratado.CTe.Empresa.CNPJ : cargaCTe.CTe.Empresa.CNPJ)}</CTETranspIDF>

                            <CTEClienteCNPJRemetente>{(possuiFilialEmissora ? cargaCTe.CTe.Remetente?.CPF_CNPJ ?? "" : cteOriginal?.Remetente?.CPF_CNPJ ?? "")}</CTEClienteCNPJRemetente> 
                            <CTEClienteCNPJTomador>{(possuiFilialEmissora ? cargaCTe.CTe.Tomador?.CPF_CNPJ ?? "" : cteOriginal?.Tomador?.CPF_CNPJ ?? "")}</CTEClienteCNPJTomador>
                            <CTEClienteNR>{(possuiFilialEmissora ? cargaCTe.CTe.Numero.ToString() ?? "" : cteOriginal?.Numero.ToString() ?? "")}</CTEClienteNR>
                            <CTEClienteSR>{(possuiFilialEmissora ? cargaCTe.CTe.Serie.Numero.ToString() ?? "" : cteOriginal?.Serie.Numero.ToString() ?? "")}</CTEClienteSR>
                            <CTEClienteData>{(possuiFilialEmissora ? cargaCTe.CTe.DataEmissao.Value.ToString("yyyy-MM-dd") ?? "" : cteOriginal?.DataEmissao.Value.ToString("yyyy-MM-dd") ?? "")}</CTEClienteData>
                            <CTEClienteKey>{(possuiFilialEmissora ? cargaCTe.CTe.Chave ?? "" : cteOriginal?.Chave ?? "")}</CTEClienteKey>
                            <CTEClienteCFOP>{(possuiFilialEmissora ? cargaCTe.CTe.CFOP.CodigoCFOP.ToString() + "AA" ?? "" : cteOriginal != null ? cteOriginal.CFOP.CodigoCFOP.ToString() + "AA" : "")}</CTEClienteCFOP>

                            <CTETranspCNPJRemetente>{(possuiFilialEmissora ? cargaCTeSubcontratado.CTe.Remetente?.CPF_CNPJ : cargaCTe.CTe.Remetente?.CPF_CNPJ ?? string.Empty)}</CTETranspCNPJRemetente>
                            <CTETranspCNPJTomador>{(possuiFilialEmissora ? cargaCTeSubcontratado.CTe.TomadorPagador?.CPF_CNPJ : cargaCTe.CTe.TomadorPagador?.CPF_CNPJ ?? string.Empty)}</CTETranspCNPJTomador>
                            <CTETranspNR>{(possuiFilialEmissora ? cargaCTeSubcontratado.CTe.Numero : cargaCTe.CTe.Numero)}</CTETranspNR>
                            <CTETranspSR>{(possuiFilialEmissora ? cargaCTeSubcontratado.CTe.Serie.Numero : cargaCTe.CTe.Serie.Numero)}</CTETranspSR>
                            <CTETranspData>{(possuiFilialEmissora ? cargaCTeSubcontratado.CTe.DataEmissao.Value.ToString("yyyy-MM-dd") : cargaCTe.CTe.DataEmissao.Value.ToString("yyyy-MM-dd"))}</CTETranspData>
                            <CTETranspKey>{(possuiFilialEmissora ? cargaCTeSubcontratado.CTe.Chave : cargaCTe.CTe.Chave)}</CTETranspKey>
                            <CTETranspValorTotal>{FormatarValor(possuiFilialEmissora ? cargaCTeSubcontratado.CTe.ValorAReceber : cargaCTe.CTe.ValorAReceber)}</CTETranspValorTotal>
                            <CTETranspCFOP>{(possuiFilialEmissora ? cargaCTeSubcontratado.CTe.CFOP.CodigoCFOP : cargaCTe.CTe.CFOP.CodigoCFOP)}AA</CTETranspCFOP>    
                            <CidadeOrig>{(possuiFilialEmissora ? cargaCTeSubcontratado.CTe.LocalidadeInicioPrestacao.Descricao : cargaCTe.CTe.LocalidadeInicioPrestacao.Descricao)}</CidadeOrig>
                            <EstadoOrig>{(possuiFilialEmissora ? cargaCTeSubcontratado.CTe.LocalidadeTerminoPrestacao.Estado.Sigla : cargaCTe.CTe.LocalidadeInicioPrestacao.Estado.Sigla)}</EstadoOrig>
                            <CEPOrig>{(possuiFilialEmissora ? FormatarCEP(cargaCTeSubcontratado.CTe.Remetente?.CEP) : FormatarCEP(cargaCTe.CTe.Remetente.CEP_SemFormato))}</CEPOrig>
                            <CidadeDest>{(possuiFilialEmissora ? cargaCTeSubcontratado.CTe.LocalidadeTerminoPrestacao.Descricao : cargaCTe.CTe.LocalidadeTerminoPrestacao.Descricao)}</CidadeDest>
                            <EstadoDest>{(possuiFilialEmissora ? cargaCTeSubcontratado.CTe.LocalidadeTerminoPrestacao.Estado.Sigla : cargaCTe.CTe.LocalidadeTerminoPrestacao.Estado.Sigla)}</EstadoDest>
                            <CEPDest>{(possuiFilialEmissora ? FormatarCEP(cargaCTeSubcontratado.CTe.Destinatario?.CEP_SemFormato) : FormatarCEP(cargaCTe.CTe.Destinatario.CEP_SemFormato))}</CEPDest>

                            <Moeda>BRL</Moeda>
                            <ValorTotalFrete>{FormatarValor(possuiFilialEmissora ? cargaCTe.CTe.ValorAReceber : cteOriginal?.ValorAReceber ?? 0)}</ValorTotalFrete>
                            <BaseCalculoImp>{FormatarValor(possuiFilialEmissora ? cargaCTe.CTe.BaseCalculoICMS : cteOriginal?.BaseCalculoICMS ?? 0)}</BaseCalculoImp>
                            <ValorTotalICMS>{FormatarValor(possuiFilialEmissora ? cargaCTe.CTe.ValorICMS : cteOriginal?.ValorICMS ?? 0)}</ValorTotalICMS>
                            <AliquotaICMS>{FormatarValor(possuiFilialEmissora ? cargaCTe.CTe.AliquotaICMS : cteOriginal?.AliquotaICMS ?? 0)}</AliquotaICMS>
                            <Obs>{cargaCTe.CTe.ObservacoesGerais}</Obs>
                            <Status>00</Status>
                          </HEADER>";

            int quantidadeDocumentos = 1;

            foreach (var nota in cargaCTe.CTe.Documentos)
            {
                xml = xml +
                       $@"<ITEM>
                            <Item>{quantidadeDocumentos}</Item>
                            <NfeNumber>{nota.NumeroOuNumeroDaChave}</NfeNumber>
                            <NFeSerie>{nota.SerieOuSerieDaChave.PadLeft(3, '0')}</NFeSerie>
                            <NFeKey>{(!string.IsNullOrWhiteSpace(nota.ChaveNFE) ? nota.ChaveNFE : nota.Descricao)}</NFeKey>
                            <Moeda>BRL</Moeda>
                            <ValorMercadoria>{FormatarValor(nota.Valor)}</ValorMercadoria>
                          </ITEM>";
                quantidadeDocumentos++;
            }


            xml = xml + "</CTE>";

            return xml;
        }

        private string ObterXMLIntegracaoNFSManual(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido)
        {
            Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual repCargaDocumentoParaEmissaoNFSManual = new Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual documentoNFSeManual = repCargaDocumentoParaEmissaoNFSManual.BuscarPorCargaPedido(cargaPedido.Codigo);

            bool possuiFilialEmissora = cargaPedido.Carga.EmpresaFilialEmissora != null;
            string tipoCarga = possuiFilialEmissora ? "D" : "C";

            string xml = string.Empty;
            xml = $@"<CTE>
                        <HEADER>
                            <Codempresa>BR24</Codempresa>
                            <Centro>{cargaPedido.Carga.Filial?.CodigoFilialEmbarcador ?? "0"}</Centro>
                            <NrCarga>{cargaPedido.Carga.CodigoCargaEmbarcador}</NrCarga>   
                            <TipoCarga>{tipoCarga}</TipoCarga>
                            <CTETranspIDF>{cargaPedido.Carga.Empresa.CNPJ}</CTETranspIDF>
                            <CTEClienteCNPJRemetente></CTEClienteCNPJRemetente> 
                            <CTEClienteCNPJTomador></CTEClienteCNPJTomador>
                            <CTEClienteNR></CTEClienteNR>
                            <CTEClienteSR></CTEClienteSR>
                            <CTEClienteData></CTEClienteData>
                            <CTEClienteKey></CTEClienteKey>
                            <CTEClienteCFOP></CTEClienteCFOP>
                            <CTETranspCNPJRemetente>{cargaPedido.Pedido.Remetente?.CPF_CNPJ_SemFormato ?? string.Empty}</CTETranspCNPJRemetente>
                            <CTETranspCNPJTomador>{cargaPedido.Pedido.Destinatario?.CPF_CNPJ_SemFormato ?? string.Empty}</CTETranspCNPJTomador>
                            <CTETranspNR>{documentoNFSeManual?.Numero}</CTETranspNR>
                            <CTETranspSR>{documentoNFSeManual?.Serie}</CTETranspSR>
                            <CTETranspData>{documentoNFSeManual?.DataEmissao.ToString("yyyy-MM-dd")}</CTETranspData>
                            <CTETranspKey></CTETranspKey>
                            <CTETranspValorTotal>{FormatarValor(documentoNFSeManual?.ValorFrete ?? 0)}</CTETranspValorTotal>
                            <CTETranspCFOP></CTETranspCFOP>    
                            <CidadeOrig>{cargaPedido.Origem.Descricao}</CidadeOrig>
                            <EstadoOrig>{cargaPedido.Origem.Estado.Sigla}</EstadoOrig>
                            <CEPOrig>{FormatarCEP(cargaPedido.Pedido.Remetente.CEP)}</CEPOrig>
                            <CidadeDest>{cargaPedido.Destino.Descricao}</CidadeDest>
                            <EstadoDest>{cargaPedido.Destino.Estado.Sigla}</EstadoDest>
                            <CEPDest>{FormatarCEP(cargaPedido.Pedido.Destinatario.CEP)}</CEPDest>
                            <Moeda>BRL</Moeda>
                            <ValorTotalFrete>{FormatarValor(documentoNFSeManual?.ValorFrete ?? 0)}</ValorTotalFrete>
                            <BaseCalculoImp>{FormatarValor(documentoNFSeManual?.BaseCalculoISS ?? 0)}</BaseCalculoImp>
                            <ValorTotalICMS>{FormatarValor(documentoNFSeManual?.ValorISS ?? 0)}</ValorTotalICMS>
                            <AliquotaICMS></AliquotaICMS>
                            <Obs></Obs>
                            <Status>00</Status>
                          </HEADER>";

            xml = xml +
                   $@"<ITEM>
                            <Item>1</Item>
                            <NfeNumber>{documentoNFSeManual?.Numero.ToString() ?? ""}</NfeNumber>
                            <NFeSerie>{documentoNFSeManual?.Serie ?? ""}</NFeSerie>
                            <NFeKey>{documentoNFSeManual?.Chave ?? ""}</NFeKey>
                            <Moeda>BRL</Moeda>
                            <ValorMercadoria>{FormatarValor(documentoNFSeManual?.PedidoXMLNotaFiscal?.XMLNotaFiscal?.Valor ?? 0)}</ValorMercadoria>
                          </ITEM>";

            xml = xml + "</CTE>";

            return xml;
        }

        private string ObterXMLCancelamentoCTe(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoDPA configuracaoIntegracao, Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe)
        {
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(_unitOfWork);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(_unitOfWork);

            int quantidadeDocumentos = 0;

            Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = cargaCTe.CTe;
            if (cargaCTe.CTe.TipoServico == Dominio.Enumeradores.TipoServico.SubContratacao)
                cte = repCTe.BuscarPorChave(cargaCTe.CTe.ChaveCTeAnterior);

            string xmlNotas = string.Empty;
            foreach (var nota in cte.Documentos)
            {
                string chaveNFe = !string.IsNullOrWhiteSpace(nota.ChaveNFE) ? nota.ChaveNFE : nota.Descricao;

                if (Utilidades.Validate.ValidarChaveNFe(chaveNFe) && chaveNFe.Length == 44)
                {
                    xmlNotas = xmlNotas +
                           $@"<MESSAGE{5 + quantidadeDocumentos}>{chaveNFe.Substring(0, 2)};{chaveNFe.Substring(2, 2)};{chaveNFe.Substring(4, 2)};{chaveNFe.Substring(6, 14)};{chaveNFe.Substring(20, 2)};{chaveNFe.Substring(22, 3)};{chaveNFe.Substring(25, 9)};{chaveNFe.Substring(34, 1)};{chaveNFe.Substring(35, 8)};{chaveNFe.Substring(43, 1)};{FormatarValor(nota.Valor)}</MESSAGE{5 + quantidadeDocumentos}>";
                    quantidadeDocumentos++;
                }
            }

            string xml = string.Empty;
            xml = $@"<NF>
	                     <HEADER>
	                     		<NFTYPE>JZ</NFTYPE>
	                     		<DOCUMENTTYPE>4</DOCUMENTTYPE>
	                     		<DIRECTION>2</DIRECTION>
	                     		<DOCDATE>{cte.DataEmissao.Value.ToString("yyyyMMdd") ?? ""}</DOCDATE>
	                     		<PSTDATE>{cte.DataEmissao.Value.ToString("yyyyMMdd") ?? ""}</PSTDATE>
	                     		<MODEL>57</MODEL>
	                     		<MANUAL>X</MANUAL>
	                     		<CURRENCY>BRL</CURRENCY>
	                     		<COMPANYCODE>BR24</COMPANYCODE>
	                     		<BUSINESSPLACE>0032</BUSINESSPLACE>
	                     		<PARTNERFUNCTION>AG</PARTNERFUNCTION>
	                     		<CUSTOMER>583702</CUSTOMER>
	                     		<COSTUMERTYPE>C</COSTUMERTYPE>
	                     		<CANCEL>X</CANCEL>
	                     		<INCOTERM1>CIF</INCOTERM1>
	                     		<INCOTERM2>CIF</INCOTERM2>
	                     		<CF-eAMOUNT>{FormatarValor(cte.ValorAReceber)}</CF-eAMOUNT>
	                     		<NFE>X</NFE>
	                     		<CF-eNUMBER>{cte.Numero}</CF-eNUMBER>
	                     		<SERIE>{cte.Serie.Numero}</SERIE>
	                     		<STATUS>02</STATUS>
	                     	</HEADER>
	                     	<ITEM>
	                     		<ITEMNUMBER>10</ITEMNUMBER>
	                     		<ITEMTYPE>1</ITEMTYPE>
	                     		<MATERIAL>{configuracaoIntegracao.Material}</MATERIAL>
	                     		<PLANT>{cargaCTe.Carga.Filial?.CodigoFilialEmbarcador ?? "0"}</PLANT>
	                     		<MATERIALGROUP>{configuracaoIntegracao.MaterialGroup}</MATERIALGROUP>
	                     		<DESCRIPTION>{configuracaoIntegracao.Description}</DESCRIPTION>
	                     		<NCM>99</NCM>
	                     		<MATERIALORIGIN>0</MATERIALORIGIN>
	                     		<TAXSITUATIONICMS>00</TAXSITUATIONICMS>
	                     		<TAXLAWICMS>IC0</TAXLAWICMS>
	                     		<TAXSITUATIONIPI>53</TAXSITUATIONIPI>
	                     		<TAXLAWIPI>Z47</TAXLAWIPI>
	                     		<TAXSITUATIONCOFINS>01</TAXSITUATIONCOFINS>
	                     		<TAXLAWCOFINS>C01</TAXLAWCOFINS>
	                     		<TAXSITUATIONPIS>01</TAXSITUATIONPIS>
	                     		<TAXLAWPIS>P01</TAXLAWPIS>
	                     		<MATERIALUSAGE>0</MATERIALUSAGE>
	                     		<QUANTITY>1</QUANTITY>
	                     		<UNIT>EA</UNIT>
	                     		<UNITVALUE>{FormatarValor(cte.ValorAReceber)}</UNITVALUE>
	                     		<TOTALVALUE>{FormatarValor(cte.ValorAReceber)}</TOTALVALUE>
	                     		<DISCOUNT>0</DISCOUNT>
	                     		<INCLUDINGTAX>X</INCLUDINGTAX>
	                     		<CFOP>{cte.CFOP.CodigoCFOP.ToString() + "AA"}</CFOP>
	                     		<ICMS>
	                     			<TAXTYPEICMS>ICM1</TAXTYPEICMS>
	                     			<BASEAMOUNT>{FormatarValor(cte.BaseCalculoICMS)}</BASEAMOUNT>
	                     			<OTHERBASE>0</OTHERBASE>
	                     			<TAXRATE>{FormatarValor(cte.AliquotaICMS)}</TAXRATE>
	                     			<TAXVALUE>{FormatarValor(cte.ValorICMS)}</TAXVALUE>
	                     		</ICMS>
	                     		<PIS>
	                     			<TAXTYPEPIS>IPIS</TAXTYPEPIS>
	                     			<BASEAMOUNT>0.00</BASEAMOUNT>
	                     			<TAXRATE>0.00</TAXRATE>
	                     			<TAXVALUE>0.00</TAXVALUE>
	                     		</PIS>
	                     		<COFINS>
	                     			<TAXTYPECOFINS>ICOF</TAXTYPECOFINS>
	                     			<BASEAMOUNT>0.00</BASEAMOUNT>
	                     			<TAXRATE>0.00</TAXRATE>
	                     			<TAXVALUE>0.00</TAXVALUE>
	                     		</COFINS>
	                     	</ITEM>
	                     	<MESSAGE>
	                     		<MESSAGE1>;{cte.Empresa.CNPJ};{cte.Protocolo};0;{cte.DataAutorizacao.Value.ToString("ddMMyyyy")};{cte.DataAutorizacao.Value.ToString("HHmmss")};</MESSAGE1>
	                     		<MESSAGE2>1;{cte.Empresa.Localidade.Estado.CodigoIBGE};{cte.DataAutorizacao.Value.ToString("yy")};{cte.DataAutorizacao.Value.ToString("MM")};{cte.Empresa.CNPJ};{string.Format("{0:000}", cte.Serie.Numero)};{string.Format("{0:000000000}", cte.Numero)};{(!string.IsNullOrWhiteSpace(cte.Chave) ? cte.Chave.Substring(34, 1) : "1")};</MESSAGE2>
	                     		<MESSAGE3>{(!string.IsNullOrWhiteSpace(cte.Chave) ? cte.Chave.Substring(35, 8) : "")};{(!string.IsNullOrWhiteSpace(cte.Chave) ? cte.Chave.Substring(43, 1) : "")};2;000000000;0000000000</MESSAGE3>
	                     	    <MESSAGE4>{cte.LocalidadeInicioPrestacao.Descricao};{cte.LocalidadeInicioPrestacao.Estado.Sigla};{FormatarCEP(cte.Remetente.CEP_SemFormato)};{cte.LocalidadeTerminoPrestacao.Descricao};{cte.LocalidadeTerminoPrestacao.Estado.Sigla};{FormatarCEP(cte.Destinatario.CEP_SemFormato)}</MESSAGE4>
                                {xmlNotas}
                            </MESSAGE>
	                     </NF>";

            return xml;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.DPA.Dados ObterDadosIntegracaoCIOT(Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT)
        {
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> listaCargaPedidos = repCargaPedido.BuscarPorCarga(cargaCIOT.Carga.Codigo);
            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes = repCargaCTe.BuscarCargaCTePorCarga(cargaCIOT.Carga.Codigo);
            Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidadeTerceiro = ObterModalidadeTransportador(cargaCIOT.CIOT.Transportador);


            List<Dominio.ObjetosDeValor.Embarcador.Integracao.DPA.Item> itens = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.DPA.Item>();
            Dominio.ObjetosDeValor.Embarcador.Integracao.DPA.Dados dadosIntegracaoCiot = new Dominio.ObjetosDeValor.Embarcador.Integracao.DPA.Dados()
            {
                CodigoEmpresa = "BR24",
                CPFCNPJEmpresa = cargaCIOT.CIOT.Transportador.CPF_CNPJ_SemFormato,
                CNPJContratante = cargaCIOT.CIOT.Contratante.CNPJ,
                NumeroCIOT = cargaCIOT.CIOT.Numero,
                DataAberturaCIOT = cargaCIOT.CIOT.DataAbertura.Value.ToString("yyyy-MM-dd"),
                CodigoCargaEmbarcador = cargaCIOT.Carga.CodigoCargaEmbarcador,
                TipoCarga = "A",
                CodigoFilialEmbarcador = cargaCIOT.Carga.Filial?.CodigoFilialEmbarcador,
                Moeda = "BRL",
                ValorFreteSubcontratacao = cargaCIOT.ContratoFrete.ValorFreteSubcontratacao,
                BaseCalculoIRRF = cargaCIOT.ContratoFrete.BaseCalculoIRRF,
                ValorIRRF = cargaCIOT.ContratoFrete.ValorIRRF,
                AliquotaIRRF = cargaCIOT.ContratoFrete.AliquotaIRRF,
                BaseCalculoRetencao = (cargaCIOT.ContratoFrete.ValorFreteSubcontratacao * (decimal)0.20),
                NumeroDependentesIRRF = modalidadeTerceiro?.QuantidadeDependentes?.ToString() ?? string.Empty,
                ValorINSS = cargaCIOT.ContratoFrete.ValorINSS,
                AliquotaINSS = cargaCIOT.ContratoFrete.AliquotaINSS,
                ValorSENAT = cargaCIOT.ContratoFrete.ValorSENAT,
                AliquotaSENAT = cargaCIOT.ContratoFrete.AliquotaSENAT,
                ValorSEST = cargaCIOT.ContratoFrete.ValorSEST,
                AliquotaSEST = cargaCIOT.ContratoFrete.AliquotaSEST,
            };

            int quantidadeDocumentos = 1;

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in listaCargaPedidos)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe = cargaCTes.Find(cargaCTe => cargaCTe.NotasFiscais.Any(nf => nf.PedidoXMLNotaFiscal.CargaPedido.Codigo == cargaPedido.Codigo));

                string[] numerosPedido = cargaPedido.Pedido.NumeroPedidoEmbarcador.Split('_');
                string numeroPedidoDPA = numerosPedido.Length > 1 ? numerosPedido[1].ToString() : cargaPedido.Pedido.NumeroPedidoEmbarcador;

                string[] numerosNotas = !string.IsNullOrWhiteSpace(cargaCTe.CTe.NumeroNotas) ? cargaCTe.CTe.NumeroNotas.Split(',').Select(n => n.Trim()).ToArray() : Array.Empty<string>();
                string numerosNota = string.Join("/", numerosNotas);

                itens.Add(new Dominio.ObjetosDeValor.Embarcador.Integracao.DPA.Item()
                {
                    QuantidadeDocumento = quantidadeDocumentos,
                    NumeroCIOT = cargaCIOT.CIOT.Numero,
                    CidadeOrigem = cargaCTe.CTe.LocalidadeInicioPrestacao.Descricao,
                    EstadoOrigem = cargaCTe.CTe.LocalidadeInicioPrestacao.Estado.Sigla,
                    CidadeDestino = cargaCTe.CTe.LocalidadeTerminoPrestacao.Descricao,
                    EstadoDestino = cargaCTe.CTe.LocalidadeTerminoPrestacao.Estado.Sigla,
                    NumeroNFE = numerosNota,
                    PedidoFrete = numeroPedidoDPA,
                    Moeda = "BRL",
                    ValorFrete = cargaCTe.CTe.ValorFrete
                });

                quantidadeDocumentos++;
            }

            dadosIntegracaoCiot.Itens = itens;
            return dadosIntegracaoCiot;
        }

        private void SalvarHistoricoIntegracao(ref Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao, string mensagem, string jsonRequest, string jsonResponse)
        {
            Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(_unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo();
            arquivoIntegracao.Data = cargaIntegracao.DataIntegracao;
            arquivoIntegracao.Mensagem = mensagem;
            arquivoIntegracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;

            arquivoIntegracao.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonRequest, "xml", _unitOfWork);
            arquivoIntegracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonResponse, "xml", _unitOfWork);

            repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

            cargaIntegracao.ArquivosTransacao.Add(arquivoIntegracao);
        }

        private string FormatarValor(decimal valor)
        {
            string valorString = string.Format("{0:n2}", valor);

            return valorString.Replace(".", "").Replace(",", ".");
        }

        private string FormatarCEP(string cep)
        {
            if (string.IsNullOrWhiteSpace(cep))
                return string.Empty;

            return Convert.ToUInt64(cep).ToString(@"00000\-000");
        }

        private bool VerificarCargaParaIntegrar(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            //Só pode ser retornado cargas com filial emissora ou cargas que possem CTe de subcontratação (feito assim pois no DPA utilizam mesmo tipo de operação e não podem mudar)

            if (carga.EmpresaFilialEmissora != null)
                return true;

            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargasCTe = repCargaCTe.BuscarPorCarga(carga.Codigo);

            return cargasCTe != null && cargasCTe.Exists(o => o.CTe.TipoServico == Dominio.Enumeradores.TipoServico.SubContratacao);
        }

        private Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas ObterModalidadeTransportador(Dominio.Entidades.Cliente terceiro)
        {
            if (terceiro == null)
                return null;

            Repositorio.Embarcador.Pessoas.ModalidadeTransportadoraPessoas repModalidadeTerceiro = new Repositorio.Embarcador.Pessoas.ModalidadeTransportadoraPessoas(_unitOfWork);
            Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidadeTerceiro = repModalidadeTerceiro.BuscarPorPessoa(terceiro.CPF_CNPJ);
            return modalidadeTerceiro;
        }

        #endregion
    }
}
