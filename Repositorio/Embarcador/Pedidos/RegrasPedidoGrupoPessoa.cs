using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Pedidos
{
    public class RegrasPedidoGrupoPessoa : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.RegrasPedidoGrupoPessoa>
    {
        public RegrasPedidoGrupoPessoa(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Pedidos.RegrasPedidoGrupoPessoa BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.RegrasPedidoGrupoPessoa>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.RegrasPedidoGrupoPessoa> BuscarPorRegras(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.RegrasPedidoGrupoPessoa>();
            var result = from obj in query where obj.RegrasPedido.Codigo == codigo select obj;

            return result.OrderBy("Ordem ascending").ToList();
        }
    }
}
