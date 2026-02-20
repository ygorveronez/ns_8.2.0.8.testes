using System;

namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PEDIDO_CANCELAMENTO", EntityName = "PedidoCancelamento", Name = "Dominio.Entidades.Embarcador.Pedidos.PedidoCancelamento", NameType = typeof(PedidoCancelamento))]
    public class PedidoCancelamento : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PEC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Pedido", Column = "PED_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.Pedido Pedido { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PEC_DATA_CANCELAMENTO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataCancelamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PEC_MOTIVO_CANCELAMENTO", TypeType = typeof(string), Length = 500, NotNull = true)]
        public virtual string MotivoCancelamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PEC_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedidoCancelamento), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedidoCancelamento Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PEC_TIPO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoPedidoCancelamento), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoPedidoCancelamento Tipo { get; set; }

        public virtual string Descricao
        {
            get
            {
                return Pedido.Numero.ToString();
            }
        }

        public virtual string DescricaoSituacao
        {
            get
            {
                switch (Situacao)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedidoCancelamento.Cancelado:
                        return "Cancelado";
                    default:
                        return string.Empty;
                }
            }
        }

        public virtual string DescricaoTipo
        {
            get
            {
                switch (Tipo)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoPedidoCancelamento.Cancelamento:
                        return "Cancelamento";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoPedidoCancelamento.DesistenciaCarga:
                        return "Desistência da Carga";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoPedidoCancelamento.DesistenciaCarregamento:
                        return "Desistência no Carregamento";
                    default:
                        return string.Empty;
                }
            }
        }
    }
}
