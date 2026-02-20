namespace Dominio.Relatorios.Embarcador.DataSource.Frota
{
    public class OrdemServicoManutencao
    {
        public int CodigoServico { get; set; }
        public string TempoExecutado { get; set; }
        public string Data { get; set; }
        public string HoraInicio { get; set; }
        public string HoraFim { get; set; }
        public string Mecanico { get; set; }
    }
}
