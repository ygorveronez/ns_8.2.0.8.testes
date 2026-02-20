using System;
using System.Text.RegularExpressions;

namespace Dominio.ObjetosDeValor.Embarcador.Container
{
    public class CadastroDeContainer
    {
        private string container = string.Empty;
        private string tipoContainer = string.Empty;
        private string tipoContainerCodIntegracao = string.Empty;
        private string armadorBooking = string.Empty;
        private string transportadora = string.Empty;
        private string motorista = string.Empty;
        private string placa = string.Empty;

        public DateTime DataDeColeta { get; set; } = DateTime.MinValue;
        public string Container
        { 
            get => container;
            set => container = RemoveSpecialCharacters(value);
        }
        public string TipoContainer
        {
            get => tipoContainer.ToUpperInvariant();
            set => tipoContainer = RemoveSpecialCharacters(value);
        }

        public string TipoContainerCodigoIntegracao
        {
            get
            {
                if (TipoContainer.Contains("4"))
                {
                    if (TipoContainer.Contains("D") || TipoContainer.Contains("Y"))
                        tipoContainerCodIntegracao = "40D";
                    else
                        tipoContainerCodIntegracao = "40R";
                }
                else if (TipoContainer.Contains("2"))
                {
                    if (TipoContainer.Contains("D") || TipoContainer.Contains("Y"))
                        tipoContainerCodIntegracao = "20D";
                    else
                        tipoContainerCodIntegracao = "20R";
                }
                if (string.IsNullOrEmpty(tipoContainerCodIntegracao))
                    tipoContainerCodIntegracao = "40R";

                return tipoContainerCodIntegracao;
            }
        }

        public int TaraContainer { get; set; }
        public string ArmadorBooking
        {
            get => armadorBooking;
            set => armadorBooking = RemoveSpecialCharacters(value);
        }
        public string Transportadora
        {
            get => transportadora;
            set => transportadora = RemoveSpecialCharacters(value);
        }
        public string Motorista
        {
            get => motorista;
            set => motorista = RemoveSpecialCharacters(value);
        }
        public string Placa
        {
            get => placa;
            set => placa = RemoveSpecialCharacters(value).ToUpperInvariant();
        }

        private string RemoveSpecialCharacters(string input)
        {
            if (input == null) return string.Empty;
            Regex r = new Regex(@"(!|@|#|\$|%|¨|&|\*|\(|\)|\{\}|•|ª|º|´|`|~|\^|-)", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);
            return r.Replace(input, string.Empty).Replace("  ", " ").Trim();
            //esse também funciona pra remover caracteres especiais, mas também remove letras com acento:
            //(?:[^a-z0-9 ]|(?<=['\"])s) ou ([^a-z0-9 ])
        }
    }
}
