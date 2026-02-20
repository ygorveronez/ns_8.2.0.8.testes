using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Integracao;
using Dominio.ObjetosDeValor.Embarcador.Mondelez;
using Infrastructure.Services.HttpClientFactory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;

namespace Servicos.Embarcador.Integracao.Mondelez
{
    public class IntegracaoMondelez : ServicoBase
    {
        #region Constructores
        public IntegracaoMondelez(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }
        #endregion

        #region Métodos Públicos
        public bool GerarRegistroIntegracaoDrivin(ParametrosGeracaoIntegracaoDrivin parametrosGeracaoIntegracaoDrivin)
        {
            bool retorno = false;

            switch (parametrosGeracaoIntegracaoDrivin.Gatilho)
            {
                case GatilhoIntegracaoMondelezDrivin.ContatoCliente:
                case GatilhoIntegracaoMondelezDrivin.AgendamentoEntrega:
                case GatilhoIntegracaoMondelezDrivin.ReagendamentoEntrega:
                case GatilhoIntegracaoMondelezDrivin.ConfirmacaoEntrega:
                case GatilhoIntegracaoMondelezDrivin.RejeicaoEntrega:

                    if (parametrosGeracaoIntegracaoDrivin.Pedido == null) break;

                    PreencherParametrosGeracaoIntegracaoDrivin(parametrosGeracaoIntegracaoDrivin);

                    Dominio.Entidades.Cliente tomador = parametrosGeracaoIntegracaoDrivin.Pedido.ObterTomador() ?? parametrosGeracaoIntegracaoDrivin.Pedido.Remetente;
                    string observacao = $"Integração Drivin - Origem {parametrosGeracaoIntegracaoDrivin.Gatilho.ObterDescricao()}";
                    Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega pedidoOcorrenciaColetaEntrega = Servicos.Embarcador.Carga.ControleEntrega.OcorrenciaEntrega.GerarPedidoOcorrenciaColetaEntrega(tomador,
                                                                                                                                                                                                                        parametrosGeracaoIntegracaoDrivin.Pedido,
                                                                                                                                                                                                                        parametrosGeracaoIntegracaoDrivin.Carga,
                                                                                                                                                                                                                        parametrosGeracaoIntegracaoDrivin.TipoOcorrencia,
                                                                                                                                                                                                                        parametrosGeracaoIntegracaoDrivin.ConfiguracaoPortalCliente,
                                                                                                                                                                                                                        observacao,
                                                                                                                                                                                                                        parametrosGeracaoIntegracaoDrivin.EventoColetaEntrega,
                                                                                                                                                                                                                        parametrosGeracaoIntegracaoDrivin.ConfiguracaoEmbarcador,
                                                                                                                                                                                                                        parametrosGeracaoIntegracaoDrivin.ClienteMultisoftware,
                                                                                                                                                                                                                        _unitOfWork,
                                                                                                                                                                                                                        true);
                    retorno = pedidoOcorrenciaColetaEntrega != null && pedidoOcorrenciaColetaEntrega.Codigo > 0;
                    break;
            }
            return retorno;
        }

