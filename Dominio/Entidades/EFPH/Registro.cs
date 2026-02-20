using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.Entidades.EFPH
{
    public abstract class Registro
    {
        #region Construtores

        public Registro(string identificador)
        {
            this.Identificador = identificador;
            this.Registros = new List<Registro>();
            this.RegistroEFPH = new StringBuilder();
            this.EscreverDado(this.Identificador, 2);
        }

        #endregion

        #region Propriedades

        public string Identificador { get; set; }

        public List<Registro> Registros { get; set; }

        protected StringBuilder RegistroEFPH { get; set; }

        #endregion

        #region Métodos

        public abstract string ObterDadosParaArquivo();

        protected void ObterRegistrosEFPHDerivados()
        {
            foreach (Registro registro in this.Registros)
            {
                this.RegistroEFPH.Append(registro.ObterDadosParaArquivo());
            }
        }

        protected void EscreverDado(string dado, int quantidadeCaracteres)
        {
            dado = dado == null ? string.Empty : dado.Trim();

            if (!string.IsNullOrWhiteSpace(dado))
            {
                if (dado.Length > quantidadeCaracteres)
                    this.RegistroEFPH.Append(dado.Substring(0, quantidadeCaracteres));
                else if (dado.Length < quantidadeCaracteres)
                    this.RegistroEFPH.Append(dado).Append(new string(' ', quantidadeCaracteres - dado.Length));
                else
                    this.RegistroEFPH.Append(dado);
            }
            else
            {
                this.RegistroEFPH.Append(new string(' ', quantidadeCaracteres));
            }
        }

        protected void EscreverDado(DateTime data, string formato = "")
        {
            if (string.IsNullOrWhiteSpace(formato))
                this.RegistroEFPH.Append(data.ToString("yyyyMMdd"));
            else
                this.RegistroEFPH.Append(data.ToString(formato));
        }

        protected void EscreverDado(DateTime? data, string formato)
        {
            if (string.IsNullOrWhiteSpace(formato))
                this.RegistroEFPH.Append(data.Value.ToString("yyyyMMdd"));
            else
                this.RegistroEFPH.Append(data.Value.ToString(formato));
        }

        protected void EscreverDado(decimal valor, int quantidadeInteiros, int quantidadeDecimais)
        {
            string formato = string.Concat("{0:", new string('0', quantidadeInteiros), ".", new string('0', quantidadeDecimais), "}");

            string valorFormatado = string.Format(formato, valor).Replace(".", "").Replace(",", "").Replace("-", "").Replace("+", "");

            this.RegistroEFPH.Append(valorFormatado);
        }

        protected void EscreverDado(int valor, int numeroDigitos)
        {
            this.RegistroEFPH.Append(valor.ToString("D" + numeroDigitos.ToString()));
        }

        protected void EscreverDado(long valor, int numeroDigitos)
        {
            this.RegistroEFPH.Append(valor.ToString("D" + numeroDigitos.ToString()));
        }

        protected void FinalizarRegistro()
        {
            this.RegistroEFPH.AppendLine();
        }

        #endregion
    }
}
