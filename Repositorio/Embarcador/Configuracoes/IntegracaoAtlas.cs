using System.Linq;
using System.Threading;

namespace Repositorio.Embarcador.Configuracoes
{
    public class IntegracaoAtlas : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoAtlas>
    {
        #region Construtores

        public IntegracaoAtlas(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public IntegracaoAtlas(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion Construtores

        public Dominio.Entidades.Embarcador.Configuracoes.IntegracaoAtlas BuscarPrimeiroRegistro()
        {
            IQueryable<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoAtlas> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoAtlas>();

            return query.FirstOrDefault();
        }
    }
}
