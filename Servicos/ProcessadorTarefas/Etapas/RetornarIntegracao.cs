using Dominio.Entidades.ProcessadorTarefas;
using Dominio.Excecoes.Embarcador;
using Dominio.Interfaces.ProcessadorTarefas;
using Dominio.Interfaces.Repositorios.ProcessadorTarefas;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.ProcessadorTarefas;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Servicos.ProcessadorTarefas.Etapas
{
    public class RetornarIntegracao : EtapaState
    {
        private readonly ITarefaIntegracao _repositorioTarefaIntegracao;
        private readonly TarefaStrategyFactory _tarefaStrategyFactory;

        public RetornarIntegracao(ITarefaIntegracao repositorioTarefaIntegracao, TarefaStrategyFactory tarefaStrategyFactory)
        {
            _repositorioTarefaIntegracao = repositorioTarefaIntegracao;
            _tarefaStrategyFactory = tarefaStrategyFactory;
        }

        public override async Task ExecutarAsync(ContextoEtapa contexto, CancellationToken cancellationToken)
        {
            List<TarefaIntegracao> integracoesPendentes = await _repositorioTarefaIntegracao
                .ObterTarefaIntegracoesPendentes(contexto.TarefaId, cancellationToken);

            if (!integracoesPendentes.Any())
            {
                return;
            }

            IStrategyRetornoIntegracao strategyRetornoIntegracao = _tarefaStrategyFactory
                .ObterStrategy<IStrategyRetornoIntegracao>(contexto.Tarefa.TipoRequest);

            foreach (TarefaIntegracao integracaoPendente in integracoesPendentes)
            {
                await strategyRetornoIntegracao.RetornoIntegracaoTarefa(contexto, integracaoPendente, cancellationToken);
            }

            if (integracoesPendentes.Any(integracao => integracao.SituacaoIntegracao == SituacaoIntegracao.ProblemaIntegracao))
            {
                throw new ServicoException("Problema ao retornar integrações.");
            }
        }
    }
}
