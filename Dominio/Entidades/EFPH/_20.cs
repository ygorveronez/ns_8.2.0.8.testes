namespace Dominio.Entidades.EFPH
{
    public class _20 : Registro
    {
        #region Construtores

        public _20()
            : base("20")
        {
        }

        #endregion

        #region Propriedades

        public Dominio.Entidades.ConhecimentoDeTransporteEletronico CTe { get; set; }

        #endregion

        #region Métodos

        public override string ObterDadosParaArquivo()
        {
            this.EscreverDado(this.CTe.DataEmissao, "dd"); //Dia 
            this.EscreverDado("CT-e", 5); //Espécie 
            this.EscreverDado(this.CTe.Serie.Numero.ToString(), 5); //Série/Subsérie
            this.EscreverDado(this.CTe.Numero, 6); //Número Documento(Opcional)
            this.EscreverDado(0, 6); //Número Sequência
            this.EscreverDado(this.CTe.Destinatario.Exterior ? "EX" : this.CTe.Destinatario.Localidade.Estado.Sigla, 2); //Sigla Estado Destinatário
            this.EscreverDado(this.CTe.CFOP.CodigoCFOP, 4); //Código Fiscal de Operações
            this.EscreverDado(28, 4); //Histórico Identificador
            this.EscreverDado(this.CTe.Remetente.Exterior ? "9999999" : this.CTe.Remetente.Localidade.CodigoIBGE.ToString(), 25); //Município de Origem
            this.EscreverDado("", 24); //Observações
            this.EscreverDado(1, 1); //Modelo do Documento

            //Modalidade Frete
            if (this.CTe.TipoPagamento == Enumeradores.TipoPagamento.Pago)
                this.EscreverDado(1, 1); //1-CIF
            else if (this.CTe.TipoPagamento == Enumeradores.TipoPagamento.A_Pagar)
                this.EscreverDado(2, 1); //2-FOB
            else
                this.EscreverDado(3, 1); //3-OUTROS

            this.EscreverDado(0, 1); //Saída p/Empresa Exportadora

            //Situação do Documento
            if (this.CTe.Status == "A")
                this.EscreverDado(0, 1); //0=Normal
            else if (this.CTe.Status == "C")
                this.EscreverDado(1, 1); //1=Cancelado
            else if (this.CTe.Status == "D")
                this.EscreverDado(4, 1); //4=Uso Denegado
            else
                this.EscreverDado(5, 1); //5=Uso Inutilizado

            this.EscreverDado(0, 2); //Sub-Código Fiscal (SC)

            this.EscreverDado(0, 1); //Agrupar Lançamentos

            this.EscreverDado("S", 1); //Documento Eletrônico

            this.EscreverDado(this.CTe.Chave, 44); //Chave Eletrônica

            this.EscreverDado(this.CTe.DataEmissao, "ddMMyyyy"); //Data Saída

            this.EscreverDado("", 76); //Complemento de Observações

            this.EscreverDado("", 20); //Documentos Cancelados

            this.EscreverDado(1, 1); //Tipo da Fatura

            this.EscreverDado(this.CTe.Numero, 9); //Número Documento

            this.EscreverDado("", 5); //Espaços

            this.FinalizarRegistro();

            this.ObterRegistrosEFPHDerivados();

            return this.RegistroEFPH.ToString();
        }

        #endregion
    }
}
