namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoFaturamentoCTe
    {
        Faturado = 1,
        NaoFaturado = 2
    }

    public static class SituacaoFaturamentoCTeHelper
    {
        public static string ObterDescricao(this SituacaoFaturamentoCTe situacao)
        {
            switch (situacao)
            {
                case SituacaoFaturamentoCTe.Faturado: return "Faturado";
                case SituacaoFaturamentoCTe.NaoFaturado: return "Não Faturado";
                default: return string.Empty;
            }
        }
    }
}
