namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum ResponsavelPallet
    {
        Cliente = 1,
        Transportador = 2,
        Filial = 3
    }

    public static class ResponsavelPalletHelper
    {
        public static string ObterDescricao(this ResponsavelPallet responsavelPallet)
        {
            switch (responsavelPallet)
            {
                case ResponsavelPallet.Cliente: return "Cliente";
                case ResponsavelPallet.Transportador: return "Transportador";
                case ResponsavelPallet.Filial: return "Filial";
                default: return string.Empty;
            }
        }
    }
}
