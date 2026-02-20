namespace Dominio.ObjetosDeValor.Embarcador.Pessoas
{
    public class ClienteDescarga
    {
        public bool AgendamentoExigeNotaFiscal { get; set; }
        public bool AgendamentoDescargaObrigatorio { get; set; }
        public bool ExigeAgendamento { get; set; }
        public string TempoAgendamento { get; set; }
        public string LinkParaAgendamento { get; set; }
        public string FormaAgendamento { get; set; }
        public bool ExigeSenhaNoAgendamento { get; set; }

    }
}
