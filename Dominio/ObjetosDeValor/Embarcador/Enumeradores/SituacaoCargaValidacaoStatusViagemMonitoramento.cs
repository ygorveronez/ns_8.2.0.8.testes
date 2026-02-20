namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoCargaValidacaoStatusViagemMonitoramento
    {
        NaLogistica = 0,
        Nova = 1,
        CalculoFrete = 2,
        AgTransportador = 4,
        AgNFe = 5,
        PendeciaDocumentos = 6,
        AgImpressaoDocumentos = 7,
        ProntoTransporte = 8,
        EmTransporte = 9,
        LiberadoPagamento = 10,
        Encerrada = 11,
        Cancelada = 13,
        AgIntegracao = 15,
        EmTransbordo = 17,
        Anulada = 18,
        Todas = 99,
        PermiteCTeManual = 101
    }
}