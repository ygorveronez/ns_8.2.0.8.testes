namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoLeituraOCR
    {
        NaoValidado = 0,
        Inconsistente = 1,
        Validado = 2,
        Rejeitado = 3
    }
    public static class SituacaoLeituraOCRHelper
    {
        public static string ObterDescricao(this SituacaoLeituraOCR obj)
        {
            switch (obj)
            {
                case SituacaoLeituraOCR.NaoValidado:
                    return "Não validado";
                case SituacaoLeituraOCR.Inconsistente:
                    return "Inconsistente";
                case SituacaoLeituraOCR.Validado:
                    return "Validado";
                case SituacaoLeituraOCR.Rejeitado:
                    return "Rejeitado";
                default:
                    return "";
            }
        }
    }
}
