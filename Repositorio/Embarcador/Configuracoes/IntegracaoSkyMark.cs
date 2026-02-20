using System.Threading;

namespace Repositorio.Embarcador.Configuracoes
{
    public class IntegracaoSkyMark : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSkyMark>
    {
        #region Construtores

        public IntegracaoSkyMark(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public IntegracaoSkyMark(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion Construtores

        #region Métodos Públicos   

        #endregion
    }
}
