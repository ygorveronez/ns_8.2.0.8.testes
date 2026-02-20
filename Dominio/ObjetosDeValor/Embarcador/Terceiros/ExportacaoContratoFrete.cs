using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Terceiros
{
    public class ExportacaoContratoFrete
    {
        public DateTime DataAberturaCIOT { get; set; }
        public string AdicionalCTe { get; set; }
        public string NumeroFormulario { get; set; }
        public int SerieFormulario { get; set; }
        public DateTime DataEmissao { get; set; }
        public string CNPJCPFRemetente { get; set; }
        public string CNPJCPFDestinatario { get; set; }
        public decimal Peso { get; set; }
        public string Volume { get; set; }
        public string MetrosCubicos { get; set; }
        public decimal ValorUnitario { get; set; }
        public string ValorFreteBruto { get; set; }
        public string ValorFreteLiquido { get; set; }
        public string CPFMotorista { get; set; }
        public string PlacaControle { get; set; }
        public string PlacaReferencia { get; set; }
        public decimal ValorItemAdiantamento { get; set; }
        public decimal ValorPedagio { get; set; }
        public string NumeroCRTSistemaExterno { get; set; }
        public string CentroCustoGerencial { get; set; }
        public string DataCancelamento { get; set; }
        public string Observacao { get; set; }
        public string UnidadeNegocio { get; set; }
        public decimal TotalAcrescimos { get; set; }
        public decimal TotalDescontos { get; set; }
        public string NumeroDocumento { get; set; }
        public string NomeCliente { get; set; }
        public string CNPJCPFCliente { get; set; }
        public string CentroCustoOriginal { get; set; }

        public string DataEmissaoFormatada
        {
            get { return DataEmissao != DateTime.MinValue ? DataEmissao.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }
        
        public string DataAberturaCIOTFormatada
        {
            get { return DataAberturaCIOT != DateTime.MinValue ? DataAberturaCIOT.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }
    }
}
