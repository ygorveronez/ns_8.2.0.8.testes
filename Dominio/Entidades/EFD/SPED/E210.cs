namespace Dominio.Entidades.EFD.SPED
{
    public class E210 : Registro
    {
        #region Construtores

        public E210()
            : base("E210")
        {
        }

        #endregion

        #region Propriedades

        /// <summary>
        /// Indicador de movimento:
        /// 0 – Sem operações com ST
        /// 1 – Com operações de ST
        /// </summary>
        public bool IndicadorDeMovimento { get; set; }

        /// <summary>
        /// Valor do "Saldo credor de período anterior – Substituição Tributária"
        /// </summary>
        public decimal ValorSaldoCredor { get; set; }

        /// <summary>
        /// Valor total do ICMS ST de devolução de mercadorias
        /// </summary>
        public decimal ValorTotalDevolucao { get; set; }

        /// <summary>
        /// Valor total do ICMS ST de ressarcimentos
        /// </summary>
        public decimal ValorTotalRessarcimentos { get; set; }

        /// <summary>
        /// Valor total de Ajustes "Outros créditos ST" e “Estorno de débitos ST”
        /// </summary>
        public decimal ValorAjustesEstornosCreditos { get; set; }

        /// <summary>
        /// Valor total dos ajustes a crédito de ICMS ST, provenientes de ajustes do documento fiscal.
        /// </summary>
        public decimal ValorTotalAjustesCreditos { get; set; }

        /// <summary>
        /// Valor Total do ICMS retido por Substituição Tributária
        /// </summary>
        public decimal ValorTotalICMSRetido { get; set; }

        /// <summary>
        /// Valor Total dos ajustes "Outros débitos ST" " e “Estorno de créditos ST”
        /// </summary>
        public decimal ValorAjustesEstornosDebitos { get; set; }

        /// <summary>
        /// Valor total dos ajustes a débito de ICMS ST, provenientes de ajustes do documento fiscal
        /// </summary>
        public decimal ValorTotalAjustesDebitos { get; set; }

        /// <summary>
        /// Valor total de Saldo devedor antes das deduções
        /// </summary>
        public decimal ValorSaldoCredorAnterior { get; set; }

        /// <summary>
        /// Valor total dos ajustes "Deduções ST" 
        /// </summary>
        public decimal ValorSaldoAjustesDeducao { get; set; }

        /// <summary>
        /// Imposto a recolher ST (ValorSaldoCredorAnterior - ValorSaldoAjustesDeducao)
        /// </summary>
        public decimal ImpostoARecolher { get; set; }

        /// <summary>
        /// Saldo credor de ST a transportar para o período seguinte 
        /// (ValorSaldoCredor + ValorTotalDevolucao + ValorTotalRessarcimentos + ValorAjustesEstornosCreditos + ValorTotalAjustesCreditos) – (ValorTotalICMSRetido + ValorAjustesEstornosDebitos + ValorTotalAjustesDebitos)
        /// </summary>
        public decimal ValorSaldoCredorTransportar { get; set; }

        /// <summary>
        /// Valores recolhidos ou a recolher, extraapuração.
        /// </summary>
        public decimal ValorRecolhidoExtraApuracao { get; set; }

        #endregion

        #region Métodos

        public override string ObterDadosParaArquivo()
        {
            this.EscreverDado(this.IndicadorDeMovimento ? "1" : "0"); //IND_MOV_ST
            this.EscreverDado(this.ValorSaldoCredor); //VL_SLD_CRED_ANT_ST
            this.EscreverDado(this.ValorTotalDevolucao); //VL_DEVOL_ST
            this.EscreverDado(this.ValorTotalRessarcimentos); //VL_RESSARC_ST
            this.EscreverDado(this.ValorAjustesEstornosCreditos); //VL_OUT_CRED_ST
            this.EscreverDado(this.ValorTotalAjustesCreditos); //VL_AJ_CREDITOS_ST
            this.EscreverDado(this.ValorTotalICMSRetido); //VL_RETENÇAO_ST
            this.EscreverDado(this.ValorAjustesEstornosDebitos); //VL_OUT_DEB_ST
            this.EscreverDado(this.ValorTotalAjustesDebitos); //VL_AJ_DEBITOS_ST
            this.EscreverDado(this.ValorSaldoCredorAnterior); //VL_SLD_DEV_ANT_ST
            this.EscreverDado(this.ValorSaldoAjustesDeducao); //VL_DEDUÇÕES_ST
            this.EscreverDado(this.ImpostoARecolher); //VL_ICMS_RECOL_ST
            this.EscreverDado(this.ValorSaldoCredorTransportar); //VL_SLD_CRED_ST_TRANSPORTAR
            this.EscreverDado(this.ValorRecolhidoExtraApuracao); //DEB_ESP_ST

            this.FinalizarRegistro();

            this.ObterRegistrosSPEDDerivados();

            return this.RegistroSPED.ToString();
        }

        #endregion
    }
}
