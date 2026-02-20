using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.EDI.Notfis
{
    public class NotaFiscal
    {
        public string NumeroPedido { get; set; }
        public string SeriePedido { get; set; }
        public string DataPedido { get; set; }
        public string DataEntrega { get; set; }
        public string NumeroRomaneio { get; set; }
        public string CondicaoPagamento { get; set; }
        public string NaturezaMercadoria { get; set; }
        public string MeioTransporte { get; set; }
        public string TipoTransporte { get; set; }
        public string TipoCarga { get; set; }
        public string Acondicionamento { get; set; }
        public decimal PesoCubagem { get; set; }
        public string IncidenciaICMS { get; set; }
        public string Seguro { get; set; }
        public decimal Aliquota { get; set; }
        public decimal ValorCobrado { get; set; }
        public decimal FretePeso { get; set; }
        public string Placa { get; set; }
        public string PlanoCarga { get; set; }
        public string AcaoDocumento { get; set; }
        public string IndicaBonfifacao { get; set; }
        public string Filler { get; set; }
        public string Recarga { get; set; }
        public string CargaCompartilhada { get; set; }
        public string Roteiro { get; set; }
        public string CDRemessa { get; set; }
        public string TipoNF { get; set; }
        public string NumeroOrdem { get; set; }
        public string CodigoCompromisso { get; set; }
        public string CodigoDocumentoVinculado { get; set; }
        public string NumeroDI { get; set; }
        public decimal ValorCompromisso { get; set; }
        public string NumeroReferenciaEDI { get; set; }
        public string NumeroControleCliente { get; set; }
        public string PINSuframa { get; set; }
        public string TipoDeCarga { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal NFe { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.CTe.CTe CTe { get; set; }
        public List<Dominio.ObjetosDeValor.Embarcador.NFe.Produtos> Produtos { get; set; }
        public List<Dominio.ObjetosDeValor.Embarcador.NFe.ComplementoProduto> ComplementoProdutos { get; set; }
        public List<Mercadoria> Mercadorias { get; set; }
        public ComplementoNotaFiscal ComplementoNotaFiscal { get; set; }
        public Consignatario Consignatario { get; set; }
        public Recebedor Recebedor { get; set; }        
        public ResponsavelFrete ResponsavelFrete { get; set; }
        public Expedidor Expedidor { get; set; }
    }
}
