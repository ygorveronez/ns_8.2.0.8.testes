namespace Dominio.Entidades.EFD.SPEDContribuicoes
{
    public class _0110 : Registro
    {
        #region Construtores

        public _0110()
            : base("0110")
        {

        }

        #endregion

        #region Propriedades

        public Dominio.Enumeradores.IndicadorDaIncidenciaTributariaNoPeriodo IncidenciaTributariaNoPeriodo { get; set; }

        public Dominio.Enumeradores.IndicadorDeMetodoDeApropriacaoDeCreditosComuns? MetodoDeApropriacaoDeCreditosComuns { get; set; }

        public Dominio.Enumeradores.IndicadorDoTipoDeContribuicaoApuradaNoPeriodo? TipoDeContribuicaoApuradaNoPeriodo { get; set; }

        public Dominio.Enumeradores.IndicadorDoCriterioDeEscrituracaoEApuracaoAdotado? CriterioDeEscrituracaoEApuracao { get; set; }

        #endregion

        #region Metodos

        public override string ObterDadosParaArquivo()
        {
            this.EscreverDado(this.IncidenciaTributariaNoPeriodo.ToString("D"), 1); //COD_INC_TRIB
            this.EscreverDado(this.MetodoDeApropriacaoDeCreditosComuns.HasValue ? this.MetodoDeApropriacaoDeCreditosComuns.Value.ToString("D") : "", 1); //IND_APRO_CRED
            this.EscreverDado(this.TipoDeContribuicaoApuradaNoPeriodo.HasValue ? this.TipoDeContribuicaoApuradaNoPeriodo.Value.ToString("D") : "", 1); //COD_TIPO_CON

            if (this.IncidenciaTributariaNoPeriodo == Enumeradores.IndicadorDaIncidenciaTributariaNoPeriodo.IncidenciaExclusivamenteNoRegimeCumulativo)
                this.EscreverDado(this.CriterioDeEscrituracaoEApuracao.HasValue ? this.CriterioDeEscrituracaoEApuracao.Value.ToString("D") : "", 1); //IND_REG_CUM
            else
                this.EscreverDado(""); // IND_REG_CUM

            this.FinalizarRegistro();

            this.ObterRegistrosSPEDDerivados();

            return this.RegistroSPED.ToString();
        }

        #endregion
    }
}
