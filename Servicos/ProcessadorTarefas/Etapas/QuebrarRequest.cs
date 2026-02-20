using Dominio.Entidades.ProcessadorTarefas;
using Dominio.Interfaces.ProcessadorTarefas;
using Dominio.Interfaces.Repositorios.ProcessadorTarefas;
using Dominio.ObjetosDeValor.ProcessadorTarefas;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Servicos.ProcessadorTarefas.Etapas
{
    public class QuebrarRequest : EtapaState
    {
        private readonly IRequestSubtarefaRepository _repositorioSubtarefa;
        private readonly TarefaStrategyFactory _tarefaStrategyFactory;

        public QuebrarRequest(IRequestSubtarefaRepository repositorioSubtarefa, TarefaStrategyFactory tarefaStrategyFactory)
        {
            _repositorioSubtarefa = repositorioSubtarefa;
            _tarefaStrategyFactory = tarefaStrategyFactory;
        }

        public override async Task ExecutarAsync(ContextoEtapa contexto, CancellationToken cancellationToken)
        {
            IStrategyQuebraRequest strategyQuebraRequest = _tarefaStrategyFactory
                .ObterStrategy<IStrategyQuebraRequest>(contexto.Tarefa.TipoRequest);

            List<RequestSubtarefa> subtarefas = strategyQuebraRequest.QuebrarRequestTarefa(contexto);

            await _repositorioSubtarefa.InserirMuitosAsync(subtarefas, cancellationToken);
        }
    }
}