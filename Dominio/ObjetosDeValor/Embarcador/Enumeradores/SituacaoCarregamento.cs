using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoCarregamento
    {
        EmMontagem = 1,
        Fechado = 2,
        Cancelado = 3,
        AguardandoAprovacaoSolicitacao = 4,
        SolicitacaoReprovada = 5,
        Bloqueado = 6,
        GerandoCargaBackground = 7,
        FalhaIntegracao = 8
    }   

    public static class SituacaoCarregamentoHelper
    {
        public static string ObterDescricao(this SituacaoCarregamento situacao)
        {
            switch (situacao)
            {
                case SituacaoCarregamento.AguardandoAprovacaoSolicitacao: return "Aguardando Aprovação da Solicitação";
                case SituacaoCarregamento.Cancelado: return "Cancelado";
                case SituacaoCarregamento.EmMontagem: return "Em Montagem";
                case SituacaoCarregamento.Fechado: return "Fechado";
                case SituacaoCarregamento.SolicitacaoReprovada: return "Solicitação Reprovada";
                case SituacaoCarregamento.GerandoCargaBackground: return "Gerando carga em segundo plano";
                case SituacaoCarregamento.FalhaIntegracao: return "Falha na integração";
                default: return "";
            }
        }

        public static string ObterDescricaoRetirada(this SituacaoCarregamento situacao)
        {
            switch (situacao)
            {
                case SituacaoCarregamento.AguardandoAprovacaoSolicitacao: return Localization.Resources.Enumeradores.SituacaoCarregamento.Enviado;
                case SituacaoCarregamento.Bloqueado: return Localization.Resources.Enumeradores.SituacaoCarregamento.Finalizado;
                case SituacaoCarregamento.EmMontagem: return Localization.Resources.Enumeradores.SituacaoCarregamento.EmEdicao;
                case SituacaoCarregamento.FalhaIntegracao: return Localization.Resources.Enumeradores.SituacaoCarregamento.FalhaIntegracao;
                case SituacaoCarregamento.Fechado: return Localization.Resources.Enumeradores.SituacaoCarregamento.Confirmado;
                default: return ObterDescricao(situacao);
            }
        }

        public static List<SituacaoCarregamento> ObterSituacoesCarregamentoPendente()
        {
            return new List<SituacaoCarregamento>()
            {
                SituacaoCarregamento.AguardandoAprovacaoSolicitacao,
                SituacaoCarregamento.EmMontagem,
                SituacaoCarregamento.SolicitacaoReprovada
            };
        }

        public static bool CarregamentoPendente(this SituacaoCarregamento situacao)
        {
            return ObterSituacoesCarregamentoPendente().Contains(situacao);
        }
    }
}
