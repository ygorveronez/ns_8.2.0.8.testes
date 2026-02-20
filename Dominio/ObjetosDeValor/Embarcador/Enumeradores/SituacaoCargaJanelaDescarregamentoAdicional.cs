namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoCargaJanelaDescarregamentoAdicional
    {
        ForaPeriodoDescarregamento = 1
    }

    public static class SituacaoCargaJanelaDescarregamentoAdicionalHelper
    {
        public static string ObterDescricao(this SituacaoCargaJanelaDescarregamentoAdicional situacao)
        {
            switch (situacao)
            {
                case SituacaoCargaJanelaDescarregamentoAdicional.ForaPeriodoDescarregamento: return "Fora do Período de Descarregamento";
                default: return string.Empty;
            }
        }
    }
}
