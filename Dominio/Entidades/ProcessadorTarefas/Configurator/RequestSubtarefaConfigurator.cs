using Dominio.Entidades.ProcessadorTarefas;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;

namespace Dominio.Entidades.ProcessadorTarefas.Configurator
{
    public class RequestSubtarefaConfigurator
    {
        public void RegisterClassMap()
        {
            if (!BsonClassMap.IsClassMapRegistered(typeof(RequestSubtarefa)))
            {
                BsonClassMap.RegisterClassMap<RequestSubtarefa>(cm =>
                {
                    cm.SetIgnoreExtraElements(true);
                    cm.SetDiscriminator("T_REQUEST_SUBTAREFA");
                    
                    cm.MapIdMember(c => c.Id)
                        .SetElementName("_id")
                        .SetSerializer(new MongoDB.Bson.Serialization.Serializers.StringSerializer(BsonType.ObjectId))
                        .SetIgnoreIfDefault(false);
                    
                    cm.AutoMap();
                    
                    cm.GetMemberMap(c => c.TarefaId)
                        .SetElementName("RES_TAREFA_ID")
                        .SetSerializer(new MongoDB.Bson.Serialization.Serializers.StringSerializer(BsonType.ObjectId));
                    
                    cm.GetMemberMap(c => c.Dados).SetElementName("RES_DADOS");
                    
                    cm.GetMemberMap(c => c.Status)
                        .SetElementName("RES_STATUS")
                        .SetSerializer(new MongoDB.Bson.Serialization.Serializers.EnumSerializer<Dominio.ObjetosDeValor.ProcessadorTarefas.Enumeradores.StatusTarefa>(BsonType.String));
                    
                    cm.GetMemberMap(c => c.Ordem).SetElementName("RES_ORDEM");
                    cm.GetMemberMap(c => c.Tentativas).SetElementName("RES_TENTATIVAS");
                    cm.GetMemberMap(c => c.DataCriacao).SetElementName("RES_DATA_CRIACAO");
                    cm.GetMemberMap(c => c.Mensagem).SetElementName("RES_MENSAGEM");
                    cm.GetMemberMap(c => c.Resultado).SetElementName("RES_RESULTADO");
                });
            }
        }
    }
}

