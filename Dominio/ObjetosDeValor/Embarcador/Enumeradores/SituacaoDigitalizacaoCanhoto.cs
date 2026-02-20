namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoDigitalizacaoCanhoto
    {
        Todas = 0,
        PendenteDigitalizacao = 1,
        AgAprovocao = 2,
        Digitalizado = 3,
        DigitalizacaoRejeitada = 4,
        Cancelada = 5,
        AgIntegracao = 6,
        ValidacaoEmbarcador = 7
    }



    public static class SituacaoDigitalizacaoCanhotoHelper
    {
        public static string ObterDescricao(this SituacaoDigitalizacaoCanhoto situacaoDigitalizacaoCanhoto)
        {
            string retorno = "";
            switch (situacaoDigitalizacaoCanhoto)
            {
                case SituacaoDigitalizacaoCanhoto.AgAprovocao:
                    retorno = "Ag. Aprovação";
                    break;
                case SituacaoDigitalizacaoCanhoto.DigitalizacaoRejeitada:
                    retorno = "Rejeitada";
                    break;
                case SituacaoDigitalizacaoCanhoto.Digitalizado:
                    retorno = "Digitalizado";
                    break;
                case SituacaoDigitalizacaoCanhoto.PendenteDigitalizacao:
                    retorno = "Pendente";
                    break;
                case SituacaoDigitalizacaoCanhoto.Cancelada:
                    retorno = "Cancelada";
                    break;
                case SituacaoDigitalizacaoCanhoto.AgIntegracao:
                    retorno = "Ag. Integração";
                    break;
                case SituacaoDigitalizacaoCanhoto.ValidacaoEmbarcador:
                    retorno = "Validação Embarcador";
                    break;
                default:
                    break;
            }
            return retorno;
        }
    }
}
