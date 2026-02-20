using System;

namespace Dominio.Entidades.EFD.SPEDContribuicoes
{
    public class D200 : Registro
    {
        #region Construtores

        public D200() : base("D200") { }

        #endregion

        #region Propriedades

        public string Modelo { get; set; }

        public string Situacao { get; set; }

        public string Serie { get; set; }

        public int NumeroInicial { get; set; }

        public int NumeroFinal { get; set; }

        public int CFOP { get; set; }

        public DateTime DataReferencia { get; set; }

        public decimal ValorTotalDocumentos { get; set; }

        public decimal ValorTotalDescontos { get; set; }


        #endregion

        #region Métodos

        public override string ObterDadosParaArquivo()
        {
            this.EscreverDado(this.Modelo, 2); //COD_MOD
            this.EscreverDado(this.Situacao, 2); //COD_SIT
            this.EscreverDado(this.Serie, 4); //SER
            this.EscreverDado(""); //SUB
            this.EscreverDado(this.NumeroInicial, 9); //NUM_DOC_INI
            this.EscreverDado(this.NumeroFinal, 9); //NUM_DOC_FIN
            this.EscreverDado(this.CFOP, 4); //CFOP
            this.EscreverDado(this.DataReferencia); //DT_REF
            this.EscreverDado(this.ValorTotalDocumentos); //VL_DOC
            this.EscreverDado(this.ValorTotalDescontos); //VL_DESC

            this.FinalizarRegistro();

            this.ObterRegistrosSPEDDerivados();

            return this.RegistroSPED.ToString();
        }
        
        #endregion

    }
}
