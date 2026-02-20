using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Cargas
{
    public class CargaPreCalculo : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaPreCalculo>
    {
        #region Constructores
        public CargaPreCalculo(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Metodos Publicos
        public Dominio.Entidades.Embarcador.Cargas.CargaPreCalculo BuscarPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPreCalculo>();
            return query.Where(cp => cp.Carga.Codigo == codigoCarga).FirstOrDefault();
        }
        public List<Dominio.Entidades.Embarcador.Cargas.CargaPreCalculo> BuscarPorCargas(List<int> codigosCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPreCalculo>();
            return query.Where(cp => codigosCarga.Contains(cp.Carga.Codigo)).ToList();
        }
        
        #endregion
    }
}
