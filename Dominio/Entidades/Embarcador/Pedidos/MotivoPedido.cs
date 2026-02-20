using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PEDIDO_MOTIVO", EntityName = "MotivoPedido", Name = "Dominio.Entidades.Embarcador.Pedidos.MotivoPedido", NameType = typeof(MotivoPedido))]
    public class MotivoPedido : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PM_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "PM_DESCRICAO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "PM_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "PM_OBSERVACAO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoMotivo", Column = "PM_TIPO_MOTIVO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.EnumTipoMotivoPedido), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.EnumTipoMotivoPedido TipoMotivo { get; set; }

        public virtual string DescricaoAtivo
        {
            get
            {
                return this.Ativo ? "Ativo" : "Inativo";
            }
        }

        public virtual string DescricaoTipoMotivo
        {
            get 
            {
                return this.TipoMotivo.ObterDescricao();
            }
            
        }

    public virtual bool Equals(MotivoPedido other)
        {
            return other.Codigo == this.Codigo;
        }


    }
}
