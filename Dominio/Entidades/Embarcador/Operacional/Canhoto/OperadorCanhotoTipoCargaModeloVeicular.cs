using System;

namespace Dominio.Entidades.Embarcador.Operacional.Canhoto
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_OPERADOR_CANHOTO_TIPO_CARGA_MODELO_VEICULAR", EntityName = "OperadorCanhotoTipoCargaModeloVeicular", Name = "Dominio.Entidades.Embarcador.Operacional.Canhoto.OperadorCanhotoTipoCargaModeloVeicular", NameType = typeof(OperadorCanhotoTipoCargaModeloVeicular))]
    public class OperadorCanhotoTipoCargaModeloVeicular : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Operacional.Canhoto.OperadorCanhotoTipoCargaModeloVeicular>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CMV_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "OperadorCanhotoTipoCarga", Column = "CTC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Operacional.Canhoto.OperadorCanhotoTipoCarga OperadorCanhotoTipoCarga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModeloVeicularCarga", Column = "MVC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga ModeloVeicularCarga { get; set; }

        public virtual bool Equals(OperadorCanhotoTipoCargaModeloVeicular other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
