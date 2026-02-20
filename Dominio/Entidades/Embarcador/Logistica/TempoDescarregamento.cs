namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CENTRO_DESCARREGAMENTO_TEMPO_DESCARREGAMENTO", EntityName = "TempoDescarregamento", Name = "Dominio.Entidades.Embarcador.Logistica.TempoDescarregamento", NameType = typeof(TempoDescarregamento))]
    public class TempoDescarregamento : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TED_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CentroDescarregamento", Column = "CED_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento CentroDescarregamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoDeCarga", Column = "TCG_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.TipoDeCarga TipoCarga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModeloVeicularCarga", Column = "MVC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga ModeloVeicular { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TED_SKU_DE", TypeType = typeof(int), NotNull = false)]
        public virtual int? SkuDe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TED_SKU_ATE", TypeType = typeof(int), NotNull = false)]
        public virtual int? SkuAte { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Tempo", Column = "TED_TEMPO", TypeType = typeof(int), NotNull = true)]
        public virtual int Tempo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TED_TIPO_TEMPO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TempoDescarregamentoTipoTempo), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TempoDescarregamentoTipoTempo TipoTempo { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.TipoCarga.Descricao + " - " + ModeloVeicular.Descricao;
            }
        }
    }
}
