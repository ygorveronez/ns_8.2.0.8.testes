using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Entidades.Embarcador.PreCargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PRE_CARGA_OFERTA", EntityName = "PreCargaOferta", Name = "Dominio.Entidades.Embarcador.PreCargas.PreCargaOferta", NameType = typeof(PreCargaOferta))]
    public class PreCargaOferta : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PCO_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PreCarga", Column = "PCA_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual PreCarga PreCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataCriacao", Column = "PCO_DATA_CRIACAO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataCriacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataLiberacao", Column = "PCO_DATA_LIBERACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataLiberacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Situacao", Column = "PCO_SITUACAO", TypeType = typeof(SituacaoPreCargaOferta), NotNull = true)]
        public virtual SituacaoPreCargaOferta Situacao { get; set; }

        public PreCargaOferta()
        {
            DataCriacao = DateTime.Now;
        }
    }
}
