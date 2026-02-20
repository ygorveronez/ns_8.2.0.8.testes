using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Infrastructure.Services.HttpClientFactory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using Utilidades.Extensions;

namespace Servicos.Embarcador.Integracao.Cassol
{
    public class IntegracaoCassol
    {
        #region Atributos

        private readonly Repositorio.UnitOfWork _unitOfWork;
        private Dominio.Entidades.Embarcador.Configuracoes.IntegracaoCassol _configuracaoIntegracao;

        #endregion Atributos

        #region Construtores

        public IntegracaoCassol(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion Construtores

        #region Métodos Públicos

        public void IntegrarCargaDadosTransporte(Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaIntegracao)
        {
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao repositorioCargaDadosTransporteIntegracao = new Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao(_unitOfWork);

            cargaIntegracao.NumeroTentativas++;
            cargaIntegracao.DataIntegracao = DateTime.Now;

            string jsonRequest = string.Empty;
            string jsonResponse = string.Empty;

            try
            {
                ObterConfiguracaoIntegracaoCarga();

                string endPoint = $"{_configuracaoIntegracao.URLIntegracao}/tms/v1/multi/carga";

                if (!string.IsNullOrWhiteSpace(cargaIntegracao.Protocolo))
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Cassol.RequestIntegracaoDadosTransportador dadosRequisicao = PreencherRequisicaoCargaDadosTransporte(cargaIntegracao.Carga);
                    jsonRequest = JsonConvert.SerializeObject(new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Cassol.RequestIntegracaoDadosTransportador> { dadosRequisicao }, Formatting.Indented, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
                }
                else
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Cassol.RequestIntegracao dadosRequisicao = PreencherRequisicaoCargaDadosIntegracao(cargaIntegracao);
                    jsonRequest = JsonConvert.SerializeObject(new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Cassol.RequestIntegracao> { dadosRequisicao }, Formatting.Indented, new JsonSerializerSettings { DateFormatString = "yyyy-MM-ddTHH:mm:ss.fffZ" });
                }

                HttpClient requisicaoTransportador = CriarRequisicao(_configuracaoIntegracao.Token, endPoint);
                StringContent conteudoRequisicao = new StringContent(jsonRequest, System.Text.Encoding.UTF8, "application/json");

                HttpResponseMessage retornoRequisicao = requisicaoTransportador.PutAsync(endPoint, conteudoRequisicao).Result;
                jsonResponse = retornoRequisicao.Content.ReadAsStringAsync().Result;

                var objetoRetorno = jsonResponse.FromJson<dynamic>();

                if (RetornoSucesso(retornoRequisicao))
                {
                    cargaIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                    cargaIntegracao.ProblemaIntegracao = "Integrado com sucesso";
                    cargaIntegracao.Protocolo = cargaIntegracao.Carga.Codigo.ToString();
                }
                else if (retornoRequisicao.StatusCode == HttpStatusCode.NotFound)
                    throw new ServicoException("Requisição não encontrada");
                else if (retornoRequisicao.StatusCode == HttpStatusCode.InternalServerError)
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Cassol.ResponseErroInterno retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Cassol.ResponseErroInterno>(objetoRetorno);
                    throw new ServicoException($"Retorno integração: {retorno.Title}");
                }
                else
                    throw new ServicoException($"Problema ao integrar com Cassol: {objetoRetorno.error.mensagem}");
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
                cargaIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração com a Cassol";
            }

            servicoArquivoTransacao.Adicionar(cargaIntegracao, jsonRequest, jsonResponse, "json");
            repositorioCargaDadosTransporteIntegracao.Atualizar(cargaIntegracao);
        }

