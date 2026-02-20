namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum RegimeTributario
    {
        NaoInformado = 0,
        SimplesNacional = 1,
        LucroReal = 2,
        LucroPresumido = 3
    }

    public static class RegimeTributarioHelper
    {
        public static string ObterDescricao(this RegimeTributario regimeTributario)
        {
            switch (regimeTributario)
            {
                case RegimeTributario.SimplesNacional: return "Simples Nacional";
                case RegimeTributario.LucroReal: return "Lucro Real";
                case RegimeTributario.LucroPresumido: return "Lucro Presumido";
                default: return string.Empty;
            }
        }
    }
}
