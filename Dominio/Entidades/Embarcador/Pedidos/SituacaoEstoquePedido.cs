namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_SITUACAO_ESTOQUE_PEDIDO", EntityName = "SituacaoEstoquePedido", Name = "Dominio.Entidades.Embarcador.Pedidos.SituacaoEstoquePedido", NameType = typeof(SituacaoEstoquePedido))]
    public class SituacaoEstoquePedido : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "SEP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "SEP_DESCRICAO", TypeType = typeof(string), Length = 200, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegracao", Column = "SEP_CODIGO_INTEGRACAO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "SEP_OBSERVACAO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Cor", Column = "SEP_COR", TypeType = typeof(string), Length = 7, NotNull = false)]
        public virtual string Cor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BloqueiaPedido", Column = "SEP_BLOQUEIA_PEDIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool BloqueiaPedido { get; set; }
    }
}
