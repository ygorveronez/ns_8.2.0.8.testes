using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Infrastructure.Services.HttpClientFactory;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Servicos.Embarcador.Integracao.Piracanjuba
{
    public sealed class IntegracaoPiracanjuba
    {
        #region Atributos

        private readonly Repositorio.UnitOfWork _unitOfWork;

        private readonly AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware _tipoServicoMultisoftware;

        private Dominio.Entidades.Embarcador.Configuracoes.IntegracaoPiracanjuba _configuracaoIntegracao;

        private Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS _configuracaoEmbarcador;

        private readonly Servicos.MDFe _servicoMDFe;

        private readonly Servicos.NFSe _servicoNFSe;

        private readonly Servicos.CTe _servicoCTe;

        private readonly Target.ValePedagio _servicoValePedagioTarget;

        #endregion

        #region Construtores

        public IntegracaoPiracanjuba(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            _unitOfWork = unitOfWork;
            _tipoServicoMultisoftware = tipoServicoMultisoftware;
            _servicoMDFe = new Servicos.MDFe(unitOfWork);
            _servicoNFSe = new Servicos.NFSe(unitOfWork);
            _servicoCTe = new Servicos.CTe(unitOfWork);
            _servicoValePedagioTarget = new Target.ValePedagio(unitOfWork);
        }

        #endregion

        #region Métodos Comunicação

        private HttpClient CriaRequisicao(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoPiracanjuba configuracaoIntegracao, bool autentica = true)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            HttpClient requisicao = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoPiracanjuba));

            requisicao.DefaultRequestHeaders.Accept.Clear();
            requisicao.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            requisicao.DefaultRequestHeaders.Add("APIkey", configuracaoIntegracao.ClientID);

            if (autentica)
            {
                string token = ObterToken(configuracaoIntegracao);
                requisicao.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            return requisicao;
        }

        private string ObterToken(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoPiracanjuba configuracaoIntegracao)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            var client = new RestClient(configuracaoIntegracao.URLAutenticacao);
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);

            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");

            request.AddParameter("client_id", configuracaoIntegracao.ClientID);
            request.AddParameter("client_secret", configuracaoIntegracao.ClientSecret);
            request.AddParameter("grant_type", "client_credentials");

            IRestResponse response = client.Execute(request);

            if (!response.IsSuccessful)
                throw new ServicoException("Não foi possível obter o Token");

            Dominio.ObjetosDeValor.Embarcador.Pedido.ConsultarAndamentoPedido.RetornoToken retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Pedido.ConsultarAndamentoPedido.RetornoToken>(response.Content);

            return retorno.access_token;
        }

        private string ObterMensagemDeErro(dynamic corpoResposta, HttpStatusCode statusCode)
        {
            if (corpoResposta.d.detail != null)
                return $"{corpoResposta.d.detail[0].msg}. HTTP Status {(int)statusCode}";
            else if (corpoResposta.d.msg != null)
                return $"{corpoResposta.d.msg}. HTTP Status {(int)statusCode}";
            else
                return $"Ocorreu uma falha ao realizar a integração com a Piracanjuba. HTTP Status {(int)statusCode}";
        }

        #endregion

        #region Métodos Privados

        private Dominio.Entidades.Embarcador.Configuracoes.IntegracaoPiracanjuba ObterConfiguracaoIntegracaoCarga()
        {
            if (_configuracaoIntegracao != null) return _configuracaoIntegracao;

            Repositorio.Embarcador.Configuracoes.IntegracaoPiracanjuba repositorioConfiguracaoIntegracaoPiracanjuba = new Repositorio.Embarcador.Configuracoes.IntegracaoPiracanjuba(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoPiracanjuba configuracaoIntegracaoPiracanjuba = repositorioConfiguracaoIntegracaoPiracanjuba.Buscar();

            if (string.IsNullOrWhiteSpace(configuracaoIntegracaoPiracanjuba.URLIntegracaoCargaPiracanjuba) || string.IsNullOrWhiteSpace(configuracaoIntegracaoPiracanjuba.URLAutenticacao))
                throw new ServicoException("Não existe configuração de integração disponível para a Piracanjuba.");

            _configuracaoIntegracao = configuracaoIntegracaoPiracanjuba;

            return _configuracaoIntegracao;
        }

        private Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS ObterConfiguracaoEmbarcador()
        {
            if (_configuracaoEmbarcador != null) return _configuracaoEmbarcador;

            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            _configuracaoEmbarcador = configuracao;

            return configuracao;
        }

        private bool EnviarArquivosIntegracao(List<Dominio.ObjetosDeValor.Embarcador.Integracao.Piracanjuba.DocumentoCarga> documentos, Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaCargaIntegracao = null, Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaIntegracao cargaCancelamentoCargaIntegracao = null)
        {
            bool todosArquivosIntegradosComSucesso = true;
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            foreach (Dominio.ObjetosDeValor.Embarcador.Integracao.Piracanjuba.DocumentoCarga documento in documentos)
            {
                string jsonDadosRequisicao = string.Empty;
                string jsonDadosRetornoRequisicao = string.Empty;
                string mensagemErroIntegracao = string.Empty;

                try
                {
                    Dominio.Entidades.Embarcador.Configuracoes.IntegracaoPiracanjuba configuracaoIntegracao = ObterConfiguracaoIntegracaoCarga();

                    jsonDadosRequisicao = JsonConvert.SerializeObject(documento, Formatting.None, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
                    StringContent conteudoRequisicao = new StringContent(jsonDadosRequisicao, Encoding.UTF8, "application/json");

                    HttpClient client = CriaRequisicao(configuracaoIntegracao);

                    HttpResponseMessage retornoRequisicao = client.PostAsync(configuracaoIntegracao.URLIntegracaoCargaPiracanjuba, conteudoRequisicao).Result;

                    jsonDadosRetornoRequisicao = retornoRequisicao.Content.ReadAsStringAsync().Result;
                    dynamic jsonRetorno = JsonConvert.DeserializeObject<dynamic>(jsonDadosRetornoRequisicao);

                    if (!IsRetornoIsRetornoSucessoSucesso(retornoRequisicao, jsonRetorno))
                    {
                        mensagemErroIntegracao = ObterMensagemDeErro(jsonRetorno, retornoRequisicao.StatusCode);
                        todosArquivosIntegradosComSucesso = false;
                    }
                }
                catch (Exception excecao)
                {
                    Log.TratarErro(excecao, "IntegracaoPiracanjuba");
                    mensagemErroIntegracao = "Ocorreu uma falha ao enviar documento";
                    todosArquivosIntegradosComSucesso = false;
                }

                if (cargaCargaIntegracao != null)
                    servicoArquivoTransacao.Adicionar(cargaCargaIntegracao, jsonDadosRequisicao, jsonDadosRetornoRequisicao, "json", mensagemErroIntegracao);
                if (cargaCancelamentoCargaIntegracao != null)
                    servicoArquivoTransacao.Adicionar(cargaCancelamentoCargaIntegracao, jsonDadosRequisicao, jsonDadosRetornoRequisicao, "json", mensagemErroIntegracao);

                Log.TratarErro($"Requisição: {jsonDadosRequisicao}\nResposta: {jsonDadosRetornoRequisicao}", "IntegracaoPiracanjuba");
            }

            return todosArquivosIntegradosComSucesso;
        }

        private bool IsRetornoIsRetornoSucessoSucesso(HttpResponseMessage retornoRequisicao, dynamic jsonDadosRetornoRequisicao)
        {
            if (retornoRequisicao.StatusCode == HttpStatusCode.OK || retornoRequisicao.StatusCode == HttpStatusCode.Created)
                return jsonDadosRetornoRequisicao.d.sucesso;

            return false;
        }

        private string ObterPDFCTe(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Repositorio.UnitOfWork unitOfWork)
        {
            if (cte == null) throw new ServicoException("Cte não localizada");

            if (cte.ModeloDocumentoFiscal.TipoDocumentoEmissao != Dominio.Enumeradores.TipoDocumento.CTe && cte.ModeloDocumentoFiscal.TipoDocumentoEmissao != Dominio.Enumeradores.TipoDocumento.NFSe && cte.ModeloDocumentoFiscal.TipoDocumentoEmissao != Dominio.Enumeradores.TipoDocumento.NFS)
            {
                if (string.IsNullOrWhiteSpace(cte.ModeloDocumentoFiscal.Relatorio))
                    return string.Empty;

                byte[] arquivo = new Relatorios.OutrosDocumentos(_unitOfWork).ObterPdf(cte);
                return ConverterByteParaString(arquivo);
            }

            if (cte.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe)
            {
                string nomeArquivo = cte.Empresa.CNPJ + "_" + cte.Numero.ToString() + "_" + cte.Serie.Numero.ToString() + ".pdf";
                string caminhoPDF = Utilidades.IO.FileStorageService.Storage.Combine(Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoRelatorios, cte.Empresa.CNPJ, nomeArquivo);
                byte[] danfse;

                if (Utilidades.IO.FileStorageService.Storage.Exists(caminhoPDF))
                    danfse = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminhoPDF);
                else
                {
                    danfse = _servicoNFSe.ObterDANFSECTe(cte.Codigo);
                }

                if (danfse == null)
                    return string.Empty;

                return ConverterByteParaString(danfse);
            }

            if (cte.Status.Equals("A") || cte.Status.Equals("F"))
            {
                return _servicoCTe.ObterDACTE(cte.Codigo, cte.Empresa.Codigo, _unitOfWork, ObterConfiguracaoEmbarcador().UtilizarCodificacaoUTF8ConversaoPDF);
            }

            return string.Empty;
        }

        private string ObterPDFMDFe(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe)
        {
            return _servicoMDFe.ObterDAMDFE(mdfe.Codigo, mdfe.Empresa.Codigo, _unitOfWork, false);
        }

        private string ObterPDFValePedagio(Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaIntegracaoValePedagio)
        {
            Servicos.Embarcador.Carga.ValePedagio.ValePedagio servicoValePedagio = new Servicos.Embarcador.Carga.ValePedagio.ValePedagio(_unitOfWork);

            byte[] arquivo = null;
            string retorno = servicoValePedagio.ObterArquivoValePedagio(cargaIntegracaoValePedagio, ref arquivo, _tipoServicoMultisoftware);

            if (!string.IsNullOrWhiteSpace(retorno))
                throw new ServicoException(retorno);

            if (arquivo == null)
                throw new ServicoException("Não foi possível gerar o Vale Pedágio");

            return Convert.ToBase64String(arquivo);
        }

        private string ConverterByteParaString(byte[] conteudo)
        {
            if (ObterConfiguracaoEmbarcador().UtilizarCodificacaoUTF8ConversaoPDF)
                return Convert.ToBase64String(Encoding.Convert(Encoding.GetEncoding("ISO-8859-1"), Encoding.UTF8, conteudo));
            else
                return Convert.ToBase64String(conteudo);
        }

        private bool IntegrarCTesCarga(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaCargaIntegracao)
        {
            if (cargaCargaIntegracao.Carga.Empresa.EmissaoDocumentosForaDoSistema)
                return true;

            Repositorio.XMLCTe repXMLCTe = new Repositorio.XMLCTe(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repPedidoXmlNotaFiscalCte = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoPiracanjuba integracao = ObterConfiguracaoIntegracaoCarga();
            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes = repCargaCTe.BuscarPorCarga(cargaCargaIntegracao.Carga.Codigo);
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.Piracanjuba.DocumentoCarga> documentos = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Piracanjuba.DocumentoCarga>();

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe in cargaCTes)
            {
                List<string> chavesXMLNotaFiscais = repPedidoXmlNotaFiscalCte.BuscarChaveXMLNotasFiscaisPorCTe(cargaCTe.Codigo);

                string pdfCTe = ObterPDFCTe(cargaCTe.CTe, _unitOfWork);

                documentos.Add(new Dominio.ObjetosDeValor.Embarcador.Integracao.Piracanjuba.DocumentoCarga()
                {
                    AmbienteProducao = integracao.AmbienteProducaoPiracanjuba,
                    StringAmbiente = integracao.StringAmbientePiracanjuba,
                    NumeroCarga = cargaCargaIntegracao.Carga.CodigoCargaEmbarcador,
                    TipoDocumento = Dominio.ObjetosDeValor.Embarcador.Integracao.Piracanjuba.TipoDocumento.CTe,
                    ValorDocumento = cargaCTe.CTe.ValorAReceber.ToString().Replace(',', '.'),
                    NumeroDocumento = cargaCTe.CTe.Numero.ToString(),
                    Chave = !string.IsNullOrWhiteSpace(cargaCTe.CTe.Chave) ? cargaCTe.CTe.Chave : string.Empty,
                    TipoArquivo = Dominio.ObjetosDeValor.Embarcador.Integracao.Piracanjuba.TipoArquivo.PDF,
                    ConteudoArquivo = pdfCTe,
                    ChaveNotaFiscal = string.Join("\\n", chavesXMLNotaFiscais)
                });

                Dominio.Entidades.XMLCTe xml = repXMLCTe.BuscarPorCTe(cargaCTe.CTe.Codigo, Dominio.Enumeradores.TipoXMLCTe.Autorizacao);

                documentos.Add(new Dominio.ObjetosDeValor.Embarcador.Integracao.Piracanjuba.DocumentoCarga()
                {
                    AmbienteProducao = integracao.AmbienteProducaoPiracanjuba,
                    StringAmbiente = integracao.StringAmbientePiracanjuba,
                    NumeroCarga = cargaCargaIntegracao.Carga.CodigoCargaEmbarcador,
                    TipoDocumento = Dominio.ObjetosDeValor.Embarcador.Integracao.Piracanjuba.TipoDocumento.CTe,
                    ValorDocumento = cargaCTe.CTe.ValorAReceber.ToString().Replace(',', '.'),
                    NumeroDocumento = cargaCTe.CTe.Numero.ToString(),
                    Chave = !string.IsNullOrWhiteSpace(cargaCTe.CTe.Chave) ? cargaCTe.CTe.Chave : string.Empty,
                    TipoArquivo = Dominio.ObjetosDeValor.Embarcador.Integracao.Piracanjuba.TipoArquivo.XML,
                    ConteudoArquivo = xml?.XML,
                });
            }

            return EnviarArquivosIntegracao(documentos, cargaCargaIntegracao);
        }

        private bool IntegrarMDFesCarga(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaCargaIntegracao)
        {
            if (cargaCargaIntegracao.Carga.Empresa.EmissaoDocumentosForaDoSistema)
                return true;

            Repositorio.Embarcador.Cargas.CargaMDFe repCargaMDFe = new Repositorio.Embarcador.Cargas.CargaMDFe(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoPiracanjuba integracao = ObterConfiguracaoIntegracaoCarga();
            List<Dominio.Entidades.Embarcador.Cargas.CargaMDFe> cargaMDFes = repCargaMDFe.BuscarPorCarga(cargaCargaIntegracao.Carga.Codigo);
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.Piracanjuba.DocumentoCarga> documentos = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Piracanjuba.DocumentoCarga>();

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaMDFe cargaMDFe in cargaMDFes)
            {
                string pdfMDFe = ObterPDFMDFe(cargaMDFe.MDFe);

                if (string.IsNullOrWhiteSpace(pdfMDFe) || cargaMDFe.MDFe.Status != Dominio.Enumeradores.StatusMDFe.Autorizado)
                    return true;

                documentos.Add(new Dominio.ObjetosDeValor.Embarcador.Integracao.Piracanjuba.DocumentoCarga()
                {
                    AmbienteProducao = integracao.AmbienteProducaoPiracanjuba,
                    StringAmbiente = integracao.StringAmbientePiracanjuba,
                    NumeroCarga = cargaCargaIntegracao.Carga.CodigoCargaEmbarcador,
                    TipoDocumento = Dominio.ObjetosDeValor.Embarcador.Integracao.Piracanjuba.TipoDocumento.MDFe,
                    NumeroDocumento = cargaMDFe.MDFe.Numero.ToString(),
                    Chave = !string.IsNullOrWhiteSpace(cargaMDFe.MDFe.Chave) ? cargaMDFe.MDFe.Chave : string.Empty,
                    TipoArquivo = Dominio.ObjetosDeValor.Embarcador.Integracao.Piracanjuba.TipoArquivo.PDF,
                    ConteudoArquivo = pdfMDFe,
                });
            }

            return EnviarArquivosIntegracao(documentos, cargaCargaIntegracao);
        }

        private bool IntegrarValesPedagioCarga(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaCargaIntegracao)
        {
            Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repCargaValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoPiracanjuba integracao = ObterConfiguracaoIntegracaoCarga();
            List<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio> cargasValePedagio = repCargaValePedagio.BuscarPorCarga(cargaCargaIntegracao.Carga.Codigo);
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.Piracanjuba.DocumentoCarga> documentos = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Piracanjuba.DocumentoCarga>();

            foreach (Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio valePedagio in cargasValePedagio)
            {
                if (valePedagio.ValorValePedagio > 0)
                {
                    string pdfValePedagio = ObterPDFValePedagio(valePedagio);

                    documentos.Add(new Dominio.ObjetosDeValor.Embarcador.Integracao.Piracanjuba.DocumentoCarga()
                    {
                        AmbienteProducao = integracao.AmbienteProducaoPiracanjuba,
                        StringAmbiente = integracao.StringAmbientePiracanjuba,
                        NumeroCarga = cargaCargaIntegracao.Carga.CodigoCargaEmbarcador,
                        TipoDocumento = Dominio.ObjetosDeValor.Embarcador.Integracao.Piracanjuba.TipoDocumento.ValePedagio,
                        ValorDocumento = valePedagio.ValorValePedagio.ToString().Replace(',', '.'),
                        NumeroDocumento = valePedagio.NumeroValePedagio ?? string.Empty,
                        TipoArquivo = Dominio.ObjetosDeValor.Embarcador.Integracao.Piracanjuba.TipoArquivo.PDF,
                        ConteudoArquivo = pdfValePedagio,
                    });
                }
            }

            return EnviarArquivosIntegracao(documentos, cargaCargaIntegracao);
        }

        private bool IntegrarFimDaIntegracao(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaCargaIntegracao)
        {
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoPiracanjuba integracao = ObterConfiguracaoIntegracaoCarga();

            Dominio.ObjetosDeValor.Embarcador.Integracao.Piracanjuba.DocumentoCarga documento = new Dominio.ObjetosDeValor.Embarcador.Integracao.Piracanjuba.DocumentoCarga()
            {
                AmbienteProducao = integracao.AmbienteProducaoPiracanjuba,
                StringAmbiente = integracao.StringAmbientePiracanjuba,
                NumeroCarga = cargaCargaIntegracao.Carga?.CodigoCargaEmbarcador ?? string.Empty,
                TipoDocumento = Dominio.ObjetosDeValor.Embarcador.Integracao.Piracanjuba.TipoDocumento.Fim,
                TipoArquivo = Dominio.ObjetosDeValor.Embarcador.Integracao.Piracanjuba.TipoArquivo.Fim,
                Componentes = AdicionarComponentes(cargaCargaIntegracao)
            };

            return EnviarArquivosIntegracao(new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Piracanjuba.DocumentoCarga>() { documento }, cargaCargaIntegracao);
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.Piracanjuba.Componente> AdicionarComponentes(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaCargaIntegracao)
        {
            if (cargaCargaIntegracao.Carga.Empresa.EmissaoDocumentosForaDoSistema)
                return new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Piracanjuba.Componente>();

            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargasPedido = repositorioCargaPedido.BuscarPorCarga(cargaCargaIntegracao.Carga.Codigo);
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.Piracanjuba.Componente> componentes = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Piracanjuba.Componente>();

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargasPedido)
            {
                componentes.Add(new Dominio.ObjetosDeValor.Embarcador.Integracao.Piracanjuba.Componente()
                {
                    Codigo = "Z_ICMS",
                    Pedido = cargaPedido.Pedido.NumeroPedidoEmbarcador,
                    Valor = cargaPedido.ValorICMS.ToString().Replace(',', '.')
                });
            }

            return componentes.DistinctBy(x => x.Pedido).ToList();
        }

        private void IntegrarDocumentosCarga(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaCargaIntegracao)
        {
            bool integradoComSucesso = true;
            List<string> mensagemIntegracaoFalha = new List<string>();

            try
            {
                if (!IntegrarCTesCarga(cargaCargaIntegracao))
                {
                    integradoComSucesso = false;
                    mensagemIntegracaoFalha.Add($"Falha ao integrar CTes");
                }
            }
            catch (ServicoException excecao)
            {
                mensagemIntegracaoFalha.Add($"Falha ao integrar CTes ({excecao.Message})");
                integradoComSucesso = false;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao, "IntegracaoPiracanjuba");
                mensagemIntegracaoFalha.Add($"Falha ao integrar CTes");
                integradoComSucesso = false;
            }

            try
            {
                if (!IntegrarMDFesCarga(cargaCargaIntegracao))
                {
                    integradoComSucesso = false;
                    mensagemIntegracaoFalha.Add($"Falha ao integrar MDFes");
                }
            }
            catch (ServicoException excecao)
            {
                mensagemIntegracaoFalha.Add($"Falha ao integrar MDFes ({excecao.Message})");
                integradoComSucesso = false;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao, "IntegracaoPiracanjuba");
                mensagemIntegracaoFalha.Add($"Falha ao integrar MDFes");
                integradoComSucesso = false;
            }

            try
            {
                if (!IntegrarValesPedagioCarga(cargaCargaIntegracao))
                {
                    integradoComSucesso = false;
                    mensagemIntegracaoFalha.Add($"Falha ao integrar Vales Pedágio");
                }
            }
            catch (ServicoException excecao)
            {
                mensagemIntegracaoFalha.Add($"Falha ao integrar Vales Pedágio ({excecao.Message})");
                integradoComSucesso = false;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao, "IntegracaoPiracanjuba");
                mensagemIntegracaoFalha.Add($"Falha ao integrar Vales Pedágio");
                integradoComSucesso = false;
            }


            if (integradoComSucesso)
            {
                try
                {
                    if (!IntegrarFimDaIntegracao(cargaCargaIntegracao))
                    {
                        integradoComSucesso = false;
                        mensagemIntegracaoFalha.Add($"Falha ao integrar o sinal de finalização");
                    }
                }
                catch (ServicoException excecao)
                {
                    mensagemIntegracaoFalha.Add($"Falha ao integrar o sinal de finalização ({excecao.Message})");
                    integradoComSucesso = false;
                }
                catch (Exception excecao)
                {
                    Log.TratarErro(excecao, "IntegracaoPiracanjuba");
                    mensagemIntegracaoFalha.Add($"Falha ao integrar o sinal de finalização");
                    integradoComSucesso = false;
                }
            }

            if (integradoComSucesso)
            {
                cargaCargaIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                cargaCargaIntegracao.ProblemaIntegracao = "Integrado com sucesso";
            }
            else
            {
                cargaCargaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaCargaIntegracao.ProblemaIntegracao = string.Join("; ", mensagemIntegracaoFalha);
            }

        }

        private void IntegrarCTe(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoPiracanjuba integracao = ObterConfiguracaoIntegracaoCarga();

            Dominio.ObjetosDeValor.Embarcador.Integracao.Piracanjuba.DocumentoCarga documento = new Dominio.ObjetosDeValor.Embarcador.Integracao.Piracanjuba.DocumentoCarga()
            {
                AmbienteProducao = integracao.AmbienteProducaoPiracanjuba,
                StringAmbiente = integracao.StringAmbientePiracanjuba,
                NumeroCarga = carga?.CodigoCargaEmbarcador ?? string.Empty,
                TipoDocumento = Dominio.ObjetosDeValor.Embarcador.Integracao.Piracanjuba.TipoDocumento.CTe,
                NumeroDocumento = cte.Numero.ToString(),
                Chave = !string.IsNullOrWhiteSpace(cte.Chave) ? cte.Chave : string.Empty,
                TipoArquivo = Dominio.ObjetosDeValor.Embarcador.Integracao.Piracanjuba.TipoArquivo.Erro,
                ConteudoArquivo = cte.MensagemStatus?.MensagemDoErro ?? Utilidades.String.RemoveAccents(cte.MensagemRetornoSefaz),
                ChaveNotaFiscal = string.Empty,
            };

            EnviarArquivosIntegracao(new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Piracanjuba.DocumentoCarga>() { documento });
        }

        private void IntegrarMDFe(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoPiracanjuba integracao = ObterConfiguracaoIntegracaoCarga();

            Dominio.ObjetosDeValor.Embarcador.Integracao.Piracanjuba.DocumentoCarga documento = new Dominio.ObjetosDeValor.Embarcador.Integracao.Piracanjuba.DocumentoCarga()
            {
                AmbienteProducao = integracao.AmbienteProducaoPiracanjuba,
                StringAmbiente = integracao.StringAmbientePiracanjuba,
                NumeroCarga = carga?.CodigoCargaEmbarcador ?? string.Empty,
                TipoDocumento = Dominio.ObjetosDeValor.Embarcador.Integracao.Piracanjuba.TipoDocumento.MDFe,
                NumeroDocumento = mdfe.Numero.ToString(),
                Chave = !string.IsNullOrWhiteSpace(mdfe.Chave) ? mdfe.Chave : string.Empty,
                TipoArquivo = Dominio.ObjetosDeValor.Embarcador.Integracao.Piracanjuba.TipoArquivo.Erro,
                ConteudoArquivo = mdfe.MensagemStatus?.MensagemDoErro ?? Utilidades.String.RemoveAccents(mdfe.MensagemRetornoSefaz),
                ChaveNotaFiscal = string.Empty,
            };

            EnviarArquivosIntegracao(new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Piracanjuba.DocumentoCarga>() { documento });
        }

        private void IntegrarMDFePendencia(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoPiracanjuba integracao = ObterConfiguracaoIntegracaoCarga();

            Dominio.ObjetosDeValor.Embarcador.Integracao.Piracanjuba.DocumentoCarga documento = new Dominio.ObjetosDeValor.Embarcador.Integracao.Piracanjuba.DocumentoCarga()
            {
                AmbienteProducao = integracao.AmbienteProducaoPiracanjuba,
                StringAmbiente = integracao.StringAmbientePiracanjuba,
                NumeroCarga = carga.CodigoCargaEmbarcador,
                TipoDocumento = Dominio.ObjetosDeValor.Embarcador.Integracao.Piracanjuba.TipoDocumento.MDFe,
                NumeroDocumento = string.Empty,
                Chave = string.Empty,
                TipoArquivo = Dominio.ObjetosDeValor.Embarcador.Integracao.Piracanjuba.TipoArquivo.Erro,
                ConteudoArquivo = carga.MotivoPendencia,
                ChaveNotaFiscal = string.Empty,
            };

            EnviarArquivosIntegracao(new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Piracanjuba.DocumentoCarga>() { documento });
        }

        private void IntegrarValePedagio(Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaValePedagio)
        {
            cargaValePedagio.NumeroTentativas++;
            cargaValePedagio.DataIntegracao = DateTime.Now;

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoPiracanjuba integracao = ObterConfiguracaoIntegracaoCarga();

            Dominio.ObjetosDeValor.Embarcador.Integracao.Piracanjuba.DocumentoCarga documento = new Dominio.ObjetosDeValor.Embarcador.Integracao.Piracanjuba.DocumentoCarga()
            {
                AmbienteProducao = integracao.AmbienteProducaoPiracanjuba,
                StringAmbiente = integracao.StringAmbientePiracanjuba,
                NumeroCarga = cargaValePedagio.Carga.CodigoCargaEmbarcador,
                TipoDocumento = Dominio.ObjetosDeValor.Embarcador.Integracao.Piracanjuba.TipoDocumento.ValePedagio,
                NumeroDocumento = cargaValePedagio.NumeroValePedagio ?? string.Empty,
                Chave = string.Empty,
                TipoArquivo = Dominio.ObjetosDeValor.Embarcador.Integracao.Piracanjuba.TipoArquivo.Erro,
                ConteudoArquivo = Utilidades.String.RemoveAccents(cargaValePedagio.ProblemaIntegracao),
                ChaveNotaFiscal = string.Empty
            };

            EnviarArquivosIntegracao(new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Piracanjuba.DocumentoCarga>() { documento });
        }

        private void IntegrarCancelamento(Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaIntegracao cargaCancelamentoCargaIntegracao)
        {
            cargaCancelamentoCargaIntegracao.NumeroTentativas++;
            cargaCancelamentoCargaIntegracao.DataIntegracao = DateTime.Now;

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoPiracanjuba integracao = ObterConfiguracaoIntegracaoCarga();

            Dominio.ObjetosDeValor.Embarcador.Integracao.Piracanjuba.DocumentoCarga documento = new Dominio.ObjetosDeValor.Embarcador.Integracao.Piracanjuba.DocumentoCarga()
            {
                AmbienteProducao = integracao.AmbienteProducaoPiracanjuba,
                StringAmbiente = integracao.StringAmbientePiracanjuba,
                NumeroCarga = cargaCancelamentoCargaIntegracao.CargaCancelamento.Carga.CodigoCargaEmbarcador,
                TipoArquivo = Dominio.ObjetosDeValor.Embarcador.Integracao.Piracanjuba.TipoArquivo.Del,
            };

            if (EnviarArquivosIntegracao(new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Piracanjuba.DocumentoCarga>() { documento }, null, cargaCancelamentoCargaIntegracao))
            {
                cargaCancelamentoCargaIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                cargaCancelamentoCargaIntegracao.ProblemaIntegracao = "Integrado com sucesso";
                return;
            }

            cargaCancelamentoCargaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
            cargaCancelamentoCargaIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao enviar documento";
        }

        #endregion

        #region Métodos Públicos       
        public void IntegracarCanhoto(Dominio.Entidades.Embarcador.Canhotos.CanhotoIntegracao canhotoIntegracao)
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            Repositorio.Embarcador.Canhotos.CanhotoIntegracao repCanhotoIntegracao = new Repositorio.Embarcador.Canhotos.CanhotoIntegracao(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoPiracanjuba integracao = ObterConfiguracaoIntegracaoCarga();

            canhotoIntegracao.DataIntegracao = DateTime.Now;
            canhotoIntegracao.NumeroTentativas++;

            Dominio.ObjetosDeValor.Embarcador.Integracao.Piracanjuba.CanhotoNF canhotoNF = new Dominio.ObjetosDeValor.Embarcador.Integracao.Piracanjuba.CanhotoNF();

            Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto = canhotoIntegracao.Canhoto;

            DateTime? dataEntrega = null;
            if (canhoto.DataEntregaNotaCliente == null)
                dataEntrega = repCanhotoIntegracao.BuscarDataEntregaCliente(canhoto.XMLNotaFiscal.Codigo);

            List<string> chavesNF = new List<string>();

            if (canhoto.TipoCanhoto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCanhoto.Avulso)
            {
                var chavesCanhotoAvulso = (from pedidosXMLNotasFiscais in canhoto.CanhotoAvulso.PedidosXMLNotasFiscais select pedidosXMLNotasFiscais.XMLNotaFiscal.Chave).ToList();
                chavesNF.AddRange(chavesCanhotoAvulso);
            }
            else
                chavesNF.Add(canhoto?.XMLNotaFiscal?.Chave);

            if (string.IsNullOrWhiteSpace(integracao.StringAmbientePiracanjuba))
            {
                if (integracao.AmbienteProducaoPiracanjuba)
                    canhotoNF.instance = "prd";
                else
                    canhotoNF.instance = "hml";
            }
            else
                canhotoNF.instance = integracao.StringAmbientePiracanjuba;

            canhotoNF.dataEntrega = canhoto.DataEntregaNotaCliente?.ToString("yyyy-MM-dd HH:mm:ss") ?? (dataEntrega != null ? dataEntrega.Value.ToString("yyyy-MM-dd HH:mm:ss") : "");
            canhotoNF.numCarga = canhoto.Carga?.CodigoCargaEmbarcador?.PadLeft(10, '0') ?? string.Empty;
            canhotoNF.dataRegistro = canhoto.DataEnvioCanhoto.ToString("yyyy-MM-dd HH:mm:ss");
            canhotoNF.latitude = "";
            canhotoNF.longitude = "";
            canhotoNF.recebidoPor = "";
            canhotoNF.documentoRecebedor = "";
            canhotoNF.statusEntrega = (canhoto.CanhotoAvulso != null || canhoto.XMLNotaFiscal.SituacaoEntregaNotaFiscal != SituacaoNotaFiscal.NaoEntregue) ? "Concluído" : "Não Entregue";
            canhotoNF.tipoDeCanhoto = canhoto.DescricaoTipoCanhoto;

            string extensao = System.IO.Path.GetExtension(canhoto.NomeArquivo).ToLower();
            string caminho = Servicos.Embarcador.Canhotos.Canhoto.CaminhoCanhoto(canhoto, _unitOfWork);
            string fileLocation = Utilidades.IO.FileStorageService.Storage.Combine(caminho, canhoto.GuidNomeArquivo + extensao);

            byte[] bufferCanhoto = null;

            if (!Utilidades.IO.FileStorageService.Storage.Exists(fileLocation) && (canhoto?.XMLNotaFiscal?.SituacaoEntregaNotaFiscal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoNotaFiscal.NaoEntregue))
            {
                canhotoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                canhotoIntegracao.ProblemaIntegracao = "Não foi localizado o arquivo da imagem para envio.";
            }
            else
            {

                if (canhoto?.XMLNotaFiscal?.SituacaoEntregaNotaFiscal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoNotaFiscal.NaoEntregue)
                {
                    bufferCanhoto = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(fileLocation);
                    canhotoNF.canhoto = Convert.ToBase64String(bufferCanhoto);

                    if (extensao == ".jpg")
                        canhotoNF.canhotoMimeType = "image/jpeg";
                    else if (extensao == ".pdf")
                        canhotoNF.canhotoMimeType = "application/pdf";
                    else
                        canhotoNF.canhotoMimeType = "image/" + extensao.Replace(".", "");
                }
                string jsonRequest = string.Empty;
                string jsonResponse = string.Empty;

                try
                {
                    string urlCanhoto = integracao.URLIntegracaoCanhotoPiracanjuba;

                    if (!string.IsNullOrEmpty(integracao.URLIntegracaoCanhotoPiracanjubaContingencia) && integracao.DataFaturamentoNota != null)
                    {
                        bool condicaoDataEmissaoNota = canhoto.XMLNotaFiscal?.DataEmissao < integracao.DataFaturamentoNota;

                        bool condicaoCanhotoAvulso = false;
                        if (!condicaoDataEmissaoNota && canhoto.CanhotoAvulso != null)
                        {
                            Repositorio.Embarcador.Canhotos.CanhotoAvulso repositorioCanhotoAvulso = new Repositorio.Embarcador.Canhotos.CanhotoAvulso(_unitOfWork);
                            condicaoCanhotoAvulso = repositorioCanhotoAvulso.BuscarDataFaturamentoPrimeiraNota(canhoto.CanhotoAvulso.Codigo) < integracao.DataFaturamentoNota;
                        }

                        if (condicaoDataEmissaoNota || condicaoCanhotoAvulso)
                        {
                            urlCanhoto = integracao.URLIntegracaoCanhotoPiracanjubaContingencia;
                        }
                    }

                    foreach (string chaveNF in chavesNF)
                    {
                        canhotoNF.chaveNF = chaveNF;
                        HttpClient client = CriaRequisicao(integracao);

                        jsonRequest = JsonConvert.SerializeObject(canhotoNF, Formatting.Indented);
                        StringContent content = new StringContent(jsonRequest.ToString(), Encoding.UTF8, "application/json");

                        HttpResponseMessage result = client.PostAsync(urlCanhoto, content).Result;

                        if (result.StatusCode == System.Net.HttpStatusCode.OK || result.StatusCode == System.Net.HttpStatusCode.Created)
                        {
                            jsonResponse = result.Content.ReadAsStringAsync().Result;

                            Dominio.ObjetosDeValor.Embarcador.Integracao.Piracanjuba.RetornoCanhoto retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Piracanjuba.RetornoCanhoto>(result.Content.ReadAsStringAsync().Result);

                            if (retorno.DadosProcessamento?.Count > 0 ? retorno.DadosProcessamento.Exists(x => x.Sucesso) : retorno.Success)
                                canhotoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                            else
                                canhotoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                            canhotoIntegracao.ProblemaIntegracao = retorno.Msg ?? retorno.DadosProcessamento.FirstOrDefault()?.Message;

                            servicoArquivoTransacao.Adicionar(canhotoIntegracao, jsonRequest, jsonResponse, "json", canhotoIntegracao.ProblemaIntegracao ?? "");

                            Log.TratarErro($"Requisição: {jsonRequest}\nResposta: {jsonResponse}", "IntegracaoPiracanjuba");
                        }
                        else
                        {
                            canhotoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                            canhotoIntegracao.ProblemaIntegracao = "Falha ao conectar no WS Piracanjuba";

                            servicoArquivoTransacao.Adicionar(canhotoIntegracao, jsonRequest, jsonResponse, "json", canhotoIntegracao.ProblemaIntegracao ?? "");
                            Log.TratarErro($"Requisição: {jsonRequest}\nResposta: {jsonResponse}", "IntegracaoPiracanjuba");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.TratarErro(ex, "IntegracaoPiracanjuba");
                    canhotoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    canhotoIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao integrar";

                    servicoArquivoTransacao.Adicionar(canhotoIntegracao, jsonRequest, jsonResponse, "json", canhotoIntegracao.ProblemaIntegracao ?? "");
                    Log.TratarErro($"Requisição: {jsonRequest}\nResposta: {jsonResponse}", "IntegracaoPiracanjuba");
                }

            }

            repCanhotoIntegracao.Atualizar(canhotoIntegracao);
        }

        public void IntegrarCarga(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaCargaIntegracao)
        {
            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repCargaCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(_unitOfWork);

            cargaCargaIntegracao.DataIntegracao = DateTime.Now;
            cargaCargaIntegracao.NumeroTentativas++;

            try
            {
                IntegrarDocumentosCarga(cargaCargaIntegracao);
            }
            catch (ServicoException excecao)
            {
                cargaCargaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaCargaIntegracao.ProblemaIntegracao = excecao.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao, "IntegracaoPiracanjuba");

                cargaCargaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaCargaIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração com a Piracanjuba";
            }

            repCargaCargaIntegracao.Atualizar(cargaCargaIntegracao);
        }

        public void IntegrarCTeRejeitado(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)
        {
            try
            {
                Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(_unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCte = repCargaCTe.BuscarPorCTe(cte.Codigo);

                IntegrarCTe(cte, cargaCte?.Carga);
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao, "IntegracaoPiracanjuba");
            }
        }

        public void IntegrarMDFeRejeitado(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe)
        {
            try
            {
                Repositorio.Embarcador.Cargas.CargaMDFe repCargaMDFe = new Repositorio.Embarcador.Cargas.CargaMDFe(_unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaMDFe cargaMdfe = repCargaMDFe.BuscarPorMDFe(mdfe.Codigo);

                IntegrarMDFe(mdfe, cargaMdfe?.Carga);
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao, "IntegracaoPiracanjuba");
            }

        }

        public void IntegrarMDFeComPendencia(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            try
            {
                IntegrarMDFePendencia(carga);
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao, "IntegracaoPiracanjuba");
            }

        }

        public void IntegrarFalhaDeValePedagio(Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaValePedagio)
        {
            try
            {
                IntegrarValePedagio(cargaValePedagio);
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao, "IntegracaoPiracanjuba");
            }
        }

        public void IntegrarCargaCancelamento(Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaIntegracao cargaIntegracao)
        {
            try
            {
                IntegrarCancelamento(cargaIntegracao);
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao, "IntegracaoPiracanjuba");
            }
        }

        #endregion
    }
}
