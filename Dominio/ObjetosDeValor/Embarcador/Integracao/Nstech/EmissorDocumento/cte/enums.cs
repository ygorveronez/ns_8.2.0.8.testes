using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento
{
    public enum enumParticipantType
    {
        legal,
        individual,
        foreign
    }

    public enum enumIssuerType
    {
        legal,
        individual
    }

    public enum enumDocumentType
    {
        nfe,
        nf,
        other
    }

    public enum enumBeforeDocumentType
    {
        electronic,
        paper
    }

    public enum enumDocumentTypeSimplified
    {
        nfe,
        before
    }

    public enum enumDeliveryDocumentTypeSimplified
    {
        partial,
        total
    }

    public enum enumOtherDocumentType
    {
        declaration,
        pipeline,
        cfe_sat,
        nfce,
        outros
    }

    public enum PaperDocumentType
    {
        atre,
        dta,
        international_air_waybill,
        international_waybill,
        individual_bill_lading,
        tif,
        bl
    }

    public enum enumDocumentNfModel
    {
        single,
        rural_product
    }

    public enum enumModal
    {
        road,
        multimodal,
        air,
        rail,
        water,
        pipeline
    }

    public enum enumQuantityUnit
    {
        m3,
        kg,
        ton,
        unidade,
        litros,
        mmbtu
    }

    public enum enumTypeMeasure
    {
        cubagem_nfe, 
        cubagem_aferida_transportador, 
        peso_bruto_nfe, 
        peso_bruto_aferido_transportador, 
        peso_cubado, 
        peso_base_calculo_frete, 
        peso_uso_operacional, 
        caixas, 
        paletes, 
        sacas, 
        containers, 
        rolos, 
        bombonas, 
        latas, 
        litragem, 
        milhao_btu, 
        outros
    }

    public enum enumPrintFormat
    {
        portrait,
        landscape
    }

    public enum enumCteType
    {
        normal,
        complement,
        substitution,
        simplified,
        simplified_substitution
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum enumIssueType
    {
        [EnumMember(Value = "default")]
        default_,
        production,
        svc,
        epec,
        fsda
    }

    public enum enumServiceType
    {
        normal,
        subcontracting,
        transshipment,
        intermediate_transshipment,
        multimodal_associated
    }

    public enum enumTakerTaxIdication
    {
        taxpayer,
        exempt_taxpayer,
        non_taxpayer
    }

    public enum enumDeliveryDateType
    {
        no_defined_date,
        on_defined_date,
        until_defined_date,
        from_defined_date,
        defined_period
    }

    public enum enumDeliveryTimeType
    {
        no_defined_time,
        on_defined_time,
        until_defined_time,
        from_defined_time,
        defined_period
    }

    public enum enumTaxRegime
    {
        simples_nacional,
        lucro_presumido,
        lucro_real,
        micro_empreendedor_individual
    }

    public enum enumTakerType
    {
        sender,
        shipper,
        receiver,
        recipient,
        others
    }

    public enum enumCargoUnitType
    {
        container,
        uld,
        pallet,
        others
    }

    public enum enumTransportUnitType
    {
        rodoviario_tracao,
        rodoviario_reboque,
        navio,
        balsa,
        aeronave,
        vagao,
        outros
    }

    public enum enumDocumentUnit
    {
        cargo,
        transport
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum enumFiscalCode
    {
        [EnumMember(Value = "00")]
        cst00,
        [EnumMember(Value = "20")]
        cst20,
        [EnumMember(Value = "40")]
        cst40,
        [EnumMember(Value = "41")]
        cst41,
        [EnumMember(Value = "51")]
        cst51,
        [EnumMember(Value = "60")]
        cst60,
        [EnumMember(Value = "90")]
        cst90,
        other_state,
        simples_nacional,
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum enumFiscalCode40_41_51
    {
        [EnumMember(Value = "40")]
        cst40,
        [EnumMember(Value = "41")]
        cst41,
        [EnumMember(Value = "51")]
        cst51,
    }

    public enum enumDirection
    {
        north,
        east,
        south,
        west
    }

    public enum enumShipType
    {
        interior, 
        cabotagem
    }

    public enum enumContainerDocumentType
    {
        nf,
        nfe
    }

    public enum enumHandlingInformation
    {
        animal_vivo, 
        artigo_perigoso_com_declaracao_expedidor, 
        somente_em_aeronave_cargueira, 
        artigo_perigoso_sem_declaracao_expedidor, 
        artigo_perigoso_quantidade_isenta, 
        gelo_seco, nao_restrito, 
        artigo_perigoso_carga_consolidada, 
        autorizacao_autoridade_governamental, 
        baterias_ions_litio_PI965, 
        baterias_ions_litio_PI966, 
        baterias_ions_litio_PI967, 
        baterias_metal_litio_PI968, 
        baterias_metal_litio_PI969, 
        baterias_metal_litio_PI970, 
        outro
    }

    public enum enumTariffClass
    {
        minimum_rate, 
        general_rate, 
        specific_rate
    }

    public enum enumDangerousUnit
    {
        kg, 
        kgg, 
        litros, 
        ti, 
        unidades
    }

    public enum enumTrafficType
    {
        proprio, 
        mutuo, 
        rodoferroviario, 
        rodoviario
    }

    public enum enumBillingResponsible
    {
        ferrovia_origem,
        ferrovia_destino
    }

    public enum enumIssueResponsible
    {
        ferrovia_origem,
        ferrovia_destino
    }

    public enum enumRailwaysType
    {
        legal,
        foreign
    }
}
