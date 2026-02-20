namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoFluxoChecklistSuperApp
    {
        Sequencial = 1,
        NaoSequencial = 2,
    }

    public static class TipoFluxoChecklistSuperAppHelper
    {
        public static string ObterDescricao(this TipoFluxoChecklistSuperApp tipoFluxo)
        {
            switch (tipoFluxo)
            {
                case TipoFluxoChecklistSuperApp.Sequencial: return "Sequencial";
                case TipoFluxoChecklistSuperApp.NaoSequencial: return "Não Sequencial";
                default: return string.Empty;
            }
        }
        public static string ObterDescricaoSuperapp(this TipoFluxoChecklistSuperApp tipoFluxo)
        {
            switch (tipoFluxo)
            {
                case TipoFluxoChecklistSuperApp.Sequencial: return "SEQUENTIAL";
                case TipoFluxoChecklistSuperApp.NaoSequencial: return "NON_SEQUENTIAL";
                default: return string.Empty;
            }
        }
    }
}