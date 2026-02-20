using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;

namespace Repositorio.Embarcador.Devolucao
{
    public class GestaoDevolucaoLaudo : RepositorioBase<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoLaudo>
    {
        public GestaoDevolucaoLaudo(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoLaudo BuscarPorCodigo(long codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoLaudo>();
            query = query.Where(o => o.Codigo == codigo);
            return query.FirstOrDefault();
        }
    }
}
