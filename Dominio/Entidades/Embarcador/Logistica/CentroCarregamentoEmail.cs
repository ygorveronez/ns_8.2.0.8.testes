namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CENTRO_CARREGAMENTO_EMAIL", EntityName = "CentroCarregamentoEmail", Name = "Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoEmail", NameType = typeof(CentroCarregamentoEmail))]
    public class CentroCarregamentoEmail : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CCE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CentroCarregamento", Column = "CEC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Logistica.CentroCarregamento CentroCarregamento { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "Email", Column = "CCE_EMAIL", TypeType = typeof(string), Length = 150, NotNull = true)]
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
