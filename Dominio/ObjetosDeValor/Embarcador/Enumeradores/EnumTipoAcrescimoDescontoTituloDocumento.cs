namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum EnumTipoAcrescimoDescontoTituloDocumento
    {
        Geracao = 0,
        Baixa = 1
    }

    public static class EnumTipoAcrescimoDescontoTituloDocumentoHelper
    {
        public static string ObterDescricao(this EnumTipoAcrescimoDescontoTituloDocumento tipo)
        {
            switch (tipo)
            {
                case EnumTipoAcrescimoDescontoTituloDocumento.Geracao: return "Geração";
                case EnumTipoAcrescimoDescontoTituloDocumento.Baixa: return "Baixa";
                default: return string.Empty;
            }
        }
    }
}
