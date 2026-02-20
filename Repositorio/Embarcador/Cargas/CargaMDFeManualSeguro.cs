using System.Collections.Generic;
using System.Linq;
namespace Repositorio.Embarcador.Cargas
{
    public class CargaMDFeManualSeguro : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualSeguro>
    {
        public CargaMDFeManualSeguro(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualSeguro BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualSeguro>();

            query = query.Where(o => o.Codigo == codigo);

            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualSeguro> BuscarPorCargaMDFeManual(int codigoCargaMDFeManual)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualSeguro>();

            query = query.Where(o => o.CargaMDFeManual.Codigo == codigoCargaMDFeManual);

            return query.ToList();
        }
    }
}
