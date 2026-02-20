namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoDocumento
    {
        NaoLocalizado = 0,
        Aprovado = 1,
        Rejeitado = 3,
        Inconsistente = 4
    }

    public static class SituacaoDocumentoHelper
    {
        public static string ObterDescricao(this SituacaoDocumento situacao)
        {
            switch (situacao)
            {
                case SituacaoDocumento.NaoLocalizado: return "Não localizado";
                case SituacaoDocumento.Aprovado: return "Aprovado";
                case SituacaoDocumento.Rejeitado: return "Rejeitado";
                case SituacaoDocumento.Inconsistente: return "Inconsistente";
                default: return null;
            }
        }
    }
}
