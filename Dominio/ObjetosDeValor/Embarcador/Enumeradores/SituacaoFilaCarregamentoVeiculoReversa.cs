namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoFilaCarregamentoVeiculoReversa
    {
        AguardandoDescarregamento = 1,
        EmDescarregamento = 2,
        Finalizada = 3,
        Cancelada = 4
    }

    public static class SituacaoFilaCarregamentoVeiculoReversaHelper
    {
        public static string ObterCorLinha(this SituacaoFilaCarregamentoVeiculoReversa situacao)
        {
            switch (situacao)
            {
                case SituacaoFilaCarregamentoVeiculoReversa.AguardandoDescarregamento: return "#ffe699";
                case SituacaoFilaCarregamentoVeiculoReversa.Cancelada: return "#ff9999";
                case SituacaoFilaCarregamentoVeiculoReversa.EmDescarregamento: return "#cce6ff";
                case SituacaoFilaCarregamentoVeiculoReversa.Finalizada: return "#e6ffcc";
                default: return ""; 
            }
        }
    }
}
