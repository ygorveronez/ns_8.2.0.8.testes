namespace Dominio.ObjetosDeValor.Embarcador.Logistica
{
    public sealed class FiltroPesquisaAlertaMonitor
    {
        public int Codigo { get; set; }

        public string Descricao { get; set; }

        public System.DateTime? Data { get; set; }

        public System.DateTime? DataInicial { get; set; }

        public System.DateTime? DataFinal { get; set; }

        public System.DateTime? DataCadastro { get; set; }

        public System.DateTime? DataFim { get; set; }

        public ObjetosDeValor.Embarcador.Enumeradores.AlertaMonitorStatus Status { get; set; }

        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta? TipoAlerta { get; set; }

        public Dominio.Entidades.Embarcador.Cargas.Carga Carga { get; set; }

        public Dominio.Entidades.Veiculo Veiculo { get; set; }

        public System.DateTime? DataReagendamento{ get; set; }

    }
}
