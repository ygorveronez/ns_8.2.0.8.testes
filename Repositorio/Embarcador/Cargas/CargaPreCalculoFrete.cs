using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Cargas
{
    public class CargaPreCalculoFrete : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaPreCalculoFrete>
    {
        #region Constructores

        public CargaPreCalculoFrete(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Metodos Publicos

        public Dominio.Entidades.Embarcador.Cargas.CargaPreCalculoFrete BuscarPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPreCalculoFrete>();
            return query.Where(obj => obj.Carga.Codigo == codigoCarga).FirstOrDefault();
        }

        public bool ExistePorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPreCalculoFrete>();
            return query.Any(obj => obj.Carga.Codigo == codigoCarga);
        }
        
        #endregion
    }
}
