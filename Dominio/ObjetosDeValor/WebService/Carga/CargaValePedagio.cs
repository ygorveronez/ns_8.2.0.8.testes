namespace Dominio.ObjetosDeValor.WebService.Carga
{
    public class CargaValePedagio
    {
        public int CodigoValePedagioEmbarcador { get; set; }

        public virtual Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa Fornecedor { get; set; }

        public virtual Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa Responsavel { get; set; }

        public string NumeroComprovante { get; set; }

        public string CodigoAgendamentoPorto { get; set; }

        public decimal Valor { get; set; }

        public int? CodigoIntegracaoValePedagioEmbarcador { get; set; }

        public Dominio.Enumeradores.TipoCompraValePedagio TipoCompra { get; set; }

        public int QuantidadeEixos { get; set; }

        public bool NaoIncluirMDFe { get; set; }
    }
}