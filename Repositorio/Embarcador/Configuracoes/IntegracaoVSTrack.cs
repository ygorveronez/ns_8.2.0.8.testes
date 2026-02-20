using System.Linq;
using System.Threading;

namespace Repositorio.Embarcador.Configuracoes
{
    public class IntegracaoVSTrack : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoVSTrack>
    {
        #region Construtores

        public IntegracaoVSTrack(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public IntegracaoVSTrack(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion Construtores

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Configuracoes.IntegracaoVSTrack Buscar()
        {
            var consultaIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoVSTrack>();

            return consultaIntegracao.FirstOrDefault();
        }

        #endregion
    }
}
