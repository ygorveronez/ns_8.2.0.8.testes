using Newtonsoft.Json;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Hub.RetornoOferta
{
    public class CompartimentoCarga
    {
        [JsonProperty("cargoCompartment")]
        public Tipo CargoCompartment { get; set; }

        [JsonProperty("measurementUnits")]
        public List<UnidadeMedidaCompartimento> UnidadesMedida { get; set; }

        [JsonProperty("accessories")]
        public List<Tipo> Acessorios { get; set; }

        [JsonProperty("hazardClasses")]
        public List<Tipo> ClassesRisco { get; set; }
    }
}
