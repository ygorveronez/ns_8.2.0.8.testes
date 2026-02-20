using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar
{

    // new class integration 
    public class BuscaReturn
    {
        public List<dynamic> Linhas { get; set; }
    }

    public enum ETipoEmissao
    {
        EmissaoSemCIOT = 1,
        EmissaoComCIOTPadrao = 2
    }

    public enum ETipoMidiaValePedagio
    {
        ValePedagioAmbiparCartao = 1,
        PedagioEletronicoTag = 2
    }

    public class ContratoFrete
    {
        public ETipoEmissao TipoEmissaoID { get; set; }
        public ETipoMidiaValePedagio TipoMidiaValePedagio { get; set; }
        public long EmbarcadorFilialID { get; set; }
        public long TransportadorID { get; set; }
        public long MotoristaID { get; set; }
        public long? CartaoID { get; set; }
        public long? CartaoIDTransportador { get; set; }
        public long VeiculoID { get; set; }
        public long? CarretaID { get; set; }
        public long RoteiroID { get; set; }
        public int TipoOperacaoID { get; set; }
        public long TipoMercadoriaID { get; set; }
        public decimal Valor { get; set; }
        public decimal ValorAdiantamento { get; set; }
        public decimal? ValorAdiantamentoTransportadora { get; set; }
        public decimal ValorCarga { get; set; }
        public decimal PesoCarga { get; set; }

        [JsonIgnore]
        public DateTime DataEmissao
        {
            get
            {
                return DateTime.Now.Date;
            }
        }

        public DateTime? DataPrevisaoEntrega { get; set; }
        public bool RoteiroIdaVolta { get; set; }
        public int? EixoSuspensoIda { get; set; }
        public int? EixoSuspensoVolta { get; set; }
        public DateTime? DataPrevistaSaida { get; set; }
        public DateTime? DataPrevistaPrestacaoContas { get; set; }
        public string CodigoViagem { get; set; }
        public DateTime? DataPrestacaoContas { get; set; }
        public DateTime? DataQuitacao { get; set; }
        public DateTime? DataAgendamento { get; set; }
        public DateTime? DataPrevistaPagamento { get; set; }
        public DateTime? DataPagamento { get; set; }
        public string CpfCnpjDestinatario { get; set; } = "";
        public bool? IgnorarPreConfiguracaoRoteiro { get; set; } = false;
        public long PerfilSite { get; set; }
        public bool? FreteContaMotorista { get; set; } = false;
        public string TipoChavePix { get; set; } = null;
        public string TipoChavePixTransportador { get; set; } = null;
        public string ChavePix { get; set; } = null;
        public string ChavePixTransportador { get; set; } = null;
        public bool? EmitirValePedagio { get; set; } = true;
        public bool? ValePedagioCartao { get; set; } = false;
        public string TipoCreditoVPO { get; set; } = "T";
        public bool? FreteFracionado { get; set; } = false;
        public decimal? PercentualTransportador { get; set; }
        public decimal? PercentualMotorista { get; set; }
        public bool AprovarContrato { get; set; }
        public List<ContratoDocumento> ContratoDocumentos { get; set; }
        public List<ContratoEixoSuspensoParadaIda> EixoSuspensoParadasIda { get; set; } = new List<ContratoEixoSuspensoParadaIda>();
        public List<ContratoEixoSuspensoParadaVolta> EixoSuspensoParadasVolta { get; set; } = new List<ContratoEixoSuspensoParadaVolta>();
        public ContratoRegraQuitacao ContratoRegraQuitacao { get; set; }
    }

    public class ContratoDocumento
    {
        public long TipoDocumentoID { get; set; }
        public string TipoDocumento { get; set; }
        public string Numero { get; set; }
        public string Serie { get; set; }
    }

    public class ContratoEixoSuspensoParadaIda
    {
        public int Ordem { get; set; }
        public int EixosSuspenso { get; set; }
    }

    public class ContratoEixoSuspensoParadaVolta
    {
        public int Ordem { get; set; }
        public int EixosSuspenso { get; set; }
    }

    public class ContratoRegraQuitacao
    {
        public bool Tolerancia { get; set; }
        public decimal PercentualTolerancia { get; set; }
        public decimal LimiteSuperior { get; set; }
        public decimal QuebraTolerancia { get; set; }
        public string TipoPeso { get; set; }
        public bool TipoCobrancaQuebra { get; set; }
        public bool TipoCobrancaAvaria { get; set; }
    }

    // oud class
    public class hiredCo
    {
        public string document { get; set; }//": "12345679000",
        public string IE { get; set; }//": "ISENTA",
        public string RNTRC { get; set; }//": "55555",
        public string email { get; set; }//": "teste2 @teste.com",
        public string name { get; set; }//": "Nome Empresa Teste 2",
        public address address { get; set; }
        public List<phones> phones { get; set; }
    }

    public class address
    {
        public string street { get; set; }//": "Rua Silvia Crinstina",
        public string number { get; set; }//": "99",
        public string complement { get; set; }//": "Casa",
        public string neighborhood { get; set; }//": "Jardim das paineiras",
        public string zip { get; set; }//": "05555000",
        public string IBGE { get; set; }//": "966"
    }

    public class phones
    {
        public string number { get; set; }//": "999999999",
        public string areaCode { get; set; }//": "11",
        public string type { get; set; }//": "mobile "
    }

    public class sender
    {
        public string name { get; set; }//": "Sender name",
        public string CNPJ { get; set; }//": "",
        public address address { get; set; }// ": {
    }

    public class recipient
    {
        public string name { get; set; }//": "Recipiente name",
        public string CNPJ { get; set; }//": "",
        public address address { get; set; }//": {
    }

    public class CNH
    {
        public string category { get; set; }//": "E",
        public string number { get; set; }//": "12345",
        public DateTime validity { get; set; }//": "2025-01-01"
    }

    public class payment
    {
        public string pix { get; set; }//": "",
        public string cardNumber { get; set; }//": "",
        public string type { get; set; }//": "card "

    }

    public class driver
    {
        public string name { get; set; }//": "",
        public string email { get; set; }//": "",
        public string birthDate { get; set; }//": "",
        public string document { get; set; }//": "",
        public CNH CNH { get; set; }
        public address address { get; set; }
        public List<phones> phones { get; set; }
        public payment payment { get; set; } //": {
    }

    public class truck
    {
        public string plate { get; set; } //": "",
        public string axes { get; set; } //": "",
        public string branch { get; set; } //": "",
        public string model { get; set; } //": "",
        public string color { get; set; } //": "",
        public string manufactureYear { get; set; } //": "",
        public string modelYear { get; set; } //": "",
        public string renavam { get; set; } //": "",
        public string chassis { get; set; } //": "",
        public string plateIBGE { get; set; } //": "",
        public string axleType { get; set; } //": "duplo"
    }

    public class semiTrailer
    {

        public string plate { get; set; } //": "",
        public string axes { get; set; } //": "",
        public string branch { get; set; } //": "",
        public string model { get; set; } //": "",
        public string color { get; set; } //": "",
        public string manufactureYear { get; set; } //": "",
        public string modelYear { get; set; } //": "",
        public string renavam { get; set; } //": "",
        public string chassis { get; set; } //": "",
        public string plateIBGE { get; set; } //": "",
        public string axleType { get; set; } //": "duplo"
    }

    public class vehicles
    {
        public truck truck { get; set; }
        public semiTrailer semiTrailer { get; set; }
    }

    public class liftedAxles
    {
        public int going { get; set; }//": 0,
        public int @return { get; set; }//": 2
    }

    public class going
    {
        public string zip { get; set; }//": "05555-000"
    }

    public class @return
    {
        public string zip { get; set; }//": "05555 - 000",
        public int liftedAxles { get; set; }//": 2
    }

    public class stops
    {
        public List<going> going { get; set; }
        public List<@return> @retunr { get; set; }
    }

    public class cargoDetails
    {
        public int cargoTypeAnttCode { get; set; }//": 0,
        public decimal unitFreightValue { get; set; }//": 0,
        public decimal goodsKgValue { get; set; }//": 0,
        public string cargoType { get; set; }//": "",
        public string tripType { get; set; }//": "",
        public string NCMCode { get; set; }//": "",
        public decimal weightTotalQuantity { get; set; }//": 0
    }

    public class journeyTime
    {
        public string start { get; set; }//": "",
        public string finish { get; set; }//": ""
    }

    public class costDetails
    {
        public decimal tollAmount { get; set; }//": 0,
        public decimal othersValue { get; set; }//": 0,
        public decimal totalFreightValue { get; set; }//": 0
    }

    public class taxesFees
    {
        public int cargoTypeAnttCode { get; set; }//": "",
        public decimal INSSValue { get; set; }//": 0,
        public decimal IRValue { get; set; }//": 0,
        public decimal sestSenatValue { get; set; }//": 0,
        public decimal insuranceValue { get; set; }//": 0,
        public decimal othersValues { get; set; }//": 0
    }

    public class AcquittanceRules
    {
        public bool tolerance { get; set; }
        public int tolerancePercent { get; set; }
        public string shippingTypeWeight { get; set; }
        public int breakingTolerance { get; set; }
        public bool breakCharge { get; set; }
        public bool faultCharge { get; set; }
    }

    public class MandatoryDischarge
    {
        public bool deliveryDate { get; set; }
        public bool weight { get; set; }
        public bool roadScale { get; set; }
        public bool breakdown { get; set; }
        public bool invoiceStub { get; set; }
        public bool voucherToll { get; set; }
        public bool DACTe { get; set; }
    }

    public class AdvancePayment
    {
        public string date { get; set; }
        public string paymentType { get; set; }
        public decimal value { get; set; }
    }

    public class _driver
    {
        public decimal advancePercent { get; set; }
    }

    public class _hiredCo
    {
        public decimal advancePercent { get; set; }
    }

    public class Apportionment
    {
        public _driver driver { get; set; }
        public _hiredCo hiredCo { get; set; }
    }

    public class Payment
    {
        public AcquittanceRules acquittanceRules { get; set; }
        public MandatoryDischarge mandatoryDischarge { get; set; }
        public AdvancePayment advancePayment { get; set; }
        public Apportionment apportionment { get; set; }
    }

    public class Document
    {
        public string series { get; set; }
        public string number { get; set; }
        public string type { get; set; }
    }

    public class Invoice
    {
        public string series { get; set; }
        public string number { get; set; }
        public string type { get; set; }
    }

    public class TravelDocument
    {
        public Document document { get; set; }
        public List<Invoice> invoices { get; set; }
        public int valueDocumentTravel { get; set; }
    }

    public class Quitacao
    {
        public int TotalArrivalWeightKg { get; set; }//PesoTotalChegadaKg 
        public decimal FaultTotalValue { get; set; }//ValorTotalFalta 
        public bool DeliveredDocuments { get; set; }//DocumentosEntregue 
        public string Remarks { get; set; }//Observacoes
    }

    public class Cancelamento
    {
        public int contratoID { get; set; }//NumeroCIOT
        public string motivoCancelamento { get; set; }//MotivoCancelamento
    }

    public class ContratoFreteOud
    {
        public string travelCode { get; set; }//: "123",
        public string branchContractorCNPJ { get; set; }//": "",
        public bool roundTrip { get; set; }//": true,
        public bool issueTollVoucher { get; set; }//": false,
        public hiredCo hiredCo { get; set; }
        public sender sender { get; set; }//": {
        public recipient recipient { get; set; }
        public driver driver { get; set; }
        public vehicles vehicles { get; set; }
        public liftedAxles liftedAxles { get; set; }
        public stops stops { get; set; }
        public cargoDetails cargoDetails { get; set; }
        public journeyTime journeyTime { get; set; }
        public costDetails costDetails { get; set; }
        public taxesFees taxesFees { get; set; }
        public Payment payment { get; set; }
        public List<TravelDocument> travelDocuments { get; set; }
    }

    public class gerarTokenParam
    {
        public string login { get; set; }
        public string senha { get; set; }
        public string perfil { get; set; }
    }

    public class ContratoFreteAmbipar
    {
        public string travelCode { get; set; }
        public string _id { get; set; }
        public string CIOT { get; set; }
        public string released { get; set; }
        public string status { get; set; }
        public int ampcId { get; set; }
        public string branchContractorCNPJ { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime updatedAt { get; set; }
    }

    public class RetornoGerarTokenUser
    {
        public string _id { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string role { get; set; }
        public bool verified { get; set; }
        public string verification { get; set; }
    }

    public class RetornoGerarTokenParam
    {
        public string access_token { get; set; }
        public int expires_in { get; set; }
        public string token_type { get; set; }
    }

    public class CancelamentoContratoFrete
    {
        public string travelCode { get; set; }
        public int contractID { get; set; }
        public string cancelationReason { get; set; }
    }
}