using Dominio.Entidades.ProcessadorTarefas;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;

namespace Dominio.Entidades.ProcessadorTarefas.Configurator
{
    public class ArquivoIntegracaoConfigurator
    {
        public void RegisterClassMap()
        {
            if (!BsonClassMap.IsClassMapRegistered(typeof(ArquivoIntegracao)))
            {
                BsonClassMap.RegisterClassMap<ArquivoIntegracao>(cm =>
                {
                    cm.SetIgnoreExtraElements(true);
                    cm.SetDiscriminator("T_ARQUIVO_INTEGRACAO");
                    
                    cm.AutoMap();
                    
                    cm.GetMemberMap(c => c.Identifcador).SetElementName("AI_ID");
                    cm.GetMemberMap(c => c.ArquivoRequisicao).SetElementName("AI_ARQUIVO_REQUISICAO");
                    cm.GetMemberMap(c => c.ArquivoResposta).SetElementName("AI_ARQUIVO_RESPOSTA");
                    cm.GetMemberMap(c => c.Tipo).SetElementName("AI_TIPO");
                    cm.GetMemberMap(c => c.DataCriacao).SetElementName("AI_DATA_CRIACAO");
                });
            }
        }
    }
}

