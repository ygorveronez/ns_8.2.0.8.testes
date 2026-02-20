namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoContratoFinanciamento
    {
        Todos = 0,
        Aberto = 1,
        Cancelado = 2,
        Finalizado = 3
    }

    public static class SituacaoContratoFinanciamentoHelper
    {
        public static string ObterDescricao(this SituacaoContratoFinanciamento situacao)
        {
            switch (situacao)
            {
                case SituacaoContratoFinanciamento.Aberto: return "Aberto";
                case SituacaoContratoFinanciamento.Finalizado: return "Finalizado";
                case SituacaoContratoFinanciamento.Cancelado: return "Cancelado";
                default: return string.Empty;
            }
        }
    }
}
