using Dominio.Entidades.ProcessadorTarefas;
using Dominio.Interfaces.Repositorios.ProcessadorTarefas;
using Dominio.ObjetosDeValor.ProcessadorTarefas.Enumeradores;
using Hangfire;
using Hangfire.Common;
using Hangfire.States;
using MongoDB.Driver;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Servicos.ProcessadorTarefas
{
    public class OrquestradorTarefas
    {
        private readonly IProcessamentoTarefaRepository _repositorioTarefa;
        private readonly IRequestDocumentoRepository _repositorioRequest;
        private readonly IBackgroundJobClient _backgroundJobClient;

        public OrquestradorTarefas(IProcessamentoTarefaRepository repositorioTarefa, IRequestDocumentoRepository repositorioRequest, IBackgroundJobClient backgroundJobClient)
        {
            _repositorioTarefa = repositorioTarefa;
            _repositorioRequest = repositorioRequest;
            _backgroundJobClient = backgroundJobClient;
        }

        [AutomaticRetry(Attempts = 5)]
        public async Task IniciarProcessamentoAsync(string tarefaId, string requestId)
        {
            using var cts = new CancellationTokenSource();
            var cancellationToken = cts.Token;

            try
            {
                var updateDefinition = Builders<ProcessamentoTarefa>.Update
                    .Set(t => t.Status, StatusTarefa.EmProcessamento)
                    .Set(t => t.AtualizadoEm, DateTime.UtcNow)
                    .Inc(t => t.Versao, 1);

                var filter = Builders<ProcessamentoTarefa>.Filter.And(
                    Builders<ProcessamentoTarefa>.Filter.Eq(t => t.Id, tarefaId),
                    Builders<ProcessamentoTarefa>.Filter.Eq(t => t.Status, StatusTarefa.Aguardando)
                );

                ProcessamentoTarefa tarefa = null;
                int tentativas = 0;
                const int maxTentativas = 10;
                const int delayMs = 500;

                while (tarefa == null && tentativas < maxTentativas)
                {
                    tarefa = await _repositorioTarefa.ObterPorIdAsync(tarefaId, cancellationToken);

                    if (tarefa == null)
                    {
                        await Task.Delay(delayMs, cancellationToken);
                        tentativas++;
                    }
                }

                if (tarefa == null)
                {
                    Log.TratarErro($"Tarefa {tarefaId} não encontrada após {maxTentativas} tentativas", "OrquestradorTarefas");
                    return;
                }

                if (tarefa.Etapas == null || tarefa.Etapas.Count == 0)
                {
                    Log.TratarErro($"Tarefa {tarefaId} sem etapas", "OrquestradorTarefas");
                    return;
                }

                var resultado = await _repositorioTarefa.AtualizarComFiltroAsync(
                    filter,
                    updateDefinition,
                    cancellationToken
                );

                if (!resultado)
                {
                    return;
                }

                var etapaAtual = tarefa.Etapas[0];

                var job = Job.FromExpression<EtapaProcessor>(x => x.ProcessarEtapaAsync(tarefaId, requestId, 0));
                _backgroundJobClient.Create(job, new EnqueuedState(etapaAtual.Tipo.ObterFila()));
            }
            catch (Exception ex)
            {
                Log.TratarErro(ex, "OrquestradorTarefas");

                try
                {
                    await _repositorioTarefa.AtualizarStatusAsync(
                        tarefaId,
                        StatusTarefa.Falha,
                        "Erro ao iniciar processamento: " + ex.Message,
                        cancellationToken
                    );
                }
                catch (Exception exInterno)
                {
                    Log.TratarErro(exInterno, "OrquestradorTarefas - Erro ao marcar falha");
                }

                throw;
            }
        }
    }
}

