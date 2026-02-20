using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONTAINER_TIPO", EntityName = "ContainerTipo", Name = "Dominio.Entidades.Embarcador.Pedidos.ContainerTipo", NameType = typeof(ContainerTipo))]
    public class ContainerTipo : EntidadeBase, IEquatable<ContainerTipo>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CTI_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "CTI_DESCRICAO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegracao", Column = "CTI_CODIGO_INTEGRACAO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Status", Column = "CTI_STATUS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Status { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Valor", Column = "CTI_VALOR", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = false)]
        public virtual decimal Valor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PesoMaximo", Column = "CTI_PESO_MAXIMO", TypeType = typeof(decimal), Scale = 4, Precision = 15, NotNull = false)]
        public virtual decimal PesoMaximo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoDocumento", Column = "CTI_CODIGO_DOCUMENTO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoDocumento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "FFE", Column = "CTI_FFE", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string FFE { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TEU", Column = "CTI_TEU", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string TEU { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "PesoLiquido", Column = "CTI_PESO_LIQUIDO", TypeType = typeof(decimal), Scale = 4, Precision = 15, NotNull = false)]
        public virtual decimal PesoLiquido { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "Tara", Column = "CTI_TARA", TypeType = typeof(decimal), Scale = 4, Precision = 15, NotNull = false)]
        public virtual decimal Tara { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "MetrosCubicos", Column = "CTI_METROS_CUBICOS", TypeType = typeof(decimal), Scale = 4, Precision = 15, NotNull = false)]
        public virtual decimal MetrosCubicos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Integrado", Column = "CTI_INTEGRADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? Integrado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTI_TIPO_PES", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoPes), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoPes TipoPes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoMeioTransporteEDI", Column = "CTI_TIPO_MEIO_TRANSPORTE_EDI", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string TipoMeioTransporteEDI { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Tipos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CONTAINER_TIPO_ASSOCIADO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CTI_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ContainerTipoAssociado", Column = "CTA_CODIGO")]
        public virtual IList<ContainerTipoAssociado> Tipos { get; set; }

        public virtual string DescricaoStatus
        {
            get
            {
                switch (this.Status)
                {
                    case true:
                        return "Ativo";
                    case false:
                        return "Inativo";
                    default:
                        return "";
                }
            }
        }

        public virtual bool Equals(ContainerTipo other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
