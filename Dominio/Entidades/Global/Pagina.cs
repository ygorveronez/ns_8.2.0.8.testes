namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_FORMULARIO", EntityName = "Pagina", Name = "Dominio.Entidades.Pagina", NameType = typeof(Pagina))]
    public class Pagina : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "FOR_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Formulario", Column = "FOR_FORMULARIO", TypeType = typeof(string), Length = 200, NotNull = true)]
        public virtual string Formulario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "FOR_DESCRICAO", TypeType = typeof(string), Length = 200, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Menu", Column = "FOR_NOME_MENU", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string Menu { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Menu", Column = "MEN_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Menu MenuApp { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Status", Column = "FOR_STATUS", TypeType = typeof(string), Length = 1, NotNull = true)]
        public virtual string Status { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Icone", Column = "FOR_ICONE", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string Icone { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MostraNoMenu", Column = "FOR_MOSTRA_MENU", TypeType = typeof(bool), NotNull = false)]
        public virtual bool MostraNoMenu { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoAcesso", Column = "FOR_AMBIENTE", TypeType = typeof(Dominio.Enumeradores.TipoAcesso), NotNull = false)]
        public virtual Dominio.Enumeradores.TipoAcesso TipoAcesso { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoSistema", Column = "FOR_TIPO_SISTEMA", TypeType = typeof(Dominio.Enumeradores.TipoSistema), NotNull = false)]
        public virtual Dominio.Enumeradores.TipoSistema TipoSistema { get; set; }
    }
}
