using Newtonsoft.Json;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Vedacit.CargaDadosTransporte
{
    public class Pedido
    {
        /// <summary>
        /// Data e hora programadas para a entrega do pedido (formato: ISO 8601).
        /// </summary>
        [JsonProperty(PropertyName = "dtEntProgramada", Order = 0)]
        public string DataEntregaProgramada { get; set; }

        /// <summary>
        /// Data e hora programadas para a retirada do pedido (formato: ISO 8601).
        /// </summary>
        [JsonProperty(PropertyName = "dtRetProgramada", Order = 1)]
        public string DataRetiradaProgramada { get; set; }

        /// <summary>
        /// Código que representa o tipo de frete (ex: "CIF").
        /// </summary>
        [JsonProperty(PropertyName = "codFrete", Order = 2)]
        public string CodigoFrete { get; set; }

        /// <summary>
        /// Identificação do Warehouse. Caso o valor seja "00950'" o Warehouse é Itatiba, qualquer outro valor Salvador.
        /// </summary>
        [JsonProperty(PropertyName = "ciaPedido", Order = 3)]
        public string IdentificacaoPedido { get; set; }

        /// <summary>
        /// Tipo do pedido (ex: "SV").
        /// </summary>
        [JsonProperty(PropertyName = "tipPedido", Order = 4)]
        public string TipoPedido { get; set; }

        /// <summary>
        /// Número do pedido, com até 40 caracteres. Código da ordem externa 2.
        /// </summary>
        [JsonProperty(PropertyName = "nPedido", Order = 5)]
        public string NumeroPedido { get; set; }

        /// <summary>
        /// Data e hora em que o pedido foi solicitado (formato: ISO 8601).
        /// </summary>
        [JsonProperty(PropertyName = "dtSolicitacao", Order = 6)]
        public string DataSolicitacao { get; set; }

        /// <summary>
        /// Data e hora em que o pedido foi registrado (formato: ISO 8601).
        /// </summary>
        [JsonProperty(PropertyName = "dtPedido", Order = 7)]
        public string DataPedido { get; set; }

        /// <summary>
        /// Protocolo associado ao release do pedido.
        /// </summary>
        [JsonProperty(PropertyName = "protocoloRelease", Order = 8)]
        public string ProtocoloRelease { get; set; }

        /// <summary>
        /// Protocolo associado à carga do pedido.
        /// </summary>
        [JsonProperty(PropertyName = "protocoloCarga", Order = 9)]
        public string ProtocoloCarga { get; set; }

        /// <summary>
        /// Protocolo associado à carga do pedido.
        /// </summary>
        [JsonProperty(PropertyName = "numeroCarga", Order = 10)]
        public string NumeroCarga { get; set; }

        /// <summary>
        /// praca do pedido (Observação).
        /// </summary>
        [JsonProperty(PropertyName = "praca", Order = 11)]
        public string Praca { get; set; }

        /// <summary>
        /// Rota designada para a entrega, com até 11 caracteres.
        /// </summary>
        [JsonProperty(PropertyName = "rota", Order = 12)]
        public string Rota { get; set; }

        /// <summary>
        /// Código integração transportadora.
        /// </summary>
        [JsonProperty(PropertyName = "transportadora", Order = 13)]
        public string Transportadora { get; set; }

        /// <summary>
        /// Nome transportadora.
        /// </summary>
        [JsonProperty(PropertyName = "nomeTransportadora", Order = 14)]
        public string NomeTransportadora { get; set; }

        /// <summary>
        /// Lista de itens incluídos no pedido, enviar todos os itens do pedido nesta lista.
        /// </summary>
        [JsonProperty(PropertyName = "item", Order = 15)]
        public List<Item> Itens { get; set; }

        /// <summary>
        /// Informações sobre o endereço de envio.
        /// </summary>
        [JsonProperty(PropertyName = "refEnvio", Order = 16)]
        public ReferenciaEnvio ReferenciaEnvio { get; set; }

        /// <summary>
        /// Informações sobre o cliente que realizou o pedido.
        /// </summary>
        [JsonProperty(PropertyName = "cliente", Order = 17)]
        public Cliente Cliente { get; set; }

        /// <summary>
        /// Protocolo associado ao tipo de operação.
        /// </summary>
        [JsonProperty(PropertyName = "codOperacao", Order = 18)]
        public string CodigoOperacao { get; set; }
    }
}
