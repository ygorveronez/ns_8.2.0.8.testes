using System.Threading;

namespace Repositorio.Embarcador.Configuracoes
{
    public class IntegracaoCebrace : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoCebrace>
    {
        #region Construtores

        public IntegracaoCebrace(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public IntegracaoCebrace(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion Construtores
    }
}

