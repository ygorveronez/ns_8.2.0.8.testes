using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.Entidades.EDI
{
    public abstract class Registro
    {
        #region Construtores

        public Registro(string identificador)
        {

            this.Identificador = identificador;
            this.Registros = new List<Registro>();
            this.StringRegistro = new StringBuilder();
            this.EscreverDado(this.Identificador, 3);
        }

        #endregion

        #region Propriedades

        public string Identificador { get; set; }

        public List<Registro> Registros { get; set; }

        protected StringBuilder StringRegistro { get; set; }

        #endregion

        #region Métodos

        public abstract string ObterDadosParaArquivo();

        protected void ObterRegistrosDerivados()
        {
            foreach (Registro registro in this.Registros)
            {
                this.StringRegistro.Append(registro.ObterDadosParaArquivo());
            }
        }

        protected void EscreverDado(char dado, int repeticoes)
        {
            this.StringRegistro.Append(new string(dado, repeticoes));
        }

        protected void EscreverDado(string dado)
        {
            this.StringRegistro.Append(dado);
        }

        protected void EscreverDado(string dado, int numeroCaracteres)
        {
            if (!string.IsNullOrWhiteSpace(dado))
            {
                if (dado.Length > numeroCaracteres)
                {
                    this.StringRegistro.Append(dado.Remove(numeroCaracteres, (dado.Length - numeroCaracteres)));
                }
                else
                {
                    this.StringRegistro.Append(dado);

                    if (dado.Length != numeroCaracteres)
                        this.StringRegistro.Append(new string(' ', numeroCaracteres - dado.Length));
                }
            }
            else
            {
                this.StringRegistro.Append(new string(' ', numeroCaracteres));
            }
        }

        protected void EscreverDado(DateTime data, string formato = null)
        {
            if (string.IsNullOrWhiteSpace(formato))
                this.StringRegistro.Append(data.ToString("ddMMyyyy"));
            else
                this.StringRegistro.Append(data.ToString(formato));
        }

        protected void EscreverDado(DateTime? data, string formato = null)
        {
            if (string.IsNullOrWhiteSpace(formato))
                this.StringRegistro.Append(data.Value.ToString("ddMMyyyy"));
            else
                this.StringRegistro.Append(data.Value.ToString(formato));
        }

        protected void EscreverDado(decimal valor, int numeroInteiros, int numeroDecimais)
        {
            string formato = "{0:";

            for (var i = 0; i < numeroInteiros; i++)
                formato += "0";

            formato += ".";

            for (var i = 0; i < numeroDecimais; i++)
                formato += "0";

            formato += "}";

            this.StringRegistro.Append(string.Format(formato, valor).Replace(".", "").Replace(",", ""));
        }

        protected void EscreverDado(int valor, int numeroDigitos)
        {
            this.StringRegistro.Append(valor.ToString("D" + numeroDigitos.ToString()));
        }

        protected void FinalizarRegistro()
        {
            this.StringRegistro.AppendLine();
        }

        #endregion
    }
}
