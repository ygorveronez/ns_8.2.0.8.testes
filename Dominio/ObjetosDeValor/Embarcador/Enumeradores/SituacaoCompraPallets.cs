namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoCompraPallets
    {
        AgFinalizacao = 1,
        Finalizado = 2,
        Cancelado = 3
    }

    public static class SituacaoCompraPalletsHelper
    {
        public static string ObterDescricao(this SituacaoCompraPallets situacao)
        {
            switch (situacao)
            {
                case SituacaoCompraPallets.AgFinalizacao:
                    return "Ag. Finalização";
                case SituacaoCompraPallets.Finalizado:
                    return "Finalizado";
                case SituacaoCompraPallets.Cancelado:
                    return "Cancelado";
                default:
                    return "";
            }
        }
    }
}
