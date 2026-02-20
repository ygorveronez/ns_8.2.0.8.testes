namespace Dominio.ObjetosDeValor.Embarcador.GestaoPallet;

public sealed class DadosControlePallet
{
	public Enumeradores.ResponsavelPallet ResponsavelPallet { get; set; }
	public Enumeradores.TipoEstoquePallet TipoEstoquePallet { get; set; } = Enumeradores.TipoEstoquePallet.Movimentacao;
	public double CodigoCliente { get; set; }
	public int CodigoTransportador { get; set; }
	public int CodigoFilial { get; set; }

	public DadosControlePallet(Dominio.Entidades.Cliente cliente, Dominio.Entidades.Empresa transportador, Dominio.Entidades.Embarcador.Filiais.Filial filial = null)
	{
		CodigoCliente = cliente?.CPF_CNPJ ?? 0;
		CodigoTransportador = transportador?.Codigo ?? 0;
		CodigoFilial = filial?.Codigo ?? 0;
	}

	public DadosControlePallet(double cliente, int transportador, int? filial = null)
	{
		CodigoCliente = cliente;
		CodigoTransportador = transportador;
		CodigoFilial = filial ?? 0;
	}

	public DadosControlePallet(int filial)
	{
		CodigoFilial = filial;
	}
}
