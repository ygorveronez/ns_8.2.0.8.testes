using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PEDIDO_ACRESCIMO_DESCONTO", EntityName = "PedidoAcrescimoDesconto", Name = "Dominio.Entidades.Embarcador.Pedidos.PedidoAcrescimoDesconto", NameType = typeof(PedidoAcrescimoDesconto))]
    public class PedidoAcrescimoDesconto : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PAD_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Pedido", Column = "PED_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.Pedido Pedido { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Justificativa", Column = "JUS_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Fatura.Justificativa Justificativa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoJustificativa", Column = "PAD_TIPO_JUSTIFICATIVA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoJustificativa), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoJustificativa TipoJustificativa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AplicacaoValor", Column = "PAD_APLICACAO_VALOR", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.AplicacaoValorJustificativaContratoFrete), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.AplicacaoValorJustificativaContratoFrete AplicacaoValor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Valor", Column = "PAD_VALOR", TypeType = typeof(decimal), NotNull = true)]
        public virtual decimal Valor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PAD_OBSERVACAO", TypeType = typeof(string), Length = 400, NotNull = false)]
        public virtual string Observacao { get; set; }

        public virtual string Descricao => Pedido.Descricao;

        public virtual string DescricaoAplicacaoValor => AplicacaoValor.ObterDescricao();

        public virtual string DescricaoTipoJustificativa => TipoJustificativa.ObterDescricao();
    }
}
