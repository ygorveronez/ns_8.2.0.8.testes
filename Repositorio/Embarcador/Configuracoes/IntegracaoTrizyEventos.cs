using System.Threading;

namespace Repositorio.Embarcador.Configuracoes
{
    public class IntegracaoTrizyEventos : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTrizyEventos>
    {
        #region Construtores

        public IntegracaoTrizyEventos(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public IntegracaoTrizyEventos(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion Construtores
    }
}
