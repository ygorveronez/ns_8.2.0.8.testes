namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_REGRA_PEDIDO_SITUACAO_COLABORADOR", EntityName = "RegrasPedidoSituacaoColaborador", Name = "Dominio.Entidades.Embarcador.Pedidos.RegrasPedidoSituacaoColaborador", NameType = typeof(RegrasPedidoSituacaoColaborador))]
    public class RegrasPedidoSituacaoColaborador : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "RTO_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegrasPedido", Column = "RPE_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual RegrasPedido RegrasPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ordem", Column = "RTO_ORDEM", TypeType = typeof(int), NotNull = false)]
        public virtual int Ordem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Condicao", Column = "RTO_CONDICAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia Condicao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Juncao", Column = "RTO_JUNCAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia Juncao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ColaboradorSituacao", Column = "CSI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuarios.Colaborador.ColaboradorSituacao ColaboradorSituacao { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.ColaboradorSituacao?.Descricao ?? string.Empty;
            }
        }
    }

}