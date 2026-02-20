using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.Entidades.EFD
{
    public abstract class Registro
    {
        #region Construtores

        public Registro(string identificador)
        {
            this.Identificador = identificador;
            this.Registros = new List<Registro>();
            this.RegistroSPED = new StringBuilder();
            this.InserirSeparadorDeDados();
            this.EscreverDado(this.Identificador);
        }

        #endregion

        #region Propriedades

        public string Identificador { get; set; }

        public List<Registro> Registros { get; set; }

        protected StringBuilder RegistroSPED { get; set; }

        #endregion

        #region Métodos

        public abstract string ObterDadosParaArquivo();

        protected void ObterRegistrosSPEDDerivados()
        {
            foreach (Registro registro in this.Registros)
            {
                this.RegistroSPED.Append(registro.ObterDadosParaArquivo());
            }
        }

        protected void InserirSeparadorDeDados()
        {
            this.RegistroSPED.Append("|");
        }

        protected void EscreverDado(string dado, int maximoCaracteres = 0)
        {
            if (!string.IsNullOrEmpty(dado))
            {
                dado = dado.Trim();

                if (maximoCaracteres > 0)
                    this.RegistroSPED.Append(dado.Length > maximoCaracteres ? dado.Substring(0, maximoCaracteres) : dado);
                else
                    this.RegistroSPED.Append(dado);
            }

            this.InserirSeparadorDeDados();
        }

        protected void EscreverDado(DateTime data)
        {
            this.RegistroSPED.Append(data.ToString("ddMMyyyy"));
            this.InserirSeparadorDeDados();
        }

        protected void EscreverDado(DateTime? data)
        {
            this.RegistroSPED.Append(data.Value.ToString("ddMMyyyy"));
            this.InserirSeparadorDeDados();
        }

        protected void EscreverDado(decimal valor, int numeroDigitos = 0)
        {
            if (numeroDigitos > 0)
                this.RegistroSPED.Append(valor.ToString("0." + "0".PadLeft(numeroDigitos, '0')));
            else
                this.RegistroSPED.Append(valor.ToString("0.00"));
            this.InserirSeparadorDeDados();
        }

        protected void EscreverDado(int valor, int numeroDigitos = 0)
        {
            if (numeroDigitos > 0)
                this.RegistroSPED.Append(valor.ToString("D" + numeroDigitos.ToString()));
            else
                this.RegistroSPED.Append(valor.ToString());

            this.InserirSeparadorDeDados();
        }

        protected void FinalizarRegistro()
        {
            this.RegistroSPED.AppendLine();
        }

        #endregion
    }
}
