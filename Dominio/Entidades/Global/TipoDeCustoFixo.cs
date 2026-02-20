namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TIPO_CUSTO_FIXO", EntityName = "TipoDeCustoFixo", Name = "Dominio.Entidades.TipoDeCustoFixo", NameType = typeof(TipoDeCustoFixo))]
    public class TipoDeCustoFixo : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TCF_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "TCF_DESCRICAO", TypeType = typeof(string), NotNull = true, Length = 200)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Status", Column = "TCF_STATUS", TypeType = typeof(string), NotNull = true, Length = 1)]
        public virtual string Status { get; set; }

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
