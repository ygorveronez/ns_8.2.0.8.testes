namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoLeituraImagemCanhoto
    {
        Nenhum = 0,
        Automatico = 1,
        CnpjEmitenteInformadoViaMobile = 2,
        GrupoCliente = 3,
        Mobile = 4,
        MobileTransportadorDiferenteUsuario = 5
    }

    public static class TipoLeituraImagemCanhotoHelper
    {
        public static TipoLeituraImagemCanhoto ObterTipo(string tipo)
        {
            switch (tipo)
            {
                case "A": return TipoLeituraImagemCanhoto.Automatico;
                case "C": return TipoLeituraImagemCanhoto.CnpjEmitenteInformadoViaMobile;
                case "E": return TipoLeituraImagemCanhoto.MobileTransportadorDiferenteUsuario;
                case "G": return TipoLeituraImagemCanhoto.GrupoCliente;
                case "M": return TipoLeituraImagemCanhoto.Mobile;
                default: return TipoLeituraImagemCanhoto.Nenhum;
            }
        }
    }
}
