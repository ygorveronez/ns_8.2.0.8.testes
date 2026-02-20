using System;
using System.Text.RegularExpressions;

namespace Utilidades
{
    public class Validate
    {
        public static bool ValidarCPF(string cpf)
        {

            cpf = String.OnlyNumbers(cpf);
            int soma, i, resultado, digitos_iguais = 0;
            string numeros, digitos;
            digitos_iguais = 1;
            if (cpf.Length < 11)
                return false;
            for (i = 0; i < cpf.Length - 1; i++)
            {
                if (cpf.Substring(i, 1) != cpf.Substring(i + 1, 1))
                {
                    digitos_iguais = 0;
                    break;
                }
            }
            if (digitos_iguais == 0)
            {
                numeros = cpf.Substring(0, 9);
                digitos = cpf.Substring(9);
                soma = 0;
                for (i = 10; i > 1; i--)
                    soma += int.Parse(numeros.Substring(10 - i, 1)) * i;
                resultado = soma % 11 < 2 ? 0 : 11 - soma % 11;
                if (resultado != int.Parse(digitos.Substring(0, 1)))
                    return false;
                numeros = cpf.Substring(0, 10);
                soma = 0;
                for (i = 11; i > 1; i--)
                    soma += int.Parse(numeros.Substring(11 - i, 1)) * i;
                resultado = soma % 11 < 2 ? 0 : 11 - soma % 11;
                if (resultado != int.Parse(digitos.Substring(1, 1)))
                    return false;
                return true;
            }
            else
                return false;
        }

        public static bool ValidarCNPJ(string cnpj)
        {
            cnpj = String.OnlyNumbers(cnpj);
            int soma, i, resultado, pos, tamanho, digitos_iguais;
            string numeros, digitos;
            digitos_iguais = 1;
            if (cnpj.Length != 14)
                return false;
            for (i = 0; i < cnpj.Length - 1; i++)
                if (cnpj.Substring(i, 1) != cnpj.Substring(i + 1, 1))
                {
                    digitos_iguais = 0;
                    break;
                }
            if (digitos_iguais == 0)
            {
                tamanho = cnpj.Length - 2;
                numeros = cnpj.Substring(0, tamanho);
                digitos = cnpj.Substring(tamanho);
                soma = 0;
                pos = tamanho - 7;
                for (i = tamanho; i >= 1; i--)
                {
                    soma += int.Parse(numeros.Substring(tamanho - i, 1)) * pos--;
                    if (pos < 2)
                        pos = 9;
                }
                resultado = soma % 11 < 2 ? 0 : 11 - soma % 11;
                if (resultado != int.Parse(digitos.Substring(0, 1)))
                    return false;
                tamanho = tamanho + 1;
                numeros = cnpj.Substring(0, tamanho);
                soma = 0;
                pos = tamanho - 7;
                for (i = tamanho; i >= 1; i--)
                {
                    soma += int.Parse(numeros.Substring(tamanho - i, 1)) * pos--;
                    if (pos < 2)
                        pos = 9;
                }
                resultado = soma % 11 < 2 ? 0 : 11 - soma % 11;
                if (resultado != int.Parse(digitos.Substring(1, 1)))
                    return false;
                return true;
            }
            else
                return false;
        }

        public static bool ValidarChave(string chave)
        {
            chave = String.OnlyNumbers(chave);

            if (chave.Length != 44) return false;

            string chaveExtraida = chave.Substring(0, 43);
            int digitoVerificador;
            int.TryParse(chave.Substring(43, 1), out digitoVerificador);

            return Calc.Modulo11(chaveExtraida) == digitoVerificador;
        }

        public static bool ValidarChaveNFe(string chave)
        {
            chave = String.OnlyNumbers(chave);

            if (chave.Length != 44) return false;

            string tipoDocumento = chave.Substring(20, 2);
            if (!tipoDocumento.Equals("55")) return false;

            return ValidarChave(chave);
        }

        public static bool ValidarEmail(string mail)
        {
            var er = new System.Text.RegularExpressions.Regex(@"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$");
            if (er.IsMatch(mail))
                return true;
            else
                return false;
        }

        public static bool ValidarRENAVAM(string RENAVAM)
        {
            RENAVAM = String.OnlyNumbers(RENAVAM);

            if (string.IsNullOrEmpty(RENAVAM) || RENAVAM.Length != 11)
                return false;

            int[] d = new int[11];
            string sequencia = "3298765432";

            //verificando se todos os numeros são iguais **************************
            if (new string(RENAVAM[0], RENAVAM.Length) == RENAVAM)
                return false;
            RENAVAM = Convert.ToInt64(RENAVAM).ToString("00000000000");

            int v = 0;

            for (int i = 0; i < 11; i++)
                d[i] = Convert.ToInt32(RENAVAM.Substring(i, 1));

            for (int i = 0; i < 10; i++)
                v += d[i] * Convert.ToInt32(sequencia.Substring(i, 1));

            v = (v * 10) % 11; v = (v != 10) ? v : 0;
            return (v == d[10]);
        }

        public static bool ValidarPlaca(string placa)
        {
            placa = String.RemoveAllSpecialCharacters(placa);
            if (string.IsNullOrEmpty(placa) || placa.Length != 7)
                return false;

            return Regex.IsMatch(placa, @"[A-Z]{3}[0-9]{1}[A-Z]{1}[0-9]{2}|[A-Z]{3}[0-9]{4}");//Com padrão MERCOSUL
        }

        public static bool ValidarPISPASEP(string pis)
        {
            pis = String.OnlyNumbers(pis ?? string.Empty);
            int[] multiplicador = new int[10] { 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            int soma;
            int resto;

            if (string.IsNullOrWhiteSpace(pis))
                return true;

            if (pis.Trim().Length != 11)
                return false;

            pis = pis.Trim();
            pis = pis.Replace("-", "").Replace(".", "").PadLeft(11, '0');

            soma = 0;
            for (int i = 0; i < 10; i++)
                soma += int.Parse(pis[i].ToString()) * multiplicador[i];
            resto = soma % 11;
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;
            return pis.EndsWith(resto.ToString());
        }

        public static bool ValidarCPFCNPJ(string cpfCnpj)
        {
            if (string.IsNullOrWhiteSpace(cpfCnpj))
                return false;

            cpfCnpj = String.OnlyNumbers(cpfCnpj);

            if (cpfCnpj.Length == 11)
                return ValidarCPF(cpfCnpj);
            else if (cpfCnpj.Length == 14)
                return ValidarCNPJ(cpfCnpj);

            return false;
        }
    }
}
