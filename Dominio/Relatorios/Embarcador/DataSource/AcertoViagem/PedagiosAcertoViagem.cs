using System;

namespace Dominio.Relatorios.Embarcador.DataSource.AcertoViagem
{
    public class PedagiosAcertoViagem
    {
        public int Codigo { get; set; }
        public int CodigoAcerto { get; set; }
        public DateTime Data { get; set; }
        public DateTime Hora { get; set; }
        public string Rodovia { get; set; }
        public string Praca { get; set; }
        public decimal Valor { get; set; }
        public bool Importado { get; set; }
        public int CodigoVeiculo { get; set; }
        public string Placa { get; set; }
        public int LancadoManualmente { get; set; }
        public int Situacao { get; set; }
        public int Tipo { get; set; }
    }
}
