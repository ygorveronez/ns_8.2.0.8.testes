using Dominio.ObjetosDeValor.Embarcador.Integracao;
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

namespace Servicos.Embarcador.Integracao.VTEX
{
    public class IntegracaoVtex
    {
        #region Atributos Globais

        private readonly Repositorio.UnitOfWork _unitOfWork;
        private Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoVtex _configuracaoVtex;

        #endregion

        #region Construtores

        public IntegracaoVtex(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Métodos Públicos

        public HttpRequisicaoResposta AtualizarRastreamentoPedido(Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal notaFiscal, Dominio.Entidades.Embarcador.Pedidos.Pedido pedido)
        {
            HttpRequisicaoResposta httpRequisicaoResposta = new HttpRequisicaoResposta()
            {
                conteudoRequisicao = string.Empty,
                extensaoRequisicao = "json",
                conteudoResposta = string.Empty,
                extensaoResposta = "json",
                sucesso = false,
                mensagem = string.Empty
            };

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoVtex configuracaoVtex = ObterConfiguracaoIntegracao(pedido);

            if (configuracaoVtex == null || string.IsNullOrWhiteSpace(configuracaoVtex.AccountName) || string.IsNullOrWhiteSpace(configuracaoVtex.Environment))
            {
                httpRequisicaoResposta.mensagem = "A integração com a VTEX não está configurada.";
                return httpRequisicaoResposta;
            }

            if (string.IsNullOrWhiteSpace(pedido.NumeroPedidoEmbarcador))
            {
                httpRequisicaoResposta.mensagem = "Número do pedido não foi informado, o mesmo é necessário.";
                return httpRequisicaoResposta;
            }

            try
            {
                string accountName = !string.IsNullOrWhiteSpace(configuracaoVtex.Filial.AccountNameVtex) ? configuracaoVtex.Filial.AccountNameVtex : configuracaoVtex.AccountName;

                string url = $"https://{accountName}.{configuracaoVtex.Environment}.com.br/api/oms/pvt/orders/{pedido.NumeroPedidoEmbarcador}/invoice/{notaFiscal.Numero}/tracking";

                HttpClient requisicao = CriarRequisicao(url, configuracaoVtex);

                Dominio.ObjetosDeValor.Embarcador.Integracao.VTEX.RastreamentoPedido rastreamentoPedido = ObterDadosAtualizacaoRastreamentoPedido(pedido);
                string jsonRequisicao = JsonConvert.SerializeObject(rastreamentoPedido, Formatting.Indented);
                StringContent conteudoRequisicao = new StringContent(jsonRequisicao.ToString(), Encoding.UTF8, "application/json");
                HttpResponseMessage retornoRequisicao = requisicao.PutAsync(url, conteudoRequisicao).Result;
                string jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;

                httpRequisicaoResposta.conteudoRequisicao = jsonRequisicao;
                httpRequisicaoResposta.conteudoResposta = jsonRetorno;

                if (retornoRequisicao.StatusCode == HttpStatusCode.OK)
                {
                    httpRequisicaoResposta.mensagem = "Integrado com sucesso.";
                    httpRequisicaoResposta.sucesso = true;
                }
                else if (string.IsNullOrWhiteSpace(jsonRetorno))
                    httpRequisicaoResposta.mensagem = "Retorno integração: " + retornoRequisicao.StatusCode;
                else
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.VTEX.RastreamentoPedidoRetornoErro retornoErro = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.VTEX.RastreamentoPedidoRetornoErro>(jsonRetorno);
                    httpRequisicaoResposta.mensagem = retornoErro.Erro.Codigo + " - " + retornoErro.Erro.Mensagem;
                }
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);
                httpRequisicaoResposta.mensagem = "Ocorreu uma falha ao realizar a integração com a VTEX.";
            }

            return httpRequisicaoResposta;
        }



