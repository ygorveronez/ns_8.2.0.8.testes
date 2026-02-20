using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar.AcrescimoDesconto
{

    public class ContratoLancamentoManual
    {
        public int ContratoID { get; set; }
        public DateTime Data { get; set; }
        public string TipoTransacao { get; set; }
        public string Descricao { get; set; }
        public decimal Valor { get; set; }
        public string UsuarioLancamento { get; set; }
        public DateTime DataInclusao { get; set; }
        public int TipoLancamentoContratoManualID { get; set; }
    }


    public class AcrescimosDescontosAmbipar
    {
        public int contractId { get; set; }
        public string travelCode { get; set; }
        public string receiver { get; set; }
        public string reason { get; set; }
        public string note { get; set; }
        public decimal AdjustmentValue { get; set; }
        public decimal grossAdjustmentValue { get; set; }
        public decimal INSS { get; set; }
        public decimal PIS { get; set; }
        public decimal COFINS { get; set; }
        public decimal CSLL { get; set; }
        public decimal IR { get; set; }
        public decimal sestSenat { get; set; }
        public decimal insurance { get; set; }
        public decimal others { get; set; }
    }


    public class RetornoAcrescimosDescontosAmbipar
    {
        public Sender sender { get; set; }
        public Recipient recipient { get; set; }
        public Driver driver { get; set; }
        public Dictionary<string, List<object>> vehicles { get; set; }
        public CargoDetails cargoDetails { get; set; }
        public JourneyTime journeyTime { get; set; }
        public CostDetails costDetails { get; set; }
        public TaxesFees taxesFees { get; set; }
        public Payment payment { get; set; }
        public string released { get; set; }
        public string status { get; set; }
        public int ampcId { get; set; }
        public string branchContractorCNPJ { get; set; }
        public string _id { get; set; }
        public List<object> travelDocuments { get; set; }
        public List<object> logs { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime updatedAt { get; set; }
    }

    public class Sender
    {
        public string name { get; set; }
        public string CNPJ { get; set; }
    }

    public class Recipient
    {
        public string name { get; set; }
        public string CNPJ { get; set; }
    }

    public class Driver
    {
        public string CPF { get; set; }
    }

    public class CargoDetails
    {
        public int cargoTypeAnttCode { get; set; }
        public decimal unitFreightValue { get; set; }
        public decimal goodsKgValue { get; set; }
        public string cargoType { get; set; }
        public string tripType { get; set; }
        public string NCMCode { get; set; }
        public int weightTotalQuantity { get; set; }
    }

    public class JourneyTime
    {
        public string start { get; set; }
        public string finish { get; set; }
    }

    public class CostDetails
    {
        public decimal tollAmount { get; set; }
        public decimal othersValue { get; set; }
        public decimal totalFreightValue { get; set; }
    }

    public class TaxesFees
    {
        public string cargoTypeAnttCode { get; set; }
        public decimal INSSValue { get; set; }
        public decimal IRValue { get; set; }
        public decimal sestSenatValue { get; set; }
        public decimal insuranceValue { get; set; }
        public decimal othersValues { get; set; }
    }

    public class DischargeRules
    {
        public string typeTolerance { get; set; }
        public string shippingTypeWeight { get; set; }
        public string breakTypeCharge { get; set; }
        public string breakingtolerance { get; set; }
        public string faultTypeCharge { get; set; }
    }

    public class MandatoryDischarge
    {
        public string deliveryDate { get; set; }
        public string weight { get; set; }
        public string roadScale { get; set; }
        public string breakdown { get; set; }
        public string invoiceStub { get; set; }
        public string voucherToll { get; set; }
        public string DACTe { get; set; }
    }

    public class DriverApportionment
    {
        public decimal advancePercent { get; set; }
        public decimal settlePercent { get; set; }
    }

    public class HiredCoApportionment
    {
        public decimal advancePercent { get; set; }
        public decimal settlePercent { get; set; }
    }

    public class Apportionment
    {
        public DriverApportionment driver { get; set; }
        public HiredCoApportionment hiredCo { get; set; }
    }

    public class Payment
    {
        public DischargeRules dischargeRules { get; set; }
        public MandatoryDischarge mandatoryDischarge { get; set; }
        public Apportionment apportionment { get; set; }
        public List<object> advancePayment { get; set; }
    }

    
}
