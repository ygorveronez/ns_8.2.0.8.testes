using System.Linq;
using System.Threading;

namespace Repositorio.Embarcador.Configuracoes
{
    public class IntegracaoKMM : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoKMM>
    {
        #region Construtores

        public IntegracaoKMM(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public IntegracaoKMM(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion Construtores

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Configuracoes.IntegracaoKMM Buscar()
        {
            var consultaIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoKMM>();

            return consultaIntegracao.FirstOrDefault();
        }

        #endregion
    }
}
