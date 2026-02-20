namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CENTRO_DESCARREGAMENTO_EMAIL", EntityName = "CentroDescarregamentoEmail", Name = "Dominio.Entidades.Embarcador.Logistica.CentroDescarregamentoEmail", NameType = typeof(CentroDescarregamentoEmail))]
    public class CentroDescarregamentoEmail : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CDE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CentroDescarregamento", Column = "CED_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento CentroDescarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Email", Column = "CDE_EMAIL", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string Email { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.Email;
            }
        }
    }
}
