namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{

    public enum EnumSituacaoNotasPendetesIntegracaoMercadoLivre
    {
        Pendente = 0,
        Concluido = 1,
        Falha = 2,
        Todas = 3
    }

    public static class EnumSituacaoNotasPendetesIntegracaoMercadoLivreHelper
    {
        public static string ObterDescricao(this EnumSituacaoNotasPendetesIntegracaoMercadoLivre situacao)
        {
            switch (situacao)
            {
                case EnumSituacaoNotasPendetesIntegracaoMercadoLivre.Pendente: return "Pendente Download";
                case EnumSituacaoNotasPendetesIntegracaoMercadoLivre.Concluido: return "Concluído";
                case EnumSituacaoNotasPendetesIntegracaoMercadoLivre.Falha: return "Falha";
                case EnumSituacaoNotasPendetesIntegracaoMercadoLivre.Todas: return "Todas";
                default: return string.Empty;
            }
        }
    }

}
