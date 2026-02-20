namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum EnumVersaoLayoutATM
    {
        Versao200 = 1,
        Versao400 = 2
    }

    public static class EnumVersaoLayoutATMHelper
    {
        public static string Descricao(this EnumVersaoLayoutATM versaoLayoutATM)
        {
            switch (versaoLayoutATM)
            {
                case EnumVersaoLayoutATM.Versao200:
                    return "Versão 2.00";
                case EnumVersaoLayoutATM.Versao400:
                    return "Versão 4.00";
                default:
                    return string.Empty;
            }
        }
    }
}
