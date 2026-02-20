using System.Linq;
using System.Threading;

namespace Repositorio.Embarcador.Configuracoes
{
    public class IntegracaoSistemaTransben : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSistemaTransben>
    {
        public IntegracaoSistemaTransben(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public IntegracaoSistemaTransben(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSistemaTransben Buscar()
        {
            var consultaIntegracaoSistemaTransben = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSistemaTransben>();

            return consultaIntegracaoSistemaTransben.FirstOrDefault();
        }

        #endregion
    }
}
