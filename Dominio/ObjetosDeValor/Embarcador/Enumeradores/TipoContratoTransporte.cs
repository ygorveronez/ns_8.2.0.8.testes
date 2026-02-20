namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoContratoTransporte
    {
        CTC = 0,
        NDA = 1,
        EkaterraTeaLogistics = 2,
        STC = 3,
        UPA = 4,
    }

    public static class TipoContratoTransporteHelper
    {
        public static string ObterDescricao(this TipoContratoTransporte tipoContrato)
        {
            switch (tipoContrato)
            {
                case TipoContratoTransporte.CTC: return "CTC";
                case TipoContratoTransporte.NDA: return "NDA";
                case TipoContratoTransporte.EkaterraTeaLogistics: return "Ekaterra Tea Logistics";
                case TipoContratoTransporte.STC: return "STC";
                case TipoContratoTransporte.UPA: return "UPA";
                default: return string.Empty;
            }
        }
    }
}