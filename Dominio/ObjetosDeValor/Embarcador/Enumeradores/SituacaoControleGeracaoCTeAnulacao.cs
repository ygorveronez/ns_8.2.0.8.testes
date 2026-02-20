namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoControleGeracaoCTeAnulacao
    {
        AguardandoAutorizacaoAnulacao = 0,
        GerandoCTeSubstituicao = 1,
        AguardandoAutorizacaoSubstituicao = 2,
        Finalizado = 3
    }
}
