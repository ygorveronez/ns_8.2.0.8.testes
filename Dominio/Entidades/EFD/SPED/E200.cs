using System;

namespace Dominio.Entidades.EFD.SPED
{
    public class E200 : Registro
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

            this.ObterRegistrosSPEDDerivados();

            return this.RegistroSPED.ToString();
        }

        #endregion
    }
}