using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_NAVIO_OPERADOR", EntityName = "NavioOperador", Name = "Dominio.Entidades.Embarcador.Pedidos.NavioOperador", NameType = typeof(NavioOperador))]
    public class NavioOperador : EntidadeBase, IEquatable<NavioOperador>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "NOP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Navio", Column = "NAV_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Navio Navio { get; set; }
        /// <summary>
        /// operatorDetail.vesselCode
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegracao", Column = "NOP_CODIGO_INTEGRACAO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoIntegracao { get; set; }

        /// <summary>
        /// operatorDetail.operatorCode
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoOperador", Column = "NOP_CODIGO_OPERADOR", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string CodigoOperador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IdOperador", Column = "NOP_ID_OPERADOR", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string IdOperador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Status", Column = "NOP_STATUS", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.StatusNavioOperador), NotNull = false)]
        public virtual StatusNavioOperador Status { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataAtivo", Column = "NOP_DATA_ATIVO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataAtivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataInativo", Column = "NOP_DATA_INATIVO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataInativo { get; set; }


        public virtual bool Equals(NavioOperador other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
