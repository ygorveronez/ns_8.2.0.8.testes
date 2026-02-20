namespace Dominio.Entidades.Embarcador.ProdutorRural
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_FECHAMENTO_COLETA_PEDIDO_COLETA", EntityName = "FechamentoColetaProdutorPedidos", Name = "Dominio.Entidades.Embarcador.ProdutorRural.FechamentoColetaProdutorPedidos", NameType = typeof(FechamentoColetaProdutorPedidos))]
    public class FechamentoColetaProdutorPedidos : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "FCC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "FechamentoColetaProdutor", Column = "FCP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.ProdutorRural.FechamentoColetaProdutor FechamentoColetaProdutor { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PedidoColetaProdutor", Column = "PRO_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.ProdutorRural.PedidoColetaProdutor PedidoColetaProdutor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PRO_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedidoColetaProdutor), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedidoColetaProdutor Situacao { get; set; }

    }
}