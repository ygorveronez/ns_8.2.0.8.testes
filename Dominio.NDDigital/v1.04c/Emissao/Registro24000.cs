namespace Dominio.NDDigital.v104.Emissao
{
    /// <summary>
    /// Informações relativas aos impostos do CTe
    /// </summary>
    public class Registro24000 : Registro
    {
        #region Construtores

        public Registro24000(string registro)
            : base(registro)
        {
            this.GerarRegistro();
        }

        #endregion

        #region Propriedades

        public Registro24100 ICMS00 { get; set; }

        public Registro24120 ICMS20 { get; set; }

        public Registro24145 ICMS45 { get; set; }

        public Registro24160 ICMS60 { get; set; }

        public Registro24190 ICMS90 { get; set; }

        public Registro24200 ICMSOutraUF { get; set; }

        public Registro24210 ICMSSN { get; set; }

        public Registro24300 infAdFisco { get; set; }

        #endregion

        #region Métodos

        protected override void GerarRegistro()
        {
            string[] dados = this.StringRegistro.Split(';');

            this.Identificador = this.ObterString(dados[0]);
        }

        #endregion
    }
}
