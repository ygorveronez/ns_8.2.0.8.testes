using System;

namespace Dominio.Entidades.Embarcador.Operacional
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_OPERADOR_TIPO_CARGA_MODELO_VEICULAR", EntityName = "OperadorTipoCargaModeloVeicular", Name = "Dominio.Entidades.Embarcador.Operacional.OperadorTipoCargaModeloVeicular", NameType = typeof(OperadorTipoCargaModeloVeicular))]
    public class OperadorTipoCargaModeloVeicular : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Operacional.OperadorTipoCargaModeloVeicular>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "OMV_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "OperadorTipoCarga", Column = "OTC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Operacional.OperadorTipoCarga OperadorTipoCarga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModeloVeicularCarga", Column = "MVC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga ModeloVeicularCarga { get; set; }

        public virtual bool Equals(OperadorTipoCargaModeloVeicular other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
