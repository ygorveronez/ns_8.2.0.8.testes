using System;

namespace Dominio.ObjetosDeValor.Embarcador.Financeiro
{
    public class FiltroPesquisaRateioDespesaVeiculo
    {
        public decimal? ValorInicial { get; set; }
        public decimal? ValorFinal { get; set; }
        public DateTime? DataInicial { get; set; }
        public DateTime? DataFinal { get; set; }
        public string NumeroDocumento { get; set; }
        public string TipoDocumento { get; set; }
        public int CodigoVeiculo { get; set; }
        public int CodigoSegmentoVeiculo { get; set; }
        public int CodigoTipoDespesa { get; set; }
        public bool? RatearPeloPercentualFaturadoDoVeiculoNoPeriodo { get; set; }
        public int CodigoCentroResultado { get; set; }
        public double CodigoPessoa { get; set; }
        public int CodigoColaborador { get; set; }
    }
}
