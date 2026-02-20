namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoRegraQualidadeMonitoramento
    {
        PreEmbarque = 1,
        DeslocamentoParaOrigem = 2,
        PreCheckin = 3,
        EmCarregamento = 4,
        SaidaOrigem = 5,
        EmViagem = 6,
        ChegadaDestino = 7,
        Descarga = 8,
        SaidaDestino = 9
    }

    public static class TipoRegraQualidadeMonitoramentoHelper
    {
        public static string ObterDescricao(this TipoRegraQualidadeMonitoramento cluster)
        {
            switch (cluster)
            {
                case TipoRegraQualidadeMonitoramento.PreEmbarque: return "Pré embarque";
                case TipoRegraQualidadeMonitoramento.DeslocamentoParaOrigem: return "Deslocamento para origem";
                case TipoRegraQualidadeMonitoramento.PreCheckin: return "Pré Check In";
                case TipoRegraQualidadeMonitoramento.EmCarregamento: return "Em carregamento";
                case TipoRegraQualidadeMonitoramento.SaidaOrigem: return "Saída origem";
                case TipoRegraQualidadeMonitoramento.EmViagem: return "Em viagem";
                case TipoRegraQualidadeMonitoramento.ChegadaDestino: return "Chegada destino";
                case TipoRegraQualidadeMonitoramento.Descarga: return "Descarga";
                case TipoRegraQualidadeMonitoramento.SaidaDestino: return "Saída destino";
                default: return string.Empty;
            }
        }
    }
}