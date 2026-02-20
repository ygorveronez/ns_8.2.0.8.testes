namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoDiasAprovacao
    {
        DiasCorridos = 0,
        DiasUteis = 1
    }

    public static class TipoDiasAprovacaoHelper
    {
        public static string ObterDescricao(this TipoDiasAprovacao tipoDiasAprovacao)
        {
            switch (tipoDiasAprovacao)
            {
                case TipoDiasAprovacao.DiasCorridos: return "Dias Corridos";
                case TipoDiasAprovacao.DiasUteis: return "Dias Úteis";
                default: return string.Empty;
            }
        }
    }
}
