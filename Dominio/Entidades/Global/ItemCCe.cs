namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CCE_ITEM", EntityName = "ItemCCe", Name = "Dominio.Entidades.ItemCCe", NameType = typeof(ItemCCe))]
    public class ItemCCe : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "ICC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CartaDeCorrecaoEletronica", Column = "CCE_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CartaDeCorrecaoEletronica CCe { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CampoCCe", Column = "CCC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CampoCCe CampoAlterado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorAlterado", Column = "ICC_VALOR_ALTERADO", TypeType = typeof(string), Length = 500, NotNull = true)]
        public virtual string ValorAlterado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroItemAlterado", Column = "ICC_NUMERO_ITEM_ALTERADO", TypeType = typeof(int), NotNull = false)]
        public virtual int NumeroItemAlterado { get; set; }

        public virtual string GrupoCampo
        {
            get
            {
                return this.CampoAlterado.GrupoCampo;
            }
        }

        public virtual string NomeCampo
        {
            get
            {
                return this.CampoAlterado.NomeCampo;
            }
        }

        public virtual string DescricaoCampo
        {
            get
            {
                return this.CampoAlterado.Descricao;
            }
        }

        public virtual string Descricao
        {
            get
            {
                return this.CampoAlterado.Descricao;
            }
        }
    }
}
