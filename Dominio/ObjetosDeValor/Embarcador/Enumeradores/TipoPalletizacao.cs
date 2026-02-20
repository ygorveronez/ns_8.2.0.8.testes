namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoPalletizacao
    {
        Pallet = 1,
        Araras = 2,
        Racks =  3,
    }

    public static class TipoPalletizacaoHelper
    {
        public static string ObterDescricao(this TipoPalletizacao tipo)
        {
            switch (tipo)
            {
                case TipoPalletizacao.Pallet: return "Pallet";
                case TipoPalletizacao.Araras: return "Araras";
                case TipoPalletizacao.Racks: return "Racks";
                default: return string.Empty;
            }
        }
    }
}
