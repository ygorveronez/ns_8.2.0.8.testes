namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
	public enum TipoManutencaoPallet
	{
		Disponivel = 1,
		Descarte = 2,
		Sucata = 3,
	}

	public static class TipoManutencaoPalletHelper
	{
		public static string ObterDescricao(this TipoManutencaoPallet tipoManutencaoPallet)
		{
			switch (tipoManutencaoPallet)
			{
				case TipoManutencaoPallet.Disponivel: return "Disponível para uso";
				case TipoManutencaoPallet.Descarte: return "Descarte";
				case TipoManutencaoPallet.Sucata: return "Sucata";
				default: return string.Empty;
			}
		}
	}
}
