namespace Dominio.Entidades.Embarcador.Seguros
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACAO_AVERBACAO_SENIG", EntityName = "AverbacaoSenig", Name = "Dominio.Entidades.Embarcador.Seguros.AverbacaoSenig", NameType = typeof(AverbacaoSenig))]
    public class AverbacaoSenig : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CAS_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ApoliceSeguro", Column = "APS_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Seguros.ApoliceSeguro ApoliceSeguro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CAS_AVERBA_COMO_EMBARCADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AverbaComoEmbarcador { get; set; }

        public virtual string Descricao => ApoliceSeguro.Descricao;
    }
}
