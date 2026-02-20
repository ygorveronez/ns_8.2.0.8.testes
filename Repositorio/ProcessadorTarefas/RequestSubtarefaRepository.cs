using Dominio.Entidades.ProcessadorTarefas;
using Dominio.Interfaces.Repositorios;
using Dominio.Interfaces.Repositorios.ProcessadorTarefas;
using Dominio.ObjetosDeValor.ProcessadorTarefas.Enumeradores;
using MongoDB.Driver;
using Repositorio.Global.Banco;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.ProcessadorTarefas
{

    public class RequestSubtarefaRepository : RepositorioBaseMongo<RequestSubtarefa>, IRequestSubtarefaRepository
    {
        public RequestSubtarefaRepository(IMongoDbContext context) : base(context)
        {
        }

        public async Task<List<RequestSubtarefa>> ObterPorTarefaIdAsync(string tarefaId, CancellationToken cancellationToken)
        {
            var filter = Builders<RequestSubtarefa>.Filter.Eq(s => s.TarefaId, tarefaId);

            var query = Session != null
                ? Collection.Find(Session, filter)
                : Collection.Find(filter);

            return await query
                .Sort(Builders<RequestSubtarefa>.Sort.Ascending(s => s.Ordem))
                .ToListAsync(cancellationToken);
        }

        public async Task<List<RequestSubtarefa>> ObterPendentesPorTarefaIdAsync(string tarefaId, CancellationToken cancellationToken)
        {
            var filter = Builders<RequestSubtarefa>.Filter.And(
                Builders<RequestSubtarefa>.Filter.Eq(s => s.TarefaId, tarefaId),
                Builders<RequestSubtarefa>.Filter.Ne(s => s.Status, StatusTarefa.Concluida)
            );

            var query = Session != null
                ? Collection.Find(Session, filter)
                : Collection.Find(filter);

            return await query
                .Sort(Builders<RequestSubtarefa>.Sort.Ascending(s => s.Ordem))
                .ToListAsync(cancellationToken);
        }

        public async Task AtualizarStatusAsync(string id, StatusTarefa status, string mensagem, CancellationToken cancellationToken)
        {
            var filter = Builders<RequestSubtarefa>.Filter.Eq(s => s.Id, id);

            var update = Builders<RequestSubtarefa>.Update
                .Set(s => s.Status, status)
                .Set(s => s.Mensagem, mensagem);

            if (Session != null)
            {
                await Collection.UpdateOneAsync(Session, filter, update, new UpdateOptions(), cancellationToken);
            }
            else
            {
                await Collection.UpdateOneAsync(filter, update, new UpdateOptions(), cancellationToken);
            }
        }

        public async Task CriarIndicesAsync()
        {
            var idx1 = Builders<RequestSubtarefa>.IndexKeys
                .Ascending(s => s.TarefaId)
                .Ascending(s => s.Status)
                .Ascending(s => s.Ordem);

            await Collection.Indexes.CreateOneAsync(
                new CreateIndexModel<RequestSubtarefa>(idx1,
                    new CreateIndexOptions
                    {
                        Name = "idx_tarefa_status_ordem",
                        Background = true
                    })
            );

            var idx2 = Builders<RequestSubtarefa>.IndexKeys
                .Ascending(s => s.TarefaId)
                .Ascending(s => s.Ordem);

            await Collection.Indexes.CreateOneAsync(
                new CreateIndexModel<RequestSubtarefa>(idx2,
                    new CreateIndexOptions
                    {
                        Name = "idx_tarefa_ordem",
                        Background = true
                    })
            );
        }
    }
}

