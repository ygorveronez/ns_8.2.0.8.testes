using System.Linq;

namespace Repositorio.Embarcador.Configuracoes
{
    public class IntegracaoAvon : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoAvon>
    {
        public IntegracaoAvon(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Configuracoes.IntegracaoAvon BuscarPorEmpresa(int codigoEmpresa)
        {
            IQueryable<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoAvon> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoAvon>();

            query = query.Where(o => o.Empresa.Codigo == codigoEmpresa);

            return query.FirstOrDefault();
        }
    }
}
