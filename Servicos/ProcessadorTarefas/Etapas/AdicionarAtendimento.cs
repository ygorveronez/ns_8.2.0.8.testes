using Dominio.Entidades.ProcessadorTarefas;
using Dominio.Excecoes.Embarcador;
using Dominio.Interfaces.Database;
using Dominio.Interfaces.Repositorios.ProcessadorTarefas;
using Dominio.ObjetosDeValor.ProcessadorTarefas;
using Dominio.ObjetosDeValor.ProcessadorTarefas.Enumeradores;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Utilidades.Extensions;

namespace Servicos.ProcessadorTarefas.Etapas
{
    public class AdicionarAtendimento : EtapaState
    {
        private readonly ITenantService _tenantService;
        private readonly IRequestSubtarefaRepository _repositorioSubtarefa;
        private readonly IProcessamentoTarefaRepository _repositorioTarefa;
        private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(5, 5);

        public AdicionarAtendimento(ITenantService tenantService, IRequestSubtarefaRepository repositorioSubtarefa, IProcessamentoTarefaRepository repositorioTarefa)
        {
            _tenantService = tenantService;
            _repositorioSubtarefa = repositorioSubtarefa;
            _repositorioTarefa = repositorioTarefa;
        }

        public override async Task ExecutarAsync(ContextoEtapa contexto, CancellationToken cancellationToken)
        {
            List<RequestSubtarefa> subtarefas = await _repositorioSubtarefa
                .ObterPendentesPorTarefaIdAsync(contexto.TarefaId, cancellationToken);

            if (!subtarefas.Any())
                return;

            var resultadosDoc = contexto.Tarefa.Resultado ?? new BsonDocument();
            var resultadosList = resultadosDoc.Contains("atendimentos")
                ? resultadosDoc["atendimentos"].AsBsonArray
                : new BsonArray();

            var resultadosConcurrent = new ConcurrentBag<BsonDocument>();

            var tasks = subtarefas.Select(async subtarefa =>
            {
                await _semaphore.WaitAsync(cancellationToken);
                try
                {
                    await ProcessarSubtarefaAsync(subtarefa, resultadosConcurrent, contexto.TarefaId, cancellationToken);
                }
                finally
                {
                    _semaphore.Release();
                }
            });

            await Task.WhenAll(tasks);

            foreach (var resultado in resultadosConcurrent)
            {
                resultadosList.Add(resultado);
            }

            resultadosDoc["atendimentos"] = resultadosList;
            var update = Builders<ProcessamentoTarefa>.Update
                .Set(t => t.Resultado, resultadosDoc);
            await _repositorioTarefa.AtualizarAsync(contexto.TarefaId, update, cancellationToken);

            var todasSubtarefas = await _repositorioSubtarefa.ObterPorTarefaIdAsync(contexto.TarefaId, cancellationToken);
            var subtarefasPendentes = todasSubtarefas.Where(s => s.Status == StatusTarefa.Aguardando || s.Status == StatusTarefa.EmProcessamento).ToList();

            if (subtarefasPendentes.Any() && contexto.Tarefa.Etapas[contexto.IndiceEtapa].Tentativas < 5)
            {
                throw new ServicoException("Ocorreu um erro ao processar lote de Adicionar Atendimento.");
            }
        }

        private async Task ProcessarSubtarefaAsync(RequestSubtarefa subtarefa, ConcurrentBag<BsonDocument> resultadosConcurrent, string tarefaId, CancellationToken cancellationToken)
        {
            using (var unitOfWork = new Repositorio.UnitOfWork(_tenantService.StringConexao(), Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova))
            {
                try
                {
                    Dominio.ObjetosDeValor.WebService.Atendimento.AdicionarAtendimento adicionarAtendimento =
                        subtarefa.Dados.FromBsonDocument<Dominio.ObjetosDeValor.WebService.Atendimento.AdicionarAtendimento>();

                    Dominio.ObjetosDeValor.WebService.Retorno<int> retorno = await new WebService.Atendimento.Atendimento(
                        unitOfWork,
                        _tenantService.ObterTipoServicoMultisoftware(),
                        _tenantService.ObterAuditado(Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.WebServicePedidos),
                        _tenantService.AdminStringConexao()
                    ).AdicionarAtendimento(adicionarAtendimento, cancellationToken);

                    await _repositorioSubtarefa.AtualizarStatusAsync(
                        subtarefa.Id,
                        StatusTarefa.Concluida,
                        retorno.Mensagem,
                        cancellationToken);

                    var resultadoObj = new BsonDocument
                    {
                        { "codigo", subtarefa.Id },
                        { "chave_nfe", adicionarAtendimento.NotaFiscal?.Chave ?? "" },
                        { "status_integracao", retorno.Status },
                        { "descricao", retorno.Mensagem ?? "" },
                        { "account", adicionarAtendimento.Observacao ?? "" },
                        { "protocolo", tarefaId }
                    };
                    resultadosConcurrent.Add(resultadoObj);
                }
                catch (BaseException excecao)
                {
                    await _repositorioSubtarefa.AtualizarStatusAsync(
                        subtarefa.Id,
                        StatusTarefa.Falha,
                        excecao.Message,
                        cancellationToken);
                }
                catch (Exception excecao)
                {
                    Log.TratarErro(excecao, "AdicionarAtendimento");
                    await _repositorioSubtarefa.AtualizarStatusAsync(
                        subtarefa.Id,
                        StatusTarefa.Falha,
                        "Erro ao processar Adicionar Atendimento.",
                        cancellationToken);
                }
            }
        }
    }
}