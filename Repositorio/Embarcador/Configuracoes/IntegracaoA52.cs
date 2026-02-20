using System.Linq;

namespace Repositorio.Embarcador.Configuracoes
{
    public class IntegracaoA52 : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoA52>
    {
        public IntegracaoA52(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Configuracoes.IntegracaoA52 BuscarPrimeiroRegistro()
        {
            IQueryable<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoA52> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoA52>();

            return query.FirstOrDefault();
        }
    }
}
