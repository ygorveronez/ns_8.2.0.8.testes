using System.Threading;

namespace Repositorio.Embarcador.Configuracoes
{
    public class IntegracaoSenior : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSenior>
    {
        #region Construtores

        public IntegracaoSenior(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public IntegracaoSenior(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion Construtores

        #region Métodos Públicos   

        #endregion
    }
}
