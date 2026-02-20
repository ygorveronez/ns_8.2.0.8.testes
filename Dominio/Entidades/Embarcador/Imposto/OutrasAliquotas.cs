namespace Dominio.Entidades.Embarcador.Imposto
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_OUTRAS_ALIQUOTAS", EntityName = "OutrasAliquotas", Name = "Dominio.Entidades.Embarcador.OutrasAliquotas.OutrasAliquotas", NameType = typeof(OutrasAliquotas))]

    public class OutrasAliquotas : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TOA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "TOA_DESCRICAO", TypeType = typeof(string), Length = 60, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "TOA_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CST", Column = "TOA_CST", TypeType = typeof(string), Length = 5, NotNull = true)]
        public virtual string CST { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoClassificacaoTributaria", Column = "TOA_CODIGO_CLASSE_TRIBUTARIA", TypeType = typeof(string), Length = 8, NotNull = true)]
        public virtual string CodigoClassificacaoTributaria { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIndicadorOperacao", Column = "TOA_CODIGO_INDICADOR_OPERACAO", TypeType = typeof(string), Length = 6, NotNull = false)]
        public virtual string CodigoIndicadorOperacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoOperacao", Column = "TOP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.TipoOperacao TipoOperacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ZerarBase", Column = "TOA_ZERAR_BASE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ZerarBase { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Exportacao", Column = "TOA_EXPORTACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Exportacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CalcularImpostoDocumento", Column = "TOA_CALCULAR_IMPOSTO_DOCUMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CalcularImpostoDocumento { get; set; }
    }
}
