using System;

namespace Dominio.Entidades.SINTEGRA
{
    public class _10 : Registro
    {
        #region Construtores

        public _10()
            : base("10")
        {
        }

        #endregion

        #region Propriedades

        public Empresa Empresa { get; set; }

        public DateTime DataInicial { get; set; }

        public DateTime DataFinal { get; set; }

        public string CodigoEstruturaArquivo { get; set; }

        public string CodigoNaturezaOperacoes { get; set; }

        public string CodigoFinalidadeArquivo { get; set; }

        #endregion

        #region Métodos

        public override string ObterDadosParaArquivo()
        {
            this.EscreverDado(this.Empresa.CNPJ, 14); //CGC/MF
            this.EscreverDado(this.Empresa.InscricaoEstadual, 14); //Inscrição Estadual
            this.EscreverDado(this.Empresa.RazaoSocial, 35); //Nome do Contribuinte
            this.EscreverDado(this.Empresa.Localidade.Descricao, 30); //Município
            this.EscreverDado(this.Empresa.Localidade.Estado.Sigla, 2); //Unidade da Federação
            this.EscreverDado(0, 10); //Fax
            this.EscreverDado(this.DataInicial); //Data Inicial
            this.EscreverDado(this.DataFinal); //Data Final
            this.EscreverDado(this.CodigoEstruturaArquivo, 1); //Código da identificação da estrutura do arquivo magnético entregue
            this.EscreverDado(this.CodigoNaturezaOperacoes, 1); //Código da identificação da natureza das operações informadas
            this.EscreverDado(this.CodigoFinalidadeArquivo, 1); //Código da finalidade do arquivo magnético 

            this.FinalizarRegistro();

            this.ObterRegistrosSINTEGRADerivados();

            return this.RegistroSINTEGRA.ToString();
        }

        #endregion
    }
}
