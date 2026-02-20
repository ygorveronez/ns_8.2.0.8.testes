using Dominio.Excecoes.Embarcador;
using Infrastructure.Services.HttpClientFactory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Servicos.Embarcador.Integracao.Simonetti
{
    public class IntegracaoSimonetti
    {
        #region Variaveis Privadas

        readonly private Repositorio.UnitOfWork _unitOfWork;

        #endregion

        #region Constructores

        public IntegracaoSimonetti(Repositorio.UnitOfWork unitOfWork) : base() { _unitOfWork = unitOfWork; }

        #endregion

        #region Metodos de Integração

        public Dominio.ObjetosDeValor.Embarcador.Integracao.HttpRequisicaoResposta IntegrarOcorrenciasColetaEntrega(List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> notasFiscais, Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntregaIntegracao integracao)
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

            if (notasFiscais == null)
            {
                httpRequisicaoResposta.mensagem = "Nenhuma NFe localizada.";
                return httpRequisicaoResposta;
            }

            Repositorio.Embarcador.Configuracoes.IntegracaoSimonetti repIntegracaoSimonetti = new Repositorio.Embarcador.Configuracoes.IntegracaoSimonetti(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSimonetti dadosIntegracao = repIntegracaoSimonetti.BuscarDadosIntegracao();

            if (dadosIntegracao == null || !dadosIntegracao.PossuiIntegracaoSimonetti)
            {
                httpRequisicaoResposta.mensagem = "Não possui Integração com Simonetti ou está desativada.";
                return httpRequisicaoResposta;
            }

            string jsonRequest = string.Empty;
            string jsonResponse = string.Empty;

            try
            {
                HttpClient cliente = this.ObterCliente(dadosIntegracao.URLEnviaOcorrenciaSimonetti);
                jsonRequest = ObterObjetoRequisicao(notasFiscais, integracao);

                StringContent content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
                var result = cliente.PostAsync(dadosIntegracao.URLEnviaOcorrenciaSimonetti, content).Result;

                jsonResponse = result.Content.ReadAsStringAsync().Result;

                httpRequisicaoResposta.conteudoRequisicao = jsonRequest;
                httpRequisicaoResposta.conteudoResposta = jsonResponse;
                httpRequisicaoResposta.httpStatusCode = result.StatusCode;

                if (result.StatusCode != System.Net.HttpStatusCode.Accepted)
                    throw new ServicoException("Ocorreu uma falha ao tentar fazer a integração");

                httpRequisicaoResposta.sucesso = true;
            }
            catch (ServicoException excecao)
            {
                Log.TratarErro("Request: " + jsonRequest, "IntegracaoSimonetti");
                Log.TratarErro("Response: " + jsonResponse, "IntegracaoSimonetti");
                Log.TratarErro(excecao, "IntegracaoSimonetti");

                httpRequisicaoResposta.mensagem = excecao.Message;
            }

            return httpRequisicaoResposta;
        }

        #endregion

        #region Metodos Privados

        private HttpClient ObterCliente(string url)
        {
            HttpClient cliente = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoSimonetti));
            cliente.BaseAddress = new Uri(url);
            cliente.DefaultRequestHeaders.Accept.Clear();
            cliente.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return cliente;
        }

        private string ObterObjetoRequisicao(List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> notas, Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntregaIntegracao integracao)
        {

            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido repositorioCargaEntregaPedido = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido> cargaEntregaPedidos = repositorioCargaEntregaPedido.BuscarPorCarga(integracao.PedidoOcorrenciaColetaEntrega?.Carga?.Codigo ?? 0) ;
            
            Dominio.ObjetosDeValor.Embarcador.Integracao.Simonetti.RootRequest request = new Dominio.ObjetosDeValor.Embarcador.Integracao.Simonetti.RootRequest()
            {
                UUID = Guid.NewGuid().ToString(),
                OrigemNome = "multisoftware.pedido.evento_protocolo",
                OrigemId = integracao.PedidoOcorrenciaColetaEntrega?.Pedido?.Protocolo.ToString() ?? string.Empty,
                Payload = new Dominio.ObjetosDeValor.Embarcador.Integracao.Simonetti.Payload()
                {
                    ProtocoloPedido = integracao.PedidoOcorrenciaColetaEntrega?.Pedido?.Protocolo.ToString() ?? string.Empty,
                    Itens = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Simonetti.Item>()
                }
            };

            foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoProduto pedidoProduto in integracao.PedidoOcorrenciaColetaEntrega.Pedido.Produtos)
            {
                request.Payload.Itens.Add(new Dominio.ObjetosDeValor.Embarcador.Integracao.Simonetti.Item()
                {
                    Observacao = cargaEntregaPedidos.Where(a => a.CargaPedido.Pedido.Codigo == pedidoProduto.Pedido.Codigo).Select(o => o.CargaEntrega?.Observacao ?? string.Empty).FirstOrDefault(),
                    DataHoraAgendamento = string.Empty,
                    CodigoProduto = pedidoProduto.Produto?.CodigoProdutoEmbarcador ?? string.Empty,
                    CodigoOcorrencia = integracao.PedidoOcorrenciaColetaEntrega.TipoDeOcorrencia?.CodigoIntegracao ?? string.Empty,
                    DescricaoOcorrencia = integracao.PedidoOcorrenciaColetaEntrega.TipoDeOcorrencia?.Descricao ?? string.Empty,

                    PlacaVeiculo1 = integracao.PedidoOcorrenciaColetaEntrega?.Carga?.Veiculo?.Placa ?? string.Empty,
                    CPFMotorista1 = integracao.PedidoOcorrenciaColetaEntrega.Carga?.Motoristas.ElementAtOrDefault(0) != null ? integracao.PedidoOcorrenciaColetaEntrega.Carga?.Motoristas?.ElementAtOrDefault(0).CPF_Formatado ?? string.Empty : string.Empty,

                    PlacaVeiculo2 = integracao.PedidoOcorrenciaColetaEntrega?.Carga?.VeiculosVinculados.ElementAtOrDefault(0) != null ? integracao.PedidoOcorrenciaColetaEntrega?.Carga?.VeiculosVinculados.ElementAtOrDefault(0).Placa ?? string.Empty : string.Empty,
                    CPFMotorista2 = integracao.PedidoOcorrenciaColetaEntrega.Carga?.Motoristas.ElementAtOrDefault(1) != null ? integracao.PedidoOcorrenciaColetaEntrega.Carga?.Motoristas?.ElementAtOrDefault(1).CPF_Formatado ?? string.Empty : string.Empty,

                    PlacaVeiculo3 = integracao.PedidoOcorrenciaColetaEntrega?.Carga?.VeiculosVinculados.ElementAtOrDefault(1) != null ? integracao.PedidoOcorrenciaColetaEntrega?.Carga?.VeiculosVinculados.ElementAtOrDefault(1).Placa ?? string.Empty : string.Empty,
                    CPFMotorista3 = integracao.PedidoOcorrenciaColetaEntrega.Carga?.Motoristas.ElementAtOrDefault(2) != null ? integracao.PedidoOcorrenciaColetaEntrega.Carga?.Motoristas?.ElementAtOrDefault(2).CPF_Formatado ?? string.Empty : string.Empty,

                    PlacaVeiculo4 = integracao.PedidoOcorrenciaColetaEntrega?.Carga?.VeiculosVinculados.ElementAtOrDefault(2) != null ? integracao.PedidoOcorrenciaColetaEntrega?.Carga?.VeiculosVinculados.ElementAtOrDefault(2).Placa ?? string.Empty : string.Empty,
                    CPFMotorista4 = integracao.PedidoOcorrenciaColetaEntrega.Carga?.Motoristas.ElementAtOrDefault(3) != null ? integracao.PedidoOcorrenciaColetaEntrega.Carga?.Motoristas?.ElementAtOrDefault(3).CPF_Formatado ?? string.Empty : string.Empty,
                });
            }

            return JsonConvert.SerializeObject(request, Formatting.Indented);
        }
        
        #endregion

    }
}