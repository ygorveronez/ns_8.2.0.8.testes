using System.Linq;
using System.Threading;

namespace Repositorio.Embarcador.Configuracoes
{
    public class IntegracaoConecttec : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoConecttec>
    {
        #region Construtores

        public IntegracaoConecttec(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public IntegracaoConecttec(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion Construtores

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Configuracoes.IntegracaoConecttec Buscar()
        {
            var consultaIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoConecttec>();

            return consultaIntegracao.FirstOrDefault();
        }

        #endregion
    }
}
