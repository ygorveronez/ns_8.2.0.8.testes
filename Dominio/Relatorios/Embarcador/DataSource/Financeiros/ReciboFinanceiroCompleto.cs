using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Financeiros
{
    public class ReciboFinanceiroCompleto
    {
        #region Propriedades

        public string Nome { get; set; }
        public string CPF { get; set; }
        public string CNH { get; set; }
        public DateTime DataAutuacao { get; set; }
        public string AIT { get; set; }
        public string Motivo { get; set; }
        public string ConfigInfracaoTextoPadrao { get; set; }
        #endregion

        #region Propriedades com Regras

        public string DataAutuacaoFormatada
        {
            get { return DataAutuacao != DateTime.MinValue ? DataAutuacao.ToString("dd/MM/yyyy") : string.Empty; }
        }
        #endregion
    }
}
