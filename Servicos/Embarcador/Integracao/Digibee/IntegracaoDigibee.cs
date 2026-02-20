using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Infrastructure.Services.HttpClientFactory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;

namespace Servicos.Embarcador.Integracao.Digibee
{
    public class IntegracaoDigibee
    {
        #region Métodos Privados

        private static Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo AdicionarArquivoTransacao(string jsonRequisicao, string jsonRetorno, DateTime data, string mensagem, Repositorio.UnitOfWork unitOfWork)
        {
            if (string.IsNullOrWhiteSpace(jsonRequisicao) && string.IsNullOrWhiteSpace(jsonRetorno))
                return null;

            Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repositorioCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo()
            {
                ArquivoRequisicao = ArquivoIntegracao.SalvarArquivoIntegracao(jsonRequisicao, "json", unitOfWork),
                ArquivoResposta = ArquivoIntegracao.SalvarArquivoIntegracao(jsonRetorno, "json", unitOfWork),
                Data = data,
                Mensagem = mensagem,
                Tipo = TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento
            };

            repositorioCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

            return arquivoIntegracao;
        }

        private static void AdicionarArquivoTransacaoIntegracaoPagamento(Dominio.Entidades.Embarcador.Escrituracao.PagamentoIntegracao pagamentoIntegracao, string mensagem, string jsonRequisicao, string jsonRetorno, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = AdicionarArquivoTransacao(jsonRequisicao, jsonRetorno, pagamentoIntegracao.DataIntegracao, mensagem, unitOfWork);

            if (arquivoIntegracao == null)
                return;

            if (pagamentoIntegracao.ArquivosTransacao == null)
                pagamentoIntegracao.ArquivosTransacao = new List<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>();

            pagamentoIntegracao.ArquivosTransacao.Add(arquivoIntegracao);
        }

        private static void AdicionarArquivoTransacaoIntegracaoEscrituracao(Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoIntegracao loteEscrituracaoIntegracao, string mensagem, string jsonRequisicao, string jsonRetorno, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = AdicionarArquivoTransacao(jsonRequisicao, jsonRetorno, loteEscrituracaoIntegracao.DataIntegracao, mensagem, unitOfWork);

            if (arquivoIntegracao == null)
                return;

            if (loteEscrituracaoIntegracao.ArquivosTransacao == null)
                loteEscrituracaoIntegracao.ArquivosTransacao = new List<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>();

            loteEscrituracaoIntegracao.ArquivosTransacao.Add(arquivoIntegracao);
        }

        private static void AdicionarArquivoTransacaoIntegracaoCargaTransporte(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao, string mensagem, string jsonRequisicao, string jsonRetorno, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = AdicionarArquivoTransacao(jsonRequisicao, jsonRetorno, cargaIntegracao.DataIntegracao, mensagem, unitOfWork);

            if (arquivoIntegracao == null)
                return;

            if (cargaIntegracao.ArquivosTransacao == null)
                cargaIntegracao.ArquivosTransacao = new List<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>();

            cargaIntegracao.ArquivosTransacao.Add(arquivoIntegracao);
        }

        private static void AdicionarArquivoTransacaoIntegracaoCargaDadosTransporte(Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaIntegracao, string mensagem, string jsonRequisicao, string jsonRetorno, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = AdicionarArquivoTransacao(jsonRequisicao, jsonRetorno, cargaIntegracao.DataIntegracao, mensagem, unitOfWork);

            if (arquivoIntegracao == null)
                return;

            if (cargaIntegracao.ArquivosTransacao == null)
                cargaIntegracao.ArquivosTransacao = new List<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>();

            cargaIntegracao.ArquivosTransacao.Add(arquivoIntegracao);
        }

        private static void AdicionarArquivoTransacaoIntegracaoCarregamento(Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoIntegracao carregamentoIntegracao, string mensagem, string jsonRequisicao, string jsonRetorno, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = AdicionarArquivoTransacao(jsonRequisicao, jsonRetorno, carregamentoIntegracao.DataIntegracao, mensagem, unitOfWork);

            if (arquivoIntegracao == null)
                return;

            if (carregamentoIntegracao.ArquivosTransacao == null)
                carregamentoIntegracao.ArquivosTransacao = new List<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>();

            carregamentoIntegracao.ArquivosTransacao.Add(arquivoIntegracao);
        }

        private static void AdicionarArquivoTransacaoIntegracaoPedidoCancelamentoReserva(Dominio.Entidades.Embarcador.Cargas.MontagemCarga.PedidoCancelamentoReservaIntegracao pedidoCancelamentoReservaIntegracao, string mensagem, string jsonRequisicao, string jsonRetorno, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = AdicionarArquivoTransacao(jsonRequisicao, jsonRetorno, pedidoCancelamentoReservaIntegracao.DataIntegracao, mensagem, unitOfWork);

            if (arquivoIntegracao == null)
                return;

            if (pedidoCancelamentoReservaIntegracao.ArquivosTransacao == null)
                pedidoCancelamentoReservaIntegracao.ArquivosTransacao = new List<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>();

            pedidoCancelamentoReservaIntegracao.ArquivosTransacao.Add(arquivoIntegracao);
        }

        private static void IntegracaoCarregamento(Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoIntegracao carregamentoIntegracao, Dominio.Entidades.Embarcador.Configuracoes.Integracao integracao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(unitOfWork);
            Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoIntegracao repCarregamentoIntegracao = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoIntegracao(unitOfWork);
            Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido repCarregamentoPedido = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido(unitOfWork);
            Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto repCarregamentoPedidoProduto = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Pedido.RegraNumeroPedidoEmbarcador servicoRegraNumeroPedidoEmbarcador = new Pedido.RegraNumeroPedidoEmbarcador(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido> carregamentoPedidos = repCarregamentoPedido.BuscarPorCarregamento(carregamentoIntegracao.Carregamento.Codigo);
            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto> carregamentoProdutos = repCarregamentoPedidoProduto.BuscarPorCarregamento(carregamentoIntegracao.Carregamento.Codigo);
            Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCarregamento(carregamentoIntegracao.Carregamento.Codigo);

            if (carga != null)
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.Digibee.TokenAutenticacao token = ObterTokenAutenticacao(integracao);

                if (!token.FalhaAutenticacao)
                {
                    bool problemaIntegracao = false;
                    List<string> mensagensRetorno = new List<string>();
                    try
                    {
                        foreach (Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido carregamentoPedido in carregamentoPedidos)
                        {

                            HttpClient client = ObterClienteRequisicao(integracao.URLIntegracaoDigibee, integracao.APIKeyDigibee, token.Token);
                            string numeroPedidoEmbarcador = servicoRegraNumeroPedidoEmbarcador.ObterNumeroPedidoEmbarcadorSemRegra(carregamentoPedido.Pedido).ObterSomenteNumeros();
                            string jsonRequest = RetornarPedido(carga, carregamentoPedido, carregamentoProdutos, numeroPedidoEmbarcador);
                            var content = new StringContent(jsonRequest.ToString(), Encoding.UTF8, "application/json");
                            var result = client.PostAsync(integracao.URLIntegracaoDigibee, content).Result;
                            string jsonResponse = result.Content.ReadAsStringAsync().Result;
                            Dominio.ObjetosDeValor.Embarcador.Integracao.Digibee.Retorno retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Digibee.Retorno>(result.Content.ReadAsStringAsync().Result);
                            string mensagemRetorno;

                            if (retorno.code == 200)
                                mensagemRetorno = $"Integração do pedido {numeroPedidoEmbarcador} realizada com sucesso";
                            else
                            {
                                problemaIntegracao = true; ;
                                mensagemRetorno = $"Falha ao integrar o pedido {numeroPedidoEmbarcador}: {(retorno?.error ?? "Falha genérica retornada")}";
                            }

                            mensagensRetorno.Add(mensagemRetorno);
                            AdicionarArquivoTransacaoIntegracaoCarregamento(carregamentoIntegracao, mensagemRetorno, jsonRequest, jsonResponse, unitOfWork);
                        }

                        carregamentoIntegracao.SituacaoIntegracao = problemaIntegracao ? SituacaoIntegracao.ProblemaIntegracao : SituacaoIntegracao.Integrado;
                        carregamentoIntegracao.ProblemaIntegracao = string.Join(" | ", mensagensRetorno);
                    }
                    catch (Exception ex)
                    {
                        Servicos.Log.TratarErro(ex);
                        carregamentoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                        carregamentoIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao integrar";
                    }
                }
                else
                {
                    carregamentoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                    carregamentoIntegracao.ProblemaIntegracao = token.MensagemErro;

                    AdicionarArquivoTransacaoIntegracaoCarregamento(carregamentoIntegracao, token.MensagemErro, token.JsonRequisicao, token.JsonRetorno, unitOfWork);
                }
            }
            else
            {
                carregamentoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                carregamentoIntegracao.ProblemaIntegracao = "Nenhuma carga gerada para o carregamento";
            }
        }

