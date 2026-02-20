using Dominio.Entidades.ProcessadorTarefas;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;

namespace Dominio.Entidades.ProcessadorTarefas.Configurator
{
    public class IntegracaoConfigurator
    {
        public void RegisterClassMap()
        {
            if (!BsonClassMap.IsClassMapRegistered(typeof(Integracao)))
            {
                BsonClassMap.RegisterClassMap<Integracao>(cm =>
                {
                    cm.SetIgnoreExtraElements(true);
                    cm.SetDiscriminator("T_INTEGRACAO");
                    
                    cm.AutoMap();
                    
                    cm.GetMemberMap(c => c.DataIntegracao).SetElementName("AIP_DATA_INTEGRACAO");
                    cm.GetMemberMap(c => c.SituacaoIntegracao)
                        .SetElementName("AIP_SITUACAO_INTEGRACAO")
                        .SetSerializer(new MongoDB.Bson.Serialization.Serializers.EnumSerializer<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao>(BsonType.String));
                    cm.GetMemberMap(c => c.TipoIntegracao)
                        .SetElementName("AIP_TIPO_INTEGRACAO")
                        .SetSerializer(new MongoDB.Bson.Serialization.Serializers.EnumSerializer<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao>(BsonType.String));
                    cm.GetMemberMap(c => c.Tentativas).SetElementName("AIP_TENTATIVAS");
                    cm.GetMemberMap(c => c.ProblemaIntegracao).SetElementName("AIP_MENSAGEM");
                });
            }
        }
    }
}

