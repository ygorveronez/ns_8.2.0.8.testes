using System;

namespace Dominio.Entidades.EDI.DOCCOB.v30A
{
    public class CCO : Registro
    {
        #region Construtores

        public CCO(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)
            : base("353")
        {
            this.CTe = cte;

            if (this.CTe == null)
                throw new ArgumentNullException("CTe", "O CT-e não pode ser nulo para gerar um registro CCO.");
        }

        #endregion

        #region Propriedades

        private Dominio.Entidades.ConhecimentoDeTransporteEletronico CTe { get; set; }

        #endregion

        #region Métodos

        public override string ObterDadosParaArquivo()
        {
            this.EscreverDado(this.CTe.Empresa.RazaoSocial, 10);                //2.		FILIAL EMISSORA DO DOCUMENTO
            this.EscreverDado(this.CTe.Serie.Numero, 5);                        //3.		SÉRIE DO CONHECIMENTO
            this.EscreverDado(this.CTe.Numero, 12);                             //4.		NÚMERO DO CONHECIMENTO
            this.EscreverDado(this.CTe.ValorFrete, 13, 2);                      //5.        VALOR DO FRETE
            this.EscreverDado(this.CTe.DataEmissao, "ddMMyyyy");                //6.        DATA DE EMISSÃO DO CONHECIMENTO
            this.EscreverDado(this.CTe.Remetente.CPF_CNPJ_SemFormato, 14);      //7.        CGC DO REMETENTE – EMISSOR DA NF
            this.EscreverDado(this.CTe.Destinatario.CPF_CNPJ_SemFormato, 14);   //8.        CGC DO DESTINATÁRIO DA NF
            this.EscreverDado(this.CTe.Empresa.CNPJ, 14);                       //9.        CGC DO EMISSOR DO CONHECIMENTO
            this.EscreverDado(' ', 75);                                         //10.       FILLER

            this.FinalizarRegistro();

            return this.StringRegistro.ToString();
        }

        #endregion
    }
}
