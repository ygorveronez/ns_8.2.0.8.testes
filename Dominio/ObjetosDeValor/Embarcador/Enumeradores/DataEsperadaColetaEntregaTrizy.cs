namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum DataEsperadaColetaEntregaTrizy
    {
        DataPrevisao = 0,
        DataAgendamento = 1,
        DataAgendamentoTransportador = 2,
    }

    public static class DataEsperadaColetaEntregaTrizyHelper
    {
        public static string ObterDescricao(this DataEsperadaColetaEntregaTrizy data)
        {
            switch (data)
            {
                case DataEsperadaColetaEntregaTrizy.DataPrevisao: return "Data Previsão";
                case DataEsperadaColetaEntregaTrizy.DataAgendamento: return "Data de Agendamento";
                case DataEsperadaColetaEntregaTrizy.DataAgendamentoTransportador: return "Data de Agendamento do Transportador";
                default: return "";
            }
        }
    }
}
