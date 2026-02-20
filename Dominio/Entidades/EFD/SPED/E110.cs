namespace Dominio.Entidades.EFD.SPED
{
    public class E110 : Registro
    {
        #region Construtores

        public E110()
            : base("E110")
        {
        }

        #endregion

        #region Propriedades

        /// <summary>
        /// Valor total dos débitos por "Saídas e prestações com débito do imposto"
        /// </summary>
        public decimal ValorTotalDebitos { get; set; }

        /// <summary>
        /// Valor total dos ajustes a débito decorrentes do documento fiscal.
        /// </summary>
        public decimal ValorAjustesDebitos { get; set; }

        /// <summary>
        /// Valor total de "Ajustes a débito"
        /// </summary>
        public decimal ValorTotalAjustesDebitos { get; set; }

        /// <summary>
        /// Valor total de Ajustes “Estornos de créditos”
        /// </summary>
        public decimal ValorEstornosCreditos { get; set; }
        
        /// <summary>
        /// Valor total dos créditos por "Entradas e aquisições com crédito do imposto"
        /// </summary>
        public decimal ValorTotalCreditos { get; set; }
        
        /// <summary>
        /// Valor total dos ajustes a crédito decorrentes do documento fiscal.
        /// </summary>
        public decimal ValorAjustesCreditos { get; set; }

        /// <summary>
        /// Valor total de "Ajustes a crédito"
        /// </summary>
        public decimal ValorTotalAjustesCreditos { get; set; }

        /// <summary>
        /// Valor total de Ajustes “Estornos de Débitos”
        /// </summary>
        public decimal ValorEstornosDebitos { get; set; }

        /// <summary>
        /// Valor total de "Saldo credor do período anterior"
        /// </summary>
        public decimal ValorSaldoCredorAnterior { get; set; }

        /// <summary>
        /// Valor do saldo devedor apurado
        /// </summary>
        public decimal ValorSaldoDevedorApurado { get; set; }

        /// <summary>
        /// Valor total de "Deduções"
        /// </summary>
        public decimal ValorTotalDeducoes { get; set; }

        /// <summary>
        /// Valor total de "ICMS a recolher (11-12)
        /// </summary>
        public decimal ValorICMSRecolher { get; set; }

        /// <summary>
        /// Valor total de "Saldo credor a transportar para o período seguinte”
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
            this.EscreverDado(this.ValorTotalDebitos); //VL_TOT_DEBITOS
            this.EscreverDado(this.ValorAjustesDebitos); //VL_AJ_DEBITOS
            this.EscreverDado(this.ValorTotalAjustesDebitos); //VL_TOT_AJ_DEBITOS
            this.EscreverDado(this.ValorEstornosCreditos); //VL_ESTORNOS_CRED
            this.EscreverDado(this.ValorTotalCreditos); //VL_TOT_CREDITOS
            this.EscreverDado(this.ValorAjustesCreditos); //VL_AJ_CREDITOS
            this.EscreverDado(this.ValorTotalAjustesCreditos); //VL_TOT_AJ_CREDITOS
            this.EscreverDado(this.ValorEstornosDebitos); //VL_ESTORNOS_DEB
            this.EscreverDado(this.ValorSaldoCredorAnterior); //VL_SLD_CREDOR_ANT
            this.EscreverDado(this.ValorSaldoDevedorApurado); //VL_SLD_APURADO
            this.EscreverDado(this.ValorTotalDeducoes); //VL_TOT_DED
            this.EscreverDado(this.ValorICMSRecolher); //VL_ICMS_RECOLHER
            this.EscreverDado(this.ValorSaldoCredorTransportar); //VL_SLD_CREDOR_TRANSPORTAR
            this.EscreverDado(this.ValorRecolhidoExtraApuracao); //DEB_ESP

            this.FinalizarRegistro();

            this.ObterRegistrosSPEDDerivados();

            return this.RegistroSPED.ToString();
        }

        #endregion
    }
}
