namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ACERTO_TIPO_CARGA", EntityName = "TipoCarga", Name = "Dominio.Entidades.TipoCarga", NameType = typeof(TipoCarga))]
    public class TipoCarga : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "ATC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "ACE_DESCRICAO", TypeType = typeof(string), NotNull = false, Length = 200)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Status", Column = "ACE_STATUS", TypeType = typeof(string), NotNull = false, Length = 1)]
        public virtual string Status { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoEmbarcador", Column = "ACE_CODIGO_EMBARCADOR", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoEmbarcador { get; set; }

        public virtual string DescricaoStatus
        {
            get
            {
                switch (this.Status)
                {
                    case "I":
                        return "Inativo";
                    case "A":
                        return "Ativo";
                    default:
                        return "";
                }
            }
        }
    }
}
