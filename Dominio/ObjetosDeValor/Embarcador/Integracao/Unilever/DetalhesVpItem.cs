using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Unilever
{
    public class DetalhesVpItem
    {
        [JsonProperty("stageID")]
        public int StageID { get; set; }

        [JsonProperty("plateTRCK")]
        public string PlateTRCK { get; set; }

        [JsonProperty("plateBTRA")]
        public string PlateBTRA { get; set; }

        [JsonProperty("axlesTRCK")]
        public string AxlesTRCK { get; set; }

        [JsonProperty("ufTRLRID")]
        public string UfTRLRID { get; set; }

        [JsonProperty("plateTRLR")]
        public string PlateTRLR { get; set; }

        [JsonProperty("axlesTRLR")]
        public string AxlesTRLR { get; set; }

        [JsonProperty("axlesBTRA")]
        public string AxlesBTRA { get; set; }

        [JsonProperty("ufTRLRBLDID")]
        public string UfTRLRBLDID { get; set; }

        [JsonProperty("ufZBTRAIN")]
        public string UfZBTRAIN { get; set; }


    }
}
