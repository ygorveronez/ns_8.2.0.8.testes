namespace Dominio.ObjetosDeValor.CIOT
{
    public class CIOT
    {
        public int[] CodigosCTes { get; set; }
        public Veiculo Veiculo { get; set; }
        public Motorista Motorista { get; set; }
        public Contratado Contratado { get; set; }
        public string DataInicio { get; set; }
        public string DataFim { get; set; }
        public decimal Distancia { get; set; }
        public string NaturezaCarga { get; set; }
        public Enumerador.TipoPagamentoCIOT? TipoPagamento { get; set; }
        public Enumeradores.RegraQuitacaoAdiantamento? RegraQuitacaoAdiantamento { get; set; }
        public Enumeradores.RegraQuitacaoQuitacao? RegraQuitacaoQuitacao { get; set; }
        public Enumeradores.TipoFavorecido? TipoFavorecido { get; set; }
        public Enumeradores.CategoriaTransportadorANTT? CategoriaTransportadorANTT { get; set; }
        public Enumeradores.DocumentosObrigatorios? DocumentosObrigatorios { get; set; }
        public Enumeradores.TipoViagemANTT? TipoViagemANTT { get; set; }
        public Enumeradores.TipoPeso? TipoPeso { get; set; }
        public Enumeradores.RecalculoFrete? RecalculoFrete { get; set; }
        public Enumeradores.RecalculoFrete? ExigePesoChegada { get; set; }
        public Enumeradores.RecalculoFrete? TipoQuebra { get; set; }
        public Enumeradores.RecalculoFrete? TipoTolerancia { get; set; }

    }
}
