using Infrastructure.Services.HttpClientFactory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Servicos.Embarcador.Integracao.Cobasi
{
    public class IntegracaoCobasi
    {
        public static Dominio.ObjetosDeValor.Embarcador.Integracao.HttpRequisicaoResposta IntegrarOcorrencia(Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega pedidoOcorrenciaColetaEntrega, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdminMultisoftware, int codigoClienteURLAcesso)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.HttpRequisicaoResposta httpRequisicaoResposta = new Dominio.ObjetosDeValor.Embarcador.Integracao.HttpRequisicaoResposta()
            {
                conteudoRequisicao = string.Empty,
                extensaoRequisicao = "json",
                conteudoResposta = string.Empty,
                extensaoResposta = "json",
                sucesso = false,
                mensagem = string.Empty,
            };


            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXmlNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> notasFiscais = repPedidoXmlNotaFiscal.BuscarNotasFiscaisPorPedido(pedidoOcorrenciaColetaEntrega.Pedido.Codigo);

            if (notasFiscais == null || notasFiscais.Count == 0)
            {
                httpRequisicaoResposta.mensagem = "Nenhuma NFe localizada.";
            }
            else
            {
                Repositorio.Embarcador.Configuracoes.Integracao repositorioConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repositorioConfiguracaoIntegracao.Buscar();
                if (configuracaoIntegracao == null || string.IsNullOrWhiteSpace(configuracaoIntegracao.URLCobasi))
                {
                    httpRequisicaoResposta.mensagem = "Não existe configuração de integração disponível para a Cobasi.";
                }
                else
                {
                    string apiKey = configuracaoIntegracao.APIKeyCobasi; // "3cebb05edc8ed8009dc3ed4fa59a38e9f7328d534976d9d376c58a24df5c1661a6be3ebf974b9";
                    string endPoint = configuracaoIntegracao.URLCobasi; // "https://multisoftware-api.cobasi.com.br/status-tracking";

                    string jsonRequest = string.Empty, jsonResponse = string.Empty;
                    try
                    {
                        HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoCobasi));
                        client.BaseAddress = new Uri(endPoint);

                        // Headers
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        client.DefaultRequestHeaders.Add("Authorization", "Basic " + apiKey);

                        Dominio.ObjetosDeValor.Embarcador.Integracao.Cobasi.Ocorrencias ocorrencias = new Dominio.ObjetosDeValor.Embarcador.Integracao.Cobasi.Ocorrencias();
                        ocorrencias.Detalhe = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Cobasi.Detalhe>();
                        foreach (Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal notaFiscal in notasFiscais)
                        {
                            string linkOcorrencia = "";
                            if (pedidoOcorrenciaColetaEntrega.Pedido != null && !string.IsNullOrEmpty(pedidoOcorrenciaColetaEntrega.Pedido.CodigoRastreamento))
                            {
                                string urlBase = Servicos.Embarcador.GestaoEntregas.NotificaoEntrega.ObterURLBase(codigoClienteURLAcesso, tipoServicoMultisoftware, unitOfWorkAdminMultisoftware, unitOfWorkAdminMultisoftware.StringConexao,unitOfWork);
                                linkOcorrencia = Servicos.Embarcador.GestaoEntregas.NotificaoEntrega.ObterURLRastreamentoPedido(pedidoOcorrenciaColetaEntrega.Pedido.CodigoRastreamento, urlBase);
                            }

                            Dominio.ObjetosDeValor.Embarcador.Integracao.Cobasi.Detalhe detalhe = new Dominio.ObjetosDeValor.Embarcador.Integracao.Cobasi.Detalhe();
                            detalhe.chaveNFe = notaFiscal.Chave;
                            detalhe.numeroPedido = pedidoOcorrenciaColetaEntrega.Pedido.NumeroPedidoEmbarcador;
                            detalhe.urlRastreamento = linkOcorrencia;
                            detalhe.codigoOcorrencia = pedidoOcorrenciaColetaEntrega.TipoDeOcorrencia.CodigoIntegracao;
                            detalhe.descricaoOcorrencia = pedidoOcorrenciaColetaEntrega.TipoDeOcorrencia.Descricao;
                            detalhe.dataOcorrencia = pedidoOcorrenciaColetaEntrega.DataOcorrencia.ToString("dd/MM/yyyy HH:mm:ss");
                            detalhe.observacao = pedidoOcorrenciaColetaEntrega.Observacao;
                            detalhe.macrostatus = pedidoOcorrenciaColetaEntrega.TipoDeOcorrencia.GrupoTipoDeOcorrenciaDeCTe?.Descricao;
                            detalhe.codMacroStatus = pedidoOcorrenciaColetaEntrega.TipoDeOcorrencia.GrupoTipoDeOcorrenciaDeCTe?.CodigoIntegracao;

                            ocorrencias.Detalhe.Add(detalhe);
                        }

                        jsonRequest = JsonConvert.SerializeObject(ocorrencias, Formatting.Indented);

                        // Request
                        var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
                        var result = client.PostAsync(endPoint, content).Result;

                        // Response
                        jsonResponse = result.Content.ReadAsStringAsync().Result;

                        httpRequisicaoResposta.conteudoRequisicao = jsonRequest;
                        httpRequisicaoResposta.conteudoResposta = jsonResponse;
                        httpRequisicaoResposta.httpStatusCode = result.StatusCode;

                        Servicos.Log.TratarErro("URL: " + endPoint, "IntegracaoCobasi");
                        Servicos.Log.TratarErro("Authorization: " + "Basic " + apiKey, "IntegracaoCobasi");
                        Servicos.Log.TratarErro("Request: " + jsonRequest, "IntegracaoCobasi");
                        Servicos.Log.TratarErro("Response: " + jsonResponse, "IntegracaoCobasi");

                        if (result.IsSuccessStatusCode)
                        {
                            string retorno = result.Content.ReadAsStringAsync().Result;
                            if (!string.IsNullOrWhiteSpace(retorno))
                            {
                                dynamic retornoJSON = JsonConvert.DeserializeObject<dynamic>(retorno);
                                if (retornoJSON == null)
                                {
                                    httpRequisicaoResposta.mensagem = "Integração Cobasi não retornou JSON.";
                                }
                                else
                                {
                                    httpRequisicaoResposta.mensagem = (string)retornoJSON.msg ?? string.Empty;

                                    httpRequisicaoResposta.sucesso = true;
                                    DateTime dataIntegracao = DateTime.Now;
                                }
                            }
                            else
                            {
                                httpRequisicaoResposta.mensagem = "Integração Cobasi não teve retorno.";
                            }
                        }
                        else
                        {
                            httpRequisicaoResposta.mensagem = "Retorno integração Cobasi: " + result.StatusCode.ToString();
                        }

                        if (!httpRequisicaoResposta.sucesso && string.IsNullOrWhiteSpace(httpRequisicaoResposta.mensagem))
                            httpRequisicaoResposta.mensagem = "Integração cobasi não retornou sucesso.";
                    }
                    catch (Exception excecao)
                    {
                        Log.TratarErro("Request: " + jsonRequest, "IntegracaoCobasi");
                        Log.TratarErro("Response: " + jsonResponse, "IntegracaoCobasi");
                        Log.TratarErro(excecao, "IntegracaoCobasi");
                        httpRequisicaoResposta.mensagem = "Ocorreu uma falha ao comunicar com o Serviço da Cobasi.";
                    }
                }
            }

            return httpRequisicaoResposta;
        }


        //public static void IntegrarOcorrencias(Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao integracao, Repositorio.UnitOfWork unitOfWork)
        //{
        //    Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repCargaPedidoXMLNotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(unitOfWork);
        //    List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> listaDocumentosCTe = repCargaPedidoXMLNotaFiscalCTe.BuscarXMLNotasFiscaisPorCTe(integracao.CargaCTe.Codigo);

        //    Dominio.ObjetosDeValor.Embarcador.Integracao.HttpRequisicaoResposta httpRequisicaoResposta = IntegrarOcorrencia(listaDocumentosCTe, unitOfWork);

        //    integracao.NumeroTentativas += 1;
        //    integracao.DataIntegracao = DateTime.Now;
        //    integracao.ProblemaIntegracao = httpRequisicaoResposta.mensagem;
        //    integracao.SituacaoIntegracao = (httpRequisicaoResposta.sucesso) ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado : Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

        //    SalvarArquivoIntegracaoOcorrencia(integracao, httpRequisicaoResposta.conteudoRequisicao, httpRequisicaoResposta.conteudoResposta, httpRequisicaoResposta.mensagem, unitOfWork);

        //    Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao repositorioOcorrenciaCTeIntegracao = new Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao(unitOfWork);
        //    repositorioOcorrenciaCTeIntegracao.Atualizar(integracao);
        //}


        //private static Dominio.ObjetosDeValor.Embarcador.Integracao.HttpRequisicaoResposta IntegrarOcorrencia(List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> notasFiscais, Repositorio.UnitOfWork unitOfWork)
        //{
        //    Dominio.ObjetosDeValor.Embarcador.Integracao.HttpRequisicaoResposta httpRequisicaoResposta = new Dominio.ObjetosDeValor.Embarcador.Integracao.HttpRequisicaoResposta()
        //    {
        //        conteudoRequisicao = string.Empty,
        //        extensaoRequisicao = "json",
        //        conteudoResposta = string.Empty,
        //        extensaoResposta = "json",
        //        sucesso = false,
        //        mensagem = string.Empty,
        //    };

        //    if (notasFiscais == null)
        //    {
        //        httpRequisicaoResposta.mensagem = "Nenhuma NFe localizada.";
        //    }
        //    else
        //    {
        //        Repositorio.Embarcador.Configuracoes.Integracao repositorioConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
        //        Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repositorioConfiguracaoIntegracao.Buscar();
        //        if (configuracaoIntegracao == null)
        //        {
        //            httpRequisicaoResposta.mensagem = "Não existe configuração de integração disponível para a Cobasi.";
        //        }
        //        else
        //        {

        //            string apiKey = "";// configuracaoIntegracao.ApiKeyRiachuelo;
        //            string endPoint = "";// configuracaoIntegracao.URLIntegracaoEntregaNFeRiachuelo;

        //            string jsonRequest = string.Empty, jsonResponse = string.Empty;
        //            try
        //            {

        //                HttpClientHandler handler = new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate };
        //                HttpClient client = new HttpClient(handler);
        //                client.BaseAddress = new Uri(endPoint);

        //                // Headers
        //                client.DefaultRequestHeaders.Accept.Clear();
        //                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        //                client.DefaultRequestHeaders.Add("x-api-key", apiKey);
        //                client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");

        //                //Dominio.ObjetosDeValor.Embarcador.Integracao.Riachuelo.NFesEntregues nfesEntregues = new Dominio.ObjetosDeValor.Embarcador.Integracao.Riachuelo.NFesEntregues();
        //                //nfesEntregues.invoices = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Riachuelo.NFe>();
        //                foreach (Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal notaFiscal in notasFiscais)
        //                {
        //                    Dominio.ObjetosDeValor.Embarcador.Integracao.Riachuelo.NFe invoiceNotaFiscal = new Dominio.ObjetosDeValor.Embarcador.Integracao.Riachuelo.NFe();
        //                    invoiceNotaFiscal.branch = notaFiscal.Destinatario.CodigoIntegracao;
        //                    invoiceNotaFiscal.bukrs = notaFiscal.Emitente.CodigoIntegracao;
        //                    invoiceNotaFiscal.message = string.Empty;
        //                    invoiceNotaFiscal.nfeKey = notaFiscal.Chave;
        //                    invoiceNotaFiscal.nfeYear = Utilidades.Chave.ObterAno(notaFiscal.Chave);
        //                    invoiceNotaFiscal.number = notaFiscal.Numero.ToString();
        //                    invoiceNotaFiscal.serie = Utilidades.Chave.ObterSerie(notaFiscal.Chave);
        //                    invoiceNotaFiscal.status = "SUCCESS";
        //                    nfesEntregues.invoices.Add(invoiceNotaFiscal);
        //                }
        //                jsonRequest = JsonConvert.SerializeObject(nfesEntregues, Formatting.Indented);

        //                // Request
        //                var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
        //                var result = client.PostAsync(endPoint, content).Result;

        //                // Response
        //                jsonResponse = result.Content.ReadAsStringAsync().Result;

        //                httpRequisicaoResposta.conteudoRequisicao = jsonRequest;
        //                httpRequisicaoResposta.conteudoResposta = jsonResponse;
        //                httpRequisicaoResposta.httpStatusCode = result.StatusCode;

        //                Servicos.Log.TratarErro("URL: " + endPoint, "IntegracaoCobasi");
        //                Servicos.Log.TratarErro("Request: " + jsonRequest, "IntegracaoCobasi");
        //                Servicos.Log.TratarErro("Response: " + jsonResponse, "IntegracaoCobasi");

        //                if (result.IsSuccessStatusCode)
        //                {
        //                    string retorno = result.Content.ReadAsStringAsync().Result;
        //                    if (!string.IsNullOrWhiteSpace(retorno))
        //                    {
        //                        dynamic retornoJSON = JsonConvert.DeserializeObject<dynamic>(retorno);
        //                        if (retornoJSON == null)
        //                        {
        //                            httpRequisicaoResposta.mensagem = "Integração Cobasi não retornou JSON.";
        //                        }
        //                        else
        //                        {
        //                            httpRequisicaoResposta.mensagem = (string)retornoJSON.message ?? string.Empty;

        //                            if (retornoJSON.status == "SUCCESS")
        //                            {
        //                                httpRequisicaoResposta.sucesso = true;
        //                                DateTime dataIntegracao = DateTime.Now;
        //                                Repositorio.Embarcador.Canhotos.Canhoto repCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);
        //                                foreach (Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal notaFiscal in notasFiscais)
        //                                {
        //                                    Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto = notaFiscal.Canhoto;
        //                                    if (canhoto != null)
        //                                    {
        //                                        canhoto.DataIntegracaoEntrega = dataIntegracao;
        //                                        repCanhoto.Atualizar(canhoto);
        //                                    }
        //                                }
        //                            }
        //                            else
        //                            {
        //                                httpRequisicaoResposta.mensagem = "Retorno Cobasi: " + (string)retornoJSON.message ?? string.Empty;
        //                            }
        //                        }
        //                    }
        //                    else
        //                    {
        //                        httpRequisicaoResposta.mensagem = "Integração Cobasi não teve retorno.";
        //                    }
        //                }
        //                else
        //                {
        //                    httpRequisicaoResposta.mensagem = "Retorno integração Cobasi: " + result.StatusCode.ToString();
        //                }

        //                if (!httpRequisicaoResposta.sucesso && string.IsNullOrWhiteSpace(httpRequisicaoResposta.mensagem))
        //                    httpRequisicaoResposta.mensagem = "Integração Cobasi não retornou sucesso.";
        //            }
        //            catch (Exception excecao)
        //            {
        //                Log.TratarErro("Request: " + jsonRequest, "IntegracaoCobasi");
        //                Log.TratarErro("Response: " + jsonResponse, "IntegracaoCobasi");
        //                Log.TratarErro(excecao, "IntegracaoCobasi");
        //                httpRequisicaoResposta.mensagem = "Ocorreu uma falha ao comunicar com o Serviço da Cobasi.";
        //            }
        //        }
        //    }

        //    return httpRequisicaoResposta;
        //}
    }
}
