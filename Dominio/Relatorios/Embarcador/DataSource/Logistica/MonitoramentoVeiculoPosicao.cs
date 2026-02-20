using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Logistica
{
    public sealed class MonitoramentoVeiculoPosicao
    {
        private string OberDataFormatada(DateTime data)
        {
            if (data != DateTime.MinValue)
                return data.ToString("dd/MM/yyyy HH:mm");

            return "";
        }


        public Int64 Codigo { get; set; }
        public DateTime DataVeiculo { get; set; }

        public string DataVeiculoFormatada
        {
            get { return OberDataFormatada(DataVeiculo); }
        }

        public DateTime DataCadastro { get; set; }

        public string DataCadastroFormatada
        {
            get { return OberDataFormatada(DataCadastro); }
        }
        public string PlacaVeiculo { get; set; }
        public string Descricao { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Ignicao { get; set; }
        public string VelocidadeDescricao { get; set; }
        public string TemperaturaDescricao { get; set; }
        
    }
}
