namespace AdminMultisoftware.Dominio.Entidades.Modulos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PERMISSAO_PERSONALIZADA", EntityName = "PermissaoPersonalizada", Name = "AdminMultisoftware.Dominio.Entidades.Modulos.PermissaoPersonalizada", NameType = typeof(PermissaoPersonalizada))]
    public class PermissaoPersonalizada : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PPS_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Formulario", Column = "FOR_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Modulos.Formulario Formulario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoPermissao", Column = "PPS_CODIGO_PERMISSAO", TypeType = typeof(Dominio.Enumeradores.PermissaoPersonalizada), NotNull = true)]
        public virtual Dominio.Enumeradores.PermissaoPersonalizada CodigoPermissao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "PPS_DESCRICAO", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "PPS_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PPS_TRANSLATION_RESOURCE_PATH", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string TranslationResourcePath { get; set; }

        public virtual string DescricaoAtivo
        {
            get
            {
                if (this.Ativo)
                    return "Ativo";
                else
                    return "Inativo";
            }
        }
    }
}
