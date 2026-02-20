namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum QuandoIniciarMonitoramento
    {
        NaoIniciar = 1,
        AoGerarCarga = 2,
        AoInformarVeiculoNaCarga = 3,
        AoInformarVeiculoNaCargaECargaEmTransporte = 4,
        AoIniciarViagem = 5,
        EstouIndoAoIniciarViagem = 6
    }

    public static class QuandoIniciarMonitoramentoHelper
    {
        public static string ObterDescricao(this QuandoIniciarMonitoramento o)
        {
            switch (o)
            {
                case QuandoIniciarMonitoramento.NaoIniciar: return "Não iniciar";
                case QuandoIniciarMonitoramento.AoGerarCarga: return "Ao gerar carga";
                case QuandoIniciarMonitoramento.AoInformarVeiculoNaCarga: return "Ao informar o veículo na carga";
                case QuandoIniciarMonitoramento.AoInformarVeiculoNaCargaECargaEmTransporte: return "Ao informar o veículo na carga e carga estiver em transporte";
                case QuandoIniciarMonitoramento.AoIniciarViagem: return "Ao iniciar Viagem (manualmente ou via APP)";
                case QuandoIniciarMonitoramento.EstouIndoAoIniciarViagem: return "Ao receber a ação do \"Estou Indo\" do App ou inicio de Viagem";
                default: return string.Empty;
            }
        }
    }
}
