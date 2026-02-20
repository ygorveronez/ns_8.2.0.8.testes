using System;

namespace Dominio.ObjetosDeValor.Embarcador.Logistica
{
    public class TorreMonitoramento
    {
        private string DATE_HOUR_MASK = "dd/MM/yyyy HH:mm";

        public int Codigo { get; set; }
        public int Carga { get; set; }
        public string IDEquipamento { get; set; }
        public int Veiculo { get; set; }
        public DateTime? DataInicioMonitoramento { get; set; }
        public string DataInicioMonitoramentoFormatada { get { return DataInicioMonitoramento?.ToString(DATE_HOUR_MASK) ?? ""; } }
        public DateTime? DataFimMonitoramento { get; set; }
        public string DataFimMonitoramentoFormatada { get { return DataFimMonitoramento?.ToString(DATE_HOUR_MASK) ?? ""; } }
        public DateTime? DataCriacaoCarga { get; set; }
        public string DataCriacaoCargaFormatada { get { return DataCriacaoCarga?.ToString(DATE_HOUR_MASK) ?? ""; } }
        public string CargaEmbarcador { get; set; }
        public DateTime? DataCarregamentoCarga { get; set; }
        public string DataCarregamentoCargaFormatada { get { return DataCarregamentoCarga?.ToString(DATE_HOUR_MASK) ?? ""; } }
        public int AntecedenciaGrade { get; set; }
        public string Destinos { get; set; }
        public string CidadeDestino { get; set; }
        public string TipoCarga { get; set; }
        public string Transportador { get; set; }
        public string Pedidos { get; set; }
        public string Ordens { get; set; }
        public string Filial { get; set; }
        public string Posicao { get; set; }
        public decimal? ValorFrete { get; set; }
        public decimal? OcorrenciasDeslocamento { get; set; }
        public decimal? OcorrenciasFreteNegociado { get; set; }
        public decimal? OutrasOcorrencias { get; set; }
        public DateTime? DataInicioViagem { get; set; }
        public string DataInicioViagemFormatada { get { return DataInicioViagem?.ToString(DATE_HOUR_MASK) ?? ""; } }
        public DateTime? DataPrevisaoChegadaPlanta { get; set; }
        public string DataPrevisaoChegadaPlantaFormatada { get { return DataPrevisaoChegadaPlanta?.ToString(DATE_HOUR_MASK) ?? ""; } }
        public decimal? DistanciaAteDestino { get; set; }
        public decimal? DistanciaAteOrigem { get; set; }
        public DateTime? DataEntradaOrigem { get; set; }
        public DateTime? DataSaidaOrigem { get; set; }
        public string DataCheckinDescargaFormatada { get { return DataEntradaOrigem?.ToString(DATE_HOUR_MASK) ?? ""; } }
        public string DataInicioDescargaFormatada { get { return DataEntradaOrigem?.ToString(DATE_HOUR_MASK) ?? ""; } }
        public string DataFimDescargaFormatada { get { return DataSaidaOrigem?.ToString(DATE_HOUR_MASK) ?? ""; } }
        public int? TempoDescarga { get; set; }
        public DateTime? DataChegadaPlanta { get; set; }
        public string DataChegadaPlantaFormatada { get { return DataChegadaPlanta?.ToString(DATE_HOUR_MASK) ?? ""; } }
        public DateTime? DataPosicaoAtual { get; set; }
        public decimal DistanciaRealizada { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus Status { get; set; }
        public string Tracao { get; set; }
        public string Reboques { get; set; }
        public DateTime? DataPrevisaoTerminoCarga { get; set; }
        public DateTime? DataInicioViagemPrevista { get; set; }
        public DateTime? DataInicioCarregamentoJanela { get; set; }

    }
}
