using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Logistica
{
    public class CentroDescarregamentoEmail : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.CentroDescarregamentoEmail>
    {
        public CentroDescarregamentoEmail(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Logistica.CentroDescarregamentoEmail BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.CentroDescarregamentoEmail>();

            query = query.Where(o => o.Codigo == codigo);

            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.CentroDescarregamentoEmail> BuscarPorCentroDescarregamento(int codigoCentroDescarregamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.CentroDescarregamentoEmail>();

            query = query.Where(o => o.CentroDescarregamento.Codigo == codigoCentroDescarregamento);

            return query.ToList();
        }
    }
}
