namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoPaleteCliente
    {
        NaoDefinido = 0,
        Chep = 1,
        Batido = 2,
        PaleteRetorno = 3
    }

    public static class EnumTipoPaleteClienteHelper
    {
        public static string ObterDescricao(this TipoPaleteCliente situacao)
        {
            switch (situacao)
            {
                case TipoPaleteCliente.Chep: return "Chep";
                case TipoPaleteCliente.Batido: return "Batido";
                case TipoPaleteCliente.PaleteRetorno: return "Palete Retorno";
                default: return string.Empty;
            }
        }
    }
}
