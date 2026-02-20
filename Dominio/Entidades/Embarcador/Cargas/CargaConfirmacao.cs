using System;

namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_CONFIRMACAO", EntityName = "CargaConfirmacao", Name = "Dominio.Entidades.Embarcador.Cargas.CargaConfirmacao", NameType = typeof(CargaConfirmacao))]
    public class CargaConfirmacao : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CCF_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataCorfirmacaoCanhotosDigitalizados", Column = "CCF_DATA_CORFIRMACAO_CANHOTOS_DIGITALIZADOS", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataCorfirmacaoCanhotosDigitalizados { get; set; }
    }
}
