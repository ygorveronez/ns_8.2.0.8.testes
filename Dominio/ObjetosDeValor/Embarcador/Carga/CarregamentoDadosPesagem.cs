namespace Dominio.ObjetosDeValor.Embarcador.Carga
{
    public sealed class CarregamentoDadosPesagem
    {
        public decimal CapacidadeCubagem { get; set; }

        public decimal CapacidadePallet { get; set; }

        public decimal CapacidadePeso { get; set; }

        public decimal Cubagem { get; set; }

        public decimal Pallet { get; set; }

        public decimal Peso { get; set; }

        public decimal PercentualOcupacaoCubagem { get; set; }

        public decimal PercentualOcupacaoPallet { get; set; }

        public decimal PercentualOcupacaoPeso { get; set; }

        public bool PossuiCubagem { get; set; }

        public bool PossuiPallet { get; set; }

        public Enumeradores.SituacaoPesagemCarregamento SituacaoCubagem { get; set; }

        public Enumeradores.SituacaoPesagemCarregamento SituacaoPallet { get; set; }

        public Enumeradores.SituacaoPesagemCarregamento SituacaoPeso { get; set; }

        public decimal ToleranciaMinimaCubagem { get; set; }

        public decimal ToleranciaMinimaPallet { get; set; }

        public decimal ToleranciaMinimaPeso { get; set; }
    }
}
