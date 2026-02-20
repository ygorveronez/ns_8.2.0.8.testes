namespace Dominio.Entidades.EFD.SPEDContribuicoes
{
    public class _0140 : Registro
    {
        #region Construtores

        public _0140() : base("0140") { }

        #endregion

        #region Propriedades

        public Dominio.Entidades.Empresa Empresa { get; set; }

        #endregion

        #region Métodos

        public override string ObterDadosParaArquivo()
        {
            this.EscreverDado(this.Empresa.CNPJ, 60); //COD_EST
            this.EscreverDado(this.Empresa.RazaoSocial, 100); //NOME
            this.EscreverDado(this.Empresa.CNPJ); //CNPJ
            this.EscreverDado(this.Empresa.Localidade.Estado.Sigla, 2); //UF
            this.EscreverDado(this.Empresa.InscricaoEstadual, 14); //IE
            this.EscreverDado(this.Empresa.Localidade.CodigoIBGE, 7); //COD_MUN
            this.EscreverDado(""); //IM
            this.EscreverDado(this.Empresa.Suframa, 9); //SUFRAMA

            this.FinalizarRegistro();

            this.ObterRegistrosSPEDDerivados();

            return this.RegistroSPED.ToString();
        }

        #endregion
    }
}
