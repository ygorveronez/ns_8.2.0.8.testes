using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace System
{
    public static class StringExtension
    {
        #region Métodos Privados

        private static T ToFloatingPointNumber<T>(string valor, T valorPadrao)
        {
            if (string.IsNullOrWhiteSpace(valor))
                return valorPadrao;

            int indicePrimeiroPonto = valor.IndexOf('.');
            int indicePrimeiraVirgula = valor.IndexOf(',');

            if (indicePrimeiroPonto == 0 || indicePrimeiraVirgula == 0)
                valor = $"0{valor}";

            bool formatoNumericoValido = Text.RegularExpressions.Regex.IsMatch(valor, @"^-?(((\d{1,3}[.])?(\d{3}[.])*(\d{3}[,]))|((\d{1,3}[,])?(\d{3}[,])*(\d{3}[.]))|((\d+[.,])?))\d+$");

            if (!formatoNumericoValido)
                return valorPadrao;

            int indiceUltipoPonto = valor.LastIndexOf('.');
            int indiceUltimaVirgula = valor.LastIndexOf(',');
            int indiceSeparadorDecimal = Math.Max(indiceUltipoPonto, indiceUltimaVirgula);

            if (indiceSeparadorDecimal > 0)
            {
                string parteInteira = valor.Substring(0, indiceSeparadorDecimal).Replace(",", "").Replace(".", "");
                string parteDecimal = valor.Substring(indiceSeparadorDecimal + 1);

                return ToType($"{parteInteira}.{parteDecimal}", valorPadrao);
            }

            return ToType(valor, valorPadrao);
        }

        private static T ToType<T>(string valor, T valorPadrao)
        {
            try
            {
                Type underlyingT = Nullable.GetUnderlyingType(typeof(T));

                if (underlyingT == null)
                    return (T)Convert.ChangeType(valor, typeof(T), new Globalization.CultureInfo("en-US"));

                return (T)Convert.ChangeType(valor, underlyingT, new Globalization.CultureInfo("en-US"));
            }
            catch (Exception)
            {
                return valorPadrao;
            }
        }

        #endregion

        #region Métodos Públicos

        public static string AllFirstLetterToUpper(this string valor)
        {
            if (string.IsNullOrWhiteSpace(valor))
                return valor;

            string[] listaPalavras = valor.Split(' ');

            for (int i = 0; i < listaPalavras.Length; i++)
                listaPalavras[i] = listaPalavras[i].FirstLetterToUpper();

            return string.Join(" ", listaPalavras);
        }

        public static string FirstLetterToLower(this string valor)
        {
            if (string.IsNullOrWhiteSpace(valor))
                return valor;

            char[] listaCaracteresValor = valor.ToCharArray();

            listaCaracteresValor[0] = char.ToLower(listaCaracteresValor[0]);

            return new string(listaCaracteresValor);
        }

        public static string FirstLetterToUpper(this string valor)
        {
            if (string.IsNullOrWhiteSpace(valor))
                return valor;

            char[] listaCaracteresValor = valor.ToCharArray();

            listaCaracteresValor[0] = char.ToUpper(listaCaracteresValor[0]);

            return new string(listaCaracteresValor);
        }

        public static bool IsSomenteNumeros(this string valor)
        {
            if (string.IsNullOrWhiteSpace(valor))
                return false;

            return Text.RegularExpressions.Regex.IsMatch(valor, @"^\d+$");
        }

        public static string Left(this string valor, int tamanho)
        {
            if (string.IsNullOrEmpty(valor))
                return valor;

            if (tamanho >= valor.Length)
                return valor;

            return valor.Substring(0, tamanho);
        }

        public static string ObterCnpjFormatado(this string cnpj)
        {
            if (string.IsNullOrWhiteSpace(cnpj))
                return string.Empty;

            return string.Format(@"{0:00\.000\.000\/0000\-00}", cnpj.ToLong());
        }



        public static string ObterDDD(this string fone)
        {
            if (string.IsNullOrEmpty(fone))
                return string.Empty;

            if (fone.Substring(0, 1) == "(")
                return fone.Substring(1, 2);
            else
                return string.Empty;
        }

        public static string ObterFoneSemDDD(this string fone)
        {
            if (string.IsNullOrEmpty(fone))
                return string.Empty;

            if (fone.Substring(0, 1) == "(")
            {
                return fone.Substring(4, fone.Length - 4);
            }
            else
                return fone;
        }





        public static string ObterCpfFormatado(this string cpf)
        {
            if (string.IsNullOrWhiteSpace(cpf))
                return string.Empty;

            return string.Format(@"{0:000\.000\.000\-00}", cpf.ToLong());
        }

        public static string ObterCpfOuCnpjFormatado(this string cpfCnpj)
        {
            if (cpfCnpj?.Length > 11)
                return cpfCnpj.ObterCnpjFormatado();

            return cpfCnpj.ObterCpfFormatado();
        }

        /// <param name="tipoPessoa">F (Física) - J (Jurídica) - E (Exterior)</param>
        public static string ObterCpfOuCnpjFormatado(this string cpfCnpj, string tipoPessoa)
        {
            switch (tipoPessoa)
            {
                case "E": return "00.000.000/0000-00";
                case "F": return cpfCnpj.ObterCpfFormatado();
                case "J": return cpfCnpj.ObterCnpjFormatado();
                default: return string.Empty;
            }
        }

        public static string ObterPlacaFormatada(this string placa)
        {
            if (string.IsNullOrWhiteSpace(placa))
                return string.Empty;

            placa = placa.ObterSomenteNumerosELetras();
            bool placaPadraoAntigo = (placa.Length == 7) && new Text.RegularExpressions.Regex(@"^\w{3}\d{4}").IsMatch(placa);

            if (placaPadraoAntigo)
                return placa.Insert(3, "-");

            return placa;
        }

        public static string ObterSomenteCaracteresPermitidos(this string valor, string caracteresPermitidos)
        {
            if (string.IsNullOrWhiteSpace(valor))
                return valor;

            return Text.RegularExpressions.Regex.Replace(valor, $"[^({caracteresPermitidos})]", "");
        }

        public static string ObterSomenteNumeros(this string valor)
        {
            if (string.IsNullOrWhiteSpace(valor))
                return valor;

            return Text.RegularExpressions.Regex.Replace(valor, "[^0-9]", "");
        }

        public static string ObterSomenteNumerosELetras(this string valor)
        {
            if (string.IsNullOrWhiteSpace(valor))
                return valor;

            return Text.RegularExpressions.Regex.Replace(valor, "[^0-9a-zA-Z]", "");
        }

        public static string ObterSomenteNumerosELetrasComEspaco(this string valor)
        {
            if (string.IsNullOrWhiteSpace(valor))
                return valor;

            return Text.RegularExpressions.Regex.Replace(valor, "[^0-9a-zA-Z ]", "");
        }

        public static string ObterTelefoneFormatado(this string telefone)
        {
            telefone = telefone.ObterSomenteNumeros();

            if (string.IsNullOrWhiteSpace(telefone))
                return string.Empty;

            if (telefone.Length == 12)
                return string.Format(@"{0:(00) 000000-0000}", telefone.ToLong());

            if (telefone.Length == 11)
                return string.Format(@"{0:(00) 00000-0000}", telefone.ToLong());

            if (telefone.Length == 10)
                return string.Format(@"{0:(00) 0000-0000}", telefone.ToLong());

            return telefone;
        }

        public static string ObterTelefoneFormatadoCom55(this string telefone)
        {
            if (string.IsNullOrWhiteSpace(telefone))
                return string.Empty;

            // Remove tudo que não for número
            var numeros = new string(telefone.Where(char.IsDigit).ToArray());

            // Se depois de limpar ficou vazio, retorna ""
            if (string.IsNullOrWhiteSpace(numeros))
                return string.Empty;

            // Remove zeros à esquerda
            numeros = numeros.TrimStart('0');

            // Se já começar com 55 e tiver tamanho 13 (55 + DDD 2 + 9 dígitos) está correto
            if (numeros.StartsWith("55") && numeros.Length == 13)
                return "+" + numeros;

            // Se começar com DDD e número (11 dígitos) -> adiciona 55
            if (numeros.Length == 11)
                return $"+55{numeros}";

            // Se começar com DDD + fixo (10 dígitos) -> adiciona 55
            if (numeros.Length == 10)
                return $"+55{numeros}";

            // Se for fixo sem DDD (8 dígitos) não dá para formatar corretamente
            if (numeros.Length == 8)
                return string.Empty;

            // Qualquer outra coisa inválida
            return string.Empty;
        }

        public static string Right(this string valor, int tamanho)
        {
            if (string.IsNullOrEmpty(valor))
                return valor;

            if (tamanho >= valor.Length)
                return valor;

            return valor.Substring(valor.Length - tamanho, tamanho);
        }

        public static string[] SplitByLength(this string valor, int tamanho)
        {
            if (string.IsNullOrWhiteSpace(valor))
                return new string[] { };

            Collections.Generic.List<string> valores = new Collections.Generic.List<string>();

            for (int i = 0; i < valor.Length; i += tamanho)
            {
                int tamanhoLimite = (i + tamanho > valor.Length) ? valor.Length - i : tamanho;
                valores.Add(valor.Substring(i, tamanhoLimite));
            }

            return valores.ToArray();
        }

        public static string ConvertToOSPlatformPath(this string value)
        {
            if (string.IsNullOrWhiteSpace(value) || value.Length <= 2)
                return value;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                string path = Regex.Replace(value, @"^[A-Za-z]:\\?", "/");
                path = path.Replace("\\", "/");

                return path;
            }

            return value;
        }

        #endregion Métodos Públicos

        #region Métodos Públicos - Conversão

        public static bool ToBool(this string valor, bool valorPadrao = default(bool))
        {
            return valor.ToNullableBool() ?? valorPadrao;
        }

        public static DateTime ToDateTime(this string valor, DateTime valorPadrao = default(DateTime))
        {
            return valor.ToNullableDateTime() ?? valorPadrao;
        }

        public static DateTime ToDateTime(this string valor, string formato, DateTime valorPadrao = default(DateTime))
        {
            return valor.ToNullableDateTime(formato) ?? valorPadrao;
        }

        public static decimal ToDecimal(this string valor, decimal valorPadrao = default(decimal))
        {
            return ToFloatingPointNumber(valor, valorPadrao);
        }

        public static double ToDouble(this string valor, double valorPadrao = default(double))
        {
            return ToFloatingPointNumber(valor, valorPadrao);
        }

        public static TEnum ToEnum<TEnum>(this string valor, TEnum valorPadrao = default(TEnum)) where TEnum : struct
        {
            return valor.ToNullableEnum<TEnum>() ?? valorPadrao;
        }

        public static float ToFloat(this string valor, float valorPadrao = default(float))
        {
            return ToFloatingPointNumber(valor, valorPadrao);
        }

        public static int ToInt(this string valor, int valorPadrao = default(int))
        {
            return valor.ToNullableInt() ?? valorPadrao;
        }

        public static long ToLong(this string valor, long valorPadrao = default(long))
        {
            return valor.ToNullableLong() ?? valorPadrao;
        }

        /// <summary>
        /// Converte o valor no formato HH:MM em minutos
        /// </summary>
        public static int ToMinutes(this string valor, int valorPadrao = default(int))
        {
            return valor.ToNullableMinutes() ?? valorPadrao;
        }

        public static TimeSpan ToTime(this string valor, TimeSpan valorPadrao = default(TimeSpan))
        {
            return valor.ToNullableTime() ?? valorPadrao;
        }

        public static bool? ToNullableBool(this string valor)
        {
            if (string.IsNullOrWhiteSpace(valor))
                return null;

            if (char.ToLower(valor[0]) == 's')
                return true;

            if (char.ToLower(valor[0]) == 'n')
                return false;

            int? valorInteiro = ToType<int?>(valor, null);

            if (valorInteiro.HasValue)
            {
                if ((valorInteiro.Value != 0) && (valorInteiro.Value != 1))
                    return null;

                return valorInteiro.Value == 1;
            }

            return ToType<bool?>(valor, null);
        }

        public static DateTime? ToNullableDateTime(this string valor, string formato = "dd/MM/yyyy HH:mm:ss")
        {
            if (string.IsNullOrWhiteSpace(valor))
                return null;

            if (valor.Length == 10)
                valor += " 00:00:00";
            else if (valor.Length == 14)
                valor += " 00";
            else if (valor.Length == 16)
                valor += ":00";

            try
            {
                return DateTime.ParseExact(valor, formato, null, Globalization.DateTimeStyles.None);
            }
            catch
            {
                return null;
            }
        }

        public static decimal? ToNullableDecimal(this string valor)
        {
            return ToFloatingPointNumber(valor, default(decimal?));
        }

        public static double? ToNullableDouble(this string valor)
        {
            return ToFloatingPointNumber(valor, default(double?));
        }

        public static Nullable<TEnum> ToNullableEnum<TEnum>(this string valor) where TEnum : struct
        {
            try
            {
                TEnum retorno = (TEnum)Enum.Parse(typeof(TEnum), valor);

                if (Enum.IsDefined(typeof(TEnum), retorno))
                    return retorno;

                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static float? ToNullableFloat(this string valor)
        {
            return ToFloatingPointNumber(valor, default(float?));
        }

        public static int? ToNullableInt(this string valor)
        {
            if (string.IsNullOrWhiteSpace(valor))
                return null;

            return ToType(valor.Replace(".", "").Replace(",", ""), default(int?));
        }

        public static long? ToNullableLong(this string valor)
        {
            if (string.IsNullOrWhiteSpace(valor))
                return null;

            return ToType(valor, default(long?));
        }

        /// <summary>
        /// Converte o valor no formato HH:MM em minutos
        /// </summary>
        public static int? ToNullableMinutes(this string valor)
        {
            if (string.IsNullOrWhiteSpace(valor))
                return null;

            string[] valorSeparado = valor.Split(':');

            if (valorSeparado.Length != 2)
                return null;

            int? horas = valorSeparado[0].Trim().ToNullableInt();
            int? minutos = valorSeparado[1].Trim().ToNullableInt();

            if (!horas.HasValue || !minutos.HasValue)
                return null;

            return (horas.Value * 60) + minutos.Value;
        }

        public static TimeSpan? ToNullableTime(this string valor)
        {
            if (string.IsNullOrWhiteSpace(valor))
                return null;

            try
            {
                string[] horaParticionada = valor.Split(':');

                if (horaParticionada.Length == 2)
                    return new TimeSpan(hours: horaParticionada[0].ToInt(), minutes: horaParticionada[1].ToInt(), seconds: 0);

                return new TimeSpan(hours: horaParticionada[0].ToInt(), minutes: horaParticionada[1].ToInt(), seconds: horaParticionada[2].ToInt());
            }
            catch
            {
                return null;
            }
        }

        public static IO.MemoryStream ToStream(this string valor, Text.Encoding encoding = null)
        {
            IO.MemoryStream stream = new IO.MemoryStream();
            IO.StreamWriter writer;

            if (encoding != null)
                writer = new IO.StreamWriter(stream, encoding);
            else
                writer = new IO.StreamWriter(stream);

            writer.Write(valor);
            writer.Flush();

            stream.Position = 0;

            return stream;
        }

        #endregion Métodos Públicos - Conversão
    }
}
