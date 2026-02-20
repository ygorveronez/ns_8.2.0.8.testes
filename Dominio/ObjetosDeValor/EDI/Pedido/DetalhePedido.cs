using System;

namespace Dominio.ObjetosDeValor.EDI.Pedido
{
    public class DetalhePedido
    {
        public string IdentificacaoRegitro { get; set; }
        public string CodigoIdentificadorMensagem { get; set; }
        public int NumeroLinha { get; set; }
        public string CodigoEmitente { get; set; }
        public string CodigoCliente { get; set; }
        public string DescricaoCliente { get; set; }
        public string Cidade { get; set; }
        public string UF { get; set; }
        public DateTime DataFaturamento { get; set; }
        public string NumeroCarga { get; set; }
        public string StatusCarga { get; set; }
        public string DescricaoStatusCarga { get; set; }
        public string NumeroPedido { get; set; }
        public string Item { get; set; }
        public string CodigoItem { get; set; }
        public string DescricaoItem { get; set; }
        public decimal Quantidade { get; set; }
        public decimal Peso { get; set; }
        public string Filial { get; set; }
        public string Shipment_Instructions { get; set; }
        public string Shipment_Method { get; set; }
        public DateTime Schedule_Ship_Date { get; set; }
        public DateTime Schedule_Arrival_Date { get; set; }
        public string MessageIdentifierCode { get; set; }
        public string FileId { get; set; }
        public string CNPJRemetente { get; set; }
        public DateTime? DataCriacao { get; set; }
        public string NomeArquivo { get; set; }
    }
}
