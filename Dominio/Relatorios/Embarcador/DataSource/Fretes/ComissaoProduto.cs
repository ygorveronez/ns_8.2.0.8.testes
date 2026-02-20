using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Fretes
{
    public class ComissaoProduto
    {
        public int Codigo { get; set; }
        public string ContratoFrete { get; set; }
        public DateTime DataInicialContratoFrete { get; set; }
        public DateTime DataFinalContratoFrete { get; set; }
        public string Transportador { get; set; }
        public string CodigoProduto { get; set; }
        public string Produto { get; set; }
        public string GrupoPessoas { get; set; }
        public string Pessoa { get; set; }
        public decimal PercentualComissao { get; set; }

    }
}
