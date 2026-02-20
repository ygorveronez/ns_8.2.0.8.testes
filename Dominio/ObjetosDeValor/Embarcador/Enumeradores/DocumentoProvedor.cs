namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum DocumentoProvedor
    {
        NF = 0,
        CTe = 1,
        CTeComplementar = 2,
    }

    public static class EnumDocumentoProvedoreHelper
    {
        public static string ObterDescricao(this DocumentoProvedor situacao)
        {
            switch (situacao)
            {
                case DocumentoProvedor.NF: return "NF";
                case DocumentoProvedor.CTe: return "CT-e";
                case DocumentoProvedor.CTeComplementar: return "CT-e Complementar";
                default: return string.Empty;
            }
        }
    }
}

