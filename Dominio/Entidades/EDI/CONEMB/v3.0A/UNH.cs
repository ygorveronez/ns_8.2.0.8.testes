using System;

namespace Dominio.Entidades.EDI.CONEMB.v30A
{
    public class UNH : Registro
    {

        #region Construtores

        public UNH()
            : base("320")
        {
        }

        #endregion

        #region Propriedades


        #endregion

        #region Métodos

        public override string ObterDadosParaArquivo()
        {
            this.EscreverDado("CONH");                    //2. IDENTIFICAÇÃO DO DOCUMENTO
            this.EscreverDado(DateTime.Now, "ddMMHHmmss");
            this.EscreverDado(' ', 663);                   //3. FILLER

            this.FinalizarRegistro();

            return this.StringRegistro.ToString();
        }

        #endregion

    }
}
