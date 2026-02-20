namespace Dominio.NDDigital.v104.Emissao
{
    public class Registro11310 : Registro
    {
        #region Construtores

        public Registro11310(string registro)
            : base(registro)
        {
            this.GerarRegistro();
        }

        #endregion

        #region Propriedades

        /// <summary>
        /// Sigla ou código interno da Filial/Porto/Estação/Aeroporto de Origem
        /// </summary>
        public string xOrig { get; set; }

        /// <summary>
        /// Sigla ou código interno da Filial/Porto/Estação/Aeroporto de Origem
        /// </summary>
        public string xDest { get; set; }

        /// <summary>
        /// Código da Rota de Entrega
        /// </summary>
        public string xRota { get; set; }

        public Registro11311 pass { get; set; }

        #endregion

        #region Métodos

        protected override void GerarRegistro()
        {
            string[] dados = this.StringRegistro.Split(';');

            this.Identificador = this.ObterString(dados[0]);
            this.xOrig = this.ObterString(dados[1]);
            this.xDest = this.ObterString(dados[2]);
            this.xRota = this.ObterString(dados[3]);
        }

        #endregion
    }
}
