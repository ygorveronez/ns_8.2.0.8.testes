using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.Enumerador
{
    public enum Porte
    {
        Todos = 0,
        Pequeno = 1,
        Medio = 2,
        Grande = 3
    }

	public static class PorteHelper
	{
		public static string ObterDescricao(this Porte porte)
		{
			switch (porte)
			{
				case Porte.Todos: return Localization.Resources.Enumeradores.Porte.Todos;
				case Porte.Pequeno: return Localization.Resources.Enumeradores.Porte.Pequeno;
				case Porte.Medio: return Localization.Resources.Enumeradores.Porte.Medio;
				case Porte.Grande: return Localization.Resources.Enumeradores.Porte.Grande;
				default: return string.Empty;
			}
		}
	}
}
