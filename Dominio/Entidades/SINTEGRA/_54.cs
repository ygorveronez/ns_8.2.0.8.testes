namespace Dominio.Entidades.SINTEGRA
{
    public class _54 : Registro
    {
        #region Construtores

        public _54()
            : base("54")
        {
        }

        #endregion

        #region Propriedades

        public Dominio.Entidades.ItemDocumentoEntrada Item { get; set; }

        #endregion

        #region Métodos

        public override string ObterDadosParaArquivo()
        {
            this.EscreverDado(this.Item.DocumentoEntrada.Fornecedor.CPF_CNPJ_SemFormato, 14); //CNPJ
            this.EscreverDado(this.Item.DocumentoEntrada.Modelo.Numero, 2); //Modelo
            this.EscreverDado(this.Item.DocumentoEntrada.Serie, 3); //Série
            this.EscreverDado(this.Item.DocumentoEntrada.Numero, 6); //Número
            this.EscreverDado(this.Item.CFOP.CodigoCFOP, 4); //CFOP
            this.EscreverDado(this.Item.CST, 3); //CST
            this.EscreverDado(this.Item.Sequencial, 3); //Número do Item
            if (this.Item.Produto.CodigoProduto != null && this.Item.Produto.CodigoProduto != "")
                this.EscreverDado(this.Item.Produto.CodigoProduto, 14); //Código do Produto ou Serviço
            else
                this.EscreverDado(this.Item.Produto.Codigo.ToString(), 14); //Código do Produto ou Serviço
            this.EscreverDado(this.Item.Quantidade, 8, 3); //Quantidade
            this.EscreverDado(this.Item.ValorTotal, 10, 2); //Valor do Produto
            this.EscreverDado(this.Item.Desconto, 10, 2); //Valor do Desconto / Despesa Acessória 
            this.EscreverDado(this.Item.ValorICMS > 0 ? (this.Item.ValorICMS * 100) / this.Item.AliquotaICMS : 0m, 10, 2); //Base de Cálculo do ICMS
            this.EscreverDado(0m, 10, 2); //Base de Cálculo do ICMS para Substituição Tributária
            this.EscreverDado(this.Item.ValorIPI, 10, 2); //Valor do IPI
            this.EscreverDado(this.Item.AliquotaICMS, 2, 2); //Alíquota do ICMS

            this.FinalizarRegistro();

            this.ObterRegistrosSINTEGRADerivados();

            return this.RegistroSINTEGRA.ToString();
        }

        #endregion
    }
}
