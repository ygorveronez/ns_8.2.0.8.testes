using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Logistica
{
    public class CentroCarregamentoEmail : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoEmail>
    {
        public CentroCarregamentoEmail(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoEmail BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoEmail>();

            query = query.Where(o => o.Codigo == codigo);

            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoEmail> BuscarPorCentroCarregamento(int codigoCentroCarregamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoEmail>();

            query = query.Where(o => o.CentroCarregamento.Codigo == codigoCentroCarregamento);

            return query.ToList();
        }
    }
}
