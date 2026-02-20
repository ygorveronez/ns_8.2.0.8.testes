namespace Dominio.NDDigital.v104.Emissao
{
    public class Registro50000 : Registro
    {
        #region Construtores

        public Registro50000(string registro)
            : base(registro)
        {
            this.GerarRegistro();
        }

        #endregion

        #region Propriedades

        public Registro50200 infCarga { get; set; }

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
