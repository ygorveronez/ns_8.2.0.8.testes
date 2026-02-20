using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.YMS
{
    public class IntegrarCargaDadosTransporte
    {
        [JsonProperty("id")]
        public int NumeroCarga { get; set; }
        public DateTime DataCriacao { get; set; }
        [JsonProperty("sender")]
        public Cliente Remetente { get; set; }
        [JsonProperty("recipient")]
        public List<Cliente> Destinatario { get; set; }
        [JsonProperty("transporter")]
        public Cliente Transportador { get; set; }
        public List<string> OrdensEmbarque { get; set; }
        public string TipoCarga { get; set; }
        [JsonProperty("operation")]
        public string Operation { get; set; }
        public string TipoOperacao { get; set; }
        public string TipoCargaTaura { get; set; }
        public DateTime? DataCarregamento { get; set; }
        public Temperatura RangeTemperaturaTransporte { get; set; }
        public string NumeroBooking { get; set; }
        [JsonProperty("vehicleType")]
        public string ModeloVeicular { get; set; }
        [JsonProperty("documentNumber")]
        public string CpfMotorista { get; set; }
        [JsonProperty("driverName")]
        public string NomeMotorista { get; set; }
        public double PesoTotalKg { get; set; }
        public double PesoLiquidoKg { get; set; }
        public List<PedidosPorRemetente> PedidosPorRemetente { get; set; }
        public List<Produtos> Produtos { get; set; }
        [JsonProperty("product")]
        public string Produto { get; set; }
        public string ProdutoCodigo { get; set; }
        public string NumeroContainer { get; set; }
        [JsonProperty("terminal")]
        public Cliente Filial { get; set; }

        [JsonProperty("plate")]
        public string Plate { get; set; }

        [JsonProperty("additionalPlates")]
        public List<AdditionalPlate> AdditionalPlates { get; set; }
    }

    public class Cliente
    {
        [JsonProperty("cnpj")]
        public string Cnpj { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }

    public class AdditionalPlate
    {
        [JsonProperty("plate")]
        public string Plate { get; set; }
    }
}
