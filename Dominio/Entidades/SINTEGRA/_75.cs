using System;

namespace Dominio.Entidades.SINTEGRA
{
    public class _75 : Registro
    {
        #region Construtores

        public _75()
            : base("75")
        {
        }

        #endregion

        #region Propriedades

        public Dominio.Entidades.Produto Produto { get; set; }

        public DateTime DataInicial { get; set; }

        public DateTime DataFinal { get; set; }

        public string CST { get; set; }

        #endregion

        #region Métodos

        public override string ObterDadosParaArquivo()
        {
            this.EscreverDado(this.DataInicial); //Data Inicial
            this.EscreverDado(this.DataFinal); //Data Final
            if (this.Produto.CodigoProduto != null && this.Produto.CodigoProduto != "")
                this.EscreverDado(this.Produto.CodigoProduto, 14); //Código do Produto ou Serviço
            else
                this.EscreverDado(this.Produto.Codigo.ToString(), 14); //Código do Produto ou Serviço
            this.EscreverDado(this.Produto.NCM.Numero, 8); //Código NCM 
            this.EscreverDado(this.Produto.Descricao, 53); //Descrição
            this.EscreverDado(this.Produto.UnidadeMedida.Sigla, 6); //Unidade de Medida de Comercialização
            this.EscreverDado(this.CST, 3); //Situação Tributária 
            this.EscreverDado(0m, 2, 2); //Alíquota do IPI
            this.EscreverDado(0m, 2, 2); //Alíquota do ICMS
            this.EscreverDado(0m, 2, 2); //Redução da Base de Cálculo do ICMS
            this.EscreverDado(0m, 10, 2); //Base de Cálculo do ICMS de Substituição Tributária

            this.FinalizarRegistro();

            this.ObterRegistrosSINTEGRADerivados();

            return this.RegistroSINTEGRA.ToString();
        }

        #endregion
    }
}
