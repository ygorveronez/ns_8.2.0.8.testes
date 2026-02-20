using Dominio.Entidades.ProcessadorTarefas;
using Dominio.ObjetosDeValor.ProcessadorTarefas;
using System.Threading;
using System.Threading.Tasks;

namespace Dominio.Interfaces.ProcessadorTarefas;


public interface IStrategyRetornoIntegracao
{
    Task RetornoIntegracaoTarefa(ContextoEtapa contexto, TarefaIntegracao tarefaIntegracao, CancellationToken cancellationToken);
}
