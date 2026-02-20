using System;

namespace Utilidades
{
    public class Chave
    {
        public static string ObterCNPJEmitente(string chave)
        {
            if (string.IsNullOrWhiteSpace(chave) || chave.Length < 44)
                return string.Empty;

            return chave.Substring(6, 14);
        }

        public static int ObterNumero(string chave)
        {
            if (string.IsNullOrWhiteSpace(chave) || chave.Length < 44)
                return 0;

            return chave.Substring(25, 9).ToInt();
        }

        public static string ObterAno(string chave)
        {
            if (string.IsNullOrWhiteSpace(chave) || chave.Length < 44)
                return string.Empty;

            return chave.Substring(2, 2);
        }

        public static string ObterMes(string chave)
        {
            if (string.IsNullOrWhiteSpace(chave) || chave.Length < 44)
                return string.Empty;

            return chave.Substring(4, 2);
        }

        public static string ObterSerie(string chave)
        {
            if (string.IsNullOrWhiteSpace(chave) || chave.Length < 44)
                return string.Empty;

            return chave.Substring(22, 3);
        }

        public static int ObterTipoEmissao(string chave)
        {
            if (string.IsNullOrWhiteSpace(chave) || chave.Length < 44)
                return 0;

            return chave.Substring(34, 1).ToInt();
        }

        public static int ObterDigitoVerificador(string chave)
        {
            if (string.IsNullOrWhiteSpace(chave) || chave.Length < 44)
                return 0;

            return chave.Substring(43, 1).ToInt();
        }
    }
}
