using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_REDESPACHO", EntityName = "Redespacho", Name = "Dominio.Entidades.Embarcador.Cargas.Redespacho", NameType = typeof(Redespacho))]
    public class Redespacho : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Cargas.Redespacho>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "RED_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroRedespacho", Column = "RED_NUMERO", TypeType = typeof(int), NotNull = true)]
        public virtual int NumeroRedespacho { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RED_TIPO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoRedespacho), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoRedespacho TipoRedespacho { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "CargasUtilizadas", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REDESPACHO_CARGA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RED_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Carga", Column = "CAR_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Cargas.Carga> CargasUtilizadas { get; set; }


        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO_GERADA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.Carga CargaGerada { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoOperacao", Column = "TOP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.TipoOperacao TipoOperacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CODIGO_EXPEDIDOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Expedidor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataRedespacho", Column = "RED_DATA_REDESPACHO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataRedespacho { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Distancia", Column = "RED_DISTANCIA", TypeType = typeof(decimal), NotNull = false, Scale = 4, Precision = 18)]
        public virtual decimal Distancia { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "CargaPedidos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CARGA_PEDIDO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RED_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CargaPedido", Column = "CEP_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Cargas.CargaPedido> CargaPedidos { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CODIGO_RECEBEDOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Recebedor { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.NumeroRedespacho.ToString();
            }
        }

        public virtual bool Equals(Redespacho other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
