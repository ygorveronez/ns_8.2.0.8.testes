namespace Dominio.Entidades.EFPH
{
    public class _21 : Registro
    {
        #region Construtores

        public _21()
            : base("21")
        {
        }

        #endregion

        #region Propriedades

        public Dominio.Entidades.ConhecimentoDeTransporteEletronico CTe { get; set; }

        #endregion

        #region Métodos

        public override string ObterDadosParaArquivo()
        {
            this.EscreverDado(this.CTe.Status == "A" ? this.CTe.BaseCalculoICMS : 0m, 10, 2); //Valor Contábil
            this.EscreverDado(0, 9); //Frete
            this.EscreverDado(0, 9); //Seguro
            this.EscreverDado(0, 9); //Outras Despesas Acessórias
            this.EscreverDado(0, 9); //Desconto Global
            this.EscreverDado(0, 9); //Base IRPJ-1
            this.EscreverDado(0, 4); //Alíquota IRPJ-1
            this.EscreverDado(0, 9); //Base IRPJ-2
            this.EscreverDado(0, 4); //Alíquota IRPJ-2
            this.EscreverDado(0, 9); //Base IRPJ-3
            this.EscreverDado(0, 4); //Alíquota IRPJ-3
            this.EscreverDado(0, 9); //Base IRPJ-4
            this.EscreverDado(0, 4); //Alíquota IRPJ-4
            this.EscreverDado(0, 9); //Base IRPJ-5
            this.EscreverDado(0, 4); //Alíquota IRPJ-5
            this.EscreverDado(0, 9); //Deduções SIMPLES/ME-EPP-UF
            this.EscreverDado(0, 9); //Desmembra Valor Contábil 1
            this.EscreverDado(0, 9); //Desmembra Valor Contábil 2
            this.EscreverDado(0, 9); //Desmembra Valor Contábil 3
            this.EscreverDado(0, 9); //Desmembra Valor Contábil 4
            this.EscreverDado(0, 9); //Desmembra Valor Contábil 5
            this.EscreverDado(0, 9); //PIS/COFINS
            this.EscreverDado(0, 9); //Aprop.Créd.Ativo Imob.
            this.EscreverDado(0, 9); //Ressarc.de Subst.Tribut.
            this.EscreverDado(0, 9); //Transferência de Crédito 
            this.EscreverDado(0, 9); //Compl.Valor NF/ICMS
            this.EscreverDado(0, 9); //Serviço não Tributado
            this.EscreverDado(0, 3); //Reservado   
            this.EscreverDado(0, 3); //Reservado   
            this.EscreverDado(0, 9); //Abatimento NT
            this.EscreverDado(0, 4); //Centro de Custos Débito
            this.EscreverDado(0, 4); //Centro de Custos Crédito
            this.EscreverDado("", 10); //Espaços

            this.FinalizarRegistro();

            this.ObterRegistrosEFPHDerivados();

            return this.RegistroEFPH.ToString();
        }

        #endregion
    }
}
