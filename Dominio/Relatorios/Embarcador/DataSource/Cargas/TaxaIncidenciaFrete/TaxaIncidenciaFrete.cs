using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Cargas.TaxaIncidenciaFrete
{
    public class TaxaIncidenciaFrete
    {
        public int Codigo { get; set; }
        public string NumeroCarga { get; set; }
        public DateTime DataCarregamento { get; set; }
        public string Transportador { get; set; }
        public string ModeloVeiculo { get; set; }
        public string TipoCarga { get; set; }
        public string NumeroPedido { get; set; }
        public string Veiculos { get; set; }
        public string Destinatario { get; set; }
        public string Destino { get; set; }
        public string Remetente { get; set; }
        public string Origem { get; set; }
        public int NumeroColetas { get; set; }
        public int NumeroEntregas { get; set; }
        public decimal ValorTotalNotaFiscal { get; set; }
        public decimal ValorFrete { get; set; }
        public decimal TaxaIncidencia { get; set; }
    }
}
