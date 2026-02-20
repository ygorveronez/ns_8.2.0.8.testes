namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum EquipeContratoTransporte
    {
        AfricaProcurementLogistics = 0,
        ANZProcurementLogistics = 1,
        BangladeshProcurementLogistics = 2,
        EastAfricaProcurementLogistics = 3,
        EkaterraTeaLogistics = 4,
        FrenchAfricaProcurementLogistics = 5,
        HongKongLogistics = 6,
        IndiaProcurementLogistics = 7,
        IndonesiaProcurementLogistics = 8,
        IsraelProcurementLogistics = 9,
        JapanLogistics = 10,
        JapanProcurement = 11,
        MCLProcurementLogistics = 12,
        MySgProcurementLogistics = 13,
        NameProcurementLogistics = 14,
        NorthAmericaProcurementLogistics = 15,
        NorthAsiaFinance = 16,
        NorthAsiaIE = 17,
        NorthAsiaLegal = 18,
        NorthAsiaLogistics = 19,
        NorthAsiaProcurement = 20,
        NorthAsiaProcurementLogistics = 21,
        NorthAsiaSupplyChainVPTeam = 22,
        PakistanProcurementLogistics = 23,
        PhilippinesProcurementLogistics = 24,
        PhilippinesSupplyChainLogistics = 25,
        SouthAmericaGovernance = 26,
        SouthAmericaLOP = 27,
        SouthAmericaSupplyChainProcurement = 28,
        SouthernAfricaProcurementLogistics = 29,
        SriLankaProcurementLogistics = 30,
        TaiwanHongKongFINDirectorTeam = 31,
        TaiwanHongKongLegalDirectorTeam = 32,
        TaiwanHongKongProcurement = 33,
        TaiwanHongKongSCDirectorTeam = 34,
        TaiwanLogistics = 35,
        ThailandProcurementLogistics = 36,
        TurkeyProcurementLogistics = 37,
        VietnamProcurementLogistics = 38,
        WestAfricaProcurementLogistics = 39,
    }

    public static class EquipeContratoTransporteHelper
    {
        public static string ObterDescricao(this EquipeContratoTransporte equipe)
        {
            switch (equipe)
            {
                case EquipeContratoTransporte.AfricaProcurementLogistics: return "Africa-Procurement Logistics";
                case EquipeContratoTransporte.ANZProcurementLogistics: return "ANZ-Procurement Logistics";
                case EquipeContratoTransporte.BangladeshProcurementLogistics: return "Bangladesh-Procurement Logistics";
                case EquipeContratoTransporte.EastAfricaProcurementLogistics: return "East Africa-Procurement Logistics";
                case EquipeContratoTransporte.EkaterraTeaLogistics: return "Ekaterra Tea-Logistics";
                case EquipeContratoTransporte.FrenchAfricaProcurementLogistics: return "French Africa-Procurement Logistics";
                case EquipeContratoTransporte.HongKongLogistics: return "Hong Kong-Logistics";
                case EquipeContratoTransporte.IndiaProcurementLogistics: return "India-Procurement Logistics";
                case EquipeContratoTransporte.IndonesiaProcurementLogistics: return "Indonesia-Procurement Logistics";
                case EquipeContratoTransporte.IsraelProcurementLogistics: return "Israel-Procurement Logistics";
                case EquipeContratoTransporte.JapanLogistics: return "Japan-Logistics";
                case EquipeContratoTransporte.JapanProcurement: return "Japan-Procurement";
                case EquipeContratoTransporte.MCLProcurementLogistics:return "MCL-Procurement Logistics";
                case EquipeContratoTransporte.MySgProcurementLogistics: return "MySg-Procurement Logistics";
                case EquipeContratoTransporte.NameProcurementLogistics: return "Name-Procurement Logistics";
                case EquipeContratoTransporte.NorthAmericaProcurementLogistics: return "North America-Procurement Logistics";
                case EquipeContratoTransporte.NorthAsiaFinance: return "North Asia-Finance";
                case EquipeContratoTransporte.NorthAsiaIE: return "North Asia-I/E";
                case EquipeContratoTransporte.NorthAsiaLegal: return "North Asia-Legal";
                case EquipeContratoTransporte.NorthAsiaLogistics: return "North Asia-Logistics";
                case EquipeContratoTransporte.NorthAsiaProcurement: return "North Asia-Procurement";
                case EquipeContratoTransporte.NorthAsiaProcurementLogistics: return "North Asia-Procurement Logistics";
                case EquipeContratoTransporte.NorthAsiaSupplyChainVPTeam: return "North Asia-Supply Chain VP Team";
                case EquipeContratoTransporte.PakistanProcurementLogistics: return "Pakistan-Procurement Logistics";
                case EquipeContratoTransporte.PhilippinesProcurementLogistics: return "Philippines-Procurement Logistics";
                case EquipeContratoTransporte.PhilippinesSupplyChainLogistics: return "Philippines-Supply Chain Logistics";
                case EquipeContratoTransporte.SouthAmericaGovernance: return "South America-Governance";
                case EquipeContratoTransporte.SouthAmericaLOP: return "South America-LOP";
                case EquipeContratoTransporte.SouthAmericaSupplyChainProcurement: return "South America-Supply Chain Procurement";
                case EquipeContratoTransporte.SouthernAfricaProcurementLogistics: return "Southern Africa-Procurement Logistics";
                case EquipeContratoTransporte.SriLankaProcurementLogistics:  return "Sri Lanka-Procurement Logistics";
                case EquipeContratoTransporte.TaiwanHongKongFINDirectorTeam: return "Taiwan & Hong Kong-FIN Director Team";
                case EquipeContratoTransporte.TaiwanHongKongLegalDirectorTeam: return "Taiwan & Hong Kong-Legal Director Team";
                case EquipeContratoTransporte.TaiwanHongKongProcurement: return "Taiwan & Hong Kong-Procurement";
                case EquipeContratoTransporte.TaiwanHongKongSCDirectorTeam: return "Taiwan & Hong Kong-SC Director Team";
                case EquipeContratoTransporte.TaiwanLogistics: return "Taiwan-Logistics";
                case EquipeContratoTransporte.ThailandProcurementLogistics: return "Thailand-Procurement Logistics";
                case EquipeContratoTransporte.TurkeyProcurementLogistics: return "Turkey-Procurement Logistics";
                case EquipeContratoTransporte.VietnamProcurementLogistics:  return "Vietnam-Procurement Logistics";
                case EquipeContratoTransporte.WestAfricaProcurementLogistics: return "West Africa-Procurement Logistics";

                default: return string.Empty;
            }
        }
    }
}