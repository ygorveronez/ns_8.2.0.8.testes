using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Patrimonio
{
    public class Bem
    {
        public int Codigo { get; set; }

        public string Descricao { get; set; }

        public string NumeroSerie { get; set; }

        public string GrupoProduto { get; set; }

        public string CentroResultado { get; set; }

        public string Almoxarifado { get; set; }

        public decimal ValorBem { get; set; }

        public decimal PercentualDepreciacao { get; set; }

        public decimal DepreciacaoAcumulada { get; set; }

        public DateTime DataAquisicao { get; set; }

        public string FuncionarioAlocado { get; set; }

        public DateTime DataAlocado { get; set; }

        private DateTime DataGarantia { get; set; }

        private DateTime DataEntrega { get; set; }

        public decimal ValorOrcado { get; set; }

        public decimal ValorPago { get; set; }

        public string Defeito { get; set; }

        public string Observacao { get; set; }

        public string DataAquisicaoFormatada
        {
            get { return DataAquisicao != DateTime.MinValue ? DataAquisicao.ToString("dd/MM/yyyy") : ""; }
        }

        public string DataAlocadoFormatada
        {
            get { return DataAlocado != DateTime.MinValue ? DataAlocado.ToString("dd/MM/yyyy") : ""; }
        }

        public string DataGarantiaFormatada
        {
            get { return DataGarantia != DateTime.MinValue ? DataGarantia.ToString("dd/MM/yyyy") : ""; }
        }

        public string DataEntregaFormatada
        {
            get { return DataEntrega != DateTime.MinValue ? DataEntrega.ToString("dd/MM/yyyy") : ""; }
        }
    }
}