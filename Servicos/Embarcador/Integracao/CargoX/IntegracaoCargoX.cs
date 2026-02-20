using Dominio.Excecoes.Embarcador;
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

namespace Servicos.Embarcador.Integracao.CargoX
{
    public class IntegracaoCargoX
    {

        #region Métodos Públicos

        public static Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal ConsultarNotaFiscal(string numeroCarga, string chaveNFe, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Embarcador.Configuracoes.Integracao repositorioConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService repConfiguracaoWebService = new Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService(unitOfWork);
            
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoWebService configuracaoWebservice = repConfiguracaoWebService.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Empresa empresaPai = repEmpresa.BuscarEmpresaPai();
            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repositorioConfiguracaoIntegracao.Buscar();

            Servicos.Embarcador.NFe.NFe serNFe = new Servicos.Embarcador.NFe.NFe();

            if ((configuracaoIntegracao == null) || string.IsNullOrWhiteSpace(configuracaoIntegracao.URLIntegracaoCargoX) || string.IsNullOrWhiteSpace(configuracaoIntegracao.TokenCargoX))
                throw new ServicoException("Não existe configuração de integração disponível para a CargoX.");
            else
            {
                Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal = repXMLNotaFiscal.BuscarPorChave(chaveNFe);
                if (xmlNotaFiscal != null)
                    return xmlNotaFiscal;

                string endPoint = configuracaoIntegracao.URLIntegracaoCargoX;
                endPoint = string.Concat(endPoint, "invoices/", chaveNFe);

                string request = endPoint;
                string response = string.Empty;

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoCargoX));
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", configuracaoIntegracao.TokenCargoX);

