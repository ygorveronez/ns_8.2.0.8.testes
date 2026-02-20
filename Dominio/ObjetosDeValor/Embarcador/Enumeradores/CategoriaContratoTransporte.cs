namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum CategoriaContratoTransporte
    {
        Inbound = 0,
        PrimaryTransport = 1,
        SecundaryTransport = 2,
    }

    public static class CategoriaContratoTransporteHelper
    {
        public static string ObterDescricao(this CategoriaContratoTransporte categoria)
        {
            switch (categoria)
            {
                case CategoriaContratoTransporte.Inbound: return "Inbound";
                case CategoriaContratoTransporte.PrimaryTransport: return "Primary transport";
                case CategoriaContratoTransporte.SecundaryTransport: return "Secondary transport";
                default: return string.Empty;
            }
        }
    }
}