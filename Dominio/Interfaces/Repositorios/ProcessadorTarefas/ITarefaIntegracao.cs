using Dominio.Entidades.ProcessadorTarefas;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Dominio.Interfaces.Repositorios.ProcessadorTarefas
{
    public interface ITarefaIntegracao : IRepositorioGenerico<TarefaIntegracao>
    {
        Task<List<TarefaIntegracao>> ObterTarefaIntegracoesPendentes(string codigoTarefa, CancellationToken cancellationToken);

        Task<ArquivoIntegracao> ObterArquivoIntegracaoPorCodigo(string codigoArquivo, CancellationToken cancellationToken);

        Task<List<TarefaIntegracao>> ObterIntegracaoIntegradora(string codigoTarefa, ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacaoIntegracao, CancellationToken cancellationToken);

        Task AdicionarArquivoAsync(string tarefaIntegracaoId, ArquivoIntegracao arquivo, CancellationToken cancellationToken);

        Task AtualizarAsync(string tarefaIntegracaoId, MongoDB.Driver.UpdateDefinition<TarefaIntegracao> update, CancellationToken cancellationToken);
    }
}
