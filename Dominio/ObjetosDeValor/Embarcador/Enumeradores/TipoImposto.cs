namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoImposto
    {
        Nenhum = 0,
        CBS = 1,
        IBS = 2
    }

    public static class TipoImpostoHelper
    {
        public static string ObterDescricao(this TipoImposto tipoMovimento)
        {
            switch (tipoMovimento)
            {
                case TipoImposto.CBS: return "CBS";
                case TipoImposto.IBS: return "IBS";
                default: return string.Empty;
            }
        }
    }
}
