namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoModalCotacaoEspecial
	{
        Todos = 0,
        Expresso = 1,
        Dedicado = 2
    }

    public static class TipoModalCotacaoEspecialHelper
	{
        public static string ObterDescricao(this TipoModalCotacaoEspecial tipo)
        {
            switch (tipo)
            {
                case TipoModalCotacaoEspecial.Todos: return "Todos";
                case TipoModalCotacaoEspecial.Expresso: return "Expresso";
                case TipoModalCotacaoEspecial.Dedicado: return "Dedicado";
                default: return string.Empty;
            }
        }
    }
}
