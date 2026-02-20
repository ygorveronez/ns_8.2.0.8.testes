using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoAlteracaoPedido
    {
        AguardandoAprovacao = 1,
        AguardandoAprovacaoTransportador = 2,
        Aprovada = 3,
        Reprovada = 4
    }

    public static class SituacaoAlteracaoPedidoHelper
    {
        public static string ObterDescricao(this SituacaoAlteracaoPedido situacao)
        {
            switch (situacao)
            {
                case SituacaoAlteracaoPedido.AguardandoAprovacao: return "Aguardando Aprovação";
                case SituacaoAlteracaoPedido.AguardandoAprovacaoTransportador: return "Aguardando Aprovação do Transportador";
                case SituacaoAlteracaoPedido.Aprovada: return "Aprovada";
                case SituacaoAlteracaoPedido.Reprovada: return "Reprovada";
                default: return string.Empty;
            }
        }

        public static List<SituacaoAlteracaoPedido> ObterSituacoesFinalizadas()
        {
            return new List<SituacaoAlteracaoPedido>()
            {
                SituacaoAlteracaoPedido.Aprovada,
                SituacaoAlteracaoPedido.Reprovada
            };
        }

        public static List<SituacaoAlteracaoPedido> ObterSituacoesPendentes()
        {
            return new List<SituacaoAlteracaoPedido>()
            {
                SituacaoAlteracaoPedido.AguardandoAprovacao,
                SituacaoAlteracaoPedido.AguardandoAprovacaoTransportador
            };
        }
    }
}
