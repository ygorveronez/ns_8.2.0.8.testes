namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum OrigemSituacaoEntrega
    {
        UsuarioMultiEmbarcador = 1,
        UsuarioPortalTransportador = 2,
        App = 3,
        ArquivoEDI = 4,
        MonitoramentoAutomaticamente = 5,
        WebService = 6,
        LiberacaoCanhoto = 7,
        Correios = 8,
        AlertaMonitoramento = 9,
        Automatico = 10
    }


    public static class OrigemSituacaoEntregaHelper
    {

        public static string ObterDescricao(this OrigemSituacaoEntrega situacaoEnterga)
        {
            switch (situacaoEnterga)
            {
                case OrigemSituacaoEntrega.UsuarioMultiEmbarcador: return "Usuário do ME";
                case OrigemSituacaoEntrega.UsuarioPortalTransportador: return "Usuário do portal do transportador";
                case OrigemSituacaoEntrega.App: return "App";
                case OrigemSituacaoEntrega.ArquivoEDI: return "Arquivo EDI";
                case OrigemSituacaoEntrega.MonitoramentoAutomaticamente: return "Monitoramento Automaticamente";
                case OrigemSituacaoEntrega.WebService: return "WebService";
                case OrigemSituacaoEntrega.LiberacaoCanhoto: return "Liberação de Canhoto";
                case OrigemSituacaoEntrega.Correios: return "Correios";
                case OrigemSituacaoEntrega.AlertaMonitoramento: return "Alerta Monitoramento";
                case OrigemSituacaoEntrega.Automatico: return "Automatico";
                default: return "";
            }
        }

    }
}

