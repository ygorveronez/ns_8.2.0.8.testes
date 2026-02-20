using System;

namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum AlertaMonitorStatus
    {
        Todos = -1,
        EmAberto = 0,
        Finalizado = 1,
        EmTratativa = 2
    }

    public static class AlertaMonitorStatusHelper
    {

        public static ObjetosDeValor.Embarcador.Enumeradores.AlertaMonitorStatus? ObterTipoDeAlertaPorDescricao(string Descricao)
        {
            foreach (int i in Enum.GetValues(typeof(TipoAlerta)))
            {
                AlertaMonitorStatus status = (AlertaMonitorStatus)Enum.ToObject(typeof(AlertaMonitorStatus), i);
                if (AlertaMonitorStatusHelper.ObterDescricao(status) == Descricao)
                    return status;
            }
            return null;
        }

        public static string ObterDescricao(this AlertaMonitorStatus status)
        {
            switch (status)
            {
                case AlertaMonitorStatus.Todos: return "Todos";
                case AlertaMonitorStatus.EmAberto: return "Em aberto";
                case AlertaMonitorStatus.Finalizado: return "Finalizado";
                case AlertaMonitorStatus.EmTratativa: return "Em tratativa";
                default: return string.Empty;
            }
        }

        public static string ObterImagemStatus(this AlertaMonitorStatus status)
        {
            switch (status)
            {
                case AlertaMonitorStatus.Todos: return "";
                case AlertaMonitorStatus.EmAberto: return "Content/TorreControle/Icones/alertas/alerta-em-aberto.png";
                case AlertaMonitorStatus.Finalizado: return "Content/TorreControle/Icones/alertas/alerta-resolvido.png";
                case AlertaMonitorStatus.EmTratativa: return "Content/TorreControle/Icones/alertas/alerta-em-tratativa.png";
                default: return string.Empty;
            }
        }


        public static string ObterCorStatus(this AlertaMonitorStatus status)
        {
            switch (status)
            {
                case AlertaMonitorStatus.EmAberto: return "#f85858";
                case AlertaMonitorStatus.Finalizado: return "#64ED64";
                case AlertaMonitorStatus.EmTratativa: return "#EDA864";
                default: return string.Empty;
            }
        }
    }
}



