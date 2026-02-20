namespace Dominio.Entidades.Embarcador.CRM
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PROSPECCAO_PRODUTO", EntityName = "ProdutoProspect", Name = "Dominio.Entidades.Embarcador.CRM.ProdutoProspect", NameType = typeof(ProdutoProspect))]
    public class ProdutoProspect : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PP_DESCRICAO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string Nome { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "PP_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        public virtual string Descricao
        {
            get
            {
                    return this.Nome;
            }
        }
    }
}
