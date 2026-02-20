using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Vedacit.CargaDadosTransporte
{
    public class CampoPersonalizado
    {
        public Dado Item { get; set; }
    }

    public class Dado
    {
        /// <summary>
        /// Valor do release do item.
        /// </summary>
        [JsonProperty(PropertyName = "vlrRelease")]
        public double ValorRelease { get; set; }

        /// <summary>
        /// Quantidade do pedido para transporte.
        /// </summary>
        [JsonProperty(PropertyName = "qtPedidoTransf")]
        public string QuantidadePedidoTransporte { get; set; }

        /// <summary>
        /// Indica se o item está reservado ("S" para sim), com 1 caractere.
        /// </summary>
        [JsonProperty(PropertyName = "reservado")]
        public string Reservado { get; set; }

        /// <summary>
        /// Unidade de medida digitada, com até 2 caracteres.
        /// </summary>
        [JsonProperty(PropertyName = "unMedidaDigitada")]
        public string UnidadeMedidaDigitada { get; set; }

        /// <summary>
        /// Unidade de medida digitada, com até 2 caracteres.
        /// </summary>
        [JsonProperty(PropertyName = "UnMedidaPeso")]
        public string UnidadeMedidaPeso { get; set; }

        /// <summary>
        /// Unidade de medida digitada, com até 2 caracteres.
        /// </summary>
        [JsonProperty(PropertyName = "UnMedidaVolume")]
        public string UnidadeMedidaVolume { get; set; }

        /// <summary>
        /// Número da linha do item no pedido.
        /// </summary>
        [JsonProperty(PropertyName = "noLinha")]
        public double NumeroLinha { get; set; }
    }
}
