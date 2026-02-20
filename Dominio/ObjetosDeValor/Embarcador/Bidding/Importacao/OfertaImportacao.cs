namespace Dominio.ObjetosDeValor.Embarcador.Bidding.Importacao
{
    public class OfertaImportacao
    {
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLanceBidding Tipo { get; set; }
        public int BiddingTransportadorRota { get; set; }
        public bool NaoOfertar { get; set; }
        public Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga ModeloVeicular{ get; set; }
        public OfertaImportacaoDados Oferta { get; set; }
    }

    public class OfertaImportacaoDados
    {
        public int Codigo { get; set; }
        public string Quilometragem { get; set; }
        public string ValorFixo { get; set; }
        public string ValorFranquia { get; set; }
        public int ModeloVeicular { get; set; }
        public string ValorFixoMensal { get; set; }
        public string ValorKmRodado { get; set; }
        public string PorcentagemNota { get; set; }
        public string ValorViagem { get; set; }
        public string ValorEntrega { get; set; }
        public string ICMS { get; set; }
        public string ReplicarICMSDesteModeloVeicular { get; set; }
        public string FreteTonelada { get; set; }
        public string PedagioEixo { get; set; }
        public string Ajudante { get; set; }
        public string AdicionalPorEntrega { get; set; }
        public string ValorPorFranquia { get; set; }
    }
}