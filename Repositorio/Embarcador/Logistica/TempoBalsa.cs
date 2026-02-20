using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Logistica
{
    public class TempoBalsa : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.TempoBalsa>
    {
        public TempoBalsa(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Logistica.TempoBalsa> BuscarPorTrechoBalsa(int codigoTrechoBalsa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.TempoBalsa>();

            var result = from obj in query select obj;
            result = result.Where(obj => obj.TrechoBalsa.Codigo == codigoTrechoBalsa);

            return result.ToList();
        }

    }
}
