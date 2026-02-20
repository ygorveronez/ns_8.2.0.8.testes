using System;

namespace Dominio.Entidades.Embarcador.Ocorrencias
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_OCORRENCIA_INTEGRACAO", EntityName = "OcorrenciaIntegracao", Name = "Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaIntegracao", NameType = typeof(OcorrenciaIntegracao))]
    public class OcorrenciaIntegracao : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaIntegracao>
    {
        public OcorrenciaIntegracao()
        {
        }

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "OIN_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaOcorrencia", Column = "COC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia CargaOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoIntegracao", Column = "TPI_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.TipoIntegracao TipoIntegracao { get; set; }

        public virtual Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaIntegracao Clonar()
        {
            return (Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaIntegracao)this.MemberwiseClone();
        }
        public virtual bool Equals(OcorrenciaIntegracao other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
