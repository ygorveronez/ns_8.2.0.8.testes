using System;

namespace Dominio.Entidades.EDI.CONEMB.v30A
{
    public class DCC: Registro
    {
        #region Construtores

        public DCC(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)
            : base("329")
        {
            this.CTe = cte;

            if (this.CTe == null)
                throw new ArgumentNullException("cte", "O CT-e não pode ser nulo para gerar um registro DCC.");
        }

        #endregion

        #region Propriedades

        private Dominio.Entidades.ConhecimentoDeTransporteEletronico CTe { get; set; }

        #endregion

        #region Métodos

        public override string ObterDadosParaArquivo()
        {
            this.EscreverDado(' ', 14);                //2. CONSIGNATARIO
            this.EscreverDado(this.CTe.Chave, 44);     //3. CHAVE ACESSO CT-e
            this.EscreverDado(' ' , 2);                //4. TIPO NC
            this.EscreverDado(' ', 617);               //5. FILLER

            this.FinalizarRegistro();

            return this.StringRegistro.ToString();
        }

        #endregion
    }
}
