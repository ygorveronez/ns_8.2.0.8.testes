using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Cargas.Carga
{
    public class RelatorioPacotes
    {
        #region Propriedades

        public int Codigo { get; set; }
        public string Contratante { get; set; }
        public int Pedido { get; set; }
        public string Carga { get; set; }
        private DateTime DataRecebimento { get; set; }
        public string LogKey { get; set; }
        public string TipoOperacao { get; set; }
        public string Origem { get; set; }
        public string Destino { get; set; }
        public decimal Cubagem { get; set; }
        public decimal Peso { get; set; }
        public int CteAnterior { get; set; }
        public string ChaveCTe { get; set; }
        public double CNPJOrigem { get; set; }
        public double CNPJDestino { get; set; }
        public double CNPJContratante { get; set; }
        public decimal ValorCTeAnterior { get; set; }
        public string NumeroNFeVinculada { get; set; }

        #endregion

        #region Propriedades com Regras

        public string DataRecebimentoFormatada
        {
            get { return DataRecebimento != DateTime.MinValue ? DataRecebimento.ToString("dd/MM/yyyy HH:mm") : ""; }
        }

        #endregion
    }
}
