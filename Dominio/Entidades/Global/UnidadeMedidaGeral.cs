namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_UNIDADE_MEDIDA_GERAL", EntityName = "UnidadeMedidaGeral", Name = "Dominio.Entidades.UnidadeMedidaGeral", NameType = typeof(UnidadeMedidaGeral))]
    public class UnidadeMedidaGeral : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "UMG_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "UMG_DESCRICAO", TypeType = typeof(string), Length = 200, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Sigla", Column = "UMG_SIGLA", TypeType = typeof(string), Length = 200, NotNull = true)]
        public virtual string Sigla { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Status", Column = "UMG_STATUS", TypeType = typeof(string), Length = 1, NotNull = true)]
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
                        return "";
                }
            }
        }
    }
}
