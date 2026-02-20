namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoGatilhoOcorrencia
    {
        FluxoPatio = 1,
        Tracking = 2,
        AlteracaoData = 3,
        AtingirData = 4,
    }

    public static class TipoGatilhoOcorrenciaHelper
    {
        public static string ObterDescricao(this TipoGatilhoOcorrencia tipo)
        {
            switch (tipo)
            {
                case TipoGatilhoOcorrencia.FluxoPatio: return "Fluxo de Pátio";
                case TipoGatilhoOcorrencia.Tracking: return "Tracking";
                case TipoGatilhoOcorrencia.AlteracaoData: return "Alteração de data";
                case TipoGatilhoOcorrencia.AtingirData: return "Atingir data";
                default: return string.Empty;
            }
        }
    }
}
