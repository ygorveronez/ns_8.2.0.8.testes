using System;

namespace Dominio.Entidades.Embarcador.CotacaoPedido
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_COTACAO_PEDIDO_CUBAGEM", EntityName = "CotacaoPedidoCubagem", Name = "Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedidoCubagem", NameType = typeof(CotacaoPedidoCubagem))]
    public class CotacaoPedidoCubagem : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedidoCubagem>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CTC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CotacaoPedido", Column = "CTP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedido CotacaoPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Altura", Column = "CPC_ALTURA", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal Altura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Comprimento", Column = "CPC_COMPRIMENTO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal Comprimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Largura", Column = "CPC_LARGURA", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal Largura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QtdVolume", Column = "CPC_QTD_VOLUME", TypeType = typeof(int), NotNull = false)]
        public virtual int QtdVolume { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MetroCubico", Column = "CPC_METRO_CUBICO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal MetroCubico { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "FatorCubico", Column = "CPC_FATOR_CUBICO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal FatorCubico { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PesoCubado", Column = "CPC_PESO_CUBADO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal PesoCubado { get; set; }
        public virtual Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedidoCubagem Clonar()
        {
            return (Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedidoCubagem)this.MemberwiseClone();
        }

        public virtual string Descricao
        {
            get
            {
                return this.Codigo.ToString();
            }
        }

        public virtual bool Equals(CotacaoPedidoCubagem other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
