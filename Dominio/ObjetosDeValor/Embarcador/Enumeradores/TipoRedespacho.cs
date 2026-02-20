namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoRedespacho
    {
        Redespacho = 1,
        Reentrega = 2,
    }

    public static class TipoRedespachoHelper
    {
      
        public static string ObterDescricao(this TipoRedespacho tipoRedespacho)
        {
            switch (tipoRedespacho)
            {
                case TipoRedespacho.Redespacho: return "Redespacho";
                case TipoRedespacho.Reentrega: return "Reentrega";
                default: return "";
            }
        }
    }
}
