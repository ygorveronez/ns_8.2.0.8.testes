using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Utilidades.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.LBC
{
    public sealed class FreteRequisicaoItem
    {
        [JsonIgnore]
        public bool AguardarRetornoIntegracao { get; set; }

        [JsonProperty(PropertyName = "extRateId")]
        public int Codigo { get; set; }

        [JsonProperty(PropertyName = "extContractId")]
        public string CodigoContratoTransportador { get; set; }

        [JsonProperty(PropertyName = "supplier")]
        public string RazaoSocialTransportador { get; set; }

        [JsonProperty(PropertyName = "contract")]
        public string NomeContratoTransportador { get; set; }

        [JsonProperty(PropertyName = "rStruct")]
        public string EstruturaTabela { get; set; }

        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }

        [JsonProperty(PropertyName = "oCntry")]
        public string PaisOrigem { get; set; }

        [JsonProperty(PropertyName = "oLevel")]
        public string TipoOrigem { get; set; }

        [JsonProperty(PropertyName = "oMesorgn")]
        public string Messagem { get; set; }

        [JsonProperty(PropertyName = "oZipset")]
        public string CodigoIntegracaoOrigem { get; set; }

        [JsonProperty(PropertyName = "oLoad")]
        public string Carregamento { get; set; }

        [JsonProperty(PropertyName = "dLoad")]
        public string dLoad { get; set; }

        [JsonProperty(PropertyName = "dCntry")]
        public string PaisDestino { get; set; }

        [JsonProperty(PropertyName = "dLevel")]
        public string TipoDestino { get; set; }

        [JsonProperty(PropertyName = "dZipset")]
        public string CodigoIntegracaoDestino { get; set; }

        [JsonProperty(PropertyName = "loadGrp")]
        public string GrupoCarga { get; set; }

        [JsonProperty(PropertyName = "rateType")]
        public string TipoTaxa { get; set; }

        [JsonProperty(PropertyName = "comType")]
        public string TipoCompromisso { get; set; }

        [JsonProperty(PropertyName = "comTU")]
        public string UnidadeTempoCompromisso { get; set; }

        [JsonProperty(PropertyName = "comUOM")]
        public string UOMCompromisso { get; set; }

        [JsonProperty(PropertyName = "comLvl")]
        public string NivelCompromisso { get; set; }

        [JsonProperty(PropertyName = "comVal")]
        public string Compromisso { get; set; }

        [JsonProperty(PropertyName = "capGroupManaged")]
        public string CapacidadeGrupo { get; set; }

        [JsonProperty(PropertyName = "mode")]
        public string ModoContrato { get; set; }

        [JsonProperty(PropertyName = "eqpClass")]
        public string CategoriaEquipamento { get; set; }

        [JsonProperty(PropertyName = "eqpType")]
        public string TipoEquipamento { get; set; }

        [JsonProperty(PropertyName = "sapNum")]
        public string CodigoIntegracaoTransportador { get; set; }

        [JsonProperty(PropertyName = "srvcLvl")]
        public string NivelServico { get; set; }

        [JsonProperty(PropertyName = "otmDomain")]
        public string DominioOTM { get; set; }

        [JsonProperty(PropertyName = "transPP")]
        public string PontoPlanejamentoTransporte { get; set; }

        [JsonProperty(PropertyName = "delSys")]
        public string CodigoIntegracaoCanalVenda { get; set; }

        [JsonProperty(PropertyName = "opType")]
        public string CodigoIntegracaoCanalEntrega { get; set; }

        [JsonProperty(PropertyName = "cCode")]
        public string CodigoCircuito { get; set; }

        [JsonProperty(PropertyName = "inType")]
        public string TipoIntegracao { get; set; }

        [JsonProperty(PropertyName = "totStops")]
        public string QuantidadeEntregas { get; set; }

        [JsonProperty(PropertyName = "effDate")]
        [JsonConverter(typeof(DateTimeConverter), new object[] { "yyyy-MM-ddTHH:mm:ss.fffZ" })]
        public DateTime? DataInicioVigencia { get; set; }

        [JsonProperty(PropertyName = "expDate")]
        [JsonConverter(typeof(DateTimeConverter), new object[] { "yyyy-MM-ddTHH:mm:ss.fffZ" })]
        public DateTime? DataFimVigencia { get; set; }

        [JsonProperty(PropertyName = "comments")]
        public string Observacao { get; set; }

        [JsonProperty(PropertyName = "adValorem")]
        public decimal? ValorAdValorem { get; set; }

        [JsonProperty(PropertyName = "riskManagement")]
        public decimal? ValorGerenciamentoRisco { get; set; }

        [JsonProperty(PropertyName = "deliveryRate")]
        public decimal? ValorPorEntrega { get; set; }

        [JsonProperty(PropertyName = "frCurrency")]
        public string MoedaValor { get; set; }

        [JsonProperty(PropertyName = "frRate")]
        public decimal? Valor { get; set; }

        [JsonProperty(PropertyName = "puCurrency")]
        public string MoedaValorPorUnidade { get; set; }

        [JsonProperty(PropertyName = "puRateUOM")]
        public string TipoValorPorUnidade { get; set; }

        [JsonProperty(PropertyName = "puTollFractioned")]
        public decimal? ValorPorUnidadeFracionado { get; set; }

        [JsonProperty(PropertyName = "puRatePU")]
        public decimal? ValorPorUnidade { get; set; }

        [JsonProperty(PropertyName = "fcCurrency")]
        public string MoedaValorPorViagem { get; set; }

        [JsonProperty(PropertyName = "fcCostPerTrailer")]
        public decimal? ValorPorViagem { get; set; }

        [JsonProperty(PropertyName = "fcUOMs")]
        public string FrequenciaContrato { get; set; }

        [JsonProperty(PropertyName = "fcMinEquipmentCount")]
        public decimal? TotalVeiculosContrato { get; set; }

        [JsonProperty(PropertyName = "bulkLiquidPlan")]
        public string PlanoLiquidacao { get; set; }

        [JsonProperty(PropertyName = "weekOne")]
        public int? Semana1 { get; set; }

        [JsonProperty(PropertyName = "weekTwo")]
        public int? Semana2 { get; set; }

        [JsonProperty(PropertyName = "weekThree")]
        public int? Semana3 { get; set; }

        [JsonProperty(PropertyName = "weekFour")]
        public int? Semana4 { get; set; }

        [JsonProperty(PropertyName = "maxConnectTime")]
        public int? TempoMaximoConexao { get; set; }

        [JsonProperty(PropertyName = "lastUsedInOTM")]
        public string UtilizadaPorUltimaVez { get; set; }

        [JsonProperty(PropertyName = "estVol")]
        public int? EstVol { get; set; }

        [JsonProperty(PropertyName = "awdPrty")]
        public string AwdPrty { get; set; }

        [JsonProperty(PropertyName = "scacAlias")]
        public string ScacAlias { get; set; }

        [JsonProperty(PropertyName = "sapSC")]
        public string SapSC { get; set; }

        [JsonProperty(PropertyName = "fuelProg")]
        public string FuelProg { get; set; }

        [JsonProperty(PropertyName = "hazmat")]
        public string Hazmat { get; set; }

        [JsonProperty(PropertyName = "lowAlt")]
        public string LowAlt { get; set; }

        [JsonProperty(PropertyName = "ttime")]
        public int? Ttime { get; set; }

        [JsonProperty(PropertyName = "srvcAvl")]
        public List<string> SrvcAvl { get; set; }

        [JsonProperty(PropertyName = "incStops")]
        public int? IncStops { get; set; }

        [JsonProperty(PropertyName = "pickStops")]
        public int? PickStops { get; set; }

        [JsonProperty(PropertyName = "delStops")]
        public int? DelStops { get; set; }

        [JsonProperty(PropertyName = "stopCur")]
        public string StopCur { get; set; }

        [JsonProperty(PropertyName = "costPStop")]
        public int? CostPStop { get; set; }

        [JsonProperty(PropertyName = "returnPerc")]
        public int? ReturnPerc { get; set; }

        [JsonProperty(PropertyName = "unplannedCostType")]
        public string UnplannedCostType { get; set; }

        [JsonProperty(PropertyName = "unplannedCost")]
        public string UnplannedCost { get; set; }

        [JsonProperty(PropertyName = "frFreight")]
        public int? FrFreight { get; set; }

        [JsonProperty(PropertyName = "frFuel")]
        public int? FrFuel { get; set; }

        [JsonProperty(PropertyName = "frTempCost")]
        public int? FrTempCost { get; set; }

        [JsonProperty(PropertyName = "frTolls")]
        public int? FrTolls { get; set; }

        [JsonProperty(PropertyName = "puMinUnits")]
        public int? PuMinUnits { get; set; }

        [JsonProperty(PropertyName = "puMaxUnits")]
        public int? PuMaxUnits { get; set; }

        [JsonProperty(PropertyName = "puTotalCost")]
        public int? PuTotalCost { get; set; }

        [JsonProperty(PropertyName = "puBaseCost")]
        public string PuBaseCost { get; set; }

        [JsonProperty(PropertyName = "puFixedCost")]
        public string PuFixedCost { get; set; }

        [JsonProperty(PropertyName = "puFuelCost")]
        public int? PuFuelCost { get; set; }

        [JsonProperty(PropertyName = "puMinChrg")]
        public int? PuMinChrg { get; set; }

        [JsonProperty(PropertyName = "puMaxChrg")]
        public int? PuMaxChrg { get; set; }

        [JsonProperty(PropertyName = "modBy")]
        public string ModBy { get; set; }

        [JsonProperty(PropertyName = "modDate")]
        public string ModDate { get; set; }
    }
}
