namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum ControleAlertaForma
    {
        PainelAlerta = 1,
        Notificacao = 2,
        Email = 3
    }

    public static class ControleAlertaFormaHelper
    {
        public static string ObterDescricao(this ControleAlertaForma controleAlertaForma)
        {
            switch (controleAlertaForma)
            {
                case ControleAlertaForma.PainelAlerta: return "Painel de Alerta";
                case ControleAlertaForma.Notificacao: return "Notificação";
                case ControleAlertaForma.Email: return "E-mail";
                default: return string.Empty;
            }
        }
    }
}