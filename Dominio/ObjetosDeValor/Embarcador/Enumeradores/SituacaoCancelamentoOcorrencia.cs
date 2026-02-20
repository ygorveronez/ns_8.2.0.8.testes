namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoCancelamentoOcorrencia
    {
        EmCancelamento = 1,
        Cancelada = 2,
        RejeicaoCancelamento = 3,
        AguardandoIntegracao = 4,
        FalhaIntegracao = 5
    }

    public static class SituacaoCancelamentoOcorrenciaHelper
    {
        public static string ObterDescricao(this SituacaoCancelamentoOcorrencia situacao)
        {
            switch (situacao)
            {
                case SituacaoCancelamentoOcorrencia.AguardandoIntegracao: return "Aguardando Integração";
                case SituacaoCancelamentoOcorrencia.Cancelada: return "Cancelada";
                case SituacaoCancelamentoOcorrencia.EmCancelamento: return "Em Cancelamento";
                case SituacaoCancelamentoOcorrencia.FalhaIntegracao: return "Falha na Integração";
                case SituacaoCancelamentoOcorrencia.RejeicaoCancelamento: return "Rejeição no Cancelamento";
                default: return string.Empty;
            }
        }
    }
}
