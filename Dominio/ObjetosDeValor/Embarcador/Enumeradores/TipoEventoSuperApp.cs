namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoEventoSuperApp
    {
        InicioDeViagem = 0,
        InicioDeOperacao = 1,
        EvidenciasDaEntrega = 2,
        Customizado = 3,
        FimDeOperacao = 4,
    }

    public static class TipoEventoSuperAppHelper
    {
        public static string ObterDescricao(this TipoEventoSuperApp tipo)
        {
            switch (tipo)
            {
                case TipoEventoSuperApp.InicioDeViagem: return "Início de Viagem";
                case TipoEventoSuperApp.InicioDeOperacao: return "Início de Carregamento/Descarregamento";
                case TipoEventoSuperApp.EvidenciasDaEntrega: return "Evidências da Entrega";
                case TipoEventoSuperApp.Customizado: return "Evento Customizado";
                case TipoEventoSuperApp.FimDeOperacao: return "Fim de Carregamento/Descarregamento";
                default: return string.Empty;
            }
        }
        public static string ObterTypeSuperApp(this TipoEventoSuperApp tipo)
        {
            switch (tipo)
            {
                case TipoEventoSuperApp.InicioDeViagem: return "START_TRAVEL";
                case TipoEventoSuperApp.InicioDeOperacao: return "START_OPERATION";
                case TipoEventoSuperApp.EvidenciasDaEntrega: return "DELIVERY_RECEIPT";
                case TipoEventoSuperApp.Customizado: return "CUSTOM";
                case TipoEventoSuperApp.FimDeOperacao: return "END_OPERATION";
                default: return string.Empty;
            }
        }

    }
}