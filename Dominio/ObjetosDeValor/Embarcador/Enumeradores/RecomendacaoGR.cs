namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum RecomendacaoGR
    {
        AguardandoIntegracao = 1,
        AguardandoValidacaoGR = 2,
        Recomendado = 3,
        NaoRecomendado = 4,
        Falha = 5,
        PendenciaDocumentos = 6
    }

    public static class RecomendacaoGRHelper
    {
        public static string ObterDescricao(this RecomendacaoGR recomendacaoGR)
        {
            switch (recomendacaoGR)
            {
                case RecomendacaoGR.AguardandoIntegracao: return "Aguardando Consulta";
                case RecomendacaoGR.AguardandoValidacaoGR: return "Aguardando Validação GR";
                case RecomendacaoGR.Recomendado: return "Recomendado";
                case RecomendacaoGR.NaoRecomendado: return "Não Recomendado";
                case RecomendacaoGR.Falha: return "Falha";
                case RecomendacaoGR.PendenciaDocumentos: return "Pendência de Documentos";
                default: return string.Empty;
            }
        }

    }
}
