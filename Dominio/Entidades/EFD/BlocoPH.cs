using System.Collections.Generic;
using System.Text;


namespace Dominio.Entidades.EFD
{
    public class BlocoPH
    {
        #region Construtores

        public BlocoPH(string identificador)
        {
            this.Identificador = identificador;
            this.RegistrosPH = new List<RegistroPH>();
        }

        #endregion

        #region Propriedades

        public string Identificador { get; set; }

        public List<RegistroPH> RegistrosPH { get; set; }

        #endregion

        #region Métodos

        public string ObterDadosParaArquivo()
        {
            StringBuilder sb = new StringBuilder();
            foreach (RegistroPH registro in this.RegistrosPH)
                sb.Append(registro.ObterDadosParaArquivo());
            return sb.ToString();
        }

        public int ObterTotalDeRegistros()
        {
            return this.ContarRegistros(this.RegistrosPH);
        }

        private int ContarRegistros(List<RegistroPH> registrosPH)
        {
            int count = 0;
            foreach (RegistroPH registro in registrosPH)
            {
                if (registro.Identificador != "0001" && registro.Identificador != "C001" && registro.Identificador != "C990" && registro.Identificador != "D001" && registro.Identificador != "D990" && registro.Identificador != "9990")
                    count += 1;

                count += this.ContarRegistros(registro.Registros);

            }
            return count;
        }

        #endregion

    }
}
