namespace Dominio.Entidades.SINTEGRA
{
    public class _50 : Registro
    {
        #region Construtores

        public _50()
            : base("50")
        {
        }

        #endregion

        #region Propriedades

        public Dominio.Entidades.DocumentoEntrada DocumentoEntrada { get; set; }

        public int CFOP { get; set; }

        public decimal Aliquota { get; set; }

        public decimal ValorTotal { get; set; }

        public decimal BaseCalculoICMS { get; set; }

        public decimal ValorTotalICMS { get; set; }

        public decimal ValorOutros { get; set; }

        #endregion

        #region Métodos

        public override string ObterDadosParaArquivo()
        {
            this.EscreverDado(this.DocumentoEntrada.Fornecedor.CPF_CNPJ_SemFormato, 14); //CNPJ
            this.EscreverDado(this.DocumentoEntrada.Fornecedor.IE_RG, 14); //Inscrição Estadual
            this.EscreverDado(this.DocumentoEntrada.DataEmissao); //Data de emissão ou recebimento
            this.EscreverDado(this.DocumentoEntrada.Fornecedor.Localidade.Estado.Sigla, 2); //Unidade da Federação
            this.EscreverDado(this.DocumentoEntrada.Modelo.Numero, 2); //Modelo
            this.EscreverDado(this.DocumentoEntrada.Serie, 3); //Série
            this.EscreverDado(this.DocumentoEntrada.Numero, 6); //Número
            this.EscreverDado(this.CFOP, 4); //CFOP
            this.EscreverDado("T", 1); //Emitente
            this.EscreverDado(this.ValorTotal, 11, 2); //Valor Total
            this.EscreverDado(this.BaseCalculoICMS, 11, 2); //Base de Cálculo do ICMS
            this.EscreverDado(this.ValorTotalICMS, 11, 2); //Valor do ICMS
            this.EscreverDado(0m, 11, 2); //Isenta ou não-tributada
            this.EscreverDado(this.ValorOutros, 11, 2); //Outras
            this.EscreverDado(this.Aliquota, 2, 2); //Alíquota
            this.EscreverDado("N", 1); //Situação
            
            this.FinalizarRegistro();

            this.ObterRegistrosSINTEGRADerivados();

            return this.RegistroSINTEGRA.ToString();
        }

        #endregion
    }
}
