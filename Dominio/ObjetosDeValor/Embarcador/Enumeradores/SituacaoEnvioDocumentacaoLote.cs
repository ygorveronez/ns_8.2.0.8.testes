namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoEnvioDocumentacaoLote
    {
        Aguardando = 1,
        Finalizado = 2,
        Falha = 3
    }

    public static class SituacaoEnvioDocumentacaoLoteHelper
    {
        public static string ObterDescricao(this SituacaoEnvioDocumentacaoLote situacao)
        {
            switch (situacao)
            {
                case SituacaoEnvioDocumentacaoLote.Aguardando: return "Aguardando";
                case SituacaoEnvioDocumentacaoLote.Finalizado: return "Finalizado";
                case SituacaoEnvioDocumentacaoLote.Falha: return "Falha";                
                default: return null;
            }
        }
    }
}
