using System;

namespace Dominio.Entidades.EDI.DOCCOB.v30A
{
    public class UNB : Registro
    {

        #region Construtores

        public UNB(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)
            : base("000")
        {
            this.CTe = cte;

            if (this.CTe == null)
                throw new ArgumentNullException("cte", "O CT-e não pode ser nulo para gerar um registro UNB.");
        }

        #endregion

        #region Propriedades

        private Dominio.Entidades.ConhecimentoDeTransporteEletronico CTe { get; set; }

        #endregion

        #region Métodos

        public override string ObterDadosParaArquivo()
        {
            this.EscreverDado(this.CTe.Empresa.RazaoSocial, 35);//2. IDENTIFICAÇÃO DO REMETENTE
            this.EscreverDado(this.CTe.Remetente.Nome, 35);     //3. IDENTIFICAÇÃO DO DESTINATÁRIO
            this.EscreverDado(DateTime.Now, "ddMMyy");          //4. DATA
            this.EscreverDado(DateTime.Now, "HHmm");            //5. HORA
            this.EscreverDado("CO");                           //6. IDENTIFICAÇÃO DO INTERCÂMBIO
            this.EscreverDado(DateTime.Now, "ddMMHHmmss");
            this.EscreverDado(' ', 75);                        //7. FILLER

            this.FinalizarRegistro();

            return this.StringRegistro.ToString();
        }

        #endregion

    }
}
