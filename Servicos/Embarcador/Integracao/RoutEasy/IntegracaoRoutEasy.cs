using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Integracao.RoutEasy;
using Infrastructure.Services.HttpClientFactory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Integracao.RoutEasy
{
    public class IntegracaoRoutEasy
    {
        #region Atributos

        private readonly Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado _auditado;
        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion Atributos

        #region Construtores

        public IntegracaoRoutEasy(Repositorio.UnitOfWork unitOfWork) : this(unitOfWork, auditado: null) { }

        public IntegracaoRoutEasy(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            _auditado = auditado;
            _unitOfWork = unitOfWork;
        }

        #endregion Construtores

        #region Métodos Públicos

        public void IntegrarPedidos(Dominio.Entidades.Embarcador.Roteirizador.RoteirizadorIntegracao roteirizadorIntegracao)
        {
            Repositorio.Embarcador.Roteirizador.RoteirizadorIntegracao repositorioRoteirizadorIntegracao = new Repositorio.Embarcador.Roteirizador.RoteirizadorIntegracao(_unitOfWork);
            Repositorio.Embarcador.Roteirizador.RoteirizadorIntegracaoPedido repositorioRoteirizadorIntegracaoPedido = new Repositorio.Embarcador.Roteirizador.RoteirizadorIntegracaoPedido(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Roteirizador.RoteirizadorIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Roteirizador.RoteirizadorIntegracaoArquivo>(_unitOfWork);

            string jsonRequisicao = string.Empty;
            string jsonRetorno = string.Empty;

            roteirizadorIntegracao.NumeroTentativas += 1;
            roteirizadorIntegracao.DataIntegracao = DateTime.Now;

            try
            {
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoRouteasy configuracaoIntegracao = ObterConfiguracaoIntegracao();
                List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = repositorioRoteirizadorIntegracaoPedido.BuscarPedidosPorRoteirizadorIntegracao(roteirizadorIntegracao.Codigo);
                Dominio.ObjetosDeValor.Embarcador.Integracao.RoutEasy.IntegracaoPedido integracaoPedido = ObterIntegracaoPedido(pedidos, configuracaoIntegracao);
                jsonRequisicao = JsonConvert.SerializeObject(integracaoPedido, Formatting.Indented);
                StringContent conteudoRequisicao = new StringContent(jsonRequisicao, Encoding.UTF8, "application/json");
                HttpClient requisicao = CriarRequisicao(configuracaoIntegracao.APIKey);
                HttpResponseMessage retornoRequisicao = requisicao.PostAsync($"{configuracaoIntegracao.URL}/orders/import", conteudoRequisicao).Result;
                jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;

                if ((retornoRequisicao.StatusCode == HttpStatusCode.OK) || (retornoRequisicao.StatusCode == HttpStatusCode.Created) || (retornoRequisicao.StatusCode == HttpStatusCode.Accepted))
                {
                    roteirizadorIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                    roteirizadorIntegracao.ProblemaIntegracao = "Integração realizada com sucesso";
                }
                else
                {
                    roteirizadorIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                    roteirizadorIntegracao.ProblemaIntegracao = "Integração realizada com falha";
                }

                servicoArquivoTransacao.Adicionar(roteirizadorIntegracao, jsonRequisicao, jsonRetorno, "json");
            }
            catch (ServicoException excecao)
            {
                roteirizadorIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                roteirizadorIntegracao.ProblemaIntegracao = excecao.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                roteirizadorIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                roteirizadorIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração com a RoutEasy";

                servicoArquivoTransacao.Adicionar(roteirizadorIntegracao, jsonRequisicao, jsonRetorno, "json");
            }

            repositorioRoteirizadorIntegracao.Atualizar(roteirizadorIntegracao);

            if (roteirizadorIntegracao.SituacaoIntegracao == SituacaoIntegracao.Integrado)
                repositorioRoteirizadorIntegracaoPedido.AtualizarSituacaoRoteirizacaoPedidosPorRoteirizadorIntegracao(roteirizadorIntegracao.Codigo, SituacaoRoteirizadorIntegracao.Integrado);
        }

        public void AtualizarPedidos(Dominio.Entidades.Embarcador.Roteirizador.RoteirizadorIntegracao roteirizadorIntegracao)
        {
            Repositorio.Embarcador.Roteirizador.RoteirizadorIntegracao repositorioRoteirizadorIntegracao = new Repositorio.Embarcador.Roteirizador.RoteirizadorIntegracao(_unitOfWork);
            Repositorio.Embarcador.Roteirizador.RoteirizadorIntegracaoPedido repositorioRoteirizadorIntegracaoPedido = new Repositorio.Embarcador.Roteirizador.RoteirizadorIntegracaoPedido(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Roteirizador.RoteirizadorIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Roteirizador.RoteirizadorIntegracaoArquivo>(_unitOfWork);

            string jsonRequisicao = string.Empty;
            string jsonRetorno = string.Empty;

            roteirizadorIntegracao.NumeroTentativas += 1;
            roteirizadorIntegracao.DataIntegracao = DateTime.Now;

            try
            {
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoRouteasy configuracaoIntegracao = ObterConfiguracaoIntegracao();
                List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = repositorioRoteirizadorIntegracaoPedido.BuscarPedidosPorRoteirizadorIntegracao(roteirizadorIntegracao.Codigo);

                PedidosAtualizacaoSituacao request = new PedidosAtualizacaoSituacao
                {
                    OrderNumbers = pedidos.Select(p => p.NumeroPedidoEmbarcador).ToList(),
                    Site = pedidos.FirstOrDefault()?.Filial.CodigoFilialEmbarcador
                };

                jsonRequisicao = JsonConvert.SerializeObject(request, Formatting.Indented);
                StringContent conteudoRequisicao = new StringContent(jsonRequisicao, Encoding.UTF8, "application/json");
                HttpClient requisicao = CriarRequisicao(configuracaoIntegracao.APIKey);

                HttpResponseMessage retornoRequisicao = requisicao.PutAsync($"{configuracaoIntegracao.URL}/orders/update-status-to-new?api_key={configuracaoIntegracao.APIKey}", conteudoRequisicao).Result;
                jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;

                if (retornoRequisicao.IsSuccessStatusCode)
                {
                    roteirizadorIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                    roteirizadorIntegracao.ProblemaIntegracao = "Atualização de status realizada com sucesso";
                }
                else
                {
                    roteirizadorIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                    roteirizadorIntegracao.ProblemaIntegracao = $"Atualização de status realizada com falha: {retornoRequisicao.StatusCode}";
                }

                servicoArquivoTransacao.Adicionar(roteirizadorIntegracao, jsonRequisicao, jsonRetorno, "json");
            }
            catch (ServicoException excecao)
            {
                roteirizadorIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                roteirizadorIntegracao.ProblemaIntegracao = excecao.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                roteirizadorIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                roteirizadorIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao atualizar o status do pedido na RoutEasy";

                servicoArquivoTransacao.Adicionar(roteirizadorIntegracao, jsonRequisicao, jsonRetorno, "json");
            }

            repositorioRoteirizadorIntegracao.Atualizar(roteirizadorIntegracao);

            if (roteirizadorIntegracao.SituacaoIntegracao == SituacaoIntegracao.Integrado)
                repositorioRoteirizadorIntegracaoPedido.AtualizarSituacaoRoteirizacaoPedidosPorRoteirizadorIntegracao(roteirizadorIntegracao.Codigo, SituacaoRoteirizadorIntegracao.Integrado);
        }

        public void IntegrarPedidosAtualizacaoSituacao(Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaDadosTransporteIntegracao)
        {
            Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao repositorioCargaDadosTransporteIntegracao = new Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoHistorico repCargaPedidoHistorico = new Repositorio.Embarcador.Cargas.CargaPedidoHistorico(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            cargaDadosTransporteIntegracao.DataIntegracao = DateTime.Now;
            cargaDadosTransporteIntegracao.NumeroTentativas++;

            string jsonRequisicao = string.Empty;
            string jsonRetorno = string.Empty;

            try
            {
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoRouteasy configuracaoIntegracao = ObterConfiguracaoIntegracao();

                List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoHistorico> pedidoHistoricos = repCargaPedidoHistorico.BuscarParaIntegracaoRouteasy(cargaDadosTransporteIntegracao.Carga.Codigo).GetAwaiter().GetResult();

                if (pedidoHistoricos.Count == 0)
                    throw new ServicoException("Registros não encontrados");

                PedidosAtualizacaoSituacao request = new PedidosAtualizacaoSituacao
                {
                    OrderNumbers = pedidoHistoricos.Select(h => h.Pedido.NumeroPedidoEmbarcador).ToList(),
                    Site = cargaDadosTransporteIntegracao.Carga.Filial.CodigoFilialEmbarcador
                };

                jsonRequisicao = JsonConvert.SerializeObject(request, Formatting.Indented);
                StringContent conteudoRequisicao = new StringContent(jsonRequisicao, Encoding.UTF8, "application/json");

                HttpClient httpClient = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoRoutEasy));
                httpClient.BaseAddress = new Uri(configuracaoIntegracao.URL);
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                string endpoint = $"orders/update-status-to-new?api_key={configuracaoIntegracao.APIKey}";

                HttpResponseMessage response = httpClient.PutAsync(endpoint, conteudoRequisicao).Result;
                jsonRetorno = response.Content.ReadAsStringAsync().Result;

                if (response.IsSuccessStatusCode)
                {
                    foreach (var historico in pedidoHistoricos)
                    {
                        historico.SituacaoIntegracao = CargaPedidoHistoricoSituacaoIntegracao.Integrado;
                        repCargaPedidoHistorico.Atualizar(historico);
                    }

                    cargaDadosTransporteIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                    cargaDadosTransporteIntegracao.ProblemaIntegracao = "Integração realizada com sucesso";
                }
                else
                {
                    cargaDadosTransporteIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                    cargaDadosTransporteIntegracao.ProblemaIntegracao = $"Falha na integração: {response.StatusCode} - {jsonRetorno}";
                }

                servicoArquivoTransacao.Adicionar(cargaDadosTransporteIntegracao, jsonRequisicao, jsonRetorno, "json");
            }
            catch (ServicoException ex)
            {
                cargaDadosTransporteIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaDadosTransporteIntegracao.ProblemaIntegracao = ex.Message;
            }
            catch (Exception ex)
            {
                Log.TratarErro(ex);
                cargaDadosTransporteIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaDadosTransporteIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração com a RoutEasy";

                if (!string.IsNullOrEmpty(jsonRequisicao))
                {
                    servicoArquivoTransacao.Adicionar(cargaDadosTransporteIntegracao, jsonRequisicao, jsonRetorno, "json");
                }
            }
            finally
            {
                repositorioCargaDadosTransporteIntegracao.Atualizar(cargaDadosTransporteIntegracao);
            }
        }

        public async Task IntegrarPedidoAtualizacaoSituacao(Dominio.Entidades.Embarcador.Pedidos.PedidoIntegracao pedidoIntegracao)
        {
            Repositorio.Embarcador.Pedidos.PedidoIntegracao repositorioPedidoIntegracao = new(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Pedidos.PedidoIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Pedidos.PedidoIntegracaoArquivo>(_unitOfWork);

            pedidoIntegracao.Tentativas++;
            pedidoIntegracao.DataIntegracao = DateTime.Now;

            string jsonRequisicao = string.Empty;
            string jsonRetorno = string.Empty;

            try
            {
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoRouteasy configuracaoIntegracao = await ObterConfiguracaoIntegracaoAsync();

                if (pedidoIntegracao.Pedido == null)
                    throw new ServicoException("Registro não encontrado");

                PedidosAtualizacaoSituacao request = new PedidosAtualizacaoSituacao
                {
                    OrderNumbers = new List<string>() { pedidoIntegracao.Pedido.NumeroPedidoEmbarcador },
                    Site = pedidoIntegracao.Pedido.Filial.CodigoFilialEmbarcador
                };

                jsonRequisicao = JsonConvert.SerializeObject(request, Formatting.Indented);
                StringContent conteudoRequisicao = new StringContent(jsonRequisicao, Encoding.UTF8, "application/json");

                HttpClient httpClient = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoRoutEasy));
                httpClient.BaseAddress = new Uri(configuracaoIntegracao.URL);
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                string endpoint = $"orders/update-status-to-new?api_key={configuracaoIntegracao.APIKey}";

                HttpResponseMessage response = await httpClient.PutAsync(endpoint, conteudoRequisicao);
                jsonRetorno = response.Content.ReadAsStringAsync().Result;

                if (response.IsSuccessStatusCode)
                {
                    pedidoIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                    pedidoIntegracao.ProblemaIntegracao = "Integração realizada com sucesso";
                }
                else
                {
                    pedidoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                    pedidoIntegracao.ProblemaIntegracao = $"Falha na integração: {response.StatusCode} - {jsonRetorno}";
                }

                servicoArquivoTransacao.Adicionar(pedidoIntegracao, jsonRequisicao, jsonRetorno, "json");
            }
            catch (ServicoException ex)
            {
                pedidoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                pedidoIntegracao.ProblemaIntegracao = ex.Message;
            }
            catch (Exception ex)
            {
                Log.TratarErro(ex);
                pedidoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                pedidoIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração com a RoutEasy";

                if (!string.IsNullOrEmpty(jsonRequisicao))
                {
                    servicoArquivoTransacao.Adicionar(pedidoIntegracao, jsonRequisicao, jsonRetorno, "json");
                }
            }
            finally
            {
                await repositorioPedidoIntegracao.AtualizarAsync(pedidoIntegracao);
            }
        }

        public void IntegrarCancelamentoPedidos(Dominio.Entidades.Embarcador.Roteirizador.RoteirizadorIntegracao roteirizadorIntegracao)
        {
            Repositorio.Embarcador.Roteirizador.RoteirizadorIntegracao repositorioRoteirizadorIntegracao = new Repositorio.Embarcador.Roteirizador.RoteirizadorIntegracao(_unitOfWork);
            Repositorio.Embarcador.Roteirizador.RoteirizadorIntegracaoPedido repositorioRoteirizadorIntegracaoPedido = new Repositorio.Embarcador.Roteirizador.RoteirizadorIntegracaoPedido(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Roteirizador.RoteirizadorIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Roteirizador.RoteirizadorIntegracaoArquivo>(_unitOfWork);

            string jsonRequisicao = string.Empty;
            string jsonRetorno = string.Empty;

            roteirizadorIntegracao.NumeroTentativas += 1;
            roteirizadorIntegracao.DataIntegracao = DateTime.Now;

            try
            {
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoRouteasy configuracaoIntegracao = ObterConfiguracaoIntegracao();
                List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = repositorioRoteirizadorIntegracaoPedido.BuscarPedidosPorRoteirizadorIntegracao(roteirizadorIntegracao.Codigo);
                Dominio.ObjetosDeValor.Embarcador.Integracao.RoutEasy.IntegracaoPedidoCancelamento integracaoPedidoCancelamento = ObterIntegracaoPedidoCancelamento(pedidos);
                jsonRequisicao = JsonConvert.SerializeObject(integracaoPedidoCancelamento, Formatting.Indented);
                StringContent conteudoRequisicao = new StringContent(jsonRequisicao, Encoding.UTF8, "application/json");
                HttpClient requisicao = CriarRequisicao(configuracaoIntegracao.APIKey);
                HttpResponseMessage retornoRequisicao = requisicao.PostAsync($"{configuracaoIntegracao.URL}/orders/cancel", conteudoRequisicao).Result;
                jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;

                if ((retornoRequisicao.StatusCode == HttpStatusCode.OK) || (retornoRequisicao.StatusCode == HttpStatusCode.Created) || (retornoRequisicao.StatusCode == HttpStatusCode.Accepted))
                {
                    roteirizadorIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                    roteirizadorIntegracao.ProblemaIntegracao = "Integração realizada com sucesso";
                }
                else
                {
                    roteirizadorIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                    roteirizadorIntegracao.ProblemaIntegracao = "Integração realizada com falha";
                }

                servicoArquivoTransacao.Adicionar(roteirizadorIntegracao, jsonRequisicao, jsonRetorno, "json");
            }
            catch (ServicoException excecao)
            {
                roteirizadorIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                roteirizadorIntegracao.ProblemaIntegracao = excecao.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                roteirizadorIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                roteirizadorIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração com a RoutEasy";

                servicoArquivoTransacao.Adicionar(roteirizadorIntegracao, jsonRequisicao, jsonRetorno, "json");
            }

            repositorioRoteirizadorIntegracao.Atualizar(roteirizadorIntegracao);

            if (roteirizadorIntegracao.SituacaoIntegracao == SituacaoIntegracao.Integrado)
                repositorioRoteirizadorIntegracaoPedido.AtualizarSituacaoRoteirizacaoPedidosPorRoteirizadorIntegracao(roteirizadorIntegracao.Codigo, SituacaoRoteirizadorIntegracao.Cancelado);
        }

        #endregion Métodos Públicos

        #region Métodos Privados

        private Dominio.Entidades.Embarcador.Roteirizador.RoteirizadorIntegracao AdicionarRoteirizadorIntegracao(List<int> codigosPedidos, TipoRoteirizadorIntegracao tipo)
        {
            Repositorio.Embarcador.Roteirizador.RoteirizadorIntegracao repositorioRoteirizadorIntegracao = new Repositorio.Embarcador.Roteirizador.RoteirizadorIntegracao(_unitOfWork);
            Repositorio.Embarcador.Roteirizador.RoteirizadorIntegracaoPedido repositorioRoteirizadorIntegracaoPedido = new Repositorio.Embarcador.Roteirizador.RoteirizadorIntegracaoPedido(_unitOfWork);
            List<int> codigosPedidosIntegrar;

            if (tipo == TipoRoteirizadorIntegracao.CancelarPedido)
                codigosPedidosIntegrar = repositorioRoteirizadorIntegracaoPedido.BuscarCodigoPedidosIntegradosRoteirizador(codigosPedidos);
            else
                codigosPedidosIntegrar = codigosPedidos;

            if (codigosPedidosIntegrar.Count == 0)
                throw new ServicoException("Não existem pedidos para serem integrados");

            Dominio.Entidades.Embarcador.Roteirizador.RoteirizadorIntegracao roteirizadorIntegracao = new Dominio.Entidades.Embarcador.Roteirizador.RoteirizadorIntegracao()
            {
                Tipo = tipo,
                Usuario = _auditado?.Usuario,
                ProblemaIntegracao = string.Empty
            };

            repositorioRoteirizadorIntegracao.Inserir(roteirizadorIntegracao);
            repositorioRoteirizadorIntegracaoPedido.InserirPedidosPorRoteirizadorIntegracao(roteirizadorIntegracao.Codigo, codigosPedidosIntegrar);

            return roteirizadorIntegracao;
        }

        private HttpClient CriarRequisicao(string apiKey)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            HttpClient requisicao = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoRoutEasy));

            requisicao.DefaultRequestHeaders.Accept.Clear();
            requisicao.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            requisicao.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);

            return requisicao;
        }

        private Dominio.Entidades.Embarcador.Configuracoes.IntegracaoRouteasy ObterConfiguracaoIntegracao()
        {
            return ObterConfiguracaoIntegracaoAsync().GetAwaiter().GetResult();
        }

        private async Task<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoRouteasy> ObterConfiguracaoIntegracaoAsync()
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoRouteasy repositorioIntegracaoRouteasy = new Repositorio.Embarcador.Configuracoes.IntegracaoRouteasy(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoRouteasy configuracaoIntegracao = await repositorioIntegracaoRouteasy.BuscarPrimeiroRegistroAsync();

            if (!(configuracaoIntegracao?.PossuiIntegracao ?? false))
                throw new ServicoException("Não existe configuração de integração disponível para a RoutEasy.");

            if (string.IsNullOrWhiteSpace(configuracaoIntegracao.URL))
                throw new ServicoException("URL de integração não configurada para a RoutEasy.");

            if (string.IsNullOrWhiteSpace(configuracaoIntegracao.APIKey))
                throw new ServicoException("API Key de integração não configurada para a RoutEasy.");

            return configuracaoIntegracao;
        }

        #endregion Métodos Privados

        #region Métodos Privados - Integração de Pedidos

        private Dominio.ObjetosDeValor.Embarcador.Integracao.RoutEasy.IntegracaoPedido ObterIntegracaoPedido(List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos, Dominio.Entidades.Embarcador.Configuracoes.IntegracaoRouteasy configuracaoIntegracaoRouteasy)
        {
            Repositorio.Embarcador.Pedidos.PedidoProduto repositorioPedidoProduto = new Repositorio.Embarcador.Pedidos.PedidoProduto(_unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoAdicional repositorioPedidoAdicional = new Repositorio.Embarcador.Pedidos.PedidoAdicional(_unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.Integracao.RoutEasy.IntegracaoPedido integracaoPedido = new Dominio.ObjetosDeValor.Embarcador.Integracao.RoutEasy.IntegracaoPedido()
            {
                Pedidos = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.RoutEasy.Pedido>()
            };

            List<int> codigosPedidos = pedidos.Select(o => o.Codigo).ToList();

            List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto> pedidosProdutos = repositorioPedidoProduto.BuscarPorPedidos(codigosPedidos);
            List<Dominio.Entidades.Embarcador.Pedidos.PedidoAdicional> pedidosAdicionais = repositorioPedidoAdicional.BuscarPorPedidos(codigosPedidos);

            foreach (Dominio.Entidades.Embarcador.Pedidos.Pedido pedido in pedidos)
            {
                List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto> produtos = pedidosProdutos.FindAll(pedidoProduto => pedidoProduto.Pedido.Codigo == pedido.Codigo);
                Dominio.Entidades.Embarcador.Pedidos.PedidoAdicional pedidoAdicional = pedidosAdicionais.Find(pedidoAdicional => pedidoAdicional.Pedido.Codigo == pedido.Codigo);

                integracaoPedido.Pedidos.Add(ObterPedidoIntegrar(pedido, produtos, pedidoAdicional, configuracaoIntegracaoRouteasy));
            }

            return integracaoPedido;
        }

        private string AjustarTimer(DateTime? dataCriacaoVenda, int deslocamentoHoras = 0)
        {
            try
            {
                if (!dataCriacaoVenda.HasValue)
                    return string.Empty;

                DateTime dataAjustada = dataCriacaoVenda.Value.AddHours(deslocamentoHoras);
                return dataAjustada.ToDateTimeStringISO8601();
            }
            catch
            {
                return string.Empty;
            }
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.RoutEasy.Pedido ObterPedidoIntegrar(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto> produtos, Dominio.Entidades.Embarcador.Pedidos.PedidoAdicional pedidoAdicional, Dominio.Entidades.Embarcador.Configuracoes.IntegracaoRouteasy configuracaoIntegracaoRouteasy)
        {
            Dominio.Entidades.Cliente clienteOrigem = pedido.Expedidor ?? pedido.Remetente;
            Dominio.Entidades.Cliente clienteDestino = pedido.Recebedor ?? pedido.Destinatario;
            Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco enderecoOrigem = (pedido.Expedidor != null) ? pedido.EnderecoExpedidor : pedido.EnderecoOrigem;
            Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco enderecoDestino = (pedido.Recebedor != null) ? pedido.EnderecoRecebedor : pedido.EnderecoDestino;
            bool servicoColeta = clienteDestino.CPF_CNPJ_SemFormato == pedido.Filial.CNPJ_SemFormato;
            Dominio.Entidades.Cliente cliente = servicoColeta ? clienteOrigem : clienteDestino;
            Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco endereco = servicoColeta ? enderecoOrigem : enderecoDestino;
            bool usarOutroEndereco = servicoColeta ? pedido.UsarOutroEnderecoOrigem : pedido.UsarOutroEnderecoDestino;
            //data limite pra agendar pedido.DataValidade

            Dominio.ObjetosDeValor.Embarcador.Integracao.RoutEasy.Pedido pedidoIntegrar = new Dominio.ObjetosDeValor.Embarcador.Integracao.RoutEasy.Pedido()
            {
                CodigoIntegracaoFilial = pedido.Filial.CodigoFilialEmbarcador,
                NumeroNotasFiscais = pedido.NotasFiscais != null ? string.Join(",", pedido.NotasFiscais.Select(notaFiscal => notaFiscal.Numero)) : string.Empty,
                NumeroPedido = pedido.NumeroPedidoEmbarcador,
                TipoServicoRoteirizacao = servicoColeta ? "pickup" : "delivery",
                DataCriacaoPedido = configuracaoIntegracaoRouteasy.EnviarDataCriacaoVendaPedidoAbaAdicionaisIntegracao ? AjustarTimer(pedidoAdicional?.DataCriacaoVenda, 3) : AjustarTimer(pedido.DataCriacao, 3),
                Reentrega = pedido.ReentregaSolicitada,
                DadosCarregamento = ObterDadosCarregamento(pedido.PesoTotal, pedido.ValorTotalNotasFiscais, pedido.CubagemTotal, produtos),
                LocalColetaEntrega = new Dominio.ObjetosDeValor.Embarcador.Integracao.RoutEasy.LocalColetaEntrega()
                {
                    CodigoCliente = cliente.CodigoIntegracao,
                    NomeCliente = cliente.Nome,
                    Restricao = new Dominio.ObjetosDeValor.Embarcador.Integracao.RoutEasy.PedidoRestricao()
                    {
                        CodigoAgrupamentoCarregamento = pedido.CodigoAgrupamentoCarregamento ?? string.Empty
                    }
                },
                Metadados = new Dominio.ObjetosDeValor.Embarcador.Integracao.RoutEasy.PedidoMetadados()
                {
                    Protocolo = pedido.Protocolo
                },
                Itens = ObterPedidoItens(produtos),
                InformacoesAdicionais = ObterPedidoInformacoesAdicionais(pedido),
                DataPrevisaoEntrega = AjustarTimer(pedido.PrevisaoEntrega, 3),
                DataColetaPedido = AjustarTimer(pedido.DataCarregamentoPedido, 3),
                Restricao = ObterRestricao(pedido, pedidoAdicional)
            };

            if (pedido.DataValidade.HasValue)
                pedidoIntegrar.RestricoesAgendamento = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.RoutEasy.PedidoRestricaoAgendamento>()
            {
                new Dominio.ObjetosDeValor.Embarcador.Integracao.RoutEasy.PedidoRestricaoAgendamento()
                {
                    DataLimite = pedido.DataValidade
                }
            };

            pedidoIntegrar.LocalColetaEntrega.Endereco = ObterEndereco(cliente, endereco, usarOutroEndereco);

            return pedidoIntegrar;
        }

        private List<decimal> ObterDadosCarregamento(decimal pesoTotal, decimal valorTotalNotasFiscais, decimal cubagemTotal, List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto> pedidoProdutos)
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoRouteasy repositorioConfiguracaoIntegracaoRouteasy = new Repositorio.Embarcador.Configuracoes.IntegracaoRouteasy(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoRouteasy configuracaoIntegracaoRouteasy = repositorioConfiguracaoIntegracaoRouteasy.Buscar();

            string configuracaoLoads = configuracaoIntegracaoRouteasy?.ConfiguracaoLoads ?? string.Empty;

            decimal totalLT = 0;
            decimal totalUN = 0;
            decimal totalBAG = 0;
            decimal totalSAC = 0;
            decimal nPalets = 0;

            foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoProduto pedidoProduto in pedidoProdutos)
            {
                string unidadePrimaria = pedidoProduto.Produto?.SiglaUnidade?.ToUpper() ?? string.Empty;
                string unidadeSecundaria = pedidoProduto.UnidadeMedidaSecundaria?.Trim()?.ToUpper() ?? string.Empty;
                nPalets = pedidoProduto.QuantidadePalet;

                if (unidadePrimaria != unidadeSecundaria)
                {
                    switch (unidadeSecundaria)
                    {
                        case "LT":
                            totalLT += pedidoProduto.QuantidadeSecundaria;
                            break;
                        case "UN":
                            totalUN += pedidoProduto.QuantidadeSecundaria;
                            break;
                        case "BAG":
                            totalBAG += pedidoProduto.QuantidadeSecundaria;
                            break;
                        case "SAC":
                            totalSAC += pedidoProduto.QuantidadeSecundaria;
                            break;
                    }
                }

                switch (unidadePrimaria)
                {
                    case "LT":
                        totalLT += pedidoProduto.Quantidade;
                        break;
                    case "UN":
                        totalUN += pedidoProduto.Quantidade;
                        break;
                    case "BAG":
                        totalBAG += pedidoProduto.Quantidade;
                        break;
                    case "SAC":
                        totalSAC += pedidoProduto.Quantidade;
                        break;
                }
            }

            Dictionary<string, decimal> valores = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase)
            {
                { "Peso", pesoTotal },
                { "Valor", valorTotalNotasFiscais },
                { "Litro", totalLT },
                { "Unidade", totalUN },
                { "Bag", totalBAG },
                { "Sac", totalSAC },
                { "Volume", cubagemTotal },
                { "Pallet", nPalets }
            };

            if (string.IsNullOrWhiteSpace(configuracaoLoads))
                configuracaoLoads = "#Peso#Volume#Valor";

            List<decimal> dadosCarregamento = new List<decimal>();
            string[] campos = configuracaoLoads.Split(new char[] { '#' }, StringSplitOptions.RemoveEmptyEntries);

            if (campos.Length > 6)
                throw new InvalidOperationException("A configuração de Loads suporta no máximo 6 tags.");

            foreach (string campo in campos)
            {
                if (valores.TryGetValue(campo.Trim(), out var valor))
                    dadosCarregamento.Add(valor);
                else
                    dadosCarregamento.Add(0);
            }

            return dadosCarregamento;
        }

        private List<string> ObterSkillsPedidoRestricao(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Dominio.Entidades.Embarcador.Pedidos.PedidoAdicional pedidoAdicional)
        {
            List<string> skills = new List<string>();

            skills.Add(pedido.TipoDeCarga?.CodigoTipoCargaEmbarcador);
            skills.Add(pedidoAdicional?.GrupoFreteMaterial);

            return skills;
        }

        private List<string> ObterPedidoInformacoesAdicionais(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido)
        {
            List<string> adicionais = new List<string>();

            adicionais.Add(!string.IsNullOrWhiteSpace(pedido.Observacao) ? pedido.Observacao : "");
            adicionais.Add($"{pedido.TipoOperacao?.Descricao} | {pedido.CanalEntrega?.Descricao}");
            adicionais.Add(null);

            return adicionais;
        }

        private PedidoRestricao ObterRestricao(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Dominio.Entidades.Embarcador.Pedidos.PedidoAdicional pedidoAdicional)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.RoutEasy.PedidoRestricao pedidoRestricao = new Dominio.ObjetosDeValor.Embarcador.Integracao.RoutEasy.PedidoRestricao();
            pedidoRestricao.CodigoIntegracaoTipoCarga = ObterSkillsPedidoRestricao(pedido, pedidoAdicional);
            pedidoRestricao.DetalhesVeiculo = ObterCamposVeiculo(pedido);
            pedidoRestricao.NivelPrioridade = pedido.CanalEntrega?.NivelPrioridade ?? 0;
            if(!string.IsNullOrWhiteSpace(pedido.RegiaoDestino?.Descricao))
                pedidoRestricao.Regiao = pedido.RegiaoDestino?.Descricao;

            return pedidoRestricao;
        }

        private DetalhesVeiculo ObterCamposVeiculo(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido)
        {
            if (pedido.ModeloVeicularCarga == null)
                return null;

            return new DetalhesVeiculo()
            {
                CondicoesVeiculo = new List<CondicoesVeiculo>()
                {
                    new CondicoesVeiculo()
                    {
                        ChaveCampo = "name",
                        TipoCampo = "string",
                        Operador = "eq",
                        DescricaoModeloVeicular = pedido.ModeloVeicularCarga?.Descricao ?? string.Empty
                    }
                }
            };
        }

        private string ObterCampoPersonalizadoProduto(Dominio.Entidades.Embarcador.Pedidos.PedidoProduto produto)
        {
            string numeroLinha = null;

            if (produto == null || string.IsNullOrWhiteSpace(produto.CamposPersonalizados))
                return numeroLinha;

            try
            {
                // Validamos se o json é válido
                JToken.Parse(produto.CamposPersonalizados);

                Dominio.ObjetosDeValor.Embarcador.Integracao.Vedacit.CargaDadosTransporte.CampoPersonalizado campoPersonalizado = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Vedacit.CargaDadosTransporte.CampoPersonalizado>(produto.CamposPersonalizados);

                numeroLinha = campoPersonalizado.Item.NumeroLinha.ToString();
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);
            }

            return numeroLinha;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.RoutEasy.PedidoItem> ObterPedidoItens(List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto> produtos)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.RoutEasy.PedidoItem> itensPedido = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.RoutEasy.PedidoItem>();

            if (produtos.Count == 0)
                return itensPedido;

            foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoProduto pedidoProduto in produtos)
            {
                itensPedido.Add(new Dominio.ObjetosDeValor.Embarcador.Integracao.RoutEasy.PedidoItem
                {
                    Codigo = ObterCampoPersonalizadoProduto(pedidoProduto) + " | " + pedidoProduto.Produto.CodigoProdutoEmbarcador,
                    Descricao = pedidoProduto.Produto.Descricao,
                    Quantidade = pedidoProduto.Quantidade
                });
            }

            return itensPedido;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.RoutEasy.Endereco ObterEndereco(Dominio.Entidades.Cliente cliente, Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco endereco, bool usarOutroEndereco)
        {
            if (endereco != null)
            {
                string longitude = usarOutroEndereco ? endereco.ClienteOutroEndereco.Longitude : cliente.Longitude;
                string latitude = usarOutroEndereco ? endereco.ClienteOutroEndereco.Latitude : cliente.Latitude;

                return new Dominio.ObjetosDeValor.Embarcador.Integracao.RoutEasy.Endereco()
                {
                    Logradouro = endereco.Endereco,
                    Numero = endereco.Numero,
                    Bairro = endereco.Bairro,
                    Cep = endereco.CEP,
                    Localidade = endereco.Localidade.Descricao,
                    Estado = endereco.Localidade.Estado.Sigla,
                    Pais = endereco.Localidade.Pais?.Nome,
                    InformacoesAdicionais = string.Empty,
                    Geolocalizacao = new Dominio.ObjetosDeValor.Embarcador.Integracao.RoutEasy.Geolocalizacao()
                    {
                        Latitude = latitude,
                        Longitude = longitude
                    }
                };
            }

            return new Dominio.ObjetosDeValor.Embarcador.Integracao.RoutEasy.Endereco()
            {
                Logradouro = cliente.Endereco,
                Numero = cliente.Numero,
                Bairro = cliente.Bairro,
                Cep = cliente.CEP,
                Localidade = cliente.Localidade.Descricao,
                Estado = cliente.Localidade.Estado.Sigla,
                Pais = cliente.Localidade.Pais?.Nome,
                InformacoesAdicionais = string.Empty,
                Geolocalizacao = new Dominio.ObjetosDeValor.Embarcador.Integracao.RoutEasy.Geolocalizacao()
                {
                    Latitude = cliente.Latitude,
                    Longitude = cliente.Longitude
                }
            };
        }

        #endregion Métodos Privados - Integração de Pedidos

        #region Métodos Privados - Integração de Cancelamento de Pedidos

        private Dominio.ObjetosDeValor.Embarcador.Integracao.RoutEasy.IntegracaoPedidoCancelamento ObterIntegracaoPedidoCancelamento(List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.RoutEasy.IntegracaoPedidoCancelamento integracaoPedido = new Dominio.ObjetosDeValor.Embarcador.Integracao.RoutEasy.IntegracaoPedidoCancelamento()
            {
                Pedidos = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.RoutEasy.PedidoCancelamento>()
            };

            foreach (Dominio.Entidades.Embarcador.Pedidos.Pedido pedido in pedidos)
                integracaoPedido.Pedidos.Add(ObterPedidoCancelamentoIntegrar(pedido));

            return integracaoPedido;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.RoutEasy.PedidoCancelamento ObterPedidoCancelamentoIntegrar(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.RoutEasy.PedidoCancelamento pedidoIntegrar = new Dominio.ObjetosDeValor.Embarcador.Integracao.RoutEasy.PedidoCancelamento()
            {
                CodigoIntegracaoFilial = pedido.Filial.CodigoFilialEmbarcador,
                NumeroPedido = pedido.NumeroPedidoEmbarcador
            };

            return pedidoIntegrar;
        }

        #endregion Métodos Privados - Integração de Cancelamento de Pedidos
    }
}
