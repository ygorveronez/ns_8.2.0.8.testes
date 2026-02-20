using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Operacional.Canhoto
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_OPERADOR_CANHOTO_TIPO_CARGA", EntityName = "OperadorCanhotoTipoCarga", Name = "Dominio.Entidades.Embarcador.Operacional.Canhoto.OperadorCanhotoTipoCarga", NameType = typeof(OperadorCanhotoTipoCarga))]
    public class OperadorCanhotoTipoCarga : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Operacional.Canhoto.OperadorCanhotoTipoCarga>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CTC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "OperadorCanhoto", Column = "OPC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Operacional.Canhoto.OperadorCanhoto OperadorCanhoto { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoDeCarga", Column = "TCG_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.TipoDeCarga TipoDeCarga { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "ModelosVeiculares", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_OPERADOR_CANHOTO_TIPO_CARGA_MODELO_VEICULAR")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CTC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "OperadorCanhotoTipoCargaModeloVeicular", Column = "CMV_CODIGO")]
        public virtual IList<OperadorCanhotoTipoCargaModeloVeicular> ModelosVeiculares { get; set; }

        public virtual bool Equals(OperadorCanhotoTipoCarga other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
