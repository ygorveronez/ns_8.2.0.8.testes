using System.Threading;

namespace Repositorio.Embarcador.Configuracoes
{
    public class ConfiguracaoIntegracaoSkymark : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoIntegracaoSkymark>
    {
        #region Construtores

        public ConfiguracaoIntegracaoSkymark(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public ConfiguracaoIntegracaoSkymark(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion Construtores

        #region Métodos Públicos   

        #endregion
    }
}
