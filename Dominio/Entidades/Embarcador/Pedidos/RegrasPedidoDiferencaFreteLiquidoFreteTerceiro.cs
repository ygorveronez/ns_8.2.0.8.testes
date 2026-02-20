namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_REGRA_PEDIDO_PERCENTUAL_DIFERENCA_FRETE_LIQUIDO_PARA_FRETE_TERCEIRO", EntityName = "RegrasPedidoPercentualDiferencaFreteLiquidoParaFreteTerceiro", Name = "Dominio.Entidades.Embarcador.Pedidos.RegrasPedidoPercentualDiferencaFreteLiquidoParaFreteTerceiro", NameType = typeof(RegrasPedidoPercentualDiferencaFreteLiquidoParaFreteTerceiro))]
    public class RegrasPedidoPercentualDiferencaFreteLiquidoParaFreteTerceiro : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "RPP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegrasPedido", Column = "RPE_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual RegrasPedido RegrasPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ordem", Column = "RPP_ORDEM", TypeType = typeof(int), NotNull = false)]
        public virtual int Ordem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Condicao", Column = "RPD_CONDICAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia Condicao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Juncao", Column = "RPD_JUNCAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia Juncao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DiferencaFreteLiquidoParaFreteTerceiro", Column = "RPP_PERCENTUAL_DIFERENCA_FRETE_LIQUIDO_PARA_FRETE_TERCEIRO", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = false)]
        public virtual decimal DiferencaFreteLiquidoParaFreteTerceiro { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.DiferencaFreteLiquidoParaFreteTerceiro.ToString("n2");
            }
        }
    }

}