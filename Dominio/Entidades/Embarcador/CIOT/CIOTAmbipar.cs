namespace Dominio.Entidades.Embarcador.CIOT
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CIOT_AMBIPAR", EntityName = "CIOTAmbipar", Name = "Dominio.Entidades.Embarcador.CIOT.CIOTAmbipar", NameType = typeof(CIOTAmbipar))]
    public class CIOTAmbipar : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CAP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConfiguracaoCIOT", Column = "CCT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.CIOT.ConfiguracaoCIOT ConfiguracaoCIOT { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CAP_USUARIO", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CAP_SENHA", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string Senha { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CAP_URL", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string URL { get; set; }

        public virtual string Descricao
        {
            get { return Usuario; }
        }
    }
}
