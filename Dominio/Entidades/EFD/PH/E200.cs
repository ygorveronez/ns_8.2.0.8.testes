using System;

namespace Dominio.Entidades.EFD.PH
{
    public class E200 : RegistroPH
    {
        #region Construtores

        public E200()
            : base("E200")
        {
        }

        #endregion

        #region Propriedades

        public Empresa Empresa { get; set; }

        public DateTime DataInicial { get; set; }

        public DateTime DataFinal { get; set; }

        #endregion

        #region Métodos

        public override string ObterDadosParaArquivo()
        {
            this.EscreverDado(this.Empresa.Localidade.Estado.Sigla); //UF
            this.EscreverDado(this.DataInicial); //DT_INI
            this.EscreverDado(this.DataFinal); //DT_FIN

            this.FinalizarRegistro();

            this.ObterRegistrosPHDerivados();

            return this.RegistrosPH.ToString();
        }

        #endregion
    }
}