        private static void IntegracaoCarregamentoConsinco(Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoIntegracao carregamentoIntegracao, Dominio.Entidades.Embarcador.Configuracoes.Integracao integracao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(unitOfWork);
            Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoIntegracao repCarregamentoIntegracao = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoIntegracao(unitOfWork);
            Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido repCarregamentoPedido = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido(unitOfWork);
            Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto repCarregamentoPedidoProduto = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);


            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido> carregamentoPedidos = repCarregamentoPedido.BuscarPorCarregamento(carregamentoIntegracao.Carregamento.Codigo);
            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto> carregamentoProdutos = repCarregamentoPedidoProduto.BuscarPorCarregamento(carregamentoIntegracao.Carregamento.Codigo);
            Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCarregamento(carregamentoIntegracao.Carregamento.Codigo);

            try
            {
                if (carga != null)
                {

                    Dominio.ObjetosDeValor.Embarcador.Integracao.Digibee.TokenAutenticacao token = ObterTokenAutenticacao(integracao);

                    if (!token.FalhaAutenticacao)
                    {
                        HttpClient client = ObterClienteRequisicao(integracao.URLIntegracaoDigibee, integracao.APIKeyDigibeeGeral, token.Token);
                        string jsonRequest = RetornarPedidoPadraoConsisco(carga, carregamentoPedidos, carregamentoProdutos, unitOfWork);
                        var content = new StringContent(jsonRequest.ToString(), Encoding.UTF8, "application/json");

                        var result = client.PostAsync(integracao.URLIntegracaoDigibee, content).Result;
                        string jsonResponse = result.Content.ReadAsStringAsync().Result;

                        Dominio.ObjetosDeValor.Embarcador.Integracao.Digibee.Retorno retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Digibee.Retorno>(result.Content.ReadAsStringAsync().Result);

                        if (retorno.code == 200)
                        {
                            carregamentoIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                            carregamentoIntegracao.ProblemaIntegracao = "Integração realizada com sucesso";
                        }
                        else
                        {
                            carregamentoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                            carregamentoIntegracao.ProblemaIntegracao = Utilidades.String.Left(retorno?.error ?? "Falha genérica retornada", 200);
                        }

                        AdicionarArquivoTransacaoIntegracaoCarregamento(carregamentoIntegracao, carregamentoIntegracao.ProblemaIntegracao, jsonRequest, jsonResponse, unitOfWork);
                    }
                    else
                    {
                        carregamentoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                        carregamentoIntegracao.ProblemaIntegracao = token.MensagemErro;

                        AdicionarArquivoTransacaoIntegracaoCarregamento(carregamentoIntegracao, token.MensagemErro, token.JsonRequisicao, token.JsonRetorno, unitOfWork);
                    }
                }
                else
                {
                    carregamentoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                    carregamentoIntegracao.ProblemaIntegracao = "Nenhuma carga gerada para o carregamento";
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                carregamentoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                carregamentoIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao integrar";
            }
        }

        private static void IntegracaoPedidoCancelamentoReserva(Dominio.Entidades.Embarcador.Cargas.MontagemCarga.PedidoCancelamentoReservaIntegracao pedidoCancelamentoReservaIntegracao, Dominio.Entidades.Embarcador.Configuracoes.Integracao integracao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(unitOfWork);
            Repositorio.Embarcador.Cargas.MontagemCarga.PedidoCancelamentoReservaIntegracao repPedidoCancelamentoReservaIntegracao = new Repositorio.Embarcador.Cargas.MontagemCarga.PedidoCancelamentoReservaIntegracao(unitOfWork);
            Repositorio.Embarcador.Cargas.MontagemCarga.PedidoProdutoCancelamentoReservaIntegracao repPedidoProdutoCancelamentoReservaIntegracao = new Repositorio.Embarcador.Cargas.MontagemCarga.PedidoProdutoCancelamentoReservaIntegracao(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.PedidoProdutoCancelamentoReservaIntegracao> produtosCancelamento = repPedidoProdutoCancelamentoReservaIntegracao.BuscarPorPedidoIntegracao(pedidoCancelamentoReservaIntegracao.Codigo);

            if (pedidoCancelamentoReservaIntegracao != null)
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.Digibee.TokenAutenticacao token = ObterTokenAutenticacao(integracao);

                if (!token.FalhaAutenticacao)
                {
                    try
                    {
                        HttpClient client = ObterClienteRequisicao(integracao.URLIntegracaoCancelamentoDigibee, integracao.APIKeyDigibee, token.Token);

                        //client.DefaultRequestHeaders.Add("Authorization", (token.jwt ? "Bearer " : "") + token.token);

                        var pedidoDigibee = new
                        {
                            numeroPedidoERP = pedidoCancelamentoReservaIntegracao.Pedido.NumeroPedidoEmbarcador,
                            itensCancelados = new List<dynamic>()
                        };
                        for (int i = 0; i < produtosCancelamento.Count; i++)
                        {
                            pedidoDigibee.itensCancelados.Add(new
                            {
                                IdDemanda = produtosCancelamento[i].PedidoProduto?.IdDemanda ?? "",
                                codigoProduto = produtosCancelamento[i].PedidoProduto.Produto.CodigoProdutoEmbarcador,
                                quantidadeUnitariaPedida = (int)produtosCancelamento[i].Quantidade
                            });
                        }

                        string jsonRequest = JsonConvert.SerializeObject(pedidoDigibee, Newtonsoft.Json.Formatting.Indented);
                        var content = new StringContent(jsonRequest.ToString(), Encoding.UTF8, "application/json");

                        var result = client.PostAsync(integracao.URLIntegracaoCancelamentoDigibee, content).Result;
                        string jsonResponse = result.Content.ReadAsStringAsync().Result;

                        Dominio.ObjetosDeValor.Embarcador.Integracao.Digibee.Retorno retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Digibee.Retorno>(result.Content.ReadAsStringAsync().Result);

                        if (retorno.code == 200)
                        {
                            pedidoCancelamentoReservaIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                            pedidoCancelamentoReservaIntegracao.ProblemaIntegracao = "Integração realizada com sucesso" + (!string.IsNullOrEmpty(retorno?.message ?? "") ? " - " + retorno?.message : "");
                        }
                        else
                        {
                            pedidoCancelamentoReservaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                            pedidoCancelamentoReservaIntegracao.ProblemaIntegracao = retorno?.error ?? "Falha genérica retornada";
                        }

                        AdicionarArquivoTransacaoIntegracaoPedidoCancelamentoReserva(pedidoCancelamentoReservaIntegracao, pedidoCancelamentoReservaIntegracao.ProblemaIntegracao, jsonRequest, jsonResponse, unitOfWork);
                    }
                    catch (Exception ex)
                    {
                        Servicos.Log.TratarErro(ex);
                        pedidoCancelamentoReservaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                        pedidoCancelamentoReservaIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao integrar";
                    }
                }
                else
                {
                    pedidoCancelamentoReservaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                    pedidoCancelamentoReservaIntegracao.ProblemaIntegracao = token.MensagemErro;

                    AdicionarArquivoTransacaoIntegracaoPedidoCancelamentoReserva(pedidoCancelamentoReservaIntegracao, token.MensagemErro, token.JsonRequisicao, token.JsonRetorno, unitOfWork);
                }
            }
            else
            {
                pedidoCancelamentoReservaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                pedidoCancelamentoReservaIntegracao.ProblemaIntegracao = "Nenhuma carga gerada para o carregamento";
            }
        }

