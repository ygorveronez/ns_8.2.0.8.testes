namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACAO_EMISSAO_DOCUMENTOS_EMBARCADOR", EntityName = "ConfiguracaoEmissaoDocumentoEmbarcador", Name = "Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoEmissaoDocumentoEmbarcador", NameType = typeof(ConfiguracaoEmissaoDocumentoEmbarcador))]
    public class ConfiguracaoEmissaoDocumentoEmbarcador : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CED_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoOperacao", Column = "TOP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pedidos.TipoOperacao TipoOperacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Cliente { get; set; }

        public virtual string Descricao
        {
            get
            {
                return $"{TipoOperacao?.Descricao ?? string.Empty} - {Cliente?.Nome ?? string.Empty}";
            }
        }
    }
}