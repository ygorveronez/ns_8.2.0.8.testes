using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Integracao.KMM;
using Infrastructure.Services.HttpClientFactory;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Xml.Serialization;

namespace Servicos.Embarcador.Integracao.KMM
{
    public partial class IntegracaoKMM
    {
        #region Atributos Globais

        private readonly Repositorio.UnitOfWork _unitOfWork;
        private readonly AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware _tipoServicoMultisoftware;

        #endregion Atributos Globais

        #region Construtores


        public IntegracaoKMM(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            _unitOfWork = unitOfWork;
            _tipoServicoMultisoftware = tipoServicoMultisoftware;
        }
        public IntegracaoKMM(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion Construtores

        #region Métodos Públicos
        public string LPadCpfCnpj(string tipo, string cpfCnpj)
        {
            if (String.IsNullOrEmpty(cpfCnpj) || String.IsNullOrEmpty(tipo))
            {
                return null;
            }

            string lpadded = cpfCnpj;
            if (tipo.Equals("F"))
            {
                lpadded = cpfCnpj.PadLeft(11, '0');
            }
            else if (tipo.Equals("J"))
            {
                lpadded = cpfCnpj.PadLeft(14, '0');
            }

            return lpadded;
        }
        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.retornoWebService Transmitir(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoKMM configuracaoIntegracaoKMM, object request, string method = "POST")
        {
            var retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.retornoWebService();
            /*
            02002 - Não foi possível cancelar a carga (já está cancelado).
            03001 - Não foi possível inserir o documento (já está inserido).
            03002 - Não foi possível cancelar o documento (já está cancelado).
             */
            List<int> codigoSucesso = new List<int> { 02002, 03001, 03003, 05001, 05003 };
            try
            {
                if (!(configuracaoIntegracaoKMM?.PossuiIntegracao ?? false))
                    throw new ServicoException("Não possui configuração para KMM.");

                String token = RecuperarToken(configuracaoIntegracaoKMM);

                if (string.IsNullOrEmpty(token))
                    throw new ServicoException("Processo Abortado! Não foi possível obter o token de integração.");

                string url = $"{configuracaoIntegracaoKMM.URL}";
                HttpClient requisicao = CriarRequisicao(url, token);

                retorno.jsonRequisicao = JsonConvert.SerializeObject(request, Formatting.Indented);

                StringContent conteudoRequisicao = new StringContent(retorno.jsonRequisicao.ToString(), Encoding.UTF8, "application/json");

                HttpResponseMessage retornoRequisicao = null;

                if (method == "GET")
                {
                    var requisicaoPatch = new HttpRequestMessage(new HttpMethod("GET"), url)
                    {
                        Content = conteudoRequisicao
                    };
                    retornoRequisicao = requisicao.SendAsync(requisicaoPatch).Result;
                }
                else
                    retornoRequisicao = requisicao.PostAsync(url, conteudoRequisicao).Result;

                retorno.jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;

                if (retornoRequisicao.StatusCode == HttpStatusCode.OK)
                {

                    // tratamento retorno vazio
                    if (retorno.jsonRetorno.IndexOf("{\"success\":true,\"code\":200,\"result\":[]}") >= 0)
                    {
                        retorno.jsonRetorno = retorno.jsonRetorno.Replace("\"result\":[]", "\"result\":null");
                    }

                    var retornoWS = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.RetornoPadrao>(retorno.jsonRetorno);

                    if (retornoWS == null || !retornoWS.Success || retornoWS.Result == null)
                        throw new ServicoException($"Falha ao conectar no WS KMM: {retornoWS.Code}");


                    bool sucesso;
                    bool.TryParse((retornoWS.Result["success"] ?? "false").ToString(), out sucesso);

                    int code;
                    int.TryParse((retornoWS.Result["code"] ?? 0).ToString(), out code);
                    retorno.CodigoErro = code;

                    if (!sucesso)
                    {
                        if(retornoWS.Result["code"] == null || !codigoSucesso.Any(x => x == code))
                        {
                            var error = new DictionaryEntry();
                            error = retornoWS.Result.OfType<DictionaryEntry>().Where(x => x.Key.ToString() == "error" || x.Key.ToString() == "erro").FirstOrDefault();
                            
                            if (retornoWS != null && (!retornoWS.Success || !(error.Value?.ToString() == "0" || error.Value?.ToString() == null)))
                            {
                                if (error.Value?.ToString() != "0")
                                {
                                    var mensagem = retornoWS?.Result.OfType<DictionaryEntry>().Where(x => x.Key.ToString() == "mensagem").FirstOrDefault();
                                    throw new ServicoException($"{mensagem?.Value?.ToString()} - {error.Value?.ToString()}");
                                }
                                else
                                {
                                    throw new ServicoException($"{retornoWS.Message}\n{retornoWS.Details}");
                                }
                            }
                            else
                            {
                                throw new ServicoException($"Erro ao receber mensagem dos erros de integração: {retornoWS.Result["code"] ?? ""}");
                            }
                        }
                    }

                    retorno.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                    if (retornoWS != null && retornoWS.Result != null && retornoWS.Result["mensagem"] != null && !codigoSucesso.Any(x => x == code))
                    {
                        retorno.ProblemaIntegracao = (retornoWS.Result["mensagem"]).ToString();
                    } else
                    {
                        retorno.ProblemaIntegracao = "Registro integrado com sucesso";
                    }
                }
                else if (retorno.jsonRetorno.IndexOf("message") >= 0)
                {
                    var retornoWS = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.RetornoPadrao>(retorno.jsonRetorno);
                    if (!retornoWS.Success)
                        throw new ServicoException($"{retornoWS.Message}");
                }
                else
                    throw new ServicoException($"Falha ao conectar no WS KMM: {retornoRequisicao.StatusCode}");
            }
            catch (ServicoException ex)
            {
                retorno.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                retorno.ProblemaIntegracao = ex.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                retorno.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                retorno.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração da KMM";
            }

            if (retorno?.ProblemaIntegracao.Length > 300)
                retorno.ProblemaIntegracao = retorno.ProblemaIntegracao.Substring(0, 300);

            return retorno;
        }

        private HttpClient CriarRequisicao(string url, string accessToken)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            HttpClient requisicao = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoKMM));
            requisicao.BaseAddress = new Uri(url);
            requisicao.DefaultRequestHeaders.Accept.Clear();
            requisicao.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            if (accessToken != null)
            {
                requisicao.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            }

