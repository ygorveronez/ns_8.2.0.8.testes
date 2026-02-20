using System.Threading;


namespace Repositorio.Embarcador.Configuracoes
{
    public class IntegracaoOlfar : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoOlfar>
    {
        #region Construtores

        public IntegracaoOlfar(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public IntegracaoOlfar(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion Construtores
    }
}
