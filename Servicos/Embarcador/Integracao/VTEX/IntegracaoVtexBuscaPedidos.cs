using Dominio.Entidades.Embarcador.Configuracoes;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Servicos.Embarcador.Integracao.VTEX
{
    public class IntegracaoVtexBuscaPedidos
    {
        #region Atributos Globais

        Repositorio.UnitOfWork unitOfWork;
        string stringConexaoAdmin;
        AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware;
        Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado;

        #endregion

        #region Construtores

        public IntegracaoVtexBuscaPedidos(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware _tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado _auditado, string stringConexaoAdmin)
        {
            this.unitOfWork = unitOfWork;
            this.tipoServicoMultisoftware = _tipoServicoMultisoftware;
            this.auditado = _auditado;
            this.stringConexaoAdmin = stringConexaoAdmin;
        }

        #endregion

        #region Métodos Públicos

        /// <summary>
        /// Integra Feed de pedidos
        /// </summary>
        public void IntegrarListaPedidosPorFeed()
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoVtex repConfiguracaoVtex = new Repositorio.Embarcador.Configuracoes.ConfiguracaoVtex(unitOfWork);
            Repositorio.Embarcador.Integracao.PedidoAguardandoIntegracao repPedidoAguardandoIntegracao = new Repositorio.Embarcador.Integracao.PedidoAguardandoIntegracao(unitOfWork);

            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repConfiguracao.BuscarConfiguracaoPadrao();

            List<ConfiguracaoVtex> configuracoes = repConfiguracaoVtex.BuscarTodos();

            foreach (ConfiguracaoVtex configuracao in configuracoes)
            {
                string request;
                string response;

                List<Dominio.ObjetosDeValor.Embarcador.Integracao.VTEX.RetornoPedidosFeed> listaPedidos = BuscarPedidosFeedNaApi(configuracao, out request, out response);

                Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao arquivoRequisicao = ArquivoIntegracao.SalvarArquivoIntegracao(request, "json", unitOfWork);
                Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao arquivoResposta = ArquivoIntegracao.SalvarArquivoIntegracao(response, "json", unitOfWork);

                List<string> listHandles = new List<string>();

                foreach (Dominio.ObjetosDeValor.Embarcador.Integracao.VTEX.RetornoPedidosFeed pedido in listaPedidos)
                {
                    Dominio.Entidades.Embarcador.Integracao.PedidoAguardandoIntegracao pedidoAguardandoIntegracao = new Dominio.Entidades.Embarcador.Integracao.PedidoAguardandoIntegracao();

                    unitOfWork.Start();

                    try
                    {
                        if (pedido.state == "payment-approved" || pedido.state == "authorize-fulfillment")//insere para tratar um a um depois
                        {
                            pedidoAguardandoIntegracao.Filial = configuracao.Filial;
                            pedidoAguardandoIntegracao.IdIntegracao = pedido.orderId;
                            pedidoAguardandoIntegracao.SituacaoIntegracao = SituacaoPedidoAguardandoIntegracao.AgIntegracao;
                            pedidoAguardandoIntegracao.CreatedAt = DateTime.Now;
                            pedidoAguardandoIntegracao.DataCriacaoPedido = pedido.availableDate.HasValue ? pedido.availableDate.Value : DateTime.Now;
                            pedidoAguardandoIntegracao.TipoIntegracao = TipoIntegracao.VTEX;
                            repPedidoAguardandoIntegracao.Inserir(pedidoAguardandoIntegracao);

                            AdicionarArquivoTransacao(pedidoAguardandoIntegracao, arquivoRequisicao, arquivoResposta, unitOfWork);

                            repPedidoAguardandoIntegracao.Inserir(pedidoAguardandoIntegracao);
                        }

                        if (pedido.state == "canceled")//ja faz o cancelamento nao precisa tratar depois
                        {
                            pedidoAguardandoIntegracao.Filial = configuracao.Filial;
                            pedidoAguardandoIntegracao.IdIntegracao = pedido.orderId;
                            pedidoAguardandoIntegracao.SituacaoIntegracao = SituacaoPedidoAguardandoIntegracao.Integrado;
                            pedidoAguardandoIntegracao.CreatedAt = DateTime.Now;
                            pedidoAguardandoIntegracao.DataCriacaoPedido = pedido.availableDate.HasValue ? pedido.availableDate.Value : DateTime.Now;
                            pedidoAguardandoIntegracao.TipoIntegracao = TipoIntegracao.VTEX;

                            repPedidoAguardandoIntegracao.Inserir(pedidoAguardandoIntegracao);

                            AdicionarArquivoTransacao(pedidoAguardandoIntegracao, arquivoRequisicao, arquivoResposta, unitOfWork);

                            CancelarPedidoFeedNoSistema(pedido, pedidoAguardandoIntegracao);

                            repPedidoAguardandoIntegracao.Atualizar(pedidoAguardandoIntegracao);

                        }

                        listHandles.Add(pedido.handle);
                    }
                    catch (ServicoException e)
                    {
                        pedidoAguardandoIntegracao.Informacao = e.Message;
                        pedidoAguardandoIntegracao.SituacaoIntegracao = SituacaoPedidoAguardandoIntegracao.ProblemaIntegracao;

                        repPedidoAguardandoIntegracao.Atualizar(pedidoAguardandoIntegracao);
                    }
                    catch (Exception e)
                    {
                        Log.TratarErro(e);

                        pedidoAguardandoIntegracao.Informacao = "Erro genérico";
                        pedidoAguardandoIntegracao.SituacaoIntegracao = SituacaoPedidoAguardandoIntegracao.ProblemaIntegracao;

                        repPedidoAguardandoIntegracao.Atualizar(pedidoAguardandoIntegracao);
                    }
                    finally
                    {
                        unitOfWork.CommitChanges();
                    }
                }

                confirmarRecebimentoPedidoFeed(listHandles, configuracao);
            }
        }

        /// <summary>
        /// Salva a lista de pedidos da VTEX no banco na entidade PedidoAguardandoIntegracao. Depois, cada um deles deve ser integrado separadamente.
        /// </summary>
        /// 
        //public void SalvarListaPedidos()
        //{
        //    Repositorio.Embarcador.Configuracoes.ConfiguracaoVtex repConfiguracaoVtex = new Repositorio.Embarcador.Configuracoes.ConfiguracaoVtex(unitOfWork);
        //    Repositorio.Embarcador.Integracao.PedidoAguardandoIntegracao repPedidoAguardandoIntegracao = new Repositorio.Embarcador.Integracao.PedidoAguardandoIntegracao(unitOfWork);

        //    var configuracoes = repConfiguracaoVtex.BuscarTodos();

        //    unitOfWork.Start();

        //    foreach (var configuracao in configuracoes)
        //    {
        //        string request = "";
        //        string response = "";

        //        var listaPedidos = BuscarListaPedidos(configuracao, configuracao.DataUltimaIntegracao ?? DateTime.Now.AddDays(-1), DateTime.Now.AddDays(1), out request, out response);
        //        Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao arquivoRequisicao = ArquivoIntegracao.SalvarArquivoIntegracao(request, "json", unitOfWork);
        //        Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao arquivoResposta = ArquivoIntegracao.SalvarArquivoIntegracao(response, "json", unitOfWork);

        //        // Salva pedidos no banco para integração posterior
        //        foreach (var pedido in listaPedidos)
        //        {
        //            Dominio.Entidades.Embarcador.Integracao.PedidoAguardandoIntegracao pedidoAguardandoIntegracao = new Dominio.Entidades.Embarcador.Integracao.PedidoAguardandoIntegracao();

        //            pedidoAguardandoIntegracao.Filial = configuracao.Filial;
        //            pedidoAguardandoIntegracao.IdIntegracao = pedido.orderId;
        //            pedidoAguardandoIntegracao.SituacaoIntegracao = SituacaoPedidoAguardandoIntegracao.AgIntegracao;
        //            pedidoAguardandoIntegracao.CreatedAt = DateTime.Now;
        //            pedidoAguardandoIntegracao.DataCriacaoPedido = pedido.creationDate;
        //            pedidoAguardandoIntegracao.TipoIntegracao = TipoIntegracao.VTEX;
        //            pedidoAguardandoIntegracao.Informacao = "";
        //            pedidoAguardandoIntegracao.NumeroTentativas = 1;
        //            repPedidoAguardandoIntegracao.Inserir(pedidoAguardandoIntegracao);
        //            AdicionarArquivoTransacao(pedidoAguardandoIntegracao, arquivoRequisicao, arquivoResposta, unitOfWork);
        //        }

        //        if (listaPedidos.Count > 0)
        //        {
        //            // Salva a última data de criação dos pedidos nas configurações da VTEX para ser usada posteriormente
        //            DateTime dataUltimoPedido = listaPedidos.LastOrDefault().creationDate;
        //            configuracao.DataUltimaIntegracao = dataUltimoPedido.AddMinutes(-1).AddSeconds(-dataUltimoPedido.Second).ToUniversalTime(); // Menos um minuto para consultar os proximos pedidos
        //            repConfiguracaoVtex.Atualizar(configuracao);
        //        }
        //    }

        //    unitOfWork.CommitChanges();
        //}

        /// <summary>
        /// Integra os pedidos, um por um.
        /// </summary>
        /// 
        public void IntegrarPedidos()
        {
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repConfiguracao.BuscarConfiguracaoPadrao();
            Repositorio.Embarcador.Integracao.PedidoAguardandoIntegracao repPedidoAguardandoIntegracao = new Repositorio.Embarcador.Integracao.PedidoAguardandoIntegracao(unitOfWork);
            List<Dominio.Entidades.Embarcador.Integracao.PedidoAguardandoIntegracao> listaPedidoAguardandoIntegracao = repPedidoAguardandoIntegracao.BuscarListaPorSituacoesETipoIntegracao(new List<SituacaoPedidoAguardandoIntegracao>() { SituacaoPedidoAguardandoIntegracao.AgIntegracao, SituacaoPedidoAguardandoIntegracao.ErroGenerico }, TipoIntegracao.VTEX, 2000, 6);

            //listaPedidoAguardandoIntegracao.Clear();
            //Dominio.Entidades.Embarcador.Integracao.PedidoAguardandoIntegracao integracaoDebug = repPedidoAguardandoIntegracao.BuscarPorCodigo(1007604);
            //listaPedidoAguardandoIntegracao.Add(integracaoDebug);

            Repositorio.Embarcador.Integracao.IntegracaoDocas repIntegracaoDocas = new Repositorio.Embarcador.Integracao.IntegracaoDocas(unitOfWork);
            List<Dominio.Entidades.Embarcador.Integracao.IntegracaoDocas> docas = repIntegracaoDocas.ConsultarTodos();
            (new Servicos.Embarcador.Integracao.VTEX.IntegracaoVtex(unitOfWork)).IntegracaoDeDocasVtex(false, docas);




            foreach (Dominio.Entidades.Embarcador.Integracao.PedidoAguardandoIntegracao pedidoAguardandoIntegracao in listaPedidoAguardandoIntegracao)
            {
                IntegrarPedidoAguardandoIntegracao(pedidoAguardandoIntegracao.Codigo, configuracaoEmbarcador, docas);
            }
        }

        public void NotificarFilaIntegracao()
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoVtex repConfiguracaoVtex = new Repositorio.Embarcador.Configuracoes.ConfiguracaoVtex(unitOfWork);
            ConfiguracaoVtex configuracaoVTEX = repConfiguracaoVtex.BuscarPrimeiroRegistro();

            if (configuracaoVTEX != null && !string.IsNullOrWhiteSpace(configuracaoVTEX.EmailsNotificacao) && configuracaoVTEX.QuantidadeNotificacao > 0)
            {
                Repositorio.Embarcador.Integracao.PedidoAguardandoIntegracao repPedidoAguardandoIntegracao = new Repositorio.Embarcador.Integracao.PedidoAguardandoIntegracao(unitOfWork);

                DateTime dataReferencia = DateTime.Now.AddMinutes(-25);
                int quantidadePendentes = repPedidoAguardandoIntegracao.ContarPorTipoESituacao(TipoIntegracao.VTEX, SituacaoPedidoAguardandoIntegracao.AgIntegracao, dataReferencia);

                if (quantidadePendentes > configuracaoVTEX.QuantidadeNotificacao)
                {
                    Servicos.Email svcEmail = new Servicos.Email(unitOfWork);

                    string assunto = "Integração VTEX pendentes.";
                    string ambiente = Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().IdentificacaoAmbiente;
                    if (!string.IsNullOrWhiteSpace(ambiente))
                        assunto = assunto + " - " + ambiente + ".";

                    StringBuilder stBuilder = new StringBuilder();
                    stBuilder
                        .Append("Atenção, existem " + quantidadePendentes + " integrações da VTEX pendentes.")
                        .AppendLine() // Quebra de linha
                        .AppendLine();

                    stBuilder.Append("<br /> <br />");
                    stBuilder.Append("Favor não responder! E-mail enviado automaticamente para: " + configuracaoVTEX.EmailsNotificacao).Append("<br /> <br />");

                    List<string> listaEmails = new List<string>();
                    listaEmails = configuracaoVTEX.EmailsNotificacao.Split(';').ToList();

                    System.Text.StringBuilder ss = new System.Text.StringBuilder();
                    ss.Append("MultiSoftware - http://www.multicte.com.br/ <br />");

                    for (int j = 0; j < listaEmails.Distinct().Count(); j++)
                        svcEmail.EnviarEmail(string.Empty, string.Empty, string.Empty, listaEmails[j], "", "", assunto, stBuilder.ToString(), string.Empty, null, ss.ToString(), true, listaEmails[j], 0, unitOfWork);

                }
            }
        }


        #endregion

        #region Métodos Privados

        /// <summary>
        /// Integra os pedidos, um por um.
        /// </summary>
        /// 
        public void IntegrarPedidoAguardandoIntegracao(long CodigoPedidoAguardandoIntegracao, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, List<Dominio.Entidades.Embarcador.Integracao.IntegracaoDocas> docas)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoVtex repConfiguracaoVtex = new Repositorio.Embarcador.Configuracoes.ConfiguracaoVtex(unitOfWork);
            Repositorio.Embarcador.Integracao.PedidoAguardandoIntegracao repPedidoAguardandoIntegracao = new Repositorio.Embarcador.Integracao.PedidoAguardandoIntegracao(unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);

            string request = string.Empty;
            string response = string.Empty;

            Dominio.Entidades.Embarcador.Integracao.PedidoAguardandoIntegracao pedidoAguardandoIntegracao = repPedidoAguardandoIntegracao.BuscarPorCodigo(CodigoPedidoAguardandoIntegracao);

            if (pedidoAguardandoIntegracao == null)
                return;

            if (pedidoAguardandoIntegracao.Filial == null)
                return;

            ConfiguracaoVtex configuracao = repConfiguracaoVtex.BuscarPorFilial(pedidoAguardandoIntegracao.Filial.Codigo);

            if (configuracao == null)
                return;

            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(stringConexaoAdmin);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.VTEX.Pedido dadosCompletos = BuscarPedidoNaApi(configuracao, pedidoAguardandoIntegracao.IdIntegracao, out request, out response);
                unitOfWork.Start();

                Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao arquivoRequisicao = ArquivoIntegracao.SalvarArquivoIntegracao(request, "json", unitOfWork);
                Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao arquivoResposta = ArquivoIntegracao.SalvarArquivoIntegracao(response, "json", unitOfWork);

                pedidoAguardandoIntegracao.NumeroTentativas += 1;

                if (dadosCompletos.status == "canceled")
                {
                    Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = repPedido.BuscarPorNumeroPedidoEmbarcador(dadosCompletos.orderId);

                    pedidoAguardandoIntegracao.SituacaoIntegracao = SituacaoPedidoAguardandoIntegracao.Integrado;

                    if (pedido != null)
                    {
                        string retorno = CancelarPedidoNoSistema(pedido, configuracaoEmbarcador);
                        pedidoAguardandoIntegracao.Informacao = "Pedido cancelado na vtex recebido pelo feed";

                        if (string.IsNullOrEmpty(retorno))
                        {
                            pedidoAguardandoIntegracao.Informacao = retorno;
                            pedidoAguardandoIntegracao.NumeroTentativas = 3;
                            pedidoAguardandoIntegracao.SituacaoIntegracao = SituacaoPedidoAguardandoIntegracao.ProblemaIntegracao;
                        }
                    }
                    else
                        pedidoAguardandoIntegracao.Informacao = "Pedido cancelado na vtex recebido pelo feed, não existente no sistema";


                    repPedidoAguardandoIntegracao.Atualizar(pedidoAguardandoIntegracao);

                    unitOfWork.CommitChanges();
                    return;
                }

                bool pedidoCadastrado = false;
                ValidarDadosPedido(dadosCompletos, pedidoAguardandoIntegracao, out pedidoCadastrado);

                if (pedidoCadastrado)
                {
                    pedidoAguardandoIntegracao.SituacaoIntegracao = SituacaoPedidoAguardandoIntegracao.Integrado;
                    pedidoAguardandoIntegracao.Informacao = "Pedido já está cadastrado no sistema";
                    repPedidoAguardandoIntegracao.Atualizar(pedidoAguardandoIntegracao);

                    unitOfWork.CommitChanges();
                    return;
                }

                CriadorPedidoIntegracaoVtex criadorPedidoIntegracaoVtex = new CriadorPedidoIntegracaoVtex(unitOfWork, unitOfWorkAdmin);
                criadorPedidoIntegracaoVtex.CriarPedido(dadosCompletos, pedidoAguardandoIntegracao, out string informacao, tipoServicoMultisoftware, configuracaoEmbarcador, docas);

                pedidoAguardandoIntegracao.SituacaoIntegracao = SituacaoPedidoAguardandoIntegracao.Integrado;
                pedidoAguardandoIntegracao.Informacao = informacao;

                AdicionarArquivoTransacao(pedidoAguardandoIntegracao, arquivoRequisicao, arquivoResposta, unitOfWork);

                repPedidoAguardandoIntegracao.Atualizar(pedidoAguardandoIntegracao);

                unitOfWork.CommitChanges();
            }
            catch (BaseException e)
            {
                unitOfWork.Rollback();

                unitOfWork.Start();

                pedidoAguardandoIntegracao.NumeroTentativas += 1;
                pedidoAguardandoIntegracao.Informacao = e.Message;
                pedidoAguardandoIntegracao.SituacaoIntegracao = SituacaoPedidoAguardandoIntegracao.ProblemaIntegracao;
                repPedidoAguardandoIntegracao.Atualizar(pedidoAguardandoIntegracao);

                if (!string.IsNullOrEmpty(request) && !string.IsNullOrEmpty(response))
                {
                    Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao arquivoRequisicao = ArquivoIntegracao.SalvarArquivoIntegracao(request, "json", unitOfWork);
                    Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao arquivoResposta = ArquivoIntegracao.SalvarArquivoIntegracao(response, "json", unitOfWork);
                    AdicionarArquivoTransacao(pedidoAguardandoIntegracao, arquivoRequisicao, arquivoResposta, unitOfWork);
                }

                unitOfWork.CommitChanges();

            }
            catch (Exception e)
            {
                unitOfWork.Rollback();
                Log.TratarErro(e);

                unitOfWork.Start();

                pedidoAguardandoIntegracao = repPedidoAguardandoIntegracao.BuscarPorCodigo(CodigoPedidoAguardandoIntegracao);
                pedidoAguardandoIntegracao.NumeroTentativas += 1;

                if (pedidoAguardandoIntegracao.NumeroTentativas == 5)
                {
                    pedidoAguardandoIntegracao.Informacao = "Erro genérico";
                    pedidoAguardandoIntegracao.SituacaoIntegracao = SituacaoPedidoAguardandoIntegracao.ProblemaIntegracao;
                }
                else
                    pedidoAguardandoIntegracao.SituacaoIntegracao = SituacaoPedidoAguardandoIntegracao.ErroGenerico;

                repPedidoAguardandoIntegracao.Atualizar(pedidoAguardandoIntegracao);

                if (!string.IsNullOrEmpty(request) && !string.IsNullOrEmpty(response))
                {
                    Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao arquivoRequisicao = ArquivoIntegracao.SalvarArquivoIntegracao(request, "json", unitOfWork);
                    Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao arquivoResposta = ArquivoIntegracao.SalvarArquivoIntegracao(response, "json", unitOfWork);
                    AdicionarArquivoTransacao(pedidoAguardandoIntegracao, arquivoRequisicao, arquivoResposta, unitOfWork);
                }

                unitOfWork.CommitChanges();
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
            }
        }

        /*
         * Retorna os dados completos de um pedido
         */
        private Dominio.ObjetosDeValor.Embarcador.Integracao.VTEX.Pedido BuscarPedidoNaApi(ConfiguracaoVtex configuracaoVtex, string orderId, out string request, out string response)
        {
            string Uri = $"https://{configuracaoVtex.AccountName}.{configuracaoVtex.Environment}.com.br/api/oms/pvt/orders/{orderId}";
            request = Uri;
            response = "";

            //HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoVtexBuscaPedidos));
            //client.BaseAddress = new Uri(Uri);
            //client.DefaultRequestHeaders.Accept.Clear();
            //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //client.DefaultRequestHeaders.Add("X-VTEX-API-AppToken", configuracaoVtex.XVtexApiAppToken);
            //client.DefaultRequestHeaders.Add("X-VTEX-API-AppKey", configuracaoVtex.XVtexApiAppKey);
            //client.DefaultRequestHeaders.Add("Connection", "keep-alive");
            //client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");
            //client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("Mozilla", "5.0"));

            //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            //var respostaServer = client.GetAsync(Uri).Result;

            //if (respostaServer.IsSuccessStatusCode)
            //{
            //    string body = respostaServer.Content.ReadAsStringAsync().Result;
            //    response = Encoding.UTF8.GetString(body);
            //    Dominio.ObjetosDeValor.Embarcador.Integracao.VTEX.Pedido pedido = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.VTEX.Pedido>(response);
            //    return pedido;
            //}

            RestClient client = new RestClient(Uri);
            client.Timeout = -1;
            RestRequest requestnew = new RestRequest(Method.GET);
            requestnew.AddHeader("X-VTEX-API-AppToken", configuracaoVtex.XVtexApiAppToken);
            requestnew.AddHeader("X-VTEX-API-AppKey", configuracaoVtex.XVtexApiAppKey);
            requestnew.AddHeader("Cookie", "janus_sid=e83b009b-11c5-4b35-86d0-9efe995b2966");
            IRestResponse responsenew = client.Execute(requestnew);
            response = responsenew.Content;

            Dominio.ObjetosDeValor.Embarcador.Integracao.VTEX.Pedido pedido = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.VTEX.Pedido>(response);
            return pedido;
        }

        /*
        * Retorna os dados completos de um pedido
        */
        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.VTEX.RetornoPedidosFeed> BuscarPedidosFeedNaApi(ConfiguracaoVtex configuracaoVtex, out string requeststring, out string responsestring)
        {
            string Uri = $"https://{configuracaoVtex.AccountName}.{configuracaoVtex.Environment}.com.br/api/orders/feed";
            requeststring = Uri;

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            RestClient client = new RestClient(Uri);
            client.Timeout = -1;
            RestRequest request = new RestRequest(Method.GET);
            request.AddHeader("X-VTEX-API-AppToken", configuracaoVtex.XVtexApiAppToken);
            request.AddHeader("X-VTEX-API-AppKey", configuracaoVtex.XVtexApiAppKey);

            IRestResponse response = client.Execute(request);

            List<Dominio.ObjetosDeValor.Embarcador.Integracao.VTEX.RetornoPedidosFeed> pedidos = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.VTEX.RetornoPedidosFeed>() { };
            responsestring = response.Content;

            if (response.StatusCode == HttpStatusCode.OK)
            {
                List<Dominio.ObjetosDeValor.Embarcador.Integracao.VTEX.RetornoPedidosFeed> retorno = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Integracao.VTEX.RetornoPedidosFeed>>(response.Content);
                pedidos.AddRange(retorno);
            }
            else
            {
                Repositorio.Embarcador.Integracao.PedidoAguardandoIntegracao repPedidoAguardandoIntegracao = new Repositorio.Embarcador.Integracao.PedidoAguardandoIntegracao(unitOfWork);
                Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao arquivoRequisicao = ArquivoIntegracao.SalvarArquivoIntegracao(requeststring, "json", unitOfWork);
                Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao arquivoResposta = ArquivoIntegracao.SalvarArquivoIntegracao(responsestring, "json", unitOfWork);

                Dominio.Entidades.Embarcador.Integracao.PedidoAguardandoIntegracao pedidoAguardandoIntegracao = new Dominio.Entidades.Embarcador.Integracao.PedidoAguardandoIntegracao();
                pedidoAguardandoIntegracao.IdIntegracao = "";
                pedidoAguardandoIntegracao.SituacaoIntegracao = SituacaoPedidoAguardandoIntegracao.ProblemaIntegracao;
                pedidoAguardandoIntegracao.CreatedAt = DateTime.Now;
                pedidoAguardandoIntegracao.DataCriacaoPedido = DateTime.Now;
                pedidoAguardandoIntegracao.TipoIntegracao = TipoIntegracao.VTEX;
                pedidoAguardandoIntegracao.Informacao = "Erro ao Buscar Pedidos no Feed da Vtex - URL: " + Uri + "Erro: " + response.ErrorMessage;

                repPedidoAguardandoIntegracao.Inserir(pedidoAguardandoIntegracao);

                AdicionarArquivoTransacao(pedidoAguardandoIntegracao, arquivoRequisicao, arquivoResposta, unitOfWork);

                Log.TratarErro("Retorno " + Uri + " - " + response.ErrorMessage);
            }

            return pedidos;
        }

        private void ValidarDadosPedido(Dominio.ObjetosDeValor.Embarcador.Integracao.VTEX.Pedido dadosPedido, Dominio.Entidades.Embarcador.Integracao.PedidoAguardandoIntegracao pedidoAguardandoIntegracao, out bool pedidocadastrado)
        {
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            pedidocadastrado = false;
            Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = repPedido.BuscarPorNumeroPedidoEmbarcador(dadosPedido.orderId);

            if (pedido != null && pedido.SituacaoPedido != SituacaoPedido.Cancelado)
                pedidocadastrado = true;
        }

        private string CancelarPedidoNoSistema(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador)
        {
            if (pedido != null && pedido.SituacaoPedido != SituacaoPedido.Cancelado)
            {
                if (pedido.CargasPedido.Count > 0)
                    return "A integração tentou cancelar o pedido, mas o cancelamento não foi finalizado porque ele já tem uma carga ligada à ele";

                Servicos.Embarcador.Pedido.Pedido.CancelarPedido(
                    out string erro,
                    pedido,
                    TipoPedidoCancelamento.Cancelamento,
                    null,
                    "Cancelamento pela integração VTEX",
                    unitOfWork,
                    tipoServicoMultisoftware,
                    auditado,
                    configuracaoEmbarcador, null
                );

                if (!string.IsNullOrEmpty(erro))
                    return erro;
            }

            return "";
        }

        private void CancelarPedidoFeedNoSistema(Dominio.ObjetosDeValor.Embarcador.Integracao.VTEX.RetornoPedidosFeed dadosPedido, Dominio.Entidades.Embarcador.Integracao.PedidoAguardandoIntegracao pedidoAguardandoIntegracao)
        {
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repositorioPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga repositorioConfiguracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork);

            Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = repPedido.BuscarPorNumeroPedidoEmbarcador(dadosPedido.orderId);

            if (pedido == null)
            {
                pedidoAguardandoIntegracao.Informacao = "Pedido" + dadosPedido.orderId + " cancelado pelo feed";
                return;
            }

            if (pedido.SituacaoPedido == SituacaoPedido.Cancelado)
                return;

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            if (!(pedido.CanalEntrega?.GerarCargaAutomaticamente ?? false))
            {
                if (pedido.CargasPedido.Count > 0)
                    throw new ServicoException("A integração tentou cancelar o pedido, mas o cancelamento não foi finalizado porque ele já tem uma carga ligada à ele");

                Servicos.Embarcador.Pedido.Pedido.CancelarPedido(
                    out string erro,
                    pedido,
                    TipoPedidoCancelamento.Cancelamento,
                    null,
                    "Cancelamento pela integração VTEX FEED",
                    unitOfWork,
                    tipoServicoMultisoftware,
                    auditado,
                    configuracaoTMS, null
                );

                if (!string.IsNullOrEmpty(erro))
                    throw new ServicoException(erro);

                return;
            }

            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repositorioCargaPedido.BuscarCargaAtualPorPedido(pedido.Codigo);

            if (cargaPedido == null)
                throw new ServicoException("A integração tentou cancelar o pedido, mas o cancelamento não foi finalizado porque o pedido não possui carga!");

            if (repositorioPedidoXMLNotaFiscal.VerificarSeExistePorCargaPedido(cargaPedido.Codigo))
                throw new ServicoException("A integração tentou cancelar o pedido, mas o cancelamento não foi finalizado porque o pedido já possui notas fiscais vinculadas!");

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = repositorioConfiguracaoGeralCarga.BuscarPrimeiroRegistro();

            Servicos.Embarcador.Carga.CargaPedido.RemoverPedidoCarga(cargaPedido.Carga, cargaPedido, configuracaoTMS, tipoServicoMultisoftware, unitOfWork, configuracaoGeralCarga, null, true);

            Dominio.ObjetosDeValor.Embarcador.Carga.CargaCancelamentoAdicionar cargaCancelamentoAdicionar = new Dominio.ObjetosDeValor.Embarcador.Carga.CargaCancelamentoAdicionar()
            {
                Carga = cargaPedido.Carga,
                MotivoCancelamento = "Cancelamento pela integração VTEX FEED",
                TipoServicoMultisoftware = tipoServicoMultisoftware,
                Usuario = null
            };

            Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cancelamentoCarga = Servicos.Embarcador.Carga.Cancelamento.GerarCargaCancelamento(cargaCancelamentoAdicionar, configuracaoTMS, unitOfWork);

            Servicos.Embarcador.Carga.Cancelamento.SolicitarCancelamentoCarga(ref cancelamentoCarga, unitOfWork, unitOfWork.StringConexao, tipoServicoMultisoftware);
            Servicos.Auditoria.Auditoria.Auditar(auditado, cargaPedido.Carga, "Adicionou cancelamento  via integração para pedidos que gerar carga automaticamente e não possui notas", unitOfWork);
            Servicos.Auditoria.Auditoria.Auditar(auditado, cargaPedido.Pedido, "Adicionou cancelamento  via integração para pedidos que gerar carga automaticamente e não possui notas", unitOfWork);
        }

        private bool confirmarRecebimentoPedidoFeed(List<string> listHandles, ConfiguracaoVtex configuracaoVtex)
        {
            string Uri = $"https://{configuracaoVtex.AccountName}.{configuracaoVtex.Environment}.com.br/api/orders/feed/";
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            Dominio.ObjetosDeValor.Embarcador.Integracao.VTEX.handlesEnvio handles = new Dominio.ObjetosDeValor.Embarcador.Integracao.VTEX.handlesEnvio();
            handles.handles = listHandles;

            string handlesjson = JsonConvert.SerializeObject(handles, Formatting.Indented);

            RestClient client = new RestClient(Uri);
            client.Timeout = -1;
            RestRequest request = new RestRequest(Method.POST);
            request.AddHeader("X-VTEX-API-AppToken", configuracaoVtex.XVtexApiAppToken);
            request.AddHeader("X-VTEX-API-AppKey", configuracaoVtex.XVtexApiAppKey);
            request.AddHeader("Content-Type", "application/json");

            request.AddParameter("application/json", handlesjson, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);

            if (response.StatusCode == HttpStatusCode.OK)
                return true;
            else
                return false;

        }

        private void AdicionarArquivoTransacao(Dominio.Entidades.Embarcador.Integracao.PedidoAguardandoIntegracao pedidoAguardandoIntegracao, Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao arquivoRequisicao, Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao arquivoResposta, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repositorioCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo()
            {
                ArquivoRequisicao = arquivoRequisicao,
                ArquivoResposta = arquivoResposta,
                Data = DateTime.Now,
                Mensagem = "Dados Obtidos para Procesamento",
                Tipo = TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento
            };

            repositorioCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

            if (pedidoAguardandoIntegracao.ArquivosTransacao == null)
                pedidoAguardandoIntegracao.ArquivosTransacao = new List<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>();

            try
            {
                pedidoAguardandoIntegracao.ArquivosTransacao.Add(arquivoIntegracao);
            }
            catch (Exception)
            {
                //vamos enterrar a excessao e tentar adicionar os arquivos;
                pedidoAguardandoIntegracao.ArquivosTransacao = new List<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>();
                pedidoAguardandoIntegracao.ArquivosTransacao.Add(arquivoIntegracao);
            }

        }

        #endregion
    }
}
