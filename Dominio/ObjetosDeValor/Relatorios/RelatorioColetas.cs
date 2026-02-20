using System;

namespace Dominio.ObjetosDeValor.Relatorios
{
    public class RelatorioColetas
    {
        public int CodigoPedido { get; set; }
        public DateTime? DataInicial { get; set; }
        public int Numero { get; set; }
        public string CPFCNPJ { get; set; }
        public string Nome { get; set; }
        public decimal ValorNFs { get; set; }
        public decimal ValorFrete { get; set; }
        public decimal PesoTotal { get; set; }
        public string Veiculo { get; set; }
        public string Motorista { get; set; }
    }
}
