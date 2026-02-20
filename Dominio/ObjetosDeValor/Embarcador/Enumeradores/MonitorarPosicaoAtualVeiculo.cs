namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum MonitorarPosicaoAtualVeiculo
    {
        Todos = 1,
        ComMonitoramentoEmAndamento = 2,
        PossuiContratoDeFrete = 3,
        ComMonitoramentoEmAndamentoOuPossuiContratoDeFrete = 4
    }

    public static class MonitorarPosicaoAtualVeiculoHelper
    {
        public static string ObterDescricao(this MonitorarPosicaoAtualVeiculo m)
        {
            switch (m)
            {
                case MonitorarPosicaoAtualVeiculo.Todos: return "Todos os veículos";
                case MonitorarPosicaoAtualVeiculo.ComMonitoramentoEmAndamento: return "Com monitoramento em andamento";
                case MonitorarPosicaoAtualVeiculo.PossuiContratoDeFrete: return "Possui contrato de frete";
                case MonitorarPosicaoAtualVeiculo.ComMonitoramentoEmAndamentoOuPossuiContratoDeFrete: return "Com monitoramento em andamento ou possui contrato de frete";
                default: return string.Empty;
            }
        }
    }
}



