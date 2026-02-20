namespace Dominio.Entidades.EFD.SPED
{
    public class B001 : Registro
    {
        #region Construtores

        public B001()
            : base("B001")
        {
        }

        #endregion

        #region Propriedades
        
        #endregion

        #region Métodos

        public override string ObterDadosParaArquivo()
        {
            this.EscreverDado("1"); //IND_DAD 
            this.FinalizarRegistro();

            this.ObterRegistrosSPEDDerivados();

            return this.RegistroSPED.ToString();
        }

        #endregion
    }
}
