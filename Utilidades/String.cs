using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Utilidades
{
    public class String
    {
        #region Métodos Públicos

        public static string SanitizeString(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            input = Regex.Replace(input.Trim(), @"\s+", " ");
            input = RemoveAccents(input);
            input = Regex.Replace(input, @"[^a-zA-Z0-9_ ]+", string.Empty);

            return input;
        }

        public static string RemoveAccents(string text)
        {
            StringBuilder sbReturn = new StringBuilder();
            var arrayText = text.Normalize(NormalizationForm.FormD).ToCharArray();
            foreach (char letter in arrayText)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(letter) != UnicodeCategory.NonSpacingMark)
                    sbReturn.Append(letter);
            }
            return sbReturn.ToString();
        }

        public static string RemoveAllSpecialCharacters(string param)
        {
            if (string.IsNullOrWhiteSpace(param))
                param = string.Empty;
            return Regex.Replace(Regex.Replace(param, "[\\\\/]", "-"), @"[^0-9a-zA-Z\._ ]", string.Empty);
        }

        public static string RemoveAllSpecialCharactersNotCommon(string param)
        {
            if (string.IsNullOrWhiteSpace(param))
                param = string.Empty;

            //Mantém apenas números, letras, letras com acentuação, e os caracteres especiais mais comuns, o restante é removido.
            return Regex.Replace(param, @"(?i)[^0-9a-záéíóúàèìòùâêîôûãõç\s ,.;<>:?@!#$%&*()/\\_+-=]", "");//Caso tiver mais algum, basta incluir
        }

        public static string RemoveDiacritics(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark && unicodeCategory != System.Globalization.UnicodeCategory.Format)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }

        public static string RemoverCaracteresEspecialSerpro(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return string.Empty;

            str = str.Replace("&Aacute;", "Á");
            str = str.Replace("&aacute;", "á");
            str = str.Replace("&Acirc;", "Â");
            str = str.Replace("&acirc;", "â");
            str = str.Replace("&Agrave;", "À");
            str = str.Replace("&agrave;", "à");
            str = str.Replace("&Aring;", "A");
            str = str.Replace("&aring;", "a");
            str = str.Replace("&Atilde;", "Ã");
            str = str.Replace("&atilde;", "ã");
            str = str.Replace("&Auml;", "Ä");
            str = str.Replace("&auml;", "ä");
            str = str.Replace("&AElig;", "AE");
            str = str.Replace("&aelig;", "ae");
            str = str.Replace("&Eacute;", "É");
            str = str.Replace("&eacute;", "é");
            str = str.Replace("&Ecirc;", "Ê");
            str = str.Replace("&ecirc;", "ê");
            str = str.Replace("&Egrave;", "È");
            str = str.Replace("&egrave;", "è");
            str = str.Replace("&Euml;", "Ë");
            str = str.Replace("&euml;", "ë");
            str = str.Replace("&ETH;", "E");
            str = str.Replace("&eth;", "o");
            str = str.Replace("&Iacute;", "Í");
            str = str.Replace("&iacute;", "í");
            str = str.Replace("&Icirc;", "Î");
            str = str.Replace("&icirc;", "î");
            str = str.Replace("&Igrave;", "Ì");
            str = str.Replace("&igrave;", "ì");
            str = str.Replace("&Iuml;", "Ï");
            str = str.Replace("&iuml;", "ï");
            str = str.Replace("&Oacute;", "Ó");
            str = str.Replace("&oacute;", "ó");
            str = str.Replace("&Ocirc;", "Ô");
            str = str.Replace("&ocirc;", "ô");
            str = str.Replace("&Ograve;", "Ò");
            str = str.Replace("&ograve;", "ò");
            str = str.Replace("&Oslash;", "O");
            str = str.Replace("&oslash;", "o");
            str = str.Replace("&Otilde;", "Õ");
            str = str.Replace("&otilde;", "õ");
            str = str.Replace("&Ouml;", "Ö");
            str = str.Replace("&ouml;", "ö");
            str = str.Replace("&Uacute;", "Ú");
            str = str.Replace("&uacute;", "ú");
            str = str.Replace("&Ucirc;", "Û");
            str = str.Replace("&ucirc;", "û");
            str = str.Replace("&Ugrave;", "Ù");
            str = str.Replace("&ugrave;", "ù");
            str = str.Replace("&Uuml;", "Ü");
            str = str.Replace("&uuml;", "ü");
            str = str.Replace("&Ccedil;", "Ç");
            str = str.Replace("&ccedil;", "ç");
            str = str.Replace("&Ntilde;", "Ñ");
            str = str.Replace("&ntilde;", "ñ");
            str = str.Replace("&Yacute;", "Ý");
            str = str.Replace("&yacute;", "ý");
            str = str.Replace("&quot;", "");
            str = str.Replace("&lt;", "<");
            str = str.Replace("&gt;", ">");
            str = str.Replace("&amp;", "&");
            str = str.Replace("&reg;", "");
            str = str.Replace("&copy;", "");
            str = str.Replace("&THORN;", "p");
            str = str.Replace("&thorn;", "b");
            str = str.Replace("&szlig;", "b");

            return str;
        }

        public static string RemoveSpecialCharacters(string str)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in str)
            {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '.' || c == '_' || c == ' ')
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        public static string RemoveSpecialCharactersLatitudeLongitude(string str)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in str)
            {
                if ((c >= '0' && c <= '9') || c == '.' || c == '-')
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        public static string Base64Encode(string text)
        {
            byte[] textoAsBytes = System.Text.Encoding.ASCII.GetBytes(text);
            string resultado = System.Convert.ToBase64String(textoAsBytes);
            return resultado;
        }

        public static string Base64Decode(string text)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(text);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        public static string ReplaceInvalidCharacters(string param)
        {
            if (!string.IsNullOrEmpty(param))
                return param.Replace("\"", "'").Replace("\f", "").Replace("\n", " ");
            else
                return param;
        }

        public static string RemoveSpecifiedStringAndReplace(string toRemove, string textToSeek, string toReplace)
        {
            if (string.IsNullOrWhiteSpace(toRemove) || string.IsNullOrWhiteSpace(textToSeek))
                textToSeek = string.Empty;
            
            return textToSeek.Replace(toRemove, toReplace);
        }

        #endregion Métodos Públicos

        #region Métodos Públicos Migrados

        public static string Left(string param, int length)
        {
            if (param == null) return null;
            return param.Left(length);
        }

        public static string Right(string param, int length)
        {
            if (param == null) return null;
            return param.Right(length);
        }

        public static string OnlyNumbers(string param)
        {
            return param.ObterSomenteNumeros();
        }

        public static string OnlyNumbersAndChars(string param)
        {
            return param.ObterSomenteNumerosELetras();
        }

        public static System.IO.MemoryStream ToStream(string param, System.Text.Encoding encoding = null)
        {
            return param.ToStream(encoding);
        }

        public static string GetHttpStatusDescription(int statusCode)
        {
            switch (statusCode)
            {
                case 200: return "OK";
                case 404: return "Not Found";

                default: return statusCode.ToString();
            }
        }

        #endregion Métodos Públicos Migrados
    }
}
