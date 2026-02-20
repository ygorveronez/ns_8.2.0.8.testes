namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoEmissaoMDFe
    {
        Normal = 1,
        ContingenciaMDFe = 5
    }

    public static class TipoEmissaoMDFeHelper
    {
        public static string ObterDescricao(this TipoEmissaoMDFe o)
        {
            switch (o)
            {
                case TipoEmissaoMDFe.Normal: return "1 - Normal";
                case TipoEmissaoMDFe.ContingenciaMDFe: return "5 - Contingência MDFE";
                default: return "Normal";
            }
        }
    }
}
