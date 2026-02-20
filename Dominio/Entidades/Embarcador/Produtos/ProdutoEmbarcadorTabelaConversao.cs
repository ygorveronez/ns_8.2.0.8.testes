
namespace Dominio.Entidades.Embarcador.Produtos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PRODUTO_EMBARCADOR_TABELA_CONVERSAO_UNIDADES", EntityName = "ProdutoEmbarcadorTabelaConversao", Name = "Dominio.Entidades.Embarcador.Embarcador.ProdutoEmbarcadorTabelaConversao", NameType = typeof(ProdutoEmbarcadorTabelaConversao))]
    public class ProdutoEmbarcadorTabelaConversao : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PTC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PTC_QUANTIDADE_DE", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal QuantidadeDe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PTC_QUANTIDADE_PARA", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal QuantidadePara { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConversaoDeUnidade", Column = "CDU_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Produtos.ConversaoDeUnidade TipoConversao { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.TipoConversao != null ? $"({this.TipoConversao.Sigla}) {this.TipoConversao.Descricao} ↔ ({this.TipoConversao.UnidadeDeMedida.Sigla}) {this.TipoConversao.UnidadeDeMedida.Descricao} " : string.Empty;

            }
        }
    }
}
