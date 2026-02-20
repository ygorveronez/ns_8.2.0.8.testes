namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoEmissorDocumento
    {
        Integrador = 0,
        NSTech = 1,
        Migrate = 3
    }

    public static class TipoEmissorDocumentoHelper
    {
        public static string ObterDescricao(this TipoEmissorDocumento TipoEmissorDocumento)
        {
            switch (TipoEmissorDocumento)
            {
                case TipoEmissorDocumento.Integrador: return "Integrador";
                case TipoEmissorDocumento.NSTech: return "NSTech";
                default: return string.Empty;
            }
        }
    }
}