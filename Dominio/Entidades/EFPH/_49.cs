namespace Dominio.Entidades.EFPH
{
    public class _49 : Registro
    {
        #region Construtores

        public _49()
            : base("49")
        {
        }

        #endregion

        #region Propriedades

        public Dominio.Entidades.ConhecimentoDeTransporteEletronico CTe { get; set; }

        public decimal PesoTotal { get; set; }

        #endregion

        #region Métodos

        public override string ObterDadosParaArquivo()
        {
            this.EscreverDado("", 12); //Número do despacho
            this.EscreverDado(this.CTe.Remetente.CPF_CNPJ, 14); //CNPJ/CPF remetente das mercadorias
            this.EscreverDado(this.CTe.Remetente.IE_RG, 14); //Inscrição Estadual
            this.EscreverDado(this.CTe.Remetente.Exterior ? 9999999 : this.CTe.Remetente.Localidade.CodigoIBGE, 7); //Código IBGE Município de Origem
            this.EscreverDado(this.CTe.Destinatario.CPF_CNPJ, 14); //CNPJ/CPF destinatário das mercadorias
            this.EscreverDado(this.CTe.Destinatario.IE_RG, 14); //Inscrição Estadual
            this.EscreverDado(this.CTe.Destinatario.Exterior ? 9999999 : this.CTe.Destinatario.Localidade.CodigoIBGE, 7); //Código IBGE Município de Destino
            this.EscreverDado(0, 1); //Tipo Transporte carga coletada
            this.EscreverDado(this.CTe.Remetente.CPF_CNPJ, 14); //CNPJ Contribuinte Local Coleta
            this.EscreverDado(this.CTe.Remetente.IE_RG, 14); //Inscrição Estadual
            this.EscreverDado(this.CTe.Remetente.Exterior ? 9999999 : this.CTe.Remetente.Localidade.CodigoIBGE, 7); //Código IBGE Município da Coleta
            this.EscreverDado(this.CTe.Destinatario.CPF_CNPJ, 14); //CNPJ Contribuinte Local Entrega
            this.EscreverDado(this.CTe.Destinatario.IE_RG, 14); //Inscrição Estadual
            this.EscreverDado(this.CTe.Destinatario.Exterior ? 9999999 : this.CTe.Destinatario.Localidade.CodigoIBGE, 7); //Código IBGE Município da Entrega
            this.EscreverDado(this.CTe.ModeloDocumentoFiscal.Numero, 2); //Código do Modelo Documento Fiscal
            this.EscreverDado(this.CTe.Serie.Numero, 5); //Série
            this.EscreverDado(this.CTe.Numero, 9); //Número
            this.EscreverDado(this.CTe.DataEmissao, "ddMMyyyy"); //Data emissão
            this.EscreverDado(this.CTe.ValorAReceber, 10, 2); //Valor total
            this.EscreverDado(this.CTe.ValorTotalMercadoria, 10, 2); //Valor das mercadorias
            this.EscreverDado(1, 8); //Quantidade de volumes
            this.EscreverDado(this.PesoTotal, 8, 2); //Peso bruto dos volumes Kg
            this.EscreverDado(this.PesoTotal, 8, 2); //Peso líquido dos volumes
            this.EscreverDado("", 25); //Espaços

            this.FinalizarRegistro();

            this.ObterRegistrosEFPHDerivados();

            return this.RegistroEFPH.ToString();
        }

        #endregion
    }
}
