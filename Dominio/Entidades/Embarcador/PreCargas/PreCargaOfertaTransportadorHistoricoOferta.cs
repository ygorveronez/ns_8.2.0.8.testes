using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.Entidades.Embarcador.PreCargas
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_PRE_CARGA_OFERTA_TRANSPORTADOR_HISTORICO_OFERTA", EntityName = "PreCargaOfertaTransportadorHistoricoOferta", Name = "Dominio.Entidades.Embarcador.PreCargas.PreCargaOfertaTransportadorHistoricoOferta", NameType = typeof(PreCargaOfertaTransportadorHistoricoOferta))]
    public class PreCargaOfertaTransportadorHistoricoOferta : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PHO_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PreCargaOfertaTransportadorHistorico", Column = "PTH_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual PreCargaOfertaTransportadorHistorico PreCargaOfertaTransportadorHistorico { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "PTH_DESCRICAO", TypeType = typeof(string), Length = 300, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ordem", Column = "PTH_ORDEM", TypeType = typeof(int), NotNull = true)]
        public virtual int Ordem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PTH_PERCENTUAL_CARGAS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal PercentualCargas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PTH_PERCENTUAL_CONFIGURADO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal PercentualConfigurado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Prioridade", Column = "PTH_PRIORIDADE", TypeType = typeof(int), NotNull = false)]
        public virtual int Prioridade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Tipo", Column = "PTH_TIPO", TypeType = typeof(TipoHistoricoOfertaTransportador), NotNull = true)]
        public virtual TipoHistoricoOfertaTransportador Tipo { get; set; }
    }
}
