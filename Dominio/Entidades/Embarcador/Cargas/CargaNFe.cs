using System;

namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_NFE", EntityName = "CargaNFe", Name = "Dominio.Entidades.Embarcador.Cargas.CargaNFe", NameType = typeof(CargaNFe))]
    public class CargaNFe : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Cargas.CargaNFe>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CCT_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO_ORIGEM", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.Carga CargaOrigem { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "NotaFiscal", Column = "NFI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal NotaFiscal { get; set; }

        public virtual Dominio.Entidades.Embarcador.Cargas.CargaNFe Clonar()
        {
            return (Dominio.Entidades.Embarcador.Cargas.CargaNFe)this.MemberwiseClone();
        }
        public virtual string Descricao
        {
            get
            {
                return (this.Carga?.Descricao ?? string.Empty) + " - " + (this.NotaFiscal?.Descricao ?? string.Empty);
            }
        }

        public virtual bool Equals(CargaNFe other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
