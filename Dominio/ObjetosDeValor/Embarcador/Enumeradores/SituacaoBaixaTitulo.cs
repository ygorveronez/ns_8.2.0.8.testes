namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoBaixaTitulo
    {
        Iniciada = 1,
        EmNegociacao = 2,
        Finalizada = 3,
        Cancelada = 4,
        EmGeracao = 5,
        EmFinalizacao = 6
    }

    public static class SituacaoBaixaTituloHelper
    {
        public static string ObterDescricao(this SituacaoBaixaTitulo situacaoBaixaTitulo)
        {
            switch (situacaoBaixaTitulo)
            {
                case SituacaoBaixaTitulo.Iniciada:
                    return "Iniciada";
                case SituacaoBaixaTitulo.EmNegociacao:
                    return "Em Negociação";
                case SituacaoBaixaTitulo.Finalizada:
                    return "Finalizada";
                case SituacaoBaixaTitulo.Cancelada:
                    return "Cancelada";
                case SituacaoBaixaTitulo.EmGeracao:
                    return "Em Geração";
                case SituacaoBaixaTitulo.EmFinalizacao:
                    return "Em Finalização";
                default:
                    return "";
            }
        }
    }
}
