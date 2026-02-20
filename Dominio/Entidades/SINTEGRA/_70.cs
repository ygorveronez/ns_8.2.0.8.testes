namespace Dominio.Entidades.SINTEGRA
{
    public class _70 : Registro
    {
        #region Construtores

        public _70()
            : base("70")
        {
        }

        #endregion

        #region Propriedades

        public Dominio.Entidades.ConhecimentoDeTransporteEletronico CTe { get; set; }

        #endregion

        #region Métodos

        public override string ObterDadosParaArquivo()
        {
            this.EscreverDado(!string.IsNullOrWhiteSpace(this.CTe.Tomador.CPF_CNPJ_SemFormato) ? long.Parse(this.CTe.Tomador.CPF_CNPJ_SemFormato) : 0, 14); //CNPJ
            this.EscreverDado(this.CTe.Tomador.IE_RG, 14); //Inscrição Estadual
            this.EscreverDado(this.CTe.DataEmissao); //Data de emissão / utilização
            this.EscreverDado(this.CTe.Tomador.Localidade?.Estado?.Sigla ?? string.Empty, 2); //Unidade da Federação
            this.EscreverDado(this.CTe.ModeloDocumentoFiscal.Numero, 2); //Modelo
            this.EscreverDado(this.CTe.Serie.Numero.ToString(), 1); //Série
            this.EscreverDado("", 2); //Subsérie
            this.EscreverDado(this.CTe.Numero, 6); //Número
            this.EscreverDado(this.CTe.CFOP.CodigoCFOP, 4); //CFOP
            this.EscreverDado(this.CTe.ValorAReceber, 11, 2); //Valor total do documento fiscal
            this.EscreverDado(this.CTe.BaseCalculoICMS, 12, 2); //Base de Cálculo do ICMS
            this.EscreverDado(this.CTe.ValorICMS, 12, 2); //Valor do ICMS
            this.EscreverDado(0m, 12, 2); //Isenta ou não-tributada
            this.EscreverDado(this.CTe.ValorAReceber - this.CTe.BaseCalculoICMS, 12, 2); //Outras

            if (this.CTe.TipoPagamento == Enumeradores.TipoPagamento.Pago)
                this.EscreverDado("1", 1); //CIF
            else if (this.CTe.TipoPagamento == Enumeradores.TipoPagamento.A_Pagar)
                this.EscreverDado("2", 1); //FOB
            else
                this.EscreverDado("0", 1); //OUTROS

            if (this.CTe.Status == "A")
                this.EscreverDado("N", 1); //Situação
            else
                this.EscreverDado("S", 1); //Situação

            this.FinalizarRegistro();

            this.ObterRegistrosSINTEGRADerivados();

            return this.RegistroSINTEGRA.ToString();
        }

        #endregion
    }
}
