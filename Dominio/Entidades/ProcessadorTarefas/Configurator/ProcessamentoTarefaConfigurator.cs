using Dominio.Entidades.ProcessadorTarefas;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;

namespace Dominio.Entidades.ProcessadorTarefas.Configurator
{
    public class ProcessamentoTarefaConfigurator
    {
        public void RegisterClassMap()
        {
            if (!BsonClassMap.IsClassMapRegistered(typeof(ProcessamentoTarefa)))
            {
                BsonClassMap.RegisterClassMap<ProcessamentoTarefa>(cm =>
                {
                    cm.SetIgnoreExtraElements(true);
                    cm.SetDiscriminator("T_PROCESSAMENTO_TAREFA");
                    
                    cm.MapIdMember(c => c.Id)
                        .SetElementName("_id")
                        .SetSerializer(new MongoDB.Bson.Serialization.Serializers.StringSerializer(BsonType.ObjectId))
                        .SetIgnoreIfDefault(false);
                    
                    cm.AutoMap();
                    
                    cm.GetMemberMap(c => c.Status)
                        .SetElementName("PRO_STATUS")
                        .SetSerializer(new MongoDB.Bson.Serialization.Serializers.EnumSerializer<Dominio.ObjetosDeValor.ProcessadorTarefas.Enumeradores.StatusTarefa>(MongoDB.Bson.BsonType.String));
                    
                    cm.GetMemberMap(c => c.TipoRequest)
                        .SetElementName("PRO_TIPO_REQUEST")
                        .SetSerializer(new MongoDB.Bson.Serialization.Serializers.EnumSerializer<Dominio.ObjetosDeValor.ProcessadorTarefas.Enumeradores.TipoRequest>(MongoDB.Bson.BsonType.String));
                    
                    cm.GetMemberMap(c => c.EtapaAtual).SetElementName("PRO_ETAPA_ATUAL");
                    cm.GetMemberMap(c => c.RequestId)
                        .SetElementName("PRO_REQUEST_ID")
                        .SetSerializer(new MongoDB.Bson.Serialization.Serializers.StringSerializer(BsonType.ObjectId));
                    cm.GetMemberMap(c => c.CriadoEm).SetElementName("PRO_CRIADO_EM");
                    cm.GetMemberMap(c => c.AtualizadoEm).SetElementName("PRO_ATUALIZADO_EM");
                    cm.GetMemberMap(c => c.CodigoIntegradora).SetElementName("PRO_CODIGO_INTEGRADORA");
                    cm.GetMemberMap(c => c.JobId).SetElementName("PRO_JOB_ID");
                    cm.GetMemberMap(c => c.Etapas).SetElementName("PRO_ETAPAS");
                    cm.GetMemberMap(c => c.Resultado).SetElementName("PRO_RESULTADO");
                    cm.GetMemberMap(c => c.Versao).SetElementName("PRO_VERSAO");
                });
            }
        }
    }
}
