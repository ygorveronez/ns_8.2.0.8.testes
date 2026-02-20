namespace Dominio.Relatorios.Embarcador.DataSource.Logistica
{
    public sealed class ResumoAgendamentoColeta
    {

        public string Situacao { get; set; }

        public string SenhaAgendamento { get; set; }

        public string DataAgendamento { get; set; }

        public string VolumesAgendados { get; set; }
        
        public string Observacao { get; set; }

        public string NumeroCarga { get; set; }

        public string DescricaoFilial { get; set; }

        public string TipoDeCarga { get; set; }

    }
}
