using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Logistica
{
    public class CentroCarregamentoPunicao : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoPunicao>
    {
        public CentroCarregamentoPunicao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoPunicao> BuscarPorCentroDeCarregamento(int codigoCentroCarregamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoPunicao>();
            var result = from obj in query select obj;
            result = result.Where(o => o.CentroCarregamento.Codigo == codigoCentroCarregamento);
            return result.ToList();
        }
    }
}