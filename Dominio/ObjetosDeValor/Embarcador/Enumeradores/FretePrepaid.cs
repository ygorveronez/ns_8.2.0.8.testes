namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum FretePrepaid
    {
        Collect = 1,
        Prepaid = 2,
        PrepaidAbroad = 3,
    }

    public static class FretePrepaidHelper
    {
        public static string ObterDescricao(this FretePrepaid tipo)
        {
            switch (tipo)
            {
                case FretePrepaid.Collect: return "Collect";
                case FretePrepaid.Prepaid: return "Prepaid";
                case FretePrepaid.PrepaidAbroad: return "Prepaid Abroad";
                default: return string.Empty;
            }
        }
    }
}
