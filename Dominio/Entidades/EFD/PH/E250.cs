using System;

namespace Dominio.Entidades.EFD.PH
{
    public class E250 : RegistroPH
    {
        #region Construtores

        public E250()
            : base("E250")
        {
        }

        #endregion

        #region Propriedades

        /// <summary>
        /// Código da obrigação a recolher
        /// </summary>
        public string CodigoObrigacaoRecolher { get; set; }

        /// <summary>
        /// Valor da obrigação ICMS ST a recolher
        /// </summary>
        public decimal ValorObrigacaoRecolher { get; set; }

        /// <summary>
        /// Data de vencimento da obrigação
        /// </summary>
        public DateTime DataObrigacaoRecolher { get; set; }

        /// <summary>
        /// Código de receita referente à obrigação, próprio da unidade da federação do contribuinte substituído.
        /// </summary>
        public int CodigoReceitaObrigacao { get; set; }

        /// <summary>
        /// Número do processo ou auto de infração ao qual a obrigação está vinculada, se houver
        /// </summary>
        public string NumeroProcesso { get; set; }

        /// <summary>
        /// Indicador da origem do processo: 
        /// 0- SEFAZ;
        /// 1- Justiça Federal;
        /// 2- Justiça Estadual;
        /// 9- Outros
        /// </summary>
        public string IndicadorOrigemProcesso { get; set; }

        /// <summary>
        /// Descrição resumida do processo que embasou o lançamento
        /// </summary>
        public string DescricaoResumidaProcesso { get; set; }

        /// <summary>
        /// Descrição complementar das obrigações a recolher
        /// </summary>
        public string DescricaoComplementarObrigacoes { get; set; }

        /// <summary>
        /// Informe o mês de referência no formato “mmaaaa” 
        /// </summary>
        public string MesReferencia { get; set; }

        #endregion

        #region Métodos

        public override string ObterDadosParaArquivo()
        {
            this.EscreverDado(this.CodigoObrigacaoRecolher); //COD_OR
            this.EscreverDado(this.ValorObrigacaoRecolher); //VL_OR
            this.EscreverDado(this.DataObrigacaoRecolher); //DT_VCTO
            this.EscreverDado(this.CodigoReceitaObrigacao); //COD_REC
            this.EscreverDado(this.NumeroProcesso); //NUM_PROC
            this.EscreverDado(this.IndicadorOrigemProcesso); //IND_PROC
            this.EscreverDado(this.DescricaoResumidaProcesso); //PROC
            this.EscreverDado(this.DescricaoComplementarObrigacoes); //TXT_COMPL
            this.EscreverDado(this.MesReferencia); //MES_REF

            this.FinalizarRegistro();

            this.ObterRegistrosPHDerivados();

            return this.RegistrosPH.ToString();
        }

        #endregion
    }
}
