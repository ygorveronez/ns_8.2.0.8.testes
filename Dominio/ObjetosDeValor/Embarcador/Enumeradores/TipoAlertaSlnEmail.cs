namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoAlertaSlnEmail
    {
        TempoFaltante = 1,
        TempoExcedido = 2
    }
    
    public static class TipoAlertaSlnEmailHelper
    {
        public static string ObterDescricao(this TipoAlertaSlnEmail tipo)
        {
            switch (tipo)
            {
                case TipoAlertaSlnEmail.TempoFaltante: return Localization.Resources.Enumeradores.TipoAlertaSlnEmail.AlertaTempoFaltante;
                case TipoAlertaSlnEmail.TempoExcedido: return Localization.Resources.Enumeradores.TipoAlertaSlnEmail.AlertaTempoExcedido;
                default: return string.Empty;
            }
        }
    }


}
