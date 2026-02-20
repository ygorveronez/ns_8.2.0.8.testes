using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Cargas.ControleEntrega
{
    public class CargaEntregaProduto : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaProduto>
    {
        public CargaEntregaProduto(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaProduto BuscarPorCodigo(int codigo)
        {
            return ObterQueryBuscarPorCodigos([codigo]).FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaProduto> BuscarPorCodigos(IEnumerable<int> codigos, int codigoCargaEntregaNotaFiscal)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaProduto> query = ObterQueryBuscarPorCodigos(codigos);
            IQueryable<int> codigosXMLNotasFiscais = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal>()
                .Where(cargaEntregaNotaFiscal => cargaEntregaNotaFiscal.Codigo == codigoCargaEntregaNotaFiscal)
                .Select(cargaEntregaNotaFiscal => cargaEntregaNotaFiscal.PedidoXMLNotaFiscal.XMLNotaFiscal.Codigo);

            query = query.Where(cargaEntregaProduto => codigosXMLNotasFiscais.Contains(cargaEntregaProduto.XMLNotaFiscal.Codigo));

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaProduto> BuscarPorCargaEntregaENotaFiscal(int codigoCargaEntrega, int codigoNotaFiscal)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaProduto>();
            var result = query.Where(obj => obj.CargaEntrega.Codigo == codigoCargaEntrega);

            var queryPedidoXMLNotaFiscal = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();
            result = result.Where(o => queryPedidoXMLNotaFiscal.Where(p => p.XMLNotaFiscal.Codigo == codigoNotaFiscal && p.CargaPedido.Carga.Codigo == o.CargaEntrega.Carga.Codigo).Any());

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaProduto> BuscarPorCargaEntregaENotaFiscalProdutosDevolvidos(int codigoCargaEntrega, int codigoNotaFiscal)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaProduto>();
            var result = query.Where(obj => obj.CargaEntrega.Codigo == codigoCargaEntrega && obj.QuantidadeDevolucao > 0);

            var queryPedidoXMLNotaFiscal = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();
            result = result.Where(o => queryPedidoXMLNotaFiscal.Where(p => p.XMLNotaFiscal.Codigo == codigoNotaFiscal && p.CargaPedido.Carga.Codigo == o.CargaEntrega.Carga.Codigo).Any());

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaProduto> BuscarPorCarga(int codigoCarga)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaProduto>();
            var result = query.Where(obj => obj.CargaEntrega.Carga.Codigo == codigoCarga);

            return result
                .Fetch(obj => obj.Produto)
                .Fetch(obj => obj.XMLNotaFiscal)
                .ThenFetch(obj => obj.Canhoto)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaProduto> BuscarPorCargaPaginado(int codigoCarga, int inicioRegistros, int maximoRegistros)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaProduto>();
            var result = query.Where(obj => obj.CargaEntrega.Carga.Codigo == codigoCarga);

            return result
                .Fetch(obj => obj.Produto)
                .Fetch(obj => obj.XMLNotaFiscal)
                .ThenFetch(obj => obj.Canhoto)
                .Skip(inicioRegistros)
                .Take(maximoRegistros)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaProduto> BuscarPorCargaEntregaCodigoCarga(int codigoCarga)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaProduto>();
            var result = query.Where(obj => obj.CargaEntrega.Carga.Codigo == codigoCarga);
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaProduto> BuscarPorCargaEntrega(int codigoCargaEntrega)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaProduto>();
            var result = query.Where(obj => obj.CargaEntrega.Codigo == codigoCargaEntrega);

            return result
            .Fetch(obj => obj.Produto)
            .Fetch(obj => obj.XMLNotaFiscal)
            .ThenFetch(obj => obj.Canhoto)
            .ToList();
        }

        public List<(string Descricao, decimal Quantidade)> BuscarProdutosPorCargaEntrega(int codigoCargaEntrega)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaProduto>()
                .Where(obj => obj.CargaEntrega.Codigo == codigoCargaEntrega)
                .Select(obj => new { obj.Produto.Codigo, obj.Produto.Descricao, obj.Quantidade })
                .ToList();

            return query
                .GroupBy(obj => new { obj.Codigo, obj.Descricao })
                .Select(grupo => (grupo.Key.Descricao, grupo.Sum(obj => obj.Quantidade)))
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaProduto> BuscarProdutosDevolvidosPorCargaEntrega(int codigoCargaEntrega, int codigoChamado)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaProduto>();
            var result = query.Where(obj => obj.CargaEntrega.Codigo == codigoCargaEntrega && obj.QuantidadeDevolucao > 0 &&
                                    obj.XMLNotaFiscal.SituacaoEntregaNotaFiscal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoNotaFiscal.DevolvidaParcial);

            var queryCargaEntregaNotaFiscal = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal>();
            result = result.Where(o => queryCargaEntregaNotaFiscal.Where(obj => obj.CargaEntrega.Codigo == codigoCargaEntrega &&
                                                                                obj.PedidoXMLNotaFiscal.XMLNotaFiscal.Codigo == o.XMLNotaFiscal.Codigo &&
                                                                                (obj.Chamado == null || obj.Chamado.Codigo == codigoChamado)).Any());

            return result.Fetch(obj => obj.Produto)
                         .Fetch(obj => obj.XMLNotaFiscal)
                         .ToList();
        }

        public void ExcluirTodosPorCarga(int carga)
        {
            UnitOfWork.Sessao.CreateQuery("DELETE CargaEntregaProduto obj WHERE obj.CargaEntrega in (select cargaEntrega.Codigo from CargaEntrega cargaEntrega where cargaEntrega.Carga.Codigo = :Carga)")
                             .SetInt32("Carga", carga)
                             .ExecuteUpdate();
        }

        public void ExcluirPorNotaFiscal(int xmlNotaFiscal)
        {
            UnitOfWork.Sessao.CreateQuery("DELETE CargaEntregaProduto obj WHERE obj.XMLNotaFiscal.Codigo = :XMLNotaFiscal")
                             .SetInt32("XMLNotaFiscal", xmlNotaFiscal)
                             .ExecuteUpdate();
        }

        public bool NotaFiscalComProdutosDevolvidos(int codigoXMLNotaFiscal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaProduto>();

            var result = from obj in query where obj.XMLNotaFiscal.Codigo == codigoXMLNotaFiscal && obj.QuantidadeDevolucao > 0 select obj;

            return result.Count() > 0;
        }

        public void InsertSQLListaProdutos(List<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador> listaProdutos, List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto> cargaPedidoProdutos, Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega, Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal, bool multiplicar)
        {
            //quando um valor pode ser nulo usar assim
            //valor = DBNull.Value;
            //if (!string.IsNullOrEmpty(produtosPedido[i].PROPRIEDADE))
            //    valor = produtosPedido[i].PROPRIEDADE;
            //query.SetParameter("PROP_" + i.ToString(), valor);

            if (listaProdutos == null || listaProdutos.Count == 0 || cargaPedidoProdutos == null || cargaPedidoProdutos.Count == 0)
                return;

            int take = 450;
            int start = 0;

            while (start < listaProdutos.Count)
            {
                List<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador> Listatemp = listaProdutos.Skip(start).Take(take).ToList();

                string parameros = "( :CEN_CODIGO_[X], :PRO_CODIGO_[X], :CPP_PESO_UNITARIO_[X], :CPP_QUANTIDADE_[X] ";
                if (xmlNotaFiscal != null)
                    parameros += ", :NFX_CODIGO_[X]";
                parameros += ")";

                string sqlQuery = @"INSERT INTO T_CARGA_ENTREGA_PRODUTO ( CEN_CODIGO, PRO_CODIGO, CPP_PESO_UNITARIO, CPP_QUANTIDADE";
                if (xmlNotaFiscal != null)
                    sqlQuery += ", NFX_CODIGO";

                sqlQuery += ") values " + parameros.Replace("[X]", "0");

                for (int i = 1; i < Listatemp.Count; i++)
                    sqlQuery += ", " + parameros.Replace("[X]", i.ToString());

                var query = this.SessionNHiBernate.CreateSQLQuery(sqlQuery);

                query.SetParameter("CEN_CODIGO_0", cargaEntrega.Codigo);
                query.SetParameter("PRO_CODIGO_0", Listatemp[0].Codigo);
                query.SetParameter("CPP_PESO_UNITARIO_0", Listatemp[0].PesoUnitario);

                int quantidadeCaixa = Listatemp[0].QuantidadeCaixa;
                if (quantidadeCaixa == 0 || !multiplicar)
                    quantidadeCaixa = 1;

                query.SetParameter("CPP_QUANTIDADE_0", (quantidadeCaixa * (from obj in cargaPedidoProdutos where obj.Produto.Codigo == Listatemp[0].Codigo select obj.QuantidadeUtilizar).Sum()));
                if (xmlNotaFiscal != null)
                    query.SetParameter("NFX_CODIGO_0", xmlNotaFiscal.Codigo);

                for (int i = 1; i < Listatemp.Count; i++)
                {
                    query.SetParameter("CEN_CODIGO_" + i.ToString(), cargaEntrega.Codigo);
                    query.SetParameter("PRO_CODIGO_" + i.ToString(), Listatemp[i].Codigo);
                    query.SetParameter("CPP_PESO_UNITARIO_" + i.ToString(), Listatemp[i].PesoUnitario);

                    quantidadeCaixa = Listatemp[i].QuantidadeCaixa;
                    if (quantidadeCaixa == 0 || !multiplicar)
                        quantidadeCaixa = 1;

                    query.SetParameter("CPP_QUANTIDADE_" + i.ToString(), (quantidadeCaixa * (from obj in cargaPedidoProdutos where obj.Produto.Codigo == Listatemp[i].Codigo select obj.QuantidadeUtilizar).Sum()));

                    if (xmlNotaFiscal != null)
                        query.SetParameter("NFX_CODIGO_" + i.ToString(), xmlNotaFiscal.Codigo);
                }

                query.ExecuteUpdate();
                start += take;
            }
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaProduto> ObterQueryBuscarPorCodigos(IEnumerable<int> codigos)
        {
            return SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaProduto>()
                .Where(obj => codigos.Contains(obj.Codigo));
        }

        #endregion Métodos Privados
    }
}
