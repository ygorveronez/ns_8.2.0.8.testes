using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACAO_TIPO_OPERACAO_PAGAMENTOS", DynamicUpdate = true, EntityName = "ConfiguracaoTipoOperacaoPagamentos", Name = "Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoPagamentos", NameType = typeof(ConfiguracaoTipoOperacaoPagamentos))]
    public class ConfiguracaoTipoOperacaoPagamentos : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CTP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTP_TIPO_LIBERACAO_PAGAMENTO", TypeType = typeof(TipoLiberacaoPagamento), NotNull = false)]
        public virtual TipoLiberacaoPagamento TipoLiberacaoPagamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CentroResultado", Column = "CRE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.CentroResultado CentroResultado { get; set; }

        public virtual string Descricao
        {
            get { return "Configurações de Pagamentos"; }
        }
    }
}
