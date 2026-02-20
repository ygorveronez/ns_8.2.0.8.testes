namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoTitulo
    {
        Todos = 0,
        Receber = 1,
        Pagar = 2
    }

    public static class TipoTituloHelper
    {
        public static string ObterDescricao(this TipoTitulo tipo)
        {
            switch (tipo)
            {
                case TipoTitulo.Pagar: return "A Pagar";
                case TipoTitulo.Receber: return "A Receber";
                default: return string.Empty;
            }
        }
    }
}
