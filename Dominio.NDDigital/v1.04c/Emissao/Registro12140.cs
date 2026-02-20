using System;

namespace Dominio.NDDigital.v104.Emissao
{
    /// <summary>
    /// Grupo de informações dos demais documentos
    /// </summary>
    public class Registro12140 : Registro
    {
        #region Construtores

        public Registro12140(string registro)
            : base(registro)
        {
            this.GerarRegistro();
        }

        #endregion

        #region Propriedades

        /// <summary>
        /// Tipo de documento originário
        /// 00 - Declaração;
        /// 10 - Dutoviário;
        /// 99 - Outros.
        /// </summary>
        public int tpDoc { get; set; }

        /// <summary>
        /// Descrição
        /// </summary>
        public string descOutros { get; set; }

        /// <summary>
        /// Número
        /// </summary>
        public string nDoc { get; set; }

        /// <summary>
        /// Data da emissão
        /// </summary>
        public DateTime dEmi { get; set; }

        /// <summary>
        /// Valor do documento
        /// </summary>
        public decimal vDocFisc { get; set; }

        #endregion

        #region Métodos

        protected override void GerarRegistro()
        {
            string[] dados = this.StringRegistro.Split(';');

            this.Identificador = this.ObterString(dados[0]);
            this.tpDoc = this.ObterNumero(dados[1]);
            this.descOutros = this.ObterString(dados[2]);
            this.nDoc = this.ObterString(dados[3]);
            this.dEmi = this.ObterData(dados[4]);
            this.vDocFisc = this.ObterValor(dados[5]);
        }

        #endregion
    }
}
