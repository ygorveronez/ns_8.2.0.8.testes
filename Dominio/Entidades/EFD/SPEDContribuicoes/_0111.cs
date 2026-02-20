namespace Dominio.Entidades.EFD.SPEDContribuicoes
{
    public class _0111 : Registro
    {
        #region Construtores

        public _0111()
            : base("0111")
        {
        }

        #endregion

        #region Propriedades

        public decimal ReceitaBrutaNaoCumulativa_TributadaNoMercadoInterno { get; set; }

        public decimal ReceitaBrutaNaoCumulativa_NaoTributadaNoMercadoInterno { get; set; }

        public decimal ReceitaBrutaNaoCumulativa_Exportacao { get; set; }

        public decimal ReceitaBrutaCumulativa { get; set; }

        public decimal ReceitaBrutaTotal { get; set; }

        #endregion

        #region Métodos

        public override string ObterDadosParaArquivo()
        {
            this.EscreverDado(this.ReceitaBrutaNaoCumulativa_TributadaNoMercadoInterno); //REC_BRU_NCUM_TRIB_MI
            this.EscreverDado(this.ReceitaBrutaNaoCumulativa_NaoTributadaNoMercadoInterno); //REC_BRU_NCUM_NT_MI
            this.EscreverDado(this.ReceitaBrutaNaoCumulativa_Exportacao); //REC_BRU_NCUM_EXP
            this.EscreverDado(this.ReceitaBrutaCumulativa); //REC_BRU_CUM
            this.EscreverDado(this.ReceitaBrutaTotal); //REC_BRU_TOTAL

            this.FinalizarRegistro();

            this.ObterRegistrosSPEDDerivados();

            return this.RegistroSPED.ToString();
        }

        #endregion
    }
}
