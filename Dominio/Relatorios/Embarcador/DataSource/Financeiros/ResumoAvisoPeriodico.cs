namespace Dominio.Relatorios.Embarcador.DataSource.Financeiros
{
    public class ResumoAvisoPeriodico
    {
        public int NumeroAviso { get; set; }
        public string DataInicial { get; set; }
        public string DataFinal { get; set; }
        public string NomeTransportador { get; set; }
        public decimal PagamentoEDescontoViaCreditoEmConta { get; set; }
        public decimal PagamentoEDescontoViaConfirming { get; set; }
        public decimal CreditoEmConta { get; set; }
        public decimal TotalDeAdiantamento { get; set; }
        public decimal NotasCompensadasContraAdiantamento { get; set; }
        public decimal SaldoDeAdiantamentoEmAberto { get; set; }
        public decimal TotalGeralDosPag { get; set; }

        public decimal TotalTransportadorVencido { get; set; }
        public decimal TotalTransportadorAVencer { get; set; }
        public decimal TotalTransportadorPendencias { get; set; }

        public decimal TotalDesbloqueadoVencido { get; set; }
        public decimal TotalDesbloqueadoAVencer { get; set; }
        public decimal TotalDesbloqueadoPendencias { get; set; }

        public decimal TotalUnileverVencido { get; set; }
        public decimal TotalUnileverAVencer { get; set; }
        public decimal TotalUnileverPendencias { get; set; }

        public decimal TotalBloqueioPODVencido { get; set; }
        public decimal TotalBloqueioPODAVencer { get; set; }
        public decimal TotalBloqueioPODPendencias { get; set; }

        public decimal TotalBloqueioIrregularidadeVencido { get; set; }
        public decimal TotalBloqueioIrregularidadeAVencer { get; set; }
        public decimal TotalBloqueioIrregularidadePendencias { get; set; }

        public decimal TotalPendentes { get; set; }

        public decimal AvariasEmAberto { get; set; }
        public decimal DebitosBaixaResultado { get; set; }
        public decimal ProjecaoRecebimento { get; set; }


    }
}
