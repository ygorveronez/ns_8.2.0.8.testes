using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.FS.IntegracaoCarga.Requisicao
{
    public class Carga
    {
        [JsonProperty(PropertyName = "TIPO_OPERACAO")]
        public string TipoOperacao { get; set; }

        [JsonProperty(PropertyName = "TIPO_CARGA")]
        public string TipoCarga { get; set; }

        [JsonProperty(PropertyName = "DATA_CARREGAMENTO")]
        public string DataCarregamento { get; set; }

        [JsonProperty(PropertyName = "ID_CONTRATO")]
        public string CodigoContrato { get; set; }

        [JsonProperty(PropertyName = "ORIGEM")]
        public string Origem { get; set; }

        [JsonProperty(PropertyName = "DESTINATARIO")]
        public string Destinatario { get; set; }

        [JsonProperty(PropertyName = "DESTINO_ENTREGA")]
        public string DestinoEntrega { get; set; }

        [JsonProperty(PropertyName = "COD_PRODUTO")]
        public string CodigoProduto { get; set; }

        [JsonProperty(PropertyName = "VOLUME")]
        public string Volume { get; set; }

        [JsonProperty(PropertyName = "TRANSPORTADORA")]
        public string Transportadora { get; set; }

        [JsonProperty(PropertyName = "PLACA_CAVALO")]
        public string PlacaCavalo { get; set; }

        [JsonProperty(PropertyName = "PLACA_CARRETA")]
        public string PlacaCarreta { get; set; }

        [JsonProperty(PropertyName = "PLACA_3")]
        public string Placa3 { get; set; }

        [JsonProperty(PropertyName = "MOTORISTA")]
        public string Motorista { get; set; }

        [JsonProperty(PropertyName = "UF_PLACA")]
        public string UfPlaca { get; set; }

        [JsonProperty(PropertyName = "TARIFA")]
        public string Tarifa { get; set; }

        [JsonProperty(PropertyName = "PROTOCOLO_CARGA")]
        public string ProtocoloCarga { get; set; }

        [JsonProperty(PropertyName = "PROTOCOLO_PEDIDO")]
        public string ProtocoloPedido { get; set; }

        [JsonProperty(PropertyName = "NUMERO_CARGA_ETANOL")]
        public string NumeroCargaEtanol { get; set; }

        [JsonProperty(PropertyName = "CNPJ_ECE")]
        public string CnpjEce { get; set; }

        [JsonProperty(PropertyName = "DESCRICAO_FILIAL")]
        public string DescricaoFilial { get; set; }

        [JsonProperty(PropertyName = "IBGE")]
        public string Ibge { get; set; }

        [JsonProperty(PropertyName = "UF")]
        public string Uf { get; set; }

        [JsonProperty(PropertyName = "CNPJ_DESCARGA")]
        public string CNPJDescarga { get; set; }
    }
}
