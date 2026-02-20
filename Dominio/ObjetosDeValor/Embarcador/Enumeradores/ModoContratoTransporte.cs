namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum ModoContratoTransporte
    {
        BRAereo = 0,
        BRFerroviário = 1,
        BRMaritimo = 2,
        BRRodoviario = 3,
        BRWarehouse = 4,
    }

    public static class ModoContratoTransporteHelper
    {
        public static string ObterDescricao(this ModoContratoTransporte modo)
        {
            switch (modo)
            {
                case ModoContratoTransporte.BRAereo: return "BR Aéreo";
                case ModoContratoTransporte.BRFerroviário: return "BR Ferroviário";
                case ModoContratoTransporte.BRMaritimo: return "BR Marítimo";
                case ModoContratoTransporte.BRRodoviario: return "BR Rodoviário";
                case ModoContratoTransporte.BRWarehouse: return "BR Warehouse";
                default: return string.Empty;
            }
        }

        public static string ObterDescricaoLBC(this ModoContratoTransporte modo)
        {
            switch (modo)
            {
                case ModoContratoTransporte.BRAereo: return "BRA Air";
                case ModoContratoTransporte.BRFerroviário: return "BRA Rail";
                case ModoContratoTransporte.BRMaritimo: return "BRA Ocean";
                case ModoContratoTransporte.BRRodoviario: return "BRA truckload";
                case ModoContratoTransporte.BRWarehouse: return "BR Warehouse";
                default: return string.Empty;
            }
        }
    }
}