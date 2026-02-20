using System;

namespace Dominio.Entidades.EDI.DOCCOB.v30A
{
    public class DCO:Registro
    {
        #region Construtores

        public DCO(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, decimal valorTotal, decimal valorICMS)
            : base("352")
        {
            this.CTe = cte;
            this.ValorICMS = valorICMS;
            this.ValorTotal = valorTotal;

            if (this.CTe == null)
                throw new ArgumentNullException("CTe", "O CT-e não pode ser nulo para gerar um registro DCO.");
        }

        #endregion

        #region Propriedades

        private Dominio.Entidades.ConhecimentoDeTransporteEletronico CTe { get; set; }

        private decimal ValorICMS { get; set; }

        private decimal ValorTotal { get; set; }

        #endregion

        #region Métodos

        public override string ObterDadosParaArquivo()
        {
            this.EscreverDado(this.CTe.Empresa.RazaoSocial, 10);        //2.		FILIAL EMISSORA DO DOCUMENTO
            this.EscreverDado('0', 1);                                  //3.		TIPO DO DOCUMENTO DE COBRANÇA
            this.EscreverDado(' ', 3);                                  //4.		SÉRIE DO DOCUMENTO DE COBRANÇA
            this.EscreverDado(this.CTe.Numero, 10);                     //5.        NÚMERO DO DOCUMENTO DE COBRANÇA
            this.EscreverDado(this.CTe.DataEmissao, "ddMMyyyy");        //6.        DATA DE EMISSÃO
            this.EscreverDado(this.CTe.DataEmissao, "ddMMyyyy");        //7.        DATA DE VENCIMENTO
            this.EscreverDado(this.ValorTotal, 13, 2);                  //8.        VALOR DO DOCUMENTO DE COBRANÇA
            this.EscreverDado(' ', 3);                                  //9.        TIPO DE COBRANÇA
            this.EscreverDado(this.ValorICMS, 13, 2);                   //10.       VALOR TOTAL DO ICMS
            this.EscreverDado(' ', 15);                                 //11.       VALOR – JUROS POR DIA DE ATRASO
            this.EscreverDado(' ', 8);                                  //12.       DATA LIMITE P/ PAGTO C/ DESCONTO
            this.EscreverDado(' ', 15);                                 //13.       VALOR DO DESCONTO
            this.EscreverDado(' ', 35);                                 //14.       IDENTIFICAÇÃO DO AGENTE DE COBRANÇA
            this.EscreverDado(' ', 4);                                  //15.       NÚMERO DA AGÊNCIA BANCÁRIA
            this.EscreverDado(' ', 1);                                  //16.       DÍGITO VERIFICADOR NUM. DA AGÊNCIA
            this.EscreverDado(' ', 10);                                 //17.       NÚMERO DA CONTA CORRENTE
            this.EscreverDado(' ', 2);                                  //18.       DÍGITO VERIFICADOR CONTA CORRENTE
            this.EscreverDado('I', 1);                                  //19.       AÇÃO DO DOCUMENTO
            this.EscreverDado(' ', 3);                                  //20.       FILLER

            this.FinalizarRegistro();

            return this.StringRegistro.ToString();
        }

        #endregion
    }
}
