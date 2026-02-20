namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SubCategoriaContratoTransporte
    {
        CrossDock = 0,
        Freight = 1,
        Firework = 2,
        Shuttle = 3,
        DistributionRoutePlanning = 4,
        ThirdParty = 5,
        ModernTrade = 6,
        GeneralTrade = 7,
        PrimarySecondaryFreighICD = 8,
        ExportByRoad = 9,
        ImportByRoad = 10,
        Crossborder = 11,
        YardManagement = 12,
        Labour = 13,
        InternalOperations = 14,
        TransportRelatedServices = 15,
        Other = 16,
    }

    public static class SubCategoriaContratoTransporteHelper
    {
        public static string ObterDescricao(this SubCategoriaContratoTransporte subCategoria)
        {
            switch (subCategoria)
            {
                case SubCategoriaContratoTransporte.CrossDock: return "Cross Dock";
                case SubCategoriaContratoTransporte.Freight: return "Freight (Air/ Ocean/ Rail/ Road)";
                case SubCategoriaContratoTransporte.Firework: return "Firework";
                case SubCategoriaContratoTransporte.Shuttle: return "Shuttle";
                case SubCategoriaContratoTransporte.DistributionRoutePlanning: return "DRP (Distribution Route Planning)";
                case SubCategoriaContratoTransporte.ThirdParty: return "3rd Party";
                case SubCategoriaContratoTransporte.ModernTrade: return "Modern Trade";
                case SubCategoriaContratoTransporte.GeneralTrade: return "General Trade";
                case SubCategoriaContratoTransporte.PrimarySecondaryFreighICD: return "Primary & Secondary Freight - ICD";
                case SubCategoriaContratoTransporte.ExportByRoad: return "Export (By Road)";
                case SubCategoriaContratoTransporte.ImportByRoad: return "Import (By Road)";
                case SubCategoriaContratoTransporte.Crossborder: return "Crossborder";
                case SubCategoriaContratoTransporte.YardManagement: return "Yard Management";
                case SubCategoriaContratoTransporte.Labour: return "Labour";
                case SubCategoriaContratoTransporte.InternalOperations: return "Internal Operations";
                case SubCategoriaContratoTransporte.TransportRelatedServices: return "Transport related services";
                case SubCategoriaContratoTransporte.Other: return "Other";
                default: return string.Empty;
            }
        }
    }
}