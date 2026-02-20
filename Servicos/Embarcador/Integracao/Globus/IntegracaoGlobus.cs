using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Integracao.Globus;
using Infrastructure.Services.HttpClientFactory;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;


namespace Servicos.Embarcador.Integracao.Globus
{
    public partial class IntegracaoGlobus
    {
        #region Atributos Globais

        private readonly Repositorio.UnitOfWork _unitOfWork;
        private readonly AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware _tipoServicoMultisoftware;
        private readonly Repositorio.Embarcador.Configuracoes.IntegracaoGlobus _configuracaoIntegracaoRepositorio;

        private Dominio.Entidades.Embarcador.Configuracoes.IntegracaoGlobus _configuracaoIntegracao;
        string tokenAutenticacao = null;

        #endregion Atributos Globais

        #region Construtores
        public IntegracaoGlobus(Repositorio.UnitOfWork unitOfWork)
        {

            _unitOfWork = unitOfWork;
            _configuracaoIntegracaoRepositorio = new Repositorio.Embarcador.Configuracoes.IntegracaoGlobus(unitOfWork);
            _configuracaoIntegracao = _configuracaoIntegracaoRepositorio.Buscar();

        }

        #endregion Construtores

        #region Métodos Públicos

        #endregion

        #region Métodos Privados

        private retornoWebService Transmitir(object objEnvio, string metodo, string token, string uri, enumTipoWS tipoWS = enumTipoWS.POST)
        {
            var retornoWS = new Dominio.ObjetosDeValor.Embarcador.Integracao.Globus.retornoWebService();

            try
            {
                if (_configuracaoIntegracao == null)
                    throw new ServicoException("Processo Abortado! Integração não configurada.");

                if (string.IsNullOrEmpty(uri))
                    throw new ServicoException("Processo Abortado! URL não definida.");

                string url = null;
                if (uri.EndsWith("/"))
                    url = uri;
                else
                    url = uri + "/";
                url += metodo;

                HttpClient requisicao = CriarRequisicao(url, token);

                retornoWS.jsonRequisicao = JsonConvert.SerializeObject(objEnvio, Formatting.Indented);

                HttpResponseMessage retornoRequisicao;

                // Verifica o tipo de método para escolher o tipo de requisição correto
                if (tipoWS == enumTipoWS.POST)
                {
                    StringContent conteudoRequisicao = new StringContent(retornoWS.jsonRequisicao, Encoding.UTF8, "application/json");
                    retornoRequisicao = requisicao.PostAsync(url, conteudoRequisicao).Result;
                }
                else if (tipoWS == enumTipoWS.PATCH)
                {
                    StringContent conteudoRequisicao = new StringContent(retornoWS.jsonRequisicao, Encoding.UTF8, "application/json");
                    var requisicaoPatch = new HttpRequestMessage(new HttpMethod("PATCH"), url)
                    {
                        Content = conteudoRequisicao
                    };
                    retornoRequisicao = requisicao.SendAsync(requisicaoPatch).Result;
                }
                else if (tipoWS == enumTipoWS.DELETE)
                {
                    StringContent conteudoRequisicao = new StringContent(retornoWS.jsonRequisicao, Encoding.UTF8, "application/json");
                    var requisicaoPatch = new HttpRequestMessage(new HttpMethod("DELETE"), url)
                    {
                        Content = conteudoRequisicao
                    };
                    retornoRequisicao = requisicao.SendAsync(requisicaoPatch).Result;
                }
                else
                {
                    throw new ServicoException("Método HTTP não suportado.");
                }

                retornoWS.jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;

                if (retornoRequisicao.StatusCode == HttpStatusCode.OK || retornoRequisicao.StatusCode == HttpStatusCode.Accepted)
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Globus.retornoWebServiceError retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Globus.retornoWebServiceError>(retornoWS.jsonRetorno);

                    if (!(retorno?.success ?? true) && (retorno?.errors != null || retorno?.errors?.Count > 0))
                        throw new ServicoException($"Erro: {string.Join(", ", retorno.errors)}");

                    if (!string.IsNullOrEmpty(retorno?.MensagemDeErro))
                        throw new ServicoException($"Erro: {retorno?.MensagemDeErro}");

                    retornoWS.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                    retornoWS.ProblemaIntegracao = "Registro integrado com sucesso";
                }
                else
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Globus.retornoWebServiceError retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Globus.retornoWebServiceError>(retornoWS.jsonRetorno);

                    if (retorno?.errors != null || retorno?.errors?.Count > 0)
                        throw new ServicoException($"Erro: {string.Join(", ", retorno.errors)}");

                    if (!string.IsNullOrEmpty(retorno?.MensagemDeErro))
                        throw new ServicoException($"Erro: {retorno?.MensagemDeErro}");

                }

            }
            catch (ServicoException ex)
            {
                retornoWS.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                retornoWS.ProblemaIntegracao = ex.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                retornoWS.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                retornoWS.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração da Globus";
            }

