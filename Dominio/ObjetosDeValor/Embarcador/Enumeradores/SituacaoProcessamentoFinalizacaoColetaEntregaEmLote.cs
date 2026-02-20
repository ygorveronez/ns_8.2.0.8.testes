
namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoProcessamentoFinalizacaoColetaEntregaEmLote
    {
        PendenteFinalizacao = 0,
        EmFinalizacao = 1,
        Finalizado = 2,
        FalhaNaFinalizacao = 3
    }

    public static class SituacaoProcessamentoFinalizacaoColetaEntregaEmLoteHelper
    {
        public static string ObterDescricao(this SituacaoProcessamentoFinalizacaoColetaEntregaEmLote situacao)
        {
            switch (situacao)
            {
                case SituacaoProcessamentoFinalizacaoColetaEntregaEmLote.PendenteFinalizacao:
                    return "Pendente de finalização";
                case SituacaoProcessamentoFinalizacaoColetaEntregaEmLote.EmFinalizacao:
                    return "Em finalização";
                case SituacaoProcessamentoFinalizacaoColetaEntregaEmLote.Finalizado:
                    return "Finalizado";
                case SituacaoProcessamentoFinalizacaoColetaEntregaEmLote.FalhaNaFinalizacao:
                    return "Falha ao Finalizar";
                default:
                    return "";
            }
        }
    }
}
