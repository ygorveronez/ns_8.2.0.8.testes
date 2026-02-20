namespace Dominio.Entidades.Embarcador.CotacaoPedido
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_REGRA_COTACAO_PEDIDO_VALOR", EntityName = "RegrasCotacaoPedidoValor", Name = "Dominio.Entidades.Embarcador.CotacaoPedido.RegrasCotacaoPedidoValor", NameType = typeof(RegrasCotacaoPedidoValor))]
    public class RegrasCotacaoPedidoValor : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "RCV_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegrasCotacaoPedido", Column = "RCP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual RegrasCotacaoPedido RegrasCotacaoPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ordem", Column = "RCV_ORDEM", TypeType = typeof(int), NotNull = false)]
        public virtual int Ordem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Condicao", Column = "RCV_CONDICAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia Condicao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Juncao", Column = "RCV_JUNCAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia Juncao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Valor", Column = "RCV_VALOR", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = false)]
        public virtual decimal Valor { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.Valor.ToString("n2");
            }
        }
    }

}