using System.Collections.Generic;
using System.Linq;


namespace Repositorio
{
    public class CIOTCidadesPedagio : RepositorioBase<Dominio.Entidades.CIOTCidadesPedagio>, Dominio.Interfaces.Repositorios.CIOTCidadesPedagio
    {
        public CIOTCidadesPedagio(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.CIOTCidadesPedagio> BuscaPorCIOT(int codigoCIOT)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.CIOTCidadesPedagio>();

            var result = from obj in query where obj.CIOT.Codigo == codigoCIOT select obj;

            return result.ToList();
        }
    }
}