        public Dominio.ObjetosDeValor.Embarcador.Integracao.HttpRequisicaoResposta IntegrarOcorrenciaPedido(Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntregaIntegracao integracao)
        {
            Repositorio.Embarcador.Pedidos.PedidoOcorrenciaColetaEntregaIntegracao repPedidoOcorrenciaIntegracao = new Repositorio.Embarcador.Pedidos.PedidoOcorrenciaColetaEntregaIntegracao(_unitOfWork);

            HttpRequisicaoResposta httpRequisicaoResposta = new HttpRequisicaoResposta()
            {
                conteudoRequisicao = string.Empty,
                extensaoRequisicao = "json",
                conteudoResposta = string.Empty,
                extensaoResposta = "json",
                sucesso = false,
                mensagem = string.Empty
            };

            try
            {
                Repositorio.Embarcador.Configuracoes.IntegracaoMondelez repIntegracaoMondelez = new Repositorio.Embarcador.Configuracoes.IntegracaoMondelez(_unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoMondelez configuracaoIntegracao = repIntegracaoMondelez.BuscarPrimeiroRegistro();

                dynamic dadosRequisicao = ObterObjetoIntegracaoDrivin(integracao);

                HttpClient requisicao = CriarRequisicao(configuracaoIntegracao.URLDrivin, configuracaoIntegracao.ApiKeyDrivin);
                httpRequisicaoResposta.conteudoRequisicao = JsonConvert.SerializeObject(dadosRequisicao);
                StringContent conteudoRequisicao = new StringContent(httpRequisicaoResposta.conteudoRequisicao.ToString(), System.Text.Encoding.UTF8, "application/json");

                HttpResponseMessage retornoRequisicao = requisicao.PostAsync(configuracaoIntegracao.URLDrivin, conteudoRequisicao).Result;
                httpRequisicaoResposta.conteudoResposta = retornoRequisicao.Content.ReadAsStringAsync().Result;

                if (RetornoSucesso(retornoRequisicao))
                {
                    RetornoIntegracao retornoIntegracao = JsonConvert.DeserializeObject<RetornoIntegracao>(httpRequisicaoResposta.conteudoResposta);
                    if (retornoIntegracao.Success)
                    {
                        if (retornoIntegracao.Status == "OK")
                        {
                            httpRequisicaoResposta.sucesso = true;
                            httpRequisicaoResposta.mensagem = "Integrado com sucesso.";
                            try
                            {
                                //Se deu sucesso no envio principal, envia também para sistema Legado da Mondelez.
                                HttpClient requisicaoLegado = CriarRequisicao(configuracaoIntegracao.URLDrivin, configuracaoIntegracao.ApiKeyDrivinLegado);
                                conteudoRequisicao = new StringContent("", System.Text.Encoding.UTF8, "application/json");

                                retornoRequisicao = requisicaoLegado.PostAsync(configuracaoIntegracao.URLDrivin, conteudoRequisicao).Result;
                                if (RetornoSucesso(retornoRequisicao))
                                {
                                    RetornoIntegracao retornoIntegracaoLegado = JsonConvert.DeserializeObject<RetornoIntegracao>(retornoRequisicao.Content.ReadAsStringAsync().Result);
                                    httpRequisicaoResposta.mensagem += $" Legado: {retornoIntegracaoLegado.Status}";
                                }
                            }
                            catch (Exception ex) // Integração Legado não registra como falha.
                            {
                                httpRequisicaoResposta.mensagem += " Legado: Falha genérica";
                            }
                        }
                        else if (retornoIntegracao.Status == "Error")
                        {
                            httpRequisicaoResposta.sucesso = false;
                            httpRequisicaoResposta.mensagem = "Falha na integração. " + ObterFalhaIntegracao(retornoIntegracao);
                        }
                    }
                    else
                    {
                        httpRequisicaoResposta.sucesso = false;
                        httpRequisicaoResposta.mensagem = "Falha na integração. Falha: " + retornoIntegracao.Status;
                    }

                }
                else
                    throw new ServicoException($"Problema ao integrar com Mondelez (Status: {retornoRequisicao.StatusCode})");
            }
            catch (ServicoException excecao)
            {
                Log.TratarErro(excecao);
                httpRequisicaoResposta.mensagem = excecao.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);
                httpRequisicaoResposta.mensagem = "Problema ao tentar integrar com Mondelez.";
            }

            repPedidoOcorrenciaIntegracao.Atualizar(integracao);
            return httpRequisicaoResposta;
        }
        #endregion

