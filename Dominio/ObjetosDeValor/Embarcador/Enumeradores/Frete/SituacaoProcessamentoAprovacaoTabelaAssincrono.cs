namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores.Frete
{
    public enum SituacaoProcessamentoAprovacaoTabelaAssincrono
    {
        Pendente = 0,
        Processando = 1,
        Processado = 2,
        FalhaNoProcessamento = 3
    }

    public static class SituacaoProcessamentoAprovacaoTabelaAssincronoHelper
    {
        public static string ObterDescricao(this SituacaoProcessamentoAprovacaoTabelaAssincrono situacao)
        {
            switch (situacao)
            {
                case SituacaoProcessamentoAprovacaoTabelaAssincrono.Pendente:
                    return "Pendente";
                case SituacaoProcessamentoAprovacaoTabelaAssincrono.Processando:
                    return "Processando";
                case SituacaoProcessamentoAprovacaoTabelaAssincrono.Processado:
                    return "Processado";
                case SituacaoProcessamentoAprovacaoTabelaAssincrono.FalhaNoProcessamento:
                    return "Falha no Processamento";
                default:
                    return string.Empty;
            }
        }
    }
}
