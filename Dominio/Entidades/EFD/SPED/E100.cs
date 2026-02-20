using System;

namespace Dominio.Entidades.EFD.SPED
{
    public class E100 : Registro
    {
        #region Construtores

        public E100()
            : base("E100")
        {
        }

        #endregion

        #region Propriedades

        public DateTime DataInicial { get; set; }

        public DateTime DataFinal { get; set; }
        
        #endregion

        #region Métodos

        public override string ObterDadosParaArquivo()
        {
            this.EscreverDado(this.DataInicial); //DT_INI
            this.EscreverDado(this.DataFinal); //DT_FIN

            this.FinalizarRegistro();

            this.ObterRegistrosSPEDDerivados();

            return this.RegistroSPED.ToString();
        }

        #endregion
    }
}
