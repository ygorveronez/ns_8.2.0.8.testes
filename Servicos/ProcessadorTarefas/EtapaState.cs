using Dominio.ObjetosDeValor.ProcessadorTarefas;
using MongoDB.Bson.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Utilidades.Extensions;

namespace Servicos.ProcessadorTarefas
{
    public abstract class EtapaState
    {
        public abstract Task ExecutarAsync(ContextoEtapa contexto, CancellationToken cancellationToken);

        protected T ObterRequest<T>(ContextoEtapa contexto) where T : class
        {
            if (contexto?.RequestDoc?.Dados == null)
                return null;

            return contexto.RequestDoc.Dados.FromBsonDocument<T>();
        }

        protected T ObterRequestDireto<T>(ContextoEtapa contexto) where T : class
        {
            if (contexto?.RequestDoc?.Dados == null)
                return null;

            return BsonSerializer.Deserialize<T>(contexto.RequestDoc.Dados);
        }
    }
}
