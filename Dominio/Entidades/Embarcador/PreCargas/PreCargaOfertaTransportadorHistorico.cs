using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Entidades.Embarcador.PreCargas
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_PRE_CARGA_OFERTA_TRANSPORTADOR_HISTORICO", EntityName = "PreCargaOfertaTransportadorHistorico", Name = "Dominio.Entidades.Embarcador.PreCargas.PreCargaOfertaTransportadorHistorico", NameType = typeof(PreCargaOfertaTransportadorHistorico))]
    public class PreCargaOfertaTransportadorHistorico : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PTH_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PreCargaOfertaTransportador", Column = "POT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual PreCargaOfertaTransportador PreCargaOfertaTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Data", Column = "PTH_DATA", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime Data { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "PTH_DESCRICAO", TypeType = typeof(string), Length = 300, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Tipo", Column = "PTH_TIPO", TypeType = typeof(TipoPreCargaOfertaTransportadorHistorico), NotNull = true)]
        public virtual TipoPreCargaOfertaTransportadorHistorico Tipo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Usuario { get; set; }
    }
}
