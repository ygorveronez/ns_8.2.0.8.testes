using System;

namespace Dominio.Entidades.EDI.DOCCOB.v30A
{
    public class UNH : Registro
    {

        #region Construtores

        public UNH()
            : base("350")
        {
        }

        #endregion

        #region Propriedades


        #endregion

        #region Métodos

        public override string ObterDadosParaArquivo()
        {
            this.EscreverDado("CONH");                     //2. IDENTIFICAÇÃO DO DOCUMENTO
            this.EscreverDado(DateTime.Now, "ddMMHHmmss");
            this.EscreverDado(' ', 153);                   //3. FILLER

            this.FinalizarRegistro();

            return this.StringRegistro.ToString();
        }

        #endregion

    }
}
