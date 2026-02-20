using System.Linq;

namespace Dominio.Entidades.EFD.SPED
{
    public class E001 : Registro
    {
        #region Construtores

        public E001()
            : base("E001")
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
            this.ObterRegistrosSPEDDerivados();
            return this.RegistroSPED.ToString();
        }

        #endregion
    }
}
