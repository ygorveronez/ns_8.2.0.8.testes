namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TAGPedagio
    {
        Valida = 1,
        Invalida = 2
    }

    public static class TAGPedagioHelper
    {
        public static string ObterDescricao(this TAGPedagio situacao)
        {
            switch (situacao)
            {
                case TAGPedagio.Valida: return "Válida";
                case TAGPedagio.Invalida: return "Inválida";
                default: return string.Empty;
            }
        }
    }
}
