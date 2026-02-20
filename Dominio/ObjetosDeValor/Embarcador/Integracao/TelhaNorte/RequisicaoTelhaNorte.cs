using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.TelhaNorte
{
    public sealed class RequisicaoTelhaNorte
    {
        [JsonProperty(PropertyName = "protocolo_carga", Order = 1, Required = Required.Always)]
        public int ProtocoloCarga { get; set; }

        [JsonProperty(PropertyName = "numero_carga", Order = 8, Required = Required.Always)]
        public string NumeroCarga { get; set; }
        
        [JsonProperty(PropertyName = "tipo_operacao", Order = 8, Required = Required.Always)]
        public string TipoOperacao { get; set; }
        
        [JsonProperty(PropertyName = "local_transporte", Order = 8, Required = Required.Always)]
        public string LocalTransporte { get; set; }
        
        [JsonProperty(PropertyName = "tipo_veiculo", Order = 8, Required = Required.Always)]
        public string TipoVeiculo { get; set; }
        
        [JsonProperty(PropertyName = "usuario", Order = 8, Required = Required.Always)]
        public string Usuario { get; set; }
        
        [JsonProperty(PropertyName = "placa_veiculo", Order = 8, Required = Required.Always)]
        public string PlacaVeiculo { get; set; }
        
        [JsonProperty(PropertyName = "cpf_motorista", Order = 8, Required = Required.Always)]
        public string CPFMotorista { get; set; }
        
        [JsonProperty(PropertyName = "cnpj_transportadora", Order = 8, Required = Required.Always)]
        public string CNPJTransportador { get; set; }
        
        [JsonProperty(PropertyName = "simular", Order = 8, Required = Required.AllowNull)]
        public string Simular { get; set; }
        
        [JsonProperty(PropertyName = "data_carregamento", Order = 8, Required = Required.Always)]
        [JsonConverter(typeof(Utilidades.Json.DateTimeConverter), new object[] { "yyyy-MM-ddTHH:mm:ss" })]
        public DateTime? DataCarregamento { get; set; }
        
        [JsonProperty(PropertyName = "pedidos", Order = 8, Required = Required.Always)]
        public List<RequisicaoTelhaNortePedido> Pedidos { get; set; }
    }
}