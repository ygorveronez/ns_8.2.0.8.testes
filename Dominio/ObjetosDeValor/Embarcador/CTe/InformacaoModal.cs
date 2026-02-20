namespace Dominio.ObjetosDeValor.Embarcador.CTe
{
    public class InformacaoModal
    {
        public int PortoOrigem { get; set; }
        public string CodigoDocumentacaoPortoOrigem { get; set; }
        public int PortoPassagemUm { get; set; }
        public string CodigoDocumentacaoPortoPassagemUm { get; set; }
        public int PortoPassagemDois { get; set; }
        public string CodigoDocumentacaoPortoPassagemDois { get; set; }
        public int PortoPassagemTres { get; set; }
        public string CodigoDocumentacaoPortoPassagemTres { get; set; }
        public int PortoPassagemQuatro { get; set; }
        public string CodigoDocumentacaoPortoPassagemQuatro { get; set; }
        public int PortoPassagemCinco { get; set; }
        public string CodigoDocumentacaoPortoPassagemCinco { get; set; }
        public int PortoDestino { get; set; }
        public string CodigoDocumentacaoPortoDestino { get; set; }
        public int TerminalOrigem { get; set; }
        public string CodigoDocumentacaoTerminalOrigem { get; set; }
        public int TerminalDestino { get; set; }
        public string CodigoDocumentacaoTerminalDestino { get; set; }
        public int Viagem { get; set; }
        public string DescricaoViagem { get; set; }
        public string NumeroControle { get; set; }
        public string NumeroBooking { get; set; }
        public string DescricaoCarrier { get; set; }
        public Dominio.Enumeradores.TipoPropostaFeeder TipoPropostaFeeder { get; set; }
        public bool OcorreuSinistroAvaria { get; set; }
    }
}
