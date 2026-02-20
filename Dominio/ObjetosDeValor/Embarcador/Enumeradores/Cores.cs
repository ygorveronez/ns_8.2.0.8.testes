namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum Cores
    {
        Verde = 1,
        VerdeEscuro = 2,
        Vermelho = 3,
        Cinza = 4,
        Branco = 5,
        Azul = 6,
        Amarelo = 7,
        Laranja = 8
    }

    public static class CoresHelper
    {
        public static string Descricao(this Cores cores)
        {
            switch (cores)
            {
                case Cores.Verde:
                    return "#DFF0D8";
                case Cores.VerdeEscuro:
                    return "#006400";
                case Cores.Vermelho:
                    return "#C16565";
                case Cores.Cinza:
                    return "#777777";
                case Cores.Branco:
                    return "#FFFFFF";
                case Cores.Azul:
                    return "#ADD8E6";
                case Cores.Amarelo:
                    return "#F7F7BA";
                case Cores.Laranja:
                    return "#B8860B";
                default:
                    return string.Empty;
            }
        }
    }
}
