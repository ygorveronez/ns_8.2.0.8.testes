namespace Dominio.ObjetosDeValor
{
    public class DuplicataParcelas
    {
        public int Codigo { get; set; }

        public int Parcela { get; set; }

        public decimal Valor { get; set; }

        public string DataVcto { get; set; }

        public decimal ValorPgto { get; set; }

        public string DataPgto { get; set; }

        public Enumeradores.StatusDuplicata Status { get; set; }

        public string DescricaoStatus { get; set; }

        public string ObservacaoBaixa { get; set; }

        public string FuncionarioBaixa { get; set; }

        public int CodigoPlanoDeConta { get; set; }

        public string ContaPlanoDeConta { get; set; }

        public string DescricaoPlanoDeConta { get; set; }

    }
}
