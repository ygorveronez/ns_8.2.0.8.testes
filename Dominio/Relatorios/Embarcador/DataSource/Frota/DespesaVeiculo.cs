using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Frota
{
    public class DespesaVeiculo
    {
        public int CodigoNota { get; set; }
        public Int64 NumeroNota { get; set; }
        public DateTime DataEmissao { get; set; }
        public string Fornecedor { get; set; }
        public string NaturezaOperacao { get; set; }
        public string Produto { get; set; }
        public string GrupoProduto { get; set; }
        public decimal Quantidade { get; set; }
        public decimal ValorUnitario { get; set; }
        public decimal ValorTotal { get; set; }
        public decimal CustoUnitario { get; set; }
        public decimal CustoTotal { get; set; }
        public string Veiculo { get; set; }
        public int KM { get; set; }
    }
}