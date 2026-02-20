namespace Dominio.ObjetosDeValor.Enumerador
{
    public enum SituacaoAuditoriaCanhoto
    {
        NaoAuditado = 0,
        Aprovado = 1,
        Reprovado = 2
    }
    public static class SituacaoAuditoriaCanhotoHelper
    {
        public static string ObterDescricao(this SituacaoAuditoriaCanhoto situacaoAuditoriaCanhoto)
        {
            string retorno = "";
            switch (situacaoAuditoriaCanhoto)
            {
                case SituacaoAuditoriaCanhoto.Aprovado:
                    retorno = "Ag. Aprovação";
                    break;
                case SituacaoAuditoriaCanhoto.Reprovado:
                    retorno = "Rejeitada";
                    break;
                default:
                    retorno = "Não Auditado";
                    break;
            }
            return retorno;
        }
    }
}


