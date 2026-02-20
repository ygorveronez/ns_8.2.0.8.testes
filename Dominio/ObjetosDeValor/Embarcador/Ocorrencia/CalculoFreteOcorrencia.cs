namespace Dominio.ObjetosDeValor.Embarcador.Ocorrencia
{
    public sealed class CalculoFreteOcorrencia
    {
        public bool DividirOcorrencia { get; set; }

        public decimal HorasOcorrencia { get; set; }

        public decimal HorasOcorrenciaDestino { get; set; }

        public bool IncluirICMSFrete { get; set; }

        public string ObservacaoCTe { get; set; }

        public string ObservacaoCTeDestino { get; set; }

        public string ObservacaoOcorrencia { get; set; }

        public string ObservacaoOcorrenciaDestino { get; set; }

        public decimal PercentualAcrescimoValor { get; set; }

        public bool ValorCalculadoPorTabelaFrete { get; set; }

        public decimal ValorOcorrencia { get; set; }

        public decimal ValorOcorrenciaDestino { get; set; }
    }
}