            return requisicao;
        }

        private string RecuperarToken(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoKMM configuracaoIntegracaoKMM)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            HttpClient requisicao = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoKMM));
            requisicao.BaseAddress = new Uri(configuracaoIntegracaoKMM.URL);
            requisicao.DefaultRequestHeaders.Accept.Clear();
            requisicao.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            Hashtable request = new Hashtable
            {
                { "module", "LOGON" },
                { "operation", "LOGON" },
            };
            Hashtable parameters = new Hashtable
            {
                { "username", configuracaoIntegracaoKMM.Usuario },
                { "password", configuracaoIntegracaoKMM.Senha },
                { "cod_gestao", configuracaoIntegracaoKMM.CodGestao }
            };
            request.Add("parameters", parameters);

            string jsonRequisicao = "";
            string jsonRetorno = "";

            try
            {
                jsonRequisicao = JsonConvert.SerializeObject(request, Formatting.Indented);
                StringContent conteudoRequisicao = new StringContent(jsonRequisicao.ToString(), Encoding.UTF8, "application/json");
                HttpResponseMessage retornoRequisicao = requisicao.PostAsync(configuracaoIntegracaoKMM.URL, conteudoRequisicao).Result;
                jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;

                if (retornoRequisicao.StatusCode == HttpStatusCode.OK)
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.RetornoPadrao retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.RetornoPadrao>(jsonRetorno);

                    if (!retorno.Success)
                        throw new ServicoException($"{retorno.Code} - {retorno.Message} - {retorno.Details}");

                    return (string)retorno.Result["token"];
                }
                else
                    throw new ServicoException($"Falha ao conectar no WS KMM: {retornoRequisicao.StatusCode}");
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);
            }

            return "";
        }

        private string ObterXMLDocumento(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Dominio.Entidades.Embarcador.NFS.DadosNFSManual nfseManual, bool cancelamento = false)
        {
            Repositorio.NFSeItem repNFSeItem = new Repositorio.NFSeItem(_unitOfWork);

            TemplateDocumento documento = new TemplateDocumento();

            documento.Emitente = new Emitente();

            // Preenchendo os campos do emitente
            documento.Emitente.Cnpj = LPadCpfCnpj(cte.Empresa?.Tipo.ToString(), cte.Empresa?.CNPJ.ToString()) ?? "";
            documento.Emitente.RazaoSocial = cte.Empresa?.RazaoSocial?.ToString() ?? "";
            documento.Emitente.InscricaoEstadual = cte.Empresa?.InscricaoEstadual?.ToString() ?? "";

            if (cte.Empresa?.Localidade != null)
            {
                documento.Emitente.Bairro = cte.Empresa?.Bairro?.ToString() ?? "";
                documento.Emitente.Cep = cte.Empresa?.CEP?.ToString() ?? "";
                documento.Emitente.EndNumero = cte.Empresa?.Numero?.ToString() ?? "";
                documento.Emitente.Endereco = cte.Empresa?.Endereco?.ToString() ?? "";
                documento.Emitente.Municipio = cte.Empresa?.Localidade?.Descricao;
                documento.Emitente.MunicipioCodIBGE = cte.Empresa?.Localidade?.CodigoIBGE.ToString();
                documento.Emitente.Uf = cte.Empresa?.Localidade?.Estado.Sigla.ToString();
            }

            Dominio.Entidades.ParticipanteCTe destinatario = cte.Tomador;
            if (destinatario == null)
                destinatario = cte.Destinatario;

            documento.Destinatario = new Destinatario();

            // Preenchendo os campos do destinatário
            if(destinatario?.Tipo == Dominio.Enumeradores.TipoPessoa.Juridica)
                documento.Destinatario.Cnpj = LPadCpfCnpj(destinatario?.Tipo.ToString(), destinatario?.CPF_CNPJ.ToString()) ?? "";
            else
                documento.Destinatario.Cpf = LPadCpfCnpj(destinatario?.Tipo.ToString(), destinatario?.CPF_CNPJ.ToString()) ?? "";

            documento.Destinatario.RazaoSocial = destinatario?.Nome?.ToString() ?? "";
            documento.Destinatario.InscricaoEstadual = destinatario?.IE_RG?.ToString() ?? "";

            if (destinatario?.Localidade != null)
            {
                documento.Destinatario.Bairro = destinatario?.Bairro?.ToString() ?? "";
                documento.Destinatario.Cep = destinatario?.CEP?.ToString() ?? "";
                documento.Destinatario.EndNumero = destinatario?.Numero?.ToString() ?? "";
                documento.Destinatario.Endereco = destinatario?.Endereco?.ToString() ?? "";
                documento.Destinatario.Municipio = destinatario?.Localidade?.Descricao;
                documento.Destinatario.MunicipioCodIBGE = destinatario?.Localidade?.CodigoIBGE.ToString();
                documento.Destinatario.Uf = destinatario?.Localidade?.Estado.Sigla.ToString();
            }

            documento.Identificacao = new Identificacao();
            documento.Total = new Total();
            documento.InformacaoAdicional = new InformacaoAdicional();
            documento.Identificacao.Ambiente = cte.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Producao ? 1 : 2;

            if (nfseManual == null && !cte.ModeloDocumentoFiscal.Abreviacao.Equals("NFS-e") && !cte.ModeloDocumentoFiscal.Abreviacao.Equals("NFS"))
            {
                string codModelo = cte.ModeloDocumentoFiscal?.Numero ?? "";

                // Preenchendo os campos da nota
                documento.Total.ValorTotal = cte.ValorPrestacaoServico;
                documento.Identificacao.DataEmissao = cte.DataEmissao ?? DateTime.Now;
                documento.Identificacao.NumNota = cte.Numero.ToString() ?? "";
                documento.Identificacao.Chave = string.IsNullOrEmpty(cte.ChaveAcesso) ? cte.Codigo.ToString() : cte.ChaveAcesso;
                documento.Identificacao.Serie = cte.Serie?.Numero.ToString() ?? "";
                documento.Identificacao.SubSerie = cte.SerieRPS ?? "";
                documento.Identificacao.CodModelo = codModelo;
                documento.InformacaoAdicional.ObservacaoManual = cte.ObservacaoDaCarga ?? "";

                List<Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.Produto> listaProdutoDet = this.ObterDocumentosCTE(cte);

                if (listaProdutoDet.Count > 0)
                {
                    documento.Detalhes = new List<DetalheTemplate>();
                    documento.Detalhes.Add(new DetalheTemplate { Produtos = listaProdutoDet });
                }
                             
            }
            else
            {
                
                // Preenchendo os campos da nota
                documento.Total.ValorTotal = nfseManual?.ValorFrete ?? cte.ValorPrestacaoServico;
                documento.Identificacao.DataEmissao = nfseManual?.DataEmissao ?? cte.DataEmissao ?? DateTime.Now;
                documento.Identificacao.NumNota = nfseManual?.Numero.ToString() ?? cte.Numero.ToString() ?? "";
                documento.Identificacao.Chave = string.IsNullOrEmpty(cte.ChaveAcesso) ? cte.Codigo.ToString() : cte.ChaveAcesso;
                documento.Identificacao.Serie = nfseManual?.Serie?.Numero.ToString() ?? cte.Serie?.Numero.ToString() ?? "";
                documento.Identificacao.SubSerie = cte.SerieRPS ?? "";
                documento.Identificacao.CodModelo = "X1";
                documento.InformacaoAdicional.ObservacaoManual = nfseManual?.Observacoes ?? cte.ObservacaoDaCarga ?? "";
                
                List<Dominio.Entidades.NFSeItem> itens = repNFSeItem.BuscarPorCTe(cte.Codigo);
                Dominio.Entidades.NFSeItem item = itens.FirstOrDefault();

                List<Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.Produto> listaProdutoDet = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.Produto>();

                if (item != null)
                {
                    // Preenchendo os detalhes do produto
                    listaProdutoDet.Add(new Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.Produto
                    {
                        NumItem = 1,
                        Cfop = 0,
                        CodProduto = item.Servico?.Numero ?? "",
                        Descricao = item.Servico?.Descricao ?? "",
                        Unidade = "UN",
                        Quantidade = item.Quantidade,
                        ValorUnitario = item.ValorServico,
                        Valor = item.ValorTotal,
                        Impostos = new Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.Impostos
                        {
                            IssAliquota = cte.AliquotaISS,
                            IssValor = cte.ValorISS,
                            IssRetido = cte.ISSRetido ? 1 : 0,
                            IssBaseCalc = cte.BaseCalculoISS,
                            IrAliquota = cte.AliquotaIR,
                            IrValor = cte.ValorIR,
                            IrRetido = cte.ValorIR > 0 ? 1 : 0,
                            IrBaseCalc = cte.ValorBaseCalculoIR,
                            PisAliquota = cte.AliquotaPIS,
                            PisRetido = 0,
                            PisBaseCalc = cte.BasePIS,
                            PisValor = cte.ValorPIS,
                            CofinsAliquota = cte.AliquotaCOFINS,
                            CofinsBaseCalc = cte.BaseCOFINS,
                            CofinsRetido = 0,
                            CofinsValor = cte.ValorCOFINS,
                            InssAliquota = cte.AliquotaINSS,
                            InssBaseCalc = cte.ValorBaseCalculoINSS,
                            InssRetido = 0,
                            InssValor = cte.ValorINSS,
                            CsllAliquota = cte.AliquotaCSLL,
                            CsllBaseCalc = cte.ValorBaseCalculoCSLL,
                            CsllRetido = 0,
                            CsllValor = cte.ValorCSLL

                        }
                    });

                }
                else
                {
                    listaProdutoDet = this.ObterDocumentosCTE(cte);
                }

                if (listaProdutoDet.Count > 0)
                {
                    documento.Detalhes = new List<DetalheTemplate>();
                    documento.Detalhes.Add( new DetalheTemplate { Produtos = listaProdutoDet });
                }

            }


            if (cte.Fatura != null)
            {
                // Preenchendo as faturas
                documento.Faturas = new Faturas
                {
                    FaturaList = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.Fatura>
                    {
                        new Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.Fatura
                        {
                            NumItem = 1,
                            NumeroFatura = cte.Fatura.Numero.ToString() ?? "",
                            DataVencimento = cte.Fatura.Parcelas?.LastOrDefault()?.DataVencimento ?? DateTime.Now,
                            ValorFatura = cte.ValorAReceber,
                        }
                    }
                };
            }

            
            if (cancelamento)
            {
                if (cte.Status.Equals("C"))
                {
                    documento.Eventos = new Eventos();
                    // Preenchendo campos opcionais (cancelamento)
                    documento.Eventos.MotivoCancelamento = cte.ObservacaoCancelamento ?? "";
                    documento.Eventos.DataCancelamento = cte.DataCancelamento ?? DateTime.Now;
                    documento.Eventos.Cancelada = "S";
                }
            }


            
            // Definindo o namespace que será usado no XML
            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, "http://www.portalfiscal.inf.br/nfe");

            XmlSerializer serializer = new XmlSerializer(typeof(TemplateDocumento));

            using (StringWriter stringWriter = new StringWriter())
            {
                serializer.Serialize(stringWriter, documento, namespaces);

                return stringWriter.ToString().Replace("\r\n", "");
            }
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.Produto> ObterDocumentosCTE(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)
        {
            Repositorio.DocumentosCTE repDocumentosCTE = new Repositorio.DocumentosCTE(_unitOfWork);

            Dominio.Entidades.DocumentosCTE documentosCTe = repDocumentosCTE.BuscarPorCTe(cte.Codigo).FirstOrDefault();

            List<Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.Produto> listaProdutoDet = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.Produto>();

            if (documentosCTe != null )
            {
                listaProdutoDet.Add(new Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.Produto
                {
                    NumItem = 1,
                    Cfop = cte.CFOP?.CodigoCFOP ?? 0,
                    CodProduto = cte.ModeloDocumentoFiscal?.Abreviacao ?? "",
                    Descricao = documentosCTe.Descricao ?? "",
                    Unidade = "UN",
                    Quantidade = 1,
                    ValorUnitario = cte.ValorAReceber,
                    Valor = cte.ValorAReceber,
                    Impostos = new Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.Impostos
                    {
                        IssAliquota = cte.AliquotaISS,
                        IssValor = cte.ValorISS,
                        IssRetido = cte.ISSRetido ? 1 : 0,
                        IssBaseCalc = cte.BaseCalculoISS,
                        IrAliquota = cte.AliquotaIR,
                        IrValor = cte.ValorIR,
                        IrRetido = cte.ISSRetido ? 1 : 0,
                        IrBaseCalc = cte.ValorBaseCalculoIR,
                        PisAliquota = cte.AliquotaPIS,
                        PisRetido = 0,
                        PisBaseCalc = cte.BasePIS,
                        PisValor = cte.ValorPIS,
                        CofinsAliquota = cte.AliquotaCOFINS,
                        CofinsBaseCalc = cte.BaseCOFINS,
                        CofinsRetido = 0,
                        CofinsValor = cte.ValorCOFINS,
                        InssAliquota = cte.AliquotaINSS,
                        InssBaseCalc = cte.ValorBaseCalculoINSS,
                        InssRetido = 0,
                        InssValor = cte.ValorINSS,
                        CsllAliquota = cte.AliquotaCSLL,
                        CsllBaseCalc = cte.ValorBaseCalculoCSLL,
                        CsllRetido = 0,
                        CsllValor = cte.ValorCSLL

                    }
                });
            }

            return listaProdutoDet;
        }
        #endregion Métodos Privados
    }
}
