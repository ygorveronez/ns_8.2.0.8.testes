using System.Linq;
using System.Threading;

namespace Repositorio.Embarcador.Configuracoes
{
    public class IntegracaoJJ : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoJJ>
    {
        #region Consutrutores

        public IntegracaoJJ(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public IntegracaoJJ(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion Consutrutores

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Configuracoes.IntegracaoJJ Buscar()
        {
            var consultaIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoJJ>();

            return consultaIntegracao.FirstOrDefault();
        }

        #endregion
    }
}
