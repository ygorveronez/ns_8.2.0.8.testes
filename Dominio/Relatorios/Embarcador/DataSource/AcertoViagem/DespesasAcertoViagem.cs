using System;

namespace Dominio.Relatorios.Embarcador.DataSource.AcertoViagem
{
    public class DespesasAcertoViagem
    {
        public int Codigo { get; set; }
        public int CodigoAcerto { get; set; }
        public string NomeFornecedor { get; set; }
        public int NumeroDocumento { get; set; }
        public DateTime Data { get; set; }
        public decimal Valor { get; set; }
        public decimal Quantidade { get; set; }
        public decimal ValorTotal { get; set; }
        public string Observacao { get; set; }
        public double CNPJFornecedor { get; set; }
        public string Fornecedor { get; set; }
        public string Cidade { get; set; }
        public string Estado { get; set; }
        public int CodigoProduto { get; set; }
        public string Produto { get; set; }
        public int CodigoVeiculo { get; set; }
        public string Placa { get; set; }

    }
}
