namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum StatusLeilaoIntegracaoJanelaCarregamento
	{
		Contratado = 1,
		Descartado = 2,
		Cancelado = 3,
	}

	public static class StatusLeilaoIntegracaoJanelaCarregamentoHelper
	{
		public static string ObterDescricao(this StatusLeilaoIntegracaoJanelaCarregamento statusLeilao)
		{
			switch(statusLeilao)
			{
				case StatusLeilaoIntegracaoJanelaCarregamento.Contratado: return "Contratado";
				case StatusLeilaoIntegracaoJanelaCarregamento.Descartado: return "Descartado";
				case StatusLeilaoIntegracaoJanelaCarregamento.Cancelado: return "Cancelado";
				default: return string.Empty;
			}
		}

		public static int ObterEnumStatusLeilao(this StatusLeilaoIntegracaoJanelaCarregamento statusLeilao)
		{
			switch(statusLeilao)
			{
				case StatusLeilaoIntegracaoJanelaCarregamento.Contratado: return 1;
				case StatusLeilaoIntegracaoJanelaCarregamento.Descartado: return 2;
				case StatusLeilaoIntegracaoJanelaCarregamento.Cancelado: return 3;
				default: return 0;
			}
		}
	}
}