        private static void IntegracaoPedidoCancelamentoReservaConsinco(Dominio.Entidades.Embarcador.Cargas.MontagemCarga.PedidoCancelamentoReservaIntegracao pedidoCancelamentoReservaIntegracao, Dominio.Entidades.Embarcador.Configuracoes.Integracao integracao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(unitOfWork);
            Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoIntegracao repCarregamentoIntegracao = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoIntegracao(unitOfWork);
            Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido repCarregamentoPedido = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Cargas.MontagemCarga.PedidoCancelamentoReservaIntegracao repPedidoCancReserva = new Repositorio.Embarcador.Cargas.MontagemCarga.PedidoCancelamentoReservaIntegracao(unitOfWork);
            Pedido.RegraNumeroPedidoEmbarcador servicoRegraNumeroPedidoEmbarcador = new Pedido.RegraNumeroPedidoEmbarcador(unitOfWork);

            pedidoCancelamentoReservaIntegracao.DataIntegracao = DateTime.Now;
            pedidoCancelamentoReservaIntegracao.NumeroTentativas++;

            Dominio.ObjetosDeValor.Embarcador.Integracao.Digibee.TokenAutenticacao token = ObterTokenAutenticacao(integracao);
            if (!token.FalhaAutenticacao)
            {
                if (integracao == null || string.IsNullOrWhiteSpace(integracao.APIKeyDigibee) || string.IsNullOrWhiteSpace(integracao.URLIntegracaoCancelamentoDigibee))
                {
                    pedidoCancelamentoReservaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                    pedidoCancelamentoReservaIntegracao.ProblemaIntegracao = "Não está configurada a integração com a dibibee";
                }
                else
                {
                    try
                    {
                        HttpClient client = ObterClienteRequisicao(integracao.URLIntegracaoCancelamentoDigibee, integracao.APIKeyDigibeeGeral, token.Token);
                        string numeroPedidoEmbarcador = servicoRegraNumeroPedidoEmbarcador.ObterNumeroPedidoEmbarcadorSemRegra(pedidoCancelamentoReservaIntegracao.Pedido);
                        var jsonRequest = RetornarPedidoCancelamentoReservaConsinco(pedidoCancelamentoReservaIntegracao, numeroPedidoEmbarcador, unitOfWork);

                        var content = new StringContent(jsonRequest.ToString(), Encoding.UTF8, "application/json");

                        var result = client.PostAsync(integracao.URLIntegracaoCancelamentoDigibee, content).Result;
                        string jsonResponse = result.Content.ReadAsStringAsync().Result;

                        Dominio.ObjetosDeValor.Embarcador.Integracao.Digibee.Retorno retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Digibee.Retorno>(result.Content.ReadAsStringAsync().Result);

                        if (retorno.code == 200)
                        {
                            pedidoCancelamentoReservaIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                            pedidoCancelamentoReservaIntegracao.ProblemaIntegracao = "Integração realizada com sucesso";
                        }
                        else
                        {
                            pedidoCancelamentoReservaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                            pedidoCancelamentoReservaIntegracao.ProblemaIntegracao = retorno?.error ?? "Falha genérica retornada";
                        }

                        AdicionarArquivoTransacaoIntegracaoPedidoCancelamentoReserva(pedidoCancelamentoReservaIntegracao, pedidoCancelamentoReservaIntegracao.ProblemaIntegracao, jsonRequest, jsonResponse, unitOfWork);
                    }
                    catch (Exception ex)
                    {
                        Servicos.Log.TratarErro(ex);
                        pedidoCancelamentoReservaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                        pedidoCancelamentoReservaIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao integrar o cancelamento";
                    }

                }

                repPedidoCancReserva.Atualizar(pedidoCancelamentoReservaIntegracao);
            }
            else
            {
                pedidoCancelamentoReservaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                pedidoCancelamentoReservaIntegracao.ProblemaIntegracao = token.MensagemErro;

                AdicionarArquivoTransacaoIntegracaoPedidoCancelamentoReserva(pedidoCancelamentoReservaIntegracao, token.MensagemErro, token.JsonRequisicao, token.JsonRetorno, unitOfWork);
            }
        }

        private static HttpClient ObterClienteRequisicao(string url, string apiKey)
        {
            return ObterClienteRequisicao(url, apiKey, token: string.Empty);
        }

