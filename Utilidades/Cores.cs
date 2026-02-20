using System;

namespace Utilidades
{
    public static class Cores
    {
        public static string ObterCorPorPencentual(string corHexadecimal, decimal percentual)
        {
            System.Drawing.Color cor = System.Drawing.ColorTranslator.FromHtml(corHexadecimal);
            int corVermelha = (int)((decimal)cor.R * percentual / 100);
            int corVerde = (int)((decimal)cor.G * percentual / 100);
            int corAzul = (int)((decimal)cor.B * percentual / 100);
            System.Drawing.Color corEscura = System.Drawing.Color.FromArgb(Math.Min(corVermelha, 255), Math.Min(corVerde, 255), Math.Min(corAzul, 255));
            string corEscuraHexadecimal = System.Drawing.ColorTranslator.ToHtml(corEscura);

            return corEscuraHexadecimal;
        }
    }
}
