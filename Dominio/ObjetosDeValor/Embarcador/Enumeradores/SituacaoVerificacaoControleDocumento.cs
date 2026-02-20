namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoVerificacaoControleDocumento
    {
        AgVerificacao = 0,
        Verificado = 1
    }

    public static class SituacaoVerificacaoControleDocumentoHelper
    {
        public static bool IsVerificado(this SituacaoVerificacaoControleDocumento situacao)
        {
            return (situacao == SituacaoVerificacaoControleDocumento.Verificado);
        }

        public static string ObterDescricao(this SituacaoVerificacaoControleDocumento situacao)
        {
            switch (situacao)
            {
                case SituacaoVerificacaoControleDocumento.AgVerificacao: return "Aguardando Verificação";
                case SituacaoVerificacaoControleDocumento.Verificado: return "Verificado";
                default: return string.Empty;
            }
        }
    }
}
