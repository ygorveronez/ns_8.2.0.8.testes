namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PRE_OBS_CONTRIBUINTE", EntityName = "ObservacaoContribuintePreCTE", Name = "Dominio.Entidades.ObservacaoContribuintePreCTE", NameType = typeof(ObservacaoContribuintePreCTE))]
    public class ObservacaoContribuintePreCTE : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "POB_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PreConhecimentoDeTransporteEletronico", Column = "PCO_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual PreConhecimentoDeTransporteEletronico PreCTE { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "POB_DESCRICAO", TypeType = typeof(string), Length = 160, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Identificador", Column = "POB_IDENTIFICADOR", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string Identificador { get; set; }
    }
}
