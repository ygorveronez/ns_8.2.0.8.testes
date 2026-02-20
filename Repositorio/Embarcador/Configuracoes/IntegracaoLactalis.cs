using System.Threading;

namespace Repositorio.Embarcador.Configuracoes
{
    public class IntegracaoLactalis : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoLactalis>
    {
        public IntegracaoLactalis(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public IntegracaoLactalis(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }
    }
}
