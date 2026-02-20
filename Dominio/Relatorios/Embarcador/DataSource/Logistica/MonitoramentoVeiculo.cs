using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Logistica
{
    public class MonitoramentoParadaVeiculo
    {
        public int Codigo { get; set; }
        public int CodigoVeiculo { get; set; }
        public string Placa { get; set; }
        public string Carga { get; set; }
        public string Expedicao { get { return String.IsNullOrWhiteSpace(Carga) ? "" : $"Carregado ({Carga})"; } }
        public string Transportador { get; set; }
        public string Filial { get; set; }
        public string Situacao { get; set; }
        public string TipoParada { get; set; }
        public string DescricaoParada { get; set; }
        public string DescricaoPosicao { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string LatitudeLongitude { get { return $"{Latitude.ToString("F5").Replace(",", ".")}, {Longitude.ToString("F5").Replace(",", ".")}"; } }
        public DateTime DataInicio { get; set; }
        public string DataInicioFormatada { get { return DataFormatada(DataInicio); } }
        public DateTime DataFim { get; set; }
        public string DataFimFormatada { get { return DataFormatada(DataFim); } }
        public TimeSpan Tempo { get; set; }
        public string LocalidadeAproximada { get; set; }
        private string DataFormatada(DateTime data)
        {
            return data != DateTime.MinValue ? data.ToString("dd/MM/yyyy HH:mm") : string.Empty;
        }
    }
}
