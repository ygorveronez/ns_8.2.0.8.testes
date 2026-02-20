using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Cargas
{
    public class CargaIsca : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaIsca>
    {
        public CargaIsca(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Cargas.CargaIsca> BuscarPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaIsca>()
                .Where(obj => obj.Carga.Codigo == codigoCarga);

            return query.ToList();
        }

        #endregion
    }
}