        public void IntegrarCancelamentoCarga(Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaIntegracao cargaCancelamentoCargaIntegracao)
        {
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCancelamentoCargaIntegracao repositorioCargaCancelamentoCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCancelamentoCargaIntegracao(_unitOfWork);

            cargaCancelamentoCargaIntegracao.NumeroTentativas++;
            cargaCancelamentoCargaIntegracao.DataIntegracao = DateTime.Now;

            string jsonRequest = string.Empty;
            string jsonResponse = string.Empty;

            try
            {
                ObterConfiguracaoIntegracaoCarga();

                string endPoint = $"{_configuracaoIntegracao.URLIntegracao}/tms/v1/multi/carga/cancelar";

                jsonRequest = JsonConvert.SerializeObject(new
                {
                    carga = cargaCancelamentoCargaIntegracao.CargaCancelamento.Carga.CodigoCargaEmbarcador.ObterSomenteNumeros().ToInt(),
                    idUsuario = 123,
                    motivoDoCancelamento = cargaCancelamentoCargaIntegracao.CargaCancelamento.MotivoCancelamento
                }, Formatting.Indented, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });

                HttpClient requisicaoTransportador = CriarRequisicao(_configuracaoIntegracao.Token, endPoint);
                StringContent conteudoRequisicao = new StringContent(jsonRequest, System.Text.Encoding.UTF8, "application/json");

                HttpResponseMessage retornoRequisicao = requisicaoTransportador.PutAsync(endPoint, conteudoRequisicao).Result;
                jsonResponse = retornoRequisicao.Content.ReadAsStringAsync().Result;

                if (RetornoSucesso(retornoRequisicao))
                {
                    cargaCancelamentoCargaIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                    cargaCancelamentoCargaIntegracao.ProblemaIntegracao = "Integrado com sucesso";
                    cargaCancelamentoCargaIntegracao.Protocolo = cargaCancelamentoCargaIntegracao.CargaCancelamento.Carga.CodigoCargaEmbarcador;
                }
                else if (retornoRequisicao.StatusCode == HttpStatusCode.NotFound)
                    throw new ServicoException("Requisição não encontrada");
                else
                    throw new ServicoException($"Problema ao integrar com Cassol: {retornoRequisicao.StatusCode}");
            }
            catch (ServicoException excecao)
            {
                cargaCancelamentoCargaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaCancelamentoCargaIntegracao.ProblemaIntegracao = excecao.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                cargaCancelamentoCargaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaCancelamentoCargaIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração com a Cassol";
            }

