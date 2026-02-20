namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoNaoComparecimento
    {
        Compareceu = 0,
        NaoCompareceu = 1,
        NaoCompareceuComFalha = 2
    }

    public static class TipoNaoComparecidoHelper
    {
        public static string ObterDescricao(this TipoNaoComparecimento tipo)
        {
            switch (tipo)
            {
                case TipoNaoComparecimento.Compareceu: return "Compareceu";
                case TipoNaoComparecimento.NaoCompareceu: return "No-show";
                case TipoNaoComparecimento.NaoCompareceuComFalha: return "Falha";
                default: return string.Empty;
            }
        }
    }
}
