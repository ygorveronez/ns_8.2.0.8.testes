namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoAbrangenciaExcecaoCapacidadeCarregamento
    {
        Dia = 0,
        Periodo = 1
    }

    public static class TipoAbrangenciaExcecaoCapacidadeCarregamentoHelper
    {
        public static string ObterDescricao(this TipoAbrangenciaExcecaoCapacidadeCarregamento tipoAbrangencia)
        {
            switch (tipoAbrangencia)
            {
                case TipoAbrangenciaExcecaoCapacidadeCarregamento.Dia: return "Dia";
                case TipoAbrangenciaExcecaoCapacidadeCarregamento.Periodo: return "Período";
                default: return string.Empty;
            }
        }
    }
}
