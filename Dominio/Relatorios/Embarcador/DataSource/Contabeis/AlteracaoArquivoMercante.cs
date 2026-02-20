namespace Dominio.Relatorios.Embarcador.DataSource.Contabeis
{
    public class AlteracaoArquivoMercante
    {
        public int Codigo { get; set; }
        public string Viagem { get; set; }
        public string TerminalOrigem { get; set; }
        public string TerminalDestino { get; set; }
        public string NumeroBooking { get; set; }
        public string NumeroControle { get; set; }
        public string Container { get; set; }
        public int NumeroCTe { get; set; }
        public string NumeroManifesto { get; set; }
        public string NumeroManifestoTransbordo { get; set; }
        public string NumeroCE { get; set; }
        public string PossuiTransbordo { get; set; }
        public string StatusCTe { get; set; }
        public string PortoTransbordo { get; set; }
        public string NavioTransbordo { get; set; }
        public string Balsa { get; set; }
    }
}