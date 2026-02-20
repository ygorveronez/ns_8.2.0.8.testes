using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Carga.MontagemCarga
{
    public class SaldoPedidoProduto
    {
        #region  Métodos Públicos 

        public static bool ApresentarSaldoProdutoGridPedido(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao)
        {
            bool retornarSaldoProduto = false;
            if (pedido != null)
            {
                retornarSaldoProduto = pedido.QuebraMultiplosCarregamentos;
                if (tipoOperacao == null) tipoOperacao = pedido.TipoOperacao;
            }
            if (!retornarSaldoProduto && tipoOperacao != null)
                retornarSaldoProduto = (tipoOperacao?.SelecionarRetiradaProduto ?? false);
            return retornarSaldoProduto;
        }

        public static List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.SaldoPedidoProduto> ObterSaldoPedidoProdutos(int codigoSessaoRoteirizador, long cliente, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.PedidoProduto rep = new Repositorio.Embarcador.Pedidos.PedidoProduto(unitOfWork);

            List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto> produtos = new List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto>();
            //Contem todos os produtos que não estão totalmente carregados.
            if (codigoSessaoRoteirizador > 0)
            {
                var sessao = new Repositorio.Embarcador.Cargas.MontagemCarga.SessaoRoteirizador(unitOfWork).BuscarPorCodigo(codigoSessaoRoteirizador, false);
                produtos = rep.ProdutosNaoAtendidos(sessao.Codigo, sessao.Filial.Codigo, cliente);
            }

            List<int> codigos = (from p in produtos select p.Codigo).ToList();

            Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto repCarrPedidoProduto = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto(unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto> carregamentosPedidoProduto = repCarrPedidoProduto.BuscarPorPedidoProdutos(codigos);

            return Retornar(produtos, carregamentosPedidoProduto);
        }

        public static List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.SaldoPedidoProduto> ObterSaldoPedidoProdutos(int codigoPedido, Repositorio.UnitOfWork unitOfWork)
        {
            return ObterSaldoPedidoProdutos(new List<int> { codigoPedido }, false, unitOfWork, null, null);
        }

        public static List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.SaldoPedidoProduto> ObterSaldoPedidoProdutos(List<int> codigosPedidos, Repositorio.UnitOfWork unitOfWork)
        {
            return ObterSaldoPedidoProdutos(codigosPedidos, true, unitOfWork, null, null);
        }

        public static List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.SaldoPedidoProduto> ObterSaldoPedidoProdutos(List<int> codigosPedidos, List<int> codigosProdutos, List<int> codigosLinhasSeparacao, Repositorio.UnitOfWork unitOfWork)
        {
            return ObterSaldoPedidoProdutos(codigosPedidos, true, unitOfWork, codigosProdutos, codigosLinhasSeparacao);
        }

        public static List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.SaldoPedidoProduto> ObterSaldoPedidoProdutos(List<int> codigosPedidos, bool somenteProdutosComSaldo, Repositorio.UnitOfWork unitOfWork, List<int> codigosProdutos, List<int> codigosLinhasSeparacao)
        {
            if (codigosProdutos == null)
                codigosProdutos = new List<int>();

            if (codigosLinhasSeparacao == null)
                codigosLinhasSeparacao = new List<int>();

            Repositorio.Embarcador.Pedidos.PedidoProduto repositorioPedidoProduto = new Repositorio.Embarcador.Pedidos.PedidoProduto(unitOfWork);

            List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto> produtos = new List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto>();
            //Contem todos os produtos que não estão totalmente carregados.
            if (codigosPedidos.Count > 0)
            {
                if (somenteProdutosComSaldo)
                    produtos = repositorioPedidoProduto.ProdutosNaoAtendidos(codigosPedidos, codigosProdutos, codigosLinhasSeparacao);
                else
                    produtos = repositorioPedidoProduto.BuscarPorPedidosEProdutos(codigosPedidos, codigosProdutos, codigosLinhasSeparacao);
            }

            List<int> codigos = (from p in produtos select p.Codigo).ToList();

            Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto repCarrPedidoProduto = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto(unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto> carregamentosPedidoProduto = repCarrPedidoProduto.BuscarPorPedidoProdutos(codigos);

            return Retornar(produtos, carregamentosPedidoProduto);
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.SaldoPedidoProduto> ObterSaldoPedidoProdutosComSaldo(List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto> carregamentosPedidoProduto)
        {            
          var produtos = ProdutosNaoAtendidos(carregamentosPedidoProduto);            

            return Retornar(produtos, carregamentosPedidoProduto);
        }

        #endregion Métodos Públicos

        #region Métodos Privados


        private List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto> ProdutosNaoAtendidos(List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto> carregamentoPedidoProduto)
        {
            var pesoPorCodigo = carregamentoPedidoProduto
                .GroupBy(p => p.PedidoProduto.Codigo)
                .ToDictionary(g => g.Key, g => g.Sum(p => p.Peso));

            var produtosNaoAtendidos = new List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto>();

            foreach (var item in carregamentoPedidoProduto)
            {
                Dominio.Entidades.Embarcador.Pedidos.PedidoProduto produto = item.PedidoProduto;

                if (produtosNaoAtendidos.Any(p => p.Codigo == produto.Codigo))
                    continue;

                decimal pesoNecessario = produto.Quantidade * produto.PesoUnitario + produto.PesoTotalEmbalagem;

                decimal pesoCarregado = pesoPorCodigo.ContainsKey(produto.Codigo) ? pesoPorCodigo[produto.Codigo] : 0;

                if (pesoCarregado < pesoNecessario)
                {
                    produtosNaoAtendidos.Add(produto);
                }
            }

            return produtosNaoAtendidos;
        }

        private static List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.SaldoPedidoProduto> Retornar(List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto> produtos, List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto> carregamentosPedidoProduto)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.SaldoPedidoProduto> lista = (
                    from p in produtos
                    select new Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.SaldoPedidoProduto()
                    {
                        CodigoPedido = p.Pedido.Codigo,
                        CodigoPedidoProduto = p.Codigo,
                        PalletFechado = p.PalletFechado,
                        Cliente = (p.Pedido?.Destinatario?.CodigoIntegracao?.Length <= 0 ? p.Pedido?.Destinatario?.Nome : p.Pedido?.Destinatario?.CodigoIntegracao),
                        CodigoProduto = p.Produto.Codigo,
                        CodigoProdutoEmbarcador = p.Produto?.CodigoProdutoEmbarcador ?? string.Empty,
                        Produto = p.Produto.Descricao,
                        Categoria = p.Pedido?.CanalEntrega?.Descricao ?? string.Empty,
                        LinhaSeparacao = p?.LinhaSeparacao?.Descricao ?? string.Empty,
                        GrupoProduto = p?.Produto?.GrupoProduto?.Descricao ?? string.Empty,
                        NumeroPedidoEmbarcador = p.Pedido.NumeroPedidoEmbarcador,
                        Qtde = p.Quantidade,
                        QtdeCarregado = (from r in carregamentosPedidoProduto
                                         where r.PedidoProduto.Codigo == p.Codigo
                                         select r.Quantidade).Sum(),
                        Peso = p.PesoTotal,
                        PesoCarregado = (from r in carregamentosPedidoProduto
                                         where r.PedidoProduto.Codigo == p.Codigo
                                         select r.Peso).Sum(),
                        Pallet = p.QuantidadePalet,
                        SaldoPallet = (p.QuantidadePalet - (from r in carregamentosPedidoProduto
                                                            where r.PedidoProduto.Codigo == p.Codigo
                                                            select r.QuantidadePallet).Sum()),
                        Metro = p.MetroCubico,
                        SaldoMetro = (p.MetroCubico - (from r in carregamentosPedidoProduto
                                                       where r.PedidoProduto.Codigo == p.Codigo
                                                       select r.MetroCubico).Sum()),
                        IdDemanda = p.IdDemanda,
                        ObservacaoProduto = p.Observacao,
                        TipoEmbalagem = p.Produto.TipoEmbalagem?.CodigoIntegracao
                    }).ToList();

            return lista;
        }

        #endregion Métodos Privados
    }
}
