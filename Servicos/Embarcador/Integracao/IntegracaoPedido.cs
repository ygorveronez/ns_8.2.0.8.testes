using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Integracao
{
    public static class IntegracaoPedido
    {
        public static void GerarIntegracaoPedidoCancelamentoReserva(Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao, Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto> produtos, Dominio.Entidades.Usuario usuario, Repositorio.UnitOfWork unitOfWork)
        {
            if (pedido == null)
                return;
            SetarPedidoCancelamentoReservaIntegracao(tipoIntegracao, pedido, produtos, usuario, unitOfWork);
        }

        private static bool ValidarSePodeGerarIntegracao(Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            if (tipoIntegracao != null)
            {
                if (tipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Digibee)
                {
                    Repositorio.Embarcador.Configuracoes.Integracao repIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
                    Dominio.Entidades.Embarcador.Configuracoes.Integracao integracao = repIntegracao.Buscar();
                    if (string.IsNullOrWhiteSpace(integracao?.URLIntegracaoCancelamentoDigibee))
                        return false;
                    else
                        return true;
                }
                else
                    return true;

            }
            else return false;
        }

        private static void SetarPedidoCancelamentoReservaIntegracao(Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao, Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto> produtos, Dominio.Entidades.Usuario usuario, Repositorio.UnitOfWork unitOfWork)
        {
            if (ValidarSePodeGerarIntegracao(tipoIntegracao, unitOfWork))
            {
                Dominio.Entidades.Embarcador.Cargas.MontagemCarga.PedidoCancelamentoReservaIntegracao pedidoCancelamentoReservaIntegracao = new Dominio.Entidades.Embarcador.Cargas.MontagemCarga.PedidoCancelamentoReservaIntegracao()
                {
                    DataIntegracao = DateTime.Now,
                    DataCancelamento = DateTime.Now,
                    ProblemaIntegracao = "",
                    SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao,
                    TipoIntegracao = tipoIntegracao,
                    Pedido = pedido,
                    Usuario = usuario
                };
                List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.PedidoProdutoCancelamentoReservaIntegracao> produtosCancelamento = new List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.PedidoProdutoCancelamentoReservaIntegracao>();
                if (produtos != null)
                {
                    foreach (var prod in produtos)
                    {
                        produtosCancelamento.Add(new Dominio.Entidades.Embarcador.Cargas.MontagemCarga.PedidoProdutoCancelamentoReservaIntegracao()
                        {
                            MetroCubico = prod.MetroCubico,
                            Peso = prod.Quantidade * prod.PesoUnitario + prod.PesoTotalEmbalagem,
                            Quantidade = prod.Quantidade,
                            QuantidadePallet = prod.QuantidadePalet,
                            PedidoProduto = prod
                        });
                    }
                }
                InseriNovaIntegracao(pedidoCancelamentoReservaIntegracao, produtosCancelamento, unitOfWork);
            }
        }

        private static void InseriNovaIntegracao(Dominio.Entidades.Embarcador.Cargas.MontagemCarga.PedidoCancelamentoReservaIntegracao pedidoCancelamentoReservaIntegracao, List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.PedidoProdutoCancelamentoReservaIntegracao> produtosCancelamento, Repositorio.UnitOfWork unitOfWork)
        {
            if (pedidoCancelamentoReservaIntegracao == null)
                return;

            Repositorio.Embarcador.Cargas.MontagemCarga.PedidoCancelamentoReservaIntegracao repPedidoIntegracao = new Repositorio.Embarcador.Cargas.MontagemCarga.PedidoCancelamentoReservaIntegracao(unitOfWork);
            Repositorio.Embarcador.Cargas.MontagemCarga.PedidoProdutoCancelamentoReservaIntegracao repPedidoProdutoIntegracao = new Repositorio.Embarcador.Cargas.MontagemCarga.PedidoProdutoCancelamentoReservaIntegracao(unitOfWork);

            repPedidoIntegracao.Inserir(pedidoCancelamentoReservaIntegracao);
            foreach (var reg in produtosCancelamento)
            {
                reg.PedidoCancelamentoReservaIntegracao = pedidoCancelamentoReservaIntegracao;
                repPedidoProdutoIntegracao.Inserir(reg);
            }
        }

        public static void ProcessarIntegracaoPedidoCancelamentoReserva(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.MontagemCarga.PedidoCancelamentoReservaIntegracao repPedidoCancReserva = new Repositorio.Embarcador.Cargas.MontagemCarga.PedidoCancelamentoReservaIntegracao(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.PedidoCancelamentoReservaIntegracao> pedidosCancelamentos = repPedidoCancReserva.BuscarCarregamentoIntegracaoPendente(3, 5, "Codigo", "asc", 20);

            foreach (var pedidoIntegracao in pedidosCancelamentos)
            {
                switch (pedidoIntegracao.TipoIntegracao.Tipo)
                {
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Digibee:
                        Servicos.Embarcador.Integracao.Digibee.IntegracaoDigibee.IntegrarPedidoCancelamentoReserva(pedidoIntegracao, unitOfWork);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.TelhaNorte:
                        new TelhaNorte.IntegracaoTelhaNorte(unitOfWork).IntegrarPedidoCancelamentoReserva(pedidoIntegracao);
                        break;
                    default:
                        pedidoIntegracao.NumeroTentativas++;
                        pedidoIntegracao.DataIntegracao = DateTime.Now;
                        pedidoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                        pedidoIntegracao.ProblemaIntegracao = "Tipo de integração não implementado para integração de carregamentos.";
                        repPedidoCancReserva.Atualizar(pedidoIntegracao);
                        break;
                }
            }
        }

        public static void ReenviarIntegracaoPedidos(int pedido, Repositorio.UnitOfWork unitOfWork)
        {
            if (pedido == 0)
                return;

            ReenviarIntegracaoPedidos(new List<int>() { pedido }, unitOfWork);
        }

        public static void ReenviarIntegracaoPedidos(List<int> pedidos, Repositorio.UnitOfWork unitOfWork)
        {
            if (pedidos == null || pedidos.Count == 0)
                return;

            if (!(new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork).ExistePorTipo(TipoIntegracao.A52))) 
                return;

            const int maxParameters = 2000;
            Repositorio.Embarcador.Pedidos.PedidoIntegracao repPedidoIntegracao = new Repositorio.Embarcador.Pedidos.PedidoIntegracao(unitOfWork);

            for (int i = 0; i < pedidos.Count; i += maxParameters)
            {
                var subListaPedidos = pedidos.Skip(i).Take(maxParameters).ToList();
                ProcessarPedidos(subListaPedidos, repPedidoIntegracao);
            }
        }

        private static void ProcessarPedidos(List<int> subListaPedidos, Repositorio.Embarcador.Pedidos.PedidoIntegracao repPedidoIntegracao)
        {
            List<Dominio.Entidades.Embarcador.Pedidos.PedidoIntegracao> listaPedidoIntegracao = repPedidoIntegracao.BuscarPorPedidos(subListaPedidos);

            if (listaPedidoIntegracao == null || listaPedidoIntegracao.Count == 0)
                return;

            foreach (var pedidoIntegracao in listaPedidoIntegracao)
            {
                if (pedidoIntegracao.TipoIntegracao.Tipo != TipoIntegracao.A52)
                    continue;

                bool integrarPedidoA52 = (pedidoIntegracao.Pedido?.TipoOperacao?.IntegrarPedidoA52 ?? false) ||
                                         (pedidoIntegracao.Pedido?.CargasPedido?.Any(obj => obj.TipoOperacao?.IntegrarPedidoA52 ?? false) ?? false);

                if (!integrarPedidoA52)
                    continue;

                pedidoIntegracao.SituacaoIntegracao = SituacaoIntegracao.AgIntegracao;
                pedidoIntegracao.ProblemaIntegracao = string.Empty;

                repPedidoIntegracao.Atualizar(pedidoIntegracao);
            }
        }
    }
}
