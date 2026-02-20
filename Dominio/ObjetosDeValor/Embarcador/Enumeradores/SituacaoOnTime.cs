namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoOnTime
    {
        NaoAjustado = 0,
        ForaDoPrazo = 1,
        NoPrazo = 2
    }

    public static class SituacaoOnTimeHelper
    {
        public static string ObterDescricao(this SituacaoOnTime situacao)
        {
            switch (situacao)
            {
                case SituacaoOnTime.NaoAjustado: return "Não Ajustado";
                case SituacaoOnTime.ForaDoPrazo: return "Fora do Prazo";
                case SituacaoOnTime.NoPrazo: return "No Prazo";

                default: return string.Empty;
            }
        }
    }
}
