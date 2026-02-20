namespace Dominio.NDDigital.v104.Emissao
{
    /// <summary>
    /// Informações sobre a quantidade da carga
    /// </summary>
    public class Registro15110 : Registro
    {
        #region Construtores

        public Registro15110(string registro)
            : base(registro)
        {
            this.GerarRegistro();
        }

        #endregion

        #region Propriedades

        /// <summary>
        /// Código da Unidade de Medida
        /// 00-M3;
        /// 01-KG;
        /// 02-TON;
        /// 03-UNIDADE;
        /// 04-LITROS;
        /// 05-MMBTU.
        /// </summary>
        public string cUnid { get; set; }

        /// <summary>
        /// Tipo da Medida
        /// </summary>
        public string tpMed { get; set; }

        /// <summary>
        /// Quantidade
        /// </summary>
        public decimal qCarga { get; set; }

        #endregion

        #region Métodos

        protected override void GerarRegistro()
        {
            string[] dados = this.StringRegistro.Split(';');

            this.Identificador = this.ObterString(dados[0]);
            this.cUnid = this.ObterString(dados[1]);
            this.tpMed = this.ObterString(dados[2]);
            this.qCarga = this.ObterValor(dados[3]);
        }

        #endregion
    }
}
