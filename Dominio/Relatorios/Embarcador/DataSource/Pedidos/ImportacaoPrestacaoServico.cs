using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Pedidos
{
    public class ImportacaoPrestacaoServico
    {
        public string NumeroDI { get; set; }
        public string CodigoImportacao { get; set; }
        public string PontoReferencia { get; set; }
        public decimal ValorCarga { get; set; }
        public decimal Volume { get; set; }
        public decimal Peso { get; set; }
        public int CodigoPedido { get; set; }
        public string NumeroContainer { get; set; }
        public string NumeroBL { get; set; }
        public string NumeroNavio { get; set; }
        public double CNPJPorto { get; set; }
        public string NomePorto { get; set; }
        public string EnderecoEntrega { get; set; }
        public string BairroEntrega { get; set; }        
        public string CEPEntrega { get; set; }
        public string PontoReferenciaEntrega { get; set; }
        public int CodigoCidade { get; set; }
        public string Cidade { get; set; }
        public string Estado { get; set; }
        public DateTime DataVencimento { get; set; }
        public string Armador { get; set; }
        public int CodigoTerminal { get; set; }
        public string Terminal { get; set; }
    }
}
