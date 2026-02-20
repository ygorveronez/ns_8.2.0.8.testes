namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum AcaoFluxoGestaoPatio
    {
        Confirmar = 1,
        Voltar = 2
    }

    public static class AcaoFluxoGestaoPatioHelper
    {
        public static string ObterDescricao(this AcaoFluxoGestaoPatio etapa)
        {
            switch (etapa)
            {
                case AcaoFluxoGestaoPatio.Confirmar: return Localization.Resources.GestaoPatio.FluxoPatio.ConfirmarEtapa;
                case AcaoFluxoGestaoPatio.Voltar: return Localization.Resources.GestaoPatio.FluxoPatio.VoltarEtapa;
                default: return string.Empty;
            }
        }
    }
}
