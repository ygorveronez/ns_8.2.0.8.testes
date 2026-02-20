using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Cargas
{
    public class CargaDadosParaProcesamentoDt : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaDadosParaProcessamentoDt>
    {
        #region Constructores
        public CargaDadosParaProcesamentoDt(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Metodos Publicos

        public Dominio.Entidades.Embarcador.Cargas.CargaDadosParaProcessamentoDt BuscarPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaDadosParaProcessamentoDt>();
            query = query.Where(d => d.Carga.Codigo == codigoCarga);
            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaDadosParaProcessamentoDt> BuscarPorCargas(List<int> codigosCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaDadosParaProcessamentoDt>();
            query = query.Where(d => codigosCarga.Contains(d.Carga.Codigo));
            return query.ToList();
        }
        #endregion
    }
}
