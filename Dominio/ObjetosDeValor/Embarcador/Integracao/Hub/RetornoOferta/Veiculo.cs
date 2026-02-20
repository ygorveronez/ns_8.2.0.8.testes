using Newtonsoft.Json;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Hub.RetornoOferta
{
    public class Veiculo
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("organizationId")]
        public string IdOrganizacao { get; set; }

        [JsonProperty("year")]
        public int Ano { get; set; }

        [JsonProperty("yearModel")]
        public int AnoModelo { get; set; }

        [JsonProperty("model")]
        public Modelo Modelo { get; set; }

        [JsonProperty("owner")]
        public Proprietario Proprietario { get; set; }

        [JsonProperty("color")]
        public Tipo Cor { get; set; }

        [JsonProperty("vehicleLand")]
        public VeiculoTerrestre VeiculoTerrestre { get; set; }

        [JsonProperty("status")]
        public StatusVeiculo StatusVeiculo { get; set; }

        [JsonProperty("crew")]
        public List<EquipeVeiculo> Equipe { get; set; }

        [JsonProperty("cargoCompartments")]
        public List<CompartimentoCarga> CompartimentosCarga { get; set; }
    }
}
