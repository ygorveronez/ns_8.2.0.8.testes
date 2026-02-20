using System.Linq;
using System.Threading;

namespace Repositorio.Embarcador.Configuracoes
{
    public class IntegracaoLogvett : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoLogvett>
    {
        #region Construtores

        public IntegracaoLogvett(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public IntegracaoLogvett(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion Construtores

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Configuracoes.IntegracaoLogvett Buscar()
        {
            var consultaIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoLogvett>();

            return consultaIntegracao.FirstOrDefault();
        }

        #endregion
    }
}
