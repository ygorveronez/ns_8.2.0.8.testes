using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento
{
    public enum enumIssuerTypeMDFe
    {
        service_provider,
        own_cargo,
        globalized_cte
    }

    public enum enumTransporterType
    {
        ETC,
        TAC,
        CTC
    }

    public enum enumModalMDFe
    {
        road,
        air,
        water,
        rail
    }

    /// <summary>
    /// Representa os tipos de componentes do valor do frete.
    /// </summary>
    public enum TipoComposicaoFrete
    {
        /// <summary>
        /// Vale pedágio (01)
        /// </summary>
        ValePedagio = 1,

        /// <summary>
        /// Impostos (02)
        /// </summary>
        Impostos = 2,

        /// <summary>
        /// Seguro (03)
        /// </summary>
        Seguro = 3,

        /// <summary>
        /// Frete valor (04)
        /// </summary>
        FreteValor = 4,

        /// <summary>
        /// Outros (99)
        /// </summary>
        Outros = 99
    }

    public static class TipoComposicaoFreteHelper
    {
        public static string ObterDescricao(this TipoComposicaoFrete status)
        {
            switch (status)
            {
                case TipoComposicaoFrete.ValePedagio: return "TollVoucher";
                case TipoComposicaoFrete.Impostos: return "Taxes";
                case TipoComposicaoFrete.Seguro: return "Insurance";
                case TipoComposicaoFrete.FreteValor: return "Cargo"; // FreightValue 
                case TipoComposicaoFrete.Outros: return "Others";
                default: return string.Empty;
            }
        }
    }
    /// <summary>
    /// Representa o Indicador de forma de pagamento
    /// </summary>
    public enum IndicadorFormaPagamento
    {
        /// <summary>
        /// À Vista (00)
        /// </summary>
        AVista = 0,

        /// <summary>
        /// À Prazo (01)
        /// </summary>
        APrazo = 1
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum enumIssueTypeMDFe
    {
        [EnumMember(Value = "default")]
        default_,
        offline_contingency,
        nff_special_regime
    }

    public enum enumOfflineEmissionMode
    {
        enabled,
        disabled
    }

    public enum enumTransportUnitTypeMDFe
    {
        road_tractor,
        road_trailer,
        ship,
        ferry,
        aircraft,
        railcar,
        other
    }

    public enum enumLoadUnitType
    {
        container,
        uld,
        pallet,
        other
    }

    public enum enumUnitCode
    {
        kg,
        ton
    }

    public enum enumLoadType
    {
        solid_bulk,
        liquid_bulk,
        refrigerated,
        containerized,
        general_cargo,
        neobulk,
        dangerous_solid_bulk,
        dangerous_liquid_bulk,
        dangerous_refrigerated_cargo,
        dangerous_containerized,
        dangerous_general_cargo
    }

    public enum enumPlaceType
    {
        latLong,
        zipcode
    }

    public enum enumRespSeg
    {
        issuer,
        contractor
    }

    public enum enumVehicleCategory
    {
        two_axle,
        three_axle,
        four_axle,
        five_axle,
        six_axle,
        seven_axle,
        eight_axle,
        nine_axle,
        ten_axle,
        over_ten_axle
    }

    public enum enumTollVoucherType
    {
        tag,
        voucher,
        card
    }

    public enum enumPaymentComponentType
    {
        toll_voucher,
        taxes,
        expenses,
        freight,
        other
    }

    public enum enumpaymentMethod
    {
        cash_payment,
        installment_payment
    }

    public enum enumPrepaymentType
    {
        do_not_allow,
        allow,
        allow_with_confirmation
    }

    public enum enumBankDetailType
    {
        bank,
        ipef,
        pix
    }

    public enum enumBodyType
    {
        not_applicable,
        open,
        closed_box,
        bulk_carrier,
        container_chassis,
        sider
    }

    public enum enumWheelType
    {
        truck,
        semi_truck,
        tractor_unit,
        van,
        utility_vehicle,
        other
    }

    public enum enumNavigationType
    {
        interior,
        cabotagem
    }

    public enum enumEmptyCargoUnitType
    {
        container,
        uld,
        pallet,
        other
    }

    public enum enumEmptyTransportUnitType
    {
        truck_traction,
        trailer
    }

    public enum enumOwnerType
    {
        tac_aggregated,
        tac_independent,
        other
    }
}