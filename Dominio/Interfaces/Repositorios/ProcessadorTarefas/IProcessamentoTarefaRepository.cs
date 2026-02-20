using Dominio.Entidades.ProcessadorTarefas;
using Dominio.ObjetosDeValor.Embarcador.Integracoes.IntegracaoAssincrona;
using Dominio.ObjetosDeValor.ProcessadorTarefas.Enumeradores;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Dominio.Interfaces.Repositorios.ProcessadorTarefas
{
    public interface IProcessamentoTarefaRepository : IRepositorioGenerico<ProcessamentoTarefa>
    {
        Task<bool> AtualizarComFiltroAsync(FilterDefinition<ProcessamentoTarefa> filter, UpdateDefinition<ProcessamentoTarefa> update, CancellationToken cancellationToken);

        Task AtualizarAsync(
            string id, UpdateDefinition<ProcessamentoTarefa> update, CancellationToken cancellationToken);

        Task AtualizarStatusAsync(string id, StatusTarefa status, string mensagem, CancellationToken cancellationToken);

        Task<ProcessamentoTarefa> ObterPorIdAsync(string id, CancellationToken cancellationToken);

        Task CriarIndicesAsync();

        Task<List<ProcessamentoTarefa>> ObterPaginadoComFiltrosAsync(FiltroPesquisaIntegracaoAssincrona filtros, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametros, CancellationToken cancellationToken);

        Task<long> ContarComFiltrosAsync(FiltroPesquisaIntegracaoAssincrona filtros, CancellationToken cancellationToken);
    }
}

