namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoConciliacaoBancaria
    {
        Todos = 0,
        Aberto = 1,
        Finalizado = 2,
        Cancelado = 3
    }

    public static class SituacaoConciliacaoBancariaHelper
    {
        public static string ObterDescricao(this SituacaoConciliacaoBancaria status)
        {
            switch (status)
            {
                case SituacaoConciliacaoBancaria.Aberto: return "Aberto";
                case SituacaoConciliacaoBancaria.Finalizado: return "Finalizado";
                case SituacaoConciliacaoBancaria.Cancelado: return "Cancelado";
                default: return string.Empty;
            }
        }
    }
}
