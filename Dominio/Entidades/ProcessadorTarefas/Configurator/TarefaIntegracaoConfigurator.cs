using Dominio.Entidades.ProcessadorTarefas;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;

namespace Dominio.Entidades.ProcessadorTarefas.Configurator
{
    public class TarefaIntegracaoConfigurator
    {
        public void RegisterClassMap()
        {
            if (!BsonClassMap.IsClassMapRegistered(typeof(TarefaIntegracao)))
            {
                BsonClassMap.RegisterClassMap<TarefaIntegracao>(cm =>
                {
                    cm.SetIgnoreExtraElements(true);
                    cm.SetDiscriminator("T_TAREFA_INTEGRACAO");
                    
                    cm.MapIdMember(c => c.Id)
                        .SetElementName("_id")
                        .SetSerializer(new MongoDB.Bson.Serialization.Serializers.StringSerializer(BsonType.ObjectId))
                        .SetIgnoreIfDefault(false);
                    
                    cm.MapMember(c => c.IdTarefa).SetElementName("AIP_ID_TAREFA");
                    cm.MapMember(c => c.Arquivos).SetElementName("AIP_ARQUIVOS");
                    cm.MapMember(c => c.DataCriacao).SetElementName("AIP_DATA_CRIACAO");
                    
                    cm.AutoMap();
                });
            }
        }
    }
}

