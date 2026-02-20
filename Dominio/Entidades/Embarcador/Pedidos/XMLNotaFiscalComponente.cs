namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_XML_NOTA_FISCAL_COMPONENTE", EntityName = "XMLNotaFiscalComponente", Name = "Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalComponente", NameType = typeof(XMLNotaFiscalComponente))]
    public class XMLNotaFiscalComponente: EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "XNC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "XMLNotaFiscal", Column = "NFX_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual XMLNotaFiscal XMLNotaFiscal { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ComponenteFrete", Column = "CFR_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Frete.ComponenteFrete ComponenteFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "XNC_VALOR", TypeType = typeof(decimal), Scale = 4, Precision = 18, NotNull = false)]
        public virtual decimal Valor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "XNC_INCLUIR_ICMS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? IncluirICMS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "XNC_INCLUIR_INTEGRALMENTE_CONTRATO_FRETE_TERCEIRO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? IncluirIntegralmenteContratoFreteTerceiro { get; set; }
    }
}
