namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum StatusTemplateWhatsApp
    {
        Aprovado = 1,
        AguardandoAprovacao = 2,
        Rejeitado = 3,
    }

    public static class StatusTemplateWhatsAppHelper
    {
        public static string ObterDescricao(this StatusTemplateWhatsApp situacao)
        {
            switch (situacao)
            {
                case StatusTemplateWhatsApp.Aprovado: return "Aprovado";
                case StatusTemplateWhatsApp.AguardandoAprovacao: return "Aguardando aprovação";
                case StatusTemplateWhatsApp.Rejeitado: return "Rejeitado";
                default: return string.Empty;
            }
        }
    }
}