            if (retornoWS?.ProblemaIntegracao.Length > 300)
                retornoWS.ProblemaIntegracao = retornoWS.ProblemaIntegracao.Substring(0, 300);

            return retornoWS;
        }

        private HttpClient CriarRequisicao(string url, string accessToken)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            HttpClient requisicao = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoGlobus));
            requisicao.BaseAddress = new Uri(url);
            requisicao.DefaultRequestHeaders.Accept.Clear();
            requisicao.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            if (accessToken != null)
            {
                requisicao.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            }

            return requisicao;
        }

        private string ObterToken(string metodo, string uri, int shortCode, string senha, bool formUrlEncoded = false)
        {
            string jsonRequisicao = null;
            string jsonRetorno = null;

            if (string.IsNullOrWhiteSpace(metodo))
                return null;

            try
            {
                if (_configuracaoIntegracao == null)
                    throw new ServicoException("Processo Abortado! Integração não configurada.");

                if (string.IsNullOrEmpty(uri))
                    throw new ServicoException("Processo Abortado! URL não definida.");

                string url = null;
                if (uri.EndsWith("/"))
                    url = uri;
                else
                    url = uri + "/";
                url += metodo;

                HttpClient requisicao = CriarRequisicao(url, null);
                StringContent conteudoRequisicao = null;
                HttpResponseMessage retornoRequisicao = null;

                if (formUrlEncoded)
                {
                    var formContent = new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("grant_type", "password"),
                        new KeyValuePair<string, string>("username", shortCode.ToString()),
                        new KeyValuePair<string, string>("password", senha)
                    });

                    retornoRequisicao = requisicao.PostAsync(url, formContent).Result;
                    jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;
                }
                else
                {
                    Hashtable request = new Hashtable
                    {
                        { "shortCode", shortCode},
                        { "token", senha },
                    };

                    jsonRequisicao = JsonConvert.SerializeObject(request, Formatting.Indented);
                    conteudoRequisicao = new StringContent(jsonRequisicao.ToString(), Encoding.UTF8, "application/json");

                    retornoRequisicao = requisicao.PostAsync(url, conteudoRequisicao).Result;
                    jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;

                }

                if (retornoRequisicao.StatusCode == HttpStatusCode.OK || retornoRequisicao.StatusCode == HttpStatusCode.Accepted)
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Globus.retornoWebServiceToken retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Globus.retornoWebServiceToken>(jsonRetorno);

                    if (String.IsNullOrEmpty(retorno?.data?.token ?? null) && String.IsNullOrEmpty(retorno?.access_token ?? null))
                        throw new ServicoException($"Não retornou um token para ser utilizado!");

                    return (string)retorno?.data?.token ?? (string)retorno?.access_token ?? "";
                }
                else
                    throw new ServicoException($"Falha ao conectar no WS Globus: {retornoRequisicao.StatusCode}");

            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

            }

            return tokenAutenticacao;
        }

        private Hashtable ConverterCargaCTe(Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe, Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal notaFiscal, Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS documentoEntrada, bool cancelamento = false, int codigoDocumento = 0 )
        {
            Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(_unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);

            if (cargaCTe?.CTe == null)
                throw new Exception("A Carga CTe não possui CTe vinculado");

            var carga = cargaCTe.Carga;
            var cte = cargaCTe.CTe;
            string xmlString = "";
            var modeloDocumento = cargaCTe != null ? cte.ModeloDocumentoFiscal : notaFiscal.ModeloDocumentoFiscal;

            if (modeloDocumento.Abreviacao.Equals("CT-e") || modeloDocumento.Abreviacao.Equals("NF-e"))
            {
                if (cancelamento)
                    xmlString = new Servicos.WebService.CTe.CTe(_unitOfWork).ObterRetornoXML(cargaCTe.CTe, _unitOfWork);
                else
                    xmlString = new Servicos.WebService.CTe.CTe(_unitOfWork).ObterRetornoXMLAutorizacao(cargaCTe.CTe, _unitOfWork);

                string xMotivoCTe = Utilidades.XML.ObterConteudoTag(xmlString, "xMotivo");
                Hashtable cargaCTeHash = new Hashtable();

                cargaCTeHash.Add("InscricaoEmpresa", carga.Empresa?.CNPJ.ToString());
                cargaCTeHash.Add("Garagem", carga.Empresa?.CodigoIntegracao ?? "0");
                cargaCTeHash.Add("TipoDocumento", cte.ModeloDocumentoFiscal?.Numero ?? "0");
                cargaCTeHash.Add("Serie", cte.Serie?.Numero.ToString() ?? "");
                cargaCTeHash.Add("Conhecimento", cte.Numero);
                cargaCTeHash.Add("Fase", cancelamento ? 1 : 2);
                cargaCTeHash.Add("Sistema", _configuracaoIntegracao.SistemaIntegrarComEscritaFiscal);
                cargaCTeHash.Add("Usuario", _configuracaoIntegracao.Usuario);
                cargaCTeHash.Add("ChaveDeAcesso", cte.Chave?.ToString() ?? "");
                cargaCTeHash.Add("DataEnvio", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"));
                cargaCTeHash.Add("ConteudoXml", xmlString);
                cargaCTeHash.Add("Protocolo", cte.Protocolo ?? "");
                cargaCTeHash.Add("DataProtocolo", cte.DataAutorizacao?.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"));
                cargaCTeHash.Add("Status", cte.Status == "A" ? "A" : "C");
                cargaCTeHash.Add("Recibo", cte.NumeroRecibo ?? "");
                cargaCTeHash.Add("MensagemRecibo", xMotivoCTe ?? "");
                cargaCTeHash.Add("DataEmissao", cte.DataEmissao?.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"));
                cargaCTeHash.Add("Versao", cte.Versao ?? "");
                cargaCTeHash.Add("Situacao", cte.TipoCTE.ToString() ?? "");

                return cargaCTeHash;

            }
            else if (modeloDocumento.Abreviacao.Equals("NFS-e") || modeloDocumento.Abreviacao.Equals("NFS"))
            {
                Repositorio.ServicoNFSe repServicoNFSe = new Repositorio.ServicoNFSe(_unitOfWork);
                Dominio.Entidades.ServicoNFSe servicoNFSe = repServicoNFSe.BuscarPorEmpresa(carga.Empresa?.Codigo ?? 0);

                Hashtable cargaNFSeHash = new Hashtable();

                if (cancelamento)
                {
                    cargaNFSeHash.Add("CodigoDocumento", codigoDocumento.ToString());
                    cargaNFSeHash.Add("Usuario", _configuracaoIntegracao.Usuario);
                    cargaNFSeHash.Add("ExcluirDocumentoFiscal", false);
                }
                else
                {
                    string abreviacao = modeloDocumento.Abreviacao.Replace("-", "").Replace("_", "");
                    if (abreviacao.Length > 3)
                        abreviacao = abreviacao.Substring(0, 3);

                    cargaNFSeHash.Add("InscricaoEmpresa", carga.Empresa?.CNPJ_SemFormato.ToString() ?? "");
                    cargaNFSeHash.Add("InscricaoParticipante", cte.Tomador?.CPF_CNPJ ?? "");
                    cargaNFSeHash.Add("NumeroDocumento", cte.Numero.ToString().PadLeft(10, '0'));
                    cargaNFSeHash.Add("Serie", cte.Serie?.Numero.ToString() ?? "");
                    cargaNFSeHash.Add("CodigoTipoDocumento", abreviacao);
                    cargaNFSeHash.Add("DataEmissao", cte.DataEmissao?.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"));
                    cargaNFSeHash.Add("DataEntradaSaida", cte.DataEmissao?.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"));
                    cargaNFSeHash.Add("DataVencimento", cte.DataEmissao?.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"));
                    cargaNFSeHash.Add("CodigoIbgeMunicipio", carga.Empresa?.Localidade?.CodigoIBGE);
                    cargaNFSeHash.Add("Observacao", (cte?.ObservacoesGerais).Length > 300 ? cte.ObservacoesGerais.Substring(0,300) : cte.ObservacoesGerais);
                    cargaNFSeHash.Add("Usuario", _configuracaoIntegracao.Usuario);
                    cargaNFSeHash.Add("IntegrarContabilidade", _configuracaoIntegracao.IntegrarComContabilidade);
                    cargaNFSeHash.Add("IntegrarFinanceiro", _configuracaoIntegracao.IntegrarComContabilidade);
                    cargaNFSeHash.Add("ISSBaseCalculo", cte.BaseCalculoISS);
                    cargaNFSeHash.Add("ISSValor", cte.ValorISS);
                    cargaNFSeHash.Add("ISSRetido", cte.ISSRetido);

                    List<Hashtable> itens = new List<Hashtable>();
                    Hashtable item = new Hashtable();
                    item.Add("CodigoServico", servicoNFSe?.CodigoTributacao.ToString() ?? "0");
                    item.Add("ContaPlanoFinanceiro", modeloDocumento.TipoMovimentoEmissao?.CodigoIntegracao ?? "");
                    item.Add("Valor", cte.ValorPrestacaoServico);
                    item.Add("CustoFinanceiro", null);
                    item.Add("CustoContabil", 1); //temporario
                    item.Add("ContaContabil", 1364); //temporario
                    Hashtable valoresComplementares = new Hashtable { { "Grupo", null } };

                    item.Add("ValoresComplementares", valoresComplementares);

                    itens.Add(item);

                    cargaNFSeHash.Add("Itens", itens);
                }

                return cargaNFSeHash;
            }
            else if (!modeloDocumento.NaoGerarFaturamento)
            {
                Hashtable cargaNaoFiscalHash = new Hashtable();
                Repositorio.Embarcador.Financeiro.TituloDocumento repTituloDocumento = new Repositorio.Embarcador.Financeiro.TituloDocumento(_unitOfWork);
               
                if (cancelamento)
                {
                    cargaNaoFiscalHash.Add("CodigoDocumento", codigoDocumento.ToString());
                    cargaNaoFiscalHash.Add("Usuario", _configuracaoIntegracao.Usuario);
                }
                else
                {
                    cargaNaoFiscalHash.Add("InscricaoCliente", cte.Tomador?.CPF_CNPJ ?? "");
                    cargaNaoFiscalHash.Add("InscricaoEstatual", cte.Tomador?.IE_RG.ToString() ?? "");
                    cargaNaoFiscalHash.Add("CodigoTipoDocumento", _configuracaoIntegracao.CodigoIntegrarComContasReceber);
                    cargaNaoFiscalHash.Add("Serie", "1");
                    cargaNaoFiscalHash.Add("NumeroDocumento", cte.Numero.ToString().PadLeft(10, '0'));
                    cargaNaoFiscalHash.Add("NumeroParcela", 1);
                    cargaNaoFiscalHash.Add("Emissao", (cte?.DataEmissao ?? DateTime.Now).ToString("yyyy-MM-ddTHH:mm:ss.fffZ"));
                    cargaNaoFiscalHash.Add("Vencimento", ((cte?.DataEmissao ?? DateTime.Now).AddDays((cte.Tomador?.GrupoPessoas.DiasDePrazoFatura ?? 0))).ToString("yyyy-MM-ddTHH:mm:ss.fffZ"));
                    cargaNaoFiscalHash.Add("CodigoModelo", modeloDocumento.Numero);
                    cargaNaoFiscalHash.Add("Acrescimo", 0);
                    cargaNaoFiscalHash.Add("Desconto", 0);
                    cargaNaoFiscalHash.Add("Observacao", (cte?.ObservacoesGerais).Length > 300 ? cte.ObservacoesGerais.Substring(0, 300) : cte.ObservacoesGerais);
                    cargaNaoFiscalHash.Add("Usuario", _configuracaoIntegracao.Usuario);
                    cargaNaoFiscalHash.Add("Sistema", _configuracaoIntegracao.SistemaIntegrarComContasReceber);
                    cargaNaoFiscalHash.Add("IntegrarContabilidade", _configuracaoIntegracao.IntegrarComContasReceber);

                    List<Hashtable> itensDocumento = new List<Hashtable>();
                    Hashtable item = new Hashtable();
                    item.Add("CodigoTipoReceita", modeloDocumento.TipoMovimentoEmissao?.CodigoIntegracao ?? "");
                    item.Add("ValorItemDocumento", cte?.ValorPrestacaoServico ?? cte?.ValorAReceber ?? 0);
                    item.Add("Observacao", "");
                    item.Add("Plano", 1); // temporario
                    item.Add("ContaContabil", 799); //temporario
                    itensDocumento.Add(item);

                    cargaNaoFiscalHash.Add("ItemsDocumento", itensDocumento);
                }

                return cargaNaoFiscalHash;
            }
            else
            {
                throw new Exception($"Não é permitida integração para o modelo {modeloDocumento.Abreviacao} - {modeloDocumento.Descricao}.");
            }

        }

        private ConfiguracaoIntegracao ObterConfiguracaoComunicacao(Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCte, Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal notaFiscal, Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS documentoEntrada, bool cancelamento = false)
        {
            var modeloDocumento = "";
            if (cargaCte != null)
                modeloDocumento = cargaCte.CTe.ModeloDocumentoFiscal.Abreviacao;
            else if (notaFiscal != null)
                modeloDocumento = notaFiscal.ModeloNotaFiscal;
            else if (documentoEntrada != null)
                modeloDocumento = documentoEntrada.Modelo.Abreviacao;
            else
                throw new Exception("Nenhum documento enviado para comunicação");

            if (cargaCte != null && cargaCte?.CTe == null)
                throw new Exception("A carga não possui um documento vinculado");

            var configuracao = new ConfiguracaoIntegracao();
            if (modeloDocumento.Equals("CT-e") || modeloDocumento.Equals("NF-e") || modeloDocumento.Equals("55"))
            {
                configuracao.ShortCode = _configuracaoIntegracao.ShortCodeXML;
                configuracao.Token = _configuracaoIntegracao.TokenXML;
                configuracao.URLWebService = _configuracaoIntegracao.URLWebServiceXML;
                configuracao.EndPoint = documentoEntrada != null ? "api/Xml/XmlTerceiros" : "api/Xml/CtrcEletronico";
                configuracao.EndPointToken = "api/Autenticacao/GerarToken";
                configuracao.Method = enumTipoWS.POST;

            }
            else if (modeloDocumento.Equals("NFS-e") || modeloDocumento.Equals("NFS"))
            {
                configuracao.ShortCode = _configuracaoIntegracao.ShortCodeNFSe;
                configuracao.Token = _configuracaoIntegracao.TokenNFSe;
                configuracao.URLWebService = _configuracaoIntegracao.URLWebServiceNFSe;
                configuracao.EndPoint = "api/LivroISSSaida/Documento";
                configuracao.EndPointToken = "api/Autenticacao/GerarToken";
                configuracao.Method = cancelamento ? enumTipoWS.DELETE : enumTipoWS.POST;
            }
            else
            {
                configuracao.ShortCode = _configuracaoIntegracao.ShortCodeFinanceiro;
                configuracao.Token = null;
                configuracao.URLWebService = _configuracaoIntegracao.URLWebServiceFinanceiro;
                configuracao.EndPointToken = "";
                configuracao.EndPoint = cancelamento 
                              ? $"api/ContasReceber/Documentos?shortCode={_configuracaoIntegracao.ShortCodeFinanceiro}&token={_configuracaoIntegracao.TokenFinanceiro}" 
                              : $"api/ContasReceber/Documentos?" +
                              $"shortCode={_configuracaoIntegracao.ShortCodeFinanceiro}&" +
                              $"token={_configuracaoIntegracao.TokenFinanceiro}&" +
                              $"inscricaoEmpresa={cargaCte?.Carga?.Empresa?.CNPJ_SemFormato.ToString()}";
                configuracao.Method = cancelamento ? enumTipoWS.DELETE : enumTipoWS.POST;
            }

            if (configuracao.ShortCode == 0 || string.IsNullOrWhiteSpace(configuracao.EndPoint) || string.IsNullOrWhiteSpace(configuracao.URLWebService))
            {
                throw new Exception("Processo abortado, configuração de comunicação com Globus não encontrada ou incompleta!");
            }

            return configuracao;
        }
        #endregion Métodos Privados
    }
}
