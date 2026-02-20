namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_PEDIDO_DOCUMENTO_CTE_COMPONENTE_FRETE", EntityName = "CargaPedidoDocumentoCTeComponenteFrete", Name = "Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTeComponenteFrete", NameType = typeof(CargaPedidoDocumentoCTeComponenteFrete))]
    public class CargaPedidoDocumentoCTeComponenteFrete : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PCC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaPedidoDocumentoCTe", Column = "CDC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CargaPedidoDocumentoCTe CargaPedidoDocumentoCTe { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ComponenteFrete", Column = "CFR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Frete.ComponenteFrete ComponenteFrete { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ComponentePrestacaoCTE", Column = "CPT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.ComponentePrestacaoCTE ComponentePrestacaoCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "PCC_DESCRICAO", TypeType = typeof(string), Length = 100, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Valor", Column = "PCC_VALOR", TypeType = typeof(decimal), NotNull = true)]
        public virtual decimal Valor { get; set; }

        public virtual CargaPedidoDocumentoCTeComponenteFrete Clonar()
        {
            return (CargaPedidoDocumentoCTeComponenteFrete)this.MemberwiseClone();
        }
    }
}
