namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum CargaIndicadorVeiculoMotorista
    {
        NaoInformado = 0,
        InformadoEmbarcador = 1,
        InformadoTransportador = 2,
    }

    public static class CargaIndicadorVeiculoMotoristaHelper
    {
        public static string ObterDescricao(this CargaIndicadorVeiculoMotorista indicador)
        {
            switch (indicador)
            {
                case CargaIndicadorVeiculoMotorista.InformadoEmbarcador: return "Definido pelo embarcador";
                case CargaIndicadorVeiculoMotorista.InformadoTransportador: return "Definido pelo transportador";
                default: return string.Empty;
            }
        }
    }
}
