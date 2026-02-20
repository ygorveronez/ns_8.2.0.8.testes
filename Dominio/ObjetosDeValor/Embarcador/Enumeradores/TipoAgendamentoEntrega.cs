namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoAgendamentoEntrega
    {
        Nenhum = 0,
        SemAgenda = 1,
        AgendamentoEmTransito = 2,
        OfertadoAposConfirmacaoDeAgenda = 3,
        AgendamentoSomenteComNF = 4,
        AgendamentoTransitoNorte = 5
    }

    public static class EnumTipoAgendamentoEntregaHelper
    {
        public static string ObterDescricao(this TipoAgendamentoEntrega situacao)
        {
            switch (situacao)
            {
                case TipoAgendamentoEntrega.SemAgenda: return "Sem Agenda";
                case TipoAgendamentoEntrega.AgendamentoEmTransito: return "Agendamento em trânsito";
                case TipoAgendamentoEntrega.OfertadoAposConfirmacaoDeAgenda: return "Ofertado após confirmação de agenda";
                case TipoAgendamentoEntrega.AgendamentoSomenteComNF: return "Agendamento somente com NF";
                case TipoAgendamentoEntrega.AgendamentoTransitoNorte: return "Agendamento Trânsito Norte";
                default: return string.Empty;
            }
        }

        //Utilidades.String.RemoveDiacritics(DESCRICAO.ToLower()
        public static TipoAgendamentoEntrega ObterEnumPorDescricao(string descricao)
        {
            switch (descricao)
            {
                case "sem agenda": return TipoAgendamentoEntrega.SemAgenda;
                case "agendamento em transito": return TipoAgendamentoEntrega.AgendamentoEmTransito;
                case "ofertado apos confirmacao de agenda": return TipoAgendamentoEntrega.OfertadoAposConfirmacaoDeAgenda;
                case "agendamento somente com nf": return TipoAgendamentoEntrega.AgendamentoSomenteComNF;
                case "agendamento transito norte": return TipoAgendamentoEntrega.AgendamentoTransitoNorte;
                default: return TipoAgendamentoEntrega.Nenhum;
            }
        }
    }
}
