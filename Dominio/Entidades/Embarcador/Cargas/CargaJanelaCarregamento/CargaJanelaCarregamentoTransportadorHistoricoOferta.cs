using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CARGA_JANELA_CARREGAMENTO_TRANSPORTADOR_HISTORICO_OFERTA", EntityName = "CargaJanelaCarregamentoTransportadorHistoricoOferta", Name = "Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorHistoricoOferta", NameType = typeof(CargaJanelaCarregamentoTransportadorHistoricoOferta))]
    public class CargaJanelaCarregamentoTransportadorHistoricoOferta : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "JHO_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaJanelaCarregamentoTransportadorHistorico", Column = "JTH_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CargaJanelaCarregamentoTransportadorHistorico CargaJanelaCarregamentoTransportadorHistorico { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "JHO_DESCRICAO", TypeType = typeof(string), Length = 300, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ordem", Column = "JHO_ORDEM", TypeType = typeof(int), NotNull = true)]
        public virtual int Ordem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "JHO_PERCENTUAL_CARGAS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal PercentualCargas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "JHO_PERCENTUAL_CONFIGURADO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal PercentualConfigurado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Prioridade", Column = "JHO_PRIORIDADE", TypeType = typeof(int), NotNull = false)]
        public virtual int Prioridade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Tipo", Column = "JHO_TIPO", TypeType = typeof(TipoHistoricoOfertaTransportador), NotNull = true)]
        public virtual TipoHistoricoOfertaTransportador Tipo { get; set; }
    }
}