        private static HttpClient ObterClienteRequisicao(string url, string apiKey, string token)
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoDigibee));

            client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("apiKey", apiKey);

            if (!string.IsNullOrWhiteSpace(token))
                client.DefaultRequestHeaders.Add("Authorization", token);

            return client;
        }  

        private static Dominio.ObjetosDeValor.Embarcador.Integracao.Digibee.TokenAutenticacao ObterTokenAutenticacao(Dominio.Entidades.Embarcador.Configuracoes.Integracao integracao)
        {
            if (string.IsNullOrWhiteSpace(integracao.URLAutenticacaoDigibee))
                return new Dominio.ObjetosDeValor.Embarcador.Integracao.Digibee.TokenAutenticacao();

            Dominio.ObjetosDeValor.Embarcador.Integracao.Digibee.Credencial credencial = new Dominio.ObjetosDeValor.Embarcador.Integracao.Digibee.Credencial()
            {
                username = integracao.UsuarioAutenticacaoDigibee,
                token = integracao.SenhaAutenticacaoDigibee
            };

            string jsonRequisicao = JsonConvert.SerializeObject(credencial, Formatting.Indented);
            StringContent dadosAutenticacao = new StringContent(jsonRequisicao.ToString(), Encoding.UTF8, "application/json");
            HttpClient client = ObterClienteRequisicao(integracao.URLAutenticacaoDigibee, integracao.APIKeyDigibee);
            HttpResponseMessage retornoRequisicao = client.PostAsync(integracao.URLAutenticacaoDigibee, dadosAutenticacao).Result;
            string jsonRetornoRequisicao = retornoRequisicao.Content.ReadAsStringAsync().Result;
            Dominio.ObjetosDeValor.Embarcador.Integracao.Digibee.Retorno retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Digibee.Retorno>(jsonRetornoRequisicao);
            Dominio.ObjetosDeValor.Embarcador.Integracao.Digibee.TokenAutenticacao token = new Dominio.ObjetosDeValor.Embarcador.Integracao.Digibee.TokenAutenticacao();

            if ((retorno.code == 200) || (retorno.code == 0))
            {
                token.Token = retornoRequisicao.Headers.GetValues("Authorization").FirstOrDefault();

                //if (string.IsNullOrEmpty(token.Token)) // Não tenho documentação da autenticação JWT no qual o COBREBEM irá utilizar em produção, pelo que vi, o tokem não irá estar nos header e sim no body.
                //{
                //    token.Token = jsonResponse;
                //    token.Jwt = true;
                //}
            }
            else
            {
                token.FalhaAutenticacao = true;
                token.MensagemErro = retorno?.error ?? "";
            }

            token.JsonRequisicao = jsonRequisicao;
            token.JsonRetorno = jsonRetornoRequisicao;

            return token;
        }

        private static string RetornarPedido(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido carregamentoPedido, List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto> carregamentoProdutos, string numeroPedidoEmbarcador)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Digibee.Pedido pedidoDigibee = new Dominio.ObjetosDeValor.Embarcador.Integracao.Digibee.Pedido();
            pedidoDigibee.site = carregamentoPedido.Pedido.Filial?.CodigoFilialEmbarcador ?? "";
            pedidoDigibee.codigoDepositante = carregamentoPedido.Pedido.Filial?.CodigoFilialEmbarcador ?? "";
            pedidoDigibee.numeroPedido = carregamentoPedido.Pedido.Codigo;
            pedidoDigibee.protocoloIntegracaoCarga = carga.Codigo;
            pedidoDigibee.numeroCarga = carregamentoPedido.Carregamento.NumeroCarregamento;
            pedidoDigibee.codigoTransportadora = !string.IsNullOrWhiteSpace(carregamentoPedido.Carregamento.Empresa?.CodigoIntegracao) ? carregamentoPedido.Carregamento.Empresa.CodigoIntegracao : "000";
            pedidoDigibee.dataDoPedidoPrevisto = carregamentoPedido.Pedido.DataEntrega?.ToString("yyyy-MM-ddTHH:mm:ssZ") ?? (carregamentoPedido.Pedido.DataCriacao?.ToString("yyyy-MM-ddTHH:mm:ssZ") ?? DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ"));
            pedidoDigibee.codigoCliente = !string.IsNullOrWhiteSpace(carregamentoPedido.Pedido.Destinatario?.CodigoIntegracao) ? carregamentoPedido.Pedido.Destinatario.CodigoIntegracao : "000";
            pedidoDigibee.tipoDePedido = carregamentoPedido.Carregamento.TipoOperacao?.CodigoIntegracao ?? "";
            pedidoDigibee.itens = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Digibee.Produto>();
            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto> carregamentoPedidoProdutos = (from obj in carregamentoProdutos where obj.CarregamentoPedido.Codigo == carregamentoPedido.Codigo select obj).ToList();
            for (int i = 0; i < carregamentoPedidoProdutos.Count; i++)
            {
                Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto carregamentoPedidoProduto = carregamentoPedidoProdutos[i];
                Dominio.ObjetosDeValor.Embarcador.Integracao.Digibee.Produto produtoDigibee = new Dominio.ObjetosDeValor.Embarcador.Integracao.Digibee.Produto();
                produtoDigibee.pedidoERP = numeroPedidoEmbarcador;
                produtoDigibee.numeroItem = i + 1;
                produtoDigibee.codigoProduto = carregamentoPedidoProduto.PedidoProduto.Produto.CodigoProdutoEmbarcador;
                produtoDigibee.quantidade = (int)carregamentoPedidoProduto.Quantidade;
                produtoDigibee.unidade = string.IsNullOrWhiteSpace(carregamentoPedidoProduto.PedidoProduto.Produto.SiglaUnidade) ? "UN" : carregamentoPedidoProduto.PedidoProduto.Produto.SiglaUnidade;
                produtoDigibee.statusDoEstoque = "00";
                produtoDigibee.motivoDoBloqueioDeQualidade = null;
                pedidoDigibee.itens.Add(produtoDigibee);
            }
            string jsonRequest = JsonConvert.SerializeObject(pedidoDigibee, Newtonsoft.Json.Formatting.Indented);

            return jsonRequest;
        }

        private static string RetornarPedidoCancelamentoReservaConsinco(Dominio.Entidades.Embarcador.Cargas.MontagemCarga.PedidoCancelamentoReservaIntegracao pedidoCancelamentoReservaIntegracao, string numeroPedidoEmbarcador, Repositorio.UnitOfWork unitOfWork)
        {
            int codigoOrigem, codigoDestino;
            int.TryParse(pedidoCancelamentoReservaIntegracao.Pedido.Remetente?.CodigoIntegracao ?? "0", out codigoOrigem);
            int.TryParse(pedidoCancelamentoReservaIntegracao.Pedido.Destinatario?.CodigoIntegracao ?? "0", out codigoDestino);

            Repositorio.Embarcador.Cargas.MontagemCarga.PedidoProdutoCancelamentoReservaIntegracao repCarregamentoPedidoProduto = new Repositorio.Embarcador.Cargas.MontagemCarga.PedidoProdutoCancelamentoReservaIntegracao(unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.PedidoProdutoCancelamentoReservaIntegracao> pedidoProdutoCanc = repCarregamentoPedidoProduto.BuscarPorPedidoIntegracao(pedidoCancelamentoReservaIntegracao.Codigo);
            Dominio.ObjetosDeValor.Embarcador.Integracao.Digibee.CancelamentoReservaConsinco cancelamentoReservaConsinco = new Dominio.ObjetosDeValor.Embarcador.Integracao.Digibee.CancelamentoReservaConsinco();
            cancelamentoReservaConsinco.codigoDestino = codigoDestino;
            cancelamentoReservaConsinco.codigoOrigem = codigoOrigem;
            cancelamentoReservaConsinco.dataCancelamento = pedidoCancelamentoReservaIntegracao.DataCancelamento?.ToString("dd/MM/yyyy HH:mm:ss") ?? DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            cancelamentoReservaConsinco.numeroPedido = numeroPedidoEmbarcador.ToInt();
            cancelamentoReservaConsinco.usuario = pedidoCancelamentoReservaIntegracao.Usuario?.Nome ?? "";
            cancelamentoReservaConsinco.itens = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Digibee.ProdutoCancelamentoReservaConsinco>();
            foreach (var cargaPedidoProduto in pedidoProdutoCanc)
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.Digibee.ProdutoCancelamentoReservaConsinco produtoCancelamentoReservaConsinco = new Dominio.ObjetosDeValor.Embarcador.Integracao.Digibee.ProdutoCancelamentoReservaConsinco();
                produtoCancelamentoReservaConsinco.IdDemanda = cargaPedidoProduto.PedidoProduto?.IdDemanda ?? "";
                produtoCancelamentoReservaConsinco.codigoProduto = cargaPedidoProduto.PedidoProduto.Produto.CodigoProdutoEmbarcador;
                produtoCancelamentoReservaConsinco.quantidadeCancelada = cargaPedidoProduto.Quantidade;
                cancelamentoReservaConsinco.itens.Add(produtoCancelamentoReservaConsinco);
            }

            string jsonRequest = JsonConvert.SerializeObject(cancelamentoReservaConsinco, Newtonsoft.Json.Formatting.Indented);
            return jsonRequest;
        }

        private static string RetornarPedidoPadraoConsisco(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido> carregamentoPedidos, List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto> carregamentoProdutos, Repositorio.UnitOfWork unitOfWork)
        {
            //Dominio.ObjetosDeValor.Embarcador.Integracao.Digibee.RequestConsinco requestConsinco = new Dominio.ObjetosDeValor.Embarcador.Integracao.Digibee.RequestConsinco();
            Dominio.ObjetosDeValor.Embarcador.Integracao.Digibee.CarregamentoConsinco carregamentoConsinco = new Dominio.ObjetosDeValor.Embarcador.Integracao.Digibee.CarregamentoConsinco();
            carregamentoConsinco.numeroCargaTMS = carga.Codigo;
            carregamentoConsinco.modeloveicular = carga.ModeloVeicularCarga?.CodigoIntegracao ?? "";
            carregamentoConsinco.usuarioCarga = carga.OperadorInsercao?.Login ?? carga.Carregamento?.SessaoRoteirizador?.Usuario?.Login;
            carregamentoConsinco.pedidos = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Digibee.PedidoConsinco>();
            Pedido.RegraNumeroPedidoEmbarcador servicoRegraNumeroPedidoEmbarcador = new Pedido.RegraNumeroPedidoEmbarcador(unitOfWork);

            foreach (Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido carregamentoPedido in carregamentoPedidos)
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.Digibee.PedidoConsinco pedidoConsinco = new Dominio.ObjetosDeValor.Embarcador.Integracao.Digibee.PedidoConsinco();
                pedidoConsinco.numeroPedidoTMS = carregamentoPedido.Pedido.Codigo;

                int codigoOrigem, codigoDestino;
                int.TryParse(carregamentoPedido.Pedido.Remetente?.CodigoIntegracao ?? "0", out codigoOrigem);
                int.TryParse(carregamentoPedido.Pedido.Destinatario?.CodigoIntegracao ?? "0", out codigoDestino);
                pedidoConsinco.numeroPedidoTMS = carregamentoPedido.Pedido.Codigo;
                pedidoConsinco.codigoOrigem = codigoOrigem;
                pedidoConsinco.codigoDestino = codigoDestino;
                pedidoConsinco.numeroPedidoERP = servicoRegraNumeroPedidoEmbarcador.ObterNumeroPedidoEmbarcadorSemRegra(carregamentoPedido.Pedido).ToInt();

                pedidoConsinco.itens = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Digibee.ProdutoConsinco>();
                List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto> carregamentoPedidoProdutos = (from obj in carregamentoProdutos where obj.CarregamentoPedido.Codigo == carregamentoPedido.Codigo select obj).ToList();
                for (int i = 0; i < carregamentoPedidoProdutos.Count; i++)
                {
                    Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto carregamentoPedidoProduto = carregamentoPedidoProdutos[i];
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Digibee.ProdutoConsinco produtoConsinco = new Dominio.ObjetosDeValor.Embarcador.Integracao.Digibee.ProdutoConsinco();

                    int codigoProduto;
                    int.TryParse(carregamentoPedidoProduto.PedidoProduto.Produto.CodigoProdutoEmbarcador, out codigoProduto);
                    produtoConsinco.codigoProduto = codigoProduto;
                    produtoConsinco.quantidade = carregamentoPedidoProduto.Quantidade;
                    produtoConsinco.embalagem = "UN";
                    produtoConsinco.linhaSeparacao = carregamentoPedidoProduto.PedidoProduto.LinhaSeparacao?.Descricao ?? "";
                    produtoConsinco.quantidadeEmbalagem = carregamentoPedidoProduto.PedidoProduto.QuantidadeCaixa;
                    produtoConsinco.idDemanda = carregamentoPedidoProduto.PedidoProduto.IdDemanda ?? string.Empty;
                    pedidoConsinco.itens.Add(produtoConsinco);
                }
                carregamentoConsinco.pedidos.Add(pedidoConsinco);
            }
            //requestConsinco.body = carregamentoConsinco;
            string jsonRequest = JsonConvert.SerializeObject(carregamentoConsinco, Newtonsoft.Json.Formatting.Indented);
            return jsonRequest;
        }

        #endregion

        #region Métodos Públicos

        public static void IntegrarCargaTransporte(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repositorioCargaCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(unitOfWork);
            string jsonRequisicao = string.Empty;
            string jsonRetorno = string.Empty;

            cargaIntegracao.DataIntegracao = DateTime.Now;
            cargaIntegracao.NumeroTentativas++;

            try
            {
                Repositorio.Embarcador.Configuracoes.Integracao repositorioIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.Integracao integracao = repositorioIntegracao.Buscar();

                if ((integracao == null) || string.IsNullOrWhiteSpace(integracao.APIKeyDigibee) || string.IsNullOrWhiteSpace(integracao.URLIntegracaoDadosCargaDigibee))
                    throw new ServicoException("Não foram configurados os dados de integração com a Digibee");

                if (integracao.IntegracaoDigibeePadraoConsinco)
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Digibee.TokenAutenticacao token = ObterTokenAutenticacao(integracao);

                    if (token.FalhaAutenticacao)
                    {
                        cargaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                        cargaIntegracao.ProblemaIntegracao = token.MensagemErro;

                        AdicionarArquivoTransacaoIntegracaoCargaTransporte(cargaIntegracao, token.MensagemErro, token.JsonRequisicao, token.JsonRetorno, unitOfWork);
                    }
                    else
                    {
                        List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas;
                        bool problemaIntegracao = false;
                        List<string> mensagensRetorno = new List<string>();

                        if (cargaIntegracao.Carga.CargaAgrupada)
                        {
                            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                            cargas = repositorioCarga.BuscarCargasOriginais(cargaIntegracao.Carga.Codigo);
                        }
                        else
                            cargas = new List<Dominio.Entidades.Embarcador.Cargas.Carga>() { cargaIntegracao.Carga };

                        Repositorio.Embarcador.Cargas.CargaLacre repositorioCargaLacre = new Repositorio.Embarcador.Cargas.CargaLacre(unitOfWork);
                        Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                        Repositorio.Embarcador.Cargas.CargaMotorista repositorioCargaMotorista = new Repositorio.Embarcador.Cargas.CargaMotorista(unitOfWork);
                        List<Dominio.Entidades.Embarcador.Cargas.CargaLacre> cargaLacres = repositorioCargaLacre.BuscarPorCarga(cargaIntegracao.Carga.Codigo);
                        List<Dominio.Entidades.Usuario> motoristas = repositorioCargaMotorista.BuscarMotoristasPorCarga(cargaIntegracao.Carga.Codigo);
                        Dominio.Entidades.Embarcador.Cargas.CargaPedido primeiroCargaPedido = repositorioCargaPedido.BuscarPrimeiraPorCarga(cargaIntegracao.Carga.Codigo);

                        if ((cargaIntegracao.Carga.Empresa?.CNPJ?.ObterSomenteNumeros()?.ToLong() ?? 0) == 0)
                            return;

                        foreach (Dominio.Entidades.Embarcador.Cargas.Carga carga in cargas)
                        {
                            HttpClient client = ObterClienteRequisicao(integracao.URLIntegracaoDadosCargaDigibee, integracao.APIKeyDigibeeGeral, token.Token);


                            Dominio.ObjetosDeValor.Embarcador.Integracao.Digibee.DadosCargaConsinco cargaConsinco = new Dominio.ObjetosDeValor.Embarcador.Integracao.Digibee.DadosCargaConsinco()
                            {
                                codigoOrigem = primeiroCargaPedido?.Pedido?.Remetente?.CodigoIntegracao?.ToInt() ?? 0,
                                cpfCnpjMotorista = motoristas.FirstOrDefault()?.CPF?.ObterSomenteNumeros()?.ToLong() ?? 0,
                                cpfCnpjTransportador = cargaIntegracao.Carga.Empresa?.CNPJ?.ObterSomenteNumeros()?.ToLong() ?? 0,
                                numeroLacre = cargaLacres.FirstOrDefault()?.Descricao?.ToInt() ?? 0,
                                placaVeiculo = cargaIntegracao.Carga.Veiculo?.Placa ?? "",
                                protocoloIntegracaoCarga = carga.Protocolo
                            };

                            if (cargaConsinco.codigoOrigem == 0)
                                throw new ServicoException($"Codigo da origem remetente não pode ser zero, carga {carga.Codigo} - Digibee.");

                            if (cargaConsinco.cpfCnpjMotorista > 0)
                                cargaConsinco.digCpfCnpjMotorista = cargaConsinco.cpfCnpjMotorista.ToString().Substring(cargaConsinco.cpfCnpjMotorista.ToString().Length - 2, 2).ToLong();

                            if (cargaConsinco.cpfCnpjTransportador > 0)
                                cargaConsinco.digCpfCnpjTransportador = cargaConsinco.cpfCnpjTransportador.ToString().Substring(cargaConsinco.cpfCnpjTransportador.ToString().Length - 2, 2).ToLong();

                            jsonRequisicao = JsonConvert.SerializeObject(cargaConsinco, Formatting.Indented);
                            StringContent conteudoRequisicao = new StringContent(jsonRequisicao.ToString(), Encoding.UTF8, "application/json");
                            HttpResponseMessage retornoRequisicao = client.PostAsync(integracao.URLIntegracaoDadosCargaDigibee, conteudoRequisicao).Result;
                            jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;
                            Dominio.ObjetosDeValor.Embarcador.Integracao.Digibee.Retorno retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Digibee.Retorno>(jsonRetorno);
                            string mensagemRetorno;

                            if (retorno.code == 200)
                                mensagemRetorno = cargaIntegracao.Carga.CargaAgrupada ? $"Integração da carga {carga.CodigoCargaEmbarcador} realizada com sucesso" : "Integração realizada com sucesso";
                            else
                            {
                                string mensagemErro = retorno?.error ?? "Falha genérica retornada";
                                problemaIntegracao = true;
                                mensagemRetorno = cargaIntegracao.Carga.CargaAgrupada ? $"Falha ao integrar a carga {carga.CodigoCargaEmbarcador}: {mensagemErro}" : mensagemErro;
                            }

                            mensagensRetorno.Add(mensagemRetorno);
                            AdicionarArquivoTransacaoIntegracaoCargaTransporte(cargaIntegracao, mensagemRetorno, jsonRequisicao, jsonRetorno, unitOfWork);
                        }

                        cargaIntegracao.SituacaoIntegracao = problemaIntegracao ? SituacaoIntegracao.ProblemaIntegracao : SituacaoIntegracao.Integrado;
                        cargaIntegracao.ProblemaIntegracao = string.Join(" | ", mensagensRetorno);
                    }
                }
                else
                {
                    cargaIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                    cargaIntegracao.ProblemaIntegracao = "Não existe integração de dados da carga";
                }
            }
            catch (ServicoException excecao)
            {
                cargaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaIntegracao.ProblemaIntegracao = excecao.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                cargaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao integrar os dados da carga";

                AdicionarArquivoTransacaoIntegracaoCargaTransporte(cargaIntegracao, cargaIntegracao.ProblemaIntegracao, jsonRequisicao, jsonRetorno, unitOfWork);
            }

            repositorioCargaCargaIntegracao.Atualizar(cargaIntegracao);
        }

        public static void IntegrarCargaDadosTransporte(Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaDadosTransporteIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao repositorioCargaDadosTransporteIntegracao = new Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao(unitOfWork);
            string jsonRequisicao = string.Empty;
            string jsonRetorno = string.Empty;

            cargaDadosTransporteIntegracao.DataIntegracao = DateTime.Now;
            cargaDadosTransporteIntegracao.NumeroTentativas++;

            try
            {
                Repositorio.Embarcador.Configuracoes.Integracao repositorioIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.Integracao integracao = repositorioIntegracao.Buscar();

                if ((integracao == null) || string.IsNullOrWhiteSpace(integracao.APIKeyDigibee) || string.IsNullOrWhiteSpace(integracao.URLIntegracaoDadosCargaDigibee))
                    throw new ServicoException("Não foram configurados os dados de integração com a Digibee");

                if (integracao.IntegracaoDigibeePadraoConsinco)
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Digibee.TokenAutenticacao token = ObterTokenAutenticacao(integracao);

                    if (token.FalhaAutenticacao)
                    {
                        cargaDadosTransporteIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                        cargaDadosTransporteIntegracao.ProblemaIntegracao = token.MensagemErro;

                        AdicionarArquivoTransacaoIntegracaoCargaDadosTransporte(cargaDadosTransporteIntegracao, token.MensagemErro, token.JsonRequisicao, token.JsonRetorno, unitOfWork);
                    }
                    else
                    {
                        List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas;
                        bool problemaIntegracao = false;
                        List<string> mensagensRetorno = new List<string>();

                        if (cargaDadosTransporteIntegracao.Carga.CargaAgrupada)
                        {
                            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                            cargas = repositorioCarga.BuscarCargasOriginais(cargaDadosTransporteIntegracao.Carga.Codigo);
                        }
                        else
                            cargas = new List<Dominio.Entidades.Embarcador.Cargas.Carga>() { cargaDadosTransporteIntegracao.Carga };

                        Repositorio.Embarcador.Cargas.CargaLacre repositorioCargaLacre = new Repositorio.Embarcador.Cargas.CargaLacre(unitOfWork);
                        Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                        Repositorio.Embarcador.Cargas.CargaMotorista repositorioCargaMotorista = new Repositorio.Embarcador.Cargas.CargaMotorista(unitOfWork);
                        List<Dominio.Entidades.Embarcador.Cargas.CargaLacre> cargaLacres = repositorioCargaLacre.BuscarPorCarga(cargaDadosTransporteIntegracao.Carga.Codigo);
                        List<Dominio.Entidades.Usuario> motoristas = repositorioCargaMotorista.BuscarMotoristasPorCarga(cargaDadosTransporteIntegracao.Carga.Codigo);
                        Dominio.Entidades.Embarcador.Cargas.CargaPedido primeiroCargaPedido = repositorioCargaPedido.BuscarPrimeiraPorCarga(cargaDadosTransporteIntegracao.Carga.Codigo);

                        if ((cargaDadosTransporteIntegracao.Carga.Empresa?.CNPJ?.ObterSomenteNumeros()?.ToLong() ?? 0) == 0)
                            return;

                        foreach (Dominio.Entidades.Embarcador.Cargas.Carga carga in cargas)
                        {
                            HttpClient client = ObterClienteRequisicao(integracao.URLIntegracaoDadosCargaDigibee, integracao.APIKeyDigibeeGeral, token.Token);


                            Dominio.ObjetosDeValor.Embarcador.Integracao.Digibee.DadosCargaConsinco cargaConsinco = new Dominio.ObjetosDeValor.Embarcador.Integracao.Digibee.DadosCargaConsinco()
                            {
                                codigoOrigem = primeiroCargaPedido?.Pedido?.Remetente?.CodigoIntegracao?.ToInt() ?? 0,
                                cpfCnpjMotorista = motoristas.FirstOrDefault()?.CPF?.ObterSomenteNumeros()?.ToLong() ?? 0,
                                cpfCnpjTransportador = cargaDadosTransporteIntegracao.Carga.Empresa?.CNPJ?.ObterSomenteNumeros()?.ToLong() ?? 0,
                                numeroLacre = cargaLacres.FirstOrDefault()?.Descricao?.ToInt() ?? 0,
                                placaVeiculo = cargaDadosTransporteIntegracao.Carga.Veiculo?.Placa ?? "",
                                protocoloIntegracaoCarga = carga.Protocolo
                            };

                            if (cargaConsinco.codigoOrigem == 0)
                                throw new ServicoException($"Codigo da origem remetente não pode ser zero, carga {carga.Codigo} - Digibee.");

                            if (cargaConsinco.cpfCnpjMotorista > 0)
                                cargaConsinco.digCpfCnpjMotorista = cargaConsinco.cpfCnpjMotorista.ToString().Substring(cargaConsinco.cpfCnpjMotorista.ToString().Length - 2, 2).ToLong();

                            if (cargaConsinco.cpfCnpjTransportador > 0)
                                cargaConsinco.digCpfCnpjTransportador = cargaConsinco.cpfCnpjTransportador.ToString().Substring(cargaConsinco.cpfCnpjTransportador.ToString().Length - 2, 2).ToLong();

                            jsonRequisicao = JsonConvert.SerializeObject(cargaConsinco, Formatting.Indented);
                            StringContent conteudoRequisicao = new StringContent(jsonRequisicao.ToString(), Encoding.UTF8, "application/json");
                            HttpResponseMessage retornoRequisicao = client.PostAsync(integracao.URLIntegracaoDadosCargaDigibee, conteudoRequisicao).Result;
                            jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;
                            Dominio.ObjetosDeValor.Embarcador.Integracao.Digibee.Retorno retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Digibee.Retorno>(jsonRetorno);
                            string mensagemRetorno;

                            if (retorno.code == 200)
                                mensagemRetorno = cargaDadosTransporteIntegracao.Carga.CargaAgrupada ? $"Integração da carga {carga.CodigoCargaEmbarcador} realizada com sucesso" : "Integração realizada com sucesso";
                            else
                            {
                                string mensagemErro = retorno?.error ?? "Falha genérica retornada";
                                problemaIntegracao = true;
                                mensagemRetorno = cargaDadosTransporteIntegracao.Carga.CargaAgrupada ? $"Falha ao integrar a carga {carga.CodigoCargaEmbarcador}: {mensagemErro}" : mensagemErro;
                            }

                            mensagensRetorno.Add(mensagemRetorno);
                            AdicionarArquivoTransacaoIntegracaoCargaDadosTransporte(cargaDadosTransporteIntegracao, mensagemRetorno, jsonRequisicao, jsonRetorno, unitOfWork);
                        }

                        cargaDadosTransporteIntegracao.SituacaoIntegracao = problemaIntegracao ? SituacaoIntegracao.ProblemaIntegracao : SituacaoIntegracao.Integrado;
                        cargaDadosTransporteIntegracao.ProblemaIntegracao = string.Join(" | ", mensagensRetorno);
                    }
                }
                else
                {
                    cargaDadosTransporteIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                    cargaDadosTransporteIntegracao.ProblemaIntegracao = "Não existe integração de dados da carga";
                }
            }
            catch (ServicoException excecao)
            {
                cargaDadosTransporteIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaDadosTransporteIntegracao.ProblemaIntegracao = excecao.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                cargaDadosTransporteIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaDadosTransporteIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao integrar os dados da carga";

                AdicionarArquivoTransacaoIntegracaoCargaDadosTransporte(cargaDadosTransporteIntegracao, cargaDadosTransporteIntegracao.ProblemaIntegracao, jsonRequisicao, jsonRetorno, unitOfWork);
            }

            repositorioCargaDadosTransporteIntegracao.Atualizar(cargaDadosTransporteIntegracao);
        }

        public static void IntegrarCarregamento(Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoIntegracao carregamentoIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoIntegracao repCarregamentoIntegracao = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoIntegracao(unitOfWork);
            Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido repCarregamentoPedido = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido(unitOfWork);
            Repositorio.Embarcador.Configuracoes.Integracao repIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.Integracao integracao = repIntegracao.Buscar();

            carregamentoIntegracao.DataIntegracao = DateTime.Now;
            carregamentoIntegracao.NumeroTentativas++;

            if (integracao == null || string.IsNullOrWhiteSpace(integracao.APIKeyDigibee) || string.IsNullOrWhiteSpace(integracao.URLIntegracaoDigibee))
            {
                carregamentoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                carregamentoIntegracao.ProblemaIntegracao = "Não está configurada a integração com a dibibee";

            }
            else
            {
                if (integracao.IntegracaoDigibeePadraoConsinco)
                    IntegracaoCarregamentoConsinco(carregamentoIntegracao, integracao, unitOfWork);
                else
                    IntegracaoCarregamento(carregamentoIntegracao, integracao, unitOfWork);
            }

            repCarregamentoIntegracao.Atualizar(carregamentoIntegracao);
        }

        public static void IntegrarPedidoCancelamentoReserva(Dominio.Entidades.Embarcador.Cargas.MontagemCarga.PedidoCancelamentoReservaIntegracao pedidoCancelamentoReservaIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.MontagemCarga.PedidoCancelamentoReservaIntegracao repCarregamentoIntegracao = new Repositorio.Embarcador.Cargas.MontagemCarga.PedidoCancelamentoReservaIntegracao(unitOfWork);
            Repositorio.Embarcador.Configuracoes.Integracao repIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.Integracao integracao = repIntegracao.Buscar();

            pedidoCancelamentoReservaIntegracao.DataIntegracao = DateTime.Now;
            pedidoCancelamentoReservaIntegracao.NumeroTentativas++;

            if (integracao == null || string.IsNullOrWhiteSpace(integracao.APIKeyDigibee) || string.IsNullOrWhiteSpace(integracao.URLIntegracaoCancelamentoDigibee))
            {
                pedidoCancelamentoReservaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                pedidoCancelamentoReservaIntegracao.ProblemaIntegracao = "Não está configurada a integração com a dibibee";
            }
            else
            {
                if (integracao.IntegracaoDigibeePadraoConsinco)
                    IntegracaoPedidoCancelamentoReservaConsinco(pedidoCancelamentoReservaIntegracao, integracao, unitOfWork);
                else
                    IntegracaoPedidoCancelamentoReserva(pedidoCancelamentoReservaIntegracao, integracao, unitOfWork);
            }

            repCarregamentoIntegracao.Atualizar(pedidoCancelamentoReservaIntegracao);
        }

        public static void IntegrarDocumentosEscrituracao(Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoIntegracao loteEscrituracaoIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Escrituracao.LoteEscrituracaoIntegracao repLoteEscrituracaoIntegracao = new Repositorio.Embarcador.Escrituracao.LoteEscrituracaoIntegracao(unitOfWork);
            Repositorio.Embarcador.Escrituracao.DocumentoEscrituracao repDocumentoEscrituracao = new Repositorio.Embarcador.Escrituracao.DocumentoEscrituracao(unitOfWork);
            Servicos.CTe svCTe = new Servicos.CTe(unitOfWork);

            string jsonRequisicao = string.Empty;
            string jsonRetorno = string.Empty;

            loteEscrituracaoIntegracao.DataIntegracao = DateTime.Now;
            loteEscrituracaoIntegracao.NumeroTentativas++;

            try
            {
                Repositorio.Embarcador.Configuracoes.Integracao repositorioIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.Integracao integracao = repositorioIntegracao.Buscar();

                if ((integracao == null) || string.IsNullOrWhiteSpace(integracao.APIKeyDigibee) || string.IsNullOrWhiteSpace(integracao.URLIntegracaoEscrituracaoCTeDigibee))
                    throw new ServicoException("Não foram configurados os dados de integração com a Digibee");

                               
                bool problemaIntegracao = false;
                List<string> mensagensRetorno = new List<string>();

                HttpClient client = ObterClienteRequisicao(integracao.URLIntegracaoEscrituracaoCTeDigibee, integracao.APIKeyDigibee);
                Dominio.ObjetosDeValor.Embarcador.Integracao.Digibee.EscrituracaoCTe escrituracaoCTe = new Dominio.ObjetosDeValor.Embarcador.Integracao.Digibee.EscrituracaoCTe()
                {
                    xmlCtes = new List<string>()
                };

                Repositorio.XMLCTe repXMLCTe = new Repositorio.XMLCTe(unitOfWork);

                List<int> codigosCTes = repDocumentoEscrituracao.BuscarCodigosCTesPorLote(loteEscrituracaoIntegracao.LoteEscrituracao.Codigo);

                List<Dominio.Entidades.XMLCTe> xMLCTes = repXMLCTe.BuscarPorCTe(codigosCTes, Dominio.Enumeradores.TipoXMLCTe.Autorizacao);

                foreach (Dominio.Entidades.XMLCTe xmlCTe in xMLCTes)
                {
                    string xml = "";
                    if (xmlCTe.XMLArmazenadoEmArquivo)
                        xml = svCTe.CriarERetornarCaminhoXMLCTe(xmlCTe.CTe, "A", unitOfWork);
                    else
                        xml = xmlCTe.XML;

                    if (!string.IsNullOrWhiteSpace(xml))
                    {
                        byte[] xmlArray = Encoding.ASCII.GetBytes(xml);
                        escrituracaoCTe.xmlCtes.Add(Convert.ToBase64String(xmlArray));
                    }
                }


                jsonRequisicao = JsonConvert.SerializeObject(escrituracaoCTe, Formatting.Indented);
                StringContent conteudoRequisicao = new StringContent(jsonRequisicao.ToString(), Encoding.UTF8, "application/json");
                HttpResponseMessage retornoRequisicao = client.PostAsync(integracao.URLIntegracaoEscrituracaoCTeDigibee, conteudoRequisicao).Result;
                jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;
                Dominio.ObjetosDeValor.Embarcador.Integracao.Digibee.RetornoFiscal retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Digibee.RetornoFiscal>(jsonRetorno);
                string mensagemRetorno;

                if (retorno?.status == 200)
                {
                    mensagemRetorno = "Integração realizada com sucesso";
                    if (retorno.failed > 0)
                    {
                        problemaIntegracao = true;
                        mensagemRetorno += ". Porém do total enviado (" + retorno.total.ToString() + " registro(s)) " + retorno.failed.ToString() + " retornaram falha da Digibee.";
                    }
                }
                else
                {
                    string mensagemErro = retorno?.message ?? "A digibee não conseguiu processar o lote.";
                    problemaIntegracao = true;
                    mensagemRetorno = $"Falha ao integrar o lote {loteEscrituracaoIntegracao.LoteEscrituracao.Numero.ToString()}: {mensagemErro}";
                }

                mensagensRetorno.Add(mensagemRetorno);
                AdicionarArquivoTransacaoIntegracaoEscrituracao(loteEscrituracaoIntegracao, mensagemRetorno, jsonRequisicao, jsonRetorno, unitOfWork);


                loteEscrituracaoIntegracao.SituacaoIntegracao = problemaIntegracao ? SituacaoIntegracao.ProblemaIntegracao : SituacaoIntegracao.Integrado;
                loteEscrituracaoIntegracao.ProblemaIntegracao = string.Join(" | ", mensagensRetorno);
                // }
            }
            catch (ServicoException excecao)
            {
                loteEscrituracaoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                loteEscrituracaoIntegracao.ProblemaIntegracao = excecao.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                loteEscrituracaoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                loteEscrituracaoIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao integrar os dados da carga";

                AdicionarArquivoTransacaoIntegracaoEscrituracao(loteEscrituracaoIntegracao, loteEscrituracaoIntegracao.ProblemaIntegracao, jsonRequisicao, jsonRetorno, unitOfWork);
            }

            repLoteEscrituracaoIntegracao.Atualizar(loteEscrituracaoIntegracao);
        }
    
        public static void IntegrarDocumentosContabilizacao(Dominio.Entidades.Embarcador.Escrituracao.PagamentoIntegracao pagamentoIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Escrituracao.PagamentoIntegracao repPagamentoIntegracao = new Repositorio.Embarcador.Escrituracao.PagamentoIntegracao(unitOfWork);
            Repositorio.Embarcador.Configuracoes.Integracao repositorioIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
            Repositorio.Embarcador.Financeiro.DocumentoFaturamento repDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(unitOfWork);
            Repositorio.CTeContaContabilContabilizacao repCTeContaContabilContabilizacao = new Repositorio.CTeContaContabilContabilizacao(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro repConfiguracaoFinanceiro = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiro configuracaoFinanceiro = repConfiguracaoFinanceiro.BuscarConfiguracaoPadrao();

            string jsonRequisicao = string.Empty;
            string jsonRetorno = string.Empty;

            pagamentoIntegracao.DataIntegracao = DateTime.Now;
            pagamentoIntegracao.NumeroTentativas++;

            try
            {
                Dominio.Entidades.Embarcador.Configuracoes.Integracao integracao = repositorioIntegracao.Buscar();

                if ((integracao == null) || string.IsNullOrWhiteSpace(integracao.APIKeyDigibee) || string.IsNullOrWhiteSpace(integracao.URLIntegracaoDadosContabeisCTeDigibee))
                    throw new ServicoException("Não foram configurados os dados de integração com a Digibee");


                bool problemaIntegracao = false;
                List<string> mensagensRetorno = new List<string>();

                HttpClient client = ObterClienteRequisicao(integracao.URLIntegracaoDadosContabeisCTeDigibee, integracao.APIKeyDigibee);

                Dominio.ObjetosDeValor.Embarcador.Integracao.Digibee.PagamentoCTe pagamentoCTe = new Dominio.ObjetosDeValor.Embarcador.Integracao.Digibee.PagamentoCTe();
                pagamentoCTe.ctes = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Digibee.DadosContabeis>();

                List<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento> documentoFaturamentos = repDocumentoFaturamento.BuscarPorPagamento(pagamentoIntegracao.Pagamento.Codigo, false);

                Dominio.Entidades.Empresa empresaPai = repEmpresa.BuscarEmpresaPai();
                decimal aliquotaCOFINS = empresaPai?.Configuracao?.AliquotaCOFINS ?? 0;
                decimal aliquotaPIS = empresaPai?.Configuracao?.AliquotaPIS ?? 0;

                List<int> codigosCTes = (from obj in documentoFaturamentos where obj.CTe != null select obj.CTe.Codigo).Distinct().ToList();
                List<Dominio.Entidades.CTeContaContabilContabilizacao> pagamentoContaContabilContabilizacaos = repCTeContaContabilContabilizacao.BuscarPorCTes(codigosCTes);

                foreach (Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento documentoFaturamento in documentoFaturamentos)
                {
                    Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = documentoFaturamento.CTe;
                    if (cte != null)
                    {
                        System.Globalization.CultureInfo cultureInfo = new CultureInfo("pt-BR");

                        Dominio.ObjetosDeValor.Embarcador.Integracao.Digibee.DadosContabeis dadosContabeis = new Dominio.ObjetosDeValor.Embarcador.Integracao.Digibee.DadosContabeis();
                        List<Dominio.Entidades.CTeContaContabilContabilizacao> cteContaContabilContabilizacaos = (from obj in pagamentoContaContabilContabilizacaos where obj.Cte.Codigo == cte.Codigo select obj).ToList();
                        dadosContabeis.empresa = "0136"; //, -Valor fixo, pode ser definido na configuração das integrações
                        dadosContabeis.codLancamento = "KF"; //-Valor fixo, pode ser definido na configuração das integrações
                        dadosContabeis.dtEmissao = cte.DataEmissao.Value.ToString("ddMMyyyy"); //"22092021"; //, -Data de emissão do CT - e
                        if (integracao.AjustarDataParaCorresponderQuinzenaDigibee && pagamentoIntegracao.Pagamento.DataFinal.HasValue)
                        {
                            DateTime dataSegundaQuinzena = new DateTime(pagamentoIntegracao.Pagamento.DataFinal.Value.Year, pagamentoIntegracao.Pagamento.DataFinal.Value.Month, 16);
                            if (pagamentoIntegracao.Pagamento.DataFinal.Value.IsBewteenTwoDates(dataSegundaQuinzena, pagamentoIntegracao.Pagamento.DataFinal.Value.LastDayOfMonth()))
                                dadosContabeis.dtContabilizacao = pagamentoIntegracao.Pagamento.DataFinal.Value.LastDayOfMonth().ToString("ddMMyyyy");
                            else
                                dadosContabeis.dtContabilizacao = pagamentoIntegracao.Pagamento.DataCriacao.ToString("ddMMyyyy");
                        }
                        else
                            dadosContabeis.dtContabilizacao = pagamentoIntegracao.Pagamento.DataCriacao.ToString("ddMMyyyy"); //"19102021"; //, -Dia do envio(data fechamento)
                        dadosContabeis.moeda = "BRL"; //, -Valor fixo, pode ser definido na configuração das integrações
                        dadosContabeis.numCte = cte.Numero.ToString() + "-" + cte.Serie.Numero.ToString(); //"000126-1"; // -Informar a série do documento após "-"
                        dadosContabeis.codFornecedor = cte.Empresa.CodigoIntegracao; //"G11611"; // -Código integração do transportador;
                        dadosContabeis.valorCte = cte.ValorAReceber.ToString("n2", cultureInfo); //"658,72"; // -Valor bruto
                        dadosContabeis.dtPagamento = cte.Titulo?.DataVencimento?.ToString("ddMMyyyy") ?? cte.DataEmissao.Value.ToString("ddMMyyyy"); //.DataVencimento.ToString("") "01112021"; // -Data calculada conforme especificado na tarefa #35393
                        dadosContabeis.indConfirmig = cte.Empresa.AntecipacaoPagamento ? "L" : ""; // -Criar flag "Confirme" no cadastro do transportador se marcado L, se não vazio

                        Dominio.Entidades.CTeContaContabilContabilizacao contaDespesa = (from obj in cteContaContabilContabilizacaos where obj.TipoContaContabil == TipoContaContabil.TotalReceber select obj).FirstOrDefault();

                        dadosContabeis.contaDespesa = contaDespesa?.PlanoConta.PlanoContabilidade ?? "";
                        dadosContabeis.valorDespesa = cte.ValorAReceber.ToString("n2", cultureInfo);
                        dadosContabeis.centroCustoDespesa = cte.CentroResultado?.PlanoContabilidade ?? "";

                        Dominio.Entidades.CTeContaContabilContabilizacao contaDebitoPis = (from obj in cteContaContabilContabilizacaos where obj.TipoContaContabil == TipoContaContabil.PIS && obj.TipoContabilizacao == TipoContabilizacao.Debito select obj).FirstOrDefault();
                        dadosContabeis.contaDebitoPis = contaDebitoPis?.PlanoConta.PlanoContabilidade ?? "";

                        Dominio.Entidades.CTeContaContabilContabilizacao contaCreditoPis = (from obj in cteContaContabilContabilizacaos where obj.TipoContaContabil == TipoContaContabil.PIS && obj.TipoContabilizacao == TipoContabilizacao.Credito select obj).FirstOrDefault();
                        dadosContabeis.contaCreditoPis = contaCreditoPis?.PlanoConta.PlanoContabilidade ?? "";


                        dadosContabeis.centroCustoPis = cte.CentroResultadoPIS?.PlanoContabilidade ?? "";

                        decimal basePisCofins = cte.ValorPrestacaoServico;
                        if (configuracaoFinanceiro.NaoIncluirICMSBaseCalculoPisCofins && cte.IncluirICMSNoFrete == Dominio.Enumeradores.OpcaoSimNao.Sim)
                            basePisCofins = cte.ValorPrestacaoServico - cte.ValorICMS;


                        decimal valorPIS = Math.Round(basePisCofins * (aliquotaPIS / 100), 2, MidpointRounding.AwayFromZero);
                        dadosContabeis.valorPis = valorPIS.ToString("n2", cultureInfo);

                        Dominio.Entidades.CTeContaContabilContabilizacao contaDebitoCofins = (from obj in cteContaContabilContabilizacaos where obj.TipoContaContabil == TipoContaContabil.COFINS && obj.TipoContabilizacao == TipoContabilizacao.Debito select obj).FirstOrDefault();
                        dadosContabeis.contaDebitoCofins = contaDebitoCofins?.PlanoConta.PlanoContabilidade ?? "";

                        Dominio.Entidades.CTeContaContabilContabilizacao contaCreditoCofins = (from obj in cteContaContabilContabilizacaos where obj.TipoContaContabil == TipoContaContabil.COFINS && obj.TipoContabilizacao == TipoContabilizacao.Credito select obj).FirstOrDefault();
                        dadosContabeis.contaCreditoCofins = contaCreditoCofins?.PlanoConta.PlanoContabilidade ?? "";

                        dadosContabeis.centroCustoCofins = cte.CentroResultadoCOFINS?.PlanoContabilidade ?? "";
                        decimal valorCOFINS = Math.Round(basePisCofins * (aliquotaCOFINS / 100), 2, MidpointRounding.AwayFromZero);
                        dadosContabeis.valorCofins = valorCOFINS.ToString("n2", cultureInfo);

                        Dominio.Entidades.CTeContaContabilContabilizacao contaDebitoICMS = (from obj in cteContaContabilContabilizacaos where obj.TipoContaContabil == TipoContaContabil.ICMS && obj.TipoContabilizacao == TipoContabilizacao.Debito select obj).FirstOrDefault();
                        dadosContabeis.contaDebitoIcms = contaDebitoICMS?.PlanoConta.PlanoContabilidade ?? "";

                        Dominio.Entidades.CTeContaContabilContabilizacao contaCreditoICMS = (from obj in cteContaContabilContabilizacaos where obj.TipoContaContabil == TipoContaContabil.ICMS && obj.TipoContabilizacao == TipoContabilizacao.Credito select obj).FirstOrDefault();
                        dadosContabeis.contaCreditoIcms = contaCreditoICMS?.PlanoConta.PlanoContabilidade ?? "";

                        dadosContabeis.centroCustoIcms = cte.CentroResultadoICMS?.PlanoContabilidade ?? "";
                        dadosContabeis.valorIcms = cte.ValorICMS.ToString("n2", cultureInfo);

                        dadosContabeis.textoSap = documentoFaturamento.ModeloDocumentoFiscal?.Abreviacao == "NC" ? "ND." + pagamentoIntegracao.Pagamento.Numero.ToString() + "-" + documentoFaturamento.Empresa?.RazaoSocial : "CTE." + cte.Numero.ToString() + "-" + cte.Empresa.RazaoSocial;

                        dadosContabeis.codArmazem = cte.TomadorPagador.Cliente.CodigoIntegracao;
                        pagamentoCTe.ctes.Add(dadosContabeis);
                    }
                }

                jsonRequisicao = JsonConvert.SerializeObject(pagamentoCTe, Formatting.Indented);
                StringContent conteudoRequisicao = new StringContent(jsonRequisicao.ToString(), Encoding.UTF8, "application/json");
                HttpResponseMessage retornoRequisicao = client.PostAsync(integracao.URLIntegracaoDadosContabeisCTeDigibee, conteudoRequisicao).Result;
                jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;
                Dominio.ObjetosDeValor.Embarcador.Integracao.Digibee.RetornoFiscal retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Digibee.RetornoFiscal>(jsonRetorno);
                string mensagemRetorno;

                if (retorno?.status == 200)
                {
                    mensagemRetorno = "Integração realizada com sucesso";
                    if (retorno.failed > 0)
                    {
                        problemaIntegracao = true;
                        mensagemRetorno += ". Porém do total enviado (" + retorno.total.ToString() + " registro(s)) " + retorno.failed.ToString() + " retornaram falha da Digibee.";
                    }
                }
                else
                {
                    string mensagemErro = retorno?.message ?? "A digibee não conseguiu processar o lote.";
                    problemaIntegracao = true;
                    mensagemRetorno = $"Falha ao integrar o lote {pagamentoIntegracao.Pagamento.Numero.ToString()}: {mensagemErro}";
                }

                mensagensRetorno.Add(mensagemRetorno);
                AdicionarArquivoTransacaoIntegracaoPagamento(pagamentoIntegracao, mensagemRetorno, jsonRequisicao, jsonRetorno, unitOfWork);


                pagamentoIntegracao.SituacaoIntegracao = problemaIntegracao ? SituacaoIntegracao.ProblemaIntegracao : SituacaoIntegracao.Integrado;
                pagamentoIntegracao.ProblemaIntegracao = string.Join(" | ", mensagensRetorno);
            }
            catch (ServicoException excecao)
            {
                pagamentoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                pagamentoIntegracao.ProblemaIntegracao = excecao.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                pagamentoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                pagamentoIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao integrar os dados da escrituração";

                AdicionarArquivoTransacaoIntegracaoPagamento(pagamentoIntegracao, pagamentoIntegracao.ProblemaIntegracao, jsonRequisicao, jsonRetorno, unitOfWork);
            }

            repPagamentoIntegracao.Atualizar(pagamentoIntegracao);
        }

        #endregion
    }
}
