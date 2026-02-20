namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_OBS_CONTRIBUINTE", EntityName = "ObservacaoContribuinteCTE", Name = "Dominio.Entidades.ObservacaoContribuinteCTE", NameType = typeof(ObservacaoContribuinteCTE))]
    public class ObservacaoContribuinteCTE : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "OBC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConhecimentoDeTransporteEletronico", Column = "CON_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ConhecimentoDeTransporteEletronico CTE { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "OBC_DESCRICAO", TypeType = typeof(string), Length = 160, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Identificador", Column = "OBC_IDENTIFICADOR", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string Identificador { get; set; }

        public virtual ObservacaoContribuinteCTE Clonar()
        {
            return (ObservacaoContribuinteCTE)this.MemberwiseClone();
        }
    }
}
