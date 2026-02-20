using Dominio.Entidades.ProcessadorTarefas;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;

namespace Dominio.Entidades.ProcessadorTarefas.Configurator
{
    public class EtapaInfoConfigurator
    {
        public void RegisterClassMap()
        {
            if (!BsonClassMap.IsClassMapRegistered(typeof(EtapaInfo)))
            {
                BsonClassMap.RegisterClassMap<EtapaInfo>(cm =>
                {
                    cm.AutoMap();
                    cm.SetIgnoreExtraElements(true);
                    cm.SetDiscriminator("T_ETAPA_INFO");
                    
                    cm.GetMemberMap(c => c.Tipo)
                        .SetElementName("ETA_TIPO")
                        .SetSerializer(new MongoDB.Bson.Serialization.Serializers.EnumSerializer<Dominio.ObjetosDeValor.ProcessadorTarefas.Enumeradores.TipoEtapaTarefa>(BsonType.String));
                    
                    cm.GetMemberMap(c => c.Status)
                        .SetElementName("ETA_STATUS")
                        .SetSerializer(new MongoDB.Bson.Serialization.Serializers.EnumSerializer<Dominio.ObjetosDeValor.ProcessadorTarefas.Enumeradores.StatusTarefa>(BsonType.String));
                    
                    cm.GetMemberMap(c => c.Tentativas).SetElementName("ETA_TENTATIVAS");
                    cm.GetMemberMap(c => c.IniciadoEm).SetElementName("ETA_INICIADO_EM");
                    cm.GetMemberMap(c => c.ConcluidoEm).SetElementName("ETA_CONCLUIDO_EM");
                    cm.GetMemberMap(c => c.Mensagem).SetElementName("ETA_MENSAGEM");
                    cm.GetMemberMap(c => c.DuracaoMs).SetElementName("ETA_DURACAO_MS");
                });
            }
        }
    }
}

