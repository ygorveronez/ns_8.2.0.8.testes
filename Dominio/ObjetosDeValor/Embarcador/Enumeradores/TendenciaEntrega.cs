namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TendenciaEntrega
    {
        Nenhum = 0,
        Adiantado = 1,
        Nohorario = 2,
        Poucoatrasado = 3,
        Atrasado = 4
    }

    public static class TendenciaEntregaHelper
    {
        public static string ObterDescricao(this TendenciaEntrega tendenciaEntrega)
        {
            switch (tendenciaEntrega)
            {
                case TendenciaEntrega.Nenhum: return "Não Calculada";
                case TendenciaEntrega.Adiantado: return "Adiantado";
                case TendenciaEntrega.Nohorario: return "No Prazo";
                case TendenciaEntrega.Poucoatrasado: return "Tendência de Atraso";
                case TendenciaEntrega.Atrasado: return "Atrasado";
                default: return "";
            }
        }

        public static string ObterCorTendenciaEntrega (this TendenciaEntrega tendenciaEntrega)
        {
            switch (tendenciaEntrega)
            {
                case TendenciaEntrega.Nenhum: return "black";
                case TendenciaEntrega.Adiantado: return "green";
                case TendenciaEntrega.Nohorario: return "green";
                case TendenciaEntrega.Poucoatrasado: return "orange";
                case TendenciaEntrega.Atrasado: return "red";
                default: return "black";
            }
        }
    }
}
