using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.Entidades.SINTEGRA
{
    public abstract class Registro
    {
        #region Construtores

        public Registro(string identificador)
        {
            this.Identificador = identificador;
            this.Registros = new List<Registro>();
            this.RegistroSINTEGRA = new StringBuilder();
            this.EscreverDado(this.Identificador, 2);
        }

        #endregion

        #region Propriedades

        public string Identificador { get; set; }

        public List<Registro> Registros { get; set; }

        protected StringBuilder RegistroSINTEGRA { get; set; }

        #endregion

        #region Métodos

        public abstract string ObterDadosParaArquivo();

        protected void ObterRegistrosSINTEGRADerivados()
        {
            foreach (Registro registro in this.Registros)
            {
                this.RegistroSINTEGRA.Append(registro.ObterDadosParaArquivo());
            }
        }

        protected void EscreverDado(string dado, int quantidadeCaracteres)
        {
            dado = !string.IsNullOrWhiteSpace(dado) ? dado.Trim() : string.Empty;

            if (!string.IsNullOrWhiteSpace(dado))
            {
                if (dado.Length > quantidadeCaracteres)
                    this.RegistroSINTEGRA.Append(dado.Substring(0, quantidadeCaracteres));
                else if (dado.Length < quantidadeCaracteres)
                    this.RegistroSINTEGRA.Append(dado).Append(new string(' ', quantidadeCaracteres - dado.Length));
                else
                    this.RegistroSINTEGRA.Append(dado);
            }
            else
            {
                this.RegistroSINTEGRA.Append(new string(' ', quantidadeCaracteres));
            }
        }

        protected void EscreverDado(DateTime data)
        {
            this.RegistroSINTEGRA.Append(data.ToString("yyyyMMdd"));
        }

        protected void EscreverDado(DateTime? data)
        {
            this.RegistroSINTEGRA.Append(data.Value.ToString("yyyyMMdd"));
        }

        protected void EscreverDado(decimal valor, int quantidadeInteiros, int quantidadeDecimais)
        {
            string formato = string.Concat("{0:", new string('0', quantidadeInteiros), ".", new string('0', quantidadeDecimais), "}");

            string valorFormatado = string.Format(formato, valor).Replace(".", "").Replace(",", "").Replace("-", "").Replace("+", "");

            this.RegistroSINTEGRA.Append(valorFormatado);
        }

        protected void EscreverDado(int valor, int numeroDigitos)
        {
            this.RegistroSINTEGRA.Append(valor.ToString("D" + numeroDigitos.ToString()));
        }

        protected void EscreverDado(long valor, int numeroDigitos)
        {
            this.RegistroSINTEGRA.Append(valor.ToString("D" + numeroDigitos.ToString()));
        }

        protected void FinalizarRegistro()
        {
            this.RegistroSINTEGRA.AppendLine();
        }

        #endregion
    }
}
