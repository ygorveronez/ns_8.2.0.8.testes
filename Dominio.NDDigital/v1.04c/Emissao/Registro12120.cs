using System;

namespace Dominio.NDDigital.v104.Emissao
{
    /// <summary>
    /// Grupo de informações das Notas Fiscais
    /// </summary>
    public class Registro12120 : Registro
    {
        #region Construtores

        public Registro12120(string registro)
            : base(registro)
        {
            this.GerarRegistro();
        }

        #endregion

        #region Propriedades

        /// <summary>
        /// Número do Romaneio da NF
        /// </summary>
        public string nRoma { get; set; }

        /// <summary>
        /// Número do Pedido da NF
        /// </summary>
        public string nPed { get; set; }

        /// <summary>
        /// Modelo da Nota Fiscal
        /// </summary>
        public int mod { get; set; }

        public string serie { get; set; }

        /// <summary>
        /// Número
        /// </summary>
        public string nDoc { get; set; }

        /// <summary>
        /// Data da emissão
        /// </summary>
        public DateTime dEmi { get; set; }

        /// <summary>
        /// Base de Cálculo do ICMS
        /// </summary>
        public decimal vBC { get; set; }

        /// <summary>
        /// Valor total do ICMS
        /// </summary>
        public decimal vICMS { get; set; }

        /// <summary>
        /// Base de cálculo do ICMS ST
        /// </summary>
        public decimal vBCST { get; set; }

        /// <summary>
        /// Valor total do ICMS ST
        /// </summary>
        public decimal vST { get; set; }

        /// <summary>
        /// Valor total dos produtos
        /// </summary>
        public decimal vProd { get; set; }

        /// <summary>
        /// Valor total da NF
        /// </summary>
        public decimal vNF { get; set; }

        /// <summary>
        /// CFOP predominante
        /// </summary>
        public int nCFOP { get; set; }

        public decimal nPeso { get; set; }

        /// <summary>
        /// PIN SUFRAMA
        /// </summary>
        public int PIN { get; set; }

        public Registro12121 locRet { get; set; }

        #endregion

        #region Métodos

        protected override void GerarRegistro()
        {
            string[] dados = this.StringRegistro.Split(';');

            this.Identificador = this.ObterString(dados[0]);
            this.nRoma = this.ObterString(dados[1]);
            this.nPed = this.ObterString(dados[2]);
            this.mod = this.ObterNumero(dados[3]);
            this.serie = this.ObterString(dados[4]);
            this.nDoc = this.ObterString(dados[5]);
            this.dEmi = this.ObterData(dados[6]);
            this.vBC = this.ObterValor(dados[7]);
            this.vICMS = this.ObterValor(dados[8]);
            this.vBCST = this.ObterValor(dados[9]);
            this.vST = this.ObterValor(dados[10]);
            this.vProd = this.ObterValor(dados[11]);
            this.vNF = this.ObterValor(dados[12]);
            this.nCFOP = this.ObterNumero(dados[13]);
            this.nPeso = this.ObterValor(dados[14]);
            this.PIN = this.ObterNumero(dados[15]);
        }

        #endregion
    }
}
