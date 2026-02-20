using Dominio.Entidades.Embarcador.Pedidos;
using Infrastructure.Services.HttpClientFactory;
using Newtonsoft.Json;
using Repositorio;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Servicos.Embarcador.Integracao.AngelLira
{
    public class IntegracaoPedidosAngelLira
    {

        #region Métodos Públicos
        public static void EnviarPedido(ref Dominio.Entidades.Embarcador.Pedidos.PedidoIntegracao pedidoIntegracao, Repositorio.UnitOfWork unitOfWork, string urlWebService, string usuario, string senha)
        {
            Repositorio.Embarcador.Pedidos.PedidoIntegracaoArquivo repPedidoIntegracaoArquivo = new Repositorio.Embarcador.Pedidos.PedidoIntegracaoArquivo(unitOfWork);
            pedidoIntegracao.Tentativas += 1;
            pedidoIntegracao.DataEnvio = DateTime.Now;

            bool situacaoIntegracao = false;
            string mensagemErro = string.Empty;
            string jsonRequest = string.Empty; string jsonResponse = string.Empty;

            try
            {
                string token = ObterToken(urlWebService, usuario, senha);
                if (string.IsNullOrWhiteSpace(token))
                    mensagemErro = "Falha ao obter o token de acesso a integração de pedidos";
                else
                {
                    urlWebService += "/loadplanningintegration/v1/order";
                    HttpClient client = ObterClienteRequisicao(urlWebService, token);
                    jsonRequest = RetornarObjetoPedido(pedidoIntegracao.Pedido.Codigo, unitOfWork);

                    // Request
                    var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
                    var result = client.PostAsync(urlWebService, content).Result;

                    // Response
                    jsonResponse = result.Content.ReadAsStringAsync().Result;

                    if (result.IsSuccessStatusCode)
                    {
                        string retorno = result.Content.ReadAsStringAsync().Result;
                        if (!string.IsNullOrWhiteSpace(retorno))
                        {
                            Dominio.ObjetosDeValor.Embarcador.Integracao.AngelLira.Pedidos.DataRetorno retornoJSON = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.AngelLira.Pedidos.DataRetorno>(retorno);
                            if (retornoJSON == null)
                                mensagemErro = "Sem retorno na integração";
                            else
                            {
                                if (retornoJSON.data.error != null && retornoJSON.data.error.Count > 0)
                                {
                                    mensagemErro = "Problemas na Integração:" + retornoJSON.data.error[0].message;
                                    situacaoIntegracao = false;
                                }
                                else if (retornoJSON.data.success != null && retornoJSON.data.success.Count > 0)
                                {
                                    mensagemErro = "Integrado com sucesso.";
                                    situacaoIntegracao = true;

                                    pedidoIntegracao.CodigoIntegracaoIntegradora = retornoJSON.data.success[0].id;
                                    pedidoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgRetorno;
                                }
                            }
                        }

                    }
                    else
                        mensagemErro = "Retorno integração: " + result.StatusCode.ToString();

                    Dominio.Entidades.Embarcador.Pedidos.PedidoIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Pedidos.PedidoIntegracaoArquivo();
                    arquivoIntegracao.Data = pedidoIntegracao.DataEnvio.Value;
                    arquivoIntegracao.Mensagem = mensagemErro;
                    arquivoIntegracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;

                    Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao arquivoRequisicao = ArquivoIntegracao.SalvarArquivoIntegracao(jsonRequest, "json", unitOfWork);
                    Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao arquivoResposta = ArquivoIntegracao.SalvarArquivoIntegracao(jsonResponse, "json", unitOfWork);

                    arquivoIntegracao.ArquivoRequisicao = arquivoRequisicao;
                    arquivoIntegracao.ArquivoResposta = arquivoResposta;

                    repPedidoIntegracaoArquivo.Inserir(arquivoIntegracao);

                    pedidoIntegracao.ArquivosTransacao.Add(arquivoIntegracao);

                }
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao, "IntegracaoPedidosAngelLira");

                mensagemErro = "Ocorreu uma falha ao comunicar com o Serviço da AngelLira.";

                Dominio.Entidades.Embarcador.Pedidos.PedidoIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Pedidos.PedidoIntegracaoArquivo();
                arquivoIntegracao.Data = pedidoIntegracao.DataEnvio.Value;
                arquivoIntegracao.Mensagem = excecao.Message.Length > 400 ? excecao.Message.Substring(0, 400) : excecao.Message;
                arquivoIntegracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;
                arquivoIntegracao.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonRequest, "json", unitOfWork);
                arquivoIntegracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonResponse, "json", unitOfWork);

                repPedidoIntegracaoArquivo.Inserir(arquivoIntegracao);
                pedidoIntegracao.ArquivosTransacao.Add(arquivoIntegracao);

                pedidoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                pedidoIntegracao.ProblemaIntegracao = mensagemErro;
                return;
            }

            if (!situacaoIntegracao)
            {
                pedidoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                pedidoIntegracao.ProblemaIntegracao = mensagemErro;
            }
            else
            {
                pedidoIntegracao.ProblemaIntegracao = string.Empty;
                pedidoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgRetorno;
            }
        }

        public static void BuscarPedidoAguardando(ref Dominio.Entidades.Embarcador.Pedidos.PedidoIntegracao pedidoIntegracao, Repositorio.UnitOfWork unitOfWork, string urlWebService, string usuario, string senha)
        {
            Repositorio.Embarcador.Pedidos.PedidoIntegracaoArquivo repPedidoIntegracaoArquivo = new Repositorio.Embarcador.Pedidos.PedidoIntegracaoArquivo(unitOfWork);
            pedidoIntegracao.Tentativas += 1;
            pedidoIntegracao.DataEnvio = DateTime.Now;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            bool situacaoIntegracao = false;
            string mensagemErro = string.Empty;
            string jsonRequest = string.Empty; string jsonResponse = string.Empty;

            try
            {
                string token = ObterToken(urlWebService, usuario, senha);
                if (string.IsNullOrWhiteSpace(token))
                    mensagemErro = "Falha ao obter o token de acesso a integração de pedidos";
                else
                {
                    if (VerificarStatusPedidoAngelira(pedidoIntegracao, urlWebService, token, ref mensagemErro, ref jsonRequest, ref jsonResponse))
                    {
                        // consultar dados pedido;
                        BuscarDadosPedidoEmEfetivacao(pedidoIntegracao, urlWebService, token, unitOfWork, ref mensagemErro, ref jsonRequest, ref jsonResponse);

                        if (string.IsNullOrWhiteSpace(mensagemErro))
                        {
                            pedidoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                            pedidoIntegracao.ProblemaIntegracao = string.Empty;
                            situacaoIntegracao = true;
                        }
                    }
                    else
                    {
                        if (string.IsNullOrWhiteSpace(mensagemErro)) //status ainda nao esta como aguardando efetivação
                        {
                            pedidoIntegracao.ProblemaIntegracao = string.Empty;
                            pedidoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgRetorno;
                            return;
                        }
                    }

                    Dominio.Entidades.Embarcador.Pedidos.PedidoIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Pedidos.PedidoIntegracaoArquivo();
                    arquivoIntegracao.Data = pedidoIntegracao.DataEnvio.Value;
                    arquivoIntegracao.Mensagem = mensagemErro;
                    arquivoIntegracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;

                    arquivoIntegracao.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonRequest, "json", unitOfWork);
                    arquivoIntegracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonResponse, "json", unitOfWork);

                    repPedidoIntegracaoArquivo.Inserir(arquivoIntegracao);

                    pedidoIntegracao.ArquivosTransacao.Add(arquivoIntegracao);

                }
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao, "IntegracaoPedidosAngelLira");
                mensagemErro = "Ocorreu uma falha ao comunicar com o Serviço da AngelLira.";

                Dominio.Entidades.Embarcador.Pedidos.PedidoIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Pedidos.PedidoIntegracaoArquivo();
                arquivoIntegracao.Data = pedidoIntegracao.DataEnvio.Value;
                arquivoIntegracao.Mensagem = excecao.Message.Length > 400 ? excecao.Message.Substring(0, 400) : excecao.Message;
                arquivoIntegracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;
                arquivoIntegracao.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonRequest, "json", unitOfWork);
                arquivoIntegracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonResponse, "json", unitOfWork);

                repPedidoIntegracaoArquivo.Inserir(arquivoIntegracao);

                pedidoIntegracao.ArquivosTransacao.Add(arquivoIntegracao);

                pedidoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                pedidoIntegracao.ProblemaIntegracao = mensagemErro;
                return;
            }

            if (!situacaoIntegracao)
            {
                pedidoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                pedidoIntegracao.ProblemaIntegracao = mensagemErro;
            }
            else
            {
                pedidoIntegracao.ProblemaIntegracao = string.Empty;
                pedidoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
            }
        }

        public static void CancelarPedidoAguardandoIntegracao(ref Dominio.Entidades.Embarcador.Pedidos.PedidoIntegracao pedidoIntegracao, Repositorio.UnitOfWork unitOfWork, string urlWebService, string usuario, string senha)
        {
            Repositorio.Embarcador.Pedidos.PedidoIntegracaoArquivo repPedidoIntegracaoArquivo = new Repositorio.Embarcador.Pedidos.PedidoIntegracaoArquivo(unitOfWork);
            pedidoIntegracao.Tentativas += 1;
            pedidoIntegracao.DataEnvio = DateTime.Now;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            bool situacaoIntegracao = false;
            string mensagemErro = string.Empty;
            string jsonRequest = string.Empty; string jsonResponse = string.Empty;

            try
            {
                string token = ObterToken(urlWebService, usuario, senha);
                if (string.IsNullOrWhiteSpace(token))
                    mensagemErro = "Falha ao obter o token de acesso a integração de pedidos";
                else
                {

                    // cancelar pedido angelira;
                    CancelarPedidoIntegracao(pedidoIntegracao, urlWebService, token, unitOfWork, ref mensagemErro, ref jsonRequest, ref jsonResponse);

                    if (string.IsNullOrWhiteSpace(mensagemErro))
                    {
                        pedidoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                        pedidoIntegracao.ProblemaIntegracao = "Integração de cancelamento efetuada com sucesso";
                        situacaoIntegracao = true;
                    }

                    Dominio.Entidades.Embarcador.Pedidos.PedidoIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Pedidos.PedidoIntegracaoArquivo();
                    arquivoIntegracao.Data = pedidoIntegracao.DataEnvio.Value;
                    arquivoIntegracao.Mensagem = mensagemErro;
                    arquivoIntegracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;

                    arquivoIntegracao.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonRequest, "json", unitOfWork);
                    arquivoIntegracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonResponse, "json", unitOfWork);

                    repPedidoIntegracaoArquivo.Inserir(arquivoIntegracao);

                    pedidoIntegracao.ArquivosTransacao.Add(arquivoIntegracao);

                }
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao, "IntegracaoPedidosAngelLira");
                mensagemErro = "Ocorreu uma falha ao comunicar com o Serviço da AngelLira.";

                Dominio.Entidades.Embarcador.Pedidos.PedidoIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Pedidos.PedidoIntegracaoArquivo();
                arquivoIntegracao.Data = pedidoIntegracao.DataEnvio.Value;
                arquivoIntegracao.Mensagem = excecao.Message.Length > 400 ? excecao.Message.Substring(0, 400) : excecao.Message;
                arquivoIntegracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;
                arquivoIntegracao.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonRequest, "json", unitOfWork);
                arquivoIntegracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonResponse, "json", unitOfWork);

                repPedidoIntegracaoArquivo.Inserir(arquivoIntegracao);

                pedidoIntegracao.ArquivosTransacao.Add(arquivoIntegracao);

                pedidoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                pedidoIntegracao.ProblemaIntegracao = mensagemErro;
                return;
            }

            if (!situacaoIntegracao)
            {
                pedidoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                pedidoIntegracao.ProblemaIntegracao = mensagemErro;
            }
            else
            {
                pedidoIntegracao.ProblemaIntegracao = string.Empty;
                pedidoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
            }
        }


        #endregion

        #region Métodos Privados

        private static void BuscarDadosPedidoEmEfetivacao(PedidoIntegracao pedidoIntegracao, string urlWebService, string token, Repositorio.UnitOfWork unitOfWork, ref string mensagemErro, ref string jsonRequest, ref string jsonResponse)
        {
            jsonRequest = string.Empty;
            jsonResponse = string.Empty;
            string url = urlWebService + "/loadplanningintegration/V1/order/resource";

            Dominio.ObjetosDeValor.Embarcador.Integracao.AngelLira.Pedidos.OwnIds objetoSolicitacao = new Dominio.ObjetosDeValor.Embarcador.Integracao.AngelLira.Pedidos.OwnIds();
            List<string> listaPedidos = new List<string>();
            listaPedidos.Add(pedidoIntegracao.Pedido.Codigo.ToString());
            objetoSolicitacao.ownIds = listaPedidos;
            jsonRequest = JsonConvert.SerializeObject(objetoSolicitacao, Formatting.Indented);

            // Request
            HttpClient client = ObterClienteRequisicao(url, token);
            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
            var result = client.PostAsync(url, content).Result;

            // Response
            jsonResponse = result.Content.ReadAsStringAsync().Result;
            if (result.IsSuccessStatusCode)
            {
                if (!string.IsNullOrWhiteSpace(jsonResponse))
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.AngelLira.Pedidos.Retorno retornoJSON = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.AngelLira.Pedidos.Retorno>(jsonResponse);
                    if (retornoJSON == null)
                        mensagemErro = "Sem retorno na integração dos dados do pedido";
                    else
                    {
                        mensagemErro = SalvarDadosPedidoRetornoIntegracao(pedidoIntegracao, retornoJSON, unitOfWork);
                    }
                }
            }
            else
                mensagemErro = "Retorno integração: " + result.StatusCode.ToString();

        }

        private static string SalvarDadosPedidoRetornoIntegracao(PedidoIntegracao pedidoIntegracao, Dominio.ObjetosDeValor.Embarcador.Integracao.AngelLira.Pedidos.Retorno retornoJSON, UnitOfWork unitOfWork)
        {
            string retorno = string.Empty;
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repconfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repconfiguracaoTMS.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = pedidoIntegracao.Pedido;

            if (pedido.Veiculos == null)
                pedido.Veiculos = new List<Dominio.Entidades.Veiculo>();

            if (pedido.Motoristas == null)
                pedido.Motoristas = new List<Dominio.Entidades.Usuario>();

            Dominio.Entidades.Veiculo veiculoTracao = null;
            List<Dominio.Entidades.Veiculo> Listaveiculos = new List<Dominio.Entidades.Veiculo>();
            List<Dominio.Entidades.Usuario> ListaMotoristas = new List<Dominio.Entidades.Usuario>();

            if (retornoJSON.resource.truck != null)
            {
                veiculoTracao = repVeiculo.BuscarPorPlaca(retornoJSON.resource.truck.plate);
                if (veiculoTracao == null)
                    retorno = $"Veículo " + retornoJSON.resource.truck.plate + " não encontrado.";
            }

            if (retornoJSON.resource.trailers != null && retornoJSON.resource.trailers.Count > 0)
            {
                foreach (var veiculojson in retornoJSON.resource.trailers)
                {
                    Dominio.Entidades.Veiculo veiculoreboque = repVeiculo.BuscarPorPlaca(veiculojson.plate);

                    if (veiculoreboque != null)
                    {
                        if (veiculoTracao == null)
                            veiculoTracao = veiculoreboque;
                        else
                            Listaveiculos.Add(veiculoreboque);
                    }
                    else
                        retorno = $"Veículo " + veiculojson.plate + " não encontrado.";
                }
            }

            if (retornoJSON.resource.drivers != null && retornoJSON.resource.drivers.Count > 0)
            {
                foreach (var motoristajson in retornoJSON.resource.drivers)
                {
                    Dominio.Entidades.Usuario motorista = repUsuario.BuscarMotoristaPorNome(motoristajson.name)?.FirstOrDefault();

                    if (motorista == null)
                        retorno += $" Motorista " + retornoJSON.resource.drivers[0].name + " não encontrado;";
                    else
                        ListaMotoristas.Add(motorista);
                }
            }


            if (string.IsNullOrEmpty(retorno))
            {
                if (configuracaoTMS.PermitirSelecionarReboquePedido)
                {
                    pedido.VeiculoTracao = veiculoTracao;
                    pedido.Veiculos = Listaveiculos;
                }
                else
                {
                    if (veiculoTracao != null)
                        pedido.Veiculos.Add(veiculoTracao);

                    if (Listaveiculos?.Count > 0)
                    {
                        foreach (var vei in Listaveiculos)
                            pedido.Veiculos.Add(vei);
                    }
                }

                if (ListaMotoristas?.Count > 0)
                {
                    foreach (var mot in ListaMotoristas)
                        pedido.Motoristas.Add(mot);
                }


                repPedido.Atualizar(pedido);
            }

            return retorno;
        }

        private static bool VerificarStatusPedidoAngelira(PedidoIntegracao pedidoIntegracao, string urlWebService, string token, ref string mensagemErro, ref string jsonRequest, ref string jsonResponse)
        {
            jsonRequest = string.Empty;
            jsonResponse = string.Empty;

            bool ret = false;
            string url = urlWebService + "/loadplanningintegration/v1/order/status";

            Dominio.ObjetosDeValor.Embarcador.Integracao.AngelLira.Pedidos.OwnIds objetoSolicitacao = new Dominio.ObjetosDeValor.Embarcador.Integracao.AngelLira.Pedidos.OwnIds();
            List<string> listaPedidos = new List<string>();
            listaPedidos.Add(pedidoIntegracao.Pedido.Codigo.ToString());
            objetoSolicitacao.ownIds = listaPedidos;
            jsonRequest = JsonConvert.SerializeObject(objetoSolicitacao, Formatting.Indented);

            // Request
            HttpClient client = ObterClienteRequisicao(url, token);
            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
            var result = client.PostAsync(url, content).Result;

            // Response
            jsonResponse = result.Content.ReadAsStringAsync().Result;
            if (result.IsSuccessStatusCode)
            {
                if (!string.IsNullOrWhiteSpace(jsonResponse))
                {
                    List<Dominio.ObjetosDeValor.Embarcador.Integracao.AngelLira.Pedidos.RetornoConsultaStatus> retornoJSON = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Integracao.AngelLira.Pedidos.RetornoConsultaStatus>>(jsonResponse);
                    if (retornoJSON == null)
                        mensagemErro = "Sem retorno na integração da verificação do status do pedido";
                    else
                    {
                        if (string.IsNullOrWhiteSpace(retornoJSON[0].message))
                        {
                            if (retornoJSON[0].step != null && retornoJSON[0].step.desc == "Aguardando Efetivação") // agora buscar os dados do pedido.
                                ret = true;
                            else if (retornoJSON[0].step == null)
                                mensagemErro = "Problemas ao buscar status do pedido, por favor verifique os arquivos de integração";
                            else
                                mensagemErro = ""; //ainda nao esta aguardando efetivacao
                        }
                        else
                        {
                            mensagemErro = "Problemas ao buscar status do pedido, por favor verifique os arquivos de integração Retorno:" + retornoJSON[0].message;
                        }
                    }
                }
            }
            else
                mensagemErro = "Retorno integração da verificação do status do pedido: " + result.StatusCode.ToString();

            return ret;
        }

        private static void CancelarPedidoIntegracao(PedidoIntegracao pedidoIntegracao, string urlWebService, string token, Repositorio.UnitOfWork unitOfWork, ref string mensagemErro, ref string jsonRequest, ref string jsonResponse)
        {
            jsonRequest = string.Empty;
            jsonResponse = string.Empty;
            string url = urlWebService + "/loadplanningintegration/v1/order/cancel";

            Dominio.ObjetosDeValor.Embarcador.Integracao.AngelLira.Pedidos.OwnIds objetoSolicitacao = new Dominio.ObjetosDeValor.Embarcador.Integracao.AngelLira.Pedidos.OwnIds();
            List<string> listaPedidos = new List<string>();
            listaPedidos.Add(pedidoIntegracao.Pedido.Codigo.ToString());
            objetoSolicitacao.ownIds = listaPedidos;
            jsonRequest = JsonConvert.SerializeObject(objetoSolicitacao, Formatting.Indented);

            // Request
            HttpClient client = ObterClienteRequisicao(url, token);
            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
            var result = client.PostAsync(url, content).Result;

            // Response
            jsonResponse = result.Content.ReadAsStringAsync().Result;
            if (result.IsSuccessStatusCode)
            {
                if (!string.IsNullOrWhiteSpace(jsonResponse))
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.AngelLira.Pedidos.DataRetorno retornoJSON = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.AngelLira.Pedidos.DataRetorno>(jsonResponse);
                    if (retornoJSON == null)
                        mensagemErro = "Sem retorno na integração de cancelamento do pedido";
                    else
                    {
                        if (retornoJSON.data.error != null && retornoJSON.data.error.Count > 0)
                        {
                            mensagemErro = retornoJSON.data.error[0].message;
                        }
                        else
                        {
                            mensagemErro = String.Empty;
                        }
                    }
                }
            }
            else
                mensagemErro = "Retorno integração: " + result.StatusCode.ToString();

        }

        private static HttpClient ObterClienteRequisicao(string url, string token)
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoPedidosAngelLira));

            client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            if (!string.IsNullOrWhiteSpace(token))
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            return client;
        }

        public static string ObterToken(string url, string user, string senha)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            string endPoint = url + "/authorize/login";
            string mensagemErro = string.Empty;

            HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoPedidosAngelLira));

            client.BaseAddress = new Uri(endPoint);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            try
            {
                string jsonResponse = string.Empty;
                dynamic requestJson = new ExpandoObject();
                requestJson.password = senha;
                requestJson.username = user;

                string jsonRequest = JsonConvert.SerializeObject(requestJson, Formatting.Indented);

                var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
                var result = client.PostAsync(endPoint, content).Result;
                jsonResponse = result.Content.ReadAsStringAsync().Result;

                if (result.IsSuccessStatusCode)
                {

                    var retorno = (string)(result.Content.ReadAsStringAsync().Result);
                    if (!string.IsNullOrWhiteSpace(retorno))
                    {
                        dynamic objetoRetorno = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(retorno);

                        string tokenRetorno = (string)objetoRetorno.access_token;
                        if (!string.IsNullOrWhiteSpace(tokenRetorno))
                        {
                            return tokenRetorno;
                        }
                        else
                        {
                            Servicos.Log.TratarErro("get-token: não teve retorno", "IntegracaoAngelLira");
                            return string.Empty;

                        }
                    }
                    else
                    {
                        Servicos.Log.TratarErro("get-token: não teve retorno", "IntegracaoAngelLira");
                        return string.Empty;
                    }
                }
                else
                {
                    mensagemErro = "get-token: " + result.StatusCode.ToString();
                    Servicos.Log.TratarErro("get-token: " + mensagemErro, "IntegracaoAngelLira");
                    return string.Empty;
                }
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro("ObterToken: " + excecao, "IntegracaoAngelLira");
                Servicos.Log.TratarErro("URL: " + endPoint, "IntegracaoAngelLira");

                return string.Empty;
            }
        }

        private static string RetornarObjetoPedido(int Codpedido, Repositorio.UnitOfWork unitOfWork)
        {

            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = repPedido.BuscarPorCodigo(Codpedido);

            Repositorio.Embarcador.Pedidos.PedidoProduto repPedidoProduto = new Repositorio.Embarcador.Pedidos.PedidoProduto(unitOfWork);
            Dominio.ObjetosDeValor.Embarcador.Integracao.AngelLira.Pedidos.Pedidos pedidos = new Dominio.ObjetosDeValor.Embarcador.Integracao.AngelLira.Pedidos.Pedidos();

            Dominio.ObjetosDeValor.Embarcador.Integracao.AngelLira.Pedidos.Pedido pedidoObject = new Dominio.ObjetosDeValor.Embarcador.Integracao.AngelLira.Pedidos.Pedido();

            List<Dominio.ObjetosDeValor.Embarcador.Integracao.AngelLira.Pedidos.Delivery> entregas = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.AngelLira.Pedidos.Delivery>();
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.AngelLira.Pedidos.documents> documents = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.AngelLira.Pedidos.documents>();
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.AngelLira.Pedidos.Product> produtos = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.AngelLira.Pedidos.Product>();

            //List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto> listaPedidoProduto = repPedidoProduto.BuscarPorPedido(Codpedido);

            //foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoProduto pedidoProduto in listaPedidoProduto)
            //{
            //    Dominio.ObjetosDeValor.Embarcador.Integracao.AngelLira.Pedidos.Product produto = new Dominio.ObjetosDeValor.Embarcador.Integracao.AngelLira.Pedidos.Product
            //    {
            //        productOwnId = pedidoProduto.Produto.CodigoProdutoEmbarcador,
            //        volumetry = (int)pedidoProduto.Quantidade > 0 ? (int)pedidoProduto.Quantidade : 1,
            //        volumetryCodeId = 3, //3 fixo //pedidoProduto.Produto.SiglaUnidade == "M",
            //        measure = (int)pedidoProduto.Quantidade > 0 ? (int)pedidoProduto.Quantidade : 1,
            //        measureCodeId = 5 //5 fixo
            //    };

            //    produtos.Add(produto);
            //}

            //Dominio.ObjetosDeValor.Embarcador.Integracao.AngelLira.Pedidos.documents documento = new Dominio.ObjetosDeValor.Embarcador.Integracao.AngelLira.Pedidos.documents();
            //documento.products = produtos;
            //documents.Add(documento);

            Dominio.ObjetosDeValor.Embarcador.Integracao.AngelLira.Pedidos.Delivery entrega = new Dominio.ObjetosDeValor.Embarcador.Integracao.AngelLira.Pedidos.Delivery
            {
                clientOwnId = pedido.Destinatario.CPF_CNPJ_SemFormato, //codigo da entrega
                deliveryDate = pedido.DataEntrega.HasValue ? pedido.DataEntrega.Value : DateTime.Now.AddDays(2),
                sequence = 1,
                deliveryOwnId = pedido.Destinatario.CPF_CNPJ_SemFormato,
                documents = documents,
            };

            entregas.Add(entrega); // será sempre uma

            pedidoObject.ownId = pedido.Codigo.ToString(); //codigo pedido
            pedidoObject.loadingDate = pedido.DataInicialColeta.HasValue ? pedido.DataInicialColeta.Value.ToString("yyyy-MM-dd HH:mm:ss") : pedido.DataEntrega.HasValue ? pedido.DataEntrega.Value.ToString("yyyy-MM-dd HH:mm:ss") : DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"); // data coleta
            pedidoObject.shipperId = 3814; //991 para producao //fixo de acordo com instruções da Angellira (call)
            pedidoObject.originOwnId = pedido.Remetente.CPF_CNPJ_SemFormato; // codigo do local da coleta
            //pedidoObject.orderValue = 1.0;
            //pedidoObject.routeId = pedido.RotaFrete?.Codigo ?? 0; //codigo da rota
            //pedidoObject.vehicleTypeId = pedido.VeiculoTracao?.TipoDoVeiculo?.Codigo ?? 0;
            pedidoObject.deliveries = entregas;

            List<Dominio.ObjetosDeValor.Embarcador.Integracao.AngelLira.Pedidos.Pedido> pedidosObject = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.AngelLira.Pedidos.Pedido>();
            pedidosObject.Add(pedidoObject);

            pedidos.data = pedidosObject;

            return JsonConvert.SerializeObject(pedidos, Formatting.Indented);

        }

        #endregion


    }
}
