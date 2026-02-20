using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Financeiros
{
    public class ExtratoMotorista
    {
        #region Propriedades

        public int Codigo { get; set; }
        public DateTime Data { get; set; }
        public string Observacao { get; set; }
        public string TipoDocumento { get; set; }
        public string NumeroDocumento { get; set; }
        public string PlanoDebito { get; set; }
        public string PlanoCredito { get; set; }
        public string Motorista { get; set; }
        public int CodigoMotorista { get; set; }
        public decimal Entrada { get; set; }
        public decimal Saida { get; set; }
        public decimal Saldo { get; set; }
        public decimal SaldoAnterior { get; set; }
        public string TipoMovimento { get; set; }
        public string Despesa { get; set; }
        private DateTime DataUltimoAcerto { get; set; }
        public string CodigoIntegracao { get; set; }

        #endregion

        #region Propriedades com Regras

        public string DataFormatada
        {
            get { return Data != DateTime.MinValue ? Data.ToString("dd/MM/yyyy") : string.Empty; }
        }

        public string DataUltimoAcertoFormatada
        {
            get { return DataUltimoAcerto != DateTime.MinValue ? DataUltimoAcerto.ToString("dd/MM/yyyy") : string.Empty; }
        }

        #endregion
    }
}