        #region Métodos Privados
        public void PreencherParametrosGeracaoIntegracaoDrivin(ParametrosGeracaoIntegracaoDrivin parametrosGeracaoIntegracaoDrivin)
        {
            if (parametrosGeracaoIntegracaoDrivin.EventoColetaEntrega == EventoColetaEntrega.Todos)
            {
                EventoColetaEntrega eventoColetaEntrega = EventoColetaEntrega.Confirma;
                switch (parametrosGeracaoIntegracaoDrivin.Gatilho)
                {
                    case GatilhoIntegracaoMondelezDrivin.ConfirmacaoEntrega:
                        eventoColetaEntrega = EventoColetaEntrega.Confirma;
                        break;
                    case GatilhoIntegracaoMondelezDrivin.AgendamentoEntrega:
                        eventoColetaEntrega = EventoColetaEntrega.AgendamentoEntrega;
                        break;
                    case GatilhoIntegracaoMondelezDrivin.ReagendamentoEntrega:
                        eventoColetaEntrega = EventoColetaEntrega.ReagendamentoEntrega;
                        break;
                    case GatilhoIntegracaoMondelezDrivin.ContatoCliente:
                        eventoColetaEntrega = EventoColetaEntrega.ContatoCliente;
                        break;
                    case GatilhoIntegracaoMondelezDrivin.RejeicaoEntrega:
                        eventoColetaEntrega = EventoColetaEntrega.RejeicaoEntrega;
                        break;
                }
                parametrosGeracaoIntegracaoDrivin.EventoColetaEntrega = eventoColetaEntrega;
            }

            if (parametrosGeracaoIntegracaoDrivin.TipoOcorrencia == null)
            {
                Repositorio.Embarcador.Cargas.ControleEntrega.ConfiguracaoOcorrenciaEntrega repConfiguracaoOcorrenciaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.ConfiguracaoOcorrenciaEntrega(_unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.ConfiguracaoOcorrenciaEntrega> configuracaoOcorrenciaEntregas = repConfiguracaoOcorrenciaEntrega.BuscarRegrasPorEventoETipoAplicacao(parametrosGeracaoIntegracaoDrivin.EventoColetaEntrega, TipoAplicacaoColetaEntrega.Entrega);
                if (configuracaoOcorrenciaEntregas != null && configuracaoOcorrenciaEntregas.Count > 0)
                    parametrosGeracaoIntegracaoDrivin.TipoOcorrencia = configuracaoOcorrenciaEntregas[0].TipoDeOcorrencia;
            }
            parametrosGeracaoIntegracaoDrivin.ConfiguracaoPortalCliente ??= GestaoEntregas.ConfiguracaoPortalCliente.ObterConfiguracao(_unitOfWork);
            parametrosGeracaoIntegracaoDrivin.ConfiguracaoEmbarcador ??= new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork).BuscarPrimeiroRegistro();
            parametrosGeracaoIntegracaoDrivin.Carga ??= new Repositorio.Embarcador.Cargas.Carga(_unitOfWork).BuscarPorPedido(parametrosGeracaoIntegracaoDrivin.Pedido.Codigo);
        }

