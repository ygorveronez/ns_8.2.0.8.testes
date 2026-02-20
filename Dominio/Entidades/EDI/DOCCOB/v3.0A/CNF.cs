using System;

namespace Dominio.Entidades.EDI.DOCCOB.v30A
{
    public class CNF: Registro
    {
        #region Construtores

        public CNF(Dominio.Entidades.DocumentosCTE notaFiscal)
            : base("354")
        {
            this.NotaFiscal = notaFiscal;

            if (this.NotaFiscal == null)
                throw new ArgumentNullException("CTe", "A nota fiscal não pode ser nula para gerar um registro CNF.");
        }

        #endregion

        #region Propriedades

        private Dominio.Entidades.DocumentosCTE NotaFiscal { get; set; }

        #endregion

        #region Métodos

        public override string ObterDadosParaArquivo()
        {
                //2.        SÉRIE    
                if (!string.IsNullOrWhiteSpace(this.NotaFiscal.Serie))
                    this.EscreverDado(this.NotaFiscal.Serie, 3);
                else if (!string.IsNullOrWhiteSpace(this.NotaFiscal.ChaveNFE))
                    this.EscreverDado(this.NotaFiscal.ChaveNFE.Substring(23, 2), 3);
                else
                    this.EscreverDado(' ', 3);

                this.EscreverDado(this.NotaFiscal.Numero, 8);                                  //3.		NÚMERO DA NOTA FISCAL
                this.EscreverDado(this.NotaFiscal.DataEmissao, "ddMMyyyy");                    //4.       DATA DE EMISSÃO DA NOTA FISCAL
                this.EscreverDado(this.NotaFiscal.Peso, 5, 2);                                 //5.       PESO DA NOTA FISCAL
                this.EscreverDado(this.NotaFiscal.ValorProdutos, 13, 2);                       //6.       VALOR DA MERCADORIA NA NOTA FISCAL
                this.EscreverDado(this.NotaFiscal.CTE.Remetente.CPF_CNPJ_SemFormato, 14);      //7.       CGC DO REMETENTE – EMISSOR DA NF
                this.EscreverDado(' ', 112);                                                   //8.       FILLER
                
                this.FinalizarRegistro();

            return this.StringRegistro.ToString();
        }

        #endregion
    }
}
