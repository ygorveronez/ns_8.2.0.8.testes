namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoGatilhoNotificacao
    {
        AdicionarAgendamento = 1,
        ConfirmarAgendamento = 2,
        AlterarHorario = 3,
        DescargaArmazemExterno = 4,
        CancelarAgendamento = 5,
        NoShowNaoComparecimento = 6,
        CargaDevolvida = 7,
        CargaEntregueParcial = 8,
        CargaDevolvidaParcial = 9,
        Desagendar = 10,
        ChegadaConfirmada = 11,
        SaidaConfirmada = 12,
        DescarregamentoFinalizado = 13,
        ReagendarAgendamento = 14,
    }

    public static class TipoGatilhoNotificacaoHelper
    {
        public static string ObterDescricao(this TipoGatilhoNotificacao tipo)
        {
            switch (tipo)
            {
                case TipoGatilhoNotificacao.AdicionarAgendamento: return "Adicionar Agendamento";
                case TipoGatilhoNotificacao.ConfirmarAgendamento: return "Confirmar Agendamento";
                case TipoGatilhoNotificacao.AlterarHorario: return "Alterar Horário";
                case TipoGatilhoNotificacao.ReagendarAgendamento: return "Reagendar o Agendamento";
                case TipoGatilhoNotificacao.DescargaArmazemExterno: return "Descarga Armazém Externo";
                case TipoGatilhoNotificacao.CancelarAgendamento: return "Cancelar Agendamento";
                case TipoGatilhoNotificacao.NoShowNaoComparecimento: return "No Show / Não Comparecimento";
                case TipoGatilhoNotificacao.CargaDevolvida: return "Carga Devolvida";
                case TipoGatilhoNotificacao.CargaEntregueParcial: return "Carga Entregue Parcial";
                case TipoGatilhoNotificacao.CargaDevolvidaParcial: return "Carga Devolvida Parcial";
                case TipoGatilhoNotificacao.Desagendar: return "Desagendar";
                case TipoGatilhoNotificacao.ChegadaConfirmada: return "Chegada Confirmada";
                case TipoGatilhoNotificacao.SaidaConfirmada: return "Saída Confirmada";
                case TipoGatilhoNotificacao.DescarregamentoFinalizado: return "Descarregamento Finalizado";
                default: return string.Empty;
            }
        }
    }

}
