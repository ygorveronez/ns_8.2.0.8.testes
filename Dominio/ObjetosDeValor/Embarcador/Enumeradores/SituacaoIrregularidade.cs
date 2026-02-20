namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoIrregularidade
    {
        Aprovada= 1,
        Reprovada = 2,
        AguardandoAprovacao = 3,
        CTECancelado = 4
    }

    public static class SituacaoIrregularidadeExtensao
    {
        public static string ObterDescricao(this SituacaoIrregularidade situacaoIrregularidade)
        {
            switch (situacaoIrregularidade)
            {
                case SituacaoIrregularidade.Aprovada:
                    return "Aprovada";
                case SituacaoIrregularidade.Reprovada:
                    return "Reprovada";
                case SituacaoIrregularidade.AguardandoAprovacao:
                    return "Aguardando Aprovação";    
                case SituacaoIrregularidade.CTECancelado:
                    return "CTe Cancelado";
                default:
                    return "Não definido";
            }
        }
    }
}
