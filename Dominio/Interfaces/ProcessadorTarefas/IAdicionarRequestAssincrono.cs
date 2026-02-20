using Dominio.ObjetosDeValor.ProcessadorTarefas;
using Dominio.ObjetosDeValor.ProcessadorTarefas.Enumeradores;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Dominio.Interfaces.ProcessadorTarefas
{

    public interface IAdicionarRequestAssincrono
    {
        Task<RetornoAdicionarRequestAssincrono> SalvarAsync<T>(T objeto, TipoRequest tipoRequest, List<TipoEtapaTarefa> tiposEtapas, CancellationToken cancellationToken, int codigoIntegradora = 0) where T : class;

        Task<RetornoAdicionarRequestAssincrono> SalvarLoteAsync<T>(List<T> objetos, TipoRequest tipoRequest, List<TipoEtapaTarefa> tiposEtapas, CancellationToken cancellationToken, int codigoIntegradora = 0) where T : class;
    }
}

