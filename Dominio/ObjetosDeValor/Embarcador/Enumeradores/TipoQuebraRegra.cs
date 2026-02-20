namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoQuebraRegra
    {
        ValePallet = 1,
    }

    public static class TipoQuebraRegraHelper
    {
        public static string ObterDescricao(this TipoQuebraRegra tipoQuebraRegra)
        {
            switch (tipoQuebraRegra)
            {
                case TipoQuebraRegra.ValePallet: return "Vale Pallet";
                default: return "Padrão";
            }
        }
    }
}
