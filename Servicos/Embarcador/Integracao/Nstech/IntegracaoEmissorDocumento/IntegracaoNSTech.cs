using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento;
using Infrastructure.Services.HttpClientFactory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using Utilidades.Extensions;

namespace Servicos.Embarcador.Integracao.Nstech.IntegracaoEmissorDocumento
{
    public partial class IntegracaoNSTech
    {
        #region Atributos

        private Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoIntegracaoEmissorDocumento _configuracaoIntegracaoEmissorDocumento = null;

        #endregion

        #region Contrutores

        public IntegracaoNSTech(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoIntegracaoEmissorDocumento repConfiguracaoIntegracaoEmissorDocumento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoIntegracaoEmissorDocumento(unitOfWork);
            _configuracaoIntegracaoEmissorDocumento = repConfiguracaoIntegracaoEmissorDocumento.BuscarConfiguracaoPadrao();
        }

        public IntegracaoNSTech(string stringConexao)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoIntegracaoEmissorDocumento repConfiguracaoIntegracaoEmissorDocumento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoIntegracaoEmissorDocumento(unitOfWork);
            _configuracaoIntegracaoEmissorDocumento = repConfiguracaoIntegracaoEmissorDocumento.BuscarConfiguracaoPadrao();
        }

        public IntegracaoNSTech(Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoIntegracaoEmissorDocumento configuracaoIntegracaoEmissorDocumento)
        {
            _configuracaoIntegracaoEmissorDocumento = configuracaoIntegracaoEmissorDocumento;
        }

        #endregion

        #region Métodos Globais

        #endregion

        #region Métodos Privados

        public retornoWebService TransmitirEmissor(enumTipoWS tipoWS, object objEnvio, string metodo, string urlWebService, bool formUrlEncoded = false)
        {
            return this.Transmitir(tipoWS, null, objEnvio, metodo, urlWebService, formUrlEncoded);
        }

        public retornoWebService TransmitirEmissor(enumTipoWS tipoWS, string parametroGET, string metodo, string urlWebService)
        {
            return this.Transmitir(tipoWS, parametroGET, null, metodo, urlWebService, true);
        }

        private retornoWebService Transmitir(enumTipoWS tipoWS, string parametroGET, object objEnvio, string metodo, string urlWebService, bool formUrlEncoded = false)
        {
            var retornoWS = new retornoWebService();

            try
            {
                if (_configuracaoIntegracaoEmissorDocumento == null)
                    throw new ServicoException("Processo Abortado! Integração não configurada.");

                if (string.IsNullOrEmpty(urlWebService))
                    throw new ServicoException("Processo Abortado! URL não definida.");

                if (string.IsNullOrEmpty(_configuracaoIntegracaoEmissorDocumento.NSTechTokenAPIKey))
                    throw new ServicoException("Processo Abortado! Token não definido.");

                string url = null;
                if (urlWebService.EndsWith("/"))
                    url = urlWebService;
                else
                    url = urlWebService + "/";
                url += metodo;

                HttpContent content = null;
                HttpRequestMessage request = null;

                if (tipoWS == enumTipoWS.POST || tipoWS == enumTipoWS.PUT)
                {
                    var settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
                    retornoWS.jsonEnvio = JsonConvert.SerializeObject(objEnvio, settings);

                    if (formUrlEncoded)
                    {
                        var values = retornoWS.jsonEnvio.FromJson<Dictionary<string, object>>();
                        Dictionary<string, string> dString = values.ToDictionary(k => k.Key, k => k.Value.ToString());
                        content = new FormUrlEncodedContent(dString);
                    }
                    else
                    {
                        content = new StringContent(JsonConvert.SerializeObject(objEnvio), Encoding.UTF8, "application/json");
                    }
                }
                else if (tipoWS == enumTipoWS.GET)
                {
                    if (!string.IsNullOrEmpty(parametroGET))
                    {
                        if (!url.EndsWith("/"))
                            url += "/";
                        url += parametroGET;
                    }

                    retornoWS.jsonEnvio = url;
                }
                else if (tipoWS == enumTipoWS.PATCH)
                {
                    if (!url.EndsWith("/"))
                        url += "/";
                    url += parametroGET;

                    request = new HttpRequestMessage(new HttpMethod("PATCH"), url);

                    if (objEnvio != null)
                    {
                        retornoWS.jsonEnvio = JsonConvert.SerializeObject(objEnvio);
                        request.Content = new StringContent(retornoWS.jsonEnvio, Encoding.UTF8, "application/json");
                    }
                    else
                    {
                        retornoWS.jsonEnvio = url;
                    }
                }

                HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoNSTech));

