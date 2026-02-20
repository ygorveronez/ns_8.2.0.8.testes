using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoAcompanhamentoPedido
    {
        Todos = 0,
        AgColeta = 1,
        ColetaAgendada = 2,
        ColetaRejeitada = 3,
        EmTransporte = 4,
        ProblemaNoTransporte = 5,
        SaiuParaEntrega = 6,
        Entregue = 7,
        EntregaRejeitada = 8,
        EntregaParcial = 9
    }

    public static class SituacaoAcompanhamentoPedidoHelper
    {
        public static string ObterDescricao(this SituacaoAcompanhamentoPedido etapa)
        {
            switch (etapa)
            {
                case SituacaoAcompanhamentoPedido.Todos: return "Todos";
                case SituacaoAcompanhamentoPedido.EmTransporte: return "Em Transporte";
                case SituacaoAcompanhamentoPedido.SaiuParaEntrega: return "Saiu para Entrega";
                case SituacaoAcompanhamentoPedido.EntregaRejeitada: return "Entrega Rejeitada";
                case SituacaoAcompanhamentoPedido.EntregaParcial: return "Entrega Parcial";
                case SituacaoAcompanhamentoPedido.AgColeta: return "Ag. Coleta";
                case SituacaoAcompanhamentoPedido.ColetaAgendada: return "Coleta Agendada";
                case SituacaoAcompanhamentoPedido.ColetaRejeitada: return "Coleta Rejeitada";
                case SituacaoAcompanhamentoPedido.Entregue: return "Entregue";
                case SituacaoAcompanhamentoPedido.ProblemaNoTransporte: return "Problema no Transporte";
                default: return "";
            }
        }

        public static string ObterDescricaoPortalCliente(this SituacaoAcompanhamentoPedido etapa)
        {
            switch (etapa)
            {
                case SituacaoAcompanhamentoPedido.Todos: return "Todos";
                case SituacaoAcompanhamentoPedido.AgColeta: return "Ag. Expedição";
                case SituacaoAcompanhamentoPedido.ColetaAgendada: return "Expedição Agendada";
                case SituacaoAcompanhamentoPedido.ColetaRejeitada: return "Expedição Rejeitada";
                case SituacaoAcompanhamentoPedido.EmTransporte: return "Em Transporte";
                case SituacaoAcompanhamentoPedido.ProblemaNoTransporte: return "Problema na Transferência";
                case SituacaoAcompanhamentoPedido.SaiuParaEntrega: return "Saiu para Entrega";
                case SituacaoAcompanhamentoPedido.Entregue: return "Entregue";
                case SituacaoAcompanhamentoPedido.EntregaRejeitada: return "Entrega Rejeitada";
                case SituacaoAcompanhamentoPedido.EntregaParcial: return "Entrega Parcial";
                default: return "";
            }
        }

        public static List<SituacaoAcompanhamentoPedido> ObterSituacoesAcompanhamentoPedidoPendente()
        {
            return new List<SituacaoAcompanhamentoPedido>()
            {
                SituacaoAcompanhamentoPedido.AgColeta,
                SituacaoAcompanhamentoPedido.ColetaAgendada,
                SituacaoAcompanhamentoPedido.ColetaRejeitada,
                SituacaoAcompanhamentoPedido.EmTransporte,
                SituacaoAcompanhamentoPedido.ProblemaNoTransporte,
                SituacaoAcompanhamentoPedido.SaiuParaEntrega
            };
        }
    }
}
