namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoEventoIntegracaoJanelaCarregamento
	{
		RecebimentoLeilao = 1,
		ResultadoLeilao = 2
	}

	public static class TipoEventoIntegracaoJanelaCarregamentoHelper
	{
		public static string ObterDescricao(this TipoEventoIntegracaoJanelaCarregamento tipoEvento)
		{
			switch (tipoEvento)
			{
				case TipoEventoIntegracaoJanelaCarregamento.RecebimentoLeilao: return "Recebimento Leilão";
				case TipoEventoIntegracaoJanelaCarregamento.ResultadoLeilao: return "Retorno Leilão";
				default: return string.Empty;
			}
		}
	}
}
