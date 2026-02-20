using System;

namespace Dominio.Entidades.EFD.PH
{
    public class E100 : RegistroPH
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

            this.ObterRegistrosPHDerivados();

            return this.RegistrosPH.ToString();
        }

        #endregion
    }
}
