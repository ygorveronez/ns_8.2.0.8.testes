using System;

namespace Dominio.Relatorios.Embarcador.DataSource.AcertoViagem
{
    public class CargaCompartilhada
    {
        public DateTime DataCarga { get; set; }
        public DateTime DataAcerto { get; set; }
        public string Carga { get; set; }
        public string Emitente { get; set; }
        public string Destino { get; set; }
        public int NumeroAcerto { get; set; }
        public decimal PercentualCompartilhado { get; set; }
        public string Motoristas { get; set; }
    }
}
