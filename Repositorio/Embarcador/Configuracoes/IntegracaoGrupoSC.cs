using System.Threading;

namespace Repositorio.Embarcador.Configuracoes
{
    public class IntegracaoGrupoSC : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoGrupoSC>
    {
        #region Construtores

        public IntegracaoGrupoSC(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public IntegracaoGrupoSC(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion Construtores
    }
}
