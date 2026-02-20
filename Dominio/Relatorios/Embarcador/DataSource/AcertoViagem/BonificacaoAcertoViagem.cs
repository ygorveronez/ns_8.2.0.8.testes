using System;

namespace Dominio.Relatorios.Embarcador.DataSource.AcertoViagem
{
    public class BonificacaoAcertoViagem
    {
        #region Propriedades 

        public int Codigo { get; set; }
        private DateTime DataInicialAcerto { get; set; }
        private DateTime DataFinalAcerto { get; set; }
        public string Motorista { get; set; }
        public int NumeroAcerto { get; set; }
        public decimal Valor { get; set; }
        public string Bonificacao { get; set; }
        public string Justificativa { get; set; }
        private DateTime Data { get; set; }

        #endregion

        #region Propriedades com Regras

        public string PeriodoAcertoFormatada
        {
            get
            {
                string dataFormatada = string.Empty;
                if (DataInicialAcerto != DateTime.MinValue)
                    dataFormatada += DataInicialAcerto.ToString("dd/MM/yyyy");

                if (DataFinalAcerto != DateTime.MinValue)
                    dataFormatada += " até " + DataFinalAcerto.ToString("dd/MM/yyyy");

                return dataFormatada;
            }
        }
        public string DataFormatada
        {
            get { return Data.ToString("dd/MM/yyyy"); }
        }

        #endregion
    }
}