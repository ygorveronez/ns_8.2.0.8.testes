namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoAvaliacaoPortalCliente
    {
        Geral = 1,
        Individual = 2,
    }

    public static class TipoAvaliacaoPortalClienteHelper
    {
        public static string ObterDescricao(this TipoAvaliacaoPortalCliente tipoAvaliacaoPortalCliente)
        {
            switch (tipoAvaliacaoPortalCliente)
            {
                case TipoAvaliacaoPortalCliente.Geral: return "Geral";
                case TipoAvaliacaoPortalCliente.Individual: return "Individual";
                default: return "";
            }
        }
    }
}
