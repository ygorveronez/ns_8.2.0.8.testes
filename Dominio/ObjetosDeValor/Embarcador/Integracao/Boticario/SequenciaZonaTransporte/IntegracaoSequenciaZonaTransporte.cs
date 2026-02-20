using Newtonsoft.Json;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Boticario.SequenciaZonaTransporte
{
    public class IntegracaoSequenciaZonaTransporte
    {
        [JsonProperty(PropertyName = "numeroCargaConsolidada", Order = 1, Required = Required.Always)]
        public string NumeroCargaConsolidada { get; set; }

        [JsonProperty(PropertyName = "cargas", Order = 2, Required = Required.Always)]
        public List<Carga> Cargas { get; set; }

        [JsonProperty(PropertyName = "notasFiscais", Order = 3, Required = Required.AllowNull)]
        public List<NotasFiscais> NotasFiscais { get; set; }

        [JsonProperty(PropertyName = "transportadora", Order = 4, Required = Required.Always)]
        public Transportadora Transportadora { get; set; }

        [JsonProperty(PropertyName = "coleta", Order = 5, Required = Required.Always)]
        public Coleta Coleta { get; set; }

        [JsonProperty(PropertyName = "motorista", Order = 6, Required = Required.Always)]
        public Motorista Motorista { get; set; }

        [JsonProperty(PropertyName = "caminhao", Order = 7, Required = Required.Always)]
        public Caminhao Caminhao { get; set; }
    }
}
