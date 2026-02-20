namespace System
{
    public static class DecimalExtensions
    {
        public static string FromHoursToFormattedTime(this decimal valor)
        {
            return FromMinutesToFormattedTime(valor * 60);
        }

        public static string FromMinutesToFormattedTime(this decimal valor)
        {
            valor = valor.RoundDown(0);

            bool valorNegativo = valor < 0m;

            if (valorNegativo)
                valor *= -1;

            int minutos = (int)(valor % 60);
            int horas = (int)((valor - minutos) / 60);

            return $"{(valorNegativo ? "-" : "")}{string.Format("{0:00}", horas)}:{string.Format("{0:00}", minutos)}";
        }

        public static decimal RoundDown(this decimal valor, int precisao)
        {
            precisao = (int)Math.Pow(10, precisao);

            return Math.Floor(valor * precisao) / precisao;
        }

        public static decimal RoundUp(this decimal valor, int precisao)
        {
            precisao = (int)Math.Pow(10, precisao);

            return Math.Ceiling(valor * precisao) / precisao;
        }

        public static decimal PercentageDifferenceTo(this decimal from, decimal to)
        {
            if (from == 0m)
                return 0m;

            return to * 100 / from - 100;
        }
    }
}
