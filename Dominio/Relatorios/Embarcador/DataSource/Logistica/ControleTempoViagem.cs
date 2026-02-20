using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Logistica
{
    public class ControleTempoViagem
    {
        #region Propriedades

        public int Codigo { get; set; }

        public int NumeroNF { get; set; }

        private DateTime DataFatura { get; set; }

        private DateTime PrevisaoEntrega { get; set; }

        private DateTime DataEntregaReal { get; set; }

        public int Performance { get; set; }

        private DateTime RetornoComprovante { get; set; }

        public int DiasRetornoComprovante { get; set; }

        public string DocumentoVenda { get; set; }

        public string RazaoSocialDestinatario { get; set; }

        public string Carga { get; set; }

        public string Destino { get; set; }

        public string Transportador { get; set; }

        public decimal ValorNota { get; set; }

        public DateTime DataEntregaCanhoto { get; set; }

        public DateTime DataUltimaEtapa { get; set; }

        #endregion Propriedades

        #region Propriedades com Regras

        public string DataFaturaFormatada
        {
            get
            {
                return DataFatura != DateTime.MinValue ? DataFatura.ToString("dd/MM/yyyy") : string.Empty;
            }
        }

        public string PrevisaoEntregaFormatada
        {
            get
            {
                return PrevisaoEntrega != DateTime.MinValue ? PrevisaoEntrega.ToString("dd/MM/yyyy HH:mm") : string.Empty;
            }
        }

        public string DataEntregaRealFormatada
        {
            get
            {
                return DataEntregaReal != DateTime.MinValue ? DataEntregaReal.ToString("dd/MM/yyyy") : string.Empty;
            }
        }

        public string RetornoComprovanteFormatada
        {
            get
            {
                return RetornoComprovante != DateTime.MinValue ? RetornoComprovante.ToString("dd/MM/yyyy HH:mm") : string.Empty;
            }
        }

        public string TempoViagem
        {
            get
            {
                if (DataEntregaCanhoto == DateTime.MinValue)
                    return "";

                double totalDias = (DataEntregaCanhoto - DataUltimaEtapa).TotalDays.RoundUp(precisao: 0);

                if (totalDias <= 0d)
                    return "";

                if (totalDias == 1d)
                    return "1 dia";

                return $"{totalDias.ToString("n0")} dias";
            }
        }

        public string TempoViagemEmHoras
        {
            get
            {
                if (DataEntregaCanhoto == DateTime.MinValue)
                    return "";

                decimal totalMinutos = (decimal)(DataEntregaCanhoto - DataUltimaEtapa).TotalMinutes;

                if (totalMinutos <= 0)
                    return "";

                return totalMinutos.FromMinutesToFormattedTime();
            }
        }

        #endregion Propriedades com Regras
    }
}