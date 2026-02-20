namespace Dominio.ObjetosDeValor.WebService.GestaoPatio
{
    public sealed class FluxoPatio
    {
        public string DataConclusaoPesagem { get; set; }
        public decimal CaixasPosPerda { get; set; }
        public string DataConclusaoDocumentos { get; set; }
        public decimal PesagemInicial { get; set; }
        public decimal PesagemFinal { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio EtapaFluxoGestaoPatioAtual { get; set; }
        public string DescricaoEtapaPatioAtual { get; set; }

    }
}
