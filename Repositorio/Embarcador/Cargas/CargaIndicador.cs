using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Cargas
{
    public class CargaIndicador : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaIndicador>
    {
        public CargaIndicador(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos
        
        public Dominio.Entidades.Embarcador.Cargas.CargaIndicador BuscarPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaIndicador>()
                .Where(obj => obj.Carga.Codigo == codigoCarga);

            return query.FirstOrDefault();
        }

        #endregion
    }
}
