using Dominio.Entidades.ProcessadorTarefas;
using Dominio.Interfaces.ProcessadorTarefas;
using Dominio.Interfaces.Repositorios.ProcessadorTarefas;
using Dominio.ObjetosDeValor.ProcessadorTarefas;
using Dominio.ObjetosDeValor.ProcessadorTarefas.Enumeradores;
using Hangfire;
using Hangfire.Common;
using Hangfire.States;
using MongoDB.Bson;
using MongoDB.Driver;
using Repositorio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Utilidades.Extensions;

namespace Servicos.ProcessadorTarefas
{
    public class AdicionarRequestAssincrono : IAdicionarRequestAssincrono
    {
        private readonly IRequestDocumentoRepository _repositorioRequestDocumento;
        private readonly IProcessamentoTarefaRepository _repositorioTarefa;
        private readonly ITarefaIntegracao _repositorioTarefaIntegracao;
        private readonly Dominio.Interfaces.Database.ITenantService _tenantService;
        private readonly IBackgroundJobClient _backgroundJobClient;

        public AdicionarRequestAssincrono(IRequestDocumentoRepository repositorioRequestDocumento, IProcessamentoTarefaRepository repositorioTarefa, ITarefaIntegracao repositorioTarefaIntegracao, Dominio.Interfaces.Database.ITenantService tenantService, IBackgroundJobClient backgroundJobClient)
        {
            _repositorioRequestDocumento = repositorioRequestDocumento;
            _repositorioTarefa = repositorioTarefa;
            _repositorioTarefaIntegracao = repositorioTarefaIntegracao;
            _tenantService = tenantService;
            _backgroundJobClient = backgroundJobClient;
        }

        public async Task<RetornoAdicionarRequestAssincrono> SalvarAsync<T>(T objeto, TipoRequest tipoRequest, List<TipoEtapaTarefa> tiposEtapas, CancellationToken cancellationToken, int codigoIntegradora = 0) where T : class
        {
            var requestDoc = new RequestDocumento
            {
                Id = ObjectId.GenerateNewId().ToString(),
                Tipo = tipoRequest,
                Dados = objeto.ToBsonDocument(),
                CriadoEm = DateTime.UtcNow,
                ExpiraEm = DateTime.UtcNow.AddDays(30)
            };

            var tarefa = new ProcessamentoTarefa
            {
                Id = ObjectId.GenerateNewId().ToString(),
                RequestId = requestDoc.Id,
                TipoRequest = tipoRequest,
                Status = StatusTarefa.Aguardando,
                EtapaAtual = 0,
                CriadoEm = DateTime.UtcNow,
                AtualizadoEm = DateTime.UtcNow,
                CodigoIntegradora = codigoIntegradora > 0 ? (int?)codigoIntegradora : null,
                Versao = 1,
                Etapas = tiposEtapas.Select(tipo => new EtapaInfo
                {
                    Tipo = tipo,
                    Status = StatusTarefa.Aguardando,
                    Tentativas = 0
                }).ToList()
            };

            string fila = tiposEtapas[0].ObterFila();

            await _repositorioRequestDocumento.InserirUmAsync(requestDoc, cancellationToken);
            await _repositorioTarefa.InserirUmAsync(tarefa, cancellationToken);

            var job = Job.FromExpression<OrquestradorTarefas>(x => x.IniciarProcessamentoAsync(tarefa.Id, requestDoc.Id));
            string jobId = _backgroundJobClient.Create(job, new EnqueuedState(fila));

            var updateJobId = Builders<ProcessamentoTarefa>.Update.Set(t => t.JobId, jobId);
            await _repositorioTarefa.AtualizarAsync(tarefa.Id, updateJobId, cancellationToken);

            _ = Task.Run(async () =>
            {
                try
                {
                    if (codigoIntegradora > 0)
                    {
                        await AdicionarIntegracoesRetorno(tarefa, codigoIntegradora, CancellationToken.None);
                    }
                }
                catch (Exception ex)
                {
                    Log.TratarErro(ex, "AdicionarRequestAssincrono - Integrações");
                }
            }, CancellationToken.None);

            return new RetornoAdicionarRequestAssincrono
            {
                Protocolo = tarefa.Id,
                JobId = jobId
            };
        }

        public async Task<RetornoAdicionarRequestAssincrono> SalvarLoteAsync<T>(List<T> objetos, TipoRequest tipoRequest, List<TipoEtapaTarefa> tiposEtapas, CancellationToken cancellationToken, int codigoIntegradora = 0) where T : class
        {
            return await SalvarAsync(objetos, tipoRequest, tiposEtapas, cancellationToken, codigoIntegradora);
        }

        private async Task AdicionarIntegracoesRetorno(ProcessamentoTarefa tarefa, int codigoIntegradora, CancellationToken cancellationToken)
        {
            Repositorio.WebService.IntegradoraIntegracao repositorioIntegradoraIntegracao = new Repositorio.WebService.IntegradoraIntegracao(new UnitOfWork(_tenantService.StringConexao()), cancellationToken);

            List<Dominio.Entidades.WebService.IntegradoraIntegracao> integracoes = await repositorioIntegradoraIntegracao.BuscarPorIntegradoraAsync(codigoIntegradora);

            if (integracoes.IsNullOrEmpty())
                return;

            List<TarefaIntegracao> listaIntegracoes = new List<TarefaIntegracao>();

            foreach (Dominio.Entidades.WebService.IntegradoraIntegracao integracao in integracoes)
            {
                listaIntegracoes.Add(new TarefaIntegracao
                {
                    IdTarefa = tarefa.Id,
                    TipoIntegracao = integracao.Tipo
                });
            }

            await _repositorioTarefaIntegracao.InserirMuitosAsync(listaIntegracoes, cancellationToken);
        }
    }
}
