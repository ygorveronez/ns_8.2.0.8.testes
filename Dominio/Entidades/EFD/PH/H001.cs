using System.Linq;

namespace Dominio.Entidades.EFD.PH
{
    public class H001 : RegistroPH
    {
        #region Construtores

        public H001()
            : base("H001")
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
            this.EscreverDado(this.IndicadorDeMovimento);
            this.FinalizarRegistro();
            this.ObterRegistrosPHDerivados();
            return this.RegistrosPH.ToString();
        }

        #endregion
    }
}
