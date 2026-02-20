using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Vector.RecebimentoCarga
{
    public class Entrega
    {
        /// <summary>
        /// Nome do produto
        /// </summary>
        [JsonProperty("ProductDescription")]
        public string Descricao { get; set; }

        /// <summary>
        /// Número do NCM do produto
        /// (Nomenclatura Comum do Mercosul)
        /// </summary>
        [JsonProperty("NCM")]
        public string NCM { get; set; }

        /// <summary>
        /// Volume em Kg para ser transportado
        /// </summary>
        [JsonProperty("TotalQuantity")]
        public decimal QuantidadeTotal { get; set; }

        /// <summary>
        /// Nome da unidade de medida do produto
        /// Atualmente unificado: kg
        /// </summary>
        [JsonProperty("ProductUnitName")]
        public string UnidadeMedida { get; set; }

        /// <summary>
        /// Valor total da mercadoria entregada,
        /// necessário para consulta na GR
        /// </summary>
        [JsonProperty("MerchandiseValue")]
        public decimal ValorTotalMercadoria { get; set; }
    }
}
