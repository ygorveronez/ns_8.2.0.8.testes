namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoGatilhoOcorrenciaInicialFluxoPatio
    {
        InicioCarregamento = 1,
        ChegadaPatio = 2,
        PrevisaoCarregamento = 3,
    }

    public static class TipoGatilhoOcorrenciaInicialFluxoPatioHelper
    {
        public static string ObterDescricao(this TipoGatilhoOcorrenciaInicialFluxoPatio tipo)
        {
            switch (tipo)
            {
                case TipoGatilhoOcorrenciaInicialFluxoPatio.ChegadaPatio: return "Chegada no Patio";
                case TipoGatilhoOcorrenciaInicialFluxoPatio.InicioCarregamento: return "Início de Carregamento";
                case TipoGatilhoOcorrenciaInicialFluxoPatio.PrevisaoCarregamento: return "Previsão do Carregamento";
                default: return string.Empty;
            }
        }
    }
}
