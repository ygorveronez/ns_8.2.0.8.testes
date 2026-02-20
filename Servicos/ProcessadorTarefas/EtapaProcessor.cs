using Dominio.Entidades.ProcessadorTarefas;
using Dominio.Excecoes.Embarcador;
using Dominio.Interfaces.Repositorios.ProcessadorTarefas;
using Dominio.ObjetosDeValor.ProcessadorTarefas;
using Dominio.ObjetosDeValor.ProcessadorTarefas.Enumeradores;
using Hangfire;
using Hangfire.Common;
using Hangfire.States;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Servicos.ProcessadorTarefas
{
    public class EtapaProcessor
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IProcessamentoTarefaRepository _repositorioTarefa;
        private readonly IRequestDocumentoRepository _repositorioRequest;
        private readonly IBackgroundJobClient _backgroundJobClient;

        public EtapaProcessor(IServiceProvider serviceProvider, IProcessamentoTarefaRepository repositorioTarefa, IRequestDocumentoRepository repositorioRequest, IBackgroundJobClient backgroundJobClient)
        {
            _serviceProvider = serviceProvider;
            _repositorioTarefa = repositorioTarefa;
            _repositorioRequest = repositorioRequest;
            _backgroundJobClient = backgroundJobClient;
        }

        [AutomaticRetry(Attempts = 5)]
        public async Task ProcessarEtapaAsync(string tarefaId, string requestId, int indiceEtapa)
        {
            using var scope = _serviceProvider.CreateScope();
            using var cts = new CancellationTokenSource();
            var cancellationToken = cts.Token;
            var stopwatch = Stopwatch.StartNew();

            try
            {
                ProcessamentoTarefa tarefa = await _repositorioTarefa.ObterPorIdAsync(tarefaId, cancellationToken);

                if (tarefa == null)
                {
                    Log.TratarErro($"Tarefa {tarefaId} não encontrada", "EtapaProcessor");
                    return;
                }

                if (tarefa.EtapaAtual != indiceEtapa)
                {
                    return;
                }

                var etapaAtual = tarefa.Etapas[indiceEtapa];

                if (etapaAtual.Status == StatusTarefa.Concluida)
                {
                    return;
                }

                if (etapaAtual.Status == StatusTarefa.Falha && etapaAtual.Tentativas >= 5)
                {
                    Log.TratarErro($"Tarefa {tarefaId} etapa {indiceEtapa} falhou após {etapaAtual.Tentativas} tentativas", "EtapaProcessor");
                    return;
                }

                if (etapaAtual.IniciadoEm == null)
                {
                    var updateInicio = Builders<ProcessamentoTarefa>.Update
                        .Set($"PRO_ETAPAS.{indiceEtapa}.ETA_INICIADO_EM", DateTime.UtcNow)
                        .Set($"PRO_ETAPAS.{indiceEtapa}.ETA_STATUS", StatusTarefa.EmProcessamento)
                        .Inc($"PRO_ETAPAS.{indiceEtapa}.ETA_TENTATIVAS", 1)
                        .Set(t => t.AtualizadoEm, DateTime.UtcNow);

                    await _repositorioTarefa.AtualizarAsync(tarefaId, updateInicio, cancellationToken);
                    tarefa = await _repositorioTarefa.ObterPorIdAsync(tarefaId, cancellationToken);
                    etapaAtual = tarefa.Etapas[indiceEtapa];
                }
                else if (etapaAtual.Status == StatusTarefa.Falha)
                {
                    var updateRetry = Builders<ProcessamentoTarefa>.Update
                        .Set($"PRO_ETAPAS.{indiceEtapa}.ETA_STATUS", StatusTarefa.EmProcessamento)
                        .Set(t => t.AtualizadoEm, DateTime.UtcNow);

                    await _repositorioTarefa.AtualizarAsync(tarefaId, updateRetry, cancellationToken);
                    tarefa = await _repositorioTarefa.ObterPorIdAsync(tarefaId, cancellationToken);
                    etapaAtual = tarefa.Etapas[indiceEtapa];
                }

                RequestDocumento requestDoc = null;
                if (EtapaPrecisaDeRequest(etapaAtual.Tipo))
                {
                    requestDoc = await _repositorioRequest.ObterPorIdAsync(requestId, cancellationToken);
                }

                var etapaFactory = scope.ServiceProvider.GetRequiredService<EtapaStateFactory>();
                var etapaState = etapaFactory.ObterEtapa(etapaAtual.Tipo);

                var contexto = new ContextoEtapa
                {
                    TarefaId = tarefaId,
                    RequestDoc = requestDoc,
                    Tarefa = tarefa,
                    IndiceEtapa = indiceEtapa
                };

                await etapaState.ExecutarAsync(contexto, cancellationToken);

                stopwatch.Stop();

                await MarcarEtapaConcluidaAsync(tarefaId, indiceEtapa, stopwatch.ElapsedMilliseconds, cancellationToken);

                var tarefaAtualizada = await _repositorioTarefa.ObterPorIdAsync(tarefaId, cancellationToken);

                if (tarefaAtualizada != null)
                {
                    await AvancarParaProximaEtapaAsync(tarefaAtualizada, requestId, cancellationToken);
                }
            }
            catch (ServicoException servicoEx)
            {
                stopwatch.Stop();
                await TratarErroEtapaAsync(tarefaId, indiceEtapa, servicoEx.Message, stopwatch.ElapsedMilliseconds, cancellationToken);
                throw;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                Log.TratarErro(ex, "EtapaProcessor");
                await TratarErroEtapaAsync(tarefaId, indiceEtapa, "Erro inesperado: " + ex.Message, stopwatch.ElapsedMilliseconds, cancellationToken);
                throw;
            }
        }

        private async Task MarcarEtapaConcluidaAsync(string tarefaId, int indiceEtapa, long duracaoMs, CancellationToken cancellationToken)
        {
            var update = Builders<ProcessamentoTarefa>.Update
                .Set($"PRO_ETAPAS.{indiceEtapa}.ETA_STATUS", StatusTarefa.Concluida)
                .Set($"PRO_ETAPAS.{indiceEtapa}.ETA_CONCLUIDO_EM", DateTime.UtcNow)
                .Set($"PRO_ETAPAS.{indiceEtapa}.ETA_DURACAO_MS", duracaoMs)
                .Set($"PRO_ETAPAS.{indiceEtapa}.ETA_MENSAGEM", "Etapa realizada com sucesso!")
                .Set(t => t.AtualizadoEm, DateTime.UtcNow);

            await _repositorioTarefa.AtualizarAsync(tarefaId, update, cancellationToken);
        }

        private async Task TratarErroEtapaAsync(string tarefaId, int indiceEtapa, string mensagem, long duracaoMs, CancellationToken cancellationToken)
        {
            var tarefa = await _repositorioTarefa.ObterPorIdAsync(tarefaId, cancellationToken);

            var update = Builders<ProcessamentoTarefa>.Update
                .Set($"PRO_ETAPAS.{indiceEtapa}.ETA_STATUS", StatusTarefa.Falha)
                .Set($"PRO_ETAPAS.{indiceEtapa}.ETA_MENSAGEM", mensagem)
                .Set($"PRO_ETAPAS.{indiceEtapa}.ETA_DURACAO_MS", duracaoMs)
                .Inc($"PRO_ETAPAS.{indiceEtapa}.ETA_TENTATIVAS", 1)
                .Set(t => t.AtualizadoEm, DateTime.UtcNow);

            if (tarefa != null)
            {
                int tentativasAtuais = tarefa.Etapas[indiceEtapa].Tentativas;
                if (tentativasAtuais + 1 >= 5)
                {
                    update = update.Set(t => t.Status, StatusTarefa.Falha);
                }
            }

            await _repositorioTarefa.AtualizarAsync(tarefaId, update, cancellationToken);
        }

        private async Task AvancarParaProximaEtapaAsync(ProcessamentoTarefa tarefa, string requestId, CancellationToken cancellationToken)
        {
            var tarefaAtualizada = await _repositorioTarefa.ObterPorIdAsync(tarefa.Id, cancellationToken);

            if (tarefaAtualizada == null)
            {
                return;
            }

            int proximoIndice = tarefaAtualizada.EtapaAtual + 1;

            if (proximoIndice < tarefaAtualizada.Etapas.Count)
            {
                var update = Builders<ProcessamentoTarefa>.Update
                    .Set(t => t.EtapaAtual, proximoIndice)
                    .Set(t => t.AtualizadoEm, DateTime.UtcNow);

                await _repositorioTarefa.AtualizarAsync(tarefaAtualizada.Id, update, cancellationToken);

                var proximaEtapa = tarefaAtualizada.Etapas[proximoIndice];

                var job = Job.FromExpression<EtapaProcessor>(x => x.ProcessarEtapaAsync(tarefaAtualizada.Id, requestId, proximoIndice));
                _backgroundJobClient.Create(job, new EnqueuedState(proximaEtapa.Tipo.ObterFila()));
            }
            else
            {
                var update = Builders<ProcessamentoTarefa>.Update
                    .Set(t => t.Status, StatusTarefa.Concluida)
                    .Set(t => t.AtualizadoEm, DateTime.UtcNow);

                await _repositorioTarefa.AtualizarAsync(tarefaAtualizada.Id, update, cancellationToken);
            }
        }

        private bool EtapaPrecisaDeRequest(TipoEtapaTarefa tipo)
        {
            return tipo switch
            {
                TipoEtapaTarefa.FecharCarga => false,
                TipoEtapaTarefa.RetornarIntegracao => false,
                _ => true
            };
        }
    }
}
