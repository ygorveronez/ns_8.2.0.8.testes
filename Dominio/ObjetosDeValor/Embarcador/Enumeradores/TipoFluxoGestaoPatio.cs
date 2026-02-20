namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoFluxoGestaoPatio
    {
        Origem = 1,
        Destino = 2
    }

    public static class TipoFluxoGestaoPatioHelper
    {
        public static string ObterDescricao(this TipoFluxoGestaoPatio tipo)
        {
            switch (tipo)
            {
                case TipoFluxoGestaoPatio.Destino: return "Destino";
                case TipoFluxoGestaoPatio.Origem: return "Origem";
                default: return string.Empty;
            }
        }
    }
}
