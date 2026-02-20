namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_REGRA_TIPO_OPERACAO_RECEBEDOR", EntityName = "RegraTipoOperacaoRecebedor", Name = "Dominio.Entidades.Embarcador.Pedidos.RegraTipoOperacaoRecebedor", NameType = typeof(RegraTipoOperacaoRecebedor))]
    public class RegraTipoOperacaoRecebedor : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "RTR_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Recebedor { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegraTipoOperacao", Column = "RTO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual RegraTipoOperacao RegraTipoOperacao { get; set; }
    }
}