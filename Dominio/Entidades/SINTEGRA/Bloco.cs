using System.Collections.Generic;
using System.Text;

namespace Dominio.Entidades.SINTEGRA
{
    public class Bloco
    {

        #region Construtores

        public Bloco(string identificador)
        {
            this.Identificador = identificador;
            this.Registros = new List<Registro>();
        }

        #endregion

        #region Propriedades

        public string Identificador { get; set; }

        public List<Registro> Registros { get; set; }

        #endregion

        #region Métodos

        public string ObterDadosParaArquivo()
        {
            StringBuilder sb = new StringBuilder();
            foreach (Registro registro in this.Registros)
                sb.Append(registro.ObterDadosParaArquivo());
            return sb.ToString();
        }

        public int ObterTotalDeRegistros()
        {
            return this.ContarRegistros(this.Registros);
        }

        private int ContarRegistros(List<Registro> registros)
        {
            int count = 0;
            foreach (Registro registro in registros)
            {
                count += 1;
                count += this.ContarRegistros(registro.Registros);
            }
            return count;
        }

        #endregion

    }
}
