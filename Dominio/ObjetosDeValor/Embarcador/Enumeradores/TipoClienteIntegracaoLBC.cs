namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoClienteIntegracaoLBC
    {
        Nenhum = 0,
        Aeroporto = 1,
        Porto = 2,
        TP = 3,
        Fornecedor = 4
    }

    public static class TipoClienteIntegracaoHelper
    {
        public static string ObterDescricao(this TipoClienteIntegracaoLBC tipo)
        {
            switch (tipo)
            {
                case TipoClienteIntegracaoLBC.Nenhum: return "Nenhum";
                case TipoClienteIntegracaoLBC.Aeroporto: return "Aeroporto";
                case TipoClienteIntegracaoLBC.Porto: return "Porto";
                case TipoClienteIntegracaoLBC.TP: return "Transit Point";
                case TipoClienteIntegracaoLBC.Fornecedor: return "Fornecedor";
                default: return string.Empty;
            }
        }
    }
}