                // TIMEOUT EM MILISEGUNDOS
                client.Timeout = TimeSpan.FromMilliseconds(300000);

                // TLS 1.2
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                client.DefaultRequestHeaders.Add("x-nstech-api-key", _configuracaoIntegracaoEmissorDocumento.NSTechTokenAPIKey);

                HttpResponseMessage response = null;
                if (tipoWS == enumTipoWS.POST)
                    response = client.PostAsync(url, content).Result;
                else if (tipoWS == enumTipoWS.GET)
                    response = client.GetAsync(url).Result;
                else if (tipoWS == enumTipoWS.PATCH)
                    response = client.SendAsync(request).Result;
                else if (tipoWS == enumTipoWS.PUT)
                    response = client.PutAsync(url, content).Result;
                else if (tipoWS == enumTipoWS.DELETE)
                    response = client.DeleteAsync(url).Result;

                retornoWS.jsonRetorno = response.Content.ReadAsStringAsync().Result;
                retornoWS.StatusCode = (int)response.StatusCode;

                if (!response.IsSuccessStatusCode)
                {
                    retornoWS.erro = true;
                    if (string.IsNullOrEmpty(retornoWS.jsonRetorno))
                        retornoWS.mensagem = string.Format(@"Não foi possível consumir o Web Service {0}: {1}.", metodo, response.RequestMessage);
                    else
                        retornoWS.mensagem = ExtrairMenssagemRetorno(retornoWS, metodo);
                }
                else
                {
                    retornoWS.erro = false;
                }

            }
            catch (ServicoException ex)
            {
                retornoWS.erro = true;
                retornoWS.mensagem = ex.Message;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                retornoWS.erro = true;
                retornoWS.mensagem = $"Ocorreu uma falha ao consumir o metodo {metodo}.";
            }

            return retornoWS;
        }

        private string ExtrairMenssagemRetorno(retornoWebService retornoWS, string metodo)
        {
            if (string.IsNullOrWhiteSpace(retornoWS?.jsonRetorno))
                return "";

            try
            {
                MensagemRetorno objetoRetorno = JsonConvert.DeserializeObject<MensagemRetorno>(retornoWS.jsonRetorno);

                if (objetoRetorno == null)
                    return "Erro ao processar resposta do serviço.";

                if (!string.IsNullOrEmpty(objetoRetorno.Type))
                {
                    switch (objetoRetorno.Type)
                    {
                        case "document_is_authorized":
                            return "";

                        case "document_in_processing":
                            return "O documento está em processado.";

                        case "document_is_canceled":
                            return "O documento já está cancelado.";

                        case "document_is_invalid":
                            return "O documento está em um status desconhecido.";

                        case "event_in_processing":
                            return "O evento está em processado.";

                        case "event_is_authorized":
                            return "O evento já foi autorizado.";

                        case "event_is_canceled":
                            return "O evento já está cancelado.";

                        case "event_is_invalid":
                            return "O evento está em um status desconhecido.";

                        case "cte_not_found":
                            return "O CT-e não existe.";

                        case "cte_exists_external_id":
                            return "Já existe um CTe registrado com o external ID informado para esse tenant.";

                        case "cte_event_not_found":
                            return "O Evento não existe.";

                        case "cte_event_exists_external_id":
                            return "Já existe um evento de CTe registrado com o external ID informado para esse tenant.";

                        case "mdfe_is_closed":
                            return "O documento está encerrado.";

                        case "mdfe_not_found":
                            return "O MDFe não existe.";

                        case "mdfe_exists_external_id":
                            return "Já existe um MDFe registrado com o external ID informado para esse tenant.";

                        case "mdfe_event_not_found":
                            return "O Evento não existe.";

                        case "mdfe_event_exists_external_id":
                            return "Já existe um evento de MDFe registrado com o external ID informado para esse tenant.";

                        case "certificate_exists":
                            return "O certificado já está cadastrado na nossa base de dados. Não há necessidade de enviar o certificado pois ele já se encontra no sistema.";

                        case "reading_error":
                            return "Erro ao ler o certificado. Verifique se o certificado enviado é um certificado valido ou se a senha dele está correta.";

                        case "owner_document_invalid":
                            return "O documento enviado no campo {ownerDocument}, não é o mesmo que está associado ao certificado. Enviar um certificado que tenha o mesmo CNPJ/CPF do campo {ownerDocument}.";

                        case "private_key_invalid":
                            return "A chave privada do certificado é inválida. Enviar um certificado válido.";

                        case "certificate_invalid":
                            return "A chave púbica do certificado é inválida. Enviar um certificado válido.";

                        case "certificate_invalid_icp":
                            return "O certificado enviado não está no padrão ICP-Brasil. Enviar um certificado válido.";

                        case "certificate_expired":
                            return "O certificado enviado já está vencido. Enviar um certificado válido.";

                        case "certificate_not_found":
                            return "O certificado não foi encontrado. Fornecer um filtro que atenda a pelo menos um certificado cadastrado em nossa base de dados.";

                        case "certificate_exists_external_id":
                            return "O certificado já está cadastrado na nossa base de dados. Utilize um external ID único por tenant.";

                        case "generic_http_500":
                            return "Erro interno. Entrar em contato com o suporte técnico.";

                        default:
                            {
                                Servicos.Log.TratarErro($"Erro não tratado: {retornoWS.ToJson()}", "EmissorNSTech");
                                return "Erro não tratado entre em contato com administrador do sistema.";
                            }
                    }
                }
                else
                {
                    if (objetoRetorno.BodyErrors?.Issues != null && objetoRetorno.BodyErrors.Issues.Count > 0)
                    {
                        List<string> mensagens = new List<string>();

                        foreach (Issue issue in objetoRetorno.BodyErrors.Issues)
                            mensagens.Add(TranslateSchema(issue));

                        return "Erros de validação encontrados: " + string.Join(" ", mensagens);
                    }
                    else
                    {
                        if (string.IsNullOrWhiteSpace(retornoWS.jsonRetorno.ToString()))
                            return string.Empty;

                        string semHtml = Regex.Replace(retornoWS.jsonRetorno.ToString(), "<.*?>", string.Empty);

                        string limpo = Regex.Replace(semHtml, @"[^a-zA-Z0-9\s]", string.Empty);

                        limpo = Regex.Replace(limpo, @"\s+", " ").Trim();

                        return limpo;
                    }
                }
            }
            catch (Newtonsoft.Json.JsonException ex)
            {
                Servicos.Log.TratarErro(string.Concat("Erro ao processar resposta do serviço Integracao CTE JsonException", metodo), "MensagensCTE");
                Servicos.Log.TratarErro($"Retorno WS JsonException StatusCode: {retornoWS.StatusCode}", "MensagensCTE");
                Servicos.Log.TratarErro($"Retorno WS JsonException: {JsonConvert.SerializeObject(retornoWS)}", "MensagensCTE");
                Servicos.Log.TratarErro(ex, "MensagensCTE");
                return "Erro ao processar resposta do serviço Integracao CTE.";
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(string.Concat("Erro ao processar resposta do serviço Integracao CTE Exception", metodo), "MensagensCTE");
                Servicos.Log.TratarErro($"Retorno WS Exception StatusCode: {retornoWS.StatusCode}", "MensagensCTE");
                Servicos.Log.TratarErro($"Retorno WS Exception: {JsonConvert.SerializeObject(retornoWS)}", "MensagensCTE");
                Servicos.Log.TratarErro(ex, "MensagensCTE");
                return "Erro ao processar resposta do serviço Integracao CTE.";
            }
        }

        private string TranslateSchema(Issue issue)
        {
            string path = issue.Path != null ? string.Join(".", issue.Path) : "";
            string normalizedPath = path.Replace("[", "").Replace("]", "");

            switch ((normalizedPath, issue.Code))
            {
                case ("data.recipient.address.country.code", "invalid_type"):
                    return "O código do país do destinatário é inválido.";

                case ("data.recipient.phone", "too_small"):
                    return $"O telefone do destinatário  deve conter no mínimo {issue.Minimum} caracteres.";

                default:
                    Servicos.Log.TratarErro($"normalizedPath=>{normalizedPath} | issue.Code: {issue.Code} ", "MensagensCTENaoTraduzida");

                    if (issue.Code == "invalid_type" && issue.Expected != null && issue.Received != null)
                        return $"O campo {normalizedPath} esperava um valor do tipo {issue.Expected}, mas recebeu {issue.Received}.";

                    path = issue.Path != null ? string.Join(", ", issue.Path) : "caminho desconhecido";
                    return $" Codigo: {issue.Code}, Caminho: {path}, Mensagem: {issue.Message}";
            }
        }

        #endregion
    }
}
