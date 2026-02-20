namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoDataAlteracaoGatilho
    {
        DataAgendamentoEntregaTransportador = 1,
    }

    public static class DataGatilhoHelper
    {
        public static string ObterDescricao(this TipoDataAlteracaoGatilho tipo)
        {
            switch (tipo)
            {
                case TipoDataAlteracaoGatilho.DataAgendamentoEntregaTransportador: return "Data de Agendamento de Entrega do Transportador";
                default: return string.Empty;
            }
        }
    }
}
