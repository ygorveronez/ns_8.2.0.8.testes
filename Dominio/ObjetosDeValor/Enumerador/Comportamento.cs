using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.Enumerador
{
	public enum Comportamento
	{
		Todos = 0,
		Docil = 1,
		Obediente = 2,
		Agressivo = 3,
		Inquieto = 4,
		Medroso = 5,
	}

	public static class ComportamentoHelper
	{
		public static string ObterDescricao(this Comportamento Comportamento)
		{
			switch (Comportamento)
			{
				case Comportamento.Todos: return Localization.Resources.Enumeradores.Comportamento.Todos;
				case Comportamento.Docil: return Localization.Resources.Enumeradores.Comportamento.Docil;
				case Comportamento.Obediente: return Localization.Resources.Enumeradores.Comportamento.Obediente;
				case Comportamento.Agressivo: return Localization.Resources.Enumeradores.Comportamento.Agressivo;
				case Comportamento.Inquieto: return Localization.Resources.Enumeradores.Comportamento.Inquieto;
				case Comportamento.Medroso: return Localization.Resources.Enumeradores.Comportamento.Medroso;
				default: return string.Empty;
			}
		}
	}
}
