namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum LayoutImpressaoOrdemColeta
	{
		LayoutPadrao = 1,
		LayoutOrdemCarregamento = 2,
		LayoutColetaContainer = 3,
		LayoutOrdemColetaAuxiliar = 4,
	}

    public static class LayoutImpressaoOrdemColetaHelper
    {
        public static string ObterDescricao(this LayoutImpressaoOrdemColeta layout)
        {
            switch (layout)
            {
                case LayoutImpressaoOrdemColeta.LayoutPadrao: return Localization.Resources.Enumeradores.LayoutImpressaoOrdemColeta.LayoutPadrao;
                case LayoutImpressaoOrdemColeta.LayoutOrdemCarregamento: return Localization.Resources.Enumeradores.LayoutImpressaoOrdemColeta.LayoutOrdemCarregamento;
                case LayoutImpressaoOrdemColeta.LayoutColetaContainer: return Localization.Resources.Enumeradores.LayoutImpressaoOrdemColeta.LayoutColetaContainer;
                case LayoutImpressaoOrdemColeta.LayoutOrdemColetaAuxiliar: return Localization.Resources.Enumeradores.LayoutImpressaoOrdemColeta.LayoutOrdemColetaAuxiliar;
                default: return string.Empty;
            }
        }
    }
}
