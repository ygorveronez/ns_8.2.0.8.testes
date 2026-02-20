using System;

namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_LOCAIS_PRESTACAO", EntityName = "CargaLocaisPrestacao", Name = "Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacao", NameType = typeof(CargaLocaisPrestacao))]
    public class CargaLocaisPrestacao : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacao>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CLP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.Carga Carga { get; set; }
        
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "LOC_INICIO_PRESTACAO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Localidade LocalidadeInicioPrestacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "LOC_TERMINO_PRESTACAO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Localidade LocalidadeTerminoPrestacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "LOC_FRONTEIRA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Localidade LocalidadeFronteira { get; set; }
        public virtual Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacao Clonar()
        {
            return (Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacao)this.MemberwiseClone();
        }
        public virtual bool Equals(CargaLocaisPrestacao other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
