namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoRegraAutorizacaoToleranciaPesagem
    {
        Peso = 1,
        Percentual = 2,
    }

    public static class TipoRegraAutorizacaoToleranciaPesagemHelper
    {
        public static string ObterDescricao(this TipoRegraAutorizacaoToleranciaPesagem tipo)
        {
            switch (tipo)
            {
                case TipoRegraAutorizacaoToleranciaPesagem.Peso: return "Peso";
                case TipoRegraAutorizacaoToleranciaPesagem.Percentual: return "Percentual";
                default: return string.Empty;
            }
        }
    }
}
