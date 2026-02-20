namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum FormaGeracaoTituloFatura
    {
        Padrao = 0,
        PorDocumento = 1,
        PorParcela = 2
    }

    public static class FormaGeracaoTituloFaturaHelper
    {
        public static string ObterDescricao(this FormaGeracaoTituloFatura situacao)
        {
            switch (situacao)
            {
                case FormaGeracaoTituloFatura.Padrao: return "Usar o padrão";
                case FormaGeracaoTituloFatura.PorDocumento: return "Gerar título por documento";
                case FormaGeracaoTituloFatura.PorParcela: return "Gerar título por parcela";
                default: return string.Empty;
            }
        }
    }
}
