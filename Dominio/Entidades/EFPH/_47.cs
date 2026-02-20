namespace Dominio.Entidades.EFPH
{
    public class _47 : Registro
    {
        #region Construtores

        public _47()
            : base("47")
        {
        }

        #endregion

        #region Propriedades

        public Dominio.Entidades.ConhecimentoDeTransporteEletronico CTe { get; set; }

        public Dominio.Entidades.VeiculoCTE Veiculo { get; set; }

        #endregion

        #region Métodos

        public override string ObterDadosParaArquivo()
        {
            //Tipo de CT-e
            if (this.CTe.TipoCTE == Enumeradores.TipoCTE.Normal)
                this.EscreverDado(0, 1); //0=Normal
            else if (this.CTe.TipoCTE == Enumeradores.TipoCTE.Complemento)
                this.EscreverDado(1, 1); //1=Complemento de Valores
            else
                this.EscreverDado(2, 1); //2=Anulação de Débito

            this.EscreverDado(this.CTe.ChaveCTEReferenciado, 44); //Chave do CT-e referência
            this.EscreverDado(this.CTe.Destinatario.Exterior? 9999999: this.CTe.Destinatario.Localidade.CodigoIBGE, 7); //Código IBGE Município de Destino
            this.EscreverDado("", 40); //Reservado
            this.EscreverDado("", 2); //Reservado
            this.EscreverDado(this.Veiculo != null ? this.Veiculo.Placa : string.Empty, 8); //Placa Identif.Veículo
            this.EscreverDado(this.Veiculo != null ? this.Veiculo.SiglaUF : string.Empty, 2); //UF Placa Veículo
            this.EscreverDado("", 14); //CNPJ/CPF Consignatário
            this.EscreverDado("", 14); //CNPJ/CPF Redespachante
            this.EscreverDado(0, 1); //Indicador Redespacho
            this.EscreverDado(0m, 10, 2); //Sec/Cat (serviços de coleta custo adicional de transp)
            this.EscreverDado(0m, 10, 2); //Valores de despacho
            this.EscreverDado(0m, 10, 2); //Valores de pedágio
            this.EscreverDado(0m, 10, 2); //Outros Valores
            this.EscreverDado("", 73); //Espaços

            this.FinalizarRegistro();

            this.ObterRegistrosEFPHDerivados();

            return this.RegistroEFPH.ToString();
        }

        #endregion
    }
}
