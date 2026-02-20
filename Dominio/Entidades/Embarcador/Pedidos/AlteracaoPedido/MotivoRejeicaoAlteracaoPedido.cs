using System;

namespace Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedido
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_MOTIVO_REJEICAO_ALTERACAO_PEDIDO", EntityName = "MotivoRejeicaoAlteracaoPedido", Name = "Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedido.MotivoRejeicaoAlteracaoPedido", NameType = typeof(MotivoRejeicaoAlteracaoPedido))]
    public class MotivoRejeicaoAlteracaoPedido : EntidadeBase, IEquatable<MotivoRejeicaoAlteracaoPedido>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "RAP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "RAP_DESCRICAO", TypeType = typeof(string), Length = 200, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "RAP_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "RAP_OBSERVACAO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Tipo", Column = "RAP_TIPO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoMotivoRejeicaoAlteracaoPedido), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoMotivoRejeicaoAlteracaoPedido Tipo { get; set; }

        public virtual string DescricaoAtivo
        {
            get { return this.Ativo ? "Ativo" : "Inativo"; }
        }

        public virtual bool Equals(MotivoRejeicaoAlteracaoPedido other)
        {
            return (this.Codigo == other.Codigo);
        }
    }
}
