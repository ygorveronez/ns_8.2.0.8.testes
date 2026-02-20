namespace Dominio.Relatorios.Embarcador.DataSource.Financeiros
{
    public class PosicaoDocumentoReceber
    {
        public int Codigo { get; set; }
        public string Numero { get; set; }
        public int Serie { get; set; }
        public string Empresa { get; set; }
        public string Remetente { get; set; }
        public string Destinatario { get; set; }
        public string Origem { get; set; }
        public string Destino { get; set; }
        public string Tomador { get; set; }
        public string GrupoPessoasTomador { get; set; }
        public string Veiculo { get; set; }
        public string Motorista { get; set; }
        public decimal ValorICMS { get; set; }
        public decimal ValorLiquidoDocumento { get; set; }
        public decimal ValorBrutoDocumento { get; set; }
        public string DataEmissao { get; set; }
        public string DataAutorizacao { get; set; }
        public string DataAnulacao { get; set; }
        public string DataCancelamento { get; set; }
        //public Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDocumentoFaturamento Situacao { get; set; }
        public decimal ValorDocumentoEmTitulo { get; set; }
        public decimal ValorTotalTitulo { get; set; }
        public decimal ValorAcrescimoGeracao { get; set; }
        public decimal ValorDescontoGeracao { get; set; }
        public decimal ValorAcrescimoBaixa { get; set; }
        public decimal ValorDescontoBaixa { get; set; }
        public decimal ValorPago { get; set; }
        public decimal ValorBaixadoDocumento { get; set; }
        public decimal ValorBaixadoAcrescimo { get; set; }
        public decimal ValorPendentePagamento { get; set; }
        public decimal ValorAFaturar { get; set; }
        //public string DescricaoSituacao
        //{
        //    get
        //    {
        //        switch (Situacao)
        //        {
        //            case Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDocumentoFaturamento.Autorizado:
        //                return "Autorizado";
        //            case Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDocumentoFaturamento.Cancelado:
        //                return "Cancelado";
        //            case Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDocumentoFaturamento.Anulado:
        //                return "Anulado";
        //            default:
        //                return "";
        //        }
        //    }
        //}
    }
}
