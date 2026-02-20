namespace Dominio.NDDigital.v104.Cancelamento
{
    public class Registro00000:Registro 
    {
        #region Construtores

        public Registro00000(string registro)
            : base(registro)
        {
            this.GerarRegistro();
        }

        #endregion

        #region Propriedades

        public string versao { get; set; }
        public string tpOperacao { get; set; }

        #endregion

        #region Métodos

        protected override void GerarRegistro()
        {
            string[] dados = this.StringRegistro.Split(';');

            this.Identificador = this.ObterString(dados[0]);
            this.versao = this.ObterString(dados[1]);
            this.tpOperacao = this.ObterString(dados[2]);
        }

        #endregion
    }
}
