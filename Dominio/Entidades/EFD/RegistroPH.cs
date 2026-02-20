using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.Entidades.EFD
{
    public abstract class RegistroPH
    {
        #region Construtores

        public RegistroPH(string identificador)
        {
            this.Identificador = identificador;
            this.Registros = new List<RegistroPH>();
            this.RegistrosPH = new StringBuilder();
            if (identificador == "0001" || identificador == "C001" || identificador == "C990" || identificador == "D001" || identificador == "D990" || identificador == "9990")
                this.RegistrosPH = new StringBuilder();
            else
                this.InserirSeparadorDeDados();
            this.EscreverDado(this.Identificador);
        }

        #endregion

        #region Propriedades

        public string Identificador { get; set; }

        public List<RegistroPH> Registros { get; set; }

        protected StringBuilder RegistrosPH { get; set; }

        #endregion

        #region Métodos

        public abstract string ObterDadosParaArquivo();

        protected void ObterRegistrosPHDerivados()
        {
            foreach (RegistroPH registro in this.Registros)
            {
                this.RegistrosPH.Append(registro.ObterDadosParaArquivo());
            }
        }

        protected void InserirSeparadorDeDados()
        {
            this.RegistrosPH.Append("|");
        }

        protected void EscreverDado(string dado, int maximoCaracteres = 0)
        {
            if (!string.IsNullOrEmpty(dado))
            {
                dado = dado.Trim();

                if (dado == "0001" || dado == "C001" || dado == "C990" || dado == "D001" || dado == "D990" || dado == "9990")
                    return;

                if (maximoCaracteres > 0)
                    this.RegistrosPH.Append(dado.Length > maximoCaracteres ? dado.Substring(0, maximoCaracteres) : dado);
                else
                    this.RegistrosPH.Append(dado);
            }

            this.InserirSeparadorDeDados();
        }

        protected void EscreverDado(DateTime data)
        {
            this.RegistrosPH.Append(data.ToString("ddMMyyyy"));
            this.InserirSeparadorDeDados();
        }

        protected void EscreverDado(DateTime? data)
        {
            this.RegistrosPH.Append(data.Value.ToString("ddMMyyyy"));
            this.InserirSeparadorDeDados();
        }

        protected void EscreverDado(decimal valor)
        {
            this.RegistrosPH.Append(valor.ToString("0.00"));
            this.InserirSeparadorDeDados();
        }

        protected void EscreverDado(int valor, int numeroDigitos = 0)
        {
            if (numeroDigitos > 0)
                this.RegistrosPH.Append(valor.ToString("D" + numeroDigitos.ToString()));
            else
                this.RegistrosPH.Append(valor.ToString());

            this.InserirSeparadorDeDados();
        }

        protected void EscreverDado(long valor, int numeroDigitos = 0)
        {
            if (numeroDigitos > 0)
                this.RegistrosPH.Append(valor.ToString("D" + numeroDigitos.ToString()));
            else
                this.RegistrosPH.Append(valor.ToString());

            this.InserirSeparadorDeDados();
        }

        protected void FinalizarRegistro()
        {
            this.RegistrosPH.AppendLine();
        }

        #endregion
    }
}
