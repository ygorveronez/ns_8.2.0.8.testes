namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoGestaoSolicitacaoMonitoramentoIntegracaoPontoControle
    {
        Origem = 1,
        Coleta = 2,
        Entrega = 3,
        Destino = 4
    }

    public static class TipoGestaoSolicitacaoMonitoramentoIntegracaoPontoControleHelper
    {
        public static string ObterDescricao(this TipoGestaoSolicitacaoMonitoramentoIntegracaoPontoControle o)
        {
            switch (o)
            {
                case TipoGestaoSolicitacaoMonitoramentoIntegracaoPontoControle.Origem: return "Origem";
                case TipoGestaoSolicitacaoMonitoramentoIntegracaoPontoControle.Coleta: return "Coleta";
                case TipoGestaoSolicitacaoMonitoramentoIntegracaoPontoControle.Entrega: return "Entrega";
                case TipoGestaoSolicitacaoMonitoramentoIntegracaoPontoControle.Destino: return "Destino";
                default: return string.Empty;
            }
        }
    }
}
