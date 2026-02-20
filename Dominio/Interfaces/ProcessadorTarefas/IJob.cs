using System.Threading.Tasks;

namespace Dominio.Interfaces.ProcessadorTarefas
{
    public interface IJob
    {
        Task ExecutarAsync();
    }
}
