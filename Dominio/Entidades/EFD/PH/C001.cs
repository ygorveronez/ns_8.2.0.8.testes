using System.Linq;

namespace Dominio.Entidades.EFD.PH
{
    public class C001 : RegistroPH
    {
        #region Construtores

        public C001()
            : base("C001")
        {
        }

        #endregion

        #region Propriedades

        private string IndicadorDeMovimento
        {
            get
            {
                return this.Registros.Count() > 0 ? "0" : "1";
            }
        }

        #endregion

        #region Métodos

        public override string ObterDadosParaArquivo()
        {
            //this.EscreverDado(this.IndicadorDeMovimento);
            //this.FinalizarRegistro();
            this.ObterRegistrosPHDerivados();
            return this.RegistrosPH.ToString();
        }

        #endregion
    }
}
