using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.EMP
{
    public class Root
    {
        public Vessel vessel { get; set; }
    }

    public class AuxiliaryEngineProfileDetail
    {
        public object auxiliaryEngineMaker { get; set; }
        public object auxiliaryEngineType { get; set; }
        public object serialNumber { get; set; }
        public string maxElectricalPower { get; set; }
        public object rpmMax { get; set; }
        public object maxGeneratorPowerKw { get; set; }
        public object estBasicLoadAtSea { get; set; }
        public object estBasicLoadAtPort { get; set; }
        public object aeEconomizer { get; set; }
        public string kwhCounterInstalled { get; set; }
        public string reeferKwh { get; set; }
        public object stroke { get; set; }
        public object minOperatingLoad { get; set; }
        public object powerFactorCosPhi { get; set; }
        public object frequency { get; set; }
        public object mcr { get; set; }
    }

    public class CapacityDetail
    {
        public string numberOfCranes { get; set; }
        public string craneCapacity { get; set; }
        public object reeferContainerCapacity { get; set; }
        public string totalNumberOfReeferPlugs { get; set; }
        public string totalContainerCapacity { get; set; }
        public bool ondeckRussianStowage { get; set; }
        public string minBay { get; set; }
        public string maxBay { get; set; }
        public string minRow { get; set; }
        public string maxRow { get; set; }
        public string tierOnDeckMin { get; set; }
        public string tierOnDeckMax { get; set; }
        public string tierUnderDeckMin { get; set; }
        public string tierUnderDeckMax { get; set; }
        public object teuAT14TS { get; set; }
        public string totalReeferPlugsInHold { get; set; }
        public string totalReeferPlugsOnDeck { get; set; }
    }

    public class CarrierDetail
    {
        public string code { get; set; }
        public string name { get; set; }
        public int fromDate { get; set; }
        public int toDate { get; set; }
    }

    public class CommunicationMasterDetail
    {
        public object mcs { get; set; }
        public object comments { get; set; }
        public object otherCommunication { get; set; }
        public List<object> communicationDetails { get; set; }
        public List<object> satCDetails { get; set; }
    }

    public class MachineryDetail
    {
        public string numberOfMainEngine { get; set; }
        public string numberOfAuxiliaryEngine { get; set; }
        public string totalPowerFromAuxilaryEngine { get; set; }
        public List<MainEngineProfileDetail> mainEngineProfileDetails { get; set; }
        public List<AuxiliaryEngineProfileDetail> auxiliaryEngineProfileDetails { get; set; }
        public object whrsDetails { get; set; }
        public List<ShaftGeneratorDetail> shaftGeneratorDetails { get; set; }
        public List<object> thrusterDetails { get; set; }
        public List<object> boilerDetails { get; set; }
    }

    public class MainEngineProfileDetail
    {
        public object engineBuilder { get; set; }
        public string engineDesigner { get; set; }
        public object serialNumber { get; set; }
        public object numberOfCylinders { get; set; }
        public object mainEngineTier { get; set; }
        public string mcr { get; set; }
        public string csrPower { get; set; }
        public object csrPercentage { get; set; }
        public object minMcrPower { get; set; }
        public object minMcrPercentage { get; set; }
        public object rpmMax { get; set; }
        public string type { get; set; }
        public object torsionMeterInstalled { get; set; }
        public object torsionMeterLocation { get; set; }
        public string cylinderOilBlender { get; set; }
        public object auxBlowerLimit { get; set; }
        public object nominalMaxRpm { get; set; }
        public object noOfAuxiliaryBlower { get; set; }
        public object auxiliaryBlowersPower { get; set; }
        public string equippedWithAuxBlow { get; set; }
        public string equippedWithTCCO { get; set; }
        public object tccoLimit { get; set; }
        public string meMaxShaftRPM { get; set; }
        public object csrSetPoint { get; set; }
        public object stroke { get; set; }
        public object powerFullRated { get; set; }
        public object powerDerated { get; set; }
    }

    public class OperatorDetail
    {
        public string operatorCode { get; set; }
        public string operatorName { get; set; }
        public string ownershipType { get; set; }
        public string vesselCode { get; set; }
        public int inFleetDate { get; set; }
        public int outFleetDate { get; set; }
        public string operatorStatus { get; set; }
        public bool gttsUsed { get; set; }
        public string oldMoves { get; set; }
        public object operatorReference { get; set; }
        public object operatorLetReletName { get; set; }
        public string operatorId { get; set; }
        public object vesselName { get; set; }
        public List<CarrierDetail> carrierDetails { get; set; }
    }

    public class OwnershipDetail
    {
        public string ownerType { get; set; }
        public string owner { get; set; }
    }


    public class ShaftGeneratorDetail
    {
        public string shaftGeneratorInstalled { get; set; }
        public string shaftMotorInstalled { get; set; }
        public object shaftGeneratorPowerInKw { get; set; }
        public object shaftMotorPowerRating { get; set; }
    }

    public class SpecificationDetail
    {
        public string lengthOverall { get; set; }
        public string beam { get; set; }
        public string height { get; set; }
        public string designDraft { get; set; }
        public string scantlingDraft { get; set; }
        public object summerDraft { get; set; }
        public string deadWeight { get; set; }
        public string grossTonnage { get; set; }
        public string netTonnage { get; set; }
        public string minCruisingSpeed { get; set; }
        public string maxCruisingSpeed { get; set; }
        public object lightShipWeight { get; set; }
        public object minimumFreeBoardHeight { get; set; }
        public object fuelClass { get; set; }
        public object vmod { get; set; }
        public object ballastWaterCapacity { get; set; }
        public object minMetacentricHeightGm { get; set; }
        public object freshWaterAllowance { get; set; }
        public string suezGrossTonnage { get; set; }
        public string suezNetTonnage { get; set; }
        public object panamaGrossTonnage { get; set; }
        public object panamaNetTonnage { get; set; }
        public object lengthBetweenPerpendiculars { get; set; }
        public object freshWaterCapacity { get; set; }
        public string hsfoStorageCapacity { get; set; }
        public string mgomdoStorageCapacity { get; set; }
        public string ulsfoStorageCapacity { get; set; }
        public string hsfoReceivingCapacityPerHour { get; set; }
        public object mgomdoReceivingCapacityPerHour { get; set; }
        public object lsfoReceivingCapacityPerHour { get; set; }
    }

    public class Vessel
    {
        public string imoNumber { get; set; }
        public string vesselTypeCode { get; set; }
        public string vesselTypeName { get; set; }
        public object officialNumber { get; set; }
        public string vesselOwnership { get; set; }
        public string vesselStatus { get; set; }
        public string flagCountryCode { get; set; }
        public string flagState { get; set; }
        public string vesselName { get; set; }
        public object aliasName { get; set; }
        public object previousName { get; set; }
        public int previousVesselFromDate { get; set; }
        public int previousVesselToDate { get; set; }
        public int rebuildDate { get; set; }
        public int keelLayingDate { get; set; }
        public int builtDate { get; set; }
        public object hullNumber { get; set; }
        public string classificationSocietyCode { get; set; }
        public string buildingYardName { get; set; }
        public string callSign { get; set; }
        public string portOfRegistry { get; set; }
        public string buildCountry { get; set; }
        public string countryOfRegistry { get; set; }
        public object underlyingFlag { get; set; }
        public object referencePortOfRegistry { get; set; }
        public string vesselID { get; set; }
        public object iceClass { get; set; }
        public int ismExpiryDate { get; set; }
        public int expectedDeliveryDate { get; set; }
        public int takeOverDate { get; set; }
        public object scrubberOnBoard { get; set; }
        public object vesselClass { get; set; }
        public object vesselSegmentation { get; set; }
        public object scrubberParDetails { get; set; }
        public int dryDockingEventDate { get; set; }
        public int lastHullCleaningDate { get; set; }
        public int lastPropPolishingDate { get; set; }
        public int lastPropInspectionDate { get; set; }
        public string scantlingDisplacementM3 { get; set; }
        public object heightOfAnemometer { get; set; }
        public object propellerType { get; set; }
        public object propellerDiameter { get; set; }
        public object propellerPitch { get; set; }
        public string noOfPropeller { get; set; }
        public object noOfPropellerBlades { get; set; }
        public object minimumDraftAFT { get; set; }
        public object minimumDraftFore { get; set; }
        public string blockCoeDesignDraft { get; set; }
        public string projectedFrontalArea { get; set; }
        public object fleetGroup { get; set; }
        public object classNotation { get; set; }
        public object finStabilisersInstalled { get; set; }
        public int lastDryDockDate { get; set; }
        public int nextDryDockDate { get; set; }
        public int underWaterSurveyDate { get; set; }
        public List<OperatorDetail> operatorDetails { get; set; }
        public List<SpecificationDetail> specificationDetails { get; set; }
        public List<CommunicationMasterDetail> communicationMasterDetails { get; set; }
        public List<CapacityDetail> capacityDetails { get; set; }
        public List<OwnershipDetail> ownershipDetails { get; set; }
        public List<MachineryDetail> machineryDetails { get; set; }
        public object retrofitDetails { get; set; }
    }


}
