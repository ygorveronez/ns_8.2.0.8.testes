using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Fechamento
{
    public class FechamentoOld
    {
        public int Numero { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
        public string Contrato { get; set; }
        public string TipoFechamento { get; set; }
        public string TipoFranquia { get; set; }

        public int SumarizadoViagensRealizadasTotalViagens { get; set; }
        public decimal SumarizadoViagensRealizadasValorTotalPagoTabela { get; set; }
        public decimal SumarizadoViagensRealizadasAdicionalKM { get; set; }
        public decimal SumarizadoViagensRealizadasValorMedioPagoTabela { get; set; }
        public decimal SumarizadoViagensRealizadasTotal { get; set; }

        public int SumarizadoFranquiaTotalKMFranquia { get; set; }
        public int SumarizadoFranquiaTotalKMRealizado { get; set; }
        public int SumarizadoFranquiaTotalKMExcedido { get; set; }
        public decimal SumarizadoFranquiaValorKMFranquia { get; set; }
        public decimal SumarizadoFranquiaValorKMExcedido { get; set; }
        public decimal SumarizadoFranquiaValorTotalKMFranquia { get; set; }
        public decimal SumarizadoFranquiaValorTotalKMExcedido { get; set; }
        public decimal SumarizadoFranquiaTotal { get; set; }

        public decimal SumarizadoFechamentoTotalJaPagoTabela { get; set; }
        public decimal SumarizadoFechamentoTotalAcordado { get; set; }
        public decimal SumarizadoFechamentoTotalFranquia { get; set; }
        public decimal SumarizadoFechamentoFranquiaExcedente { get; set; }
        public decimal SumarizadoFechamentoDiferenca { get; set; }

        public decimal ValorPagar { get; set; }
        public decimal TotalDescontos { get; set; }
        public decimal TotalAcrescimos { get; set; }
        public decimal ValorFinal { get; set; }
    }
}
