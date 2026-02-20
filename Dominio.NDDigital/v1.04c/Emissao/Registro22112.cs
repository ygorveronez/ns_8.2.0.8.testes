using System;

namespace Dominio.NDDigital.v104.Emissao
{
    /// <summary>
    /// informações da nota fiscal ou conhecimento de transporte emitido pelo Tomador
    /// </summary>
    public class Registro22112 : Registro
    {
        #region Construtores

        public Registro22112(string registro)
            : base(registro)
        {
            this.GerarRegistro();
        }

        #endregion

        #region Propriedades

        /// <summary>
        /// CNPJ do emitente do documento fiscal
        /// </summary>
        public string CNPJ { get; set; }

        /// <summary>
        /// Código do modelo do documento fiscal
        /// </summary>
        public string mod { get; set; }

        /// <summary>
        /// Série do documento fiscal
        /// </summary>
        public int serie { get; set; }

        /// <summary>
        /// Sub-serie do documento fiscal
        /// </summary>
        public int subserie { get; set; }

        /// <summary>
        /// Numero do documento fiscal
        /// </summary>
        public int nro { get; set; }

        /// <summary>
        /// Valor do documento fiscal
        /// </summary>
        public decimal valor { get; set; }

        /// <summary>
        /// Informar a data de emissão do documento fiscal
        /// </summary>
        public DateTime dEmi { get; set; }

        #endregion

        #region Métodos

        protected override void GerarRegistro()
        {
            string[] dados = this.StringRegistro.Split(';');

            this.Identificador = this.ObterString(dados[0]);
            this.CNPJ = this.ObterString(dados[1]);
            this.mod = this.ObterString(dados[2]);
            this.serie = this.ObterNumero(dados[3]);
            this.subserie = this.ObterNumero(dados[4]);
            this.nro = this.ObterNumero(dados[5]);
            this.valor = this.ObterValor(dados[6]);
            this.dEmi = this.ObterData(dados[7]);
        }

        #endregion
    }
}
