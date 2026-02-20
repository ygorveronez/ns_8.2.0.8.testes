namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoOperacaoMovimentacaoEstoquePallet
    {
        AvariaReforma = 11,
        AvariaReformaPorTransportador = 18,
        ClienteEntrada = 7,
        ClienteFilial = 9,
        ClienteSaida = 8,
        ClienteTransportador = 15,
        FilialAvaria = 10,
        FilialCliente = 2,
        FilialEntrada = 0,
        FilialSaida = 1,
        FilialTransportador = 3,
        ReformaAvaria = 12,
        ReformaAvariaPorTransportador = 19,
        ReformaFilial = 13,
        ReformaTransportador = 17,
        TransportadorAvaria = 16,
        TransportadorCliente = 14,
        TransportadorEntrada = 4,
        TransportadorFilial = 6,
        TransportadorSaida = 5
    }

    public static class TipoOperacaoMovimentacaoEstoquePalletHelper
    {
        public static string ObterObservacao(this TipoOperacaoMovimentacaoEstoquePallet tipoOperacao)
        {
            switch (tipoOperacao)
            {
                case TipoOperacaoMovimentacaoEstoquePallet.AvariaReforma: return "Movimentação de pallets avariados para reforma";
                case TipoOperacaoMovimentacaoEstoquePallet.AvariaReformaPorTransportador: return "Movimentação de pallets avariados para reforma";
                case TipoOperacaoMovimentacaoEstoquePallet.ClienteEntrada: return "Movimentação de entrada de pallets para o cliente";
                case TipoOperacaoMovimentacaoEstoquePallet.ClienteFilial: return "Movimentação de pallets do cliente para a filial";
                case TipoOperacaoMovimentacaoEstoquePallet.ClienteSaida: return "Movimentação de saída de pallets para o cliente";
                case TipoOperacaoMovimentacaoEstoquePallet.ClienteTransportador: return "Movimentação de pallets do cliente para o transportador";
                case TipoOperacaoMovimentacaoEstoquePallet.FilialAvaria: return "Movimentação de pallets da filial para avaria";
                case TipoOperacaoMovimentacaoEstoquePallet.FilialCliente: return "Movimentação de pallets da filial para o cliente";
                case TipoOperacaoMovimentacaoEstoquePallet.FilialEntrada: return "Movimentação de entrada de pallets para a filial";
                case TipoOperacaoMovimentacaoEstoquePallet.FilialSaida: return "Movimentação de saída de pallets para a filial";
                case TipoOperacaoMovimentacaoEstoquePallet.FilialTransportador: return "Movimentação de pallets da filial para o transportador";
                case TipoOperacaoMovimentacaoEstoquePallet.ReformaAvaria: return "Movimentação de pallets da reforma para avaria";
                case TipoOperacaoMovimentacaoEstoquePallet.ReformaAvariaPorTransportador: return "Movimentação de pallets da reforma para avaria";
                case TipoOperacaoMovimentacaoEstoquePallet.ReformaFilial: return "Movimentação de pallets da reforma para a filial";
                case TipoOperacaoMovimentacaoEstoquePallet.ReformaTransportador: return "Movimentação de pallets da reforma para a transportador";
                case TipoOperacaoMovimentacaoEstoquePallet.TransportadorAvaria: return "Movimentação de pallets do transportador para avaria";
                case TipoOperacaoMovimentacaoEstoquePallet.TransportadorCliente: return "Movimentação de pallets do transportador para o cliente";
                case TipoOperacaoMovimentacaoEstoquePallet.TransportadorEntrada: return "Movimentação de entrada de pallets para o transportador";
                case TipoOperacaoMovimentacaoEstoquePallet.TransportadorFilial: return "Movimentação de pallets do transportador para a filial";
                case TipoOperacaoMovimentacaoEstoquePallet.TransportadorSaida: return "Movimentação de saída de pallets para o transportador";
                default: return string.Empty;
            }
        }
    }
}
