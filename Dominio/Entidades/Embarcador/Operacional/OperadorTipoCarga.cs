using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Operacional
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_OPERADOR_TIPO_CARGA", EntityName = "OperadorTipoCarga", Name = "Dominio.Entidades.Embarcador.Operacional.OperadorTipoCarga", NameType = typeof(OperadorTipoCarga))]
    public class OperadorTipoCarga : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Operacional.OperadorTipoCarga>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "OTC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "OperadorLogistica", Column = "OPL_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Operacional.OperadorLogistica OperadorLogistica { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoDeCarga", Column = "TCG_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.TipoDeCarga TipoDeCarga { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "ModelosVeiculares", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_OPERADOR_TIPO_CARGA_MODELO_VEICULAR")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "OTC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "OperadorTipoCargaModeloVeicular", Column = "OMV_CODIGO")]
        public virtual IList<OperadorTipoCargaModeloVeicular> ModelosVeiculares { get; set; }

        public virtual bool Equals(OperadorTipoCarga other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
