using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;


namespace Repositorio.Embarcador.Pedidos
{
    public class XMLNotaFiscalProduto : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalProduto>
    {
        public XMLNotaFiscalProduto(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public XMLNotaFiscalProduto(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken)
        {
        }

        #region Métodos Públicos

        public bool VerificarExistePorPedido(int pedido, int nota)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalProduto>();
            var result = from obj in query where obj.XMLNotaFiscal.Codigo == nota && (obj.Pedido.Codigo == pedido || obj.Pedido == null) select obj;
            return result.Any();
        }

        public Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalProduto BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalProduto>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalProduto> BuscarPorCTe(int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalProduto>();
            var result = (from obj in query where obj.XMLNotaFiscal.CTEs.Any(o => o.Codigo == codigoCTe) select obj);

            return result
                .Fetch(o => o.XMLNotaFiscal)
                .Fetch(o => o.Produto)
                .OrderBy(o => o.XMLNotaFiscal.Numero)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalProduto> BuscarPorNotaFiscal(int codigoXMLNotaFiscal, bool contemProdutoInterno)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalProduto>();
            var result = from obj in query where obj.XMLNotaFiscal.Codigo == codigoXMLNotaFiscal select obj;
            if (contemProdutoInterno)
                result = result.Where(o => o.ProdutoInterno != null);
            return result.ToList();
        }

