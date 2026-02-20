using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_TRAJETO_CARGA", EntityName = "CargaTrajetoCarga", DynamicUpdate = true, Name = "Dominio.Entidades.Embarcador.Cargas.CargaTrajetoCarga", NameType = typeof(CargaTrajetoCarga))]
    public class CargaTrajetoCarga : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CTC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaTrajeto", Column = "CTM_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CargaTrajeto CargaTrajeto { get; set; }
        
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ordem", Column = "CTC_ORDEM", TypeType = typeof(int), NotNull = false)]
        public virtual int Ordem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Data", Column = "CTM_DATA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime Data { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoTrajetoCarga", Column = "CTC_SITUACAO_TRAJETO_CARGA", TypeType = typeof(SituacaoTrajetoCarga), NotNull = false)]
        public virtual SituacaoTrajetoCarga SituacaoTrajetoCarga { get; set; }
    }
}
