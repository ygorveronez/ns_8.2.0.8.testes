using System.Linq;
using System.Threading;

namespace Repositorio.Embarcador.Configuracoes
{
    public class IntegracaoCamil : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoCamil>
    {
        #region Construtores

        public IntegracaoCamil(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public IntegracaoCamil(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion Construtores

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Configuracoes.IntegracaoCamil Buscar()
        {
            var consultaIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoCamil>();
            return consultaIntegracao.FirstOrDefault();
        }

        #endregion
    }
}

