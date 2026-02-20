namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoDigitalizacaoGuiaRecolhimento
    {
        NaoDigitalizado = 0,
        Digitalizado = 1
    }



    public static class SituacaoDigitalizacaoGuiaRecolhimentoHelper
    {
        public static string ObterDescricao(this SituacaoDigitalizacaoGuiaRecolhimento situacaoDigitalizacaoGuia)
        {
            string retorno = "";
            switch (situacaoDigitalizacaoGuia)
            {
                case SituacaoDigitalizacaoGuiaRecolhimento.Digitalizado:
                    retorno = "Ag. Aprovação";
                    break;
                case SituacaoDigitalizacaoGuiaRecolhimento.NaoDigitalizado:
                    retorno = "Rejeitada";
                    break;
                default:
                    break;
            }
            return retorno;
        }
    }
}
