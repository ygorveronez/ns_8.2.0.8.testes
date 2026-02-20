namespace Dominio.NDDigital.v104.Emissao
{
    /// <summary>
    /// Informações relativas aos impostos do CTe
    /// </summary>
    public class Registro14000 : Registro
    {
        #region Construtores

        public Registro14000(string registro)
            : base(registro)
        {
            this.GerarRegistro();
        }

        #endregion

        #region Propriedades

        /// <summary>
        /// Incluir ICMS no Frete: 0 - Não, 1 - Sim
        /// </summary>
        public int incluirICMSNoFrete { get; set; }

        /// <summary>
        /// Percentual do ICMS Recolhido
        /// </summary>
        public decimal percentualICMSRecolhido { get; set; }

        public Registro14100 ICMS00 { get; set; }

        public Registro14120 ICMS20 { get; set; }

        public Registro14145 ICMS45 { get; set; }

        public Registro14160 ICMS60 { get; set; }

        public Registro14190 ICMS90 { get; set; }

        public Registro14200 ICMSOutraUF { get; set; }

        public Registro14210 ICMSSN { get; set; }

        public Registro14300 infAdFisco { get; set; }

        #endregion

        #region Métodos

        protected override void GerarRegistro()
        {
            string[] dados = this.StringRegistro.Split(';');

            this.Identificador = this.ObterString(dados[0]);
            this.incluirICMSNoFrete = this.ObterNumero(dados[1]);
            this.percentualICMSRecolhido = this.ObterValor(dados[2]);
        }

        #endregion
    }
}
