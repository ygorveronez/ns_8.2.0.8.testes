using System;

namespace Dominio.NDDigital.v104.Emissao
{
    /// <summary>
    /// Dados das duplicatas
    /// </summary>
    public class Registro22056 : Registro
    {
        #region Construtores

        public Registro22056(string registro)
            : base(registro)
        {
            this.GerarRegistro();
        }

        #endregion

        #region Propriedades

        /// <summary>
        /// Número da duplicata
        /// </summary>
        public string nDup { get; set; }

        /// <summary>
        /// Data de vencimento da duplicata
        /// </summary>
        public DateTime dVenc { get; set; }

        /// <summary>
        /// Valor da duplicata
        /// </summary>
        public decimal vDup { get; set; }

        #endregion

        #region Métodos

        protected override void GerarRegistro()
        {
            string[] dados = this.StringRegistro.Split(';');

            this.Identificador = this.ObterString(dados[0]);
            this.nDup = this.ObterString(dados[1]);
            this.dVenc = this.ObterData(dados[2]);
            this.vDup = this.ObterValor(dados[3]);
        }

        #endregion
    }
}
