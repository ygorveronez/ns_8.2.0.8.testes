using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Frota
{
    public class InfracaoTituloEmpresa : RepositorioBase<Dominio.Entidades.Embarcador.Frota.InfracaoTituloEmpresa>
    {
        public InfracaoTituloEmpresa(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Frota.InfracaoTituloEmpresa> BuscarPorInfracao(int codigoInfracao)
        {
            IQueryable<Dominio.Entidades.Embarcador.Frota.InfracaoTituloEmpresa> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.InfracaoTituloEmpresa>();

            query = query.Where(o => o.Infracao.Codigo == codigoInfracao);

            return query.ToList();
        }
    }
}
