using System.Collections.Generic;

namespace Dominio.NDDigital.v104.Emissao
{
    /// <summary>
    /// Informações sobre a carga
    /// </summary>
    public class Registro15100 : Registro
    {
        #region Construtores

        public Registro15100(string registro)
            : base(registro)
        {
            this.infQ = new List<Registro15110>();

            this.GerarRegistro();
        }

        #endregion

        #region Propriedades

        /// <summary>
        /// Valor total da carga
        /// </summary>
        public decimal vCarga { get; set; }

        /// <summary>
        /// Produto predominante
        /// </summary>
        public string proPred { get; set; }

        /// <summary>
        /// Outras características da carga
        /// </summary>
        public string xOutCat { get; set; }

        public List<Registro15110> infQ { get; set; }

        #endregion

        #region Métodos

        protected override void GerarRegistro()
        {
            string[] dados = this.StringRegistro.Split(';');

            this.Identificador = this.ObterString(dados[0]);
            this.vCarga = this.ObterValor(dados[1]);
            this.proPred = this.ObterString(dados[2]);
            this.xOutCat = this.ObterString(dados[3]);
        }

        #endregion
    }
}
