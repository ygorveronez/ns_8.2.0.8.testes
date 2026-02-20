namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoCalculoContratoFreteADA
    {
        DiasEntreAgendamentoPrevisaoSaida = 1
    }
    public static class TipoCalculoContratoFreteADAHelper
    {
        public static string ObterDescricao(this TipoCalculoContratoFreteADA tipoCalculo)
        {
            switch (tipoCalculo)
            {
                case TipoCalculoContratoFreteADA.DiasEntreAgendamentoPrevisaoSaida:
                    return "Dias entre o Agendamento e a Previsão de Saída";
                default:
                    return string.Empty;
            }
        }
    }
}
