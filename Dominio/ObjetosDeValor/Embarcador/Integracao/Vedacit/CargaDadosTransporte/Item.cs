using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Vedacit.CargaDadosTransporte
{
    public class Item
    {
        /// <summary>
        /// Valor do release do item.
        /// </summary>
        [JsonProperty(PropertyName = "vlrRelease", Order = 0)]
        public double ValorRelease { get; set; }

        /// <summary>
        /// Identificação do release do item, com até 20 caracteres. Código identificador da ordem externa do pedido.
        /// </summary>
        [JsonProperty(PropertyName = "release", Order = 1)]
        public string Release { get; set; }

        /// <summary>
        /// Unidade de medida do item (ex: "UN"), com até 2 caracteres.
        /// </summary>
        [JsonProperty(PropertyName = "unidade", Order = 2)]
        public string Unidade { get; set; }

        /// <summary>
        /// Quantidade do pedido para transporte.
        /// </summary>
        [JsonProperty(PropertyName = "qtPedidoTrans", Order = 3)]
        public string QuantidadePedidoTransporte { get; set; }

        /// <summary>
        /// Indica se o item está reservado ("S" para sim), com 1 caractere.
        /// </summary>
        [JsonProperty(PropertyName = "reservado", Order = 4)]
        public string Reservado { get; set; }

        /// <summary>
        /// Quantidade total do item.
        /// </summary>
        [JsonProperty(PropertyName = "qtde", Order = 5)]
        public string Quantidade { get; set; }

        /// <summary>
        /// Valor por unidade do item.
        /// </summary>
        [JsonProperty(PropertyName = "vlrPrecoUnidade", Order = 6)]
        public decimal ValorPrecoUnidade { get; set; }

        /// <summary>
        /// Unidade de medida digitada, com até 2 caracteres.
        /// </summary>
        [JsonProperty(PropertyName = "unMedidaDigitada", Order = 7)]
        public string UnidadeMedidaDigitada { get; set; }

        /// <summary>
        /// Código do produto (SKU).
        /// </summary>
        [JsonProperty(PropertyName = "codProduto", Order = 8)]
        public string CodigoProduto { get; set; }

        /// <summary>
        /// Unidade de medida de entrada, com até 2 caracteres.
        /// </summary>
        [JsonProperty(PropertyName = "unMedidaEntrada", Order = 9)]
        public string UnidadeMedidaEntrada { get; set; }

        /// <summary>
        /// Número da linha do item no pedido.
        /// </summary>
        [JsonProperty(PropertyName = "noLinha", Order = 10)]
        public double NumeroLinha { get; set; }
    }
}
