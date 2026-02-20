using System.Collections.Generic;
using System.Linq;
namespace Repositorio.Embarcador.Cargas
{
    public class CargaMDFeManualValePedagio : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualValePedagio>
    {
        public CargaMDFeManualValePedagio(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualValePedagio BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualValePedagio>();

            query = query.Where(o => o.Codigo == codigo);

            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualValePedagio> BuscarPorCargaMDFeManual(int codigoCargaMDFeManual)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualValePedagio>();

            query = query.Where(o => o.CargaMDFeManual.Codigo == codigoCargaMDFeManual);

            return query.ToList();
        }
    }
}
