using System;

namespace Dominio.Relatorios.Embarcador.DataSource.AcertoViagem
{
    public class TempoDeViagem
    {
        public string Veiculo { get; set; }
        public string Motorista { get; set; }
        public string Frota { get; set; }
        public DateTime DataInicial { get; set; }
        public DateTime DataFinal { get; set; }
        public DateTime DataSaida { get; set; }
        public int TempoViagem { get; set; }

        public int DireitoFolga
        {
            get { return TempoViagem > 0 ? TempoViagem / 6 : 0; }
        }

        public string DataSaidaFormatada
        {
            get { return DataSaida != DateTime.MinValue ? DataSaida.ToString("dd/MM/yyyy") : string.Empty; }
        }
        public string DataInicialFormatada
        {
            get { return DataInicial != DateTime.MinValue ? DataSaida.ToString("dd/MM/yyyy") : string.Empty; }
        }
        public string DataFinalFormatada
        {
            get { return DataFinal != DateTime.MinValue ? DataSaida.ToString("dd/MM/yyyy") : string.Empty; }
        }
    }
}
