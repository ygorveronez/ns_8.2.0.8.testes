namespace Dominio.Relatorios.Embarcador.DataSource.Logistica
{
    public sealed class ResumoAgendamentoColetaPedidos
    {
        public string Remetente { get; set; }
        public string NumeroPedido { get; set; }
        public decimal QuantidadeProduto { get; set; }
        public int QuantidadeDeCaixas { get; set; }
        public decimal PrecoUnitarioProduto { get; set; }
        public decimal Valor { get; set; }
        public string TipoOperacao { get; set; }
        public string TipoCarga { get; set; }
        public string Setor { get; set; }
        public string CodigoIntegracaoEDescricaoProduto { get; set; }
    }
}
