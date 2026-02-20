namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_OBS_FISCO", EntityName = "ObservacaoFiscoCTE", Name = "Dominio.Entidades.ObservacaoFiscoCTE", NameType = typeof(ObservacaoFiscoCTE))]
    public class ObservacaoFiscoCTE : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "OBF_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConhecimentoDeTransporteEletronico", Column = "CON_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ConhecimentoDeTransporteEletronico CTE { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "OBF_DESCRICAO", TypeType = typeof(string), Length = 60, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Identificador", Column = "OBF_IDENTIFICADOR", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string Identificador { get; set; }

        public virtual ObservacaoFiscoCTE Clonar()
        {
            return (ObservacaoFiscoCTE)this.MemberwiseClone();
        }
    }
}
