using Dominio.Entidades.ProcessadorTarefas;
using System.Threading;
using System.Threading.Tasks;

namespace Dominio.Interfaces.Repositorios.ProcessadorTarefas
{
    public interface IRequestDocumentoRepository : IRepositorioGenerico<RequestDocumento>
    {

        Task<RequestDocumento> ObterPorIdAsync(string id, CancellationToken cancellationToken);

        Task CriarIndicesAsync();
    }
}

