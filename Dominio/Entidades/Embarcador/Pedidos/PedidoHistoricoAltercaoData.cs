using System;

namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PEDIDO_HISTORICO_ALTERACAO_DATA", EntityName = "PedidoHistoricoAlteracaoData", Name = "Dominio.Entidades.Embarcador.Pedidos.PedidoHistoricoAlteracaoData", NameType = typeof(PedidoHistoricoAlteracaoData))]
    public class PedidoHistoricoAlteracaoData : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PHD_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataAlteracao", Column = "PHD_DATA_ALTERACAO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataAlteracao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Pedido", Column = "PED_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pedido Pedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoData", Column = "PDH_TIPO_DATA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoDataPedido), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoDataPedido TipoData { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataAnterior", Column = "PHD_DATA_ANTERIOR", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataAnterior { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Responsavel", Column = "PDH_RESPONSAVEL", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.ResponsavelAlteracaoDataPedido), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.ResponsavelAlteracaoDataPedido Responsavel { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "PHD_OBSERVACAO", Type = "StringClob", NotNull = true)]
        public virtual string Observacao { get; set; }

        public virtual string Descricao { get { return Codigo.ToString(); } }
    }
}
