using System.Linq;
using System.Threading;

namespace Repositorio.Embarcador.Configuracoes
{
    public class IntegracaoRuntec : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoRuntec>
    {
        #region Construtores

        public IntegracaoRuntec(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public IntegracaoRuntec(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion Construtores

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Configuracoes.IntegracaoRuntec Buscar()
        {
            var consultaIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoRuntec>();

            return consultaIntegracao.FirstOrDefault();
        }

        #endregion
    }
}
