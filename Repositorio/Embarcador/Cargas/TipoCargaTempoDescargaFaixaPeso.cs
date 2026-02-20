using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Cargas
{
    public class TipoCargaTempoDescargaFaixaPeso : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.TipoCargaTempoDescargaFaixaPeso>
    {
        public TipoCargaTempoDescargaFaixaPeso(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Cargas.TipoCargaTempoDescargaFaixaPeso> BuscarPorTipoCarga(int codigoTipoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.TipoCargaTempoDescargaFaixaPeso>();

            var result = from obj in query select obj;
            result = result.Where(tmv => tmv.TipoDeCarga.Codigo == codigoTipoCarga);

            return result.ToList();
        }

    }
}
