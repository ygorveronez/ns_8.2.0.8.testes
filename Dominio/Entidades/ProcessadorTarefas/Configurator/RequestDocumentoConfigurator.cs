using Dominio.Entidades.ProcessadorTarefas;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;

namespace Dominio.Entidades.ProcessadorTarefas.Configurator
{
    public class RequestDocumentoConfigurator
    {
        public void RegisterClassMap()
        {
            if (!BsonClassMap.IsClassMapRegistered(typeof(RequestDocumento)))
            {
                BsonClassMap.RegisterClassMap<RequestDocumento>(cm =>
                {
                    cm.SetIgnoreExtraElements(true);
                    cm.SetDiscriminator("T_REQUEST_DOCUMENTO");
                    
                    cm.MapIdMember(c => c.Id)
                        .SetElementName("_id")
                        .SetSerializer(new MongoDB.Bson.Serialization.Serializers.StringSerializer(BsonType.ObjectId))
                        .SetIgnoreIfDefault(false);
                    
                    cm.AutoMap();
                    
                    cm.GetMemberMap(c => c.Tipo)
                        .SetElementName("REQ_TIPO")
                        .SetSerializer(new MongoDB.Bson.Serialization.Serializers.EnumSerializer<Dominio.ObjetosDeValor.ProcessadorTarefas.Enumeradores.TipoRequest>(BsonType.String));
                    
                    cm.GetMemberMap(c => c.Dados).SetElementName("REQ_DADOS");
                    cm.GetMemberMap(c => c.CriadoEm).SetElementName("REQ_CRIADO_EM");
                    cm.GetMemberMap(c => c.ExpiraEm).SetElementName("REQ_EXPIRA_EM");
                });
            }
        }
    }
}

