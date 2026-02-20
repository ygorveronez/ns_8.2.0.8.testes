using System;

namespace Dominio.Entidades.Embarcador.Acerto
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ACERTO_OCORRENCIA", EntityName = "AcertoOcorrencia", Name = "Dominio.Entidades.Embarcador.Acerto.AcertoOcorrencia", NameType = typeof(AcertoOcorrencia))]
    public class AcertoOcorrencia : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Acerto.AcertoOcorrencia>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "AOC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "AcertoViagem", Column = "ACV_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Acerto.AcertoViagem AcertoViagem { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaOcorrencia", Column = "COC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia CargaOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "LancadoManualmente", Column = "AOC_LANCADO_MANUALMENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool LancadoManualmente { get; set; }
        public virtual string Descricao
        {
            get
            {
                return (this.CargaOcorrencia?.NumeroOcorrencia ?? 0).ToString();
            }
        }

        public virtual bool Equals(AcertoOcorrencia other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
