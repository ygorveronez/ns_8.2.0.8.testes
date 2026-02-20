namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoNotificacaoMotoristaSMS
    {
        FilaCarregamentoManual = 0,
        GestaoDePatioManual = 1,
    }

    public static class TipoNotificacaoMotoristaSMSHelper
    {
        public static string ObterDescricao(this TipoNotificacaoMotoristaSMS tipo)
        {
            switch (tipo)
            {
                case TipoNotificacaoMotoristaSMS.FilaCarregamentoManual: return "Fila Carregamento - Envio Manual";
                case TipoNotificacaoMotoristaSMS.GestaoDePatioManual: return "Gestão de Pátio - Envio Manual";
                default: return string.Empty;
            }
        }
    }
}
