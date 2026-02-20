using System;

namespace Dominio.NDDigital
{
    public abstract class Registro
    {
        #region Construtores

        public Registro(string registro)
        {
            this.StringRegistro = registro;
        }

        #endregion

        #region Propriedades

        protected string StringRegistro { get; set; }

        public string Identificador { get; set; }

        #endregion

        #region Métodos

        protected virtual void GerarRegistro()
        {
        }

        protected int ObterNumero(string param)
        {
            int num = 0;
            int.TryParse(this.ObterString(param), out num);
            return num;
        }

        protected DateTime ObterData(string param, string pattern = null)
        {
            DateTime data = DateTime.MinValue;
            DateTime.TryParseExact(this.ObterString(param), string.IsNullOrWhiteSpace(pattern) ? "yyyy-MM-dd" : pattern, null, System.Globalization.DateTimeStyles.None, out data);
            return data;
        }

        protected TimeSpan ObterHora(string param, string pattern = "")
        {
            TimeSpan hora = TimeSpan.MinValue;
            TimeSpan.TryParseExact(this.ObterString(param), string.IsNullOrWhiteSpace(pattern) ? "HH:mm:ss" : pattern, null, out hora);
            return hora;
        }

        protected decimal ObterValor(string param)
        {
            decimal valor = 0;
            decimal.TryParse(this.ObterString(param), System.Globalization.NumberStyles.Any, new System.Globalization.CultureInfo("en-US"), out valor);
            return valor;
        }

        protected string ObterString(string param)
        {
            if (!string.IsNullOrEmpty(param))
                return param.Trim();
            else
                return param;
        }

        #endregion
    }
}
