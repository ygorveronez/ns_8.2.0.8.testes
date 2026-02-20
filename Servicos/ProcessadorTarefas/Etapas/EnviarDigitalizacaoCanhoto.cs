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
    public class EnviarDigitalizacaoCanhoto : EtapaState
    {
        private readonly ITenantService _tenantService;
        private readonly IRequestSubtarefaRepository _repositorioSubtarefa;
        private readonly IProcessamentoTarefaRepository _repositorioTarefa;
        private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(5, 5);

        public EnviarDigitalizacaoCanhoto(
            ITenantService tenantService,
            IRequestSubtarefaRepository repositorioSubtarefa,
            IProcessamentoTarefaRepository repositorioTarefa)
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
            var canhotosArray = resultadosDoc.Contains("canhotos")
                ? resultadosDoc["canhotos"].AsBsonArray
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
                canhotosArray.Add(resultado);
            }

            resultadosDoc["canhotos"] = canhotosArray;
            var update = Builders<ProcessamentoTarefa>.Update
                .Set(t => t.Resultado, resultadosDoc);
            await _repositorioTarefa.AtualizarAsync(contexto.TarefaId, update, cancellationToken);

            var todasSubtarefas = await _repositorioSubtarefa.ObterPorTarefaIdAsync(contexto.TarefaId, cancellationToken);
            var subtarefasPendentes = todasSubtarefas.Where(s => s.Status == StatusTarefa.Aguardando || s.Status == StatusTarefa.EmProcessamento).ToList();
            
            if (subtarefasPendentes.Any() && contexto.Tarefa.Etapas[contexto.IndiceEtapa].Tentativas < 5)
            {
                throw new ServicoException("Ocorreu um erro ao processar envio de digitalização de canhotos.");
            }
        }

        private async Task ProcessarSubtarefaAsync(
            RequestSubtarefa subtarefa,
            ConcurrentBag<BsonDocument> resultadosConcurrent,
            string tarefaId,
            CancellationToken cancellationToken)
        {
            using (var unitOfWork = new Repositorio.UnitOfWork(
                _tenantService.StringConexao(),
                Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova))
            {
                try
                {
                    Dominio.ObjetosDeValor.Embarcador.NFe.Canhoto canhotoIntegracao =
                        subtarefa.Dados.FromBsonDocument<Dominio.ObjetosDeValor.Embarcador.NFe.Canhoto>();

                    Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork, cancellationToken);
                    Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = await repositorioConfiguracaoTMS.BuscarConfiguracaoPadraoAsync();
                    Dominio.Entidades.WebService.Integradora integradora = null;

                    Dominio.ObjetosDeValor.WebService.Retorno<int> retorno = await new WebService.Canhoto.Canhoto(
                        unitOfWork,
                        _tenantService.ObterTipoServicoMultisoftware(),
                        _tenantService.ObterCliente(),
                        _tenantService.ObterAuditado(Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.WebServicePedidos),
                        _tenantService.AdminStringConexao()
                    ).EnviarDigitalizacaoCanhotoAsync(canhotoIntegracao, integradora, cancellationToken);

                    bool sucesso = retorno.CodigoMensagem == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.Sucesso;

                    await _repositorioSubtarefa.AtualizarStatusAsync(
                        subtarefa.Id,
                        StatusTarefa.Concluida,
                        retorno.Mensagem,
                        cancellationToken);

                    var canhotoDoc = new BsonDocument
                    {
                        { "codigo", subtarefa.Id },
                        { "chave_nfe", canhotoIntegracao?.ChaveAcesso ?? "" },
                        { "status_integracao", sucesso },
                        { "descricao", retorno.Mensagem ?? "" },
                        { "protocolo", tarefaId }
                    };
                    resultadosConcurrent.Add(canhotoDoc);
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
                    Log.TratarErro(excecao, "EnviarDigitalizacaoCanhoto");
                    await _repositorioSubtarefa.AtualizarStatusAsync(
                        subtarefa.Id,
                        StatusTarefa.Falha,
                        "Erro ao processar envio de digitalização de canhoto.",
                        cancellationToken);
                }
            }
        }
    }
}
