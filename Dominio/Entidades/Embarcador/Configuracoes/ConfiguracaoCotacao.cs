namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_COTACAO", EntityName = "ConfiguracaoCotacao", Name = "Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCotacao", NameType = typeof(ConfiguracaoCotacao))]
    public class ConfiguracaoCotacao : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CCO_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCO_GRAVAR_NUMERO_COTACAO_OBSERVACAO_INTERNA_AO_CRIAR_PEDIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GravarNumeroCotacaoObservacaoInternaAoCriarPedido { get; set; }

        public virtual string Descricao
        {
            get { return "Configuração para Cotação"; }
        }
    }
}
