namespace Dominio.Entidades.Embarcador.Seguros
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACAO_AVERBACAO_PORTOSEGURO", EntityName = "AverbacaoPortoSeguro", Name = "Dominio.Entidades.Embarcador.Seguros.AverbacaoPortoSeguro", NameType = typeof(AverbacaoPortoSeguro))]
    public class AverbacaoPortoSeguro : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CAP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ApoliceSeguro", Column = "APS_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Seguros.ApoliceSeguro ApoliceSeguro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CAP_USUARIO", TypeType = typeof(string), Length = 200, NotNull = true)]
        public virtual string Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CAP_SENHA", TypeType = typeof(string), Length = 200, NotNull = true)]
        public virtual string Senha { get; set; }

        public virtual string Descricao
        {
            get
            {
                return ApoliceSeguro.Descricao;
            }
        }
    }
}
