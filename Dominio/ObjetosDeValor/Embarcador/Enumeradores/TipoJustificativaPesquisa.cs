namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoJustificativaPesquisa
    {
        Todos = 0,
        Desconto = 1,
        Acrescimo = 2
    }

    public static class TipoJustificativaPesquisaHelper
    {
        public static string ObterDescricao(this TipoJustificativaPesquisa tipo)
        {
            switch (tipo)
            {
                case TipoJustificativaPesquisa.Desconto: return "Desconto";
                case TipoJustificativaPesquisa.Acrescimo: return "Acréscimo";
                case TipoJustificativaPesquisa.Todos: return "Todos";
                default: return string.Empty;
            }
        }
    }
}
