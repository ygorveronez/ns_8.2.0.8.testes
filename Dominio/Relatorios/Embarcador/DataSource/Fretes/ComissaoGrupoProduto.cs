using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Fretes
{
    public class ComissaoGrupoProduto
    {
        public int Codigo { get; set; }
        public string ContratoFrete { get; set; }
        public DateTime DataInicialContratoFrete { get; set; }
        public DateTime DataFinalContratoFrete { get; set; }
        public string Transportador { get; set; }
        public string GrupoProdutos { get; set; }
        public string GrupoPessoas { get; set; }
        public string Pessoa { get; set; }
        public decimal PercentualComissao { get; set; }
    }
}