        private HttpClient CriarRequisicao(string endpoint, string apiKey)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            HttpClient requisicao = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoMondelez));
            requisicao.BaseAddress = new Uri(endpoint);
            requisicao.DefaultRequestHeaders.Accept.Clear();
            requisicao.DefaultRequestHeaders.Add("Content", "application/json");
            requisicao.DefaultRequestHeaders.Add("x-api-key", apiKey);

            return requisicao;
        }

        private bool RetornoSucesso(HttpResponseMessage retornoRequisicao)
        {
            return (retornoRequisicao.StatusCode == HttpStatusCode.OK) || (retornoRequisicao.StatusCode == HttpStatusCode.Created);
        }

        private dynamic ObterObjetoIntegracaoDrivin(Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntregaIntegracao integracao)
        {
            bool obteveDataIntegracao = false;
            dynamic dadosPedido = new ExpandoObject();
            dadosPedido.operation_code = integracao.PedidoOcorrenciaColetaEntrega.Carga.CodigoCargaEmbarcador; //Shipment7(NO MULTI É NUMERO CARGA)
            dadosPedido.order_container = integracao.PedidoOcorrenciaColetaEntrega.Pedido.NumeroOrdem ?? string.Empty; //Shipment8(NO MULTI É NUMERO ORDEM PEDIDO)

            switch (integracao.PedidoOcorrenciaColetaEntrega.EventoColetaEntrega)
            {
                case EventoColetaEntrega.ContatoCliente:
                    Repositorio.Embarcador.Anexo.Anexo<Dominio.Entidades.Embarcador.Logistica.AgendamentoEntregaPedidoClienteAnexo, Dominio.Entidades.Embarcador.Pedidos.Pedido> repositorioAgendamentoEntregaPedidoClienteAnexo = new(_unitOfWork);
                    List<Dominio.Entidades.Embarcador.Logistica.AgendamentoEntregaPedidoClienteAnexo> anexos = repositorioAgendamentoEntregaPedidoClienteAnexo.BuscarPorEntidade(integracao.PedidoOcorrenciaColetaEntrega.Pedido.Codigo);
                    if (anexos?.Count > 0)
                    {
                        dadosPedido.min_delivery_date = anexos[0].DataCadastro.ToString("yyyy-MM-dd"); //Data do primeiro contato com o Cliente
                        obteveDataIntegracao = true;
                    }
                    break;

                case EventoColetaEntrega.ReagendamentoEntrega:
                    dadosPedido.max_delivery_date = integracao.PedidoOcorrenciaColetaEntrega.Pedido.DataAgendamento?.ToString("yyyy-MM-dd"); //Data de Agendamento de Entrega
                    obteveDataIntegracao = true;
                    break;

                case EventoColetaEntrega.AgendamentoEntrega:
                    dadosPedido.delivery_date = integracao.PedidoOcorrenciaColetaEntrega.Pedido.DataAgendamento?.ToString("yyyy-MM-dd"); //Data de Agendamento de Entrega
                    obteveDataIntegracao = true;
                    break;

                case EventoColetaEntrega.Confirma:
                case EventoColetaEntrega.RejeicaoEntrega:
                    Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repositorioCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(_unitOfWork);
                    List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> listaCargaEntrega = repositorioCargaEntrega.BuscarEntregaPorPedido(integracao.PedidoOcorrenciaColetaEntrega.Pedido.Codigo, 0);

                    bool existeDevolucaoParcial = listaCargaEntrega.Any(c => c.DevolucaoParcial);

                    dadosPedido.reason_name = existeDevolucaoParcial ? "DriveIn Devolução Parcial" : "DriveIn Devolução Total"; //Descrição do Tipo de Devolução
                    dadosPedido.reason_code = existeDevolucaoParcial ? "D03" : "D02"; //Código do Tipo de Devolução da Entrega

                    if (listaCargaEntrega?.Count > 0)
                    {
                        dadosPedido.order_status = string.Empty;
                        dadosPedido.pod_timestamp = integracao.DataIntegracao.ToString("o"); //Data e hora do POD

                        Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = listaCargaEntrega[0];


                        dadosPedido.orders = new List<dynamic>();

                        foreach (var item in cargaEntrega.NotasFiscais)
                        {
                            dynamic dadosPedidoNota = new ExpandoObject();
                            dadosPedidoNota.code = item.PedidoXMLNotaFiscal?.CargaPedido?.Pedido.NumeroPedidoEmbarcador ?? string.Empty;
                            dadosPedidoNota.purchase_order = item.PedidoXMLNotaFiscal?.XMLNotaFiscal?.Numero.ToString() ?? string.Empty;

                            //Status de POD: approved ou rejected
                            if (cargaEntrega.DevolucaoParcial)
                            {
                                dadosPedidoNota.order_status = "partial";
                                dadosPedidoNota.pod_timestamp = cargaEntrega.DataConfirmacao?.ToString("o"); //Data e hora do POD
                                obteveDataIntegracao = true;

                            }
                            else if (cargaEntrega.Situacao == SituacaoEntrega.Entregue)
                            {
                                dadosPedidoNota.order_status = "approved";
                                dadosPedidoNota.pod_timestamp = cargaEntrega.DataConfirmacao?.ToString("o"); //Data e hora do POD
                                obteveDataIntegracao = true;
                            }
                            else if (cargaEntrega.Situacao == SituacaoEntrega.Rejeitado || cargaEntrega.Situacao == SituacaoEntrega.Reentergue)
                            {
                                dadosPedidoNota.order_status = "rejected";
                                dadosPedidoNota.pod_timestamp = cargaEntrega.DataRejeitado?.ToString("o"); //Data e hora do POD
                                obteveDataIntegracao = true;
                            }

                            dadosPedido.orders.Add(dadosPedidoNota);


                        }


                    }
                    break;

            }

            if (!obteveDataIntegracao) throw new ServicoException($"Falha na obtenção da data para integração: {integracao.PedidoOcorrenciaColetaEntrega.EventoColetaEntrega?.ObterDescricao()}");

            dynamic objeto = new ExpandoObject();
            objeto.orders_data = new List<dynamic>() { dadosPedido };
            return objeto;
        }

        private string ObterFalhaIntegracao(RetornoIntegracao retornoIntegracao)
        {
            StringBuilder falhaIntegracao = new StringBuilder();
            foreach (Response r in retornoIntegracao.Response)
                falhaIntegracao.Append($"Campo: {r.Description} | Valor: {r.Details[0].Code} | Motivo: {r.Details[0].Description}; ");
            return falhaIntegracao.ToString();
        }

        #endregion

        #region Classes Retorno MondelezDrivin
        public class Detail
        {
            public string Code { get; set; }
            public string Description { get; set; }
        }

        public class Response
        {
            public string Description { get; set; }
            public List<Detail> Details { get; set; }
        }

        public class RetornoIntegracao
        {
            public bool Success { get; set; }
            public string Status { get; set; }
            public List<Response> Response { get; set; }
        }
        #endregion
    }
}