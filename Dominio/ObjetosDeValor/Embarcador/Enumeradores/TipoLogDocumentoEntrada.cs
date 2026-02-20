namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoLogDocumentoEntrada
    {
        Abertura = 1,
        Finalizacao = 2,
        Cancelamento = 3,
        Anulacao = 4
    }

    public static class TipoLogDocumentoEntradaHelper
    {
        public static string ObterDescricao(this TipoLogDocumentoEntrada tipoLog)
        {
            switch (tipoLog)
            {
                case TipoLogDocumentoEntrada.Abertura: return "Abertura";
                case TipoLogDocumentoEntrada.Finalizacao: return "Finalização";
                case TipoLogDocumentoEntrada.Cancelamento: return "Cancelamento";
                case TipoLogDocumentoEntrada.Anulacao: return "Anulação";
                default: return string.Empty;
            }
        }
    }
}
