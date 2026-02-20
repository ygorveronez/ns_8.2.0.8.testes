namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
	public enum StatusCotacaoEspecial
	{
		Todos = 0,
		AguardandoAprovacao = 1,
		Aprovado = 2,
		Reprovado = 3,
		AguardandoAnalise = 4,
	}

	public static class StatusCotacaoEspecialoHelper
	{
		public static string Descricao(this StatusCotacaoEspecial statusCotacaoEspecial)
		{
			switch (statusCotacaoEspecial)
			{
				case StatusCotacaoEspecial.Todos:
					return "Todos";
				case StatusCotacaoEspecial.AguardandoAprovacao:
					return "Aguardando Aprovação";
				case StatusCotacaoEspecial.Aprovado:
					return "Aprovado";
				case StatusCotacaoEspecial.Reprovado:
					return "Reprovado";
				case StatusCotacaoEspecial.AguardandoAnalise:
					return "Aguardando Análise";
				default:
					return string.Empty;
			}
		}
	}
}
