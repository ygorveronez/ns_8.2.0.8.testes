namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoJustificativa
    {
        Todos = 0,
        Desconto = 1,
        Acrescimo = 2
    }

    public static class TipoJustificativaHelper
    {
        public static string ObterDescricao(this TipoJustificativa tipo)
        {
            switch (tipo)
            {
                case TipoJustificativa.Desconto: return "Desconto";
                case TipoJustificativa.Acrescimo: return "Acréscimo";
                default: return string.Empty;
            }
        }
    }
}
