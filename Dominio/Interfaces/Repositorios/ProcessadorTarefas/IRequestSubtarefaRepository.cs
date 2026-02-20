using Dominio.Entidades.ProcessadorTarefas;
using Dominio.ObjetosDeValor.ProcessadorTarefas.Enumeradores;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Dominio.Interfaces.Repositorios.ProcessadorTarefas
{
    public interface IRequestSubtarefaRepository : IRepositorioGenerico<RequestSubtarefa>
    {
        Task<List<RequestSubtarefa>> ObterPorTarefaIdAsync(string tarefaId, CancellationToken cancellationToken);

        Task<List<RequestSubtarefa>> ObterPendentesPorTarefaIdAsync(string tarefaId, CancellationToken cancellationToken);

        Task AtualizarStatusAsync(string id, StatusTarefa status, string mensagem, CancellationToken cancellationToken);

        Task CriarIndicesAsync();
    }
}

