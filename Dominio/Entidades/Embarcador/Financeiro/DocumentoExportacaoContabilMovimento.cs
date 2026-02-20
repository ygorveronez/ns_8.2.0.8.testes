namespace Dominio.Entidades.Embarcador.Financeiro
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_DOCUMENTO_EXPORTACAO_CONTABIL_CONTA", EntityName = "DocumentoExportacaoContabilConta", Name = "Dominio.Entidades.Embarcador.Financeiro.DocumentoExportacaoContabilConta", NameType = typeof(DocumentoExportacaoContabilConta))]
    public class DocumentoExportacaoContabilConta : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "DCC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "DocumentoExportacaoContabil", Column = "DEC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.DocumentoExportacaoContabil DocumentoExportacaoContabil { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PlanoConta", Column = "PLA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.PlanoConta PlanoConta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "DCC_CONTA_CONTABIL", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string ContaContabil { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "DCC_TIPO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.DebitoCredito), NotNull = true)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.DebitoCredito Tipo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CentroResultado", Column = "CRE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.CentroResultado CentroResultado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "DCC_CODIGO_CENTRO_RESULTADO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string CodigoCentroResultado { get; set; }
    }
}
