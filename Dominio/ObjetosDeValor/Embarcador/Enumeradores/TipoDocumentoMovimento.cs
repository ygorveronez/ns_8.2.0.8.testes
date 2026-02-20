namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoDocumentoMovimento
    {
        Manual = 1,
        NotaEntrada = 2,
        CTe = 3,
        Faturamento = 4,
        Recibo = 5,
        Pagamento = 6,
        Recebimento = 7,
        NotaSaida = 8,
        Outros = 9,
        Acerto = 10,
        ContratoFrete = 11,
        AdiantamentoMotorista = 12,
        Carga = 13,
        Cheque = 14,
        PendenciaMotorista = 15,
    }

    public static class TipoDocumentoMovimentoHelper
    {
        public static string ObterDescricao(this TipoDocumentoMovimento tipoDocumento)
        {
            switch (tipoDocumento)
            {
                case TipoDocumentoMovimento.Manual: return "Manual";
                case TipoDocumentoMovimento.NotaEntrada: return "Nota de Entrada";
                case TipoDocumentoMovimento.CTe: return "Documento Emitido";
                case TipoDocumentoMovimento.Faturamento: return "Faturamento";
                case TipoDocumentoMovimento.Recibo: return "Recibo";
                case TipoDocumentoMovimento.Pagamento: return "Pagamento";
                case TipoDocumentoMovimento.Recebimento: return "Recebimento";
                case TipoDocumentoMovimento.NotaSaida: return "Nota de Saída";
                case TipoDocumentoMovimento.Outros: return "Outros";
                case TipoDocumentoMovimento.Acerto: return "Acerto de Viagem";
                case TipoDocumentoMovimento.ContratoFrete: return "Contrato de Frete";
                case TipoDocumentoMovimento.AdiantamentoMotorista: return "Adiantamento Motorista";
                case TipoDocumentoMovimento.Carga: return "Carga";
                case TipoDocumentoMovimento.Cheque: return "Cheque";
                case TipoDocumentoMovimento.PendenciaMotorista: return "Pendência Motorista";
                default: return string.Empty;
            }
        }
    }
}
