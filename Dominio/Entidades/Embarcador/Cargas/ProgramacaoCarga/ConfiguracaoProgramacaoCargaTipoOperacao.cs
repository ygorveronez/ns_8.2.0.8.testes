namespace Dominio.Entidades.Embarcador.Cargas.ProgramacaoCarga
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_PROGRAMACAO_CARGA_TIPO_OPERACAO", EntityName = "ConfiguracaoProgramacaoCargaTipoOperacao", Name = "Dominio.Entidades.Embarcador.Cargas.ProgramacaoCarga.ConfiguracaoProgramacaoCargaTipoOperacao", NameType = typeof(ConfiguracaoProgramacaoCargaTipoOperacao))]
    public class ConfiguracaoProgramacaoCargaTipoOperacao : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CTO_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConfiguracaoProgramacaoCarga", Column = "CPC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ConfiguracaoProgramacaoCarga ConfiguracaoProgramacaoCarga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoOperacao", Column = "TOP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pedidos.TipoOperacao TipoOperacao { get; set; }
    }
}
