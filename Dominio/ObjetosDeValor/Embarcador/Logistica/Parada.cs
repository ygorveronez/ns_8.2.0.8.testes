using System;

namespace Dominio.ObjetosDeValor.Embarcador.Logistica
{
    public class Parada
    {
        public int CodigoVeiculo { get; set; }
        public Dominio.Entidades.Embarcador.Logistica.Posicao Posicao { get; set; }
        public bool Alerta { get; set; }
        public string Tipo { get; set; }
        public string Descricao { get; set; }
        public string Placa { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
        public TimeSpan Tempo { get; set; }
        public string TempoFormatado
        {
            get
            {
                string formato = String.Empty;
                if (Tempo.Days > 0)
                {
                    formato = $"{Tempo.Days} dias ";
                }
                return formato + Tempo.ToString(@"hh\:mm\:ss");
            }
        }
    }
}
