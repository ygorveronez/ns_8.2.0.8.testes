using System.Linq;

namespace Repositorio.Embarcador.Configuracoes
{
    public class IntegracaoNSTech : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoNSTech>
    {
        public IntegracaoNSTech(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Configuracoes.IntegracaoNSTech Buscar()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoNSTech>();

            var result = from obj in query select obj;

            return result.FirstOrDefault();
        }

    }
}
