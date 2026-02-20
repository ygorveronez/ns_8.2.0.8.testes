using MongoDB.Bson.Serialization.Attributes;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao
{
    public class RetornoObjetoTarefa
    {
        [BsonElement("protocolo")]
        public string Protocolo { get; set; }

        [BsonElement("status")]
        public bool Status { get; set; }

        [BsonElement("mensagem")]
        public string Mensagem { get; set; }

        [BsonElement("codigo_carga_primeiro_trecho")]
        public int CodigoCargaPrimeiroTrecho { get; set; }

        [BsonElement("codigo")]
        public string Codigo { get; set; }
    }
}
