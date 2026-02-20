namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoCobranca
    {
        Banco = 1,
        Carteira = 2,
        Cartao = 3,
        Cheque = 4
    }

    public static class TipoCobrancaHelper
    {
        public static string ObterDescricao(this TipoCobranca tipo)
        {
            switch (tipo)
            {
                case TipoCobranca.Banco: return "Banco";
                case TipoCobranca.Carteira: return "Carteira";
                case TipoCobranca.Cartao: return "Cartão";
                case TipoCobranca.Cheque: return "Cheque";
                default: return string.Empty;
            }
        }
    }
}
