namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoImpressao
    {
        Pendente = 0,
        Solicitado = 1,
        Impresso = 2,
        Falha = 9
    }

    public static class SituacaoImpressaoDescricao
    {
        public static string Descricao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImpressao situacaoImpressao)
        {
            switch (situacaoImpressao)
            {
                case ObjetosDeValor.Embarcador.Enumeradores.SituacaoImpressao.Pendente:
                    return "Pendente solicitação de Impressão";
                case ObjetosDeValor.Embarcador.Enumeradores.SituacaoImpressao.Solicitado:
                    return "Impressão solicitada";
                case ObjetosDeValor.Embarcador.Enumeradores.SituacaoImpressao.Impresso:
                    return "Impresso com sucesso";
                case ObjetosDeValor.Embarcador.Enumeradores.SituacaoImpressao.Falha:
                    return "Falha na impressão";
                default:
                    return "";
            }
        }
    }
}