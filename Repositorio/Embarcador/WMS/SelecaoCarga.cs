using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.WMS
{
    public class SelecaoCarga : RepositorioBase<Dominio.Entidades.Embarcador.WMS.SelecaoCarga>
    {
        public SelecaoCarga(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.WMS.SelecaoCarga BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.WMS.SelecaoCarga>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.WMS.SelecaoCarga> BuscarPorSelecao(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.WMS.SelecaoCarga>();
            var result = from obj in query where obj.Selecao.Codigo == codigo select obj;
            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Pedidos.PedidoProduto BuscarPrimeiroProdutoCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var result = from obj in query where obj.Carga.Codigo == codigoCarga select obj;

            var resultPedidos = result.Select(obj => obj.Pedido);

            var queryProduto = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto>();
            var resultProduto = from obj in queryProduto where resultPedidos.Contains(obj.Pedido) select obj;

            return resultProduto.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto> BuscarProdutosCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var result = from obj in query where obj.Carga.Codigo == codigoCarga select obj;

            var resultPedidos = result.Select(obj => obj.Pedido);

            var queryProduto = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto>();
            var resultProduto = from obj in queryProduto where resultPedidos.Contains(obj.Pedido) select obj;

            return resultProduto.ToList();
        }

        public List<Dominio.Entidades.Embarcador.WMS.SelecaoCarga> BuscarPorCargas(List<int> codigosCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.WMS.SelecaoCarga>();

            var result = from obj in query where codigosCarga.Contains(obj.Codigo) select obj;

            return result
                .Fetch(obj => obj.Selecao)
                .Fetch(obj => obj.Carga)
                .ThenFetch(obj => obj.DadosSumarizados)
                .Fetch(obj => obj.Carga)
                .ThenFetch(obj => obj.TipoOperacao)
                .ToList();
        }
    }
}