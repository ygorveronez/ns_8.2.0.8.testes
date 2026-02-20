using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.Enumerador
{
    public enum Pelagem
    {
        Todas = 0,
        Curta = 1,
        Media = 2,
        Longa = 3
    }

	public static class PelagemHelper
	{
		public static string ObterDescricao(this Pelagem Pelagem)
		{
			switch (Pelagem)
			{
				case Pelagem.Todas: return Localization.Resources.Enumeradores.Pelagem.Todas;
				case Pelagem.Curta: return Localization.Resources.Enumeradores.Pelagem.Curta;
				case Pelagem.Media: return Localization.Resources.Enumeradores.Pelagem.Media;
				case Pelagem.Longa: return Localization.Resources.Enumeradores.Pelagem.Longa;
				default: return string.Empty;
			}
		}
	}
}
