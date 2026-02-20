namespace Dominio.NDDigital.v104.Emissao
{
    public class Registro10000: Registro
    {
        #region Construtores

        public Registro10000(string registro)
            : base(registro)
        {
            this.GerarRegistro();
        }

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
