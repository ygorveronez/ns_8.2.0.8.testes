namespace Dominio.Entidades.EFD.PH
{
    public class B001 : RegistroPH
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

            this.ObterRegistrosPHDerivados();

            return this.RegistrosPH.ToString();
        }

        #endregion
    }
}
