using System.Linq;
using System.Threading;

namespace Repositorio.Embarcador.Configuracoes
{
    public class IntegracaoBalancaKIKI : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoBalancaKIKI>
    {
        #region Construtores

        public IntegracaoBalancaKIKI(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public IntegracaoBalancaKIKI(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion Construtores

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Configuracoes.IntegracaoBalancaKIKI Buscar()
        {
            var consultaIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoBalancaKIKI>();

            return consultaIntegracao.FirstOrDefault();
        }

        #endregion
    }
}
