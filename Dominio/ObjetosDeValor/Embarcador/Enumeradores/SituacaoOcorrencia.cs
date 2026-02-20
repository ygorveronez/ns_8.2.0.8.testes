using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoOcorrencia
    {
        Todas = 0,
        AgConfirmacaoUso = 1,
        AgAprovacao = 2,
        Finalizada = 3,
        Rejeitada = 4,
        RejeitadaEtapaEmissao = 19,
        //AgEmissaoCTeComplementar = 6,
        EmEmissaoCTeComplementar = 7,
        Cancelada = 9,
        PendenciaEmissao = 10,
        AgIntegracao = 12,
        FalhaIntegracao = 13,
        AgInformacoes = 14,
        AgAutorizacaoEmissao = 15,
        AutorizacaoPendente = 16,
        SemRegraAprovacao = 17,
        SemRegraEmissao = 18,
        AgAceiteTransportador = 20,
        DebitoRejeitadoTransportador = 21,
        /// <summary>
        /// esta situação é utilizada apenas no relatório, pois a ocorrência fica com status cancelado quando o cancelamento é do tipo anulação
        /// </summary>
        Anulada = 22
    }

    public static class SituacaoOcorrenciaHelper
    {
        public static bool IsPermiteDelegar(this SituacaoOcorrencia situacao)
        {
            return (
                (situacao == SituacaoOcorrencia.AgAprovacao) ||
                (situacao == SituacaoOcorrencia.AgAutorizacaoEmissao) ||
                (situacao == SituacaoOcorrencia.AutorizacaoPendente)
            );
        }

        public static bool IsPermiteVoltarParaEtapaCadastro(this SituacaoOcorrencia situacao)
        {
            return (
                (situacao == SituacaoOcorrencia.AgAprovacao) ||
                (situacao == SituacaoOcorrencia.Rejeitada) ||
                (situacao == SituacaoOcorrencia.SemRegraAprovacao)
            );
        }

        public static string ObterDescricao(this SituacaoOcorrencia situacao)
        {
            switch (situacao)
            {
                case SituacaoOcorrencia.AgAceiteTransportador: return Localization.Resources.Enumeradores.SituacaoOcorrencia.AgAceiteTransportador;
                case SituacaoOcorrencia.AgAprovacao: return Localization.Resources.Enumeradores.SituacaoOcorrencia.AgAprovacao;
                case SituacaoOcorrencia.AgAutorizacaoEmissao: return Localization.Resources.Enumeradores.SituacaoOcorrencia.AgAprovacaoEmissao;
                case SituacaoOcorrencia.AgConfirmacaoUso: return Localization.Resources.Enumeradores.SituacaoOcorrencia.AgConfirmacaoUso;
                //case SituacaoOcorrencia.AgEmissaoCTeComplementar: return "Ag. Emissão de Documento Complementar";
                case SituacaoOcorrencia.AgInformacoes: return Localization.Resources.Enumeradores.SituacaoOcorrencia.AgInformacoes;
                case SituacaoOcorrencia.AgIntegracao: return Localization.Resources.Enumeradores.SituacaoOcorrencia.AgIntegracao;
                case SituacaoOcorrencia.Anulada: return Localization.Resources.Enumeradores.SituacaoOcorrencia.Anulada;
                case SituacaoOcorrencia.AutorizacaoPendente: return Localization.Resources.Enumeradores.SituacaoOcorrencia.AprovacaoPendente;
                case SituacaoOcorrencia.Cancelada: return Localization.Resources.Enumeradores.SituacaoOcorrencia.Cancelada;
                case SituacaoOcorrencia.DebitoRejeitadoTransportador: return Localization.Resources.Enumeradores.SituacaoOcorrencia.DebitoReijeitadoPeloTransportador;
                case SituacaoOcorrencia.EmEmissaoCTeComplementar: return Localization.Resources.Enumeradores.SituacaoOcorrencia.EmEmissaoDocumentoComplementar;
                case SituacaoOcorrencia.FalhaIntegracao: return Localization.Resources.Enumeradores.SituacaoOcorrencia.FalhaNaIntegracao;
                case SituacaoOcorrencia.Finalizada: return Localization.Resources.Enumeradores.SituacaoOcorrencia.Finalizada;
                case SituacaoOcorrencia.PendenciaEmissao: return Localization.Resources.Enumeradores.SituacaoOcorrencia.PendenciaEmissao;
                case SituacaoOcorrencia.Rejeitada: return Localization.Resources.Enumeradores.SituacaoOcorrencia.Rejeitada;
                case SituacaoOcorrencia.RejeitadaEtapaEmissao: return Localization.Resources.Enumeradores.SituacaoOcorrencia.RejeitadaEtapaEmissao;
                case SituacaoOcorrencia.SemRegraAprovacao: return Localization.Resources.Enumeradores.SituacaoOcorrencia.SemRegraAprovacao;
                case SituacaoOcorrencia.SemRegraEmissao: return Localization.Resources.Enumeradores.SituacaoOcorrencia.SemRegraEmissao;
                default: return string.Empty;
            }
        }

        public static List<SituacaoOcorrencia> ObterSituacoesAprovadasOuEmAprovacao()
        {
            return new List<SituacaoOcorrencia>()
            {
                SituacaoOcorrencia.AgAprovacao,
                SituacaoOcorrencia.AgAutorizacaoEmissao,
                SituacaoOcorrencia.AutorizacaoPendente,
                SituacaoOcorrencia.SemRegraAprovacao,
                SituacaoOcorrencia.FalhaIntegracao,
                SituacaoOcorrencia.AgAceiteTransportador,
                SituacaoOcorrencia.AgInformacoes,
                SituacaoOcorrencia.PendenciaEmissao,
                SituacaoOcorrencia.EmEmissaoCTeComplementar,
                SituacaoOcorrencia.AgConfirmacaoUso,
                SituacaoOcorrencia.Finalizada
            };

        }
    }
}
