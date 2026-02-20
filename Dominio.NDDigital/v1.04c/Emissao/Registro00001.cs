namespace Dominio.NDDigital.v104.Emissao
{
    public class Registro00001 : Registro
    {
        #region Construtores

        public Registro00001(string registro)
            : base(registro)
        {
            this.GerarRegistro();
        }

        #endregion

        #region Propriedades

        public string idLote { get; set; }

        #endregion

        #region Métodos

        protected override void GerarRegistro()
        {
            string[] dados = this.StringRegistro.Split(';');

            this.Identificador = this.ObterString(dados[0]);
            this.idLote = this.ObterString(dados[1]);
        }

        #endregion
    }
}