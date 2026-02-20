namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoTituloNegociacao
    {
        Todos = 0,
        Originais = 1,
        Negociacao = 2
    }

    public static class TipoTituloNegociacaoHelper
    {
        public static string ObterDescricao(this TipoTituloNegociacao status)
        {
            switch (status)
            {
                case TipoTituloNegociacao.Todos: return "Todos";
                case TipoTituloNegociacao.Originais: return "Originais";
                case TipoTituloNegociacao.Negociacao: return "Gerados de Negociação";
                default: return string.Empty;
            }
        }
    }
}
