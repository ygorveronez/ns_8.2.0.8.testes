namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum VidaPneu
    {
        PneuNovo = 1,
        PrimeiroRecape = 2,
        SegundoRecape = 3,
        TerceiroRecape = 4,
        QuartoRecape = 5,
        QuintoRecape = 6,
        SextoRecape = 7
    }

    public static class VidaPneuHelper
    {
        public static string ObterDescricao(this VidaPneu vida)
        {
            switch (vida)
            {
                case VidaPneu.PneuNovo: return "Pneu Novo (1ª Vida)";
                case VidaPneu.PrimeiroRecape: return "1º Recape (2ª Vida)";
                case VidaPneu.SegundoRecape: return "2º Recape (3ª Vida)";
                case VidaPneu.TerceiroRecape: return "3º Recape (4ª Vida)";
                case VidaPneu.QuartoRecape: return "4º Recape (5ª Vida)";
                case VidaPneu.QuintoRecape: return "5º Recape (6ª Vida)";
                case VidaPneu.SextoRecape: return "6º Recape (7ª Vida)";
                default: return string.Empty;
            }
        }
    }
}
