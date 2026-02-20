namespace Dominio.Relatorios.Embarcador.DataSource.Frota
{
    public sealed class MovimentacaoPneuVeiculoDados
    {
        public int Codigo { get; set; }

        public string BandaRodagem { get; set; }

        public string Dimensao { get; set; }

        public int KmAtualRodado { get; set; }

        public string Marca { get; set; }

        public string Modelo { get; set; }

        public string NumeroFogo { get; set; }

        public string Posicao { get; set; }

        public decimal Sulco { get; set; }

        public decimal ValorAquisicao { get; set; }

        public decimal ValorCustoAtualizado { get; set; }

        public decimal ValorCustoKmAtualizado { get; set; }

        public string VidaAtual { get; set; }
        public string DataMovimentacao { get; set; }
    }
}
