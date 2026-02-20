namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACAO_TIPO_OPERACAO_CONTROLE_ENTREGA_SETOR", EntityName = "TipoOperacaoControleEntregaSetor", Name = "Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoControleEntregaSetor", NameType = typeof(TipoOperacaoControleEntregaSetor))]
    public class TipoOperacaoControleEntregaSetor : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TOS_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConfiguracaoTipoOperacaoControleEntrega", Column = "COE_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ConfiguracaoTipoOperacaoControleEntrega ConfiguracaoTipoOperacaoControleEntrega { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Setor", Column = "SET_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Setor Setor { get; set; }
    }
}