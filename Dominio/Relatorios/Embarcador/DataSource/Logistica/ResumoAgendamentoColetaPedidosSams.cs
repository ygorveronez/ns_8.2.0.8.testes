namespace Dominio.Relatorios.Embarcador.DataSource.Logistica
{
    public sealed class ResumoAgendamentoColetaPedidosSams
    {
        public string Remetente { get; set; }
        public string NumeroPedido { get; set; }

        public decimal QuantidadeCaixas { get; set; }

        public decimal Valor { get; set; }

        public string TipoOperacao { get; set; }

        public string TipoCarga { get; set; }

        public string Setor { get; set; }
        public string DescricaoFilial { get; set; }

        public string CodigoIntegracaoEDescricaoProduto { get; set; }
        public int QuantidadeItens { get; set; }
    }
}
