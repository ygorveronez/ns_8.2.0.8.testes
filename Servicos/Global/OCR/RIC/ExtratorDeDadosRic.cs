using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Servicos.Global
{
    public static class ExtratorDeDadosRic
    {
        #region Propiedades 

        public static Regex _regexPlaca = new Regex(@"(\w{3} \d{1}\w{1}\d{2})|(\w{3} \d{4})|(\w{3}\d{4})|(\w{3}-\d{4})|(\w{3}\d{1}\w{1}\d{2})|(\w{3}-\d{1}\w{1}\d{2})", RegexOptions.IgnoreCase);
        public static Regex _regexContainer = new Regex(@"(\w{4}\d{6}-\d{1})|(\w{4} \d{6} \d{1})|(\w{4} \d{3} \d{3}-\d{1})|(\w{4}\d{7})|(\w{4}-\d{7})|(\w{4} \d{7})|(\w{4} \d{6}-\d{1})|(\w{4} \d{3}-\d{3}-\d{1})|(\w{4}-\d{6}-\d{1})", RegexOptions.IgnoreCase);
        public static Regex _regexHora = new Regex(@"(\d{2}:\d{2})");
        public static Regex _regexData = new Regex(@"(\d{2}/\d{2}/\d{4})");
        public static Regex _regexNumeros = new Regex(@"(\d+)");
        public static Regex _regexTipoContainer = new Regex(@"([A-Z]{2}[0-9]{2})");
        public static Regex _regexTara = new Regex(@"([0-9]{4})");

        #endregion
      
        #region Metodos Publicos
        public static string Placa(string valor)
        {
            if (string.IsNullOrEmpty(valor))
                return string.Empty;
            var placa = FormatarComRegex(valor, _regexPlaca);
            var idxZero = 0;
            while (idxZero > -1)
            {
                idxZero = placa.IndexOf("0");
                if (idxZero >= 0 && idxZero <= 2) //tem zero no lugar de letra. O OCR interpretou a letra O como 0
                {
                    placa = placa.Remove(idxZero, 1);
                    placa = placa.Insert(idxZero, "O");
                }
                else
                {
                    idxZero = -1;
                }
            }
            return placa;
        }

        public static string Container(string valor)
        {
            if (string.IsNullOrEmpty(valor))
                return string.Empty;
            return FormatarComRegex(valor, _regexContainer);
        }

        public static string Tara(string valor)
        {
            if (string.IsNullOrEmpty(valor))
                return string.Empty;
            var idxVirgula = valor.IndexOf(",");
            if (idxVirgula > -1)
                valor = valor.Substring(0, idxVirgula);
            valor = valor.Replace(".", string.Empty);
            return FormatarComRegex(valor, _regexNumeros);
        }

        public static string Data(string valor)
        {
            if (string.IsNullOrEmpty(valor))
                return string.Empty;
            return FormatarComRegex(valor, _regexData);
        }

        public static string Hora(string valor)
        {
            if (string.IsNullOrEmpty(valor))
                return string.Empty;
            return FormatarComRegex(valor, _regexHora);
        }

        /// <summary>
        /// As vezes o valor aparece na linha de baixo do campo. As vezes aparece ao lado.
        /// Isso pode acontecer no mesmo modelo de RIC, dependendo da imagem.
        /// É preciso verificar cada campo. É isso o que o algoritmo abaixo faz.
        /// </summary>
        public static string ExtrairValor(List<string> textoOCR, int i, List<string> modelosDeCampoOCR, bool transportadora = false, int numeroRecursao = 1, Regex filtro = null)
        {
            if (textoOCR == null || i >= textoOCR.Count)
                return string.Empty;

            if (numeroRecursao == 6)
                return string.Empty;

            var linhaTexto = textoOCR[i];

            if (modelosDeCampoOCR == null || !modelosDeCampoOCR.Any())
                return string.Empty;
            if (string.IsNullOrEmpty(linhaTexto))
                return string.Empty;

            bool linhaAtualContemCampoDesejado = modelosDeCampoOCR.Any(x => linhaTexto.Contains(x));
            if (numeroRecursao == 1 && !linhaAtualContemCampoDesejado)
                return string.Empty;

            if (filtro != null)
            {
                var m = filtro.Match(linhaTexto);
                if (m.Success)
                    return m.Value;
                else
                    return ExtrairValor(textoOCR, i + 1, modelosDeCampoOCR, transportadora, numeroRecursao + 1, filtro);
            }

            modelosDeCampoOCR = modelosDeCampoOCR.OrderByDescending(x => x.Length).ToList();
            string valor = linhaTexto.Trim().Trim(new char[] { '-', '.', ':', '/', '"' }).Trim();

            foreach (var strModelo in modelosDeCampoOCR)
                if (valor == strModelo)
                    valor = string.Empty;

            if (!string.IsNullOrEmpty(valor) && numeroRecursao == 1) //se o numeroRecursao for maior que 1 então está procurando por valores e não pela propriedade
            {
                foreach (var strModelo in modelosDeCampoOCR)
                {
                    //No caso da transportadora, o replace pode remover parte do nome da transportadora, então se usa o StartsWith
                    //Nos demais campos se usa o Contains porque pode ter caracteres antes da propriedade, principalmente se vem no formato de quebra de linha
                    if ((transportadora && valor.Trim().StartsWith(strModelo)) || (!transportadora && valor.Contains(strModelo)))
                    {
                        //só deve fazer o replace uma vez. No caso da transportadora, o replace pode remover parte do nome da transportadora
                        var idx = valor.IndexOf(strModelo);
                        if (idx == -1)
                            continue;
                        valor = valor.Substring(idx + strModelo.Length);
                        break;
                    }
                }
            }
            valor = valor.Replace("•", string.Empty).Replace("*", string.Empty).Replace("=", string.Empty).Replace("  ", " ");
            //valor = valor.Trim().Trim("-".ToCharArray()).Trim(".".ToCharArray()).Trim(":".ToCharArray()).Trim("/".ToCharArray()).Trim();
            valor = valor.Trim().Trim(new char[] { '-', '.', ':', '/', '"' }).Trim();

            if (string.IsNullOrEmpty(valor)) //a linha atual possuia apenas o nome do campo. O valor está na proxima linha.
                valor = ExtrairValor(textoOCR, i + 1, modelosDeCampoOCR, transportadora, numeroRecursao + 1, filtro);

            return valor;
        }
        #endregion


        #region Metodos Privados
        private static string FormatarComRegex(string valor, Regex regex)
        {
            Match rxResult = regex.Match(valor);
            string strResult = string.Empty;
            if (rxResult.Success)
                strResult = rxResult.Value.Replace(" ", string.Empty).Replace("-", string.Empty);

            return strResult;
        }

        #endregion
    }
}
