namespace Dominio.NDDigital.v104.Emissao
{
    /// <summary>
    /// Informações referentes a previsão de entrega
    /// </summary>
    public class Registro11320 : Registro
    {
        #region Construtores

        public Registro11320(string registro)
            : base(registro)
        {
            this.GerarRegistro();
        }

        #endregion

        #region Propriedades

        public Registro11321 semData { get; set; }

        public Registro11322 comData { get; set; }

        public Registro11323 noPeriodo { get; set; }

        public Registro11327 semHora { get; set; }

        public Registro11328 comHora { get; set; }

        public Registro11329 noInter { get; set; }

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
