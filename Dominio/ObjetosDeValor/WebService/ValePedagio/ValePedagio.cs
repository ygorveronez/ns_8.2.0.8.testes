using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.WebService.ValePedagio
{
    public class ValePedagio
    {
        public virtual int ProtocoloCarga { get; set; }
        public virtual string NumeroValePedagio { get; set; }
        public virtual decimal ValorValePedagio { get; set; }
        public Embarcador.Pessoas.Pessoa Fornecedor { get; set; }
        public Embarcador.Pessoas.Pessoa Responsavel { get; set; }
        public Dominio.Enumeradores.TipoCompraValePedagio TipoCompra { get; set; }
        public string PDF { get; set; }
        public string NumeroStage { get; set; }
        public string NumeroRecibo { get; set; }
        public bool VpCancelado { get; set; }
        public string CNPJFornecedor { get; set; }
        public string CNPJPagador { get; set; }
        public string Observacao { get; set; }
        public SituacaoValePedagio Status { get; set; }
        public bool NaoPossuiValePedagio { get; set; }
        public TipoRotaFrete TipoPercursoVP { get; set; }
    }
}
