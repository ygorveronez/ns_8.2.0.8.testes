using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Unilever
{
    public class Item : Detalhes
    {
        [JsonProperty("fisDocType")]
        public string FisDocType { get; set; }

        [JsonProperty("fisDocKey")]
        public string FisDocKey { get; set; } 
        
        [JsonProperty("plateTRLR")]
        public string PlateTRLR { get; set; }

        [JsonProperty("docdat")]
        public string Docdat { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("weight")]
        public decimal Peso { get; set; }  
        
        [JsonProperty("axlesTRLR")]
        public int AxlesTRLR { get; set; }

        [JsonProperty("value")]
        public decimal Value { get; set; }

        [JsonProperty("nFunt")]
        public string NFunt { get; set; }

        [JsonProperty("costRelevant")]
        public string CostRelevant { get; set; }

        [JsonProperty("podType")]
        public string PodType { get; set; }

        [JsonProperty("podDatTime")]
        public string PodDatTime { get; set; }

        [JsonProperty("refusal")]
        public string Refusal { get; set; }

        [JsonProperty("accident")]
        public string Accident { get; set; }

        [JsonProperty("stageID")]
        public string StageID { get; set; }       
        
        [JsonProperty("axlesTRCK")]
        public int AxlesTRCK { get; set; }  
        
        [JsonProperty("plateTRCK")]
        public string PlacaTrck { get; set; }

    }
}
