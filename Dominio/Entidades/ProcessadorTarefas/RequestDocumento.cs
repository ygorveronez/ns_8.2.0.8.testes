using Dominio.ObjetosDeValor.ProcessadorTarefas.Enumeradores;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Dominio.Entidades.ProcessadorTarefas
{
    [BsonIgnoreExtraElements]
    public class RequestDocumento
    {
        public RequestDocumento()
        {
            Id = ObjectId.GenerateNewId().ToString();
            CriadoEm = DateTime.UtcNow;
            ExpiraEm = DateTime.UtcNow.AddDays(30);
        }

        [BsonId]
        public string Id { get; set; }

        public TipoRequest Tipo { get; set; }

        public BsonDocument Dados { get; set; }

        public DateTime CriadoEm { get; set; }

        public DateTime ExpiraEm { get; set; }

        [BsonIgnore]
        public string Codigo
        {
            get { return Id; }
        }

        [BsonIgnore]
        public string TipoFormatado
        {
            get { return Tipo.ObterDescricao(); }
        }

        [BsonIgnore]
        public string CriadoEmFormatado
        {
            get { return CriadoEm.ToLocalTime().ToString("dd/MM/yyyy HH:mm:ss"); }
        }
    }
}

