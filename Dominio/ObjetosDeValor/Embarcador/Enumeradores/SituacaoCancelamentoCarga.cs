namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoCancelamentoCarga
    {
        AgConfirmacao = 0,
        EmCancelamento = 1,
        Cancelada = 2,
        RejeicaoCancelamento = 3,
        Anulada = 4,
        Reprovada = 5,
        AgCancelamentoMDFe = 6,
        AgCancelamentoAverbacaoMDFe = 7,
        AgCancelamentoCTe = 8,
        AgCancelamentoAverbacaoCTe = 9,
        AgIntegracao = 10,
        FinalizandoCancelamento = 11,
        AgAprovacaoSolicitacao = 12,
        SolicitacaoReprovada = 13,
        Cancelamento = 14,
        Anulacao = 15,
        AgIntegracaoDadosCancelamento = 16,
        AgIntegracaoCancelamentoCIOT = 17,
        AgCancelamentoFatura = 18,
        AgCancelamentoGNRE = 19,
    }

    public static class SituacaoCancelamentoCargaHelper
    {
        public static string Descricao(this SituacaoCancelamentoCarga situacaoCancelamentoCarga)
        {
            switch (situacaoCancelamentoCarga)
            {
                case SituacaoCancelamentoCarga.AgAprovacaoSolicitacao: return "Ag. Aprovação Solicitação";
                case SituacaoCancelamentoCarga.AgCancelamentoAverbacaoCTe: return "Cancelando as Averbações dos CT-es";
                case SituacaoCancelamentoCarga.AgCancelamentoAverbacaoMDFe: return "Cancelando as Averbações dos MDF-es";
                case SituacaoCancelamentoCarga.AgCancelamentoCTe: return "Cancelando os CT-es";
                case SituacaoCancelamentoCarga.AgCancelamentoMDFe: return "Cancelando os MDF-es";
                case SituacaoCancelamentoCarga.AgConfirmacao: return "Ag. Confirmação";
                case SituacaoCancelamentoCarga.AgIntegracao: return "Realizando Integrações";
                case SituacaoCancelamentoCarga.Anulada: return "Anulada";
                case SituacaoCancelamentoCarga.Cancelada: return "Cancelada";
                case SituacaoCancelamentoCarga.EmCancelamento: return "Em Cancelamento";
                case SituacaoCancelamentoCarga.FinalizandoCancelamento: return "Finalizando o cancelamento";
                case SituacaoCancelamentoCarga.RejeicaoCancelamento: return "Rejeição no Cancelamento";
                case SituacaoCancelamentoCarga.Reprovada: return "Reprovada";
                case SituacaoCancelamentoCarga.SolicitacaoReprovada: return "Solicitação Reprovada";
                case SituacaoCancelamentoCarga.Cancelamento: return "Cancelamento";
                case SituacaoCancelamentoCarga.Anulacao: return "Anulação";
                case SituacaoCancelamentoCarga.AgIntegracaoDadosCancelamento: return "Ag. Integração Dados Cancelamento";
                case SituacaoCancelamentoCarga.AgIntegracaoCancelamentoCIOT: return "Ag. Cancelamento CIOT";
                case SituacaoCancelamentoCarga.AgCancelamentoFatura: return "Ag. Cancelamento Fatura";
                case SituacaoCancelamentoCarga.AgCancelamentoGNRE: return "Ag. Cancelamento GNRE";
                default: return string.Empty;
            }
        }

        public static bool IspermitirReenvio(this SituacaoCancelamentoCarga situacaoCancelamentoCarga)
        {
            return (
                (situacaoCancelamentoCarga == SituacaoCancelamentoCarga.RejeicaoCancelamento) ||
                (situacaoCancelamentoCarga == SituacaoCancelamentoCarga.SolicitacaoReprovada)
            );
        }
    }
}
