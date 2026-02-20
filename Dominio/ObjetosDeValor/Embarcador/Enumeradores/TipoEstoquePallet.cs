namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
	public enum TipoEstoquePallet
	{
		Movimentacao = 1,
		Manutencao = 2,
	}

	public static class TipoEstoquePalletHelper
	{
		public static string ObterDescricao(this TipoEstoquePallet tipoEstoquePallet)
		{
			switch (tipoEstoquePallet)
			{
				case TipoEstoquePallet.Movimentacao: return "Movimentação";
				case TipoEstoquePallet.Manutencao: return "Manutenção";
				default: return string.Empty;
			}
		}
	}
}
