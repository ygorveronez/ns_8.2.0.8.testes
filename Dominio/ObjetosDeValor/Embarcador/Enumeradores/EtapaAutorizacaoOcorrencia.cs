namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum EtapaAutorizacaoOcorrencia
    {
        AprovacaoOcorrencia = 0,
        EmissaoOcorrencia = 1
    }

    public static class EtapaAutorizacaoOcorrenciaHelper
    {
        public static string ObterDescricao(this EtapaAutorizacaoOcorrencia etapaAprovacao)
        {
            switch (etapaAprovacao)
            {
                case EtapaAutorizacaoOcorrencia.AprovacaoOcorrencia: return Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.AprovacaoOcorrencia;
                case EtapaAutorizacaoOcorrencia.EmissaoOcorrencia: return Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.EmissaoOcorrencia;
                default: return string.Empty;
            }
        }
    }
}