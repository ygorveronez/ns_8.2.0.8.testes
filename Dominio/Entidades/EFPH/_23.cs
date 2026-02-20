namespace Dominio.Entidades.EFPH
{
    public class _23 : Registro
    {
        #region Construtores

        public _23()
            : base("23")
        {
        }

        #endregion

        #region Propriedades

        public Dominio.Entidades.ConhecimentoDeTransporteEletronico CTe { get; set; }

        #endregion

        #region Métodos

        public override string ObterDadosParaArquivo()
        {
            this.EscreverDado(this.CTe.BaseCalculoICMS, 10, 2); //Base de Cálculo 1
            this.EscreverDado(this.CTe.AliquotaICMS, 2, 2); //Alíquota 1
            this.EscreverDado(this.CTe.ValorICMS, 10, 2); //Valor ICMS 1
            this.EscreverDado(0m, 10, 2); //Base de Cálculo 2
            this.EscreverDado(0m, 2, 2); //Alíquota 2
            this.EscreverDado(0m, 10, 2); //Valor ICMS 2
            this.EscreverDado(0m, 10, 2); //Base de Cálculo 3
            this.EscreverDado(0m, 2, 2); //Alíquota 3
            this.EscreverDado(0m, 10, 2); //Valor ICMS 3
            this.EscreverDado(0m, 10, 2); //Base de Cálculo 4
            this.EscreverDado(0m, 2, 2); //Alíquota 4
            this.EscreverDado(0m, 10, 2); //Valor ICMS 4
            this.EscreverDado(0m, 10, 2); //Base de Cálculo 5
            this.EscreverDado(0m, 2, 2); //Alíquota 5
            this.EscreverDado(0m, 10, 2); //Valor ICMS 5
            this.EscreverDado(0m, 10, 2); //Isentas
            this.EscreverDado(0m, 10, 2); //Outras
            this.EscreverDado(0m, 5, 2); //Redução da Base de ICMS
            this.EscreverDado("", 83); //Espaços

            this.FinalizarRegistro();

            this.ObterRegistrosEFPHDerivados();

            return this.RegistroEFPH.ToString();
        }

        #endregion
    }
}
