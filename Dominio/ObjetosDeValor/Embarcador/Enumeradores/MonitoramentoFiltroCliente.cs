namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum MonitoramentoFiltroCliente
    {
        Nenhum = 0,
        EmAlvo = 1,
        ComColeta = 2,
        ComEntrega = 3,
        ComColetaOuEntrega = 4
    }

    public static class MonitoramentoFiltroClienteHelper
    {
        public static string ObterDescricao(this MonitoramentoFiltroCliente monitoramentoFiltroCliente)
        {
            switch (monitoramentoFiltroCliente)
            {
                case MonitoramentoFiltroCliente.Nenhum: return "Nenhum";
                case MonitoramentoFiltroCliente.EmAlvo: return "Em alvo";
                case MonitoramentoFiltroCliente.ComColeta: return "Com coleta";
                case MonitoramentoFiltroCliente.ComEntrega: return "Com entrega";
                case MonitoramentoFiltroCliente.ComColetaOuEntrega: return "Com coleta ou entrega";
                default: return string.Empty;
            }
        }
    }
}
