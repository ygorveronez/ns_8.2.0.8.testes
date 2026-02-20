using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Cargas.ControleEntrega
{
    public class CargaEntregaFotoGTA : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaFotoGTA>
    {
        public CargaEntregaFotoGTA(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaFotoGTA> BuscarPorCargaEntrega(int codigoCargaEntrega)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaFotoGTA>();
            var result = query.Where(obj => obj.CargaEntrega.Codigo == codigoCargaEntrega);
            return result.ToList();
        }

    }
}
