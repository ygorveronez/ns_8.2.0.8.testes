using Dominio.Entidades.Embarcador.Bidding;

namespace Dominio.Entidades.Embarcador.Chamados
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_HISTORICO_NOTA_FISCAL_CHAMADO", EntityName = "HistoricoNotaFiscalChamado", Name = "Dominio.Entidades.Embarcador.Chamados.HistoricoNotaFiscalChamado", NameType = typeof(HistoricoNotaFiscalChamado))]
    public class HistoricoNotaFiscalChamado : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "HNC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "HNC_SITUACAO_NOTA_FISCAL_CHAMADO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoNotaFiscal), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoNotaFiscal SituacaoNotaFiscal { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Chamado", Column = "CHA_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Chamado Chamado { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "XMLNotaFiscal", Column = "NFX_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pedidos.XMLNotaFiscal XMLNotaFiscal { get; set; }
    }
}