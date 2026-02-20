namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoIntegracaoEMP
    {
        NotPersist = 0,
        Integrado = 1,
        ErroResolvido = 2,
        NaoInformado = 10,
    }

    public static class SituacaoIntegracaoEMPHelper
    {
        public static string ObterDescricao(this SituacaoIntegracaoEMP status)
        {
            switch (status)
            {
                case SituacaoIntegracaoEMP.NotPersist: return "Not Persist";
                case SituacaoIntegracaoEMP.Integrado: return "Integrado";
                case SituacaoIntegracaoEMP.ErroResolvido: return "Erro Resolvido";
                case SituacaoIntegracaoEMP.NaoInformado: return "Não Informado";
                default: return string.Empty;
            }
        }
    }
}
