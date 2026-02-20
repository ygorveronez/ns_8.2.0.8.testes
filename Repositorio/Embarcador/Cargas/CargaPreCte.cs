using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Cargas
{
    public class CargaPreCte : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaPreCte>
    {
        #region Constructores
        public CargaPreCte(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Metodos Publicos
        public Dominio.Entidades.Embarcador.Cargas.CargaPreCte BuscarPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query <Dominio.Entidades.Embarcador.Cargas.CargaPreCte>();

            return query.Where(c => c.Carga.Codigo == codigoCarga).FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPreCte> BuscarDocumentosPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPreCte>();

            return query.Where(c => c.Carga.Codigo == codigoCarga).ToList();
        } 
        
   
        #endregion
    }
}
