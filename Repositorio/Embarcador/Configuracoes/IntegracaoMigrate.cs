using System.Linq;
using System.Threading;

namespace Repositorio.Embarcador.Configuracoes
{
    public class IntegracaoMigrate : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoMigrate>
    {
        #region Construtores

        public IntegracaoMigrate(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public IntegracaoMigrate(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion Construtores

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Configuracoes.IntegracaoMigrate Buscar()
        {
            var consultaIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoMigrate>();

            return consultaIntegracao.FirstOrDefault();
        }

        #endregion
    }
}
