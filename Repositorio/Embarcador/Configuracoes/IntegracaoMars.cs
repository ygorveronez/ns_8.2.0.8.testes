using System.Linq;
using System.Threading;

namespace Repositorio.Embarcador.Configuracoes
{
    public class IntegracaoMars : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoMars>
    {
        #region Construtores

        public IntegracaoMars(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public IntegracaoMars(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion Construtores

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Configuracoes.IntegracaoMars Buscar()
        {
            var consultaIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoMars>();

            return consultaIntegracao.FirstOrDefault();
        }

        #endregion
    }
}
