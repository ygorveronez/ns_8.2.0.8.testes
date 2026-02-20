using Dominio.Entidades.ProcessadorTarefas;
using Dominio.Interfaces.Repositorios;
using Dominio.Interfaces.Repositorios.ProcessadorTarefas;
using MongoDB.Driver;
using Repositorio.Global.Banco;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.ProcessadorTarefas
{
    public class RequestDocumentoRepository : RepositorioBaseMongo<RequestDocumento>, IRequestDocumentoRepository
    {
        public RequestDocumentoRepository(IMongoDbContext context) : base(context)
        {
        }

        public async Task<RequestDocumento> ObterPorIdAsync(string id, CancellationToken cancellationToken)
        {
            var filter = Builders<RequestDocumento>.Filter.Eq(r => r.Id, id);

            return Session != null
                ? await Collection.Find(Session, filter).FirstOrDefaultAsync(cancellationToken)
                : await Collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
        }

        public async Task CriarIndicesAsync()
        {
            var idx1 = Builders<RequestDocumento>.IndexKeys
                .Ascending(r => r.ExpiraEm);

            await Collection.Indexes.CreateOneAsync(
                new CreateIndexModel<RequestDocumento>(idx1,
                    new CreateIndexOptions
                    {
                        Name = "idx_ttl_expira_em",
                        ExpireAfter = TimeSpan.Zero,
                        Background = true
                    })
            );

            var idx2 = Builders<RequestDocumento>.IndexKeys
                .Ascending(r => r.Tipo)
                .Descending(r => r.CriadoEm);

            await Collection.Indexes.CreateOneAsync(
                new CreateIndexModel<RequestDocumento>(idx2,
                    new CreateIndexOptions
                    {
                        Name = "idx_tipo_data",
                        Background = true
                    })
            );
        }
    }
}

