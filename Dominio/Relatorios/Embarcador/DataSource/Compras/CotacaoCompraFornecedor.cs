using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Compras
{
    public class CotacaoCompraFornecedor
    {
        public int Numero { get; set; }
        public DateTime DataEmissao { get; set; }
        public DateTime DataPrevisao { get; set; }
        public string Descricao { get; set; }
        public string Produto { get; set; }
        public decimal Quantidade { get; set; }
        public decimal ValorUnitario { get; set; }
        public decimal ValorTotal { get; set; }
    }
}
