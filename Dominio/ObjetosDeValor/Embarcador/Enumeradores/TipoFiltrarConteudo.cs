namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoFiltrarConteudo
    {
        TextoLivre = 1,
        ExpressaoRegular = 2,
    }

    public static class TipoFiltrarConteudoHelper
    {
        public static string ObterDescricao(this TipoFiltrarConteudo tipoFiltrarConteudo)
        {
            switch (tipoFiltrarConteudo)
            {
                case TipoFiltrarConteudo.TextoLivre: return "Texto Livre";
                case TipoFiltrarConteudo.ExpressaoRegular: return "Expressão Regular";
                default: return string.Empty;
            }
        }
    }
}
