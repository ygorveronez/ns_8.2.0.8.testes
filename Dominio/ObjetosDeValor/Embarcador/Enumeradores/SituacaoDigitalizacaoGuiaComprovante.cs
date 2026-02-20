namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoDigitalizacaoGuiaComprovante
    {
        NaoDigitalizado = 0,
        Digitalizado = 1
    }



    public static class SituacaoDigitalizacaoGuiaComprovanteHelper
    {
        public static string ObterDescricao(this SituacaoDigitalizacaoGuiaComprovante situacaoDigitalizacaoGuia)
        {
            string retorno = "";
            switch (situacaoDigitalizacaoGuia)
            {
                case SituacaoDigitalizacaoGuiaComprovante.Digitalizado:
                    retorno = "Digitalizado";
                    break;
                case SituacaoDigitalizacaoGuiaComprovante.NaoDigitalizado:
                    retorno = "Não Digitalizado";
                    break;
                default:
                    break;
            }
            return retorno;
        }
    }
}
