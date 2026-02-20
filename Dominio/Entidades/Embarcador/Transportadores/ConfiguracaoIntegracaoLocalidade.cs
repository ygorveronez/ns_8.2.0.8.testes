namespace Dominio.Entidades.Embarcador.Transportadores
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACAO_INTEGRACAO_LOCALIDADE", EntityName = "ConfiguracaoIntegracaoLocalidade", Name = "Dominio.Entidades.Embarcador.Transportadores.ConfiguracaoIntegracaoLocalidade", NameType = typeof(ConfiguracaoIntegracaoLocalidade))]
    public class ConfiguracaoIntegracaoLocalidade : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CIE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "LOC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Localidade Localidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIE_TIPO_INTEGRACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao TipoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIE_CODIGO_INTEGRACAO", TypeType = typeof(int), NotNull = false)]
        public virtual int CodigoIntegracao { get; set; }

    }
}
