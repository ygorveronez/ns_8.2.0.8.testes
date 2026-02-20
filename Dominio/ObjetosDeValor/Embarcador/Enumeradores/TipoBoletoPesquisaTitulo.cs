namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoBoletoPesquisaTitulo
    {
        Todos = 0,
        ComBoleto = 1,
        SemBoleto = 2,
        ComRemessa = 3,
        SemRemessa = 4,
    }

    public static class TipoBoletoPesquisaTituloHelper
    {
        public static string ObterDescricao(this TipoBoletoPesquisaTitulo tipoBoleto)
        {
            switch (tipoBoleto)
            {
                case TipoBoletoPesquisaTitulo.ComBoleto: return "Com Boleto";
                case TipoBoletoPesquisaTitulo.SemBoleto: return "Sem Boleto";
                case TipoBoletoPesquisaTitulo.ComRemessa: return "Com Remessa";
                case TipoBoletoPesquisaTitulo.SemRemessa: return "Sem Remessa";
                default: return string.Empty;
            }
        }
    }
}
