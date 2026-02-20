namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum CentroCarregamentoTipoOperacaoTipo
    {
        CapacMaiorVeiculo = 0,
        TotalCliente = 1
    }

    public static class CentroCarregamentoTipoOperacaoTipoHelper
    {
        public static string ObterDescricao(this CentroCarregamentoTipoOperacaoTipo o)
        {
            switch (o)
            {
                case CentroCarregamentoTipoOperacaoTipo.CapacMaiorVeiculo: return "Capacidade maior veículo";
                case CentroCarregamentoTipoOperacaoTipo.TotalCliente: return "Peso total cliente";
                default: return string.Empty;
            }
        }
    }
}
