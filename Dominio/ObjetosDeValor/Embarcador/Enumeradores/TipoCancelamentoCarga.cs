namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoCancelamentoCarga
    {
        Cancelamento = 0,
        Anulacao = 1
    }

    public static class TipoCancelamentoCargaHelper
    {
        public static string ObterDescricao(this TipoCancelamentoCarga tipoCancelamento)
        {
            switch (tipoCancelamento)
            {
                case TipoCancelamentoCarga.Cancelamento: return "Cancelamento";
                case TipoCancelamentoCarga.Anulacao: return "Anulação";
                default: return string.Empty;
            }
        }
    }
}
