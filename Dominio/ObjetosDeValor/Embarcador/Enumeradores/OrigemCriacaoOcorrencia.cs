namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum OrigemCriacaoOcorrencia
    {
        MultiMobile = 1,
        Monitoramento = 2,
        Manual = 3,
        Integracao = 4,
        ManualTransportador = 5,
        ManualEmbarcador = 6,
    }


    public static class OrigemCriacaoOcorrenciaHelper
    {

        public static string ObterDescricao(this OrigemCriacaoOcorrencia situacaoOcorrencia)
        {
            switch (situacaoOcorrencia)
            {
                case OrigemCriacaoOcorrencia.MultiMobile: return "App";
                case OrigemCriacaoOcorrencia.Monitoramento: return "Monitoramento";
                case OrigemCriacaoOcorrencia.Manual: return "Manual";
                case OrigemCriacaoOcorrencia.Integracao: return "Integração";
                case OrigemCriacaoOcorrencia.ManualTransportador: return "Manual - transportador";
                case OrigemCriacaoOcorrencia.ManualEmbarcador: return "Manual - embarcador";
                default: return "";
            }
        }

    }
}

