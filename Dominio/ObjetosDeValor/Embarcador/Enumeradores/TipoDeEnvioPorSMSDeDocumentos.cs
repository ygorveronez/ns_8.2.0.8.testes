namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoDeEnvioPorSMSDeDocumentos
    {
        Nenhum = 0,
        SMS = 1,
        WhatsApp = 2,
    }

    public static class TipoDeEnvioPorSMSDeDocumentosHelper
    {
        public static string ObterDescricao(this TipoDeEnvioPorSMSDeDocumentos status)
        {
            switch (status)
            {
                case TipoDeEnvioPorSMSDeDocumentos.Nenhum: return "Nenhum";
                case TipoDeEnvioPorSMSDeDocumentos.SMS: return "SMS";
                default: return "";
            }
        }
    }
}
