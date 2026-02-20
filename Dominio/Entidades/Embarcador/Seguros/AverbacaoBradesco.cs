namespace Dominio.Entidades.Embarcador.Seguros
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACAO_AVERBACAO_BRADESCO", EntityName = "AverbacaoBradesco", Name = "Dominio.Entidades.Embarcador.Seguros.AverbacaoBradesco", NameType = typeof(AverbacaoBradesco))]
    public class AverbacaoBradesco : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CAB_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ApoliceSeguro", Column = "APS_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Seguros.ApoliceSeguro ApoliceSeguro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CAB_TOKEN", TypeType = typeof(string), Length = 100, NotNull = true)]
        public virtual string Token { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CAB_WSDL_QUORUM", TypeType = typeof(string), Length = 100, NotNull = true)]
        public virtual string WSDLQuorum { get; set; }

        public virtual string Descricao
        {
            get
            {
                return ApoliceSeguro.Descricao;
            }
        }
    }
}
