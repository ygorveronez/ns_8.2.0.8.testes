using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.PedidoVenda
{
    public class VendaDiretaItem : RepositorioBase<Dominio.Entidades.Embarcador.PedidoVenda.VendaDiretaItem>
    {
        public VendaDiretaItem(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.PedidoVenda.VendaDiretaItem> BuscarPorVendaDireta(int codigoVendaDireta)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PedidoVenda.VendaDiretaItem>();

            query = from obj in query where obj.VendaDireta.Codigo == codigoVendaDireta select obj;

            return query.ToList();
        }
    }
}