            servicoArquivoTransacao.Adicionar(cargaCancelamentoCargaIntegracao, jsonRequest, jsonResponse, "json");
            repositorioCargaCancelamentoCargaIntegracao.Atualizar(cargaCancelamentoCargaIntegracao);
        }

        #endregion Métodos Públicos

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Cassol.RequestIntegracao PreencherRequisicaoCargaDadosIntegracao(Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaIntegracao)
        {
            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repositorioCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(_unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoProduto repositorioPedidoProduto = new Repositorio.Embarcador.Pedidos.PedidoProduto(_unitOfWork);
            Repositorio.Embarcador.Pessoas.ClienteOutroEndereco repositorioClienteOutroEndereco = new Repositorio.Embarcador.Pessoas.ClienteOutroEndereco(_unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.Integracao.Cassol.RequestIntegracao objetoIntegracaoCassol = new Dominio.ObjetosDeValor.Embarcador.Integracao.Cassol.RequestIntegracao();
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.Cassol.RequestIntegracaoItensCarga> itensCarga = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Cassol.RequestIntegracaoItensCarga>();

            Dominio.Entidades.Embarcador.Cargas.Carga carga = cargaIntegracao.Carga;

            objetoIntegracaoCassol.NumeroCarga = Utilidades.String.OnlyNumbers(carga.CodigoCargaEmbarcador).ToInt();
            objetoIntegracaoCassol.CodigoFilial = Utilidades.String.OnlyNumbers(carga.Filial.CodigoFilialEmbarcador).ToInt();
            objetoIntegracaoCassol.DataCriacaoCarga = carga.DataCriacaoCarga;

            if (carga.CargaAgrupada)
            {
                List<Dominio.ObjetosDeValor.WebService.Carga.Carga> numeroCargasAgrupadas = repositorioCarga.BuscarNumeroCargasAgrupadas(carga.Codigo);
                objetoIntegracaoCassol.NumeroCargasAgrupadas = string.Join(", ", numeroCargasAgrupadas.Select(r => r.NumeroCarga));
            }

            if (carga.DataCarregamentoCarga.HasValue)
                objetoIntegracaoCassol.DataTerminoCarregamento = carga.DataCarregamentoCarga.Value;

            objetoIntegracaoCassol.TipoCarregamento = carga.TipoCarregamento.CodigoIntegracao;
            objetoIntegracaoCassol.BoxConferido = string.IsNullOrEmpty(cargaIntegracao.Protocolo) ? "N" : "S";
            objetoIntegracaoCassol.CodigoUsuarioMulti = 1111;
            objetoIntegracaoCassol.IntegradoMultiEmbarcador = "S";

            List<Dominio.Entidades.Embarcador.Pedidos.Pedido> listaPedidos = repositorioCargaPedido.BuscarPedidosPorCarga(carga.Codigo);
            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> listaCargasEntregas = repositorioCargaEntrega.BuscarPorPedidos(listaPedidos.Select(pedido => pedido.Codigo).ToList());
            List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto> listaPedidosProdutos = repositorioPedidoProduto.BuscarPorCarga(carga.Codigo);

            foreach (Dominio.Entidades.Embarcador.Pedidos.Pedido pedido in listaPedidos)
            {
                int codigoEndereco = 0;
                Dominio.Entidades.Cliente destinatario = pedido.ObterDestinatario();

                if (pedido.UsarOutroEnderecoDestino)
                {
                    Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEndereco endereco = repositorioClienteOutroEndereco.BuscarPorPessoa(destinatario.CPF_CNPJ).FirstOrDefault();

                    if (endereco != null)
                        codigoEndereco = Utilidades.String.OnlyNumbers(endereco.CodigoEmbarcador).ToInt();
                }

                List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto> pedidoProdutos = listaPedidosProdutos.FindAll(pedidoProduto => pedidoProduto.Pedido.Codigo == pedido.Codigo);
                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = listaCargasEntregas.Find(cargaEntrega => cargaEntrega.Pedidos.Any(p => p.Codigo == pedido.Codigo));

                foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoProduto pedidoProduto in pedidoProdutos)
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Cassol.RequestIntegracaoItensCarga itemCarga = new Dominio.ObjetosDeValor.Embarcador.Integracao.Cassol.RequestIntegracaoItensCarga();

                    itemCarga.NumeroCarga = objetoIntegracaoCassol.NumeroCarga;
                    itemCarga.CodigoFilial = objetoIntegracaoCassol.CodigoFilial;
                    itemCarga.NumeroPedido = Utilidades.String.OnlyNumbers(pedido.NumeroPedidoEmbarcador).ToLong();
                    itemCarga.TipoPedidoColeta = cargaEntrega?.Coleta != null ? "S" : "N";
                    itemCarga.EnderecoCodigoCliente = Utilidades.String.OnlyNumbers(destinatario?.CodigoIntegracao).ToInt();
                    itemCarga.CodigoEndereco = codigoEndereco;
                    itemCarga.QuantidadeItens = pedidoProduto.Quantidade.ToString("n2").ToDecimal();
                    itemCarga.CodigoProduto = Utilidades.String.OnlyNumbers(pedidoProduto.Produto.CodigoProdutoEmbarcador).ToInt();

                    itensCarga.Add(itemCarga);
                }
            }

            objetoIntegracaoCassol.ItensCarga = itensCarga;

            return objetoIntegracaoCassol;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Cassol.RequestIntegracaoDadosTransportador PreencherRequisicaoCargaDadosTransporte(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            Repositorio.Embarcador.Cargas.CargaMotorista repositorioCargaMotorista = new Repositorio.Embarcador.Cargas.CargaMotorista(_unitOfWork);
            Dominio.ObjetosDeValor.Embarcador.Integracao.Cassol.RequestIntegracaoDadosTransportador objetoIntegracaoCassolTransportador = new Dominio.ObjetosDeValor.Embarcador.Integracao.Cassol.RequestIntegracaoDadosTransportador();
            List<Dominio.Entidades.Embarcador.Cargas.CargaMotorista> cargaMotoristas = repositorioCargaMotorista.BuscarPorCarga(carga.Codigo);

            Dominio.Entidades.Embarcador.Cargas.CargaMotorista cargaMotorista = cargaMotoristas.FirstOrDefault();

            objetoIntegracaoCassolTransportador.NumeroCarga = Utilidades.String.OnlyNumbers(carga.CodigoCargaEmbarcador).ToInt();
            objetoIntegracaoCassolTransportador.BoxConferido = "S";
            objetoIntegracaoCassolTransportador.CodigoMotorista = Utilidades.String.OnlyNumbers(cargaMotorista?.Motorista.CodigoIntegracao).ToLong();
            objetoIntegracaoCassolTransportador.CodigoVeiculo = Utilidades.String.OnlyNumbers(carga.Veiculo?.CodigoIntegracao).ToInt();

            return objetoIntegracaoCassolTransportador;
        }

        private void ObterConfiguracaoIntegracaoCarga()
        {
            if (_configuracaoIntegracao != null)
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoCassol repositorioConfiguracaoIntegracaoCassol = new Repositorio.Embarcador.Configuracoes.IntegracaoCassol(_unitOfWork);
            _configuracaoIntegracao = repositorioConfiguracaoIntegracaoCassol.BuscarPrimeiroRegistro();

            if (_configuracaoIntegracao == null || !_configuracaoIntegracao.PossuiIntegracao)
                throw new ServicoException("Não existe configuração de integração disponível para a Cassol.");
        }

        private HttpClient CriarRequisicao(string token, string endpoint)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            HttpClient requisicao = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoCassol));

            requisicao.BaseAddress = new Uri(endpoint);
            requisicao.DefaultRequestHeaders.Accept.Clear();
            requisicao.DefaultRequestHeaders.Add("Content", "application/json");
            requisicao.DefaultRequestHeaders.Add("authorization", $"Bearer {token}");

            return requisicao;
        }

        private bool RetornoSucesso(HttpResponseMessage retornoRequisicao)
        {
            return (retornoRequisicao.StatusCode == HttpStatusCode.OK) || (retornoRequisicao.StatusCode == HttpStatusCode.Created);
        }

        #endregion Métodos Privados
    }
}
