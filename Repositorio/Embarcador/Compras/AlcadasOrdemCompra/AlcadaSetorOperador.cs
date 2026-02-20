using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Compras.AlcadasOrdemCompra
{
    public class AlcadaSetorOperador : RepositorioBase<Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.AlcadaSetorOperador>
    {
        public AlcadaSetorOperador(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.AlcadaSetorOperador BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.AlcadaSetorOperador>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.AlcadaSetorOperador> BuscarPorRegra(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.AlcadaSetorOperador>();
            var result = from obj in query where obj.RegrasOrdemCompra.Codigo == codigo select obj;

            return result.OrderBy("Ordem ascending").ToList();
        }
    }
}