        public void IntegracaoDeDocasVtex(bool ForcaAtualizacao, List<Dominio.Entidades.Embarcador.Integracao.IntegracaoDocas> integracoesDocas)
        {
            try
            {
                Repositorio.Embarcador.Integracao.IntegracaoDocas repIntegracaoDocas = new Repositorio.Embarcador.Integracao.IntegracaoDocas(_unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoVtex repConfiguracaoVtex = new Repositorio.Embarcador.Configuracoes.ConfiguracaoVtex(_unitOfWork);
                List<Dominio.ObjetosDeValor.Embarcador.Integracao.VTEX.Docas.PickupStore> docasVtex = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.VTEX.Docas.PickupStore>();
                if (!ForcaAtualizacao)
                    if (integracoesDocas.Count > 0 && (DateTime.Now - integracoesDocas.Max(x => x.DataIntegracao)).TotalHours < 24)
                        return;

                foreach (var configuracaoVtex in repConfiguracaoVtex.BuscarAtivos())
                {
                    string Uri = $"https://{configuracaoVtex.AccountName}.{configuracaoVtex.Environment}.com.br/api/logistics/pvt/configuration/docks";
                    string requeststring = Uri;

                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                    var client = new RestClient(Uri);
                    client.Timeout = -1;
                    var request = new RestRequest(Method.GET);
                    request.AddHeader("X-VTEX-API-AppToken", configuracaoVtex.XVtexApiAppToken);
                    request.AddHeader("X-VTEX-API-AppKey", configuracaoVtex.XVtexApiAppKey);
                    IRestResponse response = client.Execute(request);
                    string responsestring = response.Content;
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        List<Dominio.ObjetosDeValor.Embarcador.Integracao.VTEX.Docas.PickupStore> docasFilialVtex = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Integracao.VTEX.Docas.PickupStore>>(response.Content);
                        docasVtex.AddRange(docasFilialVtex);
                    }
                    else
                    {
                        Log.TratarErro("Retorno " + Uri + " - " + response.ErrorMessage);

                        /*Repositorio.Embarcador.Integracao.PedidoAguardandoIntegracao repPedidoAguardandoIntegracao = new Repositorio.Embarcador.Integracao.PedidoAguardandoIntegracao(_unitOfWork);
                        Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao arquivoRequisicao = ArquivoIntegracao.SalvarArquivoIntegracao(requeststring, "json", _unitOfWork);
                        Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao arquivoResposta = ArquivoIntegracao.SalvarArquivoIntegracao(responsestring, "json", _unitOfWork);

                        Dominio.Entidades.Embarcador.Integracao.PedidoAguardandoIntegracao pedidoAguardandoIntegracao = new Dominio.Entidades.Embarcador.Integracao.PedidoAguardandoIntegracao();
                        pedidoAguardandoIntegracao.IdIntegracao = "";
                        pedidoAguardandoIntegracao.SituacaoIntegracao = SituacaoPedidoAguardandoIntegracao.ProblemaIntegracao;
                        pedidoAguardandoIntegracao.CreatedAt = DateTime.Now;
                        pedidoAguardandoIntegracao.DataCriacaoPedido = DateTime.Now;
                        pedidoAguardandoIntegracao.TipoIntegracao = TipoIntegracao.VTEX;
                        pedidoAguardandoIntegracao.Informacao = "Erro ao Buscar Pedidos no Feed da Vtex - URL: " + Uri + "Erro: " + response.ErrorMessage;
                        */
                    }
                }



                Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(_unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao = repTipoIntegracao.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.VTEX);
                foreach (var docaVtex in docasVtex)
                {
                    try
                    {
                        if (docaVtex.Id == null) //|| docaVtex.PickupStoreInfo == null || docaVtex.PickupStoreInfo.DockId == null || docaVtex.PickupStoreInfo.DockId == "null")
                            continue;

                        //string idIntegracao = docaVtex.Id ?? "" + " - " + docaVtex.PickupStoreInfo?.DockId ?? "";
                        string idIntegracao = docaVtex.PickupStoreInfo?.DockId.ToString() ?? "";
                        var docaMulti = integracoesDocas.Where(z => z.CodigoIntegracao == idIntegracao).FirstOrDefault();
                        if (docaMulti == null)
                        {
                            repIntegracaoDocas.Inserir(new Dominio.Entidades.Embarcador.Integracao.IntegracaoDocas
                            {
                                CodigoIntegracao = idIntegracao,
                                Nome = docaVtex.Name ?? "",
                                TempoMedioCarregamento = ConvertTimeEmSegundos(docaVtex.DockTimeFake),
                                DataIntegracao = DateTime.Now,
                                ProblemaIntegracao = "",
                                TipoIntegracao = tipoIntegracao
                            }); ;
                        }
                        else
                        {
                            docaMulti.TempoMedioCarregamento = ConvertTimeEmSegundos(docaVtex.DockTimeFake);
                            docaMulti.DataIntegracao = DateTime.Now;
                            repIntegracaoDocas.Atualizar(docaMulti);
                        }
                    }
                    catch (Exception e)
                    {

                    }
                }
            }
            catch (Exception e)
            {
                Servicos.Log.TratarErro(e);
            }
        }
        #endregion

        #region Métodos Privados

        private int ConvertTimeEmSegundos(string time)
        {
            try
            {
                TimeSpan timeSpan = TimeSpan.Parse(time);
                return (int)timeSpan.TotalSeconds;
            }
            catch (Exception)
            {
                return -1;
            }
        }


        private Dominio.ObjetosDeValor.Embarcador.Integracao.VTEX.RastreamentoPedido ObterDadosAtualizacaoRastreamentoPedido(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.VTEX.RastreamentoPedidoEvento> listaEventos = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.VTEX.RastreamentoPedidoEvento>();
            Dominio.ObjetosDeValor.Embarcador.Integracao.VTEX.RastreamentoPedidoEvento evento = new Dominio.ObjetosDeValor.Embarcador.Integracao.VTEX.RastreamentoPedidoEvento()
            {
                Cidade = pedido.Destino.Descricao,
                Estado = pedido.Destino.Estado.Sigla,
                Descricao = "PEDIDO ENTREGUE",
                Data = DateTime.Now.ToString("yyyy-MM-dd")
            };
            listaEventos.Add(evento);

            return new Dominio.ObjetosDeValor.Embarcador.Integracao.VTEX.RastreamentoPedido()
            {
                EstaEntregue = true,
                Eventos = listaEventos
            };
        }

        private HttpClient CriarRequisicao(string url, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoVtex configuracaoVtex)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            HttpClient requisicao = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoVtex));

            requisicao.BaseAddress = new Uri(url);
            requisicao.DefaultRequestHeaders.Accept.Clear();
            requisicao.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            requisicao.DefaultRequestHeaders.Add("X-VTEX-API-AppToken", configuracaoVtex.XVtexApiAppToken);
            requisicao.DefaultRequestHeaders.Add("X-VTEX-API-AppKey", configuracaoVtex.XVtexApiAppKey);

            return requisicao;
        }

        private Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoVtex ObterConfiguracaoIntegracao(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido)
        {
            if (_configuracaoVtex != null)
                return _configuracaoVtex;

            Repositorio.Embarcador.Configuracoes.ConfiguracaoVtex repConfiguracaoVtex = new Repositorio.Embarcador.Configuracoes.ConfiguracaoVtex(_unitOfWork);
            _configuracaoVtex = repConfiguracaoVtex.BuscarPorFilial(pedido.Filial.Codigo);

            return _configuracaoVtex;
        }

        #endregion
    }
}
