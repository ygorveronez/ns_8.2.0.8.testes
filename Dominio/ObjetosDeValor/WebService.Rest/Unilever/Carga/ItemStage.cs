using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.WebService.Rest.Unilever.Carga
{
    [XmlRoot(ElementName = "item", Namespace = "")]
	public class ItemStage
	{
		[XmlElement(ElementName = "Tsnum")]
		public int Tsnum { get; set; }

		[XmlElement(ElementName = "Stcd1")]
		public string Stcd1 { get; set; }

		[XmlElement(ElementName = "OriginTariff")]
		public string OriginTariff { get; set; }

		[XmlElement(ElementName = "DestTariff")]
		public string DestTariff { get; set; }

		[XmlElement(ElementName = "LegInd")]
		public int LegInd { get; set; }

		[XmlElement(ElementName = "LegIndDesc")]
		public string LegIndDesc { get; set; }

		[XmlElement(ElementName = "PricinCalcDat")]
		public string PricinCalcDat { get; set; }

		[XmlElement(ElementName = "Vbeln", Namespace = "")]
		public Vbeln Vbeln { get; set; }

		[XmlElement(ElementName = "Incoterms")]
		public string Incoterms { get; set; }

		[XmlElement(ElementName = "Plant")]
		public int Plant { get; set; }

		[XmlElement(ElementName = "OrigCity")]
		public string OrigCity { get; set; }

		[XmlElement(ElementName = "OrigPostCode")]
		public string OrigPostCode { get; set; }

		[XmlElement(ElementName = "OrigCountry")]
		public string OrigCountry { get; set; }

		[XmlElement(ElementName = "OrigPlantCode")]
		public string OrigPlantCode { get; set; }

		[XmlElement(ElementName = "OrigPlantDesc")]
		public string OrigPlantDesc { get; set; }

		[XmlElement(ElementName = "OrigVendor")]
		public string OrigVendor { get; set; }

		[XmlElement(ElementName = "OrigCustomer")]
		public string OrigCustomer { get; set; }

		[XmlElement(ElementName = "OrigNode")]
		public string OrigNode { get; set; }

		[XmlElement(ElementName = "OrigNodeDesc")]
		public string OrigNodeDesc { get; set; }

		[XmlElement(ElementName = "OrigShipPnt")]
		public string OrigShipPnt { get; set; }

		[XmlElement(ElementName = "OrigShipPntDesc")]
		public string OrigShipPntDesc { get; set; }

		[XmlElement(ElementName = "DestCity")]
		public string DestCity { get; set; }

		[XmlElement(ElementName = "DestPostCode")]
		public string DestPostCode { get; set; }

		[XmlElement(ElementName = "DestCountry")]
		public string DestCountry { get; set; }

		[XmlElement(ElementName = "DestPlantCode")]
		public string DestPlantCode { get; set; }

		[XmlElement(ElementName = "DestPlantDesc")]
		public string DestPlantDesc { get; set; }

		[XmlElement(ElementName = "DestVendor")]
		public string DestVendor { get; set; }

		[XmlElement(ElementName = "DestCustomer")]
		public string DestCustomer { get; set; }

		[XmlElement(ElementName = "DestNode")]
		public string DestNode { get; set; }

		[XmlElement(ElementName = "DestNodeDesc")]
		public string DestNodeDesc { get; set; }

		[XmlElement(ElementName = "DestShipPnt")]
		public string DestShipPnt { get; set; }

		[XmlElement(ElementName = "DestShipPntDesc")]
		public string DestShipPntDesc { get; set; }

		[XmlElement(ElementName = "LoadType")]
		public string LoadType { get; set; }

		[XmlElement(ElementName = "ShipProc")]
		public string ShipProc { get; set; }

		[XmlElement(ElementName = "DelSyst")]
		public string DelSyst { get; set; }

		[XmlElement(ElementName = "LoadFm")]
		public string LoadFm { get; set; }

		[XmlElement(ElementName = "EquipType")]
		public string EquipType { get; set; }

		[XmlElement(ElementName = "EquipNumb")]
		public int EquipNumb { get; set; }

		[XmlElement(ElementName = "Grouping")]
		public string Grouping { get; set; }

		[XmlElement(ElementName = "PalletCount")]
		public string PalletCount { get; set; }

		[XmlElement(ElementName = "GrossValue")]
		public string GrossValue { get; set; }

		[XmlElement(ElementName = "Toll")]
		public string Toll { get; set; }

		[XmlElement(ElementName = "FreeFreight")]
		public string FreeFreight { get; set; }

		[XmlElement(ElementName = "CteReady")]
		public string CteReady { get; set; }

		[XmlElement(ElementName = "MinFreight")]
		public string MinFreight { get; set; }

		[XmlElement(ElementName = "Distance")]
		public double Distance { get; set; }

		[XmlElement(ElementName = "StgWeight")]
		public string StgWeight { get; set; }

		//[XmlElement(ElementName = "Ses", Namespace = "")]
		//public Ses Ses { get; set; }

		[XmlElement(ElementName = "Nfe", Namespace = "")]
		public Nfe Nfe { get; set; }

		//[XmlElement(ElementName = "Vp", Namespace = "")]
		//public Vp Vp { get; set; }

		//[XmlElement(ElementName = "Log")]
		//public string Log { get; set; }

		//[XmlElement(ElementName = "Pricing", Namespace = "")]
		//public Pricing Pricing { get; set; }
	}
}
