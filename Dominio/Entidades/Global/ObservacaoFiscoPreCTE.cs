namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PRE_OBS_FISCO", EntityName = "ObservacaoFiscoPreCTE", Name = "Dominio.Entidades.ObservacaoFiscoPreCTE", NameType = typeof(ObservacaoFiscoPreCTE))]
    public class ObservacaoFiscoPreCTE: EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PBF_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PreConhecimentoDeTransporteEletronico", Column = "PCO_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual PreConhecimentoDeTransporteEletronico preCTE { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "PBF_DESCRICAO", TypeType = typeof(string), Length = 60, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Identificador", Column = "PBF_IDENTIFICADOR", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string Identificador { get; set; }
    }
}
