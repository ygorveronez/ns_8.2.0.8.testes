using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Logistica
{
    public class MonitoramentoHistoricoTemperatura
    {
        public Int64 Codigo { get; set; }
        public string NumeroCarga { get; set; }
        public string Placa { get; set; }
        public string Reboques { get; set; }
        public string Transportador { get; set; }
        public string Motoristas { get; set; }
        public DateTime DataEvento { get; set; }
        public string FaixaTemperatura { get; set; }
        public decimal FaixaInicial { get; set; }
        public decimal FaixaFinal { get; set; }
        public string CDOrigem { get; set; }
        public string PosicaoDescricao { get; set; }
        public string Destino { get; set; }
        public DateTime DataEntradaLoja { get; set; }
        public DateTime DataSaidaLoja { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public decimal Temperatura { get; set; }
        public DateTime DataCriacaoCarga { get; set; }
        public string DataEventoFormatada
        {
            get { return DataEvento != DateTime.MinValue ? DataEvento.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty; }
        }
        
        public string DataCriacaoCargaFormatada
        {
            get { return DataCriacaoCarga != DateTime.MinValue ? DataCriacaoCarga.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty; }
        }

        public string DataEntradaLojaFormatada
        {
            get { return DataEntradaLoja != DateTime.MinValue ? DataEntradaLoja.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string DataSaidaLojaFormatada
        {
            get { return DataSaidaLoja != DateTime.MinValue ? DataSaidaLoja.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }
    }
}
