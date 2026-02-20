namespace Dominio.Relatorios.Embarcador.DataSource.Financeiros
{
    public class ResumoTermoQuitacao
    {
        public int NumeroTermo { get; set; }
        public string DataInicial { get; set; }
        public string DataFinal { get; set; }
        public string NomeTransportador { get; set; }
        public decimal PagEDescViaCredEmConta { get; set; }
        public decimal PagEDescViaConfirming { get; set; }
        public decimal CredEmConta { get; set; }
        public decimal TotalDeAdiant { get; set; }
        public decimal NotasCompContraAdiant { get; set; }
        public decimal SaldoDeAdiantEmAberto { get; set; }
        public decimal TotalGeralDosPag { get; set; }

    }
}
