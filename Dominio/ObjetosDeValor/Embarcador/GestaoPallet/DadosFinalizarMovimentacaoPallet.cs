namespace Dominio.ObjetosDeValor.Embarcador.GestaoPallet
{
	public sealed class DadosFinalizarMovimentacaoPallet
	{
		public int QuantidadeDevolvida { get; set; }
		public int QuantidadeDescarte { get; set; }
		public int QuantidadeManutencao { get; set; }
		public Entidades.Embarcador.Filiais.Filial Filial { get; set; }
		public Entidades.Embarcador.Cargas.Carga Carga { get; set; }
		public Entidades.Embarcador.Pedidos.XMLNotaFiscal XMLNotaFiscal { get; set; }
		public string Observacao { get; set; }
	}
}
