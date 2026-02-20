namespace Dominio.NDDigital.v104.Emissao
{
    /// <summary>
    /// Tributação pelo ICMS60 - ICMS cobrado por substituição tributária.Responsabilidade do recolhimento do ICMS atribuído ao tomador ou 3º por ST
    /// </summary>
    public class Registro24160 : Registro
    {
        #region Construtores

        public Registro24160(string registro)
            : base(registro)
        {
            this.GerarRegistro();
        }

        #endregion

        #region Propriedades

        /// <summary>
        /// Tributação do Serviço: 60 - ICMS cobrado anteriormente por substituição tributária
        /// </summary>
        public string CST { get; set; }

        /// <summary>
        /// Valor da BC do ICMS ST retido
        /// </summary>
        public decimal vBCSTRet { get; set; }

        /// <summary>
        /// Valor do ICMS ST retido
        /// </summary>
        public decimal vICMSSTRet { get; set; }

        /// <summary>
        /// Alíquota do ICMS
        /// </summary>
        public decimal pICMSSTRet { get; set; }

        /// <summary>
        /// Valor do Crédito outorgado/Presumido
        /// </summary>
        public decimal vCred { get; set; }

        #endregion

        #region Métodos

        protected override void GerarRegistro()
        {
            string[] dados = this.StringRegistro.Split(';');

            this.Identificador = this.ObterString(dados[0]);
            this.CST = this.ObterString(dados[1]);
            this.vBCSTRet = this.ObterValor(dados[2]);
            this.vICMSSTRet = this.ObterValor(dados[3]);
            this.pICMSSTRet = this.ObterValor(dados[4]);
            this.vCred = this.ObterValor(dados[5]);
        }

        #endregion
    }
}
