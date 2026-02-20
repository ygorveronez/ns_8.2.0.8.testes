namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoTransportadorCentroCarregamento
    {
        Todos = 1,
        TodosComTipoVeiculoCarga = 2,
        TodosCentroCarregamento = 3,
        TodosCentroCarregamentoComTipoVeiculoCarga = 4,
        PorPrioridadeDeRota = 5,
        PorGrupoRegional = 6,
        TransportadorExclusivo = 7,
        PrioridadePorFilaCarregamento = 8
    }

    public static class TipoTransportadorCentroCarregamentoHelper
    {
        public static string ObterDescricao(this TipoTransportadorCentroCarregamento tipo)
        {
            switch (tipo)
            {
                case TipoTransportadorCentroCarregamento.Todos: return Localization.Resources.Gerais.Geral.Todos;
                case TipoTransportadorCentroCarregamento.TodosComTipoVeiculoCarga: return Localization.Resources.Enumeradores.TipoTransportadorCentroCarregamento.TodosComTipoDeVeiculoDaCarga;
                case TipoTransportadorCentroCarregamento.TodosCentroCarregamento: return Localization.Resources.Enumeradores.TipoTransportadorCentroCarregamento.TodosDoCentroDeCarregamento;
                case TipoTransportadorCentroCarregamento.TodosCentroCarregamentoComTipoVeiculoCarga: return Localization.Resources.Enumeradores.TipoTransportadorCentroCarregamento.TodosDoCentroDeCarregamentoComTipoDeVeiculoDaCarga;
                case TipoTransportadorCentroCarregamento.PorPrioridadeDeRota: return Localization.Resources.Enumeradores.TipoTransportadorCentroCarregamento.PorPrioridadeDeTransportadorNaRota;
                case TipoTransportadorCentroCarregamento.PorGrupoRegional: return Localization.Resources.Enumeradores.TipoTransportadorCentroCarregamento.PorGrupoRegional;
                case TipoTransportadorCentroCarregamento.TransportadorExclusivo: return Localization.Resources.Enumeradores.TipoTransportadorCentroCarregamento.TransportadorExclusivo;
                case TipoTransportadorCentroCarregamento.PrioridadePorFilaCarregamento: return Localization.Resources.Enumeradores.TipoTransportadorCentroCarregamento.PorFilaCarregamento;

                default: return string.Empty;
            }
        }
    }
}
