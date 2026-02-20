using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_NAVIO", EntityName = "Navio", Name = "Dominio.Entidades.Embarcador.Pedidos.Navio", NameType = typeof(Navio))]
    public class Navio : EntidadeBase, IEquatable<Navio>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "NAV_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "NAV_DESCRICAO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegracao", Column = "NAV_CODIGO_INTEGRACAO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Status", Column = "NAV_STATUS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Status { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Irin", Column = "NAV_IRIN", TypeType = typeof(string), Length = 10, NotNull = false)]
        public virtual string Irin { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoEmbarcacao", Column = "NAV_CODIGO_EMBARCACAO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string CodigoEmbarcacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoEmbarcacao", Column = "NAV_TIPO_EMBARCACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.InformacaoManuseio), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoEmbarcacao TipoEmbarcacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoDocumento", Column = "NAV_CODIGO_DOCUMENTO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoDocumento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIMO", Column = "NAV_CODIGO_IMO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoIMO { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Integrado", Column = "NAV_INTEGRADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? Integrado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CapacidadePlug", Column = "NAV_CAPACIDADE_PLUG", TypeType = typeof(decimal), NotNull = false, Scale = 4, Precision = 18)]
        public virtual decimal CapacidadePlug { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CapacidadeTeus", Column = "NAV_CAPACIDADE_TEUS", TypeType = typeof(decimal), NotNull = false, Scale = 4, Precision = 18)]
        public virtual decimal CapacidadeTeus { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CapacidadeTons", Column = "NAV_CAPACIDADE_TONS", TypeType = typeof(decimal), NotNull = false, Scale = 4, Precision = 18)]
        public virtual decimal CapacidadeTons { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NavioID", Column = "NAV_NAVIO_ID", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string NavioID { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoOperador", Column = "NAV_CODIGO_OPERADOR", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string CodigoOperador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoNavio", Column = "NAV_CODIGO_NAVIO", TypeType = typeof(string), Length = 3, NotNull = false)]
        public virtual string CodigoNavio { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Operadores", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_NAVIO_OPERADOR")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "NAV_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "NavioOperador", Column = "NOP_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Pedidos.NavioOperador> Operadores { get; set; }
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

        public virtual bool Equals(Navio other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
