namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoEnvioIntegracaoNeokohm
    {
        InicioViagem = 1,
        FimViagem = 2
    }

    public static class TipoEnvioIntegracaoNeokohmHelper
    {
        public static string ObterDescricao(this TipoEnvioIntegracaoNeokohm tipoEnvio)
        {
            switch (tipoEnvio)
            {
                case TipoEnvioIntegracaoNeokohm.InicioViagem: return "Início de Viagem";
                case TipoEnvioIntegracaoNeokohm.FimViagem: return "Fim de Viagem";
                default: return string.Empty;
            }
        }
    }
}