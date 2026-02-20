namespace System
{
    public static class DoubleExtensions
    {
        public static double RoundDown(this double valor, int precisao)
        {
            precisao = (int)Math.Pow(10, precisao);

            return Math.Floor(valor * precisao) / precisao;
        }

        public static double RoundUp(this double valor, int precisao)
        {
            precisao = (int)Math.Pow(10, precisao);

            return Math.Ceiling(valor * precisao) / precisao;
        }
    }
}
