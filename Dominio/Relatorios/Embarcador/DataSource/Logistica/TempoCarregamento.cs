using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Logistica
{
    public class TempoCarregamento
    {
        public int Codigo { get; set; }
        public string CentroCarregamento { get; set; }
        public DateTime DataCarregamento { get; set; }
        public string NumeroCarga { get; set; }
        public string Transportador { get; set; }
        public string Motorista { get; set; }
        public string ModeloVeiculo { get; set; }
        public string Veiculo { get; set; }
        public string DataEntregaGuarita { get; set; }
        public string DataChegadaVeiculo { get; set; }
        public string DataFinalCarregamento { get; set; }
        public string DataLiberacaoVeiculo { get; set; }
        public double TempoTerminoCarregamento { get; set; }
        public double TempoLiberacaoVeiculo { get; set; }
        public double TempoTotalDeCarregamento { get; set; }
        public double TempoParaEntradaVeiculo { get; set; }
        public string DataProgramadaInicial { get; set; }
        public string DiferencaDatasProgEProgInicial { get; set; }
        public string FimFaturamento { get; set; }
        public string TipoOperacao { get; set; }
        public string ObservacaoCarregamento { get; set; }


    }
}
