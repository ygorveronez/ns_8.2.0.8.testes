using System;

namespace Dominio.Relatorios.Embarcador.DataSource.SAC
{
    public class NotasFiscaisDocumento
    {
        public int Codigo { get; set; }
        public int CodigoCTe { get; set; }
        public string Numero { get; set; }
        public string Serie { get; set; }
        public DateTime DataEmissao { get; set; }
        public decimal Peso { get; set; }
        public int Volume { get; set; }
        public decimal ValorMercadoria { get; set; }
        public string Protocolo { get; set; }
        public string DataLibera { get; set; }
    }
}
