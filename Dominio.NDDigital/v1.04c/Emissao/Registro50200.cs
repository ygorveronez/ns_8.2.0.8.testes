namespace Dominio.NDDigital.v104.Emissao
{
    public class Registro50200 : Registro
    {
        #region Construtores

        public Registro50200(string registro)
            : base(registro)
        {
            this.GerarRegistro();
        }

        #endregion

        #region Propriedades

        public int numCarga { get; set; }

        public int numUnidade { get; set; }

        #endregion

        #region Métodos

        protected override void GerarRegistro()
        {
            string[] dados = this.StringRegistro.Split(';');

            this.Identificador = this.ObterString(dados[0]);
            this.numUnidade = this.ObterNumero(dados[1]);
            this.numCarga = this.ObterNumero(dados[2]);
        }

        #endregion
    }
}
