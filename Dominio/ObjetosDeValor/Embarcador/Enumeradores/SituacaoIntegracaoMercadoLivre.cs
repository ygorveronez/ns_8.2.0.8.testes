namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoIntegracaoMercadoLivre
    {
        Todas = 99,
        PendenteDownload = 0,
        PendenteProcessamento = 1,
        Concluido = 2,
        Desconsiderado = 3
    }

    public static class SituacaoIntegracaoMercadoLivreHelper
    {
        public static string ObterDescricao(this SituacaoIntegracaoMercadoLivre status)
        {
            switch (status)
            {
                case SituacaoIntegracaoMercadoLivre.PendenteDownload: return "Pend. Download";
                case SituacaoIntegracaoMercadoLivre.PendenteProcessamento: return "Pend. Processamento";
                case SituacaoIntegracaoMercadoLivre.Concluido: return "Concluido";
                case SituacaoIntegracaoMercadoLivre.Desconsiderado: return "Desconsiderado";
                case SituacaoIntegracaoMercadoLivre.Todas: return "Todas";
                default: return string.Empty;
            }
        }
    }
}
