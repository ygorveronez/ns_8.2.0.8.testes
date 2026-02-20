namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoReformaPallet
    {
        Todas = 0,
        AguardandoNfeSaida = 1,
        AguardandoRetorno = 2,
        CanceladaNfeSaida = 3,
        CanceladaRetorno = 4,
        Finalizada = 5
    }

    public static class SituacaoReformaPalletHelper
    {
        public static string ObterDescricao(this SituacaoReformaPallet situacaoReformaPallet)
        {
            switch (situacaoReformaPallet)
            {
                case SituacaoReformaPallet.AguardandoNfeSaida: return "Aguardando NF-e de Saída";
                case SituacaoReformaPallet.AguardandoRetorno: return "Aguardando Retorno";
                case SituacaoReformaPallet.Finalizada: return "Finalizada";
                case SituacaoReformaPallet.CanceladaNfeSaida: return "Cancelada (NF-e de Saída)";
                case SituacaoReformaPallet.CanceladaRetorno: return "Cancelada (Retorno)";
                default: return "Todas";
            }
        }
    }
}