        public (string CodigoNCM, string Descricao) BuscarNCMProdutoMaiorValorPorCarga(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> subQueryPedidoXMLNotaFiscal =
                SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>()
                                 .Where(obj => obj.CargaPedido.Carga.Codigo == codigoCarga);

            IQueryable<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalProduto> query =
                SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalProduto>()
                                 .Where(obj => subQueryPedidoXMLNotaFiscal.Select(o => o.XMLNotaFiscal).Contains(obj.XMLNotaFiscal));

            return query
                .OrderByDescending(o => o.ValorProduto)
                .WithOptions(o => o.SetTimeout(600))
                .Select(x => ValueTuple.Create(
                    x.NCM,
                    x.Produto.Descricao
                ))?.FirstOrDefault() ?? (string.Empty, string.Empty);
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalProduto> BuscarPorNotaFiscais(List<int> codigoXMLNotaFiscal)
        {
            //não usar, pode ter mais que 2100 notas e não aceitar o parametro
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalProduto>();
            var result = from obj in query where codigoXMLNotaFiscal.Contains(obj.XMLNotaFiscal.Codigo) select obj;
            return result
                .Fetch(obj => obj.Produto)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalProduto> BuscarPorNotaFiscais(int codigoCarga)
        {
            return this.BuscarPorNotaFiscais(codigoCarga, 0);
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalProduto> BuscarPorNotaFiscais(int codigoCarga, int codigoPedido)
        {
            var queryCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            queryCargaPedido = queryCargaPedido.Where(o => o.Carga.Codigo == codigoCarga);

            var queryPedidoXMLNotaFiscal = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();
            queryPedidoXMLNotaFiscal = queryPedidoXMLNotaFiscal.Where(o => queryCargaPedido.Any(c => c == o.CargaPedido));

            var queryXMLNotaFiscal = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>();
            queryXMLNotaFiscal = queryXMLNotaFiscal.Where(o => queryPedidoXMLNotaFiscal.Any(c => c.XMLNotaFiscal == o));

            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalProduto>();

            if (codigoPedido > 0)
                query = query.Where(x => x.Pedido.Codigo == codigoPedido);

            query = query.Where(o => queryXMLNotaFiscal.Any(c => c == o.XMLNotaFiscal));

            return query
                .Fetch(obj => obj.Produto)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalProduto> BuscarPorCargaEPedido(int codigoCarga, int codigoPedido)
        {
            List<int> codigosXMLNotaFiscal = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>()
                .Where(obj => obj.CargaPedido.Pedido.Codigo == codigoPedido && obj.CargaPedido.Carga.Codigo == codigoCarga)
                .Select(obj => obj.XMLNotaFiscal.Codigo)
                .ToList();

            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalProduto>()
                .Where(obj => codigosXMLNotaFiscal.Contains(obj.XMLNotaFiscal.Codigo));

            return query
                .Fetch(obj => obj.Produto)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalProduto> BuscarPorCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> subQueryPedidoXMLNotaFiscal =
                SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>()
                                 .Where(obj => obj.CargaPedido.Carga == carga);

            IQueryable<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalProduto> query =
                SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalProduto>()
                                 .Where(obj => subQueryPedidoXMLNotaFiscal.Select(o => o.XMLNotaFiscal).Contains(obj.XMLNotaFiscal));

            return query.Fetch(o => o.Produto)
                        .Fetch(o => o.XMLNotaFiscal)
                        .WithOptions(o => o.SetTimeout(600))
                        .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalProduto> BuscarPorNotaFiscal(int codigoXMLNotaFiscal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalProduto>();
            var result = from obj in query where obj.XMLNotaFiscal.Codigo == codigoXMLNotaFiscal select obj;
            return result
                .Fetch(obj => obj.Produto)
                .ToList();
        }

        public Task<List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalProduto>> BuscarPorNotaFiscalAsync(int codigoXMLNotaFiscal, CancellationToken cancellationToken)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalProduto>();
            var result = from obj in query where obj.XMLNotaFiscal.Codigo == codigoXMLNotaFiscal select obj;
            return result
                .Fetch(obj => obj.Produto)
                .ToListAsync(cancellationToken);
        }

        public Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador BuscarPrimeiroProdutoPorNotaFiscal(int codigoXMLNotaFiscal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalProduto>();

            var result = query.Where(obj => obj.XMLNotaFiscal.Codigo == codigoXMLNotaFiscal).Select(obj => obj.Produto);

            return result.FirstOrDefault();
        }

        public decimal? BuscarQuantidadePorNotaFiscal(int codigoXMLNotaFiscal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalProduto>();

            var result = from obj in query where obj.XMLNotaFiscal.Codigo == codigoXMLNotaFiscal select obj;

            return result.Select(o => (decimal?)o.Quantidade).Sum();
        }

        public void ExcluirTodosPorXMLNotaFiscal(int codigoXMLNotaFiscal)
        {
            UnitOfWork.Sessao.CreateQuery("DELETE XMLNotaFiscalProduto obj where obj.XMLNotaFiscal.Codigo =:XMLNotaFiscal")
                             .SetInt32("XMLNotaFiscal", codigoXMLNotaFiscal)
                             .ExecuteUpdate();
        }

        public Task ExcluirTodosPorXMLNotaFiscalAsync(int codigoXMLNotaFiscal)
        {
            return UnitOfWork.Sessao.CreateQuery("DELETE XMLNotaFiscalProduto obj where obj.XMLNotaFiscal.Codigo =:XMLNotaFiscal")
                             .SetInt32("XMLNotaFiscal", codigoXMLNotaFiscal)
                             .ExecuteUpdateAsync();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalProduto> BuscarPorNotaFiscal(List<int> codigosXMLNotaFiscal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalProduto>()
                .Where(o => codigosXMLNotaFiscal.Contains(o.XMLNotaFiscal.Codigo));

            return query.ToList();
        }

        public string ObterSiglaUnidadeMediaPorNota(int codigoNota)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalProduto>();

            query = from obj in query where obj.XMLNotaFiscal.Codigo == codigoNota select obj;

            return query.Select(n => n.UnidadeMedida).FirstOrDefault();
        }

        public List<Dominio.ObjetosDeValor.Embarcador.GestaoPallet.ProdutoQuantidadePallet> BuscarProdutoEQuantidadePorNotaFiscal(int codigoXMLNotaFiscal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalProduto>();

            var result = from obj in query where obj.XMLNotaFiscal.Codigo == codigoXMLNotaFiscal select obj;

            return result.Select(o => new Dominio.ObjetosDeValor.Embarcador.GestaoPallet.ProdutoQuantidadePallet()
            {
                Produto = o.Produto,
                Quantidade = o.Quantidade
            })
            .ToList();
        }

        #endregion
    }
}
