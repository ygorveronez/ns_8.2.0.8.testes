namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ALIQUOTA_ICMS", EntityName = "AliquotaDeICMS", Name = "Dominio.Entidades.AliquotaDeICMS", NameType = typeof(AliquotaDeICMS))]
    public class AliquotaDeICMS : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "AIC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Aliquota", Column = "AIC_ALIQUOTA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal Aliquota { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Status", Column = "AIC_STATUS", TypeType = typeof(string), Length = 1, NotNull = true)]
        public virtual string Status { get; set; }

        public virtual string DescricaoStatus

        {
            get
            {
                switch (this.Status)
                {
                    case "A":
                        return "Ativo";
                    case "I":
                        return "Inativo";
                    default: 
                        return string.Empty;
                }
            }
        }
    }
}
