namespace Dominio.NDDigital.v104.Emissao
{
    public class Registro11311 : Registro
    {
        #region Construtores

        public Registro11311(string registro)
            : base(registro)
        {
            this.GerarRegistro();
        }

        #endregion

        #region Propriedades

        /// <summary>
        /// Sigla ou código interno da Filial/Porto/Estação/Aeroporto de Passagem
        /// </summary>
        public string xPass { get; set; }

        #endregion

        #region Métodos

        protected override void GerarRegistro()
        {
            string[] dados = this.StringRegistro.Split(';');

            this.Identificador = this.ObterString(dados[0]);
            this.xPass = this.ObterString(dados[1]);
        }

        #endregion
    }
}
