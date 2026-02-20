using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
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
using System.Threading;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Integracao.Magalog
{
    public class IntegracaoMagalog
    {
        #region Atributos Globais

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion

        #region Construtores

        public IntegracaoMagalog(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Métodos Públicos - Embarcador

        public static bool IntegracaoKeycloak()
        {
            string authToken = ObterTokenKeycloak("https://keycloak-staging.luizalabs.com/auth/realms/B2B/protocol/openid-connect/token", "teste-api", "38d16aa5-1495-4dc9-84eb-900fd5f11392", "client_credentials");

            if (string.IsNullOrWhiteSpace(authToken))
            {
                return false;
            }

            var caminhoXml = Servicos.FS.GetPath("C:\\Temp\\35200924230747000285570100000052501000525096.xml");

            string jsonResponse = string.Empty;
            string jsonRequest = string.Empty;

            EnviarRetornoEscrituracao("http://shenzi-api-staging.magazineluiza.com.br/scar/v1/recivexml", authToken, caminhoXml, "A", "CTE", "1P", "1P", ref jsonRequest, ref jsonResponse);

            return true;
        }

        public static void IntegrarCarga(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repositorioCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(unitOfWork);
            Repositorio.Embarcador.Configuracoes.Integracao repositorioConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
            //Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(unitOfWork);

            Dominio.Entidades.Empresa empresaPai = repEmpresa.BuscarEmpresaPai();

            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repositorioConfiguracaoIntegracao.Buscar();

            if ((configuracaoIntegracao == null) || !configuracaoIntegracao.PossuiIntegracaoMagalog || string.IsNullOrWhiteSpace(configuracaoIntegracao.URLProducaoMagalog))
            {
                cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                cargaIntegracao.ProblemaIntegracao = "Não existe configuração de integração disponível para a Magalog.";
            }
            else
            {
                cargaIntegracao.NumeroTentativas += 1;
                cargaIntegracao.DataIntegracao = DateTime.Now;

                string endPoint = (empresaPai.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Producao) ? configuracaoIntegracao.URLProducaoMagalog : configuracaoIntegracao.URLHomologacaoMagalog;
                if (!endPoint.Contains("loadIntegration"))
                    endPoint = string.Concat(endPoint, "loadIntegration");
                string mensagemErro = string.Empty;
                string clientRequestContent = string.Empty;
                string clientResponseContent = string.Empty;

                try
                {
                    bool retorno = RetornarCarga(cargaIntegracao.Carga, ref cargaIntegracao, endPoint, configuracaoIntegracao.TokenMagalog, unitOfWork, ref clientRequestContent, ref clientResponseContent, out mensagemErro);

                    if (!retorno)
                    {
                        cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                        cargaIntegracao.ProblemaIntegracao = mensagemErro;

                        //Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo();
                        //arquivoIntegracao.Data = cargaIntegracao.DataIntegracao;
                        //arquivoIntegracao.Mensagem = cargaIntegracao.ProblemaIntegracao;
                        //arquivoIntegracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;

                        ////arquivoIntegracao.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(clientRequestContent, "json", unitOfWork);
                        //arquivoIntegracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(clientResponseContent, "json", unitOfWork);

                        //repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

                        //cargaIntegracao.ArquivosTransacao.Add(arquivoIntegracao);
                    }
                    else
                    {
                        cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                        cargaIntegracao.ProblemaIntegracao = string.Empty;
                        repositorioCargaIntegracao.Atualizar(cargaIntegracao);

                        //Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo();
                        //arquivoIntegracao.Data = cargaIntegracao.DataIntegracao;
                        //arquivoIntegracao.Mensagem = cargaIntegracao.ProblemaIntegracao;
                        //arquivoIntegracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;

                        //arquivoIntegracao.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(clientRequestContent, "json", unitOfWork);
                        //arquivoIntegracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(clientResponseContent, "json", unitOfWork);

                        //repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

                        //cargaIntegracao.ArquivosTransacao.Add(arquivoIntegracao);
                    }
                }
                catch (Exception excecao)
                {
                    Servicos.Log.TratarErro(excecao, "IntegracaoMagalog");
                    cargaIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao comunicar com o Web Service da Magalog.";
                    cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                    Servicos.Log.TratarErro(JsonConvert.SerializeObject(clientRequestContent), "IntegracaoMagalog");
                    Servicos.Log.TratarErro(JsonConvert.SerializeObject(clientResponseContent), "IntegracaoMagalog");
                }
            }

            repositorioCargaIntegracao.Atualizar(cargaIntegracao);
        }

        public static void IntegrarCargaEscrituracao(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repositorioCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(unitOfWork);
            Repositorio.Embarcador.Configuracoes.Integracao repositorioConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
            Repositorio.XMLCTe repXMLCTe = new Repositorio.XMLCTe(unitOfWork);

            Dominio.Entidades.Empresa empresaPai = repEmpresa.BuscarEmpresaPai();

            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repositorioConfiguracaoIntegracao.Buscar();

            if ((configuracaoIntegracao == null) || !configuracaoIntegracao.PossuiIntegracaoMagalog || string.IsNullOrWhiteSpace(configuracaoIntegracao.URLEscrituracaoMagalog))
            {
                cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                cargaIntegracao.ProblemaIntegracao = "Não existe configuração de integração disponível para a Magalog.";
            }
            else
            {
                cargaIntegracao.NumeroTentativas += 1;
                cargaIntegracao.DataIntegracao = DateTime.Now;

                string authToken = ObterTokenKeycloak(configuracaoIntegracao.URLKeyCloacMagalog, configuracaoIntegracao.IDKeyCloacMagalog, configuracaoIntegracao.SecretKeyCloacMagalog, "client_credentials");

                if (string.IsNullOrWhiteSpace(authToken))
                {
                    cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    cargaIntegracao.ProblemaIntegracao = "Keycloac não retornou token de integração.";
                }
                else
                {
                    //List<int> listaCargaPedidos = repCargaPedido.BuscarCodigosPorCarga(cargaIntegracao.Carga.Codigo);

                    //foreach (int codigoCargaPedido in listaCargaPedidos)
                    //{
                    List<int> listaCargaCTe = repCargaCTe.BuscarCodigosPorCarga(cargaIntegracao.Carga.Codigo);

                    if (listaCargaCTe.Count > 0)
                    {
                        foreach (int codigoCargaCTe in listaCargaCTe)
                        {
                            Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe = repCargaCTe.BuscarPorCodigo(codigoCargaCTe);

                            if (cargaCTe != null && cargaCTe.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe)
                            {
                                Servicos.Log.TratarErro("Retornando CTe " + cargaCTe.CTe.Chave, "IntegracaoMagalogEscrituracao");

                                var xmlCTE = repXMLCTe.BuscarPorCTe(cargaCTe.CTe.Codigo, Dominio.Enumeradores.TipoXMLCTe.Autorizacao);

                                if (xmlCTE == null)
                                {
                                    cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                                    cargaIntegracao.ProblemaIntegracao = "XML CTe não disponível.";
                                    Servicos.Log.TratarErro("XML CTe não disponível " + cargaCTe.CTe.Chave, "IntegracaoMagalogEscrituracao");
                                }
                                else
                                {
                                    var caminhoXml = string.Empty;

                                    Servicos.CTe serCTe = new Servicos.CTe(unitOfWork);
                                    caminhoXml = serCTe.ObterCaminhoArmazenamentoXMLCTeArquivo(cargaCTe.CTe, "A", unitOfWork);

                                    if (!Utilidades.IO.FileStorageService.Storage.Exists(caminhoXml))
                                    {
                                        if (!xmlCTE.XMLArmazenadoEmArquivo)
                                        {
                                            byte[] data = System.Text.Encoding.UTF8.GetBytes(xmlCTE.XML);
                                            Utilidades.IO.FileStorageService.Storage.WriteAllBytes(caminhoXml, data);
                                        }
                                    }


                                    if (!Utilidades.IO.FileStorageService.Storage.Exists(caminhoXml))
                                    {
                                        cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                                        cargaIntegracao.ProblemaIntegracao = "Arquivo XML CTe não disponível.";

                                        Servicos.Log.TratarErro("XML CTe não disponível " + cargaCTe.CTe.Chave, "IntegracaoMagalogEscrituracao");
                                    }
                                    else
                                    {
                                        string jsonResponse = string.Empty;
                                        string jsonRequest = string.Empty;

                                        try
                                        {
                                            string tipoCTe = cargaCTe.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe ? cargaCTe.CTe.Status == "C" ? "CTECanc" : cargaCTe.CTe.Status == "I" ? "CTEInut" : "CTE" : "NFSe";

                                            bool retorno = EnviarRetornoEscrituracao(configuracaoIntegracao.URLEscrituracaoMagalog, authToken, caminhoXml, cargaCTe.CTe.Status, tipoCTe, cargaCTe.Carga.TipoOperacao?.Descricao, cargaCTe.Carga.TipoOperacao?.CodigoIntegracao, ref jsonRequest, ref jsonResponse);

                                            if (!retorno)
                                            {
                                                cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                                                cargaIntegracao.ProblemaIntegracao = "Falha na integração, consulte o histórico para verificação.";

                                                Servicos.Log.TratarErro("Falha na integração CTe " + cargaCTe.CTe.Chave, "IntegracaoMagalogEscrituracao");

                                                Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo();
                                                arquivoIntegracao.Data = cargaIntegracao.DataIntegracao;
                                                arquivoIntegracao.Mensagem = cargaIntegracao.ProblemaIntegracao;
                                                arquivoIntegracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;

                                                arquivoIntegracao.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonRequest, "json", unitOfWork);
                                                arquivoIntegracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonResponse, "json", unitOfWork);

                                                repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);
                                                cargaIntegracao.ArquivosTransacao.Add(arquivoIntegracao);
                                            }
                                            else
                                            {
                                                cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                                                cargaIntegracao.ProblemaIntegracao = string.Empty;
                                                repositorioCargaIntegracao.Atualizar(cargaIntegracao);

                                                Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo();
                                                arquivoIntegracao.Data = cargaIntegracao.DataIntegracao;
                                                arquivoIntegracao.Mensagem = cargaIntegracao.ProblemaIntegracao;
                                                arquivoIntegracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;

                                                arquivoIntegracao.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonRequest, "json", unitOfWork);
                                                arquivoIntegracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonResponse, "json", unitOfWork);

                                                repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);
                                                cargaIntegracao.ArquivosTransacao.Add(arquivoIntegracao);
                                            }
                                        }
                                        catch (Exception excecao)
                                        {
                                            Servicos.Log.TratarErro(excecao, "IntegrarCargaEscrituracao");
                                            cargaIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao comunicar com o Web Service da Magalog.";
                                            cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                                        }
                                    }
                                }
                            }
                            else if (cargaCTe != null && cargaCTe.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao != Dominio.Enumeradores.TipoDocumento.CTe)
                            {
                                cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                                cargaIntegracao.ProblemaIntegracao = "Tipo de documento não disponível para retorno.";
                            }
                            else if (cargaCTe == null)
                            {
                                //if (cargaPedido?.PossuiNFSManual ?? false)
                                //{
                                //    cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                                //    cargaIntegracao.ProblemaIntegracao = "Não disponível retorno para NFS Manual.";
                                //}
                                //else
                                //{
                                cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                                cargaIntegracao.ProblemaIntegracao = "Documento não disponível para retorno.";
                                //}
                            }
                        }
                    }
                    else
                    {
                        cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                        cargaIntegracao.ProblemaIntegracao = "Carga sen CTE disponível para retorno.";
                    }
                    //}
                }
            }

            repositorioCargaIntegracao.Atualizar(cargaIntegracao);
        }

        public static void CancelarCargaEscrituracao(Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaIntegracao cargaIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCancelamentoCargaIntegracao repositorioCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCancelamentoCargaIntegracao(unitOfWork);
            Repositorio.Embarcador.Configuracoes.Integracao repositorioConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
            Repositorio.XMLCTe repXMLCTe = new Repositorio.XMLCTe(unitOfWork);

            Dominio.Entidades.Empresa empresaPai = repEmpresa.BuscarEmpresaPai();

            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repositorioConfiguracaoIntegracao.Buscar();

            if ((configuracaoIntegracao == null) || !configuracaoIntegracao.PossuiIntegracaoMagalog || string.IsNullOrWhiteSpace(configuracaoIntegracao.URLEscrituracaoMagalog))
            {
                cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                cargaIntegracao.ProblemaIntegracao = "Não existe configuração de integração disponível para a Magalog.";
            }
            else
            {
                cargaIntegracao.NumeroTentativas += 1;
                cargaIntegracao.DataIntegracao = DateTime.Now;

                string authToken = ObterTokenKeycloak(configuracaoIntegracao.URLKeyCloacMagalog, configuracaoIntegracao.IDKeyCloacMagalog, configuracaoIntegracao.SecretKeyCloacMagalog, "client_credentials");

                if (string.IsNullOrWhiteSpace(authToken))
                {
                    cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    cargaIntegracao.ProblemaIntegracao = "Keycloac não retornou token de integração.";
                }
                else
                {
                    List<int> listaCargaCTe = repCargaCTe.BuscarCodigosPorCarga(cargaIntegracao.CargaCancelamento.Carga.Codigo);
                    foreach (var codigoCargaCTe in listaCargaCTe)
                    {
                        Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe = repCargaCTe.BuscarPorCodigo(codigoCargaCTe);

                        if (cargaCTe != null && cargaCTe.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe)
                        {
                            Dominio.Entidades.XMLCTe xmlCTE = null;

                            if (cargaCTe.CTe.Status == "C" || cargaCTe.CTe.Status == "I")
                                xmlCTE = repXMLCTe.BuscarPorCTe(cargaCTe.CTe.Codigo, Dominio.Enumeradores.TipoXMLCTe.Cancelamento);
                            else
                                xmlCTE = repXMLCTe.BuscarPorCTe(cargaCTe.CTe.Codigo, Dominio.Enumeradores.TipoXMLCTe.Autorizacao);

                            if (xmlCTE == null)
                            {
                                cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                                cargaIntegracao.ProblemaIntegracao = "XML CTe não disponível.";
                            }
                            else
                            {
                                var caminhoXml = string.Empty;
                                string tipo = cargaCTe.CTe.Status == "C" || cargaCTe.CTe.Status == "I" ? "C" : "A";

                                Servicos.CTe serCTe = new Servicos.CTe(unitOfWork);
                                caminhoXml = serCTe.ObterCaminhoArmazenamentoXMLCTeArquivo(cargaCTe.CTe, tipo, unitOfWork);

                                if (!Utilidades.IO.FileStorageService.Storage.Exists(caminhoXml))
                                {
                                    if (!xmlCTE.XMLArmazenadoEmArquivo)
                                    {
                                        byte[] data = System.Text.Encoding.UTF8.GetBytes(xmlCTE.XML);
                                        Utilidades.IO.FileStorageService.Storage.WriteAllBytes(caminhoXml, data);
                                    }
                                }


                                if (!Utilidades.IO.FileStorageService.Storage.Exists(caminhoXml))
                                {
                                    cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                                    cargaIntegracao.ProblemaIntegracao = "Arquivo XML CTe não disponível.";
                                }
                                else
                                {
                                    string jsonResponse = string.Empty;
                                    string jsonRequest = string.Empty;

                                    try
                                    {
                                        string tipoCTe = cargaCTe.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe ? cargaCTe.CTe.Status == "C" ? "CTECanc" : cargaCTe.CTe.Status == "I" ? "CTEInut" : "CTE" : "NFSe";

                                        bool retorno = EnviarRetornoEscrituracao(configuracaoIntegracao.URLEscrituracaoMagalog, authToken, caminhoXml, cargaCTe.CTe.Status, tipoCTe, cargaCTe.Carga.TipoOperacao?.Descricao, cargaCTe.Carga.TipoOperacao?.CodigoIntegracao, ref jsonRequest, ref jsonResponse);

                                        if (!retorno)
                                        {
                                            cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                                            cargaIntegracao.ProblemaIntegracao = "Falha na integração, consulte o histórico para verificação.";

                                            Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo();
                                            arquivoIntegracao.Data = cargaIntegracao.DataIntegracao;
                                            arquivoIntegracao.Mensagem = cargaIntegracao.ProblemaIntegracao;
                                            arquivoIntegracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;

                                            arquivoIntegracao.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonRequest, "json", unitOfWork);
                                            arquivoIntegracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonResponse, "json", unitOfWork);

                                            repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);
                                            cargaIntegracao.ArquivosTransacao.Add(arquivoIntegracao);
                                        }
                                        else
                                        {
                                            cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                                            cargaIntegracao.ProblemaIntegracao = string.Empty;
                                            repositorioCargaIntegracao.Atualizar(cargaIntegracao);

                                            Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo();
                                            arquivoIntegracao.Data = cargaIntegracao.DataIntegracao;
                                            arquivoIntegracao.Mensagem = cargaIntegracao.ProblemaIntegracao;
                                            arquivoIntegracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;

                                            arquivoIntegracao.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonRequest, "json", unitOfWork);
                                            arquivoIntegracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonResponse, "json", unitOfWork);

                                            repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);
                                            cargaIntegracao.ArquivosTransacao.Add(arquivoIntegracao);
                                        }
                                    }
                                    catch (Exception excecao)
                                    {
                                        Servicos.Log.TratarErro(excecao, "IntegrarCargaEscrituracao");
                                        cargaIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao comunicar com o Web Service da Magalog.";
                                        cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                                    }
                                }
                            }
                        }
                        else if (cargaCTe != null && cargaCTe.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao != Dominio.Enumeradores.TipoDocumento.CTe)
                        {
                            cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                            cargaIntegracao.ProblemaIntegracao = "Tipo de documento não disponível para retorno.";
                        }
                        else
                        {
                            cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                            cargaIntegracao.ProblemaIntegracao = "Documento não disponível para retorno.";
                        }
                    }
                }
            }

            repositorioCargaIntegracao.Atualizar(cargaIntegracao);
        }

        public static void IntegrarCTeRetornoWS(int codigoCTe, string stringConexao)
        {
            try
            {
                Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);

                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Repositorio.Embarcador.Configuracoes.Integracao repositorioConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaIntegracao repCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaIntegracao(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe = repCargaCTe.BuscarPorCTe(codigoCTe);
                if (cargaCTe != null)
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaIntegracao cargaIntegragao = repCargaIntegracao.BuscarPorCargaETipoIntegracao(cargaCTe.Carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Magalog);

                    if (cargaIntegragao != null)
                    {
                        Dominio.Entidades.Empresa empresaPai = repEmpresa.BuscarEmpresaPai();
                        Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repositorioConfiguracaoIntegracao.Buscar();

                        string endPoint = (empresaPai.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Producao) ? configuracaoIntegracao.URLProducaoMagalog : configuracaoIntegracao.URLHomologacaoMagalog;
                        if (!endPoint.Contains("loadIntegration"))
                            endPoint = string.Concat(endPoint, "loadIntegration");
                        if (configuracaoIntegracao != null && configuracaoIntegracao.PossuiIntegracaoMagalog && !string.IsNullOrWhiteSpace(endPoint))
                        {
                            string mensagemErro = string.Empty;
                            string clientRequestContent = string.Empty;
                            string clientResponseContent = string.Empty;

                            Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao = null;

                            bool retorno = RetornarCarga(cargaCTe.Carga, ref cargaIntegracao, endPoint, configuracaoIntegracao.TokenMagalog, unitOfWork, ref clientRequestContent, ref clientResponseContent, out mensagemErro);

                            if (!retorno)
                                Servicos.Log.TratarErro("IntegrarCTeRejeitado: Falha ao retornar carga " + cargaCTe.Carga.CodigoCargaEmbarcador + ": " + mensagemErro, "IntegracaoMagalog");
                            else
                                Servicos.Log.TratarErro("IntegrarCTeRejeitado: Sucesso ao retornar carga " + cargaCTe.Carga.CodigoCargaEmbarcador, "IntegracaoMagalog");
                        }
                        else
                            Servicos.Log.TratarErro("Configuraçâo integração magalog nâo encontrada ou nâo configurada", "IntegracaoMagalog");
                    }
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("IntegrarCTeRejeitado: " + ex, "IntegracaoMagalog");
            }
        }

        public static void IntegrarOcorrencia(Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao integracao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Embarcador.Configuracoes.Integracao repositorioConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
            Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao repOcorrenciaCTeIntegracao = new Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao(unitOfWork);
            Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoArquivo repOcorrenciaCTeIntegracaoArquivo = new Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoArquivo(unitOfWork);

            Dominio.Entidades.Empresa empresaPai = repEmpresa.BuscarEmpresaPai();
            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repositorioConfiguracaoIntegracao.Buscar();

            if ((configuracaoIntegracao == null) || !configuracaoIntegracao.PossuiIntegracaoMagalog || string.IsNullOrWhiteSpace(configuracaoIntegracao.URLProducaoMagalog))
            {
                integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                integracao.ProblemaIntegracao = "Não existe configuração de integração disponível para a Magalog.";
            }
            else
            {
                integracao.NumeroTentativas += 1;
                integracao.DataIntegracao = DateTime.Now;

                string endPoint = (empresaPai.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Producao) ? configuracaoIntegracao.URLProducaoMagalog : configuracaoIntegracao.URLHomologacaoMagalog;
                if (!endPoint.Contains("loadIntegration"))
                    endPoint = string.Concat(endPoint, "loadIntegration");
                string mensagemErro = string.Empty;
                string clientRequestContent = string.Empty;
                string clientResponseContent = string.Empty;
                bool retorno = false;

                retorno = RetornarCTeOcorrencia(integracao, endPoint, configuracaoIntegracao.TokenMagalog, unitOfWork, ref clientRequestContent, ref clientResponseContent, out mensagemErro);

                if (!retorno)
                {
                    integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    integracao.ProblemaIntegracao = mensagemErro;

                    Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoArquivo();
                    arquivoIntegracao.Data = integracao.DataIntegracao;
                    arquivoIntegracao.Mensagem = integracao.ProblemaIntegracao;
                    arquivoIntegracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;

                    arquivoIntegracao.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(clientRequestContent, "json", unitOfWork);
                    arquivoIntegracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(clientResponseContent, "json", unitOfWork);

                    repOcorrenciaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

                    integracao.ArquivosTransacao.Add(arquivoIntegracao);
                }
                else
                {
                    try
                    {
                        integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                        repOcorrenciaCTeIntegracao.Atualizar(integracao);

                        Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoArquivo();
                        arquivoIntegracao.Data = integracao.DataIntegracao;
                        arquivoIntegracao.Mensagem = integracao.ProblemaIntegracao;
                        arquivoIntegracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;

                        arquivoIntegracao.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(clientRequestContent, "json", unitOfWork);
                        arquivoIntegracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(clientResponseContent, "json", unitOfWork);

                        repOcorrenciaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

                        integracao.ArquivosTransacao.Add(arquivoIntegracao);
                    }
                    catch (Exception excecao)
                    {
                        Servicos.Log.TratarErro(excecao);
                        integracao.ProblemaIntegracao = "Ocorreu uma falha ao comunicar com o Web Service da Magalog.";
                        integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                        Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoArquivo();
                        arquivoIntegracao.Data = integracao.DataIntegracao;
                        arquivoIntegracao.Mensagem = excecao.Message.Length > 400 ? excecao.Message.Substring(0, 400) : excecao.Message;
                        arquivoIntegracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;


                        arquivoIntegracao.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(clientRequestContent, "json", unitOfWork);
                        arquivoIntegracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(clientResponseContent, "json", unitOfWork);

                        repOcorrenciaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

                        integracao.ArquivosTransacao.Add(arquivoIntegracao);
                        Servicos.Log.TratarErro(JsonConvert.SerializeObject(clientResponseContent));
                    }
                }
            }

            repOcorrenciaCTeIntegracao.Atualizar(integracao);
        }

        public static void IntegrarMDFeManual(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repositorioCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(unitOfWork);
            Repositorio.Embarcador.Configuracoes.Integracao repositorioConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(unitOfWork);

            Dominio.Entidades.Empresa empresaPai = repEmpresa.BuscarEmpresaPai();

            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repositorioConfiguracaoIntegracao.Buscar();

            if ((configuracaoIntegracao == null) || !configuracaoIntegracao.PossuiIntegracaoMagalog || string.IsNullOrWhiteSpace(configuracaoIntegracao.URLProducaoMagalog))
            {
                cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                cargaIntegracao.ProblemaIntegracao = "Não existe configuração de integração disponível para a Magalog.";
            }
            else
            {
                cargaIntegracao.NumeroTentativas += 1;
                cargaIntegracao.DataIntegracao = DateTime.Now;

                string endPoint = (empresaPai.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Producao) ? configuracaoIntegracao.URLProducaoMagalog : configuracaoIntegracao.URLHomologacaoMagalog;
                if (endPoint.Contains("loadIntegration"))
                    endPoint = endPoint.Replace("loadIntegration", "mdfeIntegration");
                else
                    endPoint = string.Concat(endPoint, "mdfeIntegration");
                string mensagemErro = string.Empty;
                string clientRequestContent = string.Empty;
                string clientResponseContent = string.Empty;

                bool retorno = RetornarMDFeCarga(cargaIntegracao.Carga, endPoint, configuracaoIntegracao.TokenMagalog, unitOfWork, ref clientRequestContent, ref clientResponseContent, out mensagemErro);

                if (!retorno)
                {
                    cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    cargaIntegracao.ProblemaIntegracao = mensagemErro;

                    Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo();
                    arquivoIntegracao.Data = cargaIntegracao.DataIntegracao;
                    arquivoIntegracao.Mensagem = cargaIntegracao.ProblemaIntegracao;
                    arquivoIntegracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;

                    arquivoIntegracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(clientResponseContent, "json", unitOfWork);

                    repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

                    cargaIntegracao.ArquivosTransacao.Add(arquivoIntegracao);
                }
                else
                {
                    try
                    {
                        cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                        repositorioCargaIntegracao.Atualizar(cargaIntegracao);

                        Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo();
                        arquivoIntegracao.Data = cargaIntegracao.DataIntegracao;
                        arquivoIntegracao.Mensagem = cargaIntegracao.ProblemaIntegracao;
                        arquivoIntegracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;

                        arquivoIntegracao.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(clientRequestContent, "json", unitOfWork);
                        arquivoIntegracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(clientResponseContent, "json", unitOfWork);

                        repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

                        cargaIntegracao.ArquivosTransacao.Add(arquivoIntegracao);
                    }
                    catch (Exception excecao)
                    {
                        Servicos.Log.TratarErro(excecao, "IntegracaoMagalog");
                        cargaIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao comunicar com o Web Service da Magalog.";
                        cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                        Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo();
                        arquivoIntegracao.Data = cargaIntegracao.DataIntegracao;
                        arquivoIntegracao.Mensagem = excecao.Message.Length > 400 ? excecao.Message.Substring(0, 400) : excecao.Message;
                        arquivoIntegracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;


                        arquivoIntegracao.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(clientRequestContent, "json", unitOfWork);
                        arquivoIntegracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(clientResponseContent, "json", unitOfWork);

                        repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

                        cargaIntegracao.ArquivosTransacao.Add(arquivoIntegracao);
                        Servicos.Log.TratarErro(JsonConvert.SerializeObject(clientResponseContent), "IntegracaoMagalog");
                    }
                }
            }

            repositorioCargaIntegracao.Atualizar(cargaIntegracao);
        }

        public static void IntegrarMDFeRetornoWS(int codigoMDFe, Repositorio.UnitOfWork unitOfWork)
        {
            try
            {
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Repositorio.Embarcador.Configuracoes.Integracao repositorioConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
                Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaMDFeManual repCargaMDFeManual = new Repositorio.Embarcador.Cargas.CargaMDFeManual(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repositorioConfiguracaoIntegracao.Buscar();

                if (configuracaoIntegracao != null && !string.IsNullOrWhiteSpace(configuracaoIntegracao.TokenMagalog))
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual cargaMDFeManual = repCargaMDFeManual.BuscarPorMDFe(codigoMDFe);
                    if (cargaMDFeManual != null)
                    {
                        Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe = repMDFe.BuscarPorCodigo(codigoMDFe);
                        if (mdfe != null)
                        {
                            Dominio.Entidades.Empresa empresaPai = repEmpresa.BuscarEmpresaPai();

                            string endPoint = (empresaPai.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Producao) ? configuracaoIntegracao.URLProducaoMagalog : configuracaoIntegracao.URLHomologacaoMagalog;
                            if (endPoint.Contains("loadIntegration"))
                                endPoint = endPoint.Replace("loadIntegration", "mdfeIntegration");
                            else
                                endPoint = string.Concat(endPoint, "mdfeIntegration");
                            string mensagemErro = string.Empty;
                            string clientRequestContent = string.Empty;
                            string clientResponseContent = string.Empty;

                            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                            HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoMagalog));

                            client.BaseAddress = new Uri(endPoint);
                            client.DefaultRequestHeaders.Accept.Clear();
                            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                            if (configuracaoIntegracao.TokenMagalog != null)
                                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", configuracaoIntegracao.TokenMagalog);

                            string mensagemValidacao = mdfe.MensagemStatus != null ? mdfe.MensagemStatus.CodigoDoErro.ToString() + " - " + mdfe.MensagemStatus.MensagemDoErro : mdfe.MensagemRetornoSefaz;

                            Dominio.ObjetosDeValor.Embarcador.Integracao.Magalog.RetornoCarga retornoCarga = new Dominio.ObjetosDeValor.Embarcador.Integracao.Magalog.RetornoCarga()
                            {
                                protocol = new Dominio.ObjetosDeValor.Embarcador.Integracao.Magalog.Protocolos()
                                {
                                    idLoad = cargaMDFeManual.Codigo,
                                    idOrder = null

                                },
                                idMessage = string.IsNullOrWhiteSpace(mensagemValidacao) ? 0 : mdfe.MensagemStatus != null ? mdfe.MensagemStatus.CodigoDoErro : 0,
                                createAt = DateTime.Now,
                                message = mensagemValidacao
                            };

                            string jsonRequest = JsonConvert.SerializeObject(retornoCarga, Formatting.Indented, new Newtonsoft.Json.JsonSerializerSettings() { NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore });

                            Servicos.Log.TratarErro(jsonRequest, "IntegracaoMagalog");
                            

                            var content = new StringContent(jsonRequest.ToString(), Encoding.UTF8, "application/json");

                            var result = client.PostAsync(endPoint, content).Result;
                            Servicos.Log.TratarErro($"Response: {result.ToString()}", "IntegracaoMagalog");

                            string jsonResponse = result.Content.ReadAsStringAsync().Result;

                            Servicos.Log.TratarErro(jsonResponse, "IntegracaoMagalog");

                            if (!result.IsSuccessStatusCode)
                                Servicos.Log.TratarErro("IntegrarMDFeRejeitado: Falha ao retornar protocolo " + cargaMDFeManual.Codigo.ToString() + ": " + mensagemErro, "IntegracaoMagalog");
                            else
                                Servicos.Log.TratarErro("IntegrarMDFeRejeitado: Sucesso ao retornar " + cargaMDFeManual.Codigo.ToString(), "IntegracaoMagalog");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("IntegrarMDFeRejeitado: " + ex, "IntegracaoMagalog");
            }
        }

        public void IntegrarDocumentosEscrituracaoOcorrencia(Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao ocorrenciaCTeIntegracao)
        {
            Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao repOcorrenciaCTeIntegracao = new Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.Integracao repositorioConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(_unitOfWork);
            Repositorio.XMLCTe repXMLCTe = new Repositorio.XMLCTe(_unitOfWork);

            ArquivoTransacao<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoArquivo>(_unitOfWork);
            Servicos.CTe serCTe = new Servicos.CTe(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repositorioConfiguracaoIntegracao.Buscar();

            string jsonResponse = string.Empty;
            string jsonRequest = string.Empty;

            try
            {
                if ((configuracaoIntegracao == null) || !configuracaoIntegracao.PossuiIntegracaoMagalog || string.IsNullOrWhiteSpace(configuracaoIntegracao.URLEscrituracaoMagalog))
                    throw new ServicoException("Não existe configuração de integração disponível para a Magalog Escrituração.");

                ocorrenciaCTeIntegracao.NumeroTentativas += 1;
                ocorrenciaCTeIntegracao.DataIntegracao = DateTime.Now;

                string authToken = ObterTokenKeycloak(configuracaoIntegracao.URLKeyCloacMagalog, configuracaoIntegracao.IDKeyCloacMagalog, configuracaoIntegracao.SecretKeyCloacMagalog, "client_credentials");

                if (string.IsNullOrWhiteSpace(authToken))
                    throw new ServicoException("Keycloac não retornou token de integração.");

                Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe = ocorrenciaCTeIntegracao.CargaCTe;
                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = ocorrenciaCTeIntegracao.CargaCTe.CTe;

                if (cte.ModeloDocumentoFiscal.TipoDocumentoEmissao != Dominio.Enumeradores.TipoDocumento.CTe)
                    throw new ServicoException("Tipo de documento não disponível para retorno.");

                Dominio.Entidades.XMLCTe xmlCTE = repXMLCTe.BuscarPorCTe(cte.Codigo, Dominio.Enumeradores.TipoXMLCTe.Autorizacao);
                if (xmlCTE == null)
                {
                    Servicos.Log.TratarErro("XML CTe não disponível " + cte.Chave, "IntegrarDocumentosEscrituracaoOcorrencia");
                    throw new ServicoException("XML CTe não disponível.");
                }

                string caminhoXml = serCTe.ObterCaminhoArmazenamentoXMLCTeArquivo(cte, "A", _unitOfWork);
                
                if (!Utilidades.IO.FileStorageService.Storage.Exists(caminhoXml))
                {
                    if (!xmlCTE.XMLArmazenadoEmArquivo)
                    {
                        byte[] data = System.Text.Encoding.UTF8.GetBytes(xmlCTE.XML);
                        Utilidades.IO.FileStorageService.Storage.WriteAllBytes(caminhoXml, data);
                    }
                }
                if (!Utilidades.IO.FileStorageService.Storage.Exists(caminhoXml))
                {
                    Servicos.Log.TratarErro("XML CTe não disponível " + cte.Chave, "IntegrarDocumentosEscrituracaoOcorrencia");
                    throw new ServicoException("Arquivo XML CTe não disponível.");
                }

                string tipoCTe = cte.Status == "C" ? "CTECanc" : cte.Status == "I" ? "CTEInut" : "CTE";
                bool retorno = EnviarRetornoEscrituracao(configuracaoIntegracao.URLEscrituracaoMagalog, authToken, caminhoXml, cte.Status, tipoCTe, cargaCTe.Carga.TipoOperacao?.Descricao, cargaCTe.Carga.TipoOperacao?.CodigoIntegracao, ref jsonRequest, ref jsonResponse);

                if (retorno)
                {
                    ocorrenciaCTeIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                    ocorrenciaCTeIntegracao.ProblemaIntegracao = string.Empty;
                }
                else
                {
                    Servicos.Log.TratarErro("Falha na integração CTe " + cargaCTe.CTe.Chave, "IntegracaoMagalogEscrituracao");
                    throw new ServicoException("Falha na integração, consulte o histórico para verificação.");
                }
            }
            catch (ServicoException ex)
            {
                ocorrenciaCTeIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                ocorrenciaCTeIntegracao.ProblemaIntegracao = ex.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao, "IntegrarDocumentosEscrituracaoOcorrencia");

                ocorrenciaCTeIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                ocorrenciaCTeIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração da Magalog Escrituração";
            }

            servicoArquivoTransacao.Adicionar(ocorrenciaCTeIntegracao, jsonRequest, jsonResponse, "json");

            repOcorrenciaCTeIntegracao.Atualizar(ocorrenciaCTeIntegracao);
        }

        #endregion

        #region Métodos Públicos - MultiCTe

        public static void IntegrarCTeParaEscrituracao(ref Dominio.Entidades.CTeIntegracaoRetorno cteIntegracaoRetorno, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.Integracao repositorioConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
            Repositorio.CTeIntegracaoRetornoLog repCTeIntegracaoRetornoLog = new Repositorio.CTeIntegracaoRetornoLog(unitOfWork);
            Repositorio.IntegracaoCTe repIntegracaoCTe = new Repositorio.IntegracaoCTe(unitOfWork);
            Repositorio.XMLCTe repXMLCTe = new Repositorio.XMLCTe(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repositorioConfiguracaoIntegracao.Buscar();

            if ((configuracaoIntegracao == null) || !configuracaoIntegracao.PossuiIntegracaoMagalog || string.IsNullOrWhiteSpace(configuracaoIntegracao.URLEscrituracaoMagalog))
            {
                cteIntegracaoRetorno.SituacaoIntegracao = Dominio.Enumeradores.SituacaoCTeIntegracaoRetorno.Falha;
                cteIntegracaoRetorno.ProblemaIntegracao = "Não existe configuração de integração disponível para a Magalog.";
                cteIntegracaoRetorno.NumeroTentativas += 1;
                cteIntegracaoRetorno.DataIntegracao = DateTime.Now;
            }
            else
            {
                string authToken = ObterTokenKeycloak(configuracaoIntegracao.URLKeyCloacMagalog, configuracaoIntegracao.IDKeyCloacMagalog, configuracaoIntegracao.SecretKeyCloacMagalog, "client_credentials");

                if (string.IsNullOrWhiteSpace(authToken))
                {
                    cteIntegracaoRetorno.SituacaoIntegracao = Dominio.Enumeradores.SituacaoCTeIntegracaoRetorno.Falha;
                    cteIntegracaoRetorno.ProblemaIntegracao = "Keycloac não retornou token de integração.";
                    cteIntegracaoRetorno.NumeroTentativas += 1;
                    cteIntegracaoRetorno.DataIntegracao = DateTime.Now;
                }
                else
                {
                    if (cteIntegracaoRetorno.CTe != null)
                    {
                        Dominio.Entidades.XMLCTe xmlCTE = null;
                        if (cteIntegracaoRetorno.CTe.Status == "C" || cteIntegracaoRetorno.CTe.Status == "I")
                            xmlCTE = repXMLCTe.BuscarPorCTe(cteIntegracaoRetorno.CTe.Codigo, Dominio.Enumeradores.TipoXMLCTe.Cancelamento);
                        else
                            xmlCTE = repXMLCTe.BuscarPorCTe(cteIntegracaoRetorno.CTe.Codigo, Dominio.Enumeradores.TipoXMLCTe.Autorizacao);

                        if (xmlCTE == null)
                        {
                            cteIntegracaoRetorno.SituacaoIntegracao = Dominio.Enumeradores.SituacaoCTeIntegracaoRetorno.Falha;
                            cteIntegracaoRetorno.ProblemaIntegracao = "XML CTe não disponível.";
                            cteIntegracaoRetorno.NumeroTentativas += 1;
                            cteIntegracaoRetorno.DataIntegracao = DateTime.Now;
                        }
                        else
                        {
                            string caminhoXml = string.Empty;
                            string tipo = cteIntegracaoRetorno.CTe.Status == "C" || cteIntegracaoRetorno.CTe.Status == "I" ? "C" : "A";

                            Servicos.CTe serCTe = new Servicos.CTe(unitOfWork);
                            caminhoXml = serCTe.ObterCaminhoArmazenamentoXMLCTeArquivo(cteIntegracaoRetorno.CTe, tipo, unitOfWork);

                            if (!Utilidades.IO.FileStorageService.Storage.Exists(caminhoXml))
                            {
                                if (!xmlCTE.XMLArmazenadoEmArquivo)
                                {
                                    byte[] data = System.Text.Encoding.UTF8.GetBytes(xmlCTE.XML);
                                    Utilidades.IO.FileStorageService.Storage.WriteAllBytes(caminhoXml, data);
                                }
                            }

                            if (!Utilidades.IO.FileStorageService.Storage.Exists(caminhoXml))
                            {
                                cteIntegracaoRetorno.SituacaoIntegracao = Dominio.Enumeradores.SituacaoCTeIntegracaoRetorno.Falha;
                                cteIntegracaoRetorno.ProblemaIntegracao = "Arquivo XML CTe não disponível.";
                                cteIntegracaoRetorno.NumeroTentativas += 1;
                                cteIntegracaoRetorno.DataIntegracao = DateTime.Now;
                            }
                            else
                            {
                                string jsonResponse = string.Empty;
                                string jsonRequest = string.Empty;

                                try
                                {
                                    var integracaoCTe = repIntegracaoCTe.BuscarPorCTe(cteIntegracaoRetorno.CTe.Codigo);
                                    string codigoTipoOperacao = integracaoCTe?.CodigoTipoOperacao;
                                    string tipoCTe = cteIntegracaoRetorno.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe ? cteIntegracaoRetorno.CTe.Status == "C" ? "CTECanc" : cteIntegracaoRetorno.CTe.Status == "I" ? "CTEInut" : "CTE" : "NFSe";

                                    bool retorno = EnviarRetornoEscrituracao(configuracaoIntegracao.URLEscrituracaoMagalog, authToken, caminhoXml, cteIntegracaoRetorno.CTe.Status, tipoCTe, codigoTipoOperacao, codigoTipoOperacao, ref jsonRequest, ref jsonResponse);

                                    if (!retorno)
                                    {
                                        cteIntegracaoRetorno.SituacaoIntegracao = Dominio.Enumeradores.SituacaoCTeIntegracaoRetorno.Falha;
                                        cteIntegracaoRetorno.ProblemaIntegracao = "Falha na integração, consulte o histórico para verificação.";
                                        cteIntegracaoRetorno.NumeroTentativas += 1;
                                        cteIntegracaoRetorno.DataIntegracao = DateTime.Now;

                                        Dominio.Entidades.CTeIntegracaoRetornoLog cteIntegracaoRetornoLog = new Dominio.Entidades.CTeIntegracaoRetornoLog();
                                        cteIntegracaoRetornoLog.CTeIntegracaoRetorno = cteIntegracaoRetorno;
                                        cteIntegracaoRetornoLog.Data = DateTime.Now;
                                        cteIntegracaoRetornoLog.Mensagem = "Falha na integração, consulte o histórico para verificação.";
                                        cteIntegracaoRetornoLog.Request = jsonRequest;
                                        cteIntegracaoRetornoLog.Response = jsonResponse;
                                        repCTeIntegracaoRetornoLog.Inserir(cteIntegracaoRetornoLog);

                                    }
                                    else
                                    {

                                        cteIntegracaoRetorno.SituacaoIntegracao = Dominio.Enumeradores.SituacaoCTeIntegracaoRetorno.Sucesso;
                                        cteIntegracaoRetorno.ProblemaIntegracao = string.Empty;
                                        cteIntegracaoRetorno.NumeroTentativas += 1;
                                        cteIntegracaoRetorno.DataIntegracao = DateTime.Now;

                                        Dominio.Entidades.CTeIntegracaoRetornoLog cteIntegracaoRetornoLog = new Dominio.Entidades.CTeIntegracaoRetornoLog();
                                        cteIntegracaoRetornoLog.CTeIntegracaoRetorno = cteIntegracaoRetorno;
                                        cteIntegracaoRetornoLog.Data = DateTime.Now;
                                        cteIntegracaoRetornoLog.Mensagem = "Sucesso";
                                        cteIntegracaoRetornoLog.Request = jsonRequest;
                                        cteIntegracaoRetornoLog.Response = jsonResponse;
                                        repCTeIntegracaoRetornoLog.Inserir(cteIntegracaoRetornoLog);
                                    }
                                }
                                catch (Exception excecao)
                                {
                                    Servicos.Log.TratarErro(excecao, "IntegrarCTeParaEscrituracao");
                                    cteIntegracaoRetorno.ProblemaIntegracao = "Ocorreu uma falha ao comunicar com o serviço da Magalog.";
                                    cteIntegracaoRetorno.SituacaoIntegracao = Dominio.Enumeradores.SituacaoCTeIntegracaoRetorno.Falha;
                                    cteIntegracaoRetorno.NumeroTentativas += 1;
                                    cteIntegracaoRetorno.DataIntegracao = DateTime.Now;
                                }
                            }
                        }
                    }
                    //else if (cteIntegracaoRetorno.NFSe != null)
                    //{
                    //    cteIntegracaoRetorno.ProblemaIntegracao = "Documento não disponível para integração.";
                    //    cteIntegracaoRetorno.SituacaoIntegracao = Dominio.Enumeradores.SituacaoCTeIntegracaoRetorno.Falha;
                    //    cteIntegracaoRetorno.NumeroTentativas += 1;
                    //}
                    else
                    {
                        cteIntegracaoRetorno.ProblemaIntegracao = "Documento não disponível para integração";
                        cteIntegracaoRetorno.SituacaoIntegracao = Dominio.Enumeradores.SituacaoCTeIntegracaoRetorno.Falha;
                        cteIntegracaoRetorno.NumeroTentativas += 1;
                        cteIntegracaoRetorno.DataIntegracao = DateTime.Now;
                    }

                }
            }
        }

        public static void IntegrarRetornoCTe(ref Dominio.Entidades.CTeIntegracaoRetorno cteIntegracaoRetorno, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.Integracao repositorioConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
            Repositorio.CTeIntegracaoRetornoLog repCTeIntegracaoRetornoLog = new Repositorio.CTeIntegracaoRetornoLog(unitOfWork);
            Repositorio.XMLCTe repXMLCTe = new Repositorio.XMLCTe(unitOfWork);


            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repositorioConfiguracaoIntegracao.Buscar();

            if ((configuracaoIntegracao == null) || !configuracaoIntegracao.PossuiIntegracaoMagalog || string.IsNullOrWhiteSpace(configuracaoIntegracao.URLCTeMultiCTeMagalog))
            {
                cteIntegracaoRetorno.SituacaoIntegracao = Dominio.Enumeradores.SituacaoCTeIntegracaoRetorno.Falha;
                cteIntegracaoRetorno.ProblemaIntegracao = "Não existe configuração de integração disponível para a Magalog.";
                cteIntegracaoRetorno.NumeroTentativas += 1;
                cteIntegracaoRetorno.DataIntegracao = DateTime.Now;
            }
            else
            {
                string token = configuracaoIntegracao.TokenCTeMultiCTeMagalog;
                string endPoint = configuracaoIntegracao.URLCTeMultiCTeMagalog;
                string jsonRequest = string.Empty;
                string jsonResponse = string.Empty;

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoMagalog));

                client.BaseAddress = new Uri(endPoint);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                if (token != null)
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                if (cteIntegracaoRetorno.CTe != null)
                {
                    try
                    {

                        string mensagemValidacao = cteIntegracaoRetorno.CTe.Status == "F" ? "Contingência FSDA" : cteIntegracaoRetorno.CTe != null ? cteIntegracaoRetorno.CTe.MensagemStatus != null ? cteIntegracaoRetorno.CTe.MensagemStatus.CodigoDoErro.ToString() + " " + cteIntegracaoRetorno.CTe.MensagemStatus.MensagemDoErro : cteIntegracaoRetorno.CTe.MensagemRetornoSefaz : string.Empty;

                        int statusMensagem = 999;
                        if (!string.IsNullOrWhiteSpace(mensagemValidacao))
                        {
                            int posicao = mensagemValidacao.IndexOf("->");
                            if (posicao > -1)
                            {
                                posicao = posicao + 2;
                                if (mensagemValidacao.Length > posicao + 3)
                                    int.TryParse(mensagemValidacao.Substring(posicao, 3), out statusMensagem);
                            }
                        }

                        Dominio.ObjetosDeValor.Embarcador.Integracao.Magalog.Retorno retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.Magalog.Retorno()
                        {
                            sefaz = new Dominio.ObjetosDeValor.Embarcador.Integracao.Magalog.RetornoSefaz()
                            {
                                status = cteIntegracaoRetorno.CTe.Status == "F" ? 5 : string.IsNullOrWhiteSpace(mensagemValidacao) ? statusMensagem : cteIntegracaoRetorno.CTe.MensagemStatus != null ? cteIntegracaoRetorno.CTe.MensagemStatus.CodigoDoErro : statusMensagem,
                                description = mensagemValidacao,
                                protocolo = cteIntegracaoRetorno.CTe.Protocolo,
                                type = new Dominio.ObjetosDeValor.Embarcador.Integracao.Magalog.RetornoSefazType()
                                {
                                    code = cteIntegracaoRetorno.CTe.TipoEmissaoChaveCTe != "1" ? 5 : 1, //   int.Parse(cteIntegracaoRetorno.CTe.TipoEmissaoChaveCTe),
                                    description = cteIntegracaoRetorno.CTe.TipoEmissaoChaveCTe == "1" ? "Normal" : cteIntegracaoRetorno.CTe.TipoEmissaoChaveCTe == "4" ? "EPEC" : cteIntegracaoRetorno.CTe.TipoEmissaoChaveCTe == "5" ? "FSDA" : cteIntegracaoRetorno.CTe.TipoEmissaoChaveCTe == "7" ? "SVC-RS" : cteIntegracaoRetorno.CTe.TipoEmissaoChaveCTe == "8" ? "SVC-SP" : "Normal"
                                }
                            },
                            cte = new Dominio.ObjetosDeValor.Embarcador.Integracao.Magalog.RetornoCTe()
                            {
                                key = cteIntegracaoRetorno.CTe.Chave,
                                number = cteIntegracaoRetorno.CTe.Numero
                            }
                        };

                        jsonRequest = JsonConvert.SerializeObject(retorno, Formatting.Indented);

                        Servicos.Log.TratarErro(jsonRequest, "IntegracaoMagalogMultiCTe");

                        var content = new StringContent(jsonRequest.ToString(), Encoding.UTF8, "application/json");

                        var result = client.PostAsync(endPoint, content).Result;

                        Servicos.Log.TratarErro($"Response: {result.ToString()}", "IntegracaoMagalog");
                        jsonResponse = result.Content.ReadAsStringAsync().Result;

                        Servicos.Log.TratarErro(jsonResponse, "IntegracaoMagalogMultiCTe");

                        if (result.IsSuccessStatusCode)
                        {
                            cteIntegracaoRetorno.DataIntegracao = DateTime.Now;
                            cteIntegracaoRetorno.SituacaoIntegracao = Dominio.Enumeradores.SituacaoCTeIntegracaoRetorno.Sucesso;
                            cteIntegracaoRetorno.ProblemaIntegracao = string.Empty;
                            cteIntegracaoRetorno.NumeroTentativas += 1;
                            cteIntegracaoRetorno.DataIntegracao = DateTime.Now;

                            Dominio.Entidades.CTeIntegracaoRetornoLog cteIntegracaoRetornoLog = new Dominio.Entidades.CTeIntegracaoRetornoLog();
                            cteIntegracaoRetornoLog.CTeIntegracaoRetorno = cteIntegracaoRetorno;
                            cteIntegracaoRetornoLog.Data = DateTime.Now;
                            cteIntegracaoRetornoLog.Mensagem = "Sucesso";
                            cteIntegracaoRetornoLog.Request = jsonRequest;
                            cteIntegracaoRetornoLog.Response = jsonResponse;
                            repCTeIntegracaoRetornoLog.Inserir(cteIntegracaoRetornoLog);
                        }
                        else
                        {
                            cteIntegracaoRetorno.SituacaoIntegracao = Dominio.Enumeradores.SituacaoCTeIntegracaoRetorno.Falha;
                            cteIntegracaoRetorno.ProblemaIntegracao = "Falha na integração, consulte o histórico para verificação.";
                            cteIntegracaoRetorno.NumeroTentativas += 1;
                            cteIntegracaoRetorno.DataIntegracao = DateTime.Now;

                            Dominio.Entidades.CTeIntegracaoRetornoLog cteIntegracaoRetornoLog = new Dominio.Entidades.CTeIntegracaoRetornoLog();
                            cteIntegracaoRetornoLog.CTeIntegracaoRetorno = cteIntegracaoRetorno;
                            cteIntegracaoRetornoLog.Data = DateTime.Now;
                            cteIntegracaoRetornoLog.Mensagem = "Falha na integração, consulte o histórico para verificação.";
                            cteIntegracaoRetornoLog.Request = jsonRequest;
                            cteIntegracaoRetornoLog.Response = jsonResponse;
                            repCTeIntegracaoRetornoLog.Inserir(cteIntegracaoRetornoLog);
                        }
                    }
                    catch (Exception ex)
                    {
                        Servicos.Log.TratarErro(cteIntegracaoRetorno?.Codigo + ": " + ex, "IntegracaoMagalogMultiCTe");

                        cteIntegracaoRetorno.ProblemaIntegracao = "Falha ao retornar integração";
                        cteIntegracaoRetorno.SituacaoIntegracao = Dominio.Enumeradores.SituacaoCTeIntegracaoRetorno.Falha;
                    }
                }
                //else if (cteIntegracaoRetorno.NFSe != null)
                //{
                //    Dominio.ObjetosDeValor.Embarcador.Integracao.Magalog.Retorno retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.Magalog.Retorno()
                //    {
                //        sefaz = new Dominio.ObjetosDeValor.Embarcador.Integracao.Magalog.RetornoSefaz()
                //        {
                //            status = cteIntegracaoRetorno.NFSe.Status == Dominio.Enumeradores.StatusNFSe.AgAprovacaoNFSeManual ? 99 : cteIntegracaoRetorno.NFSe.Status == Dominio.Enumeradores.StatusNFSe.Autorizado ? 100 : 8,
                //            description = cteIntegracaoRetorno.NFSe.Status == Dominio.Enumeradores.StatusNFSe.AgAprovacaoNFSeManual ? "99 NFS Manual" : cteIntegracaoRetorno.NFSe.Status == Dominio.Enumeradores.StatusNFSe.Autorizado ? "NFSe Autorizada" : "NFSe não autorizada",
                //            protocolo = cteIntegracaoRetorno.NFSe.CodigoVerificacao,
                //            type = new Dominio.ObjetosDeValor.Embarcador.Integracao.Magalog.RetornoSefazType()
                //            {
                //                code = 1,
                //                description = "Normal" 
                //            }
                //        },
                //        cte = new Dominio.ObjetosDeValor.Embarcador.Integracao.Magalog.RetornoCTe()
                //        {
                //            key = string.Empty,
                //            number = cteIntegracaoRetorno.NFSe.Numero
                //        }
                //    };

                //    jsonRequest = JsonConvert.SerializeObject(retorno, Formatting.Indented);

                //    Servicos.Log.TratarErro(jsonRequest, "IntegracaoMagalog");

                //    var content = new StringContent(jsonRequest.ToString(), Encoding.UTF8, "application/json");

                //    var result = client.PostAsync(endPoint, content).Result;

                //    jsonResponse = result.Content.ReadAsStringAsync().Result;

                //    Servicos.Log.TratarErro(jsonResponse, "IntegracaoMagalog");

                //    if (result.IsSuccessStatusCode)
                //    {
                //        cteIntegracaoRetorno.SituacaoIntegracao = Dominio.Enumeradores.SituacaoCTeIntegracaoRetorno.Sucesso;
                //        cteIntegracaoRetorno.ProblemaIntegracao = string.Empty;
                //        cteIntegracaoRetorno.NumeroTentativas += 1;

                //        Dominio.Entidades.CTeIntegracaoRetornoLog cteIntegracaoRetornoLog = new Dominio.Entidades.CTeIntegracaoRetornoLog();
                //        cteIntegracaoRetornoLog.CTeIntegracaoRetorno = cteIntegracaoRetorno;
                //        cteIntegracaoRetornoLog.Data = DateTime.Now;
                //        cteIntegracaoRetornoLog.Mensagem = "Sucesso";
                //        cteIntegracaoRetornoLog.Request = jsonRequest;
                //        cteIntegracaoRetornoLog.Response = jsonResponse;
                //        repCTeIntegracaoRetornoLog.Inserir(cteIntegracaoRetornoLog);
                //    }
                //    else
                //    {
                //        cteIntegracaoRetorno.SituacaoIntegracao = Dominio.Enumeradores.SituacaoCTeIntegracaoRetorno.Falha;
                //        cteIntegracaoRetorno.ProblemaIntegracao = "Falha na integração, consulte o histórico para verificação.";
                //        cteIntegracaoRetorno.NumeroTentativas += 1;

                //        Dominio.Entidades.CTeIntegracaoRetornoLog cteIntegracaoRetornoLog = new Dominio.Entidades.CTeIntegracaoRetornoLog();
                //        cteIntegracaoRetornoLog.CTeIntegracaoRetorno = cteIntegracaoRetorno;
                //        cteIntegracaoRetornoLog.Data = DateTime.Now;
                //        cteIntegracaoRetornoLog.Mensagem = "Falha na integração, consulte o histórico para verificação.";
                //        cteIntegracaoRetornoLog.Request = jsonRequest;
                //        cteIntegracaoRetornoLog.Response = jsonResponse;
                //        repCTeIntegracaoRetornoLog.Inserir(cteIntegracaoRetornoLog);
                //    }
                //}
                else
                {
                    cteIntegracaoRetorno.SituacaoIntegracao = Dominio.Enumeradores.SituacaoCTeIntegracaoRetorno.Falha;
                    cteIntegracaoRetorno.ProblemaIntegracao = "Documento não disponível.";
                    cteIntegracaoRetorno.NumeroTentativas += 1;
                }
            }
        }

        public static void IntegrarRetornoMultiCTeWS(int codigoCTe, string status, string stringConexao)
        {
            try
            {
                Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);
                Repositorio.CTeIntegracaoRetorno repCTeIntegracaoRetorno = new Repositorio.CTeIntegracaoRetorno(unitOfWork);
                Dominio.Entidades.CTeIntegracaoRetorno cteIntegracaoRetorno = repCTeIntegracaoRetorno.BuscarUltipoPorPorCTeTipo(codigoCTe, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Magalog);
                if (cteIntegracaoRetorno != null)
                {
                    if (status == cteIntegracaoRetorno.CTe.Status)
                        Servicos.Embarcador.Integracao.Magalog.IntegracaoMagalog.IntegrarRetornoCTe(ref cteIntegracaoRetorno, unitOfWork);
                    else
                    {
                        Servicos.Log.TratarErro("IntegrarRetornoCTeWS: CTe " + cteIntegracaoRetorno.CTe.Chave + " com status diferente.", "IntegracaoMagalogMultiCTe");
                        cteIntegracaoRetorno.SituacaoIntegracao = Dominio.Enumeradores.SituacaoCTeIntegracaoRetorno.Aguardando;
                        repCTeIntegracaoRetorno.Atualizar(cteIntegracaoRetorno);
                    }
                }

                if (status == "A" || status == "C" || status == "I")
                {
                    cteIntegracaoRetorno = repCTeIntegracaoRetorno.BuscarUltipoPorPorCTeTipo(codigoCTe, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.MagalogEscrituracao);
                    if (cteIntegracaoRetorno != null)
                    {
                        cteIntegracaoRetorno.SituacaoIntegracao = Dominio.Enumeradores.SituacaoCTeIntegracaoRetorno.Aguardando;
                        repCTeIntegracaoRetorno.Atualizar(cteIntegracaoRetorno);
                    }
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("IntegrarRetornoCTeWS: " + ex, "IntegracaoMagalogMultiCTe");
            }
        }

        public static void IntegrarRetornoMDFe(ref Dominio.Entidades.MDFeIntegracaoRetorno mdfeIntegracaoRetorno, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.Integracao repositorioConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
            Repositorio.MDFeIntegracaoRetornoLog repMDFeIntegracaoRetornoLog = new Repositorio.MDFeIntegracaoRetornoLog(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repositorioConfiguracaoIntegracao.Buscar();

            if ((configuracaoIntegracao == null) || !configuracaoIntegracao.PossuiIntegracaoMagalog || string.IsNullOrWhiteSpace(configuracaoIntegracao.URLMDFeMultiCTeMagalog))
            {
                mdfeIntegracaoRetorno.SituacaoIntegracao = Dominio.Enumeradores.SituacaoCTeIntegracaoRetorno.Falha;
                mdfeIntegracaoRetorno.ProblemaIntegracao = "Não existe configuração de integração disponível para a Magalog.";
                mdfeIntegracaoRetorno.NumeroTentativas += 1;
            }
            else
            {
                string token = configuracaoIntegracao.TokenMDFeMultiCTeMagalog;
                string endPoint = configuracaoIntegracao.URLMDFeMultiCTeMagalog;
                string jsonRequest = string.Empty;
                string jsonResponse = string.Empty;

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoMagalog));

                client.BaseAddress = new Uri(endPoint);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                if (token != null)
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                if (mdfeIntegracaoRetorno.MDFe != null)
                {
                    try
                    {
                        string mensagemValidacao = mdfeIntegracaoRetorno.MDFe.Status == Dominio.Enumeradores.StatusMDFe.EmitidoContingencia ? "Contingência MDFe" : mdfeIntegracaoRetorno.MDFe != null ? mdfeIntegracaoRetorno.MDFe.MensagemStatus != null ? mdfeIntegracaoRetorno.MDFe.MensagemStatus.CodigoDoErro.ToString() + " " + mdfeIntegracaoRetorno.MDFe.MensagemStatus.MensagemDoErro : mdfeIntegracaoRetorno.MDFe.MensagemRetornoSefaz : string.Empty;

                        Dominio.ObjetosDeValor.Embarcador.Integracao.Magalog.RetornoMDFe retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.Magalog.RetornoMDFe()
                        {
                            sefaz = new Dominio.ObjetosDeValor.Embarcador.Integracao.Magalog.RetornoSefaz()
                            {
                                status = mdfeIntegracaoRetorno.MDFe.Status == Dominio.Enumeradores.StatusMDFe.EmitidoContingencia ? 5 : string.IsNullOrWhiteSpace(mensagemValidacao) ? 0 : mdfeIntegracaoRetorno.MDFe.MensagemStatus != null ? mdfeIntegracaoRetorno.MDFe.MensagemStatus.CodigoDoErro : 0,
                                description = mensagemValidacao,
                                protocolo = mdfeIntegracaoRetorno.MDFe.Protocolo,
                            },
                            mdfe = new Dominio.ObjetosDeValor.Embarcador.Integracao.Magalog.DadosMDFe()
                            {
                                accessKey = mdfeIntegracaoRetorno.MDFe.Chave,
                                number = mdfeIntegracaoRetorno.MDFe.Numero
                            }
                        };

                        jsonRequest = JsonConvert.SerializeObject(retorno, Formatting.Indented);

                        Servicos.Log.TratarErro(jsonRequest, "IntegracaoMDFeMagalog");

                        var content = new StringContent(jsonRequest.ToString(), Encoding.UTF8, "application/json");

                        var result = client.PostAsync(endPoint, content).Result;
                        Servicos.Log.TratarErro($"Response: {result.ToString()}", "IntegracaoMagalog");

                        jsonResponse = result.Content.ReadAsStringAsync().Result;

                        Servicos.Log.TratarErro(jsonResponse, "IntegracaoMDFeMagalog");

                        if (result.IsSuccessStatusCode)
                        {
                            mdfeIntegracaoRetorno.SituacaoIntegracao = Dominio.Enumeradores.SituacaoCTeIntegracaoRetorno.Sucesso;
                            mdfeIntegracaoRetorno.ProblemaIntegracao = string.Empty;
                            mdfeIntegracaoRetorno.NumeroTentativas += 1;

                            Dominio.Entidades.MDFeIntegracaoRetornoLog mdfeIntegracaoRetornoLog = new Dominio.Entidades.MDFeIntegracaoRetornoLog();
                            mdfeIntegracaoRetornoLog.MDFeIntegracaoRetorno = mdfeIntegracaoRetorno;
                            mdfeIntegracaoRetornoLog.Data = DateTime.Now;
                            mdfeIntegracaoRetornoLog.Mensagem = "Sucesso";
                            mdfeIntegracaoRetornoLog.Request = jsonRequest;
                            mdfeIntegracaoRetornoLog.Response = jsonResponse;
                            repMDFeIntegracaoRetornoLog.Inserir(mdfeIntegracaoRetornoLog);
                        }
                        else
                        {
                            mdfeIntegracaoRetorno.SituacaoIntegracao = Dominio.Enumeradores.SituacaoCTeIntegracaoRetorno.Falha;
                            mdfeIntegracaoRetorno.ProblemaIntegracao = "Falha na integração, consulte o histórico para verificação.";
                            mdfeIntegracaoRetorno.NumeroTentativas += 1;

                            Dominio.Entidades.MDFeIntegracaoRetornoLog mdfeIntegracaoRetornoLog = new Dominio.Entidades.MDFeIntegracaoRetornoLog();
                            mdfeIntegracaoRetornoLog.MDFeIntegracaoRetorno = mdfeIntegracaoRetorno;
                            mdfeIntegracaoRetornoLog.Data = DateTime.Now;
                            mdfeIntegracaoRetornoLog.Mensagem = "Falha na integração, consulte o histórico para verificação.";
                            mdfeIntegracaoRetornoLog.Request = jsonRequest;
                            mdfeIntegracaoRetornoLog.Response = jsonResponse;
                            repMDFeIntegracaoRetornoLog.Inserir(mdfeIntegracaoRetornoLog);
                        }
                    }
                    catch (Exception ex)
                    {
                        Servicos.Log.TratarErro(mdfeIntegracaoRetorno?.Codigo + ": " + ex, "IntegracaoMDFeMagalog");

                        mdfeIntegracaoRetorno.ProblemaIntegracao = "Falha ao retornar integração";
                        mdfeIntegracaoRetorno.SituacaoIntegracao = Dominio.Enumeradores.SituacaoCTeIntegracaoRetorno.Falha;
                        mdfeIntegracaoRetorno.DataIntegracao = DateTime.Now;
                    }
                }
                else
                {
                    mdfeIntegracaoRetorno.SituacaoIntegracao = Dominio.Enumeradores.SituacaoCTeIntegracaoRetorno.Falha;
                    mdfeIntegracaoRetorno.ProblemaIntegracao = "Documento não disponível.";
                    mdfeIntegracaoRetorno.NumeroTentativas += 1;
                }
            }
        }

        public static void IntegrarRetornoMDFeMultiCTeWS(int codigoMDFe, string stringConexao)
        {
            try
            {
                Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);
                Repositorio.MDFeIntegracaoRetorno repMDFeIntegracaoRetorno = new Repositorio.MDFeIntegracaoRetorno(unitOfWork);
                Dominio.Entidades.MDFeIntegracaoRetorno mdfeIntegracaoRetorno = repMDFeIntegracaoRetorno.BuscarUltipoPorPorMDFeTipo(codigoMDFe, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Magalog);
                if (mdfeIntegracaoRetorno != null)
                    Servicos.Embarcador.Integracao.Magalog.IntegracaoMagalog.IntegrarRetornoMDFe(ref mdfeIntegracaoRetorno, unitOfWork);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("IntegrarRetornoMDFeMultiCTeWS: " + ex, "IntegracaoMDFeMagalog");
            }
        }

        #endregion

        #region Métodos Privados

        private static string ObterTokenKeycloak(string endPoint, string client_id, string client_secret, string grant_type)
        {
            HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoMagalog));

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));

            var content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "client_id", client_id },
                { "client_secret", client_secret },
                { "grant_type", grant_type }
            });

            string jsonRequest = JsonConvert.SerializeObject(content, Formatting.Indented, new Newtonsoft.Json.JsonSerializerSettings() { NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore });
            Servicos.Log.TratarErro(endPoint, "ObterTokenKeycloak");
            Servicos.Log.TratarErro("client_id = " + client_id, "ObterTokenKeycloak");
            Servicos.Log.TratarErro("client_secret = " + client_secret, "ObterTokenKeycloak");
            Servicos.Log.TratarErro("grant_type = " + grant_type, "ObterTokenKeycloak");

            var result = client.PostAsync(endPoint, content).Result;

            string jsonResponse = result.Content.ReadAsStringAsync().Result;
            Servicos.Log.TratarErro(jsonResponse, "ObterTokenKeycloak");

            if (!result.IsSuccessStatusCode)
                return null;

            dynamic obj = JsonConvert.DeserializeObject<dynamic>(jsonResponse);

            return (string)obj.access_token;
        }

        private static bool EnviarRetornoEscrituracao(string endPoint, string token, string caminhoXml, string situacaoCTe, string codigoDocumento, string descricaoOperacao, string codigoOperacao, ref string request, ref string response)
        {
            HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoMagalog));

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);

            string nomeArquivo = ObterNomeArquivoXml(caminhoXml);
            if (situacaoCTe == "C")
                nomeArquivo = nomeArquivo.Replace(".xml", "CTeEventCancel.xml");
            // else if (situacaoCTe == "I")
            //     nomeArquivo = nomeArquivo.Replace(".xml", "CTeEventInut.xml");

            MultipartFormDataContent content = new MultipartFormDataContent
            {
                { ObterStreamXml(caminhoXml), "xmlfile", ObterNomeArquivoXml(nomeArquivo) },
                { ObterJsonInformacoesCliente(codigoDocumento, descricaoOperacao, codigoOperacao), "infoclient" }
            };

            request = JsonConvert.SerializeObject(content, Formatting.Indented);

            Servicos.Log.TratarErro(request, "IntegracaoMagalogEscrituracao");

            HttpResponseMessage result = client.PostAsync(endPoint, content).Result;

            response = result.Content.ReadAsStringAsync().Result;

            Servicos.Log.TratarErro(result.IsSuccessStatusCode ? result.StatusCode.ToString("g") + " (true)" : response, "IntegracaoMagalogEscrituracao");

            if (!result.IsSuccessStatusCode)
                return false;

            return true;
        }

        private static StringContent ObterJsonInformacoesCliente(string codigoDocumento, string descricaoOperacao, string codigoOperacao)
        {
            //var json = @"{
            //                ""codDocto"":""NFSe / CTe"", 
            //                ""infComplementar"": {
            //                                        ""cteTipoOpCodigoIntegracao"":""MAGALU"",
            //                                        ""cteTipoOpDescricao"":""MAGALU SP""
            //                                      }
            //            }";

            Dominio.ObjetosDeValor.Embarcador.Integracao.Magalog.Informacoes informacoes = new Dominio.ObjetosDeValor.Embarcador.Integracao.Magalog.Informacoes();
            informacoes.codDocto = codigoDocumento;
            informacoes.infComplementar = new Dominio.ObjetosDeValor.Embarcador.Integracao.Magalog.InformacoesComplementares()
            {
                cteTipoOpCodigoIntegracao = codigoOperacao,
                cteTipoOpDescricao = descricaoOperacao
            };

            var json = JsonConvert.SerializeObject(informacoes, Formatting.Indented);

            return new StringContent(json);
        }

        private static StreamContent ObterStreamXml(string caminhoXml)
        {
            var filestream = Utilidades.IO.FileStorageService.Storage.OpenRead(caminhoXml);
            return new StreamContent(filestream);
        }

        private static string ObterNomeArquivoXml(string caminhoXml)
        {
            return System.IO.Path.GetFileName(caminhoXml);
        }

        private static bool RetornarCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, ref Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao, string endPoint, string token, Repositorio.UnitOfWork unitOfWork, ref string jsonRequest, ref string jsonResponse, out string erro)
        {
            try
            {
                erro = "";
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);

                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> listaCargaPedidos = repCargaPedido.BuscarPorCarga(carga.Codigo);

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in listaCargaPedidos)
                {
                    HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoMagalog));

                    client.BaseAddress = new Uri(endPoint);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    if (token != null)
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                    Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe = repCargaCTe.BuscarPorCargaPedido(cargaPedido.Codigo);

                    if (cargaCTe != null)
                    {
                        bool documentoAutorizado = cargaCTe.CTe != null && cargaCTe.CTe.Status == "R" ? false : true;
                        string mensagemValidacao = cargaCTe.CTe.Status == "F" ? "Contingência FSDA" : cargaCTe.CTe != null ? cargaCTe.CTe.MensagemStatus != null ? cargaCTe.CTe.MensagemStatus.CodigoDoErro.ToString() + " " + cargaCTe.CTe.MensagemStatus.MensagemDoErro : cargaCTe.CTe.MensagemRetornoSefaz : string.Empty;

                        Dominio.ObjetosDeValor.Embarcador.Integracao.Magalog.RetornoCarga retornoCarga = new Dominio.ObjetosDeValor.Embarcador.Integracao.Magalog.RetornoCarga()
                        {
                            protocol = new Dominio.ObjetosDeValor.Embarcador.Integracao.Magalog.Protocolos()
                            {
                                idLoad = cargaPedido.CargaOrigem != null ? cargaPedido.CargaOrigem.Codigo : carga.Codigo,
                                idOrder = cargaPedido.Pedido.Codigo
                            },
                            idMessage = cargaCTe.CTe?.Status == "F" ? 5 : string.IsNullOrWhiteSpace(mensagemValidacao) ? 0 : cargaCTe.CTe.MensagemStatus != null ? cargaCTe.CTe.MensagemStatus.CodigoDoErro : 0,
                            createAt = DateTime.Now,
                            message = mensagemValidacao
                        };

                        jsonRequest = JsonConvert.SerializeObject(retornoCarga, Formatting.Indented);

                        Servicos.Log.TratarErro(jsonRequest, "IntegracaoMagalog");

                        var content = new StringContent(jsonRequest.ToString(), Encoding.UTF8, "application/json");

                        var result = client.PostAsync(endPoint, content).Result;
                        Servicos.Log.TratarErro($"Response: {result.ToString()}", "IntegracaoMagalog");

                        jsonResponse = result.Content.ReadAsStringAsync().Result;

                        Servicos.Log.TratarErro(jsonResponse, "IntegracaoMagalog");

                        if (cargaIntegracao != null)
                            SalvarHistoricoIntegracao(ref cargaIntegracao, jsonRequest, jsonResponse, unitOfWork);

                        if (!result.IsSuccessStatusCode)
                            return false;
                    }
                    else
                    {
                        if (cargaPedido.PossuiNFSManual)
                        {
                            Dominio.ObjetosDeValor.Embarcador.Integracao.Magalog.RetornoCarga retornoCarga = new Dominio.ObjetosDeValor.Embarcador.Integracao.Magalog.RetornoCarga()
                            {
                                protocol = new Dominio.ObjetosDeValor.Embarcador.Integracao.Magalog.Protocolos()
                                {
                                    idLoad = cargaPedido.CargaOrigem != null ? cargaPedido.CargaOrigem.Codigo : carga.Codigo,
                                    idOrder = cargaPedido.Pedido.Codigo
                                },
                                idMessage = 99,
                                createAt = DateTime.Now,
                                message = "99 NFs Manual"
                            };

                            jsonRequest = JsonConvert.SerializeObject(retornoCarga, Formatting.Indented);

                            Servicos.Log.TratarErro(jsonRequest, "IntegracaoMagalog");

                            var content = new StringContent(jsonRequest.ToString(), Encoding.UTF8, "application/json");

                            var result = client.PostAsync(endPoint, content).Result;

                            jsonResponse = result.Content.ReadAsStringAsync().Result;

                            Servicos.Log.TratarErro(jsonResponse, "IntegracaoMagalog");

                            if (cargaIntegracao != null)
                                SalvarHistoricoIntegracao(ref cargaIntegracao, jsonRequest, jsonResponse, unitOfWork);

                            if (!result.IsSuccessStatusCode)
                                return false;
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
                Servicos.Log.TratarErro(e, "IntegracaoMagalog");
                erro = "Erro genérico ao enviar requisição para WebService.";
                return false;
            }
        }

        private static bool RetornarCTeOcorrencia(Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao ocorrenciaCTeIntegracao, string endPoint, string token, Repositorio.UnitOfWork unitOfWork, ref string jsonRequest, ref string jsonResponse, out string erro)
        {
            try
            {
                erro = "";
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                Repositorio.Embarcador.Configuracoes.Integracao repIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
                Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repCargaOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo repCargaCTeComplementoInfo = new Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao repCargaOcorrenciaAutorizacao = new Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
                Repositorio.ConhecimentoDeTransporteEletronico repCte = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracoes.Integracao integracao = repIntegracao.Buscar();

                if (ocorrenciaCTeIntegracao != null && ocorrenciaCTeIntegracao.CargaCTe != null && ocorrenciaCTeIntegracao.CargaCTe.CTe != null)
                {
                    HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoMagalog));

                    client.BaseAddress = new Uri(endPoint);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    if (token != null)
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                    bool documentoAutorizado = ocorrenciaCTeIntegracao.CargaCTe.CTe != null && ocorrenciaCTeIntegracao.CargaCTe.CTe.Status == "R" ? false : true;
                    string mensagemValidacao = ocorrenciaCTeIntegracao.CargaCTe.CTe.Status == "F" ? "Contingência FSDA" : ocorrenciaCTeIntegracao.CargaCTe.CTe != null ? ocorrenciaCTeIntegracao.CargaCTe.CTe.MensagemStatus != null ? ocorrenciaCTeIntegracao.CargaCTe.CTe.MensagemStatus.CodigoDoErro.ToString() + " " + ocorrenciaCTeIntegracao.CargaCTe.CTe.MensagemStatus.MensagemDoErro : ocorrenciaCTeIntegracao.CargaCTe.CTe.MensagemRetornoSefaz : string.Empty;

                    Dominio.ObjetosDeValor.Embarcador.Integracao.Magalog.RetornoOcorrencia retornoOcorrencia = new Dominio.ObjetosDeValor.Embarcador.Integracao.Magalog.RetornoOcorrencia()
                    {
                        idMessage = ocorrenciaCTeIntegracao.CargaCTe.CTe.Status == "F" ? 5 : string.IsNullOrWhiteSpace(mensagemValidacao) ? 0 : ocorrenciaCTeIntegracao.CargaCTe.CTe.MensagemStatus != null ? ocorrenciaCTeIntegracao.CargaCTe.CTe.MensagemStatus.CodigoDoErro : 0,
                        createAt = DateTime.Now,
                        message = mensagemValidacao,
                        Object = new Dominio.ObjetosDeValor.Embarcador.Integracao.Magalog.Object()
                        {
                            id = ocorrenciaCTeIntegracao.CargaOcorrencia.Codigo.ToString()
                        },
                        cte = new Dominio.ObjetosDeValor.Embarcador.Integracao.Magalog.CTe()
                        {
                            protocol = ocorrenciaCTeIntegracao.CargaCTe.CTe.Codigo.ToString()
                        }
                    };

                    jsonRequest = JsonConvert.SerializeObject(retornoOcorrencia, Formatting.Indented);

                    Servicos.Log.TratarErro(jsonRequest, "IntegracaoMagalog");

                    var content = new StringContent(jsonRequest.ToString(), Encoding.UTF8, "application/json");

                    var result = client.PostAsync(endPoint, content).Result;
                    Servicos.Log.TratarErro($"Response: {result.ToString()}", "IntegracaoMagalog");

                    jsonResponse = result.Content.ReadAsStringAsync().Result;

                    Servicos.Log.TratarErro(jsonResponse, "IntegracaoMagalog");

                    if (!result.IsSuccessStatusCode)
                        return false;
                }
                else
                {
                    erro = "Nenhum documento da ocorrencia localizado para retornar";
                    return false;
                }

                return true;
            }
            catch (Exception e)
            {
                Servicos.Log.TratarErro(e);
                erro = "Falha ao retornar ocorrência para Magalog";
                return false;
            }
        }

        private static void SalvarHistoricoIntegracao(ref Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao, string jsonRequest, string jsonResponse, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo();
            arquivoIntegracao.Data = cargaIntegracao.DataIntegracao;
            arquivoIntegracao.Mensagem = cargaIntegracao.ProblemaIntegracao;
            arquivoIntegracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;

            arquivoIntegracao.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonRequest, "json", unitOfWork);
            arquivoIntegracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonResponse, "json", unitOfWork);

            repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

            cargaIntegracao.ArquivosTransacao.Add(arquivoIntegracao);
        }

        private static bool RetornarMDFeCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, string endPoint, string token, Repositorio.UnitOfWork unitOfWork, ref string jsonRequest, ref string jsonResponse, out string erro)
        {
            try
            {
                erro = "";
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaMDFe repCargaMDFe = new Repositorio.Embarcador.Cargas.CargaMDFe(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaMDFeManual repCargaMDFeManual = new Repositorio.Embarcador.Cargas.CargaMDFeManual(unitOfWork);

                HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoMagalog));

                client.BaseAddress = new Uri(endPoint);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                if (token != null)
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                List<Dominio.Entidades.Embarcador.Cargas.CargaMDFe> listaCargaMDFe = repCargaMDFe.BuscarPorCargaDesc(carga.Codigo);

                if (listaCargaMDFe != null && listaCargaMDFe.Count > 0)
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual cargaMDFeManual = repCargaMDFeManual.BuscarPorMDFe(listaCargaMDFe.FirstOrDefault().MDFe.Codigo);

                    if (cargaMDFeManual != null)
                    {

                        bool documentoAutorizado = listaCargaMDFe.FirstOrDefault().MDFe.Status == Dominio.Enumeradores.StatusMDFe.Autorizado; //cargaCTe.CTe != null && cargaCTe.CTe.Status == "R" ? false : true;
                        string mensagemValidacao = listaCargaMDFe.FirstOrDefault().MDFe.MensagemStatus != null ? listaCargaMDFe.FirstOrDefault().MDFe.MensagemStatus.CodigoDoErro.ToString() + " - " + listaCargaMDFe.FirstOrDefault().MDFe.MensagemStatus.MensagemDoErro : listaCargaMDFe.FirstOrDefault().MDFe.MensagemRetornoSefaz; //cargaCTe.CTe.Status == "F" ? "Contingência FSDA" : cargaCTe.CTe != null ? cargaCTe.CTe.MensagemStatus != null ? cargaCTe.CTe.MensagemStatus.CodigoDoErro.ToString() + " " + cargaCTe.CTe.MensagemStatus.MensagemDoErro : cargaCTe.CTe.MensagemRetornoSefaz : string.Empty;

                        Dominio.ObjetosDeValor.Embarcador.Integracao.Magalog.RetornoCarga retornoCarga = new Dominio.ObjetosDeValor.Embarcador.Integracao.Magalog.RetornoCarga()
                        {
                            protocol = new Dominio.ObjetosDeValor.Embarcador.Integracao.Magalog.Protocolos()
                            {
                                idLoad = cargaMDFeManual.Codigo,
                                idOrder = null

                            },
                            idMessage = string.IsNullOrWhiteSpace(mensagemValidacao) ? 0 : listaCargaMDFe.FirstOrDefault().MDFe.MensagemStatus != null ? listaCargaMDFe.FirstOrDefault().MDFe.MensagemStatus.CodigoDoErro : 0,
                            createAt = DateTime.Now,
                            message = mensagemValidacao
                        };

                        //jsonRequest = JsonConvert.SerializeObject(retornoCarga, Formatting.Indented);
                        jsonRequest = JsonConvert.SerializeObject(retornoCarga, Formatting.Indented, new Newtonsoft.Json.JsonSerializerSettings() { NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore });

                        Servicos.Log.TratarErro(jsonRequest, "IntegracaoMagalog");

                        var content = new StringContent(jsonRequest.ToString(), Encoding.UTF8, "application/json");

                        var result = client.PostAsync(endPoint, content).Result;
                        Servicos.Log.TratarErro($"Response: {result.ToString()}", "IntegracaoMagalog");
                        jsonResponse = result.Content.ReadAsStringAsync().Result;

                        Servicos.Log.TratarErro(jsonResponse, "IntegracaoMagalog");

                        if (!result.IsSuccessStatusCode)
                            return false;
                    }
                    else
                    {
                        Servicos.Log.TratarErro("MDFe " + listaCargaMDFe.FirstOrDefault().MDFe.Codigo.ToString() + " da carga " + carga.Codigo.ToString() + " sem MDFe Manual.", "IntegracaoMagalog");
                    }
                }
                else
                {
                    Servicos.Log.TratarErro("Carga " + carga.Codigo.ToString() + " sem MDFe Manual.", "IntegracaoMagalog");
                }

                return true;
            }
            catch (Exception e)
            {
                Servicos.Log.TratarErro(e, "IntegracaoMagalog");
                erro = "Erro genérico ao enviar requisição para WebService.";
                return false;
            }
        }

        private class LoggingHandler : DelegatingHandler
        {
            public LoggingHandler(HttpMessageHandler innerHandler)
                : base(innerHandler)
            {
            }

            protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                Servicos.Log.TratarErro("Request: " + request.ToString(), "IntegracaoMagalog");

                HttpResponseMessage response = await base.SendAsync(request, cancellationToken);
                Servicos.Log.TratarErro("Response: " + response.ToString(), "IntegracaoMagalog");

                return response;
            }
        }

        #endregion
    }
}