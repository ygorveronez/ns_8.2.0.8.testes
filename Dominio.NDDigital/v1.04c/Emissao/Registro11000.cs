namespace Dominio.NDDigital.v104.Emissao
{
    /// <summary>
    /// Versão de Schema e chave do CT-e
    /// </summary>
    public class Registro11000 : Registro
    {
        #region Construtores

        public Registro11000(string registro)
            : base(registro)
        {
            this.GerarRegistro();
        }

        #endregion

        #region Propriedades

        public string versao { get; set; }

        public string id { get; set; }

        public Registro11100 ide { get; set; }

        public Registro11300 compl { get; set; }

        public Registro12000 emit { get; set; }

        public Registro12100 rem { get; set; }

        public Registro12200 exped { get; set; }

        public Registro12300 receb { get; set; }

        public Registro12400 dest { get; set; }

        public Registro13000 vPrest { get; set; }

        public Registro14000 imp { get; set; }

        public Registro15000 infCTeNorm { get; set; }

        public Registro23000 infCteComp { get; set; }

        public Registro25000 infCteAnu { get; set; }

        public Registro50000 infIntegracao { get; set; }

        #endregion

        #region Métodos

        protected override void GerarRegistro()
        {
            string[] dados = this.StringRegistro.Split(';');

            this.Identificador = this.ObterString(dados[0]);
            this.versao = this.ObterString(dados[1]);
            this.id = this.ObterString(dados[2]);
        }

        #endregion
    }
}
