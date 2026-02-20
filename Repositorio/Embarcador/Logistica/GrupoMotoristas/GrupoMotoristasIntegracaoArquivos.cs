using System.Threading;

namespace Repositorio.Embarcador.Logistica.GrupoMotoristas
{
    public class GrupoMotoristasIntegracaoArquivos(UnitOfWork unitOfWork, CancellationToken cancellationToken)
        : Abstrato.RepositorioRelacionamentoGrupoMotoristaIntegracao<Dominio.Entidades.Embarcador.Logistica.GrupoMotoristasIntegracaoArquivos>(
            unitOfWork,
            cancellationToken
        )
    { }
}
