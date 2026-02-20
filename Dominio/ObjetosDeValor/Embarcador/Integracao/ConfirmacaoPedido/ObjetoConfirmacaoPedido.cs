using MongoDB.Bson.Serialization.Attributes;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.ConfirmacaoPedido
{
    public class ObjetoConfirmacaoPedido
    {
        [BsonElement("codigo")]
        public string Codigo { get; set; }

        [BsonElement("numero_pedido")]
        public string NumeroPedido { get; set; }

        [BsonElement("protocolo")]
        public int Protocolo { get; set; }

        [BsonElement("protocolo_integracao_filial")]
        public string ProtocoloIntegracaoFilial { get; set; }
    }
}