                try
                {
                    Servicos.Log.TratarErro("Request ConsultarNotaFiscal " + request, "IntegracaoCargoX");
                    var result = client.GetAsync(endPoint).Result;
                    response = result.Content.ReadAsStringAsync().Result;
                    Servicos.Log.TratarErro("Response ConsultarNotaFiscal " + response, "IntegracaoCargoX");

                    if (result.IsSuccessStatusCode)
                    {
                        var retorno = (string)(result.Content.ReadAsStringAsync().Result);
                        if (!string.IsNullOrWhiteSpace(retorno))
                        {
                            dynamic retornoXML = JsonConvert.DeserializeObject<dynamic>(retorno);

                            if (retornoXML == null)
                                throw new ServicoException("Consulta na CargoX não retornou XML.");

                            string stringBase64 = (string)retornoXML.xml;
                            byte[] data = System.Convert.FromBase64String(stringBase64);
                            var stream = new MemoryStream(Encoding.UTF8.GetBytes(System.Text.ASCIIEncoding.ASCII.GetString(data) ?? ""));
                            StreamReader reader = new StreamReader(stream);

                            serNFe.BuscarDadosNotaFiscal(out string erro, out xmlNotaFiscal, reader, unitOfWork, null, true, false, false, null, false, false, null, null,  null, configuracaoWebservice?.CadastroAutomaticoPessoaExterior ?? false);

                            repXMLNotaFiscal.Inserir(xmlNotaFiscal);

                            return xmlNotaFiscal;
                        }
                        else
                        {
                            //throw new ServicoException("Consulta na CargoX não teve retorno.");                            
                            Servicos.Log.TratarErro("Consulta na CargoX não teve retorno.", "IntegracaoCargoX");
                            IntegrarSituacaoDiversas(numeroCarga, "error", 9999, string.Concat(request, " - ", string.IsNullOrWhiteSpace(response) ? "Consulta NFe não teve retorno." : response), chaveNFe, unitOfWork);
                            return null;
                        }
                    }
                    else
                    {
                        //throw new ServicoException("Consulta na CargoX retornou falha.");
                        Servicos.Log.TratarErro("Não retornou sucesso.", "IntegracaoCargoX");
                        IntegrarSituacaoDiversas(numeroCarga, "error", 9999, request + " - Não retornou sucesso.", chaveNFe, unitOfWork);
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex, "IntegracaoCargoX");
                    IntegrarSituacaoDiversas(numeroCarga, "error", 9999, request + " - Retornou falha.", chaveNFe, unitOfWork);
                    //throw new ServicoException("Não foi possível consultar nota na CargoX.");
                    return null;
                }
            }
        }

        public static void IntegrarCTeMDFe(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repositorioCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(unitOfWork);
            Repositorio.Embarcador.Configuracoes.Integracao repositorioConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaMDFe repCargaMDFe = new Repositorio.Embarcador.Cargas.CargaMDFe(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repositorioConfiguracaoIntegracao.Buscar();
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            if ((configuracaoIntegracao == null) || string.IsNullOrWhiteSpace(configuracaoIntegracao.URLIntegracaoCargoX) || string.IsNullOrWhiteSpace(configuracaoIntegracao.TokenCargoX))
            {
                cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                cargaIntegracao.ProblemaIntegracao = "Não existe configuração de integração disponível para a CargoX.";
            }
            else
            {
                Servicos.WebService.CTe.CTe servicoCte = new Servicos.WebService.CTe.CTe(unitOfWork);
                Servicos.MDFe servicoMDFe = new Servicos.MDFe(unitOfWork);

                cargaIntegracao.NumeroTentativas += 1;
                cargaIntegracao.DataIntegracao = DateTime.Now;

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                string endPoint = configuracaoIntegracao.URLIntegracaoCargoX;
                endPoint = string.Concat(endPoint, "freights/", cargaIntegracao.Carga.CodigoCargaEmbarcador.ToString(), "/documents");

                bool situacaoIntegracao = false;
                string mensagemErro = string.Empty;

                List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> listaCargaCTe = repCargaCTe.BuscarPorCarga(cargaIntegracao.Carga.Codigo);

                Dominio.ObjetosDeValor.Embarcador.Integracao.CargoX.RetornoDocumento retornoDocumento = new Dominio.ObjetosDeValor.Embarcador.Integracao.CargoX.RetornoDocumento();
                retornoDocumento.ctes = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.CargoX.Documento>();
                retornoDocumento.nfses = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.CargoX.Documento>();
                foreach (var cargaCTe in listaCargaCTe)
                {
                    if (cargaCTe.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe)
                    {
                        string xmlCTe = servicoCte.ObterRetornoXMLPorStatus(cargaCTe.CTe, "A", unitOfWork);
                        byte[] byt = !string.IsNullOrEmpty(xmlCTe) ? System.Text.Encoding.UTF8.GetBytes(xmlCTe) : null;

                        Dominio.ObjetosDeValor.Embarcador.Integracao.CargoX.Documento documentoCTe = new Dominio.ObjetosDeValor.Embarcador.Integracao.CargoX.Documento();
                        documentoCTe.access_key = cargaCTe.CTe.Chave;
                        documentoCTe.xml = byt != null ? Convert.ToBase64String(byt) : string.Empty;
                        documentoCTe.pdf = servicoCte.ObterRetornoPDF(cargaCTe.CTe, configuracaoTMS.UtilizarCodificacaoUTF8ConversaoPDF, unitOfWork);
                        retornoDocumento.ctes.Add(documentoCTe);
                    }
                    else
                    {//string.Format("{0:n2}", cte.Valor)
                        string xmlNFSe = $@"<NFSe>
                                           <cnpjEmissor>{cargaCTe.CTe.Empresa.CNPJ}</cnpjEmissor>
                                           <nomeEmissor>{cargaCTe.CTe.Empresa.RazaoSocial}</nomeEmissor> 
                                           <imEmissor>{cargaCTe.CTe.Empresa.InscricaoMunicipal}</imEmissor> 
                                           <numero>{cargaCTe.CTe.Numero}</numero>
                                           <serie>{cargaCTe.CTe.Serie.Numero}</serie>
                                           <codigoVerificacao>{cargaCTe.CTe.Protocolo}</codigoVerificacao>
                                           <valor>{string.Format("{0:n2}", cargaCTe.CTe.BaseCalculoISS)}</valor>
                                           <baseCalculoISS>{string.Format("{0:n2}", cargaCTe.CTe.BaseCalculoISS)}</baseCalculoISS>
                                           <aliquotaISS>{string.Format("{0:n2}", cargaCTe.CTe.AliquotaISS)}</aliquotaISS>
                                           <valorISS>{string.Format("{0:n2}", cargaCTe.CTe.ValorISS)}</valorISS>
                                           <valorISSRetido>{string.Format("{0:n2}", cargaCTe.CTe.ValorISSRetido)}</valorISSRetido>
                                           <cnpjTomador>{cargaCTe.CTe.TomadorPagador.CPF_CNPJ}</cnpjTomador>
                                           <nomeTomador>{cargaCTe.CTe.TomadorPagador.Nome}</cnpjTomador>
                                          </NFSe>";

                        byte[] byt = !string.IsNullOrEmpty(xmlNFSe) ? System.Text.Encoding.UTF8.GetBytes(xmlNFSe) : null;

                        Dominio.ObjetosDeValor.Embarcador.Integracao.CargoX.Documento documentoNFSe = new Dominio.ObjetosDeValor.Embarcador.Integracao.CargoX.Documento();
                        documentoNFSe.access_key = cargaCTe.CTe.Chave;
                        documentoNFSe.xml = byt != null ? Convert.ToBase64String(byt) : string.Empty;
                        documentoNFSe.pdf = servicoCte.ObterRetornoPDF(cargaCTe.CTe, configuracaoTMS.UtilizarCodificacaoUTF8ConversaoPDF, unitOfWork);
                        retornoDocumento.nfses.Add(documentoNFSe);
                    }
                }

                List<Dominio.Entidades.Embarcador.Cargas.CargaMDFe> listaCargaMDFe = repCargaMDFe.BuscarPorCarga(cargaIntegracao.Carga.Codigo);
                retornoDocumento.mdfes = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.CargoX.Documento>();
                foreach (var cargaMDFe in listaCargaMDFe)
                {
                    var retornoEventoMDFe = servicoMDFe.ObterDadosIntegradosMDFe(cargaMDFe.MDFe.Codigo, cargaMDFe.MDFe.Empresa.Codigo, unitOfWork);

                    byte[] byt = retornoEventoMDFe != null && !string.IsNullOrWhiteSpace(retornoEventoMDFe.XML) ? System.Text.Encoding.UTF8.GetBytes(retornoEventoMDFe.XML) : null;

                    Dominio.ObjetosDeValor.Embarcador.Integracao.CargoX.Documento documentoMDFe = new Dominio.ObjetosDeValor.Embarcador.Integracao.CargoX.Documento();
                    documentoMDFe.access_key = cargaMDFe.MDFe.Chave;
                    documentoMDFe.xml = byt != null ? Convert.ToBase64String(byt) : string.Empty;
                    documentoMDFe.pdf = servicoMDFe.ObterDAMDFE(cargaMDFe.MDFe.Codigo, cargaMDFe.MDFe.Empresa.Codigo, unitOfWork, configuracaoTMS.UtilizarCodificacaoUTF8ConversaoPDF);
                    retornoDocumento.mdfes.Add(documentoMDFe);
                }

                HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoCargoX));

                client.BaseAddress = new Uri(endPoint);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", configuracaoIntegracao.TokenCargoX);

                string jsonRequest = string.Empty;
                string jsonResponse = string.Empty;

                try
                {
                    jsonRequest = JsonConvert.SerializeObject(retornoDocumento, Formatting.Indented);
                    var content = new StringContent(jsonRequest.ToString(), Encoding.UTF8, "application/json");
                    var result = client.PostAsync(endPoint, content).Result;
                    jsonResponse = result.Content.ReadAsStringAsync().Result;

                    if (result.IsSuccessStatusCode)
                    {
                        //Conforme retorno do e-mail quando for sucesso não tem objeto de retorno
                        //Dominio.ObjetosDeValor.Embarcador.Integracao.CargoX.Retorno retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.CargoX.Retorno>(result.Content.ReadAsStringAsync().Result);
                        //if (retorno == null)
                        //    throw new Exception("Consulta na CargoX não teve retorno.");
                        //else
                        //{
                        situacaoIntegracao = true;
                        mensagemErro = string.Empty;

                        Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo();
                        arquivoIntegracao.Data = cargaIntegracao.DataIntegracao;
                        arquivoIntegracao.Mensagem = mensagemErro;
                        arquivoIntegracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;

                        arquivoIntegracao.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonRequest, "json", unitOfWork);
                        arquivoIntegracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonResponse, "json", unitOfWork);

                        repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

                        cargaIntegracao.ArquivosTransacao.Add(arquivoIntegracao);
                        //}
                    }
                    else
                    {
                        Dominio.ObjetosDeValor.Embarcador.Integracao.CargoX.Retorno retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.CargoX.Retorno>(result.Content.ReadAsStringAsync().Result);
                        if (retorno == null)
                            mensagemErro = result.StatusCode.ToString();
                        else
                            mensagemErro = retorno.details.FirstOrDefault()?.message;
                        if (string.IsNullOrWhiteSpace(mensagemErro))
                            mensagemErro = "Falha na integração com a CargoX.";
                        else
                            mensagemErro = "Retorno CargoX: " + mensagemErro;

                        situacaoIntegracao = false;

                        Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo();
                        arquivoIntegracao.Data = cargaIntegracao.DataIntegracao;
                        arquivoIntegracao.Mensagem = mensagemErro;
                        arquivoIntegracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;

                        arquivoIntegracao.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonRequest, "json", unitOfWork);
                        arquivoIntegracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonResponse, "json", unitOfWork);

                        repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

                        cargaIntegracao.ArquivosTransacao.Add(arquivoIntegracao);

                        cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                        cargaIntegracao.ProblemaIntegracao = mensagemErro;
                        repositorioCargaIntegracao.Atualizar(cargaIntegracao);
                    }
                }
                catch (Exception excecao)
                {
                    Servicos.Log.TratarErro(excecao, "IntegracaoCargoX");
                    Servicos.Log.TratarErro("Request: " + jsonRequest, "IntegracaoCargoX");
                    Servicos.Log.TratarErro("Response: " + jsonResponse, "IntegracaoCargoX");

                    mensagemErro = "Ocorreu uma falha ao comunicar com o Serviço da CargoX.";
                    situacaoIntegracao = false;

                    Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo();
                    arquivoIntegracao.Data = cargaIntegracao.DataIntegracao;
                    arquivoIntegracao.Mensagem = excecao.Message.Length > 400 ? excecao.Message.Substring(0, 400) : excecao.Message;
                    arquivoIntegracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;


                    arquivoIntegracao.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonRequest, "json", unitOfWork);
                    arquivoIntegracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonResponse, "json", unitOfWork);

                    repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

                    cargaIntegracao.ArquivosTransacao.Add(arquivoIntegracao);

                    cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    cargaIntegracao.ProblemaIntegracao = mensagemErro;
                    repositorioCargaIntegracao.Atualizar(cargaIntegracao);
                    return;
                }

                if (!situacaoIntegracao)
                {
                    cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    cargaIntegracao.ProblemaIntegracao = mensagemErro;
                }
                else
                {
                    cargaIntegracao.ProblemaIntegracao = string.Empty;
                    cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                }
            }
            repositorioCargaIntegracao.Atualizar(cargaIntegracao);
        }

        public static void IntegrarSituacaoCTe(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repositorioCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(unitOfWork);
            Repositorio.Embarcador.Configuracoes.Integracao repositorioConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaMDFe repCargaMDFe = new Repositorio.Embarcador.Cargas.CargaMDFe(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repositorioConfiguracaoIntegracao.Buscar();
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            if ((configuracaoIntegracao == null) || string.IsNullOrWhiteSpace(configuracaoIntegracao.URLIntegracaoCargoX) || string.IsNullOrWhiteSpace(configuracaoIntegracao.TokenCargoX))
                Servicos.Log.TratarErro("IntegrarSituacaoCTe: Não existe configuração de integração disponível para a CargoX.", "IntegracaoCargoX");
            else
            {
                Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe = repCargaCTe.BuscarPorCTe(cte.Codigo);

                if (cargaCTe != null)
                {
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                    string endPoint = configuracaoIntegracao.URLIntegracaoCargoX;
                    endPoint = string.Concat(endPoint, "freights/", cargaCTe.Carga.CodigoCargaEmbarcador.ToString(), "/emission");
                    string mensagemErro = string.Empty;

                    HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoCargoX));

                    client.BaseAddress = new Uri(endPoint);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", configuracaoIntegracao.TokenCargoX);

                    string jsonRequest = string.Empty;
                    string jsonResponse = string.Empty;

                    try
                    {
                        Dominio.ObjetosDeValor.Embarcador.Integracao.CargoX.RetornoEmissao retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.CargoX.RetornoEmissao();
                        retorno.status = cte.Status == "E" ? "processing" : cte.Status == "R" && cte.MensagemStatus != null && cte.MensagemStatus.CodigoDoErro == 8888 ? "warning" : "error";
                        if (cte.Status == "R")
                        {
                            retorno.details = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.CargoX.RetornoEmissaoDetalhe>();
                            Dominio.ObjetosDeValor.Embarcador.Integracao.CargoX.RetornoEmissaoDetalhe detalhe = new Dominio.ObjetosDeValor.Embarcador.Integracao.CargoX.RetornoEmissaoDetalhe();
                            detalhe.code = cte.MensagemStatus?.CodigoDoErro ?? 9999;
                            detalhe.message = cte.MensagemStatus?.MensagemDoErro ?? cte.MensagemRetornoSefaz;
                            detalhe.reference = "CTe: " + cte.Chave;
                            retorno.details.Add(detalhe);
                        }

                        jsonRequest = JsonConvert.SerializeObject(retorno, Formatting.Indented);
                        var content = new StringContent(jsonRequest.ToString(), Encoding.UTF8, "application/json");
                        var result = client.PutAsync(endPoint, content).Result;
                        jsonResponse = result.Content.ReadAsStringAsync().Result;

                        if (result.IsSuccessStatusCode)
                        {
                            Servicos.Log.TratarErro("URL: " + endPoint, "IntegracaoCargoX");
                            Servicos.Log.TratarErro("Request: " + jsonRequest, "IntegracaoCargoX");
                            Servicos.Log.TratarErro("Response: " + jsonResponse, "IntegracaoCargoX");
                        }
                        else
                            throw new Exception(result.StatusCode.ToString());
                    }
                    catch (Exception excecao)
                    {
                        Servicos.Log.TratarErro("IntegrarSituacaoCTe: " + excecao, "IntegracaoCargoX");

                        Servicos.Log.TratarErro("URL: " + endPoint, "IntegracaoCargoX");
                        Servicos.Log.TratarErro("Request: " + jsonRequest, "IntegracaoCargoX");
                        Servicos.Log.TratarErro("Response: " + jsonResponse, "IntegracaoCargoX");

                        return;
                    }
                }
            }
        }

        public static void IntegrarSituacaoMDFe(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repositorioCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(unitOfWork);
            Repositorio.Embarcador.Configuracoes.Integracao repositorioConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaMDFe repCargaMDFe = new Repositorio.Embarcador.Cargas.CargaMDFe(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repositorioConfiguracaoIntegracao.Buscar();
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            if ((configuracaoIntegracao == null) || string.IsNullOrWhiteSpace(configuracaoIntegracao.URLIntegracaoCargoX) || string.IsNullOrWhiteSpace(configuracaoIntegracao.TokenCargoX))
                Servicos.Log.TratarErro("IntegrarSituacaoMDFe: Não existe configuração de integração disponível para a CargoX.", "IntegracaoCargoX");
            else
            {
                Dominio.Entidades.Embarcador.Cargas.CargaMDFe cargaMDFe = repCargaMDFe.BuscarPorMDFe(mdfe.Codigo);

                if (cargaMDFe != null)
                {
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                    string endPoint = configuracaoIntegracao.URLIntegracaoCargoX;
                    endPoint = string.Concat(endPoint, "freights/", cargaMDFe.Carga.CodigoCargaEmbarcador.ToString(), "/emission");
                    string mensagemErro = string.Empty;

                    HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoCargoX));

                    client.BaseAddress = new Uri(endPoint);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", configuracaoIntegracao.TokenCargoX);

                    string jsonRequest = string.Empty;
                    string jsonResponse = string.Empty;

                    try
                    {
                        Dominio.ObjetosDeValor.Embarcador.Integracao.CargoX.RetornoEmissao retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.CargoX.RetornoEmissao();
                        retorno.status = mdfe.Status == Dominio.Enumeradores.StatusMDFe.Enviado ? "processing" : mdfe.Status == Dominio.Enumeradores.StatusMDFe.Rejeicao && mdfe.MensagemStatus != null && mdfe.MensagemStatus.CodigoDoErro == 8888 ? "warning" : "error";
                        if (mdfe.Status == Dominio.Enumeradores.StatusMDFe.Rejeicao)
                        {
                            retorno.details = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.CargoX.RetornoEmissaoDetalhe>();
                            Dominio.ObjetosDeValor.Embarcador.Integracao.CargoX.RetornoEmissaoDetalhe detalhe = new Dominio.ObjetosDeValor.Embarcador.Integracao.CargoX.RetornoEmissaoDetalhe();
                            detalhe.code = mdfe.MensagemStatus?.CodigoDoErro ?? 9999;
                            detalhe.message = mdfe.MensagemStatus?.MensagemDoErro ?? mdfe.MensagemRetornoSefaz;
                            detalhe.reference = "MDFe: " + mdfe.Chave;
                            retorno.details.Add(detalhe);
                        }

                        jsonRequest = JsonConvert.SerializeObject(retorno, Formatting.Indented);
                        var content = new StringContent(jsonRequest.ToString(), Encoding.UTF8, "application/json");
                        var result = client.PutAsync(endPoint, content).Result;
                        jsonResponse = result.Content.ReadAsStringAsync().Result;

                        if (result.IsSuccessStatusCode)
                        {
                            Servicos.Log.TratarErro("URL: " + endPoint, "IntegracaoCargoX");
                            Servicos.Log.TratarErro("Request: " + jsonRequest, "IntegracaoCargoX");
                            Servicos.Log.TratarErro("Response: " + jsonResponse, "IntegracaoCargoX");
                        }
                        else
                            throw new Exception(result.StatusCode.ToString());
                    }
                    catch (Exception excecao)
                    {
                        Servicos.Log.TratarErro("IntegrarSituacaoMDFe: " + excecao, "IntegracaoCargoX");

                        Servicos.Log.TratarErro("URL: " + endPoint, "IntegracaoCargoX");
                        Servicos.Log.TratarErro("Request: " + jsonRequest, "IntegracaoCargoX");
                        Servicos.Log.TratarErro("Response: " + jsonResponse, "IntegracaoCargoX");

                        return;
                    }
                }
            }
        }

        public static void IntegrarSituacaoEncerramentoMDFe(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repositorioCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(unitOfWork);
            Repositorio.Embarcador.Configuracoes.Integracao repositorioConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaMDFe repCargaMDFe = new Repositorio.Embarcador.Cargas.CargaMDFe(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repositorioConfiguracaoIntegracao.Buscar();
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            if ((configuracaoIntegracao == null) || string.IsNullOrWhiteSpace(configuracaoIntegracao.URLIntegracaoCargoX) || string.IsNullOrWhiteSpace(configuracaoIntegracao.TokenCargoX))
                Servicos.Log.TratarErro("IntegrarSituacaoEncerramentoMDFe: Não existe configuração de integração disponível para a CargoX.", "IntegracaoCargoX");
            else
            {
                Dominio.Entidades.Embarcador.Cargas.CargaMDFe cargaMDFe = repCargaMDFe.BuscarPorMDFe(mdfe.Codigo);

                if (cargaMDFe != null)
                {
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                    string endPoint = configuracaoIntegracao.URLIntegracaoCargoX;
                    endPoint = string.Concat(endPoint, "freights/", cargaMDFe.Carga.CodigoCargaEmbarcador.ToString(), "/mdfe");
                    string mensagemErro = string.Empty;

                    HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoCargoX));

                    client.BaseAddress = new Uri(endPoint);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", configuracaoIntegracao.TokenCargoX);

                    string jsonRequest = string.Empty;
                    string jsonResponse = string.Empty;

                    try
                    {
                        Dominio.ObjetosDeValor.Embarcador.Integracao.CargoX.RetornoEmissao retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.CargoX.RetornoEmissao();
                        retorno.status = mdfe.Status == Dominio.Enumeradores.StatusMDFe.EmEncerramento ? "processing" : mdfe.Status == Dominio.Enumeradores.StatusMDFe.Encerrado ? "success" : mdfe.Status == Dominio.Enumeradores.StatusMDFe.Autorizado && mdfe.MensagemStatus != null && mdfe.MensagemStatus.CodigoDoErro == 8888 ? "warning" : "error";
                        if (mdfe.Status == Dominio.Enumeradores.StatusMDFe.Rejeicao)
                        {
                            retorno.details = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.CargoX.RetornoEmissaoDetalhe>();
                            Dominio.ObjetosDeValor.Embarcador.Integracao.CargoX.RetornoEmissaoDetalhe detalhe = new Dominio.ObjetosDeValor.Embarcador.Integracao.CargoX.RetornoEmissaoDetalhe();
                            detalhe.code = mdfe.MensagemStatus?.CodigoDoErro ?? 9999;
                            detalhe.message = mdfe.MensagemStatus?.MensagemDoErro ?? mdfe.MensagemRetornoSefaz;
                            detalhe.reference = "MDFe: " + mdfe.Chave;
                            retorno.details.Add(detalhe);
                        }
                        else if (mdfe.Status == Dominio.Enumeradores.StatusMDFe.Encerrado)
                        {
                            retorno.details = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.CargoX.RetornoEmissaoDetalhe>();
                            Dominio.ObjetosDeValor.Embarcador.Integracao.CargoX.RetornoEmissaoDetalhe detalhe = new Dominio.ObjetosDeValor.Embarcador.Integracao.CargoX.RetornoEmissaoDetalhe();
                            detalhe.code = 200;
                            detalhe.message = mdfe.MensagemStatus?.MensagemDoErro ?? mdfe.MensagemRetornoSefaz;
                            detalhe.finished_at = mdfe.DataEncerramento.HasValue ? mdfe.DataEncerramento.Value.ToString("yyyy-MM-dd HH:mm:ss -0300") : string.Empty;
                            retorno.details.Add(detalhe);
                        }

                        jsonRequest = JsonConvert.SerializeObject(retorno, Formatting.Indented);
                        var content = new StringContent(jsonRequest.ToString(), Encoding.UTF8, "application/json");
                        var result = client.PutAsync(endPoint, content).Result;
                        jsonResponse = result.Content.ReadAsStringAsync().Result;

                        if (result.IsSuccessStatusCode)
                        {
                            Servicos.Log.TratarErro("URL: " + endPoint, "IntegracaoCargoX");
                            Servicos.Log.TratarErro("Request: " + jsonRequest, "IntegracaoCargoX");
                            Servicos.Log.TratarErro("Response: " + jsonResponse, "IntegracaoCargoX");
                        }
                        else
                            throw new Exception(result.StatusCode.ToString());
                    }
                    catch (Exception excecao)
                    {
                        Servicos.Log.TratarErro("IntegrarSituacaoEncerramentoMDFe: " + excecao, "IntegracaoCargoX");

                        Servicos.Log.TratarErro("URL: " + endPoint, "IntegracaoCargoX");
                        Servicos.Log.TratarErro("Request: " + jsonRequest, "IntegracaoCargoX");
                        Servicos.Log.TratarErro("Response: " + jsonResponse, "IntegracaoCargoX");

                        return;
                    }
                }
            }
        }

        public static void IntegrarSituacaoDiversas(string numeroCarga, string status, int code, string message, string reference, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repositorioCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(unitOfWork);
            Repositorio.Embarcador.Configuracoes.Integracao repositorioConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaMDFe repCargaMDFe = new Repositorio.Embarcador.Cargas.CargaMDFe(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repositorioConfiguracaoIntegracao.Buscar();
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            if ((configuracaoIntegracao == null) || string.IsNullOrWhiteSpace(configuracaoIntegracao.URLIntegracaoCargoX) || string.IsNullOrWhiteSpace(configuracaoIntegracao.TokenCargoX))
                Servicos.Log.TratarErro("IntegrarSituacaoCTe: Não existe configuração de integração disponível para a CargoX.", "IntegracaoCargoX");
            else
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                string endPoint = configuracaoIntegracao.URLIntegracaoCargoX;
                endPoint = string.Concat(endPoint, "freights/", numeroCarga, "/emission");
                string mensagemErro = string.Empty;

                HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoCargoX));

                client.BaseAddress = new Uri(endPoint);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", configuracaoIntegracao.TokenCargoX);

                string jsonRequest = string.Empty;
                string jsonResponse = string.Empty;

                try
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.CargoX.RetornoEmissao retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.CargoX.RetornoEmissao();
                    retorno.status = status; // "warning" : "error";  
                    retorno.details = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.CargoX.RetornoEmissaoDetalhe>();
                    Dominio.ObjetosDeValor.Embarcador.Integracao.CargoX.RetornoEmissaoDetalhe detalhe = new Dominio.ObjetosDeValor.Embarcador.Integracao.CargoX.RetornoEmissaoDetalhe();
                    detalhe.code = code;
                    detalhe.message = message;
                    detalhe.reference = reference;
                    retorno.details.Add(detalhe);

                    jsonRequest = JsonConvert.SerializeObject(retorno, Formatting.Indented);
                    var content = new StringContent(jsonRequest.ToString(), Encoding.UTF8, "application/json");
                    var result = client.PutAsync(endPoint, content).Result;
                    jsonResponse = result.Content.ReadAsStringAsync().Result;

                    if (result.IsSuccessStatusCode)
                    {
                        Servicos.Log.TratarErro("URL: " + endPoint, "IntegracaoCargoX");
                        Servicos.Log.TratarErro("Request: " + jsonRequest, "IntegracaoCargoX");
                        Servicos.Log.TratarErro("Response: " + jsonResponse, "IntegracaoCargoX");
                    }
                    else
                        throw new Exception(result.StatusCode.ToString());
                }
                catch (Exception excecao)
                {
                    Servicos.Log.TratarErro("IntegrarSituacaoCTe: " + excecao, "IntegracaoCargoX");

                    Servicos.Log.TratarErro("URL: " + endPoint, "IntegracaoCargoX");
                    Servicos.Log.TratarErro("Request: " + jsonRequest, "IntegracaoCargoX");
                    Servicos.Log.TratarErro("Response: " + jsonResponse, "IntegracaoCargoX");

                    return;
                }
            }
        }


        #endregion

        #region Métodos Privados
        private static string RemoveTroublesomeCharacters(string inString)
        {
            if (inString == null) return null;

            StringBuilder newString = new StringBuilder();
            char ch;

            for (int i = 0; i < inString.Length; i++)
            {

                ch = inString[i];
                // remove any characters outside the valid UTF-8 range as well as all control characters
                // except tabs and new lines
                if ((ch < 0x00FD && ch > 0x001F) || ch == '\t' || ch == '\n' || ch == '\r')
                {
                    newString.Append(ch);
                }
            }
            return newString.ToString();

        }
        #endregion
    }
}
