using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Conecttec
{

    public enum SituacaoAbastecimentoConecttec
    {
        RESERVE_SUCCESSFUL = 0,
        RESERVE_FAILED = 1,
        RESERVE_TIMEDOUT = 2,
        RESERVE_CANCELLED = 3,
        AUTH_SUCESSFUL = 4,
        AUTH_FAILED = 5,
        AUTH_TIMEDOUT = 6,
        AUTH_CANCELLED = 7, 
        STOP_SUCCESSFUL = 8, 
        STOP_FAILED = 9, 
        STATUS_CHANGED = 10, 
        RUNNING_TOTAL = 11, 
        RESERVE_INTERRUPTED = 12, 
        AUTH_INTERRUPTED = 13,
        STOP_INTERRUPTED = 14,  
        AUTH_WITH_RESTRICTION = 15, 
        PRICE_CHANGED = 16,
    }

    public static class SituacaoAbastecimentoConecttecHelper
    {
        public static string ObterDescricao(this SituacaoAbastecimentoConecttec situacao)
        {
            switch (situacao)
            {
                case SituacaoAbastecimentoConecttec.RESERVE_SUCCESSFUL: return "Reserva realizada com sucesso.";
                case SituacaoAbastecimentoConecttec.RESERVE_FAILED: return "Falha na reserva.";
                case SituacaoAbastecimentoConecttec.RESERVE_TIMEDOUT: return "Tempo de reserva esgotado.";
                case SituacaoAbastecimentoConecttec.RESERVE_CANCELLED: return "Reserva cancelada com sucesso.";
                case SituacaoAbastecimentoConecttec.AUTH_SUCESSFUL: return "Autorização realizada com sucesso.";
                case SituacaoAbastecimentoConecttec.AUTH_FAILED: return "Reserva via não autorizada.";
                case SituacaoAbastecimentoConecttec.AUTH_TIMEDOUT: return "Tempo de autorização esgotado.";
                case SituacaoAbastecimentoConecttec.AUTH_CANCELLED: return "Autorização cancelada.";
                case SituacaoAbastecimentoConecttec.STOP_SUCCESSFUL: return "Cancelamento de reserva com sucesso.";
                case SituacaoAbastecimentoConecttec.STOP_FAILED: return "Cancelamento de reserva sem sucesso.";
                case SituacaoAbastecimentoConecttec.STATUS_CHANGED: return "Mudança de status da bomba.";
                case SituacaoAbastecimentoConecttec.RUNNING_TOTAL: return "Informações do abastecimento em curso.";
                case SituacaoAbastecimentoConecttec.RESERVE_INTERRUPTED: return " Reserva interrompida.";
                case SituacaoAbastecimentoConecttec.AUTH_INTERRUPTED: return "Autorização interrompida.";
                case SituacaoAbastecimentoConecttec.STOP_INTERRUPTED: return " Não utilizado no momento.";
                case SituacaoAbastecimentoConecttec.AUTH_WITH_RESTRICTION: return "Autorização externa em dispenser GNV.";
                case SituacaoAbastecimentoConecttec.PRICE_CHANGED: return "Descreve os preços recentes quando há alteração na automação.";
                default: return string.Empty;
            }
        }
    }
}
