namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum MonitoramentoEventoData
    {
        Padrao = 0,
        PosicaoVeiculo = 1,
        EntradaCliente = 2,
        SaidaCliente = 3,
        JanelaCarregamento = 4,
        JanelaDescarregamento = 5,
        EntregaPedidoPrevista = 6,
        InicioEntregaPrevista = 7,
        FinalEntregaPrevista = 8,
        InicioEntregaReprogramada = 9,
        FinalEntregaReprogramada = 10,
        InicioEntregaRealizada = 11,
        FinalEntregaRealizada = 12,
        DataAtual = 13,
        DataAgendamentodeEntrega = 14,
        ETAPrimeiraColeta = 15,
        DataCarregamento = 16,
        PrevisaoEntrega = 17,
    }

    public static class MonitoramentoEventoDataHelper
    {
        public static string ObterDescricao(this MonitoramentoEventoData data)
        {
            switch (data)
            {
                case MonitoramentoEventoData.Padrao: return "Padrão";
                case MonitoramentoEventoData.DataAtual: return "Data atual";
                case MonitoramentoEventoData.PosicaoVeiculo: return "Posição do veículo";
                case MonitoramentoEventoData.EntradaCliente: return "Entrada no cliente";
                case MonitoramentoEventoData.SaidaCliente: return "Saída do cliente";
                case MonitoramentoEventoData.JanelaCarregamento: return "Janela de carregamento";
                case MonitoramentoEventoData.JanelaDescarregamento: return "Janela de descarregamento";
                case MonitoramentoEventoData.EntregaPedidoPrevista: return "Entrega do pedido prevista";
                case MonitoramentoEventoData.InicioEntregaPrevista: return "Início da entrega prevista";
                case MonitoramentoEventoData.FinalEntregaPrevista: return "Fim da entrega prevista";
                case MonitoramentoEventoData.InicioEntregaReprogramada: return "Início da entrega reprogramada";
                case MonitoramentoEventoData.FinalEntregaReprogramada: return "Fim da entrega reprogramada";
                case MonitoramentoEventoData.InicioEntregaRealizada: return "Início da entrega realizada";
                case MonitoramentoEventoData.FinalEntregaRealizada: return "Fim da entrega realizada";
                case MonitoramentoEventoData.DataAgendamentodeEntrega: return "Data agendamento de entrega";
                case MonitoramentoEventoData.ETAPrimeiraColeta: return "ETA da primeira coleta";
                case MonitoramentoEventoData.DataCarregamento: return "Data de Carregamento";
                case MonitoramentoEventoData.PrevisaoEntrega: return "Previsão de Entrega";
                default: return "";
            }
        }
    }
}
