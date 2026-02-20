using System;
using System.Text.RegularExpressions;

namespace Utilidades
{
    public class Decimal
    {
        public static decimal Converter(string valor)
		{
			if (string.IsNullOrWhiteSpace(valor))
				return 0m;

			// Mantém apenas dígitos, possíveis separadores e sinal negativo
			string valorHigienizado = Regex.Replace(valor.Trim(), @"[^0-9.,-]", "").Trim();

			return valorHigienizado.ToDecimal();
		}
    }
}
