namespace Dominio.NDDigital.v104.Emissao
{
    /// <summary>
    /// ICMS devido à UF de origem da prestação, quando diferente da UF do emitente
    /// </summary>
    public class Registro24200 : Registro
    {
        #region Construtores

        public Registro24200(string registro)
            : base(registro)
        {
            this.GerarRegistro();
        }

        #endregion

        #region Propriedades

        /// <summary>
        /// Tributação do Serviço: 90 - ICMS outros
        /// </summary>
        public string CST { get; set; }

        /// <summary>
        /// Percentual de redução da BC do ICMS
        /// </summary>
        public decimal pRedBCOutraUF { get; set; }

        /// <summary>
        /// Valor da BC do ICMS
        /// </summary>
        public decimal vBCOutraUF { get; set; }

        /// <summary>
        /// Alíquota do ICMS
        /// </summary>
        public decimal pICMSOutraUF { get; set; }

        /// <summary>
        /// Valor do ICMS devido outra UF
        /// </summary>
        public decimal vICMSOutraUF { get; set; }

        #endregion

        #region Métodos

        protected override void GerarRegistro()
        {
            string[] dados = this.StringRegistro.Split(';');

            this.Identificador = this.ObterString(dados[0]);
            this.CST = this.ObterString(dados[1]);
            this.pRedBCOutraUF = this.ObterValor(dados[2]);
            this.vBCOutraUF = this.ObterValor(dados[3]);
            this.pICMSOutraUF = this.ObterValor(dados[4]);
            this.vICMSOutraUF = this.ObterValor(dados[5]);
        }

        #endregion
    }
}
