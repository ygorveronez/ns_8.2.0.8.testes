namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum CanhotoOrigemDigitalizacao
    {
        Mobile = 1,
        Portal = 2,
        Integracao = 3,
        MobileSemValidacaoAut = 4
    }

    public static class CanhotoOrigemDigitalizacaoHelper
    {
        public static string ObterDescricao(this CanhotoOrigemDigitalizacao origem)
        {
            switch (origem)
            {
                case CanhotoOrigemDigitalizacao.Mobile: return "Mobile";
                case CanhotoOrigemDigitalizacao.Portal: return "Portal";
                case CanhotoOrigemDigitalizacao.Integracao: return "Integração";
                case CanhotoOrigemDigitalizacao.MobileSemValidacaoAut: return "Mobile Sem Validação Aut";
                default: return string.Empty;
            }
        }
    }
}
