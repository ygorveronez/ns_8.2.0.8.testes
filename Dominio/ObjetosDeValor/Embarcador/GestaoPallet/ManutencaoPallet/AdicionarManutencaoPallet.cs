namespace Dominio.ObjetosDeValor.Embarcador.GestaoPallet;

public sealed class AdicionarManutencaoPallet
{
	public int QuantidadePallet { get; set; }
	public Entidades.Embarcador.Filiais.Filial Filial { get; set; }
	public Entidades.Embarcador.Cargas.Carga Carga { get; set; }
	public Entidades.Embarcador.Pedidos.XMLNotaFiscal XMLNotaFiscal { get; set; }
	public Enumeradores.TipoEntradaSaida TipoMovimentacao { get; set; }
	public Enumeradores.TipoManutencaoPallet TipoManutencaoPallet { get; set; }
	public string Observacao { get; set; }
}