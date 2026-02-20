namespace Dominio.Entidades.Embarcador.Transportadores
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACAO_INTEGRACAO_EMPRESA", EntityName = "ConfiguracaoIntegracaoEmpresa", Name = "Dominio.Entidades.Embarcador.Transportadores.ConfiguracaoIntegracaoEmpresa", NameType = typeof(ConfiguracaoIntegracaoEmpresa))]
    public class ConfiguracaoIntegracaoEmpresa : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CIE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIE_TIPO_INTEGRACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao TipoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIE_CODIGO_INTEGRACAO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string CodigoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIE_CODIGO_CLIENTE_OPENTECH", TypeType = typeof(int), NotNull = false)]
        public virtual int CodigoClienteOpenTech { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIE_CODIGO_PAS_OPENTECH", TypeType = typeof(int), NotNull = false)]
        public virtual int CodigoPASOpenTech { get; set; }

        public virtual string Descricao
        {
            get { return Codigo.ToString(); }
        }
    }
}
